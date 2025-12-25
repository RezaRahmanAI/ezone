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
    public class ROrderDetailService : IROrderDetailService
    {
        private readonly IBaseRepository<ROrderDetail> _ReturnDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ROrderDetailService(IBaseRepository<ROrderDetail> returnDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IUnitOfWork unitOfWork)
        {
            _ReturnDetailRepository = returnDetailRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddReturnOrderDetail(ROrderDetail rOrderDetail)
        {
            _ReturnDetailRepository.Add(rOrderDetail);
        }

        public void SaveReturnOrderDetail()
        {
            _unitOfWork.Commit();
        }

        //public IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
        //    Tuple<decimal, decimal, string, string, int, string>>>
        //    GetPurchaseOrderDetailById(int id)
        //{
        //    return _purchaseOrderDetailRepository.GetPurchaseOrderDetailById(_productRepository, _colorRepository, id);
        //}

        public void DeleteReturnOrderDetail(int id)
        {
            _ReturnDetailRepository.Delete(x => x.ROrderID == id);
        }
    }
}
