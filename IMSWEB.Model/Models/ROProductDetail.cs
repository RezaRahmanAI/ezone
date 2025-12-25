using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class ROProductDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ROPDID { get; set; }

        public int ProductID { get; set; }

        public int ColorID { get; set; }

        [Required]
        public string IMENO { get; set; }

        public int RODetailID { get; set; }

        public virtual ROrderDetail ReturnDetail { get; set; }

        public virtual Product Product { get; set; }
    }
}
