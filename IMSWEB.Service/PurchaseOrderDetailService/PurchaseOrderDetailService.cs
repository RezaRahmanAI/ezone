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
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IBaseRepository<POrderDetail> _purchaseOrderDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<Godown> _godownRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderDetailService(IBaseRepository<POrderDetail> purchaseOrderDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, 
            IUnitOfWork unitOfWork, IBaseRepository<Godown> godownRepository)
        {
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
            _godownRepository = godownRepository;
        }

        public void AddPurchaseOrderDetail(POrderDetail pOrderDetail)
        {
            _purchaseOrderDetailRepository.Add(pOrderDetail);
        }

        public void SavePurchaseOrderDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
            Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<int, string>>>>
            GetPurchaseOrderDetailById(int id)
        {
            return _purchaseOrderDetailRepository.GetPurchaseOrderDetailById(_productRepository,
                _colorRepository,_godownRepository, id);
        }

        public void DeletePurchaseOrderDetail(int id)
        {
            _purchaseOrderDetailRepository.Delete(x => x.POrderID == id);
        }

        public IQueryable<POrderDetail> GetPOrderDetailByID(int POrderID)
        {
            return _purchaseOrderDetailRepository.AllIncluding(i => i.POProductDetails).Where(i => i.POrderID == POrderID);
        }
        public POrderDetail GetPOrderDetailsByPurchaseOrderId(int id)
        {
            return _purchaseOrderDetailRepository.FindBy(x => x.POrderID == id).First();
        }
        public POrderDetail GetPODetailByPOPDID(int POPDID)
        {
            return _purchaseOrderDetailRepository.All.FirstOrDefault(i => i.POrderID == POPDID);
        }
    }
}
