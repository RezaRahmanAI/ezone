using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISRVProductDetailService
    {
        void AddSRVProductDetail(IMSWEB.Model.SRVProductDetail sRVProductDetail);
        void SaveSRVProductDetail();
        IEnumerable<IMSWEB.Model.SRVProductDetail> GetSRVProductDetailsById(int SRVDetailId, int productId);
        IEnumerable<IMSWEB.Model.SRVProductDetail> GetProductDetailsById(int productId);
        void DeleteSRVProductDetail(int id);
    }
}
