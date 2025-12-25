using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IShareInvestmentService
    {
        void Add(ShareInvestment ShareInvestment);
        void Update(ShareInvestment ShareInvestment);
        void Save();
        IQueryable<ShareInvestment> GetAll();
        Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>> GetAllAsync(EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType);

        Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>> GetAllLiabilityAsync(EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType);
        ShareInvestment GetById(int id);
        void Delete(int id);


        void DeleteInvm(ShareInvestment ShareInvestment, int id);


        List<LiabilityReportModel> TotalLiabilityPayRec(DateTime asOnDate, int concernID);
        List<VoucherTransactionReportModel> VoucherTransactionLedgerData(DateTime fromDate,
        DateTime toDate, int ConcernID, int ExpenseItemID, string headType);
    }
}
