using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class ToplineProductGroupViewModel
    {
        public int ProdGrpID { get; set; }
        public int ProdSubGrpID { get; set; }
        public string Name { get; set; }

        public int? BPSR_ToplineID { get; set; }
        public int? BPSR_ProdGrpID { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        public string NetSales { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        public string FranCalcRoyalty { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        public string FranCalcAdvertising { get; set; }

        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        public string TotalTickets { get; set; }

        public int SalesTypeId { get; set; }
    }


    public class EditToplineViewModel
    {
        public EditToplineViewModel()
        {
            ProductGroups = new List<ToplineProductGroupViewModel>();
        }

        [Required(ErrorMessage = "Sales required")]
        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        [NetSalesValidation(ErrorMessage = "Total Sales must equal sum of Detail Sales")]
        [Display(Name = "Sales")]
        public string NetSales { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        [FranCalcRoyaltyValidation(ErrorMessage = "Total Calc. Franchise Fees must equal sum of Detail Calc. Franchise Fees")]
        [Display(Name = "Calc. Franchise Fees")]
        public string FranCalcRoyalty { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        [FranCalcAdvertisingValidation(ErrorMessage = "Total Calc. Advertising Fees must equal sum of Detail Calc. Advertising Fees")]
        [Display(Name = "Calc. Advertising Fees")]
        public string FranCalcAdvertising { get; set; }

        [Required(ErrorMessage = "Tickets required")]
        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        [Display(Name = "Tickets")]
        public string TotalTickets { get; set; }

        [Display(Name = "Product Sub Group")]
        public List<ToplineProductGroupViewModel> ProductGroups { get; set; }


        [Display(Name = "Sales Type: ")]
        public int SalesTypeId { get; set; }
        public string SalesTypeCode { get; set; }
        public IList<System.Web.Mvc.SelectListItem> SalesTypeList { get; set; }

        public int ToplineId { get; set; }
        public int StatusId { get; set; }
        public string LocalStoreId { get; set; }
        public string Title { get; set; }

        public bool IsApprove { get; set; }
        public bool CanApprove { get; set; } 
    }


    public class ToplineViewModel
    {
        public int ToplineId { get; set; }
        public string StoreId { get; set; }
        public string Status { get; set; }
        public string PeriodEndDate { get; set; }
        public string NetSales { get; set; }
        public string FranCalcRoyalty { get; set; }
        public string FranCalcAdvertising { get; set; }
        public string TotalTickets { get; set; }
        public string SalesTypeCode { get; set; }

        public bool IsPastDue { get; set; }
        public bool CanEdit { get; set; }
        public bool CanApprove { get; set; }
        public bool CanSubmit { get; set; }
    }


    public class ToplineListViewModel
    {
        public ToplineListViewModel()
        {
            StoreIds = new List<System.Web.Mvc.SelectListItem>();
            StatusIds = new List<System.Web.Mvc.SelectListItem>();
            PeriodEndDates = new List<System.Web.Mvc.SelectListItem>();
        }

        [Display(Name = "Store # ")]
        public string StoreId { get; set; }
        public IList<System.Web.Mvc.SelectListItem> StoreIds { get; set; }

        [Display(Name = "Status: ")]
        public string StatusId { get; set; }
        public IList<System.Web.Mvc.SelectListItem> StatusIds { get; set; }

        [Display(Name = "Reporting Period: ")]
        public string PeriodEndDate { get; set; }
        public IList<System.Web.Mvc.SelectListItem> PeriodEndDates { get; set; }

        [Display(Name = "Past Due")]
        public bool IsPastDue { get; set; }
        
        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
    }

}
