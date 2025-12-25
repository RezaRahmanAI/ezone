using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTCustomerDueDateWise
    {
        public int CustomerID { get; set; }
        public decimal Sales { get; set; }
        public decimal CashCollectionInterestAmt { get; set; }
        public decimal HireIntestrestAmt { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal SalesReceive { get; set; }
        public decimal CollectionAmt { get; set; }
        public decimal CollectionReturnAmt { get; set; }
        public decimal InstallmentCollection { get; set; }
        public decimal TotalCollection { get; set; }
        public decimal TotalCollectionReturn { get; set; }
        public decimal SaleReturn { get; set; }
        public decimal ClosingDue { get; set; }
        public decimal OpeningDue { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerType { get; set; }
        public decimal BankTransCashCollectionReturnBank {  get; set; }
        public string ConcernName { get; set; }
        public decimal CashCollectionsTypeAdjustment { get; set; }

    }
}
