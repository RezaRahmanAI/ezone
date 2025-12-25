using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    [Table("HireSalesReturnCustomerDueAdjustment")]
    public class HireSalesReturnCustomerDueAdjustment
    {
        [Key]
        public int Id { get; set; }
        public decimal AdjDue { get; set; }
        public decimal TotalRemainingDue { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CreditSalesId { get; set; }
        public int CustomerId { get; set; }
        public int ConcernId { get; set; }
        public decimal ToCustomerPayAmt { get; set; }
        public string Remarks { get; set; }
        public string MemoNo { get; set; }

        [ForeignKey("CreditSalesId")]
        public virtual CreditSale CreditSale { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("ConcernId")]
        public virtual SisterConcern SisterConcern { get; set; }
    }
}
