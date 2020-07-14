using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Driven.App.BrandPeriodSalesReporting.Models;

namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class PlaceholderHelpers
    {
        public enum PlaceholderFormat { Unset = 0, FmtString = 1, FmtDate = 2, FmtMoney = 3, FmtNumber = 4, FmtDecimal = 5 };

        public static string ApplyPlaceholders(string message, List<PlaceholderValue_Result> placeholders)
        {
            var result = message;

            foreach (var p in placeholders)
            {
                var oldValue = String.Format("[[{0}]]", p.Name);
                var newValue = FormatPlaceholderValue(p.Value, (PlaceholderFormat)p.Format);
                message = message.Replace(oldValue, newValue);
            }

            return message;
        }


        private static string FormatPlaceholderValue(string value, PlaceholderFormat fmt)
        {
            var result = value;

            switch (fmt)
            {
                case PlaceholderFormat.FmtDate:
                    result = (Convert.ToDateTime(value)).ToString("MM/dd/yyyy");
                    break;

                case PlaceholderFormat.FmtMoney:
                    result = string.Format("{0:C}", Convert.ToDecimal(value));
                    break;

                case PlaceholderFormat.FmtNumber:
                    result = string.Format("{0:n0}", Convert.ToInt32(value));
                    break;

                case PlaceholderFormat.FmtDecimal:
                    result = string.Format("{0:0.##}", Convert.ToDecimal(value));
                    break;
            }

            return result;
        }
    }
}
