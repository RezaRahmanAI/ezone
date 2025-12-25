using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GetSupplierViewModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [Display(Name="Contact No.")]
        public string ContactNo { get; set; }

        [Display(Name="Picture")]
        public string PhotoPath { get; set; }

        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [Display(Name = "Total Due")]
        public string TotalDue { get; set; }
    }

    public class CreateSupplierViewModel : GetSupplierViewModel, IValidatableObject
    {
        public string Address { get; set; }

        [Display(Name = "Sister Concern")]
        public string ConcernId { get; set; }

        [Display(Name = "Opening Due")]
        public string OpeningDue { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateSupplierViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}