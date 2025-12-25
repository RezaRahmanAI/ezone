using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class BankTransReportModel
    {
        public int ConcernID { get; set; }
        public string ConcernName { get; set; }
        public int BankID { get; set; }
        public int? FromToBankID { get; set; }
        public string BankName { get; set; }
        public string AccountNO { get; set; }
        public string AccountName { get; set; }
        public DateTime TransDate { get; set; }
        public string TransactionNo { get; set; }
        public string ChecqueNo { get; set; }
        public decimal Amount { get; set; }
        public string FromToAccountNo { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public int? SupplierID { get; set; }
        public int? CustomerID { get; set; }
        public decimal Opening { get; set; }
        public decimal Closing { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdraw { get; set; }
        public decimal CashCollection { get; set; }
        public decimal CashDelivery { get; set; }
        public decimal FundIN { get; set; }
        public decimal FundOut { get; set; }
        public decimal LiaPay { get; set; }

        public decimal LiaRec { get; set; }

        public decimal BankIncome { get; set; }

        public decimal BankExpense { get; set; }


    }
}
