using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class BankLoanCollection : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
        public DateTime CollectionDate { get; set; }
        public decimal CollectionAmount { get; set; }
        public int ConcernId { get; set; }
        public decimal SDPS { get; set; }
        public decimal Savings { get; set; }
        public int? CCLoanId { get; set; }
        public bool IsOnlyInterest { get; set; }
        public EnumBankLoanType CollectionType { get; set; }
        public decimal PrevTotalGivenAmount { get; set; }
        public decimal TotalGivenAmount { get; set; }
        public decimal PrevTotalInterestAmount { get; set; }
        public decimal TotalInterestAmount { get; set; }

        [ForeignKey("ConcernId")]
        public virtual SisterConcern SisterConcern { get; set; }
    }
}
