
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class BankSummaryReportModel
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public decimal Opening { get; set; }
        public decimal Deposit { get; set; }
        public decimal WidthDraw { get; set; }
        public decimal Amount
        {
            get;
            set;
        }
    }
}
