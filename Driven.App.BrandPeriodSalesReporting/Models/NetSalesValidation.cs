using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class NetSalesValidation : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance.GetType() == typeof(Models.EditToplineViewModel))
            {
                var model = (Models.EditToplineViewModel)validationContext.ObjectInstance;
                var totalSales = ConvertHelpers.ToMoney(model.NetSales) ?? 0;

                decimal calcTotalSales = 0;
                foreach (var current in model.ProductGroups)
                {
                    calcTotalSales += ConvertHelpers.ToMoney(current.NetSales) ?? 0;
                }

                if ((calcTotalSales != 0) && (calcTotalSales != totalSales))
                {
                    //return new ValidationResult("Total Sales must equal sum of Detail Sales");
                }
            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "netsales"
            };
        }

    }
}
