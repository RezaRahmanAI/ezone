using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ReplacementReportModel
    {
        public int SOrderID { get; set; }
        public DateTime SalesDate { get; set; }
        public DateTime PODate { get; set; }
        public string Invoice { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnInvoice { get; set; }
        public string  CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerMobile { get; set; }
        public string DamageProudct { get; set; }
        public string DamageIMEI { get; set; }
        public decimal DamageQty { get; set; }
        public decimal DamageSalesRate { get; set; }
        public string ReplaceProduct { get; set; }
        public string ReplaceIMEI { get; set; }
        public decimal ReplaceQty { get; set; }
        public decimal ReplaceRate { get; set; }
        public string  Remarks { get; set; }
    }
}
