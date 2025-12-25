using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class HireAccountDetailsReportModel
    {
        public HireAccountDetailsReportModel()
        {
            RunningAccountList = new List<RunningAccountModel>();
        }
        public int OpeningAccount { get; set; }
        public decimal OpeningAccountValue { get; set; }
        public int ClosingAccount { get; set; }
        public decimal ClosingAccountValue { get; set; }
        public int RunningAccount { get; set; }
        public decimal RunningAccountValue { get; set; }
        public List<RunningAccountModel> RunningAccountList { get; set; }
    }

    public class RunningAccountModel
    {
        public string InvoiceNo { get; set; }
        public DateTime Date{ get; set; }
        public string CustomerName { get; set; }
        public string RefName { get; set; }
        public string ProductName { get; set; }
        public List<string> ProductList { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal RemainingAmt { get; set; }

        public int ConcernID { get; set; }

        public string ConcernName { get; set; }

        public string CustomerCode { get; set; }
        public int NoOfInstallment { get; set; }

        public string Address { get; set; }

        public string ContactNo { get; set; }
    }
}
