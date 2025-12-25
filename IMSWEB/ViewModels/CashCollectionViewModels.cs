using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GetCashCollectionViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Entry Date")]
        public string EntryDate { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; }

        [Display(Name = "A/C")]
        public string Code{ get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        //[Display(Name = "Supplier Name")]
        //public string SupplierName { get; set; }

        [Display(Name = "AccountNo")]
        public string AccountNo { get; set; }

        [Display(Name = "Amount")]
        public string Amount { get; set; }

        [Display(Name = "Transaction Type")]
        public EnumTranType TransactionType { get; set; }
        public EnumDeliveryDropdownTranType DeliveryType { get; set; } 

        public string Remarks { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

    }

    public class CreateCashCollectionViewModel : GetCashCollectionViewModel,IValidatableObject
    {
        public string CashCollectionID { get; set; }

        [Display(Name = "Payment Type")]
        public EnumPayType PaymentType { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Entry Date")]
        public string EntryDate { get; set; }
        public string RemindDate { get; set; }

        [Display(Name = "Amount")]
        public string Amount { get; set; }
        public string TempAmount { get; set; }

        [Display(Name = "Total Due")]
        public string CurrentDue { get; set; }

        [Display(Name = "Adjustment")]
        public string AdjustAmt { get; set; }
        public string TempAdjustAmt { get; set; }

        [Display(Name = "Due Amt.")]
        public string BalanceDue { get; set; }

        [Display(Name = "AccountNo")]
        public string AccountNo { get; set; }

        [Display(Name = "MBAccount No")]
        public string MBAccountNo { get; set; }

        [Display(Name = "BKashNo")]
        public string BKashNo { get; set; }

 

        [Display(Name = "Customer Name")]
        public string CustomerID { get; set; }
        public string EmployeeID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierID { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; }

        public string ConcernID { get; set; }

        public string CreatedBy { get; set; }

        public string CreateDate { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedDate { get; set; }

        [Display(Name = "Send SMS ")]
        public bool IsSmsEnable { get; set; } = true;

        [Display(Name = "Trans. Type")]
        [Required(ErrorMessage = "Trans Type is required.")]
        public EnumDropdownTranType Type { get; set; }
        [Display(Name = "Trans. Type")]
        [Required(ErrorMessage = "Trans Type is required.")]
        public EnumEmobileDropdownTranType EmobileType { get; set; }

        [Display(Name = "Trans. Type")]
        [Required(ErrorMessage = "Trans Type is required.")]
        public EnumDeliveryDropdownTranType DeliveryType { get; set; }
        [Display(Name = "Trans. Type")]
        [Required(ErrorMessage = "Trans Type is required.")]
        public EnumEmobileDeliveryDropdownTranType EmobileDeliveryType { get; set; }

        [Display(Name = "Interest Amt")]
        public string InterestAmt { get; set; }


        public ICollection<System.Web.Mvc.SelectListItem> CustomerItems { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> SupplierItems { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateCashCollectionViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }


    }    
}