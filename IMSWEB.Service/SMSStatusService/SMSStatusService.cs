using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SMSStatusService : ISMSStatusService
    {
        private readonly IBaseRepository<SMSStatus> _SMSStatusRepository;
        private readonly IBaseRepository<SMSFormate> _SMSFormateRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<SisterConcern> _sisterConcernRepo;
        private readonly IBaseRepository<SystemInformation> _systemInfomationRepo;
        private readonly IUnitOfWork _unitOfWork;


        public SMSStatusService(IBaseRepository<SMSStatus> SMSStatusRepository,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<SMSFormate> SMSFormateRepository,
            IUnitOfWork unitOfWork, IBaseRepository<SisterConcern> sisterConcernRepo, IBaseRepository<SystemInformation> systemInfomationRepo)
        {
            _SMSStatusRepository = SMSStatusRepository;
            _unitOfWork = unitOfWork;
            _CustomerRepository = CustomerRepository;
            _SMSFormateRepository = SMSFormateRepository;
            _sisterConcernRepo = sisterConcernRepo;
            _systemInfomationRepo = systemInfomationRepo;
        }

        //public async Task<IEnumerable<SMSStatus>> GetAllAsync()
        //{
        //    return await _SMSStatusRepository.GetAllAsync();
        //}
        public void Add(SMSStatus SMSStatus)
        {
            _SMSStatusRepository.Add(SMSStatus);
        }

        public void AddRange(List<SMSStatus> SMSStatuses)
        {
            foreach (var item in SMSStatuses)
            {
                _SMSStatusRepository.Add(item);
            }
        }

        public void Update(SMSStatus SMSStatus)
        {
            _SMSStatusRepository.Update(SMSStatus);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<SMSStatus> GetAll()
        {
            return _SMSStatusRepository.All;
        }

        public SMSStatus GetById(int id)
        {
            return _SMSStatusRepository.FindBy(x => x.SMSStatusID == id).First();
        }

        public void Delete(int id)
        {
            _SMSStatusRepository.Delete(x => x.SMSStatusID == id);
        }

        public IQueryable<SMSStatus> GetAllIQueryable()
        {
            return _SMSStatusRepository.All;
        }

        public IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>>
             GetAll(DateTime fromDate, DateTime toDate, int Status)
        {
            return _SMSStatusRepository.GetAll(_CustomerRepository, _SMSFormateRepository, fromDate, toDate, Status);
        }


        public IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string, string, decimal>>>
            GetAllReport(DateTime fromDate, DateTime toDate, int Status, bool isAdminReport, int concernID)
        {
            return _SMSStatusRepository.GetAllReport(_CustomerRepository, _SMSFormateRepository, _sisterConcernRepo, _systemInfomationRepo, fromDate, toDate, Status, isAdminReport, concernID);
        }

        public IQueryable<SMSStatus> GetAllConcern()
        {
            return _SMSStatusRepository.GetAll();

        }
    }
}
