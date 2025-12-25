using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SRVisit : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SRVisit()
        {
            SRVisitDetails = new HashSet<SRVisitDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRVisitID { get; set; }

        public DateTime VisitDate { get; set; }

        [Required]
        [StringLength(150)]
        public string ChallanNo { get; set; }

        public int EmployeeID { get; set; }

        public int Status { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SRVisitDetail> SRVisitDetails { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
