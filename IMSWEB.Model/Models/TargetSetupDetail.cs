using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TargetSetupDetail
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int TID { get; set; }
        public decimal Quantity { get; set; }
        public virtual TargetSetup TargetSetup { get; set; }
    }
}
