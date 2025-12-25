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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IBaseRepository<POrder> _basePurchaseOrderRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IBaseRepository<POProductDetail> _pOProductetailRepository;
        private readonly IBaseRepository<POrderDetail> _pOrderDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<Stock> _StockRepository;
        private readonly IBaseRepository<StockDetail> _StockDetailRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<CreditSale> _CreditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSaleDetailsRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<SRVisit> _SRVisitRepository;
        private readonly IBaseRepository<SRVisitDetail> _SRVisitDetailRepository;
        private readonly IBaseRepository<SRVProductDetail> _SRVProductDetailRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<BankTransaction> _BankTransactionRepository;
        private readonly IBaseRepository<ApplicationUser> _UserRepository;
        private readonly IBaseRepository<Bank> _BankRepository;
        private readonly IBaseRepository<CashCollection> _cashCollectionRepository;
        private readonly IBaseRepository<Transfer> _TransferRepository;
        private readonly IBaseRepository<TransferDetail> _TransferDetailRepository;
        private readonly IBaseRepository<Godown> _GodownRepository;
        private readonly IBaseRepository<ROrder> _ROrderRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<TransactionPOrder> _transPOrderRepository;
        private readonly IBaseRepository<TransactionPOrderDetail> _transPOrderDetailRepository;

        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderService(IBaseRepository<POrder> basePurchaseOrderRepository,
            IPurchaseOrderRepository purchaseOrderRepository, IBaseRepository<POProductDetail> pOProductetailRepository,
            IBaseRepository<Supplier> supplierRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<POrderDetail> pOrderDetailRepository,
            IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            IBaseRepository<Stock> StockRepository, IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
            IUnitOfWork unitOfWork, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> CategoryRepository, IBaseRepository<Employee> EmployeeRepository,
            IBaseRepository<SRVisit> SRVisitRepository,
            IBaseRepository<SRVisitDetail> SRVisitDetailRepository,
            IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
             IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<ApplicationUser> UserRepository,
            IBaseRepository<Bank> BankRepository, IBaseRepository<CashCollection> cashCollectionRepository,
            IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
            IBaseRepository<Godown> GodownRepository, IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ROrderDetail> ROrderDetailRepository,
            IBaseRepository<TransactionPOrder> transPOrderRepository, IBaseRepository<TransactionPOrderDetail> transPOrderDetailRepository
            )
        {
            _basePurchaseOrderRepository = basePurchaseOrderRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _pOProductetailRepository = pOProductetailRepository;
            _colorRepository = colorRepository;
            _pOrderDetailRepository = pOrderDetailRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _StockRepository = StockRepository;
            _StockDetailRepository = StockDetailRepository;
            _CustomerRepository = CustomerRepository;
            _CreditSaleRepository = CreditSaleRepository;
            _CreditSaleDetailsRepository = CreditSaleDetailsRepository;
            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;
            _EmployeeRepository = EmployeeRepository;
            _SRVisitRepository = SRVisitRepository;
            _SRVisitDetailRepository = SRVisitDetailRepository;
            _SRVProductDetailRepository = SRVProductDetailRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _BankTransactionRepository = BankTransactionRepository;
            _UserRepository = UserRepository;
            _BankRepository = BankRepository;
            _cashCollectionRepository = cashCollectionRepository;
            _TransferRepository = TransferRepository;
            _TransferDetailRepository = TransferDetailRepository;
            _GodownRepository = GodownRepository;
            _ROrderRepository = ROrderRepository;
            _ROrderDetailRepository = ROrderDetailRepository;
            _transPOrderRepository = transPOrderRepository;
            _transPOrderDetailRepository = transPOrderDetailRepository;
        }
        public IQueryable<POrder> GetAllIQueryable()
        {
            return _basePurchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.Purchase);
        }
        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string, EnumPurchaseType, Tuple<int>>>> GetAllPurchaseOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID)
        {
            return await _basePurchaseOrderRepository.GetAllPurchaseOrderAsync(_supplierRepository, _SisterConcernRepository, fromDate, toDate, IsVATManager, concernID);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
          string, string, EnumPurchaseType>>> GetAllDeliveryOrderAsync()
        {
            return await _basePurchaseOrderRepository.GetAllDeliveryOrderAsync(_supplierRepository);
        }
        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllDamageReturnOrderAsync()
        {
            return await _basePurchaseOrderRepository.GetAllDamageReturnOrderAsync(_supplierRepository);
        }
        //public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync()
        //{
        //    return await _basePurchaseOrderRepository.GetAllReturnPurchaseOrderAsync(_supplierRepository);
        //}

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync(DateTime fromDate, DateTime toDate)
        {
            return await _basePurchaseOrderRepository.GetAllReturnPurchaseOrderAsync(_supplierRepository, fromDate, toDate);
        }

        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, 
            Tuple<decimal, decimal, string, string>>>
            GetPurchaseReport(DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, bool IsAdminReport, int concernID)
        {
            return _basePurchaseOrderRepository.GetPurchaseReport(_supplierRepository, _SisterConcernRepository,
                fromDate, toDate, PurchaseType, IsAdminReport, concernID);
        }
        public IEnumerable<ProductWisePurchaseModel> GetPurchaseDetailReportByConcernID(DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, bool IsVATManager, int concernID)
        {
            return _basePurchaseOrderRepository.GetPurchaseDetailReportByConcernID(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, _colorRepository, _SisterConcernRepository, fromDate, toDate, PurchaseType, IsVATManager, concernID);
        }

        //public IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string>>>
        //  GetPurchaseDetailReportBySupplierID(DateTime fromDate, DateTime toDate, int concernID, int supplierId)
        //{
        //    return _basePurchaseOrderRepository.GetPurchaseDetailReportBySupplierID(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, _colorRepository, fromDate, toDate, concernID,supplierId);
        //}

        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>>
        GetPurchaseDetailReportBySupplierID(DateTime fromDate, DateTime toDate, int ConcernId, int SupplierID)
        {
            return _basePurchaseOrderRepository.GetPurchaseDetailReportBySupplierID(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, fromDate, toDate, ConcernId, SupplierID);
        }

        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetPurchaseByProductID(DateTime fromDate, DateTime toDate, int ConcernId, int productID)
        {
            return _basePurchaseOrderRepository.GetPurchaseByProductID(_pOrderDetailRepository, _productRepository, fromDate, toDate, ConcernId, productID);
        }

        public void AddPurchaseOrder(POrder purchaseOrder)
        {
            _basePurchaseOrderRepository.Add(purchaseOrder);
        }

        public Tuple<bool, int> AddPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.AddPurchaseOrderUsingSP(dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }
        public Tuple<bool, int> AddReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
         DataTable dtPOProductDetail)
        {
            return _purchaseOrderRepository.AddReturnPurchaseOrderUsingSP(dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail);
        }

        public bool UpdatePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
             DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.UpdatePurchaseOrderUsingSP(purchaseOrderId, dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }
        public bool UpdateDeliveryOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.UpdatePurchaseOrderUsingSP(purchaseOrderId, dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }
        public void DeletePurchaseOrderDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail)
        {
            _purchaseOrderRepository.DeletePurchaseOrderDetailUsingSP(supplierId, porderDetailId, productId,
                colorId, userId, quantity, totalDue, dtPOProductDetail);
        }

        public int CheckProductStatusByPOId(int id)
        {
            return _purchaseOrderRepository.CheckProductStatusByPOId(id);
        }

        public int CheckTransferProductStatusByPOId(int id)
        {
            return _purchaseOrderRepository.CheckTransferProductStatusByPOId(id);
        }

        public int CheckProductStatusByPODetailId(int id)
        {
            return _purchaseOrderRepository.CheckProductStatusByPODetailId(id);
        }

        public int CheckIMENoDuplicacyByConcernId(int concernId, string imeNo)
        {
            return _purchaseOrderRepository.CheckIMENoDuplicacyByConcernId(concernId, imeNo);
        }

        public void SavePurchaseOrder()
        {
            _unitOfWork.Commit();
        }

        public void Update(POrder Porder)
        {
            _basePurchaseOrderRepository.Update(Porder);
        }
        public POrder GetPurchaseOrderById(int id)
        {
            return _basePurchaseOrderRepository.FindBy(x => x.POrderID == id).First();
        }

        public bool DeletePurchaseOrderUsingSP(int id, int userId)
        {
            return _purchaseOrderRepository.DeletePurchaseOrderUsingSP(id, userId);
        }


        public AdvanceSearchModel AdvanceSearchByIMEI(int ConcernID, string IMEINO)
        {
            return _basePurchaseOrderRepository.AdvanceSearchByIMEI(_pOrderDetailRepository, _pOProductetailRepository, _productRepository, _SOrderRepository, _SOrderDetailRepository, _StockDetailRepository, _StockRepository, _supplierRepository,
                _CustomerRepository, _CreditSaleRepository, _CreditSaleDetailsRepository, _TransferRepository, _TransferDetailRepository, _SisterConcernRepository, _ROrderRepository, _ROrderDetailRepository,_GodownRepository, ConcernID, IMEINO);
        }

        public AdvanceSearchModel AdvanceSearchByIMEINew(int ConcernID, string IMEINO)
        {
            return _basePurchaseOrderRepository.AdvanceSearchByIMEINew(ConcernID, IMEINO);
        }



        public List<ProductWisePurchaseModel> ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int concernID, int supplierID, EnumPurchaseType PurchaseType)
        {
            return _basePurchaseOrderRepository.ProductWisePurchaseReport(_pOrderDetailRepository, _productRepository, _supplierRepository, concernID, supplierID, fromDate, toDate, PurchaseType);
        }

        public List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReport(int CompanyID, int CategoryID, 
            int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType,
            bool IsAdminReport, int SelectedConcernID, int SupplierID)
        {
            return _basePurchaseOrderRepository.ProductWisePurchaseDetailsReport(_pOrderDetailRepository,
                _productRepository, _CompanyRepository, _CategoryRepository, _SisterConcernRepository, _supplierRepository,
                CompanyID, CategoryID, ProductID, fromDate, toDate, PurchaseType,IsAdminReport,SelectedConcernID, SupplierID);
        }
        public List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReportNew(int CompanyID, int CategoryID,
        int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, int SupplierID)
        {
            return _basePurchaseOrderRepository.ProductWisePurchaseDetailsReportNew(_pOrderDetailRepository,
                _productRepository, _CompanyRepository, _CategoryRepository, _supplierRepository,
                CompanyID, CategoryID, ProductID, fromDate, toDate, PurchaseType, SupplierID);
        }

        public AdvanceSearchModel SRVisitAdvanceSearchByIMEI(int ConcernID, string IMEINO)
        {
            return _basePurchaseOrderRepository.SRVisitAdvanceSearchByIMEI(_pOrderDetailRepository, _pOProductetailRepository, _productRepository, _supplierRepository, _SRVisitRepository, _SRVisitDetailRepository, _SRVProductDetailRepository, _EmployeeRepository, ConcernID, IMEINO);
        }


        public bool AddDeliveryOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail, DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.AddDeliveryOrderUsingSP(dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }

        public POProductDetail GetDamagePOPDetail(string DamageIMEI, int ProductID, int ColorID)
        {
            return _basePurchaseOrderRepository.GetDamageReturnPOPDetail(_pOrderDetailRepository, _pOProductetailRepository, DamageIMEI, ProductID, ColorID);
        }
        public POrder GetDamagerReturnOrderByChallanNo(string ChallanNo)
        {
            return _basePurchaseOrderRepository.All.FirstOrDefault(i => i.ChallanNo == ChallanNo && i.Status == (int)EnumPurchaseType.DamageReturn);
        }
        public IEnumerable<ProductWisePurchaseModel> GetDamagePOReport(DateTime fromDate, DateTime toDate, int SupplierID)
        {
            return _basePurchaseOrderRepository.GetDamagePOReport(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, _colorRepository, fromDate, toDate, SupplierID);
        }

        public IEnumerable<ProductWisePurchaseModel> GetDamageReturnProductDetails(int ProductID, int ColorID)
        {
            return _basePurchaseOrderRepository.GetDamageReturnProductDetails(_pOrderDetailRepository, _pOProductetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _colorRepository, ProductID, ColorID);
        }

        public IEnumerable<ProductWisePurchaseModel> DamageReturnProductDetailsReport(int SupplierID, DateTime fromDate, DateTime toDate)
        {
            return _basePurchaseOrderRepository.DamageReturnProductDetailsReport(_pOrderDetailRepository, _pOProductetailRepository, _productRepository, _CompanyRepository, _CategoryRepository, _colorRepository, SupplierID, fromDate, toDate);
        }
        public IQueryable<ProductWisePurchaseModel> AdminPurchaseReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _basePurchaseOrderRepository.AdminPurchaseReport(_supplierRepository, _SisterConcernRepository, ConcernID, fromDate, toDate, EnumPurchaseType.Purchase);
        }

        public List<LedgerAccountReportModel> SupplierLedger(DateTime fromdate, DateTime todate, int SupplierID)
        {
            return _basePurchaseOrderRepository.SupplierLedger(_pOrderDetailRepository, _productRepository, _CompanyRepository, _CategoryRepository
                , _supplierRepository, _UserRepository, _BankTransactionRepository, _cashCollectionRepository, _BankRepository, SupplierID, fromdate, todate);
        }

        public bool IsProductPurchase(int ProductID)
        {
            var porderdetails = from po in _basePurchaseOrderRepository.All
                                join pod in _pOrderDetailRepository.All on po.POrderID equals pod.POrderID
                                where pod.ProductID == ProductID && po.Status == (int)EnumPurchaseType.Purchase
                                select pod;
            if (porderdetails.Count() > 0)
                return true;

            return false;
        }

        public bool IsReturnFound(int porderId)
        {
            string query = string.Format(@"SELECT ISNULL(SUM(ReturnQty),0) FROM dbo.POProductDetails WHERE ReturnSDetailID
                                            IN(
	                                            SELECT SD.SDetailID FROM dbo.POrders PO
	                                            JOIN dbo.POrderDetails POD ON PO.POrderID = POD.POrderID
	                                            JOIN dbo.StockDetails SD ON POD.POrderDetailID = SD.POrderDetailID
	                                            WHERE PO.POrderID = {0}
                                            )", porderId);
            try
            {
                decimal rQty = _basePurchaseOrderRepository.SQLQuery<decimal>(query);
                return rQty > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public IEnumerable<ProductWisePurchaseModel> GetPurchaseDetailReportByPOrderID(int POrderID)
        {
            return _basePurchaseOrderRepository.GetPurchaseDetailReportByPOrderID(_transPOrderRepository,_transPOrderDetailRepository, _productRepository, _pOProductetailRepository, _UserRepository, POrderID);
        }

        public Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync()
        {
            throw new NotImplementedException();
        }
        public bool IsPOReturn(int PoId)
        {
            if (_pOProductetailRepository.All.Any(i => i.ReturnQty > 0 && i.POrderDetail.POrder.POrderID == PoId))
                return true;

            return false;
        }

        public int CheckHireSalesProductStatusByPOId(int id)
        {
            return _purchaseOrderRepository.CheckHireSalesProductStatusByPOId(id);
        }
        public Tuple<bool, int> AddDamagePurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.AddDamagePurchaseOrderUsingSP(dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }
        public bool UpdateDamagePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
             DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            return _purchaseOrderRepository.UpdateDamagePurchaseOrderUsingSP(purchaseOrderId, dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail, dtStock, dtStockDetail);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string, EnumPurchaseType, Tuple<int>>>> GetAllDamagePurchaseOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID)
        {
            return await _basePurchaseOrderRepository.GetAllDamagePurchaseOrderAsync(_supplierRepository, _SisterConcernRepository, fromDate, toDate, IsVATManager, concernID);
        }
        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllDamageReturnPurchaseOrderAsync(DateTime fromDate, DateTime toDate)
        {
            return await _basePurchaseOrderRepository.GetAllDamageReturnPurchaseOrderAsync(_supplierRepository, fromDate, toDate);
        }

        public Tuple<bool, int> AddDamageReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
         DataTable dtPOProductDetail)
        {
            return _purchaseOrderRepository.AddDamageReturnPurchaseOrderUsingSP(dtPurchaseOrder, dtPODetail,
                   dtPOProductDetail);
        }

    }
}
