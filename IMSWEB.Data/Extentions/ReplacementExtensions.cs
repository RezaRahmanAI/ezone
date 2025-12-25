using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ReplacementExtensions
    {
        public static List<ReplacementReportModel> ReplacementReport(this IBaseRepository<SOrder> replacementOrderRepository, 
                                                                      IBaseRepository<SOrderDetail> sOrderDetailsRepository, IBaseRepository<Customer> customerRepository,
                                                                      IBaseRepository<Product> productRepository,
                                                                      IBaseRepository<StockDetail> stockDetailRepository,
                                                                      IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, 
                                                                      int CustomerID, DateTime fromDate, DateTime toDate)
        {
            //fromDate = fromDate.AddTicks(1);
            //toDate = toDate.AddTicks(-1);
            IEnumerable<SOrder> ReplacementOrders = null;
            if (CustomerID == 0)
                ReplacementOrders = replacementOrderRepository.All.Where(i => i.IsReplacement == 1 && i.InvoiceDate >= fromDate && i.InvoiceDate<=toDate );
            else
                ReplacementOrders = replacementOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.IsReplacement == 1 && i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate);

            //var salesOrders = replacementOrderRepository.All;
            var sOrderDetais = sOrderDetailsRepository.All;
            var products = productRepository.All;
            var customers = customerRepository.All;
            var stockDetails = stockDetailRepository.All;
            var SOrders = replacementOrderRepository.All;
            var POrders = POrderRepository.All;
            var POrderDetails = POrderDetailRepository.All;

            var result = (from s in ReplacementOrders
                         join sod in sOrderDetais on s.SOrderID equals sod.RepOrderID
                         join stockD in stockDetails on sod.SDetailID equals stockD.SDetailID
                         join POD in POrderDetails on stockD.POrderDetailID equals POD.POrderDetailID
                         join PO in POrders on POD.POrderID equals PO.POrderID
                         join pro in products on sod.ProductID equals pro.ProductID
                         join cus in customers on s.CustomerID equals cus.CustomerID
                         select new 
                         {
                             PODate = PO.OrderDate,
                             SOrderID=sod.SOrderID,
                             //SalesDate = s.InvoiceDate,
                             //Invoice = s.InvoiceNo,

                             ReturnDate = s.InvoiceDate,
                             ReturnInvoice = s.InvoiceNo,
                             CustomerCode = cus.Code,
                             CustomerName = cus.Name,
                             CustomerAddress = cus.Address,
                             CustomerMobile = cus.ContactNo,
                             DamageProudct = pro.ProductName,
                             DamageIMEI = stockD.IMENO,
                             DamageQty = sod.Quantity,
                             DamageSalesRate = sod.UTAmount,
                             RepStockDetailID=sod.RStockDetailID,
                             //ReplaceProduct = pro.ProductName,

                             //ReplaceIMEI =stockDetailRepository.FindBy(i=>i.SDetailID==sod.RStockDetailID).FirstOrDefault().IMENO,
                             ReplaceQty = sod.Quantity,
                             ReplaceRate = (decimal)sod.RepUnitPrice,
                             Remarks = sod.Remarks
                         }).ToList();

            var items = (from res in result
                        join sto in stockDetails on res.RepStockDetailID equals sto.SDetailID
                        join SO in SOrders on res.SOrderID equals SO.SOrderID
                        join pro in products on sto.ProductID equals pro.ProductID
                        select new ReplacementReportModel
                        {
                            //SOrderID=sod.SOrderID,
                            SalesDate = SO.InvoiceDate,
                            Invoice = SO.InvoiceNo,

                            ReturnDate = res.ReturnDate,
                            ReturnInvoice = res.ReturnInvoice,
                            CustomerCode = res.CustomerCode,
                            CustomerName = res.CustomerName,
                            CustomerAddress = res.CustomerAddress,
                            CustomerMobile = res.CustomerMobile,
                            DamageProudct = res.DamageProudct,
                            DamageIMEI = res.DamageIMEI,
                            DamageQty = res.DamageQty,
                            DamageSalesRate = res.DamageSalesRate,
                            //RepStockDetailID=res.RepStockDetailID,

                            ReplaceProduct = pro.ProductName,
                            ReplaceIMEI = sto.IMENO,
                            ReplaceQty = res.ReplaceQty,
                            ReplaceRate = (decimal)res.ReplaceRate,
                            Remarks = res.Remarks,
                            PODate = res.PODate
                        }).ToList();



            return items;
        }

        /// <summary>
        /// Date: 25-Mar-2018
        /// Author: aminul
        /// </summary>
        /// <param name="returnOrderRepository"></param>
        /// <param name="sOrderDetailsRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="productRepository"></param>
        /// <param name="stockDetailRepository"></param>
        /// <param name="CustomerID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static List<ReturnReportModel> ReturntReport(this IBaseRepository<SOrder> returnOrderRepository, IBaseRepository<SOrderDetail> sOrderDetailsRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockDetailRepository, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            fromDate = fromDate.AddDays(-1);
            toDate = toDate.AddDays(-1);
            IEnumerable<SOrder> ReturnOrders = null;
            if (CustomerID == 0)
                ReturnOrders = returnOrderRepository.FindBy(i => i.Status == (int)EnumSalesType.ProductReturn && i.InvoiceDate > fromDate && i.InvoiceDate < toDate);
            else
                ReturnOrders = returnOrderRepository.FindBy(i => i.CustomerID == CustomerID && i.Status == (int)EnumSalesType.ProductReturn && i.InvoiceDate > fromDate && i.InvoiceDate < toDate);

            //var salesOrders = replacementOrderRepository.All;
            var sOrderDetais = sOrderDetailsRepository.All;
            var products = productRepository.All;
            var customers = customerRepository.All;
            var stockDetails = stockDetailRepository.All;
            var result = (from s in ReturnOrders
                          join sod in sOrderDetais on s.SOrderID equals sod.SOrderID
                          join stockD in stockDetails on sod.SDetailID equals stockD.SDetailID
                          join pro in products on sod.ProductID equals pro.ProductID
                          join cus in customers on s.CustomerID equals cus.CustomerID
                          select new ReturnReportModel
                          {
                              SOrderID = sod.SOrderID,
                              ReturnDate = s.InvoiceDate,
                              ReturnInvoice = s.InvoiceNo,
                              CustomerCode = cus.Code,
                              CustomerName = cus.Name,
                              CustomerAddress = cus.Address,
                              CustomerMobile = cus.ContactNo,
                              Remarks = sod.Remarks,
                              
                              
                              ProductName = pro.ProductName,
                              ReturnIMEI = stockD.IMENO,
                              ReturnQty = sod.Quantity,
                              ReturnAmount = sod.UnitPrice
 
                          }).ToList();



            return result;
        }

        public static List<ReplacementReportModel> DamageReport(this IBaseRepository<SOrder> replacementOrderRepository, IBaseRepository<SOrderDetail> sOrderDetailsRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockDetailRepository, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            //fromDate = fromDate.AddTicks(1);
            //toDate = toDate.AddTicks(-1);
            IEnumerable<SOrder> ReplacementOrders = null;
            if (CustomerID == 0)
                ReplacementOrders = replacementOrderRepository.All.Where(i => i.IsReplacement == 1 && i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate);
            else
                ReplacementOrders = replacementOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.IsReplacement == 1 && i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate);

            //var salesOrders = replacementOrderRepository.All;
            var sOrderDetais = sOrderDetailsRepository.All;
            var products = productRepository.All;
            var customers = customerRepository.All;
            var stockDetails = stockDetailRepository.All;
            var SOrders = replacementOrderRepository.All;
            var result = (from s in ReplacementOrders
                          join sod in sOrderDetais on s.SOrderID equals sod.RepOrderID
                          join stockD in stockDetails on sod.SDetailID equals stockD.SDetailID
                          join pro in products on sod.ProductID equals pro.ProductID
                          join cus in customers on s.CustomerID equals cus.CustomerID
                          select new
                          {
                              SOrderID = sod.SOrderID,
                              //SalesDate = s.InvoiceDate,
                              //Invoice = s.InvoiceNo,

                              ReturnDate = s.InvoiceDate,
                              ReturnInvoice = s.InvoiceNo,
                              CustomerCode = cus.Code,
                              CustomerName = cus.Name,
                              CustomerAddress = cus.Address,
                              CustomerMobile = cus.ContactNo,
                              DamageProudct = pro.ProductName,
                              DamageIMEI = stockD.IMENO,
                              DamageQty = sod.Quantity,
                              DamageSalesRate = sod.UTAmount,
                              RepStockDetailID = sod.RStockDetailID,
                              //ReplaceProduct = pro.ProductName,

                              //ReplaceIMEI =stockDetailRepository.FindBy(i=>i.SDetailID==sod.RStockDetailID).FirstOrDefault().IMENO,
                              ReplaceQty = sod.Quantity,
                              ReplaceRate = (decimal)sod.RepUnitPrice,
                              Remarks = sod.Remarks
                          }).ToList();

            var items = (from res in result
                         join sto in stockDetails on res.RepStockDetailID equals sto.SDetailID
                         join SO in SOrders on res.SOrderID equals SO.SOrderID
                         join pro in products on sto.ProductID equals pro.ProductID
                         select new ReplacementReportModel
                         {
                             //SOrderID=sod.SOrderID,
                             SalesDate = SO.InvoiceDate,
                             Invoice = SO.InvoiceNo,

                             ReturnDate = res.ReturnDate,
                             ReturnInvoice = res.ReturnInvoice,
                             CustomerCode = res.CustomerCode,
                             CustomerName = res.CustomerName,
                             CustomerAddress = res.CustomerAddress,
                             CustomerMobile = res.CustomerMobile,
                             DamageProudct = res.DamageProudct,
                             DamageIMEI = res.DamageIMEI,
                             DamageQty = res.DamageQty,
                             DamageSalesRate = res.DamageSalesRate,
                             //RepStockDetailID=res.RepStockDetailID,

                             ReplaceProduct = pro.ProductName,
                             ReplaceIMEI = sto.IMENO,
                             ReplaceQty = res.ReplaceQty,
                             ReplaceRate = (decimal)res.ReplaceRate,
                             Remarks = res.Remarks
                         }).ToList();



            return items;
        }
    
    }
}
