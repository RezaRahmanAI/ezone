using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateSystemInformationViewModelValidator : AbstractValidator<CreateSystemInformationViewModel>
    {
        public CreateSystemInformationViewModelValidator()
        {
            RuleFor(systemInformation => systemInformation.TelephoneNo)
                .NotEmpty()
                .WithMessage("ContactNo is required.")
                .Length(1, 250)
                .WithMessage("ContactNo must be between 1 and 250 in length.");

            RuleFor(systemInformation => systemInformation.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 350)
                .WithMessage("Name must be between 1 and 350 in length.");

            
        }
    }
}