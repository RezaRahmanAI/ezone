using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
   public interface ITransferHistoryRepository
    {
       bool AddTransferHistoryUsingSP(DataTable dtTransferHistory);
    }
}
