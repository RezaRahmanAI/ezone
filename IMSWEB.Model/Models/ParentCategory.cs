using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ParentCategory : AuditTrailModel
    {
        public ParentCategory()
        {
            Categories = new HashSet<Category>();
        }
        [Key]
        public int PCategoryID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public virtual SisterConcern SisterConcern { get; set; }
        //public int ConcernID { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
