using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSPaymentMasterDetails : AuditTrailModel
    {
        [Key]
        public int SMSPaymentDetailsID { get; set; }
        public int SMSPaymentMasterID { get; set; }
        public string ReceiptNo { get; set; }
        public decimal RecAmount { get; set; }
        public DateTime RecDate { get; set; }
        public string PaymentMobNo { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public string StatusMessage { get; set; }
        public string ErrorCocde { get; set; }
        public string ErrorMessage { get; set; }

        [StringLength(150)]
        public string PaymentId { get; set; }
        [StringLength(150)]
        public string PaymentReference { get; set; }
        //public int ConcernID { get; set; }
        //public SisterConcern SisterConcern { get; set; }
        [StringLength(500)]
        public string ConcernName { get; set; }

    }
}

