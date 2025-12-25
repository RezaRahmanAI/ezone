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
    public class ROrderService : IROrderService
    {
        private readonly IBaseRepository<ROrder> _baseROrderRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<ROProductDetail> _ROProductDetailRepository;

        private readonly IROrderRepository _ROrderRepository;

        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<StockDetail> _stockdetailRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;

        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<SisterConcern> _sisterReporsitory;
        private readonly IBaseRepository<HireSalesReturnCustomerDueAdjustment> _creditSaleReturnRepository;
        private readonly IBaseRepository<CreditSale> _creditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _creditSaleDetailsRepository;

        public ROrderService(IBaseRepository<ROrder> baseROrderRepository,
            IROrderRepository returnOrderRepository, IBaseRepository<ROProductDetail> rOProductDetailRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<ROrderDetail> rOrderDetailRepository,
            IBaseRepository<StockDetail> stockdetailRepository, 
            IBaseRepository<Employee> employeeRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> CategoryRepository, IUnitOfWork unitOfWork,IBaseRepository<SisterConcern> sisterReporsitory,
            IBaseRepository<HireSalesReturnCustomerDueAdjustment> creditSaleReturnRepository, IBaseRepository<CreditSale> creditSaleRepository,
            IBaseRepository<CreditSaleDetails> creditSaleDetailsRepository)
        {
            _baseROrderRepository = baseROrderRepository;
            _ROrderRepository = returnOrderRepository;
            _ROrderDetailRepository = rOrderDetailRepository;
            _ROProductDetailRepository = rOProductDetailRepository;
            _CustomerRepository = customerRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _stockdetailRepository = stockdetailRepository;
            _employeeRepository = employeeRepository;

            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;

            _unitOfWork = unitOfWork;
            _sisterReporsitory = sisterReporsitory;
            _creditSaleReturnRepository = creditSaleReturnRepository;
            _creditSaleRepository = creditSaleRepository;
            _creditSaleDetailsRepository = creditSaleDetailsRepository;

        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string>>> GetAllReturnOrderAsync()
        {
            return await _baseROrderRepository.GetAllReturnOrderAsync(_CustomerRepository);
        }

        public void AddReturnOrder(ROrder returnOrder)
        {
            _baseROrderRepository.Add(returnOrder);
        }

        public void AddReturnOrderUsingSP(DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            _ROrderRepository.AddReturnOrderUsingSP(dtReturnOrder, dtRODetail,
                dtROProductDetail, dtStock, dtStockDetail);
        }

        public void UpdateReturnOrderUsingSP(int returnOrderId, DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            _ROrderRepository.UpdateReturnOrderUsingSP(returnOrderId, dtReturnOrder, dtRODetail,
                dtROProductDetail, dtStock, dtStockDetail);
        }

        public void DeleteReturnOrderDetailUsingSP(int customerId, int rorderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail)
        {
            _ROrderRepository.DeleteReturnOrderDetailUsingSP(customerId, rorderDetailId, productId,
                colorId, userId, quantity, totalDue, dtPOProductDetail);
        }

        public int CheckProductStatusByROId(int id)
        {
            return _ROrderRepository.CheckProductStatusByROId(id);
        }

        public int CheckProductStatusByRODetailId(int id)
        {
            return _ROrderRepository.CheckProductStatusByRODetailId(id);
        }

        public void SaveReturnOrder()
        {
            _unitOfWork.Commit();
        }

        public ROrder GetReturnOrderById(int id)
        {
            return _baseROrderRepository.FindBy(x => x.ROrderID == id).First();
        }

        public void DeleteReturnOrderUsingSP(int id, int userId)
        {
            _ROrderRepository.DeleteReturnOrderUsingSP(id, userId);
        }

        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
          GetReturnReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            //commented to skip error
            return _baseROrderRepository.GetReturnReportByConcernID(_CustomerRepository, _ROrderDetailRepository, fromDate, toDate, concernID, CustomerType);

        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, 
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, decimal,int, string, int, int, string,
                    Tuple<decimal>>>>>
        GetReturnDetailReportByConcernID(DateTime fromDate, DateTime toDate, int CustomerType, int concernID, bool IsAdminReport)
        {
            return _baseROrderRepository.GetReturnDetailReportByConcernID(_ROrderDetailRepository,
                _productRepository, _stockdetailRepository, _CustomerRepository, _CategoryRepository, 
                _sisterReporsitory, _creditSaleRepository, _creditSaleDetailsRepository, _creditSaleReturnRepository,
                fromDate, toDate, CustomerType,concernID,IsAdminReport);
        }


        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
       GetReturnDetailReportByReturnID(int ReturnID, int concernID)
        {
            return _baseROrderRepository.GetReturnDetailReportByReturnID(_ROrderDetailRepository, _productRepository, _stockdetailRepository, ReturnID, concernID);
        }



        public List<ProductWiseSalesReportModel> ProductWiseReturnReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseROrderRepository.ProductWiseReturnReport(_ROrderDetailRepository, _CustomerRepository, _employeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }
        public List<ProductWiseSalesReportModel> ProductWiseReturnDetailsReport(int CompanyID, int CategoryID, 
            int ProductID, DateTime fromDate, DateTime toDate, int CustomerType,int CustomerID)
        {

            return _baseROrderRepository.ProductWiseReturnDetailsReport(_ROrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository, _stockdetailRepository, _CustomerRepository, _creditSaleRepository, _creditSaleDetailsRepository, _creditSaleReturnRepository
                , CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType,CustomerID);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync(DateTime fromDate, DateTime toDate)
        {
            return await _baseROrderRepository.GetReturnOrdersAsync(_ROrderDetailRepository, _CustomerRepository, fromDate, toDate);
        }
        public bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            return _ROrderRepository.AddReturnOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }
        public bool AddHireSalesReturnUsingSP(DataTable dtHireSaleCustomerDue, DataTable dtCreditDetails, decimal salePrice, decimal purchasePrice)
        {
            return _ROrderRepository.AddHireSaleReturnUsingSP(dtHireSaleCustomerDue, dtCreditDetails, salePrice, purchasePrice);
        }


    }
}
