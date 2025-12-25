using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class SalaryProcessErrorVM
    {
        [Display(Name="Code")]
        public string Code { get; set; }
        [Display(Name = "Employee")]
        public string Name { get; set; }
        public string Error { get; set; }
    }
}