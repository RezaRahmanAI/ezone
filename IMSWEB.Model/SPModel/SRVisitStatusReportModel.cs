using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB.SPViewModels
{
    public class SRVisitStatusReportModel
    {
        public DateTime SRVDate { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Company { get; set; }
        public decimal RQty { get; set; }
        public string RIMENO { get; set; }
        public decimal SQty { get; set; }
        public string SIMENO { get; set; }
    }
}