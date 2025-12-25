using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class ProductPickerInStockReportModel
    {
        public int ConcernID { get; set; }
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int CompanyID { get; set; }
        public int ColorID { get; set; }
        public int GodownID { get; set; }
        public string ProductName { get; set; }
        public int StockDetailsId { get; set; }
        public string ProductCode { get; set; }
        public string CategoryName { get; set; }
        public string ColorName { get; set; }
        public string CompanyName { get; set; }
        public string GodownName { get; set; }
        public string IMENO { get; set; }
        public decimal MRPRate { get; set; }
        public decimal PRate { get; set; }
        public decimal PreStock { get; set; }
        public int ProductType { get; set; }
        public decimal Quantity { get; set; }
        public string Service { get; set; }
        public DateTime OrderDate { get; set; }


    }
}
