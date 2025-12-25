using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories.StockRepository
{
   public interface IStockRepository
    {
       IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int concernID, int ProductID);
        void SaveStockValue(int ConcernID);


        IEnumerable<StockReportWithDateReportModel> StockReportWithDate(int ConcernID, int ProductID, int CompanyID, int CategoryID);

        IEnumerable<StockForcastingReportModel> StockForcastingReport(DateTime fromDate, DateTime toDate, int ConcernID);

        IEnumerable<StockForcastingReportModel> StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, int ProductID);
    }


}
