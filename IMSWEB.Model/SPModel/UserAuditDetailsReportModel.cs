using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class UserAuditDetailsReportModel
    {
        public DateTime EntryDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public EnumObjectType ObjectType { get; set; }
        public string Name { get; set; }
        public EnumActionType ActionType { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
