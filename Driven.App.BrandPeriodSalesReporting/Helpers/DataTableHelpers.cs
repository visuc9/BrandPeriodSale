using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Driven.App.BrandPeriodSalesReporting.Models;

namespace Driven.App.BrandPeriodSalesReporting.Helpers
{
    public static class DataTableHelpers
    {
        public static DataTableResult BuildDataTableResult(int draw, int recordsTotal, int recordsFiltered, List<object> data)
        {
            return new DataTableResult()
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }
    }
}
