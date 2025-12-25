using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class ReplacementOrderInvoiceModel
    {
        public ReplaceOrderDetailViewModel SODetail { get; set; }
        public ICollection<ReplaceOrderDetailViewModel> SODetails { get; set; }
        public ReplaceOrderViewModel SalesOrder { get; set; }
    }

    public class ReplaceOrderDetailViewModel
    {
        public string SODetailId { get; set; }

        public string SalesOrderId { get; set; }

        public string ProductId { get; set; }

        public string StockDetailId { get; set; }
        public string RStockDetailId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ColorId { get; set; }

        public string ColorName { get; set; }

        public string UnitPrice { get; set; }

        public string PPDPercentage { get; set; }

        public string PPDAmount { get; set; }

        public string Quantity { get; set; }

        public EnumStatus Status { get; set; }

        public string MRPRate { get; set; }

        public string UTAmount { get; set; }

        public string PreviousStock { get; set; }

        public string IMENo { get; set; }

        public string PPOffer { get; set; }

        public string DamageIMEINO { get; set; }

        public string DamageUnitPrice { get; set; }

        public string ReplaceIMEINO { get; set; }
        public string DamageProductName { get; set; }
        public int RepOrderID { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }

        public IEnumerable<StockDetail> StockDetails { get; set; }
    }

    public class ReplaceOrderViewModel
    {

        public ReplaceOrderViewModel()
        {
            OfferDescription = "Currently available Offer with this Product";
        }

        public string SalesOrderId { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; }

        [Display(Name = "VAT Percent.")]
        public string VATPercentage { get; set; }

        [Display(Name = "VAT Amount")]
        public string VATAmount { get; set; }

        [Display(Name = "Flat Dis. Per.")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis. Amt.")]
        public string TotalDiscountAmount { get; set; }

        [Display(Name = "Total Dis.")]
        public string NetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "Adjust. Amount")]
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

        [Display(Name = "Previous Due")]
        public string CurrentDue { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Total Offer")]
        public string TotalOffer { get; set; }


        public string OfferDescription { get; set; }

        [Display(Name = "Damage Total")]
        public string DamageTotalAmount { get; set; }

        [Display(Name = "Replace Total")]
        public string ReplaceTotalAmount { get; set; }
    }

    public class GetSalesOrderViewModel
    {
        public string SalesOrderId { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmount { get; set; }

        public string Status { get; set; }
    }
}
