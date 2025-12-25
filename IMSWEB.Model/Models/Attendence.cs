using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public partial class Attendence
    {
        public int ID { get; set; }
        public int EmployeeNo { get; set; }
        public int AccountNo { get; set; }
        public string Name { get; set; }
        public System.DateTime Date { get; set; }
        public string Timetable { get; set; }
        public string OnDuty { get; set; }
        public string OffDuty { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Normal { get; set; }
        public Nullable<decimal> Realtime { get; set; }
        public string Late { get; set; }
        public string Early { get; set; }
        public Nullable<int> Absent { get; set; }
        public string OTTime { get; set; }
        public string WorkTime { get; set; }
        public string Exception { get; set; }
        public Nullable<int> MustCheckIn { get; set; }
        public Nullable<int> MustCheckOut { get; set; }
        public string Department { get; set; }
        public Nullable<decimal> NDays { get; set; }
        public Nullable<decimal> Weekend { get; set; }
        public Nullable<decimal> Holiday { get; set; }
        public string ATTTime { get; set; }
        public Nullable<decimal> NDaysOT { get; set; }
        public Nullable<decimal> WeekendOT { get; set; }
        public Nullable<decimal> HolidayOT { get; set; }
        public int AttenMonthID { get; set; }
        public virtual AttendenceMonth AttendenceMonth { get; set; }
    }
}
