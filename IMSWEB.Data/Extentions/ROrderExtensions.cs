using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ROrderExtensions
    {




        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, string>>> GetAllReturnOrderAsync(this IBaseRepository<ROrder> returnOrderRepository,
            IBaseRepository<Customer> customerRepository)
        {
            IQueryable<Customer> customers = customerRepository.All;

            var items = await returnOrderRepository.All.
                GroupJoin(customers, p => p.CustomerID, s => s.CustomerID,
                (p, c) => new { ReturnOrder = p, Customers = c }).
                SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { ReturnOrder = p.ReturnOrder, Customer = c })
                .Select(x => new
                {
                    x.ReturnOrder.ROrderID,
                    x.ReturnOrder.InvoiceNo,
                    x.ReturnOrder.ReturnDate,
                    x.Customer.Name,
                    x.Customer.CompanyName,
                    x.Customer.ContactNo
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string>
                (
                    x.ROrderID,
                    x.InvoiceNo,
                    x.ReturnDate,
                    x.Name,
                    x.CompanyName,
                    x.ContactNo
                )).ToList();
        }


        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
           GetReturnReportByConcernID(this IBaseRepository<ROrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
           IBaseRepository<ROrderDetail> ROrderDetailRepsitory,
           DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerType == 0)
                Customers = customerRepository.All;
            else if (CustomerType == (int)EnumCustomerType.Retail)
                Customers = customerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Retail);
            else if (CustomerType == (int)EnumCustomerType.Dealer)
                Customers = customerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Dealer);
            else if (CustomerType == (int)EnumCustomerType.Hire)
                Customers = customerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Hire);
            //else if (CustomerType == (int)EnumCustomerType.Project)
            //    Customers = customerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Project);
            //else if (CustomerType == (int)EnumCustomerType.Mela)
            //    Customers = customerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Mela);

            var oSalesData = (from SO in salesOrderRepository.All
                              join SOD in ROrderDetailRepsitory.All on SO.ROrderID equals SOD.ROrderID
                              join cus in Customers on SO.CustomerID equals cus.CustomerID
                              where (SO.ReturnDate >= fromDate && SO.ReturnDate <= toDate)
                              group SO by new
                              {
                                  cus.Code,
                                  cus.Name,
                                  SO.InvoiceNo,
                                  InvoiceDate = SO.ReturnDate,
                                  SO.GrandTotal,
                                  NetDiscount = 0m,
                                  TotalAmount = SO.GrandTotal,
                                  RecAmount = SO.PaidAmount,
                                  PaymentDue = SO.GrandTotal - SO.PaidAmount,
                                  AdjAmount = 0m,

                              } into g
                              select new
                              {
                                  g.Key.Code,
                                  g.Key.Name,
                                  g.Key.InvoiceDate,
                                  g.Key.InvoiceNo,
                                  g.Key.GrandTotal,
                                  g.Key.NetDiscount,
                                  g.Key.TotalAmount,
                                  g.Key.RecAmount,
                                  g.Key.PaymentDue,
                                  g.Key.AdjAmount,
                                  TotalOffer = 0m
                              }).ToList();

            //var Replacements = (from SO in salesOrderRepository.All
            //                    join SOD in ROrderDetailRepsitory.All on SO.SOrderID equals SOD.RepOrderID
            //                    join cus in Customers on SO.CustomerID equals cus.CustomerID
            //                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
            //                    group SO by new
            //                    {
            //                        cus.Code,
            //                        cus.Name,
            //                        InvoiceNo = "REP-" + SO.InvoiceNo,
            //                        SO.InvoiceDate,
            //                        SO.GrandTotal,
            //                        SO.NetDiscount,
            //                        SO.TotalAmount,
            //                        SO.RecAmount,
            //                        SO.PaymentDue,
            //                        SO.AdjAmount,

            //                    } into g
            //                    select new
            //                    {
            //                        g.Key.Code,
            //                        g.Key.Name,
            //                        g.Key.InvoiceDate,
            //                        g.Key.InvoiceNo,
            //                        g.Key.GrandTotal,
            //                        g.Key.NetDiscount,
            //                        g.Key.TotalAmount,
            //                        g.Key.RecAmount,
            //                        g.Key.PaymentDue,
            //                        g.Key.AdjAmount,
            //                        TotalOffer = g.Select(i => i.SOrderDetails).FirstOrDefault()
            //                    }).ToList();

            //oSalesData.AddRange(Replacements);

            return oSalesData.Select(x => new Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                (
                 x.Code,
                 x.Name,
                x.InvoiceDate,
                                    x.InvoiceNo,
                                    x.GrandTotal,
                                    x.NetDiscount,
                                    x.TotalAmount,
                                     new Tuple<decimal, decimal, decimal, decimal>(
                                    (decimal)x.RecAmount,
                                    x.PaymentDue,
                                    x.AdjAmount,
                                    x.TotalOffer
                                    )

                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, decimal, int, string, int, int, string,
                    Tuple<decimal>>>>>
        GetReturnDetailReportByConcernID(this IBaseRepository<ROrder> salesOrderRepository,
                IBaseRepository<ROrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
                IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Customer> customeRepository,
                IBaseRepository<Category> categoryRepository, IBaseRepository<SisterConcern> sisterconcerRepository,
                IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailsRepository,
                IBaseRepository<HireSalesReturnCustomerDueAdjustment> creditSalesReturnRepository,
                DateTime fromDate, DateTime toDate, int CustomerType, int concernID, bool IsAdminReport)
        {
            IQueryable<Customer> Customers = null;
            IQueryable<ROrder> sOrders = null;
            IQueryable<Product> products = null;
            IQueryable<Category> categories = null;
            IQueryable<SisterConcern> concerns = null;
            IQueryable<HireSalesReturnCustomerDueAdjustment> creditReturns = null;
            IQueryable<CreditSale> creditSaleOrders = null;

            if (IsAdminReport)
            {
                if (concernID > 0)
                {
                    if (CustomerType > 0)
                        Customers = customeRepository.GetAll().Where(i => i.CustomerType == (EnumCustomerType)CustomerType && i.ConcernID == concernID);
                    else
                        Customers = customeRepository.GetAll().Where(i => i.ConcernID == concernID);

                    sOrders = salesOrderRepository.GetAll().Where(i => i.ConcernID == concernID);
                    products = productRepository.GetAll().Where(i => i.ConcernID == concernID);
                    categories = categoryRepository.GetAll().Where(i => i.ConcernID == concernID);
                    concerns = sisterconcerRepository.GetAll().Where(i => i.ConcernID == concernID);
                    creditSaleOrders = creditSalesRepository.GetAll().Where(i => i.ConcernID == concernID);
                    creditReturns = creditSalesReturnRepository.GetAll().Where(i => i.ConcernId == concernID);
                }
                else
                {
                    if (CustomerType > 0)
                        Customers = customeRepository.GetAll().Where(i => i.CustomerType == (EnumCustomerType)CustomerType);
                    else
                        Customers = customeRepository.GetAll();

                    sOrders = salesOrderRepository.GetAll();
                    products = productRepository.GetAll();
                    categories = categoryRepository.GetAll();
                    concerns = sisterconcerRepository.GetAll();
                    creditSaleOrders = creditSalesRepository.GetAll();
                    creditReturns = creditSalesReturnRepository.GetAll();
                }
            }
            else
            {
                if (CustomerType > 0)
                    Customers = customeRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);
                else
                    Customers = customeRepository.All;

                sOrders = salesOrderRepository.All;

                products = productRepository.All;
                categories = categoryRepository.All;
                concerns = sisterconcerRepository.All;
                creditSaleOrders = creditSalesRepository.All;
                creditReturns = creditSalesReturnRepository.All;
            }

            var oSalesDetailData = (from SO in sOrders
                                    join sis in concerns on SO.ConcernID equals sis.ConcernID
                                    join CUS in Customers on SO.CustomerID equals CUS.CustomerID
                                    join SOD in SorderDetailRepository.All on SO.ROrderID equals SOD.ROrderID
                                    join P in products on SOD.ProductID equals P.ProductID
                                    join CAT in categories on P.CategoryID equals CAT.CategoryID
                                    join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                    where (SO.ReturnDate >= fromDate && SO.ReturnDate <= toDate)
                                    select new
                                    {
                                        SO.ROrderID,
                                        SO.InvoiceNo,
                                        SO.Customer.Name,
                                        InvoiceDate = SO.ReturnDate,
                                        SO.GrandTotal,
                                        NetDiscount = 0m,
                                        TotalAmount = SO.GrandTotal,
                                        RecAmount = SO.PaidAmount,
                                        PaymentDue = SO.GrandTotal - SO.PaidAmount,
                                        P.ProductID,
                                        P.ProductName,
                                        SOD.UnitPrice,
                                        SOD.UTAmount,
                                        PPDAmount = 0m,
                                        PPOffer = 0m,
                                        SOD.Quantity,
                                        std.IMENO,
                                        ColorName = std.Color.Name,
                                        AdjAmount = 0m,
                                        CustomerType = (int)CUS.CustomerType == (int)EnumCustomerType.Dealer ? (int)EnumCustomerType.Dealer : (int)EnumCustomerType.Retail,
                                        CategoryName = CAT.Description,
                                        CategoryID = CAT.CategoryID,
                                        PCategoryID = CAT.PCategoryID,
                                        concernName = sis.Name
                                    }).OrderBy(i => i.ROrderID).ToList();



            var oCreditSalesDetailData = (from SO in creditReturns
                                          join sis in concerns on SO.ConcernId equals sis.ConcernID
                                          join CUS in Customers on SO.CustomerId equals CUS.CustomerID
                                          join cs in creditSaleOrders on SO.CreditSalesId equals cs.CreditSalesID
                                          join SOD in creditSalesDetailsRepository.All on cs.CreditSalesID equals SOD.CreditSalesID
                                          join P in products on SOD.ProductID equals P.ProductID
                                          join CAT in categories on P.CategoryID equals CAT.CategoryID
                                          join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                          where (SO.TransactionDate >= fromDate && SO.TransactionDate <= toDate) && SOD.IsProductReturn == 1
                                          select new
                                          {
                                              ROrderID = SO.Id,
                                              InvoiceNo = SO.MemoNo,
                                              SO.Customer.Name,
                                              InvoiceDate = SO.TransactionDate,
                                              GrandTotal = SO.TotalRemainingDue,
                                              NetDiscount = 0m,
                                              TotalAmount = SO.TotalRemainingDue,
                                              RecAmount = SO.AdjDue,
                                              PaymentDue = SO.TotalRemainingDue - SO.AdjDue,
                                              P.ProductID,
                                              P.ProductName,
                                              SOD.UnitPrice,
                                              SOD.UTAmount,
                                              PPDAmount = 0m,
                                              PPOffer = 0m,
                                              SOD.Quantity,
                                              std.IMENO,
                                              ColorName = std.Color.Name,
                                              AdjAmount = 0m,
                                              CustomerType = (int)CUS.CustomerType,
                                              //CustomerType = (int)CUS.CustomerType == (int)EnumCustomerType.Dealer ? (int)EnumCustomerType.Dealer : (int)EnumCustomerType.Retail,
                                              CategoryName = CAT.Description,
                                              CategoryID = CAT.CategoryID,
                                              PCategoryID = CAT.PCategoryID,
                                              concernName = sis.Name
                                          }).OrderBy(i => i.ROrderID).ToList();


            //var Replacements = (from SO in salesOrderRepository.All
            //                    join SOD in SorderDetailRepository.All on SO.ROrderID equals SOD.RepOrderID
            //                    join P in productRepository.All on SOD.ProductID equals P.ProductID
            //                    join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
            //                    where (SO.ReturnDate>= fromDate && SO.ReturnDate <= toDate )
            //                    select new
            //                    {
            //                        SO.ROrderID,
            //                        InvoiceNo = "REP-" + SO.InvoiceNo,
            //                        SO.Customer.Name,
            //                        SO.InvoiceDate,
            //                        SO.GrandTotal,
            //                        SO.NetDiscount,
            //                        SO.TotalAmount,
            //                        SO.RecAmount,
            //                        SO.PaymentDue,
            //                        P.ProductID,
            //                        P.ProductName,
            //                        SOD.UnitPrice,
            //                        SOD.UTAmount,
            //                        SOD.PPDAmount,
            //                        SOD.PPOffer,
            //                        SOD.Quantity,
            //                        std.IMENO,
            //                        ColorName = std.Color.Name,
            //                        SO.AdjAmount
            //                    }).OrderBy(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(oCreditSalesDetailData);

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal,
                Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, decimal, int, string, int, int, string,
                Tuple<decimal>>>>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                 x.ProductName,
                 x.Name,
                x.UTAmount / x.Quantity,
                //x.PPDAmount,
                x.UTAmount / x.Quantity,
                 x.GrandTotal,
                 new Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                 Tuple<int, decimal, int, string, int, int, string,
                 Tuple<decimal>>>(
                                    x.NetDiscount,
                                    x.TotalAmount,
                                   (decimal)x.RecAmount,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO,
                                   x.ColorName,
                                   new Tuple<int, decimal, int, string, int, int, string,
                                   Tuple<decimal>>
                                   (
                                       x.ROrderID,
                                       x.AdjAmount,
                                       x.CustomerType,
                                       x.CategoryName,
                                       x.CategoryID,
                                       x.PCategoryID,
                                       x.concernName,
                                       new Tuple<decimal>
                                       (
                                          x.TotalAmount)
                                   )
                                   )

                ));
        }





        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
       GetReturnDetailReportByReturnID(this IBaseRepository<ROrder> salesOrderRepository, IBaseRepository<ROrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
       IBaseRepository<StockDetail> stockdetailRepository, int ReturnID, int concernID)
        {
            var oSalesDetailData = (from SO in salesOrderRepository.All
                                    join SOD in SorderDetailRepository.All on SO.ROrderID equals SOD.ROrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                    where (SO.ROrderID == ReturnID)
                                    select new
                                    {
                                        SO.ROrderID,
                                        SO.InvoiceNo,
                                        SO.Customer.Name,
                                        InvoiceDate = SO.ReturnDate,
                                        SO.GrandTotal,
                                        NetDiscount = 0m,
                                        TotalAmount = SO.GrandTotal,
                                        RecAmount = SO.PaidAmount,
                                        PaymentDue = SO.GrandTotal - SO.PaidAmount,
                                        P.ProductID,
                                        P.ProductName,
                                        SOD.UnitPrice,
                                        SOD.UTAmount,
                                        PPDAmount = 0m,
                                        PPOffer = 0m,
                                        SOD.Quantity,
                                        std.IMENO,
                                        ColorName = std.Color.Name,
                                        AdjAmount = 0m
                                    }).OrderBy(x => x.ROrderID).ToList();

            //var Replacements = (from SO in salesOrderRepository.All
            //                    join SOD in SorderDetailRepository.All on SO.ROrderID equals SOD.RepOrderID
            //                    join P in productRepository.All on SOD.ProductID equals P.ProductID
            //                    join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
            //                    where (SO.ReturnDate>= fromDate && SO.ReturnDate <= toDate )
            //                    select new
            //                    {
            //                        SO.ROrderID,
            //                        InvoiceNo = "REP-" + SO.InvoiceNo,
            //                        SO.Customer.Name,
            //                        SO.InvoiceDate,
            //                        SO.GrandTotal,
            //                        SO.NetDiscount,
            //                        SO.TotalAmount,
            //                        SO.RecAmount,
            //                        SO.PaymentDue,
            //                        P.ProductID,
            //                        P.ProductName,
            //                        SOD.UnitPrice,
            //                        SOD.UTAmount,
            //                        SOD.PPDAmount,
            //                        SOD.PPOffer,
            //                        SOD.Quantity,
            //                        std.IMENO,
            //                        ColorName = std.Color.Name,
            //                        SO.AdjAmount
            //                    }).OrderBy(x => x.SOrderID).ToList();

            //oSalesDetailData.AddRange(Replacements);

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                 x.ProductName,
                 x.Name,
                x.UTAmount / x.Quantity,
                //x.PPDAmount,
                x.UTAmount / x.Quantity,
                 x.GrandTotal,
                 new Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>(
                                    x.NetDiscount,
                                    x.TotalAmount,
                                   (decimal)x.RecAmount,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO,
                                   x.ColorName,
                                   new Tuple<int, decimal>(x.ROrderID, x.AdjAmount)
                                   )

                ));
        }



        public static List<ProductWiseSalesReportModel> ProductWiseReturnReport(this IBaseRepository<ROrder> SOrderRepository, IBaseRepository<ROrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Product> ProductRepository, int ConcernID, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            List<ROrder> SOrders = new List<ROrder>();
            if (CustomerID != 0)
                SOrders = SOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.ReturnDate >= fromDate && i.ReturnDate <= toDate && i.ConcernID == ConcernID).ToList();
            else
                SOrders = SOrderRepository.All.Where(i => i.ReturnDate >= fromDate && i.ReturnDate <= toDate && i.ConcernID == ConcernID).ToList();

            var SOrderDetails = SOrderDetailRepo.All;
            var Products = ProductRepository.All;
            var Customers = CustomerRepository.All;
            var Employees = EmployeeRepository.All;


            var result = from SO in SOrders
                         join SOD in SOrderDetails on SO.ROrderID equals SOD.ROrderID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join C in Customers on SO.CustomerID equals C.CustomerID
                         join E in Employees on C.EmployeeID equals E.EmployeeID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.ReturnDate,
                             EmployeeCode = E.Code,
                             EmployeeName = E.Name,
                             CustomerCode = C.Code,
                             CustomerName = C.Name,
                             Address = C.Address,
                             Mobile = C.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - 0m - 0m,
                             TotalAmount = SOD.UTAmount
                         };

            var fresult = from r in result
                          group r by new { r.Date, r.EmployeeCode, r.EmployeeName, r.CustomerCode, r.CustomerName, r.Address, r.Mobile, r.ProductName, r.SalesRate } into g
                          select new ProductWiseSalesReportModel
                          {
                              Date = g.Key.Date,
                              EmployeeCode = g.Key.EmployeeCode,
                              EmployeeName = g.Key.EmployeeName,
                              CustomerCode = g.Key.CustomerCode,
                              CustomerName = g.Key.CustomerName,
                              Address = g.Key.Address,
                              Mobile = g.Key.Mobile,
                              ProductName = g.Key.ProductName,
                              SalesRate = g.Key.SalesRate,
                              Quantity = g.Sum(i => i.Quantity),
                              TotalAmount = g.Sum(i => i.TotalAmount)
                          };

            return fresult.ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
 string, decimal, EnumSalesType>>> GetReturnOrdersAsync(this IBaseRepository<ROrder> SOrderRepository, IBaseRepository<ROrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepo, DateTime fromDate, DateTime toDate)
        {
            var SOrders = SOrderRepository.All;
            //var SOrderDetails = SOrderDetailRepo.All;
            var Customers = CustomerRepo.All;

            var result = await (from so in SOrders
                                join cus in Customers on so.CustomerID equals cus.CustomerID
                                where so.ReturnDate >= fromDate && so.ReturnDate <= toDate
                                select new
                                {
                                    SOrderID = so.ROrderID,
                                    so.InvoiceNo,
                                    SalesDate = so.ReturnDate,
                                    CustomerName = cus.Name,
                                    cus.ContactNo,
                                    cus.TotalDue,
                                    Status = 1
                                }).OrderByDescending(s => s.SOrderID).ToListAsync();


            return result.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.SalesDate,
                    x.CustomerName,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status
                )).ToList();
        }


        public static List<ProductWiseSalesReportModel> ProductWiseReturnDetailsReport(this IBaseRepository<ROrder> SOrderRepository,
         IBaseRepository<ROrderDetail> SOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
         IBaseRepository<Category> CategoryRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<StockDetail> StockDetailRepository,
         IBaseRepository<Customer> CustomerRepository, IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailsRepository,
         IBaseRepository<HireSalesReturnCustomerDueAdjustment> creditSalesReturnRepository,
         int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate,
         int CustomerType, int CustomerID)
        {
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

            var SOrderDetails = SOrderDetailRepo.All;

            var SOrders = SOrderRepository.All.Where(i => i.ReturnDate >= fromDate && i.ReturnDate <= toDate);


            var CreditSalesrDetails = creditSalesDetailsRepository.All;

            var CreditSalesOrders = creditSalesRepository.All;
            var CreditSalesReturnOrders = creditSalesReturnRepository.All.Where(i => i.TransactionDate >= fromDate && i.TransactionDate <= toDate);
            IQueryable<Customer> Customers = null;

            if (CustomerID > 0)
            {
                Customers = CustomerRepository.All.Where(i => i.CustomerID == CustomerID);
            }
            else
            {
                if (CustomerType > 0)
                    Customers = CustomerRepository.All.Where(i => (int)i.CustomerType == CustomerType);
                else
                    Customers = CustomerRepository.All;
            }


            var result = (from SO in SOrders
                          join c in Customers on SO.CustomerID equals c.CustomerID
                          join SOD in SOrderDetails on SO.ROrderID equals SOD.ROrderID
                          join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                          join P in Products on SOD.ProductID equals P.ProductID
                          join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                          join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                          select new ProductWiseSalesReportModel
                          {
                              Date = SO.ReturnDate,
                              InvoiceNo = SO.InvoiceNo,
                              ProductID = P.ProductID,
                              CategoryID = CAT.CategoryID,
                              CompanyID = COM.CompanyID,
                              ProductName = P.ProductName,
                              CategoryName = CAT.Description,
                              CompanyName = COM.Name,
                              Quantity = SOD.Quantity,
                              SalesRate = SOD.UnitPrice - 0m,
                              TotalAmount = SOD.UTAmount,
                              IMEI = STD.IMENO,
                              CustomerType = (int)c.CustomerType
                          }).ToList();

            var result2 = (from SO in CreditSalesReturnOrders
                           join c in Customers on SO.CustomerId equals c.CustomerID
                           join cs in CreditSalesOrders on SO.CreditSalesId equals cs.CreditSalesID
                           join SOD in CreditSalesrDetails on cs.CreditSalesID equals SOD.CreditSalesID
                           join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                           join P in Products on SOD.ProductID equals P.ProductID
                           join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                           join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                           where SOD.IsProductReturn == 1
                           select new ProductWiseSalesReportModel
                           {
                               Date = SO.TransactionDate,
                               InvoiceNo = SO.MemoNo,
                               ProductID = P.ProductID,
                               CategoryID = CAT.CategoryID,
                               CompanyID = COM.CompanyID,
                               ProductName = P.ProductName,
                               CategoryName = CAT.Description,
                               CompanyName = COM.Name,
                               Quantity = SOD.Quantity,
                               SalesRate = SO.TotalRemainingDue - 0m,
                               TotalAmount = SO.TotalRemainingDue,
                               IMEI = STD.IMENO,
                               CustomerType = (int)c.CustomerType
                           }).ToList();

            result.AddRange(result2);

            return result.ToList();
        }




    }
}
