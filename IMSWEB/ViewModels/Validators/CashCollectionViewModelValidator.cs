using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateCashCollectionViewModelValidator : AbstractValidator<CreateCashCollectionViewModel>
    {
        public CreateCashCollectionViewModelValidator()
        {
            //RuleFor(item => item.BankName)
            //    .NotEmpty()
            //    .WithMessage("Bank Name is required.")
            //    .Length(1, 150)
            //    .WithMessage("Bank Name must be between 1 and 150 in length.");

            RuleFor(item => item.PaymentType)
                .NotEmpty()
                .WithMessage("Payment Type is required.");
        }
    }
}