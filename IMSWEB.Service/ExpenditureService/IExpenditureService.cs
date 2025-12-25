using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IExpenditureService
    {
        void AddExpenditure(Expenditure expenditure);
        void UpdateExpenditure(Expenditure expenditure);
        void SaveExpenditure();
        Task<IEnumerable<Expenditure>> GetAllExpenditureByUserIDAsync(int UserID, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate);
        IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<int>>> GetforExpenditureReport(DateTime fromDate, DateTime toDate, int concernId,EnumCompanyTransaction Status,
            int ExpenseItemID, bool isAdminReport);

        Expenditure GetExpenditureById(int id);
        void DeleteExpenditure(int id);
        Task<IEnumerable<Expenditure>> GetAllIncomeAsync( DateTime fromDate, DateTime toDate);
        decimal GetExpenditureAmountByUserID(int UserID, DateTime fromDate, DateTime toDate);

        Task<IEnumerable<Expenditure>> GetAllIncomeAsync(DateTime fromDate, DateTime toDate, List<EnumWFStatus> enumWFStatus);

        Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate, List<EnumWFStatus> enumWFStatus);


    }
}
