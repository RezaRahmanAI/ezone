using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using IMSWEB.Model.SPModel;

namespace IMSWEB.Data
{
    public class AccountingRepository : IAccountingRepository
    {
        private IMSWEBContext _dbContext;

        #region Properties

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected IMSWEBContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Init()); }
        }

        public AccountingRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public IEnumerable<RPTTrialBalance> GetTrialBalance(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_TrialBalance " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<RPTTrialBalance>(sql).ToList();

                if (data != null && data.Any())
                {
                    List<RPTTrialBalance> drResult = data.Where(d => !string.IsNullOrEmpty(d.DebitParticular) && (d.Debit.HasValue && d.Debit.Value != 0) || d.DebitParticular.Equals("Customer Due"))
                        .OrderBy(d => d.SL)
                        .ToList();
                    var groupedDrResult = drResult
                                            .GroupBy(item => item.DebitParticular)
                                            .Select((group, index) => new
                                            {
                                                SL = index + 1,
                                                DebitParticular = group.Key,
                                                TotalDebit = group.Sum(item => item.Debit ?? 0)
                                            })
                                            .ToList();
                    List<RPTTrialBalance> crResult = data.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && (d.Credit.HasValue && d.Credit.Value != 0) || d.CreditParticular.Equals("Customer Due"))
                        .OrderBy(d => d.SL)
                        .ToList();


                    #region debit credit equal with suspense a/c
                    if (drResult != null && drResult.Any() && crResult != null && crResult.Any())
                    {
                        decimal totalDebit = drResult.Sum(d => d.Debit.Value);
                        decimal totalCredit = crResult.Sum(d => d.Credit.Value);
                        decimal suspenseAmt = totalDebit - totalCredit;
                        int maxSL = crResult.Max(d => d.SL) + 1;
                        if (maxSL > 0)
                        {
                            crResult.Add(new RPTTrialBalance
                            {
                                SL = maxSL,
                                DebitParticular = string.Empty,
                                Debit = 0m,
                                CreditParticular = "Suspense Account",
                                Credit = suspenseAmt
                            });
                        }
                    }
                    #endregion

                    #region cross join
                    var groupedCrResult = crResult
                                            .GroupBy(item => item.CreditParticular)
                                            .Select((group, index) => new
                                            {
                                                SL = index + 1,
                                                CreditParticular = group.Key,
                                                TotalCredit = group.Sum(item => item.Credit ?? 0)
                                            })
                                            .ToList();

                    var leftJoinResult = from r in groupedDrResult
                                         join cr in groupedCrResult on r.SL equals cr.SL into crGroup
                                         from cr in crGroup.DefaultIfEmpty()
                                         select new RPTTrialBalance
                                         {
                                             SL = r.SL,
                                             DebitParticular = r.DebitParticular,
                                             CreditParticular = cr?.CreditParticular,
                                             Debit = r.TotalDebit,
                                             Credit = cr?.TotalCredit ?? 0m
                                         };

                    var rightJoinResult = from cr in groupedCrResult
                                          join r in groupedDrResult on cr.SL equals r.SL into rGroup
                                          from r in rGroup.DefaultIfEmpty()
                                          where r == null
                                          select new RPTTrialBalance
                                          {
                                              SL = cr.SL,
                                              DebitParticular = r?.DebitParticular,
                                              CreditParticular = cr.CreditParticular,
                                              Debit = r?.TotalDebit ?? 0m,
                                              Credit = cr.TotalCredit
                                          };
                    #endregion

                    var finalResult = leftJoinResult.Union(rightJoinResult).ToList();




                    return finalResult.ToList();
                }

                else
                    return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<RPTTrialBalance> GetBalanceSheet(DateTime asOnDate, int ConcernID)
        {
            try
            {
                DateTime tdate = asOnDate;
                string sql = "exec sp_BalanceSheetNew " + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<RPTTrialBalance>(sql).ToList();

                if (data != null && data.Any())
                {

                    //var discounts = data.Where(d => d.DebitParticular.Equals("Sales Discount") && d.Debit.HasValue).ToList();
                    //decimal disAmt = discounts.Sum(d => d.Debit.Value);

                    List<RPTTrialBalance> drResult = data.Where(d => !string.IsNullOrEmpty(d.DebitParticular) && (d.Debit.HasValue && d.Debit.Value != 0) || d.DebitParticular.Equals("Account Receivable") || d.IsHeader)
                        .OrderBy(d => d.SL)
                        .ToList();
                    //var groupedDrResult = drResult
                    //                        .GroupBy(item => item.DebitParticular)
                    //                        .Select((group, index) => new
                    //                        {
                    //                            SL = index + 1,
                    //                            DebitParticular = group.Key,
                    //                            TotalDebit = group.Sum(item => item.Debit ?? 0),
                    //                            IsHeader = group.First().IsHeader
                    //                        })
                    //                        .ToList();
                    List<RPTTrialBalance> crResult = data.Where(d => !string.IsNullOrEmpty(d.CreditParticular))
                        .OrderBy(d => d.SL)
                        .ToList();


                    #region debit credit equal with suspense a/c
                    if (drResult != null && drResult.Any() && crResult != null && crResult.Any())
                    {
                        decimal totalDebit = drResult.Where(d => !d.IsHeader).Sum(d => d.Debit.Value);
                        decimal totalCredit = crResult.Where(d => !d.IsHeader).Sum(d => d.Credit.Value);
                        decimal suspenseAmt =  totalCredit - totalDebit;
                        int maxSL = drResult.Max(d => d.SL) + 1;
                        if (maxSL > 0)
                        {
                            drResult.Add(new RPTTrialBalance
                            {
                                SL = maxSL,
                                DebitParticular = "Owner's Equity",
                                Debit = suspenseAmt,
                                CreditParticular = string.Empty,
                                Credit = 0m
                            });
                        }
                    }
                    #endregion

                    #region cross join
                    var groupedDrResult = drResult
                        .GroupBy(item => item.DebitParticular)
                        .Select((group, index) => new
                        {
                            SL = index + 1,
                            DebitParticular = group.Key,
                            TotalDebit = group.Sum(item => item.Debit ?? 0),
                            IsHeader = group.First().IsHeader
                        })
                        .ToList();
                    var groupedCrResult = crResult
                                            .GroupBy(item => item.CreditParticular)
                                            .Select((group, index) => new
                                            {
                                                SL = index + 1,
                                                CreditParticular = group.Key,
                                                TotalCredit = group.Sum(item => item.Credit ?? 0),
                                                IsCrHeader = group.First().IsHeader
                                            })
                                            .ToList();

                    var leftJoinResult = from r in groupedDrResult
                                         join cr in groupedCrResult on r.SL equals cr.SL into crGroup
                                         from cr in crGroup.DefaultIfEmpty()
                                         select new RPTTrialBalance
                                         {
                                             SL = r.SL,
                                             DebitParticular = r.DebitParticular,
                                             CreditParticular = cr?.CreditParticular,
                                             Debit = r.TotalDebit,
                                             Credit = cr?.TotalCredit ?? 0m,
                                             IsHeader = r.IsHeader,
                                             IsCrHeader = cr?.IsCrHeader ?? false

                                         };

                    var rightJoinResult = from cr in groupedCrResult
                                          join r in groupedDrResult on cr.SL equals r.SL into rGroup
                                          from r in rGroup.DefaultIfEmpty()
                                          where r == null
                                          select new RPTTrialBalance
                                          {
                                              SL = cr.SL,
                                              DebitParticular = r?.DebitParticular,
                                              CreditParticular = cr.CreditParticular,
                                              Debit = r?.TotalDebit ?? 0m,
                                              Credit = cr.TotalCredit,
                                              IsCrHeader = cr.IsCrHeader,
                                              IsHeader = r?.IsHeader ?? false
                                          };
                    #endregion

                    var finalResult = leftJoinResult.Union(rightJoinResult).ToList();




                    return finalResult.ToList();
                }

                else
                    return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_ProfitandLossAccount " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ProfitLossReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_BalanceSheet " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ProfitLossReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
