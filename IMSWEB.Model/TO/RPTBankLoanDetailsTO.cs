using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTBankLoanDetailsTO
    {
        public string Code { get; set; }
        public string BankName { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal TotalSDPS { get; set; }
        public decimal TotalSavings { get; set; }
    }
}
