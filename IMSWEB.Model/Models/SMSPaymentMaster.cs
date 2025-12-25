using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SMSPaymentMaster : AuditTrailModel
    {
        [Key]
        public int SMSPaymentMasterID { get; set; } 
        public int ParentID { get; set; } 
        public int IsMasking { get; set; } 
        public decimal OpeningBalance { get; set; } 
        public decimal PerSMSCharge { get; set; } 
        public decimal TotalRecAmt { get; set; } 
        public int ConcernID { get; set; }
        public SisterConcern SisterConcern { get; set; }


    }
}

