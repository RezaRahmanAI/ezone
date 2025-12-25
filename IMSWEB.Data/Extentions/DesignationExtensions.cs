using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class DesignationExtensions
    {
        public static async Task<IEnumerable<Designation>> GetAllDesignationAsync(this IBaseRepository<Designation> designationRepository)
        {
            return await designationRepository.All.ToListAsync();
        }
    }
}
