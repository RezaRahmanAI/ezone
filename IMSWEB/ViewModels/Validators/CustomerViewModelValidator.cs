using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateCustomerViewModelValidator : AbstractValidator<CreateCustomerViewModel>
    {
        public CreateCustomerViewModelValidator()
        {
            RuleFor(customer => customer.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 50)
                .WithMessage("Code must be between 1 and 50 in length.");

            RuleFor(customer => customer.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 150)
                .WithMessage("Name must be between 1 and 150 in length.");

            RuleFor(customer => customer.ContactNo)
                .NotEmpty()
                .WithMessage("Contact No. is required.")
                .Length(1, 150)
                .WithMessage("Contact No. must be between 1 and 150 in length.");
            //RuleFor(customer => customer.TotalDue)
            //    .NotEmpty()
            //    .WithMessage("Total Due is required.");

            RuleFor(customer => customer.CustomerType)
                .NotEmpty()
                .WithMessage("Customer Type is required.");

            //RuleFor(customer => customer.EmployeeId)
            //    .NotEmpty()
            //    .WithMessage("Employee is required.");

            //RuleFor(customer => customer.CusDueLimit)
            //    .NotEmpty()
            //    .WithMessage("Due Limit is required.");

            //RuleFor(customer => customer.ConcernId)
            //    .NotEmpty()
            //    .WithMessage("Concern is required.");
        }
    }
}