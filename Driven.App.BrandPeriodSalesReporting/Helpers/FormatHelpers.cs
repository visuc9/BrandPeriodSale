using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class FormatHelpers
    {
        public const string Money_Regex = @"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$";
        public const string Money_ErrorMessage = "Currency only";

        public const string Number_Regex = @"^(\d+|\d{1,3}(,\d{3})*)?$"; //
        public const string Number_ErrorMessage = "Numbers only";

        public const string Integer_Regex = @"^[1-9]\d*$";
        public const string Integer_ErrorMessage = "Positive digits only";


        public static string FormatPeriodDate(DateTime dt)
        {
            string result = string.Empty;

            try
            {
                result = dt.ToString("MM/dd/yyyy");
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }


        public static string FormatStatus(int statusId)
        {
            string result = string.Empty;

            try
            {
                switch (statusId)
                {
                    case 1:
                        result = "Not Started";
                        break;

                    case 2:
                        result = "In Progress";
                        break;

                    case 3:
                        result = "For Approval";
                        break;

                    case 4:
                        result = "Approved";
                        break;
                    
                    case 5:
                        result = "Correction Pending";
                        break;

                    case 6:
                        result = "Correction Approved";
                        break;
                        
                    case 7:
                        result = "Correction Denied";
                        break;

                    case 8:
                        result = "Correction Closed";
                        break;
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }

        public static string FormatSageRecordType(int id)
        {
            var result = string.Empty;

            if (id == 1)
            {
                result = "Franchise Fee";
            }
            else if (id == 2)
            {
                result = "Advertising Fee";
            }

            return result;
        }

        public static string FormatNumber(int? n)
        {
            string result = string.Empty;

            try
            {
                result = string.Format("{0:n0}", n);
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }


        public static string FormatMoney(decimal? m)
        {
            string result = string.Empty;

            try
            {
                result = String.Format("{0:C}", m);
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }

        public static string FormatSalesType(int salesTypeId)
        {
            string result = string.Empty;

            try
            {
                switch (salesTypeId)
                {
                    case 1:
                        result = "Non-Fleet";
                        break;

                    case 2:
                        result = "Fleet";
                        break;

                    case 3:
                        result = "Internal";
                        break;
                        
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }
    }
}
