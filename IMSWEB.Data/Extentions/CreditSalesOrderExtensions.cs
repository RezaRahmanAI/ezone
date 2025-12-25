using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CreditSalesOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string, int>>>> GetAllSalesOrderAsync(this IBaseRepository<CreditSale> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, bool IsVATManager,
            int concernID, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo, int page, int pageSize)
        {
            IQueryable<Customer> customers = customerRepository.All.AsNoTracking();
            IQueryable<CreditSale> creditSales = salesOrderRepository.All.AsNoTracking()
                                                .Where(i => i.IsStatus == EnumSalesType.Sales || i.IsStatus == EnumSalesType.Pending);

            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                creditSales = creditSales.Where(i => i.InvoiceNo.Contains(InvoiceNo));
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
                creditSales = creditSales.Where(i => (i.SalesDate >= fromDate && i.SalesDate <= toDate));

            var baseQuery = from so in creditSales
                            join c in customers on so.CustomerID equals c.CustomerID
                            select new ProductWiseSalesReportModel
                            {
                                SOrderID = so.CreditSalesID,
                                InvoiceNo = so.InvoiceNo,
                                Date = so.SalesDate,
                                CustomerCode = c.Code,
                                CustomerName = c.Name,
                                Mobile = c.ContactNo,
                                PaymentDue = so.Remaining,
                                IsStatus = so.IsStatus,
                                TotalAmount = so.NetAmount,
                                IsReturn = so.IsReturn
                            };

            var orderedQuery = baseQuery.OrderByDescending(i => i.Date).ThenByDescending(i => i.InvoiceNo);

            if (!IsVATManager)
            {
                var pagedItems = await orderedQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                return pagedItems.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>
                    (
                        x.SOrderID,
                        x.InvoiceNo,
                        x.Date,
                        x.CustomerName,
                        x.Mobile,
                        x.PaymentDue,
                        x.IsStatus,
                        new Tuple<string, int>
                        (x.CustomerCode, x.IsReturn)
                    )).ToList();
            }

            var oConcern = await SisterConcernRepository.All.AsNoTracking()
                .FirstOrDefaultAsync(i => i.ConcernID == concernID);
            var totalAmount = await baseQuery.SumAsync(i => (decimal?)i.TotalAmount) ?? 0m;
            var salesPercent = oConcern?.SalesShowPercent ?? 100m;
            var falesSales = (totalAmount * salesPercent) / 100m;
            var falesSalesCount = 0m;
            var pagedData = new List<ProductWiseSalesReportModel>();
            var start = (page - 1) * pageSize;
            var filteredIndex = 0;

            foreach (var item in orderedQuery.AsEnumerable())
            {
                falesSalesCount += item.TotalAmount;
                if (falesSalesCount > falesSales)
                {
                    break;
                }

                if (filteredIndex >= start && pagedData.Count < pageSize)
                {
                    pagedData.Add(item);
                }

                filteredIndex++;
                if (filteredIndex >= start + pageSize)
                {
                    break;
                }
            }

            return pagedData.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.PaymentDue,
                    x.IsStatus,
                    new Tuple<string, int>
                    (x.CustomerCode, x.IsReturn)
                )).ToList();
        }

        public static IEnumerable<Tuple<int, int, int, int,
            decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string, decimal>>> GetCustomSalesOrderDetails(this IBaseRepository<CreditSaleDetails> saleOrderDetailsRepository,
            int orderId, IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> stockDetails = stockDetailRepository.All;

            var items = saleOrderDetailsRepository.FindBy(x => x.CreditSalesID == orderId)
                .GroupJoin(stockDetails, s => s.StockDetailID, d => d.SDetailID, (s, d) => new { SaleDetails = s, Details = d })
                .SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SaleDetails = s.SaleDetails, Details = d })
                .GroupJoin(products, d => d.Details.ProductID, p => p.ProductID,
                (d, p) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (d, p) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = p })
                .GroupJoin(colors, d => d.Details.ColorID, c => c.ColorID,
                (d, c) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = d.Products, Color = c })
                .Select(x => new
                {
                    x.SaleDetails.CreditSalesID,
                    x.SaleDetails.CreditSaleDetailsID,
                    x.SaleDetails.ProductID,
                    x.SaleDetails.StockDetailID,
                    x.SaleDetails.Quantity,
                    x.SaleDetails.MPRate,
                    x.SaleDetails.UnitPrice,
                    x.SaleDetails.UTAmount,
                    x.Products.ProductName,
                    x.Details.IMENO,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    x.SaleDetails.IntTotalAmt
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, int, decimal, decimal, decimal,
                Tuple<decimal, string, string, int, string, decimal>>
                (
                    x.CreditSalesID,
                    x.CreditSaleDetailsID,
                    x.ProductID,
                    x.StockDetailID,
                    x.Quantity,
                    x.MPRate,
                    x.UnitPrice,
                    new Tuple<decimal, string, string, int, string, decimal>(x.UTAmount, x.ProductName, x.IMENO, x.ColorId, x.ColorName, x.IntTotalAmt)
                ));
        }


        /// <summary>
        /// Update Date: 25-12-19
        /// Author: aminul
        /// Reason: Get all the schedules by the day and month of the date range.
        /// </summary>
        public static IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(this IBaseRepository<CreditSale> CreditSOrderRepository,
             IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
             IBaseRepository<Product> productRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailRepository,
             IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<CashCollection> CashRepo,
             DateTime fromDate, DateTime toDate, EnumCustomerType customerType, int EmployeeID)
        {

            List<UpcommingScheduleReport> UpCommings = new List<UpcommingScheduleReport>();
            IQueryable<Customer> customers = null;
            IQueryable<Employee> employees = null;
            if (customerType != 0)
                customers = customerRepository.All.Where(i => i.CustomerType == customerType);
            else
                customers = customerRepository.All;

            if (EmployeeID != 0)
                employees = EmployeeRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                employees = EmployeeRepository.All;

            #region Credit Sales

            var oCSalesDetails = (from cs in CreditSOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales)
                                  join csd in creditScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                  join cus in customers on cs.CustomerID equals cus.CustomerID
                                  join emp in employees on cus.EmployeeID equals emp.EmployeeID
                                  where
                                  ((csd.MonthDate >= fromDate && csd.MonthDate >= fromDate)
                                  && (csd.MonthDate <= toDate && csd.MonthDate <= toDate)
                                  && csd.PaymentStatus == "Due" && csd.RemindDate == null)
                                  select new
                                  {
                                      EmployeeID = cus.EmployeeID,
                                      EmployeeName = emp.Name,
                                      Name = cus.Name,
                                      cs.InvoiceNo,
                                      cs.CreditSalesID,
                                      cus.Code,
                                      cus.CustomerID,

                                      RefName = cus.RefName + "," + cus.RefContact + "/" + cs.GuarName + "," + cs.GuarContactNo,
                                      cus.ContactNo,
                                      cus.Address,
                                      cus.CreditDue,
                                      cs.SalesDate,
                                      PaymentDate = csd.MonthDate,
                                      cs.TSalesAmt,
                                      cs.NetAmount,
                                      cs.FixedAmt,
                                      cs.InstallmentPeriod,
                                      cs.NoOfInstallment,
                                      Remaining = cs.TSalesAmt - cs.DownPayment,
                                      csd.PaymentStatus,
                                      csd.Remarks,
                                      cs.DownPayment,
                                      InsRec = cs.TSalesAmt - cs.DownPayment - cs.Remaining,
                                      TotalRec = cs.TSalesAmt - cs.Remaining,
                                      closing = cs.Remaining,
                                      InstallmentAmt = csd.InstallmentAmt,
                                      csd.RemindDate,
                                      csd.CSScheduleID
                                  }).ToList();

            var oCSalesDetailsForRemindDate = (from cs in CreditSOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales)
                                               join csd in creditScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                               join cus in customers on cs.CustomerID equals cus.CustomerID
                                               join emp in employees on cus.EmployeeID equals emp.EmployeeID
                                               where
                                               //((csd.RemindDate.Value.Day >= fromDate.Day && csd.RemindDate.Value.Month >= fromDate.Month)
                                               // && (csd.RemindDate.Value.Day <= toDate.Day && csd.RemindDate.Value.Month <= toDate.Month)
                                               ((csd.RemindDate >= fromDate && csd.RemindDate >= fromDate)
                                                && (csd.RemindDate <= toDate && csd.RemindDate <= toDate)
                                                && csd.PaymentStatus == "Due" && csd != null && csd.RemindDate != null)
                                               select new
                                               {
                                                   EmployeeID = cus.EmployeeID,
                                                   EmployeeName = emp.Name,
                                                   Name = cus.Name,
                                                   cs.InvoiceNo,
                                                   cs.CreditSalesID,
                                                   cus.Code,
                                                   cus.CustomerID,

                                                   cus.RefName,
                                                   cus.ContactNo,
                                                   cus.Address,
                                                   cus.CreditDue,
                                                   cs.SalesDate,
                                                   PaymentDate = (DateTime)csd.MonthDate,
                                                   cs.TSalesAmt,
                                                   cs.NetAmount,
                                                   cs.FixedAmt,
                                                   cs.InstallmentPeriod,
                                                   cs.NoOfInstallment,
                                                   Remaining = cs.TSalesAmt - cs.DownPayment,
                                                   csd.PaymentStatus,
                                                   csd.Remarks,
                                                   cs.DownPayment,
                                                   InsRec = cs.TSalesAmt - cs.DownPayment - cs.Remaining,
                                                   TotalRec = cs.TSalesAmt - cs.Remaining,
                                                   closing = cs.Remaining,
                                                   InstallmentAmt = csd.InstallmentAmt,
                                                   csd.RemindDate,
                                                   csd.CSScheduleID
                                               }).ToList();

            oCSalesDetails.AddRange(oCSalesDetailsForRemindDate);
            var oCSalesDetailsSorting = oCSalesDetails.OrderByDescending(o => o.PaymentDate);

            //var oCSalesDetailsGroup = (from csd in oCSalesDetailsSorting
            //                           group csd by new
            //                           {
            //                               csd.EmployeeName,
            //                               csd.EmployeeID,

            //                               csd.Code,
            //                               csd.Name,
            //                               csd.CustomerID,
            //                               csd.RefName,
            //                               csd.ContactNo,
            //                               csd.Address,
            //                               csd.CreditSalesID,
            //                               csd.SalesDate,
            //                               csd.InvoiceNo,
            //                               csd.TSalesAmt,
            //                               csd.DownPayment,
            //                               csd.NetAmount,
            //                               csd.FixedAmt,
            //                               csd.InstallmentPeriod,
            //                               csd.NoOfInstallment,
            //                               csd.Remaining, // Remaining after Downpayment
            //                               csd.Remarks,
            //                               csd.closing,  // remainig field
            //                               csd.InsRec,
            //                               csd.TotalRec,
            //                               csd.CreditDue
            //                           } into g
            //                           select new
            //                           {
            //                               g.Key.EmployeeName,
            //                               g.Key.EmployeeID,

            //                               g.Key.Code,
            //                               g.Key.Name,
            //                               g.Key.CustomerID,
            //                               g.Key.DownPayment,
            //                               g.Key.InvoiceNo,
            //                               g.Key.RefName,
            //                               g.Key.ContactNo,
            //                               g.Key.Address,
            //                               g.Key.SalesDate,
            //                               g.Key.CreditSalesID,
            //                               g.Key.TSalesAmt,
            //                               g.Key.Remaining,
            //                               g.Key.NetAmount,
            //                               g.Key.FixedAmt,
            //                               g.Key.InstallmentPeriod,
            //                               g.Key.NoOfInstallment,
            //                               g.Key.Remarks,
            //                               g.Key.closing,  // remainig field
            //                               g.Key.InsRec,
            //                               g.Key.TotalRec,
            //                               g.Key.CreditDue,
            //                               TotalPaymentDue = g.Sum(o => o.InstallmentAmt),
            //                               PaymentDate = g.Select(o => o.PaymentDate).FirstOrDefault()
            //                           }
            //                          );

            //var oCSDs = oCSalesDetailsGroup.ToList();
            UpcommingScheduleReport oUpcomming = null;
            foreach (var item in oCSalesDetailsSorting)
            {
                oUpcomming = new UpcommingScheduleReport();
                decimal defaultInstallment = 0m;
                var deafult = oCSalesDetails.Where(o => o.PaymentDate < toDate && o.RemindDate == null && o.CustomerID == item.CustomerID);
                if (deafult.ToList().Count != 0)
                    defaultInstallment = deafult.Sum(o => o.InstallmentAmt);
                var deafultForRemind = oCSalesDetails.Where(o => o.RemindDate < fromDate && o.RemindDate != null && o.CustomerID == item.CustomerID);
                if (deafultForRemind.ToList().Count != 0)
                    defaultInstallment = defaultInstallment + deafultForRemind.Sum(o => o.InstallmentAmt);

                DateTime? RemindDate = oCSalesDetails.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate && o.CustomerID == item.CustomerID).Select(o => o.RemindDate).FirstOrDefault();
                var oCSP = (from CSP in creditSalesDetailRepository.All
                            join P in productRepository.All on CSP.ProductID equals P.ProductID
                            where (CSP.CreditSalesID == item.CreditSalesID)
                            select new
                            {
                                P.ProductName
                            });

                oUpcomming.EmployeeID = item.EmployeeID;
                oUpcomming.EmployeeName = item.EmployeeName;
                oUpcomming.InvoiceNo = item.InvoiceNo;
                oUpcomming.CustomerCode = item.Code;
                oUpcomming.CustomerName = item.Name;
                oUpcomming.CustomerRefName = item.RefName;
                oUpcomming.CustomerConctact = item.ContactNo;
                oUpcomming.CustomerAddress = item.Address;
                oUpcomming.ProductName = oCSP.Select(i => i.ProductName).ToList();
                oUpcomming.SalesDate = item.SalesDate;
                oUpcomming.PaymentDate = item.PaymentDate;
                oUpcomming.TSalesAmt = item.TSalesAmt;
                oUpcomming.NetAmount = item.NetAmount;
                oUpcomming.SalesPrice = item.NetAmount;
                oUpcomming.FixedAmt = item.FixedAmt == null ? 0m : (decimal)item.FixedAmt;
                oUpcomming.InstallmentPeriod = item.InstallmentPeriod;
                oUpcomming.NoOfInstallment = item.NoOfInstallment;
                oUpcomming.Remaining = item.Remaining;
                oUpcomming.TotalPaymentDue = item.InstallmentAmt; //Math.Round((decimal)item.TotalPaymentDue, 0);
                oUpcomming.Remarks = item.Remarks;
                oUpcomming.DownPayment = item.DownPayment;
                oUpcomming.Remaining = item.closing;
                oUpcomming.RemaindDate = item.PaymentDate;//RemindDate;
                oUpcomming.TotalInstCollectionAmt = item.InsRec;
                oUpcomming.CreditDue = item.CreditDue;
                oUpcomming.DefaultAmount = Math.Round((decimal)defaultInstallment, 0);
                oUpcomming.CustomerID = item.CustomerID;
                oUpcomming.CSScheduleID = item.CSScheduleID;
                UpCommings.Add(oUpcomming);

            }
            #endregion

            #region CashSales Order
            var CashSOrdersSchedules = (from c in customers
                                        join emp in employees on c.EmployeeID equals emp.EmployeeID
                                        join so in SOrderRepository.All on c.CustomerID equals so.CustomerID
                                        join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                        join p in productRepository.All on sod.ProductID equals p.ProductID
                                        where so.Status == (int)EnumSalesType.Sales && c.TotalDue != 0
                                        && ((c.RemindDate.Value.Day >= fromDate.Day && c.RemindDate.Value.Month >= fromDate.Month)
                                             && (c.RemindDate.Value.Day <= toDate.Day && c.RemindDate.Value.Month <= toDate.Month)
                                            )
                                        select new
                                        {
                                            EmployeeID = c.EmployeeID,
                                            EmployeeName = emp.Name,
                                            c.CustomerID,
                                            c.Code,
                                            c.Name,
                                            c.Address,
                                            c.ContactNo,
                                            c.TotalDue,
                                            c.RefName,
                                            c.RemindDate,
                                            c.RefContact,
                                            c.RefAddress,
                                            so.SOrderID,
                                            so.InvoiceDate,
                                            so.InvoiceNo,
                                            so.TotalAmount,
                                            so.RecAmount,
                                            p.ProductName
                                        }).OrderByDescending(i => i.InvoiceDate);

            var CashRemindOrders = (from so in CashSOrdersSchedules
                                    group so by new
                                    {
                                        so.EmployeeID,
                                        so.EmployeeName,
                                        so.CustomerID,
                                        so.Code,
                                        so.Name,
                                        so.Address,
                                        so.ContactNo,
                                        so.TotalAmount,
                                        so.RecAmount,
                                        so.TotalDue,
                                        so.RefName,
                                        so.RefContact,
                                        so.RefAddress,
                                        so.RemindDate,
                                        so.SOrderID,
                                        so.InvoiceDate,
                                        so.InvoiceNo
                                    } into g
                                    select new
                                    {
                                        g.Key.EmployeeName,
                                        g.Key.EmployeeID,
                                        g.Key.CustomerID,
                                        g.Key.Name,
                                        g.Key.Code,
                                        g.Key.Address,
                                        g.Key.ContactNo,
                                        g.Key.TotalDue,
                                        g.Key.TotalAmount,
                                        g.Key.RecAmount,
                                        g.Key.RefName,
                                        g.Key.RefContact,
                                        g.Key.RemindDate,
                                        g.Key.RefAddress,
                                        SalesDate = g.Key.InvoiceDate,
                                        InvoiceNo = g.Key.InvoiceNo,
                                        ProductName = g.Select(i => i.ProductName).ToList()
                                    }).OrderByDescending(i => i.SalesDate).ToList();

            var FinalCashRemindOrders = (from so in CashRemindOrders
                                         group so by new
                                         {
                                             so.EmployeeID,
                                             so.EmployeeName,
                                             so.CustomerID,
                                             so.Code,
                                             so.Name,
                                             so.Address,
                                             so.ContactNo,
                                             so.TotalDue,
                                             so.TotalAmount,
                                             so.RecAmount,
                                             so.RefName,
                                             so.RefContact,
                                             so.RefAddress,
                                             so.RemindDate
                                         } into g
                                         select new UpcommingScheduleReport
                                         {
                                             EmployeeID = g.Key.EmployeeID,
                                             EmployeeName = g.Key.EmployeeName,
                                             CustomerID = g.Key.CustomerID,
                                             CustomerName = g.Key.Name,
                                             CustomerCode = g.Key.Code,
                                             CustomerAddress = g.Key.Address,
                                             CustomerConctact = g.Key.ContactNo,
                                             TotalPaymentDue = g.Key.TotalDue,
                                             CreditDue = g.Key.TotalDue,
                                             DownPayment = (decimal)g.Key.RecAmount,
                                             SalesPrice = g.Key.TotalAmount,
                                             CustomerRefName = g.Key.RefName,
                                             CustomerRefContact = g.Key.RefContact,
                                             RemaindDate = g.Key.RemindDate,
                                             SalesDate = g.Select(i => i.SalesDate).FirstOrDefault(),
                                             InvoiceNo = g.Select(i => i.InvoiceNo).FirstOrDefault(),
                                             ProductName = g.Select(i => i.ProductName).FirstOrDefault(),
                                             InstallmentPeriod = "Cash Sales"
                                         }).OrderByDescending(i => i.SalesDate);

            #endregion

            #region Cash Collection
            var Cash = (from c in customers
                                        join emp in employees on c.EmployeeID equals emp.EmployeeID
                                        join cc in  CashRepo.All on c.CustomerID equals cc.CustomerID
                                        where cc.TransactionType == EnumTranType.FromCustomer 
                                        && ((c.RemindDate.Value.Day >= fromDate.Day && c.RemindDate.Value.Month >= fromDate.Month)
                                             && (c.RemindDate.Value.Day <= toDate.Day && c.RemindDate.Value.Month <= toDate.Month)
                                            )
                                        select new
                                        {
                                            EmployeeID = c.EmployeeID,
                                            EmployeeName = emp.Name,
                                            c.CustomerID,
                                            c.Code,
                                            c.Name,
                                            c.Address,
                                            c.ContactNo,
                                            c.TotalDue,
                                            c.RefName,
                                            c.RemindDate,
                                            c.RefContact,
                                            c.RefAddress,
                                            cc.CashCollectionID,
                                            cc.EntryDate,
                                            cc.ReceiptNo,
                                            cc.Amount
                                                                                                
                                        }).OrderByDescending(i => i.EntryDate);

            var CashC = (from so in Cash
                         group so by new
                                    {
                                        so.EmployeeID,
                                        so.EmployeeName,
                                        so.CustomerID,
                                        so.Code,
                                        so.Name,
                                        so.Address,
                                        so.ContactNo,
                                        so.Amount,
                                        
                                        so.TotalDue,
                                        so.RefName,
                                        so.RefContact,
                                        so.RefAddress,
                                        so.RemindDate,
                                        so.CashCollectionID,
                                        so.EntryDate,
                                        so.ReceiptNo
                                    } into g
                                    select new
                                    {
                                        g.Key.EmployeeName,
                                        g.Key.EmployeeID,
                                        g.Key.CustomerID,
                                        g.Key.Name,
                                        g.Key.Code,
                                        g.Key.Address,
                                        g.Key.ContactNo,
                                        g.Key.TotalDue,
                                        g.Key.Amount,
                                       
                                        g.Key.RefName,
                                        g.Key.RefContact,
                                        g.Key.RemindDate,
                                        g.Key.RefAddress,
                                        SalesDate = g.Key.EntryDate,
                                        InvoiceNo = g.Key.ReceiptNo
                                       
                                    }).OrderByDescending(i => i.SalesDate).ToList();

            var FinalCashC = (from so in CashC
                              group so by new
                                         {
                                             so.EmployeeID,
                                             so.EmployeeName,
                                             so.CustomerID,
                                             so.Code,
                                             so.Name,
                                             so.Address,
                                             so.ContactNo,
                                             so.TotalDue,
                                             so.Amount,
                                             
                                             so.RefName,
                                             so.RefContact,
                                             so.RefAddress,
                                             so.RemindDate
                                         } into g
                                         select new UpcommingScheduleReport
                                         {
                                             EmployeeID = g.Key.EmployeeID,
                                             EmployeeName = g.Key.EmployeeName,
                                             CustomerID = g.Key.CustomerID,
                                             CustomerName = g.Key.Name,
                                             CustomerCode = g.Key.Code,
                                             CustomerAddress = g.Key.Address,
                                             CustomerConctact = g.Key.ContactNo,
                                             TotalPaymentDue = g.Key.TotalDue,
                                             CreditDue = g.Key.TotalDue,
                                             DownPayment = (decimal)g.Key.Amount,
                                             SalesPrice = g.Key.Amount,

                                             CustomerRefName = g.Key.RefName,
                                             CustomerRefContact = g.Key.RefContact,
                                             RemaindDate = g.Key.RemindDate,
                                             SalesDate = g.Select(i => i.SalesDate).FirstOrDefault(),
                                             InvoiceNo = g.Select(i => i.InvoiceNo).FirstOrDefault(),
                                             ProductName = new List<string> {"Not available"},
                                             InstallmentPeriod = "Cash Collection"
                                         }).OrderByDescending(i => i.SalesDate);

            #endregion

            UpCommings.AddRange(FinalCashRemindOrders);
            UpCommings.AddRange(FinalCashC);
            return UpCommings;
        }


        public static IEnumerable<UpcommingScheduleReport> GetScheduleCollection(this IBaseRepository<CreditSale> salesOrderRepository,
          IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
          IBaseRepository<Employee> employeeRepository, IBaseRepository<SisterConcern> sisterConcernRepository,
          DateTime fromDate, DateTime toDate, int concernID, int EmployeeID, bool IsAdminReport)
        {
            IQueryable<CreditSale> creditSales = null;
            IQueryable<Customer> customers = null;
            IQueryable<Employee> employees = null;
            IQueryable<SisterConcern> sisterConcerns = null;

            if (IsAdminReport)
            {
                if (concernID > 0)
                {
                    customers = customerRepository.GetAll().Where(i => i.ConcernID == concernID);
                    employees = employeeRepository.GetAll().Where(i => i.ConcernID == concernID);
                    creditSales = salesOrderRepository.GetAll().Where(i => i.ConcernID == concernID);
                    sisterConcerns = sisterConcernRepository.GetAll().Where(i => i.ConcernID == concernID);
                }
                else
                {
                    customers = customerRepository.GetAll();
                    employees = employeeRepository.GetAll();
                    creditSales = salesOrderRepository.GetAll();
                    creditSales = salesOrderRepository.GetAll();
                    sisterConcerns = sisterConcernRepository.GetAll();
                }
            }
            else
            {
                if (EmployeeID > 0)
                {
                    customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
                    employees = employeeRepository.All.Where(i => i.EmployeeID == EmployeeID);
                }
                else
                {

                    customers = customerRepository.All;
                    employees = employeeRepository.All;
                }

                sisterConcerns = sisterConcernRepository.All;
                creditSales = salesOrderRepository.All;
            }

            var items = (from cs in creditSales
                         join csd in creditScheduleRepository.GetAll() on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customers on cs.CustomerID equals cus.CustomerID
                         join emp in employees on cus.EmployeeID equals emp.EmployeeID
                         join sis in sisterConcerns on cs.ConcernID equals sis.ConcernID
                         where ((csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) || ((csd.MonthDate >= fromDate && csd.MonthDate <= toDate))
                         && (csd.InstallmentAmt != 0 && cs.IsStatus == EnumSalesType.Sales))
                         select new UpcommingScheduleReport
                         {
                             CustomerCode = cus.Code,
                             InvoiceNo = cs.InvoiceNo,
                             CustomerName = cus.Name,
                             EmployeeName = emp.Name,
                             EmployeeID = emp.EmployeeID,
                             CustomerConctact = cus.ContactNo,
                             CustomerAddress = cus.Address,
                             // pro.ProductName,
                             SalesDate = cs.SalesDate,
                             PaymentDate = csd.PaymentDate,
                             ScheduleDate = csd.MonthDate,
                             TSalesAmt = cs.TSalesAmt,
                             NetAmount = cs.NetAmount,
                             FixedAmt = (decimal)cs.FixedAmt,
                             Remaining = cs.Remaining,
                             InstallmentAmount = csd.InstallmentAmt,
                             Remarks = csd.Remarks,
                             DownPayment = cs.DownPayment,
                             InstallmentPeriod = cs.InstallmentPeriod,
                             PenaltyInterest = cs.PenaltyInterest,
                             SalesPrice = cs.TSalesAmt,
                             NoOfInstallment = cs.NoOfInstallment,
                             NoOfRemainingInstallment = cs.NoOfInstallment - (int)csd.ScheduleNo,
                             ExpectedInstallment = csd.ExpectedInstallment,
                             PaymentStatus = csd.PaymentStatus,
                             ConcernName = sis.Name
                         }).OrderBy(i => i.InstallmentPeriod).ToList();
            return items;
        }

        /// <summary>
        /// Defaulting Customer Summary
        /// </summary>
        /// <param name="salesOrderRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="creditScheduleRepository"></param>
        /// <param name="date"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(this IBaseRepository<CreditSale> salesOrderRepository,
         IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository, DateTime date, int concernID)
        {
            var items = (from o in salesOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.Remaining > 0)
                         join od in creditScheduleRepository.All on o.CreditSalesID equals od.CreditSalesID
                         join c in customerRepository.All on o.CustomerID equals c.CustomerID
                         where (od.MonthDate <= date && od.PaymentStatus == "Due")
                         group od by new { o.CustomerID, c.Code, c.Name, c.ContactNo, c.Address } into g //c.CompanyName
                         select new
                         {
                             code = g.Key.Code,
                             name = g.Key.Name, //g.Key.CompanyName
                             contact = g.Key.ContactNo + " & " + g.Key.Address,
                             count = g.Count(),
                             amount = g.Sum(od => od.InstallmentAmt)
                         }).ToList();
            return items.Select(x => new Tuple<string, string, string, decimal, decimal>
                (
                    x.code,
                    x.name,
                    x.contact,
                    x.count,
                    x.amount
                ));
        }

        /// <summary>
        /// Defaulting customer Details
        /// </summary>
        /// <param name="salesOrderRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="creditScheduleRepository"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>>
            GetDefaultingCustomer(this IBaseRepository<CreditSale> salesOrderRepository,
                                 IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
                                 DateTime fromDate, DateTime toDate, int concernID)
        {
            var CreditSchedules = creditScheduleRepository.All;
            var items = (from cs in salesOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.Remaining > 0)
                         join csd in CreditSchedules on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         where (cs.SalesDate >= fromDate && cs.SalesDate <= toDate) && csd.PaymentStatus == "Due" && cs.ConcernID == concernID && csd.MonthDate <= DateTime.Today
                         select new
                         {
                             cus.CustomerID,
                             cus.Code,
                             cs.CreditSalesID,
                             cs.InvoiceNo,
                             cus.Name,
                             ContactNo = cus.ContactNo + " & " + cus.Address,
                             // pro.ProductName,
                             cs.SalesDate,
                             csd.MonthDate,
                             cs.TSalesAmt,
                             cs.NetAmount,
                             cs.FixedAmt,
                             cs.Remaining,
                             csd.InstallmentAmt,
                             csd.Remarks,
                             cs.DownPayment,
                             cs.PenaltyInterest
                         }).ToList();

            var Result = (from item in items
                          group item by new { item.CustomerID, item.Code, item.Name, item.ContactNo, item.CreditSalesID, item.TSalesAmt, item.NetAmount, item.FixedAmt, item.Remaining, item.DownPayment, item.PenaltyInterest } into g
                          select new
                          {
                              g.Key.CustomerID,
                              g.Key.Code,
                              g.LastOrDefault().InvoiceNo,
                              g.Key.Name,
                              g.Key.ContactNo,
                              g.LastOrDefault().SalesDate,
                              g.LastOrDefault().MonthDate,
                              TSalesAmt = g.Key.TSalesAmt,
                              NetAmount = g.Key.NetAmount,
                              FixedAmt = g.Key.FixedAmt,
                              Remaining = g.Key.Remaining,
                              DefaultAmt = g.Sum(i => i.InstallmentAmt),
                              g.LastOrDefault().Remarks,
                              DownPayment = g.Key.DownPayment,
                              PenaltyInterest = g.Key.PenaltyInterest,
                              g.Key.CreditSalesID,
                              TotalReceiveAmt = CreditSchedules.Where(i => i.CreditSalesID == g.Key.CreditSalesID && i.PaymentStatus == "Paid").Count() == 0 ? g.Key.DownPayment : CreditSchedules.Where(i => i.CreditSalesID == g.Key.CreditSalesID && i.PaymentStatus == "Paid").Sum(j => j.InstallmentAmt) + g.Key.DownPayment,
                          }).Where(i => i.DefaultAmt > 0);

            var Result2 = (from item in Result
                           group item by new { item.CustomerID, item.Code, item.Name, item.ContactNo } into g
                           select new
                           {
                               g.Key.Code,
                               g.LastOrDefault().InvoiceNo,
                               g.Key.Name,
                               g.Key.ContactNo,
                               g.LastOrDefault().SalesDate,
                               g.LastOrDefault().MonthDate,
                               TSalesAmt = g.Sum(i => i.TSalesAmt),
                               NetAmount = g.Sum(i => i.NetAmount),
                               FixedAmt = g.Sum(i => i.FixedAmt),
                               Remaining = g.Sum(i => i.Remaining),
                               DefaultAmt = g.Sum(i => i.DefaultAmt),
                               g.LastOrDefault().Remarks,
                               DownPayment = g.Sum(i => i.DownPayment),
                               PenaltyInterest = g.Sum(i => i.PenaltyInterest),
                               g.LastOrDefault().CreditSalesID,
                               TotalReceiveAmt = Convert.ToDecimal(g.Sum(i => i.TotalReceiveAmt)),
                           }).Where(i => i.DefaultAmt > 0);

            return Result2.Select(x => new Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>
                (
                    x.Code,
                    x.InvoiceNo,
                    x.Name,
                    x.ContactNo,
                    x.SalesDate,
                    x.MonthDate,
                    x.TSalesAmt,
                    new Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>
                    (
                    x.NetAmount,
                    (decimal)x.FixedAmt,
                    x.Remaining,
                    x.DefaultAmt,
                    x.Remarks,
                    x.DownPayment,
                    x.PenaltyInterest,
                    new Tuple<int, decimal>(x.CreditSalesID, x.TotalReceiveAmt)
                    )
                ));
        }


        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, int, string, Tuple<string, string>>>>
           GetCreditSalesReportByConcernID(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
            DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            IQueryable<Customer> Customers = customerRepository.All;

            var oSalesData = (from SO in CreditSaleRepository.All
                              join SOD in CreditSaleDetailsRepository.All on SO.CreditSalesID equals SOD.CreditSalesID
                              join cus in Customers on SO.CustomerID equals cus.CustomerID
                              where (SO.SalesDate >= fromDate && SO.SalesDate <= toDate && SO.IsStatus == EnumSalesType.Sales && SO.ConcernID == concernID )
                              group SO by new
                              {
                                  cus.CustomerID,
                                  cus.Code,
                                  cus.Name,
                                  cus.Address,
                                  cus.ContactNo,
                                  cus.TotalDue,
                                  SO.InvoiceNo,
                                  SO.SalesDate,
                                  SO.TSalesAmt,
                                  SO.Discount,
                                  SO.NetAmount,
                                  SO.DownPayment,
                                  Remaining = SO.NetAmount - SO.DownPayment,
                                  SO.InstallmentPeriod
                              } into g
                              select new
                              {
                                  g.Key.Code,
                                  g.Key.Name,
                                  g.Key.Address,
                                  g.Key.ContactNo,
                                  g.Key.SalesDate,
                                  g.Key.InvoiceNo,
                                  g.Key.TSalesAmt,
                                  g.Key.Discount,
                                  g.Key.NetAmount,
                                  g.Key.DownPayment,
                                  g.Key.Remaining,
                                  TotalOffer = g.Select(i => i.CreditSaleDetails).FirstOrDefault(),
                                  g.Key.TotalDue,
                                  g.Key.CustomerID,
                                  g.Key.InstallmentPeriod
                              }).ToList();

            return oSalesData.Select(x => new Tuple<string, string, DateTime, string, decimal, decimal, decimal,
                Tuple<decimal, decimal, decimal, decimal, decimal, int, string, Tuple<string, string>>>
                (
                 x.Code,
                 x.Name,
                x.SalesDate,
                x.InvoiceNo,
                x.TSalesAmt,
                x.Discount,
                x.NetAmount,
                    new Tuple<decimal, decimal, decimal, decimal, decimal, int, string, Tuple<string, string>>(
                        (decimal)x.DownPayment,
                        x.Remaining,
                        0m,
                        x.TotalOffer.Sum(i => i.PPOffer),
                        x.TotalDue,
                        x.CustomerID,
                        x.InstallmentPeriod,
                        new Tuple<string, string>(x.Address, x.ContactNo)
                )

                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, int, string, string, int, int, string>>>>
          GetCreditSalesDetailReportByConcernID(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                    IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockdetailRepository,
                    IBaseRepository<Customer> customeRepository, IBaseRepository<Category> categoryRepository,
                    IBaseRepository<SisterConcern> SisterConcernRepository,
                    DateTime fromDate, DateTime toDate, int concernID, bool IsAdminReport)
        {
            IQueryable<CreditSale> creditSales = null;
            IQueryable<Customer> customers = null;
            IQueryable<Product> products = null;
            IQueryable<Category> categories = null;
            IQueryable<SisterConcern> concerns = null;
            if (IsAdminReport)
            {
                if (concernID > 0)
                {
                    creditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == concernID);
                    products = productRepository.GetAll().Where(i => i.ConcernID == concernID);
                    categories = categoryRepository.GetAll().Where(i => i.ConcernID == concernID);
                    concerns = SisterConcernRepository.GetAll().Where(i => i.ConcernID == concernID);
                    customers = customeRepository.GetAll().Where(i => i.ConcernID == concernID);

                }
                else
                {
                    creditSales = CreditSaleRepository.GetAll();
                    products = productRepository.GetAll();
                    categories = categoryRepository.GetAll();
                    concerns = SisterConcernRepository.GetAll();
                    customers = customeRepository.GetAll();
                }
            }
            else
            {
                if (concernID > 0)
                {
                    creditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == concernID);
                    products = productRepository.GetAll().Where(i => i.ConcernID == concernID);
                    categories = categoryRepository.GetAll().Where(i => i.ConcernID == concernID);
                    concerns = SisterConcernRepository.GetAll().Where(i => i.ConcernID == concernID);
                    customers = customeRepository.GetAll().Where(i => i.ConcernID == concernID);

                }
                else
                {
                    creditSales = CreditSaleRepository.All;
                    products = productRepository.All;
                    categories = categoryRepository.All;
                    concerns = SisterConcernRepository.All;
                    customers = customeRepository.All;
                }
                //creditSales = CreditSaleRepository.All;
                //products = productRepository.All;
                //categories = categoryRepository.All;
                //concerns = SisterConcernRepository.All;
                //customers = customeRepository.All;
            }
            var oSalesDetailData = (from SOD in CreditSaleDetailsRepository.All
                                    join SO in creditSales on SOD.CreditSalesID equals SO.CreditSalesID
                                    join sis in concerns on SO.ConcernID equals sis.ConcernID
                                    join CUS in customers on SO.CustomerID equals CUS.CustomerID
                                    join P in products on SOD.ProductID equals P.ProductID
                                    join CAT in categories on P.CategoryID equals CAT.CategoryID
                                    join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                    where (SO.SalesDate >= fromDate && SO.SalesDate <= toDate && SO.IsStatus == EnumSalesType.Sales)
                                    select new ProductWiseSalesReportModel
                                    {
                                        SOrderID = SO.CreditSalesID,
                                        InvoiceNo = "H-" + SO.InvoiceNo,
                                        CustomerName = SO.Customer.Name,
                                        Date = SO.SalesDate,
                                        GrandTotal = SO.TSalesAmt,
                                        NetDiscount = SO.Discount,
                                        TotalAmount = SO.NetAmount,
                                        RecAmount = SO.DownPayment,                                   
                                        PaymentDue = SO.NetAmount - SO.DownPayment,
                                        //PaymentDue = SO.Remaining,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.UnitPrice,
                                        UTAmount = SOD.UTAmount,
                                        PPOffer = SOD.PPOffer,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        ColorName = std.Color.Name,
                                        CustomerType = (int)EnumCustomerType.Hire,
                                        CategoryName = CAT.Description,
                                        CategoryID = CAT.CategoryID,
                                        PCategoryID = CAT.PCategoryID,
                                        ConcernName = sis.Name
                                    }).OrderByDescending(i => i.Date).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal,
                Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                Tuple<int, int, string, string, int, int, string>>>
                (
                 x.Date,
                 x.InvoiceNo,
                 x.ProductName,
                 x.CompanyName,
                 x.UnitPrice,
                 //x.PPDAmount,
                 x.UTAmount,// - x.PPOffer,
                 x.GrandTotal,
                 new Tuple<decimal, decimal, decimal, decimal, decimal, string, string,
                 Tuple<int, int, string, string, int, int, string>>(
                                    x.NetDiscount,
                                    x.TotalAmount,
                                   (decimal)x.RecAmount,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO,
                                   x.ColorName,
                                   new Tuple<int, int, string, string, int, int, string>(
                                       x.SOrderID,
                                       x.CustomerType,
                                       x.CategoryName,
                                       x.CustomerName,
                                       x.CategoryID,
                                       x.PCategoryID,
                                       x.ConcernName
                                       )
                                   )

                ));
        }

        public static IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>>
            GetCreditCollectionReport(this IBaseRepository<CreditSale> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
            DateTime fromDate, DateTime toDate, int concernID, int CustomerID)
        {
            IEnumerable<CreditSale> CrditSaleList = new List<CreditSale>();
            if (CustomerID > 0)
                CrditSaleList = salesOrderRepository.All.Where(i => i.CustomerID == CustomerID);
            else
                CrditSaleList = salesOrderRepository.All;

            var items = (from cs in CrditSaleList
                         join csd in creditScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid"
                         select new
                         {
                             cus.Code,
                             cs.InvoiceNo,
                             cus.Name,
                             cus.Address,
                             cus.ContactNo,
                             cs.SalesDate,
                             csd.PaymentDate,
                             cs.TSalesAmt,
                             cs.NetAmount,
                             cs.FixedAmt,
                             cs.Remaining,
                             csd.InstallmentAmt,
                             csd.Remarks,
                             cs.DownPayment
                         }).ToList();

            return items.Select(x => new Tuple<string, string, string, string, DateTime, DateTime, decimal,
                Tuple<decimal, decimal, decimal, decimal, string, decimal>>
                (
                    x.Code,
                    x.InvoiceNo,
                    x.Name,
                    x.ContactNo + ", " + x.Address,
                    x.SalesDate,
                    x.PaymentDate,
                    x.TSalesAmt,
                    new Tuple<decimal, decimal, decimal, decimal, string, decimal>
                    (
                    x.NetAmount,
                    (decimal)x.FixedAmt,
                    x.Remaining,
                    x.InstallmentAmt,
                    x.Remarks,
                    x.DownPayment
                    )
                ));
        }

        public static decimal GetDefaultAmount(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSalesSchedule> CreditSalesSchedulesRepository, int CreditSaleID, DateTime FromDate)
        {
            var CSS = CreditSalesSchedulesRepository.All.Where(i => i.CreditSalesID == CreditSaleID && i.MonthDate < FromDate && i.PaymentStatus == "Due");
            if (CSS.Count() > 0)
            {
                return CSS.Sum(i => i.InstallmentAmt);
            }
            return 0m;
        }

        public static List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(this IBaseRepository<CreditSale> CreditSOrderRepository, IBaseRepository<CreditSaleDetails> CreditSOrderDetailRepo,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Product> ProductRepository,
            int ConcernID, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            List<CreditSale> CreditSOrders = new List<CreditSale>();
            if (CustomerID != 0)
                CreditSOrders = CreditSOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.SalesDate >= fromDate && i.SalesDate <= toDate).ToList();
            else
                CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate).ToList();

            var CreditSOrderDetails = CreditSOrderDetailRepo.All;
            var Products = ProductRepository.All;
            var Customers = CustomerRepository.All;
            var Employees = EmployeeRepository.All;


            var result = from SO in CreditSOrders.Where(i => i.IsStatus == EnumSalesType.Sales)
                         join SOD in CreditSOrderDetails on SO.CreditSalesID equals SOD.CreditSalesID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join C in Customers on SO.CustomerID equals C.CustomerID
                         join E in Employees on C.EmployeeID equals E.EmployeeID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.SalesDate,
                             EmployeeCode = E.Code,
                             EmployeeName = E.Name,
                             CustomerCode = C.Code,
                             CustomerName = C.Name,
                             Address = C.Address,
                             Mobile = C.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPOffer,
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
        public static List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(this IBaseRepository<CreditSale> CreditSOrderRepository,
    IBaseRepository<CreditSaleDetails> CreditSOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
    IBaseRepository<Category> CategoryRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Customer> CustomerRepository,
    int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType, int CustomerID)
        {
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

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


            var CreditSOrderDetails = CreditSOrderDetailRepo.All;
            var CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales);

            var result = from SO in CreditSOrders
                         join c in Customers on SO.CustomerID equals c.CustomerID
                         join SOD in CreditSOrderDetails on SO.CreditSalesID equals SOD.CreditSalesID
                         join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                         join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.SalesDate,
                             InvoiceNo = SO.InvoiceNo,
                             ProductName = P.ProductName,
                             CategoryName = CAT.Description,
                             CompanyName = COM.Name,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPOffer,
                             TotalAmount = SOD.UTAmount,
                             IMEI = STD.IMENO,
                             CustomerType = (int)c.CustomerType
                         };

            return result.ToList();
        }

        public static List<SOredersReportModel> SRWiseCreditSalesReport(this IBaseRepository<CreditSale> CreditSOrderRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepo,
         IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository,
          int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            var CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales).ToList();

            if (EmployeeID != 0)
                Customers = CustomerRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                Customers = CustomerRepository.All;

            var Employees = EmployeeRepository.All;


            var downpayments = (from SO in CreditSOrders
                                join C in Customers on SO.CustomerID equals C.CustomerID
                                join E in Employees on C.EmployeeID equals E.EmployeeID
                                select new SOredersReportModel
                                {
                                    InvoiceDate = SO.SalesDate,
                                    InvoiceNo = SO.InvoiceNo,
                                    EmployeeCode = E.Code,
                                    EmployeeName = E.Name,
                                    CustomerCode = C.Code,
                                    CustomerName = C.Name,
                                    CustomerAddress = C.Address,
                                    CustomerContactNo = C.ContactNo,
                                    RecAmount = SO.DownPayment,
                                    AdjAmount = SO.LastPayAdjAmt,
                                }).ToList();

            var InstallCollections = from SO in CreditSOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales)
                                     join sod in CreditSalesScheduleRepo.All on SO.CreditSalesID equals sod.CreditSalesID
                                     join C in Customers on SO.CustomerID equals C.CustomerID
                                     join E in Employees on C.EmployeeID equals E.EmployeeID
                                     where sod.PaymentStatus.Equals("Paid") && (sod.PaymentDate >= fromDate && sod.PaymentDate <= toDate)
                                     select new SOredersReportModel
                                     {
                                         InvoiceDate = sod.PaymentDate,
                                         InvoiceNo = SO.InvoiceNo + "-" + sod.ScheduleNo,
                                         EmployeeCode = E.Code,
                                         EmployeeName = E.Name,
                                         CustomerCode = C.Code,
                                         CustomerName = C.Name,
                                         CustomerAddress = C.Address,
                                         CustomerContactNo = C.ContactNo,
                                         RecAmount = sod.InstallmentAmt,
                                         AdjAmount = 0,
                                     };

            downpayments.AddRange(InstallCollections);
            return downpayments;
        }

        public static IQueryable<SOredersReportModel> GetAdminCrSalesReport(this IBaseRepository<CreditSale> SOrderRepository,
             IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID != 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var Sales = from so in SOrderRepository.GetAll()
                        join c in Customers on so.CustomerID equals c.CustomerID
                        join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                        where so.IsStatus == EnumSalesType.Sales && (so.SalesDate >= fromDate && so.SalesDate <= toDate)
                        select new SOredersReportModel
                        {
                            ConcernID = so.ConcernID,
                            ConcernName = s.Name,
                            CustomerCode = c.Code,
                            CustomerAddress = c.Address,
                            CustomerName = c.Name,
                            CustomerContactNo = c.ContactNo,
                            CustomerTotalDue = c.TotalDue,
                            InvoiceDate = so.SalesDate,
                            InvoiceNo = so.InvoiceNo,
                            Grandtotal = so.TSalesAmt,
                            NetDiscount = so.Discount,
                            TotalOffer = 0m,
                            AdjAmount = 0m,
                            TotalAmount = so.NetAmount,
                            RecAmount = (decimal)so.DownPayment,
                            PaymentDue = so.NetAmount - (decimal)so.DownPayment,
                            InstallmentPeriod = so.InstallmentPeriod,
                            CustomerType = EnumCustomerType.Hire
                        };

            return Sales.OrderBy(i => i.ConcernID).ThenByDescending(i => i.InvoiceDate);
        }


        public static IQueryable<CashCollectionReportModel> AdminInstallmentColllections(this IBaseRepository<CreditSale> SOrderRepository,
                    IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                    IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
                    int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID != 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var Sales = from so in SOrderRepository.GetAll()
                        join cs in CreditSalesScheduleRepository.GetAll() on so.CreditSalesID equals cs.CreditSalesID
                        join c in Customers on so.CustomerID equals c.CustomerID
                        join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                        where so.IsStatus == EnumSalesType.Sales && cs.PaymentStatus.Equals("Paid") && (cs.PaymentDate >= fromDate && cs.PaymentDate <= toDate)
                        select new CashCollectionReportModel
                        {
                            ConcernID = so.ConcernID,
                            ConcernName = s.Name,
                            CustomerCode = c.Code,
                            CustomerName = c.Name,
                            EntryDate = cs.PaymentDate,
                            ReceiptNo = so.InvoiceNo + "-" + cs.ScheduleNo,
                            Amount = cs.InstallmentAmt,
                            ModuleType = "Installment",
                            AdjustAmt = (so.NoOfInstallment == cs.ScheduleNo && cs.InstallmentAmt > 0) ? so.LastPayAdjAmt : 0m,
                            CustomerType = EnumCustomerType.Hire,
                        };

            return Sales;
        }


        public static HireAccountDetailsReportModel HireAccountDetails(this IBaseRepository<CreditSale> SOrderRepository,
                              IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                              IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                              IBaseRepository<Product> ProductRepository,
                              int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            IQueryable<CreditSale> CreditSales = null;
            IQueryable<Product> Products = null;
            HireAccountDetailsReportModel oReport = new HireAccountDetailsReportModel();
            if (ConcernID != 0)
            {
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID );
                CreditSales = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                Products = ProductRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            }
            else
            {
                Customers = CustomerRepository.GetAll();
                CreditSales = SOrderRepository.GetAll();
                Products = ProductRepository.GetAll();
            }

            oReport.OpeningAccount = CreditSales.Count(i => i.IsStatus == EnumSalesType.Sales && (i.SalesDate >= fromDate && i.SalesDate <= toDate));
            if (oReport.OpeningAccount > 0)
                oReport.OpeningAccountValue = CreditSales.Where(i => i.IsStatus == EnumSalesType.Sales && (i.SalesDate >= fromDate && i.SalesDate <= toDate)).Sum(i => (i.NetAmount - i.DownPayment));

            var closingSales = from so in CreditSales
                               join cs in CreditSalesScheduleRepository.GetAll() on so.CreditSalesID equals cs.CreditSalesID
                               where so.IsStatus == EnumSalesType.Sales && so.Remaining == 0 && cs.PaymentStatus.Equals("Paid")
                               && (cs.PaymentDate >= fromDate && cs.PaymentDate <= toDate)
                               select new { so, cs };

            oReport.ClosingAccount = closingSales.GroupBy(i => i.so.CreditSalesID).Count();
            if (oReport.ClosingAccount > 0)
                oReport.ClosingAccountValue = closingSales.Sum(i => i.cs.InstallmentAmt);



            var RunningSales = (from so in CreditSales
                                join sod in CreditSaleDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                                join p in Products on sod.ProductID equals p.ProductID
                                join c in Customers on so.CustomerID equals c.CustomerID
                                join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                                where so.IsStatus == EnumSalesType.Sales && so.Remaining > 0  && c.CreditDue > 0
                                   && (so.SalesDate >= fromDate && so.SalesDate <= toDate)
                                select new RunningAccountModel
                                {
                                    ConcernID = so.ConcernID,
                                    ConcernName = s.Name,
                                    CustomerCode = c.Code,
                                    CustomerName = c.Name,
                                    Address = c.Address,
                                    ContactNo = c.ContactNo,
                                    Date = so.SalesDate,
                                    InvoiceNo = so.InvoiceNo,
                                    SalesPrice = so.NetAmount,
                                    RemainingAmt = so.Remaining,
                                    RefName = c.RefName,
                                    ProductName = p.ProductName,
                                    NoOfInstallment = so.NoOfInstallment,
                                }).ToList();
            var finalRunning = from so in RunningSales
                               group so by new
                               {
                                   so.ConcernID,
                                   so.ConcernName,
                                   so.CustomerCode,
                                   so.CustomerName,
                                   so.Address,
                                   so.ContactNo,
                                   so.Date,
                                   so.InvoiceNo,
                                   so.SalesPrice,
                                   so.RemainingAmt,
                                   so.RefName,
                                   so.NoOfInstallment
                               } into g
                               select new RunningAccountModel
                               {
                                   ConcernID = g.Key.ConcernID,
                                   ConcernName = g.Key.ConcernName,
                                   CustomerCode = g.Key.CustomerCode,
                                   CustomerName = g.Key.CustomerName,
                                   Address = g.Key.Address,
                                   ContactNo = g.Key.ContactNo,
                                   Date = g.Key.Date,
                                   InvoiceNo = g.Key.InvoiceNo,
                                   SalesPrice = g.Key.SalesPrice,
                                   RemainingAmt = g.Key.RemainingAmt,
                                   RefName = g.Key.RefName,
                                   NoOfInstallment = g.Key.NoOfInstallment,
                                   ProductList = g.Select(i => i.ProductName).ToList()
                               };

            oReport.RunningAccountList = finalRunning.OrderBy(i => i.Date).ThenBy(i => i.InvoiceNo).ToList();
            oReport.RunningAccount = oReport.RunningAccountList.Count();

            if (oReport.RunningAccount > 0)
                oReport.RunningAccountValue = oReport.RunningAccountList.Sum(i => i.RemainingAmt);

            return oReport;
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
                   string, decimal, EnumSalesType, Tuple<string, string>>>>
                           GetAllPendingSalesOrderAsync(this IBaseRepository<CreditSale> salesOrderRepository,
                           IBaseRepository<Customer> customerRepository, IBaseRepository<ApplicationUser> UserRepository)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<ApplicationUser> users = UserRepository.All;

            var items = await (from cs in salesOrderRepository.All
                               join cus in customers on cs.CustomerID equals cus.CustomerID
                               join us in UserRepository.All on cs.CreatedBy equals us.Id
                               where (cs.IsStatus == EnumSalesType.Pending)
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = cs.CreditSalesID,
                                   InvoiceNo = cs.InvoiceNo,
                                   Date = cs.SalesDate,
                                  
                                   CustomerName = cus.Name,
                                   Mobile = cus.ContactNo,
                                   PaymentDue = cs.Remaining,
                                   IsStatus = cs.IsStatus,
                                   TotalAmount = cs.NetAmount,
                                  
                                   UserName = us.UserName,
                                  
                               }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType,
                Tuple<string, string>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.PaymentDue,
                    x.IsStatus,
                    new Tuple<string, string>
                    (x.CustomerCode,
                     
                    
                     x.UserName
                    
                     )
                )).OrderByDescending(x => x.Item3).ToList();
        }

        public static IEnumerable<UpcommingScheduleReport> GetScheduleCollection(this IBaseRepository<CreditSale> salesOrderRepository,
        IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository, IBaseRepository<Employee> employeeRepository,
        DateTime fromDate, DateTime toDate, string Status)
        {
            IQueryable<CreditSalesSchedule> schedules = creditScheduleRepository.All;
            if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                schedules = creditScheduleRepository.All.Where(i => (i.PaymentDate >= fromDate && i.PaymentDate <= toDate)
                 || (i.MonthDate >= fromDate && i.MonthDate <= toDate));

            if (!string.IsNullOrWhiteSpace(Status))
                schedules = schedules.Where(i => i.PaymentStatus == Status);

            var items = (from cs in salesOrderRepository.All
                         join csd in schedules on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         join emp in employeeRepository.All on cus.EmployeeID equals emp.EmployeeID
                         where csd.InstallmentAmt != 0
                         select new UpcommingScheduleReport
                         {
                             CSScheduleID = csd.CSScheduleID,
                             CustomerCode = cus.Code,
                             InvoiceNo = cs.InvoiceNo,
                             CustomerName = cus.Name,
                             EmployeeName = emp.Name,
                             EmployeeID = emp.EmployeeID,
                             CustomerConctact = cus.ContactNo,
                             CustomerAddress = cus.Address,
                             SalesDate = cs.SalesDate,
                             PaymentDate = csd.PaymentDate,
                             ScheduleDate = csd.MonthDate,
                             TSalesAmt = cs.TSalesAmt,
                             NetAmount = cs.NetAmount,
                             FixedAmt = (decimal)cs.FixedAmt,
                             Remaining = cs.Remaining,
                             InstallmentAmount = csd.InstallmentAmt,
                             Remarks = csd.Remarks,
                             DownPayment = cs.DownPayment,
                             InstallmentPeriod = cs.InstallmentPeriod,
                             PenaltyInterest = cs.PenaltyInterest,
                             SalesPrice = cs.TSalesAmt,
                             NoOfInstallment = cs.NoOfInstallment,
                             NoOfRemainingInstallment = cs.NoOfInstallment - (int)csd.ScheduleNo,
                             ExpectedInstallment = csd.ExpectedInstallment,
                             PaymentStatus = csd.PaymentStatus,
                         }).OrderBy(i => i.InstallmentPeriod).ToList();
            return items;
        }

        public static IEnumerable<AdjustmentReportModel> GetAdjustmentReport(this IBaseRepository<CreditSale> saleRepository, IBaseRepository<CreditSalesSchedule> creditSalesScheduleRepository,
            IBaseRepository<Customer> customerRepository,DateTime fromDate, DateTime toDate)
        {
            IQueryable<CreditSale> creditSales = saleRepository.All;
            IQueryable<CreditSalesSchedule> creditSalesSchedules = creditSalesScheduleRepository.All;
            IQueryable<Customer> customers = customerRepository.All;

            var data = (from cs in creditSales
                        join csd in creditSalesSchedules on cs.CreditSalesID equals csd.CreditSalesID
                        join co in customers on cs.CustomerID equals co.CustomerID
                        where csd.PaymentStatus == "Paid" && csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate
                        select new AdjustmentReportModel
                        {
                            CSScheduleID = csd.CSScheduleID,
                            CustomerName = co.Name,
                            CustomerCode = co.Code,
                            Date = csd.PaymentDate,
                            InvoiceNo = cs.InvoiceNo,
                            AdjutmentAmt = csd.LastPayAdjust

                        }).Where(i=>i.AdjutmentAmt>0).ToList();

            return data;
        }

        public static IEnumerable<UpcommingScheduleReport> GetLastPayAdjAmt(this IBaseRepository<CreditSale> salesOrderRepository,
        IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
        IBaseRepository<Product> productRepository, IBaseRepository<CreditSaleDetails> creditSaleDetailsRepository,
        DateTime fromDate, DateTime toDate, int concernID)
        {
            var items = (from cs in salesOrderRepository.All
                         join csd in creditSaleDetailsRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                         join css in creditScheduleRepository.All on csd.CreditSalesID equals css.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         join pro in productRepository.All on csd.ProductID equals pro.ProductID
                         where (css.PaymentDate >= fromDate && css.PaymentDate <= toDate && css.LastPayAdjust > 0) && (cs.ConcernID == concernID)
                         select new UpcommingScheduleReport
                         {
                             CustomerCode = cus.Code,
                             InvoiceNo = cs.InvoiceNo,
                             CustomerName = cus.Name,
                             CustomerConctact = cus.ContactNo,
                             CustomerAddress = cus.Address,
                             ProName = pro.ProductName,
                             SalesDate = cs.SalesDate,
                             TSalesAmt = cs.TSalesAmt,
                             NetAmount = cs.NetAmount,
                             FixedAmt = (decimal)cs.FixedAmt,
                             Remaining = cs.Remaining,
                             DownPayment = cs.DownPayment,
                             InstallmentPeriod = cs.InstallmentPeriod,
                             PenaltyInterest = cs.PenaltyInterest,
                             SalesPrice = cs.TSalesAmt,
                             NoOfInstallment = cs.NoOfInstallment,
                             LastPayAdjAmt = css.LastPayAdjust,
                             PaymentDate = css.PaymentDate

                         }).OrderBy(i => i.SalesDate).ToList();

            var FinalData = from fd in items
                            group fd by new
                            {
                                fd.InvoiceNo,

                            } into g
                            select new UpcommingScheduleReport
                            {
                                CustomerCode = g.Select(i => i.CustomerCode).FirstOrDefault(),
                                InvoiceNo = g.Key.InvoiceNo,
                                CustomerName = g.Select(i => i.CustomerName).FirstOrDefault(),
                                CustomerConctact = g.Select(i => i.CustomerConctact).FirstOrDefault(),
                                CustomerAddress = g.Select(i => i.CustomerAddress).FirstOrDefault(),
                                SalesDate = g.Select(i => i.SalesDate).FirstOrDefault(),
                                TSalesAmt = g.Select(i => i.TSalesAmt).FirstOrDefault(),
                                NetAmount = g.Select(i => i.NetAmount).FirstOrDefault(),
                                FixedAmt = g.Select(i => i.FixedAmt).FirstOrDefault(),
                                Remaining = g.Select(i => i.Remaining).FirstOrDefault(),
                                DownPayment = g.Select(i => i.DownPayment).FirstOrDefault(),
                                InstallmentPeriod = g.Select(i => i.InstallmentPeriod).FirstOrDefault(),
                                PenaltyInterest = g.Select(i => i.PenaltyInterest).FirstOrDefault(),
                                SalesPrice = g.Select(i => i.SalesPrice).FirstOrDefault(),
                                NoOfInstallment = g.Select(i => i.NoOfInstallment).FirstOrDefault(),
                                ProductName = g.Select(i => i.ProName).ToList(),
                                PaymentDate = g.Select(i => i.PaymentDate).FirstOrDefault(),
                                LastPayAdjAmt = g.Select(i => i.LastPayAdjAmt).FirstOrDefault(),

                            };
            return FinalData;
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>> GetAllSalesReturnOrderAsync(this IBaseRepository<CreditSale> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<SisterConcern> SisterConcernRepository, DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<CreditSale> creditSales = salesOrderRepository.All
                                                .Where(i => (i.IsStatus == EnumSalesType.Sales) && i.IsReturn == 1);

            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                creditSales = creditSales.Where(i => i.InvoiceNo.Contains(InvoiceNo));
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
                creditSales = creditSales.Where(i => (i.ReturnDate >= fromDate && i.ReturnDate <= toDate));

            var items = await (from so in creditSales
                               join c in customers on so.CustomerID equals c.CustomerID
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = so.CreditSalesID,
                                   InvoiceNo = so.InvoiceNo,
                                   Date = so.ReturnDate.Value,
                                   CustomerCode = c.Code,
                                   CustomerName = c.Name,
                                   Mobile = c.ContactNo,
                                   PaymentDue = so.Remaining,
                                   IsStatus = so.IsStatus,
                                   TotalAmount = so.NetAmount,
                                   IsReturn = so.IsReturn
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

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.PaymentDue,
                    x.IsStatus,
                    new Tuple<string, int>
                    (x.CustomerCode, x.IsReturn)
                )).OrderByDescending(x => x.Item3).ToList();
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, string, decimal, Tuple<string, string, decimal>>>
        GetHireReturnDetailReportByReturnID(this IBaseRepository<CreditSale> salesOrderRepository, IBaseRepository<CreditSaleDetails> SorderDetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<HireSalesReturnCustomerDueAdjustment> HireSalesReturnRepository,
        IBaseRepository<StockDetail> stockdetailRepository, int ReturnID, int concernID)
        {
            var oSalesDetailData = (from SO in salesOrderRepository.All
                                    join SOD in SorderDetailRepository.All on SO.CreditSalesID equals SOD.CreditSalesID
                                    join HRO in HireSalesReturnRepository.All on SO.CreditSalesID equals HRO.CreditSalesId
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                    where (SO.CreditSalesID == ReturnID && SOD.IsProductReturn == 1)
                                    select new
                                    {
                                        HRO.Id,
                                        SO.InvoiceNo,
                                        SO.Customer.Name,
                                        InvoiceDate = HRO.TransactionDate,
                                        NetDiscount = 0m,
                                        AdjDue = HRO.AdjDue,
                                        P.ProductID,
                                        P.ProductName,
                                        SOD.UnitPrice,
                                        SOD.UTAmount,
                                        PPDAmount = 0m,
                                        PPOffer = 0m,
                                        SOD.Quantity,
                                        std.IMENO,
                                        AdjAmount = 0m,
                                        MemoNo = HRO.MemoNo,
                                        Remarks = HRO.Remarks,
                                        ToCustomerPay = HRO.ToCustomerPayAmt
                                    }).OrderBy(x => x.Id).ToList();


            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, string, decimal, Tuple<string, string, decimal>>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                 x.ProductName,
                 x.Name,
                 x.AdjDue,
                 x.IMENO,
                 x.Quantity,
                 new Tuple<string, string, decimal>
                 (
                     x.MemoNo,
                     x.Remarks,
                     x.ToCustomerPay
                     )
                ));
        }


    }
}
