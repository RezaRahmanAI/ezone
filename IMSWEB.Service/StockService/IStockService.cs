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
    public interface IStockService
    {
        void AddStock(Stock stock);
        void SaveStock();
        IQueryable<Stock> GetAllStock();
        Stock GetStockById(int id);
        Stock GetStockByProductIdandGodownID(int ProductID, int GodownID);
        Stock GetStockByProductIdandColorIDandGodownID(int ProductID, int GodownID,int ColorID);
        
        Stock GetStockByProductId(int id);
        Task<IEnumerable<Tuple<int, string, string, string,
            decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>>> GetAllStockAsync(int ConcernID, bool IsVATManager);

        Task<IEnumerable<Tuple<int, string, string, string,
            string, string, string, Tuple<string>>>> GetAllStockDetailAsync(int ConcernID, bool IsVATManager);

        IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
           GetforStockReport(string userName, int concernId, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType);
        void DeleteStock(int id);
        IEnumerable<Tuple<string, string, decimal, decimal, decimal,decimal,DateTime>> GetPriceProtectionReport(string userName, int concernId, DateTime dFDate,DateTime dTDate);
        IEnumerable<Tuple<int, string, string>> GetStockDetailsByID(int stockId);
        IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int concernID, int ProductID);
        bool IsIMEIAvailableForSRVisit(int ProductID, int ColorID, string IMEI);
        string GetStockProductsHistory(int StockID);
        List<StockLedger> GetStockLedgerReport(int reportType, string CompanyName, string CategoryName, string ProductName, DateTime dFDate, DateTime dTDate, int ConcernID);
        List<ProductDetailsModel> GetStockProductsBySupplier(int SupplierID);
        List<ProductDetailsModel> GetSupplierStockDetails(int SupplierID, int ProductID, int ColorID, int GodownID);

        IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
          GetforAdminStockReport(string userName, int concernId, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID);

        bool IsIMEIExistInGodown(int ConcernID, int GodownID, string IMEI);
        ProductDetailsModel GetStockIMEIDetail(string IMEI);
        ProductDetailsModel GetIMEIDetail(string IMEI);
        ProductDetailsModel GetStockIMEIDetailsByLastSomedigit(string IMEI);
        ProductDetailsModel GetSRVisitIMEIDetails(string IMEI, int EmployeeID);
        IQueryable<ProductDetailsModel> GetStocksByProductId(int id);
        void SaveStockValue(int ConcernID);

        IQueryable<ProductDetailsModel> GetStockDetails();

        IQueryable<ProductDetailsModel> GetStocs();

        IEnumerable<StockReportWithDateReportModel> StockReportWithDate(int ConcernID, int ProductID, int CompanyID, int CategoryID);

        IEnumerable<StockForcastingReportModel> StockForcastingReport(DateTime fromDate, DateTime toDate, int ConcernID);

        IEnumerable<StockForcastingReportModel> StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, int ProductID);
        ProductDetailsModel GetIMEIDetails(string IMEI, bool isStockIMEI = false);
        IQueryable<Stock> GetAll();
        List<ProductDetailsModel> GetDamageStockProductsBySupplier(int SupplierID);
        ProductDetailsModel GetDamageStockIMEIDetail(string IMEI);

        IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
          GetforAdminProductStockReport(string userName, int concernId, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID);

        List<StockLedger> GetRateWiseStockLedgerReport(int reportType, string CompanyName, string CategoryName, string ProductName, DateTime dFDate, DateTime dTDate, int ConcernID);

        List<ProductDetailsModel> GetSupplierDamageStockDetails(int SupplierID, int ProductID, int ColorID, int GodownID);

        IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
           GetforStockReportZeroQty(string userName, int concernId, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType);

        IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, decimal>>>
     GetforStockReportNew(string userName, int concernId, int reportType, List<int> CompanyIds, List<int> CategoriesList, List<int> ProductIds, List<int> GodownIds, List<int> ColorIds, bool IsVATManager);

    }
}
