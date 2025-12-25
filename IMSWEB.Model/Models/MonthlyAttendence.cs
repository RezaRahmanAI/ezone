using System;
using System.ComponentModel.DataAnnotations;

namespace IMSWEB.Model
{
    public class MonthlyAttendence : AuditTrailModel
    {
        [Key]
        public int MAID { get; set; }
        public DateTime Month { get; set; }
        public int Days { get; set; }
        public int OTDays { get; set; }
        public virtual Employee Employee { get; set; }
        public int EmployeeID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
    }
}
