using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class BankLoanTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Bank")]
        public string BankName { get; set; }
        [Display(Name = "Date")]
        public DateTime LoanDate { get; set; }
        [Display(Name = "Principle")]
        public decimal PrincipleLoanAmount { get; set; }
        [Display(Name = "Interest %")]
        public decimal InterestPercentage { get; set; }
        [Display(Name = "Fee %")]
        public decimal ProcessingFeePercentage { get; set; }
        [Display(Name = "Net Loan")]
        public decimal TotalLoanAmount { get; set; }
        [Display(Name = "No Of Installment")]
        public int NoOfInstallment { get; set; }
    }
}
