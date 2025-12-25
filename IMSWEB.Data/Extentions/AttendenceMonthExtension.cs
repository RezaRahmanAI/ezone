using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class AttendenceMonthExtension
    {
        public static async Task<List<Attendence>> GetAttendencByAttenMonthIDAsync(
                        this IBaseRepository<AttendenceMonth> AttendenceMonthRepository,
                        IBaseRepository<Attendence> AttendencReposiotry,
                        int AttenMonthID
            )
        {
            return await AttendencReposiotry.All.Where(i => i.AttenMonthID == AttenMonthID).ToListAsync();
        }

        public static IQueryable<Attendence> GetAllEntriesByAccountNo(this IBaseRepository<Attendence> attendenceRepository,
            IBaseRepository<AttendenceMonth> attendenceMonthRepository, int EmployeeID, DateTime SalaryProcessMonth)
        {
            var date = ConstantData.GetFirstAndLastDateOfMonth(SalaryProcessMonth);

            var attend = from am in attendenceMonthRepository.All
                   join a in attendenceRepository.All on am.AttenMonthID equals a.AttenMonthID
                   where (am.AttendencMonth >= date.Item1 && am.AttendencMonth <= date.Item2) && a.AccountNo == EmployeeID
                   select a;
            return attend;
        }
    }
}
