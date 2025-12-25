using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class GradeSalaryChangeType:AuditTrailModel
    {
        [Key]
        public int GradeSalaryChangeTypeID { get; set; }
        public string Name { get; set; }
        public int EffectType { get; set; }
        public int SequenceNo { get; set; }
        public int Status { get; set; }
    }
}
