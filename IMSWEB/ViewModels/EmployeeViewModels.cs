using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class EmployeePickerModel
    {
        public EmployeePickerModel()
        {
            Employees = new List<GetEmployeeViewModel>();
        }
        public List<GetEmployeeViewModel> Employees { get; set; }
        public string EmployeeId { get; set; }
    }
    public class GetEmployeeViewModel
    {
        [Display(Name="Select")]
        public bool IsSelected { get; set; }
        public string Id { get; set; }

        public string Code { get; set; }
        [Display(Name = "Account No.")]
        public int? MachineEMPID { get; set; }

        public string Name { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        [Display(Name = "Picture")]
        public string PhotoPath { get; set; }

        [Display(Name = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }
        [Display(Name = "Religion")]
        public string ReligionName { get; set; }
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        public string GradeName { get; set; }
        public string Status { get; set; }


    }

    public class CreateEmployeeViewModel : GetEmployeeViewModel, IValidatableObject
    {
        [Display(Name = "Father Name")]
        public string FName { get; set; }

        [Display(Name = "Mother Name")]
        public string MName { get; set; }

        [Display(Name = "Email")]
        public string EmailId { get; set; }

        [Display(Name = "National Id")]
        public string NId { get; set; }

        [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }

        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Gross Salary")]
        public string GrossSalary { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "SR Due Limit")]
        public string SRDueLimit { get; set; }

        [Display(Name = "Sister Concern")]
        public string ConcernId { get; set; }
        public int? DepartmentID { get; set; }
        [Display(Name = "Location")]
        public int? LocationID { get; set; }

        [Display(Name = "Religion")]
        public int? ReligionID { get; set; }

        [Display(Name = "Gender")]
        public int? GenderID { get; set; }

        [Display(Name = "Grade")]
        public int? GradeID { get; set; }
        [Display(Name = "Payment Mode")]
        public int? PaymentMode { get; set; }
        public int? IsEligibleOT { get; set; }
        [Display(Name = "End Of Contract Date")]
        public DateTime? EndOfContractDate { get; set; }
        public DateTime? DateOfConfirmation { get; set; }
        [Display(Name = "Account No.")]
        public int MachineEMPID { get; set; }
        [Display(Name = "Designation")]
        public ICollection<SelectListItem> Designations { get; set; }
        [Display(Name = "Religion")]
        public ICollection<SelectListItem> Religions { get; set; }

        [Display(Name = "Department")]
        public ICollection<SelectListItem> Departments { get; set; }
        public List<GetEmployeeViewModel> Employees { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateEmployeeViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}