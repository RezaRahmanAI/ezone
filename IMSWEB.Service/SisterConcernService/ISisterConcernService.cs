using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Model.TO.Bkash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISisterConcernService
    {
        void AddSisterConcern(SisterConcern SisterConcern);
        void UpdateSisterConcern(SisterConcern SisterConcern);
        void SaveSisterConcern();
        IEnumerable<SisterConcern> GetAllSisterConcern();
        Task<IEnumerable<SisterConcern>> GetAllSisterConcernAsync();
        SisterConcern GetSisterConcernById(int id);
        void DeleteSisterConcern(int id);
        IEnumerable<SisterConcern> GetAll();
        IEnumerable<SisterConcern> GetToAll();
        List<SisterConcern> GetFamilyTree(int ConcernID);
        List<IdNameList> GetFamilyTreeDDL(int ConcernID);
        List<SisterConcern> GetFamilyTreeForNotLoggedInUser(int ConcernID);
        bool IsChildConcern(int concernId);
        List<SisterConcerPayTO> GetAllConcernByConcernId(int concernId);
        string GetConcernNameById(int concernId);
        int GetParentConcernId(int concernId);
    }
}
