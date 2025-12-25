using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CashInHandReportModel
    {
        public DateTime TransDate { get; set; }
        public int id { get; set; }
        public string Expense { get; set; }
        public decimal ExpenseAmt { get; set; }
        public decimal OpeningCshInHand { get; set; }
        public decimal ClosingCshInHand { get; set; }
        public decimal CurrentCshInHand { get; set; }
        public string Income { get; set; }
        public decimal IncomeAmt { get; set; }
        public string Module { get; set; }
        public string EmployeeName { get; set; }
        public string ConcernName { get; set; }
    }
}
