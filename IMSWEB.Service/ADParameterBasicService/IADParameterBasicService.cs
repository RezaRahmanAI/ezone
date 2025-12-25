using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IADParameterBasicService
    {
        IEnumerable<Tuple<int, List<string>, string, string, string, string>> GetAllowancesDeductionRuleSetupAsPerGrade(int AllowORDeduct);
        List<Grade> GetUnassignedGrades(int AllowanceDeductID);

        void Add(ADParameterBasic ADParameterBasic);
        void Update(ADParameterBasic ADParameterBasic);
        void Save();
        IEnumerable<ADParameterBasic> GetAll();
        //Task<IEnumerable<ADParameterBasic>> GetAllAsync();
        ADParameterBasic GetById(int id);
        void Delete(int id);
    }
}
