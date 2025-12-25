using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ServiceCharge : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        public int ConcernId { get; set; }
        public decimal TotalServiceCollection { get; set; }
        public int ServiceYear { get; set; }

        [ForeignKey("ConcernId")]
        public virtual SisterConcern SisterConcern { get; set; }
        public virtual ICollection<ServiceChargeDetails> ServiceChargeDetails { get; set; }
    }
}
