using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;

namespace IMSWEB.Data
{
    public interface IROrderRepository
    {
        void AddReturnOrderUsingSP(DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void UpdateReturnOrderUsingSP(int returnOrderId, DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void DeleteReturnOrderDetailUsingSP(int customerId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtROProductDetail);

        void DeleteReturnOrderUsingSP(int orderId, int userId);

        int CheckProductStatusByROId(int id);

        int CheckProductStatusByRODetailId(int id);
        bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        bool AddHireSaleReturnUsingSP(DataTable dtSalesOrder, DataTable dtCreditDetails, decimal salesPrice, decimal purchasePrice);

    }
}
