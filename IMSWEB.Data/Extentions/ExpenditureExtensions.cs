using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ExpenditureExtensions
    {
        public static async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense)
                         join ed in expenditureRepository.All on ei.ExpenseItemID equals ed.ExpenseItemID
                         where ed.EntryDate >= fromDate && ed.EntryDate <= toDate
                         select ed;
            return await result.Include(i => i.ExpenseItem).OrderByDescending(i => i.EntryDate).ToListAsync();
        }
        public static async Task<IEnumerable<Expenditure>> GetAllExpenditureByUserID(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, int UserID, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense)
                         join ed in expenditureRepository.All.Where(i => i.CreatedBy == UserID) on ei.ExpenseItemID equals ed.ExpenseItemID
                         where ed.EntryDate >= fromDate && ed.EntryDate <= toDate
                         select ed;
            return await result.OrderByDescending(i => i.EntryDate).ToListAsync();
        }
        public static IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<int>>> 
            GetforReport(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepository,
            IBaseRepository<ApplicationUser> UserRepository,
            IBaseRepository<SisterConcern> sisterConcernRepo,
            DateTime fromDate, DateTime toDate, int concernID, EnumCompanyTransaction Status,
            int ExpenseItemID, bool isAdminReport)
        {
            IQueryable<ExpenseItem> expenseItems = null;
            IQueryable<Expenditure> expenditures = null;
            IQueryable<ApplicationUser> users = null;
            if (isAdminReport)
            {
                if (concernID > 0)
                {
                    users = UserRepository.GetAll().Where(i => i.ConcernID == concernID);
                    expenditures = expenditureRepository.GetAll().Where(i => i.ConcernID == concernID);
                    expenseItems = expenseItemRepository.GetAll().Where(i => i.Status == Status && i.ConcernID == concernID);

                }
                else
                {
                    users = UserRepository.GetAll();
                    expenditures = expenditureRepository.GetAll();
                    expenseItems = expenseItemRepository.GetAll().Where(i => i.Status == Status);
                }

            }
            else
            {
                if (ExpenseItemID != 0)
                    expenseItems = expenseItemRepository.All.Where(i => i.ExpenseItemID == ExpenseItemID);
                else
                    expenseItems = expenseItemRepository.All.Where(i => i.Status == Status);

                users = UserRepository.All;
                expenditures = expenditureRepository.All;
            }



            var oExpenseData = (from exps in expenditures
                                join exi in expenseItems on exps.ExpenseItemID equals exi.ExpenseItemID
                                join u in users on exps.CreatedBy equals u.Id
                                join sis in sisterConcernRepo.GetAll() on exps.ConcernID equals sis.ConcernID
                                where (exps.EntryDate >= fromDate && exps.EntryDate <= toDate)
                                group exps by new
                                {
                                    exps.EntryDate,
                                    exi.Description,
                                    exps.Purpose,
                                    exps.VoucherNo,
                                    u.UserName,
                                    ConcernName=sis.Name,
                                    Status = exps.CashInHandReportStatus
                                } into g
                                select new
                                {
                                    EntryDate = g.Key.EntryDate,
                                    ItemName = g.Key.Description,
                                    Purpose = g.Key.Purpose,
                                    VoucherNo = g.Key.VoucherNo,
                                    Amount = g.Sum(i3 => i3.Amount),
                                    UserName = g.Key.UserName,
                                    g.Key.ConcernName,
                                    g.Key.Status
                                    
                                }
                                             ).ToList();
            return oExpenseData.Select(x => new Tuple<DateTime, string, string, decimal, string, string, string, Tuple<int>>
                (
                x.EntryDate,
                x.ItemName,
                x.Purpose,
                x.Amount,
                x.VoucherNo,
                x.UserName,
                x.ConcernName,new Tuple<int>(x.Status)
                )).OrderByDescending(x => x.Item1).ToList();

        }


        public static async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Income)
                         join ed in expenditureRepository.All on ei.ExpenseItemID equals ed.ExpenseItemID
                         where (ed.EntryDate >= fromDate && ed.EntryDate <= toDate)
                         select ed;
            return await result.Include(i => i.ExpenseItem).OrderByDescending(i => i.EntryDate).ToListAsync();
        }

        public static decimal GetAllExpenditureAmountByUserID(this IBaseRepository<Expenditure> expenditureRepository, IBaseRepository<ExpenseItem> expenseItemRepo, int UserID, DateTime fromDate, DateTime toDate)
        {
            var expenditures = expenditureRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate && i.CreatedBy == UserID);
            var eitems = expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense);
            decimal expenseamount = 0;

            var result = from ei in eitems
                         join ed in expenditures on ei.ExpenseItemID equals ed.ExpenseItemID
                         select ed;
            if (result.Count() != 0)
                expenseamount = result.Sum(i => i.Amount);
            return expenseamount;
        }

        public static async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, IBaseRepository<ApplicationUser> UserRepository, DateTime fromDate, DateTime toDate, List<EnumWFStatus> enumWFStatus)
        {
            IQueryable<Expenditure> expenditures = null;
            IQueryable<ApplicationUser> users = UserRepository.All;
            if (fromDate != DateTime.MinValue)
                expenditures = expenditureRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate);
            else
                expenditures = expenditureRepository.All;



            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Income)
                         join ed in expenditures on ei.ExpenseItemID equals ed.ExpenseItemID
                         join us in UserRepository.All on ed.CreatedBy equals us.Id
                         where enumWFStatus.Contains(ed.WFStatus)
                         select ed;
            return await result.Include(i => i.ExpenseItem).Include(i => i.User).OrderByDescending(i => i.EntryDate).ToListAsync();
        }

        public static async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, DateTime fromDate, DateTime toDate, List<EnumWFStatus> enumWFStatus)
        {
            IQueryable<Expenditure> expenditures = null;
            if (fromDate != DateTime.MinValue)
                expenditures = expenditureRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate);
            else
                expenditures = expenditureRepository.All;

            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense)
                         join ed in expenditures on ei.ExpenseItemID equals ed.ExpenseItemID
                         where enumWFStatus.Contains(ed.WFStatus)
                         select ed;
            return await result.Include(i => i.ExpenseItem).OrderByDescending(i => i.EntryDate).ToListAsync();
        }

    }
}
