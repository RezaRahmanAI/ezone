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
    public class CashCollectionRepository : ICashCollectionRepository
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

        public CashCollectionRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public void UpdateTotalDue(int CustomerID, int SupplierID, int BankID, int BankWithdrawID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDue", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;
                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void UpdateTotalDuePaymentReturn(int SupplierID, int BankID, int BankWithdrawID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDuePaymentReturn", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;
                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateTotalDueCustomerPaymentReturn(int CustomerID, int BankID, int BankWithdrawID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueCustomerPaymentReturn", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;
                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void UpdateTotalDueWhenEdit(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueWhenEditNew", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@CashCollectionID", SqlDbType.Int).Value = CashCollectionID;
                    cmd.Parameters.Add("@NewCollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateTotalDueWhenEditReturnType(int CustomerID, int SupplierID, int CashCollectionID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueWhenEditReturnType", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@CashCollectionID", SqlDbType.Int).Value = CashCollectionID;
                    cmd.Parameters.Add("@NewCollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public IEnumerable<DailyCashBookLedgerModel> DailyCashBookLedger(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_DailyCashBookLedger " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<DailyCashBookLedgerModel>(sql).ToList();
                return data.Where(i => i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<CashInHandReportModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Empty;

                if (ReportType == 1)
                {   //biplob
                    //if (ConcernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                    //    sql = "exec sp_KSDailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                    //else
                    if(CustomerType==0)                 

                    sql = "exec sp_DailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                    else if(CustomerType==(int)EnumSubCustomerType.Showroom)
                        sql = "exec sp_DeskDailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                    else
                        sql = "exec sp_TypwWiseDailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'" + "," + "'" + CustomerType + "'";

                }
                else if (ReportType == 2)
                    sql = "exec sp_MonthlyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                else if (ReportType == 3)
                    sql = "exec sp_YearlyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";


                var data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdateTotalDueForInvestment(int SIHID, int SIHId, int BankID, int BankWithdrawID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueForInvestment", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SIHID", SqlDbType.Int).Value = SIHID;
                    cmd.Parameters.Add("@SIHIdV2", SqlDbType.Int).Value = SIHId;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;

                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTotalDueForExpenditure(int ExpenseItemID,int BankID, int BankWithdrawID, decimal TotalRecAmt, int ConcernID,int userId,string Remarks, DateTime TranDate)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueForExpenditure", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ExpenseItemID", SqlDbType.Int).Value = ExpenseItemID;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;
                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    cmd.Parameters.Add("@ConcernID", SqlDbType.Int).Value = ConcernID;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Remarks", SqlDbType.Char).Value = Remarks;
                    cmd.Parameters.Add("@TranDate", SqlDbType.DateTime).Value = TranDate;


                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<ServiceChargeModel>ServiceChargeReport(int Month, int Year)
        {
            try
            {
                string sql = "exec SP_ServiceCharge " + "'" + Month + "'"+","+"'" + Year + "'";
                var data = DbContext.Database.SqlQuery<ServiceChargeModel>(sql).ToList();
                return data.ToList();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }




        public IEnumerable<CashInHandReportModel> AdminCashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType, List<SisterConcern> sisterConcernList)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                List<CashInHandReportModel> data = new List<CashInHandReportModel>();

                if (ReportType == 1)
                {
                    if (CustomerType == 0 && sisterConcernList != null && sisterConcernList.Any())
                    {
                        foreach (var item in sisterConcernList)
                        {
                            string sql = "exec sp_DailyCashInHand @FromDate, @ToDate, @ConcernID";
                            var parameters = new SqlParameter[]
                            {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate),
                        new SqlParameter("@ConcernID", item.ConcernID)
                            };
                            //data.AddRange(DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList());
                            //data.AddRange(item.Name);
                            var cashInHandReportModels = DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList();
   
                            foreach (var reportModel in cashInHandReportModels)
                            {
                                reportModel.ConcernName = item.Name;
                                if(reportModel.Income== "Opening Cash In Hand" & reportModel.IncomeAmt > 0)
                                {
                                    reportModel.OpeningCshInHand = reportModel.IncomeAmt;
                                }
                                if (reportModel.Expense == "Closing Cash In Hand" & reportModel.ExpenseAmt > 0)
                                {
                                    reportModel.ClosingCshInHand = reportModel.ExpenseAmt;
                                }

                            }

                            data.AddRange(cashInHandReportModels);

                        }
                    }
                    else if (CustomerType == (int)EnumSubCustomerType.Showroom)
                    {
                        string sql = "exec sp_DeskDailyCashInHand @FromDate, @ToDate, @ConcernID";
                        var parameters = new SqlParameter[]
                        {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@ConcernID", ConcernID)
                        };
                        data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList();
                    }
                    else
                    {
                        string sql = "exec sp_TypwWiseDailyCashInHand @FromDate, @ToDate, @ConcernID, @CustomerType";
                        var parameters = new SqlParameter[]
                        {
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@ConcernID", ConcernID),
                    new SqlParameter("@CustomerType", CustomerType)
                        };
                        data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList();
                    }
                }
                else if (ReportType == 2)
                {
                    string sql = "exec sp_MonthlyCashInHand @FromDate, @ToDate, @ConcernID";
                    var parameters = new SqlParameter[]
                    {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate),
                new SqlParameter("@ConcernID", ConcernID)
                    };
                    data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList();
                }
                else if (ReportType == 3)
                {
                    string sql = "exec sp_YearlyCashInHand @FromDate, @ToDate, @ConcernID";
                    var parameters = new SqlParameter[]
                    {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate),
                new SqlParameter("@ConcernID", ConcernID)
                    };
                    data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql, parameters).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
