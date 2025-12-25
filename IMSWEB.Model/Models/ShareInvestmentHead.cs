using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ShareInvestmentHead : AuditTrailModel
    {
        public ShareInvestmentHead()
        {
            this.ShareInvestments = new HashSet<ShareInvestment>();
        }
        [Key]
        public int SIHID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int ConcernID { get; set; }
        public decimal Balance { get; set; }
        public decimal OpeningBalance { get; set; }
        [StringLength(15)]
        public string OpeningType { get; set; }
        public DateTime OpeningDate { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public virtual ICollection<ShareInvestment> ShareInvestments { get; set; }
    }
}
