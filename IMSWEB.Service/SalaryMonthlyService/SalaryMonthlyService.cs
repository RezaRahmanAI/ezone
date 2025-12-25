using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SalaryMonthlyService : ISalaryMonthlyService
    {
        private readonly IBaseRepository<SalaryMonthly> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<SalaryMonthlyDetail> _SalaryMonthlyDetailRepsitory;

        public SalaryMonthlyService(IBaseRepository<SalaryMonthly> baseRepository,
            IBaseRepository<SalaryMonthlyDetail> SalaryMonthlyDetailRepsitory,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _SalaryMonthlyDetailRepsitory = SalaryMonthlyDetailRepsitory;
        }

        public void Add(SalaryMonthly SalaryMonthly)
        {
            _baseRepository.Add(SalaryMonthly);
        }

        public void Update(SalaryMonthly SalaryMonthly)
        {
            _baseRepository.Update(SalaryMonthly);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<SalaryMonthly> GetAllIQueryable()
        {
            return _baseRepository.All;
        }

        public async Task<IEnumerable<SalaryMonthly>> GetAllAsync()
        {
            return await _baseRepository.GetAllSalaryMonthlyAsync();
        }

        public SalaryMonthly GetById(int id)
        {
            return _baseRepository.FindBy(x => x.SalaryMonthlyID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.SalaryMonthlyID == id);
        }
        public List<SalaryMonthlyDetail> GetSalaryMonthlyDetailBy(int SalaryMonthlyID)
        {
            return _SalaryMonthlyDetailRepsitory.All.Where(i => i.SalaryMonthlyID == SalaryMonthlyID).ToList();
        }
    }
}
