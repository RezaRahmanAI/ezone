using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SystemInformationService : ISystemInformationService
    {
        private readonly IBaseRepository<SystemInformation> _systemInformationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemInformationService(IBaseRepository<SystemInformation> systemInformationRepository, IUnitOfWork unitOfWork)
        {
            _systemInformationRepository = systemInformationRepository;
            _unitOfWork = unitOfWork;
        }

        public void UpdateSystemInformation(SystemInformation systemInformation)
        {
            _systemInformationRepository.Update(systemInformation);
        }

        public void SaveSystemInformation()
        {
            _unitOfWork.Commit();
        }

        public SystemInformation GetSystemInformationById(int id)
        {
            return _systemInformationRepository.FindBy(x => x.SystemInfoID == id).First();
        }
        public SystemInformation GetSystemInformationByConcernId(int id)
        {
            return _systemInformationRepository.FindBy(x => x.ConcernID == id).First();
        }

        public IQueryable<SystemInformation> GetAllConcernSysInfo()
        {
            return _systemInformationRepository.GetAll();
        }

        public bool IsApprovalSystemEnable()
        {
            return _systemInformationRepository.All.FirstOrDefault().ApprovalSystemEnable == 1 ? true : false;
        }
        public bool IsEditApprovalSystemEnable()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsEditApprovalSystemEnable == 1 ? true : false;
        }
        public bool IsBankBalanceShow()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsBankBalanceShow == 1 ? true : false;
        }
        public bool IsEditReqPermission()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsEditReqPermission == 1 ? true : false;
        }

        public bool IsEditReqPermissionFalse()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsEditReqPermission == 0 ? true : false;
        }




        //    public List<TOHomeWidget> GetHomeWidgeSales(string dataLength, int concernId)
        //    {
        //        DateTime dayLength = DateTime.Now;

        //        if (int.TryParse(dataLength, out int days))
        //        {
        //            dayLength = dayLength.AddDays(-days);
        //        }

        //     string query = string.Format(@"
        //    SELECT COALESCE(SUM(TotalAmount), 0) AS Amount, 'SalesAmount' AS TrType 
        //    FROM SOrders 
        //    WHERE ConcernID = {0} AND Status != 2 AND CAST(InvoiceDate AS DATE) >= CAST('{1}' AS DATE)

        //    UNION ALL

        //    SELECT COALESCE(SUM(TotalAmt), 0) AS Amount, 'PurchaseAmount' AS TrType 
        //    FROM POrders 
        //    WHERE ConcernID = {0}  AND Status != 2 AND CAST(OrderDate AS DATE) >= CAST('{1}' AS DATE)

        //    UNION ALL

        //    SELECT COALESCE(SUM(Amount), 0) AS Amount, 'CashCollection' AS TrType 
        //    FROM CashCollections 
        //    WHERE ConcernID = {0} AND TransactionType =1 AND CAST(EntryDate AS DATE) >= CAST('{1}' AS DATE)

        //    UNION ALL

        //    SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Income' AS TrType 
        //    FROM Expenditures ex
        //    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
        //    WHERE ex.ConcernID = {0} AND ei.Status = 2 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)

        //    UNION ALL

        //    SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Expense' AS TrType 
        //    FROM Expenditures ex
        //    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
        //    WHERE ex.ConcernID = {0} AND ei.Status = 1 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
        //", concernId, dayLength.ToString("yyyy-MM-dd"));

        //        try
        //        {
        //            return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            List<TOHomeWidget> list = new List<TOHomeWidget>();
        //            list.Add(new TOHomeWidget { Amount = 0m, TrType = "SalesAmount" });
        //            list.Add(new TOHomeWidget { Amount = 0m, TrType = "PurchaseAmount" });
        //            list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income" });
        //            list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense" });
        //            list.Add(new TOHomeWidget { Amount = 0m, TrType = "CashCollection" });
        //            return list;
        //        }
        //    }



        public List<TOHomeWidget> GetHomeWidgeSales(string dataLength, int concernId)
        {
            DateTime dayLength = DateTime.Now;

            if (int.TryParse(dataLength, out int days))
            {
                dayLength = dayLength.AddDays(-days);
            }

            string query = string.Format(@"
    SELECT 
        COALESCE(SUM(TotalAmount), 0) + 
        COALESCE((SELECT SUM(TSalesAmt) FROM CreditSales 
                 WHERE ConcernID = {0} AND Status != 2 
                 AND CAST(IssueDate AS DATE) >= CAST('{1}' AS DATE)), 0) AS Amount, 
        'SalesAmount' AS TrType 
    FROM SOrders 
    WHERE ConcernID = {0} AND Status != 2 AND CAST(InvoiceDate AS DATE) >= CAST('{1}' AS DATE)

    UNION ALL

    SELECT COALESCE(SUM(TotalAmt), 0) AS Amount, 'PurchaseAmount' AS TrType 
    FROM POrders 
    WHERE ConcernID = {0} AND Status != 2 AND CAST(OrderDate AS DATE) >= CAST('{1}' AS DATE)

    UNION ALL

    SELECT COALESCE(SUM(Amount), 0) AS Amount, 'CashCollection' AS TrType 
    FROM CashCollections 
    WHERE ConcernID = {0} AND TransactionType = 1 AND CAST(EntryDate AS DATE) >= CAST('{1}' AS DATE)

    UNION ALL

    SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Income' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 2 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)

    UNION ALL

    SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Expense' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 1 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
    ", concernId, dayLength.ToString("yyyy-MM-dd"));

            try
            {
                return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
            }
            catch (Exception ex)
            {
                List<TOHomeWidget> list = new List<TOHomeWidget>();
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "SalesAmount" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "PurchaseAmount" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "CashCollection" });
                return list;
            }
        }





        public List<TOHomeWidget> GetYearlyData(string dataLength, int concernId)
        {
            DateTime dayLength = DateTime.Now;
            int currentYear = DateTime.Now.Year;

            if (int.TryParse(dataLength, out int days))
            {
                dayLength = dayLength.AddDays(-days);
            }

            string query = string.Format(@"
    SELECT
        DATEPART(YEAR, InvoiceDate) AS Year,
        DATEPART(MONTH, InvoiceDate) AS Month,
        COALESCE(SUM(TotalAmount), 0) AS Amount,
        'SalesAmount' AS TrType 
    FROM SOrders 
    WHERE ConcernID = {0} AND Status != 2 AND InvoiceDate >= '{1}' AND DATEPART(YEAR, InvoiceDate) = {2}
    GROUP BY DATEPART(YEAR, InvoiceDate), DATEPART(MONTH, InvoiceDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, OrderDate) AS Year,
        DATEPART(MONTH, OrderDate) AS Month,
        COALESCE(SUM(TotalAmt), 0) AS Amount,
        'PurchaseAmount' AS TrType 
    FROM POrders 
    WHERE ConcernID = {0} AND Status != 2 AND OrderDate >= '{1}' AND DATEPART(YEAR, OrderDate) = {2}
    GROUP BY DATEPART(YEAR, OrderDate), DATEPART(MONTH, OrderDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, EntryDate) AS Year,
        DATEPART(MONTH, EntryDate) AS Month,
        COALESCE(SUM(Amount), 0) AS Amount,
        'CashCollection' AS TrType 
    FROM CashCollections 
    WHERE ConcernID = {0} AND EntryDate >= '{1}' AND DATEPART(YEAR, EntryDate) = {2}
    GROUP BY DATEPART(YEAR, EntryDate), DATEPART(MONTH, EntryDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, ex.EntryDate) AS Year,
        DATEPART(MONTH, ex.EntryDate) AS Month,
        COALESCE(SUM(ex.Amount), 0) AS Amount,
        'Income' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 2 AND ex.EntryDate >= '{1}' AND DATEPART(YEAR, ex.EntryDate) = {2}
    GROUP BY DATEPART(YEAR, ex.EntryDate), DATEPART(MONTH, ex.EntryDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, ex.EntryDate) AS Year,
        DATEPART(MONTH, ex.EntryDate) AS Month,
        COALESCE(SUM(ex.Amount), 0) AS Amount,
        'Expense' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 1 AND ex.EntryDate >= '{1}' AND DATEPART(YEAR, ex.EntryDate) = {2}
    GROUP BY DATEPART(YEAR, ex.EntryDate), DATEPART(MONTH, ex.EntryDate)
    ", concernId, dayLength.ToString("yyyy-MM-dd"), currentYear);

            try
            {
                return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
            }
            catch (Exception ex)
            {
                List<TOHomeWidget> list = new List<TOHomeWidget>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "SalesAmount", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "PurchaseAmount", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "CashCollection", Month = i, Year = DateTime.Now.Year });
                }
                return list;
            }
        }

        public bool IsParentCateShow()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsParentCateShow == 1 ? true : false;
        }
        public bool IsRPRateShow()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsRPRateShow == 1 ? true : false;
        }
        public bool IsEmobileCustomerView()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsEmobileCustomerView == 1 ? true : false;
        }
        public bool IsHoqueSOrderSalesRate()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsHoqueSOrderSalesRate == 1 ? true : false;
        }

        public bool IsCCDebitCreditAdjHide()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsCCDebitCreditAdjHide == 1 ? true : false;
        }
        public bool IsEcomputerShow()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsEcomputerShow == 1 ? true : false;
        }
        public bool IsProductTypeHide()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsProductTypeHide == 1 ? true : false;
        }

        public bool IsMrpUpdateShowForManager()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsMrpUpdateShowForManager == 1 ? true : false;
        }

        public bool IsDOShow()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsDOShow == 1 ? true : false;
        }
        public bool IsBankApprovalEnable()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsBankApprovalEnable == 1 ? true : false;
        }

        public bool IsDateWiseProductPicker()
        {
            return _systemInformationRepository.All.FirstOrDefault().IsDateWiseProductPicker == 1 ? true : false;
        }

    }
}


