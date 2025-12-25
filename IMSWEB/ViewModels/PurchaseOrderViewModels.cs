using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class PurchaseOrderViewModel
    {
        public CreatePurchaseOrderDetailViewModel PODetail { get; set; }
        public ICollection<CreatePurchaseOrderDetailViewModel> PODetails { get; set; }
        public CreatePurchaseOrderViewModel PurchaseOrder { get; set; }
        public CreateProductViewModel Product { get; set; }
    }

    public class CreatePurchaseOrderDetailViewModel
    {
        public CreatePurchaseOrderDetailViewModel()
        {
            POProductDetails = new HashSet<POProductDetail>();
        }
        public string PODetailId { get; set; }

        public string PurchaseOrderId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public EnumStatus Status { get; set; }

        [Display(Name = "Color")]
        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Actu.Pur.Rate")]
        public string UnitPrice { get; set; }

        [Display(Name = "MRP Rate")]
        public string MRPRate { get; set; }

        [Display(Name = "RP Rate")]
        public string EMOBRPRate { get; set; }
        [Display(Name = "Pur. Rate")]
        public string ECOMRPRate { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get; set; }

        public int CalculatedQuantity { get; set; }

        [Display(Name = "Total Amt.")]
        public string TAmount { get; set; }

        [Display(Name = "Dis.Per.")]
        public string PPDisPercentage { get; set; }

        [Display(Name = "Dis. Amt")]
        public string PPDiscountAmount { get; set; }

        [Display(Name = "Prv. Stock")]
        public string PreviousStock { get; set; }
        public int ProductType { get; set; }

        [Display(Name = "Cash Sales Rate")]
        public string SalesRate { get; set; }
        [Display(Name = "MRP")]
        public string EMOBMRPRate { get; set; }
        [Display(Name = "Extra Dis.Per.")]
        public string ExtraPPDISPer { get; set; }
        [Display(Name = "Extra Dis.Amt.")]
        public string ExtraPPDISAmt { get; set; }
        public string PPOffer { get; set; }
        [Display(Name = "Credit SRate 6")]
        public string CreditSalesRate { get; set; }
        [Display(Name = "Credit SRate 12")]
        public string CRSalesRate12Month { get; set; }
        [Display(Name = "Credit SRate 3")]
        public string CRSalesRate3Month { get; set; }

        [Display(Name = "Godown")]
        public string GodownID { get; set; }
        [Display(Name = "Godown")]
        public string GodownName { get; set; }
        public ICollection<POProductDetail> POProductDetails { get; set; }

        public ICollection<Stock> Stocks { get; set; }

        public ICollection<StockDetail> StockDetails { get; set; }
        public List<int> SDetailIDList { get; set; }

        public decimal RQuantity { get; set; }

        public decimal PRate { get; set; }
        [Display(Name = "DO")]
        public string DONo { get; set; }

        public int DOID { get; set; }
        public int DOrderDetailID { get; set; }
    }

    public class CreatePurchaseOrderViewModel
    {
        public string PurchaseOrderId { get; set; }

        [Display(Name = "Challan")]
        public string ChallanNo { get; set; }

        [Display(Name = "Invoice No")]
        [StringLength(150)]
        public string InvoiceNo { get; set; }

        [Display(Name = "Pur. Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; }

        [Display(Name = "Flat Dis. Per.")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis. Amt")]
        public string TotalDiscountAmount { get; set; }
        public string tempFlatDiscountAmount { get; set; }

        [Display(Name = "Total Dis.")]
        public string NetDiscount { get; set; }
        public string tempNetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "Adj. Amt")]
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

        [Display(Name = "Labour Cost")]
        public string LabourCost { get; set; }

        [Display(Name = "Prv. Due")]
        public string CurrentDue { get; set; }

        [Display(Name = "Is Damage PO")]
        public bool IsDamagePO { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "TPQty")]
        public string TPQty { get; set; }
        [Display(Name = "A.I.T (%)")]
        public string VATPercentage { get; set; }

        [Display(Name = "A.I.T")]
        public string VATAmount { get; set; }
    }

    public class GetPurchaseOrderViewModel
    {
        public string PurchaseOrderId { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        public string Status { get; set; }
        public string EditReqStatus { get; set; }
    }


    public class PurchaseReturnOrderViewModel
    {
        public CreatePurchaseOrderViewModel PurchaseOrder { get; set; }
        public CreatePOProductDetailViewModel POProductDetails { get; set; }
        public List<CreatePOProductDetailViewModel> POProductDetailList { get; set; }
    }

    public class CreatePOProductDetailViewModel
    {
        public int POPDID { get; set; }
        public int SDetailID { get; set; }

        [Display(Name = "IMEI")]
        public string IMENo { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ColorsId { get; set; }
        public decimal PreviousStock { get; set; }

        public decimal Quantity { get; set; }

        public int GodownID { get; set; }

        public int ProductType { get; set; }

        public decimal PRate { get; set; }
        public decimal RQuantity { get; set; }
        public string SupplierId { get; set; }
    }
}