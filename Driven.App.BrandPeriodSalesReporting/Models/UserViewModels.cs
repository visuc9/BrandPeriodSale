using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
        public bool IsReportsAvailable { get; set; }
        public int? SubBrandId { get; set; }
        public Dictionary<int, string> AvailableSubBrands { get; set; }
        public string UserType { get; set; }
        public List<string> UserAuthStores { get; set; }
    }


    public class AboutViewModel
    {
        public bool DebugMode { get; set; }
        public string Version { get; set; }
        public string Configuration { get; set; }

        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserGroups { get; set; }
        public string UserIsAdmin { get; set; }
        public string UserIsApprover { get; set; }
        public string UserIsReportsAvailable { get; set; }
        public string UserCurrentSubBrandId { get; set; }
        public string UserCurrentSubBrandName { get; set; }
        public string UserAvailableSubBrands { get; set; }

        public string UserApproverGroups { get; set; }
        public string UserApproverMembers { get; set; }
        
        public string EmailDebug { get; set; }
        public string EmailEnabled { get; set; }
        public string EmailFrom { get; set; }
        public string EmailForceTo { get; set; }
        public string EmailSmtpHost { get; set; }
        public string EmailSmtpUser { get; set; }
        public string EmailSmtpPass { get; set; }

        public string AuthFake { get; set; }
        public string AuthFakeGroups { get; set; }
        public string AuthDomainName { get; set; }
        public string AuthExpireTimeSpan { get; set; }
    }

}
