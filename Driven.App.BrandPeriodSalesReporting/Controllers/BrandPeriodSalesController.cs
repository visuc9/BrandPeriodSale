using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Driven.Business.Royalty;
using Driven.Business.Royalty.Interfaces;
using Driven.Data.Entity.Royalty;

using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    public class BrandPeriodSalesController : BaseController
    {
        private const int mc_ActionResult_MaxRows = 10000;
        private const int mc_ActionResult_MaxJsonLength = 86753090; // Int32.MaxValue

        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private enum ToplineStatus { NotStarted = 1, InProgress = 2, ForApproval = 3, Submitted = 4 };

        private enum AvailableSalesType { NonFleet = 1, Fleet = 2 }

        private IBrandPeriodSalesService _brandPeriodSalesService;

        public BrandPeriodSalesController() : this(new BrandPeriodSalesService()) { }

        public BrandPeriodSalesController(IBrandPeriodSalesService brandPeriodSalesService)
        {
            _brandPeriodSalesService = brandPeriodSalesService;
        }


        public ActionResult Index()
        {
            var subBrandId = GetCurrentSubBrandId();
            var userViewModel = GetCurrentUserViewModel();

            var myStatuses = _brandPeriodSalesService.GetStatuses().ToList();
            var myPeriods = _brandPeriodSalesService.GetPeriodEndDates(subBrandId).ToList();
            var myStores = _brandPeriodSalesService.GetStoreIds(subBrandId).OrderBy(x => x, new SemiNumericComparer());

            var userAuthStores = (!userViewModel.IsAdmin && !userViewModel.IsApprover && userViewModel.UserType == "FZ")
                    ? myStores.Intersect(userViewModel.UserAuthStores)
                    : myStores;

            var model = new ToplineListViewModel();

            model.StoreIds = (from c in userAuthStores select new SelectListItem() { Text = c, Value = c }).ToList();
            model.StoreId = string.Empty;

            var allowedStatus = Enum.GetValues(typeof(ToplineStatus)).Cast<int>();
            var filteredStatus = myStatuses.Where(o => allowedStatus.Contains(o.BPSR_StatusID));

            model.StatusIds = (from c in filteredStatus select new SelectListItem() { Text = c.BPSR_StatusDescription, Value = c.BPSR_StatusID.ToString() }).ToList();
            //model.StatusIds = (from c in myStatuses select new SelectListItem() { Text = c.BPSR_StatusDescription, Value = c.BPSR_StatusID.ToString() }).ToList();
            model.StatusId = string.Empty;

            model.PeriodEndDates = (from c in myPeriods select new SelectListItem() { Text = FormatHelpers.FormatPeriodDate(c), Value = FormatHelpers.FormatPeriodDate(c) }).ToList();
            model.PeriodEndDate = (model.PeriodEndDates != null && model.PeriodEndDates.Count > 0) ? model.PeriodEndDates.FirstOrDefault().Value : string.Empty;

            model.IsPastDue = false;
            model.IsAdmin = userViewModel.IsAdmin;
            model.IsApprover = userViewModel.IsApprover;

            return View(model);
        }


        [HttpGet]
        public ActionResult GetToplines(string storeId, DateTime? periodEndDate, int? statusId, bool? isPastDue)
        {
            var uvm = this.GetCurrentUserViewModel();
            var subBrandId = this.GetCurrentSubBrandId();
            var dueDate = this.GetSubBrandDueDate(subBrandId).Date;

            List<BPSR_Topline> toplines = null;
            if (uvm.UserType == "FZ")
            {
                toplines = _brandPeriodSalesService.GetToplines(subBrandId, storeId, periodEndDate, statusId, isPastDue, dueDate).Where(x => uvm.UserAuthStores.Contains(x.LocalStoreID))
                    .Take(mc_ActionResult_MaxRows).ToList();
            }
            else
            {
                toplines = _brandPeriodSalesService.GetToplines(subBrandId, storeId, periodEndDate, statusId, isPastDue, dueDate)
                    .Take(mc_ActionResult_MaxRows).ToList();
            }


            var model = this.BindToplinesViewModel(toplines, dueDate, uvm.DisplayName, uvm.IsAdmin, uvm.IsApprover);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        // This ActionResult will override the existing JsonResult and will automatically set the maximum JSON length
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = mc_ActionResult_MaxJsonLength
            };
        }


        private DateTime GetSubBrandDueDate(int subBrandId)
        {
            try
            {
                return _brandPeriodSalesService.GetSubBrandDueDate(subBrandId);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                return DateTime.Now;
            }
        }


        [HttpGet]
        public ActionResult EditTopline(int toplineId)
        {
            try
            {
                var uvm = GetCurrentUserViewModel();

                var topline = _brandPeriodSalesService.GetToplineById(toplineId);
                if (topline == null || topline.SubBrandID != uvm.SubBrandId || !this.CanEditTopline(topline.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate);
                var vm = this.BindEditToplineViewModel(topline, availableToplinePrdSubGrps, uvm.IsAdmin, uvm.IsApprover);
                return PartialView("_EditTopline", vm);
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
        public ActionResult EditTopline(EditToplineViewModel model)
        {
            var success = false;
            var message = mc_ExceptionMessage_Error;

            if (ModelState.IsValid)
            {
                try
                {
                    var uvm = GetCurrentUserViewModel();
                    var topline = _brandPeriodSalesService.GetToplineById(model.ToplineId);
                    var allowOperation = (model.IsApprove)
                        ? this.CanApproveTopline(topline.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover)
                        : this.CanEditTopline(topline.BPSR_StatusID, uvm.IsAdmin, uvm.IsApprover);

                    if (topline == null || topline.SubBrandID != uvm.SubBrandId || !allowOperation)
                    {
                        Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                        return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                    }

                    UpdateTopline(model);
                    success = true;
                    message = "Successfully updated Reporting Period.";
                }
                catch (Exception ex)
                {
                    _log.Error(FormatException(ex));
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
                }
            }
            model.SalesTypeList = this.GetAvailableSalesTypeList(model.SalesTypeId);
            ViewBag.DialogResult = BuildDialogResult(success, model.ToplineId.ToString(), message);
            return PartialView("_EditTopline", model);
        }


        private void UpdateTopline(EditToplineViewModel model)
        {
            var uvm = GetCurrentUserViewModel();
            var service = new BrandPeriodSalesService();
            var topline = service.GetToplineById(model.ToplineId);

            // update parent
            topline.NetSales = ConvertHelpers.ToMoney(model.NetSales);
            topline.FranCalcRoyalty = ConvertHelpers.ToMoney(model.FranCalcRoyalty);
            topline.FranCalcAdvertising = ConvertHelpers.ToMoney(model.FranCalcAdvertising);
            topline.TotalTickets = ConvertHelpers.ToNumber(model.TotalTickets);

            if (model.IsApprove)
            {
                topline.BPSR_StatusID = (int)ToplineStatus.ForApproval;
                topline.SubmitterName = uvm.DisplayName;
            }
            else if (topline.BPSR_StatusID == (int)ToplineStatus.NotStarted || topline.BPSR_StatusID == (int)ToplineStatus.ForApproval)
            {
                topline.BPSR_StatusID = (int)ToplineStatus.InProgress;
            }

            // update children
            var requestedUpdates = model.ProductGroups.ToDictionary(x => x.ProdSubGrpID, x => x);
            var potentialUpdates = topline.BPSR_ProdGrp.ToList().ToDictionary(x => x.ProdSubGrpID, x => x);
            var availableToplinePrdSubGrps = _brandPeriodSalesService.GetToplinePrdSubGrps(topline.LocalStoreID, topline.SubBrandID, topline.PeriodEndDate).ToList();

            foreach (var current in requestedUpdates.Values)
            {
                if (current.BPSR_ProdGrpID == null && (current.NetSales != null || current.FranCalcRoyalty != null || current.FranCalcAdvertising != null || current.TotalTickets != null))
                {
                    // check if requested is valid
                    if (availableToplinePrdSubGrps.Any(a => a.ProdGrpID == current.ProdGrpID && a.ProdSubGrpID == current.ProdSubGrpID))
                    {
                        // check if already exists
                        var existingEntry = topline.BPSR_ProdGrp.FirstOrDefault(f => f.ProdGrpID == current.ProdGrpID && f.ProdSubGrpID == current.ProdSubGrpID);
                        if (existingEntry == null)
                        {
                            CreateToplineProductGroup(topline, current);
                        }
                        else
                        {
                            UpdateToplineProductGroup(existingEntry, current);
                        }
                    }
                }
                else
                {
                    // update existing entry
                    var existingEntry = topline.BPSR_ProdGrp.FirstOrDefault(f => f.BPSR_ProdGrpID == current.BPSR_ProdGrpID);
                    if (existingEntry != null)
                    {
                        UpdateToplineProductGroup(existingEntry, current);
                    }
                }
            }

            // save
            service.SaveChanges();
        }


        private void UpdateToplineProductGroup(BPSR_ProdGrp existingEntry, ToplineProductGroupViewModel model)
        {
            var netSales = ConvertHelpers.ToMoney(model.NetSales);
            var franCalcRoyalty = ConvertHelpers.ToMoney(model.FranCalcRoyalty);
            var franCalcAdvertising = ConvertHelpers.ToMoney(model.FranCalcAdvertising);
            var totalTickets = ConvertHelpers.ToNumber(model.TotalTickets);

            if (existingEntry.NetSales != netSales)
                existingEntry.NetSales = netSales;

            if (existingEntry.FranCalcRoyalty != franCalcRoyalty)
                existingEntry.FranCalcRoyalty = franCalcRoyalty;

            if (existingEntry.FranCalcAdvertising != franCalcAdvertising)
                existingEntry.FranCalcAdvertising = franCalcAdvertising;

            if (existingEntry.TotalTickets != totalTickets)
                existingEntry.TotalTickets = totalTickets;

        }


        private void CreateToplineProductGroup(BPSR_Topline topline, ToplineProductGroupViewModel current)
        {
            topline.BPSR_ProdGrp.Add(new BPSR_ProdGrp()
            {
                BPSR_ToplineID = topline.BPSR_ToplineID,
                ProdGrpID = current.ProdGrpID,
                ProdSubGrpID = current.ProdSubGrpID,
                NetSales = ConvertHelpers.ToMoney(current.NetSales),
                FranCalcRoyalty = ConvertHelpers.ToMoney(current.FranCalcRoyalty),
                FranCalcAdvertising = ConvertHelpers.ToMoney(current.FranCalcAdvertising),
                TotalTickets = ConvertHelpers.ToNumber(current.TotalTickets)
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitToplines(string toplines)
        {
            var success = false;
            var keyId = string.Empty;
            var message = string.Empty;

            try
            {
                var entryCount = 0;
                var uvm = GetCurrentUserViewModel();
                var service = new BrandPeriodSalesService();

                foreach (var toplineId in toplines.Split(',').Select(t => int.Parse(t)).ToList())
                {
                    var topline = service.GetToplineById(toplineId);

                    if (topline != null && topline.SubBrandID == uvm.SubBrandId && CanSubmitTopline(topline.BPSR_StatusID, topline.SubmitterName, uvm.DisplayName, uvm.IsAdmin, uvm.IsApprover))
                    {
                        topline.ApproverName = uvm.DisplayName;
                        topline.BPSR_StatusID = (int)ToplineStatus.Submitted;
                        service.SaveChanges();
                        entryCount += 1;
                    }
                }

                success = true;
                message = String.Format("Successfully approved {0} {1}.", entryCount, ((entryCount == 1) ? "Reporting Period" : "Reporting Periods"));
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            var result = BuildDialogResult(success, keyId, message);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private List<ToplineViewModel> BindToplinesViewModel(List<BPSR_Topline> toplines, DateTime dueDate, string approverName, bool isAdmin, bool isApprover)
        {
            var results = new List<ToplineViewModel>();

            foreach (var t in toplines)
            {
                var item = new ToplineViewModel()
                {
                    ToplineId = t.BPSR_ToplineID,
                    StoreId = t.LocalStoreID,
                    Status = FormatHelpers.FormatStatus(t.BPSR_StatusID),
                    PeriodEndDate = FormatHelpers.FormatPeriodDate(t.PeriodEndDate),
                    NetSales = FormatHelpers.FormatMoney(t.NetSales),
                    FranCalcRoyalty = FormatHelpers.FormatMoney(t.FranCalcRoyalty),
                    FranCalcAdvertising = FormatHelpers.FormatMoney(t.FranCalcAdvertising),
                    TotalTickets = FormatHelpers.FormatNumber(t.TotalTickets),
                    SalesTypeCode = FormatHelpers.FormatSalesType(t.SalesTypeID), //t.SalesType.SalesTypeCode, //FormatSalesType
                    IsPastDue = this.IsPastDue(dueDate, t.PeriodEndDate),

                    //CanEdit = this.CanEditTopline(t.BPSR_StatusID, isAdmin, isApprover),
                    //CanApprove = this.CanApproveTopline(t.BPSR_StatusID, isAdmin, isApprover),
                    //CanSubmit = this.CanSubmitTopline(t.BPSR_StatusID, isAdmin, isApprover)

                    CanEdit = this.CanEditTopline(t.BPSR_StatusID, isAdmin, isApprover),
                    CanApprove = this.CanApproveTopline(t.BPSR_StatusID, isAdmin, isApprover),
                    CanSubmit = this.CanSubmitTopline(t.BPSR_StatusID, t.SubmitterName, approverName, isAdmin, isApprover)
                };

                results.Add(item);
            }

            return results;
        }


        private string FormatTitle(string operation, string store, DateTime periodEndDate)
        {
            return String.Format("{0} - Store #{1} - Reporting Period {2}", operation, store, FormatHelpers.FormatPeriodDate(periodEndDate));
        }


        private EditToplineViewModel BindEditToplineViewModel(BPSR_Topline topline, IEnumerable<ToplinePrdSubGrps_Result> availableToplinePrdSubGrps, bool isAdmin, bool isApprover)
        {
            var model = new EditToplineViewModel()
            {
                ToplineId = topline.BPSR_ToplineID,
                StatusId = topline.BPSR_StatusID,
                LocalStoreId = topline.LocalStoreID,

                NetSales = FormatHelpers.FormatMoney(topline.NetSales),
                TotalTickets = FormatHelpers.FormatNumber(topline.TotalTickets),
                FranCalcRoyalty = FormatHelpers.FormatMoney(topline.FranCalcRoyalty),
                FranCalcAdvertising = FormatHelpers.FormatMoney(topline.FranCalcAdvertising),

                Title = FormatTitle("Enter Sales", topline.LocalStoreID, topline.PeriodEndDate),
                ProductGroups = BindToplineProductsGroupViewModel(topline.BPSR_ProdGrp, availableToplinePrdSubGrps),

                IsApprove = false,
                CanApprove = this.CanApproveTopline(topline.BPSR_StatusID, isAdmin, isApprover),
                SalesTypeId = topline.SalesTypeID,
                SalesTypeList = this.GetAvailableSalesTypeList(topline.SalesTypeID)
            };

            return model;
        }


        private List<ToplineProductGroupViewModel> BindToplineProductsGroupViewModel(IEnumerable<BPSR_ProdGrp> prodGrps, IEnumerable<ToplinePrdSubGrps_Result> availableToplinePrdSubGrps)
        {
            var results = new List<ToplineProductGroupViewModel>();

            try
            {
                // trap and protect this potential exception due to data integrity issues (may not be unique)
                var dict = prodGrps.Where(s => s.ProdSubGrpID != null).ToDictionary(x => x.ProdSubGrpID, x => x);

                foreach (var p in availableToplinePrdSubGrps)
                {
                    var v = new ToplineProductGroupViewModel()
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
                        v.FranCalcRoyalty = FormatHelpers.FormatMoney(m.FranCalcRoyalty);
                        v.FranCalcAdvertising = FormatHelpers.FormatMoney(m.FranCalcAdvertising);
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


        private bool IsPastDue(DateTime dueDate, DateTime periodEndDate)
        {
            return (dueDate > periodEndDate.Date);
        }


        private bool CanEditTopline(int status, bool isAdmin, bool isApprover)
        {
            var canEdit = false;
            var toplineStatus = (ToplineStatus)status;

            if (toplineStatus == ToplineStatus.NotStarted || toplineStatus == ToplineStatus.InProgress || toplineStatus == ToplineStatus.ForApproval)
                return true;

            return canEdit;
        }


        private bool CanApproveTopline(int status, bool isAdmin, bool isApprover)
        {
            var canApprove = false;
            var toplineStatus = (ToplineStatus)status;

            if (toplineStatus == ToplineStatus.NotStarted || toplineStatus == ToplineStatus.InProgress || toplineStatus == ToplineStatus.ForApproval)
                canApprove = true;

            return canApprove;
        }


        private bool CanSubmitTopline(int status, string submitterName, string approverName, bool isAdmin, bool isApprover)
        {
            var canSubmit = false;
            var toplineStatus = (ToplineStatus)status;

            if (isAdmin || isApprover)
            {
                if (toplineStatus == ToplineStatus.ForApproval)
                {
                    if (SecurityHelpers.IsValidSubmitterApprover(submitterName, approverName))
                    {
                        return true;
                    }
                }
            }

            return canSubmit;
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

        private List<SelectListItem> GetAvailableSalesTypeList(int? salesTypeId)
        {
            List<SelectListItem> res = new List<SelectListItem>();
            foreach (int i in Enum.GetValues(typeof(AvailableSalesType)))
            {
                SelectListItem stli = new SelectListItem() { Value = i.ToString(), Text = FormatHelpers.FormatSalesType(i) };
                if (i == salesTypeId)
                    stli.Selected = true;
                res.Add(stli);

            }
            return res;
        }
    }
}
