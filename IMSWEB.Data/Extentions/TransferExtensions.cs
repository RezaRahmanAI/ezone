using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class TransferExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>>>
            GetAllAsync(this IBaseRepository<Transfer> TransferRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, int ConcernID)
        {
            var data = await (from t in TransferRepository.GetAll()
                              join fs in SisterConcernRepository.GetAll() on t.FromConcernID equals fs.ConcernID
                              join ts in SisterConcernRepository.GetAll() on t.ToConcernID equals ts.ConcernID
                              where t.TransferDate >= fromDate && t.TransferDate <= toDate && (t.ToConcernID == ConcernID || t.FromConcernID == ConcernID)
                              select new
                              {
                                  t.TransferID,
                                  t.TransferNo,
                                  t.TransferDate,
                                  t.ToConcernID,
                                  ToConcernName = ts.Name,
                                  FromConcernName = fs.Name,
                                  t.Remarks,
                                  t.TotalAmount,
                                  t.Status
                              }).OrderByDescending(i => i.TransferDate).ThenByDescending(i => i.TransferNo).ToListAsync();

            return data.Select(x => new Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>(
                         x.TransferID,
                         x.TransferNo,
                         x.TransferDate,
                         x.TotalAmount,
                         0m,
                         0m,
                         x.Status, new Tuple<string, string>(x.ToConcernName, x.FromConcernName)
                ));
        }


        public static IEnumerable<ProductWisePurchaseModel>
            GetDetailsByID(this IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Color> ColorRepository, IBaseRepository<Godown> GodownRepository, int TransferID)
        {
            var Details = (from td in TransferDetailRepository.GetAll()
                           join p in ProductRepository.GetAll() on td.ToProductID equals p.ProductID
                           join col in ColorRepository.GetAll() on td.ToColorID equals col.ColorID
                           join cat in CategoryRepository.GetAll() on p.CategoryID equals cat.CategoryID
                           join com in CompanyRepository.GetAll() on p.CompanyID equals com.CompanyID
                           join g in GodownRepository.GetAll() on td.ToGodownID equals g.GodownID
                           where td.TransferID == TransferID
                           select new ProductWisePurchaseModel
                           {
                               ProductID = td.ToProductID,
                               ColorName = col.Name,
                               GodownName = g.Name,
                               MRP = td.PRate,
                               TotalAmount = td.UTAmount,
                               ProductName = p.ProductName,
                               ProductCode = p.Code,
                               CategoryName = cat.Description,
                               CompanyName = com.Name,
                               IMENO = td.IMEI,
                               Quantity = td.Quantity,
                               SalesRate = td.SRate,
                               TotalSalesRate = td.Quantity * td.SRate
                           }).ToList();

            return Details;
        }

        public static IEnumerable<ProductWisePurchaseModel>
                GetTransferReport(this IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
                IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Company> CompanyRepository,
                IBaseRepository<Color> ColorRepository, IBaseRepository<Godown> GodownRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                IBaseRepository<StockDetail> StockDetailRepository, DateTime FromDate, DateTime ToDate, int ConcernID)
        {
            IQueryable<Transfer> transfers = null;
            if (ConcernID > 0)
            {
                transfers = TransferRepository.GetAll().Where(i => i.FromConcernID == ConcernID || i.ToConcernID == ConcernID);
            }
            else
            {
                transfers = TransferRepository.GetAll();
            }
            var Details = (from t in transfers
                           join fs in SisterConcernRepository.GetAll() on t.FromConcernID equals fs.ConcernID
                           join ts in SisterConcernRepository.GetAll() on t.ToConcernID equals ts.ConcernID
                           join td in TransferDetailRepository.GetAll() on t.TransferID equals td.TransferID
                           join p in ProductRepository.GetAll() on td.ToProductID equals p.ProductID
                           join col in ColorRepository.GetAll() on td.ToColorID equals col.ColorID
                           join cat in CategoryRepository.GetAll() on p.CategoryID equals cat.CategoryID
                           join com in CompanyRepository.GetAll() on p.CompanyID equals com.CompanyID
                           join std in StockDetailRepository.GetAll() on td.SDetailID equals std.SDetailID
                           join tg in GodownRepository.GetAll() on td.ToGodownID equals tg.GodownID
                           join fg in GodownRepository.GetAll() on std.GodownID equals fg.GodownID


                           where (t.TransferDate >= FromDate && t.TransferDate <= ToDate) /*&& (t.FromConcernID == ConcernID || t.ToConcernID == ConcernID)*/
                           select new ProductWisePurchaseModel
                           {
                               FromConcernName = fs.Name,
                               ToConcernName = ts.Name,
                               ChallanNo = t.TransferNo,
                               Date = t.TransferDate,
                               NetTotal = t.TotalAmount,
                               ProductID = td.ToProductID,
                               ColorName = col.Name,
                               GodownName = tg.Name,
                               FromGodownName = fg.Name,
                               MRP = td.PRate,
                               TotalAmount = td.UTAmount,
                               ProductName = p.ProductName,
                               ProductCode = p.Code,
                               CategoryName = cat.Description,
                               CompanyName = com.Name,
                               IMENO = td.IMEI,
                               Quantity = td.Quantity,
                           }).ToList();

            return Details;
        }

        public static IEnumerable<ProductWisePurchaseModel>
                GetTransferReportFromTo(this IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
                IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Company> CompanyRepository,
                IBaseRepository<Color> ColorRepository, IBaseRepository<Godown> GodownRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                IBaseRepository<StockDetail> StockDetailRepository, DateTime FromDate, DateTime ToDate, int FromConern, int ToConcern)
        {
            var Details = (from t in TransferRepository.GetAll()
                           join fs in SisterConcernRepository.GetAll() on t.FromConcernID equals fs.ConcernID
                           join ts in SisterConcernRepository.GetAll() on t.ToConcernID equals ts.ConcernID
                           join td in TransferDetailRepository.GetAll() on t.TransferID equals td.TransferID
                           join p in ProductRepository.GetAll() on td.ToProductID equals p.ProductID
                           join col in ColorRepository.GetAll() on td.ToColorID equals col.ColorID
                           join cat in CategoryRepository.GetAll() on p.CategoryID equals cat.CategoryID
                           join com in CompanyRepository.GetAll() on p.CompanyID equals com.CompanyID
                           join std in StockDetailRepository.GetAll() on td.SDetailID equals std.SDetailID
                           join tg in GodownRepository.GetAll() on td.ToGodownID equals tg.GodownID
                           join fg in GodownRepository.GetAll() on std.GodownID equals fg.GodownID
                           where (t.TransferDate >= FromDate && t.TransferDate <= ToDate) && t.FromConcernID == FromConern && t.ToConcernID == ToConcern
                           select new ProductWisePurchaseModel
                           {
                               FromConcernName = fs.Name,
                               ToConcernName = ts.Name,
                               ChallanNo = t.TransferNo,
                               Date = t.TransferDate,
                               NetTotal = t.TotalAmount,
                               ProductID = td.ToProductID,
                               ColorName = col.Name,
                               GodownName = tg.Name,
                               FromGodownName = fg.Name,
                               MRP = td.PRate,
                               TotalAmount = td.UTAmount,
                               ProductName = p.ProductName,
                               ProductCode = p.Code,
                               CategoryName = cat.Description,
                               CompanyName = com.Name,
                               IMENO = td.IMEI,
                               Quantity = td.Quantity,
                           }).ToList();

            return Details;
        }
    }
}
