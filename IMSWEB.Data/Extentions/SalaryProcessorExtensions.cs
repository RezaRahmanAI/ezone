using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SalaryProcessorExtensions
    {
        public static IEnumerable<Tuple<int, DateTime, int>> GetUndoAbleSalaryProcess(this IBaseRepository<SalaryProcess> SalaryProcessRepository)
        {
            var result = (from sp in SalaryProcessRepository.All
                         where sp.IsFinalized == 0 
                         select new
                         {
                             sp.SalaryProcessID,
                             sp.SalaryMonth,
                             TotalEmployee = sp.SalaryMonthlys.Count()
                         }).ToList();

            return result.Select(x => new Tuple<int, DateTime, int>(
                         x.SalaryProcessID,
                         x.SalaryMonth,
                         x.TotalEmployee
                ));
        }
    }
}
