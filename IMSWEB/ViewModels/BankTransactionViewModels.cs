using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;

namespace IMSWEB
{
    public class CreateBankTransactionViewModel : IValidatableObject
    {
        public string BankTranID { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime? TranDate { get; set; }
        [Display(Name = "Transaction No")]
        public string TransactionNo { get; set; }

        [Display(Name = "Transaction Type")]
        public EnumTransactionType TransactionType { get; set; }
        public string BankName { get; set; }

        [Display(Name = "From A/C NO.")]
        public string BankID { get; set; }

        public string CustomerName { get; set; }
        [Display(Name = "Customer")]
        public string CustomerID { get; set; }

        public string AnotherBankName { get; set; }

        [Display(Name = "To A/C NO.")]
        public string AnotherBankID { get; set; }

        public string SupplierName { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierID { get; set; }

        [Range(1,double.MaxValue,ErrorMessage ="Amount is required.")]
        public decimal Amount { get; set; }
        // public int ProductType { get; set; }

        [Display(Name = "Checque No")]
        public string ChecqueNo { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "Investment Head")]
        public string SIHID { get; set; }

        public List<ShareInvestmentHead> Heads { get; set; }

        public string ExpenseItemID { get; set; }

        public string IncomeItemID { get; set; }
        [Display(Name = "Concern")]
        public int ConcernId { get; set; }
        public string CreatedBy { get; set; }

        public string CreateDate { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedDate { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateBankTransactionViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class GetBankTransactionViewModel
    {
        public int BankTranID { get; set; }

        [Display(Name = "Trans. No")]
        public string TransactionNo { get; set; }

        [Display(Name = "Trans. Type")]
        public EnumTransactionType TransactionType { get; set; }
        public string BankID { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public string CustomerName { get; set; }

        public string CustomerID { get; set; }
        public string SupplierName { get; set; }

        public string SupplierID { get; set; }
        public string AnotherBankName { get; set; }
        public string AnotherBankID { get; set; }
        public decimal Amount { get; set; }
        // public int ProductType { get; set; }
        public DateTime TranDate { get; set; }
        public string ChecqueNo { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        [Display(Name = "Account No")]
        public string AccountNo { get; set; }
       
        [Display(Name = "Status")]
        public EnumWFStatus WFStatus { get; set; }

    }
}