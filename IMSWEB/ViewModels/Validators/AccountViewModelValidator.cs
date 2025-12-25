using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class ExternalLoginConfirmationViewModelValidator : AbstractValidator<ExternalLoginConfirmationViewModel>
    {
        public ExternalLoginConfirmationViewModelValidator()
        {
            RuleFor(account => account.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");
        }
    }

    public class VerifyCodeViewModelValidator : AbstractValidator<VerifyCodeViewModel>
    {
        public VerifyCodeViewModelValidator()
        {
            RuleFor(account => account.Provider)
                .NotEmpty()
                .WithMessage("Provider is required.");

            RuleFor(account => account.Code)
                .NotEmpty()
                .WithMessage("Code is required.");
        }
    }

    public class ForgotViewModelValidator : AbstractValidator<ForgotViewModel>
    {
        public ForgotViewModelValidator()
        {
            RuleFor(account => account.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");
        }
    }

    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(account => account.UserName)
                .NotEmpty()
                .WithMessage("User Name is required.");

            RuleFor(account => account.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(account => account.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");

            RuleFor(account => account.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Length(6, 100)
                .WithMessage("The Password must be at least 6 characters long.");
        }
    }

    public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator()
        {
            RuleFor(account => account.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");

            RuleFor(account => account.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Length(6, 100)
                .WithMessage("The Password must be at least 6 characters long.");
        }
    }

    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(account => account.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email must be a valid Email.");
        }
    }
}