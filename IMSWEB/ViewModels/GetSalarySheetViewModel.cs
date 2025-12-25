using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GetSalarySheetViewModel
    {
        public GetSalarySheetViewModel()
        {
            Employees = new List<GetEmployeeViewModel>();
        }
        public DateTime SalaryMonth { get; set; }
        public int DepartmentID { get; set; }
        public string EmployeeId { get; set; }
        public List<GetEmployeeViewModel> Employees { get; set; }
    }
}