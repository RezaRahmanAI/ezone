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
    public interface ICashCollectionRepository
    {
        void UpdateTotalDue(int CustomerId, int SupplierId, int BankID, int BankWithdrawID, decimal TotalDue);
        void UpdateTotalDuePaymentReturn(int SupplierId, int BankID, int BankWithdrawID, decimal TotalDue);
        IEnumerable<DailyCashBookLedgerModel> DailyCashBookLedger(DateTime fromDate, DateTime toDate,int ConcernID);
        void UpdateTotalDueWhenEdit(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt);
        IEnumerable<CashInHandReportModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType);
        void UpdateTotalDueForInvestment(int SIHID, int SIHId, int BankID, int BankWithdrawID, decimal TotalRecAmt);

        void UpdateTotalDueForExpenditure(int ExpenseItemID, int BankID, int BankWithdrawID, decimal TotalRecAmt, int ConcernID,int userId, string Remarks, DateTime TranDate);
        IEnumerable<ServiceChargeModel> ServiceChargeReport(int Month, int Year);

        void UpdateTotalDueWhenEditReturnType(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt);

        IEnumerable<CashInHandReportModel> AdminCashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType, List<SisterConcern> sisterConcernList);
        void UpdateTotalDueCustomerPaymentReturn(int CustomerID, int BankID, int BankWithdrawID, decimal TotalRecAmt);
    }
}
