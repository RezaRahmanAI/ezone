using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
   public class ExtraCommissionSetup
    {
       [Key]
        public int ID { get; set; }
        public int CategoryID1 { get; set; }
        public int CategoryID2 { get; set; }
        public int ConcernID { get; set; }
        public int CompanyID { get; set; }
        public EnumCommissionType Status { get; set; }
    }
}
