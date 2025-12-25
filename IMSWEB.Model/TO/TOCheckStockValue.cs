using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class TOCheckStockValue
    {
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public decimal PurchaseRate { get; set; }
    }
}
