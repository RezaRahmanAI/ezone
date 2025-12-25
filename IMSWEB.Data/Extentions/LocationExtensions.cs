using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class LocationExtensions
    {
        public static async Task<IEnumerable<Location>> GetAllLocationAsync(this IBaseRepository<Location> locationRepository)
        {
            return await locationRepository.All.ToListAsync();
        }
    }
}
