using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class UserExtensions
    {
        public static async Task<IEnumerable<ApplicationUser>> GetAllUserAsync(this IBaseRepository<ApplicationUser> userRepository)
        {
            return await userRepository.All.ToListAsync();
        }
    }
}
