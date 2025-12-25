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
    public interface IPurchaseOrderService
    {
        IQueryable<POrder> GetAllIQueryable();
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string, EnumPurchaseType, Tuple<int>>>> GetAllPurchaseOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID);

        Task<IEnumerable<Tuple<int, string, DateTime, string,
          string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync();


        Task<IEnumerable<Tuple<int, string, DateTime, string,
         string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<Tuple<int, string, DateTime, string,
       string, string, EnumPurchaseType>>> GetAllDeliveryOrderAsync();

        Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllDamageReturnOrderAsync();
        POrder GetDamagerReturnOrderByChallanNo(string ChallanNo);
        void AddPurchaseOrder(POrder purchaseOrder);

        Tuple<bool, int> AddPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);

        Tuple<bool, int> AddReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail);

        bool UpdatePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        bool UpdateDeliveryOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
    DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void DeletePurchaseOrderDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail);

        void SavePurchaseOrder();
        void Update(POrder Porder);

        POrder GetPurchaseOrderById(int id);

        bool DeletePurchaseOrderUsingSP(int id, int userId);

        int CheckProductStatusByPOId(int id);
        int CheckTransferProductStatusByPOId(int id);

        int CheckIMENoDuplicacyByConcernId(int concernId, string imeNo);

        int CheckProductStatusByPODetailId(int id);

        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, string, string>>>
            GetPurchaseReport(DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, bool IsAdminReport, int concernID);

        IEnumerable<ProductWisePurchaseModel>GetPurchaseDetailReportByConcernID(DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, bool IsVATManager, int concernID);

        IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>>
        GetPurchaseDetailReportBySupplierID(DateTime fromDate, DateTime toDate, int concernID, int supplierID);



        IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetPurchaseByProductID(DateTime fromDate, DateTime toDate, int concernID, int productID);

        AdvanceSearchModel AdvanceSearchByIMEI(int ConcernID, string IMEINO);
        AdvanceSearchModel AdvanceSearchByIMEINew(int ConcernID, string IMEINO);

        List<ProductWisePurchaseModel> ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int concernID, int supplierID, EnumPurchaseType PurchaseType);
        List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReport(int CompanyID, int CategoryID, 
            int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, 
            bool IsAdminReport, int SelectedConcernID, int SupplierID);
        List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReportNew(int CompanyID, int CategoryID,
        int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, int SupplierID);
        AdvanceSearchModel SRVisitAdvanceSearchByIMEI(int ConcernID, string IMEINO);
        bool AddDeliveryOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
        DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        POProductDetail GetDamagePOPDetail(string DamageIMEI, int ProductID, int ColorID);

        IEnumerable<ProductWisePurchaseModel> GetDamagePOReport(DateTime fromDate, DateTime toDate, int SupplierID);
        IEnumerable<ProductWisePurchaseModel> GetDamageReturnProductDetails(int ProductID, int ColorID);
        IEnumerable<ProductWisePurchaseModel> DamageReturnProductDetailsReport(int SupplierID, DateTime fromDate, DateTime toDate);
        IQueryable<ProductWisePurchaseModel> AdminPurchaseReport(DateTime fromDate, DateTime toDate, int ConcernID);

        List<LedgerAccountReportModel> SupplierLedger(DateTime fromdate, DateTime todate, int SupplierID);
        bool IsProductPurchase(int ProductID);
        bool IsReturnFound(int porderId);
        IEnumerable<ProductWisePurchaseModel> GetPurchaseDetailReportByPOrderID(int POrderID);
        bool IsPOReturn(int PoId);
        int CheckHireSalesProductStatusByPOId(int id);
        bool UpdateDamagePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
             DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        Tuple<bool, int> AddDamagePurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);

        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string, EnumPurchaseType, Tuple<int>>>> GetAllDamagePurchaseOrderAsync(DateTime fromDate, DateTime toDate, bool IsVATManager, int concernID);

        Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllDamageReturnPurchaseOrderAsync(DateTime fromDate, DateTime toDate);

        Tuple<bool, int> AddDamageReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail);
    }
}
