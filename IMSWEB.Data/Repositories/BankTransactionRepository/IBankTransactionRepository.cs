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
    public interface IBankTransactionRepository
    {

        List<BankLedgerModel> BankLedgerUsingSP(DateTime fromdate, DateTime todate, int ConcernID, int BankID);
        List<BankTransReportModel> BankTransactionsReport(DateTime fromdate, DateTime todate, int BankID, int concernId);
        
    }
}
