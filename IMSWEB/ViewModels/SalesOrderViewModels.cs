using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class SalesOrderViewModel
    {
        public SalesOrderViewModel()
        {
            IMEIList = new List<ProductDetailsModel>();
        }
        public CreateSalesOrderDetailViewModel SODetail { get; set; }
        public List<CreateSalesOrderDetailViewModel> SODetails { get; set; }
        public CreateSalesOrderViewModel SalesOrder { get; set; }
        public ICollection<CreateSaleOfferViewModel> SalesOffers { get; set; }
        public List<ProductDetailsModel> IMEIList { get; set; }

    }

    public class CreateSalesOrderDetailViewModel
    {
        public string SODetailId { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }
        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }

        public string StockDetailId { get; set; }
        public string RStockDetailId { get; set; }

        public string dSOrderDetailID { get; set; }
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }
        public int GodownID { get; set; }

        public decimal PRate { get; set; }

        [Display(Name = "Sales Rate")]
        public string UnitPrice { get; set; }

        [Display(Name = "Dis. Per.")]
        public string PPDPercentage { get; set; }

        [Display(Name = "Dis. Amt.")]
        public string PPDAmount { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get; set; }

        public EnumStatus Status { get; set; }

        [Display(Name = "Pur. Rate")]
        public string MRPRate { get; set; }

        [Display(Name = "Total")]
        public string UTAmount { get; set; }

        [Display(Name = "Stock")]
        public string PreviousStock { get; set; }

        [Display(Name = "IMEI")]
        public string IMENo { get; set; }
        public List<string> IMEIList { get; set; }

        [Display(Name = "PP Offer")]
        public string PPOffer { get; set; }

        [Display(Name = "Damage IMEI")]
        public string DamageIMEINO { get; set; }

        [Display(Name = "Damage UnitPrice")]
        public string DamageUnitPrice { get; set; }

        [Display(Name = "Replace IMEI")]
        public string ReplaceIMEINO { get; set; }
        [Display(Name = "Damage Product")]
        public string DamageProductName { get; set; }
        public int RepOrderID { get; set; }
        //[Required(ErrorMessage="Remarks Required")]
        public string Remarks { get; set; }
        public int ProductType { get; set; }
        [Display(Name = "Compressor")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts")]
        public string SparePartsWarrentyMonth { get; set; }
        [Display(Name="Service")]
        public string ServiceWarrentyMonth { get; set; }
        public int DamagePOPDID { get; set; }

        [Display(Name="Replace Type")]
        public EnumReplaceProductType ReplaceProductType { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }

        public IEnumerable<StockDetail> StockDetails { get; set; }
        public int CreditSalesDetailId { get; set; }
        [Display(Name = "Sale Rate")]
        public decimal CRSalePrice { get; set; }
        [Display(Name = "Purchase Rate")]
        public decimal CRPurchasePrice { get; set; }
        [Display(Name = "Warranty")]
        public string Warranty { get; set; }
        [Display(Name = "Service")]
        public string Service { get; set; }
        public string dSalesQuantity { get; set; }
        public string dSOrderDetailsId { get; set; }
        public string StockID { get; set; }
    }

    public class CreateSalesOrderViewModel
    {

        public CreateSalesOrderViewModel()
        {
            OfferDescription = "Currently available Offer with this Product";
            CardTypes = new List<CardType>();
        }

        public string SalesOrderId { get; set; }

        [Display(Name = "Inv. No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Remind Date")]
        public string RemindDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; } //To store Net discount in view
        public string TempFlatDiscountAmount { get; set; } //To store Flat discount in view

        [Display(Name = "VAT Percent.")]
        public string VATPercentage { get; set; }

        [Display(Name = "VAT Amount")]
        public string VATAmount { get; set; }

        [Display(Name = "Flat Dis. Per.")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis. Amt.")]
        public string TotalDiscountAmount { get; set; }

        [Display(Name = "Net Discount")]
        public string NetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "Adjust. Amt")]
        public string AdjAmount { get; set; }

        [Display(Name = "Grand Total")]
        public string GrandTotal { get; set; }

        [Display(Name = "Pay Amount")]
        public string RecieveAmount { get; set; }

        [Display(Name = "Payment Due")]
        public string PaymentDue { get; set; }

        [Display(Name = "Total Due")]
        public string TotalDue { get; set; }

        public string Status { get; set; }

        [Display(Name = "Prev. Due")]
        public string CurrentDue { get; set; }

        [Display(Name = "Trams & Condition")]
        public string TramsAndCondition { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Total Offer")]
        public string TotalOffer { get; set; }

        [Display(Name = "TSQty")]
        public string TSQty { get; set; }


        public string OfferDescription { get; set; }

        [Display(Name="Damage Total")]
        public string DamageTotalAmount { get; set; }

        [Display(Name = "Replace Total")]
        public string ReplaceTotalAmount { get; set; }


        [Display(Name = "Bank")]
        public int BankID { get; set; }

        [Display(Name = "Card Type")]
        public int CardTypeID { get; set; }

        [Display(Name = "Card Paid Amt.")]
        public decimal CardPaidAmount { get; set; }

        [Display(Name = "Cash Paid Amt.")]
        public decimal CashPaidAmount { get; set; }
        //public int CardTypeSetupID { get; set; }
        public List<CardType> CardTypes { get; set; }
        public string CardTypeSetupID { get; set; }
        public decimal DepositChargePercent { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        [Display(Name = "Send SMS ")]
        public bool IsSmsEnable { get; set; } = true;


        [Display(Name = "Send SMS ")]
        public bool IsBanglaSmsEnable { get; set; } = true;
        
        public string TQty { get; set; }
        public decimal CreditRemainingDue { get; set; }
        public decimal ToCustomerPayAmt { get; set; }
        public int HireSalesId { get; set; }
        [Display(Name = "Memo No")]
        public string MemoNo { get; set; }
        public decimal PrevDue { get; set; }
    }

    public class GetSalesOrderViewModel
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
        [Display(Name = "Inv. Amt")]
        public string InvAmt { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmount { get; set; }

        public string Status { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string EditReqStatus { get; set; }
    }
}