using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISessionMasterService
    {
        void Add(SessionMaster model);
        void Update(SessionMaster model);
        bool Save();
        IQueryable<SessionMaster> GetAll();
        int GetActiveSessionId(int userId);
    }
}
