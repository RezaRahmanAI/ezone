using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IBankService
    {
        void AddBank(Bank Bank);
        void UpdateBank(Bank Bank);
        void SaveBank();
        IEnumerable<Bank> GetAllBank();
        IQueryable<Bank> GetAll(int ConcernID);
        Task<IEnumerable<Bank>> GetAllBankAsync();
        Task<IEnumerable<Bank>> GetAllBankByParentConcernAsync(int concernId);
        Bank GetBankById(int id);
        void DeleteBank(int id);
        IEnumerable<IdNameDDLTO> GetAllBankForDDL();
    }
}
