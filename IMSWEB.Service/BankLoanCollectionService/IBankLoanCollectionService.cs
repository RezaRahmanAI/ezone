using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IBankLoanCollectionService
    {
        void Add(BankLoanCollection model);
        void Update(BankLoanCollection model);
        bool Save();
        IEnumerable<BankLoanCollection> GetAll();
        Task<IEnumerable<BankLoanCollectionTO>> GetAllAsync(DateTime fromDate, DateTime toDate);
        BankLoanCollection GetById(int id);
        int GetLastCollectionIdForCCLoan(int id);
        void Delete(int id);
        bool IsDeleteAllowed(int collectionId);
        BankLoan GetBankLoanByCollectionId(int collectionId);
        bool DelecteBankLoanCollectionUsingSP(int collectionId, EnumBankLoanType collectionType);
        RPTBankLoanCollectionInvTO GetLoanCollectionInvoiceData(int collectionId);
        List<RPTBankDueLoanTO> GetAllPendingLoanAsOnDate(DateTime currentDate);
        bool IsCollectionFoundByCCLoanId(int id);
        Tuple<int, int> GetLoanIdByCollectionId(int id);
    }
}
