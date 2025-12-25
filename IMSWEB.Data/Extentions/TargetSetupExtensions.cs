using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class TargetSetupExtensions
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>>> GetAllAsync(this IBaseRepository<TargetSetup> TargetSetupRepository,
            IBaseRepository<Employee> EmployeeReposiotry, IBaseRepository<Designation> DesignationRepository, IBaseRepository<Department> DepartmentRepository)
        {
            var result = await (from ts in TargetSetupRepository.All
                                join emp in EmployeeReposiotry.All on ts.EmployeeID equals emp.EmployeeID
                                join des in DesignationRepository.All on emp.DesignationID equals des.DesignationID into ldes
                                from des in ldes.DefaultIfEmpty()
                                join dept in DepartmentRepository.All on emp.DepartmentID equals dept.DepartmentId into ldept
                                from dept in ldept.DefaultIfEmpty()
                                select new
                                {
                                    ts.TID,
                                    ts.TargetMonth,
                                    ts.Status,
                                    ts.Quantity,
                                    ts.Amount,
                                    EmployeeName = emp.Name,
                                    DesignationName = des == null ? "" : des.Description,
                                    DepartmentName = dept == null ? "" : dept.DESCRIPTION
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>(
                          x.TID,
                          x.TargetMonth,
                          x.Status,
                          x.Quantity,
                          x.Amount,
                          x.EmployeeName,
                          x.DesignationName, new Tuple<string>(x.DepartmentName)
                )).OrderByDescending(i => i.Item2);
        }
    }
}
