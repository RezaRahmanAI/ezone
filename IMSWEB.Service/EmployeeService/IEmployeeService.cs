using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmployeeService
    {
        void AddEmployee(Employee Employee);
        void UpdateEmployee(Employee Employee);
        void SaveEmployee();
        IEnumerable<Employee> GetAllEmployee();
        IQueryable<Employee> GetAllEmployeeIQueryable();

        Task<IEnumerable<Tuple<int, string, string,
            string, string, DateTime, string,Tuple<int,EnumActiveInactive>>>> GetAllEmployeeAsync();
        Employee GetEmployeeById(int id);
        void DeleteEmployee(int id);
        IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>> GetAllEmployeeDetails();
        IQueryable<Employee> GetAllEmployeeIQueryable(int ConcernID);

        IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>
            GetAllEmployeeDetails(int DepartmentID = 0);

        string GetEmpNameById(int employeeId);
    }
}
