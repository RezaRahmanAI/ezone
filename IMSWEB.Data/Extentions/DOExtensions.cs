using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class DOExtensions
    {
        public static List<ProductWisePurchaseModel> ProductWisePurchaseDOReport(this IBaseRepository<DO> doRepository,
        IBaseRepository<DODetail> doDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
        IBaseRepository<Category> categoryRepository, IBaseRepository<Supplier> supplierRepository,
        int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

            var DOrders = doRepository.All.Where(i => i.Date >= fromDate && i.Date <= toDate && i.Status != EnumDOStatus.Return); ;
            var DOrderDetails = doDetailRepository.All;
            var result = from DO in DOrders
                         join sup in supplierRepository.All on DO.SupplierID equals sup.SupplierID
                         join DOD in DOrderDetails on DO.DOID equals DOD.DOID
                         join P in Products on DOD.ProductID equals P.ProductID
                         join com in CompanyRepository.All on P.CompanyID equals com.CompanyID
                         join cate in categoryRepository.All on P.CategoryID equals cate.CategoryID
                         select new ProductWisePurchaseModel
                         {
                             Date = DO.Date,
                             ChallanNo = DO.DONo,
                             ProductName = P.ProductName,
                             CompanyName = com.Name,
                             CategoryName = cate.Description,
                             Quantity = DOD.DOQty,
                             PurchaseRate = DOD.UnitPrice,
                             TotalAmount = DOD.TotalAmt,
                             GivenQty = DOD.GivenQty

                         };

            return result.ToList();


        }
    }
}
