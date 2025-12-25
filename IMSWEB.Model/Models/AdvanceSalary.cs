using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public partial class AdvanceSalary
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Date { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Remarks { get; set; }
        public int ConcernID { get; set; }
        public int SalaryProcessID { get; set; }
        public virtual Employee Employee { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string DesignationName { get; set; }
        [NotMapped]
        public string GradeName { get; set; }
        [NotMapped]
        public string EmployeeName { get; set; }
        [NotMapped]
        public string EmployeeCode { get; set; }
        public EnumSalaryType SalaryType { get; set; }
        public int IsAdvanceLoanPay { get; set; }

    }
}
