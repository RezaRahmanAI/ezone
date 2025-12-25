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
    public class POProductDetailService : IPOProductDetailService
    {
        private readonly IBaseRepository<POProductDetail> _pOProductDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public POProductDetailService(IBaseRepository<POProductDetail> pOProductDetailRepository, IUnitOfWork unitOfWork)
        {
            _pOProductDetailRepository = pOProductDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddPOProductDetail(POProductDetail pOProductDetail)
        {
            _pOProductDetailRepository.Add(pOProductDetail);
        }

        public void SavePOProductDetail()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<POProductDetail> GetAll()
        {
            return _pOProductDetailRepository.All;
        }


        public IEnumerable<POProductDetail> GetPOProductDetailsById(int poDetailId, int productId)
        {
            return _pOProductDetailRepository.FindBy(x=>x.POrderDetailID == poDetailId && x.ProductID == productId);
        }

        public IEnumerable<POProductDetail> GetProductDetailsById(int productId)
        {
            return _pOProductDetailRepository.FindBy(x => x.ProductID == productId);
        }

        public IEnumerable<POProductDetail> GetByIMEINo(string iemiNo)
        {
            //return _pOProductDetailRepository.All.FindBy(x => x.IMENO.ToLower() == iemiNo && );
            return _pOProductDetailRepository.All.Where(x => x.IMENO.ToLower() == iemiNo);
        }

        public void DeletePOProductDetail(int id)
        {
            _pOProductDetailRepository.Delete(x => x.POPDID == id);
        }

        public  POProductDetail GetPOPDetailByPOPDID(int POPDID)
        {
            return _pOProductDetailRepository.All.FirstOrDefault(i => i.POPDID == POPDID);
        }
        public IEnumerable<POProductDetail> GetPOProductDetailsById(int poDetailId, int productId, int ColorID)
        {
            return _pOProductDetailRepository.FindBy(x => x.POrderDetailID == poDetailId && x.ProductID == productId && x.ColorID == ColorID);
        }
    }
}
