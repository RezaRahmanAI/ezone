using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductWiseBenefitModel
    {
        public string Code { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string IMENO { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal NetSales { get; set; }

        public decimal PurchaseTotal { get; set; }
        public decimal CommisionProfit { get; set; }
        public decimal HireProfit { get; set; }
        public decimal HireCollection { get; set; }
        public decimal TotalProfit { get; set; }
        public DateTime SalesDate { get; set; }
        public decimal Quantity { get; set; }
        public int CustomerID { get; set; }

    }
}
