using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateCategoryViewModel : IValidatableObject
    {
        public CreateCategoryViewModel()
        {
            Parents = new List<ParentCategoryViewModel>();
        }
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [Display(Name = "Sister concern")]
        public string ConcernId { get; set; }

        [Display(Name = "Parent Category")]
        [Required(ErrorMessage ="Parent is required.")]
        public int PCategoryID { get; set; }
        public List<ParentCategoryViewModel> Parents { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CategoryViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}