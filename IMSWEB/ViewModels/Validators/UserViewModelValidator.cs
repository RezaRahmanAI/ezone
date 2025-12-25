using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
        public CreateUserViewModelValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");

            RuleFor(user => user.UserName)
                .NotEmpty()
                .WithMessage("User name is required.");

            //RuleFor(user => user.Password)
            //    .NotEmpty()
            //    .WithMessage("Password is required.")
            //    .Length(6, 100)
            //    .WithMessage("Password must be at least 6 characters long.");

            //RuleFor(user => user.RePassword)
            //    .Equal(user => user.Password)
            //    .WithMessage("Password doesn't matched.");

            RuleFor(user => user.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Length(1, 12)
                .WithMessage("Phone number must be between 1 and 12 in length.");

            RuleFor(user => user.ConcernId)
                .NotEmpty()
                .WithMessage("Sister concern is required.");

            RuleFor(user => user.RoleName)
                .NotEmpty()
                .WithMessage("Role is required.");
        }
    }

    public class UpdateUserInfoViewModelValidator : AbstractValidator<UpdateUserInfoViewModel>
    {
        public UpdateUserInfoViewModelValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");

            RuleFor(user => user.UserName)
                .NotEmpty()
                .WithMessage("User name is required.");

            //RuleFor(user => user.Password)
            //    .NotEmpty()
            //    .WithMessage("Password is required.")
            //    .Length(6, 100)
            //    .WithMessage("Password must be at least 6 characters long.");

            //RuleFor(user => user.RePassword)
            //    .Equal(user => user.Password)
            //    .WithMessage("Password doesn't matched.");

            RuleFor(user => user.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Length(1, 12)
                .WithMessage("Phone number must be between 1 and 12 in length.");
        }
    }
}