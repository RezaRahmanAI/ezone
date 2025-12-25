using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class MenuViewModelValidator : AbstractValidator<MenuViewModel>
    {
        public MenuViewModelValidator()
        {
            RuleFor(menu => menu.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .Length(2, 300)
                .WithMessage("Title must be between 2 and 300 chars in length.");

            RuleFor(menu => menu.Url)
                .NotEmpty()
                .WithMessage("Url is required.")
                .Length(2, 300)
                .WithMessage("Url must be between 2 and 500 chars in length."); ;

            RuleFor(menu => menu.Description)
                .Length(0, 400)
                .WithMessage("Description can be max 400 chars in length.");
        }
    }
}