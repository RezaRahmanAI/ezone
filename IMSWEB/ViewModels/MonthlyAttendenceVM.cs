using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class MonthlyAttendenceVM
    {
        public int? MAID { get; set; }
        public string Month { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Days is not valid.")]
        public int Days { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "OT Days is not valid.")]
        [Display(Name = "OT days")]
        public int OTDays { get; set; }
        public int EmployeeID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Designation")]
        public string DesignationName { get; set; }

        [Display(Name = "Is Select")]
        public bool IsSelect { get; set; }
    }

    public class CreateMonthlyAttendenceVM
    {
        public CreateMonthlyAttendenceVM()
        {
            Attendences = new List<MonthlyAttendenceVM>();
            Departments = new List<DepartmentViewModel>();
        }
        public int DepartmentID { get; set; }

        public List<MonthlyAttendenceVM> Attendences { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
    }
}