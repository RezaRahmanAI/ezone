using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateEmployeeViewModelValidator : AbstractValidator<CreateEmployeeViewModel>
    {
        public CreateEmployeeViewModelValidator()
        {
            RuleFor(employee => employee.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .Length(1, 50)
                .WithMessage("Code must be between 1 and 50 in length.");

            RuleFor(employee => employee.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(1, 150)
                .WithMessage("Name must be between 1 and 150 in length.");

            RuleFor(employee => employee.DesignationName)
                .NotEmpty()
                .WithMessage("Designation is required.");

            //RuleFor(employee => employee.GrossSalary)
            //    .NotEmpty()
            //    .WithMessage("GrossSalary is required.");

            //RuleFor(employee => employee.SRDueLimit)
            //    .NotEmpty()
            //    .WithMessage("SR Due Limit is required.");
    
            //RuleFor(employee => employee.ConcernId)
            //    .NotEmpty()
            //    .WithMessage("Sister Concern is required.");
        }
    }
}