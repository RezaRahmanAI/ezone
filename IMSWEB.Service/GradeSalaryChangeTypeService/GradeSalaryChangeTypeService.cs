using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class GradeSalaryChangeTypeService : IGradeSalaryChangeTypeService
    {
        private readonly IBaseRepository<GradeSalaryChangeType> _GradeSalaryChangeTypeRepository;
        private readonly IUnitOfWork _unitOfWork;


        public GradeSalaryChangeTypeService(IBaseRepository<GradeSalaryChangeType> BaseAllowanceDeductionRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _GradeSalaryChangeTypeRepository = BaseAllowanceDeductionRepository;
        }

        public void Add(GradeSalaryChangeType GradeSalaryChangeType)
        {
            _GradeSalaryChangeTypeRepository.Add(GradeSalaryChangeType);
        }

        public void Update(GradeSalaryChangeType GradeSalaryChangeType)
        {
            _GradeSalaryChangeTypeRepository.Update(GradeSalaryChangeType);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<GradeSalaryChangeType> GetAll()
        {
            return _GradeSalaryChangeTypeRepository.All.Where(i=>i.Status==(int)EnumActiveInactive.Active).ToList();
        }

        //public async Task<IEnumerable<GradeSalaryChangeType>> GetAllAsync()
        //{
        //    return await _GradeSalaryChangeTypeRepository.GetAllAllowanceAsync();
        //}

        public GradeSalaryChangeType GetById(int id)
        {
            return _GradeSalaryChangeTypeRepository.FindBy(x => x.GradeSalaryChangeTypeID == id).First();

        }

        public void Delete(int id)
        {

            _GradeSalaryChangeTypeRepository.Delete(x => x.GradeSalaryChangeTypeID == id);
        }
  
    }
}
