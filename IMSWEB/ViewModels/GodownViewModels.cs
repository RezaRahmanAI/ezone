using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateGodownViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [Display(Name = "Sister concern")]
        public string ConcernId { get; set; }

        public bool ISCommon { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateGodownViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}