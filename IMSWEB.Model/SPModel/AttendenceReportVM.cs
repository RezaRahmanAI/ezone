using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class AttendencReportModel
    {
        public int AccountNo { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public int? Absent { get; set; }
        public string dAbsent { get; set; }
        public DateTime Date { get; set; }
        public string OTTime { get; set; }
        public string Late { get; set; }
    }
}
