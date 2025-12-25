using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IROrderService
    {
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string>>> GetAllReturnOrderAsync();

        void AddReturnOrder(ROrder oReturn);

        void AddReturnOrderUsingSP(DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void UpdateReturnOrderUsingSP(int ReturnOrderId, DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void DeleteReturnOrderDetailUsingSP(int customerId, int rorderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtROProductDetail);



        void SaveReturnOrder();

        ROrder GetReturnOrderById(int id);

        void DeleteReturnOrderUsingSP(int id, int userId);

        int CheckProductStatusByROId(int id);

        int CheckProductStatusByRODetailId(int id);

        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
         GetReturnReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType);

        IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, 
            Tuple<decimal, decimal, decimal, decimal, decimal, string, string, 
                Tuple<int, decimal,int, string, int, int, string,
                    Tuple<decimal>>>>>
         GetReturnDetailReportByConcernID(DateTime fromDate, DateTime toDate, int CustomerType, int concernID, bool IsAdminReport);

        IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
        GetReturnDetailReportByReturnID(int ReturnID, int concernID);

        
        List<ProductWiseSalesReportModel> ProductWiseReturnReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID);
        List<ProductWiseSalesReportModel> ProductWiseReturnDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType, int CustomerID);

        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync(DateTime fromDate, DateTime toDate);
        bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        bool AddHireSalesReturnUsingSP(DataTable dtHireSaleCustomerDue, DataTable dtCreditDetails, decimal salePrice, decimal purchasePrice);
    }
}
