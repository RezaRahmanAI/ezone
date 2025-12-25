using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class SMSBillPaymentViewModel
    {
        public int? BillPayID { get; set; }

        [Display(Name="Receipt No.")]
        public string ReceiptNo { get; set; }

        [Display(Name = "Paid From Date")]
        public DateTime PaidFromDate { get; set; }

        [Display(Name = "Paid To Date")]
        public DateTime PaidToDate { get; set; }

        [Display(Name = "Recharge Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Recharge Amount")]
        public decimal SmsBalance { get; set; }
        public string Remarks { get; set; }
    }
}