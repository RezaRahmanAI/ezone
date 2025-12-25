using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class TransactionPOrder : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransactionPOrder()
        {
            POrderDetails = new HashSet<TransactionPOrderDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TransPOrderID { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(150)]
        public string ChallanNo { get; set; }

        [StringLength(150)]
        public string InvoiceNo { get; set; }
        public int SupplierID { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal TDiscount { get; set; }

        public decimal TotalAmt { get; set; }

        public decimal RecAmt { get; set; }

        public decimal PaymentDue { get; set; }

        public decimal TotalDue { get; set; }

        public int Status { get; set; }

        public decimal PPDisAmt { get; set; }

        public decimal NetDiscount { get; set; }

        public decimal LaborCost { get; set; }

        public decimal AdjAmount { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? IsDamageOrder { get; set; }
        public decimal TPQty { get; set; }

        public string Remarks { get; set; }
        public int EditReqStatus { get; set; }
        public int ActionStatus { get; set; }
        public int POrderID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionPOrderDetail> POrderDetails { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
