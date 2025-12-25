using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IStockDetailService
    {
        void AddStockDetail(StockDetail StockDetail);
        void SaveStockDetail();
        IEnumerable<StockDetail> GetStockDetailByProductId(int id);
        void DeleteStockDetail(int id);
        StockDetail GetById(int id);
        IQueryable<StockDetail> GetAll();
        StockDetail GetStockDetail(int ProductID, int ColorID, string IMEI);
        IEnumerable<StockDetail> GetStockDetailByProductIdColorID(int ProductID, int ColorID);
        void Update(StockDetail StockDetail);
        StockDetail GetNewById(int id);
    }
}
