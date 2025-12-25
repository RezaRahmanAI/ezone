using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class PrevBalanceExtension
    {
        /// <summary>
        /// For Daily Cash Statement
        /// </summary>
        /// <param name="SOrderRepository"></param>
        /// <param name="PrevBalanceRepository"></param>
        /// <param name="entrydate"></param>
        /// <param name="ConcernID"></param>
        public static List<PrevBalance> DailyBalanceProcess(this IBaseRepository<PrevBalance> PrevBalanceRepository, IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<POrder> POrderRepository, IBaseRepository<CashCollection> CashCollectionRepository,
            IBaseRepository<Expenditure> ExpenditureRepository, IBaseRepository<ExpenseItem> ExpenseItemRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
            IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<ROrder> ROrderRepository, IBaseRepository<ShareInvestment> ShareInvestmentRepository,
            IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository, IBaseRepository<AdvanceSalary> AdvanceSalaryRepository, IBaseRepository<HireSalesReturnCustomerDueAdjustment> hireSalesReturnRepository, IBaseRepository<BankLoan> bankLoanRepository, IBaseRepository<BankLoanCollection> bankLoanCollectionRepository,
            int ConcernID)
        {

            var AllPrevBalances = PrevBalanceRepository.All;

            DateTime currentdate = DateTime.Today;
            decimal opening = 0;
            PrevBalance prevBalance = null;
            string dFromDate = currentdate.Date.ToString();
            string sToDate = currentdate.ToString("dd MMM yyyy") + " 11:59:59 PM";
            DateTime dFFdate = Convert.ToDateTime(dFromDate);
            DateTime dTTdate = Convert.ToDateTime(sToDate);

            List<PrevBalance> PrevbalanceList = new List<PrevBalance>();

            var SOrders = SOrderRepository.All;
            var SalesReturns = ROrderRepository.All;
            var CreditSales = CreditSaleRepository.All;
            var CreditSchedules = CreditSalesScheduleRepository.All;

            var POrders = POrderRepository.All;
            var Expenditures = ExpenditureRepository.All;
            var ExpenseItems = ExpenseItemRepository.All;
            var CashCollections = CashCollectionRepository.All;

            var BankTrans = BankTransactionRepository.All;
            var ShareInvestment = ShareInvestmentRepository.All;
            var ShareInvestmentHead = ShareInvestmentHeadRepository.All;
            var AdvanceSalary = AdvanceSalaryRepository.All;
            var hireSalesReturn = hireSalesReturnRepository.All;
            var bankLoans = bankLoanRepository.All;
            var bankLoanCollection = bankLoanCollectionRepository.All;

            PrevBalance oLastDayBalance = null;

            //oLastDayBalance = AllPrevBalances.FirstOrDefault(x => x.Date >= dFFdate && x.Date <= dTTdate);
            if (oLastDayBalance == null)
            {
                if (AllPrevBalances.Count() > 0)
                {
                    oLastDayBalance = AllPrevBalances.OrderByDescending(x => x.Date).First();
                    if (oLastDayBalance.Date > currentdate.AddDays(-35))
                    {
                        DateTime prevDate = currentdate.AddDays(-35);
                        List<PrevBalance> temp = AllPrevBalances.Where(x => x.Date <= prevDate).ToList();
                        if (temp.Count > 0)
                        {
                            oLastDayBalance = temp.OrderByDescending(x => x.Date).First();
                        }
                        else
                        {
                            oLastDayBalance = AllPrevBalances.OrderBy(x => x.Date).First();
                        }
                    }

                    Dictionary<DateTime, decimal> preDatacoll = CalculatePrevBalance(SOrders, POrders, CashCollections, Expenditures, ExpenseItems,
                        CreditSales, CreditSchedules, BankTrans, SalesReturns, ShareInvestment, ShareInvestmentHead, AdvanceSalary, hireSalesReturn, bankLoans, bankLoanCollection, oLastDayBalance.Date, dTTdate);

                    decimal dailyCash = 0;
                    opening = oLastDayBalance.Amount;
                    PrevBalance oBalancetemp = null;
                    bool flag = false;
                    for (DateTime date = oLastDayBalance.Date.Date; date <= dTTdate; date = date.AddDays(1))
                    {
                        flag = false;
                        oBalancetemp = AllPrevBalances.FirstOrDefault(x => x.Date == date);
                        if (oBalancetemp != null)
                        {
                            prevBalance = oBalancetemp;
                        }
                        else
                        {
                            prevBalance = new PrevBalance();
                            flag = true;
                        }


                        dailyCash = preDatacoll[date];
                        opening = opening + dailyCash;
                        prevBalance.Amount = opening;

                        prevBalance.Date = date;
                        prevBalance.Amount = opening;
                        prevBalance.ConcernID = ConcernID;
                        if (flag)
                            PrevbalanceList.Add(prevBalance);
                    }
                }
                else
                {
                    Dictionary<DateTime, decimal> preDatacoll = CalculatePrevBalance(SOrders, POrders, CashCollections, Expenditures, ExpenseItems,
                        CreditSales, CreditSchedules, BankTrans, SalesReturns, ShareInvestment, ShareInvestmentHead, AdvanceSalary, hireSalesReturn, bankLoans, bankLoanCollection,
                        dFFdate.AddDays(-60), dTTdate);

                    decimal dailyCash = 0;
                    for (DateTime date = dFFdate.AddDays(-60); date <= dTTdate; date = date.AddDays(1))
                    {
                        prevBalance = new PrevBalance();


                        dailyCash = preDatacoll[date];
                        opening = opening + dailyCash;
                        prevBalance.Amount = opening;

                        prevBalance.Date = date;

                        prevBalance.ConcernID = ConcernID;
                        PrevbalanceList.Add(prevBalance);
                    }
                }
            }
            return PrevbalanceList;
        }


        /// <summary>
        /// For Daily Cash Statement
        /// </summary>
        /// <param name="SOrderRepository"></param>
        /// <param name="POrderRepository"></param>
        /// <param name="CashCollectionRepository"></param>
        /// <param name="ExpenditureRepository"></param>
        /// <param name="dFFdate"></param>
        /// <param name="dTTdate"></param>
        /// <param name="concernId"></param>
        /// <returns></returns>
        private static Dictionary<DateTime, decimal> CalculatePrevBalance(IQueryable<SOrder> SOrders, IQueryable<POrder> POrders,
            IQueryable<CashCollection> CashCollections, IQueryable<Expenditure> Expenditures, IQueryable<ExpenseItem> ExpenseItems,
            IQueryable<CreditSale> CreditSales, IQueryable<CreditSalesSchedule> CreditSalesSchedules, IQueryable<BankTransaction> BankTransactions,
            IQueryable<ROrder> SalesReturns, IQueryable<ShareInvestment> ShareInvestment, IQueryable<ShareInvestmentHead> ShareInvestmentHead, IQueryable<AdvanceSalary> AdvanceSalaries, IQueryable<HireSalesReturnCustomerDueAdjustment> hireSalesReturn, IQueryable<BankLoan> bankLoans, IQueryable<BankLoanCollection> bankLoanCollections,
            DateTime dFFdate, DateTime dTTdate)
        {
            Dictionary<DateTime, decimal> dcPrevData = new Dictionary<DateTime, decimal>();

            decimal credit = 0m;
            decimal debit = 0m;

            var RetailSales = (from cs in SOrders
                               where (cs.InvoiceDate >= dFFdate && cs.InvoiceDate <= dTTdate && cs.Status == (int)EnumSalesType.Sales)
                               select new
                               {
                                   cs.InvoiceDate,
                                   cs.GrandTotal,
                                   RecAmount = cs.RecAmount,
                                   cs.TDAmount,
                                   cs.TotalDue,
                               }).ToList();

            var SalesRetuns = (from cs in SalesReturns
                               where (cs.ReturnDate >= dFFdate && cs.ReturnDate <= dTTdate)
                               select new
                               {
                                   InvoiceDate = cs.ReturnDate,
                                   cs.GrandTotal,
                                   RecAmount = cs.PaidAmount,
                                   TDAmount = 0,
                                   TotalDue = 0,
                               }).ToList();

            var CreditSaleList = (from cs in CreditSales
                                  where (cs.SalesDate >= dFFdate && cs.SalesDate <= dTTdate && cs.IsStatus == EnumSalesType.Sales)
                                  select new
                                  {
                                      cs.SalesDate,
                                      cs.NetAmount,
                                      RecAmount = cs.DownPayment, //+ cs.AgreementAmt,
                                      cs.Discount,
                                  }).ToList();



            var CreditProcessingFee = (from cs in CreditSales
                                  where (cs.SalesDate >= dFFdate && cs.SalesDate <= dTTdate && cs.IsStatus == EnumSalesType.Sales)
                                  select new
                                  {
                                      cs.SalesDate,
                                      cs.NetAmount,
                                      RecAmount = cs.ProcessingFee, //+ cs.AgreementAmt,
                                      cs.Discount,
                                  }).ToList();

            var Installments = (from cs in CreditSales
                                join css in CreditSalesSchedules on cs.CreditSalesID equals css.CreditSalesID
                                where (css.PaymentDate >= dFFdate && css.PaymentDate <= dTTdate && cs.IsStatus == EnumSalesType.Sales && css.PaymentStatus == "Paid" && css.InstallmentAmt > 0)
                                select new
                                {
                                    css.PaymentDate,
                                    RecAmount = css.InstallmentAmt,
                                    LastPayAdjust = cs.LastPayAdjAmt,
                                }).ToList();

            var oPurchase = (from cs in POrders
                             where (cs.OrderDate >= dFFdate && cs.OrderDate <= dTTdate && cs.Status == (int)EnumPurchaseType.Purchase)
                             select new
                             {
                                 cs.OrderDate,
                                 cs.GrandTotal,
                                 cs.RecAmt,
                                 cs.NetDiscount,
                                 cs.TotalDue,
                             }).ToList();

            var oPurchaseRetunrs = (from cs in POrders
                                    where (cs.OrderDate >= dFFdate && cs.OrderDate <= dTTdate && cs.Status == (int)EnumPurchaseType.ProductReturn)
                                    select new
                                    {
                                        cs.OrderDate,
                                        cs.GrandTotal,
                                        cs.RecAmt,
                                        cs.NetDiscount,
                                        cs.TotalDue,
                                    }).ToList();

            var oHireSalesReturn = (from cs in hireSalesReturn
                                    join hs in CreditSales on cs.CreditSalesId equals hs.CreditSalesID
                                    where (cs.TransactionDate >= dFFdate && cs.TransactionDate <= dTTdate)
                                    select new
                                    {
                                        cs.TransactionDate,
                                        hs.ReturnDate,
                                        cs.TotalRemainingDue,
                                        cs.AdjDue,
                                        cs.ToCustomerPayAmt
                                    }).ToList();

            var oExpense = (from exp in Expenditures
                            join expi in ExpenseItems on exp.ExpenseItemID equals expi.ExpenseItemID
                            where (exp.EntryDate >= dFFdate && exp.EntryDate <= dTTdate && exp.CashInHandReportStatus == 0)
                            select new
                            {
                                exp.EntryDate,
                                expi.Description,
                                exp.Amount,
                                exp.Purpose,
                                expi.Status,
                                exp.VoucherNo
                            }).ToList();

            var oCashCollections = (from csd in CashCollections
                                    where (csd.EntryDate >= dFFdate && csd.EntryDate <= dTTdate && csd.TransactionType == EnumTranType.FromCustomer)
                                    select new
                                    {
                                        csd.EntryDate,
                                        csd.Amount,
                                        csd.PaymentType,
                                        csd.AccountNo
                                    }).ToList();

            var oCashCollectionsReturn = (from csd in CashCollections
                                          where (csd.EntryDate >= dFFdate && csd.EntryDate <= dTTdate && csd.TransactionType == EnumTranType.CollectionReturn && csd.CustomerID > 0)
                                          select new
                                          {
                                              csd.EntryDate,
                                              csd.Amount,
                                              csd.PaymentType,
                                              csd.AccountNo
                                          }).ToList();

            var oCashPayments = (from csd in CashCollections
                                 where (csd.EntryDate >= dFFdate && csd.EntryDate <= dTTdate && csd.TransactionType == EnumTranType.ToCompany)
                                 select new
                                 {
                                     csd.EntryDate,
                                     csd.Amount,
                                     csd.PaymentType,
                                     csd.AccountNo,
                                 }).ToList();

            var oCashPaymentsReturn = (from csd in CashCollections
                                       where (csd.EntryDate >= dFFdate && csd.EntryDate <= dTTdate && csd.TransactionType == EnumTranType.CollectionReturn && csd.SupplierID > 0)
                                       select new
                                       {
                                           csd.EntryDate,
                                           csd.Amount,
                                           csd.PaymentType,
                                           csd.AccountNo
                                       }).ToList();

            var Deposit = (from csd in BankTransactions
                           where (csd.TranDate >= dFFdate && csd.TranDate <= dTTdate && csd.TransactionType == (int)EnumTransactionType.Deposit && csd.WFStatus == EnumWFStatus.Approved) 
                           select new
                           {
                               csd.TranDate,
                               csd.Amount,
                               csd.TransactionType,
                               csd.TransactionNo,
                           }).ToList();


            var withdraws = (from csd in BankTransactions
                             where (csd.TranDate >= dFFdate && csd.TranDate <= dTTdate && csd.TransactionType == (int)EnumTransactionType.Withdraw && csd.WFStatus == EnumWFStatus.Approved)
                             select new
                             {
                                 csd.TranDate,
                                 csd.Amount,
                                 csd.TransactionType,
                                 csd.TransactionNo,
                             }).ToList();

            var bankexpense = (from csd in BankTransactions
                               where (csd.TranDate >= dFFdate && csd.TranDate <= dTTdate && csd.TransactionType == (int)EnumTransactionType.BankExpense && csd.WFStatus == EnumWFStatus.Approved)
                               select new
                               {
                                   csd.TranDate,
                                   csd.Amount,
                                   csd.TransactionType,
                                   csd.TransactionNo,
                               }).ToList();

            var bankincome = (from csd in BankTransactions
                              where (csd.TranDate >= dFFdate && csd.TranDate <= dTTdate && csd.TransactionType == (int)EnumTransactionType.BankIncome && csd.WFStatus == EnumWFStatus.Approved)
                              select new
                              {
                                  csd.TranDate,
                                  csd.Amount,
                                  csd.TransactionType,
                                  csd.TransactionNo,
                              }).ToList();

            var ShareInvestments = (from shi in ShareInvestment
                                    join shih in ShareInvestmentHead on shi.SIHID equals shih.SIHID
                                    where (shi.EntryDate >= dFFdate && shi.EntryDate <= dTTdate && (shih.ParentId == (int)EnumInvestmentType.Liability || shih.ParentId == (int)EnumInvestmentType.PF || shih.ParentId == (int)EnumInvestmentType.FDR || shih.ParentId == (int)EnumInvestmentType.Security) && shi.CashInHandReportStatus == 0)
                                    select new
                                    {
                                        shi.EntryDate,
                                        shih.Name,
                                        shi.Amount,
                                        shi.Purpose,
                                        shih.ParentId,
                                        ParentName = EnumInvestmentType.Liability.ToString(),
                                        shi.TransactionType
                                    }).ToList();

            var ShareInvestmentCurrentAssest = (from shi in ShareInvestment
                                                join shih in ShareInvestmentHead on shi.SIHID equals shih.SIHID
                                                where (shi.EntryDate >= dFFdate && shi.EntryDate <= dTTdate && shih.ParentId == (int)EnumInvestmentType.CurrentAsset && shi.CashInHandReportStatus == 0)
                                                select new
                                                {
                                                    shi.EntryDate,
                                                    shih.Name,
                                                    shi.Amount,
                                                    shi.Purpose,
                                                    shih.ParentId,
                                                    ParentName = EnumInvestmentType.CurrentAsset.ToString(),
                                                    shi.TransactionType
                                                }).ToList();

            var oAdvanceSalaries = (from ads in AdvanceSalaries
                                    where ads.Date >= dFFdate && ads.Date <= dTTdate
                                    select new
                                    {
                                        ads.Date,
                                        ads.Amount,
                                        ads.Remarks
                                    }).ToList();


            var bankLoansData = (from bl in bankLoans
                                 where (bl.LoanDate >= dFFdate && bl.LoanDate <= dTTdate)
                                 select new
                                 {
                                     bl.LoanDate,
                                     bl.PrincipleLoanAmount,
                                     RecAmount = 0,
                                     TDAmount = 0,
                                     TotalDue = 0,
                                 }).ToList();

            var bankLoanDetailsData = (from bl in bankLoanCollections
                                       where (bl.CollectionDate >= dFFdate && bl.CollectionDate <= dTTdate)
                                       select new
                                       {
                                           bl.CollectionDate,
                                           CollectionAmount = bl.CollectionAmount + bl.SDPS,
                                           RecAmount = 0,
                                           TDAmount = 0,
                                           TotalDue = 0,
                                       }).ToList();


            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            for (DateTime date = dFFdate; date <= dTTdate; date = date.AddDays(1))
            {
                debit = 0m;
                credit = 0m;
                fromDate = date.AddDays(-1);
                toDate = date.AddSeconds(-1);

                #region Debit *** DEAD=Debit->expense, assests, drawings

                debit = debit + oExpense.Sum(x => (x.Status == EnumCompanyTransaction.Expense && x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                debit = debit + oCashPayments.Sum(x => (x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                debit = debit + Deposit.Sum(x => (x.TranDate >= fromDate && x.TranDate <= toDate) ? x.Amount : 0m);

                debit = debit + oPurchase.Sum(x => (x.OrderDate >= fromDate && x.OrderDate <= toDate) ? x.RecAmt : 0m);

                debit = debit + oHireSalesReturn.Sum(x => (x.TransactionDate >= fromDate && x.TransactionDate <= toDate) ? x.ToCustomerPayAmt : 0m);

                debit = debit + SalesRetuns.Sum(x => (x.InvoiceDate >= fromDate && x.InvoiceDate <= toDate) ? (decimal)x.RecAmount : 0m);

                debit = debit + ShareInvestments.Sum(x => (x.TransactionType == EnumInvestTransType.Pay && x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                debit = debit + oCashCollectionsReturn.Sum(x => (x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                debit = debit + oAdvanceSalaries.Sum(x => (x.Date >= fromDate && x.Date <= toDate) ? x.Amount : 0m);

                debit = debit + bankincome.Sum(x => (x.TranDate >= fromDate && x.TranDate <= toDate) ? x.Amount : 0m);
                debit = debit + bankLoanDetailsData.Sum(x => (x.CollectionDate >= fromDate && x.CollectionDate <= toDate) ? x.CollectionAmount : 0m);

                #endregion


                #region Credit *** CLIC= Credit-> Liabilities,Incomes,Capitals, bankloan

                credit = credit + withdraws.Sum(x => (x.TranDate >= fromDate && x.TranDate <= toDate) ? x.Amount : 0m);

                credit = credit + bankexpense.Sum(x => (x.TranDate >= fromDate && x.TranDate <= toDate) ? x.Amount : 0m);

                credit = credit + oExpense.Sum(x => (x.Status == EnumCompanyTransaction.Income && x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                credit = credit + oCashCollections.Sum(x => (x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                credit = credit + oCashPaymentsReturn.Sum(x => (x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);

                credit = credit + CreditSaleList.Sum(x => (x.SalesDate >= fromDate && x.SalesDate <= toDate) ? x.RecAmount : 0m);

                credit = credit + CreditProcessingFee.Sum(x => (x.SalesDate >= fromDate && x.SalesDate <= toDate) ? x.RecAmount : 0m);

                credit = credit + Installments.Sum(x => (x.PaymentDate >= fromDate && x.PaymentDate <= toDate) ? x.RecAmount : 0m);

                credit = credit + RetailSales.Sum(x => (x.InvoiceDate >= fromDate && x.InvoiceDate <= toDate) ? (decimal)x.RecAmount : 0m);

                credit = credit + oPurchaseRetunrs.Sum(x => (x.OrderDate >= fromDate && x.OrderDate <= toDate) ? (decimal)x.RecAmt : 0m);

                credit = credit + ShareInvestments.Sum(x => (x.TransactionType == EnumInvestTransType.Receive && x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);
                credit = credit + ShareInvestmentCurrentAssest.Sum(x => (x.EntryDate >= fromDate && x.EntryDate <= toDate) ? x.Amount : 0m);
                credit = credit + bankLoansData.Sum(d => (d.LoanDate >= fromDate && d.LoanDate <= toDate) ? d.PrincipleLoanAmount : 0m);

                #endregion

                dcPrevData.Add(date, (credit - debit));
            }
            return dcPrevData;
        }



        /// <summary>
        /// Date: 19-10-2020
        /// Author: aminul
        /// </summary>
        /// <returns></returns>
        public static List<CustomerOpeningDue> CustomerOpeningDueSave(this IBaseRepository<CustomerOpeningDue> CustomerOpeningDueRepository,
             IBaseRepository<SOrder> SOrderRepository,
             IBaseRepository<CashCollection> CashCollectionRepository,
             IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<Customer> CustomerRepository,
             int ConcernID)
        {

            var AllDues = CustomerOpeningDueRepository.All;

            DateTime currentdate = DateTime.Today;
            decimal opening = 0;
            CustomerOpeningDue prevBalance = null;
            string dFromDate = currentdate.Date.ToString();
            string sToDate = currentdate.ToString("dd MMM yyyy") + " 11:59:59 PM";
            DateTime dFFdate = Convert.ToDateTime(dFromDate);
            DateTime dTTdate = Convert.ToDateTime(sToDate);

            List<CustomerOpeningDue> OpeningDueList = new List<CustomerOpeningDue>();

            var SOrders = SOrderRepository.All.Where(i => i.Status == (int)EnumSalesType.Sales);
            var SalesRetunrs = SOrderRepository.All.Where(i => i.Status == (int)EnumSalesType.ProductReturn);
            var CashCollections = CashCollectionRepository.All;
            var Customers = CustomerRepository.All;//.Where(i => i.CustomerType == EnumCustomerType.Dealer && i.DealerType == EnumDealerType.Cash);
            var BankTrans = BankTransactionRepository.All;

            CustomerOpeningDue oLastDayBalance = null;

            oLastDayBalance = AllDues.FirstOrDefault(x => x.Date >= dFFdate && x.Date <= dTTdate);
            if (oLastDayBalance == null)
            {
                if (AllDues.Count() > 0)
                {
                    oLastDayBalance = AllDues.OrderByDescending(x => x.Date).First();
                    if (oLastDayBalance.Date > currentdate.AddDays(-35))
                    {
                        DateTime prevDate = currentdate.AddDays(-35);
                        List<CustomerOpeningDue> temp = AllDues.Where(x => x.Date <= prevDate).ToList();
                        if (temp.Count > 0)
                        {
                            oLastDayBalance = temp.OrderByDescending(x => x.Date).First();
                        }
                        else
                        {
                            oLastDayBalance = AllDues.OrderBy(x => x.Date).First();
                        }
                    }

                    var preDatacoll = CalculateOpeninDue(SOrders, CashCollections,
                        SalesRetunrs, BankTrans, oLastDayBalance.Date, dTTdate, Customers);

                    decimal dailyCash = 0;
                    //opening = oLastDayBalance.OpeningDue;
                    CustomerOpeningDue oBalancetemp = null;
                    bool flag = false;
                    foreach (var item in Customers)
                    {
                        opening = AllDues.Any(x => x.CustomerID == item.CustomerID) ? AllDues.Where(x => x.CustomerID == item.CustomerID).OrderByDescending(i => i.Date).FirstOrDefault().OpeningDue : 0m; ;
                        for (DateTime date = oLastDayBalance.Date.Date; date <= dTTdate; date = date.AddDays(1))
                        {
                            flag = false;
                            oBalancetemp = AllDues.FirstOrDefault(x => x.Date == date && x.CustomerID == item.CustomerID);
                            if (oBalancetemp != null)
                            {
                                prevBalance = oBalancetemp;
                            }
                            else
                            {
                                prevBalance = new CustomerOpeningDue();
                                flag = true;
                            }

                            dailyCash = preDatacoll.Any(i => i.Date == date && i.CustomerID == item.CustomerID) ?
                                preDatacoll.FirstOrDefault(i => i.Date == date && i.CustomerID == item.CustomerID).ClosingDue : 0m;

                            opening = opening + dailyCash;
                            prevBalance.OpeningDue = opening;

                            prevBalance.Date = date;
                            prevBalance.OpeningDue = opening;
                            prevBalance.ConcernID = ConcernID;
                            prevBalance.CustomerID = item.CustomerID;
                            if (flag)
                                OpeningDueList.Add(prevBalance);
                        }
                    }
                }
                else
                {
                    var preDatacoll = CalculateOpeninDue(SOrders, CashCollections,
                         SalesRetunrs, BankTrans,
                         dFFdate.AddDays(-60), dTTdate, Customers);

                    decimal dailyCash = 0;
                    foreach (var item in Customers)
                    {
                        opening = item.OpeningDue;
                        for (DateTime date = dFFdate.AddDays(-30); date <= dTTdate; date = date.AddDays(1))
                        {
                            prevBalance = new CustomerOpeningDue();

                            dailyCash = preDatacoll.Any(i => i.Date == date && i.CustomerID == item.CustomerID) ?
                                preDatacoll.FirstOrDefault(i => i.Date == date && i.CustomerID == item.CustomerID).ClosingDue : 0m;

                            opening = opening + dailyCash;
                            prevBalance.OpeningDue = opening;

                            prevBalance.Date = date;
                            prevBalance.OpeningDue = opening;
                            prevBalance.ConcernID = ConcernID;
                            prevBalance.CustomerID = item.CustomerID;

                            prevBalance.ConcernID = ConcernID;
                            OpeningDueList.Add(prevBalance);
                        }
                    }
                }
            }
            return OpeningDueList;
        }

        private static List<CustomerDueReport> CalculateOpeninDue(IQueryable<SOrder> SOrders,
            IQueryable<CashCollection> CashCollections, IQueryable<SOrder> SalesRetunrs,
            IQueryable<BankTransaction> BankTrans, DateTime dFFdate, DateTime dTTdate, IQueryable<Customer> Customers)
        {
            List<CustomerDueReport> dcPrevData = new List<CustomerDueReport>();

            decimal credit = 0m;
            decimal debit = 0m;

            var RetailSales = (from so in SOrders
                               join c in Customers on so.CustomerID equals c.CustomerID
                               where (so.InvoiceDate >= dFFdate && so.InvoiceDate <= dTTdate)
                               select new
                               {
                                   so.CustomerID,
                                   so.InvoiceDate,
                                   CreditAdj = so.AdjAmount,
                                   Credit = (decimal)so.RecAmount,
                                   Debit = (decimal)(so.TotalAmount),
                               }).ToList();

            var SalesRetuns = (from cs in SalesRetunrs
                               join c in Customers on cs.CustomerID equals c.CustomerID
                               where (cs.InvoiceDate >= dFFdate && cs.InvoiceDate <= dTTdate)
                               select new
                               {
                                   cs.CustomerID,
                                   cs.InvoiceDate,
                                   Credit = (decimal)(cs.TotalAmount),
                                   Debit = (decimal)(cs.RecAmount),
                               }).ToList();

            var oCashCollections = (from cc in CashCollections
                                    join c in Customers on cc.CustomerID equals c.CustomerID
                                    where (cc.EntryDate >= dFFdate && cc.EntryDate <= dTTdate && cc.TransactionType == EnumTranType.FromCustomer)
                                    select new
                                    {
                                        cc.CustomerID,
                                        cc.EntryDate,
                                        Credit = cc.Amount,
                                        CreditAdj = cc.AdjustAmt,
                                    }).ToList();

            var oCashCollectionReturns = (from cc in CashCollections
                                          join c in Customers on cc.CustomerID equals c.CustomerID
                                          where (cc.EntryDate >= dFFdate && cc.EntryDate <= dTTdate && cc.TransactionType == EnumTranType.CollectionReturn)
                                          select new
                                          {
                                              cc.CustomerID,
                                              cc.EntryDate,
                                              Debit = cc.Amount,
                                              CreditAdj = cc.AdjustAmt,
                                          }).ToList();


            var BankCollection = (from bt in BankTrans
                                  join c in Customers on bt.CustomerID equals c.CustomerID
                                  where (bt.TranDate >= dFFdate && bt.TranDate <= dTTdate && bt.TransactionType == (int)EnumTransactionType.CashCollection)
                                  select new
                                  {
                                      bt.CustomerID,
                                      bt.TranDate,
                                      Credit = bt.Amount,
                                      CreditAdj = 0m,
                                  }).ToList();

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            foreach (var item in Customers)
            {
                for (DateTime date = dFFdate; date <= dTTdate; date = date.AddDays(1))
                {
                    debit = 0m;
                    credit = 0m;
                    fromDate = date.AddDays(-1);
                    toDate = date.AddSeconds(-1);

                    #region Debit *** DEAD=Debit->expense, assests, drawings
                    debit = debit + RetailSales.Sum(x => ((x.InvoiceDate >= fromDate && x.InvoiceDate <= toDate) && x.CustomerID == item.CustomerID) ? (decimal)(x.Debit) : 0m);
                    debit = debit + SalesRetuns.Sum(x => ((x.InvoiceDate >= fromDate && x.InvoiceDate <= toDate) && x.CustomerID == item.CustomerID) ? (decimal)x.Debit : 0m);
                    debit = debit + oCashCollectionReturns.Sum(x => ((x.EntryDate >= fromDate && x.EntryDate <= toDate) && x.CustomerID == item.CustomerID) ? (decimal)x.Debit : 0m);

                    #endregion

                    #region Credit *** CLIC= Credit-> Liabilities,Incomes,Capitals
                    credit = credit + oCashCollections.Sum(x => ((x.EntryDate >= fromDate && x.EntryDate <= toDate) && x.CustomerID == item.CustomerID) ? (x.Credit + x.CreditAdj) : 0m);
                    credit = credit + BankCollection.Sum(x => ((x.TranDate >= fromDate && x.TranDate <= toDate) && x.CustomerID == item.CustomerID) ? (decimal)(x.Credit + x.CreditAdj) : 0m);
                    #endregion

                    dcPrevData.Add(new CustomerDueReport() { CustomerID = item.CustomerID, Date = date, ClosingDue = (debit - credit) });
                }
            }

            return dcPrevData;
        }

    }
}
