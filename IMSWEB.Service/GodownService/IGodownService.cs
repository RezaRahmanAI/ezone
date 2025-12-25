using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IGodownService
    {
        void AddGodown(Godown godown);
        void UpdateGodown(Godown godown);
        void SaveGodown();
        IEnumerable<Godown> GetAllGodown();
        IEnumerable<Godown> GetAllConcernGodown();
        IEnumerable<Godown> GetAllGodownByConcern(int ConcernID);
        Task<IEnumerable<Godown>> GetAllGodownAsync();
        Godown GetGodownById(int id);
        Godown GetGodownByName(string name);

        Task<IEnumerable<Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>>> GetAllTransferHistoryAsync();
        
        void DeleteGodown(int id);
    }
}
