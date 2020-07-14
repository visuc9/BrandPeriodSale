using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class SalesCorrectionListViewModel
    {
        public SalesCorrectionListViewModel()
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

        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
    }


    public class ToplineCorrectionViewModel
    {
        public int RequestId { get; set; }
        public int ToplineId { get; set; }
        
        public string StoreId { get; set; }
        public string Status { get; set; }
        public string PeriodEndDate { get; set; }
        
        public string ActiveSales { get; set; }
        public string Sales { get; set; }

        public string ActiveTickets { get; set; }
        public string Tickets { get; set; }

        public bool CanEdit { get; set; }
        public bool CanApprove { get; set; }
    }


    public class ApproveToplineCorrectionViewModel
    {
        public ApproveToplineCorrectionViewModel()
        {
            Denials = new List<System.Web.Mvc.SelectListItem>();
            ApproveStatusIds = new List<System.Web.Mvc.SelectListItem>();
        }

        [Display(Name = "Store - Reporting Period")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int ApproveStatus { get; set; }
        public IList<System.Web.Mvc.SelectListItem> ApproveStatusIds { get; set; }

        [Required]
        [Display(Name = "Reason for Denial")]
        public string Denial { get; set; }
        public IList<System.Web.Mvc.SelectListItem> Denials { get; set; }

        [Required]
        [Display(Name = "Denial Description")]
        [StringLength(500)]
        public string DenialDescription { get; set; }

        public int RequestId { get; set; }
        public int ToplineId { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public string LocalStoreId { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
    }


    public class CreateToplineCorrectionViewModel
    {
        public CreateToplineCorrectionViewModel()
        {
            CorrectionTypes = new List<System.Web.Mvc.SelectListItem>();
            ProductGroups = new List<ToplineCorrectionProductGroupViewModel>();
            LocalStoreIds = new List<System.Web.Mvc.SelectListItem>();
        }

        [Required]
        [Display(Name = "Reason")]
        [StringLength(500)]
        public string Reason { get; set; }

        [Required]
        [Display(Name = "Correction Type")]
        public int CorrectionType { get; set; }
        public IList<System.Web.Mvc.SelectListItem> CorrectionTypes { get; set; }

        [Required]
        [Display(Name = "Store # ")]
        public string LocalStoreId { get; set; }
        public IList<System.Web.Mvc.SelectListItem> LocalStoreIds { get; set; }

        [Display(Name = "Active Sales")]
        public string NetSales { get; set; }

        [Display(Name = "Active Tickets")]
        public string TotalTickets { get; set; }

        [Required(ErrorMessage = "Sales required")]
        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        [NetSalesValidation(ErrorMessage = "Total Sales must equal sum of Detail Sales")]
        [Display(Name = "Sales")]
        public string NetSalesCorrection { get; set; }

        [Required(ErrorMessage = "Tickets required")]
        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        [Display(Name = "Tickets")]
        public string TotalTicketsCorrection { get; set; }

        [Display(Name = "Product Sub Group")]
        public List<ToplineCorrectionProductGroupViewModel> ProductGroups { get; set; }

        [Display(Name = "Sales Type: ")]
        public int SalesTypeId { get; set; }

        [Display(Name = "Sales Type: ")]
        public string SalesTypeDescription { get; set; }
        public int RequestId { get; set; }
        public int ToplineId { get; set; }
        public int StatusId { get; set; }
        public string Title { get; set; }

        public DateTime PeriodEndDate { get; set; }
    }


    public class EditToplineCorrectionViewModel
    {
        public EditToplineCorrectionViewModel()
        {
            CorrectionTypes = new List<System.Web.Mvc.SelectListItem>();
            ProductGroups = new List<ToplineCorrectionProductGroupViewModel>();
        }

        [Required]
        [Display(Name = "Reason")]
        [StringLength(500)]
        public string Reason { get; set; }

        [Required]
        [Display(Name = "Correction Type")]
        public int CorrectionType { get; set; }
        public IList<System.Web.Mvc.SelectListItem> CorrectionTypes { get; set; }

        [Display(Name = "Active Sales")]
        public string NetSales { get; set; }

        [Display(Name = "Active Tickets")]
        public string TotalTickets { get; set; }

        [Required(ErrorMessage = "Sales required")]
        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        [NetSalesValidation(ErrorMessage = "Total Sales must equal sum of Detail Sales")]
        [Display(Name = "Sales")]
        public string NetSalesCorrection { get; set; }

        [Required(ErrorMessage = "Tickets required")]
        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        [Display(Name = "Tickets")]
        public string TotalTicketsCorrection { get; set; }

        [Display(Name = "Product Sub Group")]
        public List<ToplineCorrectionProductGroupViewModel> ProductGroups { get; set; }

        [Display(Name = "Store # ")]
        public string LocalStoreId { get; set; }

        [Display(Name = "Sales Type: ")]
        public int SalesTypeId { get; set; }

        [Display(Name = "Sales Type: ")]
        public string SalesTypeDescription { get; set; }
        public int RequestId { get; set; }
        public int ToplineId { get; set; }
        public int StatusId { get; set; }
        public string Title { get; set; }
    }


    public class ToplineCorrectionProductGroupViewModel
    {
        public int ProdGrpID { get; set; }
        public int ProdSubGrpID { get; set; }
        public string Name { get; set; }

        public int? BPSR_ToplineID { get; set; }
        public int? BPSR_ProdGrpID { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        public string NetSales { get; set; }

        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        public string TotalTickets { get; set; }

        [RegularExpression(FormatHelpers.Money_Regex, ErrorMessage = FormatHelpers.Money_ErrorMessage)]
        public string NetSalesCorrection { get; set; }

        [RegularExpression(FormatHelpers.Number_Regex, ErrorMessage = FormatHelpers.Number_ErrorMessage)]
        public string TotalTicketsCorrection { get; set; }

    }

}
