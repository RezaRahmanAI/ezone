using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IMiscellaneousService<T> where T : class
    {
        string GetUniqueKey(Func<T, int> codeSelector);
        T GetDuplicateEntry(Expression<Func<T, bool>> duplicateSelector);
    }
}
