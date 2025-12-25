using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CreditInterestPercentage
    {
        [Key]
        public int IntPercentageID { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public int ConcernID { get; set; }
        public SisterConcern SisterConcern { get; set; }
        public DateTime EffectDate { get; set; }
    }
}

