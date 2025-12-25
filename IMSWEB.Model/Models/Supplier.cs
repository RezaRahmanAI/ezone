using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            CashCollections = new HashSet<CashCollection>();
            POrders = new HashSet<POrder>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public string Name { get; set; }

        public string OwnerName { get; set; }

        [StringLength(150)]
        public string ContactNo { get; set; }

        public string Address { get; set; }

        [StringLength(250)]
        public string PhotoPath { get; set; }

        public decimal OpeningDue { get; set; }

        public decimal TotalDue { get; set; }

        public int ConcernID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashCollection> CashCollections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POrder> POrders { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
    }
}
