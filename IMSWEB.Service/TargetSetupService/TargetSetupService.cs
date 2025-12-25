using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class TargetSetupService : ITargetSetupService
    {
        private readonly IBaseRepository<TargetSetup> _BaseTargetSetupRepository;
        private readonly IBaseRepository<TargetSetupDetail> _TargetSetupDetailRepository;
        private readonly IBaseRepository<Designation> _DesignationRepository;
        private readonly IBaseRepository<Department> _DepartmentRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITargetSetupRepository _TargetSetupRepository;


        public TargetSetupService(IBaseRepository<TargetSetup> baseRepository,
            IBaseRepository<Designation> DesignationRepository, IBaseRepository<Department> DepartmentRepository,
            IBaseRepository<Employee> EmployeeRepository, ITargetSetupRepository TargetSetupRepository,
            IBaseRepository<TargetSetupDetail> TargetSetupDetailRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _BaseTargetSetupRepository = baseRepository;
            _DepartmentRepository = DepartmentRepository;
            _DesignationRepository = DesignationRepository;
            _EmployeeRepository = EmployeeRepository;
            _TargetSetupRepository = TargetSetupRepository;
            _TargetSetupDetailRepository = TargetSetupDetailRepository;
        }

        public void Add(TargetSetup TargetSetup)
        {
            _BaseTargetSetupRepository.Add(TargetSetup);
        }

        public void Update(TargetSetup TargetSetup)
        {
            _BaseTargetSetupRepository.Update(TargetSetup);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<TargetSetup> GetAllIQueryable()
        {
            return _BaseTargetSetupRepository.All.Where(i => i.Status == (int)EnumActiveInactive.Active);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>>> GetAllAsync()
        {
            return await _BaseTargetSetupRepository.GetAllAsync(_EmployeeRepository, _DesignationRepository, _DepartmentRepository);
        }

        public TargetSetup GetById(int id)
        {
            return _BaseTargetSetupRepository.FindBy(x => x.TID == id).First();

        }

        public void Delete(int id)
        {
            _BaseTargetSetupRepository.Delete(x => x.TID == id);
        }
        public bool AddTargetSetupUsingSP(DataTable dtTargetSetup, DataTable dtTargetSetupDetails)
        {
            return _TargetSetupRepository.AddTargetSetupUsingSP(dtTargetSetup, dtTargetSetupDetails);
        }
        public List<TargetSetupDetail> GetTargetSetupDetailsById(int TID)
        {
            return _TargetSetupDetailRepository.All.Where(i => i.TID == TID).ToList();
        }


        public bool DeleteTargetSetupUsingSP(int TID)
        {
            return _TargetSetupRepository.DeleteTargetSetupUsingSP(TID);
        }
        public TargetSetup GetByEmployeeIDandTargetMonth(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _BaseTargetSetupRepository.AllIncluding(i => i.TargetSetupDetails).FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.TargetMonth >= fromDate && i.TargetMonth <= toDate));
        }
    }
}
