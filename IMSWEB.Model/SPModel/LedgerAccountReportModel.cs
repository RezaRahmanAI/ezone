using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class LedgerAccountReportModel
    {
        public LedgerAccountReportModel()
        {
            ProductList = new List<string>();
        }
        public DateTime Date { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }
        public string InvoiceNo { get; set; }
        public decimal DebitAdj { get; set; }
        public decimal CreditAdj { get; set; }
        public decimal CreditAdjFlatDis { get; set; }
        public decimal CreditAdjTotalPPDis { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal SalesReturn { get; set; }
        public decimal CashCollectionAmt { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public decimal Quantity { get; set; }
        public decimal CInterAmt { get; set; }
        public decimal CrInterestAmount { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal InvoiceDue { get; set; }
        public List<string> ProductList{ get; set; }
        public string EnteredBy { get; set; }
        public string Remarks { get; set; }
        public decimal CashCollectionReturn { get; set; }
        public decimal CashCollectionIntAmt { get ; set; }
        public decimal TotalAmtWD { get; set; }
        public decimal TotalAdjDis { get; set; }
    }
}
