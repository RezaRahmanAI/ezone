using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTCustomerAdjustmentDateWise
    {
        public string ReceiptNo { get; set; }
        public decimal Amount { get; set; }
        //public string TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; } 
        public decimal AdjutmentAmt { get; set; }
    }
}
