using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class ExpenseItem : AuditTrailModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpenseItem()
        {
            Expenditures = new HashSet<Expenditure>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenseItemID { get; set; }

        [Required]
        [StringLength(150)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public EnumCompanyTransaction Status { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expenditure> Expenditures { get; set; }

      
    }
}
