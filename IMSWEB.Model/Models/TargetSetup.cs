using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TargetSetup:AuditTrailModel
    {
        public TargetSetup()
        {
            TargetSetupDetails = new HashSet<TargetSetupDetail>();
        }
        [Key]
        public int TID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime TargetMonth { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public int ConcernID { get; set; }
        public int Status { get; set; }
        public ICollection<TargetSetupDetail> TargetSetupDetails { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
    }
}
