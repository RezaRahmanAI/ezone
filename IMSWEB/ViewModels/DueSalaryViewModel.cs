using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class DueSalaryViewModel
    {
        public string ID { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeID { get; set; }
        [Required(ErrorMessage = "Amount is Required")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Date is Required")]
        public System.DateTime Date { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Remarks { get; set; }
        public int ConcernID { get; set; }
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        [Display(Name = "Designation")]
        public string DesignationName { get; set; }
        [Display(Name = "Grade")]
        public string GradeName { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        [Display(Name = "Code")]
        public string EmployeeCode { get; set; }
        public string SalaryType { get; set; }
    }
}