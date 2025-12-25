using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace IMSWEB.Service
{
    public class CashCollectionService : ICashCollectionService
    {
        private readonly IBaseRepository<CashCollection> _BaseCashCollectionRepository;
        private readonly IBaseRepository<BankTransaction> _BaseBankTransactionRepository;
        private readonly IBaseRepository<Bank> _BaseBankRepository;
        private readonly ICashCollectionRepository _cashCollectionRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<Supplier> _SupplierRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly ISisterConcernService _sisterConcernService;


        private readonly IBaseRepository<POrderDetail> _POrderDetailRepository;
        private readonly IBaseRepository<POrder> _POrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;

        private readonly IBaseRepository<CreditSale> _CreditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSaleDetailsRepository;
        private readonly IBaseRepository<CreditSalesSchedule> _CreditSalesScheduleRepository;
        private readonly IBaseRepository<Stock> _StockRepository;
        private readonly IBaseRepository<StockDetail> _StockDetailRepository;

        private readonly IBaseRepository<ExpenseItem> _ExpenseItemRepository;
        private readonly IBaseRepository<Expenditure> _ExpenditureRepository;
        private readonly IBaseRepository<Bank> _BankRepository;
        private readonly IBaseRepository<BankTransaction> _BankTransactionRepository;
        private readonly IBaseRepository<ROrder> _ROrderRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<ApplicationUser> _UserRepository;



        public CashCollectionService(IBaseRepository<CashCollection> baseCashCollectionRepository,
            IBaseRepository<BankTransaction> BaseBankTransactionRepository,
             IBaseRepository<Bank> BaseBankRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            ICashCollectionRepository cashCollectionRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> EmployeeRepository,
            IBaseRepository<Supplier> supplierRepository,

                IBaseRepository<POrder> POrderRepository,
            IBaseRepository<POrderDetail> POrderDetailRepository,
            IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<SOrderDetail> SOrderDetailRepository,

             IBaseRepository<CreditSale> CreditSaleRepository,
         IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
         IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
         IBaseRepository<Stock> StockRepository,
         IBaseRepository<StockDetail> StockDetailRepository,

          IBaseRepository<ExpenseItem> ExpenseItemRepository,
         IBaseRepository<Expenditure> ExpenditureRepository,
          IBaseRepository<Bank> BankRepository,
          IBaseRepository<BankTransaction> BankTransactionRepository,

            IBaseRepository<ROrder> ROrderRepository,
          IBaseRepository<ROrderDetail> ROrderDetailRepository,
          ISisterConcernService sisterConcernService,

            IUnitOfWork unitOfWork, IBaseRepository<ApplicationUser> userRepository)
        {
            _BaseCashCollectionRepository = baseCashCollectionRepository;
            _cashCollectionRepository = cashCollectionRepository;
            _SupplierRepository = supplierRepository;
            _CustomerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _BaseBankTransactionRepository = BaseBankTransactionRepository;
            _BaseBankRepository = BaseBankRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _SOrderRepository = SOrderRepository;

            _POrderDetailRepository = POrderDetailRepository;
            _POrderRepository = POrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _SOrderRepository = SOrderRepository;

            _CreditSaleRepository = CreditSaleRepository;
            _CreditSaleDetailsRepository = CreditSaleDetailsRepository;
            _CreditSalesScheduleRepository = CreditSalesScheduleRepository;
            _StockRepository = StockRepository;
            _StockDetailRepository = StockDetailRepository;

            _ExpenseItemRepository = ExpenseItemRepository;
            _ExpenditureRepository = ExpenditureRepository;
            _BankRepository = BankRepository;
            _BankTransactionRepository = BankTransactionRepository;

            _ROrderRepository = ROrderRepository;
            _ROrderDetailRepository = ROrderDetailRepository;
            _UserRepository = userRepository;
            _sisterConcernService = sisterConcernService;


        }

        public void AddCashCollection(CashCollection cashCollection)
        {
            _BaseCashCollectionRepository.Add(cashCollection);
        }

        public void UpdateCashCollection(CashCollection cashCollection)
        {
            _BaseCashCollectionRepository.Update(cashCollection);
        }

        public void SaveCashCollection()
        {
            _unitOfWork.Commit();
        }

        public void UpdateTotalDue(int CustomerID, int SupplierID, int BankID, int BankWithdrawID, decimal TotalDue)
        {
            _cashCollectionRepository.UpdateTotalDue(CustomerID, SupplierID, BankID, BankWithdrawID, TotalDue);
        }

        public void UpdateTotalDuePaymentReturn(int SupplierID, int BankID, int BankWithdrawID, decimal TotalDue)
        {
            _cashCollectionRepository.UpdateTotalDuePaymentReturn(SupplierID, BankID, BankWithdrawID, TotalDue);
        }
        public void UpdateTotalDueCustomerPaymentReturn(int CustomerID, int BankID, int BankWithdrawID, decimal TotalDue)
        {
            _cashCollectionRepository.UpdateTotalDueCustomerPaymentReturn(CustomerID, BankID, BankWithdrawID, TotalDue);
        }

        public IEnumerable<CashCollection> GetAllCashCollection()
        {
            return _BaseCashCollectionRepository.All.ToList();
        }
        public IQueryable<CashCollection> GetAllIQueryable()
        {
            return _BaseCashCollectionRepository.All;
        }
        public async Task<IEnumerable<Tuple<int, DateTime, string, string,
        string, string, string, Tuple<string, string>>>> GetAllCashCollAsync(DateTime fromDate, DateTime toDate)
        {
            return await _BaseCashCollectionRepository.GetAllCashCollAsync(_CustomerRepository, fromDate, toDate);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, string, string,
        string, string, string>>> GetAllCashDelivaeryAsync(DateTime fromDate, DateTime toDate)
        {
            return await _BaseCashCollectionRepository.GetAllCashDelivaeryAsync(_SupplierRepository, fromDate, toDate);
        }

        public async Task<IEnumerable<CashCollection>> GetAllCashCollectionAsync()
        {
            return await _BaseCashCollectionRepository.GetAllCashCollectionAsync();
        }

        public CashCollection GetCashCollectionById(int id)
        {
            return _BaseCashCollectionRepository.FindBy(x => x.CashCollectionID == id).First();
        }

        public void DeleteCashCollection(int id)
        {
            _BaseCashCollectionRepository.Delete(x => x.CashCollectionID == id);
        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, string, string, string, string, string, EnumCustomerType>>>
        GetCashCollectionData(DateTime fromDate, DateTime toDate, int ConcernId, int CustomerID, EnumCustomerType customerType)
        {
            return _BaseCashCollectionRepository.GetCashCollectionData(_CustomerRepository, fromDate, toDate, ConcernId, CustomerID, customerType);
        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>>
        GetCashDeliveryData(DateTime fromDate, DateTime toDate, int ConcernId, int SupplierID, bool IsAdmin)
        {
            return _BaseCashCollectionRepository.GetCashDeliveryData(_SupplierRepository, fromDate, toDate, ConcernId, SupplierID, IsAdmin, _SisterConcernRepository);
        }



        public IEnumerable<DailyCashBookLedgerModel> DailyCashBookLedger(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _cashCollectionRepository.DailyCashBookLedger(fromDate, toDate, ConcernID);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string>>>> GetAllCashCollByEmployeeIDAsync(int EmployeeID)
        {
            return await _BaseCashCollectionRepository.GetAllCashCollByEmployeeIDAsync(_CustomerRepository, EmployeeID);
        }


        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>> GetSRWiseCashCollectionReportData(DateTime fromDate, DateTime toDate, int concernID, int EmployeeID)
        {
            return _BaseCashCollectionRepository.GetSRWiseCashCollectionReportData(_CustomerRepository, _EmployeeRepository, _BaseBankRepository, _BaseBankTransactionRepository, fromDate, toDate, concernID, EmployeeID);
        }

        public void UpdateTotalDueWhenEdit(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt)
        {
            _cashCollectionRepository.UpdateTotalDueWhenEdit(CustomerID, SupplierID, CashCollectionID, TotalRecAmt);
        }

        public void UpdateTotalDueWhenEditReturnType(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt)
        {
            _cashCollectionRepository.UpdateTotalDueWhenEditReturnType(CustomerID, SupplierID, CashCollectionID, TotalRecAmt);
        }
        public IQueryable<CashCollectionReportModel> AdminCashCollectionReport(DateTime fromDate,
            DateTime toDate, int ConcernID, EnumCustomerType customerType, int customerID)
        {
            return _BaseCashCollectionRepository.AdminCashCollectionReport(_CustomerRepository,
                _SisterConcernRepository, fromDate, toDate, ConcernID, customerType, customerID);
        }

        public IEnumerable<CashInHandReportModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType)
        {
            return _cashCollectionRepository.CashInHandReport(fromDate, toDate, ReportType, ConcernID, CustomerType);
        }
        public bool IsCommissionApplicable(DateTime fromDate, DateTime toDate, int EmployeeID)
        {
            return _BaseCashCollectionRepository.IsCommissionApplicable(_CustomerRepository, _BaseBankTransactionRepository, _SOrderRepository, fromDate, toDate, EmployeeID);
        }

        public List<CashInHandModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _BaseCashCollectionRepository.CashInHandReport(



                _POrderRepository,
                _POrderDetailRepository,
                _SOrderRepository,
                _SOrderDetailRepository,
                _CreditSaleRepository,
                _CreditSaleDetailsRepository,
                _CreditSalesScheduleRepository,

                _StockRepository,
                _StockDetailRepository,
                 _ExpenseItemRepository,

               _ExpenditureRepository,
               _BankRepository,
               _BankTransactionRepository,

              _ROrderRepository,
             _ROrderDetailRepository,

                 _CustomerRepository,



                _SisterConcernRepository,


                fromDate,
                toDate,
                ConcernID
                );
        }



        public List<CashInHandModel> ProfitAndLossReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _BaseCashCollectionRepository.ProfitAndLossReport(



                _POrderRepository,
                _POrderDetailRepository,
                _SOrderRepository,
                _SOrderDetailRepository,
                _CreditSaleRepository,
                _CreditSaleDetailsRepository,
                _CreditSalesScheduleRepository,

                _StockRepository,
                _StockDetailRepository,
                 _ExpenseItemRepository,

               _ExpenditureRepository,
               _BankRepository,
               _BankTransactionRepository,
                _ROrderRepository,
             _ROrderDetailRepository,
                 _CustomerRepository,



                _SisterConcernRepository,


                fromDate,
                toDate,
                ConcernID
                );
        }
        public List<SummaryReportModel> SummaryReport(DateTime fromDate, DateTime toDate, decimal OpeningCashInHand, decimal CurrentCashInHand, decimal ClosingCashInHand, int ConcernID)
        {
            return _BaseCashCollectionRepository.SummaryReport(



                _POrderRepository,
                _POrderDetailRepository,
                _SOrderRepository,
                _SOrderDetailRepository,
                _CreditSaleRepository,
                _CreditSaleDetailsRepository,
                _CreditSalesScheduleRepository,

                _StockRepository,
                _StockDetailRepository,
                 _ExpenseItemRepository,

               _ExpenditureRepository,
               _BankRepository,
               _BankTransactionRepository,

                 _CustomerRepository,



                _SisterConcernRepository,


                fromDate,
                toDate,
               OpeningCashInHand,
               CurrentCashInHand,
               ClosingCashInHand,
                ConcernID
                );
        }


        public List<TransactionReportModel> MonthlyTransactionReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _BaseCashCollectionRepository.MonthlyTransactionReport(
               _POrderRepository,
               _POrderDetailRepository,
               _SOrderRepository,
               _SOrderDetailRepository,
               _CreditSaleRepository,
               _CreditSaleDetailsRepository,
               _CreditSalesScheduleRepository,
               _StockRepository,
               _StockDetailRepository,
               _ExpenseItemRepository,
               _ExpenditureRepository,
               _BankRepository,
               _BankTransactionRepository,
               _ROrderRepository,
               _ROrderDetailRepository,
               _CustomerRepository,
               _SisterConcernRepository,
                fromDate,
                toDate,
                ConcernID
                );
        }


        public List<TransactionReportModel> MonthlyAdminTransactionReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _BaseCashCollectionRepository.MonthlyAdminTransactionReport(_POrderRepository,
               _POrderDetailRepository,
               _SOrderRepository,
               _SOrderDetailRepository,
               _CreditSaleRepository,
               _CreditSaleDetailsRepository,
               _CreditSalesScheduleRepository,
               _StockRepository,
               _StockDetailRepository,
               _ExpenseItemRepository,
               _ExpenditureRepository,
               _BankRepository,
               _BankTransactionRepository,
               _ROrderRepository,
               _ROrderDetailRepository,
               _CustomerRepository,
               _SisterConcernRepository,
                fromDate,
                toDate,
                ConcernID
                );
        }

        public Tuple<decimal, decimal, decimal, decimal, decimal> CustomerWiseCashCollection(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _BaseCashCollectionRepository.CustomerWiseCashCollection(_SOrderRepository, _CreditSaleRepository,
                _CreditSalesScheduleRepository, _CustomerRepository, _BankRepository, _BankTransactionRepository, fromDate, toDate, ConcernID);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, string, string,
                         string, string, string, Tuple<string, string, string>>>> GetAllPendingCashCollAsync()
        {
            return await _BaseCashCollectionRepository.GetAllPendingCashCollAsync(_CustomerRepository, _UserRepository);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, string, string,
       string, string, string>>> GetAllCashDelivaeryAsync(DateTime fromDate, DateTime toDate, List<EnumTranType> enumTranTypes)
        {
            return await _BaseCashCollectionRepository.GetAllCashDelivaeryAsync(_SupplierRepository, fromDate, toDate, enumTranTypes);
        }
        public void UpdateTotalDueForInvestment(int SIHID, int SIHId, int BankID, int BankWithdrawID, decimal TotalDue)
        {
            _cashCollectionRepository.UpdateTotalDueForInvestment(SIHID, SIHId, BankID, BankWithdrawID, TotalDue);
        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, string, string, string, string, string, EnumCustomerType>>>
        GetCashCollectionDataFoAll(DateTime fromDate, DateTime toDate, int ConcernId, int CustomerID, EnumCustomerType customerType)
        {
            return _BaseCashCollectionRepository.GetCashCollectionDataForAll(_CustomerRepository, fromDate, toDate, ConcernId, CustomerID, customerType);
        }

        public void UpdateTotalDueForExpenditure(int ExpenseItemID, int BankID, int BankWithdrawID, decimal TotalDue, int ConcernID, int userId, string Remarks, DateTime TranDate)
        {
            _cashCollectionRepository.UpdateTotalDueForExpenditure(ExpenseItemID, BankID, BankWithdrawID, TotalDue, ConcernID, userId, Remarks, TranDate);
        }


        public IEnumerable<AdjustmentReportModel> GetAdjustmentReport(DateTime fromDate, DateTime toDate)
        {
            return _BaseCashCollectionRepository.GetAdjustmentReport(_CustomerRepository, fromDate, toDate);
        }

        public IEnumerable<CashCollectionReportModel> CashCollectionReportData(DateTime fromDate,
        DateTime toDate, int ConcernID, int customerID, int ReportType)
        {
            return _BaseCashCollectionRepository.CashCollectionReportData(_CustomerRepository,
                _SisterConcernRepository, _SOrderRepository, _CreditSaleRepository, _CreditSalesScheduleRepository, _BankTransactionRepository, _BankRepository, fromDate, toDate, ConcernID, customerID, ReportType);
        }

        public IEnumerable<ServiceChargeModel> ServiceCharge(int Month, int Year)
        {
            return _cashCollectionRepository.ServiceChargeReport(Month, Year);
        }




        public IEnumerable<RPTCustomerAdjustmentDateWise> GetCustomerDateWiseAdjustmet(int concernId, DateTime fromDate, DateTime toDate, EnumTranType adjustmentType, int CustomerId)
        {
            return _BaseCashCollectionRepository.GetDebitCreditAdjustmentReport(_CustomerRepository, concernId,
              fromDate, toDate, adjustmentType, CustomerId);
        }



        public IEnumerable<RPTCustomerAdjustmentDateWise> GetSupplierDebitAdjustment(int concernId, DateTime fromDate, DateTime toDate, EnumTranType adjustmentType, int SupplierId)
        {
            return _BaseCashCollectionRepository.GetSupplierDebitCreditAdjustmentReport(_SupplierRepository, concernId,
              fromDate, toDate, adjustmentType, SupplierId);
        }

        public IEnumerable<CashCollectionReportModel> DiscountAdjReportData(DateTime fromDate,
        DateTime toDate, int ConcernID, int customerID, int ReportType)
        {
            return _BaseCashCollectionRepository.DiscountAdjReportData(_CustomerRepository,
                _SisterConcernRepository, _SOrderRepository, _CreditSaleRepository, _CreditSalesScheduleRepository, _BankTransactionRepository, _BankRepository, fromDate, toDate, ConcernID, customerID, ReportType);
        }



        public IEnumerable<CashInHandReportModel> AdminCashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int SelectedConcern)
        {
            if (SelectedConcern == 0)
            { 
                var concernData = _sisterConcernService.GetFamilyTree(ConcernID);
                return _cashCollectionRepository.AdminCashInHandReport(fromDate, toDate, ReportType, ConcernID, SelectedConcern, concernData);
            }
            else
            {  if (SelectedConcern > 0)
                {
                    ConcernID = SelectedConcern;
                }               
                return _cashCollectionRepository.CashInHandReport(fromDate, toDate, ReportType, ConcernID, 0);
            }

        }

    }
}
