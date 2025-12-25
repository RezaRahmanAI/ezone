using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model

{
    public class PaySlip
    {
        public string Allowance { get; set; }
        public decimal AllowanceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Deduction { get; set; }
        public decimal DeductionAmount { get; set; }
    }

    public class AdvanceSalaryReport
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal AdvanceAmt { get; set; }
        public int ID { get; set; }

        public DateTime Date { get; set; }
        
        public string Remarks { get; set; }
    }
}
