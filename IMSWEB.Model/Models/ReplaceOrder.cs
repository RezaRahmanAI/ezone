using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ReplaceOrder
    {

        public ReplaceOrder()
        {
            ROrderDetails = new HashSet<ReplaceOrderDetail>();
        }
        public string SalesOrderId { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

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
        public virtual ICollection<ReplaceOrderDetail> ROrderDetails { get; set; }
    }
}
