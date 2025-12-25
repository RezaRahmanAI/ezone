using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IROProductDetailService
    {
        void AddROProductDetail(ROProductDetail rOProductDetail);
        void SaveROProductDetail();
        IEnumerable<ROProductDetail> GetROProductDetailsById(int poDetailId, int productId);
        IEnumerable<ROProductDetail> GetProductDetailsById(int productId);
        void DeleteROProductDetail(int id);
    }
}
