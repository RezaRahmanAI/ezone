using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISRVisitDetailService
    {
        void AddSRVisitDetail(SRVisitDetail sRVisitDetail);
        void SaveSRVisitDetail();
        IEnumerable<Tuple<int, int, int, decimal, string, string, int, Tuple<string>>> GetSRVisitDetailById(int id);
        void DeleteSRVisitDetail(int id);
    }
}
