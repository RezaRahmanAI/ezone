using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class CashCollTranHistoryService : ICashCollTranHistoryService
    {
        private readonly IBaseRepository<CashCollTranHistory> _cashCollTranHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<ExpenseItem> _expenseItemRepository;
        private readonly IBaseRepository<CashCollection> _cashCollectionRepository;
        private readonly IBaseRepository<CreditSale> _creditSalesRepository;
        private readonly IBaseRepository<POrder> _porderRepository;
        private readonly IBaseRepository<Expenditure> _expenditureRepository;
        private readonly IBaseRepository<SOrder> _sorderRepository;
        private readonly IBaseRepository<SisterConcern> _sisterConcernRepository;
        private readonly IBaseRepository<ApplicationUser> _userRepository;
        private readonly IBaseRepository<SessionMaster> _sessionMasterRepository;
        private readonly IBaseRepository<ApplicationUserRole> _userRoleRepository;
        private readonly IBaseRepository<ApplicationRole> _roleRepository;


        public CashCollTranHistoryService(IBaseRepository<CashCollTranHistory> cashCollTranHistoryRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Supplier> supplierRepository, IBaseRepository<ExpenseItem> expenseItemRepository, IBaseRepository<CashCollection> cashCollectionRepository,
            IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<POrder> porderRepository, IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<SOrder> sorderRepository, IBaseRepository<ApplicationUser> userRepository, IBaseRepository<SisterConcern> sisterConcernRepository,
            IBaseRepository<SessionMaster> sessionMasterRepository, IBaseRepository<ApplicationUserRole> userRoleRepository, IBaseRepository<ApplicationRole> roleRepository,
            IUnitOfWork unitOfWork)
        {
            _cashCollTranHistoryRepository = cashCollTranHistoryRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _expenseItemRepository = expenseItemRepository;
            _cashCollectionRepository = cashCollectionRepository;
            _creditSalesRepository = creditSalesRepository;
            _porderRepository = porderRepository;
            _expenditureRepository = expenditureRepository;
            _sorderRepository = sorderRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _sisterConcernRepository = sisterConcernRepository;
            _sessionMasterRepository = sessionMasterRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public void Add(CashCollTranHistory model)
        {
            _cashCollTranHistoryRepository.Add(model);
        }
        public void Update(CashCollTranHistory model)
        {
            _cashCollTranHistoryRepository.Update(model);
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
    }
}
