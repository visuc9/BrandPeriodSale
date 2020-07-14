using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;
using System.Security.Claims;

namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }


        public string FormatException(Exception ex)
        {

            string userName = string.Empty;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var UserIdentity = (ClaimsIdentity)User.Identity;
                userName = UserIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            }
            string msg = (ex.InnerException != null) ? ex.Message + "; " + ex.InnerException.Message : ex.Message;
            string trace = (ex.StackTrace != null) ? ex.StackTrace : string.Empty;
            return String.Format("{0} - {1} - {2}", userName, msg, trace);
        }


        public string BuildDialogResult(bool success, string value, string message)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = success, Value = value, Message = message });
        }


        public string BuildDialogResult(bool success, string message)
        {
            return BuildDialogResult(success, null, message);
        }

        
        public int GetCurrentSubBrandId()
        {
            int defaultSubBrandId = 0;
            var model = GetCurrentUserViewModel();
            return (model != null) ? (model.SubBrandId ?? defaultSubBrandId) : defaultSubBrandId;
        }

        
        public UserViewModel GetCurrentUserViewModel()
        {
            return SecurityHelpers.GetUserViewModel();
        }

    }
}
