using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class CorrectionTypesViewModel
    {
        public int CorrectionTypeId { get; set; }
        public string CorrectionTypeCodeId { get; set; }
        public string CorrectionTypeDescription { get; set; }
    }


    public class CorrectionTypeListViewModel
    {
    }


    public class CreateCorrectionTypeViewModel
    {
        [RegularExpression(FormatHelpers.Integer_Regex, ErrorMessage = FormatHelpers.Integer_ErrorMessage)]
        [Required(ErrorMessage = "Correction Type Id required")]
        [Display(Name = "Correction Type Id")]
        public string CorrectionTypeId { get; set; }

        [StringLength(25)]
        [Required(ErrorMessage = "Correction Type Code required")]
        [Display(Name = "Correction Type Code")]
        public string CorrectionTypeCodeId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Correction Type Description required")]
        [Display(Name = "Correction Type Description")]
        public string CorrectionTypeDescription { get; set; }
    }


    public class EditCorrectionTypeViewModel
    {
        [Display(Name = "Correction Type Id")]
        public int CorrectionTypeId { get; set; }

        [Display(Name = "Correction Type Code")]
        public string CorrectionTypeCodeId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Correction Type Description required")]
        [Display(Name = "Correction Type Description")]
        public string CorrectionTypeDescription { get; set; }
    }

}
