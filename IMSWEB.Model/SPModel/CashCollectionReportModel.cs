using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CashCollectionReportModel
    {
        public int CashCollectionID { get; set; }

        public EnumPayType PaymentType { get; set; }
        public string ModuleType { get; set; }

        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string ChecqueNo { get; set; }
        public string BranchName { get; set; }
        public DateTime? EntryDate { get; set; }
        public decimal Amount { get; set; }
        public decimal AdjustAmt { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal TotalDue { get; set; }
        public string BKashNo { get; set; }
        public EnumTranType TransactionType { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public int? SupplierID { get; set; }
        public string ReceiptNo { get; set; }
        public int ConcernID { get; set; }
        public string ConcernName { get; set; }
        public string Remarks { get; set; }
        public EnumCustomerType CustomerType { get; set; }
        public decimal TotalDis { get; set; }
    }
}
