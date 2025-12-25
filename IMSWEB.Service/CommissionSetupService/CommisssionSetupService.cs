using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CommisssionSetupService : ICommisssionSetupService
    {
        private readonly IBaseRepository<CommissionSetup> _baseRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CommisssionSetupService(IBaseRepository<CommissionSetup> baseRepository,
            IBaseRepository<Employee> EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _EmployeeRepository = EmployeeRepository;
        }

        public void Add(CommissionSetup CommissionSetup)
        {
            _baseRepository.Add(CommissionSetup);
        }

        public void Update(CommissionSetup CommissionSetup)
        {
            _baseRepository.Update(CommissionSetup);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<CommissionSetup> GetAll()
        {
            return _baseRepository.All.Where(i => i.Status == (int)EnumActiveInactive.Active);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync(_EmployeeRepository);
        }

        public CommissionSetup GetById(int id)
        {
            return _baseRepository.FindBy(x => x.CSID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.CSID == id);
        }


        public IQueryable<CommissionSetup> GetByEmployeeIDandMonth(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.All.Where(i => i.EmployeeID == EmployeeID && i.CommissionMonth>=fromDate && i.CommissionMonth<=toDate);
        }
    }
}
