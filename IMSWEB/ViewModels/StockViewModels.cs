using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GetStockViewModel
    {
        public string Id { get; set; }

        public string Code { get; set; }
        public string ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "L.P. Price")]
        public decimal LPPrice { get; set; }

        [Display(Name = "MRP Rate")]
        public decimal MRPRate { get; set; }
        [Display(Name = "Color")]
        public string ColorID { get; set; }
        public string ColorName { get; set; }
        [Display(Name = "Cash Sales Rate")]
        public decimal SalesRate { get; set; }

        [Display(Name = "Credit Sales Rate 6")]
        public decimal CreditSalesRate { get; set; }

        [Display(Name = "Credit Sales Rate 3")]
        public decimal CreditSalesRate3 { get; set; }

        [Display(Name = "Credit Sales Rate 12")]
        public decimal CreditSalesRate12 { get; set; }

        public string GodownName { get; set; }

    }

    public class GetStockDetailViewModel
    {
        public string SDetailID { get; set; }

        public string StockCode { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }


        [Display(Name = "IMEI NO")]
        public string IMENO { get; set; }

        public EnumStockStatus Status { get; set; }
        public string ColorName { get; set; }
        public string GodownName { get; set; }
    }

}