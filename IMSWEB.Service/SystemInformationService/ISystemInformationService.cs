using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface ISystemInformationService
    {
        void UpdateSystemInformation(SystemInformation systemInformation);

        void SaveSystemInformation();

        SystemInformation GetSystemInformationById(int id);
        SystemInformation GetSystemInformationByConcernId(int id);
        IQueryable<SystemInformation> GetAllConcernSysInfo();

        bool IsApprovalSystemEnable();
        bool IsEditApprovalSystemEnable();
        bool IsBankBalanceShow();

        List<TOHomeWidget> GetHomeWidgeSales(string dataLength, int concernId);
        List<TOHomeWidget> GetYearlyData(string dataLength, int concernId);
        bool IsParentCateShow();
        bool IsRPRateShow();
        bool IsEmobileCustomerView();
        bool IsEditReqPermission();
        bool IsEditReqPermissionFalse();
        bool IsEcomputerShow();
        bool IsHoqueSOrderSalesRate();
        bool IsProductTypeHide();
        bool IsCCDebitCreditAdjHide();
        bool IsMrpUpdateShowForManager();
        bool IsDOShow();
        bool IsBankApprovalEnable();
        bool IsDateWiseProductPicker();

    }
}
