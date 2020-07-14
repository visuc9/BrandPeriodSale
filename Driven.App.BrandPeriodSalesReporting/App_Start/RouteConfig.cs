using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Driven.App.BrandPeriodSalesReporting
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.AppendTrailingSlash = true;
            //routes.LowercaseUrls = true;
            routes.IgnoreRoute("favicon.ico");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: "Home", url: "Home", defaults: new { controller = "BrandPeriodSales", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                defaults: new { controller = "BrandPeriodSales", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
