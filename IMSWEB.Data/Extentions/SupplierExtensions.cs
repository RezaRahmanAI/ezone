using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SupplierExtensions
    {
        public static async Task<IEnumerable<Supplier>> GetAllSupplierAsync(this IBaseRepository<Supplier> supplierRepository)
        {
            return await supplierRepository.All.ToListAsync();
        }

        public static IEnumerable<Tuple<string, string, string, string, string, decimal, string>>
        ConcernWiseSupplierDueRpt(this IBaseRepository<Supplier> supplierRepository,
            IBaseRepository<SisterConcern> concernRepository,
            int concernID, int supplierId,bool IsAdminReport)
        {
            IQueryable<Supplier> suppliers = null;
            if (!IsAdminReport)
            {
                if (supplierId > 0)
                    suppliers = supplierRepository.All.Where(i => i.SupplierID == supplierId);
                else
                    suppliers = supplierRepository.All;
            }
            else //admin report purpose
            {
                if (concernID > 0)
                    suppliers = supplierRepository.GetAll().Where(i => i.SupplierID == supplierId);
                else
                    suppliers = supplierRepository.GetAll();
            }

            var oSupplierDueData = (from Sup in suppliers
                                    join sis in concernRepository.GetAll() on Sup.ConcernID equals sis.ConcernID
                                    select new
                                    {
                                        SupCode = Sup.Code,
                                        CusName = Sup.Name,
                                        OwnerName = Sup.OwnerName,
                                        Sup.ContactNo,
                                        Sup.Address,
                                        Sup.TotalDue,
                                        ConcernName= sis.Name
                                    }).ToList();

            return oSupplierDueData.Select(x => new Tuple<string, string, string, string, string, decimal,string>
                (
                 x.SupCode,
                 x.CusName,
                 x.OwnerName,
                 x.ContactNo,
                 x.Address,
                 x.TotalDue,
                 x.ConcernName
                ));

        }



        public static int GetSupplierIdBySDetailId(
            this IBaseRepository<Supplier> supplierRepository,
            IBaseRepository<StockDetail> stockDetailRepository,
            IBaseRepository<POrder> pOrderRepository,
            IBaseRepository<POrderDetail> pOrderDetailRepository,
            int sDetailId)
        {
            // Join StockDetail, PurchaseOrderDetail, and PurchaseOrder to find the SupplierID
            var supplierId = (from stockDetail in stockDetailRepository.GetAll()
                              join pOrderDetail in pOrderDetailRepository.GetAll()
                              on stockDetail.POrderDetailID equals pOrderDetail.POrderDetailID
                              join pOrder in pOrderRepository.GetAll()
                              on pOrderDetail.POrderID equals pOrder.POrderID
                              where stockDetail.SDetailID == sDetailId
                              select pOrder.SupplierID).FirstOrDefault();

            return supplierId;
        }

    }
}
