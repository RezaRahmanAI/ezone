using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateSupplierViewModelValidator : AbstractValidator<CreateSupplierViewModel>
    {
        public CreateSupplierViewModelValidator()
        {
            RuleFor(supplier => supplier.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 50)
                .WithMessage("Code must be between 1 and 50 in length.");

            RuleFor(supplier => supplier.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 150)
                .WithMessage("Name must be between 1 and 150 in length.");

            RuleFor(supplier => supplier.ContactNo)
                .NotEmpty()
                .WithMessage("Contact No. is required.")
                .Length(1, 150)
                .WithMessage("Contact No. must be between 1 and 150 in length.");

            //RuleFor(supplier => supplier.TotalDue)
            //    .NotEmpty()
            //    .WithMessage("Total Due is required.");

            //RuleFor(supplier => supplier.ConcernId)
            //    .NotEmpty()
            //    .WithMessage("Sister Concern is required.");
        }
    }
}