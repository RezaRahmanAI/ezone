using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CreditInterestPercentageExtensions
    {
        public static async Task<IEnumerable<CreditInterestPercentage>> GetAllCreditInterestPercentageAsync(this IBaseRepository<CreditInterestPercentage> CreditInterestPercentageRepository)
        {
            return await CreditInterestPercentageRepository.All.ToListAsync();
        }
    }
}
