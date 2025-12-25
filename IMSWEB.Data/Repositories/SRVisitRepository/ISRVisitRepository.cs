using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using IMSWEB.SPViewModels;

namespace IMSWEB.Data
{
    public interface ISRVisitRepository
    {
        bool AddSRVisitChallanUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail);

        bool UpdateSRVisitChallanUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail);

        void DeleteSRVisitDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity,DataTable dtPOProductDetail);

        bool DeleteSRVisitUsingSP(int orderId, int userId);

        IEnumerable<SRVisitStatusReportModel> SRVisitStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        IEnumerable<SRVisitReportModel> SRVisitReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        IEnumerable<SRWiseCustomerStatusReportModel> SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);

        bool ReturnSRVisitUsingSP(DataTable dt, int EmployeeID);
        bool CheckSRVisitReturnValidity(int SRVisitID);

    }
}
