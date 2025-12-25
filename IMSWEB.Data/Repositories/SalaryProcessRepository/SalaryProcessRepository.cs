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
    public class SalaryProcessRepository : ISalaryProcessRepository
    {
        public bool UndoSalaryProcessUsingSP(int SalaryProcessID)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UndoSalaryProcess", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalaryProcessID", SqlDbType.Int).Value = SalaryProcessID;

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


        public bool FinalizeSalaryMonthUsingSP(DateTime fromDate, DateTime toDate, int ConcernID, int FinalizedBy, DateTime MonthEndDate, DateTime NextMonth,DataTable dtWeeklyHolidays)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_FinalizeMonth", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@fromDate", SqlDbType.DateTime).Value = fromDate;
                    cmd.Parameters.Add("@toDate", SqlDbType.DateTime).Value = toDate;
                    cmd.Parameters.Add("@ConcernID", SqlDbType.Int).Value = ConcernID;
                    cmd.Parameters.Add("@FinalizedBy", SqlDbType.Int).Value = FinalizedBy;
                    cmd.Parameters.Add("@MonthEndDate", SqlDbType.DateTime).Value = MonthEndDate;
                    cmd.Parameters.Add("@NextMonth", SqlDbType.DateTime).Value = NextMonth;
                    cmd.Parameters.Add("@tblHolidayCalender", SqlDbType.Structured).Value = dtWeeklyHolidays;

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
