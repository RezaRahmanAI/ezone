using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public partial class AttendenceMonth : AuditTrailModel
    {
        public AttendenceMonth()
        {
            this.Attendences = new HashSet<Attendence>();
        }
        [Key]
        public int AttenMonthID { get; set; }
        public System.DateTime AttendencMonth { get; set; }
        public int IsFinalize { get; set; }
  
        public int ConcernID { get; set; }
        public virtual ICollection<Attendence> Attendences { get; set; }
    }
}
