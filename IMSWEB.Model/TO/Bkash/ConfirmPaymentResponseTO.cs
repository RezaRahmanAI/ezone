using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO.Bkash
{
    public class ConfirmPaymentResponseTO
    {
        public string paymentID { get; set; }
        public string customerMsisdn { get; set; }
        public string payerReference { get; set; }
        public string paymentExecuteTime { get; set; }
        public string trxID { get; set; }
        public string transactionStatus { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string intent { get; set; }
        public string merchantInvoiceNumber { get; set; }
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
