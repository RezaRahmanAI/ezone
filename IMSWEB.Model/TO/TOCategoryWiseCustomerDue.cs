using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class TOCategoryWiseCustomerDue
    {
        public int CustomerID { get; set; }
        public decimal SalesDue { get; set; }
        public decimal HireDue { get; set; }
        public decimal TotalDue { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerType { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public decimal TInterestAmount { get; set; }
        public decimal CrInterestAmount { get; set; }
    }
}
