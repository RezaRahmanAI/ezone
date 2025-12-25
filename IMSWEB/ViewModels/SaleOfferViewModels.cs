using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;

namespace IMSWEB
{

    public class GetSaleOfferViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Offer Code")]
        public string OfferCode { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        public string ToDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Offer Value")]
        public string OfferValue { get; set; }

        [Display(Name = "Offer Type")]
        public string OfferType { get; set; }

        [Display(Name = "Threshold Value")]
        public string ThresholdValue { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }


    }

    public class CreateSaleOfferViewModel : GetSaleOfferViewModel,IValidatableObject
    {
        public string OfferID { get; set; }

        [Display(Name = "Offer Code")]
        public string OfferCode { get; set; }

        [Display(Name = "Product")]
        public string ProductID { get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        public string ToDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Offer Value")]
        public string OfferValue { get; set; }

        [Display(Name = "Payment Type")]
        public EnumSalesOfferType OfferType { get; set; }

        [Display(Name = "Threshold Value")]
        public string ThresholdValue { get; set; }

        [Display(Name = "Status")]
        public EnumOfferStatus Status { get; set; }

        public string ConcernID { get; set; }

        public string CreatedBy { get; set; }

        public string CreateDate { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateSaleOfferViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}