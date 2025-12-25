using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class GodownExtensions
    {
        public static async Task<IEnumerable<Godown>> GetAllGodownAsync(this IBaseRepository<Godown> godownRepository)
        {
            return await godownRepository.All.ToListAsync();
        }

        public static async Task<IEnumerable<Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>>> GetAllTransferHistoryAsync(this IBaseRepository<Godown> godownRepository, IBaseRepository<Product> productRepository, IBaseRepository<TransferHistory> transferHistoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Category> categoryRepository)
        {
            var products = productRepository.All;
            var transferhistories = transferHistoryRepository.All;
            var companies = companyRepository.All;
            var categories = categoryRepository.All;
          //  var models = modelRepository.All;
            var godowns = godownRepository.All;
            var result = await (from tran in transferhistories
                                join pro in products on tran.ProductId equals pro.ProductID
                                join com in companies on pro.CompanyID equals com.CompanyID
                             //   join mod in models on pro.ModelID equals mod.ModelID
                                join cat in categories on pro.CategoryID equals cat.CategoryID
                               
                                select new
                                {
                                    TransferHID = tran.TransferHID,
                                    ProductId = 1,
                                    ProductName = pro.ProductName,
                                    TransferDate = tran.TransferDate,
                                    ToGodown = tran.ToGodown,
                                    ToGodownName = godowns.FirstOrDefault(i => i.GodownID == tran.ToGodown).Name,
                                    FromGodown = tran.FromGodown,
                                    FromGodownName = godowns.FirstOrDefault(i => i.GodownID == tran.FromGodown).Name,
                                    Qty = tran.Qty,
                                    CompanyName = com.Name,
                                    CategoryName = cat.Description,
                                    ModelName = ""
                                }).OrderByDescending(i => i.TransferHID).ToListAsync();

            return result.Select(x => new Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>
                (
                    x.TransferHID,
                    x.ProductId,
                    x.ProductName,
                    x.TransferDate.Date,
                    x.ToGodown,
                    x.ToGodownName,
                    x.FromGodown,
                    new Tuple<string, decimal, string, string, string>(
                        x.FromGodownName,
                        x.Qty,
                        x.CompanyName,
                        x.CategoryName,
                        x.ModelName
                        )

                )).OrderByDescending(i => i.Item1).ToList();

        }

    }
}
