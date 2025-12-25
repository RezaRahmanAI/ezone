using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateBankViewModel : IValidatableObject
    {
        public string Id { get; set; }
        public string Code { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Account No.")]
        public string AccountNo { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }

        [Display(Name = "Balance")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Is CC Loan Applicable?")]
        public bool IsAdvancedDueLimitApplicable { get; set; }
        [Display(Name = "CC Loan Amount Limit")]
        public decimal AdvancedAmountLimit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new BankViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}