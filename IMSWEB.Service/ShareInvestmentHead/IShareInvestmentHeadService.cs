using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IShareInvestmentHeadService
    {
        void Add(ShareInvestmentHead ShareInvestmentHead);
        void Update(ShareInvestmentHead ShareInvestmentHead);

        //void Delete(ShareInvestmentHead ShareInvestmentHead);
        void Save();
        IQueryable<ShareInvestmentHead> GetAll();
        Task<IEnumerable<Tuple<int, string, string, string>>> GetAllAsync();
        IEnumerable<Tuple<int, string, string, string>> GetListByID(int ID);
        ShareInvestmentHead GetById(int id);
        void Delete(int id);
        bool IsChildExist(int SIHID);
        bool IsTransactionFound(int id, int concernId);
    }
}
