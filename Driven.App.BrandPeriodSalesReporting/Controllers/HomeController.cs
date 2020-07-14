using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Configuration;

using Driven.App.BrandPeriodSalesReporting.Models;
using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Controllers
{
    public class HomeController : BaseController
    {
        private const string mc_Configuration_Version = "Version";
        private const string mc_Configuration_Configuration = "Configuration";

        private const string mc_Configuration_EmailFrom = "EmailFrom";
        private const string mc_Configuration_EmailDebug = "EmailDebug";
        private const string mc_Configuration_EmailEnabled = "EmailEnabled";
        private const string mc_Configuration_EmailForceTo = "EmailForceTo";
        private const string mc_Configuration_EmailSmtpHost = "EmailSmtpHost";
        private const string mc_Configuration_EmailSmtpUser = "EmailSmtpUser";
        private const string mc_Configuration_EmailSmtpPass = "EmailSmtpPass";

        private const string mc_Configuration_AuthFake = "AuthFake";
        private const string mc_Configuration_AuthFakeGroups = "AuthFakeGroups";
        private const string mc_Configuration_AuthDomainName = "AuthDomainName";
        private const string mc_Configuration_AuthExpireTimeSpan = "AuthExpireTimeSpan";


        public static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ActionResult Index()
        {
            return RedirectToAction("", "BrandPeriodSales");
        }


        private string FormatSubBrandList(Dictionary<int, string> subBrands)
        {
            var results = new List<string>();

            foreach (KeyValuePair<int, string> entry in subBrands)
            {
                results.Add(String.Format("({0}) {1}", entry.Key, entry.Value));
            }
            
            return String.Join(",", results);
        }


        public ActionResult About()
        {
            var uvm = GetCurrentUserViewModel();

            var currentSubBrandId = GetCurrentSubBrandId();
            var currentSubBrand = uvm.AvailableSubBrands.Where(x => x.Key == currentSubBrandId).FirstOrDefault();

            string currentSubBrandName = string.Empty;
            if (!currentSubBrand.Equals(default(KeyValuePair<int, string>)))
            {
                currentSubBrandName = currentSubBrand.Value;
            }

            var userGroups = SecurityHelpers.GetUserGroups();
            var approverGroups = SecurityHelpers.GetSubBrandApproverGroups(currentSubBrandId);
            
            var approverEmails = new List<string>();
            if (uvm.UserType != "FZ")
            {
                foreach (var groupName in approverGroups)
                {
                    approverEmails = approverEmails.Union(SecurityHelpers.GetGroupEmailAddresses(groupName)).ToList();
                }
            }

            var model = new AboutViewModel() 
            { 
                UserName = uvm.Name,
                UserDisplayName = uvm.DisplayName,
                UserEmailAddress = uvm.EmailAddress, 
                UserIsAdmin = uvm.IsAdmin.ToString(), 
                UserIsApprover = uvm.IsApprover.ToString(),
                UserIsReportsAvailable = uvm.IsReportsAvailable.ToString(), 
                
                UserCurrentSubBrandId = currentSubBrandId.ToString(), 
                UserCurrentSubBrandName = currentSubBrandName,
                UserAvailableSubBrands = FormatSubBrandList(uvm.AvailableSubBrands),

                UserGroups = String.Join(", ", userGroups),
                UserApproverGroups = String.Join(", ", approverGroups),
                UserApproverMembers = String.Join(", ", approverEmails),

                AuthFake = ConfigurationManager.AppSettings[mc_Configuration_AuthFake] ?? "",
                AuthFakeGroups = ConfigurationManager.AppSettings[mc_Configuration_AuthFakeGroups] ?? "",
                AuthDomainName = ConfigurationManager.AppSettings[mc_Configuration_AuthDomainName] ?? "",
                AuthExpireTimeSpan = ConfigurationManager.AppSettings[mc_Configuration_AuthExpireTimeSpan] ?? "",

                Version = ConfigurationManager.AppSettings[mc_Configuration_Version] ?? "",
                Configuration = ConfigurationManager.AppSettings[mc_Configuration_Configuration] ?? "",
                DebugMode = HttpContext.IsDebuggingEnabled,

                EmailDebug = ConfigurationManager.AppSettings[mc_Configuration_EmailDebug] ?? "",
                EmailEnabled = ConfigurationManager.AppSettings[mc_Configuration_EmailEnabled] ?? "",
                EmailFrom = ConfigurationManager.AppSettings[mc_Configuration_EmailFrom] ?? "",
                EmailForceTo = ConfigurationManager.AppSettings[mc_Configuration_EmailForceTo] ?? "",
                EmailSmtpHost = ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpHost] ?? "",
                EmailSmtpUser = ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpUser] ?? "",
                EmailSmtpPass = ((ConfigurationManager.AppSettings[mc_Configuration_EmailSmtpPass] ?? "").Length > 0) ? "****" : ""
            };

            return View("About", model);
        }


        [AllowAnonymous]
        public ActionResult ThrowSomethingBad()
        {
            //try
            //{
                throw new Exception("Test something bad.");
            //}
            //catch (Exception ex)
            //{
            //    _log.Error(FormatException(ex));
            //    throw ex;
            //}
        }

        
        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ErrorAntiforgery()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ErrorNotFound()
        {
            return View();
        }

    }
}
