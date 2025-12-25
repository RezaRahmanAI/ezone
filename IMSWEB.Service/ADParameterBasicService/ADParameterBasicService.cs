using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ADParameterBasicService : IADParameterBasicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<ADParameterBasic> _ADParameterBasicRepository;
        private readonly IBaseRepository<ADParameterGrade> _ADParameterGradeRepository;
        private readonly IBaseRepository<ADParameterEmployee> _ADParameterEmployeeRepository;
        private readonly IBaseRepository<Grade> _GradeRepository;
        private readonly IBaseRepository<AllowanceDeduction> _AllowanceDeductionRepository;


        public ADParameterBasicService(IBaseRepository<AllowanceDeduction> BaseAllowanceDeductionRepository,
            IBaseRepository<ADParameterBasic> ADParameterBasicRepository, IBaseRepository<ADParameterGrade> ADParameterGradeRepository,
            IBaseRepository<ADParameterEmployee> ADParameterEmployeeRepository, IBaseRepository<Grade> GradeRepository, IBaseRepository<AllowanceDeduction> AllowanceDeductionRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ADParameterBasicRepository = ADParameterBasicRepository;
            _ADParameterGradeRepository = ADParameterGradeRepository;
            _ADParameterEmployeeRepository = ADParameterEmployeeRepository;
            _GradeRepository = GradeRepository;
            _AllowanceDeductionRepository = AllowanceDeductionRepository;
        }

        public IEnumerable<Tuple<int, List<string>, string, string, string, string>> GetAllowancesDeductionRuleSetupAsPerGrade(int AllowORDeduct)
        {
            return _ADParameterBasicRepository.GetAllowancesDeductionRuleSetupAsPerGrade(_ADParameterGradeRepository, _ADParameterEmployeeRepository, _AllowanceDeductionRepository, _GradeRepository, AllowORDeduct);
        }

        public List<Grade> GetUnassignedGrades(int AllowanceDeductID)
        {
            return _ADParameterBasicRepository.GetUnassignedGrades(_ADParameterGradeRepository, _ADParameterEmployeeRepository, _GradeRepository, AllowanceDeductID);
        }

        public void Add(ADParameterBasic Grade)
        {
            _ADParameterBasicRepository.Add(Grade);
        }

        public void Update(ADParameterBasic Grade)
        {
            _ADParameterBasicRepository.Update(Grade);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<ADParameterBasic> GetAll()
        {
            return _ADParameterBasicRepository.All.Where(i => i.Status == (int)EnumActiveInactive.Active).ToList();
        }

        //public async Task<IEnumerable<ADParameterBasic>> GetAllAsync()
        //{
        //    return await _ADParameterBasicRepository.GetAllGradeAsync();
        //}

        public ADParameterBasic GetById(int id)
        {
            return _ADParameterBasicRepository.FindBy(x => x.ADParameterID == id).First();

        }

        public void Delete(int id)
        {
            _ADParameterBasicRepository.Delete(x => x.ADParameterID == id);
        }
    }
}
