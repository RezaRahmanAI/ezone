using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CommissionSetupViewModel
    {
        public string CSID { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [Display(Name = "Achv. From(%)")]
        public decimal AchievedPercentStart { get; set; }

        [Display(Name = "Achv. To(%)")]
        public decimal AchievedPercentEnd { get; set; }

        [Display(Name = "Commission Month")]
        [Required(ErrorMessage= "Commission Month is required")]
        public DateTime CommissionMonth { get; set; }

        [Display(Name = "Commission Amount")]
        [Required(ErrorMessage = "Commisssion Amt is required")]
        public decimal CommisssionAmt { get; set; }

        [Display(Name = "Commission Percent")]
        [Required(ErrorMessage = "Commission Percent is required")]
        public decimal CommissionPercent { get; set; }
        public string Status { get; set; }

    }
}