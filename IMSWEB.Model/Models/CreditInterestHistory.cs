using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CreditInterestHistory
    {
        public int Id { get; set; }
        public DateTime InterestDate { get; set; }
        public decimal InterestAmount { get; set; }
        public int HireSaleId { get; set; }
        public bool IsFirstTime { get; set; }

        [ForeignKey("HireSaleId")]
        public CreditSale CreditSale { get; set; }
    }
}
