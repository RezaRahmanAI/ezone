using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTBankDueLoanTO
    {
        public string LoanCode { get; set; }
        public string BankName { get; set; }
        public int ScheduleNo { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string Status { get; set; }
        public decimal InstallmentAmount { get; set; }
    }
}
