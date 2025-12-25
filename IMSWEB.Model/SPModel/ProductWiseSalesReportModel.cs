using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductWiseSalesReportModel
    {
        public int SOrderID { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int CompanyID { get; set; }

        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Adjustment { get; set; }
        public string CompanyName { get; set; }
        public int  CustomerType { get; set; }
        public string CategoryName { get; set; }
        public string IMEI { get; set; }


        public decimal NetDiscount { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal? RecAmount { get; set; }

        public decimal PaymentDue { get; set; }

        public decimal AdjAmount { get; set; }

        public decimal UP { get; set; }
        public decimal UTAM { get; set; }


        public decimal PPAdjAmount { get; set; }

        public decimal TotalOffer { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal UTAmount { get; set; }

        public string IMENO { get; set; }

        public string ColorName { get; set; }

        public decimal PPDAmount { get; set; }

        public decimal PPOffer { get; set; }

        public decimal TotalDue { get; set; }

        public int Status { get; set; }

        public int IsReplacement { get; set; }

        public EnumSalesType IsStatus { get; set; }
        public int PCategoryID { get; set; }
        public string ConcernName { get; set; }

        public string UserName { get; set; }
        public int IsReturn { get; set; }
        public decimal SalesTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal NetSales { get; set; }
        public decimal PurchaseTotal { get; set; }

        public decimal CommisionProfit { get; set; }

        public decimal HireProfit { get; set; }

        public decimal HireCollection { get; set; }

        public decimal TotalProfit { get; set; }
        public string ProductCode { get; set; }
        public int EditReqStatus { get; set; }
    }
}
