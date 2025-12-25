using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SaleOfferExtensions
    {
        public static async Task<IEnumerable<SaleOffer>> GetAllSaleOfferAsync(this IBaseRepository<SaleOffer> saleOfferRepository)
        {
            return await saleOfferRepository.All.ToListAsync();
        }

        public static async Task<IEnumerable<Tuple<int, string, string, DateTime, DateTime, string, string, Tuple<string, string, string>>>>
        GetAllSOfferAsync(this IBaseRepository<SaleOffer> saleOfferRepository, IBaseRepository<Product> productRepository)
        {
            IQueryable<Product> products = productRepository.All;

            var items = await saleOfferRepository.All.Join(products,
                saleoff => saleoff.ProductID, pro => pro.ProductID, (saleoff, pro) => new { SalesOffer = saleoff, Product = pro }).
                Select(x => new
                {
                    x.SalesOffer.OfferID,
                    x.SalesOffer.OfferCode,
                    x.SalesOffer.Product.ProductName,
                    x.SalesOffer.FromDate,
                    x.SalesOffer.ToDate,
                    x.SalesOffer.Description,
                    x.SalesOffer.OfferValue,                   
                    x.SalesOffer.OfferType,
                    x.SalesOffer.ThresholdValue,
                    x.SalesOffer.Status                

                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, DateTime, DateTime, string, string, Tuple<string, string, string>>
                (
                    x.OfferID,
                    x.OfferCode,
                    x.ProductName,
                    x.FromDate.Value,
                    x.ToDate.Value,
                    x.Description,
                    x.OfferValue.ToString(),
                        new Tuple<string,string,string>
                        (
                            x.OfferType.ToString(),
                            x.ThresholdValue.ToString(),
                            x.Status.ToString()
                        )
                )).OrderByDescending(x=>x.Item1).ToList();
        }

    }
}
