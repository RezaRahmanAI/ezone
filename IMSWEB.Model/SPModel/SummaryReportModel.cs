
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class SummaryReportModel
    {
        public int id { get; set; }
        public string Category { get; set; }
        public string Head { get; set; }
        public decimal Total{ get; set; }
        public decimal Amount { get; set; }
    }
}
