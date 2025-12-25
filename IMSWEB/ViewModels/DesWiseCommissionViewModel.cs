using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class DesWiseCommissionViewModel
    {
        public string ID { get; set; }
        [Display(Name = "Commission(%)")]
        public Decimal CommissionPercent { get; set; }
        [Required(ErrorMessage="Designation is Required.")]
        [Display(Name = "Designation")]
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
    }
}