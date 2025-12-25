using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
   public class SupplierLedgerReportModel
    {
        public DateTime Date { get; set; }
        public string ChallanNo { get; set; }
        public decimal OpeningDue { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RecAmount { get; set; }
        public decimal AdjustAmt { get; set; }
        public decimal PaymentDue { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CashDelivery { get; set; }
        public decimal ClosingDue { get; set; }
    }
}
