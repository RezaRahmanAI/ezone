using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using IMSWEB.Model.SPModel;
namespace IMSWEB.Data
{
    public interface IDORepository
    {
        Tuple<bool, int> ADDDOEntry(DO doModel, int DOID);
        bool DeleteByID(int DOID, int UserID, DateTime dateTime);
    }
}
