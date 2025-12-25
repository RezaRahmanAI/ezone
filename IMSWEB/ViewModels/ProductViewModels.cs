using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;
using PagedList;
namespace IMSWEB
{
    public class CreateProductViewModel : IValidatableObject
    {
        public string ProductId { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Category")]
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        [Display(Name = "Company")]
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }

        [Display(Name = "Unit Type")]
        public EnumUnitType UnitType { get; set; }

        [Display(Name = "Discount")]
        public string PWDiscount { get; set; }

        [Display(Name = "Discount From")]
        public DateTime? DisDurationFDate { get; set; }

        [Display(Name = "Discount To")]
        public DateTime? DisDurationToDate { get; set; }
        [Display(Name = "Compressor Warrenty")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel Warrenty")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor Warrenty")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts Warrenty")]
        public string SparePartsWarrentyMonth { get; set; }
        [Display(Name = "Service Warrenty")]
        public string ServiceWarrentyMonth { get; set; }
        [Display(Name = "Product Type")]
        public string Compressor { get; set; }
        public string Motor { get; set; }
        public string Panel { get; set; }
        public string Service { get; set; }
        public string SpareParts { get; set; }
        public decimal MRP { get; set; }
        [Display(Name = "Purchase Rate")]
        public decimal ECOMRP { get; set; }
        public decimal RP { get; set; }

        [Display(Name = "Product Type")]
        public EnumProductType ProductType { get; set; }

        [Display(Name ="Parent Category")]
        public string PCategoryID { get; set; }
        public string PCategoryName { get; set; }

        [Display(Name ="Pur. Rate")]
        public decimal PurchaseRate { get; set; }
        [Display(Name = "M. Warranty")]
        public string UserInputWarranty { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateProductViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class  GetProductViewModel 
    {
        public int ProductId { get; set; }
        public int CategoryID { get; set; }
        public int StockDetailsId { get; set; }

        [Display(Name = "Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }

        public int ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Company")]



        public string CompanyName { get; set; }

        [Display(Name = "Discount")]
        public decimal PWDiscount { get; set; }

        [Display(Name = "Pre. Stock")]
        public decimal PreStock { get; set; }

        [Display(Name = "IMENo/Barcode")]
        public string IMENo { get; set; }

        [Display(Name = "MRP Rate")]
        public decimal MRPRate { get; set; }
        [Display(Name = "MRP Rate 12")]
        public decimal MRPRate12 { get; set; }
        [Display(Name = "RP")]
        public decimal RP { get; set; }



        public decimal CashSalesRate { get; set; }

        public string OfferDescription { get; set; }

        public int ProductType { get; set; }
        [Display(Name = "Compressor Warrenty(Month)")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel Warrenty(Month)")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor Warrenty(Month)")]



        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts Warrenty(Month)")]
        public string SparePartsWarrentyMonth { get; set; }

        [Display(Name = "Service Warrenty(Month)")]
        public string ServiceWarrentyMonth { get; set; }
        public bool IsSelect { get; set; }
        public EnumStatus Status { get; set; }

        [Display(Name = "M. Warranty")]
        public string UserInputWarranty { get; set; }

        public int GodownID { get; set; }
        public decimal Quantity { get; set; }
        public string GodownName { get; set; }

        public decimal PRate { get; set; }
        [Display(Name = "Service Warrenty(Month)")]
        public string Service { get; set; }
        [Display(Name = "Date")]
        public string OrderDate { get; set; }
    }
}