using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class UserAuditExtension
    {
        public static IEnumerable<UserAuditDetailsReportModel>
       GetUserAuditReport(this IBaseRepository<UserAuditDetail> userAuditDetailRepository, IBaseRepository<Customer> customerRepository,
       IBaseRepository<Supplier> supplierRepository, IBaseRepository<ExpenseItem> expenseItemRepository, IBaseRepository<POrder> pOrderRepository,
       IBaseRepository<SOrder> sOrderRepository, IBaseRepository<CreditSale> creditSaleRepository, IBaseRepository<CashCollection> cashCollectionRepository,
       IBaseRepository<Expenditure> expenditureRepository, IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<SessionMaster> sessionMasterRepository,
       IBaseRepository<ApplicationUser> userRepository, IBaseRepository<ApplicationUserRole> userRoleRepository, IBaseRepository<ApplicationRole> roleRepository,
       DateTime FromDate, DateTime ToDate, int ConcernID, EnumObjectType ObjectType)
        {

            if (ObjectType == EnumObjectType.Purchase)
            {
                var purchasedetails = (from uad in userAuditDetailRepository.GetAll()
                                       join po in pOrderRepository.GetAll() on uad.ObjectID equals po.POrderID
                                       join sup in supplierRepository.GetAll() on po.SupplierID equals sup.SupplierID
                                       join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                       join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                       from use in lj.DefaultIfEmpty()
                                       join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                       join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                       where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.Purchase
                                       select new UserAuditDetailsReportModel
                                       {
                                           EntryDate = uad.ActivityDtTime,
                                           InvoiceDate = po.OrderDate,
                                           InvoiceNo = po.ChallanNo,
                                           Name = sup.Name,
                                           UserName = use.UserName,
                                           UserRole = rol.Name,
                                           ActionType = uad.ActionType,
                                           ObjectType = uad.ObjectType

                                       }).ToList();
                return purchasedetails.OrderBy(i => i.EntryDate);
            }
            else if (ObjectType == EnumObjectType.Sales)
            {
                var SalesDetails = (from uad in userAuditDetailRepository.GetAll()
                                    join so in sOrderRepository.GetAll() on uad.ObjectID equals so.SOrderID
                                    join cus in customerRepository.GetAll() on so.CustomerID equals cus.CustomerID
                                    join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                    join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                    from use in lj.DefaultIfEmpty()
                                    join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                    join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                    where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.Sales
                                    select new UserAuditDetailsReportModel
                                    {
                                        EntryDate = uad.ActivityDtTime,
                                        InvoiceDate = so.InvoiceDate,
                                        InvoiceNo = so.InvoiceNo,
                                        Name = cus.Name,
                                        UserName = use.UserName,
                                        UserRole = rol.Name,
                                        ActionType = uad.ActionType,
                                        ObjectType = uad.ObjectType

                                    }).ToList();
                return SalesDetails.OrderBy(i => i.EntryDate);
            }
            else if (ObjectType == EnumObjectType.HireSales)
            {
                var HireSalesDetails = (from uad in userAuditDetailRepository.GetAll()
                                        join so in creditSaleRepository.GetAll() on uad.ObjectID equals so.CreditSalesID
                                        join cus in customerRepository.GetAll() on so.CustomerID equals cus.CustomerID
                                        join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                        join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                        from use in lj.DefaultIfEmpty()
                                        join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                        join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                        where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.HireSales
                                        select new UserAuditDetailsReportModel
                                        {
                                            EntryDate = uad.ActivityDtTime,
                                            InvoiceDate = so.SalesDate,
                                            InvoiceNo = so.InvoiceNo,
                                            Name = cus.Name,
                                            UserName = use.UserName,
                                            UserRole = rol.Name,
                                            ActionType = uad.ActionType,
                                            ObjectType = uad.ObjectType

                                        }).ToList();
                return HireSalesDetails.OrderBy(i => i.EntryDate);
            }
            else if (ObjectType == EnumObjectType.CashCollection)
            {
                var CashCollectionDetails = (from uad in userAuditDetailRepository.GetAll()
                                             join so in cashCollectionRepository.GetAll() on uad.ObjectID equals so.CashCollectionID
                                             join cus in customerRepository.GetAll() on so.CustomerID equals cus.CustomerID
                                             join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                             join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                             from use in lj.DefaultIfEmpty()
                                             join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                             join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                             where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.CashCollection
                                             select new UserAuditDetailsReportModel
                                             {
                                                 EntryDate = uad.ActivityDtTime,
                                                 InvoiceDate = (DateTime)so.EntryDate,
                                                 InvoiceNo = so.ReceiptNo,
                                                 Name = cus.Name,
                                                 UserName = use.UserName,
                                                 UserRole = rol.Name,
                                                 ActionType = uad.ActionType,
                                                 ObjectType = uad.ObjectType

                                             }).ToList();
                return CashCollectionDetails.OrderBy(i => i.EntryDate);
            }
            else if (ObjectType == EnumObjectType.CashDelivery)
            {
                var CashDeliveryDetails = (from uad in userAuditDetailRepository.GetAll()
                                           join so in cashCollectionRepository.GetAll() on uad.ObjectID equals so.CashCollectionID
                                           join cus in supplierRepository.GetAll() on so.SupplierID equals cus.SupplierID
                                           join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                           join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                           from use in lj.DefaultIfEmpty()
                                           join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                           join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                           where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.CashDelivery
                                           select new UserAuditDetailsReportModel
                                           {
                                               EntryDate = uad.ActivityDtTime,
                                               InvoiceDate = (DateTime)so.EntryDate,
                                               InvoiceNo = so.ReceiptNo,
                                               Name = cus.Name,
                                               UserName = use.UserName,
                                               UserRole = rol.Name,
                                               ActionType = uad.ActionType,
                                               ObjectType = uad.ObjectType

                                           }).ToList();
                return CashDeliveryDetails.OrderBy(i => i.EntryDate);
            }
            else if (ObjectType == EnumObjectType.Income)
            {
                var IncomeDetails = (from uad in userAuditDetailRepository.GetAll()
                                     join exd in expenditureRepository.GetAll() on uad.ObjectID equals exd.ExpenditureID
                                     join exi in expenseItemRepository.GetAll() on exd.ExpenseItemID equals exi.ExpenseItemID
                                     join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                     join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                     from use in lj.DefaultIfEmpty()
                                     join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                     join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                     where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.Income
                                     select new UserAuditDetailsReportModel
                                     {
                                         EntryDate = uad.ActivityDtTime,
                                         InvoiceDate = exd.EntryDate,
                                         InvoiceNo = exd.VoucherNo,
                                         Name = exi.Description,
                                         UserName = use.UserName,
                                         UserRole = rol.Name,
                                         ActionType = uad.ActionType,
                                         ObjectType = uad.ObjectType

                                     }).ToList();
                return IncomeDetails.OrderBy(i => i.EntryDate);
            }
            else
            {
                var ExpenseDetails = (from uad in userAuditDetailRepository.GetAll()
                                      join exd in expenditureRepository.GetAll() on uad.ObjectID equals exd.ExpenditureID
                                      join exi in expenseItemRepository.GetAll() on exd.ExpenseItemID equals exi.ExpenseItemID
                                      join sm in sessionMasterRepository.GetAll() on uad.SessionID equals sm.SessionID
                                      join use in userRepository.GetAll() on sm.UserID equals use.Id into lj
                                      from use in lj.DefaultIfEmpty()
                                      join usrol in userRoleRepository.GetAll() on use.Id equals usrol.UserId
                                      join rol in roleRepository.GetAll() on usrol.RoleId equals rol.Id
                                      where DbFunctions.TruncateTime(uad.ActivityDtTime) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(uad.ActivityDtTime) <= DbFunctions.TruncateTime(ToDate) && uad.ObjectType == EnumObjectType.Expense
                                      select new UserAuditDetailsReportModel
                                      {
                                          EntryDate = uad.ActivityDtTime,
                                          InvoiceDate = exd.EntryDate,
                                          InvoiceNo = exd.VoucherNo,
                                          Name = exi.Description,
                                          UserName = use.UserName,
                                          UserRole = rol.Name,
                                          ActionType = uad.ActionType,
                                          ObjectType = uad.ObjectType

                                      }).ToList();
                return ExpenseDetails.OrderBy(i => i.EntryDate);
            }

        }

    }
}
