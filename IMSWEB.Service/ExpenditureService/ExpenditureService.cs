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
    public class ExpenditureService : IExpenditureService
    {
        private readonly IBaseRepository<Expenditure> _ExpenditureRepository;
        private readonly IBaseRepository<ExpenseItem> _expenseItemRepository;
        private readonly IBaseRepository<SOrder> _sorderRepository;
        private readonly IBaseRepository<ApplicationUser> _userRepository;
        private readonly IBaseRepository<SisterConcern> _sisterConcernRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ExpenditureService(IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepository, 
            IBaseRepository<ApplicationUser> userRepository, 
            IBaseRepository<SOrder> sorderRepository, IUnitOfWork unitOfWork,
            IBaseRepository<SisterConcern> sisterConcernRepo)
        {
            _ExpenditureRepository = expenditureRepository;
            _expenseItemRepository = expenseItemRepository;
            _sorderRepository = sorderRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _sisterConcernRepo = sisterConcernRepo;
        }

        public void AddExpenditure(Expenditure expenditure)
        {
            _ExpenditureRepository.Add(expenditure);
        }

        public void UpdateExpenditure(Expenditure expenditure)
        {
            _ExpenditureRepository.Update(expenditure);
        }

        public void SaveExpenditure()
        {
            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Expenditure>> GetAllExpenditureByUserIDAsync(int UserID, DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllExpenditureByUserID(_expenseItemRepository, UserID, fromDate, toDate);
        }

        public async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllExpenditureAsync(_expenseItemRepository, fromDate, toDate);
        }

        public Expenditure GetExpenditureById(int id)
        {
            return _ExpenditureRepository.FindBy(x => x.ExpenditureID == id).First();
        }
        public IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string,Tuple<int>>> 
            GetforExpenditureReport(DateTime fromDate, DateTime toDate, int concernID, 
            EnumCompanyTransaction Status,
            int ExpenseItemID, bool isAdminReport)
        {
            return _ExpenditureRepository.GetforReport(_expenseItemRepository, _userRepository, 
                _sisterConcernRepo,
                fromDate, toDate, concernID, Status, ExpenseItemID,isAdminReport);
        }

        public void DeleteExpenditure(int id)
        {
            _ExpenditureRepository.Delete(x => x.ExpenditureID == id);
        }

        public async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllIncomeAsync(_expenseItemRepository, fromDate, toDate);
        }
        public decimal GetExpenditureAmountByUserID(int UserID, DateTime fromDate, DateTime toDate)
        {
            return _ExpenditureRepository.GetAllExpenditureAmountByUserID(_expenseItemRepository, UserID, fromDate, toDate);
        }

        public async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(DateTime fromDate, DateTime toDate
           , List<EnumWFStatus> enumWFStatus)
        {
            return await _ExpenditureRepository.GetAllIncomeAsync(_expenseItemRepository, _userRepository, fromDate, toDate, enumWFStatus);
        }

        public async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate, List<EnumWFStatus> enumWFStatus)
        {
            return await _ExpenditureRepository.GetAllExpenditureAsync(_expenseItemRepository, fromDate, toDate, enumWFStatus);
        }
    }
}
