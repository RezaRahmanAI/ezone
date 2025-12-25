using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
   public class SRWiseCustomerStatusReportModel
    {
        public int ConcernID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int CustomerID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public decimal OpeningDue { get; set; }
        public decimal SlaesAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal Collection { get; set; }
        public decimal ClosingAmount { get; set; }
        public decimal Quantity { get; set; }
        public string CustomerType { get; set; }
        public string ConcernName { get; set; }
        public decimal TotalDue { get; set; }


    }
}
