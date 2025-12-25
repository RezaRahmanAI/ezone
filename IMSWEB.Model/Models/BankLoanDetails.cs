using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class BankLoanDetails
    {
        [Key]
        public int Id { get; set; }
        public int BankLoanId { get; set; }
        [MaxLength(10)]
        public string Status { get; set; }
        public DateTime InstallmentDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal ExpectedInstallmentAmount { get; set; }
        public decimal PenaltyChargeAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal InterestAmount { get; set; }
        public int ScheduleNo { get; set; }
        [MaxLength(500)]
        public string Remarks { get; set; }
        public decimal LastPayAdjustment { get; set; }
        public int? LoanCollectionId { get; set; }

        [ForeignKey("BankLoanId")]
        public virtual BankLoan BankLoan { get; set; }


    }
}
