using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class POrderDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public POrderDetail()
        {
            POProductDetails = new HashSet<POProductDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int POrderDetailID { get; set; }

        public int ProductID { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TAmount { get; set; }

        public int POrderID { get; set; }

        public decimal PPDISPer { get; set; }

        public decimal PPDISAmt { get; set; }

        public decimal MRPRate { get; set; }

        public int ColorID { get; set; }
        public decimal SalesRate { get; set; }
        public decimal ExtraPPDISPer { get; set; }
        public decimal ExtraPPDISAmt { get; set; }
        public decimal PPOffer { get; set; }
        public decimal CreditSalesRate { get; set; }
        public decimal CRSalesRate12Month { get; set; }
        public decimal CRSalesRate3Month { get; set; }

        public int GodownID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POProductDetail> POProductDetails { get; set; }

        public virtual POrder POrder { get; set; }

        public virtual Product Product { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }


        [ForeignKey("GodownID")]
        public virtual Godown Godown { get; set; }
        public int? DOrderDetailId { get; set; }
    }
}
