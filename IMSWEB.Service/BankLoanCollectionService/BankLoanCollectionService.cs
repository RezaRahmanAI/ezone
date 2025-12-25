using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Data.Repositories;

namespace IMSWEB.Service
{
    public class BankLoanCollectionService : IBankLoanCollectionService
    {
        private readonly IBaseRepository<BankLoanCollection> _baseRepository;
        private readonly IBaseRepository<BankLoan> _bankLoanRepository;
        private readonly IBaseRepository<BankLoanDetails> _bankLoanDetailsRepository;
        private readonly IBaseRepository<Bank> _bankRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBankLoanCollectionRepository _bankLoanCollectionRepository;

        public BankLoanCollectionService(IBaseRepository<BankLoanCollection> baseRepository, IUnitOfWork unitOfWork, IBaseRepository<BankLoan> bankLoanRepository, IBaseRepository<BankLoanDetails> bankLoanDetailsRepository, IBaseRepository<Bank> bankRepository, IBankLoanCollectionRepository bankLoanCollectionRepository)
        {
            _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
            _bankLoanRepository = bankLoanRepository;
            _bankLoanDetailsRepository = bankLoanDetailsRepository;
            _bankRepository = bankRepository;
            _bankLoanCollectionRepository = bankLoanCollectionRepository;

        }

        public void Add(BankLoanCollection model)
        {
            _baseRepository.Add(model);
        }

        public void Update(BankLoanCollection model)
        {
            _baseRepository.Update(model);
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<BankLoanCollection> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        public async Task<IEnumerable<BankLoanCollectionTO>> GetAllAsync(DateTime fromDate, DateTime toDate)
        {
            return await _baseRepository.GetAllBankLoanCollectionAsync(_bankLoanRepository, _bankLoanDetailsRepository, _bankRepository, fromDate, toDate);
        }

        public BankLoanCollection GetById(int id)
        {
            return _baseRepository.FindBy(x=>x.Id == id).First();
        }

        public int GetLastCollectionIdForCCLoan(int id)
        {
            BankLoanCollection collection = _baseRepository.FindBy(x => x.Id == id).First();
            BankLoanCollection lastCollection = _baseRepository.All.Where(d => d.CCLoanId == collection.CCLoanId).OrderByDescending(d => d.Id).FirstOrDefault();
            return lastCollection != null ? lastCollection.Id : 0;

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.Id == id);
        }

        public bool IsDeleteAllowed(int collectionId)
        {

            BankLoanCollection collection = _baseRepository.FindBy(d => d.Id == collectionId).FirstOrDefault();

            bool isAllowed = true;
            if (collection != null)
            {
                BankLoanDetails loanDetails = _bankLoanDetailsRepository.FindBy(d => d.LoanCollectionId == collection.Id).FirstOrDefault();
                if (loanDetails != null)
                {
                    BankLoanDetails nextLoanDueOrPaid = _bankLoanDetailsRepository.FindBy(d => d.Id > loanDetails.Id && d.BankLoanId == loanDetails.BankLoanId && d.Status.Equals("Paid")).FirstOrDefault();
                    isAllowed = nextLoanDueOrPaid == null;
                }
            }

            return isAllowed;
        }

        public BankLoan GetBankLoanByCollectionId(int collectionId)
        {
            var data = (from bc in _baseRepository.All
                        join bld in _bankLoanDetailsRepository.All on bc.Id equals bld.LoanCollectionId
                        join bl in _bankLoanRepository.All on bld.BankLoanId equals bl.Id
                        where bc.Id == collectionId
                        select bl).FirstOrDefault();
            return data;
        }

        public bool DelecteBankLoanCollectionUsingSP(int collectionId, EnumBankLoanType collectionType)
        {
            return _bankLoanCollectionRepository.DelecteBankLoanCollectionUsingSP(collectionId, collectionType);
        }

        public RPTBankLoanCollectionInvTO GetLoanCollectionInvoiceData(int collectionId)
        {

            BankLoanCollection collection = _baseRepository.All.Where(d => d.Id == collectionId).FirstOrDefault();

            if (collection.CollectionType == EnumBankLoanType.Normal)
            {
                var data = (
                            from bc in _baseRepository.All
                            join bld in _bankLoanDetailsRepository.All on bc.Id equals bld.LoanCollectionId
                            join bl in _bankLoanRepository.All on bld.BankLoanId equals bl.Id
                            join b in _bankRepository.All on bl.BankId equals b.BankID
                            where bc.Id == collectionId
                            select new RPTBankLoanCollectionInvTO
                            {
                                ReceiptNo = bc.Code,
                                CollectionDate = bc.CollectionDate,
                                LoanAmount = bld.ExpectedInstallmentAmount,
                                ReceiveAmount = bc.CollectionAmount,
                                BankName = b.BankName,
                                InstallmentDate = bld.InstallmentDate,
                                SDPS = bc.SDPS,
                                Savings = bc.Savings,
                                CollectionType = "Normal"
                            }
                        ).FirstOrDefault();

                return data;
            }
            else
            {
                return new RPTBankLoanCollectionInvTO();
            }
            
        }

        public List<RPTBankDueLoanTO> GetAllPendingLoanAsOnDate(DateTime currentDate)
        {
            return _bankLoanCollectionRepository.GetAllPendingLoanAsOnDate(_bankLoanDetailsRepository, _bankLoanRepository, _bankRepository, currentDate);
        }

        public bool IsCollectionFoundByCCLoanId(int id)
        {
            List<BankLoanCollection> data = _baseRepository.FindBy(d => d.CCLoanId == id).ToList();
            return data.Any();
        }

        public Tuple<int, int> GetLoanIdByCollectionId(int id)
        {
            BankLoanDetails details = _bankLoanDetailsRepository.FindBy(d => d.LoanCollectionId != null && d.LoanCollectionId == id).FirstOrDefault();
            return details != null ? new Tuple<int, int>(details.BankLoanId, details.Id) : new Tuple<int, int>(0, 0);
        }

    }
}
