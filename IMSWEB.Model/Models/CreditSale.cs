using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class CreditSale : IModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CreditSale()
        {
            CreditSaleDetails = new HashSet<CreditSaleDetails>();
            CreditSalesSchedules = new HashSet<CreditSalesSchedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CreditSalesID { get; set; }

        [Required]
        [StringLength(150)]
        public string InvoiceNo { get; set; }

        public int CustomerID { get; set; }

        [Column(TypeName = "money")]
        public decimal TSalesAmt { get; set; }

        public int NoOfInstallment { get; set; }

        [Column(TypeName = "money")]
        public decimal IntallmentPrinciple { get; set; }

        public DateTime IssueDate { get; set; }

        [StringLength(250)]
        public string UserName { get; set; }

        [Column(TypeName = "money")]
        public decimal Remaining { get; set; }

        public decimal InterestRate { get; set; }

        public decimal InterestAmount { get; set; }

        public DateTime SalesDate { get; set; }

        [Column(TypeName = "money")]
        public decimal DownPayment { get; set; }

        [Column(TypeName = "money")]
        public decimal? WInterestAmt { get; set; }

        [Column(TypeName = "money")]
        public decimal? FixedAmt { get; set; }

        public EnumSalesType IsStatus { get; set; }

        public int UnExInstallment { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        [Column(TypeName = "money")]
        public decimal NetAmount { get; set; }

        public bool ISUnexpected { get; set; }

        public string Remarks { get; set; }

        public int Status { get; set; }

        public decimal VATPercentage { get; set; }

        public decimal VATAmount { get; set; }

        public decimal ProcessingFee { get; set; }
        public int ConcernID { get; set; }
        public int IsReturn { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public decimal TotalOffer { get; set; }
        public decimal PenaltyInterest { get; set; }
        public decimal LastPayAdjAmt { get; set; }
        public string InstallmentPeriod { get; set; }
        public int BankTransID { get; set; }
        public decimal CardPaidAmount { get; set; }
        public int CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public int IsWeekly { get; set; }
        public string GuarName { get; set; }
        public string GuarContactNo { get; set; }
        public string GuarAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditSaleDetails> CreditSaleDetails { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditSalesSchedule> CreditSalesSchedules { get; set; }

    }
}
