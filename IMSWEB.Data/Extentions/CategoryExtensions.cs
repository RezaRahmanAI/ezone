using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CategoryExtensions
    {
        public static async Task<IEnumerable<Category>> GetAllCategoryAsync(this IBaseRepository<Category> categoryRepository)
        {
            return await categoryRepository.All.ToListAsync();
        }
    }
}
