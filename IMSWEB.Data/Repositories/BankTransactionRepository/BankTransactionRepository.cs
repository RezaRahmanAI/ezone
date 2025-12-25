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
    public class BankTransactionRepository : IBankTransactionRepository
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

        public BankTransactionRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion
        public List<BankLedgerModel> BankLedgerUsingSP(DateTime fromdate, DateTime todate, int ConcernID, int BankID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                //string sql = "exec sp_BankLedger " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                string sql = "exec sp_BankLedger ";
                var data = DbContext.Database.SqlQuery<BankLedgerModel>(sql).ToList();
                if (BankID == 0)
                    return data.ToList();
                else
                    return data.Where(i => i.BankID == BankID).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BankTransReportModel> BankTransactionsReportOld(DateTime fromdate, DateTime todate, int BankID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_BankTransReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + BankID + "'";
                var data = DbContext.Database.SqlQuery<BankTransReportModel>(sql).ToList();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BankTransReportModel> BankTransactionsReport(DateTime fromdate, DateTime todate, int BankID, int concernId)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_BankTransReportNew " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + BankID + "'," + concernId + "";
                var data = DbContext.Database.SqlQuery<BankTransReportModel>(sql).ToList();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
