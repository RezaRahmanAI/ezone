using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMSWEB.Model;
using System.ComponentModel.DataAnnotations;

namespace IMSWEB
{
    public class CreditInterestPercentageViewModel
    {
        public int IntPercentageID { get; set; }
        public string Code { get; set; }

        [Display(Name = "Int. Percentage")]
        [Required(ErrorMessage = "Int. Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Int. Percentage will be 0-100")]
        public decimal Percentage { get; set; }
        [Display(Name = "Sister concern")]
        public string ConcernId { get; set; }
        [Display(Name = "Effect Date")]
        public string EffectDate { get; set; }
    }
}