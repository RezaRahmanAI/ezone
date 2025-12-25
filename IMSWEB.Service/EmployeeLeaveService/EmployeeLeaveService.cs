using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class EmployeeLeaveService : IEmployeeLeaveService
    {
        private readonly IBaseRepository<EmployeeLeave> _baseRepository;
        private readonly IBaseRepository<Department> _DepartmentRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<Designation> _DesignationRepository;
        private readonly IBaseRepository<Grade> _GradeRepository;
        private readonly IUnitOfWork _unitOfWork;


        public EmployeeLeaveService(IBaseRepository<EmployeeLeave> baseRepository,
            IBaseRepository<Department> DepartmentRepository, IBaseRepository<Employee> EmployeeRepository,
            IBaseRepository<Designation> DesignationRepository, IBaseRepository<Grade> GradeRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _DepartmentRepository = DepartmentRepository;
            _EmployeeRepository = EmployeeRepository;
            _DesignationRepository = DesignationRepository;
            _GradeRepository = GradeRepository;
        }

        public void Add(EmployeeLeave EmployeeLeave)
        {
            _baseRepository.Add(EmployeeLeave);
        }

        public void Update(EmployeeLeave EmployeeLeave)
        {
            _baseRepository.Update(EmployeeLeave);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<EmployeeLeave> GetAllIQueryable()
        {
            return _baseRepository.All.Where(i => i.Status == (int)EnumActiveInactive.Active);
        }
        public async Task<IEnumerable<Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>>> GetAllAsync(DateTime fromDate, DateTime toDate)
        {
            return await _baseRepository.GetAllAsync(_DepartmentRepository, _DesignationRepository, _GradeRepository, _EmployeeRepository, fromDate, toDate);
        }
        public EmployeeLeave GetById(int id)
        {
            return _baseRepository.FindBy(x => x.EmployeeLeaveID == id).First();
        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.EmployeeLeaveID == id);
        }
    }
}
