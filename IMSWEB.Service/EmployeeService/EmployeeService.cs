using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<Designation> _designationRepository;
        private readonly IBaseRepository<Department> _DepartmentRepository;
        private readonly IBaseRepository<Grade> _GradeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IBaseRepository<Employee> employeeRepository,
            IBaseRepository<Designation> designationRepository,
            IBaseRepository<Department> DepartmentRepository,
             IBaseRepository<Grade> GradeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _designationRepository = designationRepository;
            _unitOfWork = unitOfWork;
            _DepartmentRepository = DepartmentRepository;
            _GradeRepository = GradeRepository;
        }

        public void AddEmployee(Employee Employee)
        {
            _employeeRepository.Add(Employee);
        }

        public void UpdateEmployee(Employee Employee)
        {
            _employeeRepository.Update(Employee);
        }

        public void SaveEmployee()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeRepository.All.ToList();
        }
        public IQueryable<Employee> GetAllEmployeeIQueryable()
        {
            return _employeeRepository.All;
        }
        public IQueryable<Employee> GetAllEmployeeIQueryable(int ConcernID)
        {
            return _employeeRepository.GetAllByConcernID(ConcernID);
        }
        public async Task<IEnumerable<Tuple<int, string, string, string,
            string, DateTime, string, Tuple<int, EnumActiveInactive>>>> GetAllEmployeeAsync()
        {
            return await _employeeRepository.GetAllEmployeeAsync(_designationRepository);
        }

        public IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>> GetAllEmployeeDetails()
        {
            return _employeeRepository.GetAllEmployeeDetails(_designationRepository, _DepartmentRepository, _GradeRepository);
        }
        public Employee GetEmployeeById(int id)
        {
            return _employeeRepository.FindBy(x => x.EmployeeID == id).First();
        }

        public void DeleteEmployee(int id)
        {
            _employeeRepository.Delete(x => x.EmployeeID == id);
        }

        public IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>
           GetAllEmployeeDetails(int DepartmentID = 0)
        {
            return _employeeRepository.GetAllEmployeeDetails(_designationRepository, _DepartmentRepository, _GradeRepository, DepartmentID);
        }

        public string GetEmpNameById(int employeeId)
        {
            var emp = _employeeRepository.FindBy(e => e.EmployeeID == employeeId).FirstOrDefault();
            return emp != null ? emp.Name : string.Empty;
        }
    }
}
