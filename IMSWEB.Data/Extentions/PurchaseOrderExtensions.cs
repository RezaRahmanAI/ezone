using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class PurchaseOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, string, EnumPurchaseType, Tuple<int>>>> GetAllPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
            IBaseRepository<Supplier> supplierRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, bool IsVATManager, int ConcernID)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.Purchase && (i.OrderDate >= fromDate && i.OrderDate <= toDate)).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new ProductWisePurchaseModel
                {
                    POrderID = x.PurchaseOrder.POrderID,
                    ChallanNo = x.PurchaseOrder.ChallanNo,
                    Date = x.PurchaseOrder.OrderDate,
                    SupplierName = x.Supplier.Name,
                    OwnerName = x.Supplier.OwnerName,
                    Mobile = x.Supplier.ContactNo,
                    Status = x.PurchaseOrder.Status,
                    TAmount = x.PurchaseOrder.TotalAmt,
                    EditReqStatus = x.PurchaseOrder.EditReqStatus
                }).ToListAsync();

            List<ProductWisePurchaseModel> finalData = new List<ProductWisePurchaseModel>();
            if (IsVATManager)
            {
                items = items.OrderByDescending(i => i.Date).ToList();
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID);
                decimal FalesPurchase = (items.Sum(i => i.TAmount) * oConcern.PurchaseShowPercent) / 100m;
                decimal FalesPurchaseCount = 0m;

                foreach (var item in items)
                {
                    FalesPurchaseCount += item.TAmount;
                    if (FalesPurchaseCount <= FalesPurchase)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType, Tuple<int>>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.Date,
                    x.SupplierName,
                    x.OwnerName,
                    x.Mobile,
                    (EnumPurchaseType)x.Status,
                    new Tuple<int>
                    (x.EditReqStatus)
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
                     string, string, EnumPurchaseType>>> GetAllDeliveryOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
                    IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DeliveryOrder).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
