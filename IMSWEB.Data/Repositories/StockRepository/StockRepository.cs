using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories.StockRepository
{
    public class StockRepository : IStockRepository
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

        public StockRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion
        public IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int ConcernID, int ProductID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_DailyStockVSSalesSummary " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<DailyStockVSSalesSummaryReportModel>(sql).ToList();
                if (ProductID != 0)
                    return data.Where(i => i.ConcernID == ConcernID && i.ProductID==ProductID).ToList();
                else
                    return data.Where(i => i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveStockValue(int ConcernID)
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_BalanceSheetDataProcess", sqlcon))
                    {
                        cmd.CommandTimeout = 60;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ConcernID", SqlDbType.Int).Value = ConcernID;
                        sqlcon.Open();
                        cmd.ExecuteNonQuery();
                        
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<StockReportWithDateReportModel> StockReportWithDate(int ConcernID, int ProductID, int CompanyID, int CategoryID)
        {
            try
            {

                string sql = "exec SP_getStockDataWithDays" + "'" + ConcernID + "'" + "," + "'" + ProductID + "'" + "," + "'" + CompanyID + "'" + "," + "'" + CategoryID + "'";
                var data = DbContext.Database.SqlQuery<StockReportWithDateReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public IEnumerable<StockForcastingReportModel> StockForcastingReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec SP_StockForcastingReport" + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<StockForcastingReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public IEnumerable<StockForcastingReportModel> StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, int ProductID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec SP_StockForcastingReportProductWise" + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ProductID + "'";
                var data = DbContext.Database.SqlQuery<StockForcastingReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

    }
}
