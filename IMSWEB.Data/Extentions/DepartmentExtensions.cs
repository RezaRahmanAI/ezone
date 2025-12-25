using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class DepartmentExtensions
    {
        public static async Task<IEnumerable<Department>> GetAllDepartmentAsync(this IBaseRepository<Department> DepartmentRepository)
        {
            return await DepartmentRepository.All.ToListAsync();
        }
    }
}
