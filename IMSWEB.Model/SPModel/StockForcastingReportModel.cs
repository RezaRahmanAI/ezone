using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class StockForcastingReportModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string CompanyName { get; set; }
        public decimal MonthSalesQty { get; set; }
        public decimal AverageSalesQty { get; set; }
        public decimal PresentStockQty { get; set; }
        public decimal NeedSalesQty { get; set; }
        public int SMonth { get; set; }
        public int CategoryID { get; set; }
        public int CompanyID { get; set; }
        public string ConcernName { get; set; }

        public int UserConcernID { get; set; }
        public int SYear { get; set; }





    }
}