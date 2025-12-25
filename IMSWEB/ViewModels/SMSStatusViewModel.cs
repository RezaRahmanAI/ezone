using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMSWEB.Model;
using System.ComponentModel.DataAnnotations;
namespace IMSWEB
{
    public class SMSStatusViewModel
    {
        public string Code { get; set; }

        [Display(Name = "Type")]
        public EnumSMSType SMSFormateID { get; set; }

        [Display(Name = "Type")]
        public string SMSFormateDescription { get; set; }

        [Display(Name = "Status")]
        public EnumSMSSendStatus SendingStatus { get; set; }
        public int NoOfSMS { get; set; }
        public int? CustomerID { get; set; }
        public string ContactNo { get; set; }
        public DateTime EntryDate { get; set; }
        public string SMS { get; set; }
        public string ResponseMsg { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
    }
}