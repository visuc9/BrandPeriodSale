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
    public class Notification2Controller : BaseController
    {
        private const string mc_ExceptionMessage_Error = "Something went wrong.";
        private const string mc_ExceptionMessage_NoAccess = "You are not allowed to perform this operation.";

        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private INotificationService _notificationService;

        public Notification2Controller() : this(new NotificationService()) { }

        public Notification2Controller(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        public ActionResult Index()
        {
            var uvm = GetCurrentUserViewModel();
            var subBrandId = GetCurrentSubBrandId();
            var subBrandName = uvm.AvailableSubBrands.Where(x => x.Key == subBrandId).Select(r => r.Value).FirstOrDefault();

            // TODO: Read service values into model
            var model = new ListNotification2ViewModel()
            {
                SubBrandId = subBrandId,
                SubBrandName = subBrandName,
                
                EmailEnabled = true,
                EmailTime = "08:00",
                AvailableEmailTimes = (from m in GetTimes() select new SelectListItem() { Text = m, Value = m }).ToList(),
                
                CallEnabled = true,
                CallTime = "10:00",
                AvailableCallTimes = (from m in GetTimes() select new SelectListItem() { Text = m, Value = m }).ToList()
            };

            return View(model);
        }


        [HttpGet]
        public ActionResult GetNotifications()
        {
            try
            {
                var uvm = GetCurrentUserViewModel();

                // TODO: Read service values into model
                var model = new List<Notification2ViewModel>();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(FormatException(ex));
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return Json(mc_ExceptionMessage_Error, JsonRequestBehavior.AllowGet);
            }
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
            var model = new EditLevelNotification2ViewModel()
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
        public ActionResult EditLevelNotification(EditLevelNotification2ViewModel model)
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

            var model = new EditBrandNotification2ViewModel() 
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
        public ActionResult EditBrandNotification(EditBrandNotification2ViewModel model)
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


        private List<string> GetTimes()
        {
            var results = new List<string>();
            for (int i = 0; i <= 23; i++)
            {
                results.Add(String.Format("{0:D2}:00", i));
            }

            return results;
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
