using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SRVProductDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRVisitPDID { get; set; }

        public int ProductID { get; set; }

        public int ColorID { get; set; }

        [Required]
        public string IMENO { get; set; }

        public int SRVisitDID { get; set; }
        public int Status { get; set; }
        public int SDetailID { get; set; }

        public virtual SRVisitDetail SRVisitDetail { get; set; }

        public virtual Product Product { get; set; }
    }
}
