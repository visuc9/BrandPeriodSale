using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;
using Driven.Business.Royalty.Interfaces;
using Driven.Business.Royalty;
using Driven.Data.Entity.Royalty;
using System.Globalization;
using System.Security.Claims;

namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    public class NotificationController : BaseController
    {
        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICenterService _centerService;
        private IBrandService _brandService;
        private INotificationService _notificationService;

        public NotificationController() : this(new CenterService(), new BrandService(), new NotificationService()) { }

        public NotificationController(ICenterService centerService, IBrandService brandService, INotificationService notificationService)
        {
            _centerService = centerService;
            _brandService = brandService;
            _notificationService = notificationService;
        }


        public ActionResult Index()
        {
            var subBrandId = GetCurrentSubBrandId();
            var subBrand = _brandService.GetSubBrandById(subBrandId);
            var uvm = SecurityHelpers.GetUserViewModel();

            var model = new NotificationListViewModel()
            {
                SubBrandId = subBrand.SubBrandID,
                isEmailEnabled = subBrand.EmailEnabled,
                EmailSendTime = TimespanToString(subBrand.EmailSendTime),
                isCallEnabled = subBrand.CallEnabled,
                CallSendTime = TimespanToString(subBrand.CallSendTime),
                CanEdit = CanEditNotification(uvm.IsAdmin)
            };

            var statusList = _centerService.GetCenterStatus();
            foreach (var status in statusList)
            {
                model.StoreStatusList.Add(new SelectListItem() { Text = status.CenterStatusDescription.ToString(), Value = status.CenterStatusID.ToString() });
            }

            return View(model);
        }


        public ActionResult EditLevelNotification(int subBrandId, int level)
        {
            var uvm = GetCurrentUserViewModel();

            if (!uvm.AvailableSubBrands.Any(x => x.Key == subBrandId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
            }

            var notification = _notificationService.GetLevelNotification(subBrandId, level);
            var model = new EditLevelNotificationViewModel()
            {
                Level = level,
                SubBrandId = subBrandId
            };

            if (notification != null)
            {
                model.NotificationId = notification.NotificationID;
                model.CallText = notification.CallText;
                model.EmailBody = notification.EmailBody;
                model.EmailSubject = notification.EmailSubject;
            }

            return PartialView("_EditLevelNotification", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevelNotification(EditLevelNotificationViewModel model)
        {
            var message = mc_ExceptionMessage_Error;
            var uvm = GetCurrentUserViewModel();

            if (!uvm.AvailableSubBrands.Any(x => x.Key == model.SubBrandId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
            }

            try
            {
                model.CallText = StripHtmlWrapper(HttpUtility.UrlDecode(model.CallText));
                model.EmailSubject = StripHtmlWrapper(HttpUtility.UrlDecode(model.EmailSubject));
                model.EmailBody = HttpUtility.UrlDecode(model.EmailBody);

                if (ModelState.IsValid)
                {
                    _notificationService.UpdateLevelNotification(model.SubBrandId, model.Level, model.EmailSubject, model.EmailBody, model.CallText, uvm.Name);
                    message = String.Format("Level {0} Notification was successfully updated.", model.Level);
                    ViewBag.DialogResult = BuildDialogResult(true, message);
                    return PartialView("_EditLevelNotification", model);
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(false, message);
            return PartialView("_EditLevelNotification", model);
        }


        public ActionResult EditBrandNotification(int subBrandId)
        {
            var uvm = GetCurrentUserViewModel();
            if (!uvm.AvailableSubBrands.Any(x => x.Key == subBrandId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
            }

            var notification = _notificationService.GetSubBrandNotification(subBrandId);

            var model = new EditBrandNotificationViewModel()
            {
                SubBrandId = subBrandId,
                EmailBody = notification.EmailBody,
                EmailSubject = notification.EmailSubject,
                Recipients = notification.Recipients
            };

            return PartialView("_EditBrandNotification", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBrandNotification(EditBrandNotificationViewModel model)
        {
            var message = mc_ExceptionMessage_Error;
            var uvm = GetCurrentUserViewModel();

            if (!uvm.AvailableSubBrands.Any(x => x.Key == model.SubBrandId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
            }

            try
            {
                model.Recipients = StripHtmlWrapper(HttpUtility.UrlDecode(model.Recipients));
                model.EmailSubject = StripHtmlWrapper(HttpUtility.UrlDecode(model.EmailSubject));
                model.EmailBody = HttpUtility.UrlDecode(model.EmailBody);

                if (!ValidateRecipients(model.Recipients))
                    ModelState.AddModelError("Recipients", "The Recipients field is not a valid e-mail address.");

                if (ModelState.IsValid)
                {
                    _notificationService.UpdateSubBrandNotification(model.SubBrandId, model.Recipients, model.EmailSubject, model.EmailBody, uvm.Name);

                    message = "Brand Notification was successfully updated.";
                    ViewBag.DialogResult = BuildDialogResult(true, message);
                    return PartialView("_EditBrandNotification", model);
                }
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            ViewBag.DialogResult = BuildDialogResult(false, message);
            return PartialView("_EditBrandNotification", model);
        }

        
        [HttpGet]
        public ActionResult GetNotification(string storeId, string statusId)
        {
            var subBrandId = GetCurrentSubBrandId();

            var uvm = this.GetCurrentUserViewModel();
            var canEdit = this.CanEditNotification(uvm.IsAdmin);

            var query = _centerService.GetCenterListBySubBrand(subBrandId);

            if (storeId != null && storeId.Trim().Length > 0) 
            {
                query = query.Where(s => s.LocalStoreId == storeId);
            }

            int centerStatusId = 0;
            if (statusId != null && Int32.TryParse(statusId, out centerStatusId))
            {
                query = query.Where(s => s.CenterStatusId == centerStatusId);
            }

            var model = new List<NotificationViewModel>();
            var results = query.ToList();

            if (results != null)
            {
                foreach (var item in query.ToList())
                {
                    model.Add(new NotificationViewModel() { CenterId = item.CenterId, ShopId = item.ShopId, LocationStoreId = item.LocalStoreId, CenterStatusDescription = item.CenterStatusDesc, ExcludeEmail = item.ExcludeEmail, ExcludeCall = item.ExcludeCall, CanEdit = canEdit, TimeZone = item.TimeZoneOffset });
                }
            }
            
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmailNotification(int centerId, bool isEmailExclude)
        {
            var success = false;
            var keyId = string.Empty;
            var message = string.Empty;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var center = _centerService.GetCenterById(centerId);

                if (center != null && center.SubBrandID == uvm.SubBrandId && CanEditNotification(uvm.IsAdmin))
                {
                    string userName = string.Empty;
                    if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                    {
                        var UserIdentity = (ClaimsIdentity)User.Identity;
                        userName = UserIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    }

                    center.ExcludeEmail = isEmailExclude;
                    center.LastModifiedDate = DateTime.Now;
                    center.LastModifiedUser = userName;
                    //_centerService.SaveChanges(center);
                    _centerService.SaveChanges();
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                success = true;
                message = String.Format("Successfully {0} Email.", ((isEmailExclude) ? "Excluded" : "Included"));
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = "", Message = message }), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCallNotification(int centerId, bool isCallExclude)
        {
            var success = false;
            var keyId = string.Empty;
            var message = string.Empty;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var center = _centerService.GetCenterById(centerId);

                if (center != null && center.SubBrandID == uvm.SubBrandId && CanEditNotification(uvm.IsAdmin))
                {
                    string userName = string.Empty;
                    if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                    {
                        var UserIdentity = (ClaimsIdentity)User.Identity;
                        userName = UserIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    }
                    center.ExcludeCall = isCallExclude;
                    center.LastModifiedDate = DateTime.Now;
                    center.LastModifiedUser = userName;
                    //_centerService.SaveChanges(center);
                    _centerService.SaveChanges();
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                success = true;
                message = String.Format("Successfully {0} Call.", ((isCallExclude) ? "Excluded" : "Included"));
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = "", Message = message }), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTimeZoneNotification(int centerId, int timeZone)
        {
            var success = false;
            var message = string.Empty;

            try
            {
                var uvm = GetCurrentUserViewModel();
                var center = _centerService.GetCenterById(centerId);

                if (center != null && center.SubBrandID == uvm.SubBrandId && CanEditNotification(uvm.IsAdmin))
                {
                    string userName = string.Empty;
                    if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                    {
                        var UserIdentity = (ClaimsIdentity)User.Identity;
                        userName = UserIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    }
                    center.TimeZoneOffset = timeZone;
                    center.LastModifiedDate = DateTime.Now;
                    center.LastModifiedUser = userName;
                    //_centerService.SaveChanges(center);
                    _centerService.SaveChanges();
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                success = true;
                message = "Successfully Updated Time Zone.";
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = "", Message = message }), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSubBrand_Email(bool isEmailEnabled, string emailSendTime)
        {
            var success = false;
            var keyId = string.Empty;
            var message = string.Empty;
            var subBrandId = GetCurrentSubBrandId();
            DateTime dt;
            
            try
            {
                var uvm = GetCurrentUserViewModel();
                if (!uvm.AvailableSubBrands.Any(x => x.Key == subBrandId))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var brand = _brandService.GetSubBrandById(subBrandId);
                if (brand != null && brand.SubBrandID == uvm.SubBrandId && CanEditNotification(uvm.IsAdmin))
                {
                    DateTime.TryParseExact(emailSendTime, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    brand.EmailSendTime = dt.TimeOfDay;
                    brand.EmailEnabled = isEmailEnabled;
                    //brand.LastModifiedDate = DateTime.Now;
                    //brand.LastModifiedUser = User.Identity.Name;
                    //_brandService.SaveChangesSubBrand(brand);
                    _brandService.SaveChanges();
                }

                success = true;
                message = (isEmailEnabled)
                    ? String.Format("Successfully {0} Email @ {1}.", ((isEmailEnabled) ? "Enabled" : "Disabled"), emailSendTime)
                    : String.Format("Successfully {0} Email.", ((isEmailEnabled) ? "Enabled" : "Disabled"));
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = "", Message = message }), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSubBrand_Call(bool isCallEnabled, string callSendTime)
        {
            var success = false;
            var keyId = string.Empty;
            var message = string.Empty;
            var subBrandId = GetCurrentSubBrandId();
            DateTime dt;
            
            try
            {
                var uvm = GetCurrentUserViewModel();
                if (!uvm.AvailableSubBrands.Any(x => x.Key == subBrandId))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                    return Json(mc_ExceptionMessage_NoAccess, JsonRequestBehavior.AllowGet);
                }

                var brand = _brandService.GetSubBrandById(subBrandId);
                if (brand != null && brand.SubBrandID == uvm.SubBrandId && CanEditNotification(uvm.IsAdmin))
                {
                    DateTime.TryParseExact(callSendTime, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    brand.CallEnabled = isCallEnabled;
                    brand.CallSendTime = dt.TimeOfDay;
                    //brand.LastModifiedDate = DateTime.Now;
                    //brand.LastModifiedUser = User.Identity.Name;
                    //_brandService.SaveChangesSubBrand(brand);
                    _brandService.SaveChanges();
                }

                success = true;
                message = (isCallEnabled)
                    ? String.Format("Successfully {0} Call @ {1}.", ((isCallEnabled) ? "Enabled" : "Disabled"), callSendTime)
                    : String.Format("Successfully {0} Call.", ((isCallEnabled) ? "Enabled" : "Disabled"));
            }
            catch (Exception ex)
            {
                success = false;
                _log.Error(FormatException(ex));
                message = mc_ExceptionMessage_Error;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = "", Message = message }), JsonRequestBehavior.AllowGet);
        }


        private string TimespanToString(TimeSpan time)
        {
            DateTime tempDate = DateTime.Today.Add(time);
            string displayTime = tempDate.ToString("h:mm tt");
            return displayTime;
        }


        private bool CanEditNotification(bool isAdmin)
        {
            return (isAdmin);
        }


        private string StripHtmlWrapper(string s)
        {
            return (!String.IsNullOrEmpty(s)) ? s.Replace("<p>", "").Replace("</p>", "").Replace("\n", "").Replace("&nbsp;", " ").Trim() : s;
        }


        private bool ValidateRecipients(string recipients)
        {
            var validator = new EmailAddressExAttribute();
            return validator.IsValid(recipients);
        }

    }
}