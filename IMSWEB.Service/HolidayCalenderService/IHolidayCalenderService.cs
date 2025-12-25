using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IHolidayCalenderService
    {
        void Add(HolidayCalender HolidayCalender);
        void Update(HolidayCalender HolidayCalender);
        void Save();
        IQueryable<HolidayCalender> GetAllIQueryable();
        Task<IEnumerable<HolidayCalender>> GetAllAsync();
        HolidayCalender GetById(int id);
        void Delete(int id);
    }
}
