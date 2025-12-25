using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IAccountingService
    {
        IEnumerable<RPTTrialBalance> GetTrialBalance(DateTime fromDate, DateTime toDate, int ConcernID);
        IEnumerable<RPTTrialBalance> GetBalanceSheet(DateTime asOnDate, int ConcernID);

        IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID);

        IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID);
        List<ProfitLossReportModel> BalanceSheetNew(DateTime asOnDate, int concernID);
    }
}
