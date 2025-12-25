using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class EmployeeCommission : AuditTrailModel
    {
        [Key]
        public int ECID { get; set; }
        public DateTime CommissionMonth { get; set; }
        public virtual Employee Employee { get; set; }
        public int EmployeeID { get; set; }
        public decimal CommissionAmt { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
    }
}
