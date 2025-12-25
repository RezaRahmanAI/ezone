using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateExpenseItemViewModelValidator : AbstractValidator<CreateExpenseItemViewModel>
    {
        public CreateExpenseItemViewModelValidator()
        {
            RuleFor(item => item.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 150)
                .WithMessage("Code must be between 1 and 150 in length.");

            RuleFor(item => item.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 350)
                .WithMessage("Name must be between 1 and 350 in length.");

            RuleFor(item => item.Status)
                .NotEmpty()
                .WithMessage("Status is required.");
        }
    }
}