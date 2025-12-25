using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class CustomerLedgerModel
    {
        public int ConcernID { get; set; }
        public int CustomerID { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string SOrderID { get; set; }
        public decimal Opening { get; set; }
        public decimal CashSales { get; set; }
        public decimal DueSales { get; set; }
        public decimal TotalSalesAmt { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CollectionAmt { get; set; }
        public decimal AdjustAmt { get; set; }
        public decimal Closing { get; set; }
        public int ProductID { get; set; }
        public string  ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProSalesAmt { get; set; }
        public decimal ProductReturnAmt { get; set; }
        public decimal PenaltyInterestAmt { get; set; }
    }
}
