using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class ROrder : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROrder()
        {
            ROrderDetails = new HashSet<ROrderDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ROrderID { get; set; }

        [Required]
        [StringLength(150)]
        public string InvoiceNo { get; set; }

        public DateTime ReturnDate { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal PaidAmount { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }

        public int CustomerID { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROrderDetail> ROrderDetails { get; set; }
    }
}
