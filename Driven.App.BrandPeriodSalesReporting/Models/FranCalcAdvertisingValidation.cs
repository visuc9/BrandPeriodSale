using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class FranCalcAdvertisingValidation : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance.GetType() == typeof(Models.EditToplineViewModel))
            {
                var model = (Models.EditToplineViewModel)validationContext.ObjectInstance;
                var totalFranCalcAdvertising = ConvertHelpers.ToMoney(model.FranCalcAdvertising) ?? 0;

                decimal calcFranCalcAdvertising = 0;
                foreach (var current in model.ProductGroups)
                {
                    calcFranCalcAdvertising += ConvertHelpers.ToMoney(current.FranCalcAdvertising) ?? 0;
                }

                if ((calcFranCalcAdvertising != 0) && (calcFranCalcAdvertising != totalFranCalcAdvertising))
                {
                    //return new ValidationResult("Total Calc. Advertising Fees must equal sum of Detail Calc. Advertising Fees");
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "francalcadvertising"
            };
        }
    }
}
