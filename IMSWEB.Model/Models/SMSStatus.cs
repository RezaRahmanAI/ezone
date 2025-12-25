using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSStatus
    {
        public int SMSStatusID { get; set; }
        public string Code { get; set; }
        public int SMSFormateID { get; set; }
        public int SendingStatus { get; set; }
        public int NoOfSMS { get; set; }
        public int? CustomerID { get; set; }
        public string ContactNo { get; set; }
        public DateTime EntryDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SMS { get; set; }
        public string ResponseMsg { get; set; }
        public int ConcernID { get; set; }
        public int? Message_ID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }

        public virtual SMSFormate SMSFormate { get; set; }
    }
}
