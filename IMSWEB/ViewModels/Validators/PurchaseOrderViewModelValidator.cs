using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreatePurchaseOrderDetailViewModelValidator : AbstractValidator<CreatePurchaseOrderDetailViewModel>
    {
        public CreatePurchaseOrderDetailViewModelValidator()
        {
            RuleFor(product => product.UnitPrice)
                .NotEmpty()
                .WithMessage("Unit Price is required.");

            RuleFor(product => product.MRPRate)
                .NotEmpty()
                .WithMessage("MRP Rate is required.");

            RuleFor(product => product.Quantity)
                .NotEmpty()
                .WithMessage("Quantity is required.");

            RuleFor(product => product.TAmount)
                .NotEmpty()
                .WithMessage("Total Amount is required.");

            RuleFor(product => product.PPDisPercentage)
                .NotEmpty()
                .WithMessage("Discount is required.");

            RuleFor(product => product.PPDiscountAmount)
                .NotEmpty()
                .WithMessage("Discount % is required.");

            RuleFor(product => product.GodownID)
                .NotEmpty()
                .WithMessage("Godown is required.");

        }
    }

    public class CreatePurchaseOrderViewModelValidator : AbstractValidator<CreatePurchaseOrderViewModel>
    {
        public CreatePurchaseOrderViewModelValidator()
        {
            RuleFor(product => product.ChallanNo)
                .NotEmpty()
                .WithMessage("Challan No. is required.");

            //RuleFor(product => product.GrandTotal)
            //    .NotEmpty()
            //    .WithMessage("Grand Total is required.");

            RuleFor(product => product.PPDiscountAmount)
                .NotEmpty()
                .WithMessage("PP Discount is required.");

            RuleFor(product => product.TotalDiscountPercentage)
                .NotEmpty()
                .WithMessage("Total Discount is required.");

            RuleFor(product => product.TotalDiscountAmount)
                .NotEmpty()
                .WithMessage("Total Discount % is required.");

            RuleFor(product => product.NetDiscount)
                .NotEmpty()
                .WithMessage("Net Discount is required.");

            RuleFor(product => product.TotalAmount)
                .NotEmpty()
                .WithMessage("Net Total is required.");

            RuleFor(product => product.RecieveAmount)
                .NotEmpty()
                .WithMessage("Pay Amount is required.");

            //RuleFor(product => product.PaymentDue)
            //    .NotEmpty()
            //    .WithMessage("Payment Due is required.");

            //RuleFor(product => product.TotalDue)
            //    .NotEmpty()
            //    .WithMessage("Total Due is required.");

            RuleFor(product => product.LabourCost)
                .NotEmpty()
                .WithMessage("Labour Cost is required.");
        }
    }
}