using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EmailAddressExAttribute : DataTypeAttribute
    {
        private readonly EmailAddressAttribute _emailAddressAttribute = new EmailAddressAttribute();

        public EmailAddressExAttribute() : base(DataType.EmailAddress) { }

        public override bool IsValid(object value)
        {
            var emailAddr = Convert.ToString(value);
            if (string.IsNullOrWhiteSpace(emailAddr)) return false;

            var emails = emailAddr.Split(new[] { ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return emails.All(t => _emailAddressAttribute.IsValid(t));
        }

    }
}
