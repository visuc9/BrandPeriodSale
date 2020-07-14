using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class ConvertHelpers
    {
        public static decimal? ToMoney(string s)
        {
            try
            {
                return decimal.Parse(new string(s.Where(c => char.IsNumber(c) || c == '.' || c == '+' || c == '-').ToArray()));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static int? ToNumber(string s)
        {
            try
            {
                return int.Parse(new string(s.Where(c => char.IsNumber(c) || c == '.' || c == '+' || c == '-').ToArray()));
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
