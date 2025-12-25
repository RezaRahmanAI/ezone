using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateSisterConcernViewModelValidator : AbstractValidator<CreateSisterConcernViewModel>
    {
        public CreateSisterConcernViewModelValidator()
        {
            RuleFor(sisterconcern => sisterconcern.ContactNo)
                .NotEmpty()
                .WithMessage("ContactNo is required.")
                .Length(1, 250)
                .WithMessage("ContactNo must be between 1 and 250 in length.");

            RuleFor(sisterconcern => sisterconcern.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 350)
                .WithMessage("Name must be between 1 and 350 in length.");

            
        }
    }
}