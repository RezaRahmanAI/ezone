using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;

namespace IMSWEB.Service
{
    public interface ICreditSalesOrderService
    {
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string, int>>>>
            GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager,
            int concernID, string InvoiceNo = "", string ContactNo = "", string CustomerName = "",string AccountNo="");

        void AddSalesOrder(CreditSale salesOrder);

        Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules, DataTable dtBankTrans);
        IQueryable<CreditSale> GetAllIQueryable();
        bool InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID);

        void SaveSalesOrder();

        CreditSale GetSalesOrderById(int id);

        IEnumerable<CreditSaleDetails> GetSalesOrderDetails(int id);

        void UpdateSalesOrder(CreditSale creditSale);

        IEnumerable<Tuple<int, int, int, int,
            decimal, decimal, decimal, Tuple<decimal, string, string, int, string, decimal>>> GetCustomSalesOrderDetails(int id);

        IEnumerable<CreditSalesSchedule> GetSalesOrderSchedules(int id);
        IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(DateTime fromDate, DateTime toDate, EnumCustomerType customerType=0, int EmployeeID=0);
        IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, int concernID, int EmployeeID, bool IsAdmin);
        IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetCreditCollectionReport(DateTime fromDate, DateTime toDate, int concernID, int CustomerID);
        IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(DateTime date, int concernID);
        IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>>
            GetDefaultingCustomer(DateTime fromDate, DateTime toDate, int concernID);

        bool ReturnSalesOrderUsingSP(int orderId, int userId);

        void DeleteSalesOrder(int id);

        bool HasPaidInstallment(int id);

        //void CalculatePenaltySchedules(int ConcernID);

        void CorrectionStockData(int concermID);

        //CreditSale GetSalesOrderByInvoiceNo(string InvoiceNo,int concernID);
        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, 
            Tuple<decimal, decimal, decimal, decimal, decimal, int, string, Tuple<string, string>>>>
         GetCreditSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType);

        IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, int, string, string, int, int, string>>>>
        GetCreditSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, bool IsAdminReport);
        decimal GetDefaultAmount(int CreditSaleID, DateTime FromDate);
        List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID);
        List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType, int CustomerID);

        void DeleteSchedule(CreditSalesSchedule CreditSalesSchedule);
        void AddSchedule(CreditSalesSchedule CreditSalesSchedule);
        void UpdateSchedule(CreditSalesSchedule scheduel);
        List<SOredersReportModel> SRWiseCreditSalesReport(int EmployeeID, DateTime fromDate, DateTime toDate);
        IQueryable<SOredersReportModel> GetAdminCrSalesReport(int ConcernID, DateTime fromDate, DateTime toDate);
        IQueryable<CashCollectionReportModel> AdminInstallmentColllections(int ConcernID, DateTime fromDate, DateTime toDate);

        HireAccountDetailsReportModel HireAccountDetails(DateTime fromDate, DateTime toDate, int ConcernID);
        CreditSalesSchedule GetScheduleByScheduleID(int ScheduleID);
        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal,
            EnumSalesType, Tuple<string, string>>>> GetAllPendingSalesOrderAsync();

        IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, string Status);

        bool PendingInstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            int CardTypeSetupID);

        Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
                        DataTable dtSchedules);

        bool ApprovedSalesOrderUsingSP(DataTable dtSOrder, DataTable dtSOrderDetails,
       DataTable dtSchedules, DataTable dtBankTrans, int orderId);

        bool InstallmentApprovedSP(int orderId, decimal installmentAmount, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID, int ScheduleID);

        CreditSalesSchedule GetScheduleByID(int ScheduleID);

        Tuple<bool, string> IsIMEIInPendingSales(int StockDetailID, int SOrderID);
        ProductDetailsModel GetLastSalesOrderByCustomerID(int CustomerID);
        IEnumerable<AdjustmentReportModel> GetAdjustmentReport(DateTime fromDate, DateTime toDate);
        IEnumerable<UpcommingScheduleReport> GetLastPayAdjAmt(DateTime fromDate, DateTime toDate, int concernID);
        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>> GetAllSalesReturnOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID, string InvoiceNo = "", string ContactNo = "", string CustomerName = "", string AccountNo = "");

        IEnumerable<Tuple<DateTime, string, string, string, decimal, string, decimal, Tuple<string, string, decimal>>>
        GetHireReturnDetailReportByReturnID(int ReturnID, int concernID);
        HireSalesReturnCustomerDueAdjustment GetHireSalesReturnOrderById(int id);
        void AddInterestHistory(CreditInterestHistory history);
        decimal GetTotalPrevInterest(int creditSaleId);

    }
}
