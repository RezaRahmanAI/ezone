using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ShareInvestmentHeadService : IShareInvestmentHeadService
    {
        private readonly IBaseRepository<ShareInvestmentHead> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public ShareInvestmentHeadService(IBaseRepository<ShareInvestmentHead> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(ShareInvestmentHead ShareInvestmentHead)
        {
            _baseRepository.Add(ShareInvestmentHead);
        }

        public void Update(ShareInvestmentHead ShareInvestmentHead)
        {
            _baseRepository.Update(ShareInvestmentHead);
        }

        //public void Delete(ShareInvestmentHead ShareInvestmentHead)
        //{
        //    _baseRepository.Delete(ShareInvestmentHead);
        //}

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<ShareInvestmentHead> GetAll()
        {
            return _baseRepository.All;
        }
        public async Task<IEnumerable<Tuple<int, string, string, string>>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync();
        }
        public ShareInvestmentHead GetById(int id)
        {
            return _baseRepository.FindBy(x => x.SIHID == id).First();
        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.SIHID == id);
        }

        public bool IsChildExist(int SIHID)
        {
            if (_baseRepository.All.Any(i => i.ParentId == SIHID))
                return true;

            return false;
        }
        public IEnumerable<Tuple<int, string, string, string>> GetListByID(int ID)
        {
            return _baseRepository.GetByID(ID);
        }

        public bool IsTransactionFound(int id, int concernId)
        {
            string query = string.Format(@"SELECT COUNT(SIID) FROM dbo.ShareInvestments
                                            WHERE SIHID = {0} AND ConcernID = {1}", id, concernId);
            int result = 0;
            try
            {
                result = _baseRepository.SQLQuery<int>(query);
            }
            catch (Exception ex)
            {

            }
            return result > 0;
        }
    }
}
