using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Data;
namespace IMSWEB.Service
{
    public class EmpGradeSalaryAssignmentService : IEmpGradeSalaryAssignmentService
    {

        private readonly IBaseRepository<EmpGradeSalaryAssignment> _baseRepository;
        private readonly IBaseRepository<Grade> _GradeRepository;
        private readonly IBaseRepository<GradeSalaryChangeType> _GradeSalaryChangeTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
       

        public EmpGradeSalaryAssignmentService(IBaseRepository<EmpGradeSalaryAssignment> baseRepository,
             IBaseRepository<Grade> GradeRepository,IBaseRepository<GradeSalaryChangeType> GradeSalaryChangeTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _GradeRepository = GradeRepository;
            _GradeSalaryChangeTypeRepository = GradeSalaryChangeTypeRepository;
            
        }

        public void Add(EmpGradeSalaryAssignment EmpGradeSalaryAssignment)
        {
            _baseRepository.Add(EmpGradeSalaryAssignment);
        }

        public void Update(EmpGradeSalaryAssignment EmpGradeSalaryAssignment)
        {
            _baseRepository.Update(EmpGradeSalaryAssignment);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<EmpGradeSalaryAssignment> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        //public async Task<IEnumerable<EmpGradeSalaryAssignment>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllGradeAsync();
        //}

        public EmpGradeSalaryAssignment GetById(int id)
        {
            return _baseRepository.FindBy(x => x.EmpGradeSalaryID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.EmpGradeSalaryID == id);
        }

        public IEnumerable<Tuple<int, string, string, decimal?, decimal, string>> GetAllByEmployeeID(int EmployeeID)
        {
            return _baseRepository.GetAllByEmployeeID(_GradeRepository,_GradeSalaryChangeTypeRepository,EmployeeID);
        }


        public EmpGradeSalaryAssignment GetLastGradeSalaryByEmployeeID(int EmployeeID)
        {
            return   _baseRepository.GetLastGradeSalaryByEmployeeID(EmployeeID);
        }

       
    }
}
