using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IROrderDetailService
    {
        void AddReturnOrderDetail(ROrderDetail rOrderDetail);
       // void SaveReturnOrderDetail();
        //IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
        //    Tuple<decimal, decimal, string, string, int, string>>> GetReturnOrderDetailById(int id);
        void DeleteReturnOrderDetail(int id);
    }
}
