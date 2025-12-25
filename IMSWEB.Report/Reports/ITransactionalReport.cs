using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Report
{
    public interface ITransactionalReport
    {
        byte[] ExpenditureReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, EnumCompanyTransaction Status,
            int ExpenseItemID, bool isAdminReport, int selectedConcernID = 0);
        byte[] SalesReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, int reportType, string period, int CustomerType,
            bool IsFalesReport, string ClientDateTime, int selectedConcernID = 0);
        byte[] PurchaseReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, int reportType, string period, EnumPurchaseType PurchaseType, bool IsAdminReport = false, int SelectedConcernID = 0);

        byte[] SalesInvoiceReport(SOrder sorder, string userName, int concernID, bool isPreview);
        byte[] SalesInvoiceHistoryReport(int oOrderID, string userName, int concernID);

        byte[] WarrantyInvoice(int orderId, string p1, int p2);
        byte[] ChallanReport(SOrder sorder, string userName, int concernID);
        //byte[] CreditChallanReport(CreditSale sorder, string userName, int concernID);
        byte[] CustomeWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CustomerID);

        byte[] MOWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int RptType);

        byte[] MOWiseCustomerDueRpt(string userName, int concernID, int MOID, int RptType);

        byte[] SuplierWisePurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int SupplierID);

        byte[] StockDetailReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int PCategoryID, bool IsVATManager, int StockType, int MaaManager, int filetype);
        byte[] StockSummaryReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType, int filetype);
        byte[] SalesInvoiceReport(int orderId, string p1, int p2);
        byte[] ChallanReport(int orderId, string p1, int p2);
        byte[] CreditChallanReport(int orderId, string p1, int p2);

        byte[] InstallmentCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int EmployeeId, int selectedConcernID, bool IsAdminReport);

        byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string userName, int concernID, EnumCustomerType customerType = 0, int EmployeeID = 0);

        byte[] DefaultingCustomerReport(DateTime date, string userName, int concernID);
        byte[] DefaultingCustomerReport(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] CashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int customerId, EnumCustomerType customerType);

        byte[] CashDeliverReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int supplierId, bool IsAdmin, int selectedConcernID);

        byte[] CreditSalesInvoiceReport(CreditSale sorder, string userName, int concernID);

        byte[] CreditSalesInvoiceReportByID(int sorderID, string userName, int concernID);

        byte[] SRInvoiceReport(int orderId, string p1, int p2);

        byte[] SRInvoiceReportByChallanNo(string challanNo, string p1, int p2);

        byte[] MOWiseSDetailReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID);

        byte[] SRVisitStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID);

        byte[] ProductWisePriceProtection(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ProductWisePandSReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int productID);
        byte[] SRWiseCustomerSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] CustomerLedgerDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CustomerLedgerSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID, int IsOnlyDue);
        byte[] DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID);
        byte[] DailyCashBookLedger(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] ProfitAndLossReport(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] SummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ReplacementInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID);
        byte[] ReplaceInvoiceReportByID(int orderId, string username, int concernID);
        byte[] ReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID);
        byte[] ReturnInvoiceReportByID(int orderId, string username, int concernID);
        byte[] DailyWorkSheet(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] SRVisitReportUsingSP(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] SRVisitReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int ReportType);
        byte[] SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] ReplacementReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] ReturntReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CashCollectionMoneyReceipt(CashCollection cashCollection, string userName, int concernID);
        byte[] CashCollectionMoneyReceiptByID(int cashCollectionID, string userName, int concernID);

        byte[] SalesOrderMoneyReceipt(SOrder oSorder, string userName, int concernID);
        byte[] SalesOrderMoneyReceiptByID(int SOrderID, string userName, int concernID);
        byte[] SOrderMoneyReceiptByID(int SOrderID, string userName, int concernID, bool isPosRecipt);
        byte[] CashDeliveryMoneyReceiptPrint(int cashCollectionID, string userName, int concernID);
        byte[] CrditSalesMoneyReceipt(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID);
        byte[] CrditSalesMoneyReceiptByID(int CreditSalesID, string userName, int concernID);
        byte[] MonthlyBenefit(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ProductWiseBenefitReport(DateTime fromDate, DateTime toDate, int ProductID, string userName, int concernID, int CompnayID, int CategoryID, int CustomerID);
        byte[] ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int CustomerID, string userName, int concernID);
        byte[] ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int SupplierID, string userName, int concernID, EnumPurchaseType PurchaseType);
        byte[] DamageProductReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] SRWiseCashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] ProductwiseSalesDetails(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType,
               int CustomerID);

        byte[] ProductwiseSalesSummary(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType, int CustomerID);

        byte[] ProductWisePurchaseDetailsReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, int SupplierID);
        byte[] BankTransactionReport(string userName, int concernID, int reportType, int BankID, DateTime fromDate, DateTime toDate);
        byte[] POInvoice(POrder POrder, string userName, int concernID, bool isPreview);
        byte[] POInvoiceByID(int POrderID, string userName, int concernID);
        byte[] BankSummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID);

        byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID, int commonConcernId);
        byte[] GetDamagePOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate);
        byte[] GetDamageReturnPOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate);
        byte[] GetSalarySheet(DateTime dtSalaryMonth, int EmployeeID, int DepartmentID, List<int> EmployeeIDList, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth);

        byte[] GetAdvanceSalaryReport(DateTime fromDate, DateTime toDate, string userName, int ConcernID, int EmployeeID);
        byte[] GetPaySlip(DateTime dtSalaryMonth, int EmployeeID, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth);
        byte[] NewBankTransactionsReport(DateTime fromDate, DateTime toDate, int BankID, string UserName, int ConcernID, int reportConcern);
        byte[] SalesReportAdmin(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID);
        byte[] AdminPurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID);
        byte[] AdminCustomerDueRpt(string userName, int concernID, int UserCocernID, int CustomerType, int DueType);
        byte[] AdminCashCollectionReport(string userName, int concernID, int UserCocernID,
            DateTime fromDate, DateTime toDate, EnumCustomerType customerType, int customerID);
        byte[] CashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType, int filetype);
        byte[] BankTransMoneyReceipt(string userName, int concernID, int BankTranID);
        byte[] ExpenseIncomeMoneyReceipt(string userName, int concernID, int ExpenditureID, bool IsExpense);
        byte[] DailyAttendence(string userName, int concernID, int DepartmentID, DateTime Date, bool IsPresent, bool IsAbsent);
        byte[] StockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName);
        byte[] StockLedgerReportExcel(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int filetype);

        byte[] SupplierLedger(string userName, int concernID, DateTime fromDate, DateTime toDate, int SupplierID, int ReportType);
        byte[] CustomerLedger(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int CustomerID, int ReportType);

        byte[] TransferInvoiceByID(int TransferID, string UserName, int ConcernID);

        byte[] TransferReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int selectedConcernID);
        byte[] SMSReport(string UserName, int ConcernID, int Status, DateTime fromDate, DateTime toDate, bool isAdminReport, int selectedConcernID = 0);
        byte[] CustomerDueReportNew(DateTime fromDate, DateTime toDate, string userName,
            int concernID, int CustomerID, int IsOnlyDue, EnumCustomerType CustomerType,
            bool IsAdminReport, int SelectedConcernID);
        byte[] GetSummaryReport(DateTime Date, int ConcernID, string userName);
        byte[] GetTrialBalance(DateTime fromDate, DateTime toDate, string UserName, int ConcernID,
            string ClientDateTime, int selectedConcernID, bool IsAdminreport);
        byte[] ProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] BalanceSheet(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        //byte[] BalanceSheetNew(DateTime asOnDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] BalanceSheetNew(DateTime asOnDate, string UserName, int ConcernID, string ClientDateTime, int selectedConcernID, bool IsAdminreport);
        byte[] AdminStockSummaryReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID);
        byte[] HireAccountDetails(DateTime fromDate, DateTime toDate, string UserName, int ConcernID);
        byte[] MonthlyTransactionReport(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] LiabilityReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int HeadID, bool OnlyHead);

        byte[] BarCodeGenrator(POrder obj, string userName, int concernID);
        byte[] BarCodeGenratorByID(int POrderID, string userName, int concernID);
        byte[] PrintIMEI(int SDetailID, string userName, int concernID);
        byte[] DistributorAnalysis(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] AdjustmentReport(DateTime fromDate, DateTime toDate, string userName, int ReportType, int ConcernID);
        byte[] AdvanceLoanReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int EmployeeID, bool OnlyEmployee);
        byte[] StockQTYReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager);
        byte[] LastPayAdjReport(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] StockForcastingReport(DateTime fromDate, DateTime toDate, string userName, int concernID, string ClientDateTime);
        byte[] StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, string userName, int concernID, string ClientDateTime, int ProductID);
        byte[] StockDetailReportWithDate(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID);
        byte[] TotalLiabilityPayRec(DateTime asOnDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] SalesInvoiceWithOutBarcodeReport(SOrder oOrder, string userName, int concernID);
        byte[] SalesInvoiceWithOutBarcodeReport(int oOrderID, string userName, int concernID);
        byte[] CashCollectionReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID,
        int customerId, int ReportType);
        byte[] ServiceCharge(int Month, int Year, string UserName, int ConcernID, DateTime fromDate);
        byte[] HireReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID);
        byte[] HireReturnInvoiceReportByID(int orderId, string username, int concernID);
        byte[] UserAuditDetailsReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, EnumObjectType ObjectType);
        byte[] GetBankLoanInvoice(int loanId, string userName, int concernID);
        byte[] GetBankLoanCollectionInvoice(int loanCollectionId, string userName, int concernID);
        byte[] GetPendingBankLoan(string userName, int concernID);


        byte[] CustomerAdjustmentReport(DateTime fromDate, DateTime toDate, string userName,
         int concernID, EnumTranType AdjustmentType, int SelectedConcernID, int CustomerId);

        byte[] SupplierAdjustmentReport(DateTime fromDate, DateTime toDate, string userName,
      int concernID, EnumTranType AdjustmentType, int SelectedConcernID, int SupplierId);

        byte[] DiscountAdjReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID,
        int customerId, int ReportType);

        byte[] TransferReportNewFormat(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int FromConern, int ToConcern);


         
        byte[] PurchaseReportNew(DateTime fromDate, DateTime toDate, string userName,
    int concernID, int reportType, string period, EnumPurchaseType PurchaseType, bool IsAdminReport = false, int SelectedConcernID = 0);


        byte[] GetTrialBalanceNew(DateTime fromDate, DateTime toDate, string UserName, int ConcernID,
        string ClientDateTime, int selectedConcernID, bool IsAdminreport);


        byte[] AdminCashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int SelectedConcern);


        byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName,
       int concernID, int CustomerID, int IsOnlyDue, EnumCustomerType CustomerType,
       bool IsAdminReport, int SelectedConcernID);


        byte[] AdminProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] ProductWiseBenefitReportNew(DateTime fromDate, DateTime toDate, int ProductID, int CompanyID, int CategoryID, string userName, int concernID);
        byte[] PurchaseInvoiceHistoryReport(int oOrderID, string userName, int concernID);
        byte[] EmobileSalesInvoiceReport(SOrder sorder, string userName, int concernID);
        byte[] EmobileSalesInvoiceReport(int orderId, string p1, int p2, bool IsFakeInvoice);
        byte[] EmobileCreditSalesInvoiceReport(CreditSale sorder, string userName, int concernID);

        byte[] EmobileCreditSalesInvoiceReportByID(int sorderID, string userName, int concernID);
        byte[] DOInvoiceReport(string name, int concernID, int DOID);
        byte[] DOReport(string Username, int ConcernID, DateTime fromDate, DateTime toDate, int customerID, int SupplierID, int POType);
        byte[] DOInvoiceReportExcel(string userName, int concernID, int DOID, int filetype);
        byte[] ProductWisePurchaseDOReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);
        byte[] VoucherTransactionLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType);
        byte[] PrintPOSInvoice(int SOrderID, string userName, int concernID);
        byte[] AdminProductStockReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID, int filetype);

        byte[] RateWiseStockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName);
        byte[] WarrantyHireInvoice(int orderId, string p1, int p2);
        byte[] StockSummaryReportZeroQty(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType, int filetype);

        byte[] StockDetailReportNew(string userName, int concernID, int reportType, List<int> CompanyIds, List<int> CategoriesList, List<int> ProductIds, List<int> GodownIds, List<int> ColorIds, bool IsVATManager);
    }
}
