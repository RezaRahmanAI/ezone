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
    public class StockDetailService : IStockDetailService
    {
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IBaseRepository<Stock> _stockRepository;
        private readonly IUnitOfWork _unitOfWork;


        public StockDetailService(IBaseRepository<StockDetail> stockDetailRepository, IUnitOfWork unitOfWork,
            IBaseRepository<Stock> stockRepository)
        {
            _stockDetailRepository = stockDetailRepository;
            _unitOfWork = unitOfWork;
            _stockRepository = stockRepository;
        }

        public void AddStockDetail(StockDetail StockDetail)
        {
            _stockDetailRepository.Add(StockDetail);
        }

        public void SaveStockDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<StockDetail> GetStockDetailByProductId(int id)
        {
            return _stockDetailRepository.FindBy(x => x.ProductID == id);
        }

        public void DeleteStockDetail(int id)
        {
            _stockDetailRepository.Delete(x => x.SDetailID == id);
        }
        public StockDetail GetById(int id)
        {
            return _stockDetailRepository.FindBy(x => x.SDetailID == id).FirstOrDefault();
        }

        public StockDetail GetNewById(int id)
        {
            return _stockDetailRepository.FindBy(x => x.SDetailID == id && x.IsDamage == 0).FirstOrDefault();
        }


        public IQueryable<StockDetail> GetAll()
        {
            return from st in _stockRepository.All
                   join std in _stockDetailRepository.All on st.StockID equals std.StockID
                   select std;
        }

        public StockDetail GetStockDetail(int ProductID, int ColorID, string IMEI)
        {
            return _stockDetailRepository.FindBy(i => i.ProductID == ProductID && i.ColorID == ColorID && i.IMENO.Equals(IMEI.Trim())).FirstOrDefault();
        }
        public IEnumerable<StockDetail> GetStockDetailByProductIdColorID(int ProductID, int ColorID)
        {
            return _stockDetailRepository.FindBy(x => x.ProductID == ProductID && x.ColorID == ColorID && x.Status == (int)EnumStockStatus.Stock);
        }
        public void Update(StockDetail StockDetail)
        {
            _stockDetailRepository.Update(StockDetail);
        }
    }
}
