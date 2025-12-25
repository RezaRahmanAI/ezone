using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
   public class CustomerDueReportModel
    {
        public DateTime TransDate { get; set; }
        public int CustomerID { get; set; }
        public int ConcernID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal DueSales { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal TotalSalesAmt { get; set; }
        public decimal RecAmount { get; set; }
        public decimal CollectionAmt { get; set; }
        public string Status { get; set; }
        public decimal Balance { get; set; }
        public decimal AdjustAmt { get; set; }
        public decimal ReturnAmt { get; set; }
        public string InstallmentPeriod { get; set; }

    }
}
