using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
  public  interface IAdvanceSalaryService
    {
        void Add(AdvanceSalary AdvanceSalary);
        void Update(AdvanceSalary AdvanceSalary);
        void Save();
        IEnumerable<AdvanceSalary> GetAll();
        Task<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string, EnumSalaryType>>>> GetAllAsync(DateTime fromDate, DateTime toDate);
        AdvanceSalary GetById(int id);
        void Delete(int id);
        AdvanceSalary GetByEmpId(int id);
        IEnumerable<AdvanceSalaryReport> GetAdvanceSalaryReports(DateTime fromDate, DateTime toDate, int EmployeeID);
        Task<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>>> GetAllDueSalaryAsync(DateTime fromDate, DateTime toDate);

        IQueryable<AdvanceSalary> GetAdvanceSalariesQueryable();
    }
}
