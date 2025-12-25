using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ProductExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string, decimal,
            string, string, string, Tuple<decimal, decimal>>>> GetAllProductAsync(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository)
        {
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;

            var items = await productRepository.All.Join(categories, prod => prod.CategoryID, cat => cat.CategoryID, (prod, cat) => new { Product = prod, Category = cat }).
                Join(companies, prod => prod.Product.CompanyID, com => com.CompanyID, (prod, com) => new { prod.Product, prod.Category, Company = com }).
                Select(x => new
                {
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    MRP = x.Product.MRP ?? 0m,
                    x.Product.PurchaseRate
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string, 
                Tuple<decimal, decimal>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal,decimal>(x.MRP,x.PurchaseRate)
                )).ToList().OrderBy(x=>x.Item2);
        }

        public static IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForIndex(
    this IBaseRepository<Product> productRepository,
    IBaseRepository<Category> categoryRepository,
    IBaseRepository<Company> companyRepository)
        {
            var products = from p in productRepository.All
                           join com in companyRepository.All on p.CompanyID equals com.CompanyID
                           join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                           select new ProductWisePurchaseModel
                           {
                               ProductID = p.ProductID,
                               ProductCode = p.Code,
                               ProductName = p.ProductName,
                               CompanyName = com.Name,
                               CategoryName = cat.Description,
                               MRP = p.MRP ?? 0,
                               RP = p.RP
                           };

            return products;
        }


        public static IQueryable<ProductWisePurchaseModel> GetAllProductIQueryable(this IBaseRepository<Product> productRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository,
        IBaseRepository<SaleOffer> offerRepository)
        {
            var products = from p in productRepository.All
                           join com in companyRepository.All on p.CompanyID equals com.CompanyID
                           join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                           join off in offerRepository.All on p.ProductID equals off.ProductID into lp
                           from off in lp.DefaultIfEmpty()
                           select new ProductWisePurchaseModel
                           {
                               ProductID = p.ProductID,
                               ProductCode = p.Code,
                               ProductName = p.ProductName,
                               CompanyName = com.Name,
                               CompanyID = com.CompanyID,
                               CategoryName = cat.Description,
                               CategoryID = cat.CategoryID,
                               ProductType = p.ProductType,
                               UnitType = p.UnitType,
                               PWDiscount = p.PWDiscount,
                               MRP = p.MRP.HasValue ? (decimal)p.MRP : 0m,
                               CompressorWarrentyMonth = p.CompressorWarrentyMonth,
                               MotorWarrentyMonth = p.MotorWarrentyMonth,
                               PanelWarrentyMonth = p.PanelWarrentyMonth,
                               ServiceWarrentyMonth = p.ServiceWarrentyMonth,
                               SparePartsWarrentyMonth = p.SparePartsWarrentyMonth,
                               OfferDescription = off != null ? off.Description : "",
                               UserInputWarranty = p.UserInputWarranty,
                               RP = p.RP
                           };
            return products;
        }

        public static IEnumerable<Tuple<int, string, string, decimal,
            string, string, string, Tuple<decimal?, int, decimal, decimal>>> GetAllProduct(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Stock> stockRepository,
            IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<Color> colorRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            var StockDetails = stockDetailRepository.All;
            var colors = colorRepository.All;

            var items = products.GroupJoin(stocks, p => p.ProductID, s => s.ProductID,
                (p, s) => new { Product = p, Stocks = s }).
                SelectMany(s => s.Stocks.DefaultIfEmpty(), (p, s) => new { Product = p.Product, Stock = s }).
                GroupJoin(companies, ps => ps.Product.CompanyID, c => c.CompanyID,
                (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Companies = c }).
                SelectMany(c => c.Companies.DefaultIfEmpty(), (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Company = c }).
                GroupJoin(categories, psc => psc.Product.CategoryID, c => c.CategoryID,
                (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc.Company, Categories = c }).
                SelectMany(c => c.Categories.DefaultIfEmpty(), (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc, Category = c }).
                GroupJoin(colors, pscc => pscc.Stock.ColorID, c => c.ColorID,
                (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Category = pscc.Category, Company = pscc.Company, colors = c }).
                SelectMany(c => c.colors.DefaultIfEmpty(), (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Company = pscc.Company, Category = pscc.Category, Color = c }).
                GroupBy(x => new
                {
                    x.Product.ProductID,
                    x.Product.ProductName,
                    x.Product.Code,
                    x.Color.ColorID,
                    ColorName = x.Color.Name,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    x.Category.Description,
                    x.Company.Company.Name,
                    x.Product.ProductType,
                    x.Stock.GodownID,
                    MRP = x.Product.MRP ?? 0m,
                    x.Product.PurchaseRate
                }).
                Select(x => new
                {
                    ProductId = x.Key.ProductID,
                    ProductCode = x.Key.Code,
                    x.Key.ProductName,
                    x.Key.PWDiscount,
                    x.Key.PicturePath,
                    CategoryName = x.Key.Description,
                    CompanyName = x.Key.Name,
                    //PreviousStock = (decimal?)x.Where(s => s.Stock != null).Sum(s => s.Stock.Quantity),
                    PreviousStock = StockDetails.Any(i => i.ProductID == x.Key.ProductID && i.ColorID == x.Key.ColorID && i.GodownID == x.Key.GodownID && i.Status == (int)EnumStockStatus.Stock)
                                    ? x.Key.ProductType == (int)EnumProductType.NoBarcode
                                            ? StockDetails.Where(i => i.ProductID == x.Key.ProductID && i.ColorID == x.Key.ColorID && i.GodownID == x.Key.GodownID && i.Status == (int)EnumStockStatus.Stock).Sum(o => o.Quantity)
                                            : StockDetails.Count(i => i.ProductID == x.Key.ProductID && i.ColorID == x.Key.ColorID && i.GodownID == x.Key.GodownID && i.Status == (int)EnumStockStatus.Stock)
                                    : 0m,
                    x.Key.ProductType,
                    x.Key.MRP,
                    x.Key.PurchaseRate
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, int, decimal, decimal>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, int, decimal,decimal>(
                        x.PreviousStock,
                        x.ProductType,
                        x.MRP,
                        x.PurchaseRate
                        )
                )).ToList();
        }

        public static IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>
            GetAllProductFromDetail(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository, IBaseRepository<Godown> godownRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;
            IQueryable<Godown> godowns = godownRepository.All;

            IQueryable<SaleOffer> saleOffer = saleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);
            var items = details.Join(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { Detail = d, Product = p }).
                Join(companies, dp => dp.Product.CompanyID, c => c.CompanyID,
                (dp, c) => new { Product = dp.Product, Detail = dp.Detail, Company = c }).
                Join(categories, dpc => dpc.Product.CategoryID, c => c.CategoryID,
                (dpc, c) => new { Product = dpc.Product, Detail = dpc.Detail, Company = dpc.Company, Category = c }).
                Join(stocks, dpccm => dpccm.Detail.StockID, s => s.StockID,
                (dpccm, s) => new { Product = dpccm.Product, Detail = dpccm.Detail, Company = dpccm.Company, Category = dpccm.Category, Stock = s }).
                Join(colors, dpccms => dpccms.Detail.ColorID, c => c.ColorID,
                (dpccms, c) => new { Product = dpccms.Product, Detail = dpccms.Detail, Company = dpccms.Company, Category = dpccms.Category, Stock = dpccms.Stock, Color = c }).


                 Join(godowns, dpccms => dpccms.Detail.GodownID, c => c.GodownID,
                (dpccmsc, g) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, Color = dpccmsc.Color, Godown = g })
                .
                GroupJoin(saleOffer, dpccmsc => dpccmsc.Product.ProductID, o => o.ProductID,
                (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, dpccmscg.Godown, Offer = o })
                .SelectMany(
          dpccmscg => dpccmscg.Offer.DefaultIfEmpty(),
          (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, dpccmscg.Godown, Offer = o }).
                Where(x => x.Detail.Status == 1).
                Select(x => new
                {
                    DetailId = x.Detail.SDetailID,
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    PreviousStock = x.Stock.Quantity,
                    IMENO = x.Detail.IMENO,
                    MRPRate = x.Detail.SRate,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    GodownName = x.Godown.Name,
                    OfferDescription = x.Offer.Description,
                    ProductType = (EnumProductType)x.Product.ProductType,
                    x.Product.CompressorWarrentyMonth,
                    x.Product.PanelWarrentyMonth,
                    x.Product.MotorWarrentyMonth,
                    x.Product.SparePartsWarrentyMonth,
                    x.Product.ServiceWarrentyMonth,
                    x.Detail.PRate
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string,
                Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string,
                    Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>(
                        x.PreviousStock,
                        x.IMENO,
                        x.MRPRate,
                        x.DetailId,
                        x.ColorId,
                        x.ColorName,
                        x.OfferDescription,
                        new Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>
                            (
                            x.CompressorWarrentyMonth,
                            x.PanelWarrentyMonth,
                            x.MotorWarrentyMonth,
                            x.SparePartsWarrentyMonth,
                            x.ServiceWarrentyMonth,
                            x.GodownName,
                            x.PRate,
                            new Tuple<EnumProductType>(x.ProductType)
                            )
                        )
                )).ToList();
        }

        public static IEnumerable<Tuple<int, string, string, decimal,
               string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>> GetAllProductFromDetailForCredit(this IBaseRepository<Product> productRepository,
               IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
               IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository, IBaseRepository<Godown> godownRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;
            IQueryable<Godown> godowns = godownRepository.All;

            IQueryable<SaleOffer> saleOffer = saleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);
            var items = details.Join(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { Detail = d, Product = p }).
                Join(companies, dp => dp.Product.CompanyID, c => c.CompanyID,
                (dp, c) => new { Product = dp.Product, Detail = dp.Detail, Company = c }).
                Join(categories, dpc => dpc.Product.CategoryID, c => c.CategoryID,
                (dpc, c) => new { Product = dpc.Product, Detail = dpc.Detail, Company = dpc.Company, Category = c }).
                Join(stocks, dpccm => dpccm.Detail.StockID, s => s.StockID,
                (dpccm, s) => new { Product = dpccm.Product, Detail = dpccm.Detail, Company = dpccm.Company, Category = dpccm.Category, Stock = s }).
                Join(colors, dpccms => dpccms.Detail.ColorID, c => c.ColorID,
                (dpccms, c) => new { Product = dpccms.Product, Detail = dpccms.Detail, Company = dpccms.Company, Category = dpccms.Category, Stock = dpccms.Stock, Color = c }).


                 Join(godowns, dpccms => dpccms.Detail.GodownID, c => c.GodownID,
                (dpccmsc, g) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, Color = dpccmsc.Color, Godown = g })
                .
                GroupJoin(saleOffer, dpccmsc => dpccmsc.Product.ProductID, o => o.ProductID,
                (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, dpccmscg.Godown, Offer = o })
                .SelectMany(
          dpccmscg => dpccmscg.Offer.DefaultIfEmpty(),
          (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, dpccmscg.Godown, Offer = o }).
                Where(x => x.Detail.Status == 1).
                Select(x => new
                {
                    DetailId = x.Detail.SDetailID,
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    PreviousStock = x.Stock.Quantity,
                    IMENO = x.Detail.IMENO,
                    MRPRate12 = x.Detail.CRSalesRate12Month,
                    MRPRate = x.Detail.CreditSRate,
                    SalesRate = x.Detail.CRSalesRate3Month,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    OfferDescription = x.Offer.Description,
                    ProductType = x.Product.ProductType,
                    x.Product.CompressorWarrentyMonth,
                    x.Product.MotorWarrentyMonth,
                    x.Product.PanelWarrentyMonth,
                    x.Product.SparePartsWarrentyMonth,
                    x.Product.ServiceWarrentyMonth,
                    GodownName = x.Godown.Name
                }).ToList();


            var items2 = (from stock in stocks
                          join product in products on stock.ProductID equals product.ProductID
                          join cat in categories on product.CategoryID equals cat.CategoryID
                          join com in companies on product.CompanyID equals com.CompanyID
                          // join mod in models on product.ModelID equals mod.ModelID
                          join gdown in godowns on stock.GodownID equals gdown.GodownID
                          select new
                          {
                              ProductId = product.ProductID,
                              ProductCode = product.Code,
                              product.ProductName,
                              PWdiscount = 0,
                              product.PicturePath,
                              CategoryName = cat.Description,
                              CompanyName = com.Name,
                              PreviousStock = stock.Quantity,
                              ModelName = "",
                              PKTSheet = 0,
                              SalesRate = 0,
                              GodownID = gdown.GodownID,
                              GodownName = gdown.Name,
                              MRPRate = stock.LPPrice

                          }).GroupBy(x => new
                          {
                              x.ProductId,
                              x.ProductName,
                              x.ProductCode, /*x.Product.PWDiscount,*/
                              x.PicturePath,
                              x.CategoryName,
                              x.CompanyName,
                              ModelDescription = x.ModelName,
                              x.GodownID,
                              x.GodownName
                          }).
              Select(x => new
              {
                  ProductId = x.Key.ProductId,
                  ProductCode = x.Key.ProductCode,
                  x.Key.ProductName,
                  //x.Key.PWDiscount,
                  x.Key.PicturePath,
                  CategoryName = x.Key.CategoryName,
                  CompanyName = x.Key.CompanyName,
                  ModelName = x.Key.ModelDescription,
                  PreviousStock = (decimal?)x.Where(s => s != null).Sum(s => s.PreviousStock),
                  PKTSheet = x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet,
                  SalesRate = x.FirstOrDefault().SalesRate,
                  GodownID = x.FirstOrDefault() == null ? 0 : x.FirstOrDefault().GodownID,
                  GodownName = x.FirstOrDefault() == null ? "" : x.FirstOrDefault().GodownName,
                  Packet = (x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet) != 0 ? (int)(((decimal?)x.Where(s => s != null).Sum(s => s.PreviousStock)) / (x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet)) : 0,
                  MRPRate = x.FirstOrDefault().MRPRate
              }).ToList();



            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>(
                        x.PreviousStock,
                        x.IMENO,
                        x.MRPRate,
                        x.DetailId,
                        x.ColorId,
                        x.ColorName,
                        x.OfferDescription,
                        new Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>
                            (
                            x.SalesRate,
                            x.CompressorWarrentyMonth,
                            x.MotorWarrentyMonth,
                            x.PanelWarrentyMonth,
                            x.SparePartsWarrentyMonth,
                            x.ServiceWarrentyMonth,
                            x.MRPRate12,
                            new Tuple<string>
                            (x.GodownName)

                            )
                        )
                )).ToList();
        }



        public static IEnumerable<Tuple<int, string, string, decimal,
    string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>> GetAllSalesProductFromDetailByCustomerID(this IBaseRepository<Product> productRepository,
    IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
    IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository, IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> salesOrderDetailsRepository,

             IBaseRepository<Godown> godownRepository,
            int CustomerID)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;
            IQueryable<SOrderDetail> sOrderDetails = salesOrderDetailsRepository.All;
            IQueryable<Godown> godowns = godownRepository.All;

            var CustomerAllSales = salesOrderRepository.FindBy(i => i.CustomerID == CustomerID);
            var CustomerSalesDetails = (from so in CustomerAllSales.Where(i => i.Status == (int)EnumSalesType.Sales)
                                        join sod in sOrderDetails on so.SOrderID equals sod.SOrderID
                                        where sod.IsProductReturn == 0
                                        select new
                                        {
                                            StockDetailID = sod.RStockDetailID == null ? sod.SDetailID : sod.RStockDetailID,
                                            SalesPrice = sod.RStockDetailID == null ? sod.UTAmount : (decimal)sod.RepUnitPrice,
                                        }).ToList();

            //var repproduct = (from so in totalsales
            //                  join sod in sOrderDetails on so.SOrderID equals sod.RepOrderID
            //                  select new
            //                  {
            //                      StockDetailID = (int)sod.RStockDetailID,
            //                      SalesPrice = (decimal)sod.UTAmount
            //                  }).ToList();

            //CustomerSalesDetails.AddRange(repproduct);

            List<StockDetail> finaldetails = new List<StockDetail>();
            foreach (var item in CustomerSalesDetails)
            {
                var stockdetails = details.FirstOrDefault(i => i.SDetailID == item.StockDetailID);
                stockdetails.PRate = item.SalesPrice;
                finaldetails.Add(stockdetails);
            }



            //.Where(i=>(cusStockDetailIDs.Select(j=>j.StockDetailID).Contains(i.SDetailID)))

            IQueryable<SaleOffer> saleOffer = saleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var items = finaldetails.Join(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { Detail = d, Product = p }).
                Join(companies, dp => dp.Product.CompanyID, c => c.CompanyID,
                (dp, c) => new { Product = dp.Product, Detail = dp.Detail, Company = c }).
                Join(categories, dpc => dpc.Product.CategoryID, c => c.CategoryID,
                (dpc, c) => new { Product = dpc.Product, Detail = dpc.Detail, Company = dpc.Company, Category = c }).
                Join(stocks, dpccm => dpccm.Detail.StockID, s => s.StockID,
                (dpccm, s) => new { Product = dpccm.Product, Detail = dpccm.Detail, Company = dpccm.Company, Category = dpccm.Category, Stock = s }).
                Join(colors, dpccms => dpccms.Detail.ColorID, c => c.ColorID,
                (dpccms, c) => new { Product = dpccms.Product, Detail = dpccms.Detail, Company = dpccms.Company, Category = dpccms.Category, Stock = dpccms.Stock, Color = c }).

                Join(godowns, dpccms => dpccms.Detail.GodownID, c => c.GodownID,
                (dpccmsc, g) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, Color = dpccmsc.Color, Godown = g })
                .

                GroupJoin(saleOffer, dpccmsc => dpccmsc.Product.ProductID, o => o.ProductID,
                (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, Offer = o, dpccmscg.Godown })
                .SelectMany(
          dpccmsc => dpccmsc.Offer.DefaultIfEmpty(),
          (dpccmscg, o) => new { Product = dpccmscg.Product, Detail = dpccmscg.Detail, Company = dpccmscg.Company, Category = dpccmscg.Category, Stock = dpccmscg.Stock, dpccmscg.Color, Offer = o, dpccmscg.Godown }).
                Where(x => x.Detail.Status == 2).
                Select(x => new
                {
                    DetailId = x.Detail.SDetailID,
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    PreviousStock = x.Detail.Status,
                    IMENO = x.Detail.IMENO,
                    MRPRate = x.Detail.PRate,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    GodownName = x.Godown.Name
                    //OfferDescription = x.Offer.Description
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>(
                        x.PreviousStock, x.IMENO, x.MRPRate, x.DetailId, x.ColorId, x.ColorName, "", new Tuple<string>(x.GodownName))
                )).ToList();
        }



        public static IEnumerable<Tuple<int, string, string, decimal,
         string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> SRWiseGetAllProductFromDetail(this IBaseRepository<Product> productRepository,
         IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
         IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
            IBaseRepository<SRVisit> SRvisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository,
            IBaseRepository<SRVProductDetail> SRVProductDetailRepository, int EmployeeID)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;

            var SRVisitsDetails = from srv in SRvisitRepository.All.Where(i => i.EmployeeID == EmployeeID)
                                  join srvd in SRVisitDetailRepository.All on srv.SRVisitID equals srvd.SRVisitID
                                  join srvpd in SRVProductDetailRepository.All on srvd.SRVisitDID equals srvpd.SRVisitDID
                                  join sd in details on srvpd.SDetailID equals sd.SDetailID
                                  where sd.Status == (int)EnumStockStatus.Stock && srvpd.Status == (int)EnumStockStatus.Stock
                                  select sd;



            IQueryable<SaleOffer> saleOffer = saleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var items = SRVisitsDetails.Join(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { Detail = d, Product = p }).
                Join(companies, dp => dp.Product.CompanyID, c => c.CompanyID,
                (dp, c) => new { Product = dp.Product, Detail = dp.Detail, Company = c }).
                Join(categories, dpc => dpc.Product.CategoryID, c => c.CategoryID,
                (dpc, c) => new { Product = dpc.Product, Detail = dpc.Detail, Company = dpc.Company, Category = c }).
                Join(stocks, dpccm => dpccm.Detail.StockID, s => s.StockID,
                (dpccm, s) => new { Product = dpccm.Product, Detail = dpccm.Detail, Company = dpccm.Company, Category = dpccm.Category, Stock = s }).
                Join(colors, dpccms => dpccms.Detail.ColorID, c => c.ColorID,
                (dpccms, c) => new { Product = dpccms.Product, Detail = dpccms.Detail, Company = dpccms.Company, Category = dpccms.Category, Stock = dpccms.Stock, Color = c }).
                GroupJoin(saleOffer, dpccmsc => dpccmsc.Product.ProductID, o => o.ProductID,
                (dpccmsc, o) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, dpccmsc.Color, Offer = o })
                .SelectMany(
          dpccmsc => dpccmsc.Offer.DefaultIfEmpty(),
          (dpccmsc, o) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, dpccmsc.Color, Offer = o }).
                Where(x => x.Detail.Status == 1).
                Select(x => new
                {
                    DetailId = x.Detail.SDetailID,
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    PreviousStock = x.Detail.Status,
                    IMENO = x.Detail.IMENO,
                    MRPRate = x.Detail.SRate,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    OfferDescription = x.Offer.Description,
                    ProductType = x.Product.ProductType
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string>(
                        x.PreviousStock, x.IMENO, x.MRPRate, x.DetailId, x.ColorId, x.ColorName, x.OfferDescription)
                )).ToList();
        }

        public static IEnumerable<Tuple<int, string, string, string, string>> GetProductDetail(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository)
        {
            var result = (from p in productRepository.All
                          join c in categoryRepository.All on p.CategoryID equals c.CategoryID
                          join com in companyRepository.All on p.CompanyID equals com.CompanyID
                          select new
                          {
                              p.ProductID,
                              p.Code,
                              p.ProductName,
                              CategoryName = c.Description,
                              CompanyName = com.Name
                          }).ToList();
            return result.Select(i => new Tuple<int, string, string, string, string>(i.ProductID, i.Code, i.ProductName, i.CategoryName, i.CompanyName));
        }


        /// <summary>
        /// In House Damage Product Details
        /// </summary>
        public static IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string>>>>
        GetAllDamageProductFromDetail(this IBaseRepository<Product> productRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
        IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
        IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<SOrder> SOrderRepository,
        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository
            )
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> damageStockdetails = from sod in SOrderDetailRepository.All
                                                         join so in SOrderRepository.All on sod.SOrderID equals so.SOrderID
                                                         join std in stockDetailRepository.All on sod.SDetailID equals std.SDetailID
                                                         where (sod.RepOrderID != null && sod.RStockDetailID != null && so.Status != 10)
                                                         select std;

            var DamageReturns = from po in POrderRepository.All
                                join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                join popd in POProductDetailRepository.All on pod.POrderDetailID equals popd.POrderDetailID
                                where po.Status == (int)EnumPurchaseType.DamageReturn
                                select popd.IMENO;

            var details = damageStockdetails.Where(i => !DamageReturns.Contains(i.IMENO));

            IQueryable<SaleOffer> saleOffer = saleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);
            var items = details.Join(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { Detail = d, Product = p }).
                Join(companies, dp => dp.Product.CompanyID, c => c.CompanyID,
                (dp, c) => new { Product = dp.Product, Detail = dp.Detail, Company = c }).
                Join(categories, dpc => dpc.Product.CategoryID, c => c.CategoryID,
                (dpc, c) => new { Product = dpc.Product, Detail = dpc.Detail, Company = dpc.Company, Category = c }).
                Join(stocks, dpccm => dpccm.Detail.StockID, s => s.StockID,
                (dpccm, s) => new { Product = dpccm.Product, Detail = dpccm.Detail, Company = dpccm.Company, Category = dpccm.Category, Stock = s }).
                Join(colors, dpccms => dpccms.Detail.ColorID, c => c.ColorID,
                (dpccms, c) => new { Product = dpccms.Product, Detail = dpccms.Detail, Company = dpccms.Company, Category = dpccms.Category, Stock = dpccms.Stock, Color = c }).
                GroupJoin(saleOffer, dpccmsc => dpccmsc.Product.ProductID, o => o.ProductID,
                (dpccmsc, o) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, dpccmsc.Color, Offer = o })
                .SelectMany(
          dpccmsc => dpccmsc.Offer.DefaultIfEmpty(),
          (dpccmsc, o) => new { Product = dpccmsc.Product, Detail = dpccmsc.Detail, Company = dpccmsc.Company, Category = dpccmsc.Category, Stock = dpccmsc.Stock, dpccmsc.Color, Offer = o }).
                Where(x => x.Detail.Status == 2).
                Select(x => new
                {
                    DetailId = x.Detail.SDetailID,
                    ProductId = x.Product.ProductID,
                    ProductCode = x.Product.Code,
                    x.Product.ProductName,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    CategoryName = x.Category.Description,
                    CompanyName = x.Company.Name,
                    PreviousStock = x.Detail.Status,
                    IMENO = x.Detail.IMENO,
                    MRPRate = x.Detail.SRate,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    OfferDescription = x.Offer.Description,
                    ProductType = x.Product.ProductType,
                    x.Product.CompressorWarrentyMonth,
                    x.Product.PanelWarrentyMonth,
                    x.Product.MotorWarrentyMonth,
                    x.Product.SparePartsWarrentyMonth,
                    x.Product.ServiceWarrentyMonth
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string>>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string>>(
                        x.PreviousStock, x.IMENO, x.MRPRate, x.DetailId, x.ColorId, x.ColorName,
                        x.OfferDescription,
                        new Tuple<string, string, string, string, string>(x.CompressorWarrentyMonth, x.PanelWarrentyMonth, x.MotorWarrentyMonth, x.SparePartsWarrentyMonth, x.ServiceWarrentyMonth)
                        )
                )).ToList();
        }


        public static IEnumerable<Tuple<int, string, string, decimal,
