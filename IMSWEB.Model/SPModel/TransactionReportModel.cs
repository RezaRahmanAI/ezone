using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TransactionReportModel
    {
        public DateTime? EntryDate { get; set; }
        public decimal RetailSale { get; set; }
        public decimal HireSale { get; set; }
        public decimal DealerSale { get; set; }
        public decimal TotalSale { get; set; }
        public decimal RetailCash { get; set; }
        public decimal DownPayment { get; set; }
        public decimal HireCollection { get; set; }
        public decimal DealerCollection { get; set; }
        public decimal TotalCollection { get; set; }
        public decimal DailyExpense { get; set; }
        public decimal DailyIncome { get; set; }
        public decimal CompanyPayment { get; set; }
        public decimal Balance { get; set; }

        public decimal CumulativeBalance { get; set; }

        public decimal RetailCashCollection { get; set; }
        public decimal BankDepositeAmount { get; set; }
        public decimal BankWithdrowAmount { get; set; }
    }
}
