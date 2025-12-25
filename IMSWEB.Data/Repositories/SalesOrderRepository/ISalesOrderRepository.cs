using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using IMSWEB.Model.SPModel;

namespace IMSWEB.Data
{
    public interface ISalesOrderRepository
    {
        Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate, DataTable dtBankTrans);
        void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        void AddSOReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DataTable dtPurchaseOrder, DataTable dtPurchaseOrderDetail, DataTable dtPOProductDetail);
        bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, DataTable dtBankTrans);
        bool DeleteSalesOrderUsingSP(int orderId, int userId);
        void DeleteSalesOrderDetailUsingSP(int sorderDetailId, int userId);
        void CorrectionStockData(int concermID);
        List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID);
        List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue);
        bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID);

        List<ReturnReportModel> ReturnReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);
        List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);

        bool UpdatePendingSalesUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail);

        Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate);

        bool ApprovedSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, int OrderID, DataTable dtBankTrans);

        IEnumerable<ProductPickerInStockReportModel> ProductPickerInStock(int ConcernID);
        IEnumerable<ProductPickerInStockReportModel> DateWiseProductPickerInStock(int ConcernID);
    }
}
