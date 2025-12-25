using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
   public class DistributorAnalysis
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal StockQty{ get; set; }
        public decimal StockValue { get; set; }
        public decimal RetailQty { get; set; }
        public decimal RetailValue { get; set; }
        public decimal HireQty { get; set; }
        public decimal HireValue { get; set; }
        public decimal DealerQty { get; set; }
        public decimal DealerValue { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalValue { get; set; }
        public int PCategoryID { get; set; }
        public object PCategoryName { get; set; }
    }
}
