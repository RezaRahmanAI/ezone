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
using System.Data.SqlTypes;

namespace IMSWEB.Data
{
    public class CreditSalesOrderRepository : ICreditSalesOrderRepository
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

        public CreditSalesOrderRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules, DataTable dtBankTrans)
        {
            //bool Result = false;
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CSalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                    cmd.Parameters.Add("@CSODetails", SqlDbType.Structured).Value = dtSODetail;
                    cmd.Parameters.Add("@CSSchedules", SqlDbType.Structured).Value = dtSchedules;
                    cmd.Parameters.Add("@BankTrans", SqlDbType.Structured).Value = dtBankTrans;

                    //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    //returnParameter.Direction = ParameterDirection.ReturnValue;
                    var returnSOrderID = cmd.Parameters.Add("@SorderID", SqlDbType.Int);
                    returnSOrderID.Direction = ParameterDirection.Output;

                    var returnResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnResult.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int SOrderID = (int)returnSOrderID.Value;
                    int dbresult = (int)returnResult.Value;

                    //int dbresult = (int)returnParameter.Value;
                    //if (dbresult == 1)
                    //    Result = true;

                    if (dbresult == 1)
                        Result = new Tuple<bool, int>(true, SOrderID);
                }
            }
            return Result;

        }

        public bool InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InstallmentPayment", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@InstallmentAmount", SqlDbType.Decimal).Value = installmentAmount;
                    cmd.Parameters.Add("@Schedules", SqlDbType.Structured).Value = dtSchedules;
                    cmd.Parameters.Add("@LastPayAdjustment", SqlDbType.Decimal).Value = LastPayAdjustment;
                    cmd.Parameters.Add("@BankTrans", SqlDbType.Structured).Value = dtBankTrans;
                    cmd.Parameters.Add("@CardTypeSetupID", SqlDbType.Int).Value = CardTypeSetupID;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;
        }

        public bool ReturnSalesOrderUsingSP(int orderId, int userId)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;

                }
            }
            return Result;
        }

        //public void CalculatePenaltySchedules(int ConcernID)
        //{
        //    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("CreditSalesPenaltySchedules", sqlcon))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@ConcernID", SqlDbType.Int).Value = ConcernID;
        //            sqlcon.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        public void CorrectionStockData(int ConcernId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_StockCorrection", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ConcernId", SqlDbType.Int).Value = ConcernId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public System.Collections.Generic.IEnumerable<System.Tuple<string, string, string, string, System.DateTime, System.DateTime, decimal, System.Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetUpcomingSchedule(System.DateTime fromDate, System.DateTime toDate, int concernID)
        {
            throw new System.NotImplementedException();
        }

        public bool PendingInstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
           int CardTypeSetupID)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PendingInstallmentPayment", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@InstallmentAmount", SqlDbType.Decimal).Value = installmentAmount;
                    cmd.Parameters.Add("@Schedules", SqlDbType.Structured).Value = dtSchedules;
                    cmd.Parameters.Add("@LastPayAdjustment", SqlDbType.Decimal).Value = LastPayAdjustment;
                    cmd.Parameters.Add("@CardTypeSetupID", SqlDbType.Int).Value = CardTypeSetupID;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;
        }

        public Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
                DataTable dtSchedules)
        {
            //bool Result = false;
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddPendingCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CSalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                    cmd.Parameters.Add("@CSODetails", SqlDbType.Structured).Value = dtSODetail;
                    cmd.Parameters.Add("@CSSchedules", SqlDbType.Structured).Value = dtSchedules;

                    //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    //returnParameter.Direction = ParameterDirection.ReturnValue;

                    var returnSOrderID = cmd.Parameters.Add("@SorderID", SqlDbType.Int);
                    returnSOrderID.Direction = ParameterDirection.Output;

                    var returnResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnResult.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int SOrderID = (int)returnSOrderID.Value;
                    int dbresult = (int)returnResult.Value;

                    //int dbresult = (int)returnParameter.Value;
                    //if (dbresult == 1)
                    //    Result = true;

                    if (dbresult == 1)
                        Result = new Tuple<bool, int>(true, SOrderID);
                }
            }
            return Result;

        }

        public bool ApprovedSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
                                DataTable dtSchedules, DataTable dtBankTrans, int OrderID)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ApprovedCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CSalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                    cmd.Parameters.Add("@CSODetails", SqlDbType.Structured).Value = dtSODetail;
                    cmd.Parameters.Add("@CSSchedules", SqlDbType.Structured).Value = dtSchedules;
                    cmd.Parameters.Add("@BankTrans", SqlDbType.Structured).Value = dtBankTrans;
                    cmd.Parameters.Add("@CreditSalesId", SqlDbType.Int).Value = OrderID;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;

        }

        public bool InstallmentApprovedSP(int orderId, decimal installmentAmount, decimal LastPayAdjustment,
                        DataTable dtBankTrans, int CardTypeSetupID, int ScheduleID)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InstallmentApproved", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@ScheduleID", SqlDbType.Int).Value = ScheduleID;
                    cmd.Parameters.Add("@InstallmentAmount", SqlDbType.Decimal).Value = installmentAmount;
                    cmd.Parameters.Add("@LastPayAdjustment", SqlDbType.Decimal).Value = LastPayAdjustment;
                    cmd.Parameters.Add("@BankTrans", SqlDbType.Structured).Value = dtBankTrans;
                    cmd.Parameters.Add("@CardTypeSetupID", SqlDbType.Int).Value = CardTypeSetupID;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;
        }
    }
}
