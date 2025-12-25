using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSRequest
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerFatherName { get; set; }
        public string CustomerAddress { get; set; }
        public decimal PreviousDue { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal TotalReceiveAmount { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal PresentDue { get; set; }
        public DateTime Date { get; set; }
        public string TransNumber { get; set; }
        public string MobileNo { get; set; }
        public string SMS { get; set; }
        public EnumSMSType SMSType { get; set; }
        public int? Message_ID { get; set; }

        public List<string> ProductNameList { get; set; }
        public List<SMSProductDetails> ProductDetailList { get; set; }
        
    }

    public class SMSProductDetails
    {
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string EngineNo { get; set; }
        public string ChasisNo { get; set; }
    }
    public class SMSResponse
    {
        public string ResponseCode { get; set; }
        public string Descriptions { get; set; }
        public EnumSMSSendStatus SMSSendStatus { get; set; }
      
    }
}
