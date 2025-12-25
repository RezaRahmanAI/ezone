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
    public class ROProductDetailService : IROProductDetailService
    {
        private readonly IBaseRepository<ROProductDetail> _rOProductDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ROProductDetailService(IBaseRepository<ROProductDetail> rOProductDetailRepository, IUnitOfWork unitOfWork)
        {
            _rOProductDetailRepository = rOProductDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddROProductDetail(ROProductDetail rOProductDetail)
        {
            _rOProductDetailRepository.Add(rOProductDetail);
        }

        public void SaveROProductDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<ROProductDetail> GetROProductDetailsById(int roDetailId, int productId)
        {
            return _rOProductDetailRepository.FindBy(x => x.RODetailID == roDetailId && x.ProductID == productId);
        }

        public IEnumerable<ROProductDetail> GetProductDetailsById(int productId)
        {
            return _rOProductDetailRepository.FindBy(x => x.ProductID == productId);
        }

        public void DeleteROProductDetail(int id)
        {
            _rOProductDetailRepository.Delete(x => x.ROPDID == id);
        }
    }
}
