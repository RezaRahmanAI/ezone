using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class SessionMasterService : ISessionMasterService
    {
        private readonly IBaseRepository<SessionMaster> _sessionMasterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SessionMasterService(IBaseRepository<SessionMaster> sessionMasterRepository, IUnitOfWork unitOfWork)
        {
            _sessionMasterRepository = sessionMasterRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(SessionMaster model)
        {
            _sessionMasterRepository.Add(model);
        }
        public void Update(SessionMaster model)
        {
            _sessionMasterRepository.Update(model);
        }

        public int GetActiveSessionId(int userId)
        {
            var data = _sessionMasterRepository.All.Where(d => d.UserID == userId).OrderByDescending(d => d.SessionStartDT).FirstOrDefault();
            return data != null ? data.SessionID : 0;
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IQueryable<SessionMaster> GetAll()
        {
            return _sessionMasterRepository.All.OrderBy(i => i.SessionID);
        }
    }
}
