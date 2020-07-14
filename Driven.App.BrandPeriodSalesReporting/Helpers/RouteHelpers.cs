using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class RouteHelpers
    {
        public static bool IsRoute(System.Web.Routing.RouteValueDictionary values, string controller)
        {
            return IsRoute(values, controller, null);
        }


        public static bool IsRoute(System.Web.Routing.RouteValueDictionary values, string controller, string action)
        {
            try
            {
                if (controller != null && action != null)
                {
                    return (values["controller"].ToString().ToUpper() == controller.ToUpper() && values["action"].ToString().ToUpper() == action.ToUpper());
                }
                else
                {
                    return (values["controller"].ToString().ToUpper() == controller.ToUpper());
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
