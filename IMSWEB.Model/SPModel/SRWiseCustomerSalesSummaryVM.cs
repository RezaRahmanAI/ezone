using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class SRWiseCustomerSalesSummaryVM
    {
        //public DateTime SalesDate { get; set; }
        public int EmployeeID{ get; set; }
        public string SRName { get; set; }
        public int ConcernID { get; set; }
        //public int CategoryID { get; set; }
        //public string CategoryName { get; set; }
        public int CustomerID { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public decimal BarUnitPrice { get; set; }
        public decimal SmartUnitPrice { get; set; }
        public decimal BarQuantity { get; set; }

        public decimal SmartQuantity { get; set; }
        public decimal TotalPriceBar{ get; set; }
        public decimal TotalPriceSmart { get; set; }
        public decimal BarAndSmartQty { get; set; }
        public decimal BarAndSmartPrice{ get; set; }
    }
}
