using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class AdvanceSearchModel
    {
        public AdvanceSearchModel()
        {
            AdvancePODetails = new List<AdvancePODetail>();
            AdvanceSOrderDetails = new List<AdvanceSOrderDetail>();
            AdvanceTransferDetails = new List<AdvanceSOrderDetail>();
            AdvanceReplacements = new List<AdvanceSOrderDetail>();
            AdvanceSalesReturns = new List<AdvanceSOrderDetail>();
        }
        public DateTime PurchaseDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ChallanNo { get; set; }

        public DateTime SalesDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }

        public List<AdvancePODetail> AdvancePODetails { get; set; }
        public List<AdvanceSOrderDetail> AdvanceSOrderDetails { get; set; }
        public List<AdvanceSOrderDetail> AdvanceSalesReturns { get; set; }
        public List<AdvanceSOrderDetail> AdvanceReplacements { get; set; }
        public List<AdvanceSOrderDetail> AdvanceTransferDetails { get; set; }
    }

    public class AdvancePODetail
    {
        public int ID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string IMEI { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal Quantity { get; set; }
        public string CategoryName { get; set; }
        public string ChallanNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string SupplierName { get; set; }
        public string SupCode { get; set; }
    }

    public class AdvanceSOrderDetail
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }
        public string ReturnInvoiceNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string IMEI { get; set; }
        public decimal SalesRate { get; set; }
        public decimal Quantity { get; set; }
        public int Status { get; set; }
        public string FromConcernName { get; set; }

        public DateTime Date { get; set; }
        public DateTime ReturnDate { get; set; }

        public string TransferNo { get; set; }

        public string ToConcernName { get; set; }

        public int CustomerID { get; set; }

        public string ReplaceProductType { get; set; }
        public DateTime SalesDate { get; set; }
    }
}
