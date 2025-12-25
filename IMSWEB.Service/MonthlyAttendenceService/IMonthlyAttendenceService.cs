using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IMonthlyAttendenceService
    {
        void Add(MonthlyAttendence MonthlyAttendence);
        void Update(MonthlyAttendence MonthlyAttendence);
        void Save();
        IQueryable<MonthlyAttendence> GetAll();
        MonthlyAttendence GetById(int id);
        void Delete(int id);
    }
}
