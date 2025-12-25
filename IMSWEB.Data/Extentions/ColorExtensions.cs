using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ColorExtensions
    {
        public static async Task<IEnumerable<Color>> GetAllColorAsync(this IBaseRepository<Color> colorRepository)
        {
            return await colorRepository.All.ToListAsync();
        }
    }
}
