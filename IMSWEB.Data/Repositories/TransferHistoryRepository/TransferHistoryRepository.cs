using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public class TransferHistoryRepository : ITransferHistoryRepository
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

        public TransferHistoryRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }


        #endregion
        public bool AddTransferHistoryUsingSP(DataTable dtTransferHistory)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddTransferOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@dtTransferOrder", SqlDbType.Structured).Value = dtTransferHistory;
                    var ReturnValue = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    ReturnValue.Direction = ParameterDirection.ReturnValue;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    var dbresult =(int) ReturnValue.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;
        }
    }
}
