using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class EmployeeExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string,
            string, string, DateTime, string,Tuple<int, EnumActiveInactive>>>> GetAllEmployeeAsync(this IBaseRepository<Employee> employeeRepository, IBaseRepository<Designation> designationRepository)
        {
            IQueryable<Designation> designations = designationRepository.All;

            var items = await employeeRepository.All.Join(designations,
                emp => emp.DesignationID, deg => deg.DesignationID, (emp, deg) => new { Employee = emp, Designation = deg }).
                Select(x => new
                {
                    EmployeeId = x.Employee.EmployeeID,
                    EmployeeCode = x.Employee.Code,
                    x.Employee.Name,
                    x.Employee.ContactNo,
                    x.Employee.PhotoPath,
                    x.Employee.JoiningDate,
                    DesignationName = x.Designation.Description,
                    x.Employee.MachineEMPID,
                    x.Employee.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, string, string, DateTime, string,Tuple<int, EnumActiveInactive>>
                (
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.Name,
                    x.ContactNo,
                    x.PhotoPath,
                    Convert.ToDateTime(x.JoiningDate),
                    x.DesignationName,new Tuple<int, EnumActiveInactive>(
                    x.MachineEMPID,
                    x.Status)
                )).ToList();
        }



        public static IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>
     GetAllEmployeeDetails(this IBaseRepository<Employee> employeeRepository
     , IBaseRepository<Designation> designationRepository
     , IBaseRepository<Department> DepartmentRepository
     , IBaseRepository<Grade> GradeRepository

     )
        {
            IQueryable<Designation> designations = designationRepository.All;


            var items = (from emp in employeeRepository.All
                         join dg in designationRepository.All on emp.DesignationID equals dg.DesignationID into ldg
                         from dg in ldg.DefaultIfEmpty()
                         join dep in DepartmentRepository.All on emp.DepartmentID equals dep.DepartmentId into ldept
                         from dep in ldept.DefaultIfEmpty()
                         join g in GradeRepository.All on emp.GradeID equals g.GradeID into lg
                         from g in lg.DefaultIfEmpty()
                       
                         
                         //where dep.DepartmentId==(int)EnumDepartment.Sales
                         where emp.Status == EnumActiveInactive.Active
                         select new
                         {
                             EmployeeId = emp.EmployeeID,
                             EmployeeCode = emp.Code,
                             emp.Name,
                             emp.ContactNo,
                             emp.PhotoPath,
                             emp.JoiningDate,
                             DesignationName = dg == null ? "" : dg.Description,
                             DepartmentName = dep == null ? "" : dep.DESCRIPTION,
                             GradeName = g == null ? "" : g.Description
                         }).ToList();

            return items.Select(x => new Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>
                (
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.Name,
                    x.ContactNo,
                    x.PhotoPath,
                    Convert.ToDateTime(x.JoiningDate),
                    x.DesignationName, 
                    new Tuple<string, string>(x.DepartmentName, x.GradeName)
                )).ToList();
        }



        public static IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>
     GetAllEmployeeDetails(this IBaseRepository<Employee> employeeRepository
     , IBaseRepository<Designation> designationRepository
     , IBaseRepository<Department> DepartmentRepository
     , IBaseRepository<Grade> GradeRepository,
     int DepartmentID
     )
        {
            IQueryable<Designation> designations = designationRepository.All;
            IQueryable<Employee> employees = null;
            if (DepartmentID > 0)
                employees = employeeRepository.All.Where(i => i.DepartmentID == DepartmentID);
            else
                employees = employeeRepository.All;


            var items = (from emp in employees
                         join dg in designationRepository.All on emp.DesignationID equals dg.DesignationID into ldg
                         from dg in ldg.DefaultIfEmpty()
                         join dep in DepartmentRepository.All on emp.DepartmentID equals dep.DepartmentId into ldept
                         from dep in ldept.DefaultIfEmpty()
                         join g in GradeRepository.All on emp.GradeID equals g.GradeID into lg
                         from g in lg.DefaultIfEmpty()
                         where emp.Status == EnumActiveInactive.Active
                         select new
                         {
                             EmployeeId = emp.EmployeeID,
                             EmployeeCode = emp.Code,
                             emp.Name,
                             emp.ContactNo,
                             emp.PhotoPath,
                             emp.JoiningDate,
                             DesignationName = dg == null ? "" : dg.Description,
                             DepartmentName = dep == null ? "" : dep.DESCRIPTION,
                             GradeName = g == null ? "" : g.Description
                         }).ToList();

            return items.Select(x => new Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>
                (
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.Name,
                    x.ContactNo,
                    x.PhotoPath,
                    Convert.ToDateTime(x.JoiningDate),
                    x.DesignationName,
                    new Tuple<string, string>(x.DepartmentName, x.GradeName)
                )).ToList();
        }
    }
}
