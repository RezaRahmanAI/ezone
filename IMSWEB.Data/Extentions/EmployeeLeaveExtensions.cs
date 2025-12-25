using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class EmployeeLeaveExtensions
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>>> GetAllAsync(this IBaseRepository<EmployeeLeave> EmployeeLeaveRepository,
            IBaseRepository<Department> DepartmentRepository, IBaseRepository<Designation> DesignationRepository,
            IBaseRepository<Grade> GradeRepository,
            IBaseRepository<Employee> EmployeeRepository, DateTime Fromdate, DateTime toDate)
        {
            var result = await (from el in EmployeeLeaveRepository.All.Where(i => i.LeaveDate >= Fromdate && i.LeaveDate <= toDate)
                                join emp in EmployeeRepository.All on el.EmployeeID equals emp.EmployeeID
                                join dep in DepartmentRepository.All on emp.DepartmentID equals dep.DepartmentId into ldep
                                from d in ldep.DefaultIfEmpty()
                                join desg in DesignationRepository.All on emp.DesignationID equals desg.DesignationID into ldesg
                                from desg in ldesg.DefaultIfEmpty()
                                join grad in GradeRepository.All on emp.GradeID equals grad.GradeID into lgrad
                                from g in lgrad.DefaultIfEmpty()
                                //where el.LeaveDate >= Fromdate && el.LeaveDate >= toDate
                                select new
                                {
                                    EmployeeLeaveID = el.EmployeeLeaveID,
                                    LeaveDate = el.LeaveDate,
                                    LeaveType = ((EnumEmployeeLeaveType)el.LeaveType).ToString(),
                                    Description = el.Description,
                                    PaidLeave = el.PaidLeave == 1 ? true : false,
                                    Status = ((EnumEmployeeLeaveStatus)el.Status).ToString(),
                                    el.ShortLeaveHour,
                                    EmplyeeName = emp.Name,
                                    DepartmentName = d == null ? "" : d.DESCRIPTION,
                                    DesignationName = desg == null ? "" : desg.Description,
                                    GradeName = g == null ? "" : g.Description
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>
            (
               x.EmployeeLeaveID,
               x.LeaveDate,
               x.LeaveType,
               x.Description,
               x.PaidLeave,
               x.Status,
               new Tuple<decimal, string, string, string, string>(
               x.ShortLeaveHour,
               x.EmplyeeName,
               x.DepartmentName,
               x.DesignationName,
               x.GradeName
               )

            ));
        }
    }
}
