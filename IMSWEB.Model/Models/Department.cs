using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Department : AuditTrailModel
    {
        public int DepartmentId { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
    }
}
