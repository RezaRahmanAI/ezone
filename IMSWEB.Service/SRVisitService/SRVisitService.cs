using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.SPViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public class SRVisitService : ISRVisitService
    {
        private readonly IBaseRepository<SRVisit> _baseSRVisitRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly ISRVisitRepository _SRVisitRepository;
        private readonly IBaseRepository<SRVProductDetail> _SRVProductetailRepository;
        private readonly IBaseRepository<SRVisitDetail> _SRVisitDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<Stock> _StockRepository;
        private readonly IBaseRepository<StockDetail> _StockDetailRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<CreditSale> _CreditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSaleDetailsRepository;

        private readonly IUnitOfWork _unitOfWork;

        public SRVisitService(IBaseRepository<SRVisit> baseSRVisitRepository,
            ISRVisitRepository srVisitRepository, IBaseRepository<SRVProductDetail> sRVProductDetaildetailRepository,
            IBaseRepository<Employee> employeeRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<SRVisitDetail> sRVisitDetailRepository,
            IBaseRepository<Category> CategoryRepository, IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            IBaseRepository<Stock> StockRepository, IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<Supplier> supplierRepository, IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
            IUnitOfWork unitOfWork)
        {
            _baseSRVisitRepository = baseSRVisitRepository;
            _SRVisitRepository = srVisitRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _SRVProductetailRepository = sRVProductDetaildetailRepository;
            _colorRepository = colorRepository;
            _SRVisitDetailRepository = sRVisitDetailRepository;
            _CategoryRepository = CategoryRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _StockRepository = StockRepository;
            _StockDetailRepository = StockDetailRepository;
            _supplierRepository = supplierRepository;
            _CustomerRepository = CustomerRepository;
            _CreditSaleRepository = CreditSaleRepository;
            _CreditSaleDetailsRepository = CreditSaleDetailsRepository;
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, EnumSRVisitType>>> GetAllSRVisitAsync()
        {
            return await _baseSRVisitRepository.GetAllSRVisitAsync(_employeeRepository);
        }

        public IEnumerable<Tuple<string, string, DateTime, string>> GetSRVisitReportByConcernID(DateTime fromDate, DateTime toDate, int concernID)
        {
            //commented to skip error
            return _baseSRVisitRepository.GetSRVisitReportByConcernID(_employeeRepository, fromDate, toDate, concernID);

        }
        public IEnumerable<Tuple<DateTime, string, string, string, string, string>>
          GetSRVisitDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSRVisitRepository.GetSRVisitDetailReportByConcernID(_SRVisitDetailRepository, _productRepository, _SRVProductetailRepository, _colorRepository, fromDate, toDate, concernID);
        }


        public IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<string>>>
        GetSRViistDetailReportByEmployeeID(DateTime fromDate, DateTime toDate, int ConcernId, int EmployeeID)
        {
            return _baseSRVisitRepository.GetSRViistDetailReportByEmployeeID(_SRVisitDetailRepository, _productRepository, _SRVProductetailRepository, fromDate, toDate, ConcernId, EmployeeID);
        }


        public void AddSRVisitChallan(SRVisit srVisit)
        {
            _baseSRVisitRepository.Add(srVisit);
        }

        public bool AddSRVisitChallanUsingSP(DataTable dtSRVisit, DataTable dtSRVDetail,
            DataTable dtSRVProductDetail)
        {
          return  _SRVisitRepository.AddSRVisitChallanUsingSP(dtSRVisit, dtSRVDetail,
                dtSRVProductDetail);
        }

        public bool UpdateSRVisitChallanUsingSP(int SRVisitId, DataTable dtSRVisit, DataTable dtSRVDetail,
            DataTable dtSRVProductDetail)
        {
           return _SRVisitRepository.UpdateSRVisitChallanUsingSP(SRVisitId, dtSRVisit, dtSRVDetail,
                dtSRVProductDetail);
        }

        public void DeleteSRVisitDetailUsingSP(int employeeId, int SRVDetailId, int productId,
            int colorId, int userId, decimal quantity, DataTable dtSRVProductDetail)
        {
            _SRVisitRepository.DeleteSRVisitDetailUsingSP(employeeId, SRVDetailId, productId,
                colorId, userId, quantity, dtSRVProductDetail);
        }

        public void SaveSRVisit()
        {
            _unitOfWork.Commit();
        }

        public SRVisit GetSRVisitById(int id)
        {
            return _baseSRVisitRepository.FindBy(x => x.SRVisitID == id).First();
        }

        public SRVisit GetSRVisitByChallanNo(string challanNo,int ConcernID)
        {
            return _baseSRVisitRepository.FindBy(x => x.ChallanNo == challanNo && x.ConcernID == ConcernID).First();
        }


        public bool DeleteSRVisitUsingSP(int id, int userId)
        {
          return  _SRVisitRepository.DeleteSRVisitUsingSP(id, userId);
        }

        public IEnumerable<SRVisitStatusReportModel> SRVisitStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
           return _SRVisitRepository.SRVisitStatusReport(fromDate,toDate,ConcernID,SRID);
        }


        public IEnumerable<SRVisitReportModel> SRVisitReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            return _SRVisitRepository.SRVisitReport(fromDate, toDate, ConcernID, SRID);
        }


        public IEnumerable<SRWiseCustomerStatusReportModel> SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            return _SRVisitRepository.SRWiseCustomerStatusReport(fromDate, toDate, ConcernID, SRID);
        }




        public bool IsIMEIAlreadyIssuedToSR(int ProductID, int ColorID, string IMEINO)
        {
            return _baseSRVisitRepository.IsIMEIAlreadyIssuedToSR(_SRVisitDetailRepository, _SRVProductetailRepository, ProductID, ColorID, IMEINO);
        }


        public IEnumerable<AdvancePODetail> GetAllSRVisitStockIMEIBySRID(int EmployeeID)
        {
            return _baseSRVisitRepository.GetAllSRVisitStockIMEIBySRID(_SRVisitDetailRepository, _SRVProductetailRepository, _productRepository, _CategoryRepository, EmployeeID);
        }


        public bool ReturnSRVisitUsingSP(DataTable dt, int EmployeeID)
        {
            return _SRVisitRepository.ReturnSRVisitUsingSP(dt, EmployeeID);
        }


        public bool CheckSRVisitReturnValidity(int SRVisitID)
        {
            return _SRVisitRepository.CheckSRVisitReturnValidity(SRVisitID);
        }

        public IEnumerable<SRVisitReportModel> SRVisitReportDetails(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            return _baseSRVisitRepository.SRVisitReportDetails(_SRVisitDetailRepository, _SRVProductetailRepository, _productRepository, _SOrderRepository, _SOrderDetailRepository, _StockDetailRepository, _StockRepository, _employeeRepository, _CustomerRepository, _CreditSaleRepository, _CreditSaleDetailsRepository, fromDate, toDate, SRID);
        }

        public decimal GetRemainingQuantityForSRVisit(int productId, int colorId, int concernId, int GodownId, int? employeeId = null, int stockDetailsId = 0)
        {
            decimal remainingQty = 0m;
            string empQuery = string.Empty;
            if (employeeId.HasValue)
                empQuery = string.Format(@" AND sr.EmployeeID != {0}", employeeId);

            string query = string.Format(@"SELECT SUM((sv.Quantity)) SRQty, 
                                            (SELECT s1.Quantity FROM Stocks s1 WHERE s.ProductID = s1.ProductID AND s.ColorID = s1.ColorID AND s.GodownID = s1.GodownID) StockQty
                                            FROM SRVisitDetails sv 
                                            JOIN SRVisits sr ON sv.SRVisitID = sr.SRVisitID
                                            JOIN Stocks s ON sv.ProductID = s.ProductID AND sv.ColorID = s.ColorID
                                            JOIN SRVProductDetails sd ON sv.SRVisitDID = sd.SRVisitDID
                                            WHERE  sd.Status = 1 AND sv.ProductID = {0} AND sv.ColorID = {1} AND s.ConcernID = {2} AND s.GodownID = {3} {4}
                                            GROUP BY s.ProductID, s.ColorID, s.GodownID", productId, colorId, concernId, GodownId, empQuery);



            SRQuantity data = _SRVisitDetailRepository.SQLQuery<SRQuantity>(query);
            if (data != null)
                remainingQty = data.StockQty - data.SRQty;
            else
            {
                string stockQuery = string.Empty;
                if (stockDetailsId > 0)
                {
                    stockQuery = string.Format(@"SELECT SUM(sd.Quantity) StockQty, CAST(0 as DECIMAL(18, 2)) SRQty FROM Stocks s
                                                    JOIN StockDetails sd ON s.StockID = sd.StockID
                                                    WHERE sd.IsDamage = 0 AND s.ProductID = {0} AND s.ColorID = {1} AND s.ConcernID = {2} AND s.GodownID = {3}", productId, colorId, concernId, GodownId, stockDetailsId);
                }
                else
                {
                    stockQuery = string.Format(@"SELECT Quantity StockQty, CAST(0 as DECIMAL(18, 2)) SRQty FROM Stocks WHERE ProductID = {0} AND ColorID = {1} AND ConcernID = {2} AND s.GodownID = {3}", productId, colorId, concernId, GodownId);
                }
                SRQuantity stockData = _SRVisitDetailRepository.SQLQuery<SRQuantity>(stockQuery);
                remainingQty = stockData.StockQty;
            }

            return remainingQty;
        }
    }
}
