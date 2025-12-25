using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class EmpGradeSalaryAssignmentExtension
    {
        public static IEnumerable<Tuple<int, string, string, decimal?, decimal, string>> GetAllByEmployeeID(this IBaseRepository<EmpGradeSalaryAssignment> EmpGradeSalaryAssignmentRepository,
            IBaseRepository<Grade> GradeRepository, IBaseRepository<GradeSalaryChangeType> GradeSalaryChangeTypeRepository, int EmployeeID)
        {
            var resutl = (from empgs in EmpGradeSalaryAssignmentRepository.All.Where(i => i.EmployeeID == EmployeeID && i.Status == (int)EnumActiveInactive.Active)
                          join g in GradeRepository.All on empgs.GradeID equals g.GradeID
                          join ct in GradeSalaryChangeTypeRepository.All on empgs.Type equals ct.GradeSalaryChangeTypeID
                          select new
                          {
                              empgs.EmpGradeSalaryID,
                              ChangeType = ct.Name,
                              Grade = g.Description,
                              empgs.GrossSalary,
                              empgs.BasicSalary,
                              EffectDate = empgs.EffectDate ,
                              empgs.TillDate
                          }).ToList();

            return resutl.Select(i => new Tuple<int, string, string, decimal?, decimal, string>(
                      i.EmpGradeSalaryID,
                      i.ChangeType,
                      i.Grade,
                      i.GrossSalary,
                      i.BasicSalary,
                      (i.EffectDate.ToString("dd/MM/yyyy") + " To " + i.TillDate == null ? "OnWards" : Convert.ToDateTime(i.TillDate).ToString("dd/MM/yyyy"))
                ));
        }

        
    }
}
