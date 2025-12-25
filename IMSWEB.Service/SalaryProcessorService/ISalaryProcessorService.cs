using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISalaryProcessorService
    {
        SalaryMonthly MonthlySalaryProcessByEmployeeID(Employee oEmployee, bool IsFestBonusApply, EmpGradeSalaryAssignment EMPGradeSalary,
            SystemInformation sysInfo, TargetSetup EmployeeTarget, bool IsShowroomCommission, decimal TotalCollectionAmount,
            decimal VoltageStabilizerComm, decimal ExtraCommission, decimal DSGNCommissionPercent);
        IQueryable<SalaryProcess> GetAllIQueryable();
        void Add(SalaryProcess SalaryProcess);
        void Update(SalaryProcess SalaryProcess);
        void Save();
        SalaryProcess GetById(int id);
        void Delete(int id);
        IEnumerable<Tuple<int, DateTime, int>> GetUndoAbleSalaryProcess();
        bool UndoSalaryProcessUsingSP(int SalaryProcessID);
        bool FinalizeSalaryMonthUsingSP(DateTime fromDate, DateTime toDate, int ConcernID, int FinalizedBy, DateTime MonthEndDate, DateTime NextMonth, DataTable dtWeeklyHolidays);

        bool IsSalaryProcessed(int EmployeeID, Tuple<DateTime, DateTime> salayMonth);
    }
}
