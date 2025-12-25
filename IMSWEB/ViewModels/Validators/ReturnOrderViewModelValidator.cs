using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateReturnOrderDetailViewModelValidator : AbstractValidator<CreateReturnOrderDetailViewModel>
    {
        public CreateReturnOrderDetailViewModelValidator()
        {
            RuleFor(product => product.UnitPrice)
                .NotEmpty()
                .WithMessage("Unit Price is required.");

            RuleFor(product => product.Quantity)
                .NotEmpty()
                .WithMessage("Quantity is required.");

        }
    }

    public class CreateReturnOrderViewModelValidator : AbstractValidator<CreateReturnOrderViewModel>
    {
        public CreateReturnOrderViewModelValidator()
        {
            RuleFor(product => product.InvoiceNo)
                .NotEmpty()
                .WithMessage("Invoice No. is required.");

            RuleFor(product => product.CustomerID)
                .NotEmpty()
                .WithMessage("Customer is required.");

        }
    }
}