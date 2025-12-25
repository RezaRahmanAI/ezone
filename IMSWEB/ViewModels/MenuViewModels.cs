using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class MenuViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Parent Menu")]
        public string ParentId { get; set; }

        public string Url { get; set; }

        [Display(Name ="Without View (For Report Only)")]
        public bool WithoutView { get; set; }

        [AllowHtml]
        public string Icon { get; set; }
        public int Sequence { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Menus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new MenuViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}