using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SystemInformationExtensions
    {
        public static async Task<IEnumerable<SystemInformation>> GetAllSystemInformationAsync(this IBaseRepository<SystemInformation> systemInformationRepository)
        {
            return await systemInformationRepository.All.ToListAsync();
        }
    }
}
