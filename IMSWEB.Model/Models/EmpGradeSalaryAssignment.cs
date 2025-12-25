using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class EmpGradeSalaryAssignment : AuditTrailModel
    {
        public EmpGradeSalaryAssignment()
        {

        }
        [Key]
        public int EmpGradeSalaryID { get; set; }
        public int Type { get; set; }
        public DateTime? EntryDate { get; set; }
        public int EmployeeID { get; set; }
        public DateTime EffectDate { get; set; }
        public int GradeSalaryID { get; set; }
        public DateTime? TillDate { get; set; }
        public int? ArrearInfo { get; set; }
        public decimal BasicSalary { get; set; }
        public int GradeID { get; set; }
        public decimal? GrossSalary { get; set; }
        public int? SlabNo { get; set; }
        public int Status { get; set; }
        public int Gradesalarytypeid { get; set; }
        public int ConcernID { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
