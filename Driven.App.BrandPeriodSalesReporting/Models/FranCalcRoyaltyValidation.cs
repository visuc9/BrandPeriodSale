using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Driven.App.BrandPeriodSalesReporting.Helpers;


namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class FranCalcRoyaltyValidation : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance.GetType() == typeof(Models.EditToplineViewModel))
            {
                var model = (Models.EditToplineViewModel)validationContext.ObjectInstance;
                var totalFranCalcRoyalty = ConvertHelpers.ToMoney(model.FranCalcRoyalty) ?? 0;

                decimal calcFranCalcRoyalty = 0;
                foreach (var current in model.ProductGroups)
                {
                    calcFranCalcRoyalty += ConvertHelpers.ToMoney(current.FranCalcRoyalty) ?? 0;
                }

                if ((calcFranCalcRoyalty != 0) && (calcFranCalcRoyalty != totalFranCalcRoyalty))
                {
                    //return new ValidationResult("Total Calc. Franchise Fees must equal sum of Detail Calc. Franchise Fees");
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "francalcroyalty"
            };
        }
    }
}
