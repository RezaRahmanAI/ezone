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
    public interface IAccountingRepository
    {
        IEnumerable<RPTTrialBalance> GetTrialBalance(DateTime fromDate, DateTime toDate,int ConcernID);
        IEnumerable<RPTTrialBalance> GetBalanceSheet(DateTime asOnDate, int ConcernID);

        IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID);

        IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID);
    }
}
