using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductWisePurchaseModel
    {
        public int POrderID { get; set; }
        public string ChallanNo { get; set; }
        public DateTime Date { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal StockPurchaseRate { get; set; }
        public decimal TotalStockAmount { get; set; }
        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal NetDiscount { get; set; }
        public decimal FlatDiscount { get; set; }
        public decimal LaborCost { get; set; }
        public decimal NetTotal { get; set; }
        public decimal RecAmt { get; set; }
        public decimal PaymentDue { get; set; }
        public int ProductID { get; set; }
        public int POPDID { get; set; }
        public decimal PPDISAmt { get; set; }
        public string DamageIMEI { get; set; }
        public string IMENO { get; set; }
        public int ColorID { get; set; }
      //  public string ColorName { get; set; }
        public decimal PPOffer { get; set; }
        public decimal SRate { get; set; }
        public decimal MRP { get; set; }
        public decimal RP { get; set; }
        public int? IsDamageReplaced { get; set; }
        public string ConcenName { get; set; }
        public decimal TotalCreditSR3 { get; set; }
        public decimal TotalCreditSR6 { get; set; }
        public decimal TotalCreditSR12 { get; set; }
        public string GodownName { get; set; }
        public string FromGodownName { get; set; }
        public string ToConcernName { get; set; }
        public string FromConcernName { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public string OwnerName { get; set; }
        public int Status { get; set; }
        public int ProductType { get; set; }
        public int CompanyID { get; set; }
        public EnumUnitType UnitType { get; set; }
        public decimal PWDiscount { get; set; }
        public string CompressorWarrentyMonth { get; set; }
        public string MotorWarrentyMonth { get; set; }
        public string PanelWarrentyMonth { get; set; }
        public string ServiceWarrentyMonth { get; set; }
        public string SparePartsWarrentyMonth { get; set; }
        public string OfferDescription { get; set; }

        public List<string> IMEIs { get; set; }
        public decimal AdjustAmt { get; set; }

        public string  PCategoryName { get; set; }
        public decimal ProMRP { get; set; }
        public string InvoiceNo { get; set; }
        public decimal OnlyDisAmt { get; set; }
        public decimal TotalPPDis { get; set; }
        public decimal AfterFlatDisPurchaseRate { get; set; }
        public int EditReqStatus { get; set; }
        public decimal TotalDue { get; set; }
        public int ActionStatus { get; set; }
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public decimal GivenQty { get; set; }
        public int StockID { get; set; }
        public decimal PrevStq { get; set; }
        public int GodownID { get; set; }
        public decimal SalesRate { get; set; }
        public decimal TotalSalesRate { get; set; }
        public string UserInputWarranty { get; set; }

    }

}
