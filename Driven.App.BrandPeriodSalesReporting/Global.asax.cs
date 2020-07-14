using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using log4net;
using Driven.App.BrandPeriodSalesReporting.Models;
using System.Web.Helpers;
using System.IdentityModel.Claims;
using System.Security.Claims;

namespace Driven.App.BrandPeriodSalesReporting
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Log4NetConfig.RegisterLog4net();
            AutoMapperConfig.RegisterAutoMapper();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
        }


        string FormatException(Exception ex)
        {
            string userName = string.Empty;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var UserIdentity = (ClaimsIdentity)User.Identity;
                userName = UserIdentity.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            }

            //string usr = (User != null && User.Identity != null && User.Identity.IsAuthenticated) ? User.Identity.Name : string.Empty;
            string msg = (ex.InnerException != null) ? ex.Message + "; " + ex.InnerException.Message : ex.Message;
            string trace = (ex.StackTrace != null) ? ex.StackTrace : string.Empty;
            return String.Format("{0} - {1} - {2}", userName, msg, trace);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
        }

        protected void Application_EndRequest()
        {
            if (Context.Items[AjaxAuthorizeStatusCode.PermissionDenied] is bool)
            {
                Context.Response.StatusCode = (int)System.Net.HttpStatusCode.PreconditionFailed;
                Context.Response.End();
            }
        }


        void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                Exception ex = Server.GetLastError().GetBaseException();

                if (ex is HttpAntiForgeryException)
                {
                    log.Warn(FormatException(ex));
                    Response.Clear();
                    Server.ClearError();
                    Response.Redirect("~/Home/ErrorAntiforgery", true);
                }
                else if (ex is HttpException && (((HttpException)(ex)).GetHttpCode() == 404))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.Redirect("~/Home/ErrorNotFound", true);
                }
                else
                {
                    log.Error(FormatException(ex));
                    Server.ClearError();
                    Response.Redirect("~/Home/Error");
                }
            }
            catch(Exception ex)
            {
                log.Error(FormatException(ex));
                Response.Redirect("~/Home/Error");
            }
        }


        // ** FORCE SHUTDOWN (alternate to app_offline.htm) ** 
        //void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    if ((bool)Application["SiteOpenService"] == false)
        //    {
        //        if (!Request.IsLocal)
        //        {
        //            HttpContext.Current.RewritePath("/Site_Maintenance.htm");
        //        }
        //    }
        //}

    }
}
