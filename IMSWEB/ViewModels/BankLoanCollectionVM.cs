using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class BankLoanCollectionVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Bank Loan")]
        public int BankLoanDetailsId { get; set; }
        [Display(Name = "Date")]
        public DateTime CollectionDate { get; set; }
        [Display(Name = "Collection Amount")]
        public decimal CollectionAmount { get; set; }
        [Display(Name = "Payment Amount")]
        public decimal LoanAmount { get; set; }
        [Display(Name = "Bank")]
        public int BankId { get; set; }
        public decimal SDPS { get; set; }
        public decimal Savings { get; set; }
        [Display(Name = "Total")]
        public decimal TotalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public EnumBankLoanType CollectionType { get; set; }
        public List<SelectListItem> Banks { get; set; }
        public List<SelectListItem> BankLoans { get; set; }

    }
}