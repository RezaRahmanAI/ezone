using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CustomerOpeningDue
    {
        public int ID { get; set; }

        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public decimal OpeningDue { get; set; }
        public DateTime Date { get; set; }
        public int ConcernID { get; set; }

    }
}
