using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CardTypeSetup
    {
        [Key]
        public int CardTypeSetupID { get; set; }
        public string Code { get; set; }
        public virtual Bank Bank { get; set; }
        public int BankID { get; set; }
        public virtual CardType CardType { get; set; }
        public int CardTypeID { get; set; }
        public decimal Percentage { get; set; }
        public int ConcernID { get; set; }
        public SisterConcern SisterConcern { get; set; }
    }
}

