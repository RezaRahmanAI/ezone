using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class AdvanceSalaryExtensions
    {
        public static async Task<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string, EnumSalaryType>>>> GetAllAsync(this IBaseRepository<AdvanceSalary> AdvanceRepository,
            IBaseRepository<Department> DepartmentRepository
            , IBaseRepository<Designation> DesignationRepository
            , IBaseRepository<Grade> GradeRepository
            , IBaseRepository<Employee> EmployeeRepository, DateTime fromDate, DateTime toDate
            )
        {
            var result = await (from av in AdvanceRepository.All.Where(i => i.Date >= fromDate && i.Date <= toDate)
                                join emp in EmployeeRepository.All on av.EmployeeID equals emp.EmployeeID
                                join d in DepartmentRepository.All on emp.DepartmentID equals d.DepartmentId into ld
                                from d in ld.DefaultIfEmpty()
                                join desg in DesignationRepository.All on emp.DesignationID equals desg.DesignationID into ldeg
                                from desg in ldeg.DefaultIfEmpty()
                                join g in GradeRepository.All on emp.GradeID equals g.GradeID into lg
                                from g in lg.DefaultIfEmpty()
                                select new
                              {
                                  av.ID,
                                  av.EmployeeID,
                                  emp.Code,
                                  emp.Name,
                                  DESCRIPTION = d == null ? "" : d.DESCRIPTION,
                                  DesignationName = desg == null ? "" : desg.Description,
                                  GradName = g == null ? "" : g.Description,

                                  av.Amount,
                                  av.Date,
                                  av.Remarks,
                                  av.SalaryType
                              }).OrderByDescending(i => i.Date).ToListAsync();

            return result.Select(x => new Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string, EnumSalaryType>>
                (
                   x.ID,
                   x.EmployeeID,
                   x.Code,
                   x.Name,
                   x.DESCRIPTION,
                   x.DesignationName,
                   x.GradName, new Tuple<decimal, DateTime, string, EnumSalaryType>(x.Amount, x.Date, x.Remarks, x.SalaryType)
                ));

        }


        public static IEnumerable<AdvanceSalaryReport> GetAdvanceSalaryReportsByEmployeeID(this IBaseRepository<AdvanceSalary> advanceSalaryRepository, IBaseRepository<EmpGradeSalaryAssignment> empGradesalaryRepository,
            IBaseRepository<Employee> employeeRepository, DateTime fromDate, DateTime toDate, int EmployeeID)
        {
            var data = (from A in advanceSalaryRepository.All
                        join E in employeeRepository.All on A.EmployeeID equals E.EmployeeID
                        join emp in empGradesalaryRepository.All on E.EmployeeID equals emp.EmployeeID
                        where (A.Date >= fromDate && A.Date <= toDate && (A.SalaryType == EnumSalaryType.AdvanceSalary && A.IsAdvanceLoanPay == 0) || A.SalaryType == EnumSalaryType.AdvanceLoan)

                        select new AdvanceSalaryReport
                        {
                            ID = E.EmployeeID,
                            Name = E.Name,
                            Code = E.Code,
                            BasicSalary = emp.GrossSalary.Value,
                            AdvanceAmt = A.Amount,
                            Date = A.Date,
                            Remarks = A.Remarks
                            

                        }).ToList();

            return data;
        }
        public static async Task<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>>> GetAllDueSalaryAsync(this IBaseRepository<AdvanceSalary> AdvanceRepository,
          IBaseRepository<Department> DepartmentRepository
          , IBaseRepository<Designation> DesignationRepository
          , IBaseRepository<Grade> GradeRepository
          , IBaseRepository<Employee> EmployeeRepository, DateTime fromDate, DateTime toDate
          )
        {
            var result = await (from av in AdvanceRepository.All.Where(i => i.Date >= fromDate && i.Date <= toDate && i.SalaryType == EnumSalaryType.DueSalary)
                                join emp in EmployeeRepository.All on av.EmployeeID equals emp.EmployeeID
                                join d in DepartmentRepository.All on emp.DepartmentID equals d.DepartmentId into ld
                                from d in ld.DefaultIfEmpty()
                                join desg in DesignationRepository.All on emp.DesignationID equals desg.DesignationID into ldeg
                                from desg in ldeg.DefaultIfEmpty()
                                join g in GradeRepository.All on emp.GradeID equals g.GradeID into lg
                                from g in lg.DefaultIfEmpty()
                                select new
                                {
                                    av.ID,
                                    av.EmployeeID,
                                    emp.Code,
                                    emp.Name,
                                    DESCRIPTION = d == null ? "" : d.DESCRIPTION,
                                    DesignationName = desg == null ? "" : desg.Description,
                                    GradName = g == null ? "" : g.Description,

                                    av.Amount,
                                    av.Date,
                                    av.Remarks
                                }).OrderByDescending(i => i.Date).ToListAsync();

            return result.Select(x => new Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>
                (
                   x.ID,
                   x.EmployeeID,
                   x.Code,
                   x.Name,
                   x.DESCRIPTION,
                   x.DesignationName,
                   x.GradName, new Tuple<decimal, DateTime, string>(x.Amount, x.Date, x.Remarks)
                ));

        }
    }
}
