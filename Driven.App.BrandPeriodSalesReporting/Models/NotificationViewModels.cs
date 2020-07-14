using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class NotificationViewModel
    {
        public Int64 CenterId { get; set; }
        public string ShopId { get; set; }
        public string LocationStoreId { get; set; }
        public int CenterStatusId { get; set; }
        public string CenterStatusDescription { get; set; }
        public bool ExcludeEmail { get; set; }
        public bool ExcludeCall { get; set; }
        public bool CanEdit { get; set; }
        public int? TimeZone { get; set; }
    }


    public class NotificationListViewModel
    {
        public NotificationListViewModel() 
        {
            StoreStatusList = new List<System.Web.Mvc.SelectListItem>();
            StoreStatusList.Add(new SelectListItem() { Text = "All", Value = "null" });
        }

        public IList<System.Web.Mvc.SelectListItem> StoreStatusList { get; set; }

        [Display(Name = "Status:")]
        public int CenterStatusId { get; set; }

        [Display(Name = "Local Store ID:")]
        public string LocalStoreIdSearchtxt { get; set; }

        public int SubBrandId { get; set; }
        public bool isEmailEnabled { get; set; }
        public string EmailSendTime { get; set; }
        public bool isCallEnabled { get; set; }
        public string CallSendTime { get; set; }
        public bool CanEdit { get; set; }
    }


    public class EditLevelNotificationViewModel
    {
        public int Level { get; set; }
        public int SubBrandId { get; set; }
        public int NotificationId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string EmailSubject { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string EmailBody { get; set; }

        [Required]
        [Display(Name = "Call Text")]
        public string CallText { get; set; }
    }


    public class EditBrandNotificationViewModel
    {
        public int SubBrandId { get; set; }

        [Required]
        [Display(Name = "Recipients")]
        public string Recipients { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string EmailSubject { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string EmailBody { get; set; }
    }

}
