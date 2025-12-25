using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class DesWiseCommissionExtensions
    {
        public static async Task<IEnumerable<Tuple<int, Decimal, string>>> GetAllGradeAsync(this IBaseRepository<DesWiseCommission> DesWiseCommissionRepository, IBaseRepository<Designation> DesignationRepository)
        {


            var result = await (from dwc in DesWiseCommissionRepository.All
                                join d in DesignationRepository.All on dwc.DesignationID equals d.DesignationID
                                select new
                                {
                                    dwc.ID,
                                    dwc.CommissionPercent,
                                    d.Description
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, Decimal, string>(
                 x.ID,
                 x.CommissionPercent,
                 x.Description
                ));
        }
    }
}
