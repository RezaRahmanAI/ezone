using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IPOProductDetailService
    {
        void AddPOProductDetail(POProductDetail pOProductDetail);
        void SavePOProductDetail();
        IQueryable<POProductDetail> GetAll();
        IEnumerable<POProductDetail> GetPOProductDetailsById(int poDetailId, int productId, int ColorID);

        IEnumerable<POProductDetail> GetPOProductDetailsById(int poDetailId, int productId);
        IEnumerable<POProductDetail> GetProductDetailsById(int productId);
        IEnumerable<POProductDetail> GetByIMEINo(string iemiNo);
        void DeletePOProductDetail(int id);
        POProductDetail GetPOPDetailByPOPDID(int POPDID);
    }
}
