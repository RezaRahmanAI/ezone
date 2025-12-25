using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateDesignationViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        [Display(Name="Commission(%)")]
        public decimal? CommissionPercent { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateDesignationViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}