using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class CreateBankLoanVM
    {
        public int Id { get; set; }
        [Display(Name = "Loan No.")]
        public string Code { get; set; }
        [Display(Name = "Date")]
        public DateTime LoanDate { get; set; }
        [Display(Name = "Amount")]
        public decimal PrincipleLoanAmount { get; set; }
        [Display(Name = "Interset %")]
        public decimal InterestPercentage { get; set; }
        [Display(Name = "Processing Fee %")]
        public decimal ProcessingFeePercentage { get; set; }
        [Display(Name = "Total")]
        public decimal TotalLoanAmount { get; set; }
        [Display(Name = "No Of Installment")]
        public int NoOfInstallment { get; set; }
        [Display(Name = "Installment Size")]
        public decimal InstallmentSize { get; set; }
        [Display(Name = "Penalty %")]
        public decimal PenaltyChargePercentage { get; set; }
        public int ConcernId { get; set; }
        [Display(Name = "Bank")]
        public int BankId { get; set; }
        [Display(Name = "Bank")]
        public string BankName { get; set; }
        public decimal SDPS { get; set; }
        public decimal Savings { get; set; }

        public List<SelectListItem> Banks { get; set; }
        public List<CreateBankLoanDetailsVM> LoanDetails { get; set; }
    }

    public class CreateBankLoanDetailsVM
    {
        public int Id { get; set; }
        public int BankLoanId { get; set; }
        [MaxLength(10)]
        public string Status { get; set; }
        public DateTime InstallmentDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal ExpectedInstallmentAmount { get; set; }
        public decimal PenaltyChargeAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal InterestAmount { get; set; }
        public int ScheduleNo { get; set; }
        [MaxLength(500)]
        public string Remarks { get; set; }
        public decimal LastPayAdjustment { get; set; }
    }
}