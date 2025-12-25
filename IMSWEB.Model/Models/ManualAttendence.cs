using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ManualAttendence : AuditTrailModel
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeID { get; set; }
        public string OnDuty { get; set; }
        public string OffDuty { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }

    }
}
