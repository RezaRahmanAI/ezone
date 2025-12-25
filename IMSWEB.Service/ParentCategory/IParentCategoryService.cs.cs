using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IParentCategoryService
    {
        void Add(ParentCategory ParentCategory);
        void Update(ParentCategory ParentCategory);
        void Save();
        IQueryable<ParentCategory> GetAll();
        //Task<IEnumerable<ParentCategory>> GetAllAsync();
        ParentCategory GetById(int id);
        void Delete(int id);

        IEnumerable<ParentCategory> GetParentCategories();
    }
}
