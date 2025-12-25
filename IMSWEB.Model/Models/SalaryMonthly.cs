using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public class SalaryMonthly
    {
  
        public SalaryMonthly()
        {
            this.SalaryMonthlyDetails = new HashSet<SalaryMonthlyDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalaryMonthlyID { get; set; }
        public int EmployeeID { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public int LocationID { get; set; }
        public int GradeID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public decimal PrevMonthBasic { get; set; }
        public decimal ThisMonthBasic { get; set; }
        public int IsFinalized { get; set; }
        public string Remarks { get; set; }
        public string AccountNo { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public int PFMemberType { get; set; }
        public int Gender { get; set; }
        public int IsConfirmed { get; set; }
        public int IsEligibleOT { get; set; }
        public int ReligionID { get; set; }
        public int PayrollTypeID { get; set; }
        public int WithNoDetail { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> SalaryMonth { get; set; }
        public decimal ThisMonthGross { get; set; }
        public int SalaryProcessID { get; set; }
        public int ConcernID { get; set; }
        public decimal WorkinDays { get; set; }
        public decimal OTHours { get; set; }
        public decimal? AchievedComm { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<SalaryMonthlyDetail> SalaryMonthlyDetails { get; set; }
        public virtual SalaryProcess SalaryProcess { get; set; }

    }
}
