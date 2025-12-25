using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IPrevBalanceService
    {
        void AddPrevBalance(PrevBalance prevBalance);
        void UpdatePrevBalance(PrevBalance prevBalance);
        void Save();
        IEnumerable<PrevBalance> GetAllPrevBalance();
        //Task<IEnumerable<PrevBalance>> GetAllPrevBalancelAsync();
        PrevBalance GetPrevBalanceById(int id);
        void DeletePrevBalance(int id);
        List<PrevBalance> DailyBalanceProcess(int ConcernID);
    }
}