string, string, EnumPurchaseType>>> GetAllDamageReturnOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DamageReturn).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        //public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        //string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
        //    IBaseRepository<Supplier> supplierRepository)
        //     {
        //         IQueryable<Supplier> suppliers = supplierRepository.All;

        //         var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.ProductReturn).
        //             GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
        //             (p, s) => new { PurchaseOrder = p, Suppliers = s }).
        //             SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
        //             .Select(x => new
        //             {
        //                 x.PurchaseOrder.POrderID,
        //                 x.PurchaseOrder.ChallanNo,
        //                 x.PurchaseOrder.OrderDate,
        //                 x.Supplier.Name,
        //                 x.Supplier.OwnerName,
        //                 x.Supplier.ContactNo,
        //                 x.PurchaseOrder.Status
        //             }).ToListAsync();

        //         return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
        //             (
        //                 x.POrderID,
        //                 x.ChallanNo,
        //                 x.OrderDate,
        //                 x.Name,
        //                 x.OwnerName,
        //                 x.ContactNo,
        //                 (EnumPurchaseType)x.Status
        //             )).OrderByDescending(x => x.Item1).ToList();
        //     }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
   string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
       IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;
            var POrders = purchaseOrderRepository.All;


            #region OLD

            //var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.ProductReturn || i.Status == (int)EnumPurchaseType.ReturnProductReturn).
            //    GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
            //    (p, s) => new { PurchaseOrder = p, Suppliers = s }).
            //    SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
            //    .Select(x => new
            //    {
            //        x.PurchaseOrder.POrderID,
            //        x.PurchaseOrder.ChallanNo,
            //        x.PurchaseOrder.OrderDate,
            //        x.Supplier.Name,
            //        x.Supplier.OwnerName,
            //        x.Supplier.ContactNo,
            //        x.PurchaseOrder.Status
            //    }).ToListAsync();

            #endregion

            var items = await (from po in POrders
                               join sup in suppliers on po.SupplierID equals sup.SupplierID
                               where po.Status == (int)EnumPurchaseType.ProductReturn &&
                               (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                               select new
                               {
                                   po.POrderID,
                                   po.ChallanNo,
                                   po.OrderDate,
                                   sup.Name,
                                   sup.OwnerName,
                                   sup.ContactNo,
                                   po.Status
                               }).OrderByDescending(s => s.POrderID).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal,
            Tuple<decimal, decimal, string, string>>>
            GetPurchaseReport(this IBaseRepository<POrder> purchaseOrderRepository, IBaseRepository<Supplier> supplierRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType,
            bool IsAdminReport, int ConcernID)
        {
            IQueryable<POrder> pOrders = null;
            IQueryable<Supplier> suppliers = null;
            IQueryable<SisterConcern> concerns = null;
            if (IsAdminReport)
            {
                if (ConcernID > 0)
                {
                    pOrders = purchaseOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    suppliers = supplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    concerns = SisterConcernRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                else
                {
                    pOrders = purchaseOrderRepository.GetAll();
                    suppliers = supplierRepository.GetAll();
                    concerns = SisterConcernRepository.GetAll();
                }
            }
            else
            {
                pOrders = purchaseOrderRepository.All;
                suppliers = supplierRepository.All;
                concerns = SisterConcernRepository.All;
            }

            var oPurchaseData = (from pOrd in pOrders
                                 join sis in concerns on pOrd.ConcernID equals sis.ConcernID
                                 join sup in suppliers on pOrd.SupplierID equals sup.SupplierID
                                 where (pOrd.OrderDate >= fromDate && pOrd.OrderDate <= toDate && pOrd.Status == (int)PurchaseType)
                                 group pOrd by new
                                 {
                                     sup.Code,
                                     sup.Name,
                                     pOrd.ChallanNo,
                                     pOrd.OrderDate,
                                     pOrd.GrandTotal,
                                     pOrd.NetDiscount,
                                     pOrd.TotalAmt,
                                     pOrd.RecAmt,
                                     pOrd.PaymentDue,
                                     sis.ConcernID,
                                     ConcenName = sis.Name,
                                     InvNo = pOrd.InvoiceNo
                                 } into g
                                 select new ProductWisePurchaseModel
                                 {
                                     ConcenName = g.Key.ConcenName,
                                     SupplierCode = g.Key.Code,
                                     SupplierName = g.Key.Name,
                                     Date = g.Key.OrderDate,
                                     ChallanNo = g.Key.ChallanNo,
                                     GrandTotal = g.Key.GrandTotal,
                                     NetDiscount = g.Key.NetDiscount,
                                     TotalAmount = g.Key.TotalAmt,
                                     RecAmt = g.Key.RecAmt,
                                     PaymentDue = g.Key.PaymentDue,
                                     InvoiceNo = g.Key.InvNo
                                 }).ToList();



            return oPurchaseData.Select(x => new Tuple<string, string, DateTime, string,
                decimal, decimal, decimal, Tuple<decimal, decimal, string, string>>
                  (
                    x.SupplierCode,
                    x.SupplierName,
                    x.Date,
                    x.ChallanNo,
                    x.GrandTotal,
                    x.NetDiscount,
                    x.TotalAmount,
                    new Tuple<decimal, decimal, string, string>(
                    (decimal)x.RecAmt,
                    x.PaymentDue,
                    x.ConcenName,
                    x.InvoiceNo
                    )
                  ));
        }

        public static IEnumerable<ProductWisePurchaseModel> GetPurchaseDetailReportByConcernID(this IBaseRepository<POrder> purchaseOrderRepository,
            IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, bool IsVATManager, int ConcernID)
        {

            var Porders = purchaseOrderRepository.All
                .Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType)
                .OrderByDescending(i => i.OrderDate);

            List<ProductWisePurchaseModel> filterPOrders = new List<ProductWisePurchaseModel>();
            if (IsVATManager)
            {
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID);
                decimal FalesPurchase = (Porders.Sum(i => i.TotalAmt) * oConcern.PurchaseShowPercent) / 100m;
                decimal FalePurchaseCount = 0m;

                foreach (var item in Porders)
                {
                    FalePurchaseCount += item.TotalAmt;
                    if (FalePurchaseCount <= FalesPurchase)
                    {

                        var oPurchaseDetailData = (from PO in Porders
                                                   join POD in pOrderDetailRepository.All on PO.POrderID equals POD.POrderID
                                                   join P in productRepository.All on POD.ProductID equals P.ProductID
                                                   join C in colorRepository.All on POD.ColorID equals C.ColorID
                                                   //  join POPD in poProductRepository.All on POD.ProductID equals POPD.ProductID
                                                   where (PO.POrderID == item.POrderID)
                                                   select new ProductWisePurchaseModel
                                                   {
                                                       POrderID = PO.POrderID,
                                                       ChallanNo = PO.ChallanNo,
                                                       Date = PO.OrderDate,
                                                       GrandTotal = PO.GrandTotal,
                                                       NetDiscount = PO.NetDiscount,
                                                       TotalAmount = PO.TotalAmt,
                                                       RecAmt = PO.RecAmt,
                                                       PaymentDue = PO.PaymentDue,
                                                       ProductID = P.ProductID,
                                                       ProductName = P.ProductName,
                                                       PurchaseRate = POD.UnitPrice - (((PO.AdjAmount + PO.TDiscount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount + PO.AdjAmount)),
                                                       TAmount = POD.TAmount,
                                                       PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                                       CategoryName = P.Category.Description,
                                                       IMENO = POD.POProductDetails.Select(i => i.IMENO).ToString(),
                                                       ColorName = C.Name,
                                                       PPOffer = POD.PPOffer,
                                                       Quantity = POD.Quantity
                                                   }).OrderByDescending(x => x.Date).ToList();
                        filterPOrders.AddRange(oPurchaseDetailData);

                    }
                    else
                        break;
                }
            }
            else
            {
                var oPurchaseDetailData = (from PO in Porders
                                           join POD in pOrderDetailRepository.All on PO.POrderID equals POD.POrderID
                                           join P in productRepository.All on POD.ProductID equals P.ProductID
                                           join C in colorRepository.All on POD.ColorID equals C.ColorID
                                           //  join POPD in poProductRepository.All on POD.ProductID equals POPD.ProductID
                                           select new ProductWisePurchaseModel
                                           {
                                               POrderID = PO.POrderID,
                                               ChallanNo = PO.ChallanNo,
                                               Date = PO.OrderDate,
                                               GrandTotal = PO.GrandTotal,
                                               NetDiscount = PO.NetDiscount,
                                               TotalAmount = PO.TotalAmt,
                                               RecAmt = PO.RecAmt,
                                               PaymentDue = PO.PaymentDue,
                                               ProductID = P.ProductID,
                                               ProductName = P.ProductName,
                                               PurchaseRate = POD.UnitPrice,
                                               TAmount = POD.TAmount,
                                               PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                               CategoryName = P.Category.Description,
                                               IMENO = POD.POProductDetails.Select(i => i.IMENO).ToString(),
                                               ColorName = C.Name,
                                               PPOffer = POD.PPOffer,
                                               Quantity = POD.Quantity
                                           }).OrderByDescending(x => x.Date).ToList();

                filterPOrders.AddRange(oPurchaseDetailData);
            }

            return filterPOrders;
        }


        public static IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
            Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<int, string>>>>
            GetPurchaseOrderDetailById(this IBaseRepository<POrderDetail> purchaseOrderDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<Godown> godownRepository, int orderId)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            var items = (from pod in purchaseOrderDetailRepository.All
                         join p in products on pod.ProductID equals p.ProductID
                         join col in colors on pod.ColorID equals col.ColorID into lc
                         from col in lc.DefaultIfEmpty()
                         join g in godownRepository.All on pod.GodownID equals g.GodownID into lg
                         from g in lg.DefaultIfEmpty()
                         where pod.POrderID == orderId
                         select new
                         {
                             pod.MRPRate,
                             pod.POrderDetailID,
                             pod.PPDISAmt,
                             pod.PPDISPer,
                             pod.ProductID,
                             pod.POrderID,
                             pod.Quantity,
                             pod.TAmount,
                             pod.UnitPrice,
                             p.ProductName,
                             ProductCode = p.Code,
                             ColorName = col != null ? col.Name : "N/A",
                             pod.ColorID,
                             pod.SalesRate,
                             GodownID = pod.GodownID,
                             GodownName = g != null ? g.Name : "N/A"
                         }).ToList();

            return items.Select(x => new Tuple<decimal, int, decimal, decimal, int, int, decimal,
                Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<int, string>>>
                (
                    x.MRPRate,
                    x.POrderDetailID,
                    x.PPDISAmt,
                    x.PPDISPer,
                    x.ProductID,
                    x.POrderID,
                    x.Quantity,
                    new Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<int, string>>
                    (
                        x.TAmount,
                        x.UnitPrice,
                        x.ProductName,
                        x.ProductCode,
                        x.ColorID,
                        x.ColorName,
                        x.SalesRate, new Tuple<int, string>(x.GodownID, x.GodownName)
                    )
                ));
        }

        //public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string,Tuple<string, string, string, string, string, decimal>>>>
        //GetPurchaseDetailReportBySupplierID(this IBaseRepository<POrder> purchaseOrderRepository, IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<Product> productRepository,
        //IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository, DateTime fromDate, DateTime toDate, int concernID,int supplierId)
        //{
        //    var oPurchaseDetailData = (from POD in pOrderDetailRepository.All
        //                               from PO in purchaseOrderRepository.All
        //                               from P in productRepository.All
        //                               from POP in poProductRepository.All
        //                               from C in colorRepository.All
        //                               where (POD.POrderID == PO.POrderID && POD.POrderDetailID == POP.POrderDetailID && P.ProductID == POD.ProductID && C.ColorID == POP.ColorID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == 1 && PO.ConcernID == concernID && PO.SupplierID==supplierId)
        //                               select new { PO.ChallanNo, PO.OrderDate, PO.GrandTotal, PO.TDiscount, PO.TotalAmt, PO.RecAmt, PO.PaymentDue, P.ProductID, P.ProductName, POD.UnitPrice, POD.TAmount, POD.PPDISAmt, POD.Quantity, POP.IMENO, ColorName = C.Name }).OrderByDescending(x => x.OrderDate).ToList();

        //    return oPurchaseDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string>>
        //        (
        //         x.OrderDate,
        //         x.ChallanNo,
        //        x.ProductName,
        //        x.UnitPrice,
        //        x.PPDISAmt,
        //        x.TAmount,
        //        x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string, string>(
        //                            x.TDiscount,
        //                            x.TotalAmt,
        //                           (decimal)x.RecAmt,
        //                           x.PaymentDue,
        //                           x.Quantity,
        //                           x.IMENO, x.ColorName)
        //        ));
        //}

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>>
        GetPurchaseDetailReportBySupplierID(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<POProductDetail> POProductDetailRepository, DateTime fromDate, DateTime toDate, int ConcernID, int SupplierID)
        {
            var oSalesDetailData = (from POD in POrderDetailRepository.All
                                    from PO in POrderRepository.All
                                    from P in productRepository.All
                                    from POPD in POProductDetailRepository.All
                                    where (POD.POrderID == PO.POrderID && POD.POrderDetailID == POPD.POrderDetailID && P.ProductID == POD.ProductID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == 1 && PO.SupplierID == SupplierID)
                                    select new
                                    {
                                        PO.ChallanNo,
                                        PO.OrderDate,
                                        PO.GrandTotal,
                                        PO.NetDiscount,
                                        PO.TotalAmt,
                                        PO.RecAmt,
                                        PO.PaymentDue,
                                        P.ProductID,
                                        P.ProductName,
                                        UnitPrice = (POD.UnitPrice - (((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount + PO.AdjAmount)))),
                                        //UnitPrice = POD.UnitPrice - (PO.TDiscount / PO.TPQty),
                                        //UnitPrice = POD.UnitPrice - (((PO.TDiscount / PO.TPQty) * POD.UnitPrice) / PO.TDiscount),
                                        POD.TAmount,
                                        PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                        POD.Quantity,
                                        POPD.IMENO,
                                        PO.Supplier.Name,
                                        PO.Supplier.Code,
                                        PO.Supplier.Address,
                                        PO.Supplier.ContactNo,
                                        PO.Supplier.OwnerName,
                                        PO.Supplier.TotalDue,
                                        POD.PPOffer
                                    }).OrderByDescending(x => x.OrderDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>
                (
                 x.OrderDate,
                 x.ChallanNo,
                x.ProductName,
                x.UnitPrice,
                x.PPDISAmt,
                x.TAmount,
                x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>(
                                    x.NetDiscount,
                                    x.TotalAmt,
                                   (decimal)x.RecAmt,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO,
                                   new Tuple<string, string, string, string, string, decimal, decimal>(
                                       x.Code,
                                       x.Name,
                                       x.Address,
                                       x.ContactNo,
                                       x.OwnerName,
                                       x.TotalDue,
                                       x.PPOffer
                                       ))
                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetPurchaseByProductID(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
         DateTime fromDate, DateTime toDate, int ConcernID, int productid)
        {
            var opurchase = (from POD in POrderDetailRepository.All
                             from PO in POrderRepository.All
                             from P in productRepository.All
                             where (POD.POrderID == PO.POrderID && P.ProductID == POD.ProductID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && P.ProductID == productid && PO.Status == 1)
                             group POD by new { PO.ChallanNo, PO.OrderDate, P.ProductName, POD.UnitPrice } into g
                             select new { g.Key.ChallanNo, g.Key.OrderDate, g.Key.ProductName, g.Key.UnitPrice, Quantity = g.Sum(x => x.Quantity) }).OrderBy(x => x.OrderDate).ToList();

            return opurchase.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.OrderDate,
                 x.ChallanNo,
                x.ProductName,
                x.Quantity,
                x.UnitPrice
                ));

            //return _basePurchaseOrderRepository.GetPurchaseByProductID(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, fromDate, toDate, ConcernId, productID);
        }

        public static AdvanceSearchModel AdvanceSearchByIMEI(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Stock> StockRepository, IBaseRepository<Supplier> SupplierRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository, IBaseRepository<Transfer> TransferRepository,
            IBaseRepository<TransferDetail> TransferDetailRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ROrderDetail> ROrderDetailRepository, IBaseRepository<Godown> GodownRepository,
          int ConcernID, string IMEINO)
        {
            IMEINO = IMEINO.Trim();
            AdvanceSearchModel advanceSearchModel = new AdvanceSearchModel();
            IEnumerable<StockDetail> StockDetaillist = null;
            IEnumerable<StockDetail> StockDetaillist_ = null;

            Stock objStock = null;
            StockDetail ConcernStockDetail = null;
            SOrderDetail objSOrderDetail = null;
            CreditSaleDetails objCreditSaleDetail = null;
            var Products = productRepository.All;
            var POrderDetails = POrderDetailRepository.All;
            var POProductDetails = POProductDetailRepository.All;
            var POProductDetail = POProductDetailRepository.All.Where(i => i.IMENO.Equals(IMEINO));
            var StockDetails = StockDetailRepository.All;
            var CreditSales = CreditSaleRepository.All;
            var CreditSalesDetails = CreditSaleDetailsRepository.All;
            var ROrders = ROrderRepository.All;
            var ROrderDetails = ROrderDetailRepository.All;
            var Godown = GodownRepository.All;
            var Stocks = StockRepository.All;


            #region Purchase
            if (POProductDetail != null)
            {
                var POrderDetail = POrderDetailRepository.All.Where(i => (POProductDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)) && (POProductDetail.Select(j => j.ProductID).Contains(i.ProductID)) && (POProductDetail.Select(j => j.ColorID).Contains(i.ColorID)));
                var POrder = POrderRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (POrderDetail.Select(j => j.POrderID).Contains(i.POrderID)));
                if (POrder != null)
                {
                    var concernPOrderDetail = POrderDetails.Where(i => i.POrderID == POrder.POrderID);
                    var ConcernPOProductDetail = POProductDetails.Where(i => (concernPOrderDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)));

                    var POResult = (from POD in concernPOrderDetail
                                    join POPD in ConcernPOProductDetail on POD.POrderDetailID equals POPD.POrderDetailID
                                    join P in Products on POD.ProductID equals P.ProductID
                                    select new AdvancePODetail
                                    {
                                        ProductCode = P.Code,
                                        ProductName = P.ProductName,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = 1,
                                        IMEI = POPD.IMENO
                                    }).ToList();
                    var SelectedIMEI = POResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                    POResult.Remove(SelectedIMEI);
                    POResult.Insert(0, SelectedIMEI);
                    advanceSearchModel.AdvancePODetails.AddRange(POResult);
                    advanceSearchModel.ChallanNo = POrder.ChallanNo;
                    advanceSearchModel.PurchaseDate = POrder.OrderDate;
                    var Supplier = SupplierRepository.FindBy(i => i.SupplierID == POrder.SupplierID).FirstOrDefault();
                    advanceSearchModel.SupplierCode = Supplier.Code;
                    advanceSearchModel.SupplierName = Supplier.Name;
                }

            }
            #endregion
            #region sales
            StockDetaillist = StockDetails.Where(i => i.IMENO.Equals(IMEINO) && (i.Status == (int)EnumStockStatus.Sold || i.Status == (int)EnumStockStatus.Damage));

            if (StockDetaillist != null || StockDetaillist.Count() != 0)
            {
                objStock = StockRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (StockDetaillist.Select(j => j.StockID).Contains(i.StockID)));

                if (objStock != null)
                {
                    ConcernStockDetail = StockDetails.FirstOrDefault(i => i.StockID == objStock.StockID && (i.Status == (int)EnumStockStatus.Sold || i.Status == (int)EnumStockStatus.Damage) && i.IMENO.Equals(IMEINO));
                    if (ConcernStockDetail != null)
                    {

                        objSOrderDetail = SOrderDetailRepository.All.Where(i => i.SDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();

                        bool IsReplaceOrder = false;
                        if (objSOrderDetail == null)//Replace Order
                        {
                            objSOrderDetail = SOrderDetailRepository.All.Where(i => i.RStockDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();
                            IsReplaceOrder = true;
                        }

                        if (objSOrderDetail != null) //Sales 
                        {
                            var objSorder = IsReplaceOrder ? SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.RepOrderID) : SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.SOrderID);

                            var SOrderDetails = IsReplaceOrder ? SOrderDetailRepository.All.Where(i => i.RepOrderID == objSorder.SOrderID && i.IsProductReturn == 0) : SOrderDetailRepository.All.Where(i => i.SOrderID == objSorder.SOrderID && i.IsProductReturn == 0);
                            List<AdvanceSOrderDetail> SOResult = new List<AdvanceSOrderDetail>();
                            if (!IsReplaceOrder)
                            {

                                SOResult = (from sod in SOrderDetails
                                            join so in SOrderRepository.All on sod.SOrderID equals so.SOrderID
                                            join p in Products on sod.ProductID equals p.ProductID
                                            join SD in StockDetails on sod.SDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = sod.Quantity,
                                                SalesRate = (((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity) - ((so.TDAmount + so.AdjAmount) / (so.GrandTotal - so.NetDiscount + so.TDAmount)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity))) * sod.Quantity,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }
                            else //Replace Order
                            {
                                SOResult = (from SOD in SOrderDetails
                                            join p in Products on SOD.ProductID equals p.ProductID
                                            join SD in StockDetails on SOD.RStockDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = SOD.Quantity,
                                                SalesRate = SOD.UnitPrice,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }

                            var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                            SOResult.Remove(SelectedIMEI);
                            SOResult.Insert(0, SelectedIMEI);

                            advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                            var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objSorder.CustomerID);
                            advanceSearchModel.CustomerCode = customer.Code;
                            advanceSearchModel.CustomerName = customer.Name;
                            advanceSearchModel.SalesDate = objSorder.InvoiceDate;
                            advanceSearchModel.InvoiceNo = objSorder.InvoiceNo;
                        }
                        else //Credit Sales
                        {
                            objCreditSaleDetail = CreditSalesDetails.Where(i => i.StockDetailID == ConcernStockDetail.SDetailID).OrderByDescending(i => i.CreditSaleDetailsID).FirstOrDefault();
                            if (objCreditSaleDetail != null)
                            {
                                var objCreditSOrder = CreditSales.FirstOrDefault(i => i.CreditSalesID == objCreditSaleDetail.CreditSalesID);

                                var CreditSOrderDetails = CreditSalesDetails.Where(i => i.CreditSalesID == objCreditSOrder.CreditSalesID);

                                var SOResult = (from SOD in CreditSOrderDetails
                                                join p in Products on SOD.ProductID equals p.ProductID
                                                join SD in StockDetails on SOD.StockDetailID equals SD.SDetailID
                                                select new AdvanceSOrderDetail
                                                {
                                                    ProductCode = p.Code,
                                                    ProductName = p.ProductName,
                                                    Quantity = SOD.Quantity,
                                                    SalesRate = SOD.UnitPrice,
                                                    IMEI = SD.IMENO
                                                }).ToList();
                                var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                                SOResult.Remove(SelectedIMEI);
                                SOResult.Insert(0, SelectedIMEI);

                                advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                                var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objCreditSOrder.CustomerID);
                                advanceSearchModel.CustomerCode = customer.Code;
                                advanceSearchModel.CustomerName = customer.Name;
                                advanceSearchModel.SalesDate = objCreditSOrder.IssueDate;
                                advanceSearchModel.InvoiceNo = objCreditSOrder.InvoiceNo;
                            }
                        }

                    }
                }

            }

            //Cheack if Godown Transfer in same concern
            //06-10-2021 
            //** Niloy**//

            StockDetaillist_ = StockDetails.Where(i => i.IMENO.Equals(IMEINO) && (i.Status == (int)EnumStockStatus.Damage || i.Status == (int)EnumStockStatus.Sold) && i.POrderDetailID == 0);

            if (StockDetaillist_ != null || StockDetaillist_.Count() != 0 && objSOrderDetail == null)
            {
                objStock = StockRepository.All.ToList().LastOrDefault(i => i.ConcernID == ConcernID && (StockDetaillist_.Select(j => j.StockID).Contains(i.StockID)));


                if (objStock != null)
                {
                    ConcernStockDetail = StockDetails.FirstOrDefault(i => i.StockID == objStock.StockID && (i.Status == (int)EnumStockStatus.Sold || i.Status == (int)EnumStockStatus.Damage) && i.IMENO.Equals(IMEINO) && i.POrderDetailID == 0);
                    if (ConcernStockDetail != null)
                    {

                        objSOrderDetail = SOrderDetailRepository.All.Where(i => i.SDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();

                        bool IsReplaceOrder = false;
                        if (objSOrderDetail == null)//Replace Order
                        {
                            objSOrderDetail = SOrderDetailRepository.All.Where(i => i.RStockDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();
                            IsReplaceOrder = true;
                        }

                        if (objSOrderDetail != null) //Sales 
                        {
                            var objSorder = IsReplaceOrder ? SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.RepOrderID) : SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.SOrderID);

                            var SOrderDetails = IsReplaceOrder ? SOrderDetailRepository.All.Where(i => i.RepOrderID == objSorder.SOrderID && i.IsProductReturn == 0) : SOrderDetailRepository.All.Where(i => i.SOrderID == objSorder.SOrderID && i.IsProductReturn == 0);
                            List<AdvanceSOrderDetail> SOResult = new List<AdvanceSOrderDetail>();
                            if (!IsReplaceOrder)
                            {

                                SOResult = (from sod in SOrderDetails
                                            join so in SOrderRepository.All on sod.SOrderID equals so.SOrderID
                                            join p in Products on sod.ProductID equals p.ProductID
                                            join SD in StockDetails on sod.SDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = sod.Quantity,
                                                SalesRate = (((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity) - ((so.TDAmount + so.AdjAmount) / (so.GrandTotal - so.NetDiscount + so.TDAmount)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity))) * sod.Quantity,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }
                            else //Replace Order
                            {
                                SOResult = (from SOD in SOrderDetails
                                            join p in Products on SOD.ProductID equals p.ProductID
                                            join SD in StockDetails on SOD.RStockDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = SOD.Quantity,
                                                SalesRate = SOD.UnitPrice,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }

                            var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                            SOResult.Remove(SelectedIMEI);
                            SOResult.Insert(0, SelectedIMEI);

                            advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                            var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objSorder.CustomerID);
                            advanceSearchModel.CustomerCode = customer.Code;
                            advanceSearchModel.CustomerName = customer.Name;
                            advanceSearchModel.SalesDate = objSorder.InvoiceDate;
                            advanceSearchModel.InvoiceNo = objSorder.InvoiceNo;
                        }
                        else //Credit Sales
                        {
                            objCreditSaleDetail = CreditSalesDetails.Where(i => i.StockDetailID == ConcernStockDetail.SDetailID).OrderByDescending(i => i.CreditSaleDetailsID).FirstOrDefault();
                            if (objCreditSaleDetail != null)
                            {
                                var objCreditSOrder = CreditSales.FirstOrDefault(i => i.CreditSalesID == objCreditSaleDetail.CreditSalesID);

                                var CreditSOrderDetails = CreditSalesDetails.Where(i => i.CreditSalesID == objCreditSOrder.CreditSalesID);

                                var SOResult = (from SOD in CreditSOrderDetails
                                                join p in Products on SOD.ProductID equals p.ProductID
                                                join SD in StockDetails on SOD.StockDetailID equals SD.SDetailID
                                                select new AdvanceSOrderDetail
                                                {
                                                    ProductCode = p.Code,
                                                    ProductName = p.ProductName,
                                                    Quantity = SOD.Quantity,
                                                    SalesRate = SOD.UnitPrice,
                                                    IMEI = SD.IMENO
                                                }).ToList();
                                var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                                SOResult.Remove(SelectedIMEI);
                                SOResult.Insert(0, SelectedIMEI);

                                advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                                var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objCreditSOrder.CustomerID);
                                advanceSearchModel.CustomerCode = customer.Code;
                                advanceSearchModel.CustomerName = customer.Name;
                                advanceSearchModel.SalesDate = objCreditSOrder.IssueDate;
                                advanceSearchModel.InvoiceNo = objCreditSOrder.InvoiceNo;
                            }
                        }

                    }
                }
            }
            #endregion

            #region Sales Return
            var Return = (from r in ROrders
                          join c in CustomerRepository.All on r.CustomerID equals c.CustomerID
                          join rd in ROrderDetails on r.ROrderID equals rd.ROrderID
                          join sd in StockDetailRepository.All on rd.StockDetailID equals sd.SDetailID
                          join sod in SOrderDetailRepository.All on sd.SDetailID equals sod.SDetailID
                          join so in SOrderRepository.All on sod.SOrderID equals so.SOrderID
                          join p in Products on sd.ProductID equals p.ProductID
                          where sd.IMENO.Equals(IMEINO)
                          select new AdvanceSOrderDetail
                          {
                              CustomerID = r.CustomerID,
                              CustomerCode = c.Code,
                              ReturnDate = r.ReturnDate,
                              ReturnInvoiceNo = r.InvoiceNo,
                              SalesRate = (((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity) - ((so.TDAmount + so.AdjAmount) / (so.GrandTotal - so.NetDiscount + so.TDAmount)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity))) * sod.Quantity,
                              Date = so.InvoiceDate,
                              InvoiceNo = so.InvoiceNo,
                              CustomerName = c.Name,
                              IMEI = sd.IMENO,
                              ProductCode = p.Code,
                              ProductName = p.ProductName
                          }).ToList();
            if (Return.Count() > 0)
                advanceSearchModel.AdvanceSalesReturns.AddRange(Return);

            #endregion

            #region Replacement Orders
            var Replacements = (from so in SOrderRepository.All
                                join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                                join sd in StockDetailRepository.All on sod.SDetailID equals sd.SDetailID
                                join rep in SOrderRepository.All on sod.RepOrderID equals rep.SOrderID
                                join p in Products on sd.ProductID equals p.ProductID
                                where sd.IMENO.Equals(IMEINO)
                                select new AdvanceSOrderDetail
                                {
                                    CustomerID = so.CustomerID,
                                    CustomerCode = c.Code,
                                    ReturnDate = rep.InvoiceDate,
                                    ReturnInvoiceNo = rep.InvoiceNo,
                                    SalesRate = (((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity) - ((so.TDAmount + so.AdjAmount) / (so.GrandTotal - so.NetDiscount + so.TDAmount)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity))) * sod.Quantity,
                                    Date = so.InvoiceDate,
                                    InvoiceNo = so.InvoiceNo,
                                    CustomerName = c.Name,
                                    IMEI = sd.IMENO,
                                    ProductCode = p.Code,
                                    ProductName = p.ProductName,
                                    ReplaceProductType = sod.ReplaceProductType.ToString()
                                }).ToList();
            if (Replacements.Count() > 0)
                advanceSearchModel.AdvanceReplacements.AddRange(Replacements);

            #endregion

            #region Transfer history
            var transfer = (from t in TransferRepository.GetAll()
                            join td in TransferDetailRepository.All on t.TransferID equals td.TransferID
                            join sis in SisterConcernRepository.GetAll() on t.FromConcernID equals sis.ConcernID
                            join tsis in SisterConcernRepository.GetAll() on t.ToConcernID equals tsis.ConcernID
                            join p in productRepository.GetAll() on td.ToProductID equals p.ProductID
                            where td.IMEI.Trim().Equals(IMEINO.Trim()) && (t.FromConcernID == ConcernID || t.ToConcernID == ConcernID)
                            && t.Status == (int)EnumTransferStatus.Transfer
                            select new AdvanceSOrderDetail
                            {
                                FromConcernName = sis.Name,
                                ToConcernName = tsis.Name,
                                IMEI = td.IMEI,
                                ProductCode = p.Code,
                                ProductName = p.ProductName,
                                Quantity = td.Quantity,
                                SalesRate = td.PRate,
                                Status = t.Status,
                                Date = t.TransferDate,
                                TransferNo = t.TransferNo
                            }).ToList();
            if (transfer.Count() > 0)
                advanceSearchModel.AdvanceTransferDetails = transfer;
            #endregion

            return advanceSearchModel;
        }

        public static AdvanceSearchModel AdvanceSearchByIMEINew(this IBaseRepository<POrder> _porderRepository, int ConcernID, string IMEINO)
        {
            IMEINO = IMEINO.Trim();
            AdvanceSearchModel advanceSearchModel = new AdvanceSearchModel();

            #region all purchase history

            List<AdvancePODetail> poList = _porderRepository.ExecSP<AdvancePODetail>("GetPOForAdvanceSearch @IMEI, @ConcernId",
                       new SqlParameter("IMEI", SqlDbType.NVarChar) { Value = IMEINO },
                       new SqlParameter("ConcernId", SqlDbType.Int) { Value = ConcernID }
                        ).ToList();

            if (poList.Any())
            {
                AdvancePODetail selectedIMEI = poList.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                poList.Remove(selectedIMEI);
                poList.Insert(0, selectedIMEI);
                advanceSearchModel.AdvancePODetails.AddRange(poList);
                advanceSearchModel.ChallanNo = selectedIMEI.ChallanNo;
                advanceSearchModel.PurchaseDate = selectedIMEI.OrderDate;
                advanceSearchModel.SupplierCode = selectedIMEI.SupCode;
                advanceSearchModel.SupplierName = selectedIMEI.SupplierName;

            }

            #endregion

            #region all sales history
            List<AdvanceSOrderDetail> soList = _porderRepository.ExecSP<AdvanceSOrderDetail>("GetSOForAdvanceSearch @IMEI, @ConcernId",
                       new SqlParameter("IMEI", SqlDbType.NVarChar) { Value = IMEINO },
                       new SqlParameter("ConcernId", SqlDbType.Int) { Value = ConcernID }
                        ).ToList();


            if (soList.Any())
            {
                AdvanceSOrderDetail selectedIMEI = soList.OrderByDescending(s => s.SalesDate).ThenByDescending(s => s.InvoiceNo).FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                soList.Remove(selectedIMEI);
                soList.Insert(0, selectedIMEI);
                advanceSearchModel.AdvanceSOrderDetails.Add(selectedIMEI);
                advanceSearchModel.CustomerCode = selectedIMEI.CustomerCode;
                advanceSearchModel.CustomerName = selectedIMEI.CustomerName;
                advanceSearchModel.SalesDate = selectedIMEI.SalesDate;
                advanceSearchModel.InvoiceNo = selectedIMEI.InvoiceNo;

            }

            #endregion

            #region all sales return history
            string sReturnQuery = string.Format(@"SELECT r.CustomerID, c.Code CustomerCode, c.Name CustomerName, r.ReturnDate, 
                                                r.InvoiceNo ReturnInvoiceNo, 
                                                ((((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity) - ((so.TDAmount + so.AdjAmount) / (so.GrandTotal - so.NetDiscount + so.TDAmount)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / sod.Quantity))) * sod.Quantity) SalesRate,
                                                so.InvoiceDate Date, so.InvoiceNo, sd.IMENO IMEI, p.Code ProductCode, P.ProductName
                                                FROM ROrders r
                                                JOIN ROrderDetails rd ON r.ROrderID = rd.ROrderID
                                                JOIN StockDetails sd ON rd.StockDetailID = sd.SDetailID
                                                JOIN SOrderDetails sod ON sd.SDetailID = sod.SDetailID
                                                JOIN SOrders so ON sod.SOrderID = so.SOrderID
                                                JOIN Products p ON sd.ProductID = P.ProductID
                                                LEFT JOIN Customers c ON r.CustomerID = c.CustomerID
                                                WHERE sod.IsProductReturn = 1 AND sd.IMENO = @IMEI AND r.ConcernID = @ConcernId");
            List<AdvanceSOrderDetail> salesReturn = _porderRepository.ExecSP<AdvanceSOrderDetail>(sReturnQuery,
                                                new SqlParameter("IMEI", SqlDbType.NVarChar) { Value = IMEINO },
                                                new SqlParameter("ConcernId", SqlDbType.Int) { Value = ConcernID }).ToList();
            if (salesReturn.Any())
                advanceSearchModel.AdvanceSalesReturns.AddRange(salesReturn);
            #endregion

            #region all replacement history
            string replaceQuery = string.Format(@"SELECT 
                    so.CustomerID, c.Code CustomerCode, c.Name CustomerName,rep.InvoiceDate ReturnDate, rep.InvoiceNo ReturnInvoiceNo,
                    ((((sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / NULLIF(sod.Quantity, 0)) - ((so.TDAmount + so.AdjAmount) / NULLIF((so.GrandTotal - so.NetDiscount + so.TDAmount), 0)) * (sod.UnitPrice - (sod.PPOffer + sod.PPDAmount) / NULLIF(sod.Quantity, 0)))) * sod.Quantity) SalesRate,
                    so.InvoiceDate Date, so.InvoiceNo, sd.IMENO IMEI, P.Code ProductCode, P.ProductName, 
                    CASE 
	                    WHEN sod.ReplaceProductType = 2 THEN 'No Damage'
	                    WHEN sod.ReplaceProductType = 1 THEN 'Damage'
                    END AS ReplaceProductType
                     FROM SOrders so
                    JOIN SOrderDetails sod ON so.SOrderID = sod.SOrderID
                    JOIN StockDetails sd ON sod.SDetailID = sd.SDetailID
                    JOIN SOrders rep ON sod.RepOrderID = rep.SOrderID
                    JOIN Products p ON sd.ProductID = p.ProductID
                    LEFT JOIN Customers c ON so.CustomerID = c.CustomerID
                    WHERE sd.IMENO = @IMEI AND so.ConcernID = @ConcernId");

            List<AdvanceSOrderDetail> salesReplacement = _porderRepository.ExecSP<AdvanceSOrderDetail>(replaceQuery,
                                                new SqlParameter("IMEI", SqlDbType.NVarChar) { Value = IMEINO },
                                                new SqlParameter("ConcernId", SqlDbType.Int) { Value = ConcernID }).ToList();

            if (salesReplacement.Any())
                advanceSearchModel.AdvanceReplacements.AddRange(salesReplacement);
            #endregion

            #region all transfer history
            string transferQuery = string.Format(@"SELECT 
                                    sis.Name FromConcernName, tsis.Name ToConcernName, td.IMEI, p.Code ProductCode, p.ProductName, td.Quantity, td.PRate SalesRate, t.Status, 
                                    t.TransferDate Date, t.TransferNo
                                     FROM Transfers t
                                    JOIN TransferDetails td ON t.TransferID = td.TransferID
                                    JOIN SisterConcerns sis ON T.FromConcernID = sis.ConcernID
                                    JOIN SisterConcerns tsis ON T.ToConcernID = tsis.ConcernID
                                    JOIN Products p ON td.ToProductID = P.ProductID
                                    WHERE td.IMEI = @IMEI AND (T.FromConcernID = @ConcernId OR T.ToConcernID = @ConcernId) AND T.Status = 1");

            List<AdvanceSOrderDetail> transferList = _porderRepository.ExecSP<AdvanceSOrderDetail>(transferQuery,
                                                new SqlParameter("IMEI", SqlDbType.NVarChar) { Value = IMEINO },
                                                new SqlParameter("ConcernId", SqlDbType.Int) { Value = ConcernID }).ToList();

            if (transferList.Any())
                advanceSearchModel.AdvanceTransferDetails.AddRange(transferList);
            #endregion

            return advanceSearchModel;
        }


        public static List<ProductWisePurchaseModel> ProductWisePurchaseReport(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Supplier> SupplierRepository, int ConcernID,
            int SupplierID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            List<POrder> POrders = new List<POrder>();
            if (SupplierID != 0)
                POrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.ConcernID == ConcernID && i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType).ToList();
            else
                POrders = POrderRepository.All.Where(i => i.ConcernID == ConcernID && i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType).ToList();

            var POrderDetails = POrderDetailRepository.All;
            var Suppliers = SupplierRepository.All;
            var Products = ProductRepository.All;

            var result = from PO in POrders
                         join POD in POrderDetails on PO.POrderID equals POD.POrderID
                         join S in Suppliers on PO.SupplierID equals S.SupplierID
                         join P in Products on POD.ProductID equals P.ProductID
                         select new ProductWisePurchaseModel
                         {
                             Date = PO.OrderDate,
                             SupplierCode = S.Code,
                             SupplierName = S.Name,
                             Address = S.Address,
                             Mobile = S.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = POD.Quantity,
                             PurchaseRate = POD.UnitPrice,
                             TotalAmount = POD.TAmount
                         };

            var fresult = from r in result
                          group r by new { r.Date, r.SupplierCode, r.SupplierName, r.Address, r.Mobile, r.ProductName, r.PurchaseRate } into g
                          select new ProductWisePurchaseModel
                          {
                              Date = g.Key.Date,
                              SupplierCode = g.Key.SupplierCode,
                              SupplierName = g.Key.SupplierName,
                              Address = g.Key.Address,
                              Mobile = g.Key.Mobile,
                              ProductName = g.Key.ProductName,
                              PurchaseRate = g.Key.PurchaseRate,
                              Quantity = g.Sum(i => i.Quantity),
                              TotalAmount = g.Sum(i => i.TotalAmount)
                          };

            return fresult.ToList();


        }

        public static List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReportNew(this IBaseRepository<POrder> POrderRepository,
            IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Supplier> SupplierRepository,
             int CompanyID, int CategoryID, int ProductID,
             DateTime fromDate, DateTime toDate,
             EnumPurchaseType PurchaseType, int SupplierID)
        {
            //IQueryable<Product> Products = null;
            //IQueryable<Company> Companies = null;
            //IQueryable<Category> Categories = null;
            //IQueryable<POrder> POrders = null;
            //IQueryable<SisterConcern> concerns = null;
            IQueryable<Supplier> Suppliers = null;
            var Products = ProductRepository.All;

            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);
            if (SupplierID != 0)
            {
                Suppliers = SupplierRepository.All.Where(i => i.SupplierID == SupplierID);
            }
            else
            {
                Suppliers = SupplierRepository.All;

            }

            var POrderDetails = POrderDetailRepository.All;
            var POrders = POrderRepository.All.Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType);

            var result = from PO in POrders
                         join Sup in Suppliers on PO.SupplierID equals Sup.SupplierID
                         join POD in POrderDetails on PO.POrderID equals POD.POrderID
                         join P in Products on POD.ProductID equals P.ProductID
                         join com in CompanyRepository.All on P.CompanyID equals com.CompanyID
                         join cate in categoryRepository.All on P.CategoryID equals cate.CategoryID
                         select new ProductWisePurchaseModel
                         {
                             POrderID = PO.POrderID,
                             Date = PO.OrderDate,
                             ChallanNo = PO.ChallanNo,
                             GrandTotal = PO.GrandTotal,
                             ProductName = P.ProductName,
                             CompanyName = com.Name,
                             CategoryName = cate.Description,
                             Quantity = POD.Quantity,
                             PurchaseRate = (POD.UnitPrice - (((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount + PO.AdjAmount)))),
                             MRP = POD.MRPRate,
                             TotalAmount = POD.TAmount,
                             TotalMRP = ((POD.UnitPrice - (((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount + PO.AdjAmount)))) * POD.Quantity),
                             PPDISAmt = POD.PPDISAmt,
                             PPOffer = POD.PPOffer,
                             RecAmt = PO.RecAmt,
                             PaymentDue = PO.PaymentDue,
                             NetDiscount = PO.NetDiscount,
                             NetTotal = PO.TotalAmt,
                             IMEIs = POD.POProductDetails.Select(i => i.IMENO).ToList(),
                             InvoiceNo = PO.InvoiceNo
                         };

            return result.ToList();


        }

        public static List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReport(this IBaseRepository<POrder> POrderRepository,
            IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<SisterConcern> sisterRepo, IBaseRepository<Supplier> SupplierRepository,
             int CompanyID, int CategoryID, int ProductID,
             DateTime fromDate, DateTime toDate,
             EnumPurchaseType PurchaseType, bool IsAdminReport, int SelectedConcernID, int SupplierID)
        {
            IQueryable<Product> Products = null;
            IQueryable<Company> Companies = null;
            IQueryable<Category> Categories = null;
            IQueryable<POrder> POrders = null;
            IQueryable<SisterConcern> concerns = null;
            IQueryable<Supplier> Suppliers = null;
            if (IsAdminReport)
            {
                if (SelectedConcernID > 0)
                {
                    POrders = POrderRepository.GetAll()
                            .Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate
                            && i.Status == (int)PurchaseType && i.ConcernID == SelectedConcernID);

                    Products = ProductRepository.GetAll().Where(i => i.ConcernID == SelectedConcernID);
                    Companies = CompanyRepository.GetAll().Where(i => i.ConcernID == SelectedConcernID);
                    Categories = categoryRepository.GetAll().Where(i => i.ConcernID == SelectedConcernID);
                    concerns = sisterRepo.GetAll().Where(i => i.ConcernID == SelectedConcernID);
                    Suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == SelectedConcernID);

                }
                else
                {
                    POrders = POrderRepository.GetAll()
                            .Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate
                            && i.Status == (int)PurchaseType);

                    Products = ProductRepository.GetAll();
                    concerns = sisterRepo.GetAll();
                    Companies = CompanyRepository.GetAll();
                    Categories = categoryRepository.GetAll();
                    Suppliers = SupplierRepository.GetAll();
                }
            }
            else
            {

                POrders = POrderRepository.All
                         .Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate
                         && i.Status == (int)PurchaseType);

                Products = ProductRepository.All;
                if (CompanyID != 0)
                    Products = Products.Where(i => i.CompanyID == CompanyID);
                if (CategoryID != 0)
                    Products = Products.Where(i => i.CategoryID == CategoryID);
                if (ProductID != 0)
                    Products = Products.Where(i => i.ProductID == ProductID);
                if (SupplierID != 0)
                {
                    Suppliers = SupplierRepository.All.Where(i => i.SupplierID == SupplierID);
                }

                concerns = sisterRepo.All;
                Companies = CompanyRepository.All;
                Categories = categoryRepository.All;
                Suppliers = SupplierRepository.All;
            }


            var POrderDetails = POrderDetailRepository.All;

            var result = from PO in POrders
                         join s in concerns on PO.ConcernID equals s.ConcernID
                         join POD in POrderDetails on PO.POrderID equals POD.POrderID
                         join P in Products on POD.ProductID equals P.ProductID
                         join com in Companies on P.CompanyID equals com.CompanyID
                         join cate in Categories on P.CategoryID equals cate.CategoryID
                         join Sup in Suppliers on PO.SupplierID equals Sup.SupplierID
                         select new ProductWisePurchaseModel
                         {
                             POrderID = PO.POrderID,
                             Date = PO.OrderDate,
                             ChallanNo = PO.ChallanNo,
                             GrandTotal = PO.GrandTotal,
                             ProductName = P.ProductName,
                             CompanyName = com.Name,
                             CategoryName = cate.Description,
                             Quantity = POD.Quantity,
                             PurchaseRate = POD.UnitPrice,
                             MRP = POD.MRPRate,
                             TotalAmount = POD.TAmount,
                             TotalMRP = (POD.MRPRate * POD.Quantity),
                             PPDISAmt = POD.PPDISAmt,
                             PPOffer = POD.PPOffer,
                             RecAmt = PO.RecAmt,
                             PaymentDue = PO.PaymentDue,
                             NetDiscount = PO.NetDiscount,
                             NetTotal = PO.TotalAmt,
                             IMEIs = POD.POProductDetails.Select(i => i.IMENO).ToList(),
                             ConcenName = s.Name,
                             InvoiceNo = PO.InvoiceNo,
                             TotalPPDis = PO.NetDiscount - PO.TDiscount,
                             OnlyDisAmt = PO.TDiscount,
                             //AfterFlatDisPurchaseRate = ((((POD.Quantity > 0) ? (POD.UnitPrice - POD.PPDISAmt / POD.Quantity) : 0) - ((((POD.Quantity > 0) ? (POD.UnitPrice - POD.PPDISAmt / POD.Quantity) : 0) * (PO.TDiscount)) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount)))) - ((PO.TPQty > 0) ? (PO.AdjAmount / PO.TPQty) : 0)),
                             AfterFlatDisPurchaseRate = (POD.UnitPrice - (((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount + PO.AdjAmount))))
                             //(POD.UnitPrice - (((@FlatDis + @Adjustment) * POD.UnitPrice) / (@GrandTotal - @NetDiscount + (@FlatDis + @Adjustment))))

                         };

            return result.ToList();


        }

        /// <summary>
        /// For SRVisit Issued IMEI search
        /// </summary>
        /// <param name="POrderRepository"></param>
        /// <param name="POrderDetailRepository"></param>
        /// <param name="POProductDetailRepository"></param>
        /// <param name="productRepository"></param>
        /// <param name="StockDetailRepository"></param>
        /// <param name="StockRepository"></param>
        /// <param name="SupplierRepository"></param>
        /// <param name="SRVisitRepository"></param>
        /// <param name="SRVisitDetailRepository"></param>
        /// <param name="SRVProductDetailRepository"></param>
        /// <param name="EmployeeRepository"></param>
        /// <param name="ConcernID"></param>
        /// <param name="IMEINO"></param>
        /// <returns></returns>
        public static AdvanceSearchModel SRVisitAdvanceSearchByIMEI(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Supplier> SupplierRepository, IBaseRepository<SRVisit> SRVisitRepository,
            IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
            IBaseRepository<Employee> EmployeeRepository,
            int ConcernID, string IMEINO)
        {
            IMEINO = IMEINO.Trim();
            AdvanceSearchModel advanceSearchModel = new AdvanceSearchModel();
            SRVProductDetail ObjSRVProductDetail = null;
            SRVisitDetail objSRVisitDetail = null;
            var Products = productRepository.All;
            var POrderDetails = POrderDetailRepository.All;
            var POProductDetails = POProductDetailRepository.All;
            var POProductDetail = POProductDetailRepository.All.Where(i => i.IMENO.Equals(IMEINO));
            var SRVisits = SRVisitRepository.All;
            var SRVisitDetails = SRVisitDetailRepository.All;
            var SRVProductDetails = SRVProductDetailRepository.All;

            if (POProductDetail != null)
            {
                var POrderDetail = POrderDetailRepository.All.Where(i => (POProductDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)) && (POProductDetail.Select(j => j.ProductID).Contains(i.ProductID)) && (POProductDetail.Select(j => j.ColorID).Contains(i.ColorID)));
                var POrder = POrderRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (POrderDetail.Select(j => j.POrderID).Contains(i.POrderID)));
                if (POrder != null)
                {
                    var concernPOrderDetail = POrderDetails.Where(i => i.POrderID == POrder.POrderID);
                    var ConcernPOProductDetail = POProductDetails.Where(i => (concernPOrderDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)));

                    var POResult = (from POD in concernPOrderDetail
                                    join POPD in ConcernPOProductDetail on POD.POrderDetailID equals POPD.POrderDetailID
                                    join P in Products on POD.ProductID equals P.ProductID
                                    select new AdvancePODetail
                                    {
                                        ProductCode = P.Code,
                                        ProductName = P.ProductName,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = 1,
                                        IMEI = POPD.IMENO
                                    }).ToList();
                    var SelectedIMEI = POResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                    POResult.Remove(SelectedIMEI);
                    POResult.Insert(0, SelectedIMEI);
                    advanceSearchModel.AdvancePODetails.AddRange(POResult);
                    advanceSearchModel.ChallanNo = POrder.ChallanNo;
                    advanceSearchModel.PurchaseDate = POrder.OrderDate;
                    var Supplier = SupplierRepository.FindBy(i => i.SupplierID == POrder.SupplierID).FirstOrDefault();
                    advanceSearchModel.SupplierCode = Supplier.Code;
                    advanceSearchModel.SupplierName = Supplier.Name;
                }

            }

            ObjSRVProductDetail = SRVProductDetails.FirstOrDefault(i => i.IMENO.Equals(IMEINO) && (i.Status == (int)EnumStockStatus.Sold || i.Status == (int)EnumStockStatus.Stock));

            if (ObjSRVProductDetail != null)
            {
                objSRVisitDetail = SRVisitDetails.FirstOrDefault(i => i.SRVisitDID == ObjSRVProductDetail.SRVisitDID);
                if (objSRVisitDetail != null)
                {
                    var ObjSRVisit = SRVisits.FirstOrDefault(i => i.SRVisitID == objSRVisitDetail.SRVisitID && i.Status == 1);
                    if (ObjSRVisit != null)
                    {
                        var SRVisitDetailList = SRVisitDetails.Where(i => i.SRVisitID == ObjSRVisit.SRVisitID);
                        var SRVProductDetailList = SRVProductDetails.Where(i => SRVisitDetailList.Select(j => j.SRVisitDID).Contains(i.SRVisitDID) && i.Status != (int)EnumStockStatus.Return);
                        var Result = (from SRVPD in SRVProductDetailList
                                      join P in Products on SRVPD.ProductID equals P.ProductID
                                      select new AdvanceSOrderDetail
                                      {
                                          ProductCode = P.Code,
                                          ProductName = P.ProductName,
                                          Quantity = 1,
                                          IMEI = SRVPD.IMENO,
                                          Status = SRVPD.Status
                                      }).ToList();

                        var SearchedIMEI = Result.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                        Result.Remove(SearchedIMEI);
                        Result.Insert(0, SearchedIMEI);

                        advanceSearchModel.AdvanceSOrderDetails.AddRange(Result);
                        advanceSearchModel.SalesDate = ObjSRVisit.VisitDate;
                        advanceSearchModel.InvoiceNo = ObjSRVisit.ChallanNo;
                        var Employee = EmployeeRepository.FindBy(i => i.EmployeeID == ObjSRVisit.EmployeeID).FirstOrDefault();
                        advanceSearchModel.CustomerCode = Employee.Code;
                        advanceSearchModel.CustomerName = Employee.Name;
                    }
                }
            }

            return advanceSearchModel;
        }


        public static POProductDetail GetDamageReturnPOPDetail(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
            IBaseRepository<POProductDetail> POProductDetailRepository, string DamageIMEI, int ProductID, int ColorID)
        {
            var POPD = from po in POrderRepository.All
                       join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                       join popd in POProductDetailRepository.All on pod.POrderDetailID equals popd.POrderDetailID
                       where (po.Status == (int)EnumPurchaseType.DamageReturn && popd.IMENO.Equals(DamageIMEI.Trim()) && popd.IsDamageReplaced != 1 && popd.ProductID == ProductID && popd.ColorID == ColorID)
                       select popd;

            return POPD.OrderByDescending(i => i.POrderDetailID).FirstOrDefault();
        }

        public static IEnumerable<ProductWisePurchaseModel> GetDamagePOReport(
                            this IBaseRepository<POrder> purchaseOrderRepository,
                            IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<Product> productRepository,
                            IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository,
                            DateTime fromDate, DateTime toDate, int SupplierID)
        {
            IQueryable<POrder> Porders = null;
            if (SupplierID != 0)
                Porders = purchaseOrderRepository.All.Where(i => i.SupplierID == SupplierID);
            else
                Porders = purchaseOrderRepository.All;

            var oPurchaseDetailData = (from PO in Porders
                                       join POD in pOrderDetailRepository.All on PO.POrderID equals POD.POrderID
                                       join POP in poProductRepository.All on POD.POrderDetailID equals POP.POrderDetailID
                                       join DPOP in poProductRepository.All on POP.DamagePOPDID equals DPOP.POPDID
                                       join P in productRepository.All on POD.ProductID equals P.ProductID
                                       from C in colorRepository.All
                                       where (PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == (int)EnumPurchaseType.Purchase && PO.IsDamageOrder == 1)
                                       select new ProductWisePurchaseModel
                                       {
                                           POrderID = PO.POrderID,
                                           ChallanNo = PO.ChallanNo,
                                           Date = PO.OrderDate,
                                           GrandTotal = PO.GrandTotal,
                                           NetDiscount = PO.NetDiscount,
                                           FlatDiscount = PO.TDiscount,
                                           NetTotal = PO.TotalAmt,
                                           RecAmt = PO.RecAmt,
                                           PaymentDue = PO.PaymentDue,
                                           ProductID = P.ProductID,
                                           ProductName = P.ProductName,
                                           PurchaseRate = POD.UnitPrice,
                                           TotalAmount = POD.TAmount,
                                           PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                           CategoryName = P.Category.Description,
                                           IMENO = POP.IMENO,
                                           DamageIMEI = DPOP.IMENO,
                                           ColorName = C.Name,
                                           PPOffer = POD.PPOffer,
                                           Quantity = 1
                                       }).OrderByDescending(x => x.Date).ThenByDescending(i => i.ChallanNo).ToList();
            return oPurchaseDetailData;
        }


        public static List<ProductWisePurchaseModel> GetDamageReturnProductDetails(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
       IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<Color> ColorRepository,
            int ProductID, int ColorID)
        {
            try
            {
                IQueryable<POProductDetail> POPDetails = null;
                if (ProductID != 0 && ColorID != 0)
                    POPDetails = POProductDetailRepository.All.Where(i => i.ProductID == ProductID && i.ColorID == ColorID);
                else
                    POPDetails = POProductDetailRepository.All;

                var POPD = (from po in POrderRepository.All
                            join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                            join popd in POPDetails on pod.POrderDetailID equals popd.POrderDetailID
                            join p in ProductRepository.All on popd.ProductID equals p.ProductID
                            join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                            join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                            join col in ColorRepository.All on popd.ColorID equals col.ColorID
                            where (po.Status == (int)EnumPurchaseType.DamageReturn && popd.IsDamageReplaced != 1)
                            select new ProductWisePurchaseModel
                            {
                                ProductName = p.ProductName,
                                ProductCode = p.Code,
                                IMENO = popd.IMENO,
                                CategoryName = cat.Description,
                                CompanyName = com.Name,
                                ColorName = col.Name,
                                SRate = pod.SalesRate,
                                ColorID = pod.ColorID,
                                MRP = pod.MRPRate
                            });
                return POPD.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// Get all damage IMEIs which were sent to Company for replacement
        /// and which damage IMEIs were replaced by new IMEIs
        /// </summary>
        public static List<ProductWisePurchaseModel> DamageReturnProductDetailsReport(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                                    IBaseRepository<POProductDetail> POProductDetailRepository,
                                    IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
                                    IBaseRepository<Color> ColorRepository,
                                    int SupplierID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<POrder> POrders = null;
            if (SupplierID != 0)
                POrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.OrderDate >= fromDate && i.OrderDate <= toDate);
            else
                POrders = POrderRepository.All.Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate);


            var POPD = (from po in POrders
                        join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                        join popd in POProductDetailRepository.All on pod.POrderDetailID equals popd.POrderDetailID
                        join p in ProductRepository.All on popd.ProductID equals p.ProductID
                        join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                        join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                        join col in ColorRepository.All on popd.ColorID equals col.ColorID
                        join newPOP in POProductDetailRepository.All on popd.POPDID equals newPOP.DamagePOPDID into lj
                        from newPOP in lj.DefaultIfEmpty()
                        where (po.Status == (int)EnumPurchaseType.DamageReturn)
                        select new ProductWisePurchaseModel
                        {
                            Date = po.OrderDate,
                            ChallanNo = po.ChallanNo,
                            POrderID = po.POrderID,
                            ProductName = p.ProductName,
                            ProductCode = p.Code,
                            DamageIMEI = popd.IMENO,
                            CategoryName = cat.Description,
                            CompanyName = com.Name,
                            ColorName = col.Name,
                            SRate = pod.SalesRate,
                            ColorID = pod.ColorID,
                            PurchaseRate = pod.MRPRate,
                            POPDID = popd.POPDID,
                            IsDamageReplaced = popd.IsDamageReplaced,
                            Quantity = 1,
                            IMENO = (newPOP == null ? "Pending" : newPOP.IMENO),
                            NetTotal = po.TotalAmt
                        });



            return POPD.ToList();
        }



        /// <summary>
        /// Date: 08-01-2019
        /// Author: aminul
        /// Admin Purchase report of  all concern
        /// </summary>
        public static IQueryable<ProductWisePurchaseModel> AdminPurchaseReport(this IBaseRepository<POrder> POrderRepository,
                        IBaseRepository<Supplier> SupplierRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                        int ConcernID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            IQueryable<Supplier> Suppliers = null;
            if (ConcernID != 0)
                Suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Suppliers = SupplierRepository.GetAll();

            var Result = from po in POrderRepository.GetAll()
                         join s in Suppliers on po.SupplierID equals s.SupplierID
                         join sis in SisterConcernRepository.GetAll() on po.ConcernID equals sis.ConcernID
                         where po.Status == (int)PurchaseType && (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                         select new ProductWisePurchaseModel
                         {
                             ConcenName = sis.Name,
                             SupplierCode = s.Code,
                             SupplierName = s.Name,
                             ChallanNo = po.ChallanNo,
                             Date = po.OrderDate,
                             GrandTotal = po.GrandTotal,
                             NetDiscount = po.NetDiscount,
                             TotalAmount = po.TotalAmt,
                             RecAmt = po.RecAmt,
                             PaymentDue = po.PaymentDue,
                             LaborCost = po.LaborCost
                         };
            return Result.OrderByDescending(i => i.Date);
        }


        public static List<LedgerAccountReportModel> SupplierLedger(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                             IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
                             IBaseRepository<Supplier> SupplierRepository, IBaseRepository<ApplicationUser> UserRepository,
                             IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Bank> BankRepository,
                             int SupplierID, DateTime fromDate, DateTime toDate)
        {
            List<LedgerAccountReportModel> ledgers = new List<LedgerAccountReportModel>();
            List<LedgerAccountReportModel> FinalLedgers = new List<LedgerAccountReportModel>();

            var Supplier = SupplierRepository.All.FirstOrDefault(i => i.SupplierID == SupplierID);

            #region Purchase
            var Purchases = from po in POrderRepository.All
                            join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                            join p in ProductRepository.All on pod.ProductID equals p.ProductID
                            join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where po.Status == (int)EnumPurchaseType.Purchase && po.SupplierID == SupplierID
                            select new
                            {
                                po.TotalAmt,
                                po.ChallanNo,
                                po.OrderDate,
                                po.RecAmt,
                                po.PaymentDue,
                                CreditAdjsFlatDis = po.TDiscount,
                                CreditAdjTotalPPdis = po.NetDiscount - po.TDiscount,
                                CreditAdj = po.AdjAmount + po.NetDiscount,
                                Credit = po.RecAmt,
                                CashCollectionAmt = po.RecAmt,
                                Debit = po.TotalAmt,
                                GrandTotal = po.GrandTotal,
                                pod.UnitPrice,
                                pod.TAmount,
                                pod.Quantity,
                                ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                EnteredBy = u == null ? string.Empty : u.UserName,
                                Remarks = string.Empty,
                                TotalAmtWD = po.TotalAmt,
                                TotalAdjDis = po.AdjAmount + po.NetDiscount,
                            };

            var VmPurchases = (from cs in Purchases
                               group cs by new { cs.Debit, cs.Credit, cs.CreditAdj, cs.GrandTotal, cs.CashCollectionAmt, cs.OrderDate, cs.ChallanNo, cs.EnteredBy, cs.PaymentDue, cs.CreditAdjsFlatDis, cs.CreditAdjTotalPPdis } into g
                               select new LedgerAccountReportModel
                               {
                                   VoucherType = "Purchase",
                                   InvoiceNo = g.Key.ChallanNo,
                                   Date = g.Key.OrderDate,
                                   EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                   ProductList = g.Select(i => i.ProductName).ToList(),
                                   Debit = g.Key.Debit,
                                   Credit = g.Key.Credit,
                                   CreditAdj = g.Key.CreditAdj,
                                   GrandTotal = g.Key.GrandTotal,
                                   InvoiceDue = g.Key.PaymentDue,
                                   CashCollectionAmt = g.Key.CashCollectionAmt,
                                   Quantity = g.Sum(i => i.Quantity),
                                   Balance = 0,
                                   CreditAdjFlatDis = g.Key.CreditAdjsFlatDis,
                                   CreditAdjTotalPPDis = g.Key.CreditAdjTotalPPdis,
                                   Remarks = g.Select(i => i.Remarks).FirstOrDefault(),
                                   TotalAmtWD = g.Select(i => i.TotalAmtWD).FirstOrDefault(),
                                   TotalAdjDis = g.Select(i => i.TotalAdjDis).FirstOrDefault(),
                               }).ToList();

            ledgers.AddRange(VmPurchases);
            #endregion

            #region Damage Purchase
            var DamagePurchases = from po in POrderRepository.All
                                  join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                  join p in ProductRepository.All on pod.ProductID equals p.ProductID
                                  join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                                  from u in lj.DefaultIfEmpty()
                                  where po.Status == (int)EnumPurchaseType.DamageOrder && po.SupplierID == SupplierID
                                  select new
                                  {
                                      po.TotalAmt,
                                      po.ChallanNo,
                                      po.OrderDate,
                                      po.RecAmt,
                                      po.PaymentDue,
                                      CreditAdjsFlatDis = po.TDiscount,
                                      CreditAdjTotalPPdis = po.NetDiscount - po.TDiscount,
                                      CreditAdj = po.AdjAmount + po.NetDiscount,
                                      Credit = po.RecAmt,
                                      CashCollectionAmt = po.RecAmt,
                                      Debit = po.TotalAmt,
                                      GrandTotal = po.GrandTotal,
                                      pod.UnitPrice,
                                      pod.TAmount,
                                      pod.Quantity,
                                      ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                      EnteredBy = u == null ? string.Empty : u.UserName,
                                      Remarks = string.Empty,
                                      TotalAmtWD = po.TotalAmt,
                                      TotalAdjDis = po.AdjAmount + po.NetDiscount,
                                  };

            var VmDamagePurchases = (from cs in DamagePurchases
                                     group cs by new { cs.Debit, cs.Credit, cs.CreditAdj, cs.GrandTotal, cs.CashCollectionAmt, cs.OrderDate, cs.ChallanNo, cs.EnteredBy, cs.PaymentDue, cs.CreditAdjsFlatDis, cs.CreditAdjTotalPPdis } into g
                                     select new LedgerAccountReportModel
                                     {
                                         VoucherType = "Damage Purchase",
                                         InvoiceNo = g.Key.ChallanNo,
                                         Date = g.Key.OrderDate,
                                         EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                         ProductList = g.Select(i => i.ProductName).ToList(),
                                         Debit = g.Key.Debit,
                                         Credit = g.Key.Credit,
                                         CreditAdj = g.Key.CreditAdj,
                                         GrandTotal = g.Key.GrandTotal,
                                         InvoiceDue = g.Key.PaymentDue,
                                         CashCollectionAmt = g.Key.CashCollectionAmt,
                                         Quantity = g.Sum(i => i.Quantity),
                                         Balance = 0,
                                         CreditAdjFlatDis = g.Key.CreditAdjsFlatDis,
                                         CreditAdjTotalPPDis = g.Key.CreditAdjTotalPPdis,
                                         Remarks = g.Select(i => i.Remarks).FirstOrDefault(),
                                         TotalAmtWD = g.Select(i => i.TotalAmtWD).FirstOrDefault(),
                                         TotalAdjDis = g.Select(i => i.TotalAdjDis).FirstOrDefault(),
                                     }).ToList();

            ledgers.AddRange(VmDamagePurchases);
            #endregion

            #region Purchases Return
            var PurchasesReturns = from po in POrderRepository.All
                                   join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                   join p in ProductRepository.All on pod.ProductID equals p.ProductID
                                   join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                                   from u in lj.DefaultIfEmpty()
                                   where po.Status == (int)EnumPurchaseType.ProductReturn && po.SupplierID == SupplierID
                                   select new
                                   {
                                       po.TotalAmt,
                                       po.ChallanNo,
                                       po.OrderDate,
                                       po.RecAmt,
                                       CreditAdj = po.AdjAmount + po.NetDiscount,
                                       Credit = po.TotalAmt,
                                       Return = po.TotalAmt - po.RecAmt,
                                       CashCollectionAmt = po.RecAmt,
                                       Debit = po.RecAmt,
                                       GrandTotal = po.GrandTotal,
                                       pod.UnitPrice,
                                       pod.TAmount,
                                       pod.Quantity,
                                       ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                       EnteredBy = u == null ? string.Empty : u.UserName,
                                       Remarks = po.Remarks
                                   };

            var VmPurchasesReturns = (from cs in PurchasesReturns
                                      group cs by new { cs.Debit, cs.Credit, cs.Return, cs.OrderDate, cs.ChallanNo, cs.EnteredBy } into g
                                      select new LedgerAccountReportModel
                                      {
                                          VoucherType = "PO Return",
                                          InvoiceNo = g.Key.ChallanNo,
                                          Date = g.Key.OrderDate,
                                          EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                          ProductList = g.Select(i => i.ProductName).ToList(),
                                          Debit = g.Key.Debit,
                                          Credit = g.Key.Credit,
                                          SalesReturn = g.Key.Return,
                                          Quantity = g.Sum(i => i.Quantity),
                                          Balance = 0,
                                          Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                      }).ToList();

            ledgers.AddRange(VmPurchasesReturns);
            #endregion

            #region Damage Purchases Return
            var DamagePurchasesReturns = from po in POrderRepository.All
                                         join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                         join p in ProductRepository.All on pod.ProductID equals p.ProductID
                                         join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                                         from u in lj.DefaultIfEmpty()
                                         where po.Status == (int)EnumPurchaseType.DamageReturn && po.SupplierID == SupplierID
                                         select new
                                         {
                                             po.TotalAmt,
                                             po.ChallanNo,
                                             po.OrderDate,
                                             po.RecAmt,
                                             CreditAdj = po.AdjAmount + po.NetDiscount,
                                             Credit = po.TotalAmt,
                                             Return = po.TotalAmt - po.RecAmt,
                                             CashCollectionAmt = po.RecAmt,
                                             Debit = po.RecAmt,
                                             GrandTotal = po.GrandTotal,
                                             pod.UnitPrice,
                                             pod.TAmount,
                                             pod.Quantity,
                                             ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.UnitType.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                             EnteredBy = u == null ? string.Empty : u.UserName,
                                             Remarks = po.Remarks
                                         };

            var VmDamagePurchasesReturns = (from cs in DamagePurchasesReturns
                                            group cs by new { cs.Debit, cs.Credit, cs.Return, cs.OrderDate, cs.ChallanNo, cs.EnteredBy } into g
                                            select new LedgerAccountReportModel
                                            {
                                                VoucherType = "Damage PO Return",
                                                InvoiceNo = g.Key.ChallanNo,
                                                Date = g.Key.OrderDate,
                                                EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                                ProductList = g.Select(i => i.ProductName).ToList(),
                                                Debit = g.Key.Debit,
                                                Credit = g.Key.Credit,
                                                SalesReturn = g.Key.Return,
                                                Quantity = g.Sum(i => i.Quantity),
                                                Balance = 0,
                                                Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                            }).ToList();

            ledgers.AddRange(VmDamagePurchasesReturns);
            #endregion

            #region Cash Delivery
            var CashDelivery = from cc in CashCollectionRepository.All
                               join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                               from u in lj.DefaultIfEmpty()
                               where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.ToCompany
                               select new LedgerAccountReportModel
                               {
                                   Date = (DateTime)cc.EntryDate,
                                   Debit = cc.InterestAmt,
                                   VoucherType = "Cash Delivery",
                                   Credit = cc.Amount + cc.AdjustAmt,
                                   CashCollectionAmt = cc.Amount,
                                   CreditAdj = cc.AdjustAmt,
                                   InvoiceNo = cc.ReceiptNo,
                                   EnteredBy = "Entered By: " + u.UserName,
                                   Remarks = cc.Remarks,
                                   CashCollectionIntAmt = cc.InterestAmt
                               };
            ledgers.AddRange(CashDelivery);
            #endregion

            #region Cash Delivery Return
            var CashDeliveryReturn = from cc in CashCollectionRepository.All
                                     join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                     from u in lj.DefaultIfEmpty()
                                     where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.CollectionReturn
                                     select new LedgerAccountReportModel
                                     {
                                         Date = (DateTime)cc.EntryDate,
                                         CashCollectionIntAmt = cc.InterestAmt,
                                         Credit = 0m,
                                         VoucherType = "Retrun from Supplier",
                                         Debit = cc.Amount + cc.AdjustAmt,
                                         CashCollectionReturn = 0m,
                                         CreditAdj = 0m,
                                         DebitAdj = cc.Amount + cc.AdjustAmt,
                                         InvoiceNo = cc.ReceiptNo,
                                         EnteredBy = "Entered By: " + u.UserName,
                                         Remarks = cc.Remarks
                                     };
            ledgers.AddRange(CashDeliveryReturn);
            #endregion

            #region Cash Delivery Debit Adj
            var CashDeliveryDebitAdj = from ccr in CashCollectionRepository.All
                                       join u in UserRepository.All on ccr.CreatedBy equals u.Id into lj
                                       from u in lj.DefaultIfEmpty()
                                       where ccr.SupplierID == SupplierID && ccr.TransactionType == EnumTranType.DebitAdjustment
                                       select new LedgerAccountReportModel
                                       {
                                           Date = (DateTime)ccr.EntryDate,
                                           Credit = 0m,
                                           VoucherType = "Cash Del. Debit Adj.",
                                           Debit = ccr.Amount + ccr.AdjustAmt,
                                           CashCollectionReturn = 0m,
                                           CreditAdj = 0m,
                                           DebitAdj = ccr.Amount + ccr.AdjustAmt,
                                           InvoiceNo = ccr.ReceiptNo,
                                           EnteredBy = "Entered By: " + u.UserName,
                                           Remarks = ccr.Remarks
                                       };
            ledgers.AddRange(CashDeliveryDebitAdj);
            #endregion

            #region Cash Delivery Credit Adj
            var CashDeliveryCreditAdj = from cc in CashCollectionRepository.All
                                        join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                        from u in lj.DefaultIfEmpty()
                                        where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.CreditAdjustment
                                        select new LedgerAccountReportModel
                                        {
                                            Date = (DateTime)cc.EntryDate,
                                            Debit = cc.InterestAmt,
                                            VoucherType = "Cash Del. Credit Adj.",
                                            Credit = cc.Amount + cc.AdjustAmt,
                                            CashCollectionAmt = 0m,
                                            CreditAdj = cc.Amount + cc.AdjustAmt,
                                            InvoiceNo = cc.ReceiptNo,
                                            EnteredBy = "Entered By: " + u.UserName,
                                            Remarks = cc.Remarks,
                                            InterestAmt = cc.InterestAmt
                                        };
            ledgers.AddRange(CashDeliveryCreditAdj);
            #endregion


            #region Price Protection
            var PriceProtection = from cc in CashCollectionRepository.All
                                  join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                  from u in lj.DefaultIfEmpty()
                                  where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.PriceProtectionForSupplier
                                  select new LedgerAccountReportModel
                                  {
                                      Date = (DateTime)cc.EntryDate,
                                      Debit = 0m,
                                      VoucherType = "Price Protection",
                                      Credit = cc.Amount + cc.AdjustAmt,
                                      CashCollectionAmt = cc.Amount,
                                      CreditAdj = cc.AdjustAmt,
                                      InvoiceNo = cc.ReceiptNo,
                                      EnteredBy = "Entered By: " + u.UserName,
                                      Remarks = cc.Remarks
                                  };
            ledgers.AddRange(PriceProtection);
            #endregion

            #region Promo Offer
            var PromoOffer = from cc in CashCollectionRepository.All
                             join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                             from u in lj.DefaultIfEmpty()
                             where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.PromoOfferForSupplier
                             select new LedgerAccountReportModel
                             {
                                 Date = (DateTime)cc.EntryDate,
                                 Debit = 0m,
                                 VoucherType = "Promo Offer",
                                 Credit = cc.Amount + cc.AdjustAmt,
                                 CashCollectionAmt = cc.Amount,
                                 CreditAdj = cc.AdjustAmt,
                                 InvoiceNo = cc.ReceiptNo,
                                 EnteredBy = "Entered By: " + u.UserName,
                                 Remarks = cc.Remarks
                             };
            ledgers.AddRange(PromoOffer);
            #endregion

            #region KPI
            var KPI = from cc in CashCollectionRepository.All
                      join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                      from u in lj.DefaultIfEmpty()
                      where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.KPIForSupplier
                      select new LedgerAccountReportModel
                      {
                          Date = (DateTime)cc.EntryDate,
                          Debit = 0m,
                          VoucherType = "KPI",
                          Credit = cc.Amount + cc.AdjustAmt,
                          CashCollectionAmt = cc.Amount,
                          CreditAdj = cc.AdjustAmt,
                          InvoiceNo = cc.ReceiptNo,
                          EnteredBy = "Entered By: " + u.UserName,
                          Remarks = cc.Remarks
                      };
            ledgers.AddRange(KPI);
            #endregion

            #region Incentive
            var Incentive = from cc in CashCollectionRepository.All
                            join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.IncentiveForSupplier
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)cc.EntryDate,
                                Debit = 0m,
                                VoucherType = "Incentive",
                                Credit = cc.Amount + cc.AdjustAmt,
                                CashCollectionAmt = cc.Amount,
                                CreditAdj = cc.AdjustAmt,
                                InvoiceNo = cc.ReceiptNo,
                                EnteredBy = "Entered By: " + u.UserName,
                                Remarks = cc.Remarks
                            };
            ledgers.AddRange(Incentive);
            #endregion

            #region Rate Adjust
            var RateAdjust = from cc in CashCollectionRepository.All
                            join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where cc.SupplierID == SupplierID && cc.TransactionType == EnumTranType.RateAdjustmentForSupplier
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)cc.EntryDate,
                                Debit = 0m,
                                VoucherType = "Rate Adjustment",
                                Credit = cc.Amount + cc.AdjustAmt,
                                CashCollectionAmt = cc.Amount,
                                CreditAdj = cc.AdjustAmt,
                                InvoiceNo = cc.ReceiptNo,
                                EnteredBy = "Entered By: " + u.UserName,
                                Remarks = cc.Remarks
                            };
            ledgers.AddRange(RateAdjust);
            #endregion

            #region Bank Transaction
            var bankTrans = from bt in BankTransactionRepository.All
                            join b in BankRepository.All on bt.BankID equals b.BankID
                            where bt.SupplierID == SupplierID && bt.TransactionType == 4
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)bt.TranDate,
                                Debit = 0,
                                VoucherType = "Bank",
                                Credit = bt.Amount,
                                CashCollectionAmt = bt.Amount,
                                CreditAdj = 0m,
                                InvoiceNo = bt.TransactionNo,
                                Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + bt.ChecqueNo,
                                Remarks = bt.Remarks
                            };
            ledgers.AddRange(bankTrans);
            #endregion

            #region Bank Transaction Return
            var bankReturnTrans = from bt in BankTransactionRepository.All
                                  join b in BankRepository.All on bt.BankID equals b.BankID
                                  where bt.SupplierID == SupplierID && bt.TransactionType == 10
                                  select new LedgerAccountReportModel
                                  {
                                      Date = (DateTime)bt.TranDate,
                                      Debit = bt.Amount,
                                      VoucherType = "Retrun from Supplier in Bank",
                                      Credit = 0m,
                                      CashCollectionAmt = 0m,
                                      CreditAdj = 0m,
                                      DebitAdj = bt.Amount,
                                      InvoiceNo = bt.TransactionNo,
                                      Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + bt.ChecqueNo,
                                      Remarks = bt.Remarks
                                  };
            ledgers.AddRange(bankReturnTrans);
            #endregion

            decimal balance = Supplier.OpeningDue;
            ledgers = ledgers.OrderBy(i => i.Date).ToList();
            foreach (var item in ledgers)
            {
                item.Balance = balance + (item.Debit - item.Credit);
                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.ProductList) + Environment.NewLine + item.EnteredBy : item.Particulars;
                balance = item.Balance;
            }

            var oOpening = new LedgerAccountReportModel() { Date = new DateTime(2015, 1, 1), Particulars = "Opening Balance", Debit = Supplier.OpeningDue, Balance = 0, Credit = 0 };

            if (ledgers.Count > 0)
            {
                //ledgers.Insert(0, oOpening);
                var OpeningTrans = ledgers.Where(i => i.Date < fromDate).OrderByDescending(i => i.Date).FirstOrDefault();
                if (OpeningTrans != null)
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = OpeningTrans.Date, Particulars = "Opening Balance", Balance = OpeningTrans.Balance, Debit = 0m });
                else
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Balance = Supplier.OpeningDue, Debit = 0m });

                ledgers = ledgers.Where(i => i.Date >= fromDate && i.Date <= toDate).OrderBy(i => i.Date).ToList();
                FinalLedgers.AddRange(ledgers);
            }
            else
            {
                FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Debit = Supplier.OpeningDue, Credit = 0m, Balance = Supplier.OpeningDue });
            }

            return FinalLedgers;
        }

        public static IEnumerable<ProductWisePurchaseModel> GetPurchaseDetailReportByPOrderID(this IBaseRepository<POrder> POrderRepository, IBaseRepository<TransactionPOrder> transPOrderRepository, IBaseRepository<TransactionPOrderDetail> transPOrderDetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<POProductDetail> POProductDetailRepository, IBaseRepository<ApplicationUser> userRepository, int POrderID)
        {
            IQueryable<TransactionPOrder> TransPOrders = null;

            TransPOrders = transPOrderRepository.GetAll().Where(i => i.POrderID == POrderID);

            var oPurchaseDetailData = (from PO in TransPOrders
                                       join POD in transPOrderDetailRepository.All on PO.POrderID equals POD.POrderID
                                       join P in productRepository.All on POD.ProductID equals P.ProductID
                                       //from POPD in POProductDetailRepository.All
                                       join us in userRepository.All on POD.ActionBy equals us.Id
                                       where (PO.Status == (int)EnumPurchaseType.Purchase && PO.ActionStatus == POD.ActionStatus)
                                       select new ProductWisePurchaseModel
                                       {
                                           ChallanNo = PO.ChallanNo,
                                           Date = PO.OrderDate,
                                           GrandTotal = PO.GrandTotal,
                                           NetDiscount = PO.NetDiscount,
                                           TotalAmount = PO.TotalAmt,
                                           RecAmt = PO.RecAmt,
                                           PaymentDue = PO.PaymentDue,
                                           ProductID = P.ProductID,
                                           ProductName = P.ProductName,
                                           UnitPrice = (POD.UnitPrice - (((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + (PO.TDiscount + PO.AdjAmount)))),
                                           TAmount = POD.TAmount,
                                           PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                           Quantity = POD.Quantity,
                                           //POPD.IMENO,
                                           SupplierName = PO.Supplier.Name,
                                           SupplierCode = PO.Supplier.Code,
                                           Address = PO.Supplier.Address,
                                           Mobile = PO.Supplier.ContactNo,
                                           OwnerName = PO.Supplier.OwnerName,
                                           TotalDue = PO.Supplier.TotalDue,
                                           PPOffer = POD.PPOffer,
                                           UserName = us.UserName,
                                           ActionStatus = PO.ActionStatus,
                                           ActionDate = (DateTime)POD.ActionDate
                                       }).OrderByDescending(x => x.ActionStatus).ToList();
            return oPurchaseDetailData;
        }


        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, string, EnumPurchaseType, Tuple<int>>>> GetAllDamagePurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
            IBaseRepository<Supplier> supplierRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            DateTime fromDate, DateTime toDate, bool IsVATManager, int ConcernID)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DamageOrder && (i.OrderDate >= fromDate && i.OrderDate <= toDate)).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new ProductWisePurchaseModel
                {
                    POrderID = x.PurchaseOrder.POrderID,
                    ChallanNo = x.PurchaseOrder.ChallanNo,
                    Date = x.PurchaseOrder.OrderDate,
                    SupplierName = x.Supplier.Name,
                    OwnerName = x.Supplier.OwnerName,
                    Mobile = x.Supplier.ContactNo,
                    Status = x.PurchaseOrder.Status,
                    TAmount = x.PurchaseOrder.TotalAmt,
                    EditReqStatus = x.PurchaseOrder.EditReqStatus
                }).ToListAsync();

            List<ProductWisePurchaseModel> finalData = new List<ProductWisePurchaseModel>();
            if (IsVATManager)
            {
                items = items.OrderByDescending(i => i.Date).ToList();
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID);
                decimal FalesPurchase = (items.Sum(i => i.TAmount) * oConcern.PurchaseShowPercent) / 100m;
                decimal FalesPurchaseCount = 0m;

                foreach (var item in items)
                {
                    FalesPurchaseCount += item.TAmount;
                    if (FalesPurchaseCount <= FalesPurchase)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType, Tuple<int>>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.Date,
                    x.SupplierName,
                    x.OwnerName,
                    x.Mobile,
                    (EnumPurchaseType)x.Status,
                    new Tuple<int>
                    (x.EditReqStatus)
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, string, EnumPurchaseType>>> GetAllDamageReturnPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
       IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;
            var POrders = purchaseOrderRepository.All;


            var items = await (from po in POrders
                               join sup in suppliers on po.SupplierID equals sup.SupplierID
                               where po.Status == (int)EnumPurchaseType.DamageReturn &&
                               (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                               select new
                               {
                                   po.POrderID,
                                   po.ChallanNo,
                                   po.OrderDate,
                                   sup.Name,
                                   sup.OwnerName,
                                   sup.ContactNo,
                                   po.Status
                               }).OrderByDescending(s => s.POrderID).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

    }
}
