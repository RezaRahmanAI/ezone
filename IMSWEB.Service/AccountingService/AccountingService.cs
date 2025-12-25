using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class AccountingService : IAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountingRepository _AccountingRepository;
        private readonly IBaseRepository<Customer> _customerRepository;

        public AccountingService(IUnitOfWork unitOfWork, IAccountingRepository AccountingRepository, IBaseRepository<Customer> customerRepository)
        {
            _unitOfWork = unitOfWork;
            _AccountingRepository = AccountingRepository;
            _customerRepository = customerRepository;
        }

        public IEnumerable<RPTTrialBalance> GetTrialBalance(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.GetTrialBalance(fromDate, toDate, ConcernID);
        }

        public IEnumerable<RPTTrialBalance> GetBalanceSheet(DateTime asOnDate, int ConcernID)
        {
            return _AccountingRepository.GetBalanceSheet(asOnDate, ConcernID);
        }


        public IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.ProfitLossAccount(fromDate, toDate, ConcernID);
        }

        public IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.BalanceSheet(fromDate, toDate, ConcernID);
        }

        public List<ProfitLossReportModel> BalanceSheetNew(DateTime asOnDate, int concernID)
        {
            List<ProfitLossReportModel> balanceSheetData = new List<ProfitLossReportModel>();
            ProfitLossReportModel particulars = null;


            #region Debit part
            #region Current Assets only for name
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = 0m,
                CreditParticulars = "",
                DebitParticulars = "Current Assets"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region cash in hand
            string cashInHandQuery = string.Format(@"SELECT CAST(ISNULL(pb.Amount, 0.00) AS DECIMAL(18, 2)) CahsInHand FROM PrevBalances pb WHERE CAST(pb.Date AS DATE) = CAST(@asOnDate AS DATE) AND pb.ConcernID = @concernId", asOnDate.ToString("yyyy-MM-dd"), concernID);

            decimal cashInHand = _customerRepository.ExecSP<decimal>(cashInHandQuery,
                    new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                    new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                ).FirstOrDefault();

            //decimal cashInHand = _customerRepository.SQLQuery<decimal>(cashInHandQuery);
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = cashInHand,
                CreditParticulars = "",
                DebitParticulars = "Cash in Hand"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region ac receivable / customer due
            decimal totalCustomerDue = _customerRepository.ExecSP<decimal>("GetCustomerDueByDate @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();

            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalCustomerDue,
                CreditParticulars = "",
                DebitParticulars = "Account Receiveable"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region inventory / stock value
            decimal totalStockValue = _customerRepository.ExecSP<decimal>("GetStockValue @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();

            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalStockValue,
                CreditParticulars = "",
                DebitParticulars = "Inventory"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region cash at bank / total bank balance
            decimal totalBankValue = _customerRepository.ExecSP<decimal>("GetBankBalance @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();

            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalBankValue,
                CreditParticulars = "",
                DebitParticulars = "Cash at Bank"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region Liabilities received / Loan receivable
            TOAccountRecAndPay recAndPay = _customerRepository.ExecSP<TOAccountRecAndPay>("GetLoanPayableAndReceivable @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();
            decimal totalLoanReceivable = recAndPay != null ? recAndPay.TotalReceivable : 0m;

            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalLoanReceivable,
                CreditParticulars = "",
                DebitParticulars = "Liabilities received"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region Prepaid Expenses
            decimal totalPrepaidExpense = 0m;

            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalPrepaidExpense,
                CreditParticulars = "",
                DebitParticulars = "Prepaid Expenses"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region total current assets
            decimal totalCurrentAsset = cashInHand + totalCustomerDue + totalStockValue + totalBankValue + totalLoanReceivable + totalPrepaidExpense;
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = totalCurrentAsset,
                CreditParticulars = "",
                DebitParticulars = "Total Current Assets"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region fixed assets
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = 0m,
                CreditParticulars = "",
                DebitParticulars = "Fixed Assets"
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region all fixed assets
            string fixedQuery = string.Format(@"SELECT CAST(ISNULL(SUM(si.Amount), 0.00) AS DECIMAL(18,2)) FROM ShareInvestments si
	                            INNER JOIN ShareInvestmentHeads ih ON si.SIHID = ih.SIHID
	                            WHERE si.ConcernID = @concernId AND 
	                            CAST(si.EntryDate AS DATE) <= CAST(@asOnDate AS DATE) AND ih.ParentId = 1 AND si.TransactionType = 1");

            decimal fixedAssets = _customerRepository.ExecSP<decimal>(fixedQuery,
                    new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                    new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }).FirstOrDefault();
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = fixedAssets,
                CreditParticulars = "",
                DebitParticulars = "All Fixed Asset"
            };
            balanceSheetData.Add(particulars);
            #endregion
            #endregion

            #region credit part
            #region Current Liabilities
            particulars = new ProfitLossReportModel
            {
                Credit = 0m,
                Debit = 0m,
                CreditParticulars = "Current Liabilieties",
                DebitParticulars = ""
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region Account Payable / Total Supplieres Due
            decimal totalSupplierDue = _customerRepository.ExecSP<decimal>("GetSupplierDueByDate @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();

            particulars = new ProfitLossReportModel
            {
                Credit = totalSupplierDue,
                Debit = 0m,
                CreditParticulars = "Account Payable",
                DebitParticulars = ""
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region Liabilities Pay / Loan Payable
            decimal totalloanPayable = recAndPay != null ? recAndPay.TotalPayable : 0m;

            particulars = new ProfitLossReportModel
            {
                Credit = totalloanPayable,
                Debit = 0m,
                CreditParticulars = "Liabilities Pay",
                DebitParticulars = ""
            };
            balanceSheetData.Add(particulars);
            #endregion

            #region Total Current Liabilities
            decimal totalCurrentLiabilities = totalSupplierDue + totalloanPayable;

            particulars = new ProfitLossReportModel
            {
                Credit = totalCurrentLiabilities,
                Debit = 0m,
                CreditParticulars = "Total Current Liabilities",
                DebitParticulars = ""
            };
            balanceSheetData.Add(particulars);
            #endregion

            #endregion

            return balanceSheetData;
        }
    }
}
