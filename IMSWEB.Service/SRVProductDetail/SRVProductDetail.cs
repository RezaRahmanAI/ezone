using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SRVProductDetailService : ISRVProductDetailService
    {
        private readonly IBaseRepository<SRVProductDetail> _SRVProductDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SRVProductDetailService(IBaseRepository<SRVProductDetail> sRVProductDetailRepository, IUnitOfWork unitOfWork)
        {
            _SRVProductDetailRepository = sRVProductDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddSRVProductDetail(SRVProductDetail sRVProductDetail)
        {
            _SRVProductDetailRepository.Add(sRVProductDetail);
        }

        public void SaveSRVProductDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<SRVProductDetail> GetSRVProductDetailsById(int SRVDetailId, int productId)
        {
            return _SRVProductDetailRepository.FindBy(x => (x.SRVisitDID == SRVDetailId && x.ProductID == productId) && (x.Status == (int)EnumSRVProductDetailsStatus.Stock || x.Status == (int)EnumSRVProductDetailsStatus.Sold || x.Status == (int)EnumSRVProductDetailsStatus.SalesReturn));
        }

        public IEnumerable<SRVProductDetail> GetProductDetailsById(int productId)
        {
            return _SRVProductDetailRepository.FindBy(x => x.ProductID == productId);
        }

        public void DeleteSRVProductDetail(int id)
        {
            _SRVProductDetailRepository.Delete(x => x.SRVisitPDID == id);
        }
    }
}
