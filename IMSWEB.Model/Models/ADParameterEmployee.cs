using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public partial class ADParameterEmployee
    {

        [Key]
        public string ADParameterEmpID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public Nullable<int> UserID { get; set; }
        public int ADParameterID { get; set; }
        public int AllowDeductID { get; set; }
        public int Periodicity { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> TillDate { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public int ArrearInfo { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ADEmpType { get; set; }
        public Nullable<int> ValueType { get; set; }

        public virtual ADParameterBasic ADParameterBasic { get; set; }
        public virtual Employee Employee { get; set; }
        //public virtual User User { get; set; }
    }
}
