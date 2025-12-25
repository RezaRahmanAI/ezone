using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class EmployeeCommissionService : IEmployeeCommissionService
    {
        private readonly IBaseRepository<EmployeeCommission> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public EmployeeCommissionService(IBaseRepository<EmployeeCommission> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(EmployeeCommission EmployeeCommission)
        {
            _baseRepository.Add(EmployeeCommission);
        }

        public void Update(EmployeeCommission EmployeeCommission)
        {
            _baseRepository.Update(EmployeeCommission);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }


        public IQueryable<EmployeeCommission> GetAll()
        {
            return _baseRepository.All;
        }
        public EmployeeCommission GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ECID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ECID == id);
        }
    }
}
