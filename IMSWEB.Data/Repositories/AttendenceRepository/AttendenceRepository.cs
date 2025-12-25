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
    public class AttendenceRepository : IAttendenceRepository
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

        public AttendenceRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public bool AddAttendencMonthUsingSP(DataTable dtMonth, DataTable dtAttendenc)
        {
            try
            {
                bool Result = false;

                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddAttendenceByAttenMonthID", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@dtAttenMonth", SqlDbType.Structured).Value = dtMonth;
                        cmd.Parameters.Add("@dtAttenList", SqlDbType.Structured).Value = dtAttendenc;

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
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteAttendenceByAttenMonthID(int AttenMonthID)
        {
            try
            {
                bool Result = false;

                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteAttendenceByAttenMonthID", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AttenMonthID", SqlDbType.Int).Value = AttenMonthID;

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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
