using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class DesWiseCommission : AuditTrailModel
    {
        public int ID { get; set; }
        public Decimal CommissionPercent { get; set; }
        public int DesignationID { get; set; }
        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
    }
}
