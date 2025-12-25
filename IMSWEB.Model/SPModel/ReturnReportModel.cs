using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ReturnReportModel
    {
        public int SOrderID { get; set; }
        public int ConcernID { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnInvoice { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerMobile { get; set; }
        public string Remarks { get; set; }

        public string ProductName { get; set; }
        public string ReturnIMEI { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal ReturnAmount { get; set; }
    }
}
