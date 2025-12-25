using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ADParameterGrade
    {
        [Key, Column("ADParameterID", Order = 0)]
        public int ADParameterID { get; set; }
        [Key, Column("GradeID", Order = 1)]
        public int GradeID { get; set; }
    }
}
