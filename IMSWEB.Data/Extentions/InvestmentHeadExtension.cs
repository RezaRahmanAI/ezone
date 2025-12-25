using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class InvestmentHeadExtension
    {
        public static async Task<IEnumerable<Tuple<int, string, string, string>>> GetAllAsync(this IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository)
        {
            var Result = await (from si in ShareInvestmentHeadRepository.All
                                select new
                                {
                                    si.SIHID,
                                    si.Code,
                                    si.Name,
                                    si.ParentId,
                                    ParentName =((EnumInvestmentType)si.ParentId).ToString()
                                }).Where(i => i.ParentId != 0).OrderBy(i => i.Code).ToListAsync();

            return Result.Select(x => new Tuple<int, string, string, string>(
                        x.SIHID,
                        x.Code,
                        x.Name,
                        x.ParentName
                          ));
        }
        public static IEnumerable<Tuple<int, string, string, string>> GetByID(this IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository, int SIHID)
        {
            var Result = (from ch in ShareInvestmentHeadRepository.All
                          where ch.ParentId == SIHID
                          select new
                          {
                              ch.SIHID,
                              ch.Code,
                              ch.Name,
                              ParentName = ((EnumInvestmentType)ch.ParentId).ToString()
                          }).OrderBy(i => i.Code).ToList();

            return Result.Select(x => new Tuple<int, string, string, string>(
                        x.SIHID,
                        x.Code,
                        x.Name,
                        x.ParentName
                          ));
        }
    }
}
