using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO.Bkash
{
    public class CreatePaymentResponseTO
    {
        public string paymentID { get; set; }
        public string paymentURL { get; set; }
        public string statusCode { get; set; }
        public string bkashURL { get; set; }
        public string callbackURL { get; set; }
        public string transactionStatus { get; set; }
        public string paymentCreateTime { get; set; }
        public string statusMessage { get; set; }
    }
}
