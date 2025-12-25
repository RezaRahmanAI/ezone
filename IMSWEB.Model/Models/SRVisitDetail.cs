using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SRVisitDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SRVisitDetail()
        {
            SRVProductDetails = new HashSet<SRVProductDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRVisitDID { get; set; }

        public int ProductID { get; set; }

        public decimal Quantity { get; set; }

        public int SRVisitID { get; set; }

        public int ColorID { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SRVProductDetail> SRVProductDetails { get; set; }

        public virtual SRVisit SRVisit { get; set; }

        public virtual Product Product { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }
    }
}
