using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class CommissionSetup : AuditTrailModel
    {
        [Key]
        public int CSID { get; set; }
        public int EmployeeID { get; set; }
        public decimal AchievedPercentStart { get; set; }
        public decimal AchievedPercentEnd { get; set; }
        public DateTime CommissionMonth { get; set; }
        public decimal CommisssionAmt { get; set; }
        public decimal CommissionPercent { get; set; }
        public int ConcernID { get; set; }
        public int Status { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
    }
}
