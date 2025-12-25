using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateExpenditureViewModelValidator : AbstractValidator<CreateExpenditureViewModel>
    {
        public CreateExpenditureViewModelValidator()
        {
            RuleFor(item => item.Purpose)
                .NotEmpty()
                .WithMessage("Purpose is required.");

            RuleFor(item => item.Amount)
                .NotEmpty()
                .WithMessage("Amount is required.");

            RuleFor(item => item.EntryDate)
               .NotEmpty()
               .WithMessage("EntryDate is required.");

           
        }
    }
}