using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Stock : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock()
        {
            StockDetails = new HashSet<StockDetail>();
            
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockID { get; set; }

        [Required]
        [StringLength(150)]
        public string StockCode { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Quantity { get; set; }

        public int ProductID { get; set; }

        public decimal MRPPrice { get; set; }

        public decimal LPPrice { get; set; }

        public int ConcernID { get; set; }

        public int ColorID { get; set; }

        public int GodownID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockDetail> StockDetails { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }

        [ForeignKey("GodownID")]
        public virtual Godown Godown { get; set; }
        

    }
}
