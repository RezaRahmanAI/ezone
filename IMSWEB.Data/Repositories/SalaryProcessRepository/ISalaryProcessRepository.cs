using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public interface ISalaryProcessRepository
    {
        bool UndoSalaryProcessUsingSP(int SalaryProcessID);
        bool FinalizeSalaryMonthUsingSP(DateTime fromDate, DateTime toDate, int ConcernID, int FinalizedBy, DateTime MonthEndDate, DateTime NextMonth, DataTable dtWeeklyHolidays);
    }
}
