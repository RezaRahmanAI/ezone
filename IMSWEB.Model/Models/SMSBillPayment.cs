using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSBillPayment : AuditTrailModel
    {
        [Key]
        public int BillPayID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime PaidFromDate { get; set; }
        public DateTime PaidToDate { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public int ConcernID { get; set; }
        public SisterConcern SisterConcern { get; set; }
    }
}

