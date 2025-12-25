using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class MonthlyBenefitReport
    {
        public DateTime InvoiceDate { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal PurchaseTotal { get; set; }
        public decimal TDAmount_Sale { get; set; }
        public decimal TDAmount_CreditSale { get; set; }
        public decimal FirstTotalInterest { get; set; }
        public decimal HireCollection { get; set; }
        public decimal CreditSalesTotal { get; set; }
        public decimal CreditPurchase { get; set; }
        public decimal CommisionProfit { get; set; }
        public decimal HireProfit { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal OthersIncome { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Adjustment { get; set; }
        public decimal LastPayAdjustment { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Benefit { get; set; }
    }
}
