using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class AdvanceLoanReportModel
    {
        public DateTime RecDate { get; set; }
        public decimal ReceiveAmt { get; set; }
        public DateTime PayDate { get; set; }
        public decimal PayAmt { get; set; }

        public string EmployeeName { get; set; }
     
        public string RecPurpose { get; set; }
        public string PayPurpose { get; set; }
        public int EmployeeID { get; set; }
        public EnumSalaryType RecSalaryType { get; set; }
        public EnumSalaryType PaySalaryType { get; set; }

     

    }
}
