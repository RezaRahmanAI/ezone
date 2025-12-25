using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public partial class EmployeeLeave : AuditTrailModel
    {
        public int EmployeeLeaveID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime LeaveDate { get; set; }
        public string Description { get; set; }
        public int PaidLeave { get; set; }
        public int Status { get; set; }
        public int LeaveType { get; set; }
        public decimal ShortLeaveHour { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
