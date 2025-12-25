using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IBankLoanDetailsService
    {
        void Add(BankLoanDetails model);
        void Update(BankLoanDetails model);
        bool Save();
        IEnumerable<BankLoanDetails> GetAll();
        BankLoanDetails GetById(int id);
        void Delete(int id);
        List<BankLoanDetails> GetAllLoanDetailsByBankLoanId(int bankId);
        void DeleteByBankLoanId(int bankLoanId);
        void AddMultiple(List<BankLoanDetails> list);
        Tuple<decimal, decimal> GetFirstPendingLoanAmountByBankLoanId(int bankLoanId);
        List<BankLoanDetails> GetAllDueLoanByCurrentDate(DateTime currentDate);
        List<BankLoanPenaltyDetails> GetAllPenaltyByLoanDetails(int loanDetailsId);
        BankLoanPenaltyDetails GetLastPenaltyByLoanDetails(int loanDetailsId);
        void AddPenalty(BankLoanPenaltyDetails model);
        List<RPTBankLoanDetailsTO> GetAllLoanDetails(DateTime fromDate, DateTime toDate, int concernId);
    }
}
