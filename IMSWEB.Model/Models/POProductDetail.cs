using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class POProductDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int POPDID { get; set; }
        public int? DamagePOPDID { get; set; }

        public int ProductID { get; set; }

        public int ColorID { get; set; }

        [Required]
        public string IMENO { get; set; }
        [NotMapped]
        public string DIMENO { get; set; }

        public int POrderDetailID { get; set; }
        public int? IsDamageReplaced { get; set; }
        public virtual POrderDetail POrderDetail { get; set; }

        public virtual Product Product { get; set; }

        public  decimal ReturnQty { get; set; }

        public int? ReturnSDetailID { get; set; }
    }
}
