using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMSWEB.Data
{
    public static class GradeExtension
    {
        public static async Task<IEnumerable<Grade>> GetAllGradeAsync(this IBaseRepository<Grade> GradeRepository)
        {
            return await GradeRepository.All.ToListAsync();
        }
    }
}
