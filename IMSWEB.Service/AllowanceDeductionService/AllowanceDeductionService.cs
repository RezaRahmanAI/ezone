using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class AllowanceDeductionService : IAllowanceDeductionService
    {
        private readonly IBaseRepository<AllowanceDeduction> _BaseAllowanceDeductionRepository;
        private readonly IUnitOfWork _unitOfWork;


        public AllowanceDeductionService(IBaseRepository<AllowanceDeduction> BaseAllowanceDeductionRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _BaseAllowanceDeductionRepository = BaseAllowanceDeductionRepository;
        }

        public void Add(AllowanceDeduction AllowanceDeduction)
        {
           _BaseAllowanceDeductionRepository.Add(AllowanceDeduction);
        }

        public void Update(AllowanceDeduction AllowanceDeduction)
        {
            _BaseAllowanceDeductionRepository.Update(AllowanceDeduction);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<AllowanceDeduction> GetAll()
        {
            return _BaseAllowanceDeductionRepository.All.ToList();
        }

        public async Task<IEnumerable<AllowanceDeduction>> GetAllAllowacneAsync()
        {
            return await _BaseAllowanceDeductionRepository.GetAllAllowanceAsync();
        }

        public AllowanceDeduction GetById(int id)
        {
            return _BaseAllowanceDeductionRepository.FindBy(x => x.AllowDeductID == id).First();

        }

        public void Delete(int id)
        {

            _BaseAllowanceDeductionRepository.Delete(x => x.AllowDeductID == id);
        }


        public async Task<IEnumerable<AllowanceDeduction>> GetAllDeductionAsync()
        {
            return await _BaseAllowanceDeductionRepository.GetAllDeductionAsync();
        }


       
    }
}
