using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class ServiceChargeModel
    {
        public string ConcernName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMobNo { get; set; }
        public decimal ServiceCharge { get; set; }
    }
}
