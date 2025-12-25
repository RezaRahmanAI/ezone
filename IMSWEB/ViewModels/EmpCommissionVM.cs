using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class EmpCommissionVM
    {
        public int? ECID { get; set; }
        [Display(Name = "Comm. Month")]
        public string CommissionMonth { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Commission Amt is not valid.")]
        [Display(Name = "Commission Amt")]
        public decimal CommissionAmt { get; set; }
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

    public class CreateEmpCommissionVM
    {
        public CreateEmpCommissionVM()
        {
            Commissions = new List<EmpCommissionVM>();
            Departments = new List<DepartmentViewModel>();
        }
        public int DepartmentID { get; set; }
        public List<EmpCommissionVM> Commissions { get; set; }
        public List<DepartmentViewModel> Departments { get; set; }
    }
}