using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class ROrderViewModel
    {
        public CreateReturnOrderDetailViewModel RODetail { get; set; }
        public ICollection<CreateReturnOrderDetailViewModel> RODetails { get; set; }
        public CreateReturnOrderViewModel ReturnOrder { get; set; }
    }

    public class CreateReturnOrderDetailViewModel
    {
        public string ROrderDetailID { get; set; }

        public string ROrderID { get; set; }

        [Display(Name = "ProductID")]
        public string ProductID { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        [Display(Name = "Color")]
        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get; set; }


        [Display(Name = "S.Rate")]
        public string UnitPrice { get; set; }

        [Display(Name = "UTAmount")]
        public string UTAmount { get; set; }

        public ICollection<ROProductDetail> ROProductDetails { get; set; }

        public ICollection<Stock> Stocks { get; set; }

        public ICollection<StockDetail> StockDetails { get; set; }
    }

    public class CreateReturnOrderViewModel
    {
        public string ROrderID { get; set; }

        [Display(Name = "InvoiceNo")]
        public string InvoiceNo { get; set; }

        [Display(Name = "ReturnDate")]
        public string ReturnDate { get; set; }

        [Display(Name = "CustomerID")]
        public string CustomerID { get; set; }

        [Display(Name = "GrandTotal")]
        public string GrandTotal { get; set; }

        [Display(Name = "PaidAmount")]
        public string PaidAmount { get; set; }


    }

    public class GetReturnOrderViewModel
    {
        public string ROrderID { get; set; }

        [Display(Name = "ReturnDate")]
        public string ReturnDate { get; set; }

        [Display(Name = "InvoiceNo")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }


    }
}