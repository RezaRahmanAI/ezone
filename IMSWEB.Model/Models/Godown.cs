using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Godown
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        public Godown()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GodownID { get; set; }

        [Required]
        [StringLength(250)]
        public string Code { get; set; }

        [Required]
        [StringLength(350)]
        public string Name { get; set; }

        public int ConcernID { get; set; }
        public int ISCommon { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Product> Products { get; set; }
    }
}
