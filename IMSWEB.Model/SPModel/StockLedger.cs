using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class StockLedger
    {
        public DateTime Date { get; set; }
        public int ConcernID { get; set; }
        public int ProductID { get; set; }
        public string Code { get; set; }
        public string CategoryCode { get; set; }
        public string ProductName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        public int ModelID { get; set; }
        public string ModelName { get; set; }

        public int CompanyID { get; set; }
        public string CompanyName { get; set; }

        public decimal OpeningStockQuantity { get; set; }
        public decimal PerviousPurchaseQuantity { get; set; }
        public decimal PerviousTransferInQuantity { get; set; }
        public decimal PerviousTransferOutQuantity { get; set; }


        public decimal PurchaseReturnQuantity { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal SalesReturnQuantity { get; set; }
        public decimal SalesQuantity { get; set; }
        public decimal TransferInQuantity { get; set; }
        public decimal TransferOutQuantity { get; set; }


        public decimal PreviousPurchaseQuantity { get; set; }
        public decimal PreviousPurchaseReturnQuantity { get; set; }
        public decimal PreviousSalesQuantity { get; set; }
        public decimal PreviousSalesReturnQuantity { get; set; }
        public decimal TotalStockQuantity { get; set; }

        public decimal ClosingStockQuantity { get; set; }
        public decimal OpeningStockValue { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal ClosingStockValue { get; set; }


        public decimal PreTransferInQuantity { get; set; }

        public decimal PreTransferOutQuantity { get; set; }

        public decimal PurchaseRate { get; set; }
        public decimal PrevPurchaseRate { get; set; }
        public decimal Quantity { get; set; }
        public int PCategoryID { get; set; }
        public string ParentCategoryName { get; set; }

        public decimal RepQty { get; set; }

        public decimal PreRepQty { get; set; }
        public int SOrderId { get; set; }
    }
}
