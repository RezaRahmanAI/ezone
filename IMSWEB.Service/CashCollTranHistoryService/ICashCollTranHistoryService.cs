using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICashCollTranHistoryService
    {
        void Add(CashCollTranHistory model);
        void Update(CashCollTranHistory model);
        bool Save();
    }
}
