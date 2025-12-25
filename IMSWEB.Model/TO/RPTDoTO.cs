using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTDoTO
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string DONO { get; set; }
        public DateTime DODate { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal GivenQty { get; set; }
        public decimal MRP { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal SuplierTotal { get; set; }
        public string ProductCode { get; set; }
        public string CompanyName { get; set; }
        public decimal DDLiftingPrice { get; set; }
        public decimal TAmt { get; set; }
        public int DOID { get; set; }
        public decimal PPDisPercent { get; set; }
        public decimal PPDisAmt { get; set; }
        public decimal NetDiscount { get; set; }
        public decimal FlatDiscountAmount { get; set; }
    }
}
