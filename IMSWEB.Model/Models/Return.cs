using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Return : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Return()
        {
            ReturnDetails = new HashSet<ReturnDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReturnID { get; set; }

        [Required]
        [StringLength(150)]
        public string InvoiceNo { get; set; }

        public DateTime ReturnDate { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal CustomerID { get; set; }

        public decimal SupplierID { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal TDAmt { get; set; }

        public decimal AdjAmount { get; set; }

        public decimal PaymentDue { get; set; }

       
        public int Status { get; set; }

    


       

        [StringLength(350)]
        public string Remarks { get; set; }
    

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int ConcernID { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnDetail> ReturnDetails { get; set; }
    }
}
