using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class  CategoryViewModelValidator : AbstractValidator<CreateCategoryViewModel>
    {
        public CategoryViewModelValidator()
        {
            RuleFor(company => company.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 150)
                .WithMessage("Code must be between 1 and 150 in length.");

            RuleFor(company => company.Name)
                .NotEmpty()
                .WithMessage("Description is required.")
                .Length(1, 350)
                .WithMessage("Description must be between 1 and 350 in length.");

            //RuleFor(company => company.ConcernId)
            //    .NotEmpty()
            //    .WithMessage("Sister concern is required.");
        }
    }
}