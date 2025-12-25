using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class StockDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockDetail()
        {
            CreditSaleDetails = new HashSet<CreditSaleDetails>();
            SOrderDetails = new HashSet<SOrderDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SDetailID { get; set; }

        [Required]
        [StringLength(250)]
        public string StockCode { get; set; }

        public int ProductID { get; set; }

        [Required]
        public string IMENO { get; set; }

        public int StockID { get; set; }

        public int Status { get; set; }

        public int ColorID { get; set; }
        public int GodownID { get; set; }

        public int POrderDetailID { get; set; }

        public decimal PRate { get; set; }
        public decimal SRate { get; set; }
        public decimal CreditSRate { get; set; }
        public decimal CRSalesRate12Month { get; set; }
        public decimal CRSalesRate3Month { get; set; }
        public decimal Quantity { get; set; }

        public int IsDamage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditSaleDetails> CreditSaleDetails { get; set; }

        public virtual Product Product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOrderDetail> SOrderDetails { get; set; }

        public virtual Stock Stock { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }

        [ForeignKey("GodownID")]
        public virtual Godown Godown { get; set; }

    }
}
