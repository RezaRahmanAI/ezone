using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class AllowanceDeductionExtension
    {
        public static async Task<IEnumerable<AllowanceDeduction>> GetAllAllowanceAsync(this IBaseRepository<AllowanceDeduction> AllowanceDeductionRepository)
        {
            return await AllowanceDeductionRepository.All.Where(i=>i.AllowORDeduct==(int)EnumAllowOrDeduct.Allowance).ToListAsync();
        }
        public static async Task<IEnumerable<AllowanceDeduction>> GetAllDeductionAsync(this IBaseRepository<AllowanceDeduction> AllowanceDeductionRepository)
        {
            return await AllowanceDeductionRepository.All.Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Deduction).ToListAsync();
        }
    }
}
