using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class HolidayCalenderExtensions
    {
        public static async Task<IEnumerable<HolidayCalender>> GetAllAsync(this IBaseRepository<HolidayCalender> HolidayCalenderRepository)
        {
            return await HolidayCalenderRepository.All.ToListAsync();
        }
        public static IQueryable<HolidayCalender> GetAllHolidaysBySalaryProcessMonth(this IBaseRepository<HolidayCalender> HolidayCalenderRepository, DateTime NextPaySalaryProcessDate)
        {
            var date = ConstantData.GetFirstAndLastDateOfMonth(NextPaySalaryProcessDate);
            return HolidayCalenderRepository.All.Where(i => i.Date >= date.Item1 && i.Date <= date.Item2);
        }
        public static IQueryable<HolidayCalender> GetSpecialHolidaysBySalaryProcessMonth(this IBaseRepository<HolidayCalender> HolidayCalenderRepository, DateTime NextPaySalaryProcessDate)
        {
            var date = ConstantData.GetFirstAndLastDateOfMonth(NextPaySalaryProcessDate);
            return HolidayCalenderRepository.All.Where(i => (i.Date >= date.Item1 && i.Date <= date.Item2) && i.Type == (int)EnumHolidayType.SpecialHoliday);
        }
    }
}
