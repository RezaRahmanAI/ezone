using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateGodownViewModelValidator : AbstractValidator<CreateGodownViewModel>
    {
        public CreateGodownViewModelValidator()
        {
            RuleFor(godown => godown.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 250)
                .WithMessage("Code must be between 1 and 250 in length.");

            RuleFor(godown => godown.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 350)
                .WithMessage("Name must be between 1 and 350 in length.");

            //RuleFor(company => company.ConcernId)
            //    .NotEmpty()
            //    .WithMessage("Sister concern is required.");
        }
    }
}