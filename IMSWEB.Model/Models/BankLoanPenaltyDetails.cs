using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class BankLoanPenaltyDetails : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        public int LoanDetailsId { get; set; }
        public decimal PenaltyPercentage { get; set; }
        public decimal PenaltyAmount { get; set; }
        public DateTime PenaltyDate { get; set; }
        [ForeignKey("LoanDetailsId")]

        public virtual BankLoanDetails BankLoanDetails { get; set; }
    }
}
