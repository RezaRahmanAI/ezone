using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CustomerDueReport
    {
        public int CustomerID { get; set; }

        public string Code { get; set; }

        public string CustomerName { get; set; }

        public string ContactNo { get; set; }

        public string Address { get; set; }

        public decimal TotalSales { get; set; }

        public decimal PaymentDue { get; set; }

        public decimal ReceiveAmt { get; set; }

        public DateTime Date { get; set; }

        public decimal SalesReturnCashBack { get; set; }

        public decimal SalesReturnAmt { get; set; }

        public decimal SalesReturnPayable { get; set; }

        public decimal CashCollectionAmt { get; set; }
        public decimal PrevCashCollectionReturnAmt { get; set; }
        public decimal CashCollectionReturnAmt { get; set; }

        public decimal Adjustment { get; set; }

        public decimal BankCollectionAmt { get; set; }

        public decimal OpeningDue { get; set; }

        public decimal PreviousDue { get; set; }

        public decimal PreviousSales { get; set; }

        public decimal PrevPaymentDue { get; set; }

        public decimal PrevSalesReturn { get; set; }

        public decimal PrevCashCollection { get; set; }

        public decimal PrevBankCollectionAmt { get; set; }

        public decimal PrevReturnPayable { get; set; }

        public decimal InstallmentCollection { get; set; }

        public decimal PrevInstallmentCollection { get; set; }

        public decimal TotalCollection { get; set; }

        public decimal DownPayment { get; set; }

        public decimal PrevReceiveAmt { get; set; }

        public decimal PrevDownPayment { get; set; }

        public decimal ClosingDue { get; set; }

        public EnumCustomerType CustomerType { get; set; }
        public string CustomerTypeStr { get; set; }

        public decimal PrevSalesReturnCashBack { get; set; }

        public decimal PrevAdjustment { get; set; }

        public decimal CardPaidAmount { get; set; }

        public decimal PrevCardPaidAmount { get; set; }

        public decimal CashReceiveAmt { get; set; }
        public decimal CrInterestAmt { get; set; }
        public decimal CrInterestAmt2 { get; set; }

        public decimal InterestAmt { get; set; }

        public decimal PrevCashReceiveAmt { get; set; }

        public decimal Return { get; set; }

        public decimal ReturnAmt { get; set; }

        public  decimal ReturnCashBack {get; set;}

        public decimal PrevReturn { get; set; }
        public decimal PrevCashCollectionIntAmt { get; set; }
        public decimal CashCollectionIntAmt { get; set; }
        public decimal PrevCrInterestAmt { get; set; }
        public decimal CashCollectionDebitAdjAmt { get; set; }
        public decimal PrevCashCollectionDebitAdjAmt { get; set; }
        public decimal CashCollectionCreditAdjAmt { get; set; }
        public decimal PrevCashCollectionCreditAdjAmt { get; set; }
    }
}
