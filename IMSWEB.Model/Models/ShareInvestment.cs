using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ShareInvestment : AuditTrailModel
    {
        [Key]
        public int SIID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Purpose { get; set; }
        public decimal Amount { get; set; }
        public EnumInvestTransType TransactionType { get; set; }
        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int SIHID { get; set; }
        public EnumLiabilityType LiabilityType { get; set; }
        public virtual ShareInvestmentHead ShareInvestmentHead { get; set; }
        public int CashInHandReportStatus { get; set; }
    }
}
