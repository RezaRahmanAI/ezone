using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SRVisitReportModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string TotalIMEI { get; set; }
        public int ColorID { get; set; }
        public int SDetailID { get; set; }
        public int ConcernID { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime TransDate { get; set; }
        public decimal OpeningQty { get; set; }
        public string Opening_imeno { get; set; }
        public string Opening_productno { get; set; }
        public decimal taken_qty { get; set; }
        public string taken_imeno { get; set; }
        public string taken_product { get; set; }
        public decimal Total_qty { get; set; }
        public decimal sale_qty { get; set; }
        public string sale_imeno { get; set; }
        public string sale_product { get; set; }
        public decimal balance_qty { get; set; }
        public string imeno_balance { get; set; }
        public string product_balance { get; set; }
        public List<string> OpeningIMEIList { get; set; }
        public List<string> ReceiveIMEIList { get; set; }
        public List<string> TotalIMEIList { get; set; }
        public List<string> SalesIMEIList { get; set; }
        public List<string> BalanceIMEIList { get; set; }
    }
    public class SRVisitReportMain
    {
        public SRVisitReportMain()
        {
            OpeningIMEIS = new List<SRVisitReportModel>();
            ReceiveIMEIS = new List<SRVisitReportModel>();
            TotalIMEIS = new List<SRVisitReportModel>();
            SalesIMEIS = new List<SRVisitReportModel>();
            OpeningIMEIS = new List<SRVisitReportModel>();
        }
        public int ID { get; set; }
        public DateTime TransDate { get; set; }
        public List<SRVisitReportModel> OpeningIMEIS { get; set; }
        public List<SRVisitReportModel> ReceiveIMEIS { get; set; }
        public List<SRVisitReportModel> TotalIMEIS { get; set; }
        public List<SRVisitReportModel> SalesIMEIS { get; set; }
        public List<SRVisitReportModel> BalanceIMEIS { get; set; }
    }
    public class SRVisitDate
    {
        public DateTime Date { get; set; }
    }
}
