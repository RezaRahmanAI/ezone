using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class RoleViewModelValidator : AbstractValidator<CreateRoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(company => company.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 250)
                .WithMessage("Name must be between 1 and 250 in length.");
        }
    }
}