using IMSWEB.Data;
using IMSWEB.Data.Repositories.StockRepository;
using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class StockService : IStockService
    {
        private readonly IBaseRepository<Stock> _stockRepository;
        private readonly IStockRepository _stockRepo;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _ColorRepository;
        private readonly IBaseRepository<Supplier> _SupplierRepository;
        private readonly IBaseRepository<PriceProtection> _PriceProtectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<SRVisit> _SRVisitRepository;
        private readonly IBaseRepository<SRVisitDetail> _SRVisitDetailRepository;
        private readonly IBaseRepository<SRVProductDetail> _SRVProductDetailRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<Godown> _GodownRepository;
        private readonly IBaseRepository<POrder> _POrderRepository;
        private readonly IBaseRepository<POrderDetail> _POrderDetailRepository;

        private readonly IBaseRepository<ROrder> _RorderRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<Transfer> _TransferRepository;
        private readonly IBaseRepository<TransferDetail> _TransferDetailRepository;

        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<CreditSale> _CreditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSaleDetailsRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<SaleOffer> _SaleOfferRepository;
        private readonly IBaseRepository<ParentCategory> _parentCategoryRepository;
        private readonly IBaseRepository<HireSalesReturnCustomerDueAdjustment> _hireSalesReturnRepository;

        public StockService(IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IUnitOfWork unitOfWork,
                            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository,
                            IBaseRepository<PriceProtection> priceProtectionRepository, IStockRepository stockRepo,
                            IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository,
                            IBaseRepository<SRVProductDetail> SRVProductDetailRepository, IBaseRepository<Employee> EmployeeRepository,
                            IBaseRepository<Godown> GodownRepository,IBaseRepository<POrderDetail> POrderDetailRepository,
                            IBaseRepository<ROrder> RorderRepository,IBaseRepository<ROrderDetail> rOrderDetail,
                            IBaseRepository<Transfer> TransferRepository,IBaseRepository<TransferDetail> TransferDetailRepository,
                            IBaseRepository<Category> CategoryRepository,IBaseRepository<Company> CompanyRepository,
                            IBaseRepository<POrder> POrderRepository,IBaseRepository<SOrder> SOrderRepository,
                            IBaseRepository<SOrderDetail> SOrderDetailRepository,IBaseRepository<CreditSale> CreditSaleRepository,
                            IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,IBaseRepository<SisterConcern> SisterConcernRepository,
                            IBaseRepository<SaleOffer> SaleOfferRepository,IBaseRepository<ParentCategory> parentCategoryRepository,
                            IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository
            )
        {
            _stockRepository = stockRepository;
            _stockDetailRepository = stockDetailRepository;
            _productRepository = productRepository;
            _ColorRepository = colorRepository;
            _unitOfWork = unitOfWork;
            _SupplierRepository = suppRepository;
            _PriceProtectionRepository = priceProtectionRepository;
            _stockRepo = stockRepo;
            _SRVisitRepository = SRVisitRepository;
            _SRVisitDetailRepository = SRVisitDetailRepository;
            _SRVProductDetailRepository = SRVProductDetailRepository;
            _EmployeeRepository = EmployeeRepository;
            _GodownRepository = GodownRepository;
            _POrderRepository = POrderRepository;
            _POrderDetailRepository = POrderDetailRepository;
            _CategoryRepository = CategoryRepository;
            _CompanyRepository = CompanyRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _CreditSaleRepository = CreditSaleRepository;
            _CreditSaleDetailsRepository = CreditSaleDetailsRepository;
            _RorderRepository = RorderRepository;
            _ROrderDetailRepository = rOrderDetail;
            _TransferRepository = TransferRepository;
            _TransferDetailRepository = TransferDetailRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _SaleOfferRepository = SaleOfferRepository;
            _parentCategoryRepository = parentCategoryRepository;
            _hireSalesReturnRepository = hireSalesReturnRepository;
        }

        public void AddStock(Stock Stock)
        {
            _stockRepository.Add(Stock);
        }

        public void SaveStock()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<Stock> GetAllStock()
        {
            return _stockRepository.All;
        }

        public Stock GetStockById(int id)
        {
            return _stockRepository.FindBy(x => x.StockID == id).First();
        }
        public Stock GetStockByProductIdandGodownID(int ProductID, int GodownID)
        {
            return _stockRepository.FindBy(x => x.ProductID == ProductID && x.GodownID == GodownID).FirstOrDefault();
        }

        public Stock GetStockByProductIdandColorIDandGodownID(int ProductID, int GodownID, int ColorID)
        {
            return _stockRepository.FindBy(x => x.ProductID == ProductID && x.GodownID == GodownID && x.ColorID == ColorID).FirstOrDefault();
        }
        public Stock GetStockByProductId(int id)
        {
            return _stockRepository.FindBy(x => x.ProductID == id).First();
        }
        public async Task<IEnumerable<Tuple<int, string, string, string, decimal,
            decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>>> GetAllStockAsync(int ConcernID, bool IsVATManager)
        {
            return await _stockRepository.GetAllStockAsync(_productRepository, _ColorRepository, _stockDetailRepository, _GodownRepository, _SisterConcernRepository, _CategoryRepository, _CompanyRepository, ConcernID, IsVATManager);
        }

        public async Task<IEnumerable<Tuple<int, string, string, string,
        string, string, string, Tuple<string>>>> GetAllStockDetailAsync(int ConcernID, bool IsVATManager)
        {
            return await _stockRepository.GetAllStockDetailAsync(_stockDetailRepository, _productRepository, _ColorRepository, _GodownRepository, _SisterConcernRepository, ConcernID, IsVATManager);
        }

        public IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
            GetforStockReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType)
        {//bd
            return _stockRepository.GetforReport(_productRepository, _ColorRepository, _stockDetailRepository, _GodownRepository, _SisterConcernRepository, _parentCategoryRepository,
                userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ColorID, PCategoryID, IsVATManager, StockType);
        }

        public IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> GetPriceProtectionReport(string userName, int concernID, DateTime dFDate, DateTime dTDate)
        {
            return _stockRepository.GetPriceProtectionReport(_productRepository, _ColorRepository, _SupplierRepository, _PriceProtectionRepository, userName, concernID, dFDate, dTDate);
        }

        public IEnumerable<Tuple<int, string, string>> GetStockDetailsByID(int StockId)
        {
            return _stockRepository.GetStockDetailsByID(_stockDetailRepository, StockId);
        }


        public void DeleteStock(int id)
        {
            _stockRepository.Delete(x => x.StockID == id);
        }
        public IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int concernID, int ProductID)
        {
            return _stockRepo.DailyStockVSSalesSummary(fromDate, toDate, concernID, ProductID);
        }
        public bool IsIMEIAvailableForSRVisit(int ProductID, int ColorID, string IMEI)
        {
            return _stockRepository.CheckStockIMEIForSRVisit(_stockDetailRepository, ProductID, ColorID, IMEI);
        }
        public string GetStockProductsHistory(int StockID)
        {
            return _stockRepository.GetStockProductsHistory(_stockDetailRepository, _SRVisitRepository, _SRVisitDetailRepository, _SRVProductDetailRepository, _EmployeeRepository, StockID);
        }

        public List<StockLedger> GetStockLedgerReport(int reportType, string CompanyName, string CategoryName, string ProductName, DateTime dFDate, DateTime dTDate, int ConcernID)
        {
            return _stockRepository.GetStockLedger(_stockDetailRepository, _POrderRepository,
                _POrderDetailRepository, _productRepository, _CategoryRepository, _CompanyRepository, _ColorRepository, _SOrderRepository, _SOrderDetailRepository, _CreditSaleRepository, _CreditSaleDetailsRepository,
                _TransferRepository, _TransferDetailRepository,_RorderRepository,
                _ROrderDetailRepository,_parentCategoryRepository, _hireSalesReturnRepository,
                reportType, CompanyName, CategoryName, ProductName, dFDate, dTDate, ConcernID);
        }
        public List<ProductDetailsModel> GetStockProductsBySupplier(int SupplierID)
        {
            return _stockRepository.GetStockProductsBySupplier(_stockDetailRepository, _productRepository, _ColorRepository, _SupplierRepository, _POrderRepository,
                _POrderDetailRepository, _CategoryRepository, _CompanyRepository, SupplierID);
        }
        public List<ProductDetailsModel> GetSupplierStockDetails(int SupplierID, int ProductID, int ColorID, int GodownID)
        {
            return _stockRepository.GetSupplierStockDetails(_stockDetailRepository, _POrderRepository,
                _POrderDetailRepository, _productRepository, SupplierID, ProductID, ColorID, GodownID);
        }

        public IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
     GetforAdminStockReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID)
        {
            return _stockRepository.GetforAdminStockReport(_productRepository, _ColorRepository, _stockDetailRepository,
                _SisterConcernRepository, _CompanyRepository, _CategoryRepository, _GodownRepository,
                userName, concernID, reportType, CompanyName, CategoryName, ProductName, UserConcernID);
        }


        public bool IsIMEIExistInGodown(int ConcernID, int GodownID, string IMEI)
        {
            return _stockRepository.IsIMEIExistInGodown(_stockDetailRepository, ConcernID, GodownID, IMEI);
        }
     
        public ProductDetailsModel GetStockIMEIDetail(string IMEI)
        {
            return _stockRepository.GetStockIMEIDetail(_stockDetailRepository, _productRepository, _CompanyRepository,
                _CategoryRepository, _GodownRepository, _ColorRepository, _SaleOfferRepository, IMEI,true);
        }
        public ProductDetailsModel GetIMEIDetail(string IMEI)
        {
            return _stockRepository.GetStockIMEIDetail(_stockDetailRepository, _productRepository, _CompanyRepository,
                _CategoryRepository, _GodownRepository, _ColorRepository, _SaleOfferRepository, IMEI,false);
        }

        public ProductDetailsModel GetIMEIDetails(string IMEI, bool isStockIMEI = false)
        {
            return _stockRepository.GetIMEIDetails(_stockDetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _GodownRepository, _ColorRepository, _SaleOfferRepository, IMEI, isStockIMEI);
        }
        public ProductDetailsModel GetStockIMEIDetailsByLastSomedigit(string IMEI)
        {
            return _stockRepository.GetStockIMEIDetailsByLastSomedigit(_stockDetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _GodownRepository, _ColorRepository, _SaleOfferRepository, IMEI);
        }
        public ProductDetailsModel GetSRVisitIMEIDetails(string IMEI, int EmployeeID)
        {
            return _stockRepository.GetSRVisitIMEIDetails(_stockDetailRepository, _productRepository, _CompanyRepository,
                _CategoryRepository, _GodownRepository, _ColorRepository, _SRVisitRepository, _SRVisitDetailRepository, _SRVProductDetailRepository, _SaleOfferRepository, IMEI, EmployeeID);
        }

        public IQueryable<ProductDetailsModel> GetStocksByProductId(int ProductID)
        {
            return _stockRepository.GetStocksByProductId(_stockDetailRepository,
                _productRepository, _CompanyRepository, _CategoryRepository,
                _GodownRepository, _ColorRepository, ProductID);
        }

        public void SaveStockValue(int ConcernID)
        {
            _stockRepo.SaveStockValue(ConcernID);
        }

        public IQueryable<ProductDetailsModel> GetStockDetails()
        {
            return _stockRepository.GetStockDetails(_stockDetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _GodownRepository, _ColorRepository);
        }

        public IQueryable<ProductDetailsModel> GetStocs()
        {
            return _stockRepository.GetStocks(_stockDetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _GodownRepository, _ColorRepository);
        }



        public IEnumerable<StockForcastingReportModel> StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, int ProductID)
        {

            return _stockRepo.StockForcastingReportProductWise(fromDate, toDate, ProductID);

        }
        public IEnumerable<StockReportWithDateReportModel> StockReportWithDate(int ConcernID, int ProductID, int CompanyID, int CategoryID)
        {

            return _stockRepo.StockReportWithDate(ConcernID, ProductID, CompanyID, CategoryID);

        }

        public IEnumerable<StockForcastingReportModel> StockForcastingReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {

            return _stockRepo.StockForcastingReport(fromDate, toDate, ConcernID);

        }

        public IQueryable<Stock> GetAll()
        {
            return _stockRepository.All;
        }

        public List<ProductDetailsModel> GetDamageStockProductsBySupplier(int SupplierID)
        {
            return _stockRepository.GetDamageStockProductsBySupplier(_stockDetailRepository, _productRepository, _ColorRepository, _SupplierRepository, _POrderRepository,
                _POrderDetailRepository, _CategoryRepository, _CompanyRepository, SupplierID);
        }

        public ProductDetailsModel GetDamageStockIMEIDetail(string IMEI)
        {
            return _stockRepository.GetDamageStockIMEIDetail(_stockDetailRepository, _productRepository, _CompanyRepository,
                _CategoryRepository, _GodownRepository, _ColorRepository, _SaleOfferRepository, IMEI, true);
        }

        public IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
        GetforAdminProductStockReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID)
        {
            return _stockRepository.GetforAdminProductStockReport(_productRepository, _ColorRepository, _stockDetailRepository,
                _SisterConcernRepository, _CompanyRepository, _CategoryRepository, _GodownRepository,
                userName, concernID, reportType, CompanyName, CategoryName, ProductName, UserConcernID);
        }


        public List<StockLedger> GetRateWiseStockLedgerReport(int reportType, string CompanyName, string CategoryName, string ProductName, DateTime dFDate, DateTime dTDate, int ConcernID)
        {
            return _stockRepository.GetRateWiseStockLedger(_stockDetailRepository, _POrderRepository,
                _POrderDetailRepository, _productRepository, _CategoryRepository, _CompanyRepository, _ColorRepository, _SOrderRepository, _SOrderDetailRepository, _CreditSaleRepository, _CreditSaleDetailsRepository,
                _TransferRepository, _TransferDetailRepository, _RorderRepository,
                _ROrderDetailRepository, _parentCategoryRepository, _hireSalesReturnRepository,
                reportType, CompanyName, CategoryName, ProductName, dFDate, dTDate, ConcernID);
        }

        public List<ProductDetailsModel> GetSupplierDamageStockDetails(int SupplierID, int ProductID, int ColorID, int GodownID)
        {
            return _stockRepository.GetSupplierDamageStockDetails(_stockDetailRepository, _POrderRepository,
                _POrderDetailRepository, _productRepository, SupplierID, ProductID, ColorID, GodownID);
        }

        public IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
            GetforStockReportZeroQty(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType)
        {
            return _stockRepository.GetforStockReportZeroQty(_productRepository, _ColorRepository, _stockDetailRepository, _GodownRepository, _SisterConcernRepository, _parentCategoryRepository,
                userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ColorID, PCategoryID, IsVATManager, StockType);
        }


        public IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, decimal>>>
          GetforStockReportNew(string userName, int concernID, int reportType, List<int> CompanyIds, List<int> CategoriesList, List<int> ProductIds, List<int> GodownIds, List<int> ColorIds, bool IsVATManager)
        {
            return _stockRepository.GetforReportNew(_productRepository, _ColorRepository, _stockDetailRepository, _GodownRepository, _SisterConcernRepository,
                userName, concernID, reportType, CompanyIds, CategoriesList, ProductIds, GodownIds, ColorIds, IsVATManager);
        }

    }
}
