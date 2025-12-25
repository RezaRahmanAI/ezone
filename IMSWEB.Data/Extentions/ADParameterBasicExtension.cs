using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ADParameterBasicExtension
    {
        public static IEnumerable<Tuple<int, List<string>, string, string, string, string>> GetAllowancesDeductionRuleSetupAsPerGrade(this IBaseRepository<ADParameterBasic> ADParameterBasicRepository, IBaseRepository<ADParameterGrade> ADParameterGradeRepository, IBaseRepository<ADParameterEmployee> ADParameterEmployeeRepository,
            IBaseRepository<AllowanceDeduction> AllowanceDeductionRepository, IBaseRepository<Grade> GradeRepository, int AllowORDeduct)
        {
            var ADParamBasics = ADParameterBasicRepository.All;
            var ADParamGrades = ADParameterGradeRepository.All;
            var ADParamEmployees = ADParameterEmployeeRepository.All;
            var Grades = GradeRepository.All;
            var Allowances = AllowanceDeductionRepository.All.Where(i => i.AllowORDeduct == AllowORDeduct && i.Status == (int)EnumActiveInactive.Active);
            var data = (from basic in ADParamBasics
                        join adg in ADParamGrades on basic.ADParameterID equals adg.ADParameterID
                        //join emp in ADParamEmployees on basic.ADParameterID equals emp.ADParameterID
                        join g in Grades on adg.GradeID equals g.GradeID
                        join allow in Allowances on basic.AllowDeductID equals allow.AllowDeductID
                        where basic.Status == (int)EnumActiveInactive.Active && g.Status == (int)EnumActiveInactive.Active
                        select new
                        {
                            basic.ADParameterID,
                            adg.GradeID,
                            GradeName = g.Description,
                            EntitleType = (EnumEntitleType)basic.EntitleType,
                            Periodicity = (EnumPeriodicity)basic.Periodicity,
                            allow.AllowDeductID,
                            AllowanceName = allow.Name,
                            IsTexable = basic.IsTaxable
                        }).ToList();

            var data2 = from d in data
                        group d by new { d.ADParameterID,d.EntitleType,d.Periodicity,d.AllowanceName,d.IsTexable } into g
                        select new
                        {
                            ADParameterID = g.Key.ADParameterID,
                            g.Key.EntitleType,
                            g.Key.Periodicity,
                            g.Key.AllowanceName,
                            g.Key.IsTexable,
                            GradeName = g.Select(i => i.GradeName).ToList()
                        };

            var result = data2.Select(x => new Tuple<int,List<string>, string, string, string, string>(
                x.ADParameterID,
                x.GradeName,
                x.EntitleType.ToString(),
                x.Periodicity.ToString(),
                x.AllowanceName,
                x.IsTexable == 1 ? "Yes" : "No"
                )).ToList();

            return result;
        }

        public static List<Grade> GetUnassignedGrades(this IBaseRepository<ADParameterBasic> ADParameterBasicRepository,IBaseRepository<ADParameterGrade> ADParameterGradeRepository,
                                                       IBaseRepository<ADParameterEmployee> ADParameterEmployeeRepository,IBaseRepository<Grade> GradeRepository,
                                                       int AllowanceDeductID)
        {
            var Grades =GradeRepository.All;

            var items = (from adp in ADParameterBasicRepository.All
                        join adg in ADParameterGradeRepository.All on adp.ADParameterID equals adg.ADParameterID
                        join g in Grades on adg.GradeID equals g.GradeID
                        where adp.AllowDeductID == AllowanceDeductID
                        select g);

            var othersGrades = Grades.Except(items);

            return othersGrades.ToList();
        }

    }
}
