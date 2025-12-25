using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class SalaryProcessViewModel
    {
        public SalaryProcessViewModel()
        {
            ErrorList = new List<SalaryProcessErrorVM>();
        }
        //public string EmployeeIdList { get; set; }
        [Display(Name = "Festival Bonus Apply")]
        public bool IsFestivalBonus { get; set; }
        public List<SalaryProcessErrorVM> ErrorList { get; set; }
         [Display(Name = "Salary Month")]
        public DateTime SalaryProcessMonth { get; set; }
        public int SalaryProcessID { get; set; }
        public int TotalEmployee { get; set; }
    }
}