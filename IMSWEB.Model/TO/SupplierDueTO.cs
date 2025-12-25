using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class SupplierDueTO
    {
        public int SupplierID { get; set; }
        public decimal TotalDue { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string ConcernName { get; set; }
        public decimal OpeningDue { get; set; }
    }
}
