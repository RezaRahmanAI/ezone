using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SOredersReportModel
    {
        public int SOrderID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PPDAmount { get; set; }
        public decimal PPDPercentage { get; set; }
        public decimal UTAmount { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal FlatDiscount { get; set; }
        public decimal NetDiscount { get; set; }
        public decimal TotalOffer { get; set; }
        public decimal AdjAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Grandtotal { get; set; }
        public decimal RecAmount { get; set; }
        public decimal PaymentDue { get; set; }
        public decimal Quantity { get; set; }
        public string IMENO { get; set; }

        public string CustomerAddress { get; set; }
        public string CustomerContactNo { get; set; }
        public string CustCompanyName { get; set; }
        public string CustomerNID { get; set; }
        public decimal CustomerTotalDue { get; set; }
        public EnumCustomerType CustomerType { get; set; }
        public int ConcernID { get; set; }
        public string ConcernName { get; set; }
        public string InstallmentPeriod { get; set; }
        public int ActionStatus { get; set; }
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
