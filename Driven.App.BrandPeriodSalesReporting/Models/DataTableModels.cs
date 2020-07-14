using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Driven.App.BrandPeriodSalesReporting.Models
{
    public class DataTableResult
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<object> data { get; set; }
    }


    public class DataTableParameters
    {
        public List<DataTableColumn> Columns { get; set; }
        public int Draw { get; set; }
        public int Length { get; set; }
        public List<DataTableOrder> Order { get; set; }
        public DataTableSearch Search { get; set; }
        public int Start { get; set; }
    }


    public class DataTableSearch
    {
        public bool Regex { get; set; }
        public string Value { get; set; }
    }


    public class DataTableColumn
    {
        public int Data { get; set; }
        public string Name { get; set; }
        public bool Orderable { get; set; }
        public bool Searchable { get; set; }
        public DataTableSearch Search { get; set; }
    }


    public class DataTableOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

}
