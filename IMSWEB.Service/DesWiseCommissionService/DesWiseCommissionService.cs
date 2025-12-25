using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class DesWiseCommissionService : IDesWiseCommissionService
    {
        private readonly IBaseRepository<DesWiseCommission> _baseRepository;
        private readonly IBaseRepository<Designation> _DesignationRepository;
        private readonly IUnitOfWork _unitOfWork;


        public DesWiseCommissionService(IBaseRepository<DesWiseCommission> baseRepository, IBaseRepository<Designation> DesignationRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _DesignationRepository = DesignationRepository;
        }

        public void Add(DesWiseCommission DesWiseCommission)
        {
           _baseRepository.Add(DesWiseCommission);
        }

        public void Update(DesWiseCommission DesWiseCommission)
        {
            _baseRepository.Update(DesWiseCommission);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<DesWiseCommission> GetAll()
        {
            return _baseRepository.All.ToList();
        }
        public IQueryable<DesWiseCommission> GetAllIQueryable()
        {
            return _baseRepository.All;
        }
        public async Task<IEnumerable<Tuple<int, Decimal, string>>> GetAllAsync()
        {
            return await _baseRepository.GetAllGradeAsync(_DesignationRepository);
        }

        public DesWiseCommission GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ID == id);
        }
    }
}
