using IMSWEB.Model;
using IMSWEB.SPViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SRVisitExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, EnumSRVisitType>>> GetAllSRVisitAsync(this IBaseRepository<SRVisit> srVisitRepository,
            IBaseRepository<Employee> employeeRepository)
        {
            IQueryable<Employee> employees = employeeRepository.All;

            var items = await srVisitRepository.All.
                GroupJoin(employees, p => p.EmployeeID, s => s.EmployeeID,
                (p, s) => new { SRVisit = p, Employees = s }).
                SelectMany(x => x.Employees.DefaultIfEmpty(), (p, s) => new { SRVisit = p.SRVisit, Employee = s })
                .Select(x => new
                {
                    x.SRVisit.SRVisitID,
                    x.SRVisit.ChallanNo,
                    x.SRVisit.VisitDate,
                    x.Employee.Name,
                    //x.Employee.FName,
                    x.Employee.ContactNo,
                    x.SRVisit.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, EnumSRVisitType>
                (
                    x.SRVisitID,
                    x.ChallanNo,
                    x.VisitDate,
                    x.Name,
                //x.FName,
                    x.ContactNo,
                    (EnumSRVisitType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }


        public static IEnumerable<Tuple<string, string, DateTime, string>> GetSRVisitReportByConcernID(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<Employee> employeeRepository, DateTime fromDate, DateTime toDate, int concernID)
        {
            var oPurchaseData = (from pOrd in SRVisitRepository.All
                                 join sup in employeeRepository.All on pOrd.EmployeeID equals sup.EmployeeID
                                 where (pOrd.VisitDate >= fromDate && pOrd.VisitDate <= toDate && pOrd.Status == 1 && pOrd.ConcernID == concernID)
                                 group pOrd by new
                                 {
                                     sup.Code,
                                     sup.Name,
                                     pOrd.VisitDate,
                                     pOrd.ChallanNo
                                 } into g
                                 select new
                                 {
                                     g.Key.Code,
                                     g.Key.Name,
                                     g.Key.VisitDate,
                                     g.Key.ChallanNo
                                 }).ToList();
            return oPurchaseData.Select(x => new Tuple<string, string, DateTime, string>
                  (
                   x.Code,
                   x.Name,
                   x.VisitDate,
                   x.ChallanNo
                  ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, string, string>>
           GetSRVisitDetailReportByConcernID(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<Product> productRepository,
           IBaseRepository<SRVProductDetail> SRVProductRepository, IBaseRepository<Color> colorRepository, DateTime fromDate, DateTime toDate, int concernID)
        {
            var oSRVisitDetailData = (from SRVD in SRVisitDetailRepository.All //POD
                                      from SRV in SRVisitRepository.All//PO
                                      from P in productRepository.All
                                      from SRVP in SRVProductRepository.All //POP
                                      from C in colorRepository.All
                                      where (SRVD.SRVisitID == SRV.SRVisitID && SRVD.SRVisitDID == SRVP.SRVisitDID && P.ProductID == SRVD.ProductID && C.ColorID == SRVP.ColorID && SRV.VisitDate >= fromDate && SRV.VisitDate <= toDate && SRV.Status == 1 && SRV.ConcernID == concernID)
                                      select new { SRV.ChallanNo, SRV.VisitDate, P.ProductID, P.ProductName, CateName = P.Category.Description, SRVP.IMENO, ColorName = C.Name }).OrderByDescending(x => x.VisitDate).ToList();

            return oSRVisitDetailData.Select(x => new Tuple<DateTime, string, string, string, string, string>
                (
                 x.VisitDate,
                 x.ChallanNo,
                 x.ProductName,
                 x.CateName,
                 x.IMENO,
                 x.ColorName
                ));
        }


        public static IEnumerable<Tuple<int, int, int, decimal, string, string, int, Tuple<string>>>
            GetSRVisitDetailById(this IBaseRepository<SRVisitDetail> srVisitDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, int orderId)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;

            var items = srVisitDetailRepository.All.
                GroupJoin(products, d => d.ProductID, p => p.ProductID,
                (d, p) => new { SRVisitDetail = d, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (d, p) => new { SRVisitDetail = d.SRVisitDetail, Products = p }).
                GroupJoin(colors, d => d.SRVisitDetail.ColorID, c => c.ColorID,
                (d, c) => new { SRVisitDetail = d.SRVisitDetail, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SRVisitDetail = d.SRVisitDetail, Products = d.Products, Color = c })
                .Where(x => x.SRVisitDetail.SRVisitID == orderId)
                .Select(x => new
                {
                    x.SRVisitDetail.SRVisitDID,
                    x.SRVisitDetail.ProductID,
                    x.SRVisitDetail.SRVisitID,
                    x.SRVisitDetail.Quantity,
                    x.Products.ProductName,
                    ProductCode = x.Products.Code,
                    x.Color.ColorID,
                    ColorName = x.Color.Name
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, decimal, string, string, int, Tuple<string>>
                (
                    x.SRVisitDID,
                    x.ProductID,
                    x.SRVisitID,
                    x.Quantity,
                    x.ProductName,
                    x.ProductCode,
                    x.ColorID,
                    new Tuple<string>(x.ColorName)

                ));
        }



        public static IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<string>>>
        GetSRViistDetailReportByEmployeeID(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVDetailRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<SRVProductDetail> SRVProductDetailRepository, DateTime fromDate, DateTime toDate, int ConcernID, int EmployeeID)
        {
            var oSalesDetailData = (from SRVD in SRVDetailRepository.All//POD
                                    from SRV in SRVisitRepository.All//PO
                                    from P in productRepository.All
                                    from SRVPD in SRVProductDetailRepository.All//POPD
                                    where (SRVD.SRVisitID == SRV.SRVisitID && SRVD.SRVisitDID == SRVPD.SRVisitDID && P.ProductID == SRVD.ProductID && SRV.VisitDate >= fromDate && SRV.VisitDate <= toDate && SRV.Status == 1 && SRV.EmployeeID == EmployeeID)
                                    select new { SRV.ChallanNo, SRV.VisitDate, P.ProductID, P.ProductName, SRVD.Quantity, SRVPD.IMENO, SRV.Employee.Name, SRV.Employee.Code, SRV.Employee.ContactNo }).OrderByDescending(x => x.VisitDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, string, string, string, Tuple<string>>
                (
                 x.VisitDate,
                 x.ChallanNo,
                 x.ProductName,
                 x.Quantity,
                 x.IMENO,
                 x.Name,
                 x.Code,
                 new Tuple<string>(
                                    x.ContactNo
                                   )
                ));
        }

        public static bool IsIMEIAlreadyIssuedToSR(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository, int ProductID, int ColorID, string IMEINO)
        {
            var SRVPD = SRVProductDetailRepository.All.Where(i => i.ProductID == ProductID && i.ColorID == ColorID && i.IMENO.Equals(IMEINO.Trim()) && (i.Status == (int)EnumStockStatus.Stock || i.Status == (int)EnumStockStatus.Sold)).ToList();
            if (SRVPD.Count() != 0)
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<AdvancePODetail> GetAllSRVisitStockIMEIBySRID(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, int EmployeeID)
        {
            var SRVisits = SRVisitRepository.All.Where(i => i.EmployeeID == EmployeeID);
            var SRVisitDetails = SRVisitDetailRepository.All;
            var SRVPD = SRVProductDetailRepository.All.Where(i => i.Status == (int)EnumStockStatus.Stock);
            var Products = ProductRepository.All;
            var Categories = CategoryRepository.All;

            var result = from srv in SRVisits
                         join srvd in SRVisitDetails on srv.SRVisitID equals srvd.SRVisitID
                         join srvpd in SRVPD on srvd.SRVisitDID equals srvpd.SRVisitDID
                         join p in Products on srvd.ProductID equals p.ProductID
                         join c in Categories on p.CategoryID equals c.CategoryID
                         select new AdvancePODetail
                         {
                             ID = srvpd.SRVisitPDID,
                             ProductCode = p.Code,
                             ProductName = p.ProductName,
                             IMEI = srvpd.IMENO,
                             CategoryName = c.Description
                         };
            return result;

        }

        public static IEnumerable<SRVisitReportModel> SRVisitReportDetails(this IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository,
         IBaseRepository<SRVProductDetail> SRVProductDetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<SOrder> SOrderRepository,
         IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<StockDetail> StockDetailRepository,
         IBaseRepository<Stock> StockRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Customer> CustomerRepository,
         IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
         DateTime fromDate, DateTime toDate, int EmployeeID)
        {
            List<SRVisitDate> Dates = new List<SRVisitDate>();
            List<SRVisitReportMain> ReportMainList = new List<SRVisitReportMain>();
            SRVisitReportMain ReportMain = null;
            DateTime PreviousDate = DateTime.MinValue;

            var SRVisitDates = (from sv in SRVisitRepository.All
                                where sv.EmployeeID == EmployeeID && sv.Status == (int)EnumSRVisitType.Live
                                select new SRVisitDate
                                {
                                    Date = sv.VisitDate
                                }).ToList();
            Dates.AddRange(SRVisitDates);

            var InvoiceDate = (from so in SOrderRepository.All
                               join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                               join emp in EmployeeRepository.All on c.EmployeeID equals emp.EmployeeID
                               where so.Status == (int)EnumSalesType.Sales && emp.EmployeeID == EmployeeID
                               select new SRVisitDate
                               {
                                   Date = so.InvoiceDate
                               }).ToList();

            Dates.AddRange(InvoiceDate);

            Dates = (from d in Dates
                     group d by d.Date into g
                     select new SRVisitDate
                     {
                         Date = g.Key
                     }).OrderBy(i => i.Date).ToList();
            int counter = 0;
            foreach (var item in Dates)
            {
                counter++;
                ReportMain = new SRVisitReportMain();
                ReportMain.TransDate = item.Date;
                ReportMain.ID = counter;
                var PrevDateData = ReportMainList.FirstOrDefault(i => i.ID == (counter - 1));
                if (PrevDateData != null)
                {
                    ReportMain.OpeningIMEIS = PrevDateData.BalanceIMEIS;
                }
                var SRVisits = from sv in SRVisitRepository.All
                               join svd in SRVisitDetailRepository.All on sv.SRVisitID equals svd.SRVisitID
                               join svpd in SRVProductDetailRepository.All on svd.SRVisitDID equals svpd.SRVisitDID
                               join p in productRepository.All on svd.ProductID equals p.ProductID
                               where (sv.Status == (int)EnumSRVisitType.Live && sv.EmployeeID == EmployeeID && sv.VisitDate == item.Date && (svpd.Status == (int)EnumSRVProductDetailsStatus.Stock || svpd.Status == (int)EnumSRVProductDetailsStatus.Sold))
                               select new SRVisitReportModel
                               {
                                   TransDate = sv.VisitDate,
                                   ProductID = svpd.ProductID,
                                   ColorID = svpd.ColorID,
                                   taken_product = p.ProductName,
                                   SDetailID = svpd.SDetailID,
                                   taken_imeno = svpd.IMENO
                               };

                ReportMain.ReceiveIMEIS.AddRange(SRVisits);
                ReportMain.TotalIMEIS.AddRange(ReportMain.OpeningIMEIS);
                ReportMain.TotalIMEIS.AddRange(ReportMain.ReceiveIMEIS);

                var SOrders = from so in SOrderRepository.All
                              join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                              join p in productRepository.All on sod.ProductID equals p.ProductID
                              join sd in StockDetailRepository.All on sod.SDetailID equals sd.SDetailID
                              join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                              join emp in EmployeeRepository.All on c.EmployeeID equals emp.EmployeeID
                              where (so.Status == (int)EnumSalesType.Sales && emp.EmployeeID == EmployeeID && so.InvoiceDate == item.Date)
                              select new SRVisitReportModel
                              {
                                  TransDate = so.InvoiceDate,
                                  ProductID = sd.ProductID,
                                  ColorID = sd.ColorID,
                                  taken_product = p.ProductName,
                                  SDetailID = sd.SDetailID,
                                  taken_imeno = sd.IMENO
                              };

                ReportMain.SalesIMEIS.AddRange(SOrders);

                var Replacements = from so in SOrderRepository.All
                                   join sod in SOrderDetailRepository.All on so.SOrderID equals sod.RepOrderID
                                   join sd in StockDetailRepository.All on sod.RStockDetailID equals sd.SDetailID
                                   join p in productRepository.All on sd.ProductID equals p.ProductID
                                   join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                                   join emp in EmployeeRepository.All on c.EmployeeID equals emp.EmployeeID
                                   where (so.Status == (int)EnumSalesType.Sales && emp.EmployeeID == EmployeeID && so.InvoiceDate == item.Date && so.IsReplacement == 1)
                                   select new SRVisitReportModel
                                   {
                                       TransDate = so.InvoiceDate,
                                       ProductID = sd.ProductID,
                                       ColorID = sd.ColorID,
                                       taken_product = p.ProductName,
                                       SDetailID = sd.SDetailID,
                                       taken_imeno = sd.IMENO
                                   };

                ReportMain.SalesIMEIS.AddRange(Replacements);
                ReportMain.SalesIMEIS = ReportMain.SalesIMEIS.Where(i => (ReportMain.TotalIMEIS.Select(j => j.SDetailID).Contains(i.SDetailID))).ToList();
                ReportMain.BalanceIMEIS = ReportMain.TotalIMEIS.Where(i => !(ReportMain.SalesIMEIS.Select(j => j.SDetailID).Contains(i.SDetailID))).ToList();

                ReportMainList.Add(ReportMain);

            }

            List<SRVisitReportMain> Result = ReportMainList.Where(i => i.TransDate >= fromDate && i.TransDate <= toDate).ToList();
            if (Result.Count() == 0)
            {
                var LastTrans = ReportMainList.OrderByDescending(i => i.TransDate).FirstOrDefault();
                Result.Add(LastTrans);
            }
            List<SRVisitReportModel> SRVisitReport = new List<SRVisitReportModel>();
            SRVisitReportModel oReport = null;
            foreach (var item in Result)
            {

                var Opening = (from d in item.OpeningIMEIS
                               group d by new { d.ProductID, d.ColorID, d.taken_product } into g
                               select new SRVisitReportModel
                               {
                                   TransDate = item.TransDate,
                                   ProductID = g.Key.ProductID,
                                   ProductName = g.Key.taken_product,
                                   OpeningIMEIList = g.Select(i => i.taken_imeno).ToList()
                               }).ToList();
                var Receives = (from d in item.ReceiveIMEIS
                                group d by new { d.ProductID, d.ColorID, d.taken_product } into g
                                select new SRVisitReportModel
                                {
                                    TransDate = item.TransDate,
                                    ProductID = g.Key.ProductID,
                                    ProductName = g.Key.taken_product,
                                    ReceiveIMEIList = g.Select(i => i.taken_imeno).ToList()
                                }).ToList();

                var TotalProducts = (from d in item.TotalIMEIS
                                     group d by new { d.ProductID, d.ColorID, d.taken_product } into g
                                     select new SRVisitReportModel
                                     {
                                         TransDate = item.TransDate,
                                         ProductID = g.Key.ProductID,
                                         ProductName = g.Key.taken_product,
                                         TotalIMEIList = g.Select(i => i.taken_imeno).ToList()
                                     }).OrderBy(i => i.TransDate).ToList();
                var Sales = (from d in item.SalesIMEIS
                             group d by new { d.ProductID, d.ColorID, d.taken_product } into g
                             select new SRVisitReportModel
                             {
                                 TransDate = item.TransDate,
                                 ProductID = g.Key.ProductID,
                                 ProductName = g.Key.taken_product,
                                 SalesIMEIList = g.Select(i => i.taken_imeno).ToList()
                             }).ToList();
                var Balances = (from d in item.BalanceIMEIS
                                group d by new { d.ProductID, d.ColorID, d.taken_product } into g
                                select new SRVisitReportModel
                                {
                                    TransDate = item.TransDate,
                                    ProductID = g.Key.ProductID,
                                    ProductName = g.Key.taken_product,
                                    BalanceIMEIList = g.Select(i => i.taken_imeno).ToList()
                                }).ToList();

                foreach (var sitem in TotalProducts)
                {
                    oReport = new SRVisitReportModel();
                    oReport.TransDate = item.TransDate;
                    oReport.ProductID = sitem.ProductID;
                    oReport.ProductName = sitem.ProductName;
                    oReport.OpeningIMEIList = Opening.FirstOrDefault(i => i.ProductID == sitem.ProductID) != null ? Opening.FirstOrDefault(i => i.ProductID == sitem.ProductID).OpeningIMEIList : new List<string>();
                    oReport.ReceiveIMEIList = Receives.FirstOrDefault(i => i.ProductID == sitem.ProductID) != null ? Receives.FirstOrDefault(i => i.ProductID == sitem.ProductID).ReceiveIMEIList : new List<string>();
                    oReport.TotalIMEIList = TotalProducts.FirstOrDefault(i => i.ProductID == sitem.ProductID) != null ? TotalProducts.FirstOrDefault(i => i.ProductID == sitem.ProductID).TotalIMEIList : new List<string>();
                    oReport.SalesIMEIList = Sales.FirstOrDefault(i => i.ProductID == sitem.ProductID) != null ? Sales.FirstOrDefault(i => i.ProductID == sitem.ProductID).SalesIMEIList : new List<string>();
                    oReport.BalanceIMEIList = Balances.FirstOrDefault(i => i.ProductID == sitem.ProductID) != null ? Balances.FirstOrDefault(i => i.ProductID == sitem.ProductID).BalanceIMEIList : new List<string>();
                    SRVisitReport.Add(oReport);
                }


            }
            return SRVisitReport;
        }


    }
}
