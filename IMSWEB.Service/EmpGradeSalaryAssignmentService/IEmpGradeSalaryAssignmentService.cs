using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmpGradeSalaryAssignmentService
    {
        void Add(EmpGradeSalaryAssignment EmpGradeSalaryAssignment);
        void Update(EmpGradeSalaryAssignment EmpGradeSalaryAssignment);
        void Save();
        IEnumerable<EmpGradeSalaryAssignment> GetAll();
        //Task<IEnumerable<EmpGradeSalaryAssignment>> GetAllAsync();
        EmpGradeSalaryAssignment GetById(int id);
        EmpGradeSalaryAssignment GetLastGradeSalaryByEmployeeID(int EmployeeID);
        void Delete(int id);
        IEnumerable<Tuple<int, string, string, decimal?, decimal, string>> GetAllByEmployeeID(int EmployeeID);
       

    }
}
