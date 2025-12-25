using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ADParameterGradeExtension
    {
        public static List<Grade> GetGradesByADParameterID(this IBaseRepository<ADParameterGrade> ADParameterGradeRepository, IBaseRepository<Grade> GradeRepository, int ADParameterID)
        {
            var result = (from adpg in ADParameterGradeRepository.All
                          join g in GradeRepository.All on adpg.GradeID equals g.GradeID
                          where adpg.ADParameterID == ADParameterID
                          select g).ToList();
            return result;
        }
    }
}
