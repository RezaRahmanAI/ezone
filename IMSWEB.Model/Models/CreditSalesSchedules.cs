using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class CreditSalesSchedule
    {
        [Key]
        public int CSScheduleID { get; set; }

        public DateTime MonthDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        [Column(TypeName = "money")]
        public decimal InstallmentAmt { get; set; }

        public DateTime PaymentDate { get; set; }

        public int CreditSalesID { get; set; }

        [Required]
        [StringLength(150)]
        public string PaymentStatus { get; set; }

        [Column(TypeName = "money")]
        public decimal? InterestAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal ClosingBalance { get; set; }

        public int ScheduleNo { get; set; }

        public string Remarks { get; set; }

        public int IsUnExpected { get; set; }

        public decimal HireValue { get; set; }
        public decimal NetValue { get; set; }
        public DateTime? RemindDate { get; set; }

        public virtual CreditSale CreditSale { get; set; }

        public decimal ExpectedInstallment { get; set; }
        public decimal LastPayAdjust { get; set; }

        public int BankTransID { get; set; }
        public decimal CardPaidAmount { get; set; }
        public int CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public int CreatedBy { get; set; }
    }
}
