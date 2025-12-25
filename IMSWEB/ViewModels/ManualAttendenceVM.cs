using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class CreateManualAttendenceVM
    {
        public CreateManualAttendenceVM()
        {
            Attendences = new List<ManualAttendenceVM>();
        }
        public string NextPayProcessDate { get; set; }
        public int EmployeeID { get; set; }
        public List<ManualAttendenceVM> Attendences { get; set; }
        public SelectList Employees { get; set; }
    }
    public class ManualAttendenceVM
    {
        public int? ID { get; set; }
        public string Date { get; set; }
        public int EmployeeID { get; set; }
        public string OnDuty { get; set; }
        public string OffDuty { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public bool IsSelect { get; set; }
    }
}