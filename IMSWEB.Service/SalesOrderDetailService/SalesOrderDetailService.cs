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
    public class SalesOrderDetailService : ISalesOrderDetailService
    {
        private readonly IBaseRepository<SOrderDetail> _salesOrderDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SalesOrderDetailService(IBaseRepository<SOrderDetail> salesOrderDetailRepository,
            IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository,
            IUnitOfWork unitOfWork)
        {
            _salesOrderDetailRepository = salesOrderDetailRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _stockDetailRepository = stockDetailRepository;
            _unitOfWork = unitOfWork;
        }

        //public void AddSalesOrderDetail(SOrderDetail pOrderDetail)
        //{
        //    _salesOrderDetailRepository.Add(pOrderDetail);
        //}

        public void AddSalesOrderDetail(SOrderDetail pOrderDetail)
        {
            _salesOrderDetailRepository.Add(pOrderDetail);
        }

        public IEnumerable<Tuple<int, int, int, int, string, string, string,
            Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>> GetSalesOrderDetailByOrderId(int id)
        {
            return _salesOrderDetailRepository.GetSalesOrderDetailByOrderId(id, _productRepository,
                _colorRepository, _stockDetailRepository);
        }


        //public IEnumerable<Tuple<int, int, int, int, string, string, string,
        //    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>> GetSalesOrderDetailByOrderId(int id)
        //{
        //    return _salesOrderDetailRepository.GetSalesOrderDetailByOrderId(id, _productRepository,
        //        _colorRepository, _stockDetailRepository);
        //}


        public IEnumerable<Tuple<int, int, int, int, string, string, string,
    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>> GetSalesOrderDetailByOrderIdForInvoice(int id)
        {
            return _salesOrderDetailRepository.GetSalesOrderDetailByOrderIdForInvoice(id, _productRepository,
                _colorRepository, _stockDetailRepository);
        }

        public void SaveSalesOrderDetail()
        {
            _unitOfWork.Commit();
        }

        public void DeleteSalesOrderDetail(int id)
        {
            _salesOrderDetailRepository.Delete(x => x.SOrderDetailID == id);
        }
        public IEnumerable<SOrderDetail> GetSOrderDetailsBySOrderID(int SOrderID)
        {
            return _salesOrderDetailRepository.FindBy(i => i.SOrderID == SOrderID);
        }
    }
}