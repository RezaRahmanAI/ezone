using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class GradeSalaryAssignmentExtensions
    {
        public static EmpGradeSalaryAssignment GetLastGradeSalaryByEmployeeID(this IBaseRepository<EmpGradeSalaryAssignment> EMPGSRepository,int EmployeeID)
        {
            return EMPGSRepository.All.Where(i => i.EmployeeID == EmployeeID && i.Status == (int)EnumActiveInactive.Active).OrderByDescending(i => i.EmpGradeSalaryID).FirstOrDefault();
        }
    }
}
