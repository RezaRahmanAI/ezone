using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SalesOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string, int, decimal>>>> GetAllSalesOrderAsync(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID,
            string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All.Where(x => x.IsReplacement == 0 && SalesType.Contains((EnumSalesType)x.Status));
            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Code.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));


            var items = await (from so in sorders
                               join c in customers on so.CustomerID equals c.CustomerID
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = so.SOrderID,
                                   InvoiceNo = so.InvoiceNo,
                                   Date = so.InvoiceDate,
                                   CustomerName = c.Name,
                                   CustomerCode = c.Code,
                                   Mobile = c.ContactNo,
                                   TotalDue = c.TotalDue,
                                   Status = so.Status,
                                   IsReplacement = so.IsReplacement,
                                   TotalAmount = so.TotalAmount,
                                   EditReqStatus = so.EditReqStatus
                               }).ToListAsync();

            List<ProductWiseSalesReportModel> finalData = new List<ProductWiseSalesReportModel>();
            if (IsVATManager)
            {
                items = items.OrderByDescending(i => i.Date).ToList();
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                decimal FalesSales = (items.Sum(i => i.TotalAmount) * oConcern.SalesShowPercent) / 100m;
                decimal FalesSalesCount = 0m;

                foreach (var item in items)
                {
                    FalesSalesCount += item.TotalAmount;
                    if (FalesSalesCount <= FalesSales)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int, decimal>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                    new Tuple<string, int, decimal>
                    (x.CustomerCode,
                    x.EditReqStatus,
                    x.TotalAmount)
                )).OrderByDescending(x => x.Item3).ThenByDescending(i => i.Item2).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsyncByUserID(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, int UserID,
            DateTime fromDate, DateTime toDate, EnumSalesType SalesType,
            string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All
                                        .Where(x => x.Status == (int)SalesType && x.CreatedBy == UserID);

            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Name.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));

            var items = await salesOrderRepository.All.
                GroupJoin(customers, p => p.CustomerID, c => c.CustomerID,
                (p, c) => new { SalesOrder = p, Customers = c }).
                SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { SalesOrder = p.SalesOrder, Customer = c })
                .Select(x => new
                {
                    x.SalesOrder.SOrderID,
                    x.SalesOrder.InvoiceNo,
                    x.SalesOrder.InvoiceDate,
                    x.Customer.Code,
                    x.Customer.Name,
                    x.Customer.ContactNo,
                    x.Customer.TotalDue,
                    x.SalesOrder.Status,
                    x.SalesOrder.CreatedBy,
                    x.SalesOrder.IsReplacement
                }).Where(i => i.IsReplacement == 0).OrderByDescending(i => i.InvoiceDate).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.InvoiceDate,
                    x.Name,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                   new Tuple<string>
                    (x.Code)

                )).ToList();
        }

        public static IEnumerable<SOredersReportModel> GetforSalesReport(
            this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Employee> EmployeeRepository,
            DateTime fromDate, DateTime toDate, int EmployeeID, EnumCustomerType customerType)
        {
            IQueryable<Customer> Customers = null;
            if (EmployeeID > 0)
                Customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                Customers = customerRepository.All;


            var oSalesData = (from sord in salesOrderRepository.All
                              join cus in Customers on sord.CustomerID equals cus.CustomerID
                              join emp in EmployeeRepository.All on cus.EmployeeID equals emp.EmployeeID
                              where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales && cus.CustomerType == customerType)
                              select new SOredersReportModel
                              {
                                  CustomerCode = cus.Code,
                                  CustomerName = cus.Name,
                                  CustomerAddress = cus.Address,
                                  CustomerContactNo = cus.ContactNo,
                                  InvoiceDate = sord.InvoiceDate,
                                  InvoiceNo = sord.InvoiceNo,
                                  Grandtotal = sord.GrandTotal,
                                  FlatDiscount = sord.TDAmount,
                                  TotalAmount = sord.TotalAmount,
                                  RecAmount = (decimal)sord.RecAmount,
                                  PaymentDue = sord.PaymentDue,
                                  CustomerID = sord.CustomerID,
                                  CustomerTotalDue = cus.TotalDue,
                                  EmployeeName = emp.Name,
                                  CustomerType = cus.CustomerType == EnumCustomerType.Dealer
                                  ? cus.CustomerType : EnumCustomerType.Retail

                              }).ToList();
            return oSalesData;
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>>
            GetforSalesDetailReport(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<StockDetail> stockdetailRepository, DateTime fromDate, DateTime toDate)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    from SO in salesOrderRepository.All
                                    from P in productRepository.All
                                    from std in stockdetailRepository.All
                                    where (SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == 1)
                                    select new { SO.InvoiceNo, SO.InvoiceDate, SO.GrandTotal, SO.TDAmount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, P.ProductID, P.ProductName, SOD.UnitPrice, SOD.UTAmount, SOD.PPDAmount, SOD.Quantity, std.IMENO }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                x.ProductName,
                x.UnitPrice,
                x.PPDAmount,
                x.UTAmount,
                x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string>(
                                    x.TDAmount,
                                    x.TotalAmount,
                                   (decimal)x.RecAmount,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO)
                ));
        }

        public static IEnumerable<SOredersReportModel>
         GetforSalesDetailReportByMO(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
         IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository, DateTime fromDate, DateTime toDate, int MOId)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    join SO in salesOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    join CO in customerRepository.All on SO.CustomerID equals CO.CustomerID
                                    join emp in employeeRepository.All on CO.EmployeeID equals emp.EmployeeID
                                    //where (CO.CustomerID == SO.CustomerID && SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == emp.EmployeeID && CO.EmployeeID == MOId && SO.Status ==(int) EnumSalesType.Sales)
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == MOId && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                                    select new SOredersReportModel
                                    {
                                        SOrderID = SO.SOrderID,
                                        EmployeeName = emp.Name,
                                        CustomerCode = CO.Code,
                                        CustomerName = CO.Name,
                                        InvoiceNo = SO.InvoiceNo,
                                        InvoiceDate = SO.InvoiceDate,
                                        TotalAmount = SO.TotalAmount,
                                        NetDiscount = SO.NetDiscount,
                                        AdjAmount = SO.AdjAmount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.UnitPrice,
                                        PPDAmount = SOD.PPDAmount,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        Grandtotal = SO.GrandTotal
                                    }).OrderByDescending(x => x.SOrderID).ToList();

            var Replacements = (from SO in salesOrderRepository.All
                                join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                join P in productRepository.All on SOD.ProductID equals P.ProductID
                                join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                join CO in customerRepository.All on SO.CustomerID equals CO.CustomerID
                                join emp in employeeRepository.All on CO.EmployeeID equals emp.EmployeeID
                                //where (CO.CustomerID == SO.CustomerID && SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == emp.EmployeeID && CO.EmployeeID == MOId && SO.Status ==(int) EnumSalesType.Sales)
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == MOId && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                select new SOredersReportModel
                                {
                                    SOrderID = SO.SOrderID,
                                    EmployeeName = emp.Name,
                                    CustomerCode = CO.Code,
                                    CustomerName = CO.Name,
                                    InvoiceNo = "REP-" + SO.InvoiceNo,
                                    InvoiceDate = SO.InvoiceDate,
                                    TotalAmount = SO.TotalAmount,
                                    NetDiscount = SO.NetDiscount,
                                    AdjAmount = SO.AdjAmount,
                                    RecAmount = (decimal)SO.RecAmount,
                                    PaymentDue = SO.PaymentDue,
                                    ProductName = P.ProductName,
                                    UnitPrice = (decimal)SOD.RepUnitPrice,
                                    PPDAmount = SOD.PPDAmount,
                                    Quantity = SOD.Quantity,
                                    IMENO = std.IMENO,
                                    Grandtotal = SO.GrandTotal
                                }).OrderByDescending(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(Replacements);
            return oSalesDetailData;

        }


        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesReportByConcernID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<SOrderDetail> SOrderDetailRepsitory,
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
                                  //join SOD in SOrderDetailRepsitory.All on SO.SOrderID equals SOD.SOrderID
                              join cus in Customers on SO.CustomerID equals cus.CustomerID
                              where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                              group SO by new
                              {
                                  cus.Code,
                                  cus.Name,
                                  SO.InvoiceNo,
                                  SO.InvoiceDate,
                                  SO.GrandTotal,
                                  SO.NetDiscount,
                                  SO.TotalAmount,
                                  SO.RecAmount,
                                  SO.PaymentDue,
                                  SO.AdjAmount,

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
                                  TotalOffer = g.Select(i => i.SOrderDetails).FirstOrDefault()
                              }).ToList();

            var Replacements = (from SO in salesOrderRepository.All
                                    //join SOD in SOrderDetailRepsitory.All on SO.SOrderID equals SOD.RepOrderID
                                join cus in Customers on SO.CustomerID equals cus.CustomerID
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                group SO by new
                                {
                                    cus.Code,
                                    cus.Name,
                                    InvoiceNo = "REP-" + SO.InvoiceNo,
                                    SO.InvoiceDate,
                                    SO.GrandTotal,
                                    SO.NetDiscount,
                                    SO.TotalAmount,
                                    SO.RecAmount,
                                    SO.PaymentDue,
                                    SO.AdjAmount,

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
                                    TotalOffer = g.Select(i => i.SOrderDetails).FirstOrDefault()
                                }).ToList();

            oSalesData.AddRange(Replacements);

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
                                    x.TotalOffer.Sum(i => i.PPOffer)
                                    )

                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, decimal, int, string, int, int, string,
                    Tuple<decimal>>>>>
            GetSalesDetailReportByConcernID(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Customer> customeRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<SisterConcern> sisterconcerRepository,
            DateTime fromDate, DateTime toDate, int concernID, bool IsAdminReport, int CustomerType)
        {
            IQueryable<Customer> Customers = null;
            IQueryable<SOrder> sOrders = null;
            IQueryable<Product> products = null;
            IQueryable<Category> categories = null;
            IQueryable<SisterConcern> concerns = null;

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
                }
            }
            else
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
                }
                else 
                {
                    if (CustomerType > 0)
                    {
                        if (CustomerType == 1)
                        {
                            Customers = customeRepository.All.Where(i => i.CustomerType != EnumCustomerType.Dealer);
                        }
                        else
                        {
                            Customers = customeRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);
                        }

                    }
                    else
                        Customers = customeRepository.All;
                    //if (CustomerType == 1)
                    //{
                    //    Customers = customeRepository.All.Where(i => i.CustomerType != EnumCustomerType.Dealer);
                    //}
                    //else
                    //{
                    //    //Customers = customeRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);
                    //    Customers = customeRepository.All;
                    //}

                    sOrders = salesOrderRepository.All;
                    products = productRepository.All;
                    categories = categoryRepository.All;
                    concerns = sisterconcerRepository.All;

                }
                //else
                //    Customers = customeRepository.All;

                //sOrders = salesOrderRepository.All;

                //products = productRepository.All;
                //categories = categoryRepository.All;
                //concerns = sisterconcerRepository.All;
            }


            var oSalesDetailData = (from SO in sOrders
                                    join sis in concerns on SO.ConcernID equals sis.ConcernID
                                    join CUS in Customers on SO.CustomerID equals CUS.CustomerID
                                    join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.SOrderID
                                    join P in products on SOD.ProductID equals P.ProductID
                                    join CAT in categories on P.CategoryID equals CAT.CategoryID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                                    select new ProductWiseSalesReportModel
                                    {
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        CustomerName = SO.Customer.Name,
                                        Date = SO.InvoiceDate,//
                                        GrandTotal = SO.GrandTotal,
                                        NetDiscount = SO.NetDiscount,
                                        TotalAmount = SO.TotalAmount,
                                        RecAmount = SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        UnitPrice = ((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount + SO.AdjAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount + SO.AdjAmount))),
                                        UTAmount = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount + SO.AdjAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,
                                        UP = ((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount))),
                                        UTAM = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)),
                                        //UTAM = (SOD.UnitPrice - SOD.PPDAmount) - (SO.TDAmount / SO.TSQty),
                                        PPDAmount = SOD.PPDAmount,
                                        PPOffer = SOD.PPOffer,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        ColorName = std.Color.Name,
                                        AdjAmount = SO.AdjAmount,
                                        CustomerType = (int)CUS.CustomerType == (int)EnumCustomerType.Dealer ? (int)EnumCustomerType.Dealer : (int)EnumCustomerType.Retail,
                                        CategoryName = CAT.Description,
                                        CategoryID = CAT.CategoryID,
                                        PCategoryID = CAT.PCategoryID,
                                        ConcernName = sis.Name
                                    }).OrderBy(x => x.SOrderID).ToList();

            var Replacements = (from SO in sOrders
                                join sis in concerns on SO.ConcernID equals sis.ConcernID
                                join CUS in Customers on SO.CustomerID equals CUS.CustomerID
                                join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                join P in products on SOD.ProductID equals P.ProductID
                                join CAT in categories on P.CategoryID equals CAT.CategoryID
                                join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                select new ProductWiseSalesReportModel
                                {
                                    SOrderID = SO.SOrderID,
                                    InvoiceNo = "REP-" + SO.InvoiceNo,
                                    CustomerName = SO.Customer.Name,
                                    Date = SO.InvoiceDate,//
                                    GrandTotal = SO.GrandTotal,
                                    NetDiscount = SO.NetDiscount,
                                    TotalAmount = SO.TotalAmount,
                                    RecAmount = SO.RecAmount,
                                    PaymentDue = SO.PaymentDue,
                                    ProductID = P.ProductID,
                                    ProductName = P.ProductName,
                                    UnitPrice = SOD.UnitPrice,
                                    UTAmount = SOD.UTAmount,
                                    PPDAmount = SOD.PPDAmount,
                                    PPOffer = SOD.PPOffer,
                                    Quantity = SOD.Quantity,
                                    IMENO = std.IMENO,
                                    ColorName = std.Color.Name,
                                    AdjAmount = SO.AdjAmount,
                                    CustomerType = (int)CUS.CustomerType == (int)EnumCustomerType.Dealer ? (int)EnumCustomerType.Dealer : (int)EnumCustomerType.Retail,
                                    CategoryName = CAT.Description,
                                    CategoryID = CAT.CategoryID,
                                    PCategoryID = CAT.PCategoryID,
                                    ConcernName = sis.Name
                                }).OrderBy(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(Replacements);
            List<ProductWiseSalesReportModel> finalData = new List<ProductWiseSalesReportModel>();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal,
                Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, decimal, int, string, int, int, string,
                Tuple<decimal>>>>
                (
                 x.Date,
                 x.InvoiceNo,
                 x.ProductName,
                 x.CustomerName,
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
                                       x.SOrderID,
                                       x.AdjAmount,
                                       x.CustomerType,
                                       x.CategoryName,
                                       x.CategoryID,
                                       x.PCategoryID,
                                       x.ConcernName,
                                       new Tuple<decimal>
                                       (
                                           x.UTAM * x.Quantity
                                       )

                                   )
                                   )

                ));
        }

        public static IEnumerable<SOredersReportModel> GetSalesDetailHistoryBySOrderID(
         this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository,
         IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockdetailRepository,
         IBaseRepository<Color> ColorRepository, IBaseRepository<TransactionSOrder> transactionSOrderRepository,
         IBaseRepository<TransactionSOrderDetail> transactionSOrderDetailRepository, IBaseRepository<ApplicationUser> userRepository, int SOrderID)
        {
            IQueryable<TransactionSOrder> TransSOrders = null;

            TransSOrders = transactionSOrderRepository.GetAll().Where(i => i.SOrderID == SOrderID);


            var oSalesDetailData = (from SO in TransSOrders
                                    join SOD in transactionSOrderDetailRepository.All on SO.SOrderID equals SOD.SOrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    join col in ColorRepository.All on std.ColorID equals col.ColorID
                                    join us in userRepository.All on SOD.ActionBy equals us.Id
                                    where (SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1 && SO.ActionStatus == SOD.ActionStatus)
                                    select new SOredersReportModel
                                    {
                                        CustomerID = SO.CustomerID,
                                        CustomerName = SO.Customer.Name,
                                        CustomerCode = SO.Customer.Code,
                                        CustomerAddress = SO.Customer.Address,
                                        CustomerContactNo = SO.Customer.ContactNo,
                                        CustCompanyName = SO.Customer.CompanyName,
                                        CustomerTotalDue = SO.Customer.TotalDue + SO.Customer.CreditDue,
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        InvoiceDate = SO.InvoiceDate,
                                        Grandtotal = SO.GrandTotal,
                                        FlatDiscount = SO.TDAmount,
                                        TotalAmount = SO.TotalAmount,
                                        NetDiscount = SO.NetDiscount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        AdjAmount = SO.AdjAmount,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        //UnitPrice = SOD.UnitPrice - SOD.PPDAmount,
                                        UnitPrice = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)),
                                        UTAmount = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)) * SOD.Quantity,
                                        PPDAmount = SOD.PPDAmount,
                                        PPDPercentage = SOD.PPDPercentage,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CustomerType = SO.Customer.CustomerType,
                                        CustomerNID = SO.Customer.NID,
                                        ActionStatus = SO.ActionStatus,
                                        UserName = us.UserName,
                                        ActionDate = (DateTime)SOD.ActionDate
                                    }).OrderByDescending(x => x.ActionStatus).ToList();
            return oSalesDetailData;
        }

        public static IEnumerable<SOredersReportModel> GetSalesDetailReportByCustomerID(
                                                            this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository,
                                                            IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockdetailRepository,
                                                            IBaseRepository<Color> ColorRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                                                            IBaseRepository<CreditSaleDetails> CreditSaleDetailRepository,
                                                            DateTime fromDate, DateTime toDate, int customerID
            )
        {
            var oSalesDetailData = (from SO in salesOrderRepository.All
                                    join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.SOrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    join col in ColorRepository.All on std.ColorID equals col.ColorID
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.CustomerID == customerID && SO.IsReplacement != 1)
                                    select new SOredersReportModel
                                    {
                                        CustomerID = SO.CustomerID,
                                        CustomerName = SO.Customer.Name,
                                        CustomerCode = SO.Customer.Code,
                                        CustomerAddress = SO.Customer.Address,
                                        CustomerContactNo = SO.Customer.ContactNo,
                                        CustCompanyName = SO.Customer.CompanyName,
                                        CustomerTotalDue = SO.Customer.TotalDue + SO.Customer.CreditDue,
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        InvoiceDate = SO.InvoiceDate,
                                        Grandtotal = SO.GrandTotal,
                                        FlatDiscount = SO.TDAmount,
                                        TotalAmount = SO.TotalAmount,
                                        NetDiscount = SO.NetDiscount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        AdjAmount = SO.AdjAmount,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        //UnitPrice = SOD.UnitPrice - SOD.PPDAmount,
                                        UnitPrice = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)),
                                        UTAmount = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)) * SOD.Quantity,
                                        PPDAmount = SOD.PPDAmount,
                                        PPDPercentage = SOD.PPDPercentage,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CustomerType = SO.Customer.CustomerType,
                                        CustomerNID = SO.Customer.NID
                                    }).OrderByDescending(x => x.SOrderID).ToList();

            var ReplacementOrders = (from SO in salesOrderRepository.All
                                     join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                     join P in productRepository.All on SOD.ProductID equals P.ProductID
                                     join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                     join col in ColorRepository.All on std.ColorID equals col.ColorID
                                     where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.CustomerID == customerID && SO.IsReplacement == 1)
                                     select new SOredersReportModel
                                     {
                                         CustomerID = SO.CustomerID,
                                         CustomerName = SO.Customer.Name,
                                         CustomerCode = SO.Customer.Code,
                                         CustomerAddress = SO.Customer.Address,
                                         CustomerContactNo = SO.Customer.ContactNo,
                                         CustCompanyName = SO.Customer.CompanyName,
                                         CustomerTotalDue = SO.Customer.TotalDue + SO.Customer.CreditDue,
                                         SOrderID = SO.SOrderID,
                                         InvoiceNo = SO.InvoiceNo,
                                         InvoiceDate = SO.InvoiceDate,
                                         Grandtotal = SO.GrandTotal,
                                         FlatDiscount = SO.TDAmount,
                                         TotalAmount = SO.TotalAmount,
                                         NetDiscount = SO.NetDiscount,
                                         RecAmount = (decimal)SO.RecAmount,
                                         PaymentDue = SO.PaymentDue,
                                         AdjAmount = SO.AdjAmount,
                                         ProductID = P.ProductID,
                                         ProductName = P.ProductName,
                                         UnitPrice = SOD.UnitPrice - SOD.PPDAmount,
                                         UTAmount = SOD.UTAmount,
                                         PPDAmount = SOD.PPDAmount,
                                         PPDPercentage = SOD.PPDPercentage,
                                         Quantity = SOD.Quantity,
                                         IMENO = std.IMENO,
                                         ColorName = col.Name,
                                         CustomerType = SO.Customer.CustomerType,
                                         CustomerNID = SO.Customer.NID
                                     }).OrderByDescending(x => x.SOrderID).ToList();

            var oCreditSalesDetailData = (from SO in CreditSaleRepository.All
                                          join SOD in CreditSaleDetailRepository.All on SO.CreditSalesID equals SOD.CreditSalesID
                                          join P in productRepository.All on SOD.ProductID equals P.ProductID
                                          join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                          join col in ColorRepository.All on std.ColorID equals col.ColorID
                                          where (SO.SalesDate >= fromDate && SO.SalesDate <= toDate && SO.IsStatus == EnumSalesType.Sales && SO.CustomerID == customerID)
                                          select new SOredersReportModel
                                          {
                                              CustomerID = SO.CustomerID,
                                              CustomerName = SO.Customer.Name,
                                              CustomerCode = SO.Customer.Code,
                                              CustomerAddress = SO.Customer.Address,
                                              CustomerContactNo = SO.Customer.ContactNo,
                                              CustCompanyName = SO.Customer.CompanyName,
                                              CustomerTotalDue = SO.Customer.TotalDue + SO.Customer.CreditDue,
                                              SOrderID = SO.CreditSalesID,
                                              InvoiceNo = SO.InvoiceNo,
                                              InvoiceDate = SO.SalesDate,
                                              Grandtotal = SO.TSalesAmt,
                                              FlatDiscount = 0m,
                                              TotalAmount = SO.TSalesAmt,
                                              NetDiscount = SO.Discount,
                                              RecAmount = (decimal)SO.DownPayment,
                                              PaymentDue = SO.TSalesAmt - SO.DownPayment,
                                              AdjAmount = 0m,
                                              ProductID = P.ProductID,
                                              ProductName = P.ProductName,
                                              UnitPrice = SOD.UnitPrice,
                                              UTAmount = SOD.UTAmount,
                                              PPDAmount = 0m,
                                              PPDPercentage = 0m,
                                              Quantity = SOD.Quantity,
                                              IMENO = std.IMENO,
                                              ColorName = col.Name,
                                              CustomerType = SO.Customer.CustomerType,
                                              CustomerNID = SO.Customer.NID
                                          }).OrderByDescending(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(ReplacementOrders);
            oSalesDetailData.AddRange(oCreditSalesDetailData);

            return oSalesDetailData;
        }

        public static IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
        GetSalesDetailReportByMOID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository,
        DateTime fromDate, DateTime toDate, int concernID, int MOID, int RptType)
        {
            //var oMOWiseSalesDetailData = (dynamic)null;

            if (RptType == 1)
            {
                var oAllMOWiseSalesDetailData = (from CO in customerRepository.All
                                                 from SO in salesOrderRepository.All
                                                 from Emp in employeeRepository.All
                                                 where (CO.CustomerID == SO.CustomerID && SO.Status == 1 && CO.EmployeeID == Emp.EmployeeID && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                                 select new
                                                 {
                                                     Emp.Name,
                                                     SO.InvoiceDate,
                                                     CusName = CO.Name,
                                                     SO.InvoiceNo,
                                                     SO.GrandTotal,
                                                     SO.NetDiscount,
                                                     SO.TotalAmount,
                                                     SO.RecAmount,
                                                     SO.PaymentDue,
                                                     SO.AdjAmount
                                                 }).OrderByDescending(x => x.InvoiceDate).ToList();



                return oAllMOWiseSalesDetailData.Select(x => new Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                    (
                     x.Name,
                     x.InvoiceDate,
                     x.CusName,
                     x.InvoiceNo,
                     x.GrandTotal,
                     x.NetDiscount, new Tuple<decimal, decimal, decimal, decimal>(
                                        x.TotalAmount,
                                       (decimal)x.RecAmount,
                                       x.PaymentDue,
                                       x.AdjAmount
                                       )
                    ));
            }
            else
            {
                var oMOWiseSalesDetailData = (from CO in customerRepository.All
                                              from SO in salesOrderRepository.All
                                              from Emp in employeeRepository.All
                                              where (CO.CustomerID == SO.CustomerID && SO.Status == 1 && CO.EmployeeID == Emp.EmployeeID && Emp.EmployeeID == MOID && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                              select new { Emp.Name, SO.InvoiceDate, CusName = CO.Name, SO.InvoiceNo, SO.GrandTotal, SO.NetDiscount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, SO.AdjAmount }).OrderByDescending(x => x.InvoiceDate).ToList();



                return oMOWiseSalesDetailData.Select(x => new Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                    (
                     x.Name,
                     x.InvoiceDate,
                     x.CusName,
                     x.InvoiceNo,
                     x.GrandTotal,
                     x.NetDiscount, new Tuple<decimal, decimal, decimal, decimal>(
                                        x.TotalAmount,
                                       (decimal)x.RecAmount,
                                       x.PaymentDue,
                                       x.AdjAmount)
                    ));
            }


        }

        public static IEnumerable<Tuple<string, string, string, string, string, decimal>>
        GetMOWiseCustomerDueRpt(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository,
        int concernID, int MOID, int RptType)
        {
            if (RptType == 1)
            {
                var oAllMOWiseCustomerDue = (from CO in customerRepository.All
                                             from Emp in employeeRepository.All
                                             where (CO.EmployeeID == Emp.EmployeeID && CO.TotalDue != 0)
                                             select new { EmpName = Emp.Name, CusCode = CO.Code, CusName = CO.Name, CusContact = CO.ContactNo, Address = CO.Address, TotalDue = CO.TotalDue }).OrderBy(x => x.CusCode).ToList();



                return oAllMOWiseCustomerDue.Select(x => new Tuple<string, string, string, string, string, decimal>
                    (
                     x.EmpName,
                     x.CusCode,
                     x.CusName,
                     x.CusContact,
                     x.Address,
                     x.TotalDue
                    ));
            }
            else
            {
                var oAllMOWiseCustomerDue = (from CO in customerRepository.All
                                             from Emp in employeeRepository.All
                                             where (CO.EmployeeID == Emp.EmployeeID && Emp.EmployeeID == MOID && CO.TotalDue != 0)
                                             select new { EmpName = Emp.Name, CusCode = CO.Code, CusName = CO.Name, CusContact = CO.ContactNo, Address = CO.Address, TotalDue = CO.TotalDue }).OrderBy(x => x.CusCode).ToList();



                return oAllMOWiseCustomerDue.Select(x => new Tuple<string, string, string, string, string, decimal>
                    (
                     x.EmpName,
                     x.CusCode,
                     x.CusName,
                     x.CusContact,
                     x.Address,
                     x.TotalDue
                    ));
            }


        }


        //public static IEnumerable<Tuple<int, int, int, int, string, string, string,
        //   Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>>
        //   GetSalesOrderDetailByOrderId(this IBaseRepository<SOrderDetail> salesOrderDetailRepository, int orderId, IBaseRepository<Product> productRepository,
        //   IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        //{
        //    IQueryable<Product> products = productRepository.All;
        //    IQueryable<Color> colors = colorRepository.All;
        //    IQueryable<StockDetail> details = stockDetailRepository.All;

        //    var items = salesOrderDetailRepository.All.
        //        GroupJoin(products, s => s.ProductID, p => p.ProductID,
        //        (s, p) => new { SalesOrder = s, Products = p }).
        //        SelectMany(x => x.Products.DefaultIfEmpty(), (s, p) => new { SalesOrder = s.SalesOrder, Products = p }).
        //        GroupJoin(details, s => s.SalesOrder.SDetailID, d => d.SDetailID,
        //        (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
        //        SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
        //        GroupJoin(colors, s => s.Details.ColorID, c => c.ColorID,
        //        (d, c) => new { SalesOrder = d.SalesOrder, Details = d.Details, Products = d.Products, Colors = c }).
        //        SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SalesOrder = d.SalesOrder, Products = d.Products, Details = d.Details, Color = c }).
        //        Where(x => x.SalesOrder.SOrderID == orderId).
        //        Select(x => new
        //        {
        //            x.SalesOrder.SOrderDetailID,
        //            x.SalesOrder.SOrderID,
        //            x.SalesOrder.ProductID,
        //            StockDetailID = x.SalesOrder.SDetailID,
        //            x.Products.ProductName,
        //            x.Products.Code,
        //            x.Details.IMENO,
        //            x.SalesOrder.Quantity,
        //            x.SalesOrder.UnitPrice,
        //            x.SalesOrder.MPRate,
        //            x.SalesOrder.UTAmount,
        //            x.SalesOrder.PPDPercentage,
        //            x.SalesOrder.PPDAmount,
        //            ColorId = x.Color.ColorID,
        //            ColorName = x.Color.Name,
        //            PPOffer = x.SalesOrder.PPOffer,
        //            x.Details.GodownID
        //        }).ToList();

        //    return items.Select(x => new Tuple<int, int, int, int, string, string, string,
        //        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>
        //        (
        //            x.SOrderDetailID,
        //            x.SOrderID,
        //            x.ProductID,
        //            x.StockDetailID,
        //            x.ProductName,
        //            x.Code,
        //            x.IMENO,
        //            new Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>(
        //            x.Quantity,
        //            x.UnitPrice,
        //            x.MPRate,
        //            x.UTAmount,
        //            x.PPDPercentage,
        //            x.PPDAmount,
        //            x.ColorId,
        //            new Tuple<string, int>(
        //                x.ColorName,
        //                x.GodownID
        //                ))
        //            ));
        //}



        public static IEnumerable<Tuple<int, int, int, int, string, string, string,
            Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>>
            GetSalesOrderDetailByOrderId(this IBaseRepository<SOrderDetail> salesOrderDetailRepository, int orderId, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;

            var items = salesOrderDetailRepository.All.
                GroupJoin(products, s => s.ProductID, p => p.ProductID,
                (s, p) => new { SalesDetails = s, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (s, p) => new { SalesDetails = s.SalesDetails, Products = p }).
                GroupJoin(details, s => s.SalesDetails.SDetailID, d => d.SDetailID,
                (s, d) => new { SalesDetails = s.SalesDetails, Products = s.Products, Details = d }).
                SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SalesDetails = s.SalesDetails, Products = s.Products, Details = d }).
                GroupJoin(colors, s => s.Details.ColorID, c => c.ColorID,
                (d, c) => new { SalesDetails = d.SalesDetails, Details = d.Details, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SalesDetails = d.SalesDetails, Products = d.Products, Details = d.Details, Color = c }).
                Where(x => x.SalesDetails.SOrderID == orderId).
                Select(x => new
                {
                    x.SalesDetails.SOrderDetailID,
                    x.SalesDetails.SOrderID,
                    x.SalesDetails.ProductID,
                    StockDetailID = x.SalesDetails.SDetailID,
                    x.Products.ProductName,
                    x.Products.Code,
                    x.Details.IMENO,
                    x.SalesDetails.Quantity,
                    x.SalesDetails.UnitPrice,
                    x.SalesDetails.MPRate,
                    x.SalesDetails.UTAmount,
                    x.SalesDetails.PPDPercentage,
                    x.SalesDetails.PPDAmount,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    PPOffer = x.SalesDetails.PPOffer,
                    GodownID = x.Details.GodownID,
                    ProductType = x.Products.ProductType,
                    x.SalesDetails.PRate,
                    x.SalesDetails.Warranty
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, int, string, string, string,
                Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>
                (
                    x.SOrderDetailID,
                    x.SOrderID,
                    x.ProductID,
                    x.StockDetailID,
                    x.ProductName,
                    x.Code,
                    x.IMENO,
                    new Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>(
                    x.Quantity,
                    x.UnitPrice,
                    x.MPRate,
                    x.UTAmount,
                    x.PPDPercentage,
                    x.PPDAmount,
                    x.ColorId,
                    new Tuple<string, decimal, int, int, decimal, string>(
                        x.ColorName,
                        x.PPOffer,
                        x.GodownID,
                        x.ProductType,
                        x.PRate,
                        x.Warranty
                        ))
                    ));
        }

        public static IEnumerable<Tuple<int, int, int, int, string, string, string,
        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>>
        GetSalesOrderDetailByOrderIdForInvoice(this IBaseRepository<SOrderDetail> salesOrderDetailRepository, int orderId, IBaseRepository<Product> productRepository,
        IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;

            var items = salesOrderDetailRepository.All.
                GroupJoin(products, s => s.ProductID, p => p.ProductID,
                (s, p) => new { SalesOrder = s, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (s, p) => new { SalesOrder = s.SalesOrder, Products = p }).
                GroupJoin(details, s => s.SalesOrder.SDetailID, d => d.SDetailID,
                (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
                SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
                GroupJoin(colors, s => s.Details.ColorID, c => c.ColorID,
                (d, c) => new { SalesOrder = d.SalesOrder, Details = d.Details, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SalesOrder = d.SalesOrder, Products = d.Products, Details = d.Details, Color = c }).
                Where(x => x.SalesOrder.SOrderID == orderId).
                Select(x => new
                {
                    x.SalesOrder.SOrderDetailID,
                    x.SalesOrder.SOrderID,
                    x.SalesOrder.ProductID,
                    StockDetailID = x.SalesOrder.SDetailID,
                    x.Products.ProductName,
                    x.Products.Code,
                    x.Details.IMENO,
                    x.SalesOrder.Quantity,
                    x.SalesOrder.UnitPrice,
                    x.SalesOrder.MPRate,
                    x.SalesOrder.UTAmount,
                    x.SalesOrder.PPDPercentage,
                    x.SalesOrder.PPDAmount,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    PPOffer = x.SalesOrder.PPOffer,
                    x.Products.CompressorWarrentyMonth,
                    x.Products.MotorWarrentyMonth,
                    x.Products.PanelWarrentyMonth,
                    x.Products.SparePartsWarrentyMonth
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, int, string, string, string,
                Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>
                (
                    x.SOrderDetailID,
                    x.SOrderID,
                    x.ProductID,
                    x.StockDetailID,
                    x.ProductName,
                    x.Code,
                    x.IMENO,
                    new Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>(
                    x.Quantity,
                    x.UnitPrice,
                    x.MPRate,
                    x.UTAmount,
                    x.PPDPercentage,
                    x.PPDAmount,
                    x.ColorId,
                    new Tuple<string, decimal, string, string, string, string>(
                        x.ColorName,
                        x.PPOffer,
                        x.CompressorWarrentyMonth,
                        x.MotorWarrentyMonth,
                        x.PanelWarrentyMonth,
                        x.SparePartsWarrentyMonth
                        ))
                    ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>>
            GetSalesByProductID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            DateTime fromDate, DateTime toDate, int productID)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    from SO in salesOrderRepository.All
                                    from P in productRepository.All
                                    where (SOD.SOrderID == SO.SOrderID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == 1 && SOD.ProductID == productID)
                                    select new { SO.InvoiceNo, SO.InvoiceDate, SO.GrandTotal, SO.TDAmount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, P.ProductID, P.ProductName, SOD.UnitPrice, SOD.UTAmount, SOD.PPDAmount, SOD.Quantity }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                x.ProductName,
                x.UnitPrice,
                x.Quantity
                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetSalesByProductID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<CreditSale> CreditSalesRepository, IBaseRepository<CreditSaleDetails> CreditSalesDetailRepository, DateTime fromDate, DateTime toDate, int ConcernID, int productid)
        {
            //var oSales = ((from POD in SOrderDetailRepository.All
            //               from PO in SOrderRepository.All
            //               from P in productRepository.All
            //               where (POD.SOrderID == PO.SOrderID && P.ProductID == POD.ProductID && PO.InvoiceDate >= fromDate && PO.InvoiceDate <= toDate && P.ProductID == productid && PO.Status == 1)
            //               select new {POD.StockDetailID, PO.InvoiceNo, SalesDate = PO.InvoiceDate, P.ProductName, POD.Quantity, POD.UnitPrice })
            //                 .Union(
            //                              from SOD in CreditSalesDetailRepository.All
            //                              from SO in CreditSalesRepository.All
            //                              from P in productRepository.All
            //                              where SOD.CreditSalesID == SO.CreditSalesID && P.ProductID == SOD.ProductID
            //                              && P.ProductID == productid && SO.SalesDate >= fromDate && SO.SalesDate <= toDate
            //                              select new
            //                              {
            //                                  SOD.StockDetailID,
            //                                  InvoiceNo = SO.InvoiceNo + " (Credit)",
            //                                  SO.SalesDate,
            //                                  P.ProductName,
            //                                  SOD.Quantity,
            //                                  SOD.UnitPrice
            //                              })).OrderBy(x => x.SalesDate).ToList();

            //return oSales.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
            //    (
            //     x.SalesDate,
            //     x.InvoiceNo,
            //    x.ProductName,
            //    x.Quantity,
            //    x.UnitPrice
            //    ));


            var oSales = ((from POD in SOrderDetailRepository.All
                           from PO in SOrderRepository.All
                           from P in productRepository.All
                           where (POD.SOrderID == PO.SOrderID && P.ProductID == POD.ProductID && PO.InvoiceDate >= fromDate && PO.InvoiceDate <= toDate && P.ProductID == productid && PO.Status == 1)
                           group POD by new { PO.InvoiceNo, PO.InvoiceDate, P.ProductName, POD.UnitPrice } into g
                           select new { g.Key.InvoiceNo, SalesDate = g.Key.InvoiceDate, g.Key.ProductName, Quantity = g.Sum(x => x.Quantity), g.Key.UnitPrice })
                                         .Union(
                                                      from SOD in CreditSalesDetailRepository.All
                                                      from SO in CreditSalesRepository.All
                                                      from P in productRepository.All
                                                      where SOD.CreditSalesID == SO.CreditSalesID && P.ProductID == SOD.ProductID
                                                      && P.ProductID == productid && SO.SalesDate >= fromDate && SO.SalesDate <= toDate
                                                      group SOD by new { SO.InvoiceNo, SO.SalesDate, P.ProductName, SOD.UnitPrice } into g
                                                      select new
                                                      {
                                                          InvoiceNo = g.Key.InvoiceNo + " (Credit)",
                                                          g.Key.SalesDate,
                                                          g.Key.ProductName,
                                                          Quantity = g.Sum(x => x.Quantity),
                                                          g.Key.UnitPrice
                                                      })).OrderBy(x => x.SalesDate).ToList();

            return oSales.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.SalesDate,
                 x.InvoiceNo,
                x.ProductName,
                x.Quantity,
                x.UnitPrice
                ));
        }

        /// <summary>
        /// Author:Aminul
        /// Date: 06/03/2018
        /// </summary>
        /// <param name="SOrderRepository"></param>
        /// <param name="SOrderDetailRepo"></param>
        /// <param name="CustomerRepo"></param>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType>>> GetReplacementOrdersAsync(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepo, int EmployeeID)
        {
            var SOrders = SOrderRepository.All;
            //var SOrderDetails = SOrderDetailRepo.All;
            IQueryable<Customer> CustomerList = null;
            if (EmployeeID != 0)
                CustomerList = CustomerRepo.All.Where(i => i.EmployeeID == EmployeeID);
            else
                CustomerList = CustomerRepo.All;
            var result = await (from so in SOrders.Where(i => i.IsReplacement == 1)
                                join cus in CustomerList on so.CustomerID equals cus.CustomerID
                                select new
                                {
                                    so.SOrderID,
                                    so.InvoiceNo,
                                    SalesDate = so.InvoiceDate,
                                    CustomerName = cus.Name,
                                    cus.ContactNo,
                                    cus.TotalDue,
                                    so.Status
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

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
    string, decimal, EnumSalesType>>> GetReturnOrdersAsync(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepo)
        {
            var SOrders = SOrderRepository.All;
            //var SOrderDetails = SOrderDetailRepo.All;
            var Customers = CustomerRepo.All;

            var result = await (from so in SOrders
                                where so.Status == (int)EnumSalesType.ProductReturn
                                join cus in Customers on so.CustomerID equals cus.CustomerID
                                select new
                                {
                                    so.SOrderID,
                                    so.InvoiceNo,
                                    SalesDate = so.InvoiceDate,
                                    CustomerName = cus.Name,
                                    cus.ContactNo,
                                    cus.TotalDue,
                                    so.Status
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

        public static List<ReplaceOrderDetail> GetReplaceOrderInvoiceReportByID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<StockDetail> StockDetailRepo, IBaseRepository<Product> ProductRepo, int OrderID)
        {
            List<ReplaceOrderDetail> list = new List<ReplaceOrderDetail>();
            var dbsorder = SOrderRepository.FindBy(i => i.SOrderID == OrderID);
            var sorderDetails = SOrderDetailRepo.All;
            var stockdetails = StockDetailRepo.All;
            var products = ProductRepo.All;

            var dresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.RepOrderID
                           join std in stockdetails on sod.SDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               DamageIMEINO = std.IMENO,
                               DamageProductName = p.ProductName,
                               DamageUnitPrice = sod.UnitPrice.ToString(),
                               Quantity = 1,
                               Remarks = sod.Remarks
                           }).ToList();

            var rresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.RepOrderID
                           join std in stockdetails on sod.RStockDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               ReplaceIMEINO = std.IMENO,
                               ProductName = p.ProductName,
                               UnitPrice = sod.RepUnitPrice,
                               Quantity = 1,
                               Remarks = sod.Remarks
                           }).ToList();

            var result = (from d in dresult
                          join r in rresult on d.SOrderDetailID equals r.SOrderDetailID
                          select new ReplaceOrderDetail
                          {
                              DamageProductName = d.DamageProductName,
                              DamageIMEINO = d.DamageIMEINO,
                              DamageUnitPrice = d.DamageUnitPrice,
                              Quantity = d.Quantity,
                              ProductName = r.ProductName,
                              ReplaceIMEINO = r.ReplaceIMEINO,
                              UnitPrice = (decimal)r.UnitPrice,
                              Remarks = r.Remarks
                          }).ToList();
            return result;

        }

        public static List<ReplaceOrderDetail> GetReturnOrderInvoiceReportByID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<StockDetail> StockDetailRepo, IBaseRepository<Product> ProductRepo, int OrderID)
        {
            List<ReplaceOrderDetail> list = new List<ReplaceOrderDetail>();
            var dbsorder = SOrderRepository.FindBy(i => i.SOrderID == OrderID);
            var sorderDetails = SOrderDetailRepo.All;
            var stockdetails = StockDetailRepo.All;
            var products = ProductRepo.All;

            var dresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.SOrderID
                           join std in stockdetails on sod.SDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new ReplaceOrderDetail
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               DamageIMEINO = std.IMENO,
                               DamageProductName = p.ProductName,
                               UnitPrice = sod.UnitPrice,
                               Quantity = 1,
                               MPRate = sod.MPRate
                           }).ToList();

            //var result = (from d in dresult
            //              select new ReplaceOrderDetail
            //              {
            //                  DamageProductName = d.DamageProductName,
            //                  DamageIMEINO = d.DamageIMEINO,
            //                  DamageUnitPrice = d.DamageUnitPrice,
            //                  Quantity = d.Quantity,
            //                  //ProductName = r.ProductName,
            //                  //ReplaceIMEINO = r.ReplaceIMEINO,
            //                  //UnitPrice = (decimal)r.UnitPrice
            //              }).ToList();
            return dresult;

        }


        public static List<ProductWiseSalesReportModel> ProductWiseSalesReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Product> ProductRepository, int ConcernID, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            List<SOrder> SOrders = new List<SOrder>();
            if (CustomerID != 0)
                SOrders = SOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.ConcernID == ConcernID).ToList();
            else
                SOrders = SOrderRepository.All.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.ConcernID == ConcernID).ToList();

            var SOrderDetails = SOrderDetailRepo.All;
            var Products = ProductRepository.All;
            var Customers = CustomerRepository.All;
            var Employees = EmployeeRepository.All;


            var result = from SO in SOrders.Where(i => i.Status == (int)EnumSalesType.Sales)
                         join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join C in Customers on SO.CustomerID equals C.CustomerID
                         join E in Employees on C.EmployeeID equals E.EmployeeID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.InvoiceDate,
                             EmployeeCode = E.Code,
                             EmployeeName = E.Name,
                             CustomerCode = C.Code,
                             CustomerName = C.Name,
                             Address = C.Address,
                             Mobile = C.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPDAmount - SOD.PPOffer,
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

        public static List<ProductWiseSalesReportModel> ProductWiseSalesDetailsReport(this IBaseRepository<SOrder> SOrderRepository,
               IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
               IBaseRepository<Category> CategoryRepository, IBaseRepository<Product> ProductRepository,
               IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Customer> CustomerRepository,
               int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType,
               int CustomerID)
        {
            IQueryable<Customer> Customers = null;
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

            if (CustomerID > 0)
            {
                Customers = CustomerRepository.All.Where(i => i.CustomerID == CustomerID);
            }
            else
            {
                if (CustomerType > 0)
                    Customers = CustomerRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);
                else
                    Customers = CustomerRepository.All;
            }


            var SOrderDetails = SOrderDetailRepo.All;
            var SOrders = SOrderRepository.All.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.Status == (int)EnumSalesType.Sales && i.IsReplacement == 0);

            var result = from SO in SOrders
                         join c in Customers on SO.CustomerID equals c.CustomerID
                         join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                         join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                         join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.InvoiceDate,
                             InvoiceNo = SO.InvoiceNo,
                             ProductID = P.ProductID,
                             CategoryID = CAT.CategoryID,
                             CompanyID = COM.CompanyID,
                             ProductName = P.ProductName,
                             CategoryName = CAT.Description,
                             CompanyName = COM.Name,
                             Quantity = SOD.Quantity,
                             //SalesRate = (SOD.UnitPrice - SOD.PPDAmount) - (SO.TDAmount / SO.TSQty),
                             SalesRate = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)),
                             //TotalAmount = ((SOD.UnitPrice - SOD.PPDAmount) - (SO.TDAmount / SO.TSQty)) * SOD.Quantity   /*SOD.UTAmount*/,
                             TotalAmount = ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) - ((((SOD.Quantity > 0) ? (SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) : 0) * (SO.TDAmount)) / (SO.GrandTotal - SO.NetDiscount + (SO.TDAmount)))) - ((SO.TSQty > 0) ? (SO.AdjAmount / SO.TSQty) : 0)) * SOD.Quantity,
                             IMEI = STD.IMENO,
                             CustomerType = (int)c.CustomerType
                         };

            return result.ToList();
        }

        public static decimal GetAllCollectionAmountByDateRange(this IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
            IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<BankTransaction> BankTransactionRepository, DateTime fromDate, DateTime toDate)
        {
            decimal TotalCollection = 0m;

            var CashSales = SOrderRepository.All.Where(so => so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate && so.Status == (int)EnumSalesType.Sales).ToList();
            if (CashSales.Count() > 0)
                TotalCollection += (decimal)CashSales.Sum(i => i.RecAmount);

            var Downpayment = CreditSaleRepository.All.Where(so => so.SalesDate >= fromDate && so.SalesDate <= toDate && so.IsStatus == EnumSalesType.Sales).ToList();
            if (Downpayment.Count() > 0)
                TotalCollection += (decimal)Downpayment.Sum(i => i.DownPayment);

            var InstallmentCollections = from so in CreditSaleRepository.All
                                         join css in CreditSalesScheduleRepository.All on so.CreditSalesID equals css.CreditSalesID
                                         where ((css.PaymentDate >= fromDate && css.PaymentDate <= toDate) && css.PaymentStatus.Equals("Paid") && so.IsStatus == EnumSalesType.Sales)
                                         select css;

            if (InstallmentCollections.ToList().Count() > 0)
                TotalCollection += (decimal)InstallmentCollections.Sum(i => i.InstallmentAmt);

            var CashCollections = CashCollectionRepository.All.Where(so => so.EntryDate >= fromDate && so.EntryDate <= toDate && so.TransactionType == EnumTranType.FromCustomer).ToList();
            if (CashCollections.Count() > 0)
                TotalCollection += (decimal)CashCollections.Sum(i => i.Amount);

            var BankCollections = BankTransactionRepository.All.Where(i => i.TranDate >= fromDate && i.TranDate <= toDate && i.CustomerID != 0).ToList();
            if (BankCollections.Count() > 0)
                TotalCollection += (decimal)BankCollections.Sum(i => i.Amount);

            return TotalCollection;
        }

        public static decimal GetVoltageStabilizerCommission(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
             IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
             IBaseRepository<Product> ProductRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
             DateTime fromDate, DateTime toDate)
        {
            decimal TotalVSComm = 0m;
            var TargetCategory = ExtraCommissionSetupRepository.All.FirstOrDefault(i => i.Status == EnumCommissionType.VoltageStabilizerComm);
            var Sales = (from so in SOrderRepository.All
                         join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                         join p in ProductRepository.All on sod.ProductID equals p.ProductID
                         where (so.Status == (int)EnumSalesType.Sales && so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && (p.CategoryID == TargetCategory.CategoryID1 || p.CategoryID == TargetCategory.CategoryID2)
                         select new
                         {
                             so.InvoiceDate,
                             so.CustomerID,
                             sod.ProductID,
                             p.CategoryID
                         }).ToList();

            var CreditSales = (from so in CreditSaleRepository.All
                               join sod in CreditSaleDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                               join p in ProductRepository.All on sod.ProductID equals p.ProductID
                               where (so.IsStatus == EnumSalesType.Sales && so.SalesDate >= fromDate && so.SalesDate <= toDate) && (p.CategoryID == TargetCategory.CategoryID1 || p.CategoryID == TargetCategory.CategoryID2)
                               select new
                               {
                                   InvoiceDate = so.SalesDate,
                                   so.CustomerID,
                                   sod.ProductID,
                                   p.CategoryID
                               }).ToList();

            Sales.AddRange(CreditSales);

            var SalesVoltageStabilizerComm = (from so in Sales
                                              group so by new { so.InvoiceDate, so.CustomerID } into g
                                              select new
                                              {
                                                  InvoiceDate = g.Key.InvoiceDate,
                                                  CustomerID = g.Key.CustomerID,
                                                  Categories = g.Select(i => i.CategoryID).ToList()
                                              }).ToList();

            int Flag1 = 0, Flag2 = 0, Counter = 0;
            foreach (var item in SalesVoltageStabilizerComm)
            {
                if (item.Categories.Any(i => i == TargetCategory.CategoryID1))
                    Flag1++;
                if (item.Categories.Any(i => i == TargetCategory.CategoryID2))
                    Flag2++;
                if (Flag1 > 0 && Flag2 > 0)
                {
                    Counter++;
                }
                Flag1 = 0;
                Flag2 = 0;
            }

            TotalVSComm = 250m * Counter;

            return TotalVSComm;
        }

        /// <summary>
        /// Date: 25-02-2019
        /// Author: aminul
        /// </summary>
        public static decimal GetExtraCommission(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                         IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                         IBaseRepository<Product> ProductRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
                         DateTime fromDate, DateTime toDate, int ConcernID)
        {
            decimal TotalExtraComm = 0m;

            if (ConcernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            {
                var TargetCategory = ExtraCommissionSetupRepository.All.FirstOrDefault(i => i.Status == EnumCommissionType.ExtraComm);
                var Sales = (from so in SOrderRepository.All
                             join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                             join p in ProductRepository.All on sod.ProductID equals p.ProductID
                             where (so.Status == (int)EnumSalesType.Sales && so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate)
                             && ((p.CategoryID == TargetCategory.CategoryID1 && p.CompanyID == TargetCategory.CompanyID) || (p.CategoryID == TargetCategory.CategoryID2 && p.CompanyID == TargetCategory.CompanyID))
                             //&& sod.PPDAmount <= 250
                             select new
                             {
                                 so.InvoiceDate,
                                 so.CustomerID,
                                 sod.ProductID,
                                 p.CategoryID,
                                 sod.PPDAmount,
                                 sod.PPDPercentage,
                                 sod.UnitPrice,
                                 sod.Quantity,
                                 sod.UTAmount
                             }).ToList();
                decimal TotalSalesAmt = Sales.Sum(i => i.Quantity) * 1000m;
                decimal AcceptedAmount = (TotalSalesAmt * 25m) / 100m;
                decimal TotalGivenDiscount = Sales.Sum(i => (i.PPDAmount * i.Quantity));
                if (TotalGivenDiscount <= AcceptedAmount)
                    TotalExtraComm = 250m * Sales.Count();
            }
            else if (ConcernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || ConcernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
            {
                var Creditsales6M = CreditSaleRepository.All.Where(i => i.InstallmentPeriod.ToLower().Equals("6 months")
                    && (i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales)).ToList();
                if (Creditsales6M.Count > 0)
                    TotalExtraComm = Creditsales6M.Sum(i => i.NetAmount) * .0025m;

                var Creditsales12M = CreditSaleRepository.All.Where(i => i.InstallmentPeriod.ToLower().Equals("12 months")
                    && (i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales)).ToList();
                if (Creditsales12M.Count() > 0)
                    TotalExtraComm += Creditsales12M.Sum(i => i.NetAmount) * .0050m;
            }


            return TotalExtraComm;
        }
        public static bool IsIMEIAlreadyReplaced(this IBaseRepository<SOrder> SOrderRepository,
         IBaseRepository<SOrderDetail> SOrderDetailRepo, int StockDetailID)
        {
            var RepORders = from so in SOrderRepository.All
                            join sod in SOrderDetailRepo.All on so.SOrderID equals sod.RepOrderID
                            where sod.RStockDetailID == StockDetailID && so.Status == (int)EnumSalesType.Sales && so.IsReplacement == 1
                            select sod;

            if (RepORders.Count() > 0)
                return true;
            else
                return false;
        }

        public static List<SOredersReportModel> GetAdminSalesReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            int ConcernID, DateTime fromDate, DateTime toDate,
                                    EnumCustomerType customerType, int customerID)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID > 0)
            {
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                if (customerID != 0)
                    Customers = Customers.Where(i => i.CustomerID == customerID);
                else if (customerType != 0)
                    Customers = Customers.Where(i => i.CustomerType == customerType);

            }
            else
            {
                if (customerType != 0)
                    Customers = Customers.Where(i => i.CustomerType == customerType);
                else
                    Customers = CustomerRepository.GetAll();
            }
            var Sales = (from so in SOrderRepository.GetAll()
                         join c in Customers on so.CustomerID equals c.CustomerID
                         join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                         where so.Status == (int)EnumSalesType.Sales && (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && so.IsReplacement != 1
                         select new SOredersReportModel
                         {
                             ConcernID = so.ConcernID,
                             ConcernName = s.Name,
                             CustomerCode = c.Code,
                             CustomerName = c.Name,
                             CustomerAddress = c.Address,
                             CustomerContactNo = c.ContactNo,
                             InvoiceDate = so.InvoiceDate,
                             InvoiceNo = so.InvoiceNo,
                             Grandtotal = so.GrandTotal,
                             NetDiscount = so.NetDiscount,
                             TotalOffer = 0,
                             AdjAmount = so.AdjAmount,
                             TotalAmount = so.TotalAmount,
                             RecAmount = (decimal)so.RecAmount,
                             PaymentDue = so.PaymentDue,
                             CustomerTotalDue = c.TotalDue,
                             CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType
                         }).ToList();

            var Replaces = (from so in SOrderRepository.GetAll()
                            join c in Customers on so.CustomerID equals c.CustomerID
                            join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                            where so.Status == (int)EnumSalesType.Sales && (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && so.IsReplacement == 1
                            select new SOredersReportModel
                            {
                                ConcernID = so.ConcernID,
                                ConcernName = s.Name,
                                CustomerCode = c.Code,
                                CustomerName = c.Name,
                                CustomerAddress = c.Address,
                                CustomerContactNo = c.ContactNo,
                                InvoiceDate = so.InvoiceDate,
                                InvoiceNo = "REP-" + so.InvoiceNo,
                                Grandtotal = so.GrandTotal,
                                NetDiscount = so.NetDiscount,
                                TotalOffer = 0,
                                AdjAmount = so.AdjAmount,
                                TotalAmount = so.TotalAmount,
                                RecAmount = (decimal)so.RecAmount,
                                PaymentDue = so.PaymentDue,
                                CustomerTotalDue = c.TotalDue,
                                CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType
                            });
            Sales.AddRange(Replaces);
            return Sales.OrderBy(i => i.ConcernID).ThenByDescending(i => i.InvoiceDate).ToList();
        }


        public static List<LedgerAccountReportModel> CustomerLedger(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<Customer> CustomerRepository, IBaseRepository<ApplicationUser> UserRepository, IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepo, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepo, IBaseRepository<Product> ProductRepository, IBaseRepository<Bank> BankRepository, IBaseRepository<ROrder> RorderRepository, IBaseRepository<ROrderDetail> rOrderDetail, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturn, int CustomerID, DateTime fromDate, DateTime toDate)
        {

            List<LedgerAccountReportModel> ledgers = new List<LedgerAccountReportModel>();
            List<LedgerAccountReportModel> FinalLedgers = new List<LedgerAccountReportModel>();

            var Customer = CustomerRepository.GetAll().FirstOrDefault(i => i.CustomerID == CustomerID);

            #region Cash Sales
            var CashSales = from so in SOrderRepository.All
                            join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                            join p in ProductRepository.All on sod.ProductID equals p.ProductID
                            join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where so.Status == (int)EnumSalesType.Sales && so.CustomerID == CustomerID
                            select new
                            {
                                so.TotalAmount,
                                so.InvoiceDate,
                                so.InvoiceNo,
                                so.RecAmount,
                                so.PaymentDue,
                                CreditAdj = so.AdjAmount + so.NetDiscount,
                                Credit = (decimal)so.RecAmount,
                                CashCollectionAmt = (decimal)so.RecAmount,
                                Debit = (decimal)(so.TotalAmount),
                                GrandTotal = so.GrandTotal,
                                sod.UnitPrice,
                                sod.UTAmount,
                                sod.Quantity,
                                ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + sod.SRate.ToString() + " " + sod.UTAmount.ToString(),
                                EnteredBy = u == null ? string.Empty : u.UserName,
                                Remarks = so.Remarks,
                                TotalAmtWD = so.TotalAmount,
                                TotalAdjDis = so.AdjAmount + so.NetDiscount,
                            };



            var VmCashSales = (from cs in CashSales
                               group cs by new { cs.Debit, cs.Credit, cs.CreditAdj, cs.GrandTotal, cs.CashCollectionAmt, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy, cs.PaymentDue } into g
                               select new LedgerAccountReportModel
                               {
                                   VoucherType = "Sales",
                                   InvoiceNo = g.Key.InvoiceNo,
                                   Date = g.Key.InvoiceDate,
                                   EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                   ProductList = g.Select(i => i.ProductName).ToList(),
                                   Debit = g.Key.Debit,
                                   Credit = g.Key.Credit,
                                   CreditAdj = g.Key.CreditAdj,
                                   GrandTotal = g.Key.GrandTotal,
                                   CashCollectionAmt = g.Key.CashCollectionAmt,
                                   Quantity = g.Sum(i => i.Quantity),
                                   Balance = 0,
                                   InvoiceDue = g.Key.PaymentDue,
                                   Remarks = g.Select(i => i.Remarks).FirstOrDefault(),
                                   TotalAmtWD = g.Select(i => i.TotalAmtWD).FirstOrDefault(),
                                   TotalAdjDis = g.Select(i => i.TotalAdjDis).FirstOrDefault(),
                               }).ToList();

            ledgers.AddRange(VmCashSales);
            #endregion

            #region Cash Sales Return ROrders
            var CashSalesProductReturn = from so in RorderRepository.All
                                         join sod in rOrderDetail.All on so.ROrderID equals sod.ROrderID
                                         join p in ProductRepository.All on sod.ProductID equals p.ProductID
                                         join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                                         from u in lj.DefaultIfEmpty()
                                         where so.CustomerID == CustomerID
                                         //so.Status == (int)EnumSalesType.ProductReturn && 
                                         select new
                                         {
                                             TotalAmount = so.GrandTotal,
                                             InvoiceDate = so.ReturnDate,
                                             so.InvoiceNo,
                                             RecAmount = so.PaidAmount,
                                             AdjAmount = 0m,
                                             Credit = (decimal)(so.GrandTotal),
                                             Debit = (decimal)(so.PaidAmount),
                                             Return = (decimal)(so.GrandTotal - so.PaidAmount),
                                             sod.UnitPrice,
                                             sod.UTAmount,
                                             sod.Quantity,
                                             ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + sod.UnitPrice + " " + sod.UTAmount,
                                             EnteredBy = u == null ? string.Empty : u.UserName,
                                             Remarks = so.Remarks,
                                         };

            var VmCashSalesProductReturn = (from cs in CashSalesProductReturn
                                            group cs by new { cs.Debit, cs.Credit, cs.Return, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy } into g
                                            select new LedgerAccountReportModel
                                            {
                                                VoucherType = "Sales Return",
                                                InvoiceNo = g.Key.InvoiceNo,
                                                Date = g.Key.InvoiceDate,
                                                EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                                ProductList = g.Select(i => i.ProductName).ToList(),
                                                Debit = g.Key.Debit,
                                                Credit = g.Key.Credit,
                                                SalesReturn = g.Key.Return,
                                                Quantity = g.Sum(i => i.Quantity),
                                                Balance = 0,
                                                Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                            }).ToList();

            ledgers.AddRange(VmCashSalesProductReturn);
            #endregion


            #region Cash Sales Return
            var CashSalesReturn = from so in SOrderRepository.All
                                  join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                  join p in ProductRepository.All on sod.ProductID equals p.ProductID
                                  join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                                  from u in lj.DefaultIfEmpty()
                                  where so.Status == (int)EnumSalesType.ProductReturn && so.CustomerID == CustomerID
                                  select new
                                  {
                                      so.TotalAmount,
                                      so.InvoiceDate,
                                      so.InvoiceNo,
                                      so.RecAmount,
                                      so.AdjAmount,
                                      Credit = (decimal)(so.TotalAmount),
                                      Debit = (decimal)(so.RecAmount),
                                      Return = (decimal)(so.TotalAmount - so.RecAmount),
                                      sod.UnitPrice,
                                      sod.UTAmount,
                                      sod.Quantity,
                                      ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + sod.SRate + " " + sod.UTAmount,
                                      EnteredBy = u == null ? string.Empty : u.UserName,
                                      Remarks = so.Remarks,
                                  };

            var VmCashSalesReturn = (from cs in CashSalesReturn
                                     group cs by new { cs.Debit, cs.Credit, cs.Return, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy } into g
                                     select new LedgerAccountReportModel
                                     {
                                         VoucherType = "Sales Return",
                                         InvoiceNo = g.Key.InvoiceNo,
                                         Date = g.Key.InvoiceDate,
                                         EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                         ProductList = g.Select(i => i.ProductName).ToList(),
                                         Debit = g.Key.Debit,
                                         Credit = g.Key.Credit,
                                         SalesReturn = g.Key.Return,
                                         Quantity = g.Sum(i => i.Quantity),
                                         Balance = 0,
                                         Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                     }).ToList();

            ledgers.AddRange(VmCashSalesReturn);
            #endregion

            #region Credit Sales
            var CreditSales = from so in CreditSaleRepository.All
                                  //join CSO in CreditSaleDetailsRepo.All on so.CreditSalesID equals sod.CreditSalesID
                              join sod in CreditSaleDetailsRepo.All on so.CreditSalesID equals sod.CreditSalesID
                              join p in ProductRepository.All on sod.ProductID equals p.ProductID
                              join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                              from u in lj.DefaultIfEmpty()
                              where so.IsStatus == EnumSalesType.Sales && so.CustomerID == CustomerID
                              select new
                              {
                                  so.NetAmount,
                                  so.SalesDate,
                                  so.InvoiceNo,
                                  so.InterestAmount,
                                  CreditAdj = so.Discount,
                                  Credit = so.DownPayment,
                                  CashCollectionAmt = so.DownPayment,
                                  Debit = so.NetAmount + so.PenaltyInterest + so.InterestAmount,
                                  GrandTotal = so.TSalesAmt,
                                  sod.UnitPrice,
                                  sod.UTAmount,
                                  sod.Quantity,
                                  sod.IntTotalAmt,
                                  ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + sod.UnitPrice + " " + sod.UTAmount,
                                  EnteredBy = u == null ? string.Empty : u.UserName,
                                  Remarks = so.Remarks,
                                  so.PenaltyInterest
                              };

            var VmCreditSales = (from cs in CreditSales
                                 group cs by new { cs.Debit, cs.Credit, cs.GrandTotal, cs.CashCollectionAmt, cs.SalesDate, cs.InvoiceNo, cs.EnteredBy } into g
                                 select new LedgerAccountReportModel
                                 {
                                     VoucherType = "Hire Sales",
                                     InvoiceNo = g.Key.InvoiceNo,
                                     Date = g.Key.SalesDate,
                                     EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                     ProductList = g.Select(i => i.ProductName).ToList(),
                                     Debit = g.Key.Debit - g.Sum(i => i.IntTotalAmt),
                                     Credit = g.Key.Credit,
                                     CashCollectionAmt = g.Key.CashCollectionAmt,
                                     GrandTotal = g.Key.GrandTotal - g.Sum(i => i.IntTotalAmt),
                                     Quantity = g.Sum(i => i.Quantity),
                                     CInterAmt = g.Sum(i => i.IntTotalAmt),
                                     CrInterestAmount = g.Select(i => i.InterestAmount + i.PenaltyInterest).FirstOrDefault(),

                                     Balance = 0,
                                     Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                 }).ToList();

            ledgers.AddRange(VmCreditSales);
            #endregion


            #region Credit Sales Retur
            var CreditSalesProductReturn = from so in hireSalesReturn.All
                                           where so.CustomerId == CustomerID
                                           //so.Status == (int)EnumSalesType.ProductReturn && 
                                           select new
                                           {
                                               TotalAmount = so.TotalRemainingDue,
                                               InvoiceDate = so.TransactionDate,
                                               so.MemoNo,
                                               RecAmount = so.AdjDue,
                                               AdjAmount = 0m,
                                               Credit = (decimal)(so.AdjDue),
                                               Debit = 0m,
                                               Return = (decimal)(so.AdjDue),
                                               Remarks = so.Remarks,
                                           };

            var VmCreditSalesProductReturn = (from cs in CreditSalesProductReturn
                                              group cs by new { cs.Debit, cs.Credit, cs.Return, cs.InvoiceDate, cs.MemoNo } into g
                                              select new LedgerAccountReportModel
                                              {
                                                  VoucherType = "Hire Sales Return",
                                                  InvoiceNo = g.Key.MemoNo,
                                                  Date = g.Key.InvoiceDate,
                                                  Debit = g.Key.Debit,
                                                  Credit = g.Key.Credit,
                                                  SalesReturn = g.Key.Return,
                                                  Balance = 0,
                                                  Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                              }).ToList();

            ledgers.AddRange(VmCreditSalesProductReturn);
            #endregion

            #region Installment Collection
            var CreditSchedule = from so in CreditSaleRepository.All
                                 join sod in CreditSalesScheduleRepo.All on so.CreditSalesID equals sod.CreditSalesID
                                 where so.IsStatus == EnumSalesType.Sales && sod.PaymentStatus == "Paid" && so.CustomerID == CustomerID && sod.InstallmentAmt != 0
                                 select new LedgerAccountReportModel
                                 {
                                     VoucherType = "Installment",
                                     InvoiceNo = so.InvoiceNo + "-" + sod.ScheduleNo,
                                     Date = sod.PaymentDate,
                                     Debit = 0m,
                                     Quantity = 0m,
                                     Credit = sod.InstallmentAmt + sod.LastPayAdjust,
                                     CashCollectionAmt = sod.InstallmentAmt,
                                     CreditAdj = sod.LastPayAdjust,
                                     Balance = 0,
                                     Remarks = sod.Remarks
                                 };
            ledgers.AddRange(CreditSchedule);
            #endregion

            #region Cash Collection
            var CashCollection = from cc in CashCollectionRepository.All
                                 join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                 from u in lj.DefaultIfEmpty()
                                 where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.FromCustomer
                                 select new LedgerAccountReportModel
                                 {
                                     Date = (DateTime)cc.EntryDate,
                                     Debit = cc.InterestAmt,
                                     VoucherType = "Cash Collection",
                                     Credit = cc.Amount + cc.AdjustAmt,
                                     CashCollectionAmt = cc.Amount,
                                     CreditAdj = cc.AdjustAmt,
                                     InvoiceNo = cc.ReceiptNo,
                                     EnteredBy = "Entered By: " + u.UserName,
                                     Remarks = cc.Remarks,
                                     InterestAmt = cc.InterestAmt
                                 };
            ledgers.AddRange(CashCollection);
            #endregion

            #region Cash Collection Return
            var CashCollectionReturn = from ccr in CashCollectionRepository.All
                                       join u in UserRepository.All on ccr.CreatedBy equals u.Id into lj
                                       from u in lj.DefaultIfEmpty()
                                       where ccr.CustomerID == CustomerID && ccr.TransactionType == EnumTranType.CollectionReturn
                                       select new LedgerAccountReportModel
                                       {
                                           Date = (DateTime)ccr.EntryDate,
                                           Credit = 0m,
                                           VoucherType = "Cash Collection Return",
                                           Debit = ccr.Amount + ccr.AdjustAmt,
                                           CashCollectionReturn = ccr.Amount,
                                           CreditAdj = ccr.AdjustAmt,
                                           InvoiceNo = ccr.ReceiptNo,
                                           EnteredBy = "Entered By: " + u.UserName,
                                           Remarks = ccr.Remarks
                                       };
            ledgers.AddRange(CashCollectionReturn);
            #endregion

            #region Cash Collection Debit Adj
            var CashCollectionDebitAdj = from ccr in CashCollectionRepository.All
                                         join u in UserRepository.All on ccr.CreatedBy equals u.Id into lj
                                         from u in lj.DefaultIfEmpty()
                                         where ccr.CustomerID == CustomerID && ccr.TransactionType == EnumTranType.DebitAdjustment
                                         select new LedgerAccountReportModel
                                         {
                                             Date = (DateTime)ccr.EntryDate,
                                             Credit = 0m,
                                             VoucherType = "Cash Coll Debit Adj.",
                                             Debit = ccr.Amount + ccr.AdjustAmt,
                                             CashCollectionReturn = 0m,
                                             CreditAdj = 0m,
                                             DebitAdj = ccr.Amount + ccr.AdjustAmt,
                                             InvoiceNo = ccr.ReceiptNo,
                                             EnteredBy = "Entered By: " + u.UserName,
                                             Remarks = ccr.Remarks
                                         };
            ledgers.AddRange(CashCollectionDebitAdj);
            #endregion

            #region Cash Collection Credit Adj
            var CashCollectionCreditAdj = from cc in CashCollectionRepository.All
                                          join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                          from u in lj.DefaultIfEmpty()
                                          where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.CreditAdjustment
                                          select new LedgerAccountReportModel
                                          {
                                              Date = (DateTime)cc.EntryDate,
                                              Debit = cc.InterestAmt,
                                              VoucherType = "Cash Coll Credit Adj.",
                                              Credit = cc.Amount + cc.AdjustAmt,
                                              CashCollectionAmt = 0m,
                                              CreditAdj = cc.Amount + cc.AdjustAmt,
                                              InvoiceNo = cc.ReceiptNo,
                                              EnteredBy = "Entered By: " + u.UserName,
                                              Remarks = cc.Remarks,
                                              InterestAmt = cc.InterestAmt
                                          };
            ledgers.AddRange(CashCollectionCreditAdj);
            #endregion
            #region Cash Collection RateAdjustmrntForCustomer
            var CashCollectionRateAdjustment = from cc in CashCollectionRepository.All
                                               join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                               from u in lj.DefaultIfEmpty()
                                               where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.RateAdjustmentForCustomer
                                               select new LedgerAccountReportModel
                                               {
                                                   Date = (DateTime)cc.EntryDate,
                                                   Debit = cc.InterestAmt,
                                                   VoucherType = "Cash Coll Rate Adj.",
                                                   Credit = cc.Amount + cc.AdjustAmt,
                                                   CashCollectionAmt = 0m,
                                                   CreditAdj = cc.Amount + cc.AdjustAmt,
                                                   InvoiceNo = cc.ReceiptNo,
                                                   EnteredBy = "Entered By: " + u.UserName,
                                                   Remarks = cc.Remarks,
                                                   InterestAmt = cc.InterestAmt
                                               };
            ledgers.AddRange(CashCollectionRateAdjustment);
            #endregion

            #region Cash Collection PriceProtection
            var CashCollectionPriceProtection = from cc in CashCollectionRepository.All
                                    join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                    from u in lj.DefaultIfEmpty()
                                    where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.PriceProtectionForCustomer
                                    select new LedgerAccountReportModel
                                    {
                                        Date = (DateTime)cc.EntryDate,
                                        Debit = cc.InterestAmt,
                                        VoucherType = "Cash Coll Price Protection",
                                        Credit = cc.Amount + cc.AdjustAmt,
                                        CashCollectionAmt = 0m,
                                        CreditAdj = cc.Amount + cc.AdjustAmt,
                                        InvoiceNo = cc.ReceiptNo,
                                        EnteredBy = "Entered By: " + u.UserName,
                                        Remarks = cc.Remarks,
                                        InterestAmt = cc.InterestAmt
                                    };
            ledgers.AddRange(CashCollectionPriceProtection);
            #endregion


            #region Cash Collection PromoOffer
            var CashCollectionPromoOffer = from cc in CashCollectionRepository.All
                                                join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                                from u in lj.DefaultIfEmpty()
                                                where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.PromoOfferForCustomer
                                                select new LedgerAccountReportModel
                                                {
                                                    Date = (DateTime)cc.EntryDate,
                                                    Debit = cc.InterestAmt,
                                                    VoucherType = "Cash Coll Promo Offer",
                                                    Credit = cc.Amount + cc.AdjustAmt,
                                                    CashCollectionAmt = 0m,
                                                    CreditAdj = cc.Amount + cc.AdjustAmt,
                                                    InvoiceNo = cc.ReceiptNo,
                                                    EnteredBy = "Entered By: " + u.UserName,
                                                    Remarks = cc.Remarks,
                                                    InterestAmt = cc.InterestAmt
                                                };
            ledgers.AddRange(CashCollectionPromoOffer);
            #endregion

            #region Cash Collection KPI
            var CashCollectionKPI = from cc in CashCollectionRepository.All
                                    join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                    from u in lj.DefaultIfEmpty()
                                    where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.KPIForCustomer
                                    select new LedgerAccountReportModel
                                    {
                                        Date = (DateTime)cc.EntryDate,
                                        Debit = cc.InterestAmt,
                                        VoucherType = "Cash Coll KPI",
                                        Credit = cc.Amount + cc.AdjustAmt,
                                        CashCollectionAmt = 0m,
                                        CreditAdj = cc.Amount + cc.AdjustAmt,
                                        InvoiceNo = cc.ReceiptNo,
                                        EnteredBy = "Entered By: " + u.UserName,
                                        Remarks = cc.Remarks,
                                        InterestAmt = cc.InterestAmt
                                    };
            ledgers.AddRange(CashCollectionKPI);
            #endregion

            #region Cash Collection Incentive
            var CashCollectionIncentive = from cc in CashCollectionRepository.All
                                               join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                               from u in lj.DefaultIfEmpty()
                                               where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.IncentiveForCustomer
                                               select new LedgerAccountReportModel
                                               {
                                                   Date = (DateTime)cc.EntryDate,
                                                   Debit = cc.InterestAmt,
                                                   VoucherType = "Cash Coll Incentive",
                                                   Credit = cc.Amount + cc.AdjustAmt,
                                                   CashCollectionAmt = 0m,
                                                   CreditAdj = cc.Amount + cc.AdjustAmt,
                                                   InvoiceNo = cc.ReceiptNo,
                                                   EnteredBy = "Entered By: " + u.UserName,
                                                   Remarks = cc.Remarks,
                                                   InterestAmt = cc.InterestAmt
                                               };
            ledgers.AddRange(CashCollectionIncentive);
            #endregion



            #region Bank Transaction
            var bankTrans = from bt in BankTransactionRepository.All
                            join b in BankRepository.All on bt.BankID equals b.BankID
                            where bt.CustomerID == CustomerID && bt.TransactionType == 3
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)bt.TranDate,
                                Debit = 0m,
                                VoucherType = "Bank Collect.",
                                Credit = bt.Amount,
                                CashCollectionAmt = bt.Amount,
                                CreditAdj = 0m,
                                InvoiceNo = bt.TransactionNo,
                                Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + bt.ChecqueNo,
                                Remarks = bt.Remarks
                            };
            ledgers.AddRange(bankTrans);
            #endregion

            #region Bank Collection Return
            var BankCollectionReturn = from ccr in BankTransactionRepository.All
                                       join b in BankRepository.All on ccr.BankID equals b.BankID
                                       where ccr.CustomerID == CustomerID && ccr.TransactionType == 11
                                       select new LedgerAccountReportModel
                                       {
                                           Date = (DateTime)ccr.TranDate,
                                           Credit = 0m,
                                           VoucherType = "Bank Collection Return",
                                           Debit = ccr.Amount,
                                           CashCollectionReturn = ccr.Amount,
                                           CreditAdj = 0m,
                                           InvoiceNo = ccr.TransactionNo,
                                           Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + ccr.ChecqueNo,
                                           Remarks = ccr.Remarks
                                       };
            ledgers.AddRange(BankCollectionReturn);
            #endregion


            #region OpeningDue

            decimal balance = Customer.OpeningDue;
            ledgers = ledgers.OrderBy(i => i.Date).ToList();
            foreach (var item in ledgers)
            {
                item.Balance = balance + (item.Debit - item.Credit);
                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.ProductList) + Environment.NewLine + item.EnteredBy : item.Particulars;
                //item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.EnteredBy) : item.Particulars;
                balance = item.Balance;
            }

            var oOpening = new LedgerAccountReportModel() { Date = new DateTime(2015, 1, 1), Particulars = "Opening Balance", Debit = Customer.OpeningDue, Balance = 0, Credit = 0 };

            if (ledgers.Count > 0)
            {
                //ledgers.Insert(0, oOpening);
                //var OpeningTrans = ledgers.Where(i => i.Date < fromDate).OrderByDescending(i => i.Date).FirstOrDefault();
                var OpeningTrans = ledgers.Where(i => i.Date < fromDate).OrderByDescending(i => i.Date < fromDate).LastOrDefault();
                if (OpeningTrans != null)
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = OpeningTrans.Date, Particulars = "Opening Balance", Balance = OpeningTrans.Balance, Debit = 0m });
                else
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Balance = Customer.OpeningDue, Debit = 0m });

                ledgers = ledgers.Where(i => i.Date >= fromDate && i.Date <= toDate).OrderBy(i => i.Date).ToList();
                FinalLedgers.AddRange(ledgers);
            }
            else
            {
                FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Debit = Customer.OpeningDue, Credit = 0m, Balance = Customer.OpeningDue });
            }

            return FinalLedgers;
        }

        #endregion

        /// <summary>
        /// Date: 12-02-2020
        /// Author: Aminul
        /// Customer Due report with opening closing
        /// </summary>
        /// <returns></returns>
        public static List<CustomerDueReport> CustomerDueReport(this IBaseRepository<SOrder> SOrderRepository,
                    IBaseRepository<Customer> CustomerRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
                    IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                    IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ROrderDetail> ROrderDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                    IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository, int CustomerID, DateTime fromDate, DateTime toDate, int ConcernID,
                    EnumCustomerType CustomerType, int IsOnlyDue, bool IsAdminReport)
        {
            List<CustomerDueReport> CustomerDues = new List<CustomerDueReport>();
            IQueryable<Customer> Customers = null;
            IQueryable<SOrder> SOrders = null;
            //IQueryable<ROrder> ROrders = null;
            IQueryable<CreditSale> CreditSales = null;
            IQueryable<CashCollection> CashCollections = null;
            IQueryable<BankTransaction> BankTransactions = null;
            var CreditSchedules = CreditSalesScheduleRepository.All;
            var CreditSaleDetails = CreditSaleDetailsRepository.All;

            if (IsAdminReport)
            {
                if (ConcernID > 0)
                {
                    if (CustomerID > 0)
                        Customers = CustomerRepository.GetAll().Where(i => i.CustomerID == CustomerID);
                    else if (CustomerType != 0)
                        Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID
                        && i.CustomerType == CustomerType);
                    else
                        Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);

                    SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    CreditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.IsStatus == EnumSalesType.Sales);

                    CashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == EnumTranType.FromCustomer || i.TransactionType == EnumTranType.CollectionReturn || i.TransactionType == EnumTranType.CreditAdjustment || i.TransactionType == EnumTranType.DebitAdjustment);
                    BankTransactions = BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == (int)EnumTransactionType.CashCollection);

                }
                else
                {
                    if (CustomerID > 0)
                        Customers = CustomerRepository.GetAll().Where(i => i.CustomerID == CustomerID);
                    else if (CustomerType != 0)
                        Customers = CustomerRepository.GetAll().Where(i => i.CustomerType == CustomerType);
                    else
                        Customers = CustomerRepository.GetAll();

                    SOrders = SOrderRepository.GetAll();
                    CreditSales = CreditSaleRepository.GetAll().Where(i => i.IsStatus == EnumSalesType.Sales);
                    CashCollections = CashCollectionRepository.GetAll().Where(i => i.CustomerID > 0 && i.TransactionType == EnumTranType.FromCustomer || i.TransactionType == EnumTranType.CollectionReturn || i.TransactionType == EnumTranType.CreditAdjustment || i.TransactionType == EnumTranType.DebitAdjustment);
                    BankTransactions = BankTransactionRepository.GetAll().Where(i => i.CustomerID > 0 && i.TransactionType == (int)EnumTransactionType.CashCollection);

                }

            }
            else
            {
                if (CustomerID > 0)
                    Customers = CustomerRepository.All.Where(i => i.CustomerID == CustomerID);
                else if (CustomerType != 0)
                    Customers = CustomerRepository.All.Where(i => i.CustomerType == CustomerType);
                else
                    Customers = CustomerRepository.All;

                SOrders = SOrderRepository.All;
                CreditSales = CreditSaleRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales);
                CashCollections = CashCollectionRepository.All.Where(i => i.CustomerID > 0 && (i.TransactionType == EnumTranType.FromCustomer || i.TransactionType == EnumTranType.CollectionReturn || i.TransactionType == EnumTranType.CreditAdjustment || i.TransactionType == EnumTranType.DebitAdjustment));
                BankTransactions = BankTransactionRepository.All.Where(i => i.CustomerID > 0 && i.TransactionType == (int)EnumTransactionType.CashCollection);
            }


            if (IsOnlyDue > 0)
                Customers = Customers.Where(i => (i.CreditDue + i.TotalDue) != 0);

            DateTime StartDate = new DateTime(2000, 1, 1);

            #region Opening Due
            var CustomersOpeningDue = from c in Customers
                                      select new CustomerDueReport
                                      {
                                          //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                          CustomerType = c.CustomerType,
                                          Date = StartDate,
                                          CustomerID = c.CustomerID,
                                          Code = c.Code,
                                          CustomerName = c.Name,
                                          ContactNo = c.ContactNo,
                                          Address = c.Address,
                                          TotalSales = 0m,
                                          OpeningDue = c.OpeningDue,
                                          ReceiveAmt = 0m,
                                      };

            CustomerDues.AddRange(CustomersOpeningDue);
            #endregion

            #region Sales Order
            var SalesList = from so in SOrders
                            join c in Customers on so.CustomerID equals c.CustomerID
                            where so.Status == (int)EnumSalesType.Sales
                            select new CustomerDueReport
                            {
                                //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                CustomerType = c.CustomerType,
                                Date = so.InvoiceDate,
                                CustomerID = c.CustomerID,
                                Code = c.Code,
                                CustomerName = c.Name,
                                ContactNo = c.ContactNo,
                                Address = c.Address,
                                TotalSales = so.TotalAmount,
                                PaymentDue = so.PaymentDue,
                                ReceiveAmt = (decimal)so.RecAmount,
                                CardPaidAmount = so.CardPaidAmount,
                                CashReceiveAmt = (decimal)so.RecAmount - so.CardPaidAmount
                            };
            CustomerDues.AddRange(SalesList);
            #endregion

            #region Credit Sales Order
            var CreditSalesList = from so in CreditSales
                                  join csd in CreditSaleDetails on so.CreditSalesID equals csd.CreditSalesID
                                  join c in Customers on so.CustomerID equals c.CustomerID
                                  select new
                                  {
                                      //CustomerType = EnumCustomerType.Hire, //c.CustomerType,
                                      //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,

                                      CustomerType = c.CustomerType,
                                      Date = so.SalesDate,
                                      CustomerID = c.CustomerID,
                                      Code = c.Code,
                                      CustomerName = c.Name,
                                      ContactNo = c.ContactNo,
                                      Address = c.Address,
                                      TotalSales = so.TSalesAmt,
                                      PaymentDue = so.NetAmount - so.DownPayment,
                                      DownPayment = (decimal)so.DownPayment,
                                      CardPaidAmount = so.CardPaidAmount,
                                      CashReceiveAmt = (decimal)so.DownPayment - so.CardPaidAmount,
                                      CrInterestAmt = (decimal)so.InterestAmount + so.PenaltyInterest,
                                      CrInterestAmt2 = (so.InterestAmount - csd.IntTotalAmt),
                                      InterestAmt = csd.IntTotalAmt
                                  };

            var VmCreditSales = (from cs in CreditSalesList
                                 group cs by new { cs.CustomerID, cs.Date, cs.TotalSales } into g
                                 select new CustomerDueReport
                                 {
                                     CustomerType = g.Select(i => i.CustomerType).FirstOrDefault(),
                                     Date = g.Key.Date,
                                     CustomerID = g.Key.CustomerID,
                                     Code = g.Select(i => i.Code).FirstOrDefault(),
                                     CustomerName = g.Select(i => i.CustomerName).FirstOrDefault(),
                                     ContactNo = g.Select(i => i.ContactNo).FirstOrDefault(),
                                     Address = g.Select(i => i.Address).FirstOrDefault(),
                                     TotalSales = g.Key.TotalSales - g.Sum(i => i.InterestAmt),
                                     PaymentDue = g.Select(i => i.PaymentDue).FirstOrDefault(),
                                     DownPayment = g.Select(i => i.DownPayment).FirstOrDefault(),
                                     CardPaidAmount = g.Select(i => i.CardPaidAmount).FirstOrDefault(),
                                     CashReceiveAmt = g.Select(i => i.CashReceiveAmt).FirstOrDefault(),
                                     CrInterestAmt = g.Select(i => i.CrInterestAmt).FirstOrDefault(),
                                     CrInterestAmt2 = g.Select(i => i.CrInterestAmt2).FirstOrDefault()
                                 }).ToList();

            CustomerDues.AddRange(VmCreditSales);
            #endregion

            #region Installment  Collections
            var InstallmentList = from so in CreditSales
                                  join css in CreditSchedules on so.CreditSalesID equals css.CreditSalesID
                                  join c in Customers on so.CustomerID equals c.CustomerID
                                  where css.PaymentStatus == "Paid"
                                  select new CustomerDueReport
                                  {
                                      //CustomerType = EnumCustomerType.Hire, //c.CustomerType,
                                      //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                      CustomerType = c.CustomerType,
                                      Date = css.PaymentDate,
                                      CustomerID = c.CustomerID,
                                      Code = c.Code,
                                      CustomerName = c.Name,
                                      ContactNo = c.ContactNo,
                                      Address = c.Address,
                                      TotalSales = 0,
                                      PaymentDue = 0,
                                      InstallmentCollection = css.InstallmentAmt,
                                      Adjustment = css.LastPayAdjust,
                                      CardPaidAmount = 0m,//css.CardPaidAmount,
                                      CashReceiveAmt = css.InstallmentAmt - css.CardPaidAmount
                                  };
            CustomerDues.AddRange(InstallmentList);
            #endregion

            #region Sales Return
            var SalesReturnList = from so in SOrders
                                  join c in Customers on so.CustomerID equals c.CustomerID
                                  where so.Status == (int)EnumSalesType.ProductReturn
                                  select new CustomerDueReport
                                  {
                                      CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                      //CustomerType = c.CustomerType,
                                      Date = so.InvoiceDate,
                                      CustomerID = c.CustomerID,
                                      Code = c.Code,
                                      CustomerName = c.Name,
                                      ContactNo = c.ContactNo,
                                      Address = c.Address,
                                      SalesReturnAmt = so.TotalAmount,
                                      SalesReturnPayable = so.PaymentDue,
                                      SalesReturnCashBack = (decimal)so.RecAmount,
                                  };
            CustomerDues.AddRange(SalesReturnList);
            #endregion

            #region Cash Collections
            var CashCollectionList = from cc in CashCollections
                                     join c in Customers on cc.CustomerID equals c.CustomerID
                                     //where cc.TransactionType == EnumTranType.FromCustomer
                                     select new CustomerDueReport
                                     {
                                         //CustomerType = c.CustomerType,
                                         //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                         CustomerType = c.CustomerType,
                                         Date = (DateTime)cc.EntryDate,
                                         CustomerID = c.CustomerID,
                                         Code = c.Code,
                                         CustomerName = c.Name,
                                         ContactNo = c.ContactNo,
                                         Address = c.Address,
                                         SalesReturnAmt = 0m,
                                         SalesReturnPayable = 0m,
                                         SalesReturnCashBack = 0m,
                                         CashCollectionAmt = cc.TransactionType == EnumTranType.FromCustomer ? cc.Amount : 0m,
                                         CashCollectionCreditAdjAmt = cc.TransactionType == EnumTranType.CreditAdjustment ? cc.Amount : 0m,
                                         CashCollectionReturnAmt = cc.TransactionType == EnumTranType.CollectionReturn ? cc.Amount : 0m,
                                         CashCollectionDebitAdjAmt = cc.TransactionType == EnumTranType.DebitAdjustment ? cc.Amount : 0m,
                                         Adjustment = cc.TransactionType == EnumTranType.FromCustomer ? cc.AdjustAmt : 0m,
                                         CashCollectionIntAmt = cc.InterestAmt
                                     };
            CustomerDues.AddRange(CashCollectionList);

            #endregion

            #region Cash Collection Returns
            //var CashCollectionReturnList = from cc in CashCollections
            //                         join c in Customers on cc.CustomerID equals c.CustomerID
            //                         where cc.TransactionType == EnumTranType.CollectionReturn
            //                         select new CustomerDueReport
            //                         {
            //                             //CustomerType = c.CustomerType,
            //                             //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
            //                             CustomerType = c.CustomerType,
            //                             Date = (DateTime)cc.EntryDate,
            //                             CustomerID = c.CustomerID,
            //                             Code = c.Code,
            //                             CustomerName = c.Name,
            //                             ContactNo = c.ContactNo,
            //                             Address = c.Address,
            //                             SalesReturnAmt = 0m,
            //                             SalesReturnPayable = 0m,
            //                             SalesReturnCashBack = 0m,
            //                             CashCollectionReturnAmt = cc.Amount,
            //                             Adjustment = 0m
            //                             //Adjustment = cc.AdjustAmt
            //                         };
            //CustomerDues.AddRange(CashCollectionReturnList);
            #endregion

            #region Bank Collections
            var BankTransList = from bt in BankTransactions
                                join c in Customers on bt.CustomerID equals c.CustomerID
                                select new CustomerDueReport
                                {
                                    //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                    CustomerType = c.CustomerType,
                                    Date = (DateTime)bt.TranDate,
                                    CustomerID = c.CustomerID,
                                    Code = c.Code,
                                    CustomerName = c.Name,
                                    ContactNo = c.ContactNo,
                                    Address = c.Address,
                                    SalesReturnAmt = 0m,
                                    SalesReturnPayable = 0m,
                                    SalesReturnCashBack = 0m,
                                    BankCollectionAmt = bt.Amount,
                                    Adjustment = 0m
                                };
            CustomerDues.AddRange(BankTransList);
            #endregion

            #region Cash Sales Return ROrder

            var CashSalesProductReturn = from ro in ROrderRepository.All
                                             //join rod in ROrderDetailRepository.All on ro.ROrderID equals rod.ROrderID
                                             //join p in ProductRepository.All on rod.ProductID equals p.ProductID
                                         join c in Customers on ro.CustomerID equals c.CustomerID
                                         //where ro.CustomerID == CustomerID
                                         select new CustomerDueReport
                                         {
                                             //CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType,
                                             CustomerType = c.CustomerType,
                                             Date = ro.ReturnDate,
                                             CustomerID = c.CustomerID,
                                             Code = c.Code,
                                             CustomerName = c.Name,
                                             ContactNo = c.ContactNo,
                                             Address = c.Address,
                                             ReturnAmt = ro.GrandTotal,
                                             ReturnCashBack = (decimal)ro.PaidAmount,
                                             Return = (decimal)(ro.GrandTotal - ro.PaidAmount)

                                         };
            CustomerDues.AddRange(CashSalesProductReturn);
            #endregion

            #region Hire Sales Return
            var HireSalesProductReturn = from ro in hireSalesReturnRepository.All
                                         join c in Customers on ro.CustomerId equals c.CustomerID
                                         select new CustomerDueReport
                                         {
                                             CustomerType = c.CustomerType,
                                             Date = ro.TransactionDate,
                                             CustomerID = c.CustomerID,
                                             Code = c.Code,
                                             CustomerName = c.Name,
                                             ContactNo = c.ContactNo,
                                             Address = c.Address,
                                             ReturnAmt = ro.AdjDue,
                                             ReturnCashBack = (decimal)ro.AdjDue,
                                             Return = (decimal)(ro.AdjDue)

                                         };
            CustomerDues.AddRange(HireSalesProductReturn);
            #endregion

            var GroupData = from d in CustomerDues
                            group d by new { d.CustomerType, d.CustomerID, d.Code, d.ContactNo, d.CustomerName, d.Address } into g
                            select new CustomerDueReport
                            {
                                CustomerType = g.Key.CustomerType,
                                CustomerID = g.Key.CustomerID,
                                Code = g.Key.Code,
                                CustomerName = g.Key.CustomerName,
                                Address = g.Key.Address,
                                ContactNo = g.Key.ContactNo,
                                OpeningDue = g.Sum(i => i.OpeningDue),
                                //Pre sales
                                PreviousSales = g.Where(i => i.Date < fromDate).Sum(i => i.TotalSales),
                                PrevSalesReturn = g.Where(i => i.Date < fromDate).Sum(i => i.SalesReturnAmt),
                                PrevReturnPayable = g.Where(i => i.Date < fromDate).Sum(i => i.SalesReturnPayable),
                                PrevSalesReturnCashBack = g.Where(i => i.Date < fromDate).Sum(i => i.SalesReturnCashBack),
                                PrevReturn = g.Where(i => i.Date < fromDate).Sum(i => i.Return),
                                PrevPaymentDue = g.Where(i => i.Date < fromDate).Sum(i => i.PaymentDue),

                                //prev Collections
                                PrevCashCollection = g.Where(i => i.Date < fromDate).Sum(i => i.CashCollectionAmt),
                                PrevCashCollectionCreditAdjAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CashCollectionCreditAdjAmt),
                                PrevAdjustment = g.Where(i => i.Date < fromDate).Sum(i => i.Adjustment),
                                PrevReceiveAmt = g.Where(i => i.Date < fromDate).Sum(i => i.ReceiveAmt),
                                PrevCardPaidAmount = g.Where(i => i.Date < fromDate).Sum(i => i.CardPaidAmount),
                                PrevCashReceiveAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CashReceiveAmt),
                                PrevDownPayment = g.Where(i => i.Date < fromDate).Sum(i => i.DownPayment),
                                PrevBankCollectionAmt = g.Where(i => i.Date < fromDate).Sum(i => i.BankCollectionAmt),
                                PrevInstallmentCollection = g.Where(i => i.Date < fromDate).Sum(i => i.InstallmentCollection),
                                PrevCashCollectionReturnAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CashCollectionReturnAmt),
                                PrevCashCollectionDebitAdjAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CashCollectionDebitAdjAmt),
                                PrevCashCollectionIntAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CashCollectionIntAmt),
                                PrevCrInterestAmt = g.Where(i => i.Date < fromDate).Sum(i => i.CrInterestAmt),
                                //PrevHCashCollectionIntAmt= g.Where(i => i.Date < fromDate).Sum(i => i.CrInterestAmt),                      

                                //Date range sales
                                TotalSales = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.TotalSales),
                                SalesReturnAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.SalesReturnAmt),
                                SalesReturnPayable = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.SalesReturnPayable),
                                SalesReturnCashBack = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.SalesReturnCashBack),
                                PaymentDue = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.PaymentDue),

                                //Date range Collections
                                CashCollectionAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashCollectionAmt),
                                CashCollectionCreditAdjAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashCollectionCreditAdjAmt),
                                Adjustment = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.Adjustment),
                                ReceiveAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.ReceiveAmt),
                                CardPaidAmount = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CardPaidAmount),
                                CashReceiveAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashReceiveAmt),
                                DownPayment = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.DownPayment),
                                BankCollectionAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.BankCollectionAmt),
                                InstallmentCollection = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.InstallmentCollection),
                                CashCollectionReturnAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashCollectionReturnAmt),
                                CashCollectionDebitAdjAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashCollectionDebitAdjAmt),
                                CashCollectionIntAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CashCollectionIntAmt),
                                CrInterestAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CrInterestAmt),
                                //CrInterestAmt2 = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.CrInterestAmt2),



                                //Sales Return
                                ReturnAmt = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.ReturnAmt),
                                ReturnCashBack = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.ReturnCashBack),

                                Return = g.Where(i => i.Date >= fromDate && i.Date <= toDate).Sum(i => i.Return),

                            };

            var FinalData = (from d in GroupData
                             select new CustomerDueReport
                             {
                                 CustomerType = d.CustomerType,
                                 CustomerID = d.CustomerID,
                                 Code = d.Code,
                                 CustomerName = d.CustomerName,
                                 Address = d.Address,
                                 ContactNo = d.ContactNo,
                                 OpeningDue = (d.OpeningDue + d.PreviousSales - d.PrevSalesReturn + d.PrevCashCollectionReturnAmt + d.PrevCashCollectionDebitAdjAmt + d.PrevCashCollectionIntAmt + d.PrevCrInterestAmt)
                                               - (d.PrevReceiveAmt + d.PrevDownPayment + d.PrevCashCollection + d.PrevCashCollectionCreditAdjAmt + d.PrevInstallmentCollection + d.PrevBankCollectionAmt - d.PrevSalesReturnCashBack + d.PrevAdjustment + d.PrevReturn),
                                 TotalSales = d.TotalSales - d.SalesReturnAmt,
                                 SalesReturnAmt = d.SalesReturnAmt,
                                 PaymentDue = d.PaymentDue,
                                 DownPayment = d.DownPayment,
                                 ReceiveAmt = d.ReceiveAmt,
                                 CardPaidAmount = d.CardPaidAmount,
                                 CashReceiveAmt = d.CashReceiveAmt,
                                 CashCollectionAmt = d.CashCollectionAmt + d.CashCollectionCreditAdjAmt - d.CashCollectionReturnAmt - d.CashCollectionDebitAdjAmt,
                                 InstallmentCollection = d.InstallmentCollection,
                                 BankCollectionAmt = d.BankCollectionAmt,
                                 Return = d.Return,
                                 CashCollectionIntAmt = d.CashCollectionIntAmt,
                                 CrInterestAmt = d.CrInterestAmt,
                                 //CrInterestAmt2 = d.CrInterestAmt2,


                                 TotalCollection = d.DownPayment + d.CashCollectionAmt + d.CashCollectionCreditAdjAmt + d.InstallmentCollection + d.BankCollectionAmt + d.ReceiveAmt - d.SalesReturnCashBack - d.Adjustment - d.CashCollectionReturnAmt - d.CashCollectionDebitAdjAmt,
                                 ClosingDue = (((d.OpeningDue + d.PreviousSales + d.PrevCashCollectionReturnAmt + d.PrevCashCollectionDebitAdjAmt + d.PrevCashCollectionIntAmt + d.PrevCrInterestAmt) - d.PrevSalesReturn)
                                               - (d.PrevReceiveAmt + d.PrevDownPayment + d.PrevCashCollection + d.PrevCashCollectionCreditAdjAmt + d.PrevInstallmentCollection + d.PrevBankCollectionAmt - d.PrevSalesReturnCashBack + d.PrevAdjustment + d.PrevReturn)
                                              ) +
                                              (
                                                ((d.TotalSales + d.CashCollectionReturnAmt + d.CashCollectionDebitAdjAmt + d.CashCollectionIntAmt + d.CrInterestAmt) - d.SalesReturnAmt) - (d.DownPayment + d.CashCollectionAmt + d.CashCollectionCreditAdjAmt + d.InstallmentCollection + d.BankCollectionAmt + d.ReceiveAmt - d.SalesReturnCashBack + d.Adjustment + d.Return)
                                              )

                             }).ToList();

            return FinalData;

        }

        /// <summary>
        /// Date: 01-03-202
        /// Author: aminul
        /// Reason: All transactions summary
        /// </summary>
        /// <returns></returns>
        public static List<SummaryReportModel> GetSummaryReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                    IBaseRepository<Customer> CustomerRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
                    IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                    IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
                    IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository,
                    DateTime Date, int ConcernID)
        {
            List<SummaryReportModel> summaryData = new List<SummaryReportModel>();

            var SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var SOrderDetails = SOrderDetailRepository.All;
            var CreditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.IsStatus == EnumSalesType.Sales);
            var CreditSchedules = CreditSalesScheduleRepository.All;
            var CreditDetails = CreditSaleDetailsRepository.All;
            var CashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == EnumTranType.FromCustomer);
            var BankTransactions = BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == (int)EnumTransactionType.CashCollection);
            //DateTime StartDate = new DateTime(2000, 1, 1);
            var Products = ProductRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var Categories = CategoryRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            #region Category Wise Sales Qty

            #region Sales Order Qty
            var SalesCategories = (from so in SOrders
                                   join sod in SOrderDetails on so.SOrderID equals sod.SOrderID
                                   join p in Products on sod.ProductID equals p.ProductID
                                   join cat in Categories on p.CategoryID equals cat.CategoryID
                                   where so.Status == (int)EnumSalesType.Sales && so.InvoiceDate == Date
                                   select new SummaryReportModel
                                   {
                                       id = cat.CategoryID,
                                       Head = cat.Description,
                                       Amount = sod.Quantity,
                                       Category = "Category Wise Sales Quantity"
                                   }).ToList();
            #endregion

            #region Credit Sales Order
            var CreditCategories = from so in CreditSales
                                   join sod in CreditDetails on so.CreditSalesID equals sod.CreditSalesID
                                   join p in Products on sod.ProductID equals p.ProductID
                                   join cat in Categories on p.CategoryID equals cat.CategoryID
                                   where so.SalesDate == Date
                                   select new SummaryReportModel
                                   {
                                       id = cat.CategoryID,
                                       Head = cat.Description,
                                       Amount = sod.Quantity,
                                       Category = "Category Wise Sales Quantity"
                                   };

            SalesCategories.AddRange(CreditCategories);

            var gCategoryCredit = from s in SalesCategories
                                  group s by new { s.Head, s.id, Catgory = s.Category } into g
                                  select new SummaryReportModel
                                  {
                                      id = g.Key.id,
                                      Category = g.Key.Catgory,
                                      Head = g.Key.Head,
                                      Amount = g.Sum(i => i.Amount)
                                  };

            summaryData.AddRange(gCategoryCredit);
            #endregion

            #endregion

            #region Customer Type Wise Sales Amount

            #region Sales Order amt
            var salesamount = (from so in SOrders
                               join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                               where so.Status == (int)EnumSalesType.Sales && so.InvoiceDate == Date
                               select new SummaryReportModel
                               {
                                   id = c.CustomerID,
                                   Head = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail.ToString() : c.CustomerType.ToString(),
                                   Amount = so.TotalAmount,
                                   Category = "Customer Type Wise Sales Amount"
                               }).ToList();
            #endregion

            #region Credit Sales amt
            var CreditSalesAmt = from so in CreditSales
                                 join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                                 where so.SalesDate == Date
                                 select new SummaryReportModel
                                 {
                                     id = c.CustomerID,
                                     Head = EnumCustomerType.Hire.ToString(),
                                     Amount = so.NetAmount,
                                     Category = "Customer Type Wise Sales Amount"
                                 };

            salesamount.AddRange(CreditSalesAmt);

            var gSalesAmt = from s in salesamount
                            group s by new { s.Head, Catgory = s.Category } into g
                            select new SummaryReportModel
                            {
                                id = 0,
                                Category = g.Key.Catgory,
                                Head = g.Key.Head,
                                Amount = g.Sum(i => i.Amount)
                            };

            summaryData.AddRange(gSalesAmt);
            #endregion

            #endregion

            return summaryData;
        }


        public static HireAccountDetailsReportModel DealerAccountDetails(this IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<CashCollection> CashCollectionRepository,
            DateTime fromDate, DateTime toDate, int ConcernID)
        {
            HireAccountDetailsReportModel summaryData = new HireAccountDetailsReportModel();

            var SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var CashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == EnumTranType.FromCustomer);
            var customers = CustomerRepository.All.Where(i => i.CustomerType == EnumCustomerType.Dealer && i.ConcernID == ConcernID);

            var opening = (from so in SOrders
                           join c in customers on so.CustomerID equals c.CustomerID
                           where so.Status == (int)EnumSalesType.Sales &&
                           (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate)
                           && so.PaymentDue > 0
                           select so).ToList();

            summaryData.OpeningAccount = opening.GroupBy(i => i.CustomerID).Count();

            if (summaryData.OpeningAccount > 0)
                summaryData.OpeningAccountValue = opening.Sum(i => i.PaymentDue);


            summaryData.RunningAccount = customers.Count(i => i.TotalDue != 0);
            if (summaryData.RunningAccount > 0)
                summaryData.RunningAccountValue = customers.Sum(i => i.TotalDue);


            var closing = from c in customers
                          join cc in CashCollectionRepository.All on c.CustomerID equals cc.CustomerID
                          where (cc.EntryDate >= fromDate && cc.EntryDate <= toDate) && c.TotalDue == 0
                          select c;

            if (closing.Count() > 0)
                summaryData.ClosingAccount = closing.GroupBy(i => i.CustomerID).Count();

            var closingValue = from c in closing
                               join so in SOrders on c.CustomerID equals so.CustomerID
                               select so;

            if (summaryData.ClosingAccount > 0)
                summaryData.ClosingAccountValue = closingValue.Sum(i => i.TotalAmount);

            return summaryData;
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
           string, decimal, EnumSalesType, Tuple<string, string>>>>
                               GetAllPendingSalesOrderAsync(this IBaseRepository<SOrder> salesOrderRepository,
                                               IBaseRepository<Customer> customerRepository, IBaseRepository<ApplicationUser> UserRepository)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<ApplicationUser> users = UserRepository.All;

            var items = await (from sord in salesOrderRepository.All
                               join cus in customers on sord.CustomerID equals cus.CustomerID
                               join us in UserRepository.All on sord.CreatedBy equals us.Id
                               where (sord.Status == (int)EnumSalesType.Pending)
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = sord.SOrderID,
                                   InvoiceNo = sord.InvoiceNo,
                                   Date = sord.InvoiceDate,
                                   CustomerName = cus.Name,
                                   Mobile = cus.ContactNo,
                                   TotalDue = cus.TotalDue,
                                   Status = sord.Status,
                                   IsReplacement = sord.IsReplacement,
                                   TotalAmount = sord.TotalAmount,
                                   UserName = us.UserName,


                               }).Where(i => i.IsReplacement == 0).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType,
               Tuple<string, string>>
               (
                   x.SOrderID,
                   x.InvoiceNo,
                   x.Date,
                   x.CustomerName,
                   x.Mobile,
                   x.TotalDue,
                   (EnumSalesType)x.Status,
                   new Tuple<string, string>
                   (
                    x.CustomerCode,

                    x.UserName

                    )
               )).OrderByDescending(x => x.Item3).ThenByDescending(i => i.Item2).ToList();
        }


        public static IEnumerable<SOredersReportModel> GetforSalesReportForAll(
            this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Employee> EmployeeRepository,
            DateTime fromDate, DateTime toDate, int EmployeeID, EnumCustomerType customerType)
        {
            IQueryable<Customer> Customers = null;
            if (EmployeeID > 0)
                Customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                Customers = customerRepository.All;


            var oSalesData = (from sord in salesOrderRepository.All
                              join cus in Customers on sord.CustomerID equals cus.CustomerID
                              join emp in EmployeeRepository.All on cus.EmployeeID equals emp.EmployeeID
                              where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales)
                              select new SOredersReportModel
                              {
                                  CustomerCode = cus.Code,
                                  CustomerName = cus.Name,
                                  CustomerAddress = cus.Address,
                                  CustomerContactNo = cus.ContactNo,
                                  InvoiceDate = sord.InvoiceDate,
                                  InvoiceNo = sord.InvoiceNo,
                                  Grandtotal = sord.GrandTotal,
                                  FlatDiscount = sord.TDAmount,
                                  TotalAmount = sord.TotalAmount,
                                  RecAmount = (decimal)sord.RecAmount,
                                  PaymentDue = sord.PaymentDue,
                                  CustomerID = sord.CustomerID,
                                  CustomerTotalDue = cus.TotalDue,
                                  EmployeeName = emp.Name,
                                  CustomerType = cus.CustomerType == EnumCustomerType.Dealer
                                  ? cus.CustomerType : EnumCustomerType.Retail

                              }).ToList();
            return oSalesData;
        }


        public static List<ProductWiseSalesReportModel> ProductWiseSalesBenefit(this IBaseRepository<SOrder> SOrderRepository,
               IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
               IBaseRepository<Category> CategoryRepository, IBaseRepository<Product> ProductRepository,
               IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Customer> CustomerRepository, IBaseRepository<ROrder> ROrderRepository,
               IBaseRepository<ROrderDetail> ROrderDetailRepo, IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSODetailsRepo,
               IBaseRepository<CreditSalesSchedule> CSScheduleRepo,
               int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {

            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);


            var SOrderDetails = SOrderDetailRepo.All;
            var SOrders = SOrderRepository.All.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.Status == (int)EnumSalesType.Sales && i.IsReplacement == 0);
            var ROrderDetails = ROrderDetailRepo.All;
            var ROrders = ROrderRepository.All.Where(i => i.ReturnDate >= fromDate && i.ReturnDate <= toDate);
            var CreditSaleDetails = CreditSODetailsRepo.All;
            var CreditSales = CreditSaleRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales);
            var CreditSalesSchedule = CSScheduleRepo.All;


            var result = (from SO in SOrders
                          join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                          join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                          join P in Products on SOD.ProductID equals P.ProductID
                          join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                          join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                          select new ProductWiseSalesReportModel
                          {
                              Date = SO.InvoiceDate,
                              InvoiceNo = SO.InvoiceNo,
                              ProductID = P.ProductID,
                              ProductCode = P.Code,
                              CategoryID = CAT.CategoryID,
                              CompanyID = COM.CompanyID,
                              ProductName = P.ProductName,
                              CategoryName = CAT.Description,
                              CompanyName = COM.Name,
                              Quantity = SOD.Quantity,
                              IMEI = STD.IMENO,
                              SalesTotal = (((SOD.UnitPrice - SOD.PPDAmount) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,
                              Discount = (SOD.PPDAmount + (((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,
                              PurchaseTotal = (SOD.PRate * SOD.Quantity),
                              CommisionProfit = ((((SOD.UnitPrice - SOD.PPDAmount) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (SOD.PRate * SOD.Quantity),
                              HireProfit = 0m,
                              HireCollection = 0m,
                              TotalProfit = ((((SOD.UnitPrice - SOD.PPDAmount) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (SOD.PRate * SOD.Quantity)
                          }).OrderBy(i => i.Date).ToList();

            var returnData = (from SO in ROrders
                              join SOD in ROrderDetails on SO.ROrderID equals SOD.ROrderID
                              join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                              join P in Products on SOD.ProductID equals P.ProductID
                              join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                              join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                              select new ProductWiseSalesReportModel
                              {
                                  Date = SO.ReturnDate,
                                  InvoiceNo = SO.InvoiceNo,
                                  ProductID = P.ProductID,
                                  ProductCode = P.Code,
                                  CategoryID = CAT.CategoryID,
                                  CompanyID = COM.CompanyID,
                                  ProductName = P.ProductName,
                                  CategoryName = CAT.Description,
                                  CompanyName = COM.Name,
                                  Quantity = SOD.Quantity,
                                  IMEI = STD.IMENO,
                                  SalesTotal = (SOD.UnitPrice * SOD.Quantity) * (-1),
                                  Discount = 0m,
                                  PurchaseTotal = (STD.PRate * SOD.Quantity) * (-1),
                                  CommisionProfit = ((SOD.UnitPrice * SOD.Quantity) - (STD.PRate * SOD.Quantity)) * -1,
                                  HireProfit = 0m,
                                  HireCollection = 0m,
                                  TotalProfit = ((SOD.UnitPrice * SOD.Quantity) - (STD.PRate * SOD.Quantity)) * -1
                              }).OrderBy(i => i.Date).ToList();

            var CreditSalesData = (from SO in CreditSales
                                   join SOD in CreditSaleDetails on SO.CreditSalesID equals SOD.CreditSalesID
                                   join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                                   join P in Products on SOD.ProductID equals P.ProductID
                                   join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                                   join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                                   select new ProductWiseSalesReportModel
                                   {
                                       Date = SO.SalesDate,
                                       InvoiceNo = SO.InvoiceNo,
                                       ProductID = P.ProductID,
                                       ProductCode = P.Code,
                                       CategoryID = CAT.CategoryID,
                                       CompanyID = COM.CompanyID,
                                       ProductName = P.ProductName,
                                       CategoryName = CAT.Description,
                                       CompanyName = COM.Name,
                                       Quantity = SOD.Quantity,
                                       IMEI = STD.IMENO,
                                       SalesTotal = (((SOD.UnitPrice + SOD.IntTotalAmt) - ((SO.LastPayAdjAmt + SO.Discount) * (SOD.UnitPrice + SOD.IntTotalAmt) / SO.TSalesAmt)) * SOD.Quantity),
                                       Discount = ((SO.LastPayAdjAmt + SO.Discount) * (SOD.UnitPrice + SOD.IntTotalAmt) / SO.TSalesAmt) * SOD.Quantity,
                                       PurchaseTotal = STD.PRate * SOD.Quantity,
                                       CommisionProfit = (((SOD.UnitPrice) - ((SO.LastPayAdjAmt + SO.Discount) * (SOD.UnitPrice + SOD.IntTotalAmt) / SO.TSalesAmt)) * SOD.Quantity) - (STD.PRate * SOD.Quantity),
                                       HireProfit = (SOD.IntTotalAmt * SOD.Quantity),
                                       HireCollection = 0m,
                                       TotalProfit = 0m
                                   }).OrderBy(i => i.Date).ToList();


            //var CreditCollData = (from SO in CreditSales
            //                      join SOD in CreditSaleDetails on SO.CreditSalesID equals SOD.CreditSalesID
            //                      join CSS in CreditSalesSchedule on SO.CreditSalesID equals CSS.CreditSalesID
            //                      join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
            //                      join P in Products on SOD.ProductID equals P.ProductID
            //                      join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
            //                      join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
            //                      where CSS.PaymentStatus == "Paid"
            //                      select new ProductWiseSalesReportModel
            //                      {
            //                          Date = SO.SalesDate,
            //                          InvoiceNo = SO.InvoiceNo,
            //                          ProductID = P.ProductID,
            //                          ProductCode = P.Code,
            //                          CategoryID = CAT.CategoryID,
            //                          CompanyID = COM.CompanyID,
            //                          ProductName = P.ProductName,
            //                          CategoryName = CAT.Description,
            //                          CompanyName = COM.Name,
            //                          Quantity = SOD.Quantity,
            //                          IMEI = STD.IMENO,
            //                          SalesTotal = 0m,
            //                          Discount = 0m,
            //                          CommisionProfit = 0m,
            //                          HireProfit = 0m,
            //                          HireCollection = 0m,
            //                          TotalProfit = 0m
            //                      }).OrderBy(i => i.Date).ToList();


            result.AddRange(returnData);
            result.AddRange(CreditSalesData);
            //result.AddRange(CreditCollData);


            return result.ToList();
        }


    }
}
