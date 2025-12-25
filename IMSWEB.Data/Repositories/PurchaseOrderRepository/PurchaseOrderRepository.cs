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

namespace IMSWEB.Data
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
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

        public PurchaseOrderRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public Tuple<bool, int> AddPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            //bool Result = false;
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddPurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

                    //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    //returnParameter.Direction = ParameterDirection.ReturnValue;

                    var returnPOrderID = cmd.Parameters.Add("@PorderID", SqlDbType.Int);
                    returnPOrderID.Direction = ParameterDirection.Output;

                    var returnResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnResult.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int POrderID = (int)returnPOrderID.Value;
                    int dbresult = (int)returnResult.Value;

                    //int dbresult = (int)returnParameter.Value;
                    //if (dbresult == 1)
                    //    Result = true;

                    if (dbresult == 1)
                        Result = new Tuple<bool, int>(true, POrderID);
                }
            }
            return Result;
        }

        public Tuple<bool, int> AddReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail)
        {
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddReturnPurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;

                    var returnParamResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnParamResult.Direction = ParameterDirection.Output;

                    var returnPOrderID = cmd.Parameters.Add("@OutPOrderID", SqlDbType.Int);
                    returnPOrderID.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParamResult.Value;
                    int PorderID = (int)returnPOrderID.Value;
                    if (dbresult == 1)
                    {
                        Result = new Tuple<bool, int>(true, PorderID);
                    }
                }
            }
            return Result;
        }

        public bool AddDeliveryOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
    DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddDeliveryOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    //cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    //cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

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
        public bool UpdatePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdatePurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = purchaseOrderId;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

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
        public bool UpdateDeliveryOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDeliveryOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = purchaseOrderId;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

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
        public void DeletePurchaseOrderDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeletePurchaseOrderDetail", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = supplierId;
                    cmd.Parameters.Add("@POrderDetailId", SqlDbType.Int).Value = porderDetailId;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = colorId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                    cmd.Parameters.Add("@TotalDue", SqlDbType.Decimal).Value = totalDue;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool DeletePurchaseOrderUsingSP(int orderId, int userId)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnPurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
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

        public int CheckProductStatusByPOId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckProductStatusByPOId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CheckTransferProductStatusByPOId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckTransferProductStatusByPOId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }


        public int CheckProductStatusByPODetailId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckProductStatusByPODetailId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CheckIMENoDuplicacyByConcernId(int concernId, string imeNo)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckIMENoDuplicacyByConcernId](" + concernId + ", '" + imeNo + "')", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CheckHireSalesProductStatusByPOId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckHireSalesProductStatusByPOId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public Tuple<bool, int> AddDamagePurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            //bool Result = false;
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddDamagePurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

                    //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    //returnParameter.Direction = ParameterDirection.ReturnValue;

                    var returnPOrderID = cmd.Parameters.Add("@PorderID", SqlDbType.Int);
                    returnPOrderID.Direction = ParameterDirection.Output;

                    var returnResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnResult.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int POrderID = (int)returnPOrderID.Value;
                    int dbresult = (int)returnResult.Value;

                    //int dbresult = (int)returnParameter.Value;
                    //if (dbresult == 1)
                    //    Result = true;

                    if (dbresult == 1)
                        Result = new Tuple<bool, int>(true, POrderID);
                }
            }
            return Result;
        }

        public bool UpdateDamagePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDamagePurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = purchaseOrderId;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;

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

        public Tuple<bool, int> AddDamageReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail)
        {
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddDamageReturnPurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtPurchaseOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtPODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtPOProductDetail;

                    var returnParamResult = cmd.Parameters.Add("@Result", SqlDbType.Int);
                    returnParamResult.Direction = ParameterDirection.Output;

                    var returnPOrderID = cmd.Parameters.Add("@OutPOrderID", SqlDbType.Int);
                    returnPOrderID.Direction = ParameterDirection.Output;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParamResult.Value;
                    int PorderID = (int)returnPOrderID.Value;
                    if (dbresult == 1)
                    {
                        Result = new Tuple<bool, int>(true, PorderID);
                    }
                }
            }
            return Result;
        }



    }
}
