using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class CreditSalesOrderViewModel
    {
        public CreateCreditSalesOrderDetailViewModel SODetail { get; set; }
        public ICollection<CreateCreditSalesOrderDetailViewModel> SODetails { get; set; }
        public ICollection<CreateCreditSalesSchedules> SOSchedules { get; set; }
        public CreateCreditSalesOrderViewModel SalesOrder { get; set; }
    }

    public class CreateCreditSalesOrderDetailViewModel
    {
        public string SODetailId { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }
        public int GodownID{ get; set; }

        public string StockDetailId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Sales Rate")]
        public string UnitPrice { get; set; }

        [Display(Name = "Interest(%).")]
        public string IntPercentage { get; set; }

        [Display(Name = "Interest Amt.")]
        public string IntTotalAmt { get; set; }

        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [Display(Name = "Pur. Rate")]
        public string MRPRate { get; set; }

        [Display(Name = "Total")]
        public string UTAmount { get; set; }

        [Display(Name = "Stock")]
        public string PreviousStock { get; set; }

        [Display(Name = "IME/Barcode")]
        public string IMENo { get; set; }

        [Display(Name = "PP Offer")]
        public string PPOffer { get; set; }
        [Display(Name = "Compressor")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts")]
        public string SparePartsWarrentyMonth { get; set; }

        [Display(Name = "Service")]
        public string ServiceWarrentyMonth { get; set; }
        [Display(Name = "Warranty")]
        public string Warranty { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }

        public IEnumerable<StockDetail> StockDetails { get; set; }
    }

    public class CreateCreditSalesOrderViewModel
    {
        public CreateCreditSalesOrderViewModel()
        {
            OfferDescription = "Currently available Offer with this Product";
            CardTypes = new List<CardType>();
        }
        public string SalesOrderId { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Install. Date")]
        public string InstallmentDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; }

        [Display(Name = "VAT Percent.")]
        public string VATPercentage { get; set; }

        [Display(Name = "VAT Amount")]
        public string VATAmount { get; set; }

        [Display(Name = "Flat Dis(%)")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis(amt)")]
        public string TotalDiscountAmount { get; set; }

        [Display(Name = "Total Discount")]
        public string NetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "No of Install.")]
        public string InstallmentNo { get; set; }

        [Display(Name = "Installment")]
        public string InstallmentAmount { get; set; }

        [Display(Name = "Grand Total")]
        public string GrandTotal { get; set; }

        [Display(Name = "Down Payment")]
        public string RecieveAmount { get; set; }

        [Display(Name = "Remain. Amt")]
        public string PaymentDue { get; set; }

        [Display(Name = "Last Pay Adj.")]
        public string PayAdjustment { get; set; }

        public string Status { get; set; }

        [Display(Name = "Prev. Due")]
        public string CurrentDue { get; set; }

        public string CurrentPreviousDue { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        public string WInterestAmt { get; set; }

         [Display(Name = "Interest Rate")]
        public string InterestRate { get; set; }

         [Display(Name = "Int. Amount")]
        public string InterestAmount { get; set; }

        [Display(Name = "Total Offer")]
        public string TotalOffer { get; set; }
        [Display(Name ="Offer")]
        public string OfferDescription { get; set; }

        [Display(Name = "Extend Time Interest")]
        public string ExtendTimeInterestAmount { get; set; }
        public bool IsAllPaid { get; set; }
        public string InstallmentPeriod { get; set; }

        [Display(Name = "Bank")]
        public int BankID { get; set; }

        [Display(Name = "Card Type")]
        public int CardTypeID { get; set; }

        [Display(Name = "Card Paid Amt.")]
        public decimal CardPaidAmount { get; set; }

        [Display(Name = "Cash Paid Amt.")]
        public decimal CashPaidAmount { get; set; }

        [Display(Name = "Processing Fee")]
        public decimal ProcessingFee { get; set; }
        //public int CardTypeSetupID { get; set; }
        public List<CardType> CardTypes { get; set; }
        public string CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public DateTime CreateDateTime { get; set; }

        public bool IsAgreement { get; set; }

        [Display(Name="Total Int.")]
        public string TotalInterest { get; set; }

        public string tempPPInterestTotal { get; set; }

        [Display(Name = "Send SMS ")]
        public bool IsSmsEnable { get; set; } = true;

        [Display(Name = "Weekly Installment")]
        public bool IsWeekly { get; set; }

        [Display(Name = "Guar. Name")]
        public string GuarName { get; set; }
        [Display(Name = "Contact No")]
        public string GuarContactNo { get; set; }
        [Display(Name = "Address")]
        public string GuarAddress { get; set; }
    }

    public class CreateCreditSalesSchedules
    {
        public int CSScheduleID { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Schedule")]
        public string ScheduleDate { get; set; }
        public string ScheduleNo { get; set; }
        [Display(Name = "Pay Date")]
        public string PayDate { get; set; }

        [Display(Name = "Installment")]
        public string InstallmentAmount { get; set; }

        [Display(Name = "Closing")]
        public string ClosingBalance { get; set; }

        [Display(Name = "Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Opening")]
        public string OpeningBalance { get; set; }

        public string Remarks { get; set; }

        public bool IsUnExpected { get; set; }
        public decimal HireValue { get; set; }
        [Display(Name="Net Installment")]
        public decimal NetValue { get; set; }

        public decimal ExpectedInstallment { get; set; }

        public int BankTransID { get; set; }
        public decimal CardPaidAmount { get; set; }
        public int CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public int CreatedBy { get; set; }

    }

    public class GetCreditSalesOrderViewModel
    {
        public string SalesOrderId { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "A/C")]
        public string CustomerCode { get; set; }
       
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmount { get; set; }

        public string Status { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public int IsReturn { get; set; }
    }
}