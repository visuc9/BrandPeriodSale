using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Driven.App.BrandPeriodSalesReporting.Helpers;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class DenialTypesViewModel
    {
        public int DenialTypeId { get; set; }
        public string DenialTypeCodeId { get; set; }
        public string DenialTypeDescription { get; set; }
    }


    public class DenialTypeListViewModel
    {
    }


    public class CreateDenialTypeViewModel
    {
        [RegularExpression(FormatHelpers.Integer_Regex, ErrorMessage = FormatHelpers.Integer_ErrorMessage)]
        [Required(ErrorMessage = "Denial Type Id required")]
        [Display(Name = "Denial Type Id")]
        public string DenialTypeId { get; set; }

        [StringLength(25)]
        [Required(ErrorMessage = "Denial Type Code required")]
        [Display(Name = "Denial Type Code")]
        public string DenialTypeCodeId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Denial Type Description required")]
        [Display(Name = "Denial Type Description")]
        public string DenialTypeDescription { get; set; }
    }


    public class EditDenialTypeViewModel
    {
        [Display(Name = "Denial Type Id")]
        public int DenialTypeId { get; set; }

        [Display(Name = "Denial Type Code")]
        public string DenialTypeCodeId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Denial Type Description required")]
        [Display(Name = "Denial Type Description")]
        public string DenialTypeDescription { get; set; }
    }

}
