using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class BpsrRoles
    {
        public const string Admin = "ADMIN";
        public const string Approver = "APPROVER";
    }

    public class BpsrAuthorizeAttribute : AuthorizeAttribute
    {
        public string Roles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                // check base permissions
                if (!base.AuthorizeCore(httpContext))
                    return false;

                // check no permissions
                if (Roles == "")
                    return true;

                var uvm = SecurityHelpers.GetUserViewModel();

                // check admin - admin can do anything
                if (uvm.IsAdmin)
                    return true;

                // required roles
                var requiredRoles = Roles.Split(',').Select(s => s.Trim().ToUpper()).ToList();

                // check approver
                var requireApprover = requiredRoles.Contains(BpsrRoles.Approver);
                if (requireApprover && uvm.IsApprover)
                    return true;
                
                // default reject
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
