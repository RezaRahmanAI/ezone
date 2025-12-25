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
    public class TransferRepository : ITransferRepository
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

        public TransferRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }


        #endregion
        public Tuple<bool, int> AddTransferUsingSP(DataTable dtTransferOrder, DataTable dtDetails, DataTable dtTransferFromStock, DataTable dtTransferToStock)
        {
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddTransferOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@dtTransferOrder", SqlDbType.Structured).Value = dtTransferOrder;
                    cmd.Parameters.Add("@dtTransferDetails", SqlDbType.Structured).Value = dtDetails;
                    cmd.Parameters.Add("@dtTransferFromStock", SqlDbType.Structured).Value = dtTransferFromStock;
                    cmd.Parameters.Add("@dtTransferToStock", SqlDbType.Structured).Value = dtTransferToStock;

                    var ReturnResult = cmd.Parameters.Add("@ReturnResult", SqlDbType.Int);
                    ReturnResult.Direction = ParameterDirection.Output;

                    var ReturnTransferID = cmd.Parameters.Add("@ReturnTransferID", SqlDbType.Int);
                    ReturnTransferID.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    var dbresult = (int)ReturnResult.Value;
                    int TransferID = (int)ReturnTransferID.Value;

                    if (dbresult == 1)
                    {
                        Result = new Tuple<bool, int>(true, TransferID);
                    }



                }
            }
            return Result;
        }


        public bool ReturnTranserferUsingSP(int TransferID)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ReturnTransfer", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TransferID", SqlDbType.Int).Value = TransferID;

                    var ReturnResult = cmd.Parameters.Add("@ReturnResult", SqlDbType.Int);
                    ReturnResult.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    var dbresult = (int)ReturnResult.Value;

                    if (dbresult == 1)
                    {
                        Result = true;
                    }
                }
            }
            return Result;
        }



        //public bool CheckTransProductStatusByTransDId(int id)
        //{
        //    int count = 0;
        //    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckTransProductStatusByTransDId](" + id + ")", sqlcon))
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            sqlcon.Open();
        //            count = Convert.ToInt32(cmd.ExecuteScalar());
        //        }
        //    }
        //    return count > 0;
        //}

        public int CheckTransProductStatusByTransDId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckTransProductStatusByTransDId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public int CheckTransProductHireSalesStatusByTransDId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckTransProductHireSalesStatusByTransDId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}
