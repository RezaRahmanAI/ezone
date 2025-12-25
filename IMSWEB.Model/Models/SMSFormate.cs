using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSFormate
    {
        public SMSFormate()
        {
            this.SMSStatuses = new HashSet<SMSStatus>();
        }
        public int SMSFormateID { get; set; }
        public string Code { get; set; }
        public string SMSDescription { get; set; }
        public int SMSType { get; set; }
        public virtual ICollection<SMSStatus> SMSStatuses { get; set; }
    }
}
