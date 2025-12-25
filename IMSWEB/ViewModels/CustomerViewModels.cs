using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GetCustomerViewModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string MemberID { get; set; }

        public string Name { get; set; }

        [Display(Name="Contact No.")]
        public string ContactNo { get; set; }

        [Display(Name = "Profession")]
        public string Profession { get; set; }

        [Display(Name="Picture")]
        public string PhotoPath { get; set; }

        [Display(Name = "Total Due")]
        public string TotalDue { get; set; }

        [Display(Name = "Credit Due")]
        public decimal CreditDue { get; set; }

        [Display(Name = "Customer Type")]
        public EnumCustomerType CustomerType { get; set; }

        [Display(Name = "Due Limit")]
        public string CusDueLimit { get; set; }
        public string Address { get; set; }
    }

    public class CreateCustomerViewModel : GetCustomerViewModel, IValidatableObject
    {
        [Display(Name = "Father Name")]
        public string FName { get; set; }

        [Display(Name = "Company")]
        public string CompanyId { get; set; }

        [Display(Name = "Opening Due")]
        public string OpeningDue { get; set; }

        [Display(Name = "Email")]
        public string EmailId { get; set; }

        [Display(Name = "National Id")]
        public string NId { get; set; }

        public string Address { get; set; }

        [Display(Name = "Guar. Name")]
        public string RefName { get; set; }

        [Display(Name = "Guar. Contact No.")]
        public string RefContact { get; set; }

        [Display(Name = "Guar. Father Name")]
        public string RefFName { get; set; }

        [Display(Name = "Guar. Address")]
        public string RefAddress { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }

        [Display(Name = "Concern")]
        public string ConcernId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentID { get; set; }
        public string Remarks { get; set; }
        public List<GetEmployeeViewModel> Employees { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateCustomerViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }


    public class GetAdjustmentViewModel
    {
        public string Id { get; set; }       

        [Display(Name = "Adjustment Type")]
        public EnumAdjustmentType AdjustmentType { get; set; }

    }

}