using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Religion : AuditTrailModel
    {
        public int ReligionID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int SequenceNo { get; set; }
        public int Status { get; set; }
    }
}
