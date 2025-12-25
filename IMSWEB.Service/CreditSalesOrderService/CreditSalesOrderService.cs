using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CreditSalesOrderService : ICreditSalesOrderService
    {
        private readonly IBaseRepository<CreditSale> _baseSalesOrderRepository;
        private readonly ICreditSalesOrderRepository _CreditSOrderRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSOrderDetailsRepository;
        private readonly IBaseRepository<CreditSalesSchedule> _CreditSalesScheduleRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<ApplicationUser> _UserRepository;
        private readonly IBaseRepository<CashCollection> _cas;
        private readonly IBaseRepository<HireSalesReturnCustomerDueAdjustment> _hireSalesReturnRepository;
        private readonly IBaseRepository<CreditInterestHistory> _creditInterestHistoryRepository;


        public CreditSalesOrderService(IBaseRepository<CreditSale> baseSalesOrderRepository,
            ICreditSalesOrderRepository salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<CreditSaleDetails> saleOrderDetailsRepository,
            IBaseRepository<CreditSalesSchedule> saleOrderSchedulesRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Employee> EmployeeRepository,
            IBaseRepository<StockDetail> stockDetailRepository, IUnitOfWork unitOfWork,
            IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<ApplicationUser> UserRepository, IBaseRepository<CashCollection> Cas, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository, IBaseRepository<CreditInterestHistory> creditInterestHistoryRepository)
        {
            _baseSalesOrderRepository = baseSalesOrderRepository;
            _CreditSOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _CreditSOrderDetailsRepository = saleOrderDetailsRepository;
            _CreditSalesScheduleRepository = saleOrderSchedulesRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _stockDetailRepository = stockDetailRepository;
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _UserRepository = UserRepository;
            _cas = Cas;
            _hireSalesReturnRepository = hireSalesReturnRepository;
            _creditInterestHistoryRepository = creditInterestHistoryRepository;
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string, int>>>>
            GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID,
            string InvoiceNo,string ContactNo,string CustomerName,string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsync(_customerRepository, _SisterConcernRepository,
                fromDate, toDate, IsVATManager, concernID,InvoiceNo, ContactNo, CustomerName, AccountNo);
        }

        public void AddSalesOrder(CreditSale salesOrder)
        {
            _baseSalesOrderRepository.Add(salesOrder);
        }
        public IQueryable<CreditSale> GetAllIQueryable()
        {
            return _baseSalesOrderRepository.All;
        }
        public Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules, DataTable dtBankTrans)
        {
           return _CreditSOrderRepository.AddSalesOrderUsingSP(dtSalesOrder, dtSODetail, dtSchedules, dtBankTrans);
        }

        public bool InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID)
        {
            return _CreditSOrderRepository.InstallmentPaymentUsingSP(orderId, installmentAmount, dtSchedules, LastPayAdjustment, dtBankTrans, CardTypeSetupID);
        }

        public void SaveSalesOrder()
        {
            _unitOfWork.Commit();
        }

        public void UpdateSalesOrder(CreditSale creditSale)
        {
            _baseSalesOrderRepository.Update(creditSale);
        }

        //public CreditSale GetSalesOrderByInvoiceNo(string invoiceNo,int concernID)
        //{
        //    return _baseSalesOrderRepository.FindBy(x => x.InvoiceNo == invoiceNo && x.ConcernID==concernID).First();
        //}

        public CreditSale GetSalesOrderById(int id)
        {
            return _baseSalesOrderRepository.FindBy(x => x.CreditSalesID == id).First();
        }

        public bool HasPaidInstallment(int id)
        {
            return _CreditSalesScheduleRepository.FindBy(x => x.CreditSalesID == id).Any(x => x.PaymentStatus.Equals("Paid"));
        }

        public IEnumerable<CreditSaleDetails> GetSalesOrderDetails(int id)
        {
            return _CreditSOrderDetailsRepository.FindBy(x => x.CreditSalesID == id);
        }

        public IEnumerable<Tuple<int, int, int, int,decimal, decimal, decimal, Tuple<decimal,string, string, int, string, decimal>>> GetCustomSalesOrderDetails(int id)
        {
            return _CreditSOrderDetailsRepository.GetCustomSalesOrderDetails(id, _productRepository,
                _colorRepository, _stockDetailRepository);
        }

        public IEnumerable<CreditSalesSchedule> GetSalesOrderSchedules(int id)
        {
            return _CreditSalesScheduleRepository.FindBy(x => x.CreditSalesID == id);
        }

        public bool ReturnSalesOrderUsingSP(int orderId, int userId)
        {
            return _CreditSOrderRepository.ReturnSalesOrderUsingSP(orderId, userId);
        }

        public void DeleteSalesOrder(int id)
        {
            _baseSalesOrderRepository.Delete(x => x.CreditSalesID == id);
        }

        public IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(DateTime fromDate, DateTime toDate, EnumCustomerType customerType, int EmployeeID)
        {
            return _baseSalesOrderRepository.GetUpcomingSchedule(_customerRepository, _CreditSalesScheduleRepository,
                _productRepository, _CreditSOrderDetailsRepository, _SOrderRepository, 
                _SOrderDetailRepository, _EmployeeRepository, _cas,fromDate, toDate,customerType,EmployeeID);
        }
        public IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, int concernID,int EmployeeID,bool IsAdminReport)
        {
            return _baseSalesOrderRepository.GetScheduleCollection(_customerRepository, 
                _CreditSalesScheduleRepository, _EmployeeRepository,_SisterConcernRepository,
                fromDate, toDate, concernID,EmployeeID, IsAdminReport);
        }
        public IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetCreditCollectionReport(DateTime fromDate, DateTime toDate, int concernID, int CustomerID)
        {
            return _baseSalesOrderRepository.GetCreditCollectionReport(_customerRepository, _CreditSalesScheduleRepository, fromDate, toDate, concernID, CustomerID);
        }

        public IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(DateTime date, int concernID)
        {
            return _baseSalesOrderRepository.GetDefaultingCustomer(_customerRepository, _CreditSalesScheduleRepository, date, concernID);
        }
        public IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>> GetDefaultingCustomer(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetDefaultingCustomer(_customerRepository, _CreditSalesScheduleRepository, fromDate, toDate, concernID);
        }

        //public void CalculatePenaltySchedules(int ConcernID)
        //{
        //    _CreditSOrderRepository.CalculatePenaltySchedules(ConcernID);
        //}

        public void CorrectionStockData(int ConcernId)
        {
            _CreditSOrderRepository.CorrectionStockData(ConcernId);
        }



        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, int, string, Tuple<string, string>>>>
            GetCreditSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            return _baseSalesOrderRepository.GetCreditSalesReportByConcernID(_customerRepository, _CreditSOrderDetailsRepository, fromDate, toDate, concernID, CustomerType);
        }


        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, int, string, string, int, int, string>>>>
            GetCreditSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, bool IsAdminReport)
        {
            return _baseSalesOrderRepository.GetCreditSalesDetailReportByConcernID(_CreditSOrderDetailsRepository, _productRepository,
                _stockDetailRepository, _customerRepository, _CategoryRepository, _SisterConcernRepository, fromDate, toDate, concernID, IsAdminReport);
        }
        public decimal GetDefaultAmount(int CreditSaleID, DateTime FromDate)
        {
            return _baseSalesOrderRepository.GetDefaultAmount(_CreditSalesScheduleRepository, CreditSaleID, FromDate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseCreditSalesReport(_CreditSOrderDetailsRepository, _customerRepository, _EmployeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, 
            DateTime fromDate, DateTime toDate, int CustomerType, int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseCreditSalesDetailsReport(
                _CreditSOrderDetailsRepository, _CompanyRepository, _CategoryRepository, _productRepository,
                _stockDetailRepository, _customerRepository, CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType,CustomerID);
        }



        public void DeleteSchedule(CreditSalesSchedule CreditSalesSchedule)
        {
            _CreditSalesScheduleRepository.Delete(CreditSalesSchedule);
        }
        public void AddSchedule(CreditSalesSchedule CreditSalesSchedule)
        {
            _CreditSalesScheduleRepository.Add(CreditSalesSchedule);
        }
        public void UpdateSchedule(CreditSalesSchedule scheduel)
        {
            _CreditSalesScheduleRepository.Update(scheduel);
        }
        public List<SOredersReportModel> SRWiseCreditSalesReport(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.SRWiseCreditSalesReport(_CreditSalesScheduleRepository, _customerRepository, _EmployeeRepository, EmployeeID, fromDate, toDate);
        }
        public IQueryable<SOredersReportModel> GetAdminCrSalesReport(int ConcernID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAdminCrSalesReport(_customerRepository, _SisterConcernRepository, ConcernID, fromDate, toDate);
        }
        public IQueryable<CashCollectionReportModel> AdminInstallmentColllections(int ConcernID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.AdminInstallmentColllections(_customerRepository, _SisterConcernRepository, _CreditSalesScheduleRepository, ConcernID, fromDate, toDate);
        }
        public HireAccountDetailsReportModel HireAccountDetails(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _baseSalesOrderRepository.HireAccountDetails(_customerRepository, _SisterConcernRepository, _CreditSalesScheduleRepository, _CreditSOrderDetailsRepository, _productRepository, ConcernID, fromDate, toDate);
        }

        public CreditSalesSchedule GetScheduleByScheduleID(int ScheduleID)
        {
            return _CreditSalesScheduleRepository.All.FirstOrDefault(i => i.CSScheduleID == ScheduleID);
        }

        public Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal,
            EnumSalesType, Tuple<string, string>>>>
            GetAllPendingSalesOrderAsync()
        {
            return _baseSalesOrderRepository.GetAllPendingSalesOrderAsync(_customerRepository, _UserRepository);
        }

        public IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, string Status)
        {
            return _baseSalesOrderRepository.GetScheduleCollection(_customerRepository, _CreditSalesScheduleRepository, _EmployeeRepository, fromDate, toDate, Status);
        }

        public bool PendingInstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment, int CardTypeSetupID)
        {
            return _CreditSOrderRepository.PendingInstallmentPaymentUsingSP(orderId, installmentAmount, dtSchedules, LastPayAdjustment, CardTypeSetupID);
        }

        public Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
      DataTable dtSchedules)
        {
            return _CreditSOrderRepository.AddPendingSalesOrderUsingSP(dtSalesOrder, dtSODetail, dtSchedules);
        }

        public bool ApprovedSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
       DataTable dtSchedules, DataTable dtBankTrans, int OrderID)
        {
            return _CreditSOrderRepository.ApprovedSalesOrderUsingSP(dtSalesOrder, dtSODetail, dtSchedules, dtBankTrans, OrderID);
        }

        public bool InstallmentApprovedSP(int orderId, decimal installmentAmount, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID, int ScheduleID)
        {
            return _CreditSOrderRepository.InstallmentApprovedSP(orderId, installmentAmount, LastPayAdjustment,
                dtBankTrans, CardTypeSetupID, ScheduleID);
        }

        public CreditSalesSchedule GetScheduleByID(int ScheduleID)
        {
            return _CreditSalesScheduleRepository.All.First(i => i.CSScheduleID == ScheduleID);
        }

        public Tuple<bool, string> IsIMEIInPendingSales(int StockDetailID, int SOrderID)
        {
            CreditSale PendingSOrder = null;
            if (SOrderID > 0)
            {
                PendingSOrder = (from cso in _baseSalesOrderRepository.All
                                 join csod in _CreditSOrderDetailsRepository.All on cso.CreditSalesID equals csod.CreditSalesID
                                 where cso.Status == (int)EnumSalesType.Pending && csod.CreditSaleDetailsID == StockDetailID
                                 && cso.CreditSalesID != SOrderID
                                 select cso).FirstOrDefault();
            }
            else
            {
                PendingSOrder = (from cso in _baseSalesOrderRepository.All
                                 join csod in _CreditSOrderDetailsRepository.All on cso.CreditSalesID equals csod.CreditSalesID
                                 where cso.Status == (int)EnumSalesType.Pending && csod.CreditSaleDetailsID == StockDetailID
                                 select cso).FirstOrDefault();
            }

            if (PendingSOrder != null)
                return new Tuple<bool, string>(true, PendingSOrder.InvoiceNo);

            return new Tuple<bool, string>(false, string.Empty); ;
        }




        public ProductDetailsModel GetLastSalesOrderByCustomerID(int CustomerID)
        {
            List<ProductDetailsModel> SalesProducts = new List<ProductDetailsModel>();
            List<ProductDetailsModel> Collections = new List<ProductDetailsModel>();
            //List<CashCollection> InterstAMT = new List<CashCollection>;

            //IQueryable<CashCollection> CashCollections = null;
            //IQueryable<Customer> Customers = null;
            //CashCollection InterstAMT = null;


            ProductDetailsModel productDetailsModel = null;
            var Credit = (from so in _baseSalesOrderRepository.All
                          join sod in _CreditSOrderDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                          join std in _stockDetailRepository.All on sod.StockDetailID equals std.SDetailID
                          join p in _productRepository.All on std.ProductID equals p.ProductID
                          where so.CustomerID == CustomerID && so.IsStatus == EnumSalesType.Sales
                          select new ProductDetailsModel
                          {
                              Date = so.SalesDate,
                              IMENo = std.IMENO,
                              ProductName = p.ProductName,
                              RecAmount = so.DownPayment,
                              InterestAmout = 0m
                             
                          }).OrderByDescending(i => i.Date).FirstOrDefault();
            if (Credit != null)
                SalesProducts.Add(Credit);
            var retail = (from so in _SOrderRepository.All
                          join sod in _SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                          join std in _stockDetailRepository.All on sod.SDetailID equals std.SDetailID
                          join p in _productRepository.All on std.ProductID equals p.ProductID
                          //biplob
                          join c in _customerRepository.All on so.CustomerID equals c.CustomerID
                          //join cc in _cas.All on c.CustomerID equals cc.CustomerID


                          where so.CustomerID == CustomerID && so.Status == (int)EnumSalesType.Sales
                          select new ProductDetailsModel
                          {
                              Date = so.InvoiceDate,
                              IMENo = std.IMENO,
                              ProductName = p.ProductName,
                              RecAmount = (decimal)so.RecAmount,
                              InterestAmout = 0m,

               
                          }).OrderByDescending(i => i.Date).FirstOrDefault();
            if (retail != null)
                SalesProducts.Add(retail);

            var Install = (from so in _baseSalesOrderRepository.All
                           join sod in _CreditSOrderDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                           join sodi in _CreditSalesScheduleRepository.All on so.CreditSalesID equals sodi.CreditSalesID
                           join std in _stockDetailRepository.All on sod.StockDetailID equals std.SDetailID
                           join p in _productRepository.All on std.ProductID equals p.ProductID
                           where so.CustomerID == CustomerID && so.IsStatus == EnumSalesType.Sales
                           && sodi.PaymentStatus == "Paid"
                           select new ProductDetailsModel
                           {
                               Date = sodi.PaymentDate,
                               IMENo = std.IMENO,
                               ProductName = p.ProductName,
                               RecAmount = (decimal)sodi.InstallmentAmt,
                               InterestAmout = (decimal)sodi.InterestAmount
                           }).OrderByDescending(i => i.Date).FirstOrDefault();
            if (Install != null)
                Collections.Add(Install);


            var cashcollection = (from cc in _cas.All
                                  where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.FromCustomer
                                  select new ProductDetailsModel
                                  {
                                      Date = (DateTime)cc.EntryDate,
                                      RecAmount = (decimal)cc.Amount,
                                      InterestAmout= (decimal)cc.InterestAmt,

                                  }).OrderByDescending(i => i.Date).FirstOrDefault();
            if (cashcollection != null)
                Collections.Add(cashcollection);

            if (SalesProducts.Count() > 0)
                productDetailsModel = SalesProducts.OrderByDescending(i => i.Date).FirstOrDefault();

            ProductDetailsModel collection = null;
            if (Collections.Count() > 0)
                collection = Collections.OrderByDescending(i => i.Date).FirstOrDefault();

            ProductDetailsModel result = new ProductDetailsModel();
            result.Date = productDetailsModel != null ? productDetailsModel.Date : DateTime.MinValue;
            result.RecAmount = productDetailsModel != null ? productDetailsModel.RecAmount : 0;
            result.ProductName = productDetailsModel != null ? productDetailsModel.ProductName : "";
           
            result.IMENo = productDetailsModel != null ? productDetailsModel.IMENo : "";
         

            result.CollectionDate = collection != null ? collection.Date : DateTime.MinValue;
            if (collection != null)
                result.RecAmount = collection.RecAmount;

            return result;
        }


        public IEnumerable<AdjustmentReportModel> GetAdjustmentReport(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAdjustmentReport(_CreditSalesScheduleRepository, _customerRepository, fromDate, toDate);
        }

        public IEnumerable<UpcommingScheduleReport> GetLastPayAdjAmt(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetLastPayAdjAmt(_customerRepository, _CreditSalesScheduleRepository, _productRepository, _CreditSOrderDetailsRepository, fromDate, toDate, concernID);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>> GetAllSalesReturnOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesReturnOrderAsync(_customerRepository, _SisterConcernRepository,
                fromDate, toDate, IsVATManager, concernID, InvoiceNo, ContactNo, CustomerName, AccountNo);
        }
        public IEnumerable<Tuple<DateTime, string, string, string, decimal, string, decimal, Tuple<string, string, decimal>>>
        GetHireReturnDetailReportByReturnID(int ReturnID, int concernID)
        {
            return _baseSalesOrderRepository.GetHireReturnDetailReportByReturnID(_CreditSOrderDetailsRepository, _productRepository, _hireSalesReturnRepository, _stockDetailRepository, ReturnID, concernID);
        }

        public HireSalesReturnCustomerDueAdjustment GetHireSalesReturnOrderById(int id)
        {
            return _hireSalesReturnRepository.FindBy(x => x.CreditSale.CreditSalesID == id).First();
        }

        public void AddInterestHistory(CreditInterestHistory history)
        {
            _creditInterestHistoryRepository.Add(history);
        }

        public decimal GetTotalPrevInterest(int creditSaleId)
        {
            List<CreditInterestHistory> histories = _creditInterestHistoryRepository.FindBy(d => d.HireSaleId == creditSaleId).ToList();
            return histories.Any() ? histories.Sum(d => d.InterestAmount) : 0m;
        }

    }
}
