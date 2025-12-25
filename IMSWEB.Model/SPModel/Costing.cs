using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Costing
    {
        public int id { get; set; }
        public string Expense { get; set; }
        public decimal ExpenseAmt { get; set; }
        public string Income { get; set; }
        public decimal IncomeAmt { get; set; }
      
    }
}
