using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ADParameterBasic : AuditTrailModel
    {
        public ADParameterBasic()
        {
            this.ADParameterEmployees = new HashSet<ADParameterEmployee>();
            //this.Grades = new HashSet<Grade>();
        }
        [Key]
        public int ADParameterID { get; set; }
        public int AllowDeductID { get; set; }
        public int IsConfirmOnly { get; set; }
        public decimal FlatAmount { get; set; }
        public decimal PercentOfBasic { get; set; }
        public int Periodicity { get; set; }
        public int EntitleType { get; set; }
        public int IsCurrentlyActive { get; set; }
        public System.DateTime LastActivationDate { get; set; }
        public Nullable<System.DateTime> LastDeactivationDate { get; set; }
        public int AllowOrDeduct { get; set; }
        public int IsTaxable { get; set; }
        public decimal TaxablePercentage { get; set; }
        public int IsDependsOnAttendance { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> TaxProjection { get; set; }
        public int Gender { get; set; }
        public int PayrollTypeID { get; set; }
        public Nullable<int> PeriodicValue { get; set; }
        public int IsDependsOnSpecialHour { get; set; }
        public int IsWithException { get; set; }
        public Nullable<int> IsEditable { get; set; }
        public decimal PercentOfGross { get; set; }
        public int Status { get; set; }
        public decimal ISTaxProjection { get; set; }
        public int IsDependsOnOTHour { get; set; }
        public decimal PercentOfEarnedBasic { get; set; }
        public int IsFractionateApplicable { get; set; }
        public int SequenceNO { get; set; }
        public int ConcernID { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
        public virtual ICollection<ADParameterEmployee> ADParameterEmployees { get; set; }
        //public virtual ICollection<ADParameterGrade> ADParameterGrades { get; set; }
        //public virtual ICollection<Grade> Grades { get; set; }
    }
}