string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>> GetAllProductDetails(this IBaseRepository<Product> productRepository,
IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Stock> stockRepository, IBaseRepository<Godown> godownRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            var godowns = godownRepository.All;
            //    var models = modelRepository.All;
            var items = (from stock in stocks
                         join product in products on stock.ProductID equals product.ProductID
                         join cat in categories on product.CategoryID equals cat.CategoryID
                         join com in companies on product.CompanyID equals com.CompanyID
                         // join mod in models on product.ModelID equals mod.ModelID
                         join gdown in godowns on stock.GodownID equals gdown.GodownID
                         select new
                         {
                             ProductId = product.ProductID,
                             ProductCode = product.Code,
                             product.ProductName,
                             PWdiscount = 0,
                             product.PicturePath,
                             CategoryName = cat.Description,
                             CompanyName = com.Name,
                             PreviousStock = stock.Quantity,
                             ModelName = "",
                             PKTSheet = 0,
                             SalesRate = 0,
                             GodownID = gdown.GodownID,
                             GodownName = gdown.Name,
                             MRPRate = stock.LPPrice

                         }).GroupBy(x => new
                         {
                             x.ProductId,
                             x.ProductName,
                             x.ProductCode, /*x.Product.PWDiscount,*/
                             x.PicturePath,
                             x.CategoryName,
                             x.CompanyName,
                             ModelDescription = x.ModelName,
                             x.GodownID,
                             x.GodownName
                         }).
                Select(x => new
                {
                    ProductId = x.Key.ProductId,
                    ProductCode = x.Key.ProductCode,
                    x.Key.ProductName,
                    //x.Key.PWDiscount,
                    x.Key.PicturePath,
                    CategoryName = x.Key.CategoryName,
                    CompanyName = x.Key.CompanyName,
                    ModelName = x.Key.ModelDescription,
                    PreviousStock = (decimal?)x.Where(s => s != null).Sum(s => s.PreviousStock),
                    PKTSheet = x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet,
                    SalesRate = x.FirstOrDefault().SalesRate,
                    GodownID = x.FirstOrDefault() == null ? 0 : x.FirstOrDefault().GodownID,
                    GodownName = x.FirstOrDefault() == null ? "" : x.FirstOrDefault().GodownName,
                    Packet = (x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet) != 0 ? (int)(((decimal?)x.Where(s => s != null).Sum(s => s.PreviousStock)) / (x.FirstOrDefault() == null ? 0m : x.FirstOrDefault().PKTSheet)) : 0,
                    MRPRate = x.FirstOrDefault().MRPRate
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>
          (
              x.ProductId,
              x.ProductCode,
              x.ProductName,
              //x.PWDiscount,
              x.MRPRate,
              x.PicturePath,
              x.CategoryName,
              x.CompanyName,
              new Tuple<decimal?, string, decimal, decimal, int, string, int>(
                  x.PreviousStock,
                  x.ModelName,
                  x.PKTSheet,
                  x.SalesRate,
                  x.GodownID,
                  x.GodownName,
                  x.Packet
                  )
          )).ToList();


        }

        public static IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>>
            GetAllSalesProductByCustomerID(this IBaseRepository<Product> productRepository,
                                IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
                                IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
                                IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> salesOrderDetailsRepository,
                                IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                                IBaseRepository<Godown> godownRepository,
                                int CustomerID)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;
            IQueryable<SOrderDetail> sOrderDetails = salesOrderDetailsRepository.All;
            IQueryable<Godown> godows = godownRepository.All;
            var CustomerAllSales = salesOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.Status == (int)EnumSalesType.Sales);
            List<ProductDetailsModel> SalesStockDetails = new List<ProductDetailsModel>();

            var Creditsales = (from so in CreditSaleRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.IsReturn == 0 && i.CustomerID == CustomerID)
                               join sod in CreditSaleDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                               where sod.IsProductReturn == 0 && (so.NetAmount - so.DownPayment == so.Remaining)
                               select new ProductDetailsModel
                               {
                                   StockDetailsId = sod.StockDetailID,
                                   CashSalesRate = sod.UnitPrice,
                                   SalesQty = sod.Quantity,
                               }).ToList();

            var CustomerSalesDetails = (from so in CustomerAllSales
                                        join sod in sOrderDetails on so.SOrderID equals sod.SOrderID
                                        where sod.IsProductReturn == 0
                                        select new ProductDetailsModel
                                        {
                                            StockDetailsId = sod.RStockDetailID == null ? sod.SDetailID : (int)sod.RStockDetailID,
                                            CashSalesRate = sod.RStockDetailID == null ? (sod.UnitPrice - sod.PPDAmount / sod.Quantity) : (decimal)sod.RepUnitPrice,
                                            SalesQty = sod.Quantity - sod.RQuantity,
                                        }).ToList();

            SalesStockDetails.AddRange(Creditsales);
            SalesStockDetails.AddRange(CustomerSalesDetails);

            var ret_items = (from sa in SalesStockDetails
                             join f in details on sa.StockDetailsId equals f.SDetailID
                             join p in products on f.ProductID equals p.ProductID
                             join com in companies on p.CompanyID equals com.CompanyID
                             join cat in categories on p.CategoryID equals cat.CategoryID
                             join st in stocks on f.StockID equals st.StockID
                             join clr in colors on f.ColorID equals clr.ColorID
                             join god in godows on f.GodownID equals god.GodownID
                             select new
                             {
                                 DetailId = f.SDetailID,
                                 ProductId = p.ProductID,
                                 ProductCode = p.Code,
                                 p.ProductName,
                                 p.PWDiscount,
                                 p.PicturePath,
                                 CategoryName = cat.Description,
                                 CompanyName = com.Name,
                                 PreviousStock = st.Quantity,
                                 IMENO = f.IMENO,
                                 MRPRate = sa.CashSalesRate,
                                 ColorId = clr.ColorID,
                                 ColorName = clr.Name,
                                 GodownName = god.Name,
                                 SalesQty = sa.SalesQty
                             });

            var itemsGroupBY = (from vm in ret_items
                                group vm by new
                                {
                                    vm.ProductId,
                                    vm.ProductCode,
                                    vm.ProductName,
                                    vm.PicturePath,
                                    vm.ColorId,
                                    vm.ColorName,
                                    vm.CategoryName,
                                    vm.CompanyName,
                                    vm.IMENO,
                                    vm.GodownName
                                } into g
                                select new
                                {
                                    IMENO = g.Key.IMENO,
                                    ProductId = g.Key.ProductId,
                                    ProductCode = g.Key.ProductCode,
                                    ProductName = g.Key.ProductName,
                                    CategoryName = g.Key.CategoryName,
                                    CompanyName = g.Key.CompanyName,
                                    ColorName = g.Key.ColorName,
                                    ColorId = g.Key.ColorId,
                                    PicturePath = g.Key.PicturePath,
                                    DetailId = g.Select(o => o.DetailId).FirstOrDefault(),
                                    MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                    PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                                    SalesQty = g.Sum(o => o.SalesQty),
                                    PreviousStock = g.Sum(o => o.SalesQty),
                                    GodownName = g.Key.GodownName
                                });
            return itemsGroupBY.Select(x => new Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>(
                        x.SalesQty,
                        x.IMENO,
                        x.MRPRate,
                        x.DetailId,
                        x.ColorId,
                        x.ColorName,
                        "",
                        new Tuple<string>(x.GodownName))
                )).ToList();
        }


        public static IEnumerable<ProductDetailsModel> GetSalesDetailByCustomerID(this IBaseRepository<Product> productRepository,
                            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
                            IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
                            IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> salesOrderDetailsRepository,
                            IBaseRepository<Godown> godownRepository,
                            int CustomerID, String IMEI)
        {
            IQueryable<StockDetail> StockDetails = null;
            if (string.IsNullOrEmpty(IMEI))
                StockDetails = stockDetailRepository.All;
            else
                StockDetails = stockDetailRepository.All.Where(i => i.IMENO.Equals(IMEI));

            var SalesDetails = from so in salesOrderRepository.All
                               join sod in salesOrderDetailsRepository.All on so.SOrderID equals sod.SOrderID
                               join sd in StockDetails on sod.SDetailID equals sd.SDetailID
                               join p in productRepository.All on sod.ProductID equals p.ProductID
                               join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in companyRepository.All on p.CompanyID equals com.CompanyID
                               join col in colorRepository.All on sd.ColorID equals col.ColorID
                               join g in godownRepository.All on sd.GodownID equals g.GodownID
                               //join off in saleOfferRepository.All on p.ProductID equals off.ProductID
                               where so.CustomerID == CustomerID && so.Status == (int)EnumSalesType.Sales
                               && (p.ProductType == (int)EnumProductType.NoBarcode ? sod.Quantity - sod.RQuantity > 0 : sd.Status == (int)EnumStockStatus.Sold)
                               && sod.RepOrderID == null && sod.IsProductReturn == 0
                               select new ProductDetailsModel
                               {
                                   ProductId = sod.ProductID,
                                   StockDetailsId = sd.SDetailID,
                                   ProductName = p.ProductName,
                                   ProductCode = p.Code,
                                   ColorId = col.ColorID,
                                   ColorName = col.Name,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   IMENo = sd.IMENO,
                                   MRPRate = sod.SRate,
                                   //MRPRate = Math.Round(((sod.UnitPrice - (sod.PPDAmount * sod.Quantity) / sod.Quantity) - (((sod.UnitPrice - sod.PPDAmount / sod.Quantity) * (so.TDAmount + so.AdjAmount)) / (so.GrandTotal - so.NetDiscount + (so.TDAmount + so.AdjAmount)))), 2),
                                   ProductType = p.ProductType,
                                   SalesQty = sod.Quantity,
                                   PreStock = sod.Quantity - sod.RQuantity,
                                   GodownID = g.GodownID,
                                   SOrderDetailID = sod.SOrderDetailID,
                                   StockID = sd.StockID,
                                   SOrderDetailsId = sod.SOrderDetailID,
                               };

            return SalesDetails;
        }

        public static IEnumerable<ProductDetailsModel> GetCreditSalesDetailByCustomerID(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
            IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<CreditSaleDetails> screditSaleDetailsRepository,
            int CustomerID, String IMEI)
        {
            IQueryable<StockDetail> StockDetails = null;
            if (string.IsNullOrEmpty(IMEI))
                StockDetails = stockDetailRepository.All;
            else
                StockDetails = stockDetailRepository.All.Where(i => i.IMENO.Equals(IMEI));

            var SalesDetails = from so in creditSalesRepository.All
                               join sod in screditSaleDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                               join sd in StockDetails on sod.StockDetailID equals sd.SDetailID
                               join p in productRepository.All on sod.ProductID equals p.ProductID
                               join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in companyRepository.All on p.CompanyID equals com.CompanyID
                               join col in colorRepository.All on sd.ColorID equals col.ColorID
                               //join off in saleOfferRepository.All on p.ProductID equals off.ProductID
                               where so.CustomerID == CustomerID && so.IsStatus == EnumSalesType.Sales
                               && (p.ProductType == (int)EnumProductType.NoBarcode ? 0 > 0 : sd.Status == (int)EnumStockStatus.Sold)
                               && so.IsReturn == 0
                               select new ProductDetailsModel
                               {
                                   ProductId = sod.ProductID,
                                   StockDetailsId = sd.SDetailID,
                                   ProductMemo = so.InvoiceNo,
                                   ProductName = p.ProductName,
                                   ProductCode = p.Code,
                                   ColorId = col.ColorID,
                                   ColorName = col.Name,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   IMENo = sd.IMENO,
                                   MRPRate = 0,
                                   ProductType = p.ProductType,
                                   SalesQty = sod.Quantity,
                                   PreStock = sod.Quantity,
                                   HireSalesId = so.CreditSalesID,
                                   CreditSaleDetailsId = sod.CreditSaleDetailsID
                               };

            return SalesDetails;
        }

        public static IEnumerable<Tuple<int, string, string, decimal,
          string, string, string, Tuple<decimal?, int, decimal>>> GetAllUniqueProduct(this IBaseRepository<Product> productRepository,
          IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Stock> stockRepository,
          IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<Color> colorRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            var StockDetails = stockDetailRepository.All;
            var colors = colorRepository.All;

            var items = products.GroupJoin(stocks, p => p.ProductID, s => s.ProductID,
                (p, s) => new { Product = p, Stocks = s }).
                SelectMany(s => s.Stocks.DefaultIfEmpty(), (p, s) => new { Product = p.Product, Stock = s }).
                GroupJoin(companies, ps => ps.Product.CompanyID, c => c.CompanyID,
                (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Companies = c }).
                SelectMany(c => c.Companies.DefaultIfEmpty(), (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Company = c }).
                GroupJoin(categories, psc => psc.Product.CategoryID, c => c.CategoryID,
                (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc.Company, Categories = c }).
                SelectMany(c => c.Categories.DefaultIfEmpty(), (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc, Category = c }).
                GroupJoin(colors, pscc => pscc.Stock.ColorID, c => c.ColorID,
                (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Category = pscc.Category, Company = pscc.Company, colors = c }).
                SelectMany(c => c.colors.DefaultIfEmpty(), (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Company = pscc.Company, Category = pscc.Category, Color = c }).
                GroupBy(x => new
                {
                    x.Product.ProductID,
                    x.Product.ProductName,
                    x.Product.Code,
                    x.Product.RP,
                    x.Product.PicturePath,
                    x.Category.Description,
                    x.Company.Company.Name,
                    x.Product.ProductType,
                    x.Product.MRP
                }).
                Select(x => new
                {
                    ProductId = x.Key.ProductID,
                    ProductCode = x.Key.Code,
                    x.Key.ProductName,
                    x.Key.RP,
                    x.Key.PicturePath,
                    CategoryName = x.Key.Description,
                    CompanyName = x.Key.Name,
                    PreviousStock = (decimal?)x.Where(s => s.Stock != null).Sum(s => s.Stock.Quantity),

                    // PreviousStock = StockDetails.Where(i => i.ProductID == x.Key.ProductID && i.ColorID == x.Key.ColorID && i.GodownID == x.Key.GodownID && i.Status == (int)EnumStockStatus.Stock).Sum(o => o.Quantity),

                    x.Key.ProductType,
                    MRP = (decimal)x.Key.MRP
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, int, decimal>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.RP,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, int, decimal>(
                        x.PreviousStock,
                        x.ProductType,
                        x.MRP
                        )
                )).ToList();
        }

        public static IEnumerable<Tuple<int, string, string, decimal,
            string, string, string, Tuple<decimal?, int, decimal, decimal>>> GetAllECOMProduct(this IBaseRepository<Product> productRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Stock> stockRepository,
            IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<Color> colorRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Category> categories = categoryRepository.All;
            IQueryable<Company> companies = companyRepository.All;
            IQueryable<Stock> stocks = stockRepository.All;
            var StockDetails = stockDetailRepository.All;
            var colors = colorRepository.All;

            var items = products.GroupJoin(stocks, p => p.ProductID, s => s.ProductID,
                (p, s) => new { Product = p, Stocks = s }).
                SelectMany(s => s.Stocks.DefaultIfEmpty(), (p, s) => new { Product = p.Product, Stock = s }).
                GroupJoin(companies, ps => ps.Product.CompanyID, c => c.CompanyID,
                (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Companies = c }).
                SelectMany(c => c.Companies.DefaultIfEmpty(), (ps, c) => new { Product = ps.Product, Stock = ps.Stock, Company = c }).
                GroupJoin(categories, psc => psc.Product.CategoryID, c => c.CategoryID,
                (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc.Company, Categories = c }).
                SelectMany(c => c.Categories.DefaultIfEmpty(), (psc, c) => new { Product = psc.Product, Stock = psc.Stock, Company = psc, Category = c }).
                GroupJoin(colors, pscc => pscc.Stock.ColorID, c => c.ColorID,
                (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Category = pscc.Category, Company = pscc.Company, colors = c }).
                SelectMany(c => c.colors.DefaultIfEmpty(), (pscc, c) => new { Product = pscc.Product, Stock = pscc.Stock, Company = pscc.Company, Category = pscc.Category, Color = c }).
                GroupBy(x => new
                {
                    x.Product.ProductID,
                    x.Product.ProductName,
                    x.Product.Code,
                    x.Color.ColorID,
                    ColorName = x.Color.Name,
                    x.Product.PWDiscount,
                    x.Product.PicturePath,
                    x.Category.Description,
                    x.Company.Company.Name,
                    x.Product.ProductType,
                    x.Stock.GodownID,
                    x.Product.RP,
                    x.Product.MRP
                }).
                Select(x => new
                {
                    ProductId = x.Key.ProductID,
                    ProductCode = x.Key.Code,
                    x.Key.ProductName,
                    x.Key.PWDiscount,
                    x.Key.PicturePath,
                    CategoryName = x.Key.Description,
                    CompanyName = x.Key.Name,
                    PreviousStock = (decimal?)x.Where(s => s.Stock != null).Sum(s => s.Stock.Quantity),

                    // PreviousStock = StockDetails.Where(i => i.ProductID == x.Key.ProductID && i.ColorID == x.Key.ColorID && i.GodownID == x.Key.GodownID && i.Status == (int)EnumStockStatus.Stock).Sum(o => o.Quantity),

                    x.Key.ProductType,
                    x.Key.RP,
                    MRP = x.Key.MRP.HasValue ? (decimal)x.Key.MRP : 0m
                }).ToList();

            return items.Select(x => new Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>
                (
                    x.ProductId,
                    x.ProductCode,
                    x.ProductName,
                    x.PWDiscount,
                    x.PicturePath,
                    x.CategoryName,
                    x.CompanyName,
                    new Tuple<decimal?, int, decimal, decimal>(
                        x.PreviousStock,
                        x.ProductType,
                        x.MRP,
                        x.RP
                        )
                )).ToList();
        }

        public static IQueryable<ProductWisePurchaseModel> GetDOProducts(this IBaseRepository<Product> productRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository)
        {
            var Products = from p in productRepository.All
                           join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                           join com in companyRepository.All on p.CompanyID equals com.CompanyID
                           select new ProductWisePurchaseModel
                           {
                               ProductID = p.ProductID,
                               ProductCode = cat.Description.ToLower().Equals("tiles") ? p.Code : p.Code,
                               ProductName = p.ProductName,
                               CompanyName = com.Name,
                               CategoryName = cat.Description,
                               CategoryID = cat.CategoryID,
                               ProductType = p.ProductType,
                           };
            return Products;
        }

        public static IQueryable<ProductWisePurchaseModel> GetAllProductIQueryablePO(this IBaseRepository<Product> productRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository,
        IBaseRepository<SaleOffer> offerRepository)
        {
            var products = from p in productRepository.All
                           join com in companyRepository.All on p.CompanyID equals com.CompanyID
                           join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID
                           join off in offerRepository.All on p.ProductID equals off.ProductID into lp
                           from off in lp.DefaultIfEmpty()
                           select new ProductWisePurchaseModel
                           {
                               ProductID = p.ProductID,
                               ProductCode = p.Code,
                               ProductName = p.ProductName,
                               CompanyName = com.Name,
                               CompanyID = com.CompanyID,
                               CategoryName = cat.Description,
                               CategoryID = cat.CategoryID,
                               ProductType = p.ProductType,
                               UnitType = p.UnitType,
                               PWDiscount = p.PWDiscount,
                               MRP = p.MRP.HasValue ? (decimal)p.MRP : 0m,
                               CompressorWarrentyMonth = p.CompressorWarrentyMonth,
                               MotorWarrentyMonth = p.MotorWarrentyMonth,
                               PanelWarrentyMonth = p.PanelWarrentyMonth,
                               ServiceWarrentyMonth = p.ServiceWarrentyMonth,
                               SparePartsWarrentyMonth = p.SparePartsWarrentyMonth,
                               OfferDescription = off != null ? off.Description : ""
                           };
            //var test = products.Where(p => p.ProductID == 200675).ToList();
            return products;
        }

        public static List<ProductWisePurchaseModel> GetAllProductIQueryableNew(this IBaseRepository<Product> productRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Company> companyRepository, IBaseRepository<SaleOffer> offerRepository,
        IBaseRepository<Stock> StockRepository, IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailsRepository)
        {
            var StockDetails = StockDetailsRepository.All;
            var products = (from p in productRepository.All
                           join com in companyRepository.All on p.CompanyID equals com.CompanyID
                           join cat in categoryRepository.All on p.CategoryID equals cat.CategoryID                       
                           join st in StockRepository.All on p.ProductID equals st.ProductID into lst
                           from st in lst.DefaultIfEmpty()
                           join off in offerRepository.All on p.ProductID equals off.ProductID into lp
                           from off in lp.DefaultIfEmpty()
                           join col in colorRepository.All on st.ColorID equals col.ColorID into lcol
                           from col in lcol.DefaultIfEmpty()
                           select new ProductWisePurchaseModel
                           {
                               StockID = st != null ? st.StockID : 0,
                               ProductID = p.ProductID,
                               ProductCode = p.Code,
                               ProductName = p.ProductName,
                               CompanyName = com.Name,
                               CompanyID = com.CompanyID,
                               CategoryName = cat.Description,
                               CategoryID = cat.CategoryID,
                               ProductType = p.ProductType,
                               UnitType = p.UnitType,
                               PWDiscount = p.PWDiscount,
                               PrevStq = st != null ? st.Quantity : 0,
                               MRP = p.MRP.HasValue ? (decimal)p.MRP : 0m,
                               CompressorWarrentyMonth = p.CompressorWarrentyMonth,
                               MotorWarrentyMonth = p.MotorWarrentyMonth,
                               PanelWarrentyMonth = p.PanelWarrentyMonth,
                               ServiceWarrentyMonth = p.ServiceWarrentyMonth,
                               SparePartsWarrentyMonth = p.SparePartsWarrentyMonth,
                               OfferDescription = off != null ? off.Description : "",
                               ColorName = col != null ? col.Name : "",
                               ColorID = st != null ? st.ColorID : 0,
                               GodownID = st != null ? st.GodownID : 0

                           }).ToList();

            var itemsGroupBY = (from vm in products
                                group vm by new
                                {
                                    vm.ProductID,
                                    vm.CompanyID,
                                    vm.CategoryID,
                                    vm.ProductType

                                } into g
                                select new ProductWisePurchaseModel
                                {
                                    StockID = g.FirstOrDefault().StockID,
                                    ProductID = g.Key.ProductID,
                                    ProductCode = g.FirstOrDefault().ProductCode,
                                    ProductName = g.FirstOrDefault().ProductName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CompanyID = g.Key.CompanyID,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    CategoryID = g.Key.CategoryID,
                                    ProductType = g.Key.ProductType,
                                    UnitType = g.FirstOrDefault().UnitType,
                                    PWDiscount = g.FirstOrDefault().PWDiscount,
                                    //PrevStq  = StockDetails.Any(i => i.ProductID == g.Key.ProductID && i.ColorID == g.Key.ColorID && i.GodownID == g.Key.GodownID && i.Status == (int)EnumStockStatus.Stock)
                                    //? g.Key.ProductType == (int)EnumProductType.NoBarcode
                                    //        ? StockDetails.Where(i => i.ProductID == g.Key.ProductID && i.ColorID == g.Key.ColorID && i.GodownID == g.Key.GodownID && i.Status == (int)EnumStockStatus.Stock).Sum(o => o.Quantity)
                                    //        : StockDetails.Count(i => i.ProductID == g.Key.ProductID && i.ColorID == g.Key.ColorID && i.GodownID == g.Key.GodownID && i.Status == (int)EnumStockStatus.Stock)
                                    //: 0m,
                                    PrevStq = g.FirstOrDefault().PrevStq,
                                    MRP = g.FirstOrDefault().MRP,
                                    CompressorWarrentyMonth = g.FirstOrDefault().CompressorWarrentyMonth,
                                    MotorWarrentyMonth = g.FirstOrDefault().MotorWarrentyMonth,
                                    PanelWarrentyMonth = g.FirstOrDefault().PanelWarrentyMonth,
                                    ServiceWarrentyMonth = g.FirstOrDefault().ServiceWarrentyMonth,
                                    SparePartsWarrentyMonth = g.FirstOrDefault().SparePartsWarrentyMonth,
                                    OfferDescription = g.FirstOrDefault().OfferDescription,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    ColorID = g.FirstOrDefault().ColorID
                                }).ToList();

            return itemsGroupBY;
        }

    }
}
