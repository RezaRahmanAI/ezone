using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IUserAuditDetailService
    {
        void Add(UserAuditDetail model);
        void Update(UserAuditDetail model);
        bool Save();
        IQueryable<UserAuditDetail> GetAll();
        IEnumerable<UserAuditDetailsReportModel> GetUserAuditReport(DateTime FromDate, DateTime ToDate, int ConcernID, EnumObjectType ObjectType);
    }
}
