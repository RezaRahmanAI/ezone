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
    public class TargetSetupRepository : ITargetSetupRepository
    {

        public bool AddTargetSetupUsingSP(DataTable dtTargetSetup,DataTable dtTargetSetupDetails)
        {

            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddTargetSetup", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@dtTargetSetup", SqlDbType.Structured).Value = dtTargetSetup;
                    cmd.Parameters.Add("@dtAddTargetSetupDetail", SqlDbType.Structured).Value = dtTargetSetupDetails;

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

        public bool UpdateTargetSetupUsingSP(int TID,DataTable dtTargetSetup, DataTable dtTargetSetupDetails)
        {

            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateTargetSetup", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TID", SqlDbType.Int).Value = TID;
                    cmd.Parameters.Add("@dtTargetSetup", SqlDbType.Structured).Value = dtTargetSetup;
                    cmd.Parameters.Add("@dtAddTargetSetupDetail", SqlDbType.Structured).Value = dtTargetSetupDetails;

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


        public bool DeleteTargetSetupUsingSP(int TID)
        {

            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteTargetSetupByID", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TID", SqlDbType.Int).Value = TID;

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
