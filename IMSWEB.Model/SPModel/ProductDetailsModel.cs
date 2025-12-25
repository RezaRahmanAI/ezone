using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductDetailsModel
    {
        public int ProductId { get; set; }
        public int StockDetailsId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string PicturePath { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string CompanyName { get; set; }
        public decimal PWDiscount { get; set; }
        public decimal PreStock { get; set; }
        public string IMENo { get; set; }
        public decimal MRPRate { get; set; }
        public decimal CashSalesRate { get; set; }
        public string OfferDescription { get; set; }
        public int ProductType { get; set; }
        public int CompressorWarrentyMonth { get; set; }
        public int PanelWarrentyMonth { get; set; }
        public int MotorWarrentyMonth { get; set; }
        public int SparePartsWarrentyMonth { get; set; }
        public int ServiceWarrentyMonth { get; set; }
        public decimal SalesQty { get; set; }

        public int StockID { get; set; }

        public string GodownName { get; set; }

        public decimal StockQty { get; set; }

        public StockDetail StockDetails { get; set; }

        public int Status { get; set; }

        public decimal LPPrice { get; set; }

        public decimal MRPPrice { get; set; }

        public decimal SalesPrice { get; set; }

        public decimal CreditSalesPrice { get; set; }

        public decimal CreditSalesPrice3 { get; set; }

        public decimal CreditSalesPrice12 { get; set; }

        public int GodownID { get; set; }
        public List<string>  IMEIList { get; set; }

        public decimal PRate { get; set; }

        public decimal offerValue { get; set; }

        public string PCategoryName { get; set; }

       // public EnumStockStatus Status { get; set; }

        public int SDetailID { get; set; }
        public DateTime Date { get; set; }

        public decimal RecAmount { get; set; }
        public decimal InterestAmout { get; set; }
        public DateTime CollectionDate { get; set; }
        public decimal ProMRP { get; set; }
        public string ProductMemo { get; set; }
        public int HireSalesId { get; set; }
        public int CreditSaleDetailsId { get; set; }
        public int CompanyID { get; set; }
        public int SOrderDetailID { get; set; }
        public string Service { get; set; }
        public int SOrderDetailsId { get; set; }

    }
}
