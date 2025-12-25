using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;
namespace IMSWEB
{
    public class InvestmentheadViewModel
    {
        public int SIHID { get; set; }
        public string Code { get; set; }

        [Required(ErrorMessage ="Head is required.")]
        public string Name { get; set; }

        [Display(Name="Investment Type")]
        public EnumInvestmentType ParentId { get; set; }

        [Display(Name = "Investment Type")]
        public string ParentName { get; set; }
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }
        [Display(Name = "Opening Type")]
        public EnumOpeningType OpeningType { get; set; }
        public List<ShareInvestmentHead> Heads { get; set; }
        [Display(Name = "Opening Date")]
        public string OpeningDate { get; set; }
    }
}