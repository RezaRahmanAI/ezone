using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IBaseRepository<SOrder> _baseSalesOrderRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<SOrderDetail> _sOrderDetailRepository;
        private readonly IBaseRepository<POrderDetail> _POrderDetailRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<POProductDetail> _POProductDetailRepository;
        private readonly IBaseRepository<StockDetail> _stockdetailRepository;
        private readonly IBaseRepository<CashCollection> _cashCollectionRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<CreditSale> _creditSalesRepository;
        private readonly IBaseRepository<CreditSaleDetails> _creditSalesDetailsRepository;
        //private readonly IBaseRepository<CreditSaleP> _creditSalesDetailsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<POrder> _PorderRepository;
        private readonly IBaseRepository<ROrder> _RorderRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<Color> _ColorRepository;
        private readonly IBaseRepository<CreditSalesSchedule> _CreditSalesScheduleRepository;
        private readonly IBaseRepository<ExtraCommissionSetup> _ExtraCommissionSetupRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<BankTransaction> _BankTransactionRepository;
        private readonly IBaseRepository<HireSalesReturnCustomerDueAdjustment> _hireSalesReturn;
        private readonly IBaseRepository<TransactionSOrder> _transactionSOrderRepository;
        private readonly IBaseRepository<TransactionSOrderDetail> _transactionSOrderDetailRepository;


        private readonly IBaseRepository<ApplicationUser> _UserRepository;
        private readonly IBaseRepository<Bank> _BankRepository;
        public SalesOrderService(IBaseRepository<SOrder> baseSalesOrderRepository,
                    ISalesOrderRepository salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<SOrderDetail> sorderDetailRepository, IBaseRepository<Product> productRepository,
                    IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<POrderDetail> pOrderDetail, IBaseRepository<ROrderDetail> rOrderDetail,
                    IBaseRepository<POProductDetail> pOProductDetail, IBaseRepository<Employee> employeeRepository,
                    IBaseRepository<POrder> PorderRepository, IBaseRepository<ROrder> RorderRepository, IBaseRepository<Color> ColorRepository,
                    IBaseRepository<Company> CompanyRepository,
                    IBaseRepository<Category> CategoryRepository,
                    IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailsRepository,
                    IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
                    IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
                    IBaseRepository<ApplicationUser> UserRepository,
                    IBaseRepository<Bank> BankRepository, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturn,
                    IBaseRepository<TransactionSOrder> transactionSOrderRepository, IBaseRepository<TransactionSOrderDetail> transactionSOrderDetailRepository,
                    IUnitOfWork unitOfWork
                    )
        {

            _baseSalesOrderRepository = baseSalesOrderRepository;
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _sOrderDetailRepository = sorderDetailRepository;
            _stockdetailRepository = stockdetailRepository;
            _cashCollectionRepository = cashCollectionRepository;
            _ROrderDetailRepository = rOrderDetail;
            _POrderDetailRepository = pOrderDetail;
            _POProductDetailRepository = pOProductDetail;
            _employeeRepository = employeeRepository;
            _creditSalesDetailsRepository = creditSalesDetailsRepository;
            _creditSalesRepository = creditSalesRepository;
            _unitOfWork = unitOfWork;
            _PorderRepository = PorderRepository;
            _RorderRepository = RorderRepository;
            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;
            _ColorRepository = ColorRepository;
            _CreditSalesScheduleRepository = CreditSalesScheduleRepository;
            _ExtraCommissionSetupRepository = ExtraCommissionSetupRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _BankTransactionRepository = BankTransactionRepository;
            _UserRepository = UserRepository;
            _BankRepository = BankRepository;
            _hireSalesReturn = hireSalesReturn;
            _transactionSOrderRepository = transactionSOrderRepository;
            _transactionSOrderDetailRepository = transactionSOrderDetailRepository;
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string, int, decimal>>>> GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID
            , string InvoiceNo, string ContactNo , string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsync(_customerRepository,
                _SisterConcernRepository, fromDate, toDate, SalesType, IsVATManager, concernID,
                InvoiceNo, ContactNo, CustomerName,AccountNo);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsyncByUserID(int UserID,
            DateTime fromDate, DateTime toDate, EnumSalesType SalesType, 
            string InvoiceNo, string ContactNo, string CustomerName,string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsyncByUserID(_customerRepository,
                UserID, fromDate, toDate, SalesType,
                InvoiceNo, ContactNo, CustomerName, AccountNo);
        }
        public IQueryable<SOrder> GetAllIQueryable()
        {
            return _baseSalesOrderRepository.All;
        }
        public void AddSalesOrder(SOrder salesOrder)
        {
            _baseSalesOrderRepository.Add(salesOrder);
        }

        public Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate, DataTable dtBankTrans)
        {
            return _salesOrderRepository.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate,dtBankTrans);
        }
        public void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            _salesOrderRepository.AddReplacementOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }

        public void AddSOReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DataTable dtPurchaseOrder, DataTable dtPurchaseOrderDetail, DataTable dtPOProductDetail)
        {
            _salesOrderRepository.AddSOReplacementOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail);
        }

        public void SaveSalesOrder()
        {
            _unitOfWork.Commit();
        }

        public void Update(SOrder sorder)
        {
            _baseSalesOrderRepository.Update(sorder);
        }

        public SOrder GetSalesOrderById(int id)
        {
            //return _baseSalesOrderRepository.FindBy(x => x.SOrderID == id).First();
            return _baseSalesOrderRepository.AllIncluding(d => d.SOrderDetails).Where(x => x.SOrderID == id).First();
        }

        public void DeleteSalesOrder(int id)
        {
            _baseSalesOrderRepository.Delete(x => x.SOrderID == id);
        }

        public IEnumerable<SOredersReportModel> GetforSalesReport(DateTime fromDate, DateTime toDate, int EmployeeID, EnumCustomerType customerType)
        {
            return _baseSalesOrderRepository.GetforSalesReport(_customerRepository, _employeeRepository, fromDate, toDate, EmployeeID,customerType);

        }

        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>>
           GetforSalesDetailReport(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetforSalesDetailReport(_sOrderDetailRepository, _productRepository, _stockdetailRepository, fromDate, toDate);
        }

        public IEnumerable<SOredersReportModel> GetforSalesDetailReportByMO(DateTime fromDate, DateTime toDate, int MOId)
        {
            return _baseSalesOrderRepository.GetforSalesDetailReportByMO(_sOrderDetailRepository, _productRepository, _stockdetailRepository, _customerRepository, _employeeRepository, fromDate, toDate, MOId);
        }

        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            //commented to skip error
            return _baseSalesOrderRepository.GetSalesReportByConcernID(_customerRepository, _sOrderDetailRepository, fromDate, toDate, concernID, CustomerType);

        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string, 
                Tuple<int, decimal, int, string, int, int, string,
                    Tuple<decimal>>>>>
           GetSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, bool IsVATReport, int CustomerType)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByConcernID(_sOrderDetailRepository, _productRepository, _stockdetailRepository,
                _customerRepository, _CategoryRepository, _SisterConcernRepository, fromDate, toDate, concernID, IsVATReport,CustomerType);
        }

        public IEnumerable<SOredersReportModel> GetSalesDetailReportByCustomerID(DateTime fromDate, DateTime toDate, int customerID)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByCustomerID(_sOrderDetailRepository, _productRepository, _stockdetailRepository, _ColorRepository,
                _creditSalesRepository, _creditSalesDetailsRepository, fromDate, toDate, customerID);
        }

        public IEnumerable<SOredersReportModel> GetSalesDetailHistoryBySOrderID(int SOrderID)
        {
            return _baseSalesOrderRepository.GetSalesDetailHistoryBySOrderID(_sOrderDetailRepository, _productRepository, _stockdetailRepository, _ColorRepository,
                _transactionSOrderRepository, _transactionSOrderDetailRepository, _UserRepository, SOrderID);
        }

        public IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
        GetSalesDetailReportByMOID(DateTime fromDate, DateTime toDate, int concernId, int MOID, int RptType)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByMOID(_customerRepository, _employeeRepository, fromDate, toDate, concernId, MOID, RptType);
        }

        public IEnumerable<Tuple<string, string, string, string, string, decimal>>
        GetMOWiseCustomerDueRpt(int concernId, int MOID, int RptType)
        {
            return _baseSalesOrderRepository.GetMOWiseCustomerDueRpt(_customerRepository, _employeeRepository, concernId, MOID, RptType);
        }
        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetSalesByProductID(DateTime fromDate, DateTime toDate, int ConcernId, int productID)
        {
            return _baseSalesOrderRepository.GetSalesByProductID(_sOrderDetailRepository, _productRepository, _creditSalesRepository, _creditSalesDetailsRepository, fromDate, toDate, ConcernId, productID);
        }

        public bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, DataTable dtBankTrans)
        {
            return _salesOrderRepository.UpdateSalesOrderUsingSP(userId, salesOrderId, dtSalesOrder, dtSODetail,dtBankTrans);
        }

        public bool DeleteSalesOrderUsingSP(int orderId, int userId)
        {
           return _salesOrderRepository.DeleteSalesOrderUsingSP(orderId, userId);
        }

        public void DeleteSalesOrderDetailUsingSP(int orderId, int userId)
        {
            _salesOrderRepository.DeleteSalesOrderDetailUsingSP(orderId, userId);
        }

        public void CorrectionStockData(int ConcernId)
        {
            _salesOrderRepository.CorrectionStockData(ConcernId);
        }
        public List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID)
        {
            return _salesOrderRepository.SRWiseCustomerSalesSummary(fromdate, todate, ConcernID, EmployeeID);
        }

        public List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _salesOrderRepository.CustomerLedger(fromdate, todate, ConcernID, CustomerID);
        }
        public List<LedgerAccountReportModel> CustomerLedger(DateTime fromdate, DateTime todate, int CustomerID)
        {
            return _baseSalesOrderRepository.CustomerLedger(_sOrderDetailRepository, _customerRepository, _UserRepository, _BankTransactionRepository,
                _cashCollectionRepository, _creditSalesRepository, _creditSalesDetailsRepository, _CreditSalesScheduleRepository,
                _productRepository, _BankRepository, _RorderRepository, _ROrderDetailRepository, _hireSalesReturn, CustomerID, fromdate, todate);
        }

        public List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue)
        {
            return _salesOrderRepository.CustomerDue(fromdate, todate, ConcernID, CustomerID, IsOnlyDue);
        }


        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReplacementOrdersByAsync(int EmployeeID)
        {
            return await _baseSalesOrderRepository.GetReplacementOrdersAsync(_sOrderDetailRepository, _customerRepository, EmployeeID);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync()
        {
            return await _baseSalesOrderRepository.GetReturnOrdersAsync(_sOrderDetailRepository, _customerRepository);
        }

        public List<ReplaceOrderDetail> GetReplaceOrderInvoiceReportByID(int OrderID)
        {
            return _baseSalesOrderRepository.GetReplaceOrderInvoiceReportByID(_sOrderDetailRepository, _stockdetailRepository, _productRepository, OrderID);
        }


        public bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            return _salesOrderRepository.AddReturnOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }


        public List<ReplaceOrderDetail> GetReturnOrderInvoiceReportByID(int OrderID)
        {
            return _baseSalesOrderRepository.GetReturnOrderInvoiceReportByID(_sOrderDetailRepository, _stockdetailRepository, _productRepository, OrderID);
        }


        public List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.DailyWorkSheetReport(fromdate, todate, ConcernID);
        }


        public List<ReplacementReportModel> ReplacementOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ReplacementReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, _PorderRepository, _POrderDetailRepository, CustomerID, fromdate, todate);
        }


        public List<ReturnReportModel> ReturnOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            //return _baseSalesOrderRepository.ReturntReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, CustomerID, fromdate, todate);
            return _salesOrderRepository.ReturnReport(fromdate, todate, ConcernID, CustomerID);
        }


        public List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.MonthlyBenefitReport(fromdate, todate, ConcernID);
        }


        public List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.ProductWiseBenefitReport(fromdate, todate, ConcernID);
        }

        public List<ProductWiseSalesReportModel> ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseSalesReport(_sOrderDetailRepository, _customerRepository, _employeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }

        public List<ReplacementReportModel> DamageProductReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.DamageReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, CustomerID, fromdate, todate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseSalesDetailsReport(int CompanyID,
            int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType,
               int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseSalesDetailsReport(_sOrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository,
                _stockdetailRepository, _customerRepository, CompanyID, CategoryID, ProductID, fromDate,
                toDate, CustomerType,CustomerID);
        }


        public SOrder GetLastSalesOrderByCustomerID(int CustomerID)
        {
            return _baseSalesOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.Status == (int)EnumPurchaseType.Purchase).OrderByDescending(i => i.InvoiceDate).FirstOrDefault();
        }
        public decimal GetAllCollectionAmountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAllCollectionAmountByDateRange(_creditSalesRepository, _CreditSalesScheduleRepository, _cashCollectionRepository, _BankTransactionRepository, fromDate, toDate);
        }
        public decimal GetVoltageStabilizerCommission(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetVoltageStabilizerCommission(_sOrderDetailRepository, _creditSalesRepository, _creditSalesDetailsRepository, _productRepository, _ExtraCommissionSetupRepository, fromDate, toDate);
        }
        public decimal GetExtraCommission(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _baseSalesOrderRepository.GetExtraCommission(_sOrderDetailRepository, _creditSalesRepository, _creditSalesDetailsRepository, _productRepository, _ExtraCommissionSetupRepository, fromDate, toDate, ConcernID);
        }
        public bool IsIMEIAlreadyReplaced(int StockDetailID)
        {
            return _baseSalesOrderRepository.IsIMEIAlreadyReplaced(_sOrderDetailRepository, StockDetailID);
        }

        public List<SOredersReportModel> GetAdminSalesReport(int ConcernID,
            DateTime fromDate, DateTime toDate,
                        EnumCustomerType customerType, int customerID)
        {
            return _baseSalesOrderRepository.GetAdminSalesReport(_sOrderDetailRepository, _customerRepository,
                _SisterConcernRepository, ConcernID, fromDate, toDate,customerType,customerID);
        }

        public List<CustomerDueReport> CustomerDueReport(int CustomerID, DateTime fromDate,
            DateTime toDate, int ConcernID, EnumCustomerType CustomerType, int IsOnlyDue, bool IsAdminReport)
        {
            return _baseSalesOrderRepository.CustomerDueReport(_customerRepository, 
                _BankTransactionRepository, _cashCollectionRepository, 
                _creditSalesRepository, _CreditSalesScheduleRepository,_RorderRepository, _ROrderDetailRepository, _productRepository, _creditSalesDetailsRepository,
                _hireSalesReturn, CustomerID, fromDate, toDate, ConcernID, CustomerType, IsOnlyDue,IsAdminReport);
        }

        public List<SummaryReportModel> GetSummaryReport(DateTime Date, int ConcernID)
        {
            return _baseSalesOrderRepository.GetSummaryReport(_sOrderDetailRepository, _customerRepository, _BankTransactionRepository,
                _cashCollectionRepository, _creditSalesRepository, _creditSalesDetailsRepository, _CreditSalesScheduleRepository, _productRepository,
                _CategoryRepository, Date, ConcernID);
        }


        public HireAccountDetailsReportModel DealerAccountDetails(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _baseSalesOrderRepository.DealerAccountDetails(_customerRepository,
                _cashCollectionRepository, fromDate,toDate, ConcernID);
        }

        public bool UpdatePendingSalesUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail)
        {
            return _salesOrderRepository.UpdatePendingSalesUsingSP(userId, salesOrderId, dtSalesOrder, dtSODetail);
        }

        public Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate)
        {
            return _salesOrderRepository.AddPendingSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate);
        }

        public bool ApprovedSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, int orderId, DataTable dtBankTrans)
        {
            return _salesOrderRepository.ApprovedSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, orderId, dtBankTrans);
        }
        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, decimal, EnumSalesType, Tuple<string, string>>>>
        GetAllPendingSalesOrderAsync()
        {
            return await _baseSalesOrderRepository.GetAllPendingSalesOrderAsync(_customerRepository, _UserRepository);
        }

        public Tuple<bool, string> IsIMEIInPendingSales(int StockDetailID, int SOrderID)
        {
            SOrder PendingSOrder = null;
            if (SOrderID > 0)
            {
                PendingSOrder = (from so in _baseSalesOrderRepository.All
                                 join sod in _sOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                 where so.Status == (int)EnumSalesType.Pending && sod.SDetailID == StockDetailID
                                 && so.SOrderID != SOrderID
                                 select so).FirstOrDefault();
            }
            else
            {
                PendingSOrder = (from so in _baseSalesOrderRepository.All
                                 join sod in _sOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                 where so.Status == (int)EnumSalesType.Pending && sod.SDetailID == StockDetailID
                                 select so).FirstOrDefault();
            }

            if (PendingSOrder != null)
                return new Tuple<bool, string>(true, PendingSOrder.InvoiceNo);

            return new Tuple<bool, string>(false, string.Empty); ;
        }

        public IEnumerable<SOredersReportModel> GetforSalesReportForAll(DateTime fromDate, DateTime toDate, int EmployeeID, EnumCustomerType customerType)
        {
            return _baseSalesOrderRepository.GetforSalesReportForAll(_customerRepository, _employeeRepository, fromDate, toDate, EmployeeID, customerType);

        }
        public IEnumerable<ProductPickerInStockReportModel> ProductPickerInStock(int ConcernID)
        {
            return _salesOrderRepository.ProductPickerInStock(ConcernID);
        }
        public IEnumerable<ProductPickerInStockReportModel> DateWiseProductPickerInStock(int ConcernID)
        {
            return _salesOrderRepository.DateWiseProductPickerInStock(ConcernID);
        }

        public List<TOCategoryWiseCustomerDue> GetCustomerWiseTotalDue(int customerId, int concernId)
        {
            List<TOCategoryWiseCustomerDue> customerCateGoryDue = _customerRepository.ExecSP<TOCategoryWiseCustomerDue>("GetCategorywiseCustomerDue @concernId, @CustomerID, @isOnlyDue",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 }
                        ).ToList();

            string query = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

            List<TOCustomerInfoForDueReport> customerInfo = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();

            if (customerCateGoryDue.Any())
            {
                foreach (var item in customerCateGoryDue)
                {
                    TOCustomerInfoForDueReport customer = customerInfo.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                    item.CustomerAddress = customer.CustomerAddress;
                    item.CustomerType = customer.CustomerType;
                    item.Address = customer.Address;
                    item.CustomerName = customer.CustomerName;
                    item.CompanyName = customer.CompanyName;
                    item.ContactNo = customer.ContactNo;

            
                }
            }

            return customerCateGoryDue;
        }


        public List<TOCategoryWiseCustomerDue> GetCustomerWiseTotalDueByDate(int customerId, int concernId, DateTime asOnDate)
        {
            List<TOCategoryWiseCustomerDue> customerCateGoryDue = _customerRepository.ExecSP<TOCategoryWiseCustomerDue>("GetDatewiseCustomerDue @concernId, @CustomerID, @asOnDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate }
                        ).ToList();

            return customerCateGoryDue;
        }

        public bool IsAlreadySold(string imei, int concernId)
        {
            string query = string.Format(@"SELECT ISNULL(COUNT(SDetailID), 0)  FROM
                                        (
                                            SELECT TOP(1) st.* from StockDetails st
											JOIN Stocks stk ON st.StockID = stk.StockID
											WHERE stk.ConcernID = {0} AND st.IMENO = '{1}'
                                            ORDER BY st.SDetailID DESC
                                        ) t1
                                        where Status = 2", concernId, imei);
            int count = _baseSalesOrderRepository.SQLQuery<int>(query);
            return count > 0;
        }



        public bool IsSoReturn(int SoId)
        {
            if (_sOrderDetailRepository.All.Any(i => i.IsProductReturn == 1 && i.SOrderID == SoId))
                return true;

            return false;
        }

        public List<ProductWiseSalesReportModel> ProductWiseSalesBenefit(int CompanyID,
            int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.ProductWiseSalesBenefit(_sOrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository,
                _stockdetailRepository, _customerRepository, _RorderRepository, _ROrderDetailRepository, _creditSalesRepository, _creditSalesDetailsRepository, _CreditSalesScheduleRepository, CompanyID, CategoryID, ProductID, fromDate,
                toDate);
        }

    }
}
