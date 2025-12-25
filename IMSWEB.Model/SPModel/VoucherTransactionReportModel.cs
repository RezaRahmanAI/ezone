using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model

{
    public class VoucherTransactionReportModel
    {
        public int VoucherTransactionID { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherNo { get; set; }
        public string DebitHead { get; set; }
        public decimal DebitAmount { get; set; }
        public string CreditHead { get; set; }
        public decimal CreditAmount { get; set; }
        public string Narration { get; set; }
        public string ModuleType { get; set; }
        public string Particulars { get; set; }
        public decimal Balance { get; set; }
        public decimal Opening { get; set; }
        public string ItemName { get; set; }
        public string ProjectName { get; set; }
        public decimal Qty { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string CollectionType { get; set; }
        public decimal CollectionAmount { get; set; }
        public string VoucherType { get; set; }
    }
}
