using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateUserViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Display(Name ="User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Re-Password")]
        public string RePassword { get; set; }

        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Sister Concern")]
        public string ConcernId { get; set; }

        [Display(Name = "Sister Concern")]
        public string ConcernName { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public string Status { get; set; }
        public string EmployeeId { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> VMSisterConcerns { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> VMRoles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateUserViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class UpdateUserInfoViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Re-Password")]
        public string RePassword { get; set; }

        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new UpdateUserInfoViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}