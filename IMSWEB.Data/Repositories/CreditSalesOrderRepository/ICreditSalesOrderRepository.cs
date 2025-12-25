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
    public interface ICreditSalesOrderRepository
    {
        Tuple<bool, int> AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedulesl, DataTable dtBankTrans);


        bool InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            DataTable dtBankTrans, int CardTypeSetupID);

        bool ReturnSalesOrderUsingSP(int orderId, int userId);

        //void CalculatePenaltySchedules(int ConcernID);
        void CorrectionStockData(int concermID);

        bool PendingInstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment,
            int CardTypeSetupID);

        Tuple<bool, int> AddPendingSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
                   DataTable dtSchedules);

        bool ApprovedSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
              DataTable dtSchedules, DataTable dtBankTrans, int OrderID);

        bool InstallmentApprovedSP(int orderId, decimal installmentAmount, decimal LastPayAdjustment,
                        DataTable dtBankTrans, int CardTypeSetupID, int ScheduleID);

    }
}
