using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.SPModel
{
    public class DailyStockVSSalesSummaryReportModel
    {
        public DateTime Date { get; set; }
        public int ConcernID { get; set; }
        public int ProductID { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public decimal OpeningStockQuantity { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal TotalStockQuantity { get; set; }
        public decimal SalesQuantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal ClosingStockQuantity { get; set; }
        public decimal OpeningStockValue { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal ClosingStockValue { get; set; }

    }
}
