using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ReplaceOrderDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SOrderDetailID { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal PPDPercentage { get; set; }

        public decimal PPDAmount { get; set; }

        public decimal Quantity { get; set; }

        public decimal UTAmount { get; set; }

        public int SOrderID { get; set; }

        public decimal MPRate { get; set; }
        [Display(Name = "Stock DetailID")]
        public int SDetailID { get; set; }

        [Display(Name = "Replace Stock DetailID")]
        public int? RStockDetailID { get; set; }

        public decimal PPOffer { get; set; }

        public decimal SRate { get; set; }

        public decimal PRate { get; set; }

        [NotMapped]
        public string DamageIMEINO { get; set; }
        [NotMapped]
        public string ReplaceIMEINO { get; set; }
        public int? RepOrderID { get; set; }
        public string DamageUnitPrice { get; set; }
        public string DamageProductName { get; set; }
        public string Remarks { get; set; }
        public virtual Product Product { get; set; }

        public virtual SOrder SOrder { get; set; }

        public virtual StockDetail StockDetail { get; set; }
        public virtual ReplaceOrder ROrder { get; set; }
    }
}
