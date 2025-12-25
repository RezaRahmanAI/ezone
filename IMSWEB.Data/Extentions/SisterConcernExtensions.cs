using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SisterConcernExtensions
    {
        public static async Task<IEnumerable<SisterConcern>> GetAllSisterConcernAsync(this IBaseRepository<SisterConcern> sisterConcernRepository)
        {
            return await sisterConcernRepository.All.ToListAsync();
        }
        public static IQueryable<SisterConcern> GetFamilyTree(this IBaseRepository<SisterConcern> sisterConcernRepository, int ConcernID)
        {
            var concern = sisterConcernRepository.GetAll().FirstOrDefault(i => i.ConcernID == ConcernID);
            IQueryable<SisterConcern> children = null;
            IQueryable<SisterConcern> Parent = null;
            if (concern.ParentID > 0)
            {
                children = sisterConcernRepository.GetAll().Where(i => i.ParentID == concern.ParentID);
                Parent = sisterConcernRepository.GetAll().Where(i => i.ConcernID == concern.ParentID);
                children = children.Concat(Parent);
            }
            else
            {
                children = sisterConcernRepository.GetAll().Where(i => i.ParentID == concern.ConcernID);
                Parent = sisterConcernRepository.GetAll().Where(i => i.ConcernID == concern.ConcernID);
                children = children.Concat(Parent);
            }
            return children;
        }

        public static IQueryable<SisterConcern> GetFamilyTreeForNotLoggedInUser(this IBaseRepository<SisterConcern> sisterConcernRepository, int ConcernID)
        {
            var concern = sisterConcernRepository.GetAllByConcern(ConcernID).FirstOrDefault(i => i.ConcernID == ConcernID);
            IQueryable<SisterConcern> children = null;
            IQueryable<SisterConcern> Parent = null;
            if (concern.ParentID > 0)
            {
                children = sisterConcernRepository.GetAllByConcern(ConcernID).Where(i => i.ParentID == concern.ParentID);
                Parent = sisterConcernRepository.GetAllByConcern(ConcernID).Where(i => i.ConcernID == concern.ParentID);
                children = children.Concat(Parent);
            }
            else
            {
                children = sisterConcernRepository.GetAllByConcern(ConcernID).Where(i => i.ParentID == concern.ConcernID);
                Parent = sisterConcernRepository.GetAllByConcern(ConcernID).Where(i => i.ConcernID == concern.ConcernID);
                children = children.Concat(Parent);
            }
            return children;
        }

    }
}
