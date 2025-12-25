using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ServiceChargeDetails
    {
        [Key]
        public int Id { get; set; }
        public int Month { get; set; }
        public decimal ExpectedServiceCharge { get; set; }
        public decimal PaidServiceCharge { get; set; }
        public bool IsPaid { get; set; }
        [StringLength(250)]
        public string TransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        [StringLength(50)]
        public string PaymentMobNo { get; set; }
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [StringLength(150)]
        public string PaymentId { get; set; }
        [StringLength(150)]
        public string PaymentReference { get; set; }
        [StringLength(100)]
        public string TransactionStatus { get; set; }
        [StringLength(10)]
        public string Currency { get; set; }
        [StringLength(20)]
        public string Intent { get; set; }
        [StringLength(30)]
        public string StatusCode { get; set; }
        [StringLength(500)]
        public string StatusMessage { get; set; }
        [StringLength(30)]
        public string ErrorCocde { get; set; }
        [StringLength(500)]
        public string ErrorMessage { get; set; }
        public int ServiceChargeId { get; set; }
        [ForeignKey("ServiceChargeId")]
        public virtual ServiceCharge ServiceCharge { get; set; }

        [StringLength(500)]
        public string ConcernName { get; set; }

    }
}
