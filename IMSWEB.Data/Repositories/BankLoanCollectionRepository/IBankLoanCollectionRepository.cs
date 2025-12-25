using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories
{
    public interface IBankLoanCollectionRepository
    {
        bool DelecteBankLoanCollectionUsingSP(int collectionId, EnumBankLoanType collectionType);
        List<RPTBankDueLoanTO> GetAllPendingLoanAsOnDate(IBaseRepository<BankLoanDetails> _bankLoanDetailsRepository, IBaseRepository<BankLoan> _bankLoanRepository, IBaseRepository<Bank> _bankRepository, DateTime currentDate);
    }
}
