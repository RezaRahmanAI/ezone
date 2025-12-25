using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class PaymentDueInfoTO
    {
        public decimal TotalChargeDue { get; set; }
        public DateTime ServiceFrom { get; set; }
        public DateTime ServiceTo { get; set; }
        public int ServiceRemainingMonth { get; set; }
        public decimal ServiceChargePerMonth { get; set; }
    }
}
