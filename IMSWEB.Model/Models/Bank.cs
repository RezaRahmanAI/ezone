
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    [Table("Banks")]
    public partial class Bank : AuditTrailModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bank()
        {
            BankTransactions = new HashSet<BankTransaction>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BankID { get; set; }

        [Required]
        [StringLength(150)]
        public string Code { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public string BranchName { get; set; }


        [Required]
        public string AccountNo { get; set; }
        [Required]
        public string AccountName { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal TotalAmount { get; set; }

        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BankTransaction> BankTransactions { get; set; }

        public int IsAdvancedDueLimitApplicable { get; set; }

        public decimal AdvancedAmountLimit { get; set; }
    }
}



