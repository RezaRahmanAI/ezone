using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            CreditSaleProducts = new HashSet<CreditSaleDetails>();
            DamageProducts = new HashSet<DamageProduct>();
            POProductDetails = new HashSet<POProductDetail>();
            POrderDetails = new HashSet<POrderDetail>();
            SaleOffers = new HashSet<SaleOffer>();
            SOrderDetails = new HashSet<SOrderDetail>();
            StockDetails = new HashSet<StockDetail>();
            Stocks = new HashSet<Stock>();
        }

        public int ProductID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public string ProductName { get; set; }

        [StringLength(250)]
        public string PicturePath { get; set; }

        public int CategoryID { get; set; }

        public int CompanyID { get; set; }

        public int ConcernID { get; set; }

        public decimal PWDiscount { get; set; }

        public DateTime? DisDurationFDate { get; set; }

        public DateTime? DisDurationToDate { get; set; }
        public string CompressorWarrentyMonth { get; set; }
        public string PanelWarrentyMonth { get; set; }
        public string MotorWarrentyMonth { get; set; }
        public string SparePartsWarrentyMonth { get; set; }
        public string ServiceWarrentyMonth { get; set; }
        public decimal? MRP { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal RP { get; set; }

        public virtual Category Category { get; set; }

        public virtual Company Company { get; set; }

        public EnumUnitType UnitType { get; set; }
        public int ProductType { get; set; }
        public string UserInputWarranty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditSaleDetails> CreditSaleProducts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DamageProduct> DamageProducts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POProductDetail> POProductDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POrderDetail> POrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleOffer> SaleOffers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOrderDetail> SOrderDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockDetail> StockDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
