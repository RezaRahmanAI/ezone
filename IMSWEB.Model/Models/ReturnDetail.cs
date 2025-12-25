using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class ReturnDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReturnDetailsID { get; set; }

        public int ReturnID { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PPDPercentage { get; set; }
        public decimal PPDAmount { get; set; }
        public decimal UTAmount { get; set; }   
        public int SDetailID { get; set; }

        public int GodownID { get; set; }
        public virtual Product Product { get; set; }
        public virtual Return Return { get; set; }
        public virtual StockDetail StockDetail { get; set; }
    }
}
