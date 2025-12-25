using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateBankTransactionViewModelValidator : AbstractValidator<CreateBankTransactionViewModel>
    {
        public CreateBankTransactionViewModelValidator()
        {
            //RuleFor(bnt => bnt.BankID)
            //    .NotEmpty()
            //    .WithMessage("BankName is required.")
            //    .Length(1, 50)
            //    .WithMessage("BankName must be between 1 and 50 in length.");

            //RuleFor(product => product.ProductName)
            //    .NotEmpty()
            //    .WithMessage("Name is required.");

            //RuleFor(product => product.PicturePath)
            //    .NotEmpty()
            //    .WithMessage("Picture is required.")
            //    .Length(1, 250)
            //    .WithMessage("Picture Path must be between 1 and 250 in length.");

            //RuleFor(product => product.CategoryID)
            //    .NotEmpty()
            //    .WithMessage("Category is required.");

            //RuleFor(product => product.ModelID)
            //    .NotEmpty()
            //    .WithMessage("Model is required.");

            //RuleFor(product => product.UnitType)
            //    .NotEmpty()
            //    .WithMessage("Unit type is required.");

            //RuleFor(product => product.PWDiscount)
            //    .NotEmpty()
            //    .WithMessage("Discount is required.");
        }
    }
}