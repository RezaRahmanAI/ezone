using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class DailyCashBookLedgerModel
    {
        public int ConcernID { get; set; }
        public DateTime Date { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CashSales { get; set; }
        public decimal DueCollection { get; set; }
        public decimal DownPayment { get; set; }
        public decimal InstallAmt { get; set; }
        public decimal Loan { get; set; }
        public decimal BankWithdrwal { get; set; }
        public decimal OthersIncome { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal PaidAmt { get; set; }
        public decimal Delivery { get; set; }
        public decimal EmployeeSalary { get; set; }
        public decimal Conveyance { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal LoanPaid { get; set; }
        public decimal Vat { get; set; }
        public decimal OthersExpense { get; set; }
        public decimal SRET { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
