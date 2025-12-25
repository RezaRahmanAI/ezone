using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class ROrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ROrderDetailID { get; set; }

        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UTAmount { get; set; }
        public int ROrderID { get; set; }
        public int StockDetailID { get; set; }

        public int ColorID { get; set; }
        //public int GodownID { get; set; }

        public virtual Product Product { get; set; }

        public virtual ROrder ROrder { get; set; }

        //public virtual StockDetail StockDetail { get; set; }

        [ForeignKey("ColorID")]
        public virtual Color Color { get; set; }

    }
}
