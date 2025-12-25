using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB.ViewModels
{
    public class GetReplacementOrderListVM
    {
        public int SOrderID { get; set; }
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name="Sales Date")]
        public DateTime SalesDate { get; set; }

        [Display(Name="Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmout { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}