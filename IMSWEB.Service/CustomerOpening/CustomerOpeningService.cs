using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CustomerOpeningService : ICustomerOpeningService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IBaseRepository<SOrder> _baseSalesOrderRepository;
        private readonly IBaseRepository<CashCollection> _cashCollectionRepository;
        private readonly IBaseRepository<BankTransaction> _BankTransactionRepository;
        private readonly IBaseRepository<CustomerOpeningDue> _CustomerOpeningDueRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;

        public CustomerOpeningService(IBaseRepository<CustomerOpeningDue> prevBalanceRepository, IBaseRepository<SOrder> baseSalesOrderRepository,
            IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
            IUnitOfWork unitOfWork, IBaseRepository<CustomerOpeningDue> CustomerOpeningDueRepository, IBaseRepository<Customer> CustomerRepository)
        {
            _unitOfWork = unitOfWork;
            _BankTransactionRepository = BankTransactionRepository;
            _CustomerOpeningDueRepository = CustomerOpeningDueRepository;
            _CustomerRepository = CustomerRepository;
            _baseSalesOrderRepository = baseSalesOrderRepository;
            _cashCollectionRepository = cashCollectionRepository;
        }
        public void Add(CustomerOpeningDue model)
        {
            _CustomerOpeningDueRepository.Add(model);
        }



        public void Update(CustomerOpeningDue model)
        {
            _CustomerOpeningDueRepository.Update(model);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<CustomerOpeningDue> GetAll()
        {
            return _CustomerOpeningDueRepository.GetAll();
        }

        public CustomerOpeningDue GetById(int id)
        {
            return _CustomerOpeningDueRepository.FindBy(i => i.ID == id).FirstOrDefault();
        }

        public void Delete(int id)
        {
            _CustomerOpeningDueRepository.Delete(i => i.ID == id);
        }


        public List<CustomerOpeningDue> CustomerOpeningDueSave(int ConcernID)
        {
            return _CustomerOpeningDueRepository.CustomerOpeningDueSave(_baseSalesOrderRepository, _cashCollectionRepository,
                _BankTransactionRepository, _CustomerRepository, ConcernID);
        }
    }
}
