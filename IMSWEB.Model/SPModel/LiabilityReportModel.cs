using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
   public class LiabilityReportModel
    {
        public DateTime RecDate { get; set; }
        public decimal ReceiveAmt{ get; set; }
        public DateTime PayDate { get; set; }
        public decimal PayAmt{ get; set; }

        public string HeadName { get; set; }
        public string RecType { get; set; }
        public string PayType { get; set; }
        public string RecPurpose { get; set; }
        public string PayPurpose { get; set; }
        public int HeadID { get; set; }
        public EnumLiabilityType RecLiabilityType { get; set; }
        public EnumLiabilityType PayLiabilityType { get; set; }

        public int BankLiaPayType { get; set; }

        public int BankLiaRecType { get; set; }

        public int Status { get; set; }





        public int SerialNumber { get; set; }
        public string DebitParticulars { get; set; }
        public decimal LiabilitiesReceived { get; set; }
        public string CreditParticulars { get; set; }
        public decimal LiabilitiesPay { get; set; }



    }
}
