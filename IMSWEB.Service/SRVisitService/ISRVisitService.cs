using IMSWEB.Model;
using IMSWEB.SPViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface ISRVisitService
    {
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string,EnumSRVisitType>>> GetAllSRVisitAsync();

        void AddSRVisitChallan(SRVisit srVisit);

        bool AddSRVisitChallanUsingSP(DataTable dtSRVisit, DataTable dtSRVisitDetail,
            DataTable dtSRVProductDetail);

        bool UpdateSRVisitChallanUsingSP(int SRVisitId, DataTable dtSRVisit, DataTable dtSRVisitDetail,
            DataTable dtSRVProductDetail);

        void DeleteSRVisitDetailUsingSP(int SRId, int visitDetailId, int productId,
            int colorId, int userId, decimal quantity, DataTable dtSRVProductDetail);

        void SaveSRVisit();

        SRVisit GetSRVisitById(int id);

        SRVisit GetSRVisitByChallanNo(string ChallanNo,int ConcernID);

        bool DeleteSRVisitUsingSP(int id, int userId);

        IEnumerable<Tuple<string, string, DateTime, string>> GetSRVisitReportByConcernID(DateTime fromDate, DateTime toDate, int concernID);

        IEnumerable<Tuple<DateTime, string, string, string, string, string>>
           GetSRVisitDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID);

        IEnumerable<Tuple<DateTime, string, string, decimal,string,string,string,Tuple<string>>>
        GetSRViistDetailReportByEmployeeID(DateTime fromDate, DateTime toDate, int concernID, int employeeID);

        IEnumerable<SRVisitStatusReportModel> SRVisitStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        IEnumerable<SRVisitReportModel> SRVisitReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        IEnumerable<SRVisitReportModel> SRVisitReportDetails(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        IEnumerable<SRWiseCustomerStatusReportModel> SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, int ConcernID, int SRID);
        bool IsIMEIAlreadyIssuedToSR(int ProductID, int ColorID, string IMEINO);

        IEnumerable<AdvancePODetail> GetAllSRVisitStockIMEIBySRID(int EmployeeID);
        bool ReturnSRVisitUsingSP(DataTable dt, int EmployeeID);

        bool CheckSRVisitReturnValidity(int SRVisitID);

        decimal GetRemainingQuantityForSRVisit(int productId, int colorId, int concernId, int GodownId, int? employeeId = null, int stockDetailsId = 0);
    }
}
