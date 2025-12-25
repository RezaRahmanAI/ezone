using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IBankLoanService
    {
        void Add(BankLoan model);
        void Update(BankLoan model);
        bool Save();
        IEnumerable<BankLoan> GetAll();
        Task<IEnumerable<BankLoanTO>> GetAllAsync(DateTime fromDate, DateTime toDate);
        BankLoan GetById(int id);
        void Delete(int id);
        List<IdNameDDLTO> GetForDDL();
        List<IdNameDDLTO> GetBankLoanByBankId(int bankId);
    }
}
