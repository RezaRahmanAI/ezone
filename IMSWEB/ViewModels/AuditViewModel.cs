using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class AuditViewModel
    {
        public List<GetCreditSalesOrderViewModel> HireSaleList { get; set; }
        public List<GetCashCollectionViewModel> CashCollectionList { get; set; }
        public List<GetSalesOrderViewModel> SOrderList { get; set; }
        public IEnumerable<UpcommingScheduleReport> Schedules { get; set; }
        public IEnumerable<CreateExpenditureViewModel> Incomes { get; set; }
        public IEnumerable<CreateExpenditureViewModel> Expenses { get; set; }
        public List<GetCashCollectionViewModel> CashDeliveryList { get; set; }
        public IEnumerable<GetBankTransactionViewModel> bankTransactions { get; internal set; }
    }
}