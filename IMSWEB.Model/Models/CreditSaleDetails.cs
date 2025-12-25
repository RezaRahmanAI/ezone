using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class CreditSaleDetails
    {
        [Key]
        public int CreditSaleDetailsID { get; set; }

        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public decimal UTAmount { get; set; }

        public int CreditSalesID { get; set; }

        public decimal? MPRateTotal { get; set; }

        public decimal MPRate { get; set; }

        public int StockDetailID { get; set; }
        public decimal PPOffer { get; set; }

        public decimal IntPercentage { get; set; }

        public decimal IntTotalAmt { get; set; }
        public string Compressor { get; set; }
        public string Motor { get; set; }
        public string Panel { get; set; }
        public string Spareparts { get; set; }
        public string Service { get; set; }
        public int IsProductReturn { get; set; }
        public string Warranty { get; set; }
        public virtual CreditSale CreditSale { get; set; }

        public virtual Product Product { get; set; }

        [ForeignKey("StockDetailID")]
        public virtual StockDetail StockDetail { get; set; }
    }
}
