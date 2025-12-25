using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SOrder : IModelBase,IBankPayment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOrder()
        {
            SOrderDetails = new HashSet<SOrderDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOrderID { get; set; }

        [Required]
        [StringLength(150)]
        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal VATPercentage { get; set; }

        public decimal VATAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal TDPercentage { get; set; }

        public decimal TDAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaymentDue { get; set; }

        public decimal? RecAmount { get; set; }

        public decimal NetDiscount { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }

        public string TramsAndCondition { get; set; } 


        public int CustomerID { get; set; }

        public decimal TotalDue { get; set; }

        public decimal AdjAmount { get; set; }

        public int Status { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public decimal TotalOffer { get; set; }

        public decimal PGrandTotal { get; set; }
        public int IsReplacement { get; set; }

        public int BankTransID { get; set; }
        public decimal CardPaidAmount { get; set; }
        public int CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public int EditReqStatus { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOrderDetail> SOrderDetails { get; set; }

        public decimal TSQty { get; set; }
        public decimal PrevDue { get; set; }
        public EnumEditApproveStatus IsEditApprovalStatus { get; set; }
    }
}
