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
using IMSWEB.SPViewModels;

namespace IMSWEB.Data
{
    public class SRVisitRepository : ISRVisitRepository
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

        public SRVisitRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public bool AddSRVisitChallanUsingSP(DataTable dtSRVisit, DataTable dtSRVDetail,
            DataTable dtSRVPProductDetail)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddSRVisit", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SRVisit", SqlDbType.Structured).Value = dtSRVisit;
                    cmd.Parameters.Add("@SRVisitDetails", SqlDbType.Structured).Value = dtSRVDetail;
                    cmd.Parameters.Add("@SRVProductDetails", SqlDbType.Structured).Value = dtSRVPProductDetail;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbResult = (int)returnParameter.Value;
                    if (dbResult == 1)
                        Result = true;
                }
            }

            return Result;
        }

        public bool UpdateSRVisitChallanUsingSP(int SRVisitId, DataTable dtSRVisit, DataTable dtSRVDetail,
            DataTable dtSRVPProductDetail)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSRVisit", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SRVisitID", SqlDbType.Int).Value = SRVisitId;
                    cmd.Parameters.Add("@SRVisit", SqlDbType.Structured).Value = dtSRVisit;
                    cmd.Parameters.Add("@SRVisitDetails", SqlDbType.Structured).Value = dtSRVDetail;
                    cmd.Parameters.Add("@SRVProductDetails", SqlDbType.Structured).Value = dtSRVPProductDetail;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbResult = (int)returnParameter.Value;
                    if (dbResult == 1)
                        Result = true;
                }
            }

            return Result;
        }

        public void DeleteSRVisitDetailUsingSP(int employeeId, int srvisitDetailId, int productId,
            int colorId, int userId, decimal quantity, DataTable dtSRVProductDetail)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteSRVisitDetail", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmpID", SqlDbType.Int).Value = employeeId;
                    cmd.Parameters.Add("@SRVisitDID", SqlDbType.Int).Value = srvisitDetailId;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = colorId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                    cmd.Parameters.Add("@SRVProductDetails", SqlDbType.Structured).Value = dtSRVProductDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool DeleteSRVisitUsingSP(int orderId, int userId)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnSRVisit", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SRVisitID", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbResult = (int)returnParameter.Value;
                    if (dbResult == 1)
                        Result = true;
                }
            }
            return Result;
        }

        public IEnumerable<SRVisitStatusReportModel> SRVisitStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            try
            {
                string sql = "exec sp_SRVisitStatusReport {SRID}";
                var SRData = DbContext.Database.SqlQuery<SRVisitStatusReportModel>(sql).ToList();
                return SRData;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public IEnumerable<SRVisitReportModel> SRVisitReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_SRVisitReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'" + "," + "'" + SRID + "'";
                var data = DbContext.Database.SqlQuery<SRVisitReportModel>(sql).ToList();
                //if (SRID == 0)
                    return data;
                //else
                    //return data.Where(i => i.EmployeeId == SRID).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<SRWiseCustomerStatusReportModel> SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec SRWiseCustomerStatusReport " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<SRWiseCustomerStatusReportModel>(sql).ToList();
                if (SRID == 0)
                    return data.Where(i => i.ConcernID == ConcernID);
                else
                    return data.Where(i => i.EmployeeID == SRID && i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool ReturnSRVisitUsingSP(DataTable dt, int EmployeeID)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnSRVisitLIST", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
                    cmd.Parameters.Add("@SRVProductDetails", SqlDbType.Structured).Value = dt;

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    var dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }

            return Result;
        }

        public bool CheckSRVisitReturnValidity(int SRVisitID)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckSRVisitReturnValidity](" + SRVisitID + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    int r = Convert.ToInt32(cmd.ExecuteScalar());
                    if (r == 1)
                        Result = true;
                }
            }
            return Result;
        }

    }
}
