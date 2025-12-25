using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class RPTBankLoanCollectionInvTO
    {
        public string ReceiptNo { get; set; }
        public DateTime CollectionDate { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string BankName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal SDPS { get; set; }
        public decimal Savings { get; set; }
        public string CollectionType { get; set; }
    }
}
