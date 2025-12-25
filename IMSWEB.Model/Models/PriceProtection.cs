using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class PriceProtection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PriceProtection()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceProtectionID { get; set; }
        public decimal PrvPrice { get; set; }
        public decimal ChangePrice { get; set; }
        public int PrvStockQty { get; set; }
        public int ProductID { get; set; }

        public int POrderID { get; set; }

        public int ColorID { get; set; }

        public int SupplierID { get; set; }

        public int ConcernID { get; set; }

        public DateTime PChangeDate { get; set; }

        [ForeignKey("ConcernID")]
        public virtual SisterConcern SisterConcern { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("POrderID")]
        public virtual POrder POrder { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }


    }
}
