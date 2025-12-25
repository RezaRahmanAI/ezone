using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class DOViewModel
    {
        public DOViewModel()
        {
            Details = new List<DODetailViewModel>();
        }
        public string DOID { get; set; }
        public DateTime Date { get; set; }

        [Display(Name = "OS No.")]
        public string DONo { get; set; }

        [Display(Name = "Customer")]
        public int CustomerID { get; set; }
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        public string Remarks { get; set; }

        //[Display(Name = "Total Amt")]
        //[Range(0, double.MaxValue, ErrorMessage = "Total Amt. is required.")]
        //public decimal TotalAmt { get; set; }

        [Display(Name = "Total Amt")]
        //[Range(0, double.MaxValue, ErrorMessage = "Total Amt. is required.")]
        public decimal TotalAmt { get; set; }

        [Display(Name = "Paid Amt")]
        public decimal PaidAmt { get; set; }

        //[Display(Name = "Paid Amt")]
        //[Range(0, double.MaxValue, ErrorMessage = "Paid Amt. is required.")]
        //public decimal PaidAmt { get; set; }

        [Display(Name = "Payment Due")]
        public decimal PaymentDue { get; set; }

        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        [Display(Name = "Color")]
        public string ColorID { get; set; }
        [Display(Name = "Color")]
        public string TempColorID { get; set; }


        //discount area start here 

        [Display(Name = "Flat Dis. Per.")]
        public decimal TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis. Amt")]
        public decimal TotalDiscountAmount { get; set; }


        [Display(Name = "Total Dis.")]
        public decimal NetDiscount { get; set; }
        public decimal PPDiscountAmount { get; set; } //To store Net discount in view
        public string tempFlatDiscountAmount { get; set; } //To store Flat discount in view


        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }
        public string tempNetDiscount { get; set; }
        //public string tempFlatDiscountAmount { get; set; }
        public decimal TempTootalAmount { get; set; } //To store Flat discount in view
        [Display(Name = "PP Discount")]
        public string PPDisAmt { get; set; }


        //discount area end here 

        public DODetailViewModel Detail { get; set; }
        public List<DODetailViewModel> Details { get; set; }
        public int RoundingID { get; set; }
    }

    public class DODetailViewModel
    {
        public string DODID { get; set; }

        [Display(Name = "Product")]
        public string ProductID { get; set; }

        [Display(Name = "Color")]
        public string ColorID { get; set; }

        [Display(Name = "Color Code")]
        public string ColorCode { get; set; }

        [Display(Name = "Color Name")]
        public string ColorName { get; set; }

        [Display(Name = "Order Qty")]
        public decimal DOQty { get; set; }

        [Display(Name = "Given Qty")]
        public decimal GivenQty { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        [Display(Name = "MRP Rate")]
        public decimal MRP { get; set; }

        [Display(Name = "MRP Rate")]
        public decimal DDLiftingPrice { get; set; }

        [Display(Name = "DDL.Price")]
        public decimal DDLiftingPrices { get; set; }

        [Display(Name = "Net Amt")]
        public decimal TotalAmt { get; set; }
        [Display(Name = "Product")]
        public object ProductName { get; internal set; }
        [Display(Name = "COde")]
        public string ProductCode { get; internal set; }

        [Display(Name = "Prev. Stock")]
        public string PreviousStock { get; set; }
        public string DOID { get; internal set; }

        [Display(Name = "Total Price")]
        public decimal TotalSoilPrice { get; set; }

        [Display(Name = "Total Dis.")]
        public decimal NetDiscount { get; set; }

        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }

        [Display(Name = "Flat Dis. Amt")]
        public decimal TotalDiscountAmount { get; set; }
        [Display(Name = "Dis.Per.")]
        public decimal PPDisPercent { get; set; }

        [Display(Name = "Dis. Amt")]
        public decimal PPDisAmt { get; set; }
        [Display(Name = "Pur. Rate")]
        public decimal UnitPrice { get; set; }

        public decimal TempTootalAmount { get; set; } //To store Flat discount in view

        public decimal TotalDiscountPercentage { get; set; }

    }
}