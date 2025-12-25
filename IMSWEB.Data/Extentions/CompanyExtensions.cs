using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CompanyExtensions
    {
        public static async Task<IEnumerable<Company>> GetAllCompanyAsync(this IBaseRepository<Company> companyRepository)
        {
            return await companyRepository.All.ToListAsync();
        }
    }
}
