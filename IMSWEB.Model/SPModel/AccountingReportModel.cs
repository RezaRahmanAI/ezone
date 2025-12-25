using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TrialBalanceReportModel
    {
        public int SerialNumber { get; set; }
        public string Particulars{ get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class RPTTrialBalance
    {
        public int SL { get; set; }
        public string DebitParticular { get; set; }
        public string CreditParticular { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public bool IsHeader { get; set; }
        public bool IsCrHeader { get; set; }
        public string ConcernName { get; set; }
    }

    public class ProfitLossReportModel
    {
        public int SerialNumber { get; set; }
        public string DebitParticulars { get; set; }
        public decimal Debit { get; set; }
        public string CreditParticulars { get; set; }
        public decimal Credit { get; set; }
        public bool IsHeader { get; set; }
        public bool IsCrHeader { get; set; }
        public string ConcernName { get; set; }
    }
}
