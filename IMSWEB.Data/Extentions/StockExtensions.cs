using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class StockExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string, string, decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>>>
            GetAllStockAsync(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Godown> GodownRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Company> CompanyRepository,
            int ConcernID, bool IsVATManager, int page, int pageSize)
        {
            IQueryable<Product> products = productRepository.All.AsNoTracking();
            IQueryable<Color> colors = colorRepository.All.AsNoTracking();
            var StockDetails = StockDetailRepository.All.AsNoTracking();
            var Godowns = GodownRepository.All.AsNoTracking();


            var items = await stockRepository.All.AsNoTracking().Join(products,
                stk => stk.ProductID, prod => prod.ProductID, (stk, prod) => new { Stock = stk, Product = prod }).
                Join(colors, sp => sp.Stock.ColorID, c => c.ColorID,
                (sp, c) => new { Product = sp.Product, Stock = sp.Stock, Color = c }).
                Join(Godowns, sp => sp.Stock.GodownID, g => g.GodownID,
                (sp, g) => new { Product = sp.Product, Stock = sp.Stock, Godown = g, Color = sp.Color }).
                Select(x => new ProductDetailsModel
                {
                    StockID = x.Stock.StockID,
                    ProductCode = x.Stock.StockCode,
                    ProductName = x.Product.ProductName,
                    CompanyName = x.Product.Company.Name,
                    StockQty = x.Product.ProductType == (int)EnumProductType.NoBarcode ? x.Stock.Quantity : StockDetails.Where(i => i.ProductID == x.Product.ProductID && i.GodownID == x.Godown.GodownID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).Count(),
                    LPPrice = x.Stock.LPPrice,
                    MRPPrice = x.Stock.MRPPrice,
                    ColorName = x.Color.Name,
                    ProductId = x.Product.ProductID,
                    ColorId = x.Color.ColorID,
                    SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,
                    CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CreditSRate : 0m,
                    CreditSalesPrice3 = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate3Month : 0m,
                    CreditSalesPrice12 = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate12Month : 0m,
                    GodownName = x.Godown.Name

                }).Where(x => x.StockQty > 0)
                .OrderByDescending(x => x.StockID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //var items = await (from s in stockRepository.All
            //             join p in products on s.ProductID equals p.ProductID
            //             join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
            //             join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
            //             join g in GodownRepository.All on s.GodownID equals g.GodownID
            //             join c in colors on s.ColorID equals c.ColorID
            //             select new ProductDetailsModel
            //             {
            //                 StockID = s.StockID,
            //                 ProductCode = s.StockCode,
            //                 ProductName = p.ProductName,
            //                 CompanyName = com.Name,
            //                 StockQty = p.ProductType == (int)EnumProductType.NoBarcode ? s.Quantity : StockDetails.Where(i => i.ProductID == p.ProductID && i.GodownID == g.GodownID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).Count(),
            //                 LPPrice = s.LPPrice,
            //                 MRPPrice = s.MRPPrice,
            //                 ColorName = c.Name,
            //                 ProductId = p.ProductID,
            //                 ColorId = c.ColorID,
            //                 SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,
            //                 CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).CreditSRate : 0m,
            //                 CreditSalesPrice3 = StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate3Month : 0m,
            //                 CreditSalesPrice12 = StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate12Month : 0m,
            //                 GodownName = g.Name

            //             }).Where(x => x.StockQty > 0).ToListAsync();


            List<ProductDetailsModel> finalData = new List<ProductDetailsModel>();
            if (IsVATManager)
            {
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID);
                decimal FalesStock = (items.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                decimal FalesStockCount = 0m;

                foreach (var item in items)
                {
                    FalesStockCount += item.StockQty;
                    if (FalesStockCount <= FalesStock)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return items.Select(x => new Tuple<int, string, string, string, decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>
                (
                    x.StockID,
                    x.ProductCode,
                    x.ProductName,
                    x.CompanyName,
                    x.StockQty,
                    x.LPPrice,
                    x.MRPPrice,
                    new Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>
                        (
                        x.ColorName,
                        x.ProductId,
                        x.ColorId,
                        x.SalesPrice,
                        x.CreditSalesPrice,
                        x.CreditSalesPrice3,
                        x.CreditSalesPrice12,
                        new Tuple<string>
                        (x.GodownName)
                        )
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<string>>>>
            GetAllStockDetailAsync(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Godown> GodownRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            int ConcernID, bool IsVATManager)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> stockDetail = stockDetailRepository.All;
            IQueryable<Godown> godownDetail = GodownRepository.All;

            var items = await stockDetailRepository.All.Join(products,
                stkDetail => stkDetail.ProductID, prod => prod.ProductID, (stkDetail, prod) => new { StockDetail = stkDetail, Product = prod }).
                Join(colors, sp => sp.StockDetail.ColorID, c => c.ColorID,
                (sp, c) => new { Product = sp.Product, StockDetail = sp.StockDetail, Color = c }).
                 Join(godownDetail, sp => sp.StockDetail.GodownID, g => g.GodownID,
                (spg, g) => new { Product = spg.Product, StockDetail = spg.StockDetail, Color = spg.Color, Godown = g }).
                Select(x => new ProductDetailsModel
                {
                    StockID = x.StockDetail.SDetailID,
                    ProductCode = x.StockDetail.StockCode,
                    ProductName = x.Product.ProductName,
                    CompanyName = x.Product.Company.Name,
                    IMENo = x.StockDetail.IMENO,
                    Status = x.StockDetail.Status,
                    ColorName = x.Color.Name,
                    GodownName = x.Godown.Name,
                }).ToListAsync();

            List<ProductDetailsModel> finalData = new List<ProductDetailsModel>();
            if (IsVATManager)
            {
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID);
                decimal FalesStock = (items.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                decimal FalesStockCount = 0m;

                foreach (var item in items)
                {
                    FalesStockCount += item.StockQty;
                    if (FalesStockCount <= FalesStock)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<string>>
                (
                    x.StockID,
                    x.ProductCode,
                    x.ProductName,
                    x.CompanyName,
                    x.IMENo,
                    x.Status.ToString(),
                    x.ColorName,
                    new Tuple<string>(x.GodownName)

                )).OrderByDescending(x => x.Item1).ToList();
        }


        public static IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
            GetforReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Godown> GodownRepository, IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<ParentCategory> ParentCategoryRepository,
            string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType)
        {
            var Products = productRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);
            var StockDetails = StockDetailRepository.All;
            var PCategory = ParentCategoryRepository.All;
            if (PCategoryID != 0)
                PCategory = PCategory.Where(i => i.PCategoryID == PCategoryID);


            var Godowns = GodownRepository.All;
            var Colors = colorRepository.All;

            if (GodownID != 0)
                Godowns = Godowns.Where(o => o.GodownID == GodownID);

            if (ColorID != 0)
                Colors = Colors.Where(o => o.ColorID == ColorID);

            List<ProductDetailsModel> finalData = new List<ProductDetailsModel>();

            if (StockType == 2)
            {
                var data = (from st in stockRepository.All
                            join sd in StockDetailRepository.All on st.StockID equals sd.StockID
                            join pro in Products on st.ProductID equals pro.ProductID
                            join col in Colors on st.ColorID equals col.ColorID
                            join god in Godowns on st.GodownID equals god.GodownID
                            join pc in PCategory on pro.Category.PCategoryID equals pc.PCategoryID
                            where sd.Status == (int)EnumStockStatus.Stock && sd.IsDamage == 1
                            select new
                            {
                                StockID = st.StockID,
                                productName = pro.ProductName,
                                CompanyName = pro.Company.Name,
                                CategoryName = pro.Category.Description,
                                ColorName = col.Name,
                                GodownName = god.Name,
                                Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : 1,
                                //  Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.GodownID == st.GodownID && i.Status == (int)EnumStockStatus.Stock).Count(),
                                MRPRate = st.MRPPrice,
                                sd.PRate,
                                sd.SRate,
                                sd.CRSalesRate3Month,
                                sd.CreditSRate,
                                sd.CRSalesRate12Month,
                                sd.IMENO,
                                PCategoryName = pc.Name,
                                ProMRP = pro.MRP
                                //StockDetails = StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.GodownID==st.GodownID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault(),
                                //SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                                //    ? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().SRate : 0m,
                                //CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                                //? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().CreditSRate : 0m
                            }).Where(st => st.Qty > 0).ToList();

                var oStockData = (from s in data
                                  group s by new
                                  {
                                      s.productName,
                                      s.CompanyName,
                                      s.StockID,
                                      s.ColorName,
                                      s.GodownName,
                                      //s.PRate,
                                      s.CategoryName,
                                      s.PCategoryName,
                                      s.ProMRP
                                  } into g
                                  select new ProductDetailsModel
                                  {
                                      StockID = g.Key.StockID,
                                      ProductName = g.Key.productName,
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      ColorName = g.Key.ColorName,
                                      GodownName = g.Key.GodownName,
                                      StockQty = g.Sum(i => i.Qty),
                                      MRPRate = g.Select(i => i.PRate).FirstOrDefault(),
                                      SalesPrice = g.Select(i => i.SRate).FirstOrDefault(),
                                      CreditSalesPrice3 = g.Select(i => i.CRSalesRate3Month).FirstOrDefault(),
                                      CreditSalesPrice = g.Select(i => i.CreditSRate).FirstOrDefault(),
                                      CreditSalesPrice12 = g.Select(i => i.CRSalesRate12Month).FirstOrDefault(),
                                      IMEIList = g.Select(i => i.IMENO).ToList(),
                                      PCategoryName = g.Key.PCategoryName,
                                      ProMRP = (decimal)g.Key.ProMRP
                                  }).ToList();


                if (IsVATManager)
                {
                    var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                    decimal FalesStock = (oStockData.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                    decimal FalesStockCount = 0m;

                    foreach (var item in oStockData)
                    {
                        FalesStockCount += item.StockQty;
                        if (FalesStockCount <= FalesStock)
                            finalData.Add(item);
                        else
                            break;
                    }
                }
                else
                    finalData = oStockData;
            }
            else
            {
                var data = (from st in stockRepository.All
                            join sd in StockDetailRepository.All on st.StockID equals sd.StockID
                            join pro in Products on st.ProductID equals pro.ProductID
                            join col in Colors on st.ColorID equals col.ColorID
                            join god in Godowns on st.GodownID equals god.GodownID
                            join pc in PCategory on pro.Category.PCategoryID equals pc.PCategoryID
                            where sd.Status == (int)EnumStockStatus.Stock && sd.IsDamage == 0
                            select new
                            {
                                StockID = st.StockID,
                                productName = pro.ProductName,
                                CompanyName = pro.Company.Name,
                                CategoryName = pro.Category.Description,
                                ColorName = col.Name,
                                GodownName = god.Name,
                                Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : 1,
                                //  Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.GodownID == st.GodownID && i.Status == (int)EnumStockStatus.Stock).Count(),
                                MRPRate = st.MRPPrice,
                                sd.PRate,
                                sd.SRate,
                                sd.CRSalesRate3Month,
                                sd.CreditSRate,
                                sd.CRSalesRate12Month,
                                sd.IMENO,
                                PCategoryName = pc.Name,
                                ProMRP = pro.MRP
                                //StockDetails = StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.GodownID==st.GodownID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault(),
                                //SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                                //    ? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().SRate : 0m,
                                //CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                                //? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().CreditSRate : 0m
                            }).Where(st => st.Qty > 0).ToList();

                var oStockData = (from s in data
                                  group s by new
                                  {
                                      s.productName,
                                      s.CompanyName,
                                      s.StockID,
                                      s.ColorName,
                                      s.GodownName,
                                      //s.PRate,
                                      s.CategoryName,
                                      s.PCategoryName,
                                      s.ProMRP
                                  } into g
                                  select new ProductDetailsModel
                                  {
                                      StockID = g.Key.StockID,
                                      ProductName = g.Key.productName,
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      ColorName = g.Key.ColorName,
                                      GodownName = g.Key.GodownName,
                                      StockQty = g.Sum(i => i.Qty),
                                      MRPRate = g.Select(i => i.PRate).FirstOrDefault(),
                                      SalesPrice = g.Select(i => i.SRate).FirstOrDefault(),
                                      CreditSalesPrice3 = g.Select(i => i.CRSalesRate3Month).FirstOrDefault(),
                                      CreditSalesPrice = g.Select(i => i.CreditSRate).FirstOrDefault(),
                                      CreditSalesPrice12 = g.Select(i => i.CRSalesRate12Month).FirstOrDefault(),
                                      IMEIList = g.Select(i => i.IMENO).ToList(),
                                      PCategoryName = g.Key.PCategoryName,
                                      ProMRP = (decimal)g.Key.ProMRP
                                  }).ToList();


                if (IsVATManager)
                {
                    var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                    decimal FalesStock = (oStockData.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                    decimal FalesStockCount = 0m;

                    foreach (var item in oStockData)
                    {
                        FalesStockCount += item.StockQty;
                        if (FalesStockCount <= FalesStock)
                            finalData.Add(item);
                        else
                            break;
                    }
                }
                else
                    finalData = oStockData;
            }






            return finalData.Select(x => new Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>
                                    (
                                    x.StockID,
                                    x.ProductName,
                                    x.CompanyName,
                                    x.CategoryName,
                                    x.ColorName,
                                    x.StockQty,
                                    x.MRPRate,//x.MRPRate,
                                    new Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>
                                        (
                                         x.SalesPrice,
                                         x.CreditSalesPrice,
                                         x.CreditSalesPrice3,
                                         x.ProMRP,
                                         x.GodownName,
                                         x.IMEIList, x.PCategoryName
                                        )
                                    ));
        }

        public static IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> GetPriceProtectionReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository, IBaseRepository<PriceProtection> priceprotectionRepository, string userName, int concernID, DateTime dFDate, DateTime dToDate)
        {
            var oStockData = (from pp in priceprotectionRepository.All
                              join pro in productRepository.All on pp.ProductID equals pro.ProductID
                              join col in colorRepository.All on pp.ColorID equals col.ColorID
                              join sup in suppRepository.All on pp.SupplierID equals sup.SupplierID
                              select new
                              {
                                  SuppName = sup.Name,
                                  productName = pro.ProductName,
                                  PrvPrice = pp.PrvPrice,
                                  ChangePrice = pp.ChangePrice,
                                  Qty = pp.PrvStockQty,
                                  FallAmt = ((pp.PrvPrice - pp.ChangePrice) * pp.PrvStockQty),
                                  CDate = pp.PChangeDate
                              }).ToList();

            return oStockData.Select(x => new Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>
                                (
                                x.SuppName,
                                x.productName,
                                x.PrvPrice,
                                x.ChangePrice,
                                x.Qty,
                                x.FallAmt,
                                x.CDate
                                ));


        }

        public static IEnumerable<Tuple<int, string, string>> GetStockDetailsByID(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                int stockID)
        {
            var Stocks = stockRepository.All.Where(i => i.StockID == stockID);
            var StockDetails = stockDetailRepository.All.Where(i => i.StockID == stockID && i.Status == 1);
            var oStockData = (from st in Stocks
                              join std in StockDetails on st.StockID equals std.StockID
                              select new
                              {
                                  StockID = st.StockID,
                                  SDetailID = std.SDetailID,
                                  StockCode = std.StockCode,
                                  IMEINO = std.IMENO,
                                  Status = std.Status
                              }).ToList();

            return oStockData.Select(x => new Tuple<int, string, string>
                                (
                                x.SDetailID,
                                x.StockCode,
                                x.IMEINO
                                ));
        }

        public static bool CheckStockIMEIForSRVisit(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, int ProductID, int ColorID, string IMEI)
        {
            var Stock = stockRepository.All.FirstOrDefault(i => i.ProductID == ProductID && i.ColorID == ColorID);
            if (Stock != null)
            {
                var StockDetails = stockDetailRepository.All;

                if (StockDetails.Any(i => i.ProductID == Stock.ProductID && i.ColorID == Stock.ColorID && i.IMENO.Equals(IMEI.Trim()) && i.Status == (int)EnumStockStatus.Stock))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static string GetStockProductsHistory(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
    IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
    IBaseRepository<Employee> EmployeeRepository,
    int StockID)
        {
            string History = string.Empty;
            var StockDetails = stockDetailRepository.All.Where(i => i.StockID == StockID && i.Status == (int)EnumStockStatus.Stock);

            var SRStockDetails = (from sv in SRVisitRepository.All
                                  join svd in SRVisitDetailRepository.All on sv.SRVisitID equals svd.SRVisitID
                                  join spd in SRVProductDetailRepository.All on svd.SRVisitDID equals spd.SRVisitDID
                                  join sd in StockDetails on spd.SDetailID equals sd.SDetailID
                                  join emp in EmployeeRepository.All on sv.EmployeeID equals emp.EmployeeID
                                  where spd.Status == (int)EnumSRVProductDetailsStatus.Stock && sv.Status == (int)EnumSRVisitType.Live
                                  select new
                                  {
                                      sv.EmployeeID,

                                      emp.Name,
                                      spd.SDetailID
                                  }).OrderBy(i => i.EmployeeID).ToList();

            var Result = (from sr in SRStockDetails
                          group sr by new { sr.EmployeeID, sr.Name } into g
                          select new
                          {
                              EmployeeID = g.Key.EmployeeID,
                              Name = g.Key.Name,
                              Quantity = g.Count()
                          }).OrderBy(i => i.EmployeeID).ToList();
            if (SRStockDetails.Count() > 0)
                History = "Stock= " + (StockDetails.Count() - SRStockDetails.Count()) + Environment.NewLine;
            else
                History = "Stock= " + (StockDetails.Count() - SRStockDetails.Count());

            int counter = 0;
            foreach (var item in Result)
            {
                counter++;
                if (counter != Result.Count())
                    History = History + item.Name + "= " + item.Quantity + Environment.NewLine;
                else
                    History = History + item.Name + "= " + item.Quantity;

            }
            return History;

        }


        /// <summary>
        /// Date: 07/07/2020
        /// Author: aminul
        /// Description: From MRE
        /// </summary>
        /// <returns> Stock Ledger product wise.</returns>
        public static List<StockLedger> GetStockLedger(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                       IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                       IBaseRepository<Product> productRepository, IBaseRepository<Category> categoryRepository,
                       IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
                       IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                       IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetails,
                       IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
                       IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ROrderDetail> ROrderDetailRepository,
                       IBaseRepository<ParentCategory> parentCategoryRepository, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository,
                       int reportType, string CompanyName, string CategoryName, string ProductName,
                       DateTime fromDate, DateTime toDate, int ConcernID)
        {
            IQueryable<Product> _Products = null;
            IQueryable<Category> _Categorys = null;
            IQueryable<Company> _Companys = null;
            var _Porders = POrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _POrderDetails = POrderDetailRepository.All;
            var _Stocks = stockRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _StockDetails = stockDetailRepository.All;

            if (string.IsNullOrEmpty(ProductName))
                _Products = productRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Products = productRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.ProductName.Equals(ProductName));

            if (string.IsNullOrEmpty(CategoryName))
                _Categorys = categoryRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Categorys = categoryRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.Description.Equals(CategoryName));

            if (string.IsNullOrEmpty(CompanyName))
                _Companys = companyRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Companys = companyRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.Name.Equals(CompanyName));

            var _Colors = colorRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            var _SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _SOrderDetails = SOrderDetailRepository.All;

            var _ROrders = ROrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _ROrderDetails = ROrderDetailRepository.All;

            var _hireSalesReturn = hireSalesReturnRepository.GetAll().Where(i => i.ConcernId == ConcernID);

            var _CreditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _CreditSalesDetails = CreditSaleDetails.All;

            var _Transfers = TransferRepository.GetAll();
            var _TransferDetails = TransferDetailRepository.All;

            #region Puchase
            var Purchases = (from POD in _POrderDetails
                                 //join std in _StockDetails on POD.POrderDetailID equals std.POrderDetailID
                             join PO in _Porders on POD.POrderID equals PO.POrderID
                             join P in _Products on POD.ProductID equals P.ProductID
                             join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                             join COM in _Companys on P.CompanyID equals COM.CompanyID
                             join CLR in _Colors on POD.ColorID equals CLR.ColorID
                             where PO.Status == (int)EnumPurchaseType.Purchase && PO.TotalAmt > 0 && POD.UnitPrice > 0
                             select new StockLedger
                             {
                                 Date = PO.OrderDate,
                                 Code = P.Code,
                                 CategoryCode = CAT.Code,
                                 ProductID = P.ProductID,
                                 CategoryID = P.CategoryID,
                                 PCategoryID = CAT.PCategoryID,
                                 CompanyID = P.CompanyID,
                                 ModelID = 1,
                                 ColorID = CLR.ColorID,
                                 ProductName = P.ProductName,
                                 CompanyName = COM.Name,
                                 CategoryName = CAT.Description,
                                 ModelName = "",
                                 ColorName = CLR.Name,
                                 PurchaseQuantity = POD.Quantity,
                                 PurchaseRate = POD.UnitPrice - ((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount),
                                 Quantity = POD.Quantity
                             }).OrderBy(x => x.Date);

            #endregion

            #region Transfer In
            var TransferIns = (from t in _Transfers
                               join td in _TransferDetails on t.TransferID equals td.TransferID
                               join P in _Products on td.ToProductID equals P.ProductID
                               join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                               join COM in _Companys on P.CompanyID equals COM.CompanyID
                               join CLR in _Colors on td.ToColorID equals CLR.ColorID
                               where t.ToConcernID == ConcernID && t.Status == (int)EnumTransferStatus.Transfer
                               select new StockLedger
                               {
                                   Date = t.TransferDate,
                                   Code = P.Code,
                                   CategoryCode = CAT.Code,
                                   ProductID = P.ProductID,
                                   CategoryID = P.CategoryID,
                                   PCategoryID = CAT.PCategoryID,
                                   CompanyID = P.CompanyID,
                                   ModelID = 1,
                                   ColorID = CLR.ColorID,
                                   ProductName = P.ProductName,
                                   CompanyName = COM.Name,
                                   CategoryName = CAT.Description,
                                   ModelName = "",
                                   ColorName = CLR.Name,
                                   TransferInQuantity = td.Quantity,
                                   PurchaseRate = td.PRate,
                                   Quantity = td.Quantity
                               }).OrderBy(x => x.Date);
            #endregion

            #region TransferOut
            var TransferOuts = (from t in _Transfers
                                join td in _TransferDetails on t.TransferID equals td.TransferID
                                join P in _Products on td.ProductID equals P.ProductID
                                join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                join COM in _Companys on P.CompanyID equals COM.CompanyID
                                join STD in _StockDetails on td.SDetailID equals STD.SDetailID
                                join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                where t.FromConcernID == ConcernID && t.Status == (int)EnumTransferStatus.Transfer
                                select new StockLedger
                                {
                                    Date = t.TransferDate,
                                    Code = P.Code,
                                    CategoryCode = CAT.Code,
                                    ProductID = P.ProductID,
                                    CategoryID = P.CategoryID,
                                    PCategoryID = CAT.PCategoryID,
                                    CompanyID = P.CompanyID,
                                    ModelID = 1,
                                    ColorID = CLR.ColorID,
                                    ProductName = P.ProductName,
                                    CompanyName = COM.Name,
                                    CategoryName = CAT.Description,
                                    ModelName = "",
                                    ColorName = CLR.Name,
                                    TransferOutQuantity = td.Quantity,
                                    PurchaseRate = STD.PRate,
                                    Quantity = -td.Quantity

                                }).OrderBy(x => x.Date);

            #endregion

            #region Purchase_return
            var Purchase_returns = (from POD in _POrderDetails
                                    join PO in _Porders on POD.POrderID equals PO.POrderID
                                    join P in _Products on POD.ProductID equals P.ProductID
                                    join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                    join COM in _Companys on P.CompanyID equals COM.CompanyID
                                    join CLR in _Colors on POD.ColorID equals CLR.ColorID
                                    where PO.Status == (int)EnumPurchaseType.ProductReturn
                                    select new StockLedger
                                    {
                                        Date = PO.OrderDate,
                                        Code = P.Code,
                                        CategoryCode = CAT.Code,
                                        ProductID = P.ProductID,
                                        CategoryID = P.CategoryID,
                                        PCategoryID = CAT.PCategoryID,
                                        CompanyID = P.CompanyID,
                                        ModelID = 1,
                                        ColorID = CLR.ColorID,
                                        ProductName = P.ProductName,
                                        CompanyName = COM.Name,
                                        CategoryName = CAT.Description,
                                        ModelName = "",
                                        ColorName = CLR.Name,
                                        PurchaseQuantity = 0,
                                        PurchaseReturnQuantity = POD.Quantity,
                                        SalesQuantity = 0,
                                        SalesReturnQuantity = 0,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = -POD.Quantity

                                    }).OrderBy(x => x.Date);
            #endregion

            #region Sales order
            var Sales = ((from SO in _SOrders
                          join SOD in _SOrderDetails on SO.SOrderID equals SOD.SOrderID
                          join P in _Products on SOD.ProductID equals P.ProductID
                          join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                          join COM in _Companys on P.CompanyID equals COM.CompanyID
                          join STD in _StockDetails on SOD.SDetailID equals STD.SDetailID
                          join CLR in _Colors on STD.ColorID equals CLR.ColorID
                          where SO.Status == (int)EnumSalesType.Sales
                          select new StockLedger
                          {
                              Date = SO.InvoiceDate,
                              Code = P.Code,
                              CategoryCode = CAT.Code,
                              ProductID = P.ProductID,
                              CategoryID = P.CategoryID,
                              PCategoryID = CAT.PCategoryID,
                              CompanyID = P.CompanyID,
                              ModelID = 1,
                              ColorID = CLR.ColorID,
                              ProductName = P.ProductName,
                              CompanyName = COM.Name,
                              CategoryName = CAT.Description,
                              ModelName = "",
                              ColorName = CLR.Name,
                              SalesQuantity = SOD.Quantity,
                              PurchaseQuantity = 0,
                              PurchaseReturnQuantity = 0,
                              SalesReturnQuantity = 0,
                              PurchaseRate = STD.PRate,
                              Quantity = -SOD.Quantity

                          }).OrderBy(x => x.Date));
            #endregion

            #region Credit Sales
            var CreditSales = ((from SO in _CreditSales
                                join SOD in _CreditSalesDetails on SO.CreditSalesID equals SOD.CreditSalesID
                                join P in _Products on SOD.ProductID equals P.ProductID
                                join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                join COM in _Companys on P.CompanyID equals COM.CompanyID
                                join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                where SO.IsStatus == EnumSalesType.Sales
                                select new StockLedger
                                {
                                    Date = SO.SalesDate,
                                    Code = P.Code,
                                    CategoryCode = CAT.Code,
                                    ProductID = P.ProductID,
                                    CategoryID = P.CategoryID,
                                    PCategoryID = CAT.PCategoryID,
                                    CompanyID = P.CompanyID,
                                    ModelID = 1,
                                    ColorID = CLR.ColorID,
                                    ProductName = P.ProductName,
                                    CompanyName = COM.Name,
                                    CategoryName = CAT.Description,
                                    ModelName = "",
                                    ColorName = CLR.Name,
                                    SalesQuantity = SOD.Quantity,
                                    PurchaseQuantity = 0,
                                    PurchaseReturnQuantity = 0,
                                    SalesReturnQuantity = 0,
                                    Quantity = -SOD.Quantity,
                                    PurchaseRate = STD.PRate


                                }).OrderBy(x => x.Date));
            #endregion

            #region Sales Return
            var Sales_returns = ((from SO in _ROrders
                                  join SOD in _ROrderDetails on SO.ROrderID equals SOD.ROrderID
                                  join P in _Products on SOD.ProductID equals P.ProductID
                                  join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                  join COM in _Companys on P.CompanyID equals COM.CompanyID
                                  join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                  join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                  select new StockLedger
                                  {
                                      Date = SO.ReturnDate,
                                      Code = P.Code,
                                      CategoryCode = CAT.Code,
                                      ProductID = P.ProductID,
                                      CategoryID = P.CategoryID,
                                      PCategoryID = CAT.PCategoryID,
                                      CompanyID = P.CompanyID,
                                      ModelID = 1,
                                      ColorID = CLR.ColorID,
                                      ProductName = P.ProductName,
                                      CompanyName = COM.Name,
                                      CategoryName = CAT.Description,
                                      ModelName = "",
                                      ColorName = CLR.Name,
                                      SalesReturnQuantity = SOD.Quantity,
                                      PurchaseQuantity = 0,
                                      PurchaseReturnQuantity = 0,
                                      Quantity = SOD.Quantity,
                                      PurchaseRate = STD.PRate

                                  }).OrderBy(x => x.Date));
            #endregion

            #region Hire Sales Return
            var HireSales_returns = ((from SO in _hireSalesReturn
                                      join CS in _CreditSales.Where(i => i.IsReturn == 1) on SO.CreditSalesId equals CS.CreditSalesID
                                      join SOD in _CreditSalesDetails.Where(i => i.IsProductReturn == 1) on CS.CreditSalesID equals SOD.CreditSalesID
                                      join P in _Products on SOD.ProductID equals P.ProductID
                                      join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                      join COM in _Companys on P.CompanyID equals COM.CompanyID
                                      join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                      join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                      select new StockLedger
                                      {
                                          Date = SO.TransactionDate,
                                          Code = P.Code,
                                          CategoryCode = CAT.Code,
                                          ProductID = P.ProductID,
                                          CategoryID = P.CategoryID,
                                          PCategoryID = CAT.PCategoryID,
                                          CompanyID = P.CompanyID,
                                          ModelID = 1,
                                          ColorID = CLR.ColorID,
                                          ProductName = P.ProductName,
                                          CompanyName = COM.Name,
                                          CategoryName = CAT.Description,
                                          ModelName = "",
                                          ColorName = CLR.Name,
                                          SalesReturnQuantity = SOD.Quantity,
                                          PurchaseQuantity = 0,
                                          PurchaseReturnQuantity = 0,
                                          Quantity = SOD.Quantity,
                                          PurchaseRate = STD.PRate

                                      }).OrderBy(x => x.Date));
            #endregion


            #region Replace
            var Rep = ((from SO in _SOrders
                        join SOD in _SOrderDetails on SO.SOrderID equals SOD.SOrderID
                        join P in _Products on SOD.ProductID equals P.ProductID
                        join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                        join COM in _Companys on P.CompanyID equals COM.CompanyID
                        join STD in _StockDetails on SOD.SDetailID equals STD.SDetailID
                        join CLR in _Colors on STD.ColorID equals CLR.ColorID
                        where SO.Status == (int)EnumSalesType.Sales && SOD.RStockDetailID > 0 && SOD.RepOrderID > 0
                        select new StockLedger
                        {
                            Date = SO.InvoiceDate,
                            Code = P.Code,
                            CategoryCode = CAT.Code,
                            ProductID = P.ProductID,
                            CategoryID = P.CategoryID,
                            PCategoryID = CAT.PCategoryID,
                            CompanyID = P.CompanyID,
                            ModelID = 1,
                            ColorID = CLR.ColorID,
                            ProductName = P.ProductName,
                            CompanyName = COM.Name,
                            CategoryName = CAT.Description,
                            ModelName = "",
                            ColorName = CLR.Name,
                            RepQty = SOD.RQuantity,
                            PurchaseQuantity = 0,
                            PurchaseReturnQuantity = 0,
                            SalesReturnQuantity = 0,
                            PurchaseRate = -STD.PRate,
                            Quantity = -SOD.Quantity

                        }).OrderBy(x => x.Date));
            #endregion

            List<StockLedger> Transdata = new List<StockLedger>();
            Transdata.AddRange(Purchases);
            Transdata.AddRange(Purchase_returns);
            Transdata.AddRange(Sales);
            Transdata.AddRange(CreditSales);
            Transdata.AddRange(Sales_returns);
            Transdata.AddRange(TransferIns);
            Transdata.AddRange(TransferOuts);
            Transdata.AddRange(Rep);
            Transdata.AddRange(HireSales_returns);
            List<StockLedger> DataGroupBy = new List<StockLedger>();

            if (reportType == 0)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.ProductID,
                                    item.ColorID,
                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.ProductID,
                                    ColorID = g.Key.ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().ProductName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),

                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),

                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),


                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),


                                })).ToList();

            }
            else if (reportType == 1)
            {

                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CompanyID,
                                    item.ColorID
                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CompanyID,
                                    ColorID = g.FirstOrDefault().ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CompanyName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),

                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),



                                })).ToList();
            }

            else if (reportType == 2)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CategoryID,
                                    //item.ColorID

                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CategoryID,
                                    //ColorID = g.Key.ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CategoryName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),


                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),




                                })).ToList();
            }
            else if (reportType == 3)
            {
                DataGroupBy = ((from item in Transdata
                                join p in parentCategoryRepository.GetAll() on item.PCategoryID equals p.PCategoryID
                                group new { item, p } by new
                                {
                                    item.PCategoryID,
                                    p.Name
                                } into g
                                select new StockLedger
                                {
                                    CategoryID = g.FirstOrDefault().item.CategoryID,
                                    PCategoryID = g.Key.PCategoryID,
                                    ParentCategoryName = g.Key.Name,
                                    CompanyID = g.FirstOrDefault().item.CompanyID,
                                    ModelID = g.FirstOrDefault().item.ModelID,
                                    ProductName = g.FirstOrDefault().item.CategoryName,
                                    CompanyName = g.FirstOrDefault().item.CompanyName,
                                    CategoryName = g.FirstOrDefault().item.CategoryName,
                                    ModelName = g.FirstOrDefault().item.ModelName,
                                    ColorName = g.FirstOrDefault().item.ColorName,
                                    Code = g.FirstOrDefault().item.Code,

                                    PrevPurchaseRate = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.Quantity * o.item.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.Quantity * o.item.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.PurchaseReturnQuantity),


                                    SalesQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.RepQty),

                                    TransferInQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.TransferOutQuantity),



                                })).ToList();
            }

            var FinalData = (from d in DataGroupBy
                             select new StockLedger
                             {
                                 Date = DateTime.Now,
                                 ProductID = d.ProductID,
                                 ColorID = d.ColorID,
                                 CategoryID = d.CategoryID,
                                 CompanyID = d.CompanyID,
                                 ModelID = d.ModelID,
                                 ProductName = d.ProductName,
                                 CompanyName = d.CompanyName,
                                 CategoryName = d.CategoryName,
                                 ModelName = d.ModelName,
                                 ColorName = d.ColorName,
                                 Code = d.Code,
                                 PCategoryID = d.PCategoryID,
                                 ParentCategoryName = d.ParentCategoryName,

                                 OpeningStockQuantity = (d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity + d.PreRepQty)
                                                       - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity),

                                 PurchaseQuantity = d.PurchaseQuantity,
                                 PurchaseReturnQuantity = d.PurchaseReturnQuantity,

                                 SalesQuantity = d.SalesQuantity,
                                 SalesReturnQuantity = d.SalesReturnQuantity,
                                 RepQty = d.RepQty,

                                 TransferInQuantity = d.TransferInQuantity,
                                 TransferOutQuantity = d.TransferOutQuantity,

                                 ClosingStockQuantity = (
                                                            ((d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity + d.PreRepQty)
                                                           - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity)
                                                            )
                                                            +
                                                            (
                                                              (d.PurchaseQuantity + d.TransferInQuantity + d.SalesReturnQuantity + d.RepQty) - (d.PurchaseReturnQuantity + d.TransferOutQuantity + d.SalesQuantity)
                                                            )
                                                        ),
                                 OpeningStockValue = 0m,
                                 TotalStockValue = 0m,
                                 //ClosingStockValue = (
                                 //                           ((d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity)
                                 //                          - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity)
                                 //                           )
                                 //                           +
                                 //                           (
                                 //                             (d.PurchaseQuantity + d.TransferInQuantity + d.SalesReturnQuantity) - (d.PurchaseReturnQuantity + d.TransferOutQuantity + d.SalesQuantity)
                                 //                           )
                                 //                       ) * d.PurchaseRate

                                 ClosingStockValue = d.PrevPurchaseRate + d.PurchaseRate,


                             }).OrderBy(i => i.Code).ToList();


            //string purchasePriceList = string.Join(",", FinalData.Select(p => p.ClosingStockValue).ToList());
            //var singleProd = FinalData.Where(p => p.ProductID == 254145).ToList();

            //List<TOCheckStockValue> dbResult = stockRepository.ExecSP<TOCheckStockValue>("CheckStockValue '2022-08-18', 5126").ToList();

            //var difference = FinalData.Where(f => !dbResult.Any(d => f.ProductID == d.ProductID && f.ColorID == d.ColorID && f.ClosingStockValue == d.PurchaseRate)).ToList();
            //decimal diffPurRate = difference.Sum(p => p.ClosingStockValue);
            //difference = difference.Where(d => d.ClosingStockValue > 0).OrderBy(p => p.ProductID).ToList();
            return FinalData;
        }


        /// <summary>
        /// Date: 23-02-2020
        /// Author: Aminul
        /// Reason: For purchase return
        /// </summary>
        /// <returns>Return all stock products of a supplier by ProductID,colorID and GodownID</returns>
        public static List<ProductDetailsModel> GetSupplierStockDetails(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
                        int SupplierID, int ProductID, int ColorID, int GodownID)
        {
            //var SupplierPOrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.Status == (int)EnumPurchaseType.Purchase);

            var Result = (from std in stockDetailRepository.All.Where(i => i.Status == 1 && i.IsDamage == 0)
                          join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.POrderDetailID, std.GodownID } equals new { pod.ProductID, pod.ColorID, pod.POrderDetailID, pod.GodownID }
                          join p in productRepository.All on pod.ProductID equals p.ProductID
                          //join po in SupplierPOrders on pod.POrderID equals po.POrderID
                          where pod.ProductID == ProductID && pod.ColorID == ColorID && pod.GodownID == GodownID
                          select new ProductDetailsModel
                          {
                              ProductId = pod.ProductID,
                              ColorId = pod.ColorID,
                              GodownID = pod.GodownID,
                              MRPRate = std.PRate,
                              PreStock = std.Quantity,
                              IMENo = std.IMENO,
                              StockDetailsId = std.SDetailID,
                              ProductType = p.ProductType
                          }).ToList();
            return Result;
        }

        public static List<ProductDetailsModel> GetSupplierDamageStockDetails(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
                        int SupplierID, int ProductID, int ColorID, int GodownID)
        {
            //var SupplierPOrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.Status == (int)EnumPurchaseType.Purchase);

            var Result = (from std in stockDetailRepository.All.Where(i => i.Status == 1 && i.IsDamage == 1)
                          join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.POrderDetailID, std.GodownID } equals new { pod.ProductID, pod.ColorID, pod.POrderDetailID, pod.GodownID }
                          join p in productRepository.All on pod.ProductID equals p.ProductID
                          //join po in SupplierPOrders on pod.POrderID equals po.POrderID
                          where pod.ProductID == ProductID && pod.ColorID == ColorID && pod.GodownID == GodownID
                          select new ProductDetailsModel
                          {
                              ProductId = pod.ProductID,
                              ColorId = pod.ColorID,
                              GodownID = pod.GodownID,
                              MRPRate = std.PRate,
                              PreStock = std.Quantity,
                              IMENo = std.IMENO,
                              StockDetailsId = std.SDetailID,
                              ProductType = p.ProductType
                          }).ToList();
            return Result;
        }



        /// <summary>
        /// Date: 23-02-2020
        /// Author: Aminul
        /// Reason: For purchase return
        /// </summary>
        /// <returns>Return all stock and damage products of a supplier</returns>
        public static List<ProductDetailsModel> GetStockProductsBySupplier(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Category> CategoryRepository,
                        IBaseRepository<Company> CompanyRepository,
                        int SupplierID)
        {
            List<ProductDetailsModel> Result = new List<ProductDetailsModel>();
            var SupplierPOrders = POrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.Purchase);
            if (SupplierPOrders.Count() == 0)
                return Result;

            var Products = (from std in stockDetailRepository.All.Where(i => i.IsDamage == 0 && (i.Status == (int)EnumStockStatus.Stock || i.Status == (int)EnumStockStatus.Damage))
                            join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.GodownID, std.POrderDetailID } equals new { pod.ProductID, pod.ColorID, pod.GodownID, pod.POrderDetailID }
                            join po in SupplierPOrders on pod.POrderID equals po.POrderID
                            select new
                            {
                                pod.ProductID,
                                pod.ColorID,
                                pod.GodownID,
                                std.PRate,
                                std.Quantity,
                                std.IMENO,
                                std.SDetailID,
                                ProductType = pod.Product.ProductType
                            }).ToList();

            var StockProducts = from p in Products
                                group p by new
                                {
                                    p.ProductID,
                                    p.ColorID,
                                    p.GodownID,
                                    p.IMENO,
                                    p.ProductType
                                } into g
                                select new
                                {
                                    g.Key.ProductID,
                                    g.Key.ColorID,
                                    g.Key.GodownID,
                                    g.Key.IMENO,
                                    Quantity = g.Key.ProductType == (int)EnumProductType.NoBarcode ? g.Sum(i => i.Quantity) : 1,
                                    PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                    SDetailID = g.Min(i => i.SDetailID)
                                };

            Result = (from st in StockProducts
                      join p in productRepository.All on st.ProductID equals p.ProductID
                      join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                      join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                      join col in colorRepository.All on st.ColorID equals col.ColorID
                      select new ProductDetailsModel
                      {
                          ProductCode = p.Code,
                          ProductId = p.ProductID,
                          ProductName = p.ProductName,
                          CategoryName = cat.Description,
                          CompanyName = com.Name,
                          ColorName = col.Name,
                          ColorId = st.ColorID,
                          GodownID = st.GodownID,
                          PreStock = st.Quantity,
                          IMENo = st.IMENO,
                          StockDetailsId = st.SDetailID,
                          MRPRate = st.PRate,
                          ProductType = p.ProductType
                      }).OrderBy(i => i.CategoryName).ThenBy(i => i.CompanyName).ToList();

            return Result;

        }



        /// <summary>
        /// Date: 17-05-2020
        /// Author: Aminul
        /// </summary>
        /// <returns>Admin Stock Report</returns>
        public static IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
                        GetforAdminStockReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
                        IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository,
                        IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<Company> CompanyRepository,
                        IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                        string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID)
        {
            IQueryable<Product> Products = null;
            IQueryable<Color> Colors = null;
            IQueryable<Stock> Stocks = null;
            IQueryable<Godown> Godowns = null;
            if (concernID > 0)
            {
                Products = productRepository.GetAll().Where(i => i.ConcernID == concernID);
                Colors = colorRepository.GetAll().Where(i => i.ConcernID == concernID);
                Stocks = stockRepository.GetAll().Where(i => i.ConcernID == concernID);
                Godowns = GodownRepository.GetAll().Where(i => i.ConcernID == concernID);
            }
            else
            {
                var familyTrees = SisterConcernRepository.GetFamilyTree(UserConcernID);
                Products = from p in productRepository.GetAll()
                           join s in familyTrees on p.ConcernID equals s.ConcernID
                           select p;
                Colors = from p in colorRepository.GetAll()
                         join s in familyTrees on p.ConcernID equals s.ConcernID
                         select p;
                Stocks = from p in stockRepository.GetAll()
                         join s in familyTrees on p.ConcernID equals s.ConcernID
                         select p;
                Godowns = from p in GodownRepository.GetAll()
                          join s in familyTrees on p.ConcernID equals s.ConcernID
                          select p;
            }

            if (!string.IsNullOrEmpty(CategoryName))
            {
                Products = from p in Products
                           join cat in CategoryRepository.GetAll() on p.CategoryID equals cat.CategoryID
                           where cat.Description.Equals(CategoryName, StringComparison.OrdinalIgnoreCase)
                           select p;
            }

            if (!string.IsNullOrEmpty(CompanyName))
            {
                Products = from p in Products
                           join com in CompanyRepository.GetAll() on p.CompanyID equals com.CompanyID
                           where com.Name.Equals(CompanyName, StringComparison.OrdinalIgnoreCase)
                           select p;
            }
            if (!string.IsNullOrEmpty(ProductName))
                Products = Products.Where(i => i.ProductName.Equals(ProductName, StringComparison.OrdinalIgnoreCase));

            var StockDetails = StockDetailRepository.GetAll();

            var oStockData = (from st in Stocks
                              join sd in StockDetailRepository.GetAll() on st.StockID equals sd.StockID
                              join pro in Products on st.ProductID equals pro.ProductID
                              join col in Colors on st.ColorID equals col.ColorID
                              join god in Godowns on st.GodownID equals god.GodownID
                              join sis in SisterConcernRepository.GetAll() on col.ConcernID equals sis.ConcernID
                              where sd.Status == (int)EnumStockStatus.Stock
                              select new
                              {
                                  StockID = st.StockID,
                                  productName = pro.ProductName,
                                  CompanyName = pro.Company.Name,
                                  CategoryName = pro.Category.Description,
                                  ColorName = col.Name,
                                  Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : 1,
                                  PRate = sd.PRate,
                                  //StockDetails = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock),
                                  //SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,
                                  //CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).CreditSRate : 0m
                                  ConcernName = sis.Name,
                                  sd.SRate,
                                  sd.CRSalesRate3Month,
                                  sd.CreditSRate,
                                  sd.CRSalesRate12Month,
                                  GodownName = god.Name
                              }).Where(st => st.Qty > 0).ToList();

            var finalData = from s in oStockData
                            group s by new
                            {
                                s.productName,
                                s.CompanyName,
                                s.StockID,
                                s.ColorName,
                                s.GodownName,
                                s.PRate,
                                s.CategoryName,
                                s.ConcernName
                            } into g
                            select new
                            {
                                g.Key.StockID,
                                g.Key.productName,
                                g.Key.CompanyName,
                                g.Key.CategoryName,
                                g.Key.ColorName,
                                g.Key.GodownName,
                                g.Key.ConcernName,
                                Qty = g.Sum(i => i.Qty),
                                g.Key.PRate,
                                SRate = g.Select(i => i.SRate).FirstOrDefault(),
                                CRSalesRate3Month = g.Select(i => i.CRSalesRate3Month).FirstOrDefault(),
                                CreditSRate = g.Select(i => i.CreditSRate).FirstOrDefault(),
                                CRSalesRate12Month = g.Select(i => i.CRSalesRate12Month).FirstOrDefault(),
                            };

            return finalData.Select(x => new Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>
                                (
                                x.StockID,
                                x.productName,
                                x.CompanyName,
                                x.CategoryName,
                                x.ColorName,
                                x.Qty,
                                x.PRate,
                                new Tuple<decimal, decimal, decimal, decimal, string>
                                    (
                                     x.SRate,
                                     x.CreditSRate,
                                     x.CRSalesRate3Month,
                                     x.CRSalesRate12Month,
                                     x.ConcernName
                                    )
                                ));
        }



        public static bool IsIMEIExistInGodown(this IBaseRepository<Stock> StockRepository, IBaseRepository<StockDetail> StockDetailRepository,
            int ConcernID, int GodownID, string IMEI)
        {
            var d = from s in StockRepository.GetAll()
                    join sd in StockDetailRepository.GetAll() on s.StockID equals sd.StockID
                    where s.ConcernID == ConcernID && sd.GodownID == GodownID && sd.IMENO.Equals(IMEI.Trim()) && (sd.Status == (int)EnumStockStatus.Stock)
                    select sd;
            if (d.Count() > 0)
                return true;

            return false;
        }

        public static ProductDetailsModel GetStockIMEIDetail(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
                        IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
                        IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                        IBaseRepository<Color> ColorRepository, IBaseRepository<SaleOffer> SaleOfferRepository,
                        string IMEI, bool IsStockIMEI)
        {
            IQueryable<SaleOffer> Offers = SaleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join off in Offers on p.ProductID equals off.ProductID into lj
                               from off in lj.DefaultIfEmpty()
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               where sd.IMENO.Equals(IMEI.Trim())
                               && (IsStockIMEI ? sd.Status == (int)EnumStockStatus.Stock : true) && sd.IsDamage == 0
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   MRPRate = sd.SRate,
                                   PRate = sd.PRate,
                                   offerValue = off != null ? off.OfferValue : 0m,
                                   OfferDescription = off != null ? off.Description : string.Empty,
                                   ProductType = p.ProductType,
                                   Service = p.ServiceWarrentyMonth
                               }).OrderByDescending(s => s.StockDetailsId).FirstOrDefault();
            return IMEIDetails;
        }

        public static ProductDetailsModel GetStockIMEIDetailsByLastSomedigit(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
                               IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
                               IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                               IBaseRepository<Color> ColorRepository, IBaseRepository<SaleOffer> SaleOfferRepository,
                               string IMEI)
        {
            IQueryable<SaleOffer> Offers = SaleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join off in Offers on p.ProductID equals off.ProductID into lj
                               from off in lj.DefaultIfEmpty()
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               where sd.IMENO.EndsWith(IMEI) && sd.Status == (int)EnumStockStatus.Stock && sd.IsDamage == 0
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   MRPRate = sd.SRate,
                                   PRate = sd.PRate,
                                   OfferDescription = off != null ? off.Description : string.Empty,
                                   offerValue = off != null ? off.OfferValue : 0m,
                               }).FirstOrDefault();
            return IMEIDetails;
        }

        public static ProductDetailsModel GetSRVisitIMEIDetails(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
                                    IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
                                    IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository, IBaseRepository<Color> ColorRepository,
                                    IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
                                    IBaseRepository<SaleOffer> SaleOfferRepository,
                                    string IMEI, int EmployeeID)
        {
            IQueryable<SaleOffer> Offers = SaleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join off in Offers on p.ProductID equals off.ProductID into lj
                               from off in lj.DefaultIfEmpty()
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               join svp in SRVProductDetailRepository.All on sd.SDetailID equals svp.SDetailID
                               join svd in SRVisitDetailRepository.All on svp.SRVisitDID equals svd.SRVisitDID
                               join sv in SRVisitRepository.All on svd.SRVisitID equals sv.SRVisitID
                               where sd.IMENO.EndsWith(IMEI.Trim()) && sd.Status == (int)EnumStockStatus.Stock
                               && (sv.Status == (int)EnumSRVisitType.Live && svp.Status == (int)EnumSRVProductDetailsStatus.Stock)
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   MRPRate = sd.SRate,
                                   PRate = sd.PRate,
                                   OfferDescription = off != null ? off.Description : string.Empty,
                                   offerValue = off != null ? off.OfferValue : 0m,
                               }).FirstOrDefault();
            return IMEIDetails;
        }

        public static IQueryable<ProductDetailsModel> GetStocksByProductId(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
                         IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
                         IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                         IBaseRepository<Color> ColorRepository,
                         int ProductID)
        {

            var IMEIDetails = (from s in stockRepository.All
                               join p in ProductRepository.All on s.ProductID equals p.ProductID
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on s.GodownID equals g.GodownID
                               join c in ColorRepository.All on s.ColorID equals c.ColorID
                               where s.ProductID == ProductID
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   StockQty = s.Quantity,
                                   MRPRate = s.MRPPrice
                               }).OrderBy(i => i.ColorName);
            return IMEIDetails;
        }

        public static IQueryable<ProductDetailsModel> GetStockDetails(this IBaseRepository<Stock> stockRepository,
            IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Product> ProductRepository,
            IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<Godown> GodownRepository, IBaseRepository<Color> ColorRepository)
        {
            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   Status = sd.Status,
                                   SDetailID = sd.SDetailID
                               });
            return IMEIDetails;
        }


        public static IQueryable<ProductDetailsModel> GetStocks(this IBaseRepository<Stock> stockRepository,
            IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Product> ProductRepository,
            IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<Godown> GodownRepository, IBaseRepository<Color> ColorRepository)
        {
            var stockDetails = StockDetailRepository.All;
            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               select new ProductDetailsModel
                               {
                                   StockID = s.StockID,
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   StockQty = p.ProductType == (int)EnumProductType.NoBarcode ? s.Quantity : stockDetails.Where(i => i.ProductID == p.ProductID && i.GodownID == g.GodownID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).Count(),
                                   SalesPrice = stockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? stockDetails.FirstOrDefault(i => i.ProductID == p.ProductID && i.ColorID == c.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,


                               });
            return IMEIDetails;
        }

        public static ProductDetailsModel GetIMEIDetails(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
            IBaseRepository<Color> ColorRepository, IBaseRepository<SaleOffer> SaleOfferRepository,
            string IMEI, bool IsStockIMEI)
        {
            IQueryable<SaleOffer> Offers = SaleOfferRepository.FindBy(x => x.Status == EnumOfferStatus.Ongoing && x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join off in Offers on p.ProductID equals off.ProductID into lj
                               from off in lj.DefaultIfEmpty()
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               where sd.IMENO.Equals(IMEI.Trim())
                               && (IsStockIMEI ? sd.Status == (int)EnumStockStatus.Stock : true)
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   MRPRate = sd.SRate,
                                   PRate = sd.PRate,
                                   CompanyID = p.CompanyID,
                                   OfferDescription = off != null ? off.Description : string.Empty,
                               }).FirstOrDefault();
            return IMEIDetails;
        }


        public static List<ProductDetailsModel> GetDamageStockProductsBySupplier(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Category> CategoryRepository,
                        IBaseRepository<Company> CompanyRepository,
                        int SupplierID)
        {
            List<ProductDetailsModel> Result = new List<ProductDetailsModel>();
            //var SupplierPOrders = POrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DamageOrder);
            var SupplierPOrders = POrderRepository.All;

            if (SupplierPOrders.Count() == 0)
                return Result;

            var Products = (from std in stockDetailRepository.All.Where(i => i.IsDamage == 1 && (i.Status == (int)EnumStockStatus.Stock || i.Status == (int)EnumStockStatus.Damage))
                            join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.GodownID, std.POrderDetailID } equals new { pod.ProductID, pod.ColorID, pod.GodownID, pod.POrderDetailID }
                            join po in SupplierPOrders on pod.POrderID equals po.POrderID
                            select new
                            {
                                pod.ProductID,
                                pod.ColorID,
                                pod.GodownID,
                                std.PRate,
                                std.Quantity,
                                std.IMENO,
                                std.SDetailID,
                                ProductType = pod.Product.ProductType
                            }).ToList();

            var StockProducts = from p in Products
                                group p by new
                                {
                                    p.ProductID,
                                    p.ColorID,
                                    p.GodownID,
                                    p.IMENO,
                                    p.ProductType
                                } into g
                                select new
                                {
                                    g.Key.ProductID,
                                    g.Key.ColorID,
                                    g.Key.GodownID,
                                    g.Key.IMENO,
                                    Quantity = g.Key.ProductType == (int)EnumProductType.NoBarcode ? g.Sum(i => i.Quantity) : 1,
                                    PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                    SDetailID = g.Min(i => i.SDetailID)
                                };

            Result = (from st in StockProducts
                      join p in productRepository.All on st.ProductID equals p.ProductID
                      join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                      join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                      join col in colorRepository.All on st.ColorID equals col.ColorID
                      select new ProductDetailsModel
                      {
                          ProductCode = p.Code,
                          ProductId = p.ProductID,
                          ProductName = p.ProductName,
                          CategoryName = cat.Description,
                          CompanyName = com.Name,
                          ColorName = col.Name,
                          ColorId = st.ColorID,
                          GodownID = st.GodownID,
                          PreStock = st.Quantity,
                          IMENo = st.IMENO,
                          StockDetailsId = st.SDetailID,
                          MRPRate = st.PRate,
                          ProductType = p.ProductType
                      }).OrderBy(i => i.CategoryName).ThenBy(i => i.CompanyName).ToList();

            return Result;

        }

        public static ProductDetailsModel GetDamageStockIMEIDetail(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> StockDetailRepository,
                        IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
                        IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                        IBaseRepository<Color> ColorRepository, IBaseRepository<SaleOffer> SaleOfferRepository,
                        string IMEI, bool IsStockIMEI)
        {
            IQueryable<SaleOffer> Offers = SaleOfferRepository.FindBy(x => x.FromDate <= DateTime.Today && x.ToDate >= DateTime.Today);

            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join off in Offers on p.ProductID equals off.ProductID into lj
                               from off in lj.DefaultIfEmpty()
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               where sd.IMENO.Equals(IMEI.Trim())
                               && (IsStockIMEI ? sd.Status == (int)EnumStockStatus.Stock : true) && sd.IsDamage == 1
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   MRPRate = sd.SRate,
                                   PRate = sd.PRate,
                                   offerValue = off != null ? off.OfferValue : 0m,
                                   OfferDescription = off != null ? off.Description : string.Empty,
                                   ProductType = p.ProductType,
                                   Service = p.ServiceWarrentyMonth
                               }).OrderByDescending(s => s.StockDetailsId).FirstOrDefault();
            return IMEIDetails;
        }

        public static IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>>
                                GetforAdminProductStockReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
                                IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository,
                                IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<Company> CompanyRepository,
                                IBaseRepository<Category> CategoryRepository, IBaseRepository<Godown> GodownRepository,
                                string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID)
        {
            IQueryable<Product> Products = null;
            IQueryable<Color> Colors = null;
            IQueryable<Stock> Stocks = null;
            IQueryable<Godown> Godowns = null;
            if (concernID > 0)
            {
                Products = productRepository.GetAll().Where(i => i.ConcernID == concernID);
                Colors = colorRepository.GetAll().Where(i => i.ConcernID == concernID);
                Stocks = stockRepository.GetAll().Where(i => i.ConcernID == concernID);
                Godowns = GodownRepository.GetAll().Where(i => i.ConcernID == concernID);
            }
            else
            {
                var familyTrees = SisterConcernRepository.GetFamilyTree(UserConcernID);
                Products = from p in productRepository.GetAll()
                           join s in familyTrees on p.ConcernID equals s.ConcernID
                           select p;
                Colors = from p in colorRepository.GetAll()
                         join s in familyTrees on p.ConcernID equals s.ConcernID
                         select p;
                Stocks = from p in stockRepository.GetAll()
                         join s in familyTrees on p.ConcernID equals s.ConcernID
                         select p;
                Godowns = from p in GodownRepository.GetAll()
                          join s in familyTrees on p.ConcernID equals s.ConcernID
                          select p;
            }

            if (!string.IsNullOrEmpty(CategoryName))
            {
                Products = from p in Products
                           join cat in CategoryRepository.GetAll() on p.CategoryID equals cat.CategoryID
                           where cat.Description.Equals(CategoryName, StringComparison.OrdinalIgnoreCase)
                           select p;
            }

            if (!string.IsNullOrEmpty(CompanyName))
            {
                Products = from p in Products
                           join com in CompanyRepository.GetAll() on p.CompanyID equals com.CompanyID
                           where com.Name.Equals(CompanyName, StringComparison.OrdinalIgnoreCase)
                           select p;
            }
            if (!string.IsNullOrEmpty(ProductName))
                Products = Products.Where(i => i.ProductName.Equals(ProductName, StringComparison.OrdinalIgnoreCase));

            var StockDetails = StockDetailRepository.GetAll();

            var oStockData = (from st in Stocks
                              join sd in StockDetailRepository.GetAll() on st.StockID equals sd.StockID
                              join pro in Products on st.ProductID equals pro.ProductID
                              join col in Colors on st.ColorID equals col.ColorID
                              join god in Godowns on st.GodownID equals god.GodownID
                              join sis in SisterConcernRepository.GetAll() on col.ConcernID equals sis.ConcernID
                              where sd.Status == (int)EnumStockStatus.Stock
                              select new
                              {
                                  StockID = st.StockID,
                                  productName = pro.ProductName,
                                  CompanyName = pro.Company.Name,
                                  CategoryName = pro.Category.Description,
                                  ColorName = col.Name,
                                  Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : 1,
                                  PRate = sd.PRate,
                                  ConcernName = sis.Name,
                                  sd.SRate,
                                  sd.CRSalesRate3Month,
                                  sd.CreditSRate,
                                  sd.CRSalesRate12Month,
                                  GodownName = god.Name
                              }).Where(st => st.Qty > 0).ToList();

            var finalData = from s in oStockData
                            group s by new
                            {
                                s.productName,
                                s.CompanyName,
                                //s.StockID,
                                s.ColorName,
                                //s.GodownName,
                                s.CategoryName,
                                s.ConcernName
                            } into g
                            select new
                            {
                                //g.Key.StockID,
                                g.Key.productName,
                                g.Key.CompanyName,
                                g.Key.CategoryName,
                                g.Key.ColorName,
                                //g.Key.GodownName,
                                g.Key.ConcernName,
                                Qty = g.Sum(i => i.Qty),
                                PRate = g.Select(i => i.PRate).LastOrDefault(),
                                SRate = g.Select(i => i.SRate).LastOrDefault(),
                                CRSalesRate3Month = g.Select(i => i.CRSalesRate3Month).LastOrDefault(),
                                CreditSRate = g.Select(i => i.CreditSRate).LastOrDefault(),
                                CRSalesRate12Month = g.Select(i => i.CRSalesRate12Month).LastOrDefault(),
                            };

            return finalData.Select(x => new Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string>>
                                (
                                0,
                                x.productName,
                                x.CompanyName,
                                x.CategoryName,
                                x.ColorName,
                                x.Qty,
                                x.PRate,
                                new Tuple<decimal, decimal, decimal, decimal, string>
                                    (
                                     x.SRate,
                                     x.CreditSRate,
                                     x.CRSalesRate3Month,
                                     x.CRSalesRate12Month,
                                     x.ConcernName
                                    )
                                ));
        }


        public static List<StockLedger> GetRateWiseStockLedger(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                       IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                       IBaseRepository<Product> productRepository, IBaseRepository<Category> categoryRepository,
                       IBaseRepository<Company> companyRepository, IBaseRepository<Color> colorRepository,
                       IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                       IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetails,
                       IBaseRepository<Transfer> TransferRepository, IBaseRepository<TransferDetail> TransferDetailRepository,
                       IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ROrderDetail> ROrderDetailRepository,
                       IBaseRepository<ParentCategory> parentCategoryRepository, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository,
                       int reportType, string CompanyName, string CategoryName, string ProductName,
                       DateTime fromDate, DateTime toDate, int ConcernID)
        {
            IQueryable<Product> _Products = null;
            IQueryable<Category> _Categorys = null;
            IQueryable<Company> _Companys = null;
            var _Porders = POrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _POrderDetails = POrderDetailRepository.All;
            var _Stocks = stockRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _StockDetails = stockDetailRepository.All;

            if (string.IsNullOrEmpty(ProductName))
                _Products = productRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Products = productRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.ProductName.Equals(ProductName));

            if (string.IsNullOrEmpty(CategoryName))
                _Categorys = categoryRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Categorys = categoryRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.Description.Equals(CategoryName));

            if (string.IsNullOrEmpty(CompanyName))
                _Companys = companyRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                _Companys = companyRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.Name.Equals(CompanyName));

            var _Colors = colorRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            var _SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _SOrderDetails = SOrderDetailRepository.All;

            var _ROrders = ROrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _ROrderDetails = ROrderDetailRepository.All;

            var _hireSalesReturn = hireSalesReturnRepository.GetAll().Where(i => i.ConcernId == ConcernID);

            var _CreditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var _CreditSalesDetails = CreditSaleDetails.All;

            var _Transfers = TransferRepository.GetAll();
            var _TransferDetails = TransferDetailRepository.All;

            #region Puchase
            var Purchases = (from POD in _POrderDetails
                                 //join std in _StockDetails on POD.POrderDetailID equals std.POrderDetailID
                             join PO in _Porders on POD.POrderID equals PO.POrderID
                             join P in _Products on POD.ProductID equals P.ProductID
                             join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                             join COM in _Companys on P.CompanyID equals COM.CompanyID
                             join CLR in _Colors on POD.ColorID equals CLR.ColorID
                             where PO.Status == (int)EnumPurchaseType.Purchase && PO.TotalAmt > 0 && POD.UnitPrice > 0
                             select new StockLedger
                             {
                                 Date = PO.OrderDate,
                                 Code = P.Code,
                                 CategoryCode = CAT.Code,
                                 ProductID = P.ProductID,
                                 CategoryID = P.CategoryID,
                                 PCategoryID = CAT.PCategoryID,
                                 CompanyID = P.CompanyID,
                                 ModelID = 1,
                                 ColorID = CLR.ColorID,
                                 ProductName = P.ProductName,
                                 CompanyName = COM.Name,
                                 CategoryName = CAT.Description,
                                 ModelName = "",
                                 ColorName = CLR.Name,
                                 PurchaseQuantity = POD.Quantity,
                                 PurchaseRate = POD.UnitPrice - ((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount),
                                 Quantity = POD.Quantity
                             }).OrderBy(x => x.Date);

            #endregion

            #region Transfer In
            var TransferIns = (from t in _Transfers
                               join td in _TransferDetails on t.TransferID equals td.TransferID
                               join P in _Products on td.ToProductID equals P.ProductID
                               join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                               join COM in _Companys on P.CompanyID equals COM.CompanyID
                               join CLR in _Colors on td.ToColorID equals CLR.ColorID
                               where t.ToConcernID == ConcernID && t.Status == (int)EnumTransferStatus.Transfer
                               select new StockLedger
                               {
                                   Date = t.TransferDate,
                                   Code = P.Code,
                                   CategoryCode = CAT.Code,
                                   ProductID = P.ProductID,
                                   CategoryID = P.CategoryID,
                                   PCategoryID = CAT.PCategoryID,
                                   CompanyID = P.CompanyID,
                                   ModelID = 1,
                                   ColorID = CLR.ColorID,
                                   ProductName = P.ProductName,
                                   CompanyName = COM.Name,
                                   CategoryName = CAT.Description,
                                   ModelName = "",
                                   ColorName = CLR.Name,
                                   TransferInQuantity = td.Quantity,
                                   PurchaseRate = td.PRate,
                                   Quantity = td.Quantity
                               }).OrderBy(x => x.Date);
            #endregion

            #region TransferOut
            var TransferOuts = (from t in _Transfers
                                join td in _TransferDetails on t.TransferID equals td.TransferID
                                join P in _Products on td.ProductID equals P.ProductID
                                join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                join COM in _Companys on P.CompanyID equals COM.CompanyID
                                join STD in _StockDetails on td.SDetailID equals STD.SDetailID
                                join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                where t.FromConcernID == ConcernID && t.Status == (int)EnumTransferStatus.Transfer
                                select new StockLedger
                                {
                                    Date = t.TransferDate,
                                    Code = P.Code,
                                    CategoryCode = CAT.Code,
                                    ProductID = P.ProductID,
                                    CategoryID = P.CategoryID,
                                    PCategoryID = CAT.PCategoryID,
                                    CompanyID = P.CompanyID,
                                    ModelID = 1,
                                    ColorID = CLR.ColorID,
                                    ProductName = P.ProductName,
                                    CompanyName = COM.Name,
                                    CategoryName = CAT.Description,
                                    ModelName = "",
                                    ColorName = CLR.Name,
                                    TransferOutQuantity = td.Quantity,
                                    PurchaseRate = STD.PRate,
                                    Quantity = -td.Quantity

                                }).OrderBy(x => x.Date);

            #endregion

            #region Purchase_return
            var Purchase_returns = (from POD in _POrderDetails
                                    join PO in _Porders on POD.POrderID equals PO.POrderID
                                    join P in _Products on POD.ProductID equals P.ProductID
                                    join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                    join COM in _Companys on P.CompanyID equals COM.CompanyID
                                    join CLR in _Colors on POD.ColorID equals CLR.ColorID
                                    where PO.Status == (int)EnumPurchaseType.ProductReturn
                                    select new StockLedger
                                    {
                                        Date = PO.OrderDate,
                                        Code = P.Code,
                                        CategoryCode = CAT.Code,
                                        ProductID = P.ProductID,
                                        CategoryID = P.CategoryID,
                                        PCategoryID = CAT.PCategoryID,
                                        CompanyID = P.CompanyID,
                                        ModelID = 1,
                                        ColorID = CLR.ColorID,
                                        ProductName = P.ProductName,
                                        CompanyName = COM.Name,
                                        CategoryName = CAT.Description,
                                        ModelName = "",
                                        ColorName = CLR.Name,
                                        PurchaseQuantity = 0,
                                        PurchaseReturnQuantity = POD.Quantity,
                                        SalesQuantity = 0,
                                        SalesReturnQuantity = 0,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = -POD.Quantity

                                    }).OrderBy(x => x.Date);
            #endregion

            #region Sales order
            var Sales = ((from SO in _SOrders
                          join SOD in _SOrderDetails on SO.SOrderID equals SOD.SOrderID
                          join P in _Products on SOD.ProductID equals P.ProductID
                          join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                          join COM in _Companys on P.CompanyID equals COM.CompanyID
                          join STD in _StockDetails on SOD.SDetailID equals STD.SDetailID
                          join CLR in _Colors on STD.ColorID equals CLR.ColorID
                          where SO.Status == (int)EnumSalesType.Sales
                          select new StockLedger
                          {
                              Date = SO.InvoiceDate,
                              Code = P.Code,
                              CategoryCode = CAT.Code,
                              ProductID = P.ProductID,
                              CategoryID = P.CategoryID,
                              PCategoryID = CAT.PCategoryID,
                              CompanyID = P.CompanyID,
                              ModelID = 1,
                              ColorID = CLR.ColorID,
                              ProductName = P.ProductName,
                              CompanyName = COM.Name,
                              CategoryName = CAT.Description,
                              ModelName = "",
                              ColorName = CLR.Name,
                              SalesQuantity = SOD.Quantity,
                              PurchaseQuantity = 0,
                              PurchaseReturnQuantity = 0,
                              SalesReturnQuantity = 0,
                              PurchaseRate = STD.PRate,
                              Quantity = -SOD.Quantity

                          }).OrderBy(x => x.Date));
            #endregion

            #region Credit Sales
            var CreditSales = ((from SO in _CreditSales
                                join SOD in _CreditSalesDetails on SO.CreditSalesID equals SOD.CreditSalesID
                                join P in _Products on SOD.ProductID equals P.ProductID
                                join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                join COM in _Companys on P.CompanyID equals COM.CompanyID
                                join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                where SO.IsStatus == EnumSalesType.Sales
                                select new StockLedger
                                {
                                    Date = SO.SalesDate,
                                    Code = P.Code,
                                    CategoryCode = CAT.Code,
                                    ProductID = P.ProductID,
                                    CategoryID = P.CategoryID,
                                    PCategoryID = CAT.PCategoryID,
                                    CompanyID = P.CompanyID,
                                    ModelID = 1,
                                    ColorID = CLR.ColorID,
                                    ProductName = P.ProductName,
                                    CompanyName = COM.Name,
                                    CategoryName = CAT.Description,
                                    ModelName = "",
                                    ColorName = CLR.Name,
                                    SalesQuantity = SOD.Quantity,
                                    PurchaseQuantity = 0,
                                    PurchaseReturnQuantity = 0,
                                    SalesReturnQuantity = 0,
                                    Quantity = -SOD.Quantity,
                                    PurchaseRate = STD.PRate


                                }).OrderBy(x => x.Date));
            #endregion

            #region Sales Return
            var Sales_returns = ((from SO in _ROrders
                                  join SOD in _ROrderDetails on SO.ROrderID equals SOD.ROrderID
                                  join P in _Products on SOD.ProductID equals P.ProductID
                                  join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                  join COM in _Companys on P.CompanyID equals COM.CompanyID
                                  join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                  join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                  select new StockLedger
                                  {
                                      Date = SO.ReturnDate,
                                      Code = P.Code,
                                      CategoryCode = CAT.Code,
                                      ProductID = P.ProductID,
                                      CategoryID = P.CategoryID,
                                      PCategoryID = CAT.PCategoryID,
                                      CompanyID = P.CompanyID,
                                      ModelID = 1,
                                      ColorID = CLR.ColorID,
                                      ProductName = P.ProductName,
                                      CompanyName = COM.Name,
                                      CategoryName = CAT.Description,
                                      ModelName = "",
                                      ColorName = CLR.Name,
                                      SalesReturnQuantity = SOD.Quantity,
                                      PurchaseQuantity = 0,
                                      PurchaseReturnQuantity = 0,
                                      Quantity = SOD.Quantity,
                                      PurchaseRate = STD.PRate

                                  }).OrderBy(x => x.Date));
            #endregion

            #region Hire Sales Return
            var HireSales_returns = ((from SO in _hireSalesReturn
                                      join CS in _CreditSales.Where(i => i.IsReturn == 1) on SO.CreditSalesId equals CS.CreditSalesID
                                      join SOD in _CreditSalesDetails.Where(i => i.IsProductReturn == 1) on CS.CreditSalesID equals SOD.CreditSalesID
                                      join P in _Products on SOD.ProductID equals P.ProductID
                                      join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                      join COM in _Companys on P.CompanyID equals COM.CompanyID
                                      join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                      join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                      select new StockLedger
                                      {
                                          Date = SO.TransactionDate,
                                          Code = P.Code,
                                          CategoryCode = CAT.Code,
                                          ProductID = P.ProductID,
                                          CategoryID = P.CategoryID,
                                          PCategoryID = CAT.PCategoryID,
                                          CompanyID = P.CompanyID,
                                          ModelID = 1,
                                          ColorID = CLR.ColorID,
                                          ProductName = P.ProductName,
                                          CompanyName = COM.Name,
                                          CategoryName = CAT.Description,
                                          ModelName = "",
                                          ColorName = CLR.Name,
                                          SalesReturnQuantity = SOD.Quantity,
                                          PurchaseQuantity = 0,
                                          PurchaseReturnQuantity = 0,
                                          Quantity = SOD.Quantity,
                                          PurchaseRate = STD.PRate

                                      }).OrderBy(x => x.Date));
            #endregion


            #region Replace
            var Rep = ((from SO in _SOrders
                        join SOD in _SOrderDetails on SO.SOrderID equals SOD.SOrderID
                        join P in _Products on SOD.ProductID equals P.ProductID
                        join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                        join COM in _Companys on P.CompanyID equals COM.CompanyID
                        join STD in _StockDetails on SOD.SDetailID equals STD.SDetailID
                        join CLR in _Colors on STD.ColorID equals CLR.ColorID
                        where SO.Status == (int)EnumSalesType.Sales && SOD.RStockDetailID > 0 && SOD.RepOrderID > 0
                        select new StockLedger
                        {
                            Date = SO.InvoiceDate,
                            Code = P.Code,
                            CategoryCode = CAT.Code,
                            ProductID = P.ProductID,
                            CategoryID = P.CategoryID,
                            PCategoryID = CAT.PCategoryID,
                            CompanyID = P.CompanyID,
                            ModelID = 1,
                            ColorID = CLR.ColorID,
                            ProductName = P.ProductName,
                            CompanyName = COM.Name,
                            CategoryName = CAT.Description,
                            ModelName = "",
                            ColorName = CLR.Name,
                            RepQty = SOD.RQuantity,
                            PurchaseQuantity = 0,
                            PurchaseReturnQuantity = 0,
                            SalesReturnQuantity = 0,
                            PurchaseRate = -STD.PRate,
                            Quantity = -SOD.Quantity

                        }).OrderBy(x => x.Date));
            #endregion

            List<StockLedger> Transdata = new List<StockLedger>();
            Transdata.AddRange(Purchases);
            Transdata.AddRange(Purchase_returns);
            Transdata.AddRange(Sales);
            Transdata.AddRange(CreditSales);
            Transdata.AddRange(Sales_returns);
            Transdata.AddRange(TransferIns);
            Transdata.AddRange(TransferOuts);
            Transdata.AddRange(Rep);
            Transdata.AddRange(HireSales_returns);
            List<StockLedger> DataGroupBy = new List<StockLedger>();

            if (reportType == 0)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.ProductID,
                                    item.PurchaseRate
                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.ProductID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().ProductName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),

                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),

                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),


                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),


                                })).ToList();

            }
            else if (reportType == 1)
            {

                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CompanyID,
                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CompanyID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CompanyName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),

                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),



                                })).ToList();
            }

            else if (reportType == 2)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CategoryID,
                                    //item.ColorID

                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CategoryID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CategoryName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,

                                    PrevPurchaseRate = g.Where(o => o.Date < fromDate).Sum(o => o.Quantity * o.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.Quantity * o.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),


                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.Date < fromDate).Sum(o => o.RepQty),

                                    TransferInQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.TransferOutQuantity),




                                })).ToList();
            }
            else if (reportType == 3)
            {
                DataGroupBy = ((from item in Transdata
                                join p in parentCategoryRepository.GetAll() on item.PCategoryID equals p.PCategoryID
                                group new { item, p } by new
                                {
                                    item.PCategoryID,
                                    p.Name
                                } into g
                                select new StockLedger
                                {
                                    CategoryID = g.FirstOrDefault().item.CategoryID,
                                    PCategoryID = g.Key.PCategoryID,
                                    ParentCategoryName = g.Key.Name,
                                    CompanyID = g.FirstOrDefault().item.CompanyID,
                                    ModelID = g.FirstOrDefault().item.ModelID,
                                    ProductName = g.FirstOrDefault().item.CategoryName,
                                    CompanyName = g.FirstOrDefault().item.CompanyName,
                                    CategoryName = g.FirstOrDefault().item.CategoryName,
                                    ModelName = g.FirstOrDefault().item.ModelName,
                                    Code = g.FirstOrDefault().item.Code,

                                    PrevPurchaseRate = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.Quantity * o.item.PurchaseRate),
                                    PurchaseRate = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.Quantity * o.item.PurchaseRate),

                                    PurchaseQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.PurchaseReturnQuantity),


                                    SalesQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.SalesReturnQuantity),
                                    RepQty = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.RepQty),

                                    PreviousSalesQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.PurchaseReturnQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.PurchaseQuantity),
                                    PreRepQty = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.RepQty),

                                    TransferInQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.TransferInQuantity),
                                    TransferOutQuantity = g.Where(o => o.item.Date >= fromDate && o.item.Date <= toDate).Sum(o => o.item.TransferOutQuantity),

                                    PreTransferInQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.TransferInQuantity),
                                    PreTransferOutQuantity = g.Where(o => o.item.Date < fromDate).Sum(o => o.item.TransferOutQuantity),



                                })).ToList();
            }

            var FinalData = (from d in DataGroupBy
                             select new StockLedger
                             {
                                 Date = DateTime.Now,
                                 ProductID = d.ProductID,
                                 CategoryID = d.CategoryID,
                                 CompanyID = d.CompanyID,
                                 ModelID = d.ModelID,
                                 ProductName = d.ProductName,
                                 CompanyName = d.CompanyName,
                                 CategoryName = d.CategoryName,
                                 ModelName = d.ModelName,
                                 Code = d.Code,
                                 PCategoryID = d.PCategoryID,
                                 ParentCategoryName = d.ParentCategoryName,

                                 OpeningStockQuantity = (d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity + d.PreRepQty)
                                                       - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity),

                                 PurchaseQuantity = d.PurchaseQuantity,
                                 PurchaseReturnQuantity = d.PurchaseReturnQuantity,

                                 SalesQuantity = d.SalesQuantity,
                                 SalesReturnQuantity = d.SalesReturnQuantity,
                                 RepQty = d.RepQty,

                                 TransferInQuantity = d.TransferInQuantity,
                                 TransferOutQuantity = d.TransferOutQuantity,

                                 ClosingStockQuantity = (
                                                            ((d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity + d.PreRepQty)
                                                           - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity)
                                                            )
                                                            +
                                                            (
                                                              (d.PurchaseQuantity + d.TransferInQuantity + d.SalesReturnQuantity + d.RepQty) - (d.PurchaseReturnQuantity + d.TransferOutQuantity + d.SalesQuantity)
                                                            )
                                                        ),
                                 OpeningStockValue = 0m,
                                 TotalStockValue = 0m,
                                 //ClosingStockValue = (
                                 //                           ((d.PreviousPurchaseQuantity + d.PreTransferInQuantity + d.PreviousSalesReturnQuantity)
                                 //                          - (d.PreviousPurchaseReturnQuantity + d.PreTransferOutQuantity + d.PreviousSalesQuantity)
                                 //                           )
                                 //                           +
                                 //                           (
                                 //                             (d.PurchaseQuantity + d.TransferInQuantity + d.SalesReturnQuantity) - (d.PurchaseReturnQuantity + d.TransferOutQuantity + d.SalesQuantity)
                                 //                           )
                                 //                       ) * d.PurchaseRate

                                 ClosingStockValue = d.PrevPurchaseRate + d.PurchaseRate,


                             }).OrderBy(i => i.Code).ToList();



            return FinalData;
        }


        public static IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>>
            GetforStockReportZeroQty(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Godown> GodownRepository, IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<ParentCategory> ParentCategoryRepository,
            string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType)
        {
            var Products = productRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);
            var StockDetails = StockDetailRepository.All;
            var PCategory = ParentCategoryRepository.All;
            if (PCategoryID != 0)
                PCategory = PCategory.Where(i => i.PCategoryID == PCategoryID);


            var Godowns = GodownRepository.All;
            var Colors = colorRepository.All;

            if (GodownID != 0)
                Godowns = Godowns.Where(o => o.GodownID == GodownID);

            if (ColorID != 0)
                Colors = Colors.Where(o => o.ColorID == ColorID);

            List<ProductDetailsModel> finalData = new List<ProductDetailsModel>();

            if (StockType == 2)
            {
                var data = (from st in stockRepository.All
                            join pro in Products on st.ProductID equals pro.ProductID
                            join col in Colors on st.ColorID equals col.ColorID
                            join god in Godowns on st.GodownID equals god.GodownID
                            join pc in PCategory on pro.Category.PCategoryID equals pc.PCategoryID
                            select new
                            {
                                StockID = st.StockID,
                                productName = pro.ProductName,
                                CompanyName = pro.Company.Name,
                                CategoryName = pro.Category.Description,
                                ColorName = col.Name,
                                GodownName = god.Name,
                                Qty = st.Quantity,
                                MRPRate = st.LPPrice,
                                PCategoryName = pc.Name,
                                ProMRP = pro.MRP,
                                SalesPrice = st.MRPPrice
                            }).ToList();

                var oStockData = (from s in data
                                  group s by new
                                  {
                                      s.productName,
                                      s.CompanyName,
                                      s.StockID,
                                      s.ColorName,
                                      s.GodownName,
                                      //s.PRate,
                                      s.CategoryName,
                                      s.PCategoryName,
                                      s.ProMRP
                                  } into g
                                  select new ProductDetailsModel
                                  {
                                      StockID = g.Key.StockID,
                                      ProductName = g.Key.productName,
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      ColorName = g.Key.ColorName,
                                      GodownName = g.Key.GodownName,
                                      StockQty = g.Sum(i => i.Qty),
                                      PCategoryName = g.Key.PCategoryName,
                                      ProMRP = (decimal)g.Key.ProMRP,
                                      MRPRate = g.Select(i => i.MRPRate).FirstOrDefault(),
                                      SalesPrice = g.Select(i => i.SalesPrice).FirstOrDefault(),
                                  }).ToList();


                if (IsVATManager)
                {
                    var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                    decimal FalesStock = (oStockData.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                    decimal FalesStockCount = 0m;

                    foreach (var item in oStockData)
                    {
                        FalesStockCount += item.StockQty;
                        if (FalesStockCount <= FalesStock)
                            finalData.Add(item);
                        else
                            break;
                    }
                }
                else
                    finalData = oStockData;
            }
            else
            {
                var data = (from st in stockRepository.All
                            join pro in Products on st.ProductID equals pro.ProductID
                            join col in Colors on st.ColorID equals col.ColorID
                            join god in Godowns on st.GodownID equals god.GodownID
                            join pc in PCategory on pro.Category.PCategoryID equals pc.PCategoryID
                            select new
                            {
                                StockID = st.StockID,
                                productName = pro.ProductName,
                                CompanyName = pro.Company.Name,
                                CategoryName = pro.Category.Description,
                                ColorName = col.Name,
                                GodownName = god.Name,
                                Qty = st.Quantity,
                                MRPRate = st.LPPrice,
                                PCategoryName = pc.Name,
                                ProMRP = pro.MRP,
                                SalesPrice = st.MRPPrice

                            }).ToList();

                var oStockData = (from s in data
                                  group s by new
                                  {
                                      s.productName,
                                      s.CompanyName,
                                      s.StockID,
                                      s.ColorName,
                                      s.GodownName,
                                      //s.PRate,
                                      s.CategoryName,
                                      s.PCategoryName,
                                      s.ProMRP
                                  } into g
                                  select new ProductDetailsModel
                                  {
                                      StockID = g.Key.StockID,
                                      ProductName = g.Key.productName,
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      ColorName = g.Key.ColorName,
                                      GodownName = g.Key.GodownName,
                                      StockQty = g.Sum(i => i.Qty),
                                      PCategoryName = g.Key.PCategoryName,
                                      ProMRP = (decimal)g.Key.ProMRP,
                                      MRPRate = g.Select(i => i.MRPRate).FirstOrDefault(),
                                      SalesPrice = g.Select(i => i.SalesPrice).FirstOrDefault(),
                                  }).ToList();


                if (IsVATManager)
                {
                    var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                    decimal FalesStock = (oStockData.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                    decimal FalesStockCount = 0m;

                    foreach (var item in oStockData)
                    {
                        FalesStockCount += item.StockQty;
                        if (FalesStockCount <= FalesStock)
                            finalData.Add(item);
                        else
                            break;
                    }
                }
                else
                    finalData = oStockData;
            }






            return finalData.Select(x => new Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>>
                                    (
                                    x.StockID,
                                    x.ProductName,
                                    x.CompanyName,
                                    x.CategoryName,
                                    x.ColorName,
                                    x.StockQty,
                                    x.MRPRate,//x.MRPRate,
                                    new Tuple<decimal, decimal, decimal, decimal, string, List<string>, string>
                                        (
                                         x.SalesPrice,
                                         x.CreditSalesPrice,
                                         x.CreditSalesPrice3,
                                         x.ProMRP,
                                         x.GodownName,
                                         x.IMEIList, x.PCategoryName
                                        )
                                    ));
        }


        public static IEnumerable<Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, decimal>>>
          GetforReportNew(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
          IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository,
          IBaseRepository<Godown> GodownRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
          string userName, int concernID, int reportType, List<int> CompanyIds, List<int> CategoriesList, List<int> ProductIds, List<int> GodownIds, List<int> ColorIds, bool IsVATManager)
        {
            var Products = productRepository.All;
            if (CompanyIds != null && CompanyIds.Any())
                Products = Products.Where(i => CompanyIds.Contains(i.CompanyID));
            if (CategoriesList != null && CategoriesList.Any())
                //Products = Products.Where(i => i.CategoryID == CategoriesList);
                Products = Products.Where(i => CategoriesList.Contains(i.CategoryID));
            if (ProductIds != null && ProductIds.Any())
                Products = Products.Where(i => ProductIds.Contains(i.ProductID));
            var StockDetails = StockDetailRepository.All;


            var Godowns = GodownRepository.All;
            var Colors = colorRepository.All;

            if (GodownIds != null && GodownIds.Any())
                Godowns = Godowns.Where(o => GodownIds.Contains(o.GodownID));

            if (ColorIds != null && ColorIds.Any())
                Colors = Colors.Where(o => ColorIds.Contains(o.ColorID));

            List<ProductDetailsModel> finalData = new List<ProductDetailsModel>();

            var data = (from st in stockRepository.All
                        join sd in StockDetailRepository.All on st.StockID equals sd.StockID
                        join pro in Products on st.ProductID equals pro.ProductID
                        join col in Colors on st.ColorID equals col.ColorID
                        join god in Godowns on st.GodownID equals god.GodownID
                        where sd.Status == (int)EnumStockStatus.Stock
                        select new
                        {
                            StockID = st.StockID,
                            productName = pro.ProductName,
                            CompanyName = pro.Company.Name,
                            CategoryName = pro.Category.Description,
                            ColorName = col.Name,
                            GodownName = god.Name,
                            Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : 1,
                            //Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? sd.Quantity : StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.GodownID == st.GodownID && i.Status == (int)EnumStockStatus.Stock).Count(),
                            MRPRate = st.MRPPrice,
                            // MRPRate = sd.SRate,
                            sd.PRate,
                            sd.SRate,
                            sd.CRSalesRate3Month,
                            sd.CreditSRate,
                            sd.CRSalesRate12Month,
                            sd.IMENO,
                            //StockDetails = StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.GodownID==st.GodownID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault(),
                            //SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                            //    ? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().SRate : 0m,
                            //CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null 
                            //? StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).OrderByDescending(i=>i.SDetailID).FirstOrDefault().CreditSRate : 0m
                        }).Where(st => st.Qty > 0).ToList();

            var oStockData = (from s in data
                              group s by new
                              {
                                  s.productName,
                                  s.CompanyName,
                                  s.StockID,
                                  s.ColorName,
                                  s.GodownName,
                                  //s.PRate,
                                  //s.SRate,
                                  s.CategoryName
                              } into g
                              select new ProductDetailsModel
                              {
                                  StockID = g.Key.StockID,
                                  ProductName = g.Key.productName,
                                  CompanyName = g.Key.CompanyName,
                                  CategoryName = g.Key.CategoryName,
                                  ColorName = g.Key.ColorName,
                                  GodownName = g.Key.GodownName,
                                  StockQty = g.Sum(i => i.Qty),
                                  //MRPRate = g.Key.SRate,
                                  //PRate = g.Key.PRate,
                                  PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                  SalesPrice = g.Select(i => i.SRate).FirstOrDefault(),
                                  CreditSalesPrice3 = g.Select(i => i.CRSalesRate3Month).FirstOrDefault(),
                                  CreditSalesPrice = g.Select(i => i.CreditSRate).FirstOrDefault(),
                                  CreditSalesPrice12 = g.Select(i => i.CRSalesRate12Month).FirstOrDefault(),
                                  IMEIList = g.Select(i => i.IMENO).ToList()
                              }).ToList();

            if (IsVATManager)
            {
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                decimal FalesStock = (oStockData.Sum(i => i.StockQty) * oConcern.StockShowPercent) / 100m;
                decimal FalesStockCount = 0m;

                foreach (var item in oStockData)
                {
                    FalesStockCount += item.StockQty;
                    if (FalesStockCount <= FalesStock)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = oStockData;

            return finalData.Select(x => new Tuple<int, string, string, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, List<string>, decimal>>
                                    (
                                    x.StockID,
                                    x.ProductName,
                                    x.CompanyName,
                                    x.CategoryName,
                                    x.ColorName,
                                    x.StockQty,
                                    x.MRPRate,//x.MRPRate,
                                    new Tuple<decimal, decimal, decimal, decimal, string, List<string>, decimal>
                                        (
                                         x.SalesPrice,
                                         x.CreditSalesPrice,
                                         x.CreditSalesPrice3,
                                         x.CreditSalesPrice12,
                                         x.GodownName,
                                         x.IMEIList,
                                         x.PRate
                                        )
                                    ));
        }


    }
}
