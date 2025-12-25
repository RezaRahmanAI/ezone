using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class BankLedgerModel
    {
        public int ConcernID { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
 
        public DateTime TransDate { get; set; }
        public string TransactionNo { get; set; }

        public decimal Opening { get; set; }
        public decimal Deposit { get; set; }
        public decimal Widthdraw { get; set; }
        public decimal CashCollection { get; set; }
        public decimal CashDelivery { get; set; }
        public decimal FundIN { get; set; }
        public decimal FundOut { get; set; }
        public decimal Closing { get; set; }
      
    }
}
