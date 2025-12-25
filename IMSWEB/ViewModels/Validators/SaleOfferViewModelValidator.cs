using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IMSWEB
{
    public class CreateSaleOfferViewModelValidator:AbstractValidator<CreateSaleOfferViewModel>
    {
        public CreateSaleOfferViewModelValidator()
        {
            RuleFor(saleOffer => saleOffer.OfferValue)
                .NotEmpty()
                .WithMessage("Offer Value is required.")
                .Length(1, 250)
                .WithMessage("Offer Value must be greater than one.");

            RuleFor(sisterconcern => sisterconcern.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .Length(1, 350)
                .WithMessage("Description must be between 1 and 350 in length.");

            
        }
    }
}