using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateColorViewModelValidator : AbstractValidator<CreateColorViewModel>
    {
        public CreateColorViewModelValidator()
        {
            RuleFor(color => color.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 250)
                .WithMessage("Code must be between 1 and 250 in length.");

            RuleFor(color => color.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 250)
                .WithMessage("Name must be between 1 and 250 in length.");
        }
    }
}