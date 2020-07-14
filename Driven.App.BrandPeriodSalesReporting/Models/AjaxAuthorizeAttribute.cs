using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class AjaxAuthorizeStatusCode
    {
        public const string PermissionDenied = "AjaxPermissionDenied";
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            { 
                filterContext.HttpContext.Items[AjaxAuthorizeStatusCode.PermissionDenied] = true; 
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }

}
