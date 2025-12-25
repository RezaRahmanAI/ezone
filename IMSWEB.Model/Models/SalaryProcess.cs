using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SalaryProcess
    {
        public SalaryProcess()
        {
            this.SalaryMonthlys = new HashSet<SalaryMonthly>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalaryProcessID { get; set; }
        public System.DateTime ProcessDate { get; set; }
        public Nullable<System.DateTime> MonthEndDate { get; set; }
        public int WorkDays { get; set; }
        public Nullable<int> UserID { get; set; }
        public int IsFinalized { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public int PayrollTypeID { get; set; }
        public Nullable<int> ShowInDesktopn { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public System.DateTime SalaryMonth { get; set; }
        public string ProcessCode { get; set; }
        public string Remarks { get; set; }
        public int ConcernID { get; set; }

        public virtual ICollection<SalaryMonthly> SalaryMonthlys { get; set; }
    }
}
