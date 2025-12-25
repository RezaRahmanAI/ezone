using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class AdjustmentReportModel
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string ReceiptNo { get; set; }
        public decimal AdjutmentAmt { get; set; }
        public int CSScheduleID { get; set; }
    }
}