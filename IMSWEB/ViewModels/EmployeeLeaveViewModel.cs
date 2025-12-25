using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public partial class EmployeeLeaveViewModel
    {
        public int EmployeeLeaveID { get; set; }
        [Display(Name = "Emplyee")]
        public string EmployeeID { get; set; }
        public System.DateTime LeaveDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Description { get; set; }
        [Display(Name = "Paid Leave")]
        public bool IsPaidLeave { get; set; }
        public EnumEmployeeLeaveStatus Status { get; set; }
        [Display(Name = "Leave Type")]
        public EnumEmployeeLeaveType LeaveType { get; set; }
        [Display(Name = "Short Leave Hour")]
        public decimal? ShortLeaveHour { get; set; }

        public virtual Employee Employee { get; set; }
         [Display(Name = "Emplyee")]
        public string EmplyeeName { get; set; }
         [Display(Name = "Department")]
        public string DepartmentName { get; set; }
         [Display(Name = "Designation")]
        public string DesignationName { get; set; }
         [Display(Name = "Grade")]
        public string GradeName { get; set; }
    }
}