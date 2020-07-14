using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;

using Driven.Data.Entity.Royalty;
using Driven.Business.Royalty.Interfaces;
using Driven.Business.Royalty;
using Driven.Business.NotificationTemplates;

using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    public class SalesCorrectionController : BaseController
    {
        private const string mc_Configuration_EmailFrom = "EmailFrom";
        private const string mc_Configuration_EmailDebug = "EmailDebug";
        private const string mc_Configuration_EmailEnabled = "EmailEnabled";
        private const string mc_Configuration_EmailForceTo = "EmailForceTo";
        private const string mc_Configuration_EmailSmtpHost = "EmailSmtpHost";
        private const string mc_Configuration_EmailSmtpUser = "EmailSmtpUser";
        private const string mc_Configuration_EmailSmtpPass = "EmailSmtpPass";

        private const int mc_ActionResult_MaxRows = 10000;
        private const int mc_ActionResult_MaxJsonLength = 86753090;

        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";


        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private enum ToplineStatus { NotStarted = 1, InProgress = 2, ForApproval = 3, Submitted = 4 };
        private enum ToplineCorrectionStatus { CorrectionPending = 5, CorrectionApproved = 6, CorrectionDenied = 7, CorrectionClosed = 8 };

        private ISalesCorrectionService _salesCorrectionService;
        private IBrandPeriodSalesService _brandPeriodSalesService;

        public SalesCorrectionController() : this(new SalesCorrectionService(), new BrandPeriodSalesService()) { }

        public SalesCorrectionController(ISalesCorrectionService salesCorrectionService, IBrandPeriodSalesService brandPeriodSalesService)
        {
            _salesCorrectionService = salesCorrectionService;
            _brandPeriodSalesService = brandPeriodSalesService;
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        private List<string> ValidateEmails(List<string> emails)
        {
            var results = new List<string>();
            
            foreach (var email in emails)
            {
                if (IsValidEmail(email))
                {
                    results.Add(email);
                }
            }

            return results;
        }


        private string EmailTemplateBody(string msgBody)
        {
            var result = msgBody;

            try
            {
                //var templateService = new TemplateService();
                //result = templateService.BuildBpsrCorrectionNotification(msgBody, "EmailTemplate");
            }
            catch (Exception ex)
            {
                _log.WarnFormat(FormatException(ex));
            }

            return result;
        }


        private bool SendEmail(string msgTo, string msgSubject, string msgBody)
        {
            return SendEmail(new List<string>() { msgTo }, msgSubject, msgBody);
        }


        private bool SendEmail(List<string> msgTo, string msgSubject, string msgBody)
        {
            var success = false;

            var emailFrom = ConfigurationManager.AppSettings[mc_Configuration_EmailFrom] ?? string.Empty;
            var emailDebug = !((ConfigurationManager.AppSettings[mc_Configuration_EmailDebug] ?? "1") == "0");
            var emailEnabled = ((ConfigurationManager.AppSettings[mc_Configuration_EmailEnabled] ?? "0") == "1");
            var emailForceTo = ConfigurationManager.AppSettings[mc_Configuration_EmailForceTo] ?? string.Empty;
            var emailHost = ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpHost] ?? string.Empty;
            var emailUser = ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpUser] ?? string.Empty;
            var emailPass = ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpPass] ?? string.Empty;
            
            var emailTos = ValidateEmails(msgTo);
            var emailBody = EmailTemplateBody(msgBody);

            _log.InfoFormat("Debug: {0}, DebugTo: {1}, To: {2}, From: {3}, Subject: {4}, Body: {5}", 
                emailDebug, emailForceTo, String.Join(",", emailTos), emailFrom, msgSubject, emailBody);

            try
            {
                var msg = new System.Net.Mail.MailMessage();
                msg.From = new System.Net.Mail.MailAddress(emailFrom);
                msg.Body = emailBody;
                msg.Subject = msgSubject;
                msg.IsBodyHtml = true;

                if (emailDebug)
                {
                    // override emailTo with comma seperated force to
                    var emailForceTos = emailForceTo.Split(',').Select(x => x.Trim());
                    foreach (var currentForceTo in emailForceTos)
                    {
                        msg.To.Add(currentForceTo);
                    }

                    // debug header added to body for testing
                    msg.Body = String.Format("To: {0}<br>From: {1}<br><br>", String.Join(",", emailTos), emailFrom) + msg.Body;
                }
                else
                {
                    foreach (var t in emailTos)
                    {
                        msg.To.Add(t);
                    }
                }

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(emailHost);

                if (emailEnabled) 
                {
                    client.Credentials = new System.Net.NetworkCredential(emailUser, emailPass);
                    client.Send(msg);
                }

                success = true;
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
            }

            return success;
        }


        private Task<bool> EmailSalesCorrectionSubmitConfirmAsync(string submitterName, string submitterEmailAddress, BPSR_Topline topline, BPSR_Topline_Correction correction)
        {
            _log.Info(String.Format("EmailSalesCorrectionSubmitConfirmAsync: submitterName={0}, submitterEmailAddress={1}", submitterName, submitterEmailAddress));

            var tmlBody = "Your request for Sales Corrections for [[Brand]] - [[LocalStoreID]] - [[ReportingPeriod]] have been submitted. Your request is [[RequestID]].";
            var tmlSubject = "Request for Sales Correction Submitted for [[Brand]] - [[LocalStoreID]] - [[ReportingPeriod]]";

            var brand = topline.SubBrand.SubBrandName;
            var placeholders = new List<PlaceholderValue_Result>() 
            { 
                new PlaceholderValue_Result() { Name = "Brand", Value = brand },
                new PlaceholderValue_Result() { Name = "LocalStoreID", Value = topline.LocalStoreID },
                new PlaceholderValue_Result() { Name = "ReportingPeriod", Value = FormatHelpers.FormatPeriodDate(topline.PeriodEndDate) },
                new PlaceholderValue_Result() { Name = "RequestID", Value = correction.BPSR_Topline_CorrectionID.ToString() }
            };

            var msgBody = PlaceholderHelpers.ApplyPlaceholders(tmlBody, placeholders);
            var msgSubject = PlaceholderHelpers.ApplyPlaceholders(tmlSubject, placeholders);
            var msgTo = submitterEmailAddress;

            return Task.Run(() =>
            {
                return SendEmail(msgTo, msgSubject, msgBody);
            });
        }


        private string BuildCorrectionHyperlink(int subBrandId, string localStoreId, DateTime reportingPeriod)
        {
            var url = string.Empty;

            try
            {
                var routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues.Add("id", subBrandId);
                routeValues.Add("sid", localStoreId);
                routeValues.Add("pid", reportingPeriod.ToString("yyyy-MM-dd"));

                var scheme = HttpContext.Request.RequestContext.HttpContext.Request.Url.Scheme;
                url = new UrlHelper(HttpContext.Request.RequestContext).Action("ApproveCorrections", "SalesCorrection", routeValues, scheme);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
            }
            
            return url;
        }

        
        private List<string> GetApproverEmailAddresses(int subBrandId)
        {
            var results = new List<string>();
            var approverGroups = SecurityHelpers.GetSubBrandApproverGroups(subBrandId);

            foreach (var groupName in approverGroups)
            {
                var emails = SecurityHelpers.GetGroupEmailAddresses(groupName);
                results = results.Union(emails).ToList();
            }

            return results.Distinct().ToList();
        }


        private Task<bool> EmailSalesCorrectionSubmitApproveAsync(string submitterName, string submitterEmailAddress, BPSR_Topline topline, BPSR_Topline_Correction correction)
        {
            var tmlBody = "Please review this request by clicking the link below [[CorrectionHyperlink]]";
            var tmlSubject = "Request for Sales Correction for [[Brand]] - [[LocalStoreID]] - [[ReportingPeriod]]";

            var brand = topline.SubBrand.SubBrandName;
            var placeholders = new List<PlaceholderValue_Result>()
            { 
                new PlaceholderValue_Result() { Name = "Brand", Value = brand },
                new PlaceholderValue_Result() { Name = "LocalStoreID", Value = topline.LocalStoreID },
                new PlaceholderValue_Result() { Name = "ReportingPeriod", Value = FormatHelpers.FormatPeriodDate(topline.PeriodEndDate) },
                new PlaceholderValue_Result() { Name = "CorrectionHyperlink", Value = BuildCorrectionHyperlink(topline.SubBrandID, topline.LocalStoreID, topline.PeriodEndDate) }
            };

            var msgBody = PlaceholderHelpers.ApplyPlaceholders(tmlBody, placeholders);
            var msgSubject = PlaceholderHelpers.ApplyPlaceholders(tmlSubject, placeholders);

            return Task.Run(() =>
            {
                var msgTo = GetApproverEmailAddresses(topline.SubBrandID);
                _log.Info(String.Format("EmailSalesCorrectionSubmitApproveAsync: to approvers={0}, subbrandid={1}", msgTo, topline.SubBrandID));
                return SendEmail(msgTo, msgSubject, msgBody);
            });
        }


        private Task<bool> EmailSalesCorrectionApproveAcceptAsync(string approverName, string approverEmailAddress, BPSR_Topline topline, BPSR_Topline_Correction correction)
        {
            _log.Info(String.Format("EmailSalesCorrectionApproveAcceptAsync: approverName={0}, approverEmailAddress={1}", approverName, approverEmailAddress));

            var tmlSubject = "ACTION REQ: Sales Correction Approved";

            var tmlBody = "A request for Sales Corrections for [[Brand]] - [[LocalStoreID]] - [[ReportingPeriod]] was approved.<br><br>";
            tmlBody += "Log into SAGE and reverse the invoice and payment information associated with the [[OriginalInvoice#]].<br><br>";
            tmlBody += "Once reversed, manually create a new invoice with the following number, [[NewInvoice#]]. Once created apply payment received for [[OriginalInvoice#]] to [[NewInvoice#]].<br><br>";

            tmlBody += "New Royalty Invoice Information<br>";
            tmlBody += "<b>Brand:</b> [[Brand]]<br>";
            tmlBody += "<b>Store:</b> [[LocalStoreID]]<br>";
            tmlBody += "<b>Reporting Period:</b> [[ReportingPeriod]]<br>";
            tmlBody += "<b>Invoice Type:</b> [[InvoiceType]]<br>";
            tmlBody += "<b>New Invoice #:</b> [[NewInvoice#]]<br>";
            tmlBody += "<b>Corrected Sales:</b> [[NewSalesAmount]]<br>";
            tmlBody += "<b>Sage Item Code 1:</b> [[SageItemCode1]]<br>";
            tmlBody += "<b>Sage Item Amount 1:</b> [[SageItemAmt1]]<br>";
            tmlBody += "<b>Sage Item Code 2:</b> [[SageItemCode2]]<br>";
            tmlBody += "<b>Sage Item Amount 2:</b> [[SageItemAmt2]]<br>";
            tmlBody += "<b>Sage Item Code 3:</b> [[SageItemCode3]]<br>";
            tmlBody += "<b>Sage Item Amount 3:</b> [[SageItemAmt3]]<br>";
            tmlBody += "<b>Sage Item Code 4:</b> [[SageItemCode4]]<br>";
            tmlBody += "<b>Sage Item Amount 4:</b> [[SageItemAmt4]]<br><br>";
            
            tmlBody += "New Advertising Invoice Information<br>";
            tmlBody += "<b>Brand:</b> [[Brand]]<br>";
            tmlBody += "<b>Store:</b> [[LocalStoreID]]<br>";
            tmlBody += "<b>Reporting Period:</b> [[ReportingPeriod]]<br>";
            tmlBody += "<b>Invoice Type:</b> [[InvoiceType]]<br>";
            tmlBody += "<b>New Invoice #:</b> [[NewInvoice#]]<br>";
            tmlBody += "<b>Corrected Sales:</b> [[NewSalesAmount]]<br>";
            tmlBody += "<b>Sage Item Code 1:</b> [[SageItemCode1]]<br>";
            tmlBody += "<b>Sage Item Amount 1:</b> [[SageItemAmt1]]<br>";
            tmlBody += "<b>Sage Item Code 2:</b> [[SageItemCode2]]<br>";
            tmlBody += "<b>Sage Item Amount 2:</b> [[SageItemAmt2]]<br>";
            tmlBody += "<b>Sage Item Code 3:</b> [[SageItemCode3]]<br>";
            tmlBody += "<b>Sage Item Amount 3:</b> [[SageItemAmt3]]<br>";
            tmlBody += "<b>Sage Item Code 4:</b> [[SageItemCode4]]<br>";
            tmlBody += "<b>Sage Item Amount 4:</b> [[SageItemAmt4]]<br>";

            var sageService = new SAGEItemMappingService();
            var sageMapping = sageService.SearchSAGEItemMappingBysubBrandId(topline.SubBrandID, null).FirstOrDefault();
            var brand = topline.SubBrand.SubBrandName;

            var placeholders = new List<PlaceholderValue_Result>() 
            { 
                new PlaceholderValue_Result() { Name = "Brand", Value = brand },
                new PlaceholderValue_Result() { Name = "LocalStoreID", Value = topline.LocalStoreID },
                new PlaceholderValue_Result() { Name = "ReportingPeriod", Value = FormatHelpers.FormatPeriodDate(topline.PeriodEndDate) },

                new PlaceholderValue_Result() { Name = "OriginalInvoice#", Value = topline.BPSR_ToplineID.ToString() },
                new PlaceholderValue_Result() { Name = "NewInvoice#", Value = correction.BPSR_Topline_CorrectionID.ToString() },

                new PlaceholderValue_Result() { Name = "OriginalSalesAmount", Value = FormatHelpers.FormatMoney(topline.NetSales) },
                new PlaceholderValue_Result() { Name = "NewSalesAmount", Value = FormatHelpers.FormatMoney(correction.NetSales) },

                new PlaceholderValue_Result() { Name = "InvoiceType", Value = FormatHelpers.FormatSageRecordType(sageMapping.SAGERecordTypeID) },

                //new PlaceholderValue_Result() { Name = "SageItemCode1", Value = sageMapping.ItemID_1 },
                //new PlaceholderValue_Result() { Name = "SageItemAmt1", Value = FormatHelpers.FormatMoney(sageMapping.SAGEValueMappingID_1) },
                //new PlaceholderValue_Result() { Name = "SageItemCode2", Value = sageMapping.ItemID_2 },
                //new PlaceholderValue_Result() { Name = "SageItemAmt2", Value = FormatHelpers.FormatMoney(sageMapping.SAGEValueMappingID_2) },
                //new PlaceholderValue_Result() { Name = "SageItemCode3", Value = sageMapping.ItemID_3 },
                //new PlaceholderValue_Result() { Name = "SageItemAmt3", Value = FormatHelpers.FormatMoney(sageMapping.SAGEValueMappingID_3) },
                //new PlaceholderValue_Result() { Name = "SageItemCode4", Value = sageMapping.ItemID_4 },
                //new PlaceholderValue_Result() { Name = "SageItemAmt4", Value = FormatHelpers.FormatMoney(sageMapping.SAGEValueMappingID_4) }
            };

            var msgBody = PlaceholderHelpers.ApplyPlaceholders(tmlBody, placeholders);
            var msgSubject = PlaceholderHelpers.ApplyPlaceholders(tmlSubject, placeholders);
            var msgTo = approverEmailAddress;

            return Task.Run(() =>
            {
                return SendEmail(msgTo, msgSubject, msgBody);
            });
        }


        private Task<bool> EmailSalesCorrectionApproveDenyAsync(string submitterName, string submitterEmailAddress, string denialType, string denialReason, BPSR_Topline topline, BPSR_Topline_Correction correction)
        {
            _log.Info(String.Format("EmailSalesCorrectionApproveDenyAsync: submitterName={0}, submitterEmailAddress={1}", submitterName, submitterEmailAddress));

            var tmlSubject = "Request for Sales Correction for [[Brand]] - [[LocalStoreID]] -[[ReportingPeriod]] was Denied";
            
            var tmlBody = "Your request for Sales Corrections for [[Brand]] - [[LocalStoreID]] - [[ReportingPeriod]] was Denied.<br><br>";
            tmlBody += "Please make appropriate updates and resubmit if needed.<br><br>";
            tmlBody += "<b>Brand:</b> [[Brand]]<br>";
            tmlBody += "<b>Store:</b> [[LocalStoreID]]<br>";
            tmlBody += "<b>Reporting Period:</b> [[ReportingPeriod]]<br>";
            tmlBody += "<b>Original Invoice #:</b> [[OriginalInvoice#]]<br>";
            tmlBody += "<b>New Invoice #:</b> [[NewInvoice#]]<br>";
            tmlBody += "<b>Original Sales submitted:</b> [[OriginalSalesAmount]]<br>";
            tmlBody += "<b>Corrected Sales:</b> [[NewSalesAmount]]<br>";
            tmlBody += "<b>Correction Type:</b> [[CorrectionType]]<br>";
            tmlBody += "<b>Correction Description:</b> [[CorrectionReason]]<br>";
            tmlBody += "<b>Name of Submitter:</b> [[SubmitterName]]<br>";
            tmlBody += "<b>Name of Approver:</b> [[ApproverName]]<br>";
            tmlBody += "<b>Reason for Denial:</b> [[DenialType]]<br>";
            tmlBody += "<b>Denial Description:</b> [[DenialDescription]]<br>";

            var correctionType = correction.BPSR_CorrectionType.CorrectionTypeDescription;

            var brand = topline.SubBrand.SubBrandName;
            //var denialType = (correction != null && correction.BPSR_DenialType != null && correction.BPSR_DenialType.DenialTypeDescription != null) ? correction.BPSR_DenialType.DenialTypeDescription : string.Empty;

            var placeholders = new List<PlaceholderValue_Result>() 
            { 
                new PlaceholderValue_Result() { Name = "Brand", Value = brand },
                new PlaceholderValue_Result() { Name = "LocalStoreID", Value = topline.LocalStoreID },
                new PlaceholderValue_Result() { Name = "ReportingPeriod", Value = FormatHelpers.FormatPeriodDate(topline.PeriodEndDate) },

                new PlaceholderValue_Result() { Name = "OriginalInvoice#", Value = topline.BPSR_ToplineID.ToString() },
                new PlaceholderValue_Result() { Name = "NewInvoice#", Value = correction.BPSR_Topline_CorrectionID.ToString() },
                new PlaceholderValue_Result() { Name = "OriginalSalesAmount", Value = FormatHelpers.FormatMoney(topline.NetSales) },
                new PlaceholderValue_Result() { Name = "NewSalesAmount", Value = FormatHelpers.FormatMoney(correction.NetSales) },

                new PlaceholderValue_Result() { Name = "CorrectionType", Value = correctionType },
                new PlaceholderValue_Result() { Name = "CorrectionReason", Value = correction.CorrectionReason },
                new PlaceholderValue_Result() { Name = "SubmitterName", Value = correction.SubmitterName },
                new PlaceholderValue_Result() { Name = "ApproverName", Value = submitterName },
                new PlaceholderValue_Result() { Name = "DenialType", Value = denialType.ToString() },
                new PlaceholderValue_Result() { Name = "DenialDescription", Value = denialReason }
            };

            var msgBody = PlaceholderHelpers.ApplyPlaceholders(tmlBody, placeholders);
            var msgSubject = PlaceholderHelpers.ApplyPlaceholders(tmlSubject, placeholders);
            var msgTo = submitterEmailAddress;

            return Task.Run(() =>
            {
                return SendEmail(msgTo, msgSubject, msgBody);
            });
        }


        public ActionResult SubmitCorrections()
        {
            var subBrandId = GetCurrentSubBrandId();
            var userViewModel = GetCurrentUserViewModel();

            var myStatuses = _brandPeriodSalesService.GetStatuses().ToList();
            var myPeriods = _brandPeriodSalesService.GetPeriodEndDates(subBrandId).ToList();
            var myStores = _brandPeriodSalesService.GetStoreIds(subBrandId).OrderBy(x => x, new SemiNumericComparer());

            var model = new SalesCorrectionListViewModel();

            model.StoreIds = (from c in myStores select new SelectListItem() { Text = c, Value = c }).ToList();
            model.StoreId = string.Empty;

            var allowedStatus = Enum.GetValues(typeof(ToplineCorrectionStatus)).Cast<int>();
            var filteredStatus = myStatuses.Where(o => allowedStatus.Contains(o.BPSR_StatusID));

            model.StatusIds = (from c in filteredStatus select new SelectListItem() { Text = c.BPSR_StatusDescription, Value = c.BPSR_StatusID.ToString() }).ToList();
            model.StatusId = string.Empty;

            model.PeriodEndDates = (from c in myPeriods select new SelectListItem() { Text = FormatHelpers.FormatPeriodDate(c), Value = FormatHelpers.FormatPeriodDate(c) }).ToList();
            model.PeriodEndDate = (model.PeriodEndDates != null && model.PeriodEndDates.Count > 0) ? model.PeriodEndDates.FirstOrDefault().Value : string.Empty;

            model.IsAdmin = userViewModel.IsAdmin;
            model.IsApprover = userViewModel.IsApprover;

            return View(model);
        }


        // ========================================================================================
        // optional parameters from email links; example = http://localhost:57589/SalesCorrection/ApproveCorrections/29?sid=3&pid=2017-01-28

        public ActionResult ApproveCorrections(string id, string sid, string pid)
        {
            var userViewModel = GetCurrentUserViewModel();
            var subBrandId = GetCurrentSubBrandId();

            // apply optional id (subbrand) but the user must be a member and an approver
            if (id != null && id.Length > 0)
            {
                subBrandId = BuildInputParamSubBrandId(subBrandId, id);
                if (userViewModel.AvailableSubBrands.Any(x => x.Key == subBrandId) && CanApproveToplineCorrection(userViewModel.IsAdmin, userViewModel.IsApprover))
                {
                    SecurityHelpers.SetUserSubBrandId(subBrandId);
                }
                else
                {
                    throw new System.Security.SecurityException();
                }
            }

            var myStatuses = _brandPeriodSalesService.GetStatuses().ToList();
            var myPeriods = _brandPeriodSalesService.GetPeriodEndDates(subBrandId).ToList();
            var myStores = _brandPeriodSalesService.GetStoreIds(subBrandId).OrderBy(x => x, new SemiNumericComparer());

            var model = new SalesCorrectionListViewModel();

            model.StoreIds = (from c in myStores select new SelectListItem() { Text = c, Value = c }).ToList();
            model.StoreId = string.Empty;

            var allowedStatus = Enum.GetValues(typeof(ToplineCorrectionStatus)).Cast<int>();
            var filteredStatus = myStatuses.Where(o => allowedStatus.Contains(o.BPSR_StatusID));

            model.StatusIds = (from c in filteredStatus select new SelectListItem() { Text = c.BPSR_StatusDescription, Value = c.BPSR_StatusID.ToString() }).ToList();
            model.StatusId = string.Empty;

            model.PeriodEndDates = (from c in myPeriods select new SelectListItem() { Text = FormatHelpers.FormatPeriodDate(c), Value = FormatHelpers.FormatPeriodDate(c) }).ToList();
            model.PeriodEndDate = (model.PeriodEndDates != null && model.PeriodEndDates.Count > 0) ? model.PeriodEndDates.FirstOrDefault().Value : string.Empty;

            // apply optional input parameters -> id = subbrandid; sid = storeid; pid = reportingperiod
            model.StoreId = BuildInputParamStoreId(model.StoreId, sid);
            model.PeriodEndDate = BuildInputParamPeriodEndDate(model.PeriodEndDate, pid);

            model.IsAdmin = userViewModel.IsAdmin;
            model.IsApprover = userViewModel.IsApprover;

            return View(model);
        }


        private int BuildInputParamSubBrandId(int currentSubBrandId, string newSubBrandId)
        {
            try
            {
                return (newSubBrandId != null && newSubBrandId.Length > 0) ? Convert.ToInt32(newSubBrandId) : currentSubBrandId;
            }
            catch
            {
                return currentSubBrandId;
            }
        }


        private string BuildInputParamStoreId(string currentStoreId, string newStoreId)
        {
            try
            {
                return (newStoreId != null && newStoreId.Length > 0) ? newStoreId : currentStoreId;
            }
            catch
            {
                return currentStoreId;
            }
        }


        private string BuildInputParamPeriodEndDate(string currentDate, string newDate)
        {
            try
            {
                return (newDate != null && newDate.Length > 0) ? FormatHelpers.FormatPeriodDate(DateTime.Parse(newDate)) : currentDate;
            }
            catch
            {
                return currentDate;
            }
        }


        [HttpGet]
        public ActionResult GetToplineCorrections(string storeId, DateTime? periodEndDate, int? statusId)
        {
            var uvm = this.GetCurrentUserViewModel();
            var subBrandId = this.GetCurrentSubBrandId();
            var corrections = _salesCorrectionService.GetToplineCorrections(subBrandId, storeId, periodEndDate, statusId).Take(mc_ActionResult_MaxRows).ToList();
            
            var model = this.BindToplineCorrectionsViewModel(corrections, uvm.IsAdmin, uvm.IsApprover);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetProductGroups(string localStoreId, string periodEndDate)
        {
            var uvm = GetCurrentUserViewModel();
            var subBrandId = (int)uvm.SubBrandId;
            var endDate = Convert.ToDateTime(periodEndDate);
            var topline = _brandPeriodSalesService.GetToplines(subBrandId, localStoreId, endDate, null, null, null).FirstOrDefault();

            var allowOperation = this.CanCreateToplineCorrection(uvm.IsAdmin, uvm.IsApprover);
            if (topline == null || topline.SubBrandID != subBrandId || !allowOperation)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
            }

            var model = this.BindCreateToplineCorrectionViewModel(topline, uvm.IsAdmin, uvm.IsApprover);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ApproveToplineCorrection(int requestId, int toplineId)
        {
            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var correction = _salesCorrectionService.GetToplineCorrectionById(requestId);
                var topline = _brandPeriodSalesService.GetToplineById(toplineId);

                var allowOperation = this.CanApproveToplineCorrection(correction.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover);
                if (correction == null || topline == null || topline.SubBrandID != subBrandId || correction.SubBrandID != subBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var model = this.BindApproveToplineCorrectionViewModel(topline, correction, uvm.DisplayName, uvm.IsAdmin, uvm.IsApprover);
                return PartialView("_ApproveToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        private string GetDenialTypeName(string denialType)
        { 
            try
            {
                var k = Convert.ToInt32(denialType);
                return _salesCorrectionService.GetDenialTypes().Where(x => x.BPSR_DenialTypeID == k).FirstOrDefault().DenialTypeDescription;
            }
            catch
            {
                return string.Empty;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveToplineCorrection(ApproveToplineCorrectionViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var topline = _brandPeriodSalesService.GetToplineById(model.ToplineId);
                var correction = _salesCorrectionService.GetToplineCorrectionById(model.RequestId);

                var allowOperation = this.CanApproveToplineCorrection(correction.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover);
                if (topline == null || correction == null || topline.SubBrandID != subBrandId || correction.SubBrandID != subBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                if (model.ApproveStatus != (int)ToplineCorrectionStatus.CorrectionDenied)
                {
                    ModelState["Denial"].Errors.Clear();
                    ModelState["DenialDescription"].Errors.Clear();
                }

                if (!SecurityHelpers.IsValidSubmitterApprover(correction.SubmitterName, uvm.DisplayName))
                {
                    ModelState.AddModelError("", "Submitter cannot be Approver.");
                }

                var approverName = uvm.DisplayName;
                var approverEmailAddress = uvm.EmailAddress;

                if (ModelState.IsValid)
                {
                    ProcessApproveToplineCorrection(approverName, approverEmailAddress, model);

                    if (model.ApproveStatus == (int)ToplineCorrectionStatus.CorrectionDenied)
                    {
                        try
                        {
                            var denialType = GetDenialTypeName(model.Denial);
                            var denialReason = model.DenialDescription;
                            //EmailSalesCorrectionApproveDenyAsync(approverName, approverEmailAddress, denialType, denialReason, topline, correction);
                            EmailSalesCorrectionApproveDenyAsync(correction.SubmitterName, correction.SubmitterEmailAddr, denialType, denialReason, topline, correction);
                        }
                        catch (Exception ee)
                        {
                            _log.Error(FormatException(ee));
                        }
                    }
                    else if (model.ApproveStatus == (int)ToplineCorrectionStatus.CorrectionApproved)
                    {
                        try
                        {
                            // engine will send this email
                            // EmailSalesCorrectionApproveAcceptAsync(approverName, approverEmailAddress, topline, correction);
                        }
                        catch (Exception ee)
                        {
                            _log.Error(FormatException(ee));
                        }
                    }

                    success = true;
                    message = "Successfully updated Sales Correction.";
                }
                else
                {
                    model = this.BindApproveToplineCorrectionViewModel(topline, correction, uvm.DisplayName, uvm.IsAdmin, uvm.IsApprover);
                }

                ViewBag.DialogResult = BuildDialogResult(success, message);
                return PartialView("_ApproveToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        private bool HasExistingCorrection(int subBrandId, string localStoreId, DateTime endDate)
        {
            var invalidCorrectionStatus = new int[] 
            { 
                (int)ToplineCorrectionStatus.CorrectionApproved,
                (int)ToplineCorrectionStatus.CorrectionClosed,
                (int)ToplineCorrectionStatus.CorrectionPending,
                (int)ToplineCorrectionStatus.CorrectionDenied
            };

            return _salesCorrectionService.GetToplineCorrections(subBrandId, localStoreId, endDate, null).Any(s => invalidCorrectionStatus.Contains(s.StatusID));
        }


        [HttpGet]
        public ActionResult CanCreateToplineCorrection(string localStoreId, string periodEndDate)
        {
            var success = true;
            var message = string.Empty;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var endDate = Convert.ToDateTime(periodEndDate);
                var topline = _brandPeriodSalesService.GetToplines(subBrandId, localStoreId, endDate, null, null, null).FirstOrDefault();

                var allowOperation = this.CanCreateToplineCorrection(uvm.IsAdmin, uvm.IsApprover);
                if (topline == null || topline.SubBrandID != subBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var isApproved = (topline.BPSR_StatusID == (int)ToplineStatus.Submitted);
                var isAlreadySubmitted = HasExistingCorrection(subBrandId, localStoreId, endDate);

                if (!isApproved)
                {
                    message = "The invoice must be Approved.";
                    success = false;
                }
                else if (isAlreadySubmitted)
                {
                    message = "A correction was already submitted.";
                    success = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                success = false;
            }

            return Json(new { Success = success, Message = message }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CreateToplineCorrection(string localStoreId, string periodEndDate)
        {
            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var endDate = Convert.ToDateTime(periodEndDate);
                var topline = _brandPeriodSalesService.GetToplines(subBrandId, localStoreId, endDate, null, null, null).FirstOrDefault();

                var allowOperation = this.CanCreateToplineCorrection(uvm.IsAdmin, uvm.IsApprover);
                if (topline == null || topline.SubBrandID != subBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var isApproved = (topline.BPSR_StatusID == (int)ToplineStatus.Submitted);
                var isAlreadySubmitted = HasExistingCorrection(subBrandId, localStoreId, endDate);

                if (!isApproved || isAlreadySubmitted)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var model = this.BindCreateToplineCorrectionViewModel(topline, uvm.IsAdmin, uvm.IsApprover);
                return PartialView("_CreateToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToplineCorrection(CreateToplineCorrectionViewModel model)
        {
            var requestId = 0;
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var topline = _brandPeriodSalesService.GetToplines(subBrandId, model.LocalStoreId, model.PeriodEndDate, null, null, null).FirstOrDefault();

                var allowOperation = this.CanCreateToplineCorrection(uvm.IsAdmin, uvm.IsApprover);
                var isApproved = (topline.BPSR_StatusID == (int)ToplineStatus.Submitted);
                var isAlreadySubmitted = HasExistingCorrection(subBrandId, model.LocalStoreId, model.PeriodEndDate);

                if (topline == null || topline.SubBrandID != subBrandId || !allowOperation || !isApproved || isAlreadySubmitted)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var submitterName = uvm.DisplayName;
                var submitterEmailAddress = uvm.EmailAddress;

                if (ModelState.IsValid)
                {
                    requestId = ProcessCreateToplineCorrection(submitterName, submitterEmailAddress, topline, model);
                    var correction = _salesCorrectionService.GetToplineCorrectionById(requestId);

                    try
                    {
                        EmailSalesCorrectionSubmitConfirmAsync(submitterName, submitterEmailAddress, topline, correction);
                    }
                    catch (Exception ee)
                    {
                        _log.Error(FormatException(ee));
                    }

                    try
                    {
                        EmailSalesCorrectionSubmitApproveAsync(submitterName, submitterEmailAddress, topline, correction);
                    }
                    catch (Exception ee)
                    {
                        _log.Error(FormatException(ee));
                    }

                    success = true;
                    message = "Successfully submitted Sales Correction.";
                }
                else
                {
                    model = this.BindCreateToplineCorrectionViewModel(topline, uvm.IsAdmin, uvm.IsApprover);
                }

                ViewBag.DialogResult = BuildDialogResult(success, requestId.ToString(), message);
                return PartialView("_CreateToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult EditToplineCorrection(int requestId, int toplineId)
        {
            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;

                var correction = _salesCorrectionService.GetToplineCorrectionById(requestId);
                var topline = _brandPeriodSalesService.GetToplineById(toplineId);

                var allowOperation = this.CanEditToplineCorrection(correction.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover);
                if (topline == null || correction == null || topline.SubBrandID != subBrandId || correction.SubBrandID != subBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var model = this.BindEditToplineCorrectionViewModel(topline, correction, uvm.IsAdmin, uvm.IsApprover);
                return PartialView("_EditToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditToplineCorrection(EditToplineCorrectionViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var subBrandId = (int)uvm.SubBrandId;
                var topline = _brandPeriodSalesService.GetToplineById(model.ToplineId);
                var correction = _salesCorrectionService.GetToplineCorrectionById(model.RequestId);

                var allowOperation = this.CanEditToplineCorrection(correction.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover);
                if (topline == null || correction == null || topline.SubBrandID != subBrandId || correction.SubBrandID != uvm.SubBrandId || !allowOperation)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var submitterName = uvm.DisplayName;
                var submitterEmailAddress = uvm.EmailAddress;

                if (ModelState.IsValid)
                {
                    ProcessUpdateToplineCorrection(submitterName, submitterEmailAddress, model);

                    try
                    {
                        EmailSalesCorrectionSubmitConfirmAsync(submitterName, submitterEmailAddress, topline, correction);
                    }
                    catch (Exception ee)
                    {
                        _log.Error(FormatException(ee));
                    }

                    try
                    {
                        EmailSalesCorrectionSubmitApproveAsync(submitterName, submitterEmailAddress, topline, correction);
                    }
                    catch (Exception ee)
                    {
                        _log.Error(FormatException(ee));
                    }

                    success = true;
                    message = "Successfully updated Sales Correction.";
                }
                else
                {
                    model = this.BindEditToplineCorrectionViewModel(topline, correction, uvm.IsAdmin, uvm.IsApprover);
                }

                ViewBag.DialogResult = BuildDialogResult(success, message);
                return PartialView("_EditToplineCorrection", model);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
        }


        private bool CanCreateToplineCorrection(bool isAdmin, bool isSubmitter)
        {
            //return (isAdmin || isSubmitter);
            return true;
        }


        private bool CanEditToplineCorrection(int status, bool isAdmin, bool isSubmitter)
        {
            //return (((status == (int)ToplineCorrectionStatus.CorrectionPending) || (status == (int)ToplineCorrectionStatus.CorrectionDenied)) && (isAdmin || isSubmitter));
            return ((status == (int)ToplineCorrectionStatus.CorrectionPending) || (status == (int)ToplineCorrectionStatus.CorrectionDenied));
        }


        private bool CanApproveToplineCorrection(bool isAdmin, bool isApprover)
        {
            return ((isAdmin || isApprover));
        }


        private bool CanApproveToplineCorrection(int status, bool isAdmin, bool isApprover)
        {
            return ((status == (int)ToplineCorrectionStatus.CorrectionPending) && (isAdmin || isApprover));
        }


        private void ProcessApproveToplineCorrection(string approverName, string approverEmailAddress, ApproveToplineCorrectionViewModel model)
        {
            var service = new SalesCorrectionService();
            var correction = service.GetToplineCorrectionById(model.RequestId);

            correction.ApproverName = approverName;
            correction.ApproverEmailAddr = approverEmailAddress;

            if (model.ApproveStatus == (int)ToplineCorrectionStatus.CorrectionDenied)
            {
                correction.BPSR_StatusID = model.ApproveStatus;
                correction.DenialReason = model.DenialDescription;
                correction.DenialType = Convert.ToInt32(model.Denial);
            }
            else
            {
                correction.BPSR_StatusID = model.ApproveStatus;
                correction.DenialReason = null;
                correction.DenialType = null;
            }

            service.SaveChanges();
        }


        private int ProcessCreateToplineCorrection(string submitterName, string submitterEmailAddress, BPSR_Topline topline, CreateToplineCorrectionViewModel model)
        {
            var service = new SalesCorrectionService();
            var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate).ToList();

            // create parent
            var correction = new BPSR_Topline_Correction()
            {
                SubBrandID = topline.SubBrandID,
                LocalStoreID = topline.LocalStoreID,
                PeriodEndDate = topline.PeriodEndDate,

                NetSales = ConvertHelpers.ToMoney(model.NetSalesCorrection),
                TotalTickets = ConvertHelpers.ToNumber(model.TotalTicketsCorrection),
                CorrectionReason = model.Reason,
                CorrectionType = model.CorrectionType,
                BPSR_StatusID = (int)ToplineCorrectionStatus.CorrectionPending,
                SalesTypeID = topline.SalesTypeID,
                SubmitterName = submitterName,
                SubmitterEmailAddr = submitterEmailAddress
            };

            // create children
            foreach (var current in model.ProductGroups)
            {
                if (availableToplinePrdSubGrps.Any(a => a.ProdGrpID == current.ProdGrpID && a.ProdSubGrpID == current.ProdSubGrpID))
                {
                    correction.BPSR_ProdGrp_Correction.Add(new BPSR_ProdGrp_Correction()
                    { 
                        ProdGrpID = current.ProdGrpID,
                        ProdSubGrpID = current.ProdSubGrpID,
                        NetSales = ConvertHelpers.ToMoney(current.NetSalesCorrection),
                        TotalTickets = ConvertHelpers.ToNumber(current.TotalTicketsCorrection)
                    });
                }
            }

            // save and return request
            service.CreateNewToplineCorrection(correction);
            service.SaveChanges();
            return correction.BPSR_Topline_CorrectionID;
        }


        private void ProcessUpdateToplineCorrection(string submitterName, string submitterEmailAddress, EditToplineCorrectionViewModel model)
        {
            var toplineId = model.ToplineId;
            var requestId = model.RequestId;

            var service = new SalesCorrectionService();
            var correction = service.GetToplineCorrectionById(requestId);
            var topline = _brandPeriodSalesService.GetToplineById(toplineId);

            // update parent
            correction.NetSales = ConvertHelpers.ToMoney(model.NetSalesCorrection);
            correction.TotalTickets = ConvertHelpers.ToNumber(model.TotalTicketsCorrection);
            correction.CorrectionReason = model.Reason;
            correction.CorrectionType = model.CorrectionType;
            correction.SubmitterName = submitterName;
            correction.SubmitterEmailAddr = submitterEmailAddress;
            correction.BPSR_StatusID = (int)ToplineCorrectionStatus.CorrectionPending;

            // update children
            var requestedUpdates = model.ProductGroups.ToDictionary(x => x.ProdSubGrpID, x => x);
            var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate).ToList();
            
            foreach (var current in requestedUpdates.Values)
            { 
                // check if requestd is valid
                if (availableToplinePrdSubGrps.Any(a => a.ProdGrpID == current.ProdGrpID && a.ProdSubGrpID == current.ProdSubGrpID))
                {
                    // update or create
                    var existingEntry = correction.BPSR_ProdGrp_Correction.FirstOrDefault(f => f.ProdGrpID == current.ProdGrpID && f.ProdSubGrpID == current.ProdSubGrpID && f.BPSR_Topline_CorrectionID == requestId);
                    if (existingEntry == null)
                    {
                        // create new entry
                        if (current.NetSalesCorrection != null || current.TotalTicketsCorrection != null)
                        {
                            CreateToplineCorrectionProductGroup(correction, current);
                        }
                    }
                    else
                    {
                        // update existing entry
                        if (current.NetSalesCorrection == null && current.TotalTicketsCorrection == null)
                        {
                            // leaving orphans; note if you must delete, you must remove children before parent
                            // correction.BPSR_ProdGrp_Correction.Remove(itemToDelete);
                            UpdateToplineCorrectionProductGroup(existingEntry, current);
                        }
                        else
                        {
                            UpdateToplineCorrectionProductGroup(existingEntry, current);
                        }
                    }
                }
            }

            // save
            service.SaveChanges();
        }


        private void UpdateToplineCorrectionProductGroup(BPSR_ProdGrp_Correction existingEntry, ToplineCorrectionProductGroupViewModel model)
        {
            var netSales = ConvertHelpers.ToMoney(model.NetSalesCorrection);
            var totalTickets = ConvertHelpers.ToNumber(model.TotalTicketsCorrection);

            if (existingEntry.NetSales != netSales)
                existingEntry.NetSales = netSales;

            if (existingEntry.TotalTickets != totalTickets)
                existingEntry.TotalTickets = totalTickets;
        }


        private void CreateToplineCorrectionProductGroup(BPSR_Topline_Correction correction, ToplineCorrectionProductGroupViewModel current)
        {
            correction.BPSR_ProdGrp_Correction.Add(new BPSR_ProdGrp_Correction()
            {
                ProdGrpID = current.ProdGrpID,
                ProdSubGrpID = current.ProdSubGrpID, 
                BPSR_Topline_CorrectionID = correction.BPSR_Topline_CorrectionID,
                NetSales = ConvertHelpers.ToMoney(current.NetSales),
                TotalTickets = ConvertHelpers.ToNumber(current.TotalTickets)
            });
        }


        private ApproveToplineCorrectionViewModel BindApproveToplineCorrectionViewModel(BPSR_Topline topline, BPSR_Topline_Correction correction, string userName, bool isAdmin, bool isApprover)
        {
            var denialTypes = _salesCorrectionService.GetDenialTypes().ToList();
            var statusIds = _brandPeriodSalesService.GetStatuses().ToList();
            var allowedStatus = Enum.GetValues(typeof(ToplineCorrectionStatus)).Cast<int>();
            var filteredStatus = statusIds.Where(o => allowedStatus.Contains(o.BPSR_StatusID));

            var denialType = (correction.BPSR_StatusID == (int)ToplineCorrectionStatus.CorrectionDenied) ? (correction.DenialType.ToString() ?? "") : "";
            var denials = (from c in denialTypes select new SelectListItem() { Text = c.DenialTypeDescription, Value = c.BPSR_DenialTypeID.ToString() }).ToList();
            var denialDescription = (correction.BPSR_StatusID == (int)ToplineCorrectionStatus.CorrectionDenied) ? correction.DenialReason : "";

            // submitter cannot approve
            if (!SecurityHelpers.IsValidSubmitterApprover(correction.SubmitterName, userName))
            {
                var item = statusIds.Where(x => x.BPSR_StatusID == ((int)ToplineCorrectionStatus.CorrectionApproved)).FirstOrDefault();
                if (item != null)
                {
                    statusIds.Remove(item);
                }
            }

            var model = new ApproveToplineCorrectionViewModel()
            {
                RequestId = correction.BPSR_Topline_CorrectionID,
                ToplineId = topline.BPSR_ToplineID,
                LocalStoreId = correction.LocalStoreID,
                PeriodEndDate = correction.PeriodEndDate,

                Title = this.FormatTitle(String.Format("Store #{0}", correction.LocalStoreID), correction.PeriodEndDate, topline.BPSR_ToplineID),

                ApproveStatus = correction.BPSR_StatusID,
                ApproveStatusIds = (from c in filteredStatus select new SelectListItem() { Text = c.BPSR_StatusDescription, Value = c.BPSR_StatusID.ToString() }).ToList(),

                Denial = denialType,
                Denials = denials,
                DenialDescription = denialDescription
            };

            return model;
        }


        private CreateToplineCorrectionViewModel BindCreateToplineCorrectionViewModel(BPSR_Topline topline, bool isAdmin, bool isApprover)
        {
            var storeIds = _brandPeriodSalesService.GetStoreIds(topline.SubBrandID).OrderBy(x => x, new SemiNumericComparer()); ;
            var correctionTypes = _salesCorrectionService.GetCorrectionTypes();
            var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate);

            var model = new CreateToplineCorrectionViewModel()
            {
                ToplineId = topline.BPSR_ToplineID,
                StatusId = topline.BPSR_StatusID,
                LocalStoreId = topline.LocalStoreID,
                LocalStoreIds = (from m in storeIds select new SelectListItem() { Text = m, Value = m.ToString() }).ToList(),
                
                SalesTypeId = topline.SalesTypeID,
                SalesTypeDescription = FormatHelpers.FormatSalesType(topline.SalesTypeID),
                NetSales = FormatHelpers.FormatMoney(topline.NetSales),
                TotalTickets = FormatHelpers.FormatNumber(topline.TotalTickets),

                Title = FormatTitle("Submit Sales Correction", topline.PeriodEndDate),
                PeriodEndDate = topline.PeriodEndDate,

                ProductGroups = BindCreateToplineCorrectionProductsGroupViewModel(topline.BPSR_ProdGrp, availableToplinePrdSubGrps),
                CorrectionTypes = (from m in correctionTypes select new SelectListItem() { Text = m.CorrectionTypeDescription, Value = m.BPSR_CorrectionTypeID.ToString() }).ToList()
            };

            return model;
        }


        private List<ToplineCorrectionProductGroupViewModel> BindCreateToplineCorrectionProductsGroupViewModel(IEnumerable<BPSR_ProdGrp> prodGrps, IEnumerable<ToplinePrdSubGrps_Result> availableToplinePrdSubGrps)
        {
            var results = new List<ToplineCorrectionProductGroupViewModel>();

            try
            {
                // trap and protect this potential exception due to data integrity issues (may not be unique)
                var dict = prodGrps.Where(s => s.ProdSubGrpID != null).ToDictionary(x => x.ProdSubGrpID, x => x);

                foreach (var p in availableToplinePrdSubGrps)
                {
                    var v = new ToplineCorrectionProductGroupViewModel()
                    {
                        ProdGrpID = p.ProdGrpID,
                        ProdSubGrpID = p.ProdSubGrpID,
                        Name = p.ProdSubGrpDescription 
                    };

                    if (dict.ContainsKey(p.ProdSubGrpID))
                    {
                        var m = dict[p.ProdSubGrpID];
                        
                        v.BPSR_ToplineID = m.BPSR_ToplineID;
                        v.BPSR_ProdGrpID = m.BPSR_ProdGrpID; 
                        v.NetSales = FormatHelpers.FormatMoney(m.NetSales);
                        v.TotalTickets = FormatHelpers.FormatNumber(m.TotalTickets); 
                    }

                    results.Add(v);
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
            }

            return results;
        }


        private EditToplineCorrectionViewModel BindEditToplineCorrectionViewModel(BPSR_Topline topline, BPSR_Topline_Correction correction, bool isAdmin, bool isApprover)
        {
            var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate);
            var availableToplinePrdGrpsCorrections = _salesCorrectionService.GetToplinePrdGrpCorrections(correction.BPSR_Topline_CorrectionID);
            var correctionTypes = _salesCorrectionService.GetCorrectionTypes();

            var model = new EditToplineCorrectionViewModel()
            {
                ToplineId = topline.BPSR_ToplineID,
                StatusId = topline.BPSR_StatusID,
                LocalStoreId = topline.LocalStoreID,

                NetSales = FormatHelpers.FormatMoney(topline.NetSales),
                TotalTickets = FormatHelpers.FormatNumber(topline.TotalTickets),

                Title = FormatTitle("Submit Sales Correction", topline.PeriodEndDate, topline.BPSR_ToplineID),
                ProductGroups = BindEditToplineCorrectionProductsGroupViewModel(topline.BPSR_ProdGrp, availableToplinePrdSubGrps, availableToplinePrdGrpsCorrections),
                NetSalesCorrection = FormatHelpers.FormatMoney(correction.NetSales),
                TotalTicketsCorrection = FormatHelpers.FormatNumber(correction.TotalTickets),

                SalesTypeId = topline.SalesTypeID,
                SalesTypeDescription = FormatHelpers.FormatSalesType(topline.SalesTypeID),
                RequestId = correction.BPSR_Topline_CorrectionID,
                Reason = correction.CorrectionReason,
                CorrectionType = correction.CorrectionType,
                CorrectionTypes = (from m in correctionTypes select new SelectListItem() { Text = m.CorrectionTypeDescription, Value = m.BPSR_CorrectionTypeID.ToString() }).ToList()
            };

            return model;
        }


        private List<ToplineCorrectionProductGroupViewModel> BindEditToplineCorrectionProductsGroupViewModel(IEnumerable<BPSR_ProdGrp> prodGrps, IEnumerable<ToplinePrdSubGrps_Result> availableToplinePrdSubGrps, IEnumerable<BPSR_ProdGrp_Correction> availableToplinePrdGrpsCorrections)
        {
            var results = new List<ToplineCorrectionProductGroupViewModel>();

            try
            {
                // trap and protect this potential exception due to data integrity issues (may not be unique)
                var dict = prodGrps.Where(s => s.ProdSubGrpID != null).ToDictionary(x => x.ProdSubGrpID, x => x);
                var dictCorrections = availableToplinePrdGrpsCorrections.Where(s => s.ProdSubGrpID != null).ToDictionary(x => x.ProdSubGrpID, x => x);

                foreach (var p in availableToplinePrdSubGrps)
                {
                    var v = new ToplineCorrectionProductGroupViewModel()
                    {
                        ProdGrpID = p.ProdGrpID,
                        ProdSubGrpID = p.ProdSubGrpID,
                        Name = p.ProdSubGrpDescription
                    };

                    if (dict.ContainsKey(p.ProdSubGrpID))
                    {
                        var m = dict[p.ProdSubGrpID];
                        v.BPSR_ToplineID = m.BPSR_ToplineID;
                        v.BPSR_ProdGrpID = m.BPSR_ProdGrpID;
                        v.NetSales = FormatHelpers.FormatMoney(m.NetSales);
                        v.TotalTickets = FormatHelpers.FormatNumber(m.TotalTickets);
                    }

                    if (dictCorrections.ContainsKey(p.ProdSubGrpID))
                    {
                        var x = dictCorrections[p.ProdSubGrpID];
                        v.NetSalesCorrection = FormatHelpers.FormatMoney(x.NetSales);
                        v.TotalTicketsCorrection = FormatHelpers.FormatNumber(x.TotalTickets);
                    }

                    results.Add(v);
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
            }

            return results;
        }


        private List<ToplineCorrectionViewModel> BindToplineCorrectionsViewModel(List<ToplineCorrection_Result> toplineCorrections, bool isAdmin, bool isApprover)
        {
            var results = new List<ToplineCorrectionViewModel>();

            foreach (var t in toplineCorrections)
            {
                var item = new ToplineCorrectionViewModel()
                {
                    RequestId = t.CorrectionID,
                    ToplineId = t.ToplineID,
                    StoreId = t.LocalStoreID.ToString(),
                    Status = FormatHelpers.FormatStatus(t.StatusID),
                    PeriodEndDate = FormatHelpers.FormatPeriodDate(t.PeriodEndDate),

                    Sales = FormatHelpers.FormatMoney(t.Sales),
                    ActiveSales = FormatHelpers.FormatMoney(t.ActiveSales),

                    Tickets = FormatHelpers.FormatNumber(t.Tickets),
                    ActiveTickets = FormatHelpers.FormatNumber(t.ActiveTickets),

                    CanEdit = CanEditToplineCorrection(t.StatusID, isAdmin, isApprover),
                    CanApprove = CanApproveToplineCorrection(t.StatusID, isAdmin, isApprover)
                };

                results.Add(item);
            }

            return results;
        }


        private string FormatTitle(string operation, DateTime periodEndDate)
        {
            return String.Format("{0} - Reporting Period {1}", operation, FormatHelpers.FormatPeriodDate(periodEndDate));
        }


        private string FormatTitle(string operation, DateTime periodEndDate, int invoiceId)
        {
            return String.Format("{0} - Reporting Period {1} - Invoice {2}", operation, FormatHelpers.FormatPeriodDate(periodEndDate), invoiceId);
        }


        private class SemiNumericComparer : IComparer<string>
        {
            public int Compare(string first, string second)
            {
                // for simplicity, let's assume neither is null
                float firstNumber, secondNumber;
                bool firstIsNumber = float.TryParse(first, out firstNumber);
                bool secondIsNumber = float.TryParse(second, out secondNumber);

                if (firstIsNumber)
                {
                    // if they're both numbers, compare them; otherwise first comes first
                    return secondIsNumber ? firstNumber.CompareTo(secondNumber) : -1;
                }

                // if second is a number, that should come first; otherwise compare as strings
                return secondIsNumber ? 1 : first.CompareTo(second);
            }
        }

    }
}
