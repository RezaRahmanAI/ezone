using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class StockReportWithDateReportModel
    {
        public DateTime Date { get; set; }
        public int ConcernID { get; set; }
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public string ChallanNo { get; set; }
        public string NoOfDays { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ModelName { get; set; }
        public string CompanyName { get; set; }

        public string IMENO { get; set; }

        public string CategoryName { get; set; }

        public decimal MonthSalesQty { get; set; }

        public decimal AverageSalesQty { get; set; }
        public decimal PresentStockQty { get; set; }
        public decimal NeedSalesQty { get; set; }

        public int SMonth { get; set; }
        public int CategoryID { get; set; }

        public int CompanyID { get; set; }

    }
}