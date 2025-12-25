using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMSWEB.Model;
using System.ComponentModel.DataAnnotations;

namespace IMSWEB
{
    public class CardTypeSetupViewModel
    {
        public int? CardTypeSetupID { get; set; }
        public string Code { get; set; }

        [Display(Name = "Bank")]
        public string BankName { get; set; }

        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Display(Name="Bank")]
        public int BankID { get; set; }

        [Display(Name = "Card Type")]
        public string CardTypeName { get; set; }

        [Display(Name = "Card Type")]
        public int CardTypeID { get; set; }
        public decimal Percentage { get; set; }
        public List<CardType> CardTypes { get; set; }
    }
}