using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IADParamADParameterGradeService
    {
        void Add(ADParameterGrade ADParameterGrade);
        void Update(ADParameterGrade ADParameterGrade);
        void Save();
        IEnumerable<ADParameterGrade> GetAll();
        //Task<IEnumerable<ADParameterGrade>> GetAllAsync();
        ADParameterGrade GetById(int id);
        void Delete(int id);
        List<Grade> GetGradesByADParamID(int ADParamID);
    }
}
