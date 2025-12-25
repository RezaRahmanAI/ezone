using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Expenditure : IModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpenditureID { get; set; }

        public DateTime EntryDate { get; set; }

        public string Purpose { get; set; }

        public decimal Amount { get; set; }

        public int ExpenseItemID { get; set; }

        public string VoucherNo { get; set; }

        public int ConcernID { get; set; }
        [ForeignKey("User")]
        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int IsBankTransaction { get; set; }
        public EnumSubCustomerType CustomerType { get; set; }

        public virtual ExpenseItem ExpenseItem { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        public EnumWFStatus WFStatus { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int CashInHandReportStatus { get; set; }
    }
}
