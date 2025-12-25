using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CardType
    {
        public CardType()
        {
            CardTypeSetups = new HashSet<CardTypeSetup>();
        }
        [Key]
        public int CardTypeID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Sequence { get; set; }
        public int ConcernID { get; set; }
        public SisterConcern SisterConcern { get; set; }
        public ICollection<CardTypeSetup> CardTypeSetups { get; set; }
    }
}

