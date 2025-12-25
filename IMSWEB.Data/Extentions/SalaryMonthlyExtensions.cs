


using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SalaryMonthlyExtensions
    {
        public static async Task<IEnumerable<SalaryMonthly>> GetAllSalaryMonthlyAsync(this IBaseRepository<SalaryMonthly> salaryMonthlyRepository)
        {
            return await salaryMonthlyRepository.All.ToListAsync();
        }
    }
}

