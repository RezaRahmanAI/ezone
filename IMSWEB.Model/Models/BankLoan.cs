using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class BankLoan : AuditTrailModel
    {
        public BankLoan()
        {
            BankLoanDetails = new List<BankLoanDetails>();
        }
        [Key]
        public int Id { get; set; }
        [MaxLength(15)]
        public string Code { get; set; }
        public DateTime LoanDate { get; set; }
        public decimal PrincipleLoanAmount { get; set; }
        public decimal InterestPercentage { get; set; }
        public decimal ProcessingFeePercentage { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public int NoOfInstallment { get; set; }
        public decimal InstallmentSize { get; set; }
        public decimal PenaltyChargePercentage { get; set; }
        public int ConcernId { get; set; }
        public int BankId { get; set; }
        public bool IsPaid { get; set; }
        public decimal SDPS { get; set; }
        public decimal Savings { get; set; }

        [ForeignKey("ConcernId")]
        public virtual SisterConcern SisterConcern { get; set; }
        [ForeignKey("BankId")]
        public virtual Bank Bank { get; set; }
        public virtual List<BankLoanDetails> BankLoanDetails { get; set; }

    }
}
