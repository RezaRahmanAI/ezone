using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Service;
using IMSWEB.Model;
using System.Data;
using Microsoft.Reporting.WebForms;
using IMSWEB.Report.DataSets;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using IMSWEB;
using IMSWEB.SPViewModels;

using IMSWEB.Model.SPModel;
using System.Reflection;
using BarcodeLib.Barcode.RDLCReports;
using BarcodeLib.Barcode;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Web.UI;
using IMSWEB.Model.TO;

namespace IMSWEB.Report
{
    public class TransactionalReport : ITransactionalReport
    {
        DataSet _dataSet = null;
        ReportParameter _reportParameter = null;
        List<ReportParameter> _reportParameters = null;

        IExpenditureService _expenditureService;
        ISalesOrderService _salesOrderService;
        ISalesOrderDetailService _salesOrderDetailService;
        IPurchaseOrderService _purchaseOrderService;
        ICustomerService _customerService;
        IProductService _productService;
        ICreditSalesOrderService _creditSalesOrderService;
        IStockDetailService _stockdetailService;
        IStockService _StockServce;
        IColorService _ColorServce;
        ICashCollectionService _CashCollectionService;
        ISystemInformationService _systemInformationService;
        ISRVisitService _SRVisitService;
        ISRVisitDetailService _SRVisitDetailService;
        IEmployeeService _EmployeeService;
        ISRVProductDetailService _SRVProductDetailService;
        ICategoryService _CategoryService;
        ICompanyService _CompanyService;
        IUserService _userService;
        ISupplierService _SupplierService;
        IPurchaseOrderDetailService _PurchaseOrderDetailService;
        IBankTransactionService _bankTransactionService;
        IPOProductDetailService _POProductDetailService;
        IDesignationService _DesignationService;
        ISalaryMonthlyService _SalaryMonthlyService;
        IDepartmentService _DepartmentService;
        IGradeService _GradeService;
        IEmpGradeSalaryAssignmentService _GradeSalaryAssignment;
        IBankService _BankService;
        IExpenseItemService _ExpenseItemService;
        IAttendenceService _attendenceService;

        IROrderService _returnOrderService;
        IROrderDetailService _returnDetailOrderService;
        ITransferService _TransferService;
        ISisterConcernService _SisterConcernService;
        ISMSStatusService _SMSService;
        IAccountingService _AccountingService;
        IShareInvestmentService _ShareInvestmentService;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        private readonly IParentCategoryService _parentCategoryService;
        IAdvanceSalaryService _AdvanceSalaryService;
        private readonly IUserAuditDetailService _userAuditDetailService;
        private readonly IBankLoanCollectionService _bankLoanCollectionService;

        private readonly IBankLoanService _bankLoanService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        IDOService _DOService;

        public TransactionalReport(IExpenditureService expenditureService, ICustomerService customerService, IPurchaseOrderService purchaseOrderService, IBankTransactionService bankTransactionService, ICreditSalesOrderService creditSalesOrderService,
            ISalesOrderService salesOrderService, ISalesOrderDetailService salesOrderDetailService, IProductService productService, IStockDetailService stockDetailService, IStockService stockService, ICashCollectionService cashCollectionService,
            IColorService colorServce, ISystemInformationService systemInformationService,
            ISRVisitService srVisitService, ISRVisitDetailService srVisitDetailService, IEmployeeService employeeService,
            ISRVProductDetailService srVProductDetailService, ISupplierService SupplierService,
            ICategoryService categoryService, ICompanyService companyService, IUserService userservice, IPurchaseOrderDetailService PurchaseOrderDetailService,
            IPOProductDetailService POProductDetailService, IDesignationService DesignationService
            , ISalaryMonthlyService SalaryMonthlyService, IDepartmentService DepartmentService,
                IGradeService GradeService, IEmpGradeSalaryAssignmentService GradeSalaryAssignment, IBankService BankService, IExpenseItemService ExpenseItemService,
             IAttendenceService attendenceService,
            IROrderService returnOrderService,
         IROrderDetailService returnDetailOrderService, ITransferService TransferService, ISisterConcernService SisterConcernService,
            ISMSStatusService SMSService, IAccountingService AccountingService, IShareInvestmentService ShareInvestmentService,
            IShareInvestmentHeadService ShareInvestmentHeadService,
            IParentCategoryService parentCategoryService, IAdvanceSalaryService advanceSalaryService, IUserAuditDetailService userAuditDetailService, IBankLoanService bankLoanService, ISMSBillPaymentBkashService smsBillPaymentBkashService, IBankLoanCollectionService bankLoanCollectionService, IDOService dOService

            )
        {
            _expenditureService = expenditureService;
            _salesOrderService = salesOrderService;
            _productService = productService;
            _stockdetailService = stockDetailService;
            _customerService = customerService;
            _purchaseOrderService = purchaseOrderService;
            _bankTransactionService = bankTransactionService;
            _StockServce = stockService;
            _systemInformationService = systemInformationService;
            _salesOrderDetailService = salesOrderDetailService;
            _CashCollectionService = cashCollectionService;
            _ColorServce = colorServce;
            _creditSalesOrderService = creditSalesOrderService;
            _SRVisitService = srVisitService;
            _SRVisitDetailService = srVisitDetailService;
            _EmployeeService = employeeService;
            _SRVProductDetailService = srVProductDetailService;
            _CategoryService = categoryService;
            _CompanyService = companyService;
            _userService = userservice;
            _SupplierService = SupplierService;
            _PurchaseOrderDetailService = PurchaseOrderDetailService;
            _POProductDetailService = POProductDetailService;
            _DesignationService = DesignationService;
            _SalaryMonthlyService = SalaryMonthlyService;
            _DepartmentService = DepartmentService;
            _GradeService = GradeService;
            _GradeSalaryAssignment = GradeSalaryAssignment;
            _BankService = BankService;
            _ExpenseItemService = ExpenseItemService;
            _attendenceService = attendenceService;
            _returnOrderService = returnOrderService;
            _returnDetailOrderService = returnDetailOrderService;
            _TransferService = TransferService;
            _SisterConcernService = SisterConcernService;
            _SMSService = SMSService;
            _AccountingService = AccountingService;
            _ShareInvestmentService = ShareInvestmentService;
            _ShareInvestmentHeadService = ShareInvestmentHeadService;
            _parentCategoryService = parentCategoryService;
            _AdvanceSalaryService = advanceSalaryService;
            _userAuditDetailService = userAuditDetailService;
            _bankLoanService = bankLoanService;
            _bankLoanCollectionService = bankLoanCollectionService;
            _smsBillPaymentBkashService = smsBillPaymentBkashService;
            _DOService = dOService;
        }

        public static string TakaFormat(double TotalAmt)
        {

            string sInWords = string.Empty;

            string sPoisa = string.Empty;
            string[] words = TotalAmt.ToString().Split('.');
            string sTaka = words[0];
            if (words.Length == 1)
            {
                sPoisa = "00";
            }
            else
            {
                sPoisa = words[1];
                if (sPoisa.Length == 1)
                {
                    sPoisa = sPoisa + "0";
                }
            }

            int i = sTaka.Length;
            string sDH1 = string.Empty;

            if (i == 9)
            {
                sDH1 = Spell.SpellAmount.F_Crores(sTaka, sPoisa);
            }
            else if (i == 8)
            {
                sDH1 = Spell.SpellAmount.F_Crore(sTaka, sPoisa);
            }
            else if (i == 7)
            {
                sDH1 = Spell.SpellAmount.F_Lakhs(sTaka, sPoisa);
            }
            else if (i == 6)
            {
                sDH1 = Spell.SpellAmount.F_Lakh(sTaka, sPoisa);
            }
            else if (i == 5)
            {
                sDH1 = Spell.SpellAmount.F_Thousands(sTaka, sPoisa);
            }
            else if (i == 4)
            {
                sDH1 = Spell.SpellAmount.F_Thousand(sTaka, sPoisa);
            }
            else if (i == 3)
            {
                sDH1 = Spell.SpellAmount.F_Hundred(sTaka, sPoisa);
            }
            else if (i == 2)
            {
                sDH1 = Spell.SpellAmount.Tens(sTaka);
            }

            sInWords = sDH1 + ".";

            return sInWords;

        }
        public byte[] ExpenditureReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, EnumCompanyTransaction Status,
            int ExpenseItemID, bool isAdminReport, int selectedConcernID)
        {
            try
            {
                DataRow row = null;
                TransactionalDataSet.dtExpenditureDataTable dtExpenditure = new TransactionalDataSet.dtExpenditureDataTable();

                var expenseInfos = _expenditureService.GetforExpenditureReport(fromDate, toDate, selectedConcernID, Status, ExpenseItemID, isAdminReport);

                foreach (var item in expenseInfos)
                {

                    row = dtExpenditure.NewRow();
                    row["ExpDate"] = item.Item1.ToString("dd MMM yyyy");
                    row["Description"] = item.Item3;
                    row["Amount"] = item.Item4.ToString("#,###");
                    row["ItemName"] = item.Item2;// item.ExpenseItem.Description;
                    row["VoucherNo"] = item.Item5;
                    row["UserName"] = item.Item6;
                    row["Concern"] = item.Item7;
                    if (item.Rest.Item1 == 0)
                    {
                        row["Status"] = "Show";
                    }
                    else
                    {
                        row["Status"] = "Don't Show";
                    }
                    dtExpenditure.Rows.Add(row);
                }

                dtExpenditure.TableName = "dtExpenditure";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtExpenditure);

                GetCommonParameters(userName, concernID);
                if (Status == EnumCompanyTransaction.Expense)
                    _reportParameter = new ReportParameter("Month", "Expense Report from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                else
                    _reportParameter = new ReportParameter("Month", "Income Report from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                if (isAdminReport)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptAdminExpenditure.rdlc");
                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptExpenditure.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int reportType, string period, int CustomerType, bool IsAdminReport,
            string ClientDateTime, int selectedConcernID)
        {
            try
            {
                #region Summary
                if (reportType == 10)
                {
                    var salseInfos = _salesOrderService.GetSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);
                    var CreditsalseInfos = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);
                    var ReturnInfos = _returnOrderService.GetReturnReportByConcernID(fromDate, toDate, concernID, CustomerType);

                    decimal Sales = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Item5) : 0m;
                    decimal CrediSales = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Item5) : 0m;
                    decimal ReturnAmt = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Item5) : 0m;
                    decimal NetSales = Sales + CrediSales - ReturnAmt;

                    decimal SalesRec = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal CrediSalesRec = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal ReturnAmtRec = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal NetSalesRec = SalesRec + CrediSalesRec - ReturnAmtRec;

                    decimal SalesDue = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal CrediSalesDue = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal ReturnAmtDue = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal NetSalesDue = SalesDue + CrediSalesDue - ReturnAmtDue;

                    DataRow row = null;
                    TransactionalDataSet.dtOrderDataTable dt = new TransactionalDataSet.dtOrderDataTable();
                    //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();
                    TransactionalDataSet.dtReturnOrderDataTable dtReturn = new TransactionalDataSet.dtReturnOrderDataTable();
                    #region Cash Sales
                    foreach (var item in salseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Cash Sales";
                        row["AdjustAmt"] = item.Rest.Item3;
                        row["TotalOffer"] = item.Rest.Item4;
                        dt.Rows.Add(row);
                    }
                    #endregion

                    #region Credit Sales
                    foreach (var item in CreditsalseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Credit Sales";
                        row["AdjustAmt"] = 0;
                        row["TotalOffer"] = item.Rest.Item4;
                        row["InstallmentPeriod"] = item.Rest.Item7;

                        dt.Rows.Add(row);
                    }
                    #endregion

                    #region Return Sales
                    foreach (var item in ReturnInfos)
                    {
                        row = dtReturn.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Cash Sales";
                        row["AdjustAmt"] = item.Rest.Item3;
                        row["TotalOffer"] = item.Rest.Item4;

                        dtReturn.Rows.Add(row);
                    }
                    #endregion

                    dt.TableName = "dtOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    dtReturn.TableName = "dtReturnOrder";
                    _dataSet.Tables.Add(dtReturn);
                    GetCommonParameters(userName, concernID);

                    _reportParameter = new ReportParameter("NetSales", NetSales.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("NetSalesRec", NetSalesRec.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("NetSalesDue", NetSalesDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Month", "Sales report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Month", "Sales report for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Month", "Sales report for the Year : " + fromDate.ToString("yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMonthlyOrder.rdlc");
                }
                #endregion

                #region Summary report
                else if (reportType == 1)
                {
                    TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                    TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                    DataRow row = null;

                    List<ProductWiseSalesReportModel> productWiseSalesReports = new List<ProductWiseSalesReportModel>();
                    if (CustomerType != (int)EnumCustomerType.Hire)
                    {
                        var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport, CustomerType);
                        var returnDetailInfos = _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, CustomerType, selectedConcernID, IsAdminReport);
                        salseDetailInfos.Concat(returnDetailInfos);

                        var retailSalesData = ((from item in salseDetailInfos
                                                group item by new
                                                {
                                                    SalesDate = item.Item1.Date,
                                                    CustomerType = item.Rest.Rest.Item3,
                                                    CategoryName = item.Rest.Rest.Item4,
                                                    ConcernName = item.Rest.Rest.Item7
                                                } into g
                                                select new ProductWiseSalesReportModel
                                                {
                                                    ConcernName = g.Key.ConcernName,
                                                    Date = g.Key.SalesDate.Date,
                                                    CustomerType = g.Key.CustomerType,
                                                    CategoryName = g.Key.CategoryName,
                                                    Quantity = g.Sum(o => o.Rest.Item5),
                                                    TotalAmount = g.Sum(o => o.Rest.Rest.Rest.Item1),
                                                }));

                        productWiseSalesReports.AddRange(retailSalesData);
                    }

                    if (CustomerType == 0 || CustomerType == (int)EnumCustomerType.Hire)
                    {
                        var CreditsalseDetailInfos = _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport);
                        var creditSalesData = ((from item in CreditsalseDetailInfos
                                                group item by new
                                                {
                                                    SalesDate = item.Item1.Date,
                                                    CustomerType = item.Rest.Rest.Item2,
                                                    CategoryName = item.Rest.Rest.Item3,
                                                    ConcernName = item.Rest.Rest.Item7
                                                } into g
                                                select new ProductWiseSalesReportModel
                                                {
                                                    ConcernName = g.Key.ConcernName,
                                                    Date = g.Key.SalesDate.Date,
                                                    CustomerType = g.Key.CustomerType,
                                                    CategoryName = g.Key.CategoryName,
                                                    Quantity = g.Sum(o => o.Rest.Item5),
                                                    TotalAmount = g.Sum(o => o.Rest.Item5 * o.Item5),
                                                }));

                        foreach (var grd in creditSalesData)
                        {
                            row = dt.NewRow();
                            row["SalesDate"] = grd.Date.ToString("dd MMM yyyy"); ;
                            row["NetTotal"] = grd.TotalAmount;
                            row["Quantity"] = grd.Quantity;
                            row["CategoryName"] = grd.CategoryName;
                            row["CustomerType"] = (EnumCustomerType)grd.CustomerType;
                            row["ConcernName"] = grd.ConcernName;
                            dt.Rows.Add(row);
                        }
                    }


                    decimal TotalDueSales = 0, AdjAmount = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;





                    foreach (var grd in productWiseSalesReports)
                    {
                        row = dt.NewRow();
                        row["SalesDate"] = grd.Date.ToString("dd MMM yyyy"); ;
                        row["NetTotal"] = grd.TotalAmount;
                        row["Quantity"] = grd.Quantity;
                        row["CategoryName"] = grd.CategoryName;
                        row["CustomerType"] = (EnumCustomerType)grd.CustomerType;
                        row["ConcernName"] = grd.ConcernName;

                        dt.Rows.Add(row);
                    }



                    decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                    decimal ReturnGrandTotal = 0;
                    decimal ReturnTotalDis = 0;
                    decimal ReturnNetTotal = 0;
                    decimal ReturnRecAmt = 0;
                    decimal ReturnCurrDue = 0;

                    decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                    decimal NetGrandTotal = 0;
                    decimal NetTotalDis = 0;
                    decimal NetNetTotal = 0;
                    decimal NetRecAmt = 0;
                    decimal NetCurrDue = 0;


                    NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                    NetAdjAmount = AdjAmount - ReturnAdjAmount;
                    NetGrandTotal = GrandTotal - ReturnGrandTotal;
                    NetTotalDis = TotalDis - ReturnTotalDis;
                    NetNetTotal = NetTotal - ReturnNetTotal;
                    NetRecAmt = RecAmt - ReturnRecAmt;
                    NetCurrDue = CurrDue - ReturnCurrDue;

                    dt.TableName = "dtCustomerWiseSales";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    dtReturn.TableName = "dtCustomerWiseReturn";
                    _dataSet.Tables.Add(dtReturn);


                    GetCommonParameters(userName, concernID);
                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Date", "Sales Summary  for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Date", "Sales Summary for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Date", "Sales Summary  the Year : " + fromDate.ToString("yyyy"));

                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminSalesSummary.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMonthlySalesSummary.rdlc");
                }
                #endregion

                #region Details Report
                else
                {


                    TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                    TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                    int SOrderID = 0, CreditSaleID = 0;
                    decimal TotalDueSales = 0, AdjAmount = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;

                    decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                    decimal ReturnGrandTotal = 0;
                    decimal ReturnTotalDis = 0;
                    decimal ReturnNetTotal = 0;
                    decimal ReturnRecAmt = 0;
                    decimal ReturnCurrDue = 0;
                    if (CustomerType == (int)EnumCustomerType.Dealer || CustomerType == (int)EnumCustomerType.Retail)
                    {
                        var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport, CustomerType);
                        var returnDetailInfos = _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, CustomerType, selectedConcernID, IsAdminReport);
                        foreach (var grd in salseDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                            if (SOrderID != grd.Rest.Rest.Item1)
                            {
                                TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                                GrandTotal = GrandTotal + (decimal)grd.Item7;
                                TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                                NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                                RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                                CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                                AdjAmount = AdjAmount + grd.Rest.Rest.Item2;
                            }
                            SOrderID = grd.Rest.Rest.Item1;
                            dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, (grd.Rest.Rest.Rest.Item1 / grd.Rest.Item5), grd.Rest.Rest.Rest.Item1, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2, grd.Rest.Rest.Item7);
                        }
                        SOrderID = 0;
                        foreach (var grd in returnDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                            if (SOrderID != grd.Rest.Rest.Item1)
                            {
                                ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                                ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                                ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                                ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                                ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                                ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                                ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                            }
                            SOrderID = grd.Rest.Rest.Item1;
                            dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2, grd.Rest.Rest.Item7);
                        }
                    }
                    else if (CustomerType == (int)EnumCustomerType.Hire)
                    {

                        var CreditsalseDetailInfos = _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport);
                        //For Credit Sales
                        foreach (var grd in CreditsalseDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                            if (CreditSaleID != grd.Rest.Rest.Item1)
                            {
                                TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                                GrandTotal = GrandTotal + (decimal)grd.Item7;
                                TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                                NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                                RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                                CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                            }
                            CreditSaleID = grd.Rest.Rest.Item1;
                            dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m, grd.Rest.Rest.Item7);
                        }
                    }
                    else
                    {
                        var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport, CustomerType);
                        var returnDetailInfos = _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, CustomerType, selectedConcernID, IsAdminReport);
                        foreach (var grd in salseDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                            if (SOrderID != grd.Rest.Rest.Item1)
                            {
                                TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                                GrandTotal = GrandTotal + (decimal)grd.Item7;
                                TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                                NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                                RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                                CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                                AdjAmount = AdjAmount + grd.Rest.Rest.Item2;
                            }
                            SOrderID = grd.Rest.Rest.Item1;
                            dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, (grd.Rest.Rest.Rest.Item1 / grd.Rest.Item5), grd.Rest.Rest.Rest.Item1, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2, "", grd.Rest.Rest.Item4, grd.Rest.Rest.Item7);
                        }
                        foreach (var grd in returnDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                            if (SOrderID != grd.Rest.Rest.Item1)
                            {
                                ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                                ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                                ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                                ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                                ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                                ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                                ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                            }
                            SOrderID = grd.Rest.Rest.Item1;
                            dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2, grd.Rest.Rest.Item7);
                        }
                        var CreditsalseDetailInfos = _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, selectedConcernID, IsAdminReport);
                        //For Credit Sales
                        foreach (var grd in CreditsalseDetailInfos)
                        {
                            //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                            if (CreditSaleID != grd.Rest.Rest.Item1)
                            {
                                TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                                GrandTotal = GrandTotal + (decimal)grd.Item7;
                                TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                                NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                                RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                                CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                            }
                            CreditSaleID = grd.Rest.Rest.Item1;
                            dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Rest.Rest.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m, "", grd.Rest.Rest.Item4, grd.Rest.Rest.Item7);
                        }
                    }

                    decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                    decimal NetGrandTotal = 0;
                    decimal NetTotalDis = 0;
                    decimal NetNetTotal = 0;
                    decimal NetRecAmt = 0;
                    decimal NetCurrDue = 0;

                    NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                    NetAdjAmount = AdjAmount - ReturnAdjAmount;
                    NetGrandTotal = GrandTotal - ReturnGrandTotal;
                    NetTotalDis = TotalDis - ReturnTotalDis;
                    NetNetTotal = NetTotal - ReturnNetTotal;
                    NetRecAmt = RecAmt - ReturnRecAmt;
                    NetCurrDue = CurrDue - ReturnCurrDue;

                    dt.TableName = "dtCustomerWiseSales";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    dtReturn.TableName = "dtCustomerWiseReturn";
                    _dataSet.Tables.Add(dtReturn);

                    GetCommonParameters(userName, concernID);
                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Date", "Sales details for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Date", "Sales details the Year : " + fromDate.ToString("yyyy"));

                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
                    _reportParameters.Add(_reportParameter);

                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminSalesDetails.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesDetails.rdlc");
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public byte[] PurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType,
            string period, EnumPurchaseType PurchaseType, bool IsAdminReport, int SelectedConcernID)
        {
            try
            {

                if (reportType == 1)
                {
                    var purchaseInfos = _purchaseOrderService.GetPurchaseReport(fromDate, toDate, PurchaseType, IsAdminReport, concernID);

                    DataRow row = null;

                    TransactionalDataSet.dtReceiveOrderDataTable dt = new TransactionalDataSet.dtReceiveOrderDataTable();
                    //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                    foreach (var item in purchaseInfos)
                    {
                        row = dt.NewRow();
                        row["CompanyCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["OrderDare"] = item.Item3.ToString("dd MMM yyyy");
                        row["ChallanNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DisAmt"] = item.Item6;
                        row["TotalAmt"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmt"] = item.Rest.Item2;
                        row["ConcernName"] = item.Rest.Item3;
                        row["InvoiceNo"] = item.Rest.Item4;

                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtReceiveOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    GetCommonParameters(userName, concernID);
                    if (PurchaseType == EnumPurchaseType.Purchase)
                        _reportParameter = new ReportParameter("Month", "Purchase report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (PurchaseType == EnumPurchaseType.ProductReturn)
                        _reportParameter = new ReportParameter("Month", "Purchase Return report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));

                    _reportParameters.Add(_reportParameter);
                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseOrder.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseOrder.rdlc");
                }
                else
                {
                    var Purchases = _purchaseOrderService.ProductWisePurchaseDetailsReport(0, 0, 0, fromDate, toDate, EnumPurchaseType.Purchase, IsAdminReport, SelectedConcernID, 0);


                    decimal TotalDuePurchase = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;
                    decimal TotalPPDis = 0;
                    decimal OnlyDisAmt = 0;
                    int POrderID = 0;
                    DataRow row = null;
                    if (Purchases != null)
                    {
                        TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();
                        GrandTotal = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().GrandTotal);
                        TotalPPDis = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().TotalPPDis);
                        OnlyDisAmt = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().OnlyDisAmt);
                        TotalDis = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().NetDiscount);
                        NetTotal = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().NetTotal);
                        RecAmt = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().RecAmt);
                        CurrDue = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().PaymentDue);







                        foreach (var item in Purchases)
                        {
                            if (POrderID != item.POrderID)
                            {
                                TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                                //GrandTotal = GrandTotal + item.GrandTotal;
                                //TotalDis = TotalDis + item.NetDiscount;
                                //NetTotal = NetTotal + (item.GrandTotal - item.NetDiscount);
                                //RecAmt = RecAmt + item.RecAmt;
                                //CurrDue = CurrDue + item.PaymentDue;
                                //TotalPPDis = TotalPPDis + (item.NetDiscount - item.OnlyDisAmt);
                                //OnlyDisAmt = OnlyDisAmt + item.OnlyDisAmt;

                            }
                            //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                            //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, item.Rest.Rest.Item2, item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);
                            POrderID = item.POrderID;
                            row = dt.NewRow();
                            row["PurchaseDate"] = item.Date;
                            row["ChallanNo"] = item.ChallanNo;
                            row["ProductName"] = item.ProductName;
                            row["PurchaseRate"] = item.AfterFlatDisPurchaseRate;
                            row["DisAmt"] = item.PPDISAmt;
                            row["NetAmt"] = item.TotalAmount;
                            row["GrandTotal"] = item.GrandTotal;
                            row["TotalDis"] = item.NetDiscount;
                            row["NetTotal"] = (item.GrandTotal - item.NetDiscount); //item.TotalAmount;
                            row["PaidAmt"] = item.RecAmt;
                            row["RemainingAmt"] = item.PaymentDue;
                            row["Quantity"] = item.Quantity;
                            row["ChasisNo"] = string.Join(Environment.NewLine, item.IMEIs);
                            row["Model"] = item.CategoryName;
                            row["Color"] = item.ColorName;
                            row["PPOffer"] = item.PPOffer;
                            row["DamageIMEI"] = item.DamageIMEI;
                            row["ConcernName"] = item.ConcenName;
                            row["InvoiceNo"] = item.InvoiceNo;
                            row["OnlyDisAmt"] = item.OnlyDisAmt;
                            row["TotalPPDis"] = item.TotalPPDis;
                            POrderID = item.POrderID;
                            dt.Rows.Add(row);
                        }

                        dt.TableName = "dtSuppWiseData";
                        _dataSet = new DataSet();
                        _dataSet.Tables.Add(dt);

                        GetCommonParameters(userName, concernID);
                        if (PurchaseType == EnumPurchaseType.ProductReturn)
                        {

                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details the Year : " + fromDate.ToString("yyyy"));
                        }
                        else
                        {
                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase details the Year : " + fromDate.ToString("yyyy"));
                        }

                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("OnlyDisAmt", OnlyDisAmt.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("TotalPPDis", TotalPPDis.ToString());
                        _reportParameters.Add(_reportParameter);

                    }

                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseDetails.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseDetails.rdlc");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SalesInvoiceReportPrint(SOrder oOrder, string userName, int concernID, bool isPreview = false)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);
                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                //string EmployeeName = _EmployeeService.GetEmpNameById(oOrder.CreatedBy);
                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;

                var ProductInfos = (from sd in oOrder.SOrderDetails
                                    join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                    join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                    join col in _ColorServce.GetAll() on std.ColorID equals col.ColorID
                                    select new
                                    {
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        Quantity = sd.Quantity,
                                        UnitPrice = sd.UnitPrice,
                                        SalesRate = sd.UTAmount,
                                        UTAmount = sd.UTAmount,
                                        PPDPercentage = sd.PPDPercentage,
                                        PPDAmount = sd.PPDAmount,
                                        PPOffer = sd.PPOffer,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CompanyName = p.CompanyName,
                                        CategoryName = p.CategoryName,
                                        Compressor = sd.Compressor,
                                        Motor = sd.Motor,
                                        Service = sd.Service,
                                        Spareparts = sd.Spareparts,
                                        Panel = sd.Panel,
                                        warr = sd.Warranty
                                    }).ToList();

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPDAmount,
                                            w.PPDPercentage,
                                            w.PPOffer,
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList(),
                                            warr = g.Select(i => i.warr).FirstOrDefault()
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName + "," + item.CategoryName + "," + item.CompanyName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName, item.warr);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.TotalAmount));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                var sysInfo = GetCommonParameters(userName, concernID);


                _reportParameter = new ReportParameter("FlatPercentage", oOrder.TDPercentage.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TDiscount", oOrder.NetDiscount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AdjAmount", (oOrder.AdjAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", (oOrder.PrevDue + oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Code", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PreviousDue", (oOrder.PrevDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
                var checkTramsAndCodition = currentSystemInfo.TramsAndCondition;

                if (isPreview)
                {
                    _reportParameter = new ReportParameter("Preview", "Preview Invoice");
                    _reportParameters.Add(_reportParameter);
                }
                else
                {
                    _reportParameter = new ReportParameter("Preview", "Sales Invoice");
                    _reportParameters.Add(_reportParameter);
                }


                if (currentSystemInfo.Name == "Maa Electronics (Head)" || currentSystemInfo.Name == "Maa Electronics(Nagar Bandar Branch)" ||
                    currentSystemInfo.Name == "Maa Electronics (Mokamtola Branch)" || currentSystemInfo.Name == "Maa Electronics (Sonatola Branch)" ||
                    currentSystemInfo.Name == "Maa Electronics (Dupchachia Branch)_____OLD" || currentSystemInfo.Name == "Maa Electronics (Gobindaganj Branch)" ||
                    currentSystemInfo.Name == "AC World (Rangpur Branch)" || currentSystemInfo.Name == " Maa Electronics (Birampur Branch)" ||
                    currentSystemInfo.Name == "Maa Electronics (Amtoly Branch)" || currentSystemInfo.Name == "Maa Electronics (Pirganj Branch)" ||
                    currentSystemInfo.Name == "AC World (Bogura Branch)" || currentSystemInfo.Name == "AC World (Dupchachia Branch)" ||
                    currentSystemInfo.Name == "Mother Electronics" || currentSystemInfo.Name == "Maa Electronics (Thana Branch)")
                {
                    _reportParameter = new ReportParameter("VatRegNo", currentSystemInfo.VatRegNo);
                    _reportParameters.Add(_reportParameter);

                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceME.rdlc");

                }

                if (checkTramsAndCodition == true)
                {
                    _reportParameter = new ReportParameter("TramsAndCondition", oOrder.TramsAndCondition);
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice_TramsAndCondition.rdlc");
                }


                if (concernID == (int)EnumSisterConcern.Beauty_1 || concernID == (int)EnumSisterConcern.Beauty_2)
                {
                    _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice_beauty.rdlc");
                }


                if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
                {
                    _reportParameter = new ReportParameter("Msg", " বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                    _reportParameters.Add(_reportParameter);
                }
                else if (concernID == (int)EnumSisterConcern.Ityadi_Electronic || concernID == (int)EnumSisterConcern.SHOPNO_PURON)
                {
                    _reportParameter = new ReportParameter("Msg", "এই পন্যটির বিক্রয়লব্ধ অর্থের একটি অংশ গরিব অসহায় শিশুদের সু - চিকিৎসার জন্য নিবেদিত");
                    _reportParameters.Add(_reportParameter);
                }


                if (concernID == (int)EnumSisterConcern.Niyamot)
                {
                    _reportParameter = new ReportParameter("Msg", " ব্যালেন্সের অথবা পন্য ও পন্যর মূল্যের কোন গড়মিল পরিলক্ষিত হলে 03 দিনের মধ্যে অবহিত করুন। অন্যথায় আপনার দিক হতে হিসাব ঠিক বলে বিবেচিত হবে। আদেশক্রমে কৃর্তপক্ষ");
                    _reportParameters.Add(_reportParameter);
                }
                //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice.rdlc");

                if (sysInfo.IsEcomputerShow == 1)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceEC.rdlc");

                if (sysInfo.IsSalesPPDiscountShow == 1)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice.rdlc");


                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceWPPD.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] WarrantyInvoicePrint(SOrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyNewDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyNewDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);
                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;

                var ProductInfos = (from sd in oOrder.SOrderDetails
                                    join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                    join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                    join col in _ColorServce.GetAll() on std.ColorID equals col.ColorID
                                    select new
                                    {
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        Quantity = sd.Quantity,
                                        UnitPrice = sd.UnitPrice,
                                        SalesRate = sd.UTAmount,
                                        UTAmount = sd.UTAmount,
                                        PPDPercentage = sd.PPDPercentage,
                                        PPDAmount = sd.PPDAmount,
                                        PPOffer = sd.PPOffer,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CompanyName = p.CompanyName,
                                        CategoryName = p.CategoryName,
                                        Compressor = p.CompressorWarrentyMonth,
                                        Motor = p.MotorWarrentyMonth,
                                        Service = p.ServiceWarrentyMonth,
                                        Spareparts = p.SparePartsWarrentyMonth,
                                        Panel = p.PanelWarrentyMonth,
                                        UserInputWarranty = p.UserInputWarranty
                                    }).ToList();

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPDAmount,
                                            w.PPDPercentage,
                                            w.PPOffer,
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList(),
                                            UserInputWarranty = g.Select(i => i.UserInputWarranty).FirstOrDefault(),
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Warranty Period: " + "Compressor: " + item.Compressor + ",";
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + ",";
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + ",";
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + ",";
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;
                    if (!string.IsNullOrEmpty(item.UserInputWarranty))
                        Warrenty = Warrenty + "," + item.UserInputWarranty;

                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrentyNew";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.TotalAmount));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                var sysInfo = GetCommonParameters(userName, concernID);


                _reportParameter = new ReportParameter("FlatPercentage", oOrder.TDPercentage.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TDiscount", oOrder.NetDiscount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AdjAmount", (oOrder.AdjAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", (oOrder.PrevDue + oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Code", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PreviousDue", (oOrder.PrevDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
                var checkTramsAndCodition = currentSystemInfo.TramsAndCondition;

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\WarrantySalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public byte[] ChallanReportPrint(SOrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);

                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;
                var ProductInfos = from sd in oOrder.SOrderDetails
                                   join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                   join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       PPDPercentage = sd.PPDPercentage,
                                       PPDAmount = sd.PPDAmount,
                                       PPOffer = sd.PPOffer,
                                       IMENO = std.IMENO,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       Compressor = sd.Compressor,
                                       Motor = sd.Motor,
                                       Service = sd.Service,
                                       Spareparts = sd.Spareparts,
                                       Panel = sd.Panel,
                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new { w.ProductName, w.CategoryName, w.ColorName, w.CompanyName, w.UnitPrice, w.PPDAmount, w.PPDPercentage, w.PPOffer } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            SalesRate = g.Sum(i => i.UTAmount),
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList()
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.RecAmount));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("TDiscount", oOrder.TDAmount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);
                //_reportParameter =new ReportParameter("InWordTK", sInwodTk);
                //_reportParameters.Add(_reportParameter);

                //if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\Challan.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] CreditChallanReportPrint(CreditSale oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);

                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;
                var ProductInfos = from sd in oOrder.CreditSaleDetails
                                   join std in _stockdetailService.GetAll() on sd.StockDetailID equals std.SDetailID
                                   join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       PPDPercentage = 0m,
                                       PPDAmount = 0m,
                                       PPOffer = sd.PPOffer,
                                       IMENO = std.IMENO,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       Compressor = sd.Compressor,
                                       Motor = sd.Motor,
                                       Service = sd.Service,
                                       Spareparts = sd.Spareparts,
                                       Panel = sd.Panel,
                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new { w.ProductName, w.CategoryName, w.ColorName, w.CompanyName, w.UnitPrice, w.PPDAmount, w.PPDPercentage, w.PPOffer, w.UTAmount } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            SalesRate = g.Key.UTAmount,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList()
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.DownPayment));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("TDiscount", oOrder.Discount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.NetAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.DownPayment.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.Remaining).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.SalesDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);
                //_reportParameter =new ReportParameter("InWordTK", sInwodTk);
                //_reportParameters.Add(_reportParameter);

                //if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\Challan.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] SalesInvoiceReport(SOrder oOrder, string userName, int concernID, bool isPreview)
        {
            return SalesInvoiceReportPrint(oOrder, userName, concernID, isPreview);
        }

        public byte[] ChallanReport(SOrder oOrder, string userName, int concernID)
        {
            return ChallanReportPrint(oOrder, userName, concernID);
        }


        //public byte[] CreditChallanReport(SOrder oOrder, string userName, int concernID)
        //{
        //    return CreditChallanReportPrint(oOrder, userName, concernID);
        //}


        public byte[] SalesInvoiceReport(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return SalesInvoiceReportPrint(oOrder, userName, concernID);

        }


        public byte[] ChallanReport(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return ChallanReportPrint(oOrder, userName, concernID);

        }

        public byte[] CreditChallanReport(int oOrderID, string userName, int concernID)
        {
            CreditSale oOrder = new CreditSale();
            oOrder = _creditSalesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.CreditSaleDetails = _creditSalesOrderService.GetSalesOrderDetails(Convert.ToInt32(oOrderID)).ToList();

            return CreditChallanReportPrint(oOrder, userName, concernID);

        }
        public byte[] CreditSalesInvoiceReportPrint(CreditSale oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.CreditSalesInfoDataTable dt = new TransactionalDataSet.CreditSalesInfoDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);

                DataRow oSDRow = null;
                Product product = null;
                StockDetail oSTDetail = null;
                //Color oColor = null; // Test Parpuse
                int count = 1;
                string Warrenty = string.Empty;

                foreach (CreditSalesSchedule item in oOrder.CreditSalesSchedules)
                {
                    oSDRow = dt.NewRow();

                    oSDRow["ScheduleNo"] = count;
                    oSDRow["PaymentDate"] = item.PaymentStatus == "Paid" ? Convert.ToDateTime(item.PaymentDate).ToString("dd MMM yyyy") : "";
                    oSDRow["Balance"] = item.Balance;
                    oSDRow["InstallmetAmt"] = item.InstallmentAmt;
                    oSDRow["ClosingBalance"] = item.ClosingBalance;
                    oSDRow["PaymentStatus"] = item.PaymentStatus;
                    oSDRow["ScheduleDate"] = Convert.ToDateTime(item.MonthDate).ToString("dd MMM yyyy");
                    dt.Rows.Add(oSDRow);
                    count++;


                }

                dt.TableName = "CreditSalesInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                TransactionalDataSet.CSalesProductDataTable CSProductDT = new TransactionalDataSet.CSalesProductDataTable();
                DataRow oCSPRow = null;
                int nCOunt = 1;
                #region Product Details
                var Products = from csd in oOrder.CreditSaleDetails
                               join p in _productService.GetAllProductIQueryable() on csd.ProductID equals p.ProductID
                               join cat in _CategoryService.GetAllIQueryable() on p.CategoryID equals cat.CategoryID
                               join sd in _stockdetailService.GetAll() on csd.StockDetailID equals sd.SDetailID
                               join col in _ColorServce.GetAllColor() on sd.ColorID equals col.ColorID
                               select new
                               {
                                   p.ProductID,
                                   p.ProductName,
                                   p.ProductCode,
                                   CategoryName = cat.Description,
                                   IMEI = sd.IMENO,
                                   cat.CategoryID,
                                   ColorName = col.Name,
                                   csd.UnitPrice,
                                   csd.PPOffer,
                                   csd.UTAmount,
                                   csd.Quantity,
                                   csd.Compressor,
                                   csd.Motor,
                                   csd.Panel,
                                   csd.Spareparts,
                                   csd.Service,
                                   csd.IntTotalAmt
                               };
                var GroupProducts = from p in Products
                                    group p by new
                                    {
                                        p.ProductID,
                                        p.ProductCode,
                                        p.ProductName,
                                        p.CategoryID,
                                        p.CategoryName,
                                        p.ColorName,
                                        p.UnitPrice,
                                        p.UTAmount,
                                        p.PPOffer,
                                        p.IntTotalAmt
                                    } into g
                                    select new
                                    {
                                        g.Key.ProductID,
                                        g.Key.ProductCode,
                                        g.Key.ProductName,
                                        g.Key.CategoryName,
                                        g.Key.ColorName,
                                        g.Key.UnitPrice,
                                        g.Key.PPOffer,
                                        g.Key.IntTotalAmt,
                                        Quantity = g.Sum(i => i.Quantity),
                                        UTAmount = g.Key.UnitPrice * g.Sum(i => i.Quantity),
                                        Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                        Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                        Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                        Service = g.Select(i => i.Service).FirstOrDefault(),
                                        Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                        IMEIs = g.Select(i => i.IMEI).ToList()
                                    };

                #endregion

                string IMEIs = string.Empty;
                int Count = 0;
                foreach (var item in GroupProducts)
                {
                    oCSPRow = CSProductDT.NewRow();
                    oCSPRow["SLNo"] = nCOunt.ToString();
                    oCSPRow["PName"] = item.ProductName + ", " + item.CategoryName;
                    oCSPRow["ColorName"] = item.ColorName;
                    oCSPRow["Qty"] = item.Quantity.ToString();
                    oCSPRow["UnitPrice"] = (item.UnitPrice).ToString();
                    oCSPRow["PPOffer"] = item.IntTotalAmt.ToString();
                    oCSPRow["TotalAmt"] = (item.UTAmount + item.IntTotalAmt).ToString();
                    foreach (var IMEI in item.IMEIs)
                    {
                        Count++;
                        if (item.IMEIs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }
                    oCSPRow["IMENO"] = IMEIs;
                    Count = 0;
                    CSProductDT.Rows.Add(oCSPRow);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;

                    dtWarrenty.Rows.Add(item.ProductName, IMEIs, Warrenty);
                    Warrenty = string.Empty;
                    IMEIs = string.Empty;

                }

                CSProductDT.TableName = "CSalesProduct";
                _dataSet.Tables.Add(CSProductDT);

                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.TSalesAmt));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("IssueDate", oOrder.SalesDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ProductName", "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerName", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CContactNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", "Remarks: " + oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                string salesprice = (oOrder.TSalesAmt - oOrder.TotalOffer).ToString("F");
                _reportParameter = new ReportParameter("SalesPrice", salesprice);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("DownPayment", oOrder.DownPayment.ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ProcessingFee", oOrder.ProcessingFee.ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemainingAmt", (oOrder.TSalesAmt - oOrder.TotalOffer - oOrder.DownPayment).ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerCode", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RefName", customer.RefName);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("RefCont", customer.RefContact);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("RefAdd", customer.RefAddress);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NidNo", customer.NID);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("FatherName", customer.FName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GuarName", oOrder.GuarName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GuarContactNo", oOrder.GuarContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GuarAddress", oOrder.GuarAddress);
                _reportParameters.Add(_reportParameter);


                var sum = count;
                sum = sum - 1;

                _reportParameter = new ReportParameter("ScheduleNo", "" + sum);
                _reportParameters.Add(_reportParameter);


              
                     SystemInformation currentSystemInfoH = _systemInformationService.GetSystemInformationByConcernId(concernID);
                     var checkTramsAndCodition = currentSystemInfoH.TramsAndCondition;

              

                if (currentSystemInfoH.Name == "Maa Electronics (Head)" || currentSystemInfoH.Name == "Maa Electronics(Nagar Bandar Branch)" ||
                    currentSystemInfoH.Name == "Maa Electronics (Mokamtola Branch)" || currentSystemInfoH.Name == "Maa Electronics (Sonatola Branch)" ||
                    currentSystemInfoH.Name == "Maa Electronics (Dupchachia Branch)_____OLD" || currentSystemInfoH.Name == "Maa Electronics (Gobindaganj Branch)" ||
                    currentSystemInfoH.Name == "AC World (Rangpur Branch)" || currentSystemInfoH.Name == " Maa Electronics (Birampur Branch)" ||
                    currentSystemInfoH.Name == "Maa Electronics (Amtoly Branch)" || currentSystemInfoH.Name == "Maa Electronics (Pirganj Branch)" ||
                    currentSystemInfoH.Name == "AC World (Bogura Branch)" || currentSystemInfoH.Name == "AC World (Dupchachia Branch)" ||
                    currentSystemInfoH.Name == "Mother Electronics" || currentSystemInfoH.Name == "Maa Electronics (Thana Branch)")
                {
                    _reportParameter = new ReportParameter("VatRegNo", currentSystemInfoH.VatRegNo);
                    _reportParameters.Add(_reportParameter);

                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoiceME.rdlc");

                }


             


                if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
                {
                    _reportParameter = new ReportParameter("Msg", " বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                    _reportParameters.Add(_reportParameter);
                }

                else if (concernID == (int)EnumSisterConcern.Ityadi_Electronic || concernID == (int)EnumSisterConcern.SHOPNO_PURON)
                {
                    _reportParameter = new ReportParameter("Msg", "এই পন্যটির বিক্রয়লব্ধ অর্থের একটি অংশ গরিব অসহায় শিশুদের সু - চিকিৎসার জন্য নিবেদিত।");
                    _reportParameters.Add(_reportParameter);
                }
                //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
                //{
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoiceSS.rdlc");
                //}
                //else
                //{
                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
                if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
                {
                    _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoice_beauty.rdlc");
                }
                if (concernID == (int)EnumSisterConcern.MSSohelEnterPrise)
                {
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoiceNewFormate.rdlc");
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoice.rdlc");
                // }

            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public byte[] CreditSalesInvoiceReport(CreditSale oOrder, string userName, int concernID)
        {
            return CreditSalesInvoiceReportPrint(oOrder, userName, concernID);
        }

        public byte[] CreditSalesInvoiceReportByID(int oOrderID, string userName, int concernID)
        {
            CreditSale oOrder = new CreditSale();
            oOrder = _creditSalesOrderService.GetSalesOrderById(oOrderID);
            oOrder.CreditSalesSchedules = _creditSalesOrderService.GetSalesOrderSchedules(oOrderID).ToList();
            oOrder.CreditSaleDetails = _creditSalesOrderService.GetSalesOrderDetails(oOrderID).ToList();

            return CreditSalesInvoiceReportPrint(oOrder, userName, concernID);

        }

        public byte[] SalesInvoiceHistoryReport(int oOrderID, string userName, int concernID)
        {
            try
            {
                TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                var salseDetailInfos = _salesOrderService.GetSalesDetailHistoryBySOrderID(oOrderID);
                _dataSet = new DataSet();
                DataRow srow = null;


                foreach (var item in salseDetailInfos)
                {
                    srow = dt.NewRow();

                    srow["SalesDate"] = item.InvoiceDate.ToString("dd MMM yyyy");
                    srow["InvoiceNo"] = item.InvoiceNo;
                    srow["ProductName"] = item.ProductName;
                    srow["CName"] = item.CustomerName;
                    srow["SalesPrice"] = item.UnitPrice;
                    srow["NetAmt"] = item.UTAmount;
                    srow["GrandTotal"] = item.Grandtotal;
                    srow["TotalDis"] = item.NetDiscount;
                    srow["NetTotal"] = item.TotalAmount;
                    srow["PaidAmount"] = item.RecAmount;
                    srow["RemainingAmt"] = item.PaymentDue;
                    srow["Quantity"] = item.Quantity;
                    srow["IMENo"] = item.IMENO;
                    srow["ColorInfo"] = item.ColorName;
                    srow["SalesType"] = "Cash Sales";
                    srow["AdjAmount"] = item.AdjAmount;
                    srow["UserName"] = item.UserName;
                    srow["ActionDate"] = item.ActionDate;
                    if (item.ActionStatus == 1)
                    {
                        srow["ActionStatus"] = "Create";
                    }
                    else if (item.ActionStatus == 2)
                    {
                        srow["ActionStatus"] = "Edit 1st";
                    }
                    else if (item.ActionStatus == 3)
                    {
                        srow["ActionStatus"] = "Edit 2nd";
                    }
                    else if (item.ActionStatus == 4)
                    {
                        srow["ActionStatus"] = "Edit 3rd";
                    }
                    else if (item.ActionStatus == 5)
                    {
                        srow["ActionStatus"] = "Edit 4th";
                    }
                    else if (item.ActionStatus == 6)
                    {
                        srow["ActionStatus"] = "Edit 5th";
                    }
                    else if (item.ActionStatus == 7)
                    {
                        srow["ActionStatus"] = "Edit 6th";

                    }
                    else if (item.ActionStatus == 8)
                    {
                        srow["ActionStatus"] = "Edit 7th";
                    }
                    else if (item.ActionStatus == 8)
                    {
                        srow["ActionStatus"] = "Edit 7th";
                    }
                    else if (item.ActionStatus == 9)
                    {
                        srow["ActionStatus"] = "Edit 8th";
                    }
                    else if (item.ActionStatus == 10)
                    {
                        srow["ActionStatus"] = "Edit 9th";
                    }
                    else if (item.ActionStatus == 11)
                    {
                        srow["ActionStatus"] = "Edit 10th";
                    }
                    else
                    {
                        srow["ActionStatus"] = item.ActionStatus;
                    }

                    dt.Rows.Add(srow);
                }

                dt.TableName = "dtCustomerWiseSales";
                _dataSet.Tables.Add(dt);


                GetCommonParameters(userName, concernID);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesOrderHistory.rdlc");
            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

        public byte[] PurchaseInvoiceHistoryReport(int oOrderID, string userName, int concernID)
        {
            try
            {

                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();
                var purchaseInfos = _purchaseOrderService.GetPurchaseDetailReportByPOrderID(oOrderID);
                _dataSet = new DataSet();
                DataRow srow = null;

                foreach (var item in purchaseInfos)
                {
                    srow = dt.NewRow();

                    srow["PurchaseDate"] = item.Date.ToString("dd MMM yyyy");
                    srow["ChallanNo"] = item.ChallanNo;
                    srow["ProductName"] = item.ProductName;
                    srow["PurchaseRate"] = item.UnitPrice;
                    srow["NetAmt"] = item.TAmount;
                    srow["GrandTotal"] = item.GrandTotal;
                    srow["TotalDis"] = item.NetDiscount;
                    srow["NetTotal"] = item.TotalAmount;
                    srow["PaidAmt"] = item.RecAmt;
                    srow["RemainingAmt"] = item.PaymentDue;
                    srow["Quantity"] = item.Quantity;
                    srow["Color"] = item.IMENO;
                    srow["AdjAmount"] = item.AdjustAmt;
                    srow["UserName"] = item.UserName;
                    srow["ActionDate"] = item.ActionDate;
                    if (item.ActionStatus == 1)
                    {
                        srow["ActionStatus"] = "Create";
                    }
                    else if (item.ActionStatus == 2)
                    {
                        srow["ActionStatus"] = "Edit 1st";
                    }
                    else if (item.ActionStatus == 3)
                    {
                        srow["ActionStatus"] = "Edit 2nd";
                    }
                    else if (item.ActionStatus == 4)
                    {
                        srow["ActionStatus"] = "Edit 3rd";
                    }
                    else if (item.ActionStatus == 5)
                    {
                        srow["ActionStatus"] = "Edit 4th";
                    }
                    else if (item.ActionStatus == 6)
                    {
                        srow["ActionStatus"] = "Edit 5th";
                    }
                    else if (item.ActionStatus == 7)
                    {
                        srow["ActionStatus"] = "Edit 6th";

                    }
                    else if (item.ActionStatus == 8)
                    {
                        srow["ActionStatus"] = "Edit 7th";
                    }
                    else if (item.ActionStatus == 8)
                    {
                        srow["ActionStatus"] = "Edit 7th";
                    }
                    else if (item.ActionStatus == 9)
                    {
                        srow["ActionStatus"] = "Edit 8th";
                    }
                    else if (item.ActionStatus == 10)
                    {
                        srow["ActionStatus"] = "Edit 9th";
                    }
                    else if (item.ActionStatus == 11)
                    {
                        srow["ActionStatus"] = "Edit 10th";
                    }
                    else
                    {
                        srow["ActionStatus"] = item.ActionStatus;
                    }

                    dt.Rows.Add(srow);

                }

                dt.TableName = "dtSuppWiseData";
                //_dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseOrderHistory.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] CustomeWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CustomerID)
        {
            try
            {

                var salesInfos = _salesOrderService.GetSalesDetailReportByCustomerID(fromDate, toDate, CustomerID);
                //DataRow row = null;

                decimal TotalDueSales = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;

                decimal RecAmt = 0;
                decimal CurrDue = 0;
                decimal CusTotalDue = 0;
                int customerID = 0, SOrderID = 0;
                TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                TransactionalDataSet.dtCustomerDataTable cdt = new TransactionalDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();
                DataRow srow = null;
                DataRow crow = null;

                foreach (var item in salesInfos)
                {
                    srow = dt.NewRow();
                    if (customerID != item.CustomerID)
                    {
                        CusTotalDue = item.CustomerTotalDue;
                        crow = cdt.NewRow();
                        crow["CCode"] = item.CustomerCode;
                        crow["CName"] = item.CustomerName;
                        crow["CompanyName"] = item.CustCompanyName;
                        crow["CusType"] = item.CustomerType;
                        crow["ContactNo"] = item.CustomerContactNo;
                        crow["NID"] = item.CustomerNID;
                        crow["Address"] = item.CustomerAddress;
                        crow["TotalDue"] = item.CustomerTotalDue;
                        cdt.Rows.Add(crow);
                    }

                    customerID = item.CustomerID;

                    if (SOrderID != item.SOrderID)
                    {
                        TotalDueSales = TotalDueSales + (decimal)item.PaymentDue;
                        GrandTotal = GrandTotal + (decimal)item.Grandtotal;
                        TotalDis = TotalDis + (decimal)item.NetDiscount;
                        NetTotal = NetTotal + (decimal)item.TotalAmount;
                        RecAmt = RecAmt + (decimal)item.RecAmount;
                        CurrDue = CurrDue + (decimal)item.PaymentDue;
                    }

                    SOrderID = item.SOrderID;

                    srow["SalesDate"] = item.InvoiceDate.ToString("dd MMM yyyy");
                    srow["InvoiceNo"] = item.InvoiceNo;
                    srow["ProductName"] = item.ProductName;
                    srow["CName"] = item.CustomerName;
                    srow["SalesPrice"] = item.UnitPrice;
                    srow["NetAmt"] = item.UTAmount;
                    srow["GrandTotal"] = item.Grandtotal;
                    srow["TotalDis"] = item.NetDiscount;
                    srow["NetTotal"] = item.TotalAmount;
                    srow["PaidAmount"] = item.RecAmount;
                    srow["RemainingAmt"] = item.PaymentDue;
                    srow["Quantity"] = item.Quantity;
                    srow["IMENo"] = item.IMENO;
                    srow["ColorInfo"] = item.ColorName;
                    srow["SalesType"] = "Cash Sales";
                    srow["AdjAmount"] = item.AdjAmount;
                    dt.Rows.Add(srow);
                }

                cdt.TableName = "dtCustomer";
                _dataSet.Tables.Add(cdt);

                dt.TableName = "dtCustomerWiseSales";
                //_dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "Sales report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("PrintedBy", Global.CurrentUser.UserName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", CusTotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDueUpTo", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", (GrandTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", (NetTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("FreeAmt", "0.00");
                _reportParameters.Add(_reportParameter);

                //GetCommonParameters(userName, concernID);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerWiseSales.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SuplierWisePurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int SupplierID)
        {
            try
            {
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>> purchaseInfos = _purchaseOrderService.GetPurchaseDetailReportBySupplierID(fromDate, toDate, concernID, SupplierID);
                //DataRow row = null;

                decimal TotalDueSales = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                decimal CusTotalDue = 0;
                string ChallanNo = "";

                string SuppCode = "";
                decimal QTY = 0;


                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();
                TransactionalDataSet.dtSupplierDataTable cdt = new TransactionalDataSet.dtSupplierDataTable();

                _dataSet = new DataSet();

                foreach (var grd in purchaseInfos)
                {
                    if (grd.Rest.Item6 == "No Barcode")
                    {
                        QTY = (decimal)grd.Rest.Item5;
                    }
                    else
                    {
                        QTY = 1;
                    }

                    if (SuppCode != grd.Rest.Item7.Item1)
                    {
                        CusTotalDue = grd.Item6;
                        cdt.Rows.Add(grd.Rest.Item7.Item1, grd.Rest.Item7.Item2, grd.Rest.Item7.Item5, grd.Rest.Item7.Item4, grd.Rest.Item7.Item3, grd.Item6);
                        //cdt.TableName = "dtSupplier";
                        //_dataSet.Tables.Add(cdt);
                    }

                    SuppCode = grd.Rest.Item7.Item1;

                    if (ChallanNo != grd.Item2)
                    {
                        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                        GrandTotal = GrandTotal + (decimal)grd.Item7;
                        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                    }



                    ChallanNo = grd.Item2;
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item4 * QTY, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, QTY, grd.Rest.Item6, "", grd.Rest.Item6, grd.Rest.Item7.Item7);

                }

                _dataSet.Tables.Add(cdt);
                cdt.TableName = "dtSupplier";

                dt.TableName = "dtSuppWiseData";
                //_dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "Purchase report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("PrintedBy", Global.CurrentUser.UserName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", CusTotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDueUpTo", "Total Due Upto Date: " + "0.00");
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", (GrandTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", (NetTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("FreeAmt", "0.00");
                //_reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptSupplierWiseDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int RptType)
        {
            try
            {
                IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>> MOWiseSalesInfos = _salesOrderService.GetSalesDetailReportByMOID(fromDate, toDate, concernID, MOID, RptType);

                TransactionalDataSet.MOSDetailsDataTable dt = new TransactionalDataSet.MOSDetailsDataTable();

                _dataSet = new DataSet();
                foreach (var grd in MOWiseSalesInfos)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2.ToString("dd MMM yyyy"), grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7.Item1, grd.Item7.Item2, grd.Item7.Item3, grd.Item7.Item4);

                }

                dt.TableName = "MOSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "SR wise sales summary report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseSalesDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseCustomerDueRpt(string userName, int concernID, int MOID, int RptType)
        {
            try
            {
                IEnumerable<Tuple<string, string, string, string, string, decimal>> MOWiseCustomerDue = _salesOrderService.GetMOWiseCustomerDueRpt(concernID, MOID, RptType);

                TransactionalDataSet.MOWiseDueRptDataTable dt = new TransactionalDataSet.MOWiseDueRptDataTable();

                _dataSet = new DataSet();
                foreach (var grd in MOWiseCustomerDue)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6);
                }

                dt.TableName = "MOWiseDueRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseDueRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] StockDetailReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int PCategoryID, bool IsVATManager, int StockType, int MaaManager, int filetype)
        {
            try
            {

                var stockInfos = _StockServce.GetforStockReport(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, 0, PCategoryID, IsVATManager, StockType).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                string IMENO = "";
                int count;
                //StockDetails = _stockdetailService.GetAll();
                foreach (var item in stockInfos)
                {
                    row = dtStockInfoDT.NewRow();
                    row["UnitType"] = "Pice";
                    //row["StockCode"] = item.Item1;
                    row["ProName"] = item.Item2;
                    row["CompanyName"] = item.Item3;
                    row["CategoryName"] = item.Item4;
                    row["ModelName"] = item.Item5;
                    row["Quantity"] = item.Item6;
                    if (MaaManager == 1)
                    {
                        row["PRate"] = item.Rest.Item1;
                        row["TotalPrice"] = (item.Item6 * item.Rest.Item1);
                    }
                    else
                    {
                        row["PRate"] = item.Item7;
                        row["TotalPrice"] = (item.Item6 * item.Item7);
                    }
                    row["Godown"] = item.Rest.Item5;
                    row["PCategoryName"] = item.Rest.Item7;

                    //var SDetails = _StockServce.GetStockDetailsByID(item.Item1);
                    //var SDetails = StockDetails.Where(i=>i.StockID==item.Item1).ToList();

                    IMENO = "";
                    count = 0;

                    foreach (var imei in item.Rest.Item6)
                    {
                        if (count == 0)
                            IMENO = IMENO + imei;
                        else
                            IMENO = IMENO + System.Environment.NewLine + imei;
                        count++;
                    }

                    row["StockCode"] = IMENO;

                    dtStockInfoDT.Rows.Add(row);
                }

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStock.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStock.rdlc";
                }

                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStock.rdlc";
                }


                else if (reportType == 4)
                {
                    reportName = "Stock\\rptPCategoryWiseStock.rdlc";
                }

                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, reportName, filetype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StockSummaryReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType, int filetype)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforStockReport(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ColorID, PCategoryID, IsVATManager, StockType).ToList();
                var InhouseDamageProductDetails = _productService.GetAllDamageProductFromDetail();
                //var CompanyDamageProductDetails = _purchaseOrderService.GetDamageReturnProductDetails(0, 0);
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();

                #region Product wise
                if (reportType == 0)//Product Wise
                {

                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item1, p.Item2, p.Item3, p.Item4, p.Item6, p.Item7, p.Rest.Item5 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           ProductCode = g.Key.Item2,
                                           ProductName = g.Key.Item3,

                                           CategoryName = g.Key.Item6,
                                           CompanyName = g.Key.Item7,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /* CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                            group cds by new { cds.ProductID, cds.ColorID, cds.ProductCode, cds.ProductName, cds.CompanyName, cds.CategoryName } into g
                                            select new ProductWisePurchaseModel
                                            {
                                                ProductCode = g.Key.ProductCode,
                                                ProductName = g.Key.ProductName,
                                                CategoryName = g.Key.CategoryName,

                                                CompanyName = g.Key.CompanyName,
                                                Quantity = g.Count(),
                                                TotalAmount = g.Sum(i => i.MRP),
                                            }).ToList();*/

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProName = ns.Item2, ModelName = ns.Item5, CompanyName = ns.Item3, CategoryName = ns.Item4, PRate = ns.Item7 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProName,
                                            ColorName = g.Key.ModelName,
                                            CompanyName = g.Key.CompanyName,
                                            CategoryName = g.Key.CategoryName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            PurchaseRate = g.Key.PRate,
                                        }).ToList();

                    bool IsHistoryShow = false;
                    if ((concernID != 1 || concernID != 5 || concernID != 6) && reportType == 0)
                        IsHistoryShow = true;
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;//Total Sales Price
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.TotalAmount / item.Quantity;
                        row["CreditSRate"] = item.TotalCreditSR6 / item.Quantity; //6 months
                        row["TotalCreditPrice"] = item.PurchaseRate * item.Quantity;//item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.TotalCreditSR3 / item.Quantity;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = item.TotalCreditSR12 / item.Quantity;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["Godown"] = "";

                        if (IsHistoryShow)
                        {
                            row["StockHistory"] = "";// _StockServce.GetStockProductsHistory(item.Item1);
                        }
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Company wise
                else if (reportType == 1) //Company wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6, p.Item7, p.Rest.Item5 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           CompanyName = g.Key.Item7,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /*CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                           group cds by new { cds.CompanyName, cds.CategoryName } into g
                                           select new ProductWisePurchaseModel
                                           {
                                               CategoryName = g.Key.CategoryName,
                                               CompanyName = g.Key.CompanyName,
                                               Quantity = g.Count(),
                                               TotalAmount = g.Sum(i => i.MRP),
                                           }).ToList();*/

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Company = ns.Item3, Category = ns.Item4 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            CompanyName = g.Key.Company,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Category wise
                else if (reportType == 2) //category wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /*CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                           group cds by new { cds.CategoryName } into g
                                           select new ProductWisePurchaseModel
                                           {
                                               CategoryName = g.Key.CategoryName,
                                               Quantity = g.Count(),
                                               TotalAmount = g.Sum(i => i.MRP),
                                           }).ToList();*/
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Category = ns.Item4 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Godown Wise
                else if (reportType == 3) //Godown wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /*CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                           group cds by new { cds.GodownName } into g
                                           select new ProductWisePurchaseModel
                                           {
                                               GodownName = g.Key.GodownName,
                                               Quantity = g.Count(),
                                               TotalAmount = g.Sum(i => i.MRP),
                                           }).ToList();*/
                    //var Normalstocks = (from ns in stockInfos
                    //                    group ns by new { GodownName = ns.Rest.Item5 } into g
                    //                    select new ProductWisePurchaseModel
                    //                    {
                    //                        GodownName = g.Key.GodownName,
                    //                        Quantity = g.Sum(i => i.Item6),
                    //                        TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                    //                        TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                    //                        TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                    //                        TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                    //                    }).ToList();


                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProName = ns.Item2, ModelName = ns.Item5, CompanyName = ns.Item3, CategoryName = ns.Item4, PRate = ns.Item7, GodownName = ns.Rest.Item5 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProName,
                                            ColorName = g.Key.ModelName,
                                            CompanyName = g.Key.CompanyName,
                                            CategoryName = g.Key.CategoryName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            PurchaseRate = g.Key.PRate,
                                            GodownName = g.Key.GodownName,
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;//Total Sales Price
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.TotalAmount / item.Quantity;
                        row["CreditSRate"] = item.TotalCreditSR6 / item.Quantity; //6 months
                        row["TotalCreditPrice"] = item.PurchaseRate * item.Quantity;//item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.TotalCreditSR3 / item.Quantity;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = item.TotalCreditSR12 / item.Quantity;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["Godown"] = item.GodownName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Color wise
                else if (reportType == 4) //Color wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /*CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                           group cds by new { cds.CategoryName } into g
                                           select new ProductWisePurchaseModel
                                           {
                                               CategoryName = g.Key.CategoryName,
                                               Quantity = g.Count(),
                                               TotalAmount = g.Sum(i => i.MRP),
                                           }).ToList();*/
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProductName = ns.Item2, ColorName = ns.Item5, } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProductName,
                                            ColorName = g.Key.ColorName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = 0;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["Godown"] = item.GodownName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion


                #region PCategory wise
                else if (reportType == 5) // parent category wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /*CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                           group cds by new { cds.CategoryName } into g
                                           select new ProductWisePurchaseModel
                                           {
                                               CategoryName = g.Key.CategoryName,
                                               Quantity = g.Count(),
                                               TotalAmount = g.Sum(i => i.MRP),
                                           }).ToList();*/
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Category = ns.Item4, ns.Rest.Item7 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            PCategoryName = g.Key.Item7,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["PCategoryName"] = item.PCategoryName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockSummaryInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStockSummary.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStockSummary.rdlc";
                }
                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStockSummary.rdlc";
                }
                else if (reportType == 4)
                {
                    reportName = "Stock\\rptColorWiseStockSummary.rdlc";
                }

                else if (reportType == 5)
                {
                    reportName = "Stock\\rptPCategoryWiseStockSummary.rdlc";
                }



                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, reportName, filetype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] SRInvoiceReport(int oOrderID, string userName, int concernID)
        {
            try
            {

                SRVisit oOrder = _SRVisitService.GetSRVisitById(Convert.ToInt32(oOrderID));
                var soDetails = _SRVisitDetailService.GetSRVisitDetailById(Convert.ToInt32(oOrderID));

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtSRVisitDataTable dt = new TransactionalDataSet.dtSRVisitDataTable();
                Employee employee = _EmployeeService.GetEmployeeById(oOrder.EmployeeID);
                Product product = null;
                Category oCategory = null;
                Company oCompany = null;
                string IMENO = "";

                int count = 0;

                foreach (var item in soDetails)
                {
                    product = _productService.GetProductById(item.Item2);

                    if (product != null)
                    {
                        oCategory = _CategoryService.GetCategoryById(product.CategoryID);
                        oCompany = _CompanyService.GetCompanyById(product.CompanyID);
                    }

                    IMENO = "";
                    count = 0;
                    IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    foreach (SRVProductDetail itemime in SRVPD)
                    {
                        if (count == 0)
                            IMENO = IMENO + itemime.IMENO;
                        else
                            IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                        count++;
                    }

                    dt.Rows.Add(item.Item5, oCategory.Description, oCompany.Name, SRVPD.Count(), IMENO);
                }
                orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtSRVisit";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.ChallanNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.VisitDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", employee.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", employee.ContactNo);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisit.rdlc");
            }

            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public byte[] SRInvoiceReportByChallanNo(string ChallanNo, string userName, int concernID)
        {
            try
            {
                SRVisit oOrder = _SRVisitService.GetSRVisitByChallanNo(ChallanNo, concernID);
                var soDetails = _SRVisitDetailService.GetSRVisitDetailById(Convert.ToInt32(oOrder.SRVisitID));

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtSRVisitDataTable dt = new TransactionalDataSet.dtSRVisitDataTable();
                Employee employee = _EmployeeService.GetEmployeeById(oOrder.EmployeeID);
                Product product = null;
                Category oCategory = null;
                Company oCompany = null;

                string IMENO = "";
                int count = 0;

                foreach (var item in soDetails)
                {
                    product = _productService.GetProductById(item.Item2);

                    if (product != null)
                    {
                        oCategory = _CategoryService.GetCategoryById(product.CategoryID);
                        oCompany = _CompanyService.GetCompanyById(product.CompanyID);
                    }

                    IMENO = "";
                    count = 0;
                    IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    foreach (SRVProductDetail itemime in SRVPD)
                    {
                        if (count == 0)
                            IMENO = IMENO + itemime.IMENO;
                        else
                            IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                        count++;
                    }


                    ////product = _productService.GetProductById(item.);
                    //IMENO = "";
                    //var SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    //foreach (var itemime in SRVPD)
                    //{
                    //    IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                    //}

                    dt.Rows.Add(item.Item5, oCategory.Description, oCompany.Name, item.Item4, IMENO);
                }
                orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtSRVisit";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.ChallanNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.VisitDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", employee.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", employee.ContactNo);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisit.rdlc");
            }

            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public string GetLocalTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);
            return localDateTime.ToString("dd MMM yyyy hh:mm:ss tt");
        }

        private SystemInformation GetCommonParameters(string userName, int concernID)
        {
            string logoPath = string.Empty;
            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            _reportParameters = new List<ReportParameter>();

            _reportParameter = new ReportParameter("Logo", logoPath);
            _reportParameters.Add(_reportParameter);
            string CompanyName = string.Empty;
            if (currentSystemInfo.Name.Contains("\r"))
            {
                var Sysinfo = currentSystemInfo.Name.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                CompanyName = Sysinfo[0] + Environment.NewLine + Sysinfo[1];
            }
            else
                CompanyName = currentSystemInfo.Name;

            #region Logo
            if (currentSystemInfo.CompanyLogo != null)
            {
                TransactionalDataSet.dtLogoDataTable dtLogo = new TransactionalDataSet.dtLogoDataTable();
                dtLogo.Rows.Add(currentSystemInfo.CompanyLogo, currentSystemInfo.BrandLogo);
                _dataSet.Tables.Add(dtLogo);
            }
            #endregion
            _reportParameter = new ReportParameter("CompanyName", CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Phone", currentSystemInfo.TelephoneNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("Address", currentSystemInfo.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintedBy", userName);
            _reportParameters.Add(_reportParameter);
            return currentSystemInfo;
        }

        public static string GetUserFriendlyDescription(int SalaryItemCode, string Description)
        {
            switch (SalaryItemCode)
            {
                case (int)EnumSalaryItemCode.Tot_Attend_Days_Amount:
                    return "Attendenc Salary";
                case (int)EnumSalaryItemCode.Over_Time_Amount:
                    return "Over Time";
                case (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus:
                    return "Attendence Bonus";
                case (int)EnumSalaryItemCode.Bonus:
                    return "Bonus";
                case (int)EnumSalaryItemCode.Advance_Deduction:
                    return "Advance";
                case (int)EnumSalaryItemCode.Target_Failed_Deduct:
                    return "Target Failed Deduction";
                case (int)EnumSalaryItemCode.Gross_Salary:
                    return "Gross Salary";
                case (int)EnumSalaryItemCode.Extra_Commission:
                    return "Extra Commission";
                case (int)EnumSalaryItemCode.Voltage_StabilizerComm:
                    return "Voltage Stabilizer Commission";
                case (int)EnumSalaryItemCode.Due_Salary:
                    return "Due Salary";
                case (int)EnumSalaryItemCode.Due_SalaryPay:
                    return "Due Salary Pay";
                default:
                    return Description;
            }
        }
        public byte[] DefaultingCustomerReport(DateTime date, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                IEnumerable<Tuple<string, string, string, decimal, decimal>> defaultingCustomers = _creditSalesOrderService.GetDefaultingCustomer(date, concernID);

                TransactionalDataSet.dtDefaultingCustomerDataTable dt = new TransactionalDataSet.dtDefaultingCustomerDataTable();


                foreach (var item in defaultingCustomers)
                {

                    dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);

                }
                dt.TableName = "dtDefaultingCustomer";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                List<ReportParameter> parameters = new List<ReportParameter>();
                _reportParameter = new ReportParameter("Date", " till " + date.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptDefaultingCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 05-Jun-2018
        /// New Method
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public byte[] DefaultingCustomerReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>> upComing = _creditSalesOrderService.GetDefaultingCustomer(fromDate, toDate, concernID);

                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                foreach (var item in upComing)
                {
                    dt.Rows.Add(item.Item2, item.Item3, item.Item4, "", item.Item5, item.Item6, item.Item7, item.Rest.Item1 + item.Rest.Item7, item.Rest.Rest.Item2, item.Rest.Item3, 0m, item.Rest.Item5, item.Rest.Item6, item.Rest.Item7, item.Rest.Item4, item.Rest.Rest.Item2 - item.Rest.Item6);
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("DateRange", "Default Customer Report Sales Date From: " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptDefaultingCustomerNew.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] InstallmentCollectionReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, int EmployeeId, int selectedConcernID, bool IsAdminReport)
        {
            try
            {
                _dataSet = new DataSet();
                var InstallmentCollections = _creditSalesOrderService.GetScheduleCollection(fromDate, toDate, selectedConcernID, EmployeeId, IsAdminReport);
                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                TransactionalDataSet.dtUpcomingScheduleDataTable dtZeros = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                DataRow row = null;
                foreach (var item in InstallmentCollections.Where(o => o.PaymentStatus == "Paid" && o.PaymentDate >= fromDate && o.PaymentDate <= toDate))
                {
                    row = dt.NewRow();
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.CustomerName + " & " + item.CustomerConctact + " & " + item.CustomerAddress;
                    row["ProductName"] = item.ProductName;
                    row["SalesDate"] = item.SalesDate;
                    row["PaymentDate"] = item.PaymentDate;
                    row["SalesPrice"] = item.NoOfInstallment;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.NetAmount;
                    row["RemainingAmt"] = item.Remaining;
                    row["Installment"] = item.InstallmentAmount;
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InterestAmount"] = item.PenaltyInterest;
                    row["DefaultAmount"] = item.NoOfRemainingInstallment;
                    row["TotalInstCollectionAmt"] = 0m;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    row["RemaindDate"] = item.ScheduleDate.ToString("dd MMM yyyy");
                    row["ExpectedInstallment"] = item.ExpectedInstallment;
                    row["Employee"] = item.EmployeeName;
                    row["ConcernName"] = item.ConcernName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                foreach (var item in InstallmentCollections.Where(o => o.PaymentStatus == "Due" && o.ScheduleDate >= fromDate && o.ScheduleDate <= toDate))
                {

                    row = dtZeros.NewRow();
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.CustomerName + " & " + item.CustomerConctact + " & " + item.CustomerAddress;
                    row["ProductName"] = item.ProductName;
                    row["SalesDate"] = item.SalesDate;
                    // row["PaymentDate"] = item.PaymentDate;
                    row["SalesPrice"] = item.NoOfInstallment;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.NetAmount;
                    row["RemainingAmt"] = item.Remaining + 1;
                    row["Installment"] = 0;// 
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InterestAmount"] = item.PenaltyInterest;
                    row["DefaultAmount"] = item.NoOfRemainingInstallment;
                    row["TotalInstCollectionAmt"] = 0m;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    row["RemaindDate"] = item.ScheduleDate.ToString("dd MMM yyyy");
                    row["ExpectedInstallment"] = item.InstallmentAmount;// item.ExpectedInstallment;
                    row["Employee"] = item.EmployeeName;
                    row["ConcernName"] = item.ConcernName;
                    dtZeros.Rows.Add(row);
                }

                dtZeros.TableName = "dtUpcomingScheduleZeros";
                _dataSet.Tables.Add(dtZeros);



                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PaymentDate", "Installment Collections Date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PaymentDate2", "Not yeat collected Installments Date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                if (IsAdminReport)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptAdminInstallmentCollections.rdlc");
                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptInstallmentCollections.rdlc");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        //{
        //    try
        //    {
        //        _dataSet = new DataSet();
        //        var upComing = _creditSalesOrderService.GetUpcomingSchedule(fromDate, toDate);

        //        TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
        //        decimal DefaultAmount = 0;
        //        foreach (var item in upComing)
        //        {

        //            //dt.Rows.Add(oCSDItem.InvoiceNo, oCSDItem.Name, oCSDItem.ContactNo, "", oCSDItem.SalesDate, oCSDItem.MonthDate, oCSDItem.TSalesAmt, oCSDItem.NetAmount, oCSDItem.FixedAmt, oCSDItem.Remaining, oCSDItem.InstallmentAmt, oCSDItem.Remarks, oCSDItem.DownPayment);
        //            DefaultAmount = _creditSalesOrderService.GetDefaultAmount(item.Rest.Rest.Item1, fromDate);
        //            dt.Rows.Add(item.Item2, item.Item3, item.Item4, "", item.Item5, item.Item6, item.Item7, item.Rest.Item1 + item.Rest.Item7, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4 + DefaultAmount, item.Rest.Item5, item.Rest.Item6, item.Rest.Item7, DefaultAmount);
        //        }

        //        dt.TableName = "dtUpcomingSchedule";
        //        _dataSet.Tables.Add(dt);

        //        GetCommonParameters(userName, concernID);
        //        _reportParameter = new ReportParameter("PaymentDate", fromDate.ToString("dd MMM yyyy"));
        //        _reportParameters.Add(_reportParameter);
        //        _reportParameter = new ReportParameter("ToDate", toDate.ToString("dd MMM yyyy"));
        //        _reportParameters.Add(_reportParameter);

        //        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\UpComingSchedule.rdlc");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string userName, int concernID
            , EnumCustomerType customerType, int EmployeeID)
        {
            try
            {
                _dataSet = new DataSet();
                var upComing = _creditSalesOrderService.GetUpcomingSchedule(fromDate, toDate, customerType, EmployeeID);
                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                DataRow row = null;
                string ProductName = string.Empty;
                foreach (var item in upComing)
                {
                    row = dt.NewRow();
                    //DefaultAmount = _creditSalesOrderService.GetDefaultAmount(item.CreditSalesID, fromDate);
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.CustomerCode + " & " + item.CustomerName + " & " + item.CustomerAddress + " & " + item.CustomerConctact;
                    for (int i = 0; i < item.ProductName.Count; i++)
                    {
                        ProductName = ProductName + item.ProductName[i];
                        if (item.ProductName.Count != i + 1)
                            ProductName = ProductName + System.Environment.NewLine;
                    }
                    row["ProductName"] = ProductName;
                    row["SalesDate"] = item.SalesDate;
                    row["PaymentDate"] = item.PaymentDate;
                    row["SalesPrice"] = item.SalesPrice;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.FixedAmt;
                    row["RemainingAmt"] = item.Remaining;
                    row["Installment"] = item.TotalPaymentDue;
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InterestAmount"] = item.PenaltyInterest;
                    row["DefaultAmount"] = item.DefaultAmount;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    row["TotalInstCollectionAmt"] = item.TotalInstCollectionAmt;
                    row["CustomerCode"] = item.CustomerCode;
                    row["TotalPaidAmt"] = item.NetAmount - item.Remaining;
                    row["RefNameContact"] = item.CustomerRefName /*+ " & " + item.CustomerRefContact*/;
                    row["RemaindDate"] = item.RemaindDate == null ? "" : item.RemaindDate.Value.ToString("dd MMM yyyy");
                    row["NoOfInstallment"] = item.NoOfInstallment;
                    row["Employee"] = item.EmployeeName;

                    dt.Rows.Add(row);
                    ProductName = string.Empty;
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PaymentDate", fromDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("ToDate", toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\UpComingSchedule.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] CashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int customerId, EnumCustomerType customerType)
        {
            try
            {
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();
                if (customerType != EnumCustomerType.Hire)

                {
                    var CashCollectionInfos = _CashCollectionService.GetCashCollectionData(fromDate, toDate, concernID, customerId, customerType);

                    var BankCollectionInfos = _bankTransactionService.GetBankTransactionData(fromDate, toDate, concernID, customerId, 0, 0, EnumTransactionType.CashCollection, false, customerType)
                                                .Where(i => !string.IsNullOrEmpty(i.Item3));
                    var SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, 0, customerType)
                        .Where(i => i.RecAmount > 0);
                    if (customerId != 0)
                        SalesOrders = SalesOrders.Where(i => i.CustomerID == customerId);

                    foreach (var item in SalesOrders)
                    {
                        dt.Rows.Add(item.InvoiceDate.ToString("dd MMM yyyy"), item.CustomerName + "\n" + item.CustomerAddress + " ," + item.CustomerContactNo, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cash Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo, "", item.CustomerType);
                    }
                    foreach (var grd in CashCollectionInfos)
                    {
                        dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), (grd.Item2 + "\n" + grd.Item4 + " & " + grd.Item3), grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, "", "", grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, "", grd.Rest.Item7);
                    }

                    foreach (var grd in BankCollectionInfos)
                    {
                        dt.Rows.Add(
                            grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                            grd.Item3 + "\n" + grd.Rest.Item4 + ", " + grd.Rest.Item5,  //CName
                            grd.Rest.Item4, //Address
                            grd.Rest.Item5, //ContactNo
                            0,     //TotalDue
                            grd.Rest.Item1,   //RecAmount

                            0,
                            0,
                            "Cheque",
                            grd.Item2,
                            grd.Rest.Rest.Item2,
                            grd.Rest.Rest.Item1,
                            grd.Rest.Rest.Item3,
                            grd.Rest.Rest.Item2,
                             grd.Item6,
                             "",
                             grd.Rest.Rest.Item5

                             );

                    }

                }
                if (customerType == EnumCustomerType.Hire)
                {
                    var CreditSales = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, (int)customerType)
                        .Where(i => i.Rest.Item1 > 0);
                    if (customerId != 0)
                        CreditSales = CreditSales.Where(i => i.Rest.Item6 == customerId);

                    foreach (var grd in CreditSales)
                    {
                        dt.Rows.Add(grd.Item3.ToString("dd MMM yyyy"), grd.Item2 + "\n" + grd.Rest.Rest.Item1 + ", " + grd.Rest.Rest.Item2, "Address", "Contact", grd.Rest.Item5, grd.Rest.Item1, 0m, 0m, "Credit Sales", "", "", "", "", "Emp", grd.Item4, "", EnumCustomerType.Hire.ToString());
                    }

                    var CreditCollections = _creditSalesOrderService.GetCreditCollectionReport(fromDate, toDate, concernID, customerId);
                    foreach (var grd in CreditCollections)
                    {
                        dt.Rows.Add(grd.Item6.ToString("dd MMM yyyy"), grd.Item3 + "\n" + grd.Item4, "", grd.Item4, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item3, 0m, "CreditCollection", "N/A", "N/A", "N/A", "N/A", "EMP Name", "", "", EnumCustomerType.Hire.ToString());
                    }
                }

                if (customerType == 0)
                {
                    var CashCollectionInfos = _CashCollectionService.GetCashCollectionDataFoAll(fromDate, toDate, concernID, customerId, customerType);

                    var BankCollectionInfos = _bankTransactionService.GetBankTransactionDataForAll(fromDate, toDate, concernID, customerId, 0, 0, EnumTransactionType.CashCollection, false, customerType)
                                                .Where(i => !string.IsNullOrEmpty(i.Item3));
                    var SalesOrders = _salesOrderService.GetforSalesReportForAll(fromDate, toDate, 0, customerType)
                        .Where(i => i.RecAmount > 0);
                    if (customerId != 0)
                        SalesOrders = SalesOrders.Where(i => i.CustomerID == customerId);

                    foreach (var item in SalesOrders)
                    {
                        dt.Rows.Add(item.InvoiceDate.ToString("dd MMM yyyy"), item.CustomerName + "\n" + item.CustomerAddress + " ," + item.CustomerContactNo, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cash Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo, "", item.CustomerType);
                    }
                    foreach (var grd in CashCollectionInfos)
                    {
                        dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), (grd.Item2 + "\n" + grd.Item4 + " & " + grd.Item3), grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, "", "", grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, "", grd.Rest.Item7);
                    }

                    foreach (var grd in BankCollectionInfos)
                    {
                        dt.Rows.Add(
                            grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                            grd.Item3 + "\n" + grd.Rest.Item4 + ", " + grd.Rest.Item5,  //CName
                            grd.Rest.Item4, //Address
                            grd.Rest.Item5, //ContactNo
                            0,     //TotalDue
                            grd.Rest.Item1,   //RecAmount

                            0,
                            0,
                            "Cheque",
                            grd.Item2,
                            grd.Rest.Rest.Item2,
                            grd.Rest.Rest.Item1,
                            grd.Rest.Rest.Item3,
                            grd.Rest.Rest.Item2,
                             grd.Item6,
                             "",
                             grd.Rest.Rest.Item5

                             );

                    }
                    var CreditSales = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, (int)customerType)
                        .Where(i => i.Rest.Item1 > 0);
                    if (customerId != 0)
                        CreditSales = CreditSales.Where(i => i.Rest.Item6 == customerId);

                    foreach (var grd in CreditSales)
                    {
                        dt.Rows.Add(grd.Item3.ToString("dd MMM yyyy"), grd.Item2 + "\n" + grd.Rest.Rest.Item1 + ", " + grd.Rest.Rest.Item2, "Address", "Contact", grd.Rest.Item5, grd.Rest.Item1, 0m, 0m, "Credit Sales", "", "", "", "", "Emp", grd.Item4, "", EnumCustomerType.Hire.ToString());
                    }

                    var CreditCollections = _creditSalesOrderService.GetCreditCollectionReport(fromDate, toDate, concernID, customerId);
                    foreach (var grd in CreditCollections)
                    {
                        dt.Rows.Add(grd.Item6.ToString("dd MMM yyyy"), grd.Item3 + "\n" + grd.Item4, "", grd.Item4, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item3, 0m, "CreditCollection", "N/A", "N/A", "N/A", "N/A", "EMP Name", "", "", EnumCustomerType.Hire.ToString());
                    }
                }


                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "Cash Collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] CashDeliverReport(DateTime fromDate, DateTime toDate, string userName,
            int concernID, int supplierId, bool IsAdmin, int selectedConcernID)
        {
            try
            {
                var CashCollectionInfos = _CashCollectionService.GetCashDeliveryData(fromDate, toDate, selectedConcernID, supplierId, IsAdmin);
                var BankCollectionInfos = _bankTransactionService.GetBankTransactionDataForAll(fromDate, toDate, selectedConcernID, 0, supplierId, 0, EnumTransactionType.CashDelivery, IsAdmin);
                IQueryable<Supplier> suppliers = null;
                IQueryable<POrder> pOrders = null;
                List<ProductWisePurchaseModel> purchases = new List<ProductWisePurchaseModel>();
                if (IsAdmin)
                {
                    purchases = _purchaseOrderService.AdminPurchaseReport(fromDate, toDate, selectedConcernID)
                        .Where(i => i.RecAmt != 0).ToList();
                }
                else
                {
                    if (supplierId > 0)
                        suppliers = _SupplierService.GetAllSupplier().Where(i => i.SupplierID == supplierId);
                    else
                        suppliers = _SupplierService.GetAllSupplier();


                    purchases = (from po in _purchaseOrderService.GetAllIQueryable()
                                 join s in suppliers on po.SupplierID equals s.SupplierID
                                 where po.OrderDate >= fromDate && po.OrderDate <= toDate && po.RecAmt != 0
                                 && po.Status == (int)EnumPurchaseType.Purchase
                                 select new ProductWisePurchaseModel
                                 {
                                     Date = po.OrderDate,
                                     ChallanNo = po.ChallanNo,
                                     RecAmt = po.RecAmt,
                                     SupplierName = s.Name,
                                     Address = s.Address,
                                     Mobile = s.ContactNo,
                                     PaymentDue = s.TotalDue,
                                     AdjustAmt = po.AdjAmount,
                                 }).ToList();
                }

                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();

                DataRow row = null;
                foreach (var grd in CashCollectionInfos)
                {
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, "", "", grd.Rest.Item7);
                }

                foreach (var grd in BankCollectionInfos)
                {
                    dt.Rows.Add(
                        grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                        grd.Item4,  //SupplierName
                        grd.Rest.Item6, //Address
                        grd.Rest.Item7, //ContactNo
                        0,     //TotalDue
                        grd.Rest.Item1,   //RecAmount

                        0,
                        0,
                        "Cheque",
                        grd.Item2,
                        grd.Rest.Rest.Item2,
                        grd.Rest.Rest.Item1,
                        grd.Rest.Rest.Item3,
                        grd.Rest.Rest.Item2,
                        grd.Item6,
                        grd.Rest.Rest.Item6
                        );
                }

                foreach (var item in purchases)
                {
                    row = dt.NewRow();
                    row["CollDate"] = item.Date;
                    row["CName"] = item.SupplierName;
                    row["CAddress"] = item.Mobile + " & " + item.Address;
                    row["CContact"] = item.Mobile;
                    row["TotalDue"] = item.PaymentDue;
                    row["RecAmt"] = item.RecAmt;
                    row["RemainigAmt"] = 0m;
                    row["AdjustmentAmt"] = item.AdjustAmt;
                    row["CashType"] = "Purchase";
                    row["BankName"] = "";
                    row["AccountNo"] = "";
                    row["BranchName"] = "";
                    row["ChequeNo"] = "";
                    row["EmployeeName"] = "";
                    row["InvoiceNo"] = item.ChallanNo;
                    row["ConcernName"] = item.ConcenName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Month", "Cash Delivery report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                if (IsAdmin)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRptNew.rdlc");
                //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRpt.rdlc");
                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseSDetailReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID)
        {
            try
            {
                var MOWiseSalesInfos = _salesOrderService.GetforSalesDetailReportByMO(fromDate, toDate, MOID);

                TransactionalDataSet.dtMOWSDetailsDataTable dt = new TransactionalDataSet.dtMOWSDetailsDataTable();
                _dataSet = new DataSet();
                string EmpName = "";
                int SORderID = 0;
                decimal TotalAmount = 0, GrandTotal = 0, totalDueAmount = 0, totalCashSales = 0, NetDiscount = 0;
                foreach (var grd in MOWiseSalesInfos)
                {
                    EmpName = grd.EmployeeName;

                    if (SORderID != grd.SOrderID)
                    {
                        TotalAmount = TotalAmount + grd.TotalAmount;
                        totalCashSales = totalCashSales + grd.RecAmount;
                        totalDueAmount = totalDueAmount + grd.PaymentDue;
                        GrandTotal = GrandTotal + grd.Grandtotal;
                        NetDiscount = NetDiscount + grd.NetDiscount;
                    }
                    dt.Rows.Add(grd.EmployeeName, grd.InvoiceDate, grd.InvoiceNo, grd.ProductName, grd.CustomerName, grd.UnitPrice, grd.RecAmount, grd.PaymentDue, grd.TotalAmount, grd.Grandtotal, grd.NetDiscount, grd.TotalAmount, grd.RecAmount, grd.PaymentDue, grd.Quantity, grd.IMENO, grd.CustomerCode, grd.AdjAmount);
                    SORderID = grd.SOrderID;
                }

                dt.TableName = "dtMOWSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "" + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("EmpName", "Sales Representative:[" + EmpName + "]");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", TotalAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("totalCashSales", totalCashSales.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("totalDueAmount", totalDueAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetDiscount", NetDiscount.ToString());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMOWiseSDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductWisePriceProtection(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> PWPriceProtection = _StockServce.GetPriceProtectionReport(userName, concernID, fromDate, toDate);

                TransactionalDataSet.dtPProtectionDataTable dt = new TransactionalDataSet.dtPProtectionDataTable();
                _dataSet = new DataSet();
                //string SuppName = "";

                foreach (var grd in PWPriceProtection)
                {
                    //SuppName = grd.Item6;
                    if (grd.Item5 > 0)
                        dt.Rows.Add(grd.Item1, grd.Item7, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6);
                }

                dt.TableName = "dtPProtection";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Price Protection Data From " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptPProtection.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] ProductWisePandSReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int productID)
        {
            try
            {
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> purchase = _purchaseOrderService.GetPurchaseByProductID(fromDate, toDate, concernID, productID);
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> sales = _salesOrderService.GetSalesByProductID(fromDate, toDate, concernID, productID);

                TransactionalDataSet.PWPDetailsDataTable pwp = new TransactionalDataSet.PWPDetailsDataTable();
                TransactionalDataSet.PWSDetailsDataTable pws = new TransactionalDataSet.PWSDetailsDataTable();
                _dataSet = new DataSet();
                decimal TotalPurchase = 0;
                decimal TotalSales = 0;
                decimal StockIn = 0;
                foreach (var grd in purchase)
                {
                    TotalPurchase = TotalPurchase + grd.Item4;
                    pwp.Rows.Add(grd.Item1, grd.Item2, "", "", grd.Item3, grd.Item4, grd.Item5, 0);
                }
                foreach (var grd in sales)
                {
                    TotalSales = TotalSales + grd.Item4;
                    pws.Rows.Add(grd.Item1, grd.Item2, "", "", grd.Item3, grd.Item4, grd.Item5, 0);
                }

                StockIn = TotalPurchase - TotalSales;
                pwp.TableName = "PWPDetails";
                _dataSet.Tables.Add(pws);
                pws.TableName = "PWSDetails";
                _dataSet.Tables.Add(pwp);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("OutStandingStock", Math.Round(StockIn, 0).ToString());
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPandS.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRVisitStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID)
        {
            try
            {
                //IEnumerable<SRVisitStatusReportModel> MOWiseSalesInfos= _SRVisitService.SRVisitStatusReport(fromDate, toDate, concernID, MOID);

                IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<string>>> MOWiseSalesInfos = _SRVisitService.GetSRViistDetailReportByEmployeeID(fromDate, toDate, concernID, MOID);
                TransactionalDataSet.dtSRVisitStatusDataTable dt = new TransactionalDataSet.dtSRVisitStatusDataTable();
                _dataSet = new DataSet();
                string EmpName = "";
                string IMENO = "";
                string VChallanNo = "";
                string ContactNo = "";
                string InvoiceNo = "";
                int count = 0;
                var gdata = from d in MOWiseSalesInfos
                            group d by d.Item2;
                foreach (var grd in MOWiseSalesInfos)
                {
                    EmpName = grd.Item6;
                    ContactNo = grd.Rest.Item1;
                    InvoiceNo = grd.Item2;

                    //count = 0;

                    if (VChallanNo == grd.Item2 || VChallanNo == "")
                    {
                        if (count == 0)
                            IMENO = IMENO + grd.Item5;
                        else
                            IMENO = IMENO + System.Environment.NewLine + grd.Item5;
                        count++;
                    }

                    //IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);
                    //foreach (SRVProductDetail itemime in SRVPD)
                    //{
                    //    if (count == 0)
                    //        IMENO = IMENO + itemime.IMENO;
                    //    else
                    //        IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                    //    count++;
                    //}


                    if (VChallanNo != grd.Item2)
                    {

                        dt.Rows.Add(grd.Item1, grd.Item3, "", "", grd.Item4, IMENO, "0", "0");
                        IMENO = "";
                        count = 0;
                    }
                    VChallanNo = grd.Item2;

                }

                dt.TableName = "dtSRVisitStatus";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceDate", "" + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", "Sales Representative:[" + EmpName + "]");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", ContactNo);
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisitStatus.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRWiseCustomerSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {

            var SRwiseCustomerSalesList = _salesOrderService.SRWiseCustomerSalesSummary(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRWiseCustSalesSummaryDataTable dt = new TransactionalDataSet.dtSRWiseCustSalesSummaryDataTable();
            _dataSet = new DataSet();
            foreach (var item in SRwiseCustomerSalesList)
            {
                dt.Rows.Add(item.EmployeeID, item.SRName, item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.Address, item.ContactNo, item.BarUnitPrice, item.SmartUnitPrice, item.BarQuantity, item.SmartQuantity, item.TotalPriceBar, item.TotalPriceSmart, item.BarAndSmartQty, item.BarAndSmartPrice);
            }

            dt.TableName = "dtSRWiseCustSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRWiseCustSalesSummary.rdlc");

        }
        private string GetRemarksByTransID(string TransID)
        {
            string Remarks = string.Empty;
            var ID = TransID.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (ID.Length == 2)
            {
                int Transid = Convert.ToInt32(ID[1]);
                switch (ID[0])
                {
                    case "SO":
                        var SORder = _salesOrderService.GetAllIQueryable().FirstOrDefault(i => i.SOrderID == Transid);
                        Remarks = SORder != null ? SORder.Remarks : "";
                        break;
                    case "CRO":
                        var CRS = _creditSalesOrderService.GetAllIQueryable().FirstOrDefault(i => i.CreditSalesID == Transid);
                        Remarks = CRS != null ? CRS.Remarks : "";
                        break;
                    case "CC":
                        var CC = _CashCollectionService.GetCashCollectionById(Transid);
                        Remarks = CC != null ? CC.Remarks : "";
                        break;
                    default:
                        break;
                }
            }
            return Remarks;
        }

        private List<CustomerLedgerModel> NonTransCustomers(DateTime fromDate, DateTime toDate)
        {
            var SOrders = _salesOrderService.GetAllIQueryable().Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate);
            var CashCollections = _CashCollectionService.GetAllIQueryable().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate);
            var CreditSales = _creditSalesOrderService.GetAllIQueryable().Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate);
            var Customers = _customerService.GetAllCustomer();
            var NonTransCustomers = Customers.Where(i => !SOrders.Select(j => j.CustomerID).Contains(i.CustomerID) && !CashCollections.Select(j => j.CustomerID).Contains(i.CustomerID) && !CreditSales.Select(j => j.CustomerID).Contains(i.CustomerID));
            return (from c in NonTransCustomers
                    where c.TotalDue != 0
                    select new CustomerLedgerModel
                    {
                        ConcernID = c.ConcernID,
                        CustomerID = c.CustomerID,
                        Code = c.Code,
                        CustomerName = c.Name,
                        InvoiceDate = toDate,
                        InvoiceNo = c.Code,
                        Opening = c.TotalDue,
                        TotalDue = c.TotalDue,
                        Closing = c.TotalDue
                    }).ToList();


        }
        public byte[] CustomerLedgerSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtCustomerLedgerDataTable dt = new TransactionalDataSet.dtCustomerLedgerDataTable();
            _dataSet = new DataSet();
            string Remarks = string.Empty;
            List<CustomerLedgerModel> Ledgers = new List<CustomerLedgerModel>();
            Ledgers = (from cl in customerLedgerdata
                       group cl by new { cl.ConcernID, cl.CustomerID, cl.Code, cl.CustomerName, cl.SOrderID, cl.InvoiceDate, cl.InvoiceNo, cl.Opening, cl.TotalSalesAmt, cl.CollectionAmt, cl.CashSales, cl.DueSales, cl.TotalDue, cl.AdjustAmt, cl.ProductReturnAmt, cl.PenaltyInterestAmt, cl.Closing } into g
                       select new CustomerLedgerModel
                       {
                           ConcernID = g.Key.ConcernID,
                           CustomerID = g.Key.CustomerID,
                           Code = g.Key.Code,
                           CustomerName = g.Key.CustomerName,
                           SOrderID = g.Key.SOrderID,
                           InvoiceDate = g.Key.InvoiceDate,
                           InvoiceNo = g.Key.InvoiceNo,
                           Opening = g.Key.Opening,
                           TotalSalesAmt = g.Key.TotalSalesAmt,
                           CollectionAmt = g.Key.CollectionAmt,
                           CashSales = g.Key.CashSales,
                           DueSales = g.Key.DueSales,
                           TotalDue = g.Key.TotalDue,
                           AdjustAmt = g.Key.AdjustAmt,
                           PenaltyInterestAmt = g.Key.PenaltyInterestAmt,
                           ProductReturnAmt = g.Key.ProductReturnAmt,
                           Closing = g.Key.Closing
                       }).ToList();
            decimal AllCustomerTotalDue = 0m, Closing = 0m;

            if (CustomerID > 0)
            {

                if (Ledgers.Count() == 0)
                {
                    Ledgers = (from c in _customerService.GetAll().Where(i => i.CustomerID == CustomerID)
                               select new CustomerLedgerModel
                               {
                                   Code = c.Code,
                                   CustomerName = c.Name,
                                   Opening = c.TotalDue,
                                   TotalDue = c.TotalDue,
                                   Closing = c.TotalDue,
                                   InvoiceNo = c.Code,
                                   InvoiceDate = toDate
                               }).ToList();
                }

                var totaldue = (from l in Ledgers
                                group l by new { l.InvoiceDate, l.CustomerID, l.Closing } into g
                                select new
                                {
                                    g.Key.InvoiceDate,
                                    g.Key.CustomerID,
                                    g.Key.Closing
                                }).OrderBy(i => i.CustomerID).ThenByDescending(i => i.InvoiceDate).ToList();

                var due = (from d in totaldue
                           group d by new { d.CustomerID } into g
                           select new
                           {
                               g.Key.CustomerID,
                               InvoiceDate = g.Select(i => i.InvoiceDate).FirstOrDefault(),
                               Closing = g.Select(i => i.Closing).FirstOrDefault()
                           }).ToList();
                AllCustomerTotalDue = due.Sum(i => i.Closing);
            }

            foreach (var item in Ledgers)
            {
                Remarks = item.SOrderID != null ? GetRemarksByTransID(item.SOrderID) : "";
                dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, 0, "", 0, 0, item.ProductReturnAmt, item.PenaltyInterestAmt, Remarks);
            }

            if (CustomerID == 0)
            {
                var NonTrans = NonTransCustomers(fromDate, toDate);
                AllCustomerTotalDue = _customerService.GetAll().Where(i => i.TotalDue != 0).Sum(i => i.TotalDue);
                foreach (var item in NonTrans)
                {
                    dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, 0, "", 0, 0, item.ProductReturnAmt, item.PenaltyInterestAmt, "No Transaction Exists");
                }
            }

            dt.TableName = "dtCustomerLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", " Summary Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            AllCustomerTotalDue = AllCustomerTotalDue == 0m ? Closing : AllCustomerTotalDue;
            _reportParameter = new ReportParameter("AllCustomerTotalDue", AllCustomerTotalDue.ToString("F"));
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedgerSummary.rdlc");

        }


        public byte[] CustomerLedgerDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtCustomerLedgerDataTable dt = new TransactionalDataSet.dtCustomerLedgerDataTable();
            _dataSet = new DataSet();

            foreach (var item in customerLedgerdata)
            {
                dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, item.ProductID, item.ProductName, item.Quantity, item.ProSalesAmt, item.ProductReturnAmt, item.PenaltyInterestAmt, "");
            }

            dt.TableName = "dtCustomerLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedger.rdlc");

        }


        public byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID, int IsOnlyDue)
        {
            var customerdue = _salesOrderService.CustomerDue(fromDate, toDate, concernID, CustomerID, IsOnlyDue).ToList();
            decimal closingGrandtotal = 0;

            var tempZeroClosing = (from cd in customerdue
                                   group cd by cd.CustomerID into g
                                   select new
                                   {
                                       CustomerID = g.Key,
                                       Name = g.FirstOrDefault().Name,
                                       Closing = g.LastOrDefault().Balance,
                                   }).Where(i => i.Closing == 0).ToList();

            customerdue.RemoveAll(i => tempZeroClosing.Any(j => j.CustomerID == i.CustomerID));

            List<Tuple<int, decimal>> GrandTotalCal = new List<Tuple<int, decimal>>();

            TransactionalDataSet.dtCustomerDueDataTable dt = new TransactionalDataSet.dtCustomerDueDataTable();
            _dataSet = new DataSet();
            int CID = 0;
            DateTime CDate = DateTime.MinValue;
            bool IsSalesFound = false;
            foreach (var item in customerdue)
            {
                if (CID != item.CustomerID)
                {
                    IsSalesFound = false;
                    if (item.Status.Equals("aSales") || item.Status.Equals("bCreditSales") || item.Status.Equals("RSales"))
                    {
                        if (item.TransDate >= fromDate && item.TransDate <= toDate)
                        {
                            IsSalesFound = true;
                            CDate = item.TransDate;
                            GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                            dt.Rows.Add(item.TransDate, item.CustomerID, item.ConcernID, item.TransDate, item.Code, item.Name, item.Address, item.ContactNo, item.InvoiceNo, item.SalesAmount, item.DueSales, item.InterestAmt, item.TotalSalesAmt, item.RecAmount, item.CollectionAmt, item.Status, item.Balance, item.AdjustAmt, item.ReturnAmt, item.InstallmentPeriod);
                        }
                    }
                    else
                    {
                        //no operation for Cash collection and credit collection status
                    }

                }
                else
                {
                    if (item.Status.Equals("aSales") || item.Status.Equals("bCreditSales") || item.Status.Equals("RSales"))
                    {
                        if (item.TransDate >= fromDate && item.TransDate <= toDate)
                        {
                            if (CDate == DateTime.MinValue)
                                CDate = item.TransDate;
                            IsSalesFound = true;
                            GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                            dt.Rows.Add(item.TransDate, item.CustomerID, item.ConcernID, CDate, item.Code, item.Name, item.Address, item.ContactNo, item.InvoiceNo, item.SalesAmount, item.DueSales, item.InterestAmt, item.TotalSalesAmt, item.RecAmount, item.CollectionAmt, item.Status, item.Balance, item.AdjustAmt, item.ReturnAmt, item.InstallmentPeriod);
                        }
                    }
                    else
                    {
                        if (IsSalesFound == true)
                        {
                            if (item.TransDate >= fromDate)
                            {
                                GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                                dt.Rows.Add(item.TransDate, item.CustomerID, item.ConcernID, CDate, item.Code, item.Name, item.Address, item.ContactNo, item.InvoiceNo, item.SalesAmount, item.DueSales, item.InterestAmt, item.TotalSalesAmt, item.RecAmount, item.CollectionAmt, item.Status, item.Balance, item.AdjustAmt, item.ReturnAmt, item.InstallmentPeriod);
                            }
                        }
                    }
                }

                CID = item.CustomerID;
            }

            dt.TableName = "dtCustomerDue";
            _dataSet.Tables.Add(dt);

            var cl = (from cd in GrandTotalCal
                      group cd by cd.Item1 into g
                      select new
                      {
                          Id = g.Key,
                          Closing = g.LastOrDefault().Item2,
                      }).ToList();

            closingGrandtotal = cl.Sum(i => i.Closing);


            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingGrandTotal", closingGrandtotal.ToString());
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerDueMobile.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerDue.rdlc");
        }


        public byte[] DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID)
        {
            var customerLedgerdata = _StockServce.DailyStockVSSalesSummary(fromDate, toDate, concernID, ProductID);
            TransactionalDataSet.dtDailyStockandSalesSummaryDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryDataTable();
            _dataSet = new DataSet();

            int ProductID_old = 0;

            double TotalOpeningStockQty = 0;
            double TotalOpeningStockValue = 0;
            double TotalClosingQty = 0;
            double TotalClosingValue = 0;
            double TotalClosingQtyTemp = 0;
            double TotalClosingValueTemp = 0;

            foreach (var item in customerLedgerdata)
            {
                if (ProductID_old != item.ProductID)
                {
                    TotalOpeningStockQty = TotalOpeningStockQty + (double)item.OpeningStockQuantity;
                    TotalOpeningStockValue = TotalOpeningStockValue + (double)item.OpeningStockValue;

                    TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
                    TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;

                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;

                }
                else
                {
                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;
                }
                dt.Rows.Add(item.Date, item.ConcernID, item.ProductID, item.Code, item.ProductName, item.ColorID, item.ColorName, item.OpeningStockQuantity, item.TotalStockQuantity, item.PurchaseQuantity, item.SalesQuantity, item.ClosingStockQuantity, item.OpeningStockValue, item.TotalStockValue, item.ClosingStockValue, item.ReturnQuantity, item.SalesQuantity - item.ReturnQuantity);



                ProductID_old = item.ProductID;
            }


            TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
            TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;





            dt.TableName = "dtDailyStockandSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("TotalOpeningStockQty", TotalOpeningStockQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalOpeningStockValue", Convert.ToDecimal(TotalOpeningStockValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingQty", TotalClosingQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingValue", Convert.ToDecimal(TotalClosingValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptDailyStockandSalesSummary.rdlc");
        }

        public byte[] DailyCashBookLedger(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.CashInHandReport(fromDate, toDate, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();

            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {


                    dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt);
                    //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
                }

            }





            dt.TableName = "dtDailyCashbookLedgerNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptDailyCashStatementNew.rdlc");

        }




        public byte[] ProfitAndLossReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.ProfitAndLossReport(fromDate, toDate, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();

            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {


                    dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt);
                    //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
                }

            }





            dt.TableName = "dtProfitAndLoss";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptProfitAndLossRpt.rdlc");

        }




        public byte[] SummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {



            var data = _CashCollectionService.CashInHandReport(fromDate, toDate, concernID);



            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {



                }

            }









            var Summarydata = _CashCollectionService.SummaryReport(fromDate, toDate, OpeningCashInHand, CurrentCashInHand, ClosingCashInHand, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtSummaryReportNewDataTable dt = new TransactionalDataSet.dtSummaryReportNewDataTable();
            //   TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();




            foreach (var item in Summarydata)
            {



                dt.Rows.Add(item.id, item.Category, item.Head, item.Amount, item.Total);
                //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);


            }





            dt.TableName = "dtSummaryReportNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ToDate", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);
            //_reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);
            //_reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptSummaryReport.rdlc");

        }




        public byte[] BankSummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID)
        {




            var customerLedgerdata = _StockServce.DailyStockVSSalesSummary(fromDate, toDate, concernID, ProductID);
            TransactionalDataSet.dtDailyStockandSalesSummaryDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryDataTable();
            _dataSet = new DataSet();

            int ProductID_old = 0;

            double TotalOpeningStockQty = 0;
            double TotalOpeningStockValue = 0;
            double TotalClosingQty = 0;
            double TotalClosingValue = 0;
            double TotalClosingQtyTemp = 0;
            double TotalClosingValueTemp = 0;

            foreach (var item in customerLedgerdata)
            {
                if (ProductID_old != item.ProductID)
                {
                    TotalOpeningStockQty = TotalOpeningStockQty + (double)item.OpeningStockQuantity;
                    TotalOpeningStockValue = TotalOpeningStockValue + (double)item.OpeningStockValue;

                    TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
                    TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;

                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;

                }
                else
                {
                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;
                }
                dt.Rows.Add(item.Date, item.ConcernID, item.ProductID, item.Code, item.ProductName, item.ColorID, item.ColorName, item.OpeningStockQuantity, item.TotalStockQuantity, item.PurchaseQuantity, item.SalesQuantity, item.ClosingStockQuantity, item.OpeningStockValue, item.TotalStockValue, item.ClosingStockValue, item.ReturnQuantity, item.SalesQuantity - item.ReturnQuantity);



                ProductID_old = item.ProductID;
            }


            TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
            TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;





            dt.TableName = "dtDailyStockandSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("TotalOpeningStockQty", TotalOpeningStockQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalOpeningStockValue", Convert.ToDecimal(TotalOpeningStockValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingQty", TotalClosingQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingValue", Convert.ToDecimal(TotalClosingValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptDailyStockandSalesSummary.rdlc");
        }

        //public byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID)
        //{

        //    var bankLedgerdata = _bankTransactionService.BankLedger(fromDate, toDate, concernID, BankID);
        //    TransactionalDataSet.dtBankLedgerDataTable dttemp = new TransactionalDataSet.dtBankLedgerDataTable();
        //    TransactionalDataSet.dtBankLedgerDataTable dt = new TransactionalDataSet.dtBankLedgerDataTable();

        //    _dataSet = new DataSet();
        //    double Opening = 0;
        //    double Closing = 0;
        //    double GrandClosing = 0;
        //    double GrandOpening = 0;
        //    int BankIDTemp = 0;

        //    foreach (var item in bankLedgerdata)
        //    {
        //        if (BankIDTemp != item.BankID)
        //        {
        //            Opening = (double)item.Opening;
        //            Closing = Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;

        //        }
        //        else
        //        {

        //            Opening = Closing;
        //            Closing = Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;

        //        }

        //        if (item.TransDate >= fromDate && item.TransDate <= toDate)
        //            dt.Rows.Add(item.ConcernID, item.BankID, item.BankName, item.TransDate, item.TransactionNo, Opening, item.Deposit, item.Widthdraw, item.CashCollection, item.CashDelivery, item.FundIN, item.FundOut, Closing);

        //        BankIDTemp = item.BankID;




        //    }

        //    BankIDTemp = 0;
        //    Opening = 0;
        //    foreach (var item in bankLedgerdata)
        //    {
        //        if (BankIDTemp != item.BankID)
        //        {
        //            Opening = (double)item.Opening;
        //            GrandOpening = GrandOpening + (double)item.Opening;
        //        }
        //        else
        //        {
        //            Opening = 0;
        //        }

        //        GrandClosing = GrandClosing + Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;



        //        BankIDTemp = item.BankID;

        //    }

        //    dt.TableName = "dtBankLedger";
        //    _dataSet.Tables.Add(dt);

        //    GetCommonParameters(userName, concernID);

        //    _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
        //    _reportParameters.Add(_reportParameter);

        //    _reportParameter = new ReportParameter("GrandClosing", GrandClosing.ToString());
        //    _reportParameters.Add(_reportParameter);
        //    _reportParameter = new ReportParameter("GrandOpening", GrandOpening.ToString());
        //    _reportParameters.Add(_reportParameter);


        //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankLedger.rdlc");
        //}

        public byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID, int commonConcernId)
        {

            var bankLedgerdata = _bankTransactionService.BankLedger(fromDate, toDate, BankID, commonConcernId);
            TransactionalDataSet.dtBankLedgerDataTable dt = new TransactionalDataSet.dtBankLedgerDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in bankLedgerdata)
            {
                row = dt.NewRow();
                row["ConcernID"] = item.ConcernID;
                row["BankID"] = item.BankID;
                row["BankName"] = item.BankName;
                row["TransDate"] = item.TransDate;
                row["TransactionNo"] = item.TransactionNo;
                row["Opening"] = item.Opening;
                row["Deposit"] = item.Deposit;
                row["Withdraw"] = item.Withdraw;
                row["CashCollection"] = item.CashCollection;
                row["CashDelivery"] = item.CashDelivery;
                row["FundIN"] = item.FundIN;
                row["FundOut"] = item.FundOut;
                row["Closing"] = item.Closing;
                row["AccountNo"] = item.AccountNO;
                row["AccountName"] = item.AccountName;
                row["FromToAccNo"] = item.FromToAccountNo;
                row["ConcernName"] = item.ConcernName;
                row["BankIncome"] = item.BankIncome;
                row["BankExpense"] = item.BankExpense;
                row["LiaPay"] = item.LiaPay;
                row["LiaRec"] = item.LiaRec;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtBankLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            var bank = bankLedgerdata.FirstOrDefault();
            string header = bank != null ? ("Bank:" + bank.BankName + ", A/C Name: " + bank.AccountName + ", A/C No.: " + bank.AccountNO) : "";
            _reportParameter = new ReportParameter("DateRange", "Bank Ledger of " + header + " Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("GrandClosing", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("GrandOpening", "");
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankLedger.rdlc");
        }
        public byte[] ReplacementInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID)
        {
            TransactionalDataSet.dtReplaceInvoiceDataTable dt = new TransactionalDataSet.dtReplaceInvoiceDataTable();
            _dataSet = new DataSet();
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerId);
            decimal dtotalamount = 0, rtotlaamount = 0;
            int TotalQty = 0;
            foreach (var item in ROrderDetails)
            {
                dt.Rows.Add(item.DamageProductName, item.ProductName, item.DamageIMEINO, item.ReplaceIMEINO, item.DamageUnitPrice, item.UnitPrice, item.Quantity, item.Quantity, item.Remarks);
                dtotalamount += Convert.ToDecimal(item.DamageUnitPrice);
                TotalQty += Convert.ToInt32(item.Quantity);
                rtotlaamount += Convert.ToDecimal(item.UnitPrice);
            }

            dt.TableName = "dtReplaceInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount);
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecieveAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.OrderDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("DamageTotalAmount", dtotalamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReplaceTotalAmount", rtotlaamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalQty", TotalQty.ToString());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderInvoice.rdlc");
        }


        public byte[] ReplaceInvoiceReportByID(int orderId, string username, int concernID)
        {
            TransactionalDataSet.dtReplaceInvoiceDataTable dt = new TransactionalDataSet.dtReplaceInvoiceDataTable();
            var ROrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(orderId));
            var rorderdetaisl = _salesOrderService.GetReplaceOrderInvoiceReportByID(orderId);
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerID);
            decimal dtotalamount = 0, rtotlaamount = 0;
            int TotalQty = 0;
            _dataSet = new DataSet();
            foreach (var item in rorderdetaisl)
            {
                dt.Rows.Add(item.DamageProductName, item.ProductName, item.DamageIMEINO, item.ReplaceIMEINO, item.DamageUnitPrice, item.UnitPrice, item.Quantity, item.Quantity, item.Remarks);
                dtotalamount += Convert.ToDecimal(item.DamageUnitPrice);
                TotalQty += Convert.ToInt32(item.Quantity);
                rtotlaamount += Convert.ToDecimal(item.UnitPrice);
            }

            dt.TableName = "dtReplaceInvoice";
            _dataSet.Tables.Add(dt);

            #region Parameter
            GetCommonParameters(username, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.InvoiceDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("DamageTotalAmount", dtotalamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReplaceTotalAmount", rtotlaamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalQty", TotalQty.ToString());
            _reportParameters.Add(_reportParameter);
            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderInvoice.rdlc");
        }


        public byte[] ReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID)
        {
            TransactionalDataSet.dtReturnInvoiceDataTable dt = new TransactionalDataSet.dtReturnInvoiceDataTable();
            _dataSet = new DataSet();
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerId);

            foreach (var item in ROrderDetails)
            {
                dt.Rows.Add(item.DamageProductName, item.DamageIMEINO, item.UnitPrice, item.Quantity, item.UnitPrice * item.Quantity);
            }

            dt.TableName = "dtReturnInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount);
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecieveAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.OrderDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", "Remarks: " + ROrder.Remarks);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReturnInvoice.rdlc");
        }

        public byte[] ReturnInvoiceReportByID(int orderId, string username, int concernID)
        {
            TransactionalDataSet.dtReturnInvoiceDataTable dt = new TransactionalDataSet.dtReturnInvoiceDataTable();
            var ROrder = _returnOrderService.GetReturnOrderById(Convert.ToInt32(orderId));
            var rorderdetaisl = _returnOrderService.GetReturnDetailReportByReturnID(orderId, concernID);

            Customer customer = _customerService.GetCustomerById(ROrder.CustomerID);
            _dataSet = new DataSet();
            foreach (var item in rorderdetaisl)
            {
                dt.Rows.Add(item.Item3, item.Rest.Item6, item.Item5, item.Rest.Item5, item.Item5 * item.Rest.Item5);
            }

            dt.TableName = "dtReturnInvoice";
            _dataSet.Tables.Add(dt);
            #region Parameter
            GetCommonParameters(username, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount);
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecieveAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.ReturnDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", "Remarks: " + ROrder.Remarks);
            _reportParameters.Add(_reportParameter);
            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReturnInvoice.rdlc");
        }



        public byte[] DailyWorkSheet(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var reportData = _salesOrderService.DailyWorkSheetReport(fromDate, toDate, concernID);
            TransactionalDataSet.dtDailyWorkSheetDataTable dt = new TransactionalDataSet.dtDailyWorkSheetDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.DueSales, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
            }

            dt.TableName = "dtDailyWorkSheet";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptDailyWorkSheet.rdlc");
        }

        /// <summary>
        /// Author:aminul
        /// Date: 20-Mar-2018
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public byte[] SRVisitReportUsingSP(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            var SRVisitData = _SRVisitService.SRVisitReport(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRVisitReportDataTable dt = new TransactionalDataSet.dtSRVisitReportDataTable();
            _dataSet = new DataSet();
            List<SRVisitReportModel> SRVisitList = new List<SRVisitReportModel>();
            StringBuilder strBuilderSalesIMEI = new StringBuilder();
            StringBuilder strBuilderOpening = new StringBuilder();
            StringBuilder strBuilderTaken = new StringBuilder();
            StringBuilder strBuilderBalance = new StringBuilder();

            if (SRVisitData.Count() != 0)
            {
                var tempSRVisits = SRVisitData.Where(i => i.TransDate >= fromDate && i.TransDate <= toDate);
                if (tempSRVisits.Count() == 0)
                {
                    var LastSRVisit = SRVisitData.OrderByDescending(i => i.TransDate).FirstOrDefault();
                    if (LastSRVisit.TransDate <= toDate)
                    {
                        var TodaySRVisit = new SRVisitReportModel();
                        TodaySRVisit.TransDate = toDate;
                        TodaySRVisit.balance_qty = LastSRVisit.balance_qty;
                        TodaySRVisit.ConcernID = LastSRVisit.ConcernID;
                        TodaySRVisit.EmployeeId = LastSRVisit.EmployeeId;
                        TodaySRVisit.EmployeeName = LastSRVisit.EmployeeName;
                        TodaySRVisit.imeno_balance = LastSRVisit.imeno_balance;
                        TodaySRVisit.Opening_productno = LastSRVisit.Opening_productno;
                        TodaySRVisit.Opening_imeno = LastSRVisit.imeno_balance;
                        TodaySRVisit.sale_imeno = string.Empty;
                        TodaySRVisit.taken_imeno = string.Empty;
                        SRVisitList.Add(TodaySRVisit);
                    }

                }
                else
                    SRVisitList.AddRange(tempSRVisits);
            }

            string linedraw = "------------------------------------------------------------";
            int OpeningCount = 0, OpeningGrandCount = 0, TakenCount = 0, TakenCountGrand = 0, SalesCount = 0, SalesCountGrand = 0, BalanceCount = 0, BalanceCountGrand = 0;
            foreach (var item in SRVisitList)
            {


                #region SaleIMEI
                string[] SaleIMEI = item.sale_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                SalesCount = SaleIMEI.Length;
                SalesCountGrand += SalesCount;
                string tempProduct = string.Empty;
                int productcounter = 0;
                foreach (var imei in SaleIMEI)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty)) //new product
                    {
                        strBuilderSalesIMEI.Append(product);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }

                    if (!product.Equals(tempProduct)) //last imei of product
                    {
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append("           SubTotal: " + productcounter);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        productcounter = 0;


                        tempProduct = product;
                        strBuilderSalesIMEI.Append(product);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                    }

                    strBuilderSalesIMEI.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderSalesIMEI.Append(linedraw);
                strBuilderSalesIMEI.Append(System.Environment.NewLine);
                strBuilderSalesIMEI.Append("             Total: " + SalesCount);
                #endregion

                #region Opening
                tempProduct = string.Empty;
                productcounter = 0;
                string[] Opening_imeno = item.Opening_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                OpeningCount = Opening_imeno.Length;
                OpeningGrandCount += OpeningCount;
                foreach (var imei in Opening_imeno)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderOpening.Append(product);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append("           SubTotal: " + productcounter);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderOpening.Append(product);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                    }

                    strBuilderOpening.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderOpening.Append(linedraw);
                strBuilderOpening.Append(System.Environment.NewLine);
                strBuilderOpening.Append("             Total: " + OpeningCount);
                #endregion

                #region Taken
                tempProduct = string.Empty;
                productcounter = 0;
                string[] taken_imeno = item.taken_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                TakenCount = taken_imeno.Length;
                TakenCountGrand += TakenCount;
                foreach (var imei in taken_imeno)
                {

                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderTaken.Append(product);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append("           SubTotal: " + productcounter);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderTaken.Append(product);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                    }
                    strBuilderTaken.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderTaken.Append(linedraw);
                strBuilderTaken.Append(System.Environment.NewLine);
                strBuilderTaken.Append("             Total: " + TakenCount);
                #endregion

                #region Balance
                tempProduct = string.Empty;
                productcounter = 0;
                string[] imeno_balance = item.imeno_balance.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                BalanceCount = imeno_balance.Length;
                BalanceCountGrand += BalanceCount;
                foreach (var imei in imeno_balance)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderBalance.Append(product);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append("           SubTotal: " + productcounter);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderBalance.Append(product);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                    }

                    strBuilderBalance.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;

                }
                strBuilderBalance.Append(linedraw);
                strBuilderBalance.Append(System.Environment.NewLine);
                strBuilderBalance.Append("             Total: " + BalanceCount);
                #endregion

                dt.Rows.Add(item.ConcernID, item.EmployeeId, item.EmployeeName, item.TransDate, item.OpeningQty, strBuilderOpening, item.Opening_productno, item.taken_qty, strBuilderTaken, item.taken_product, item.Total_qty, item.sale_qty, strBuilderSalesIMEI, item.sale_product, item.balance_qty, strBuilderBalance, item.product_balance);

                strBuilderBalance.Clear();
                strBuilderOpening.Clear();
                strBuilderSalesIMEI.Clear();
                strBuilderTaken.Clear();
            }

            dt.TableName = "dtSRVisitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            var SR = SRVisitData.FirstOrDefault();
            string SRName = string.Empty;
            if (SR != null)
            {
                SRName = SR.EmployeeName;
            }
            _reportParameter = new ReportParameter("SRName", SRName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DateRange", "SR visit Report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningGrandCount", OpeningGrandCount.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TakenCountGrand", TakenCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("SalesCountGrand", SalesCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BalanceCountGrand", BalanceCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Date:" + GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitReportWithIMEI.rdlc");
        }

        /// <summary>
        /// Author:aminul
        /// Date: 20-Mar-2018
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public byte[] SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            var reportData = _SRVisitService.SRWiseCustomerStatusReport(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRWiseCustomerStatusReportDataTable dt = new TransactionalDataSet.dtSRWiseCustomerStatusReportDataTable();
            _dataSet = new DataSet();
            decimal netsales = 0, expenseAmount = 0, NetExpenseAmt = 0;
            int employeeID = 0;
            reportData.OrderBy(i => i.EmployeeID);
            foreach (var item in reportData)
            {
                netsales = item.SlaesAmount - item.ReturnAmount;
                expenseAmount = _expenditureService.GetExpenditureAmountByUserID(_userService.GetUserIDByEmployeeID(item.EmployeeID), fromDate, toDate);
                if (employeeID != item.EmployeeID)
                {
                    NetExpenseAmt += expenseAmount;
                    employeeID = item.EmployeeID;
                }
                dt.Rows.Add(item.ConcernID, item.EmployeeID, item.EmployeeName, item.CustomerID, item.Code, item.Name, (item.Address + ", " + item.ContactNo), item.Address, item.Quantity, item.OpeningDue, item.SlaesAmount, item.ReturnAmount, netsales, item.Collection, item.ClosingAmount, expenseAmount);
            }

            dt.TableName = "dtSRWiseCustomerStatusReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("SRName", reportData.Count() == 0 ? "" : reportData.FirstOrDefault().EmployeeName);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("NetExpenseAmt", NetExpenseAmt.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "SR wise customer status Report Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRWiseCustomerStatusReport.rdlc");
        }


        public byte[] ReplacementReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.ReplacementOrderReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReplacementOrderReportDataTable dt = new TransactionalDataSet.dtReplacementOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.SOrderID, item.SalesDate, item.Invoice, item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.DamageProudct, item.DamageIMEI, item.DamageQty, item.DamageSalesRate, item.ReplaceProduct, item.ReplaceIMEI, item.ReplaceQty, item.ReplaceRate, item.Remarks, item.PODate);
            }

            dt.TableName = "dtReplacementOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Replacement report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderReport.rdlc");
        }


        public byte[] ReturntReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.ReturnOrderReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReturnOrderReportDataTable dt = new TransactionalDataSet.dtReturnOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.Remarks, item.ProductName, item.ReturnIMEI, item.ReturnQty, item.ReturnAmount);
            }

            dt.TableName = "dtReturnOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Return report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReturnOrderReport.rdlc");
        }


        private byte[] CashCollectionMoneyReceiptPrint(CashCollection cashCollection, string userName, int concernID)
        {
            var Customer = _customerService.GetCustomerById((int)cashCollection.CustomerID);
            var Sales = _salesOrderService.GetLastSalesOrderByCustomerID((int)cashCollection.CustomerID);
            string user = _userService.GetUserNameById(cashCollection.CreatedBy);
            _dataSet = new DataSet();
            //dt.TableName = "dtReturnOrderReport";
            //_dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReceiptNo", cashCollection.ReceiptNo);
            _reportParameters.Add(_reportParameter);
            string sInwodTk = TakaFormat(Convert.ToDouble(cashCollection.Amount.ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            //_SOrder.RecAmount.ToString()
            _reportParameter = new ReportParameter("ReceiptTK", cashCollection.Amount.ToString());
            _reportParameters.Add(_reportParameter);
            //_SOrder.InvoiceDate.ToString()
            _reportParameter = new ReportParameter("ReceiptDate", cashCollection.EntryDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TransitionType", cashCollection.TransactionType.ToString());
            _reportParameters.Add(_reportParameter);


            if (cashCollection.Remarks != null)
            {
                _reportParameter = new ReportParameter("Remarks", cashCollection.Remarks.ToString());
                _reportParameters.Add(_reportParameter);
            }

            _reportParameter = new ReportParameter("LastSalesDate", Sales != null ? Sales.InvoiceDate.ToString("dd MMM yyyy") : "");
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", Customer.Code + " " + "&" + " " + Customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("BalanceDue", (Customer.TotalDue).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CusAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContactNo", Customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Adjustment", cashCollection.AdjustAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InterestAmt", cashCollection.InterestAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);



            _reportParameter = new ReportParameter("User", user);
            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }


            //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMMoneyReceiptSS.rdlc");
            //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKMoneyReceipt.rdlc");
            //else
            if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
            {
                _reportParameter = new ReportParameter("Msg", " বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                _reportParameters.Add(_reportParameter);
            }

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMMoneyReceipt.rdlc");

        }
        public byte[] CashDeliveryMoneyReceiptPrint(int CashDeliveryID, string userName, int concernID)
        {
            var cashCollection = _CashCollectionService.GetCashCollectionById(CashDeliveryID);
            var Supplier = _SupplierService.GetSupplierById((int)cashCollection.SupplierID);
            var POrder = _purchaseOrderService.GetAllIQueryable().Where(i => i.SupplierID == (int)cashCollection.SupplierID).OrderByDescending(i => i.OrderDate).FirstOrDefault();
            string user = _userService.GetUserNameById(cashCollection.CreatedBy);
            _dataSet = new DataSet();
            //dt.TableName = "dtReturnOrderReport";
            //_dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReceiptNo", cashCollection.ReceiptNo);
            _reportParameters.Add(_reportParameter);
            string sInwodTk = TakaFormat(Convert.ToDouble(cashCollection.Amount.ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            //_SOrder.RecAmount.ToString()
            _reportParameter = new ReportParameter("ReceiptTK", cashCollection.Amount.ToString());
            _reportParameters.Add(_reportParameter);
            //_SOrder.InvoiceDate.ToString()
            _reportParameter = new ReportParameter("ReceiptDate", cashCollection.EntryDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("LastSalesDate", POrder != null ? POrder.OrderDate.ToString("dd MMM yyyy") : "");
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", Supplier.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("BalanceDue", (Supplier.TotalDue).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CusAddress", Supplier.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContactNo", Supplier.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Adjustment", cashCollection.AdjustAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TransitionType", cashCollection.TransactionType.ToString());
            _reportParameters.Add(_reportParameter);

            if (cashCollection.Remarks != null)
            {
                _reportParameter = new ReportParameter("Remarks", cashCollection.Remarks.ToString());
                _reportParameters.Add(_reportParameter);
            }

            _reportParameter = new ReportParameter("User", user);
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDMoneyReceiptSS.rdlc");
            else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDKMoneyReceipt.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDMoneyReceipt.rdlc");

        }
        public byte[] CashCollectionMoneyReceipt(CashCollection cashCollection, string userName, int concernID)
        {
            return CashCollectionMoneyReceiptPrint(cashCollection, userName, concernID);
        }
        public byte[] CashCollectionMoneyReceiptByID(int CashCollectionID, string userName, int concernID)
        {
            var cashCollection = _CashCollectionService.GetCashCollectionById(CashCollectionID);
            return CashCollectionMoneyReceiptPrint(cashCollection, userName, concernID);
        }

        public byte[] SalesOrderMoneyReceipt(SOrder oSorder, string userName, int concernID)
        {
            return SalesOrderMoneyReceiptPrint(oSorder, userName, concernID, false);
        }
        public byte[] SalesOrderMoneyReceiptByID(int SOrderID, string userName, int concernID)
        {
            var sorder = _salesOrderService.GetSalesOrderById(SOrderID);
            return SalesOrderMoneyReceiptPrint(sorder, userName, concernID, false);
        }

        public byte[] SOrderMoneyReceiptByID(int sorderId, string userName, int concernID, bool isPosRecipt)
        {
            var cashCollection = _salesOrderService.GetSalesOrderById(sorderId);
            return SalesOrderMoneyReceiptPrint(cashCollection, userName, concernID, isPosRecipt);
        }



        private byte[] CrditSalesMoneyReceiptShow(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID)
        {
            if (schedules == null)
                schedules = new CreditSalesSchedule();
            var Customer = _customerService.GetCustomerById(CreditSale.CustomerID);
            _dataSet = new DataSet();
            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CusCode", Customer.Code);
            _reportParameters.Add(_reportParameter);

            string sInwodTk = TakaFormat(Convert.ToDouble(schedules.InstallmentAmt));

            _reportParameter = new ReportParameter("CusName", Customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContact", Customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", CreditSale.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TSalesAmt", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remaining", CreditSale.Remaining.ToString("F"));
            _reportParameters.Add(_reportParameter);
            if (CreditSale.DownPayment != 0m)
                sInwodTk = TakaFormat(Convert.ToDouble(CreditSale.DownPayment.ToString()));
            else
                sInwodTk = TakaFormat(Convert.ToDouble(schedules.InstallmentAmt.ToString()));

            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            if (CreditSale.DownPayment != 0m)
                _reportParameter = new ReportParameter("InstallmentOrDownPayment", CreditSale.DownPayment.ToString("F"));
            else
                _reportParameter = new ReportParameter("InstallmentOrDownPayment", schedules.InstallmentAmt.ToString("F"));

            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PaymentDate", schedules.PaymentDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            sInwodTk = TakaFormat(Convert.ToDouble((schedules.InstallmentAmt).ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");

            _reportParameter = new ReportParameter("TReceiveAmt", (CreditSale.NetAmount + CreditSale.PenaltyInterest - CreditSale.Remaining).ToString("F"));
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("PrintedBy", Global.CurrentUser.UserName);
            //_reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("SalesDate", CreditSale.IssueDate.ToString("dd MMM yyy"));
            _reportParameters.Add(_reportParameter);

            string SModels = "";
            Product objProduct = null;
            foreach (var oSItem in details)
            {
                objProduct = _productService.GetProductById(oSItem.ProductID);
                if (SModels == "")
                {
                    SModels = objProduct.ProductName;
                }
                else
                {
                    SModels = SModels + "," + objProduct.ProductName;
                }
            }

            _reportParameter = new ReportParameter("PModels", SModels);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Adjustment", CreditSale.LastPayAdjAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);
            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\SSCreditMoneyReceipt.rdlc");
            //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\KCreditMoneyReceipt.rdlc");
            //else
            if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
            {
                _reportParameter = new ReportParameter("Msg", "বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                _reportParameters.Add(_reportParameter);
            }


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditMoneyReceipt.rdlc");

        }

        public byte[] CrditSalesMoneyReceipt(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID)
        {
            return CrditSalesMoneyReceiptShow(CreditSale, details, schedules, userName, concernID);
        }

        public byte[] CrditSalesMoneyReceiptByID(int CreditSalesID, string userName, int concernID)
        {
            var CreditSale = _creditSalesOrderService.GetSalesOrderById(CreditSalesID);

            var details = _creditSalesOrderService.GetSalesOrderDetails(CreditSalesID).ToList();
            var schedules = _creditSalesOrderService.GetSalesOrderSchedules(CreditSalesID).Where(i => i.PaymentStatus == "Paid" && i.InstallmentAmt != 0m).OrderByDescending(i => i.CSScheduleID).FirstOrDefault();
            if (schedules != null)
                CreditSale.DownPayment = 0m;
            return CrditSalesMoneyReceiptShow(CreditSale, details, schedules, userName, concernID);
        }

        public byte[] MonthlyBenefit(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var Data = _salesOrderService.MonthlyBenefitReport(fromDate, toDate, concernID);
            _dataSet = new DataSet();
            TransactionalDataSet.dtMonthlyBenefitReportDataTable dt = new TransactionalDataSet.dtMonthlyBenefitReportDataTable();

            foreach (var it in Data)
            {
                dt.Rows.Add(it.InvoiceDate,
                    it.SalesTotal + it.CreditSalesTotal - it.TDAmount_Sale - it.TDAmount_CreditSale,
                    it.PurchaseTotal + it.CreditPurchase,
                    it.TDAmount_Sale,
                    it.TDAmount_CreditSale,
                    it.FirstTotalInterest,
                    it.HireCollection,
                    it.CreditSalesTotal,
                    it.CreditPurchase,
                    it.CommisionProfit,
                    it.HireProfit,
                    it.TotalProfit,
                    it.OthersIncome,
                    it.TotalIncome,
                    it.Adjustment,
                    it.LastPayAdjustment,
                    it.TotalExpense,
                    it.Benefit);
            }


            dt.TableName = "dtMonthlyBenefitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Monthly Benefit Report From Month: " + fromDate.ToString("MMMM-yyyy") + " to " + toDate.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MonthlyBenefitRptMobile.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MonthlyBenefitRpt.rdlc");


        }


        public byte[] ProductWiseBenefitReport(DateTime fromDate, DateTime toDate, int ProductID, string userName, int concernID, int CompanyID, int CategoryID, int CustomerID)
        {
            var Data = _salesOrderService.ProductWiseBenefitReport(fromDate, toDate, concernID);
            Data = Data.OrderBy(i => i.SalesDate).ToList();
            _dataSet = new DataSet();
            TransactionalDataSet.dtBenefitRptDataTable dt = new TransactionalDataSet.dtBenefitRptDataTable();
            if (ProductID != 0)
                Data = Data.Where(i => i.ProductID == ProductID).ToList();

            string CompnayOrCategoryName = " ";
            if (CompanyID != 0)
            {
                var Cinfo = _CompanyService.GetCompanyById(CompanyID);
                CompnayOrCategoryName = Cinfo.Name;

                Data = (from da in Data
                        join p in _productService.GetAllProducts() on da.ProductID equals p.ProductID
                        join com in _CompanyService.GetAllCompany() on p.CompanyID equals com.CompanyID
                        where p.CompanyID == CompanyID
                        select da).ToList();
            }




            if (CategoryID != 0)
            {
                var Cinfo = _CategoryService.GetCategoryById(CategoryID);
                CompnayOrCategoryName = Cinfo.Description;

                Data = (from da in Data
                        join p in _productService.GetAllProducts() on da.ProductID equals p.ProductID
                        join ca in _CategoryService.GetAllIQueryable() on p.CategoryID equals ca.CategoryID
                        where p.CategoryID == CategoryID
                        select da).ToList();
            }

            if (CustomerID != 0)
            {
                var Cinfo = _customerService.GetCustomerById(CustomerID);
                CompnayOrCategoryName = "Customer Name: " + Cinfo.Name;

                Data = (from da in Data
                        where da.CustomerID == CustomerID
                        select da).ToList();
            }



            foreach (var item in Data)
            {
                dt.Rows.Add(item.Code, item.ProductName, item.CategoryName, item.IMENO, item.SalesTotal, item.Discount, item.NetSales, item.PurchaseTotal, item.CommisionProfit, item.HireProfit, item.HireCollection, item.TotalProfit, item.SalesDate, "", item.Quantity);
            }


            dt.TableName = "dtBenefitRpt";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("Month", "Product Wise Benefit Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            if (CompanyID != 0 || CategoryID != 0 || CustomerID != 0)

                _reportParameter = new ReportParameter("CompnayOrCategoryName", CompnayOrCategoryName);
            _reportParameters.Add(_reportParameter);

            if (CompanyID != 0 || CategoryID != 0 || CustomerID != 0)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\BenefitRptCompanyOrCategory.rdlc");
            }
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\BenefitRpt.rdlc");

        }


        public byte[] ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int CustomerID, string userName, int concernID)
        {
            var Data = _salesOrderService.ProductWiseSalesReport(fromDate, toDate, concernID, CustomerID);

            var ReturnData = _returnOrderService.ProductWiseReturnReport(fromDate, toDate, concernID, CustomerID);



            _dataSet = new DataSet();

            TransactionalDataSet.dtProductWiseSalesDataTable dt = new TransactionalDataSet.dtProductWiseSalesDataTable();
            TransactionalDataSet.dtProductWiseReturnDataTable dtReturn = new TransactionalDataSet.dtProductWiseReturnDataTable();


            var CreditSalesData = _creditSalesOrderService.ProductWiseCreditSalesReport(fromDate, toDate, concernID, CustomerID);

            foreach (var item in Data)
            {
                dt.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Sales");
            }

            foreach (var item in CreditSalesData)
            {
                dt.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Credit Sales");
            }


            foreach (var item in ReturnData)
            {
                dtReturn.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Sales");
            }


            decimal TotalSalesQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;
            decimal TotalReturnQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;


            decimal TotalSalesPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;
            decimal TotalReturnPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;




            decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;
            decimal NetPrice = TotalSalesPrice - TotalReturnPrice;

            dt.TableName = "dtProductWiseSales";
            _dataSet.Tables.Add(dt);
            dtReturn.TableName = "dtProductWiseReturn";
            _dataSet.Tables.Add(dtReturn);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\ProductWiseSalesRpt.rdlc");

        }

        public byte[] ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int SupplierID, string userName, int concernID, EnumPurchaseType PurchaseType)
        {
            var Data = _purchaseOrderService.ProductWisePurchaseReport(fromDate, toDate, concernID, SupplierID, PurchaseType);
            _dataSet = new DataSet();
            TransactionalDataSet.dtProductWiseSalesDataTable dt = new TransactionalDataSet.dtProductWiseSalesDataTable();

            foreach (var item in Data)
            {
                dt.Rows.Add(0, item.Date, item.SupplierCode, item.SupplierName, "", "", (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.PurchaseRate, item.TotalAmount);
            }


            dt.TableName = "dtProductWiseSales";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\ProductWisePurchaseRpt.rdlc");

        }

        public byte[] DamageProductReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.DamageProductReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReplacementOrderReportDataTable dt = new TransactionalDataSet.dtReplacementOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.SOrderID, item.SalesDate, item.Invoice, item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.DamageProudct, item.DamageIMEI, item.DamageQty, item.DamageSalesRate, item.ReplaceProduct, item.ReplaceIMEI, item.ReplaceQty, item.ReplaceRate, item.Remarks);
            }

            dt.TableName = "dtReplacementOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Damage Product report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptDamageProductReport.rdlc");
        }

        /// <summary>
        /// Date: 16-May-2018
        /// </summary>
        public byte[] SRWiseCashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            try
            {
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();

                //Cash and Bank Transactions
                var CashCollectionInfos = _CashCollectionService.GetSRWiseCashCollectionReportData(fromDate, toDate, concernID, EmployeeID);

                //Receive Amount
                var SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, EmployeeID);

                foreach (var item in SalesOrders)
                {
                    dt.Rows.Add(item.InvoiceDate, item.CustomerName, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cash Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo);
                }
                foreach (var grd in CashCollectionInfos)
                {
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7);
                }

                if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
                {
                    //Downpayment and Installemnt Collections
                    var CreditSales = _creditSalesOrderService.SRWiseCreditSalesReport(EmployeeID, fromDate, toDate);
                    foreach (var item in CreditSales)
                    {
                        dt.Rows.Add(item.InvoiceDate, item.CustomerName, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cr. Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo);
                    }
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "SR Wise Cash Collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptSRWiseCashCollection.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 17-05-2020
        /// Author: Mohammad Aminul Islam
        /// Reason: Add Customer type in search and show in report
        /// </summary>
        /// <returns>Product wise sales details report</returns>
        public byte[] ProductwiseSalesDetails(string userName, int concernID, int reportType,
                int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, int CustomerType,
                int CustomerID)
        {
            try
            {
                var Data = _salesOrderService.ProductWiseSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);
                var CreditData = _creditSalesOrderService.ProductWiseCreditSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);
                var ReturnData = _returnOrderService.ProductWiseReturnDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);

                TransactionalDataSet.PWSDetailsDataTable dt = new TransactionalDataSet.PWSDetailsDataTable();
                TransactionalDataSet.PWRDetailsDataTable dtReturn = new TransactionalDataSet.PWRDetailsDataTable();

                _dataSet = new DataSet();
                string InvoiceNo = string.Empty;

                foreach (var item in Data)
                {
                    dt.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI, (EnumCustomerType)item.CustomerType);
                }
                foreach (var item in CreditData)
                {
                    dt.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI, (EnumCustomerType)item.CustomerType);
                }
                foreach (var item in ReturnData)
                {
                    dtReturn.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI, (EnumCustomerType)item.CustomerType);
                }

                decimal TotalSalesQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;
                TotalSalesQuantity = TotalSalesQuantity + (CreditData.Count() != 0 ? CreditData.Sum(o => o.Quantity) : 0m);
                decimal TotalReturnQuantity = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.Quantity) : 0m;

                decimal TotalSalesAmt = CreditData.Count() != 0 ? CreditData.Sum(o => o.TotalAmount) : 0m;
                TotalSalesAmt = TotalSalesAmt + (Data.Count() != 0 ? Data.Sum(o => o.TotalAmount) : 0m);
                decimal TotalReturnPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;

                decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;
                decimal NetPrice = TotalSalesAmt - TotalReturnPrice;

                dt.TableName = "PWSDetails";
                _dataSet.Tables.Add(dt);

                dtReturn.TableName = "PWRDetails";
                _dataSet.Tables.Add(dtReturn);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                if (reportType == 0)
                {
                    _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptPWSalesDetails.rdlc");
                }
                else if (reportType == 1)
                {
                    _reportParameter = new ReportParameter("DateRange", "Company Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCompanyWiseSDetails.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("DateRange", "Category Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCategoryWSalesDetails.rdlc");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 17-05-2020
        /// Author: Mohammad Aminul Islam
        /// Reason: Add Customer type in search and show in report
        /// </summary>
        /// <returns>Product wise sales summary report</returns>
        public byte[] ProductwiseSalesSummary(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate,
            DateTime toDate, int CustomerType, int CustomerID)
        {
            try
            {
                var Data = _salesOrderService.ProductWiseSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);
                var CreditData = _creditSalesOrderService.ProductWiseCreditSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);
                var ReturnData = _returnOrderService.ProductWiseReturnDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, CustomerType, CustomerID);
                TransactionalDataSet.PWSDetailsDataTable dt = new TransactionalDataSet.PWSDetailsDataTable();
                TransactionalDataSet.PWRDetailsDataTable dtReturn = new TransactionalDataSet.PWRDetailsDataTable();
                _dataSet = new DataSet();
                List<ProductWiseSalesReportModel> AllSales = new List<ProductWiseSalesReportModel>();

                List<ProductWiseSalesReportModel> AllReturns = new List<ProductWiseSalesReportModel>();
                if (reportType == 0) //Product Wise
                {

                    var sales = from s in Data
                                group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate, s.CustomerType } into g
                                select new ProductWiseSalesReportModel
                                {
                                    ProductID = g.Key.ProductID,
                                    ProductName = g.Key.ProductName,
                                    CategoryID = g.Key.CategoryID,
                                    CategoryName = g.Key.CategoryName,
                                    CompanyID = g.Key.CompanyID,
                                    CompanyName = g.Key.CompanyName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    SalesRate = g.Key.SalesRate,
                                    TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate,
                                    CustomerType = g.Key.CustomerType
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate, s.CustomerType } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          ProductID = g.Key.ProductID,
                                          ProductName = g.Key.ProductName,
                                          CategoryID = g.Key.CategoryID,
                                          CategoryName = g.Key.CategoryName,
                                          CompanyID = g.Key.CompanyID,
                                          CompanyName = g.Key.CompanyName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          SalesRate = g.Key.SalesRate,
                                          TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate,
                                          CustomerType = g.Key.CustomerType
                                      };
                    AllSales.AddRange(Creditsales);



                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate, s.CustomerType } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      ProductID = g.Key.ProductID,
                                      ProductName = g.Key.ProductName,
                                      CategoryID = g.Key.CategoryID,
                                      CategoryName = g.Key.CategoryName,
                                      CompanyID = g.Key.CompanyID,
                                      CompanyName = g.Key.CompanyName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      SalesRate = g.Key.SalesRate,
                                      TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate,
                                      CustomerType = g.Key.CustomerType
                                  };
                    AllReturns.AddRange(returns);

                }
                else if (reportType == 1) // Company Wise
                {

                    var sales = from s in Data
                                group s by new { s.Date, s.CompanyID, s.CompanyName, s.CustomerType } into g
                                select new ProductWiseSalesReportModel
                                {
                                    CompanyID = g.Key.CompanyID,
                                    CompanyName = g.Key.CompanyName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    TotalAmount = g.Sum(i => i.TotalAmount),
                                    CustomerType = g.Key.CustomerType
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.CompanyID, s.CompanyName, s.CustomerType } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          CompanyID = g.Key.CompanyID,
                                          CompanyName = g.Key.CompanyName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          TotalAmount = g.Sum(i => i.TotalAmount),
                                          CustomerType = g.Key.CustomerType
                                      };



                    AllSales.AddRange(Creditsales);


                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.CompanyID, s.CompanyName, s.CustomerType } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      CompanyID = g.Key.CompanyID,
                                      CompanyName = g.Key.CompanyName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                      CustomerType = g.Key.CustomerType
                                  };
                    AllReturns.AddRange(returns);



                }
                else //Category wise
                {
                    var sales = from s in Data
                                group s by new { s.Date, s.CategoryID, s.CategoryName, s.CustomerType } into g
                                select new ProductWiseSalesReportModel
                                {
                                    CategoryID = g.Key.CategoryID,
                                    CategoryName = g.Key.CategoryName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    TotalAmount = g.Sum(i => i.TotalAmount),
                                    CustomerType = g.Key.CustomerType
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.CategoryID, s.CategoryName, s.CustomerType } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          CategoryID = g.Key.CategoryID,
                                          CategoryName = g.Key.CategoryName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          TotalAmount = g.Sum(i => i.TotalAmount),
                                          CustomerType = g.Key.CustomerType
                                      };
                    AllSales.AddRange(Creditsales);


                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.CategoryID, s.CategoryName, s.CustomerType } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      CategoryID = g.Key.CategoryID,
                                      CategoryName = g.Key.CategoryName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                      CustomerType = g.Key.CustomerType
                                  };
                    AllReturns.AddRange(returns);


                }

                foreach (var item in AllSales)
                {
                    dt.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, (EnumCustomerType)item.CustomerType);
                }

                foreach (var item in AllReturns)
                {
                    dtReturn.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, (EnumCustomerType)item.CustomerType);
                }


                decimal TotalSalesQuantity = AllSales.Count() != 0 ? AllSales.Sum(o => o.Quantity) : 0m;
                decimal TotalReturnQuantity = AllReturns.Count() != 0 ? AllReturns.Sum(o => o.Quantity) : 0m;
                decimal TotalSalesPrice = AllSales.Count() != 0 ? AllSales.Sum(o => o.TotalAmount) : 0m;
                decimal TotalReturnPrice = AllReturns.Count() != 0 ? AllReturns.Sum(o => o.TotalAmount) : 0m;

                decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;

                decimal NetPrice = TotalSalesPrice - TotalReturnPrice;



                //foreach (var item in Creditsales)
                //{
                //    dt.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.Quantity * item.SalesRate, "");
                //}

                dt.TableName = "PWSDetails";
                _dataSet.Tables.Add(dt);


                dtReturn.TableName = "PWRDetails";
                _dataSet.Tables.Add(dtReturn);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                if (reportType == 0)
                {
                    _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptPWSalesSummary.rdlc");
                }
                else if (reportType == 1)
                {
                    _reportParameter = new ReportParameter("DateRange", "Company Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCompanyWiseSalesSummary.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("DateRange", "Category Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCategoryWiseSalesSummary.rdlc");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductWisePurchaseDetailsReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType, int SupplierID)
        {
            try
            {
                List<ProductWisePurchaseModel> ReportDate = new List<ProductWisePurchaseModel>();
                TransactionalDataSet.PWPDetailsDataTable dt = new TransactionalDataSet.PWPDetailsDataTable();
                _dataSet = new DataSet();
                var Data = _purchaseOrderService.ProductWisePurchaseDetailsReportNew(CompanyID, CategoryID, ProductID, fromDate, toDate, PurchaseType, SupplierID);

                if (reportType == 0)
                {
                    ReportDate = Data;
                }
                else if (reportType == 1)
                {
                    ReportDate = (from d in Data
                                  group d by new { d.CategoryName, d.CompanyName } into g
                                  select new ProductWisePurchaseModel
                                  {
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                      TotalMRP = g.Sum(i => i.TotalMRP)
                                  }).ToList();
                }
                else
                {
                    ReportDate = (from d in Data
                                  group d by new { d.CategoryName } into g
                                  select new ProductWisePurchaseModel
                                  {
                                      CategoryName = g.Key.CategoryName,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                      TotalMRP = g.Sum(i => i.TotalMRP)
                                  }).ToList();
                }

                foreach (var item in ReportDate)
                {
                    dt.Rows.Add(item.Date, item.ChallanNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.PurchaseRate, (item.PurchaseRate * item.Quantity), item.TotalMRP);
                }

                dt.TableName = "PWPDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);
                if (PurchaseType == EnumPurchaseType.ProductReturn)
                {
                    if (reportType == 0)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPDetails.rdlc");
                    }
                    else if (reportType == 1)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Company Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCompanyWPDetails.rdlc");
                    }
                    else
                    {
                        _reportParameter = new ReportParameter("DateRange", "Category Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCategoryWPDetails.rdlc");
                    }
                }
                else
                {
                    if (reportType == 0)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPDetails.rdlc");
                    }
                    else if (reportType == 1)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Company Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCompanyWPDetails.rdlc");
                    }
                    else
                    {
                        _reportParameter = new ReportParameter("DateRange", "Category Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCategoryWPDetails.rdlc");
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] BankTransactionReport(string userName, int concernID, int reportType, int BankID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Data = _bankTransactionService.GetAllBankTransaction();
                TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                if (BankID != 0)
                {
                    Data = Data.Where(o => o.Item4 == BankID.ToString() && o.Rest.Item2 >= fromDate && o.Rest.Item2 <= toDate);
                }
                int TransType = 0;
                foreach (var item in Data)
                {
                    row = dt.NewRow();
                    TransType = Convert.ToInt32(item.Item7);

                    row["TranDate"] = item.Rest.Item2;
                    row["TransactionNo"] = item.Item6;
                    row["TransactionType"] = (EnumTransactionType)TransType;
                    row["BankName"] = item.Item2;

                    if (TransType == (int)EnumTransactionType.CashDelivery || TransType == (int)EnumTransactionType.Withdraw || TransType == (int)EnumTransactionType.FundTransfer)
                        row["Amount"] = -item.Rest.Item1;
                    else
                        row["Amount"] = item.Rest.Item1;

                    row["Remarks"] = item.Rest.Item3;
                    row["ChecqueNo"] = item.Rest.Rest.Item3;
                    row["BranchName"] = item.Rest.Rest.Item1;
                    row["AccountNO"] = item.Rest.Rest.Item2;
                    row["AccountName"] = item.Rest.Rest.Item4;
                    dt.Rows.Add(row);
                }
                dt.TableName = "BankTransaction";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("DateRange", "Bank Transaction for Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankTransactions.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] POInvoicePrint(POrder POrder, string userName, int concernID, bool isPreview = false)
        {
            TransactionalDataSet.dtPOInvoiceDataTable dtPODetail = new TransactionalDataSet.dtPOInvoiceDataTable();
            _dataSet = new DataSet();
            var Supplier = _SupplierService.GetSupplierById(POrder.SupplierID);
            string IMEI = string.Empty;
            string DIMEI = string.Empty;
            string user = _userService.GetUserNameById(POrder.CreatedBy);
            var details = from pod in POrder.POrderDetails
                          join p in _productService.GetAllProductIQueryable() on pod.ProductID equals p.ProductID
                          join c in _ColorServce.GetAll() on pod.ColorID equals c.ColorID
                          select new
                          {
                              p.ProductCode,
                              p.ProductName,
                              p.CategoryName,
                              p.CompanyName,
                              ColorName = c.Name,
                              pod.UnitPrice,
                              pod.PPDISAmt,
                              pod.PPDISPer,
                              pod.ExtraPPDISAmt,
                              pod.ExtraPPDISPer,
                              pod.PPOffer,
                              pod.Quantity,
                              pod.MRPRate,
                              pod.TAmount,
                              IMEIs = pod.POProductDetails.Select(i => i.IMENO).ToList(),
                              DIMEIs = pod.POProductDetails.Select(i => i.DIMENO).ToList()
                          };

            foreach (var item in details)
            {
                IMEI = string.Join("/n", item.IMEIs);
                DIMEI = string.Join("/n", item.DIMEIs);

                dtPODetail.Rows.Add(item.ProductName + "," + item.CategoryName,
                        item.CategoryName, item.CompanyName, item.ColorName, item.UnitPrice,
                        item.PPDISAmt, item.PPDISPer, item.ExtraPPDISAmt,
                        item.ExtraPPDISPer, item.Quantity,
                        item.TAmount, item.ProductCode,
                        item.MRPRate, item.PPOffer, IMEI, DIMEI, (item.MRPRate * item.Quantity));
                IMEI = string.Empty;
                DIMEI = string.Empty;
            }
            dtPODetail.TableName = "dtPOInvoice";
            _dataSet.Tables.Add(dtPODetail);


            GetCommonParameters(userName, concernID);

            if (POrder != null)
            {
                _reportParameter = new ReportParameter("SupplierCode", Supplier.Code);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("SupplierName", Supplier.Name);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("ChallanNo", POrder.ChallanNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("OrderDate", POrder.OrderDate.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("FlatDis", POrder.NetDiscount.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("NetTotal", POrder.TotalAmt.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PaidAmt", POrder.RecAmt.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("CurrentDue", POrder.PaymentDue.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", "Date: " + GetLocalTime());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PPDis", (POrder.NetDiscount - POrder.TDiscount).ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("OnlyFlatDis", POrder.TDiscount.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("VATAmount", POrder.VATAmount.ToString());
                _reportParameters.Add(_reportParameter);

                var TDPercentage = POrder.NetDiscount;
                if (TDPercentage > 0)
                {
                    TDPercentage = (POrder.NetDiscount / POrder.GrandTotal) * 100;
                    _reportParameter = new ReportParameter("DisPercentage", TDPercentage.ToString("0.00"));
                    _reportParameters.Add(_reportParameter);
                }

                //var TDPercentage = (POrder.GrandTotal / POrder.NetDiscount) * 100;
                //_reportParameter = new ReportParameter("FlatPercentage", TDPercentage.ToString("0.00"));
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", "Remarks: " + POrder.Remarks);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("InvoiceNo", POrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);
            }

            if (POrder.Status == (int)EnumPurchaseType.DeliveryOrder)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Delivery Order");
                _reportParameters.Add(_reportParameter);
            }
            else if (POrder.Status == (int)EnumPurchaseType.DamageReturn)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Damage Return Order");
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageReturnInvoice.rdlc");
            }
            else if (POrder.Status == (int)EnumPurchaseType.Purchase && POrder.IsDamageOrder == 1)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Damage Order");
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageRecInvoice.rdlc");
            }
            else if (POrder.Status == (int)EnumPurchaseType.ProductReturn)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Purchase Return Order");
                _reportParameters.Add(_reportParameter);
            }
            else if (isPreview)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Preview Invoice");
                _reportParameters.Add(_reportParameter);
            }
            else
            {
                _reportParameter = new ReportParameter("ReportHeader", "Purchase Order");
                _reportParameters.Add(_reportParameter);
            }

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            if (currentSystemInfo.Name == "Gadget House")
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPOInvoiceEmobile.rdlc");
            }

            if (POrder.Status == (int)EnumPurchaseType.DeliveryOrder)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDOInvoice.rdlc");
            if (concernID == (int)EnumSisterConcern.OCT || concernID == (int)EnumSisterConcern.SKY_PLUS_ELECTRONICS)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPOInvoiceWDis.rdlc");
            }
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPOInvoice.rdlc");
        }

        public byte[] POInvoice(POrder POrder, string userName, int concernID, bool isPreview)
        {
            return POInvoicePrint(POrder, userName, concernID, isPreview);
        }


        public byte[] POInvoiceByID(int POrderID, string userName, int concernID)
        {
            POrder oPOrder = new POrder();

            oPOrder = _purchaseOrderService.GetPurchaseOrderById(POrderID);
            oPOrder.POrderDetails = _PurchaseOrderDetailService.GetPOrderDetailByID(POrderID).ToList();
            if (oPOrder.IsDamageOrder == 1)
            {
                POProductDetail POPD = null;
                foreach (POrderDetail item in oPOrder.POrderDetails)
                {
                    foreach (var sitem in item.POProductDetails)
                    {
                        POPD = _POProductDetailService.GetPOPDetailByPOPDID((int)sitem.DamagePOPDID);
                        if (POPD != null)
                            sitem.DIMENO = POPD.IMENO;
                    }
                }
            }
            return POInvoicePrint(oPOrder, userName, concernID);
        }

        public byte[] GetDamagePOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Purchases = _purchaseOrderService.GetDamagePOReport(fromDate, toDate, SupplierID);

                decimal TotalDuePurchase = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();

                DataRow row = null;
                int POrderID = 0;
                foreach (var item in Purchases)
                {
                    if (POrderID != item.POrderID)
                    {
                        TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                        GrandTotal = GrandTotal + item.GrandTotal;
                        TotalDis = TotalDis + item.NetDiscount;
                        NetTotal = NetTotal + item.NetTotal;
                        RecAmt = RecAmt + item.RecAmt;
                        CurrDue = CurrDue + item.PaymentDue;
                    }
                    row = dt.NewRow();
                    row["PurchaseDate"] = item.Date;
                    row["ChallanNo"] = item.ChallanNo;
                    row["ProductName"] = item.ProductName;
                    row["PurchaseRate"] = item.PurchaseRate;
                    row["DisAmt"] = item.PPDISAmt;
                    row["NetAmt"] = item.TotalAmount;
                    row["GrandTotal"] = item.GrandTotal;
                    row["TotalDis"] = item.NetDiscount;
                    row["NetTotal"] = item.NetTotal;
                    row["PaidAmt"] = item.RecAmt;
                    row["RemainingAmt"] = item.PaymentDue;
                    row["Quantity"] = item.Quantity;
                    row["ChasisNo"] = item.IMENO;
                    row["Model"] = item.CategoryName;
                    row["Color"] = item.ColorName;
                    row["PPOffer"] = item.PPOffer;
                    row["DamageIMEI"] = item.DamageIMEI;

                    dt.Rows.Add(row);

                    //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                    //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, "1", item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);

                    POrderID = item.POrderID;
                }

                dt.TableName = "dtSuppWiseData";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Date", "Damage Purchase details from the Date : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamagePurchaseDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] GetDamageReturnPOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var DamageReutruns = _purchaseOrderService.DamageReturnProductDetailsReport(SupplierID, fromDate, toDate);

                decimal TotalDuePurchase = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();

                DataRow row = null;
                int POrderID = 0;
                foreach (var item in DamageReutruns)
                {
                    if (POrderID != item.POrderID)
                    {
                        TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                        GrandTotal = GrandTotal + item.GrandTotal;
                        TotalDis = TotalDis + item.NetDiscount;
                        NetTotal = NetTotal + item.NetTotal;
                        RecAmt = RecAmt + item.RecAmt;
                        CurrDue = CurrDue + item.PaymentDue;
                    }
                    row = dt.NewRow();
                    row["PurchaseDate"] = item.Date;
                    row["ChallanNo"] = item.ChallanNo;
                    row["ProductName"] = item.ProductName;
                    row["PurchaseRate"] = item.PurchaseRate;
                    row["DisAmt"] = item.PPDISAmt;
                    row["NetAmt"] = item.TotalAmount;
                    row["GrandTotal"] = item.GrandTotal;
                    row["TotalDis"] = item.NetDiscount;
                    row["NetTotal"] = item.NetTotal;
                    row["PaidAmt"] = item.RecAmt;
                    row["RemainingAmt"] = item.PaymentDue;
                    row["Quantity"] = item.Quantity;
                    row["ChasisNo"] = item.IMENO;
                    row["Model"] = item.CategoryName;
                    row["Color"] = item.ColorName;
                    row["PPOffer"] = item.PPOffer;
                    row["DamageIMEI"] = item.DamageIMEI;

                    dt.Rows.Add(row);

                    //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                    //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, "1", item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);

                    POrderID = item.POrderID;
                }

                dt.TableName = "dtSuppWiseData";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Date", "Damage Return Purchase details from the Date : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageReturnPODetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] GetSalarySheet(DateTime dtSalaryMonth, int EmployeeID, int DepartmentID, List<int> EmployeeIDList, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth)
        {
            List<SalaryMonthly> salaryMonthlyList = new List<SalaryMonthly>();
            var list = _SalaryMonthlyService.GetAllIQueryable();
            SalaryMonthly obj = null;
            List<SalaryMonthlyDetail> MDetails = null;
            string DepartmentName = string.Empty;
            if (EmployeeID != default(int))
            {
                var employee = _EmployeeService.GetEmployeeById(EmployeeID);
                var department = _DepartmentService.GetDepartmentById((int)employee.DepartmentID);
                obj = new SalaryMonthly();
                obj = list.FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.SalaryMonth >= SalaryMonth.Item1 && i.SalaryMonth <= SalaryMonth.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                    salaryMonthlyList.Add(obj);
                }
                return ShowSalarySheet(salaryMonthlyList, dtSalaryMonth, department.DESCRIPTION, UserName, ConcernID);
            }


            foreach (var employeeID in EmployeeIDList)
            {
                obj = new SalaryMonthly();
                obj = list.FirstOrDefault(i => i.EmployeeID == employeeID && (i.SalaryMonth >= SalaryMonth.Item1 && i.SalaryMonth <= SalaryMonth.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                    salaryMonthlyList.Add(obj);
                }
            }

            if (DepartmentID != 0)
            {
                var department = _DepartmentService.GetDepartmentById(DepartmentID);
                if (department != null)
                    DepartmentName = department.DESCRIPTION;
            }

            return ShowSalarySheet(salaryMonthlyList, dtSalaryMonth, DepartmentName, UserName, ConcernID);

        }
        private byte[] ShowSalarySheet(List<SalaryMonthly> salaryMonthlyList, DateTime SalaryMonth, string DepartmentName, string UserName, int ConcernID)
        {

            TransactionalDataSet.dtSalaryMonthlyDataTable dt = new TransactionalDataSet.dtSalaryMonthlyDataTable();
            DataRow row = null;
            var designations = _DesignationService.GetAllIQueryable();
            var employees = _EmployeeService.GetAllEmployeeIQueryable();
            decimal TotalAmount = 0, GrossSalary = 0;
            var data = (from sm in salaryMonthlyList
                        join emp in employees on sm.EmployeeID equals emp.EmployeeID
                        join d in designations on emp.DesignationID equals d.DesignationID
                        select new
                        {
                            Code = emp.Code,
                            AccountNO = emp.MachineEMPID,
                            Name = emp.Name,
                            Designation = d.Description,
                            SalaryDetails = sm.SalaryMonthlyDetails.ToList(),
                            WorkinDays = sm.WorkinDays,
                            sm.OTHours
                        }).OrderBy(i => i.Code);
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["Code"] = item.Code;
                row["Name"] = item.Name;
                row["Designation"] = item.Designation;

                foreach (SalaryMonthlyDetail sitem in item.SalaryDetails)
                {
                    if (sitem.ItemID == (int)EnumSalaryItemCode.Basic_Salary)
                        row["BasicSalary"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Gross_Salary)
                        row["GrossSalary"] = sitem.CalculatedAmount;

                    row["Attendence"] = item.WorkinDays;
                    // row["OT"] = Math.Floor(item.OTHours);

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Tot_Attend_Days_Amount)
                        row["AttendenceSalary"] = sitem.CalculatedAmount;

                    //if (sitem.ItemID == (int)EnumSalaryItemCode.Over_Time_Amount)
                    //    row["OTSalary"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus)
                        row["AttendenceBonus"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Net_Payable)
                        row["TotalAmount"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Advance_Deduction)
                    {
                        row["Advance"] = sitem.CalculatedAmount;
                        TotalAmount += sitem.CalculatedAmount;
                    }

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Net_Payable)
                    {
                        row["NetAmount"] = sitem.CalculatedAmount;
                        TotalAmount += sitem.CalculatedAmount;
                    }

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Bonus)
                        row["FestBonus"] = sitem.CalculatedAmount;

                    //if (sitem.ItemID == (int)EnumSalaryItemCode.Voltage_StabilizerComm)
                    //    row["VSCommission"] = sitem.CalculatedAmount;

                    #region Allowance Deduction
                    if (sitem.Description.Contains("House"))
                    {
                        row["HouseRent"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Medical"))
                    {
                        row["Medical"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Transport"))
                    {
                        row["Transport"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Food"))
                    {
                        row["Food"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }
                    #endregion


                    if (sitem.ItemID == (int)EnumSalaryItemCode.Commission)
                        row["Commission"] = sitem.CalculatedAmount;

                    //if (sitem.ItemID == (int)EnumSalaryItemCode.Extra_Commission)
                    //    row["ExtraCommission"] = sitem.CalculatedAmount;
                    if (sitem.ItemID == (int)EnumSalaryItemCode.Due_Salary)
                    {
                        row["DueSalary"] = sitem.CalculatedAmount;
                        TotalAmount += sitem.CalculatedAmount;
                    }
                    if (sitem.ItemID == (int)EnumSalaryItemCode.Due_SalaryPay)
                    {
                        row["DueSalaryPay"] = sitem.CalculatedAmount;
                        TotalAmount -= sitem.CalculatedAmount;
                    }

                }
                row["TotalAmount"] = TotalAmount;
                GrossSalary = 0;
                TotalAmount = 0;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtSalaryMonthly";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("ReportHeading", "Salary Sheet: " + SalaryMonth.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptSalarySheet.rdlc");
        }

        public byte[] GetPaySlip(DateTime dtSalaryMonth, int EmployeeID, string UserName, int ConcernID, Tuple<DateTime, DateTime> DateRange)
        {
            var list = _SalaryMonthlyService.GetAllIQueryable();
            SalaryMonthly obj = null;
            Employee employee = new Employee();
            List<SalaryMonthlyDetail> MDetails = new List<SalaryMonthlyDetail>();
            if (EmployeeID != 0)
            {
                employee = _EmployeeService.GetAllEmployeeIQueryable().FirstOrDefault(i => i.EmployeeID == EmployeeID);
                obj = list.FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.SalaryMonth >= DateRange.Item1 && i.SalaryMonth <= DateRange.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                }
                else
                    return null;
            }
            return ShowPaySlip(obj, employee, dtSalaryMonth, UserName, ConcernID);

        }
        private byte[] ShowPaySlip(SalaryMonthly salaryMonthly, Employee employee, DateTime dtSalaryMonth, string UserName, int ConcernID)
        {

            TransactionalDataSet.dtPaySlipDataTable dt = new TransactionalDataSet.dtPaySlipDataTable();
            DataRow row = null;
            string description = string.Empty;
            List<PaySlip> paySlipList = new List<PaySlip>();
            PaySlip objPayslip = null;
            List<SalaryMonthlyDetail> Allowancec = salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Gross).ToList();
            List<SalaryMonthlyDetail> Deductions = salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Deductions).ToList();
            var LastGradeSalary = _GradeSalaryAssignment.GetLastGradeSalaryByEmployeeID(employee.EmployeeID);
            if (Allowancec.Count() > Deductions.Count())
            {
                foreach (var item in Allowancec)
                {
                    objPayslip = new PaySlip();
                    objPayslip.Allowance = GetUserFriendlyDescription(item.ItemID, item.Description);
                    objPayslip.AllowanceAmount = item.CalculatedAmount;
                    paySlipList.Add(objPayslip);
                }
                for (int i = 0; i < Deductions.Count(); i++)
                {
                    paySlipList[i].Deduction = GetUserFriendlyDescription(Deductions[i].ItemID, Deductions[i].Description);
                    paySlipList[i].DeductionAmount = Deductions[i].CalculatedAmount;
                }
            }
            else
            {
                foreach (var item in Deductions)
                {
                    objPayslip = new PaySlip();
                    objPayslip.Allowance = GetUserFriendlyDescription(item.ItemID, item.Description);
                    objPayslip.AllowanceAmount = item.CalculatedAmount;
                    paySlipList.Add(objPayslip);
                }
                for (int i = 0; i < Allowancec.Count(); i++)
                {
                    paySlipList[i].Deduction = GetUserFriendlyDescription(Deductions[i].ItemID, Deductions[i].Description);
                    paySlipList[i].DeductionAmount = Allowancec[i].CalculatedAmount;
                }
            }
            foreach (var item in paySlipList)
            {
                row = dt.NewRow();
                row["Description"] = item.Allowance;
                row["Earnings"] = item.AllowanceAmount;
                row["TotalEarning"] = item.AllowanceAmount;
                row["Deduction"] = item.Deduction;
                row["Amount"] = item.DeductionAmount;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtPaySlip";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);


            var Grade = _GradeService.GetById((int)employee.GradeID);
            var Department = _DepartmentService.GetAllDepartmentIQueryable().FirstOrDefault(i => i.DepartmentId == employee.DepartmentID);
            var designations = _DesignationService.GetAllIQueryable().FirstOrDefault(i => i.DesignationID == employee.DesignationID);
            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("ReportHeading", "Pay Slip of the Month: " + dtSalaryMonth.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Employee", employee.Name);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("EmployeeCode", employee.Code);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Grade", Grade.Description);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Designation", designations.Description);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BasicSalary", LastGradeSalary.BasicSalary.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PaymentMode", "Cash");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("AccountNo", employee.MachineEMPID.ToString().PadLeft(5, '0'));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DepartmentName", Department.DESCRIPTION);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Print Date: " + GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptPaysSlip.rdlc");
        }
        public byte[] SRVisitReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int ReportType)
        {
            var SRVisitData = _SRVisitService.SRVisitReportDetails(fromDate, toDate, concernID, EmployeeID);
            var Employee = _EmployeeService.GetEmployeeById(EmployeeID);

            TransactionalDataSet.dtSRVisitReportDataTable dt = new TransactionalDataSet.dtSRVisitReportDataTable();
            _dataSet = new DataSet();
            List<SRVisitReportModel> SRVisitList = new List<SRVisitReportModel>();
            DataRow row = null;

            string IMEIs = string.Empty;
            int Counter = 0;
            foreach (var item in SRVisitData)
            {
                row = dt.NewRow();
                row["ConcernID"] = 0m;
                row["EmployeeId"] = 0m;
                row["EmployeeName"] = "";
                row["TransDate"] = item.TransDate;

                row["OpeningQty"] = item.OpeningIMEIList.Count();
                foreach (var sitem in item.OpeningIMEIList)
                {
                    Counter++;
                    if (Counter != item.OpeningIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["Opening_imeno"] = IMEIs;
                IMEIs = string.Empty;
                Counter = 0;
                row["Opening_productno"] = item.ProductName;

                row["taken_qty"] = item.ReceiveIMEIList.Count();
                foreach (var sitem in item.ReceiveIMEIList)
                {
                    Counter++;
                    if (Counter != item.ReceiveIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["taken_imeno"] = IMEIs;

                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.TotalIMEIList)
                {
                    Counter++;
                    if (Counter != item.TotalIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["taken_product"] = IMEIs; //Total IMEIs

                row["Total_qty"] = item.TotalIMEIList.Count();

                row["sale_qty"] = item.SalesIMEIList.Count();

                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.SalesIMEIList)
                {
                    Counter++;
                    if (Counter != item.SalesIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["sale_imeno"] = IMEIs;
                row["sale_product"] = "";

                row["balance_qty"] = item.BalanceIMEIList.Count();
                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.BalanceIMEIList)
                {
                    Counter++;
                    if (Counter != item.BalanceIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["imeno_balance"] = IMEIs;
                row["product_balance"] = "";

                dt.Rows.Add(row);

                IMEIs = string.Empty;
                Counter = 0;
            }

            dt.TableName = "dtSRVisitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);



            _reportParameter = new ReportParameter("SRName", Employee.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DateRange", "SR visit Report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningGrandCount", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TakenCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("SalesCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BalanceCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Date:" + GetLocalTime());
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1) //Summary
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitSummary.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitDetails.rdlc");
        }
        public byte[] NewBankTransactionsReport(DateTime fromDate, DateTime toDate, int BankID, string UserName, int ConcernID, int reportConcern)
        {
            var BanksTrans = _bankTransactionService.BankTransactionsReport(fromDate, toDate, BankID, reportConcern);
            TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in BanksTrans)
            {
                row = dt.NewRow();
                row["TranDate"] = item.TransDate;
                row["TransactionNo"] = item.TransactionNo;
                row["TransactionType"] = item.TransType;
                row["BankName"] = item.BankName;
                row["Amount"] = item.Amount;
                row["Remarks"] = item.Remarks;
                row["ChecqueNo"] = item.ChecqueNo;
                row["BranchName"] = "";
                row["AccountNO"] = item.AccountNO;
                row["AccountName"] = item.AccountName;
                row["ConcernName"] = item.ConcernName;
                row["FromToAccountNo"] = item.FromToAccountNo;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtBankTransaction";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Bank Transaction Report from date " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankTransReport.rdlc");

        }


        #region Admin Report

        public byte[] AdminPurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID)
        {
            try
            {
                var purchaseInfos = _purchaseOrderService.AdminPurchaseReport(fromDate, toDate, concernID);
                DataRow row = null;
                TransactionalDataSet.dtReceiveOrderDataTable dt = new TransactionalDataSet.dtReceiveOrderDataTable();

                foreach (var item in purchaseInfos)
                {
                    row = dt.NewRow();
                    row["CompanyCode"] = item.SupplierCode;
                    row["Name"] = item.SupplierName;
                    row["OrderDare"] = item.Date.ToString("dd MMM yyyy");
                    row["ChallanNo"] = item.ChallanNo;
                    row["GrandTotal"] = item.GrandTotal;
                    row["DisAmt"] = item.NetDiscount;
                    row["TotalAmt"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmt;
                    row["DueAmt"] = item.PaymentDue;
                    row["ConcernName"] = item.ConcenName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtReceiveOrder";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserConcernID);

                _reportParameter = new ReportParameter("Month", "Purchase report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));


                //else if (PurchaseType == EnumPurchaseType.ProductReturn)
                //    _reportParameter = new ReportParameter("Month", "Purchase Return report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));

                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseOrder.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 06-01-2019
        /// Author: aminul
        /// For Admin, to show all concern sales
        /// </summary>
        public byte[] SalesReportAdmin(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID)
        {
            try
            {
                var salseInfos = _salesOrderService.GetAdminSalesReport(concernID, fromDate, toDate);

                var CreditsalseInfos = _creditSalesOrderService.GetAdminCrSalesReport(concernID, fromDate, toDate);

                DataRow row = null;

                TransactionalDataSet.dtOrderDataTable dt = new TransactionalDataSet.dtOrderDataTable();

                foreach (var item in salseInfos)
                {
                    row = dt.NewRow();
                    row["CustomerCode"] = item.CustomerCode;
                    row["Name"] = item.CustomerName;
                    row["Date"] = item.InvoiceDate.ToString("dd MMM yyyy");
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["GrandTotal"] = item.Grandtotal;
                    row["DiscountAmount"] = item.NetDiscount;
                    row["Amount"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmount;
                    row["DueAmount"] = item.PaymentDue;
                    row["SalesType"] = "Cash Sales";
                    row["AdjustAmt"] = item.AdjAmount;
                    row["TotalOffer"] = item.TotalOffer;
                    row["ConcernName"] = item.ConcernName;
                    dt.Rows.Add(row);
                }

                foreach (var item in CreditsalseInfos)
                {
                    row = dt.NewRow();
                    row["CustomerCode"] = item.CustomerCode;
                    row["Name"] = item.CustomerName;
                    row["Date"] = item.InvoiceDate.ToString("dd MMM yyyy");
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["GrandTotal"] = item.Grandtotal;
                    row["DiscountAmount"] = item.NetDiscount;
                    row["Amount"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmount;
                    row["DueAmount"] = item.PaymentDue;
                    row["SalesType"] = "Credit Sales";
                    row["AdjustAmt"] = item.AdjAmount;
                    row["TotalOffer"] = item.TotalOffer;
                    row["ConcernName"] = item.ConcernName;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtOrder";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserConcernID);
                _reportParameter = new ReportParameter("Month", "Sales report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminMonthlyOrder.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] AdminCustomerDueRpt(string userName, int concernID, int UserCocernID, int CustomerType, int DueType)
        {
            try
            {
                var customerDueInfo = _customerService.AdminCustomerDueReport(concernID, CustomerType, DueType);

                TransactionalDataSet.dtCustomerDataTable dt = new TransactionalDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();
                DataRow row = null;
                foreach (var item in customerDueInfo)
                {
                    row = dt.NewRow();
                    row["CCode"] = item.Code;
                    row["CName"] = item.Name;
                    row["CompanyName"] = item.CompanyName;
                    row["CusType"] = item.CustomerType;
                    row["ContactNo"] = item.ContactNo;
                    row["NID"] = "";
                    row["Address"] = item.Address;
                    row["TotalDue"] = item.TotalDue;
                    row["ConcernName"] = item.ConcernName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCustomer";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserCocernID);
                if (DueType == 1)
                {
                    _reportParameter = new ReportParameter("ReportHeader", "Only Due Customer List.");
                    _reportParameters.Add(_reportParameter);
                }
                else
                {
                    _reportParameter = new ReportParameter("ReportHeader", "Customer List.");
                    _reportParameters.Add(_reportParameter);
                }


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] AdminCashCollectionReport(string userName, int concernID, int UserCocernID,
            DateTime fromDate, DateTime toDate, EnumCustomerType customerType, int customerID)
        {
            List<CashCollectionReportModel> collections = new List<CashCollectionReportModel>();
            try
            {
                if (customerType != EnumCustomerType.Hire)
                {
                    var CashColletions = _CashCollectionService.AdminCashCollectionReport(fromDate, toDate, concernID, customerType, customerID).ToList();
                    var BankCollection = _bankTransactionService.AdminCashCollectionByBank(concernID, fromDate, toDate, customerType, customerID).ToList();
                    collections.AddRange(CashColletions);
                    collections.AddRange(BankCollection);

                    var CashSales = _salesOrderService
                                    .GetAdminSalesReport(concernID, fromDate, toDate, customerType, customerID)
                                    .Where(i => i.RecAmount > 0);

                    var fcashsales = (from s in CashSales
                                      select new CashCollectionReportModel
                                      {
                                          EntryDate = s.InvoiceDate,
                                          CustomerName = s.CustomerName,
                                          CustomerCode = s.CustomerCode,
                                          Address = s.CustomerAddress,
                                          ContactNo = s.CustomerContactNo,
                                          TotalDue = s.CustomerTotalDue,
                                          Amount = s.RecAmount,
                                          AdjustAmt = s.AdjAmount,
                                          ModuleType = "Cash Sales",
                                          AccountNo = "N/A",
                                          ReceiptNo = s.InvoiceNo,
                                          Remarks = "",
                                          ConcernName = s.ConcernName,
                                          CustomerType = s.CustomerType
                                      }).ToList();

                    collections.AddRange(fcashsales);
                }

                if (customerType == 0 || customerType == EnumCustomerType.Hire)
                {

                    var DownPayments = _creditSalesOrderService.GetAdminCrSalesReport(concernID, fromDate, toDate)
                                        .Where(i => i.RecAmount > 0);
                    var fDownPayments = (from s in DownPayments
                                         select new CashCollectionReportModel
                                         {
                                             EntryDate = s.InvoiceDate,
                                             CustomerName = s.CustomerName,
                                             CustomerCode = s.CustomerCode,
                                             Address = s.CustomerAddress,
                                             ContactNo = s.CustomerContactNo,
                                             TotalDue = s.CustomerTotalDue,
                                             Amount = s.RecAmount,
                                             AdjustAmt = s.AdjAmount,
                                             ModuleType = "Downpayment",
                                             AccountNo = "N/A",
                                             ReceiptNo = s.InvoiceNo,
                                             Remarks = "",
                                             ConcernName = s.ConcernName,
                                             CustomerType = s.CustomerType
                                         }).ToList();

                    var installments = _creditSalesOrderService.AdminInstallmentColllections(concernID, fromDate, toDate);

                    collections.AddRange(fDownPayments);
                    collections.AddRange(installments);
                }

                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();
                DataRow row = null;
                var AllCollections = collections.OrderByDescending(i => i.EntryDate).ToList();
                foreach (var item in AllCollections)
                {
                    row = dt.NewRow();
                    row["CollDate"] = item.EntryDate;
                    row["CName"] = item.CustomerName + "\n" + item.ContactNo + " & " + item.Address;
                    row["CAddress"] = item.ContactNo + " & " + item.Address;
                    row["CContact"] = item.ContactNo;
                    row["TotalDue"] = item.TotalDue;
                    row["RecAmt"] = item.Amount;
                    row["RemainigAmt"] = item.TotalDue;
                    row["AdjustmentAmt"] = item.AdjustAmt;
                    row["CashType"] = item.ModuleType;
                    row["BankName"] = item.BankName;
                    row["AccountNo"] = item.AccountNo;
                    row["BranchName"] = item.BranchName;
                    row["ChequeNo"] = item.ChecqueNo;
                    row["EmployeeName"] = "";
                    row["InvoiceNo"] = item.ReceiptNo;
                    row["ConcernName"] = item.ConcernName;
                    row["CustomerType"] = item.CustomerType;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserCocernID);
                _reportParameter = new ReportParameter("Month", "Cash Collection from Date " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public byte[] CashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType, int filetype)
        {
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, ReportType, concernID, CustomerType).ToList();
            TransactionalDataSet.dtCashInHandDataTable dt = new TransactionalDataSet.dtCashInHandDataTable();
            _dataSet = new DataSet();
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double OpeningCashInhand = 0;
            double CurrentCashInhand = 0;
            double ClosingCashInhand = 0;

            var DataForTable = CashInHandData.Where(o => o.Expense != "Total Payable" && o.Income != "Total Receivable" && o.Expense != "Current Cash In Hand" && o.Income != "Closing Cash In Hand" && o.Income != "Opening Cash In Hand").ToList();
            var DataForTotal = CashInHandData.Where(o => o.Expense == "Total Payable" && o.Income == "Total Receivable").ToList();
            foreach (var item in DataForTable)
            {
                dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt, item.Module, item.EmployeeName);
            }

            dt.TableName = "dtCashInHand";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            TotalPayable = (double)DataForTotal.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)DataForTotal.Where(i => !(i.Expense.Equals("Header"))).Sum(o => o.IncomeAmt);
            OpeningCashInhand = (double)CashInHandData.Where(o => o.Income == "Opening Cash In Hand").ToList().Sum(o => o.IncomeAmt);
            CurrentCashInhand = (double)CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            ClosingCashInhand = (double)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInhand", OpeningCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CurrentCashInhand", CurrentCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
            {
                if (CustomerType == 0)
                    _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the date " + fromDate.ToString("dd MMM yyyy"));
                else
                    _reportParameter = new ReportParameter("DateRange", ((EnumSubCustomerType)CustomerType).ToString() + " Cash In Hand of the date " + fromDate.ToString("dd MMM yyyy"));


            }
            else if (ReportType == 2)
                _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the month  " + fromDate.ToString("MMM yyyy"));
            else if (ReportType == 3)
                _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the year  " + fromDate.ToString("yyyy"));

            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            //if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptKSDailyCashINHand.rdlc");
            //else
            //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptDailyCashINHand.rdlc");
            return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, "Others\\CashInHand\\rptDailyCashINHand.rdlc", filetype);

        }

        public byte[] BankTransMoneyReceipt(string userName, int concernID, int BankTranID)
        {
            try
            {
                string Code = string.Empty, Name = string.Empty, ContactNo = string.Empty;
                decimal TotalDue = 0;
                var BankTrans = _bankTransactionService.GetBankTransactionById(BankTranID);
                var BankInfo = _BankService.GetBankById(BankTrans.BankID);
                if (BankTrans.CustomerID != 0)
                {
                    var Customer = _customerService.GetCustomerById((int)BankTrans.CustomerID);
                    Code = Customer.Code;
                    Name = Customer.Name;
                    ContactNo = Customer.ContactNo + " & " + Customer.Address;
                    TotalDue = Customer.TotalDue;
                }
                else if (BankTrans.SupplierID != 0)
                {
                    var Supplier = _SupplierService.GetSupplierById((int)BankTrans.SupplierID);
                    Code = Supplier.Code;
                    Name = Supplier.Name;
                    ContactNo = Supplier.ContactNo + " & " + Supplier.Address;
                    TotalDue = Supplier.TotalDue;
                }

                TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
                _dataSet = new DataSet();
                DataRow row = null;


                row = dt.NewRow();

                row["TranDate"] = BankTrans.TranDate.Value.ToString("dd MMM yyyy");
                row["TransactionNo"] = BankTrans.TransactionNo;
                row["TransactionType"] = (EnumTransactionType)BankTrans.TransactionType;
                row["BankName"] = BankInfo.BankName;
                row["Amount"] = BankTrans.Amount;
                row["Remarks"] = BankTrans.Remarks;
                row["ChecqueNo"] = BankTrans.ChecqueNo;
                row["BranchName"] = BankInfo.BranchName;
                row["AccountNO"] = BankInfo.AccountNo;
                row["AccountName"] = BankInfo.AccountName;
                row["CustomerName"] = Name;
                row["CustomerCode"] = Code;
                row["TotalDue"] = TotalDue;
                row["ContactNo"] = ContactNo;
                dt.Rows.Add(row);
                dt.TableName = "BankTransaction";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBTransMoneyReceipt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ExpenseIncomeMoneyReceipt(string userName, int concernID, int ExpenditureID, bool IsExpense)
        {
            try
            {
                var expenditures = _expenditureService.GetExpenditureById(ExpenditureID);
                var head = _ExpenseItemService.GetExpenseItemById(expenditures.ExpenseItemID);
                string user = _userService.GetUserNameById(expenditures.CreatedBy);

                TransactionalDataSet.dtExpenditureDataTable dt = new TransactionalDataSet.dtExpenditureDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                row = dt.NewRow();

                row["ExpDate"] = expenditures.EntryDate.ToString("dd MMM yyyy");
                row["Description"] = expenditures.Purpose;
                row["Amount"] = expenditures.Amount;
                row["ItemName"] = head.Description;
                row["VoucherNo"] = expenditures.VoucherNo;
                row["UserName"] = expenditures.CreatedBy;
                dt.Rows.Add(row);
                dt.TableName = "dtExpenditure";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);
                if (IsExpense)
                    _reportParameter = new ReportParameter("ReportHeader", "Expense Money Receipt");
                else
                    _reportParameter = new ReportParameter("ReportHeader", "Income Money Receipt");
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptEXMoneyReceipt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] DailyAttendence(string userName, int concernID, int DepartmentID, DateTime Date, bool IsPresent, bool IsAbsent)
        {
            IQueryable<Department> departments = null;
            if (DepartmentID == 0)
                departments = _DepartmentService.GetAllDepartmentIQueryable().Where(i => i.Status == (int)EnumActiveInactive.Active);
            else
                departments = _DepartmentService.GetAllDepartmentIQueryable().Where(i => i.DepartmentId == DepartmentID);

            List<AttendencReportModel> AttendenceList = new List<AttendencReportModel>();
            int TotalEmployee = 0, PresentEmployee = 0, AbsentEmployee = 0;
            IQueryable<Employee> ActiveEmployees = _EmployeeService.GetAllEmployeeIQueryable().Where(i => i.Status == EnumActiveInactive.Active);
            if (IsPresent)
            {
                var Presents = (from da in _attendenceService.GetDetails().Where(i => i.Date == Date.Date)
                                join am in _attendenceService.GetAllIQueryable() on da.AttenMonthID equals am.AttenMonthID
                                join emp in ActiveEmployees on da.AccountNo equals emp.MachineEMPID
                                join dept in departments on emp.DepartmentID equals dept.DepartmentId
                                join desg in _DesignationService.GetAllIQueryable() on emp.DesignationID equals desg.DesignationID
                                where !string.IsNullOrEmpty(da.ClockIn)
                                select new AttendencReportModel
                                {
                                    AccountNo = emp.MachineEMPID,
                                    EmployeeName = emp.Name,
                                    Designation = desg.Description,
                                    DepartmentName = dept.DESCRIPTION,
                                    ClockIn = da.ClockIn,
                                    ClockOut = da.ClockOut,
                                    Absent = da.Absent,
                                    Late = da.Late
                                }).ToList();
                AttendenceList.AddRange(Presents);
                PresentEmployee = Presents.Count();
            }

            if (IsAbsent)
            {
                var absents = (from da in _attendenceService.GetDetails().Where(i => i.Date == Date.Date)
                               join am in _attendenceService.GetAllIQueryable() on da.AttenMonthID equals am.AttenMonthID
                               join emp in ActiveEmployees on da.AccountNo equals emp.MachineEMPID
                               join dept in departments on emp.DepartmentID equals dept.DepartmentId
                               join desg in _DesignationService.GetAllIQueryable() on emp.DesignationID equals desg.DesignationID
                               where string.IsNullOrEmpty(da.ClockIn)
                               select new AttendencReportModel
                               {
                                   AccountNo = emp.MachineEMPID,
                                   EmployeeName = emp.Name,
                                   Designation = desg.Description,
                                   DepartmentName = dept.DESCRIPTION,
                                   ClockIn = da.ClockIn,
                                   ClockOut = da.ClockOut,
                                   Absent = da.Absent,
                                   Late = da.Late
                               }).ToList();
                AttendenceList.AddRange(absents);
                AbsentEmployee = absents.Count();
            }
            TotalEmployee = PresentEmployee + AbsentEmployee;
            TransactionalDataSet.dtDailyAttendenceDataTable dt = new TransactionalDataSet.dtDailyAttendenceDataTable();

            _dataSet = new DataSet();
            DataRow row = null;
            AttendenceList = AttendenceList.OrderBy(i => i.AccountNo).ToList();
            foreach (var item in AttendenceList)
            {
                row = dt.NewRow();
                row["AccountNo"] = item.AccountNo;
                row["Name"] = item.EmployeeName;
                row["Designation"] = item.Designation;
                row["Department"] = item.DepartmentName;
                row["ClockIn"] = item.ClockIn;
                row["ClockOut"] = item.ClockOut;
                row["Late"] = item.Late;
                dt.Rows.Add(row);
            }

            _dataSet.Tables.Add(dt);
            dt.TableName = "dtDailyAttendence";

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReportHeading", "Attendence report of the date : " + Date.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalEmployee", "Total Employee: " + TotalEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Present", "Present: " + PresentEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Absent", "Absent: " + AbsentEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptDailyAttendence.rdlc");
        }


        public byte[] StockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName)
        {
            try
            {

                List<StockLedger> DataGroupBy = _StockServce.GetStockLedgerReport(reportType, CompanyName, CategoryName, ProductName, fromDate, toDate, concernID).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable();

                foreach (var item in DataGroupBy)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date;
                    row["ConcernID"] = item.ConcernID;
                    row["ProductID"] = item.ProductID;
                    row["Code"] = item.Code;
                    row["ProductName"] = item.ProductName;
                    row["ColorID"] = item.ColorID;
                    row["ColorName"] = item.ColorName;
                    row["OpeningStockQuantity"] = item.OpeningStockQuantity;
                    row["TotalStockQuantity"] = item.TotalStockQuantity;
                    row["PurchaseQuantity"] = item.PurchaseQuantity;
                    row["SalesQuantity"] = item.SalesQuantity;
                    row["SalesReturnQuantity"] = item.SalesReturnQuantity;
                    row["ClosingStockQuantity"] = item.ClosingStockQuantity;
                    row["OpeningStockValue"] = item.OpeningStockValue;
                    row["TotalStockValue"] = item.TotalStockValue;
                    row["ClosingStockValue"] = item.ClosingStockValue;
                    row["PurchaseReturn"] = item.PurchaseReturnQuantity;
                    row["TransferIN"] = item.TransferInQuantity;
                    row["TransferOUT"] = item.TransferOutQuantity;
                    row["RepQty"] = item.RepQty;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDailyStockandSalesSummary";


                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                reportName = "Stock\\rptStockLedger.rdlc";

                _reportParameter = new ReportParameter("DateRange", "Stock Ledger From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);



                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] SupplierLedger(string userName, int concernID, DateTime fromDate, DateTime toDate, int SupplierID, int ReportType)
        {

            TransactionalDataSet.dtCustomerLedgerAccountDataTable dt = new TransactionalDataSet.dtCustomerLedgerAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal LastBalance = 0m;
            int Counter = 0;
            var ledgers = _purchaseOrderService.SupplierLedger(fromDate, toDate, SupplierID);
            var Supplier = _SupplierService.GetSupplierById(SupplierID);
            foreach (var item in ledgers)
            {
                Counter++;
                row = dt.NewRow();
                row["Date"] = item.Date;
                row["Particulars"] = item.Particulars;
                row["VoucherType"] = item.VoucherType;
                row["InvoiceNo"] = item.InvoiceNo;
                row["Debit"] = item.Debit;
                row["DebitAdj"] = item.DebitAdj;
                row["Credit"] = item.Credit;
                row["CreditAdj"] = item.CreditAdj;
                row["CreditAdjFlatDis"] = item.CreditAdjFlatDis;
                row["CreditAdjTotalPPDis"] = item.CreditAdjTotalPPDis;
                row["GrandTotal"] = item.GrandTotal;
                row["CashCollection"] = item.CashCollectionAmt;
                row["SalesReturn"] = item.SalesReturn;
                row["Balance"] = item.Balance;
                row["InvoiceDue"] = item.InvoiceDue;
                row["Remarks"] = item.Remarks;
                row["CashCollectionIntAmt"] = item.CashCollectionIntAmt;
                row["TotalAmtWD"] = item.TotalAmtWD;
                row["TotalAdjDis"] = item.TotalAdjDis;

                if (ledgers.Count() == Counter)
                    LastBalance = item.Balance;

                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerLedgerAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CustomerName", Supplier.Name + " (" + Supplier.Code + ")");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerAddress", Supplier.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerContact", "Contact: " + Supplier.ContactNo);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("LastBalance", LastBalance.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptSupplierLedgerAccount.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptSupplierLedgerAccountSummary.rdlc");

        }


        public byte[] CustomerLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID, int ReportType)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, CustomerID);
            var Customer = _customerService.GetCustomerById(CustomerID);
            TransactionalDataSet.dtCustomerLedgerAccountDataTable dt = new TransactionalDataSet.dtCustomerLedgerAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal LastBalance = 0m;
            int Counter = 0;
            foreach (var item in customerLedgerdata)
            {
                Counter++;
                row = dt.NewRow();
                row["Date"] = item.Date;
                row["Particulars"] = item.Particulars;
                row["VoucherType"] = item.VoucherType;
                row["InvoiceNo"] = item.InvoiceNo;
                row["Debit"] = item.Debit;
                row["DebitAdj"] = item.DebitAdj;
                row["Credit"] = item.Credit;
                row["CreditAdj"] = item.CreditAdj;
                row["GrandTotal"] = item.GrandTotal;
                row["CashCollection"] = item.CashCollectionAmt;
                row["SalesReturn"] = item.SalesReturn;
                row["Balance"] = item.Balance;
                row["InvoiceDue"] = item.InvoiceDue;
                row["Remarks"] = item.Remarks;
                row["CashCollectionReturn"] = item.CashCollectionReturn;
                row["CashCollectionIntAmt"] = item.InterestAmt;
                row["CrInterestAmount"] = item.CrInterestAmount;
                row["CIntAmt"] = item.InterestAmt;
                row["TotalAmtWD"] = item.TotalAmtWD;
                row["TotalAdjDis"] = item.TotalAdjDis;

                if (customerLedgerdata.Count() == Counter)
                    LastBalance = item.Balance;

                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerLedgerAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CustomerName", Customer.Name + " (" + Customer.Code + ")");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerContact", "Contact: " + Customer.ContactNo);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("LastBalance", LastBalance.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedgerAccount.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedgerAccountSummary.rdlc");
        }

        public byte[] TransferInvoiceByID(int TransferID, string UserName, int ConcernID)
        {
            TransactionalDataSet.dtTransferDetailsDataTable dtTDetails = new TransactionalDataSet.dtTransferDetailsDataTable();
            _dataSet = new DataSet();
            var Transfer = _TransferService.GetById(TransferID);
            var ToConcern = _SisterConcernService.GetSisterConcernById(Transfer.ToConcernID);
            var FromConcern = _SisterConcernService.GetSisterConcernById(Transfer.FromConcernID);
            var Details = _TransferService.GetDetailsByID(TransferID);

            var GProducts = from d in Details
                            group d by new { d.ProductID, d.ProductName, d.ProductCode, d.ColorName, d.CategoryName, d.CompanyName, d.GodownName, d.MRP, d.TotalAmount, d.SalesRate } into g
                            select new
                            {
                                g.Key.ProductCode,
                                g.Key.ProductName,
                                g.Key.CategoryName,
                                g.Key.CompanyName,
                                g.Key.ColorName,
                                g.Key.GodownName,
                                g.Key.MRP,
                                TotalAmount = g.Key.MRP * g.Sum(i => i.Quantity),
                                IMEIs = g.Select(i => i.IMENO).ToList(),
                                Quantity = g.Sum(i => i.Quantity),
                                g.Key.SalesRate,
                                TotalSalesRate = g.Key.SalesRate * g.Sum(i => i.Quantity),

                            };

            DataRow row = null;
            string IMEI = string.Empty;
            foreach (var item in GProducts)
            {
                row = dtTDetails.NewRow();
                row["ProductCode"] = item.ProductCode;
                row["ProductName"] = item.ProductName + ", " + item.CategoryName;
                row["ColorName"] = item.ColorName;
                row["CategoryName"] = item.CategoryName;
                row["CompanyName"] = item.CompanyName;
                row["GodownName"] = item.GodownName;
                for (int i = 0; i < item.IMEIs.Count(); i++)
                {
                    if (i < item.IMEIs.Count() - 1)
                        IMEI = IMEI + item.IMEIs[i] + Environment.NewLine;
                    else
                        IMEI = IMEI + item.IMEIs[i];
                }
                row["IMEI"] = IMEI;
                row["PRate"] = item.MRP;
                row["Quantity"] = item.Quantity;
                row["TotalAmt"] = item.TotalAmount;
                row["SalesRate"] = item.SalesRate;
                row["TotalSalesRate"] = item.TotalSalesRate;
                dtTDetails.Rows.Add(row);
                IMEI = string.Empty;
            }
            dtTDetails.TableName = "dtTransferDetails";
            _dataSet.Tables.Add(dtTDetails);


            GetCommonParameters(UserName, ConcernID);


            _reportParameter = new ReportParameter("InvoiceNo", Transfer.TransferNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", Transfer.TransferDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Total", Transfer.TotalAmount.ToString("F"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FConcern", FromConcern.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TConcern", ToConcern.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", Transfer.Remarks);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReportHeader", "Transfer Order Invoice");
            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(ConcernID);
            if (currentSystemInfo.Name == "Maa Electronics (Head)" || currentSystemInfo.Name == "Maa Electronics(Nagar Bandar Branch)" ||
                currentSystemInfo.Name == "Maa Electronics (Mokamtola Branch)" || currentSystemInfo.Name == "Maa Electronics (Sonatola Branch)" ||
                currentSystemInfo.Name == "Maa Electronics (Dupchachia Branch)_____OLD" || currentSystemInfo.Name == "Maa Electronics (Gobindaganj Branch)" ||
                currentSystemInfo.Name == "AC World (Rangpur Branch)" || currentSystemInfo.Name == " Maa Electronics (Birampur Branch)" ||
                currentSystemInfo.Name == "Maa Electronics (Amtoly Branch)" || currentSystemInfo.Name == "Maa Electronics (Pirganj Branch)" ||
                currentSystemInfo.Name == "AC World (Bogura Branch)" || currentSystemInfo.Name == "AC World (Dupchachia Branch)" ||
                currentSystemInfo.Name == "Mother Electronics" || currentSystemInfo.Name == "Maa Electronics (Thana Branch)")
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Transfer\\METransferInvoice.rdlc");

            }
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Transfer\\TransferInvoice.rdlc");
        }

        public byte[] TransferReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int selectedConcernID)
        {
            TransactionalDataSet.dtTransferReportDataTable dtTransfer = new TransactionalDataSet.dtTransferReportDataTable();
            _dataSet = new DataSet();
            var Details = _TransferService.GetTransferReport(fromDate, toDate, selectedConcernID);

            var GProducts = from d in Details
                            group d by new
                            {
                                d.ChallanNo,
                                d.Date,
                                d.NetTotal,
                                d.ProductID,
                                d.ProductName,
                                d.ProductCode,
                                d.ColorName,
                                d.CategoryName,
                                d.CompanyName,
                                d.GodownName,
                                d.MRP,
                                d.TotalAmount,
                                d.FromGodownName
                            } into g
                            select new
                            {
                                TransferNo = g.Key.ChallanNo,
                                Date = g.Key.Date,
                                g.Key.NetTotal,
                                FromConcernName = g.Select(i => i.FromConcernName).FirstOrDefault(),
                                ToConcernName = g.Select(i => i.ToConcernName).FirstOrDefault(),
                                g.Key.ProductCode,
                                g.Key.ProductName,
                                g.Key.CategoryName,
                                g.Key.CompanyName,
                                g.Key.ColorName,
                                g.Key.GodownName,
                                g.Key.FromGodownName,
                                g.Key.MRP,
                                TotalAmount = g.Key.MRP * g.Sum(i => i.Quantity),
                                IMEIs = g.Select(i => i.IMENO).ToList(),
                                Quantity = g.Sum(i => i.Quantity)
                            };

            DataRow row = null;
            string IMEI = string.Empty;
            foreach (var item in GProducts)
            {
                row = dtTransfer.NewRow();
                row["TransferDate"] = item.Date.ToString("dd MMM yyyy");
                row["TransferNo"] = item.TransferNo;
                row["FromConcern"] = item.FromConcernName;
                row["ToConcern"] = item.ToConcernName;
                row["NetTotal"] = item.NetTotal;
                row["ProductCode"] = item.ProductCode;
                row["ProductName"] = item.ProductName + ", " + item.CategoryName;
                row["ColorName"] = item.ColorName;
                row["CategoryName"] = item.CategoryName;
                row["CompanyName"] = item.CompanyName;
                row["ToGodown"] = item.GodownName;
                for (int i = 0; i < item.IMEIs.Count(); i++)
                {
                    if (i < item.IMEIs.Count() - 1)
                        IMEI = IMEI + item.IMEIs[i] + Environment.NewLine;
                    else
                        IMEI = IMEI + item.IMEIs[i];
                }
                row["IMEI"] = IMEI;
                row["PRate"] = item.MRP;
                row["Quantity"] = item.Quantity;
                row["TotalAmt"] = item.TotalAmount;
                row["FromGodown"] = item.FromGodownName;
                dtTransfer.Rows.Add(row);
                IMEI = string.Empty;
            }

            dtTransfer.TableName = "dtTransferReport";
            _dataSet.Tables.Add(dtTransfer);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReportHeader", "Transfer Order From Date: " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Transfer\\TransferReport.rdlc");
        }

        public byte[] SMSReport(string UserName, int ConcernID, int Status, DateTime fromDate, DateTime toDate, bool isAdminReport, int selectedConcernID)
        {
            TransactionalDataSet.dtSMSStatusDataTable dt = new TransactionalDataSet.dtSMSStatusDataTable();
            _dataSet = new DataSet();
            var SMSs = _SMSService.GetAllReport(fromDate, toDate, Status, isAdminReport, selectedConcernID);
            DataRow row = null;
            foreach (var item in SMSs)
            {
                row = dt.NewRow();
                row["Code"] = item.Item2;
                row["Type"] = item.Rest.Item3;
                row["Status"] = item.Item5;
                row["NoOfSMS"] = item.Item4;
                row["CustomerCode"] = item.Item3;
                row["CustomerName"] = item.Item7;
                row["Date"] = item.Item1.ToString("dd MMM yyyy");
                row["ConcernName"] = item.Rest.Item5;
                row["SmsCharge"] = item.Rest.Item6 * item.Item4;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtSMSStatus";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(ConcernID);
            decimal previousBalance = smsAmountDetails.TotalRecAmt;

            _reportParameter = new ReportParameter("PresentBalance", previousBalance.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReportHeader", "SMS Report From Date " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            if (isAdminReport)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SMS\\rptAdminSMSReport.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SMS\\rptSMSReport.rdlc");
        }


        /// <summary>
        /// Date: 13-02-2020
        /// Author: aminul
        /// Reason: Customer Wise opening, closing due and sales ,collections
        /// </summary>
        public byte[] CustomerDueReportNew_Old(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int CustomerID, int IsOnlyDue, EnumCustomerType CustomerType, bool IsAdminReport, int SelectedConcernID)
        {
            var CustomersDue = _salesOrderService.CustomerDueReport(CustomerID, fromDate, toDate, SelectedConcernID, CustomerType, IsOnlyDue, IsAdminReport);

            TransactionalDataSet.dtCustomerDueReportDataTable dt = new TransactionalDataSet.dtCustomerDueReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in CustomersDue)
            {
                row = dt.NewRow();
                row["CustomerType"] = item.CustomerType;
                row["Customer"] = item.Code + " ," + item.CustomerName + " ," + item.ContactNo + " ," + item.Address;
                row["OpeningDue"] = item.OpeningDue;
                row["Sales"] = item.TotalSales;
                row["Collections"] = item.CashCollectionAmt;
                row["ClosingDue"] = item.ClosingDue;
                row["ReceiveAmt"] = item.ReceiveAmt + item.DownPayment;
                row["TotalCollection"] = item.TotalCollection;
                row["Return"] = item.Return;
                row["InstallmentCollection"] = item.InstallmentCollection;
                row["CashCollectionIntAmt"] = item.CashCollectionIntAmt;
                row["CrInterestAmt"] = item.CrInterestAmt;
                row["TotalAmt"] = item.OpeningDue + item.TotalSales + item.CashCollectionIntAmt + item.CrInterestAmt;


                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerDueReport";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("DateRange", "Customer Due Report  From Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptCustomerDueReport.rdlc");

        }

        public byte[] CustomerDueReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID,
            int CustomerID, int IsOnlyDue, EnumCustomerType CustomerType, bool IsAdminReport, int SelectedConcernID)
        {
            //var CustomersDue = _salesOrderService.CustomerDueReport(CustomerID, fromDate, toDate, SelectedConcernID, CustomerType, IsOnlyDue, IsAdminReport);

            var CustomersDue = _customerService.GetCustomerDateWiseTotalDue(CustomerID, concernID, fromDate, toDate, IsOnlyDue, CustomerType, SelectedConcernID);

            TransactionalDataSet.dtCustomerDueReportDataTable dt = new TransactionalDataSet.dtCustomerDueReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in CustomersDue)
            {
                row = dt.NewRow();
                row["CustomerType"] = item.CustomerType;
                row["Customer"] = item.CustomerAddress;
                row["OpeningDue"] = item.OpeningDue;
                row["Sales"] = item.Sales;
                row["Collections"] = item.CollectionAmt;
                row["ClosingDue"] = item.ClosingDue;
                row["ReceiveAmt"] = item.SalesReceive;
                row["TotalCollection"] = item.TotalCollection;
                row["Return"] = item.SaleReturn;
                row["InstallmentCollection"] = item.InstallmentCollection;
                row["CashCollectionIntAmt"] = item.CashCollectionInterestAmt;
                row["CrInterestAmt"] = item.HireIntestrestAmt;
                row["TotalAmt"] = item.TotalAmt;
                row["ConcernName"] = item.ConcernName;
                row["CashCollectionReturn"] = item.CollectionReturnAmt;
                row["CCTypeAdjustment"] = item.CashCollectionsTypeAdjustment;

                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerDueReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("DateRange", "Customer Due Report  From Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);


            if (IsAdminReport)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptCustomerDueReportNew.rdlc");
            }
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptCustomerDueReport.rdlc");

        }

        public byte[] GetSummaryReport(DateTime Date, int ConcernID, string userName)
        {
            TransactionalDataSet.dtSummaryReportNewDataTable dt = new TransactionalDataSet.dtSummaryReportNewDataTable();
            _dataSet = new DataSet();
            DateTime fromDate = Date;
            DateTime toDate = new DateTime(Date.Year, Date.Month, Date.Day, 23, 59, 59);
            var CustomersDue = _salesOrderService.CustomerDueReport(0, fromDate, toDate, ConcernID, 0, 0, false);

            #region Sales
            var Transactions = _salesOrderService.GetSummaryReport(Date, ConcernID);
            DataRow row = null;
            foreach (var item in Transactions)
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }

            decimal TotalCashSales = CustomersDue.Sum(i => i.CashReceiveAmt);
            decimal TotalCardPaidAmount = CustomersDue.Sum(i => i.CardPaidAmount);
            decimal TotalDueSales = CustomersDue.Sum(i => (i.PaymentDue));

            if (TotalCashSales > 0m)
            {
                row = dt.NewRow();
                row["Category"] = "Total Sales Summary";
                row["Head"] = "Total Cash Sales";
                row["Amount"] = TotalCashSales;
                dt.Rows.Add(row);
            }

            if (TotalCardPaidAmount > 0m)
            {
                row = dt.NewRow();
                row["Category"] = "Total Sales Summary";
                row["Head"] = "Total Bank/Card Sales";
                row["Amount"] = TotalCardPaidAmount;
                dt.Rows.Add(row);
            }

            if (TotalDueSales > 0m)
            {
                row = dt.NewRow();
                row["Category"] = "Total Sales Summary";
                row["Head"] = "Total Due Sales";
                row["Amount"] = TotalDueSales;
                dt.Rows.Add(row);
            }

            #endregion

            #region Cash collections
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, 1, ConcernID, 0).ToList();

            var DataForTable = CashInHandData.Where(o => o.Expense != "Total Payable" && o.Income != "Total Receivable" && o.Expense != "Current Cash In Hand" && o.Income != "Closing Cash In Hand" && o.Income != "Opening Cash In Hand" && o.Module != "Header").ToList();
            var DataForTotal = CashInHandData.Where(o => o.Expense == "Total Payable" && o.Income == "Total Receivable").ToList();

            decimal OpeningCashInhand = (decimal)CashInHandData.Where(o => o.Income == "Opening Cash In Hand").ToList().Sum(o => o.IncomeAmt);
            decimal CurrentCashInhand = (decimal)CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            decimal ClosingCashInhand = (decimal)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);


            SummaryReportModel oOpening = new SummaryReportModel()
            {
                Head = "Opening Cash In Hand",
                Amount = OpeningCashInhand,
                Category = "Opening Cash In Hand"
            };
            SummaryReportModel CurrentCash = new SummaryReportModel()
            {
                Head = "Current Cash In Hand",
                Amount = CurrentCashInhand,
                Category = "Current Cash In Hand"
            };
            SummaryReportModel Closing = new SummaryReportModel()
            {
                Head = "Closing Cash In Hand",
                Amount = ClosingCashInhand,
                Category = "Closing Cash In Hand"
            };
            var Incomes = (from d in DataForTable.Where(i => i.IncomeAmt > 0m)
                           group d by d.Module into g
                           select new SummaryReportModel
                           {
                               Head = g.Key,
                               Amount = g.Sum(i => i.IncomeAmt),
                               Category = "Total Collections"
                           }).ToList();

            Incomes.Insert(0, oOpening);
            foreach (var item in Incomes)
            {
                item.Head = (item.Head.Contains("1Order") || item.Head.Contains("3Order")) ? "Current Cash Sales(Retail)" : item.Head.Contains("2Order") ? "Current Cash Sales(Dealer)"
                    : item.Head.Equals("InstallmentCollection") ? "Previous collection (hire) "
                    : (item.Head.Equals("1Cash") || item.Head.Equals("3Cash")) ? "Previous collection(Retail)" : item.Head.Equals("2Cash") ? "Previous collection(Dealer)"
                    : item.Head.Equals("DownPayment") ? "Current sale (hire)" : item.Head;
            }

            foreach (var item in Incomes.OrderBy(i => i.Head))
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }


            #region expense e.g. cash delivery,bank delivery and direct expense
            var expenses = from d in DataForTable.Where(i => i.ExpenseAmt > 0m)
                           group d by d.Expense into g
                           select new SummaryReportModel
                           {
                               Head = g.Key,
                               Amount = g.Sum(i => i.ExpenseAmt),
                               Category = "Total Expenses"
                           };

            foreach (var item in expenses)
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }
            #endregion

            //Current Cash in hand
            row = dt.NewRow();
            row["Category"] = CurrentCash.Category;
            row["Head"] = CurrentCash.Head;
            row["Amount"] = CurrentCash.Amount;
            dt.Rows.Add(row);

            //Closing cash in hand
            row = dt.NewRow();
            row["Category"] = Closing.Category;
            row["Head"] = Closing.Head;
            row["Amount"] = Closing.Amount;
            dt.Rows.Add(row);

            #endregion

            #region Customer Due

            #region Previous Credit
            var PreviousDue = from d in CustomersDue
                              group d by d.CustomerType into g
                              select new SummaryReportModel
                              {
                                  Head = g.Key.ToString(),
                                  Amount = g.Sum(i => i.OpeningDue),
                                  Category = "Previous Credit"
                              };

            foreach (var item in PreviousDue)
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }
            #endregion

            #region Credit Realized
            var CreditRealized = Incomes.Where(i => i.Head.Contains("Previous")).ToList();
            foreach (var item in CreditRealized)
            {
                row = dt.NewRow();
                row["Category"] = "Credit Realized";
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }
            #endregion

            #region Current Credit
            var CurrentCredit = from d in CustomersDue
                                group d by d.CustomerType into g
                                select new SummaryReportModel
                                {
                                    Head = g.Key.ToString(),
                                    Amount = g.Sum(i => i.PaymentDue),
                                    Category = "Current Credit"
                                };

            foreach (var item in CurrentCredit)
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }
            #endregion

            #region Closing Credit
            var ClosingCredit = from d in CustomersDue
                                group d by d.CustomerType into g
                                select new SummaryReportModel
                                {
                                    Head = g.Key.ToString(),
                                    Amount = g.Sum(i => i.ClosingDue),
                                    Category = "Closing Credit"
                                };

            foreach (var item in ClosingCredit)
            {
                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                dt.Rows.Add(row);
            }
            #endregion

            #endregion

            dt.TableName = "dtSummaryReportNew";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Summary Report of the Date : " + Date.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptTransactionSummary.rdlc");

        }

        public byte[] GetTrialBalance(DateTime fromDate, DateTime toDate, string UserName, int ConcernID,
            string ClientDateTime, int selectedConcernID, bool IsAdminreport)
        {
            List<RPTTrialBalance> totaltrialBalance = new List<RPTTrialBalance>();
            List<RPTTrialBalance> trialBalances = new List<RPTTrialBalance>();
            if (IsAdminreport)
            {
                if (selectedConcernID > 0)
                {
                    trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, selectedConcernID).ToList();

                    if (trialBalances.Any())
                    {
                        var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, selectedConcernID, toDate);
                        if (cDueList != null && cDueList.Any())
                        {
                            decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Suspense Account")).FirstOrDefault().Credit -= totalDue;
                        }
                        totaltrialBalance.AddRange(trialBalances);
                    }
                    //if (trialBalances.Count() > 0)
                    //    totaltrialBalance.AddRange(trialBalances);
                }
                else
                {
                    var concerns = _SisterConcernService.GetFamilyTree(ConcernID);
                    foreach (var item in concerns)
                    {
                        trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, item.ConcernID).ToList();
                        if (trialBalances.Any())
                        {
                            var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, item.ConcernID, toDate);
                            if (cDueList != null && cDueList.Any())
                            {
                                decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                            }
                            totaltrialBalance.AddRange(trialBalances);
                        }
                        //if (trialBalances.Count() > 0)
                        //    totaltrialBalance.AddRange(trialBalances);
                    }
                }
            }
            else
            {
                trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, ConcernID).ToList();
                if (trialBalances.Any())
                {
                    var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, ConcernID, toDate);
                    if (cDueList != null && cDueList.Any())
                    {
                        decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Suspense Account")).FirstOrDefault().Credit -= totalDue;
                    }
                    totaltrialBalance.AddRange(trialBalances);
                }

            }

            TransactionalDataSet.dtTrialNewDataTable dt = new TransactionalDataSet.dtTrialNewDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            //foreach (var item in totaltrialBalance)
            //{
            //    row = dt.NewRow();
            //    row["Particulars"] = item.Particulars;
            //    row["Debit"] = item.Debit;
            //    row["Credit"] = item.Credit;
            //    dt.Rows.Add(row);
            //}

            foreach (var item in totaltrialBalance)
            {
                row = dt.NewRow();
                row["DebitParticular"] = item.DebitParticular;
                row["Debit"] = item.Debit.HasValue ? item.Debit.Value : 0m;
                row["CreditParticular"] = item.CreditParticular;
                row["Credit"] = item.Credit.HasValue ? item.Credit.Value : 0m;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtTrialNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Trial Balance of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptTrialBalance.rdlc");
        }

        public byte[] ProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _AccountingService.ProfitLossAccount(fromDate, toDate, ConcernID).ToList();
            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal debitAmt = data.Where(i => !((i.DebitParticulars.Equals("Purchase Return"))
                            || (i.DebitParticulars.Equals("Purchase"))
                            || (i.DebitParticulars.Equals("Total Expense"))
                            )).Sum(i => i.Debit);

            decimal creditAmt = data.Where(i => !((i.CreditParticulars.Equals("Gross Profit"))
                || (i.CreditParticulars.Equals("Sales"))
                || (i.CreditParticulars.Equals("Sales Return"))
                || (i.CreditParticulars.Equals("Total Income"))
                )).Sum(i => i.Credit);
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Profit and Loss Account of the date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", debitAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", creditAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetProfit", (creditAmt - debitAmt).ToString());
            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(ConcernID);
            if (ConcernID == (int)EnumSisterConcern.Beauty_2 || ConcernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptProfitLossAccount.rdlc");
        }
        public byte[] BalanceSheet(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _AccountingService.BalanceSheet(fromDate, toDate, ConcernID).ToList();
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, 2, ConcernID, 0).ToList();
            decimal ClosingCashInhand = CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            //decimal debitAmt = data.Sum(i => i.Debit);
            //decimal creditAmt = data.Where(i => !(i.CreditParticulars.Equals("Gross Profit"))).Sum(i => i.Credit);
            var debits = data.Where(i => !string.IsNullOrEmpty(i.DebitParticulars)).ToList();
            debits.Insert(0, new ProfitLossReportModel() { DebitParticulars = "Cash In Hand", Debit = ClosingCashInhand, CreditParticulars = "", Credit = 0m, SerialNumber = 1 });
            var credits = data.Where(i => !string.IsNullOrEmpty(i.CreditParticulars)).ToList();
            //var totalCA = debits.FirstOrDefault(i => i.DebitParticulars.Equals("Total Current Assets"));
            //if (totalCA != null)
            //    totalCA.Debit += ClosingCashInhand;

            List<ProfitLossReportModel> finalData = new List<ProfitLossReportModel>();
            ProfitLossReportModel bs = null;

            decimal TotalDebit = debits.Where(i => i.DebitParticulars.Contains("Total")).Sum(i => i.Debit);
            TotalDebit += ClosingCashInhand;

            decimal TotalCredit = credits.Where(i => i.CreditParticulars.Contains("Total")).Sum(i => i.Credit);
            decimal ownersEquity = TotalDebit - TotalCredit;
            credits.Add(new ProfitLossReportModel() { DebitParticulars = "", Debit = 0, CreditParticulars = "Owner's Equity", Credit = ownersEquity });
            TotalCredit += ownersEquity;
            if (debits.Count() > credits.Count())
            {
                for (int i = 0; i < debits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.DebitParticulars = debits[i].DebitParticulars;
                    bs.Debit = debits[i].Debit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < credits.Count(); i++)
                {
                    finalData[i].Credit = credits[i].Credit;
                    finalData[i].CreditParticulars = credits[i].CreditParticulars;
                }
            }
            else
            {
                for (int i = 0; i < credits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.CreditParticulars = credits[i].CreditParticulars;
                    bs.Credit = credits[i].Credit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < debits.Count(); i++)
                {
                    finalData[i].Debit = debits[i].Debit;
                    finalData[i].DebitParticulars = debits[i].DebitParticulars;
                }
            }

            foreach (var item in finalData)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Balance Sheet of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", TotalDebit.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", TotalCredit.ToString());
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("NetProfit", (creditAmt-debitAmt).ToString());
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptBalanceSheet.rdlc");
        }


        public byte[] BalanceSheetNew_Old(DateTime asOnDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _AccountingService.BalanceSheetNew(asOnDate, ConcernID);
            //var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, 2, ConcernID, 0).ToList();

            //decimal ClosingCashInhand = CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            //decimal debitAmt = data.Sum(i => i.Debit);
            //decimal creditAmt = data.Where(i => !(i.CreditParticulars.Equals("Gross Profit"))).Sum(i => i.Credit);
            var debits = data.Where(i => !string.IsNullOrEmpty(i.DebitParticulars)).ToList();
            //debits.Insert(0, new ProfitLossReportModel() { DebitParticulars = "Cash In Hand", Debit = ClosingCashInhand, CreditParticulars = "", Credit = 0m, SerialNumber = 1 });
            var credits = data.Where(i => !string.IsNullOrEmpty(i.CreditParticulars)).ToList();
            //var totalCA = debits.FirstOrDefault(i => i.DebitParticulars.Equals("Total Current Assets"));
            //if (totalCA != null)
            //    totalCA.Debit += ClosingCashInhand;

            List<ProfitLossReportModel> finalData = new List<ProfitLossReportModel>();
            ProfitLossReportModel bs = null;

            decimal TotalDebit = debits.Where(i => i.DebitParticulars.Contains("Total")).Sum(i => i.Debit);
            TotalDebit += debits.Where(d => d.DebitParticulars.ToLower().Equals("all fixed asset")).Sum(d => d.Debit);
            //TotalDebit += ClosingCashInhand;

            decimal TotalCredit = credits.Where(i => i.CreditParticulars.Contains("Total")).Sum(i => i.Credit);
            decimal ownersEquity = TotalDebit - TotalCredit;
            credits.Add(new ProfitLossReportModel() { DebitParticulars = "", Debit = 0, CreditParticulars = "Owner's Equity", Credit = ownersEquity });
            TotalCredit += ownersEquity;
            if (debits.Count() > credits.Count())
            {
                for (int i = 0; i < debits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.DebitParticulars = debits[i].DebitParticulars;
                    bs.Debit = debits[i].Debit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < credits.Count(); i++)
                {
                    finalData[i].Credit = credits[i].Credit;
                    finalData[i].CreditParticulars = credits[i].CreditParticulars;
                }
            }
            else
            {
                for (int i = 0; i < credits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.CreditParticulars = credits[i].CreditParticulars;
                    bs.Credit = credits[i].Credit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < debits.Count(); i++)
                {
                    finalData[i].Debit = debits[i].Debit;
                    finalData[i].DebitParticulars = debits[i].DebitParticulars;
                }
            }

            foreach (var item in finalData)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Balance Sheet of the date " + asOnDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", TotalDebit.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", TotalCredit.ToString());
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("NetProfit", (creditAmt-debitAmt).ToString());
            //_reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(ConcernID);
            if (ConcernID == (int)EnumSisterConcern.Beauty_2 || ConcernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptBalanceSheetNew.rdlc");
        }

        public byte[] BalanceSheetNew(DateTime asOnDate, string UserName, int ConcernID,
            string ClientDateTime, int selectedConcernID, bool IsAdminreport)
        {
            List<RPTTrialBalance> totaltrialBalance = new List<RPTTrialBalance>();
            List<RPTTrialBalance> trialBalances = new List<RPTTrialBalance>();
            if (IsAdminreport)
            {
                if (selectedConcernID > 0)
                {
                    trialBalances = _AccountingService.GetBalanceSheet(asOnDate, selectedConcernID).ToList();

                    if (trialBalances.Any())
                    {
                        var stockvalue = _StockServce.GetStockLedgerReport(2, "", "", "", asOnDate, asOnDate, selectedConcernID);
                        var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, selectedConcernID, asOnDate);
                        if (cDueList != null && cDueList.Any())
                        {
                            decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                            decimal totalvalue = stockvalue.Sum(d => d.ClosingStockValue);
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Account Receivable")).FirstOrDefault().Credit = totalDue;
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Inventory")).FirstOrDefault().Credit = totalvalue;
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.DebitParticular) && d.DebitParticular.Equals("Owner's Equity")).FirstOrDefault().Debit += totalDue + totalvalue;
                        }
                        totaltrialBalance.AddRange(trialBalances);
                    }
                    //if (trialBalances.Count() > 0)
                    //    totaltrialBalance.AddRange(trialBalances);
                }
                else
                {
                    var concerns = _SisterConcernService.GetFamilyTree(ConcernID);

                    //List<RPTCustomerDueDateWise> customerDateDueBetween = new List<RPTCustomerDueDateWise>();
                    foreach (var item in concerns)
                    {
                        trialBalances = _AccountingService.GetBalanceSheet(asOnDate, item.ConcernID).ToList();
                        if (trialBalances.Any())
                        {
                            var stockvalue = _StockServce.GetStockLedgerReport(2, "", "", "", asOnDate, asOnDate, item.ConcernID);
                            var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, item.ConcernID, asOnDate);

                            if (cDueList != null && cDueList.Any())
                            {
                                decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                                decimal totalvalue = stockvalue.Sum(d => d.ClosingStockValue);
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Account Receivable")).FirstOrDefault().Credit = totalDue;
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Inventory")).FirstOrDefault().Credit = totalvalue;
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.DebitParticular) && d.DebitParticular.Equals("Owner's Equity")).FirstOrDefault().Debit += totalDue + totalvalue;
                            }

                            foreach (var balance in trialBalances)
                            {
                                balance.ConcernName = item.Name;
                            }
                            totaltrialBalance.AddRange(trialBalances);
                            trialBalances = null;
                        }


                    }
                }
            }
            else
            {
                trialBalances = _AccountingService.GetBalanceSheet(asOnDate, ConcernID).ToList();
                if (trialBalances.Any())
                {
                    var stockvalue = _StockServce.GetStockLedgerReport(2, "", "", "", asOnDate, asOnDate, ConcernID);
                    var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, ConcernID, asOnDate);
                    if (cDueList != null && cDueList.Any())
                    {
                        decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                        decimal totalvalue = stockvalue.Sum(d => d.ClosingStockValue);
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Account Receivable")).FirstOrDefault().Credit = totalDue;
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Inventory")).FirstOrDefault().Credit = totalvalue;
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.DebitParticular) && d.DebitParticular.Equals("Owner's Equity")).FirstOrDefault().Debit += totalDue + totalvalue;
                    }
                    totaltrialBalance.AddRange(trialBalances);
                }
                //if (trialBalances.Any())
                //{
                //    var stockvalue = _StockServce.GetStockLedgerReport(2, "", "", "", asOnDate, asOnDate, ConcernID);
                //    if (stockvalue != null && stockvalue.Any())
                //    {
                //        decimal totalvalue= stockvalue.Sum(d => d.ClosingStockValue);
                //        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Inventory")).FirstOrDefault().Credit = totalvalue;
                //        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Owner's Equity")).FirstOrDefault().Credit -= totalvalue;
                //    }
                //    totaltrialBalance.AddRange(trialBalances);
                //}

            }

            TransactionalDataSet.dtTrialNewDataTable dt = new TransactionalDataSet.dtTrialNewDataTable();
            _dataSet = new DataSet();
            DataRow row = null;


            foreach (var item in totaltrialBalance)
            {
                //sumCreditPart = sumCreditPart + item.Credit.HasValue ? item.Credit.Value : 0m;
                //sumCreditPart += (item.Credit.HasValue ? item.Credit.Value : 0m);

                row = dt.NewRow();
                row["DebitParticular"] = item.DebitParticular;
                row["Debit"] = item.Debit.HasValue ? item.Debit.Value : 0m;
                row["CreditParticular"] = item.CreditParticular;
                row["Credit"] = item.Credit.HasValue ? item.Credit.Value : 0m;
                row["IsHeader"] = item.IsHeader;
                row["IsCrHeader"] = item.IsCrHeader;
                row["ConcernName"] = item.ConcernName;
                //row["SumCredit"] = sumCreditPart;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtTrialNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Balance Sheet of the date " + asOnDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            if (selectedConcernID > 0)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptBalanceSheetFinal.rdlc");
            }

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptAdminBalanceSheetFinal.rdlc");
        }

        public byte[] AdminStockSummaryReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforAdminStockReport(userName, concernID, reportType, CompanyName, CategoryName, ProductName, UserConcernID).ToList();
                //var InhouseDamageProductDetails = _productService.GetAllDamageProductFromDetailForAdmin(concernID);
                //var CompanyDamageProductDetails = _purchaseOrderService.GetDamageReturnProductDetailsForAdmin(0, 0, concernID);
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();
                if (reportType == 0)//Product Wise
                {

                    bool IsHistoryShow = false;
                    if ((concernID != 1 || concernID != 5 || concernID != 6) && reportType == 0)
                        IsHistoryShow = true;
                    foreach (var item in stockInfos)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.Item2;
                        row["CompanyName"] = item.Item3;
                        row["CategoryName"] = item.Item4;
                        row["ModelName"] = item.Item5;
                        row["Quantity"] = item.Item6;
                        row["PRate"] = item.Item7;
                        row["TotalPrice"] = (item.Item6 * item.Rest.Item1);
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.Rest.Item1;
                        row["CreditSRate"] = item.Rest.Item2;// 6 months
                        row["TotalCreditPrice"] = (item.Item6 * item.Rest.Item2);
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.Rest.Item3;
                        row["TotalCrSR3"] = (item.Item6 * item.Rest.Item3);
                        row["CrSR12"] = item.Rest.Item4;
                        row["TotalCrSR12"] = (item.Item6 * item.Rest.Item4);
                        row["ConcernName"] = item.Rest.Item5;
                        //if (IsHistoryShow)
                        //{
                        //    row["StockHistory"] = _StockServce.GetStockProductsHistory(item.Item1);
                        //}
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                else if (reportType == 1) //Company wise
                {
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Company = ns.Item3, Category = ns.Item4, Concern = ns.Rest.Item5 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            CompanyName = g.Key.Company,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            ConcenName = g.Key.Concern
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = 0;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["ConcernName"] = item.ConcenName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                else if (reportType == 2) //category wise
                {
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Category = ns.Item4, concern = ns.Rest.Item5 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            ConcenName = g.Key.concern
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = 0;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["ConcernName"] = item.ConcenName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, UserConcernID);
                if (reportType == 0)
                {
                    reportName = "Stock\\StockSummaryInfoForCredit.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStockSummaryForCredit.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStockSummaryForCredit.rdlc";
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Date: 22-07-2020
        /// Author: aminul
        /// </summary>
        /// <returns></returns>
        public byte[] HireAccountDetails(DateTime fromDate, DateTime toDate, string UserName, int ConcernID)
        {
            var hireSales = _creditSalesOrderService.HireAccountDetails(fromDate, toDate, ConcernID);

            TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            string ProductName = string.Empty;
            int Counter = 0;
            foreach (var item in hireSales.RunningAccountList)
            {
                row = dt.NewRow();
                row["SalesDate"] = item.Date;
                row["InvoiceNo"] = item.InvoiceNo;
                foreach (var p in item.ProductList)
                {
                    Counter++;
                    if (Counter == item.ProductList.Count())
                        ProductName = ProductName + p;
                    else
                        ProductName = ProductName + Environment.NewLine + p;
                }
                Counter = 0;
                row["ProductName"] = ProductName;
                ProductName = string.Empty;

                row["CName"] = item.CustomerCode + ", " + item.CustomerName + ", " + item.Address + ", " + item.ContactNo;
                row["SalesPrice"] = item.SalesPrice;
                row["NetAmt"] = 0m;
                row["GrandTotal"] = 0m;
                row["TotalDis"] = 0m;
                row["NetTotal"] = 0m;
                row["PaidAmount"] = 0m;
                row["RemainingAmt"] = item.RemainingAmt;
                row["Quantity"] = item.NoOfInstallment;
                row["IMENo"] = item.RefName;
                row["ColorInfo"] = string.Empty;
                row["SalesType"] = string.Empty;
                row["AdjAmount"] = 0m;
                //row["POMRP"] = 0m;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerWiseSales";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Hire Account Details Report of the Month : " + fromDate.ToString("MMMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningAcc", hireSales.OpeningAccount.ToString("F0"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingAcc", hireSales.ClosingAccount.ToString("F0"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RunningAcc", hireSales.RunningAccount.ToString("F0"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptHireAccountDetails.rdlc");

        }



        public byte[] MonthlyTransactionReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.MonthlyAdminTransactionReport(fromDate, toDate, concernID);

            TransactionalDataSet.dtTransactionDataTable dt = new TransactionalDataSet.dtTransactionDataTable();
            _dataSet = new DataSet();
            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;



            foreach (var item in data)
            {
                if ((item.RetailSale + item.HireSale + item.DealerCollection + item.TotalSale + item.DownPayment + item.HireCollection + item.DealerCollection +
                   item.TotalCollection + item.DailyExpense + item.CompanyPayment + Math.Abs(item.Balance)) != 0 + item.DailyIncome + item.BankWithdrowAmount)

                    dt.Rows.Add(item.EntryDate,
                        item.RetailSale,
                        item.HireSale,
                        item.DealerSale,
                        item.TotalSale,
                        item.RetailCash,
                        item.DownPayment,
                        item.HireCollection,
                        item.DealerCollection,
                        item.TotalCollection,
                        item.DailyExpense,
                        item.CompanyPayment,
                        item.Balance,
                        item.CumulativeBalance,
                        item.BankDepositeAmount,
                        item.DailyIncome,
                        item.BankWithdrowAmount);
            }

            dt.TableName = "dtMonthlyTransaction";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptMonthlyTransaction.rdlc");

        }

        public byte[] LiabilityReport_Old(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int HeadID, bool OnlyHead)
        {
            IQueryable<ShareInvestment> shareInvestments = null;
            IQueryable<BankTransaction> bankTransactions = null;
            List<LiabilityReportModel> RecPaydata = new List<LiabilityReportModel>();
            LiabilityReportModel oRecPay = null;

            decimal TotalRec = 0m, TotalPay = 0m, headRec = 0m, headPay = 0m;
            if (OnlyHead)
            {
                if (HeadID > 0)
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.SIHID == HeadID);


                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            else
            {
                if (HeadID > 0)
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate && i.SIHID == HeadID);
                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.TranDate >= fromDate && i.TranDate <= toDate && i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll().Where(i => i.TranDate >= fromDate && i.TranDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            var lReceive = (from si in shareInvestments
                            join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                            where si.TransactionType == EnumInvestTransType.Receive
                            select new LiabilityReportModel
                            {
                                HeadID = h.SIHID,
                                RecDate = si.EntryDate,
                                ReceiveAmt = si.Amount,
                                HeadName = h.Name,
                                RecLiabilityType = si.LiabilityType,
                                RecPurpose = si.Purpose,
                                Status = si.CashInHandReportStatus
                            }).OrderBy(i => i.RecDate).ToList();

            if (lReceive.Count() > 0)
                RecPaydata.AddRange(lReceive);

            var lPay = (from si in shareInvestments
                        join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                        where si.TransactionType == EnumInvestTransType.Pay
                        select new LiabilityReportModel
                        {
                            HeadID = h.SIHID,
                            RecDate = si.EntryDate,
                            PayAmt = si.Amount,
                            HeadName = h.Name,
                            PayLiabilityType = si.LiabilityType,
                            PayPurpose = si.Purpose,
                            Status = si.CashInHandReportStatus
                        }).OrderBy(i => i.RecDate).ToList();

            if (lPay.Count() > 0)
                RecPaydata.AddRange(lPay);

            var BlReceive = (from b in bankTransactions
                             join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                             where b.TransactionType == (int)EnumTransactionType.LiaRec
                             select new LiabilityReportModel
                             {
                                 HeadID = h.SIHID,
                                 RecDate = b.TranDate.Value,
                                 ReceiveAmt = b.Amount,
                                 HeadName = h.Name,
                                 BankLiaRecType = b.TransactionType,
                                 RecPurpose = b.Remarks,
                             }).OrderBy(i => i.RecDate).ToList();

            if (BlReceive.Count() > 0)
                RecPaydata.AddRange(BlReceive);

            var BlPay = (from b in bankTransactions
                         join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                         where b.TransactionType == (int)EnumTransactionType.LiaPay
                         select new LiabilityReportModel
                         {
                             HeadID = h.SIHID,
                             RecDate = b.TranDate.Value,
                             PayAmt = b.Amount,
                             HeadName = h.Name,
                             BankLiaPayType = b.TransactionType,
                             RecPurpose = b.Remarks,
                         }).OrderBy(i => i.RecDate).ToList();

            if (BlPay.Count() > 0)
                RecPaydata.AddRange(BlPay);


            var finalData = (from l in RecPaydata
                             group l by new
                             {
                                 l.HeadID,
                                 l.HeadName,
                                 l.RecDate,
                                 //l.PayDate,
                                 l.RecPurpose,
                                 l.RecLiabilityType,
                                 l.PayPurpose,
                                 l.PayLiabilityType,
                                 l.BankLiaRecType,
                                 l.BankLiaPayType,
                                 l.Status

                             } into g
                             select new LiabilityReportModel
                             {
                                 HeadID = g.Key.HeadID,
                                 HeadName = g.Key.HeadName,
                                 RecDate = g.Key.RecDate,
                                 ReceiveAmt = g.Sum(i => i.ReceiveAmt),
                                 RecPurpose = g.Key.RecPurpose,
                                 RecLiabilityType = g.Key.RecLiabilityType,

                                 //PayDate = g.Key.PayDate,
                                 PayAmt = g.Sum(i => i.PayAmt),
                                 PayPurpose = g.Key.PayPurpose,
                                 PayLiabilityType = g.Key.PayLiabilityType,
                                 BankLiaRecType = g.Key.BankLiaRecType,
                                 BankLiaPayType = g.Key.BankLiaPayType,
                                 Status = g.Key.Status

                             }).OrderBy(i => i.HeadID).ThenBy(i => i.RecDate);
            //if (lPay.Count() > lReceive.Count())
            //{
            //    foreach (var item in lPay)
            //    {
            //        oRecPay = new LiabilityReportModel();
            //        oRecPay.PayDate = item.EntryDate;
            //        oRecPay.PayAmt = item.Amount;
            //        oRecPay.HeadName = item.Name;
            //        oRecPay.PayType = item.LiabilityType.GetDisplayName();
            //        oRecPay.PayPurpose = item.PayPurpose;
            //        RecPaydata.Add(oRecPay);
            //    }

            //    for (int i = 0; i < lReceive.Count(); i++)
            //    {
            //        RecPaydata[i].RecDate = lReceive[i].EntryDate;
            //        RecPaydata[i].ReceiveAmt = lReceive[i].Amount;
            //        RecPaydata[i].HeadName = lReceive[i].Name;
            //        RecPaydata[i].RecType = lReceive[i].LiabilityType.GetDisplayName();
            //        RecPaydata[i].RecPurpose = lReceive[i].RecPurpose;
            //    }
            //}
            //else
            //{
            //    foreach (var item in lReceive)
            //    {
            //        oRecPay = new LiabilityReportModel();
            //        oRecPay.RecDate = item.EntryDate;
            //        oRecPay.ReceiveAmt = item.Amount;
            //        oRecPay.HeadName = item.Name;
            //        oRecPay.RecType = item.LiabilityType.GetDisplayName();
            //        oRecPay.RecPurpose = item.RecPurpose;

            //        RecPaydata.Add(oRecPay);
            //    }

            //    for (int i = 0; i < lPay.Count(); i++)
            //    {
            //        RecPaydata[i].PayDate = lPay[i].EntryDate;
            //        RecPaydata[i].PayAmt = lPay[i].Amount;
            //        RecPaydata[i].HeadName = lPay[i].Name;
            //        RecPaydata[i].PayType = lPay[i].LiabilityType.GetDisplayName();
            //        RecPaydata[i].PayPurpose = lPay[i].PayPurpose;
            //    }
            //}

            TransactionalDataSet.dtLiabiltyReportDataTable dt = new TransactionalDataSet.dtLiabiltyReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            int lHeadID = 0;
            foreach (var item in finalData)
            {
                if (item.HeadID != lHeadID)
                {
                    lHeadID = item.HeadID;
                    headRec = 0m;
                    headPay = 0m;
                }
                row = dt.NewRow();
                row["RecDate"] = item.RecDate == DateTime.MinValue || item.ReceiveAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["RecAmt"] = item.ReceiveAmt;
                row["PayDate"] = item.RecDate == DateTime.MinValue || item.PayAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["PayAmt"] = item.PayAmt;
                row["RecType"] = item.RecLiabilityType != 0 ? item.RecLiabilityType.GetDisplayName() : "";
                row["PayType"] = item.PayLiabilityType != 0 ? item.PayLiabilityType.GetDisplayName() : "";
                row["RecPurpose"] = item.RecPurpose;
                row["PayPurpose"] = item.PayPurpose;
                row["HeadName"] = item.HeadName;
                TotalRec += item.ReceiveAmt;
                TotalPay += item.PayAmt;

                headRec += item.ReceiveAmt;
                headPay += item.PayAmt;
                row["Balance"] = headRec - headPay;

                if (item.BankLiaPayType == 8 || item.BankLiaRecType == 9)
                {
                    row["Status"] = "Don't Show";
                }
                else
                {
                    if (item.Status == 0)
                    {
                        row["Status"] = "Show";
                    }
                    else
                    {
                        row["Status"] = "Don't Show";
                    }
                }

                dt.Rows.Add(row);
            }

            dt.TableName = "dtLiabiltyReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            if (OnlyHead)
                _reportParameter = new ReportParameter("DateRange", "Liabilty Report");
            else
                _reportParameter = new ReportParameter("DateRange", "Liabilty report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RemainingAmt", (TotalRec - TotalPay).ToString("F2"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Investment\\rptLiabilityRpt.rdlc");
        }

        public byte[] LiabilityReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int HeadID, bool OnlyHead)
        {
            IQueryable<ShareInvestment> shareInvestments = null;
            IQueryable<BankTransaction> bankTransactions = null;
            List<LiabilityReportModel> RecPaydata = new List<LiabilityReportModel>();
            List<LiabilityReportModel> prevRecPaydata = new List<LiabilityReportModel>();
            LiabilityReportModel oRecPay = null;
            decimal headOpeningAmount = 0m;
            ShareInvestmentHead shareInvestmentHead = _ShareInvestmentHeadService.GetById(HeadID);


            decimal TotalRec = 0m, TotalPay = 0m, headRec = 0m, headPay = 0m;
            if (OnlyHead)
            {
                if (HeadID > 0)
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.SIHID == HeadID);


                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            else
            {
                if (HeadID > 0)
                {
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.EntryDate <= toDate && i.SIHID == HeadID);
                    headOpeningAmount = shareInvestmentHead != null ? shareInvestmentHead.OpeningBalance : 0m;
                    if (shareInvestmentHead != null)
                    {
                        headOpeningAmount = (!string.IsNullOrEmpty(shareInvestmentHead.OpeningType) && shareInvestmentHead.OpeningType.Equals(EnumOpeningType.Payment.ToString())) ? -(shareInvestmentHead.OpeningBalance) : shareInvestmentHead.OpeningBalance;
                    }
                }

                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll().Where(i => i.EntryDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.TranDate <= toDate && i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll().Where(i => i.TranDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            var PrevLReceive = (from si in shareInvestments.Where(d => d.EntryDate < fromDate)
                                join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                                where si.TransactionType == EnumInvestTransType.Receive
                                select new LiabilityReportModel
                                {
                                    HeadID = h.SIHID,
                                    RecDate = si.EntryDate,
                                    ReceiveAmt = si.Amount,
                                    HeadName = h.Name,
                                    RecLiabilityType = si.LiabilityType,
                                    RecPurpose = si.Purpose,
                                    Status = si.CashInHandReportStatus
                                }).OrderBy(i => i.RecDate).ToList();

            var lReceive = (from si in shareInvestments.Where(d => d.EntryDate >= fromDate)
                            join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                            where si.TransactionType == EnumInvestTransType.Receive
                            select new LiabilityReportModel
                            {
                                HeadID = h.SIHID,
                                RecDate = si.EntryDate,
                                ReceiveAmt = si.Amount,
                                HeadName = h.Name,
                                RecLiabilityType = si.LiabilityType,
                                RecPurpose = si.Purpose,
                                Status = si.CashInHandReportStatus
                            }).OrderBy(i => i.RecDate).ToList();

            if (lReceive.Count() > 0)
                RecPaydata.AddRange(lReceive);

            if (PrevLReceive.Any())
                prevRecPaydata.AddRange(PrevLReceive);


            var prevLPay = (from si in shareInvestments.Where(d => d.EntryDate < fromDate)
                            join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                            where si.TransactionType == EnumInvestTransType.Pay
                            select new LiabilityReportModel
                            {
                                HeadID = h.SIHID,
                                RecDate = si.EntryDate,
                                PayAmt = si.Amount,
                                HeadName = h.Name,
                                PayLiabilityType = si.LiabilityType,
                                PayPurpose = si.Purpose,
                                Status = si.CashInHandReportStatus
                            }).OrderBy(i => i.RecDate).ToList();

            var lPay = (from si in shareInvestments.Where(d => d.EntryDate >= fromDate)
                        join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                        where si.TransactionType == EnumInvestTransType.Pay
                        select new LiabilityReportModel
                        {
                            HeadID = h.SIHID,
                            RecDate = si.EntryDate,
                            PayAmt = si.Amount,
                            HeadName = h.Name,
                            PayLiabilityType = si.LiabilityType,
                            PayPurpose = si.Purpose,
                            Status = si.CashInHandReportStatus
                        }).OrderBy(i => i.RecDate).ToList();

            if (lPay.Any())
                RecPaydata.AddRange(lPay);

            if (prevLPay.Any())
                prevRecPaydata.AddRange(prevLPay);

            var prevBlReceive = (from b in bankTransactions.Where(d => d.TranDate < fromDate)
                                 join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                 where b.TransactionType == (int)EnumTransactionType.LiaRec
                                 select new LiabilityReportModel
                                 {
                                     HeadID = h.SIHID,
                                     RecDate = b.TranDate.Value,
                                     ReceiveAmt = b.Amount,
                                     HeadName = h.Name,
                                     BankLiaRecType = b.TransactionType,
                                     RecPurpose = b.Remarks,
                                 }).OrderBy(i => i.RecDate).ToList();
            if (prevBlReceive.Any())
                prevRecPaydata.AddRange(prevBlReceive);

            var BlReceive = (from b in bankTransactions.Where(d => d.TranDate >= fromDate)
                             join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                             where b.TransactionType == (int)EnumTransactionType.LiaRec
                             select new LiabilityReportModel
                             {
                                 HeadID = h.SIHID,
                                 RecDate = b.TranDate.Value,
                                 ReceiveAmt = b.Amount,
                                 HeadName = h.Name,
                                 BankLiaRecType = b.TransactionType,
                                 RecPurpose = b.Remarks,
                             }).OrderBy(i => i.RecDate).ToList();

            if (BlReceive.Any())
                RecPaydata.AddRange(BlReceive);

            var prevBlPay = (from b in bankTransactions.Where(d => d.TranDate < fromDate)
                             join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                             where b.TransactionType == (int)EnumTransactionType.LiaPay
                             select new LiabilityReportModel
                             {
                                 HeadID = h.SIHID,
                                 RecDate = b.TranDate.Value,
                                 PayAmt = b.Amount,
                                 HeadName = h.Name,
                                 BankLiaPayType = b.TransactionType,
                                 RecPurpose = b.Remarks,
                             }).OrderBy(i => i.RecDate).ToList();

            if (prevBlPay.Any())
                prevRecPaydata.AddRange(prevBlPay);

            var BlPay = (from b in bankTransactions.Where(d => d.TranDate >= fromDate)
                         join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                         where b.TransactionType == (int)EnumTransactionType.LiaPay
                         select new LiabilityReportModel
                         {
                             HeadID = h.SIHID,
                             RecDate = b.TranDate.Value,
                             PayAmt = b.Amount,
                             HeadName = h.Name,
                             BankLiaPayType = b.TransactionType,
                             RecPurpose = b.Remarks,
                         }).OrderBy(i => i.RecDate).ToList();

            if (BlPay.Any())
                RecPaydata.AddRange(BlPay);

            #region prev final Data
            var prevFinalData = (from l in prevRecPaydata
                                 group l by new
                                 {
                                     l.HeadID,
                                     l.HeadName,
                                     l.RecDate,
                                     //l.PayDate,
                                     l.RecPurpose,
                                     l.RecLiabilityType,
                                     l.PayPurpose,
                                     l.PayLiabilityType,
                                     l.BankLiaRecType,
                                     l.BankLiaPayType,
                                     l.Status

                                 } into g
                                 select new LiabilityReportModel
                                 {
                                     HeadID = g.Key.HeadID,
                                     HeadName = g.Key.HeadName,
                                     RecDate = g.Key.RecDate,
                                     ReceiveAmt = g.Sum(i => i.ReceiveAmt),
                                     RecPurpose = g.Key.RecPurpose,
                                     RecLiabilityType = g.Key.RecLiabilityType,

                                     //PayDate = g.Key.PayDate,
                                     PayAmt = g.Sum(i => i.PayAmt),
                                     PayPurpose = g.Key.PayPurpose,
                                     PayLiabilityType = g.Key.PayLiabilityType,
                                     BankLiaRecType = g.Key.BankLiaRecType,
                                     BankLiaPayType = g.Key.BankLiaPayType,
                                     Status = g.Key.Status

                                 }).OrderBy(i => i.HeadID).ThenBy(i => i.RecDate);
            #endregion

            decimal prevLRecAmount = prevFinalData.Sum(d => d.ReceiveAmt);
            decimal prevLPayAmount = prevFinalData.Sum(d => d.PayAmt);


            var finalData = (from l in RecPaydata
                             group l by new
                             {
                                 l.HeadID,
                                 l.HeadName,
                                 l.RecDate,
                                 //l.PayDate,
                                 l.RecPurpose,
                                 l.RecLiabilityType,
                                 l.PayPurpose,
                                 l.PayLiabilityType,
                                 l.BankLiaRecType,
                                 l.BankLiaPayType,
                                 l.Status

                             } into g
                             select new LiabilityReportModel
                             {
                                 HeadID = g.Key.HeadID,
                                 HeadName = g.Key.HeadName,
                                 RecDate = g.Key.RecDate,
                                 ReceiveAmt = g.Sum(i => i.ReceiveAmt),
                                 RecPurpose = g.Key.RecPurpose,
                                 RecLiabilityType = g.Key.RecLiabilityType,

                                 //PayDate = g.Key.PayDate,
                                 PayAmt = g.Sum(i => i.PayAmt),
                                 PayPurpose = g.Key.PayPurpose,
                                 PayLiabilityType = g.Key.PayLiabilityType,
                                 BankLiaRecType = g.Key.BankLiaRecType,
                                 BankLiaPayType = g.Key.BankLiaPayType,
                                 Status = g.Key.Status

                             }).OrderBy(i => i.HeadID).ThenBy(i => i.RecDate);


            TransactionalDataSet.dtLiabiltyReportDataTable dt = new TransactionalDataSet.dtLiabiltyReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            int lHeadID = 0;

            decimal openingBalance = prevLRecAmount - prevLPayAmount;

            if ((openingBalance != 0) || (headOpeningAmount != 0))
            {
                var firstHead = finalData.FirstOrDefault();
                openingBalance += headOpeningAmount;

                row = dt.NewRow();
                row["RecDate"] = string.Empty;
                row["RecAmt"] = openingBalance > 0 ? openingBalance : 0m;
                row["PayDate"] = string.Empty;
                row["PayAmt"] = openingBalance <= 0 ? -(openingBalance) : 0m;
                row["RecType"] = string.Empty;
                row["PayType"] = string.Empty;
                row["RecPurpose"] = openingBalance > 0 ? "Opening" : string.Empty;
                row["PayPurpose"] = openingBalance <= 0 ? "Opening" : string.Empty;
                row["HeadName"] = firstHead != null ? firstHead.HeadName : shareInvestmentHead.Name;
                row["Balance"] = openingBalance;
                row["Status"] = string.Empty;
                dt.Rows.Add(row);
            }

            foreach (var item in finalData)
            {
                if (item.HeadID != lHeadID)
                {
                    lHeadID = item.HeadID;
                    headRec = 0m;
                    headPay = 0m;
                }
                row = dt.NewRow();
                row["RecDate"] = item.RecDate == DateTime.MinValue || item.ReceiveAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["RecAmt"] = item.ReceiveAmt;
                row["PayDate"] = item.RecDate == DateTime.MinValue || item.PayAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["PayAmt"] = item.PayAmt;
                row["RecType"] = item.RecLiabilityType != 0 ? item.RecLiabilityType.GetDisplayName() : "";
                row["PayType"] = item.PayLiabilityType != 0 ? item.PayLiabilityType.GetDisplayName() : "";
                row["RecPurpose"] = item.RecPurpose;
                row["PayPurpose"] = item.PayPurpose;
                row["HeadName"] = item.HeadName;
                TotalRec += item.ReceiveAmt;
                TotalPay += item.PayAmt;

                headRec += item.ReceiveAmt + headOpeningAmount;
                headPay += item.PayAmt;
                row["Balance"] = headRec - headPay;

                if (item.BankLiaPayType == 8 || item.BankLiaRecType == 9)
                {
                    row["Status"] = "Don't Show";
                }
                else
                {
                    if (item.Status == 0)
                    {
                        row["Status"] = "Show";
                    }
                    else
                    {
                        row["Status"] = "Don't Show";
                    }
                }

                dt.Rows.Add(row);
            }

            dt.TableName = "dtLiabiltyReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            if (OnlyHead)
                _reportParameter = new ReportParameter("DateRange", "Liabilty Report");
            else
                _reportParameter = new ReportParameter("DateRange", "Liabilty report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RemainingAmt", ((TotalRec - TotalPay) + openingBalance).ToString("F2"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Investment\\rptLiabilityRpt.rdlc");
        }

        // Auto barcode
        public byte[] BarCodeGenrator(POrder Data, string userName, int concernID)
        {
            List<ProductDetailsModel> IMEIs = new List<ProductDetailsModel>();
            ProductDetailsModel IMEI = null;
            ProductWisePurchaseModel product = null;
            foreach (var item in Data.POrderDetails.ToList())
            {
                product = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.ProductID == item.ProductID);
                foreach (var item2 in item.POProductDetails)
                {
                    IMEI = new ProductDetailsModel();
                    IMEI.ProductName = product.ProductName;
                    IMEI.CategoryName = product.CategoryName;
                    IMEI.IMENo = item2.IMENO;
                    IMEI.CashSalesRate = item.SalesRate;
                    IMEI.PRate = item.UnitPrice;
                    //IMEI.PartNumber = product.PartNumber; //no need
                    IMEIs.Add(IMEI);
                }
            }
            return PrintBarCodeGenrator(IMEIs, userName, concernID);
        }

        public byte[] BarCodeGenratorByID(int POrderID, string userName, int concernID)
        {
            var POrderDetails = (from pod in _PurchaseOrderDetailService.GetPOrderDetailByID(POrderID)
                                 join pop in _POProductDetailService.GetAll() on pod.POrderDetailID equals pop.POrderDetailID
                                 join p in _productService.GetAllProductIQueryable() on pod.ProductID equals p.ProductID
                                 select new ProductDetailsModel
                                 {
                                     ProductCode = p.ProductCode,
                                     ProductName = p.ProductName,
                                     CategoryName = p.CategoryName,
                                     //PartNumber = p.PartNumber,
                                     CashSalesRate = pod.SalesRate,
                                     IMENo = pop.IMENO,
                                     PRate = pod.UnitPrice
                                 }).ToList();
            return PrintBarCodeGenrator(POrderDetails, userName, concernID);
        }

        public byte[] PrintIMEI(int SDetailID, string userName, int concernID)
        {
            var POrderDetails = (from sd in _stockdetailService.GetAll()
                                 join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                 where sd.SDetailID == SDetailID
                                 select new ProductDetailsModel
                                 {
                                     ProductCode = p.ProductCode,
                                     //PartNumber = p.PartNumber,
                                     ProductName = p.ProductName,
                                     CategoryName = p.CategoryName,
                                     CashSalesRate = sd.SRate,
                                     IMENo = sd.IMENO,
                                     PRate = sd.PRate
                                 }).ToList();
            return PrintBarCodeGenrator(POrderDetails, userName, concernID);
        }


        private StringBuilder GetSymbolicRate(decimal Rate)
        {
            Dictionary<int, string> symbols = new Dictionary<int, string>();
            symbols.Add(0, "XS");
            symbols.Add(1, "A");
            symbols.Add(2, "B");
            symbols.Add(3, "C");
            symbols.Add(4, "D");
            symbols.Add(5, "V");
            symbols.Add(6, "F");
            symbols.Add(7, "G");
            symbols.Add(8, "H");
            symbols.Add(9, "i");

            StringBuilder result = new StringBuilder();
            if (Rate < 0)
                result.Append("-");

            var strRate = Rate.ToString();
            string firstPart = strRate.Substring(0, strRate.IndexOf('.'));
            //string secondPart = strRate.Substring(strRate.IndexOf('.')+1);
            int dicKey = 0;
            foreach (char item in firstPart)
            {
                dicKey = Convert.ToInt32(item.ToString());
                if (symbols.ContainsKey(dicKey))
                    result.Append(symbols[dicKey]);
            }

            //result.Append(".");

            //foreach (var item in secondPart)
            //{
            //    dicKey = Convert.ToInt32(item.ToString());
            //    if (symbols.ContainsKey(dicKey))
            //        result.Append(symbols[dicKey]);
            //}

            return result;
        }

        private byte[] PrintBarCodeGenrator(List<ProductDetailsModel> Data, string userName, int concernID)
        {
            _dataSet = new DataSet();
            TransactionalDataSet.dtBarcodeGenDataTable dt = new TransactionalDataSet.dtBarcodeGenDataTable();
            var SysInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            #region Barcode Config
            LinearRDLC barcode = new LinearRDLC();
            // set barcode type to Code 128
            barcode.Type = BarcodeType.CODE128;
            barcode.AddCheckSum = true;
            barcode.ImageWidth = 0.18875f;
            barcode.BarWidth = .1f;
            barcode.BarHeight = 25f;
            barcode.LeftMargin = 0;
            barcode.RightMargin = 0;
            barcode.TopMargin = 0;
            barcode.BottomMargin = 0;
            barcode.TextFont = new System.Drawing.Font("Times New Roman", 10, System.Drawing.FontStyle.Bold);
            barcode.ShowText = false;

            var sysInfo = GetCommonParameters(userName, concernID);

            #endregion
            foreach (var item in Data)
            {

                barcode.Data = item.IMENo;
                // set drawing barcode image format
                barcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;

                var barray = barcode.drawBarcodeAsBytes();

                if (sysInfo.BarcodeSize == EnumBarcodeSize.Size_Tow)
                    item.ProductName = item.ProductName + "\n" + GetSymbolicRate(item.PRate);

                dt.Rows.Add(item.CategoryName + ", " + item.ProductName, barray, item.IMENo, "Price TK. " + item.CashSalesRate);
            }

            dt.TableName = "dtBarcodeGen";
            _dataSet.Tables.Add(dt);

            //if (sysInfo.BarcodeSize == EnumBarcodeSize.Size_Tow)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Barcode\\rptBarcodeGenTwo.rdlc");
            //else
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Barcode\\rptBarcodeGen.rdlc");

        }

        public byte[] DistributorAnalysis(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            TransactionalDataSet.dtDistributorAnalysisDataTable dt = new TransactionalDataSet.dtDistributorAnalysisDataTable();
            _dataSet = new DataSet();
            List<DistributorAnalysis> analysis = new List<DistributorAnalysis>();
            DataRow row = null;

            #region stock and sales data
            var stockData = (from s in _StockServce.GetStockLedgerReport(3, "", "", "", fromDate, toDate, concernID)
                             select new DistributorAnalysis
                             {
                                 PCategoryID = s.PCategoryID,
                                 PCategoryName = s.ParentCategoryName,
                                 StockQty = s.ClosingStockQuantity,
                                 StockValue = s.ClosingStockValue,
                                 RetailQty = 0m,
                                 RetailValue = 0m,
                                 DealerQty = 0m,
                                 DealerValue = 0m,
                                 HireQty = 0m,
                                 HireValue = 0m,
                                 TotalQty = 0m,
                                 TotalValue = 0m
                             }).ToList();
            if (stockData.Count() > 0)
                analysis.AddRange(stockData);

            var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, concernID, false, 0).ToList();
            var ParentCategories = _parentCategoryService.GetAll().ToList();
            var retailSales = (from s in salseDetailInfos.Where(i => i.Rest.Rest.Item3 == (int)EnumCustomerType.Retail)
                               join pc in ParentCategories on s.Rest.Rest.Item6 equals pc.PCategoryID
                               group new { s, pc } by new { pc.PCategoryID, pc.Name } into g
                               select new DistributorAnalysis
                               {
                                   PCategoryID = g.Key.PCategoryID,
                                   PCategoryName = g.Key.Name,
                                   StockQty = 0m,
                                   StockValue = 0m,
                                   RetailQty = g.Sum(i => i.s.Rest.Item5),
                                   RetailValue = g.Sum(i => i.s.Rest.Rest.Rest.Item1),
                                   DealerQty = 0m,
                                   DealerValue = 0m,
                                   HireQty = 0m,
                                   HireValue = 0m,
                                   TotalQty = 0m,
                                   TotalValue = 0m
                               }).ToList();

            if (retailSales.Count() > 0)
                analysis.AddRange(retailSales);

            var dealerSales = (from s in salseDetailInfos.Where(i => i.Rest.Rest.Item3 == (int)EnumCustomerType.Dealer)
                               join pc in ParentCategories on s.Rest.Rest.Item6 equals pc.PCategoryID
                               group new { s, pc } by new { pc.PCategoryID, pc.Name } into g
                               select new DistributorAnalysis
                               {
                                   PCategoryID = g.Key.PCategoryID,
                                   PCategoryName = g.Key.Name,
                                   StockQty = 0m,
                                   StockValue = 0m,
                                   RetailQty = 0m,
                                   RetailValue = 0m,
                                   DealerQty = g.Sum(i => i.s.Rest.Item5),
                                   DealerValue = g.Sum(i => i.s.Rest.Rest.Rest.Item1),
                                   HireQty = 0m,
                                   HireValue = 0m,
                                   TotalQty = 0m,
                                   TotalValue = 0m
                               }).ToList();

            if (dealerSales.Count() > 0)
                analysis.AddRange(dealerSales);

            var CreditsalseDetailInfos = (from s in _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID, false)
                                          join pc in ParentCategories on s.Rest.Rest.Item6 equals pc.PCategoryID
                                          group new { s, pc } by new { pc.PCategoryID, pc.Name } into g
                                          select new DistributorAnalysis
                                          {
                                              PCategoryID = g.Key.PCategoryID,
                                              PCategoryName = g.Key.Name,
                                              StockQty = 0m,
                                              StockValue = 0m,
                                              HireQty = g.Sum(i => i.s.Rest.Item5),
                                              HireValue = g.Sum(i => i.s.Item6),
                                              DealerQty = 0m,
                                              DealerValue = 0m,
                                              RetailQty = 0m,
                                              RetailValue = 0m,
                                              TotalQty = 0m,
                                              TotalValue = 0m
                                          }).ToList();

            if (CreditsalseDetailInfos.Count() > 0)
                analysis.AddRange(CreditsalseDetailInfos);

            var finaldata = (from d in analysis
                             group d by new { d.PCategoryID, d.PCategoryName } into g
                             select new DistributorAnalysis
                             {
                                 PCategoryID = g.Key.PCategoryID,
                                 PCategoryName = g.Key.PCategoryName,
                                 RetailQty = g.Sum(i => i.RetailQty),
                                 RetailValue = g.Sum(i => i.RetailValue),
                                 DealerQty = g.Sum(i => i.DealerQty),
                                 DealerValue = g.Sum(i => i.DealerValue),
                                 HireQty = g.Sum(i => i.HireQty),
                                 HireValue = g.Sum(i => i.HireValue),
                                 StockQty = g.Sum(i => i.StockQty),
                                 StockValue = g.Sum(i => i.StockValue),
                                 TotalQty = g.Sum(i => i.RetailQty) + g.Sum(i => i.DealerQty) + g.Sum(i => i.HireQty),
                                 TotalValue = g.Sum(i => i.RetailValue) + g.Sum(i => i.DealerValue) + g.Sum(i => i.HireValue),
                             }).ToList();

            foreach (var item in finaldata)
            {
                row = dt.NewRow();
                row["CategoryName"] = item.PCategoryName;
                row["RetailQty"] = item.RetailQty;
                row["RetailValue"] = item.RetailValue;
                row["DealerQty"] = item.DealerQty;
                row["DealerValue"] = item.DealerValue;
                row["HireQty"] = item.HireQty;
                row["HireValue"] = item.HireValue;
                row["StockQty"] = item.StockQty;
                row["StockValue"] = item.StockValue;
                row["TotalQty"] = item.TotalQty;
                row["TotalValue"] = item.TotalValue;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtDistributorAnalysis";
            _dataSet.Tables.Add(dt);

            #endregion

            #region supplier bank delivery
            TransactionalDataSet.dtSupplierDataTable dtSuppliers = new TransactionalDataSet.dtSupplierDataTable();

            List<ProductWisePurchaseModel> delivery = new List<ProductWisePurchaseModel>();
            var SupplierBankDelivery = (from b in _bankTransactionService.GetBankTransactionDataForAll(fromDate, toDate, concernID, 0, 0, 0, EnumTransactionType.CashDelivery, false)
                                        group b by b.Item4 into g
                                        select new ProductWisePurchaseModel
                                        {
                                            SupplierName = g.Key,
                                            RecAmt = g.Sum(i => i.Rest.Item1)
                                        });
            if (SupplierBankDelivery.Count() > 0)
                delivery.AddRange(SupplierBankDelivery);

            var cashdelivery = (from cd in _CashCollectionService.GetCashDeliveryData(fromDate, toDate, concernID, 0, false)
                                group cd by cd.Item2 into g
                                select new ProductWisePurchaseModel
                                {
                                    SupplierName = g.Key,
                                    RecAmt = g.Sum(i => i.Item6)
                                });

            if (cashdelivery.Count() > 0)
                delivery.AddRange(cashdelivery);




            IQueryable<Supplier> suppliers = null;
            List<ProductWisePurchaseModel> purchases = new List<ProductWisePurchaseModel>();
            suppliers = _SupplierService.GetAllSupplier();

            purchases = (from po in _purchaseOrderService.GetAllIQueryable()
                         join s in suppliers on po.SupplierID equals s.SupplierID
                         where po.OrderDate >= fromDate && po.OrderDate <= toDate && po.RecAmt != 0
                         && po.Status == (int)EnumPurchaseType.Purchase
                         select new ProductWisePurchaseModel
                         {
                             RecAmt = po.RecAmt,
                             SupplierName = s.Name,
                         }).ToList();

            var purchaseRecvAmount = (from p in purchases
                                      group p by p.SupplierName into po
                                      select new ProductWisePurchaseModel
                                      {
                                          RecAmt = po.Sum(p => p.RecAmt),
                                          SupplierName = po.Key,
                                      }).ToList();
            if (purchaseRecvAmount.Count > 0)
                delivery.AddRange(purchaseRecvAmount);


            var Alldelivery = (from cd in delivery
                               group cd by cd.SupplierName into g
                               select new ProductWisePurchaseModel
                               {
                                   SupplierName = g.Key,
                                   RecAmt = g.Sum(i => i.RecAmt)
                               });

            foreach (var item in Alldelivery)
            {
                row = dtSuppliers.NewRow();
                row["SName"] = item.SupplierName;
                row["TotalDue"] = item.RecAmt;
                dtSuppliers.Rows.Add(row);
            }

            dtSuppliers.TableName = "dtSupplier";
            _dataSet.Tables.Add(dtSuppliers);
            #endregion

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Showroom analysis report from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            //**************************************
            #region collection
            var collections = _CashCollectionService.CustomerWiseCashCollection(fromDate, toDate, concernID);

            _reportParameter = new ReportParameter("RetailColl", collections.Item1.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DealerColl", collections.Item2.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Downpayment", collections.Item3.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InstallmentColl", collections.Item4.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalCollection", collections.Item5.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            #endregion

            //**************************************
            #region hire account details

            var hireAccountDetails = _creditSalesOrderService.HireAccountDetails(fromDate, toDate, concernID);

            _reportParameter = new ReportParameter("OpeningAcc", hireAccountDetails.OpeningAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingAcc", hireAccountDetails.ClosingAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RunningAcc", hireAccountDetails.RunningAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningValue", hireAccountDetails.OpeningAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingValue", hireAccountDetails.ClosingAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RunningAccValue", hireAccountDetails.RunningAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            #endregion

            //**************************************
            #region dealer account details

            var dealerAccountDetails = _salesOrderService.DealerAccountDetails(fromDate, toDate, concernID);

            _reportParameter = new ReportParameter("DOpening", dealerAccountDetails.OpeningAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DOpeningValue", dealerAccountDetails.OpeningAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DClosing", dealerAccountDetails.ClosingAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DClosingValue", dealerAccountDetails.ClosingAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DRunning", dealerAccountDetails.RunningAccount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DRunningValue", dealerAccountDetails.RunningAccountValue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptShowroomAnalysis.rdlc");
        }

        public byte[] GetAdvanceSalaryReport(DateTime fromDate, DateTime toDate, string userName, int ConcernID, int EmployeeID)
        {

            DataRow row = null;
            TransactionalDataSet.dtAdvanceSalaryDataTable dt = new TransactionalDataSet.dtAdvanceSalaryDataTable();
            _dataSet = new DataSet();
            var data = _AdvanceSalaryService.GetAdvanceSalaryReports(fromDate, toDate, EmployeeID);
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["Name"] = item.Code + ", " + item.Name;
                row["Date"] = item.Date.ToShortDateString();
                row["Basic"] = item.BasicSalary;
                row["Advance"] = item.AdvanceAmt;
                row["Remarks"] = item.Remarks;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtAdvanceSalary";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, ConcernID);


            _reportParameter = new ReportParameter("Date", "Advance Salary Report  for the Date of : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptSalarySheet.rdlc");


        }

        public byte[] AdjustmentReport(DateTime fromDate, DateTime toDate, string userName, int ReportType, int ConcernID)
        {
            try
            {
                #region Hire Sales Last Pay Adjustment
                if (ReportType == 1)
                {
                    DataRow row = null;
                    TransactionalDataSet.dtAdjustmentDataTable dt = new TransactionalDataSet.dtAdjustmentDataTable();
                    _dataSet = new DataSet();
                    var HireSaleData = _creditSalesOrderService.GetAdjustmentReport(fromDate, toDate);
                    foreach (var item in HireSaleData)
                    {
                        row = dt.NewRow();
                        row["Date"] = item.Date.ToShortDateString();
                        row["CustomerCode"] = item.CustomerCode;
                        row["CustomerName"] = item.CustomerName;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["Amount"] = item.AdjutmentAmt;
                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtAdjustment";
                    _dataSet.Tables.Add(dt);
                    GetCommonParameters(userName, ConcernID);
                }
                #endregion
                #region Cash Collection Adjustment
                else if (ReportType == 2)
                {
                    DataRow row = null;
                    TransactionalDataSet.dtAdjustmentDataTable dt = new TransactionalDataSet.dtAdjustmentDataTable();
                    _dataSet = new DataSet();
                    var CashCollectionData = _CashCollectionService.GetAdjustmentReport(fromDate, toDate);
                    foreach (var item in CashCollectionData)
                    {
                        row = dt.NewRow();
                        row["Date"] = item.Date.ToShortDateString();
                        row["CustomerCode"] = item.CustomerCode;
                        row["CustomerName"] = item.CustomerName;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["Amount"] = item.AdjutmentAmt;
                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtAdjustment";
                    _dataSet.Tables.Add(dt);
                    GetCommonParameters(userName, ConcernID);
                }
                #endregion
                #region All Adjustment
                else if (ReportType == 3)
                {
                    DataRow row = null;
                    TransactionalDataSet.dtAdjustmentDataTable dt = new TransactionalDataSet.dtAdjustmentDataTable();
                    _dataSet = new DataSet();
                    var HireSaleData = _creditSalesOrderService.GetAdjustmentReport(fromDate, toDate);
                    foreach (var item in HireSaleData)
                    {
                        row = dt.NewRow();
                        row["Date"] = item.Date.ToShortDateString();
                        row["CustomerCode"] = item.CustomerCode;
                        row["CustomerName"] = item.CustomerName;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["Amount"] = item.AdjutmentAmt;
                        dt.Rows.Add(row);
                    }
                    //dt.TableName = "dtAdjustment";
                    //_dataSet.Tables.Add(dt);
                    //GetCommonParameters(userName, ConcernID);

                    var CashCollectionData = _CashCollectionService.GetAdjustmentReport(fromDate, toDate);
                    foreach (var item in CashCollectionData)
                    {
                        row = dt.NewRow();
                        row["Date"] = item.Date.ToShortDateString();
                        row["CustomerCode"] = item.CustomerCode;
                        row["CustomerName"] = item.CustomerName;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["Amount"] = item.AdjutmentAmt;
                        dt.Rows.Add(row);
                    }
                    dt.TableName = "dtAdjustment";
                    _dataSet.Tables.Add(dt);
                    GetCommonParameters(userName, ConcernID);
                }
                #endregion
                _reportParameter = new ReportParameter("Date", "Adjustment Report  for the Date of : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\AdjustmentReport.rdlc");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public byte[] AdvanceLoanReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int EmployeeID, bool OnlyEmployee)
        {
            IQueryable<AdvanceSalary> advanceSalaries = null;
            IQueryable<Employee> employees = null;
            List<AdvanceLoanReportModel> RecPaydata = new List<AdvanceLoanReportModel>();

            decimal TotalRec = 0m, TotalPay = 0m, employeeRec = 0m, employeePay = 0m;

            if (OnlyEmployee)
            {
                if (EmployeeID > 0)
                    advanceSalaries = _AdvanceSalaryService.GetAdvanceSalariesQueryable().Where(i => i.EmployeeID == EmployeeID);


                else
                    advanceSalaries = from a in _AdvanceSalaryService.GetAdvanceSalariesQueryable()
                                      join e in _EmployeeService.GetAllEmployeeIQueryable() on a.EmployeeID equals e.EmployeeID
                                      where a.SalaryType == EnumSalaryType.AdvanceLoan || (a.SalaryType == EnumSalaryType.AdvanceSalary && a.IsAdvanceLoanPay == 1)
                                      select a;

            }
            else
            {
                if (EmployeeID > 0)
                    advanceSalaries = _AdvanceSalaryService.GetAdvanceSalariesQueryable().Where(i => i.EmployeeID == EmployeeID && i.Date >= fromDate && i.Date <= toDate);


                else
                    advanceSalaries = from a in _AdvanceSalaryService.GetAdvanceSalariesQueryable().Where(i => i.Date >= fromDate && i.Date <= toDate)
                                      join e in _EmployeeService.GetAllEmployeeIQueryable() on a.EmployeeID equals e.EmployeeID
                                      where a.SalaryType == EnumSalaryType.AdvanceLoan || (a.SalaryType == EnumSalaryType.AdvanceSalary && a.IsAdvanceLoanPay == 1)
                                      select a;

            }

            var loanRec = (from ads in advanceSalaries
                           join emp in _EmployeeService.GetAllEmployeeIQueryable() on ads.EmployeeID equals emp.EmployeeID
                           where ads.SalaryType == EnumSalaryType.AdvanceLoan
                           select new AdvanceLoanReportModel
                           {
                               EmployeeID = emp.EmployeeID,
                               RecDate = ads.Date,
                               ReceiveAmt = ads.Amount,
                               EmployeeName = emp.Name,
                               RecSalaryType = ads.SalaryType,
                               RecPurpose = ads.Remarks

                           }).OrderBy(i => i.RecDate).ToList();
            if (loanRec.Count() > 0)
                RecPaydata.AddRange(loanRec);


            var loanPay = (from ads in advanceSalaries
                           join emp in _EmployeeService.GetAllEmployeeIQueryable() on ads.EmployeeID equals emp.EmployeeID
                           where ads.SalaryType == EnumSalaryType.AdvanceSalary && ads.IsAdvanceLoanPay == 1
                           select new AdvanceLoanReportModel
                           {
                               EmployeeID = emp.EmployeeID,
                               RecDate = ads.Date,
                               PayAmt = ads.Amount,
                               EmployeeName = emp.Name,
                               PaySalaryType = ads.SalaryType,
                               PayPurpose = ads.Remarks

                           }).OrderBy(i => i.RecDate).ToList();
            if (loanPay.Count() > 0)
                RecPaydata.AddRange(loanPay);

            var finalData = (from l in RecPaydata
                             group l by new
                             {
                                 l.EmployeeID,
                                 l.EmployeeName,
                                 l.RecDate,
                                 l.RecPurpose,
                                 l.RecSalaryType,
                                 l.PayPurpose,
                                 l.PaySalaryType



                             } into g
                             select new AdvanceLoanReportModel
                             {
                                 EmployeeID = g.Key.EmployeeID,
                                 EmployeeName = g.Key.EmployeeName,
                                 RecDate = g.Key.RecDate,
                                 ReceiveAmt = g.Sum(i => i.ReceiveAmt),
                                 RecPurpose = g.Key.RecPurpose,
                                 RecSalaryType = g.Key.RecSalaryType,
                                 PayAmt = g.Sum(i => i.PayAmt),
                                 PayPurpose = g.Key.PayPurpose,
                                 PaySalaryType = g.Key.PaySalaryType


                             }).OrderBy(i => i.EmployeeID).ThenBy(i => i.RecDate);

            TransactionalDataSet.dtAdvanceLoanReportDataTable dt = new TransactionalDataSet.dtAdvanceLoanReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            int aEmployeeID = 0;

            foreach (var item in finalData)
            {
                if (item.EmployeeID != aEmployeeID)
                {
                    aEmployeeID = item.EmployeeID;
                    employeeRec = 0m;
                    employeePay = 0m;
                }
                row = dt.NewRow();
                row["RecDate"] = item.RecDate == DateTime.MinValue || item.ReceiveAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["RecAmt"] = item.ReceiveAmt;
                row["PayDate"] = item.RecDate == DateTime.MinValue || item.PayAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["PayAmt"] = item.PayAmt;
                row["RecType"] = item.RecSalaryType != 0 ? item.RecSalaryType.GetDisplayName() : "";
                row["PayType"] = item.PaySalaryType != 0 ? item.PaySalaryType.GetDisplayName() : "";
                row["RecPurpose"] = item.RecPurpose;
                row["PayPurpose"] = item.PayPurpose;
                row["EmployeeName"] = item.EmployeeName;
                TotalRec += item.ReceiveAmt;
                TotalPay += item.PayAmt;

                employeeRec += item.ReceiveAmt;
                employeePay += item.PayAmt;
                row["Balance"] = employeeRec - employeePay;
                dt.Rows.Add(row);

            }
            dt.TableName = "dtAdvanceLoanReport";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(UserName, ConcernID);
            if (OnlyEmployee)
                _reportParameter = new ReportParameter("DateRange", "Advance Loan Report");
            else
                _reportParameter = new ReportParameter("DateRange", "Advance loan from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RemainingAmt", (TotalRec - TotalPay).ToString("F2"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptAdvanceLoanRpt.rdlc");
        }
        public byte[] StockQTYReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforStockReport(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ColorID, PCategoryID, IsVATManager, 0).ToList();
                var InhouseDamageProductDetails = _productService.GetAllDamageProductFromDetail();
                //var CompanyDamageProductDetails = _purchaseOrderService.GetDamageReturnProductDetails(0, 0);
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();

                #region Product wise
                if (reportType == 0)//Product Wise
                {

                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item1, p.Item2, p.Item3, p.Item4, p.Item6, p.Item7, p.Rest.Item5 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           ProductCode = g.Key.Item2,
                                           ProductName = g.Key.Item3,

                                           CategoryName = g.Key.Item6,
                                           CompanyName = g.Key.Item7,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();
                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProName = ns.Item2, ModelName = ns.Item5, CompanyName = ns.Item3, CategoryName = ns.Item4, PRate = ns.Item7, ProMRP = ns.Rest.Item4 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProName,
                                            ColorName = g.Key.ModelName,
                                            CompanyName = g.Key.CompanyName,
                                            CategoryName = g.Key.CategoryName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            PurchaseRate = g.Key.PRate,
                                            ProMRP = g.Key.ProMRP
                                        }).ToList();

                    bool IsHistoryShow = false;
                    if ((concernID != 1 || concernID != 5 || concernID != 6) && reportType == 0)
                        IsHistoryShow = true;
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;//Total Sales Price
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.TotalAmount / item.Quantity;
                        row["CreditSRate"] = item.TotalCreditSR6 / item.Quantity; //6 months
                        row["TotalCreditPrice"] = item.PurchaseRate * item.Quantity;//item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.TotalCreditSR3 / item.Quantity;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = item.TotalCreditSR12 / item.Quantity;
                        row["TotalCrSR12"] = item.ProMRP;
                        row["Godown"] = "";

                        if (IsHistoryShow)
                        {
                            row["StockHistory"] = "";// _StockServce.GetStockProductsHistory(item.Item1);
                        }
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Company wise
                else if (reportType == 1) //Company wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6, p.Item7, p.Rest.Item5 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           CompanyName = g.Key.Item7,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Company = ns.Item3, Category = ns.Item4 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            CompanyName = g.Key.Company,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Category wise
                else if (reportType == 2) //category wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Category = ns.Item4 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Godown Wise
                else if (reportType == 3) //Godown wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { GodownName = ns.Rest.Item5 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            GodownName = g.Key.GodownName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = 0;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["Godown"] = item.GodownName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                #region Color wise
                else if (reportType == 4) //Color wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProductName = ns.Item2, ColorName = ns.Item5, } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProductName,
                                            ColorName = g.Key.ColorName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = 0;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["Godown"] = item.GodownName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion


                #region PCategory wise
                else if (reportType == 5) // parent category wise
                {
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item6 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           CategoryName = g.Key.Item6,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { Category = ns.Item4, ns.Rest.Item7 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            CategoryName = g.Key.Category,
                                            PCategoryName = g.Key.Item7,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            PurchaseRate = g.Sum(i => i.Item7 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                        }).ToList();
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        //row["StockCode"] = item.Item1;
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ProductName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = 0m;
                        row["CreditSRate"] = 0m;// 6 months
                        row["TotalCreditPrice"] = item.TotalCreditSR6;
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = 0m;
                        row["TotalCrSR3"] = item.TotalCreditSR3;
                        row["CrSR12"] = 0m;
                        row["TotalCrSR12"] = item.TotalCreditSR12;
                        row["PCategoryName"] = item.PCategoryName;
                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                #endregion

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockQTYInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStockQTYSummary.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStockQTYSummary.rdlc";
                }
                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStockQTYSummary.rdlc";
                }
                else if (reportType == 4)
                {
                    reportName = "Stock\\rptColorWiseStockQTYSummary.rdlc";
                }

                else if (reportType == 5)
                {
                    reportName = "Stock\\rptPCategoryWiseStockQTYSummary.rdlc";
                }

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] SalesOrderMoneyReceiptPrint(SOrder oSorder, string userName, int concernID, bool isPosRecipt)
        {
            var Customer = _customerService.GetCustomerById((int)oSorder.CustomerID);
            var Sales = _salesOrderService.GetLastSalesOrderByCustomerID((int)oSorder.CustomerID);
            string user = _userService.GetUserNameById(oSorder.CreatedBy);
            _dataSet = new DataSet();

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReceiptNo", oSorder.InvoiceNo);
            _reportParameters.Add(_reportParameter);
            string sInwodTk = TakaFormat(Convert.ToDouble(oSorder.RecAmount.ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            //_SOrder.RecAmount.ToString()
            _reportParameter = new ReportParameter("ReceiptTK", oSorder.RecAmount.ToString());
            _reportParameters.Add(_reportParameter);
            //_SOrder.InvoiceDate.ToString()
            _reportParameter = new ReportParameter("ReceiptDate", oSorder.InvoiceDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("LastSalesDate", Sales != null ? Sales.InvoiceDate.ToString("dd MMM yyyy") : "");
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", Customer.Code + " " + "&" + " " + Customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("BalanceDue", (Customer.TotalDue).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CusAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContactNo", Customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Adjustment", oSorder.AdjAmount.ToString("F"));
            _reportParameters.Add(_reportParameter);

            if (!isPosRecipt)
            {
                _reportParameter = new ReportParameter("InterestAmt", "0");
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);
            }




            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
            {
                _reportParameter = new ReportParameter("Msg", " বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                _reportParameters.Add(_reportParameter);
            }

            if (isPosRecipt)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "POS\\rptPOSCashColl.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMMoneyReceipt.rdlc");

        }

        public byte[] LastPayAdjReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                var LastPayAdjAmt = _creditSalesOrderService.GetLastPayAdjAmt(fromDate, toDate, concernID);
                TransactionalDataSet.dtLastPayAdjAmtDataTable dt = new TransactionalDataSet.dtLastPayAdjAmtDataTable();
                DataRow row = null;
                string PRO = "";
                int count;

                foreach (var item in LastPayAdjAmt)
                {
                    row = dt.NewRow();
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["CustomerCode"] = item.CustomerCode;
                    row["ContactNo"] = item.CustomerName + " & " + item.CustomerConctact;
                    row["SalesDate"] = item.SalesDate;
                    row["SalesPrice"] = item.NoOfInstallment;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.NetAmount;
                    row["RemainingAmt"] = item.Remaining;
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    row["LastPayAdjAmt"] = item.LastPayAdjAmt;
                    row["PaymentDate"] = item.PaymentDate.ToShortDateString();

                    PRO = "";
                    count = 0;

                    foreach (var prod in item.ProductName)
                    {
                        if (count == 0)
                            PRO = PRO + prod;
                        else
                            PRO = PRO + "," + System.Environment.NewLine + prod;
                        count++;
                    }
                    row["ProductName"] = PRO;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtLastPayAdjAmt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PaymentDate", "Total Last Pay Adjust Amount Date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptLastPayAdjAmtReport.rdlc");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StockForcastingReport(DateTime fromDate, DateTime toDate, string userName, int concernID, string ClientDateTime)
        {
            try
            {
                var stockInfos = _StockServce.StockForcastingReport(fromDate, toDate, concernID).ToList();
                DataRow row = null;
                string reportName = string.Empty;
                DateTime dateTime = DateTime.Today;
                decimal tQty = 0;
                int tempProId = 0;
                String sMonth1 = "", sMonth2 = "", sMonth3 = "", sMonth4 = "", sMonth5 = "", sMonth6 = "", sMonth7 = "", sMonth8 = "", sMonth9 = "", sMonth10 = "", sMonth11 = "", sMonth12 = "";

                TransactionalDataSet.dtStockForcastingDataTable dtStockForcastingDT = new TransactionalDataSet.dtStockForcastingDataTable();
                foreach (var item in stockInfos)
                {
                    if (tempProId != item.ProductID)
                    {
                        tQty = 0;
                        row = dtStockForcastingDT.NewRow();
                    }

                    row["CategoryName"] = item.CategoryName;
                    row["ProName"] = item.ProductName;
                    //row["AverageSalesQty"] = item.AverageSalesQty;                                     
                    string CompanyName = item.CompanyName;
                    string CategoryName = item.CategoryName;
                    string ProductName = item.ProductName;
                    int UserConcernID = item.UserConcernID;
                    var stockInfoss = _StockServce.GetforAdminStockReport(userName, concernID, 0, CompanyName, CategoryName, ProductName, UserConcernID).ToList();
                    decimal Quantity = stockInfoss.Sum(i => i.Item6);
                    row["PresentStockQty"] = Quantity;
                    int SMonth = item.SMonth;
                    int Syear = item.SYear;

                    decimal MonthSalesQty = item.MonthSalesQty;
                    if (SMonth == 1)
                    {
                        row["MonthSalesQty"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth1 = "Jan" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 2)
                    {
                        row["MonthSalesQty2"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth2 = "Feb" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 3)
                    {
                        row["MonthSalesQty3"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth3 = "Mar" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 4)
                    {
                        row["MonthSalesQty4"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth4 = "Apr" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 5)
                    {
                        row["MonthSalesQty5"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth5 = "May" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 6)
                    {
                        row["MonthSalesQty6"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth6 = "Jun" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 7)
                    {
                        row["MonthSalesQty7"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth7 = "Jul" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 8)
                    {
                        row["MonthSalesQty8"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth8 = "Aug" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 9)
                    {
                        row["MonthSalesQty9"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth9 = "Sep" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 10)
                    {
                        row["MonthSalesQty10"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth10 = "Oct" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 11)
                    {
                        row["MonthSalesQty11"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth11 = "Nov" + Environment.NewLine + Syear.ToString();

                    }
                    else if (SMonth == 12)
                    {
                        row["MonthSalesQty12"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth12 = "Dec" + Environment.NewLine + Syear.ToString();

                    }

                    row["AverageSalesQty"] = tQty / 12;
                    row["NeedSalesQty"] = (Quantity / (tQty / 12));
                    if (tempProId != item.ProductID)
                    {

                        if (Convert.ToString(row["MonthSalesQty"]) == "")
                            row["MonthSalesQty"] = "0";
                        if (Convert.ToString(row["MonthSalesQty2"]) == "")
                            row["MonthSalesQty2"] = "0";
                        if (Convert.ToString(row["MonthSalesQty3"]) == "")
                            row["MonthSalesQty3"] = "0";
                        if (Convert.ToString(row["MonthSalesQty4"]) == "")
                            row["MonthSalesQty4"] = "0";
                        if (Convert.ToString(row["MonthSalesQty5"]) == "")
                            row["MonthSalesQty5"] = "0";
                        if (Convert.ToString(row["MonthSalesQty6"]) == "")
                            row["MonthSalesQty6"] = "0";
                        if (Convert.ToString(row["MonthSalesQty7"]) == "")
                            row["MonthSalesQty7"] = "0";
                        if (Convert.ToString(row["MonthSalesQty8"]) == "")
                            row["MonthSalesQty8"] = "0";
                        if (Convert.ToString(row["MonthSalesQty9"]) == "")
                            row["MonthSalesQty9"] = "0";
                        if (Convert.ToString(row["MonthSalesQty10"]) == "")
                            row["MonthSalesQty10"] = "0";
                        if (Convert.ToString(row["MonthSalesQty11"]) == "")
                            row["MonthSalesQty11"] = "0";
                        if (Convert.ToString(row["MonthSalesQty12"]) == "")
                            row["MonthSalesQty12"] = "0";
                        dtStockForcastingDT.Rows.Add(row);
                    }

                    tempProId = item.ProductID;
                }

                dtStockForcastingDT.TableName = "StockForcasting";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockForcastingDT);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month1", sMonth1);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month2", sMonth2);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month3", sMonth3);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month4", sMonth4);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month5", sMonth5);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month6", sMonth6);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month7", sMonth7);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month8", sMonth8);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month9", sMonth9);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month10", sMonth10);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month11", sMonth11);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month12", sMonth12);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("DateRange", "Date From : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptStockForecasting.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StockForcastingReportProductWise(DateTime fromDate, DateTime toDate, string userName, int concernID, string ClientDateTime, int ProductID)
        {
            try
            {
                var stockInfos = _StockServce.StockForcastingReportProductWise(fromDate, toDate, ProductID).ToList();
                DataRow row = null;
                string reportName = string.Empty;
                DateTime dateTime = DateTime.Today;
                decimal tQty = 0;
                int tempProId = 0;
                decimal Quantity = 0;
                String sMonth1 = "", sMonth2 = "", sMonth3 = "", sMonth4 = "", sMonth5 = "", sMonth6 = "", sMonth7 = "", sMonth8 = "", sMonth9 = "", sMonth10 = "", sMonth11 = "", sMonth12 = "";

                TransactionalDataSet.dtStockForcastingDataTable dtStockForcastingDT = new TransactionalDataSet.dtStockForcastingDataTable();
                foreach (var item in stockInfos)
                {
                    if (tempProId != item.ProductID)
                    {
                        tQty = 0;
                        Quantity = 0;
                        row = dtStockForcastingDT.NewRow();
                    }

                    row["ConcernName"] = item.ConcernName;
                    row["ProName"] = item.ProductName;
                    string CompanyName = item.CompanyName;
                    string CategoryName = item.CategoryName;
                    string ProductName = item.ProductName;
                    int UserConcernID = item.UserConcernID;
                    var stockInfoss = _StockServce.GetforAdminStockReport(userName, concernID, 0, CompanyName, CategoryName, ProductName, UserConcernID).ToList();
                    //Quantity = stockInfoss.Find(i=>i.Rest.Item5== item.ConcernName).Item6;
                    var objStockQty = stockInfoss.FirstOrDefault(i => i.Rest.Item5 == item.ConcernName);
                    if (objStockQty != null)
                        Quantity = objStockQty.Item6;
                    else
                        Quantity = 0;
                    row["PresentStockQty"] = Quantity;
                    int SMonth = item.SMonth;
                    int Syear = item.SYear;
                    decimal MonthSalesQty = item.MonthSalesQty;
                    if (SMonth == 1)
                    {
                        row["MonthSalesQty"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth1 = "Jan" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 2)
                    {
                        row["MonthSalesQty2"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth2 = "Feb" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 3)
                    {
                        row["MonthSalesQty3"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth3 = "Mar" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 4)
                    {
                        row["MonthSalesQty4"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth4 = "Apr" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 5)
                    {
                        row["MonthSalesQty5"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth5 = "May" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 6)
                    {
                        row["MonthSalesQty6"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth6 = "Jun" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 7)
                    {
                        row["MonthSalesQty7"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth7 = "Jul" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 8)
                    {
                        row["MonthSalesQty8"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth8 = "Aug" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 9)
                    {
                        row["MonthSalesQty9"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth9 = "Sep" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 10)
                    {
                        row["MonthSalesQty10"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth10 = "Oct" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 11)
                    {
                        row["MonthSalesQty11"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth11 = "Nov" + Environment.NewLine + Syear.ToString();
                    }
                    else if (SMonth == 12)
                    {
                        row["MonthSalesQty12"] = item.MonthSalesQty;
                        tQty = tQty + item.MonthSalesQty;
                        sMonth12 = "Dec" + Environment.NewLine + Syear.ToString();
                    }

                    row["AverageSalesQty"] = tQty / 12;
                    row["NeedSalesQty"] = (Quantity / (tQty / 12));

                    if (tempProId != item.ProductID)
                    {
                        if (Convert.ToString(row["MonthSalesQty"]) == "")
                            row["MonthSalesQty"] = "0";
                        if (Convert.ToString(row["MonthSalesQty2"]) == "")
                            row["MonthSalesQty2"] = "0";
                        if (Convert.ToString(row["MonthSalesQty3"]) == "")
                            row["MonthSalesQty3"] = "0";
                        if (Convert.ToString(row["MonthSalesQty4"]) == "")
                            row["MonthSalesQty4"] = "0";
                        if (Convert.ToString(row["MonthSalesQty5"]) == "")
                            row["MonthSalesQty5"] = "0";
                        if (Convert.ToString(row["MonthSalesQty6"]) == "")
                            row["MonthSalesQty6"] = "0";
                        if (Convert.ToString(row["MonthSalesQty7"]) == "")
                            row["MonthSalesQty7"] = "0";
                        if (Convert.ToString(row["MonthSalesQty8"]) == "")
                            row["MonthSalesQty8"] = "0";
                        if (Convert.ToString(row["MonthSalesQty9"]) == "")
                            row["MonthSalesQty9"] = "0";
                        if (Convert.ToString(row["MonthSalesQty10"]) == "")
                            row["MonthSalesQty10"] = "0";
                        if (Convert.ToString(row["MonthSalesQty11"]) == "")
                            row["MonthSalesQty11"] = "0";
                        if (Convert.ToString(row["MonthSalesQty12"]) == "")
                            row["MonthSalesQty12"] = "0";

                        dtStockForcastingDT.Rows.Add(row);
                    }

                    tempProId = item.ProductID;
                }

                dtStockForcastingDT.TableName = "StockForcasting";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockForcastingDT);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month1", sMonth1);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month2", sMonth2);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month3", sMonth3);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month4", sMonth4);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month5", sMonth5);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month6", sMonth6);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month7", sMonth7);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month8", sMonth8);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month9", sMonth9);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month10", sMonth10);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month11", sMonth11);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Month12", sMonth12);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("DateRange", "Date From : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptStockForecastingProductWise.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StockDetailReportWithDate(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID)
        {
            try
            {
                //var stockInfos = _StockServce.GetforStockReportWithDate(userName, concernID, reportType, CompanyID, CategoryID, ProductID);
                var stockInfos = _StockServce.StockReportWithDate(concernID, ProductID, CompanyID, CategoryID).ToList();
                DataRow row = null;
                string reportName = string.Empty;
                DateTime dateTime = DateTime.Today;


                TransactionalDataSet.StockInfoDateWiseDataTable dtStockInfoDateWiseDT = new TransactionalDataSet.StockInfoDateWiseDataTable();
                foreach (var item in stockInfos)
                {
                    row = dtStockInfoDateWiseDT.NewRow();
                    row["ChallanNo"] = item.ChallanNo;
                    row["Description"] = item.Description;
                    row["ProductName"] = item.ProductName;
                    row["IMENO"] = item.IMENO;
                    row["StockInDate"] = item.Date.ToShortDateString();
                    row["NoOfDays"] = (dateTime - item.Date).TotalDays;
                    row["CompanyName"] = item.CompanyName;
                    row["CategoryName"] = item.CategoryName;
                    dtStockInfoDateWiseDT.Rows.Add(row);
                }


                dtStockInfoDateWiseDT.TableName = "StockInfoDateWise";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDateWiseDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockInfoWithDate.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyStockInfoWithDate.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryStockInfoWithDate.rdlc";
                }

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public byte[] TotalLiabilityPayRec(DateTime asOnDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _ShareInvestmentService.TotalLiabilityPayRec(asOnDate, ConcernID);
            //var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, 2, ConcernID, 0).ToList();

            //decimal ClosingCashInhand = CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            TransactionalDataSet.dtTotalLiabilityPayRecDataTable dt = new TransactionalDataSet.dtTotalLiabilityPayRecDataTable();
            //TransactionalDataSet.dtTotalLiabilityPayRec dt = new TransactionalDataSet.dtTotalLiabilityPayRec();
            _dataSet = new DataSet();
            DataRow row = null;
            //decimal debitAmt = data.Sum(i => i.Debit);
            //decimal creditAmt = data.Where(i => !(i.CreditParticulars.Equals("Gross Profit"))).Sum(i => i.Credit);
            var debits = data.Where(i => !string.IsNullOrEmpty(i.DebitParticulars)).ToList();
            //debits.Insert(0, new ProfitLossReportModel() { DebitParticulars = "Cash In Hand", Debit = ClosingCashInhand, CreditParticulars = "", Credit = 0m, SerialNumber = 1 });
            var credits = data.Where(i => !string.IsNullOrEmpty(i.CreditParticulars)).ToList();
            //var totalCA = debits.FirstOrDefault(i => i.DebitParticulars.Equals("Total Current Assets"));
            //if (totalCA != null)
            //    totalCA.Debit += ClosingCashInhand;

            List<LiabilityReportModel> finalData = new List<LiabilityReportModel>();
            LiabilityReportModel bs = null;

            decimal TotalDebit = debits.Where(i => i.DebitParticulars.Contains("Total")).Sum(i => i.LiabilitiesReceived);
            TotalDebit += debits.Where(d => d.DebitParticulars.ToLower().Equals("all fixed asset")).Sum(d => d.LiabilitiesReceived);
            //TotalDebit += ClosingCashInhand;

            decimal TotalCredit = credits.Where(i => i.CreditParticulars.Contains("Total")).Sum(i => i.LiabilitiesPay);
            decimal ownersEquity = TotalDebit - TotalCredit;
            //credits.Add(new LiabilityReportModel() { DebitParticulars = "", LiabilitiesReceived = 0, CreditParticulars = "Owner's Equity", LiabilitiesPay = ownersEquity });
            TotalCredit += ownersEquity;
            if (debits.Count() > credits.Count())
            {
                for (int i = 0; i < debits.Count(); i++)
                {
                    bs = new LiabilityReportModel();
                    bs.DebitParticulars = debits[i].DebitParticulars;
                    bs.LiabilitiesReceived = debits[i].LiabilitiesReceived;
                    finalData.Add(bs);
                }

                for (int i = 0; i < credits.Count(); i++)
                {
                    finalData[i].LiabilitiesPay = credits[i].LiabilitiesPay;
                    finalData[i].CreditParticulars = credits[i].CreditParticulars;
                }
            }
            else
            {
                for (int i = 0; i < credits.Count(); i++)
                {
                    bs = new LiabilityReportModel();
                    bs.CreditParticulars = credits[i].CreditParticulars;
                    bs.LiabilitiesPay = credits[i].LiabilitiesPay;
                    finalData.Add(bs);
                }

                for (int i = 0; i < debits.Count(); i++)
                {
                    finalData[i].LiabilitiesReceived = debits[i].LiabilitiesReceived;
                    finalData[i].DebitParticulars = debits[i].DebitParticulars;
                }
            }

            foreach (var item in finalData)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.LiabilitiesReceived;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.LiabilitiesPay;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Balance Sheet of the date " + asOnDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", TotalDebit.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", TotalCredit.ToString());
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("NetProfit", (creditAmt-debitAmt).ToString());
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Investment\\rptTotalLiabilityRpt.rdlc");
        }

        public byte[] SalesInvoiceWithOutBarcodeReportPrint(SOrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);
                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;

                var ProductInfos = (from sd in oOrder.SOrderDetails
                                    join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                    join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                    join col in _ColorServce.GetAll() on std.ColorID equals col.ColorID
                                    select new
                                    {
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        Quantity = sd.Quantity,
                                        UnitPrice = sd.UnitPrice,
                                        SalesRate = sd.UTAmount,
                                        UTAmount = sd.UTAmount,
                                        PPDPercentage = sd.PPDPercentage,
                                        PPDAmount = sd.PPDAmount,
                                        PPOffer = sd.PPOffer,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CompanyName = p.CompanyName,
                                        CategoryName = p.CategoryName,
                                        Compressor = sd.Compressor,
                                        Motor = sd.Motor,
                                        Service = sd.Service,
                                        Spareparts = sd.Spareparts,
                                        Panel = sd.Panel,
                                    }).ToList();

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPDAmount,
                                            w.PPDPercentage,
                                            w.PPOffer,
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList()
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName + "," + item.CategoryName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.TotalAmount));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                var sysInfo = GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("FlatPercentage", oOrder.TDPercentage.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TDiscount", oOrder.NetDiscount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Code", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PreviousDue", (customer.TotalDue - oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                if (concernID == (int)EnumSisterConcern.AP_COMPUTER || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_1 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_2 || concernID == (int)EnumSisterConcern.AP_ELECTRONICS_3)
                {
                    _reportParameter = new ReportParameter("Msg", " বি: দ্র: মেমো ছাড়া কোন প্রকার লেনদেন করিবেন না।করিলে কর্তৃপক্ষ দায়ি নহে।");
                    _reportParameters.Add(_reportParameter);
                }
                else if (concernID == (int)EnumSisterConcern.Ityadi_Electronic || concernID == (int)EnumSisterConcern.SHOPNO_PURON)
                {
                    _reportParameter = new ReportParameter("Msg", "এই পন্যটির বিক্রয়লব্ধ অর্থের একটি অংশ গরিব অসহায় শিশুদের সু - চিকিৎসার জন্য নিবেদিত।");
                    _reportParameters.Add(_reportParameter);
                }

                if (concernID == (int)EnumSisterConcern.Niyamot)
                {
                    _reportParameter = new ReportParameter("Msg", " ব্যালেন্সের অথবা পন্য ও পন্যর মূল্যের কোন গড়মিল পরিলক্ষিত হলে 03 দিনের মধ্যে অবহিত করুন। অন্যথায় আপনার দিক হতে হিসাব ঠিক বলে বিবেচিত হবে। আদেশক্রমে কৃর্তপক্ষ");
                    _reportParameters.Add(_reportParameter);
                }
                //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice.rdlc");

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
                var checkTramsAndCodition = currentSystemInfo.TramsAndCondition;

                if (concernID == (int)EnumSisterConcern.Beauty_1 || concernID == (int)EnumSisterConcern.Beauty_2)
                {
                    //_reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                    //_reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceWithOutBarcode_beauty.rdlc");
                }

                if (sysInfo.IsSalesPPDiscountShow == 1)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceWithOutBarcode.rdlc");
                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceWPPDWithOutBarcode.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public byte[] SalesInvoiceWithOutBarcodeReport(SOrder oOrder, string userName, int concernID)
        {
            return SalesInvoiceWithOutBarcodeReportPrint(oOrder, userName, concernID);
        }

        public byte[] SalesInvoiceWithOutBarcodeReport(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return SalesInvoiceWithOutBarcodeReportPrint(oOrder, userName, concernID);

        }



        public byte[] ServiceCharge(int Month, int Year, string UserName, int ConcernID, DateTime fromDate)
        {
            try
            {
                var chargeinfos = _CashCollectionService.ServiceCharge(Month, Year).ToList();
                DataRow row = null;
                _dataSet = new DataSet();
                TransactionalDataSet.dtServiceChargeDataTable dt = new TransactionalDataSet.dtServiceChargeDataTable();
                foreach (var item in chargeinfos)
                {
                    row = dt.NewRow();

                    row["Name"] = item.ConcernName;
                    row["TransactionDate"] = item.TransactionDate;
                    row["FromMobileNo"] = item.PaymentMobNo;
                    row["ServiceCharge"] = item.ServiceCharge;
                    dt.Rows.Add(row);

                }
                dt.TableName = "dtServiceCharge";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(UserName, ConcernID);

                _reportParameter = new ReportParameter("Month", "Service Charge Month of : " + fromDate.ToString("MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "bKashReport\\ServiceCharge.rdlc");
            }

            catch (Exception ex)
            {
                throw ex;
            }



        }


        public byte[] CashCollectionReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID,
        int customerId, int ReportType)
        {
            try
            {
                _dataSet = new DataSet();
                var CashCollectionInfos = _CashCollectionService.CashCollectionReportData(fromDate, toDate, concernID, customerId, ReportType).ToList();
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                DataRow row = null;

                foreach (var item in CashCollectionInfos)
                {
                    row = dt.NewRow();
                    row["CName"] = item.CustomerName;
                    row["CollDate"] = item.EntryDate;
                    row["CashType"] = item.ModuleType;
                    row["BankName"] = item.BankName;
                    row["AccountNo"] = item.AccountNo;
                    row["BranchName"] = item.BranchName;
                    row["ChequeNo"] = item.ChecqueNo;
                    row["RecAmt"] = item.Amount;
                    row["AdjustmentAmt"] = item.AdjustAmt;
                    row["TotalDue"] = item.TotalDue;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "Cash Collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionNew.rdlc");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        public byte[] HireReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID)
        {
            TransactionalDataSet.dtReturnInvoiceDataTable dt = new TransactionalDataSet.dtReturnInvoiceDataTable();
            _dataSet = new DataSet();
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerId);

            foreach (var item in ROrderDetails)
            {
                dt.Rows.Add(item.DamageProductName, item.DamageIMEINO, item.UnitPrice, item.Quantity, item.UnitPrice * item.Quantity);
            }

            dt.TableName = "dtReturnInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.OrderDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", "Remarks: " + ROrder.Remarks);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptHireReturnInvoice.rdlc");
        }

        public byte[] HireReturnInvoiceReportByID(int orderId, string username, int concernID)
        {
            TransactionalDataSet.dtReturnInvoiceDataTable dt = new TransactionalDataSet.dtReturnInvoiceDataTable();
            var ROrder = _creditSalesOrderService.GetSalesOrderById(Convert.ToInt32(orderId));
            var rorderdetails = _creditSalesOrderService.GetHireReturnDetailReportByReturnID(orderId, concernID);
            var HROrder = _creditSalesOrderService.GetHireSalesReturnOrderById(Convert.ToInt32(orderId));

            Customer customer = _customerService.GetCustomerById(ROrder.CustomerID);
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in rorderdetails)
            {
                row = dt.NewRow();
                row["ProductName"] = item.Item3;
                row["IMEI"] = item.Item6;
                row["UnitPrice"] = item.Item5;
                row["ReturnDate"] = item.Item1.ToShortDateString();
                row["Quantity"] = 1;
                row["Amount"] = item.Rest.Item3;
                dt.Rows.Add(row); ;
            }

            dt.TableName = "dtReturnInvoice";
            _dataSet.Tables.Add(dt);
            #region Parameter
            GetCommonParameters(username, concernID);

            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.ReturnDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MemoNo", HROrder.MemoNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", HROrder.Remarks);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("AdjAmt", HROrder.AdjDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ToCusPay", HROrder.ToCustomerPayAmt.ToString());
            _reportParameters.Add(_reportParameter);
            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptHireReturnInvoice.rdlc");
        }

        public byte[] UserAuditDetailsReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, EnumObjectType ObjectType)
        {
            TransactionalDataSet.dtUserAuditDataTable dt = new TransactionalDataSet.dtUserAuditDataTable();

            var userauditdetails = _userAuditDetailService.GetUserAuditReport(fromDate, toDate, ConcernID, ObjectType);
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in userauditdetails)
            {
                row = dt.NewRow();
                row["EntryDate"] = item.EntryDate;
                row["InvoiceDate"] = item.InvoiceDate.ToShortDateString();
                row["InvoiceNo"] = item.InvoiceNo;
                row["ObjectType"] = item.ObjectType;
                row["Name"] = item.Name;
                row["ActionType"] = item.ActionType;
                row["UserName"] = item.UserName;
                row["UserRole"] = item.UserRole;
                dt.Rows.Add(row); ;
            }

            dt.TableName = "dtUserAudit";
            _dataSet.Tables.Add(dt);
            #region Parameter
            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("ReportHeader", "User Audit Details From Date: " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptUserAuditDetails.rdlc");
        }

        public byte[] GetBankLoanInvoice(int loanId, string userName, int concernID)
        {
            DataTable orderdDT = new DataTable();
            TransactionalDataSet.dtBankLoanDataTable dt = new TransactionalDataSet.dtBankLoanDataTable();
            TransactionalDataSet.dtLoanScheduleDataTable dtInstallment = new TransactionalDataSet.dtLoanScheduleDataTable();
            DataRow row = null;

            #region linq data
            BankLoan bankLoan = _bankLoanService.GetById(loanId);

            if (bankLoan != null)
            {
                Bank bank = _BankService.GetBankById(bankLoan.BankId);
                row = dt.NewRow();
                row["BankName"] = bank.BankName;
                row["TotalLoanAmount"] = bankLoan.TotalLoanAmount;
                row["InterestAmount"] = (bankLoan.PrincipleLoanAmount * bankLoan.InterestPercentage) / 100;
                row["ProcessingFee"] = (bankLoan.PrincipleLoanAmount * bankLoan.ProcessingFeePercentage) / 100;
                row["LoanDate"] = bankLoan.LoanDate.ToString("dd MMM, yyyy");
                row["PrincipleLoanAmount"] = bankLoan.PrincipleLoanAmount;
                dt.Rows.Add(row);
            }

            #endregion

            #region dtSchedules
            if (bankLoan.BankLoanDetails.Any())
            {
                foreach (var schedule in bankLoan.BankLoanDetails)
                {
                    row = dtInstallment.NewRow();
                    row["ScheduleNo"] = schedule.ScheduleNo;
                    row["ScheduleDate"] = schedule.InstallmentDate.ToString("dd MMM, yyyy");
                    row["OpeningBalance"] = schedule.OpeningBalance;
                    row["InstallmentAmount"] = schedule.ExpectedInstallmentAmount;
                    row["ClosingBalance"] = schedule.ClosingBalance;
                    row["Status"] = schedule.Status;
                    row["CollectedAmount"] = schedule.Status.Equals("Paid") ? schedule.InstallmentAmount : 0m;
                    dtInstallment.Rows.Add(row);
                }
            }
            #endregion

            dt.TableName = "dtBankLoan";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            dtInstallment.TableName = "dtLoanSchedule";
            _dataSet.Tables.Add(dtInstallment);

            #region params
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("InvoiceNo", bankLoan.Code);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", bankLoan.LoanDate.ToString("dd MMM, yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
            _reportParameters.Add(_reportParameter);
            #endregion
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "BankLoan\\BankLoanInvoice.rdlc");
        }

        public byte[] GetBankLoanCollectionInvoice(int loanCollectionId, string userName, int concernID)
        {

            RPTBankLoanCollectionInvTO loanCollection = _bankLoanCollectionService.GetLoanCollectionInvoiceData(loanCollectionId);



            #region params
            _dataSet = new DataSet();
            GetCommonParameters(userName, concernID);

            if (loanCollection != null && loanCollection.CollectionType.Equals("Normal"))
            {
                _reportParameter = new ReportParameter("BankName", loanCollection.BankName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReceiptNo", loanCollection.ReceiptNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CollectionDate", loanCollection.CollectionDate.ToString("dd MMM, yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("LoanAmount", loanCollection.LoanAmount.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReceiveAmount", loanCollection.ReceiveAmount.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InstallmentDate", loanCollection.InstallmentDate.ToString("dd MMM, yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("SDPS", loanCollection.SDPS.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Savings", loanCollection.Savings.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                decimal totalAmount = loanCollection.LoanAmount + loanCollection.SDPS + loanCollection.Savings;

                _reportParameter = new ReportParameter("TotalAmount", totalAmount.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                string sInwodTk = TakaFormat((double)totalAmount);
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "BankLoan\\rptBankLoanCollectionInvoice.rdlc");
            }
            #endregion

            else
            {
                _reportParameter = new ReportParameter("BankName", loanCollection.BankName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReceiptNo", loanCollection.ReceiptNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CollectionDate", loanCollection.CollectionDate.ToString("dd MMM, yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("LoanAmount", loanCollection.LoanAmount.ToString("#.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReceiveAmount", loanCollection.ReceiveAmount.ToString("#.00"));
                _reportParameters.Add(_reportParameter);


                string sInwodTk = TakaFormat((double)loanCollection.ReceiveAmount);
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "BankLoan\\rptBankCCLoanCollectionInvoice.rdlc");
            }

        }

        public byte[] GetPendingBankLoan(string userName, int concernID)
        {
            DataTable orderdDT = new DataTable();
            TransactionalDataSet.dtPendingLoanDataTable dt = new TransactionalDataSet.dtPendingLoanDataTable();
            DataRow row = null;

            #region linq data
            DateTime currentDate = Convert.ToDateTime(GetClientDateTime());
            List<RPTBankDueLoanTO> dueLoanList = _bankLoanCollectionService.GetAllPendingLoanAsOnDate(currentDate);

            if (dueLoanList.Any())
            {
                foreach (var loan in dueLoanList)
                {
                    row = dt.NewRow();
                    row["LoanCode"] = loan.LoanCode;
                    row["BankName"] = loan.BankName;
                    row["ScheduleNo"] = loan.ScheduleNo;
                    row["InstallmentDate"] = loan.InstallmentDate.ToString("dd MMM, yyyy");
                    row["Status"] = loan.Status;
                    row["InstallmentAmount"] = loan.InstallmentAmount;
                    dt.Rows.Add(row);
                }
            }

            #endregion

            dt.TableName = "dtPendingLoan";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);


            #region params
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TodaysDate", currentDate.ToString("dd MMM, yyyy"));
            _reportParameters.Add(_reportParameter);
            #endregion
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "BankLoan\\rptPendingBankLoan.rdlc");
        }

        public string GetClientDateTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);
            return localDateTime.ToString("dd MMM yyyy HH:mm:ss");
        }


        public byte[] CustomerAdjustmentReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
    EnumTranType AdjustmentType, int SelectedConcernID, int CustomerId)
        {
            try
            {
                var CustomersAdjustment = _CashCollectionService.GetCustomerDateWiseAdjustmet(concernID, fromDate, toDate, AdjustmentType, CustomerId);

                TransactionalDataSet.dtDebitAdjustmentDataTable dt = new TransactionalDataSet.dtDebitAdjustmentDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in CustomersAdjustment)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date.ToShortDateString();
                    //row["Date"] = item.Date.ToString();
                    row["CustomerName"] = item.CustomerName;
                    row["ReceiptNo"] = item.ReceiptNo;
                    row["Amount"] = item.AdjutmentAmt;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDebitAdjustment";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Customer Debit or Credit Adjustment Report From Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\DebitCreditAdjustmentReport.rdlc");
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (log, rethrow, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }



        public byte[] SupplierAdjustmentReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
        EnumTranType AdjustmentType, int SelectedConcernID, int SupplierId)
        {
            try
            {
                var CustomersAdjustment = _CashCollectionService.GetSupplierDebitAdjustment(concernID, fromDate, toDate, AdjustmentType, SupplierId);

                TransactionalDataSet.dtDebitAdjustmentDataTable dt = new TransactionalDataSet.dtDebitAdjustmentDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in CustomersAdjustment)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date.ToShortDateString();
                    //row["Date"] = item.Date.ToString();
                    row["CustomerName"] = item.CustomerName;
                    row["ReceiptNo"] = item.ReceiptNo;
                    row["Amount"] = item.AdjutmentAmt;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDebitAdjustment";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Customer Debit or Credit Adjustment Report From Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\SupplierDebitCreditAdjustmentReport.rdlc");
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (log, rethrow, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public byte[] DiscountAdjReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID,
        int customerId, int ReportType)
        {
            try
            {
                _dataSet = new DataSet();
                var CashCollectionInfos = _CashCollectionService.DiscountAdjReportData(fromDate, toDate, concernID, customerId, ReportType).ToList();
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                DataRow row = null;

                foreach (var item in CashCollectionInfos)
                {
                    row = dt.NewRow();
                    row["CName"] = item.CustomerName;
                    row["CollDate"] = item.EntryDate;
                    row["CashType"] = item.ModuleType;
                    row["InvoiceNo"] = item.ReceiptNo;
                    row["TotalDis"] = item.TotalDis;
                    row["AdjustmentAmt"] = item.AdjustAmt;


                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "Discount & Adjustment Report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptDisAdjReport.rdlc");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        public byte[] TransferReportNewFormat(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int FromConern, int ToConcern)
        {
            TransactionalDataSet.dtTransferReportDataTable dtTransfer = new TransactionalDataSet.dtTransferReportDataTable();
            _dataSet = new DataSet();
            var Details = _TransferService.GetTransferReportFromTo(fromDate, toDate, FromConern, ToConcern);

            var GProducts = from d in Details
                            group d by new
                            {
                                d.ChallanNo,
                                d.Date,
                                d.NetTotal,
                                d.ProductID,
                                d.ProductName,
                                d.ProductCode,
                                d.ColorName,
                                d.CategoryName,
                                d.CompanyName,
                                d.GodownName,
                                d.MRP,
                                d.TotalAmount,
                                d.FromGodownName
                            } into g
                            select new
                            {
                                TransferNo = g.Key.ChallanNo,
                                Date = g.Key.Date,
                                g.Key.NetTotal,
                                FromConcernName = g.Select(i => i.FromConcernName).FirstOrDefault(),
                                ToConcernName = g.Select(i => i.ToConcernName).FirstOrDefault(),
                                g.Key.ProductCode,
                                g.Key.ProductName,
                                g.Key.CategoryName,
                                g.Key.CompanyName,
                                g.Key.ColorName,
                                g.Key.GodownName,
                                g.Key.FromGodownName,
                                g.Key.MRP,
                                TotalAmount = g.Key.MRP * g.Sum(i => i.Quantity),
                                IMEIs = g.Select(i => i.IMENO).ToList(),
                                Quantity = g.Sum(i => i.Quantity)
                            };

            DataRow row = null;
            string IMEI = string.Empty;
            foreach (var item in GProducts)
            {
                row = dtTransfer.NewRow();
                row["TransferDate"] = item.Date.ToString("dd MMM yyyy");
                row["TransferNo"] = item.TransferNo;
                row["FromConcern"] = item.FromConcernName;
                row["ToConcern"] = item.ToConcernName;
                row["NetTotal"] = item.NetTotal;
                row["ProductCode"] = item.ProductCode;
                row["ProductName"] = item.ProductName + ", " + item.CategoryName;
                row["ColorName"] = item.ColorName;
                row["CategoryName"] = item.CategoryName;
                row["CompanyName"] = item.CompanyName;
                row["ToGodown"] = item.GodownName;
                for (int i = 0; i < item.IMEIs.Count(); i++)
                {
                    if (i < item.IMEIs.Count() - 1)
                        IMEI = IMEI + item.IMEIs[i] + Environment.NewLine;
                    else
                        IMEI = IMEI + item.IMEIs[i];
                }
                row["IMEI"] = IMEI;
                row["PRate"] = item.MRP;
                row["Quantity"] = item.Quantity;
                row["TotalAmt"] = item.TotalAmount;
                row["FromGodown"] = item.FromGodownName;
                dtTransfer.Rows.Add(row);
                IMEI = string.Empty;
            }

            dtTransfer.TableName = "dtTransferReport";
            _dataSet.Tables.Add(dtTransfer);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReportHeader", "Transfer Order From Date: " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Transfer\\TransferReportNew.rdlc");
        }


        public byte[] PurchaseReportNew(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType,
            string period, EnumPurchaseType PurchaseType, bool IsAdminReport, int SelectedConcernID)
        {
            try
            {

                if (reportType == 1)
                {
                    var purchaseInfos = _purchaseOrderService.GetPurchaseReport(fromDate, toDate, PurchaseType, IsAdminReport, SelectedConcernID);

                    DataRow row = null;

                    TransactionalDataSet.dtReceiveOrderDataTable dt = new TransactionalDataSet.dtReceiveOrderDataTable();
                    //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                    foreach (var item in purchaseInfos)
                    {
                        row = dt.NewRow();
                        row["CompanyCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["OrderDare"] = item.Item3.ToString("dd MMM yyyy");
                        row["ChallanNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DisAmt"] = item.Item6;
                        row["TotalAmt"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmt"] = item.Rest.Item2;
                        row["ConcernName"] = item.Rest.Item3;
                        row["InvoiceNo"] = item.Rest.Item4;

                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtReceiveOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    GetCommonParameters(userName, concernID);
                    if (PurchaseType == EnumPurchaseType.Purchase)
                        _reportParameter = new ReportParameter("Month", "Purchase report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (PurchaseType == EnumPurchaseType.ProductReturn)
                        _reportParameter = new ReportParameter("Month", "Purchase Return report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));

                    _reportParameters.Add(_reportParameter);
                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseOrder.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseOrder.rdlc");
                }
                else
                {
                    var Purchases = _purchaseOrderService.ProductWisePurchaseDetailsReport(0, 0, 0, fromDate, toDate, EnumPurchaseType.Purchase, IsAdminReport, SelectedConcernID, 0);


                    decimal TotalDuePurchase = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;
                    decimal TotalPPDis = 0;
                    decimal OnlyDisAmt = 0;
                    int POrderID = 0;
                    DataRow row = null;
                    if (Purchases != null)
                    {
                        TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();
                        GrandTotal = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().GrandTotal);
                        TotalPPDis = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().TotalPPDis);
                        OnlyDisAmt = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().OnlyDisAmt);
                        TotalDis = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().NetDiscount);
                        NetTotal = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().NetTotal);
                        RecAmt = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().RecAmt);
                        CurrDue = Purchases.GroupBy(d => d.ChallanNo).Sum(d => d.First().PaymentDue);







                        foreach (var item in Purchases)
                        {
                            if (POrderID != item.POrderID)
                            {
                                TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                                //GrandTotal = GrandTotal + item.GrandTotal;
                                //TotalDis = TotalDis + item.NetDiscount;
                                //NetTotal = NetTotal + (item.GrandTotal - item.NetDiscount);
                                //RecAmt = RecAmt + item.RecAmt;
                                //CurrDue = CurrDue + item.PaymentDue;
                                //TotalPPDis = TotalPPDis + (item.NetDiscount - item.OnlyDisAmt);
                                //OnlyDisAmt = OnlyDisAmt + item.OnlyDisAmt;

                            }
                            //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                            //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, item.Rest.Rest.Item2, item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);
                            POrderID = item.POrderID;
                            row = dt.NewRow();
                            row["PurchaseDate"] = item.Date;
                            row["ChallanNo"] = item.ChallanNo;
                            row["ProductName"] = item.ProductName;
                            row["PurchaseRate"] = item.AfterFlatDisPurchaseRate;
                            row["DisAmt"] = item.PPDISAmt;
                            row["NetAmt"] = item.TotalAmount;
                            row["GrandTotal"] = item.GrandTotal;
                            row["TotalDis"] = item.NetDiscount;
                            row["NetTotal"] = (item.GrandTotal - item.NetDiscount); //item.TotalAmount;
                            row["PaidAmt"] = item.RecAmt;
                            row["RemainingAmt"] = item.PaymentDue;
                            row["Quantity"] = item.Quantity;
                            row["ChasisNo"] = string.Join(Environment.NewLine, item.IMEIs);
                            row["Model"] = item.CategoryName;
                            row["Color"] = item.ColorName;
                            row["PPOffer"] = item.PPOffer;
                            row["DamageIMEI"] = item.DamageIMEI;
                            row["ConcernName"] = item.ConcenName;
                            row["InvoiceNo"] = item.InvoiceNo;
                            row["OnlyDisAmt"] = item.OnlyDisAmt;
                            row["TotalPPDis"] = item.TotalPPDis;
                            POrderID = item.POrderID;
                            dt.Rows.Add(row);
                        }

                        dt.TableName = "dtSuppWiseData";
                        _dataSet = new DataSet();
                        _dataSet.Tables.Add(dt);

                        GetCommonParameters(userName, concernID);
                        if (PurchaseType == EnumPurchaseType.ProductReturn)
                        {

                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details the Year : " + fromDate.ToString("yyyy"));
                        }
                        else
                        {
                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase details the Year : " + fromDate.ToString("yyyy"));
                        }

                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("OnlyDisAmt", OnlyDisAmt.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("TotalPPDis", TotalPPDis.ToString());
                        _reportParameters.Add(_reportParameter);

                    }

                    if (IsAdminReport)
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseDetails.rdlc");
                    else
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseDetails.rdlc");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public byte[] GetTrialBalanceNew(DateTime fromDate, DateTime toDate, string UserName, int ConcernID,
           string ClientDateTime, int selectedConcernID, bool IsAdminreport)
        {
            List<RPTTrialBalance> totaltrialBalance = new List<RPTTrialBalance>();
            List<RPTTrialBalance> trialBalances = new List<RPTTrialBalance>();
            if (IsAdminreport)
            {
                if (selectedConcernID > 0)
                {
                    trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, selectedConcernID).ToList();

                    if (trialBalances.Any())
                    {
                        var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, selectedConcernID, toDate);
                        if (cDueList != null && cDueList.Any())
                        {
                            decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                            trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Suspense Account")).FirstOrDefault().Credit -= totalDue;
                        }
                        totaltrialBalance.AddRange(trialBalances);
                    }
                    //if (trialBalances.Count() > 0)
                    //    totaltrialBalance.AddRange(trialBalances);
                }
                else
                {
                    var concerns = _SisterConcernService.GetFamilyTree(ConcernID);
                    foreach (var item in concerns)
                    {
                        trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, item.ConcernID).ToList();

                        if (trialBalances.Any())
                        {
                            var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, item.ConcernID, toDate);
                            if (cDueList != null && cDueList.Any())
                            {
                                decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                                trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Suspense Account")).FirstOrDefault().Credit -= totalDue;
                            }

                            foreach (var balance in trialBalances)
                            {
                                balance.ConcernName = item.Name;
                            }
                            totaltrialBalance.AddRange(trialBalances);
                            trialBalances = null;
                        }


                    }
                }
            }
            else
            {
                trialBalances = _AccountingService.GetTrialBalance(fromDate, toDate, ConcernID).ToList();
                if (trialBalances.Any())
                {
                    var cDueList = _salesOrderService.GetCustomerWiseTotalDueByDate(0, ConcernID, toDate);
                    if (cDueList != null && cDueList.Any())
                    {
                        decimal totalDue = cDueList.Sum(d => d.TotalDue + d.CrInterestAmount + d.TInterestAmount);
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Customer Due")).FirstOrDefault().Credit = totalDue;
                        trialBalances.Where(d => !string.IsNullOrEmpty(d.CreditParticular) && d.CreditParticular.Equals("Suspense Account")).FirstOrDefault().Credit -= totalDue;
                    }
                    totaltrialBalance.AddRange(trialBalances);
                }

            }

            TransactionalDataSet.dtTrialNewDataTable dt = new TransactionalDataSet.dtTrialNewDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in totaltrialBalance)
            {
                row = dt.NewRow();
                row["DebitParticular"] = item.DebitParticular;
                row["Debit"] = item.Debit.HasValue ? item.Debit.Value : 0m;
                row["CreditParticular"] = item.CreditParticular;
                row["Credit"] = item.Credit.HasValue ? item.Credit.Value : 0m;
                row["ConcernName"] = item.ConcernName;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtTrialNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Trial Balance of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            if (selectedConcernID > 0)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptTrialBalance.rdlc");
            }

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptAdminTrialBalance.rdlc");
        }





        public byte[] AdminCashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int SelectedConcern)
        {

            var CashInHandData = _CashCollectionService.AdminCashInHandReport(fromDate, toDate, ReportType, concernID, SelectedConcern).ToList();
            TransactionalDataSet.dtAdminCashInHandDataTable dt = new TransactionalDataSet.dtAdminCashInHandDataTable();
            _dataSet = new DataSet();
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double OpeningCashInhand = 0;
            double CurrentCashInhand = 0;
            double ClosingCashInhand = 0;

            var DataForTable = CashInHandData.Where(o => o.Expense != "Total Payable" && o.Income != "Total Receivable" && o.Expense != "Current Cash In Hand" && o.Income != "Closing Cash In Hand" && o.Income != "Opening Cash In Hand").ToList();
            var DataForTotal = CashInHandData.Where(o => o.Expense == "Total Payable" && o.Income == "Total Receivable").ToList();

            var DataForTables = CashInHandData.Where(o => o.OpeningCshInHand > 0).ToList();

            var DataForTabless = CashInHandData.Where(o => o.ClosingCshInHand > 0).ToList();
            var DataForTablesss = CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList();

            DataForTable.AddRange(DataForTables);
            DataForTable.AddRange(DataForTabless);
            DataForTable.AddRange(DataForTablesss);
            foreach (var item in DataForTable)
            {
                dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt, item.Module, item.EmployeeName, item.ConcernName, item.OpeningCshInHand, item.ClosingCshInHand, item.CurrentCshInHand);
            }

            dt.TableName = "dtAdminCashInHand";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            TotalPayable = (double)DataForTotal.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)DataForTotal.Where(i => !(i.Expense.Equals("Header"))).Sum(o => o.IncomeAmt);
            OpeningCashInhand = (double)CashInHandData.Where(o => o.Income == "Opening Cash In Hand").ToList().Sum(o => o.IncomeAmt);
            CurrentCashInhand = (double)CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            ClosingCashInhand = (double)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInhand", OpeningCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CurrentCashInhand", CurrentCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
            {
                if (SelectedConcern == 0)
                    _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the date " + fromDate.ToString("dd MMM yyyy"));
                else
                    _reportParameter = new ReportParameter("DateRange", ((EnumSubCustomerType)SelectedConcern).ToString() + " Cash In Hand of the date " + fromDate.ToString("dd MMM yyyy"));


            }
            else if (ReportType == 2)
                _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the month  " + fromDate.ToString("MMM yyyy"));
            else if (ReportType == 3)
                _reportParameter = new ReportParameter("DateRange", "Cash In Hand of the year  " + fromDate.ToString("yyyy"));

            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            if (concernID == (int)EnumSisterConcern.Beauty_2 || concernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            //if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptKSDailyCashINHand.rdlc");
            //else
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptAdminDailyCashINHand.rdlc");

        }



        public byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName, int concernID,
           int CustomerID, int IsOnlyDue, EnumCustomerType CustomerType, bool IsAdminReport, int SelectedConcernID)
        {
            //var CustomersDue = _salesOrderService.CustomerDueReport(CustomerID, fromDate, toDate, SelectedConcernID, CustomerType, IsOnlyDue, IsAdminReport);

            var CustomersDue = _customerService.GetSingleCustomerDateWiseTotalDue(CustomerID, concernID, fromDate, toDate, IsOnlyDue, CustomerType, SelectedConcernID);
            TransactionalDataSet.dtCustomerDueReportDataTable dt = new TransactionalDataSet.dtCustomerDueReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in CustomersDue)
            {
                row = dt.NewRow();
                row["CustomerType"] = item.CustomerType;
                row["Customer"] = item.CustomerAddress;
                row["OpeningDue"] = item.OpeningDue;
                row["Sales"] = item.Sales;
                row["Collections"] = item.CollectionAmt;
                row["ClosingDue"] = item.ClosingDue;
                row["ReceiveAmt"] = item.SalesReceive;
                row["TotalCollection"] = item.TotalCollection;
                row["Return"] = item.SaleReturn;
                row["InstallmentCollection"] = item.InstallmentCollection;
                row["CashCollectionIntAmt"] = item.CashCollectionInterestAmt;
                row["CrInterestAmt"] = item.HireIntestrestAmt;
                row["TotalAmt"] = item.TotalAmt;
                row["ConcernName"] = item.ConcernName;
                row["CashCollectionReturn"] = item.CollectionReturnAmt;
                row["CCTypeAdjustment"] = item.CashCollectionsTypeAdjustment;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerDueReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("DateRange", "Customer Due Report  From Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);


            if (IsAdminReport)
            {
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptCustomerDueReportNew.rdlc");
            }
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CustomerReport\\rptCustomerDueReport.rdlc");

        }


        public byte[] AdminProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {
            List<ProfitLossReportModel> data = new List<ProfitLossReportModel>();
            List<ProfitLossReportModel> multiConcernData = new List<ProfitLossReportModel>();
            var sisterConcernList = _SisterConcernService.GetFamilyTree(ConcernID);

            foreach (var iteam in sisterConcernList)
            {
                multiConcernData = _AccountingService.ProfitLossAccount(fromDate, toDate, iteam.ConcernID).ToList();
                foreach (var item in multiConcernData)
                {
                    item.ConcernName = iteam.Name;
                }
                data.AddRange(multiConcernData);
                multiConcernData = null;
            }

            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal debitAmt = data.Where(i => !((i.DebitParticulars.Equals("Purchase Return"))
                            || (i.DebitParticulars.Equals("Purchase"))
                            || (i.DebitParticulars.Equals("Total Expense"))
                            )).Sum(i => i.Debit);

            decimal creditAmt = data.Where(i => !((i.CreditParticulars.Equals("Gross Profit"))
                || (i.CreditParticulars.Equals("Sales"))
                || (i.CreditParticulars.Equals("Sales Return"))
                || (i.CreditParticulars.Equals("Total Income"))
                )).Sum(i => i.Credit);
            foreach (var item in data)
            {

                if (item.DebitParticulars == "Total Expense" || item.DebitParticulars == "Net Purchase" || item.DebitParticulars == "Customer Adjustment" || item.DebitParticulars == "Bad Debt.")
                {
                    item.IsHeader = true;
                }
                if (item.CreditParticulars == "Total Income" || item.CreditParticulars == "Net Sales" || item.CreditParticulars == "Customer Adjustment")
                {
                    item.IsCrHeader = true;
                }
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                row["ConcernName"] = item.ConcernName;
                row["IsHeader"] = item.IsHeader;
                row["IsCrHeader"] = item.IsCrHeader;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Profit and Loss Account of the date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", debitAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", creditAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetProfit", (creditAmt - debitAmt).ToString());
            _reportParameters.Add(_reportParameter);

            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(ConcernID);
            if (ConcernID == (int)EnumSisterConcern.Beauty_2 || ConcernID == (int)EnumSisterConcern.Beauty_1)
            {
                _reportParameter = new ReportParameter("CompanyTitle", currentSystemInfo.CompanyTitle);
                _reportParameters.Add(_reportParameter);
            }

            //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptProfitLossAccount.rdlc");
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptAdminProfitLossAccount.rdlc");
        }

        public byte[] ProductWiseBenefitReportNew(DateTime fromDate, DateTime toDate, int ProductID, int CompanyID, int CategoryID, string userName, int concernID)
        {
            var Data = _salesOrderService.ProductWiseSalesBenefit(CompanyID, CategoryID, ProductID, fromDate, toDate);
            Data = Data.OrderBy(i => i.Date).ToList();
            _dataSet = new DataSet();
            TransactionalDataSet.dtBenefitRptDataTable dt = new TransactionalDataSet.dtBenefitRptDataTable();
            if (ProductID != 0)
                Data = Data.Where(i => i.ProductID == ProductID).ToList();

            foreach (var item in Data)
            {
                dt.Rows.Add(item.InvoiceNo, item.ProductName, item.CategoryName, item.IMEI, item.SalesTotal, item.Discount, item.NetSales, item.PurchaseTotal, item.CommisionProfit, item.HireProfit, item.HireCollection, item.TotalProfit, item.Date, item.CompanyName);
            }


            dt.TableName = "dtBenefitRpt";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("Month", "Product Wise Benefit Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\ProductWiseBenefitReportNew.rdlc");
        }

        public byte[] StockLedgerReportExcel(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int filetype)
        {
            try
            {

                List<StockLedger> DataGroupBy = _StockServce.GetStockLedgerReport(reportType, CompanyName, CategoryName, ProductName, fromDate, toDate, concernID).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable();

                foreach (var item in DataGroupBy)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date;
                    row["ConcernID"] = item.ConcernID;
                    row["ProductID"] = item.ProductID;
                    row["Code"] = item.Code;
                    row["ProductName"] = item.ProductName;
                    row["ColorID"] = item.ColorID;
                    row["ColorName"] = item.ColorName;
                    row["OpeningStockQuantity"] = item.OpeningStockQuantity;
                    row["TotalStockQuantity"] = item.TotalStockQuantity;
                    row["PurchaseQuantity"] = item.PurchaseQuantity;
                    row["SalesQuantity"] = item.SalesQuantity;
                    row["SalesReturnQuantity"] = item.SalesReturnQuantity;
                    row["ClosingStockQuantity"] = item.ClosingStockQuantity;
                    row["OpeningStockValue"] = item.OpeningStockValue;
                    row["TotalStockValue"] = item.TotalStockValue;
                    row["ClosingStockValue"] = item.ClosingStockValue;
                    row["PurchaseReturn"] = item.PurchaseReturnQuantity;
                    row["TransferIN"] = item.TransferInQuantity;
                    row["TransferOUT"] = item.TransferOutQuantity;
                    row["RepQty"] = item.RepQty;
                    row["CategoryName"] = item.CategoryName;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDailyStockandSalesSummary";


                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                reportName = "Stock\\rptStockLedgerExcel.rdlc";

                _reportParameter = new ReportParameter("DateRange", "Stock Ledger From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);



                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, reportName, filetype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] WarrantyInvoice(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return WarrantyInvoicePrint(oOrder, userName, concernID);

        }

        public byte[] EmobileSalesInvoiceReportPrint(SOrder oOrder, string userName, int concernID, bool IsFakeInvoice)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);
                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                string Warranty = string.Empty;
                string Warrans = string.Empty;
                int Count = 0;
                var ProductInfos = from sd in oOrder.SOrderDetails
                                   join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                   join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       PPDPercentage = sd.PPDPercentage,
                                       PPDAmount = sd.PPDAmount,
                                       PPOffer = sd.PPOffer,
                                       IMENO = std.IMENO,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       Compressor = sd.Compressor,
                                       Motor = sd.Motor,
                                       Service = sd.Service,
                                       Spareparts = sd.Spareparts,
                                       Panel = sd.Panel,
                                       Warranty = sd.Warranty
                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new { w.ProductName, w.CategoryName, w.ColorName, w.CompanyName, w.UnitPrice, w.PPDAmount, w.PPDPercentage, w.PPOffer, w.UTAmount } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            SalesRate = g.Key.UTAmount,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList(),
                                            Warranty = g.Select(i => i.Warranty).ToList(),
                                        };

                List<string> productInfos = new List<string>();
                List<string> productWarrantys = new List<string>();
                foreach (var item in GroupProductInfos)
                {
                    string productInfo = "<" + item.ProductName + "> ";

                    string prImeis = string.Join(",", item.IMENOs);

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + ",";
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    productInfo = productInfo + prImeis + " ";
                    productInfos.Add(productInfo);

                    string productWarranty = "<" + item.ProductName + "> ";

                    string prWarran = string.Join(",", item.Warranty);

                    foreach (var WARR in item.Warranty)
                    {
                        Count++;
                        if (item.Warranty.Count() != Count)
                            Warrans = Warrans + WARR + ",";
                        else
                            Warrans = Warrans + WARR;
                    }

                    if (prWarran == "0")
                    {
                        productWarranty = " ";
                        productWarrantys.Add(productWarranty);
                    }
                    else
                    {
                        productWarranty = productWarranty + prWarran + " ";
                        productWarrantys.Add(productWarranty);
                    }


                    dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " " + item.CategoryName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Warranty = string.Empty;
                    Warrans = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);


                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.RecAmount.ToString()));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");

                string sFInwodTk = TakaFormat(Convert.ToDouble(oOrder.GrandTotal.ToString()));
                sFInwodTk = sFInwodTk.Replace("Taka", "");
                sFInwodTk = sFInwodTk.Replace("Only", "Taka Only");



                //string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.RecAmount));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("FlatPercentage", oOrder.TDPercentage.ToString("0.00"));
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("TDiscount", oOrder.NetDiscount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);


                if (IsFakeInvoice)
                    _reportParameter = new ReportParameter("Total", (oOrder.GrandTotal).ToString("0.00"));
                else
                    _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                string allProductInfos = string.Join(" ", productInfos);

                string allproductWarrantys = string.Join(" ", productWarrantys);

                _reportParameter = new ReportParameter("ProductWithModel", allProductInfos);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ProductWarranty", allproductWarrantys);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);
                if (IsFakeInvoice)
                    _reportParameter = new ReportParameter("Paid", oOrder.GrandTotal.ToString());
                else
                    _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);
                if (IsFakeInvoice)
                    _reportParameter = new ReportParameter("CurrDue", "0.00");
                else
                    _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("Warranty", oOrder.Warrenty);
                //_reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Code", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("InvoiceTime", oOrder.CreateDate.ToShortTimeString());
                //_reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("InvoiceTime", oOrder.CreateDate.ToShortTimeString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("InvoiceDate", oOrder.CreateDate.ToString());
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PreviousDue", (oOrder.PrevDue).ToString());
                _reportParameters.Add(_reportParameter);
                if (IsFakeInvoice)
                    _reportParameter = new ReportParameter("InWordTK", sFInwodTk);
                else
                    _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                //string sInwodTkDue = TakaFormat(Convert.ToDouble(oOrder.PaymentDue.ToString()));
                //sInwodTkDue = sInwodTkDue.Replace("Taka", "");
                //sInwodTkDue = sInwodTkDue.Replace("Only", "Taka Only");

                //_reportParameter = new ReportParameter("InWordTkDue", sInwodTkDue);
                //_reportParameters.Add(_reportParameter);


                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);

                _reportParameter = new ReportParameter("VatRegNo", currentSystemInfo.VatRegNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);


                if (IsFakeInvoice)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceNoDis.rdlc");
                if (concernID == (int)EnumSisterConcern.GadetHouse)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice_GadgetHouse.rdlc");

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoiceEmobile.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public byte[] EmobileSalesInvoiceReport(SOrder oOrder, string userName, int concernID)
        {
            return EmobileSalesInvoiceReportPrint(oOrder, userName, concernID, false);
        }
        public byte[] EmobileSalesInvoiceReport(int oOrderID, string userName, int concernID, bool IsFakeInvoice)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return EmobileSalesInvoiceReportPrint(oOrder, userName, concernID, IsFakeInvoice);

        }

        public byte[] EmobileCreditSalesInvoiceReportPrint(CreditSale oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.CreditSalesInfoDataTable dt = new TransactionalDataSet.CreditSalesInfoDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);

                DataRow oSDRow = null;
                Product product = null;
                StockDetail oSTDetail = null;
                //Color oColor = null;
                int count = 1;
                string Warrenty = string.Empty;
                string Warranty = string.Empty;
                string Warrans = string.Empty;

                foreach (CreditSalesSchedule item in oOrder.CreditSalesSchedules)
                {
                    oSDRow = dt.NewRow();

                    oSDRow["ScheduleNo"] = count;
                    oSDRow["PaymentDate"] = item.PaymentStatus == "Paid" ? Convert.ToDateTime(item.PaymentDate).ToString("dd MMM yyyy") : "";
                    oSDRow["Balance"] = item.Balance;
                    oSDRow["InstallmetAmt"] = item.InstallmentAmt;
                    oSDRow["ClosingBalance"] = item.ClosingBalance;
                    oSDRow["PaymentStatus"] = item.PaymentStatus;
                    oSDRow["ScheduleDate"] = Convert.ToDateTime(item.MonthDate).ToString("dd MMM yyyy");
                    dt.Rows.Add(oSDRow);
                    count++;


                }

                dt.TableName = "CreditSalesInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                TransactionalDataSet.CSalesProductDataTable CSProductDT = new TransactionalDataSet.CSalesProductDataTable();
                DataRow oCSPRow = null;
                int nCOunt = 1;
                #region Product Details
                var Products = from csd in oOrder.CreditSaleDetails
                               join p in _productService.GetAllProductIQueryable() on csd.ProductID equals p.ProductID
                               join cat in _CategoryService.GetAllIQueryable() on p.CategoryID equals cat.CategoryID
                               join com in _CompanyService.GetAllCompany() on p.CompanyID equals com.CompanyID
                               join sd in _stockdetailService.GetAll() on csd.StockDetailID equals sd.SDetailID
                               join col in _ColorServce.GetAllColor() on sd.ColorID equals col.ColorID
                               select new
                               {
                                   p.ProductID,
                                   p.ProductName,
                                   p.ProductCode,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   IMEI = sd.IMENO,
                                   cat.CategoryID,
                                   ColorName = col.Name,
                                   csd.UnitPrice,
                                   csd.PPOffer,
                                   csd.UTAmount,
                                   csd.Quantity,
                                   csd.Compressor,
                                   csd.Motor,
                                   csd.Panel,
                                   csd.Spareparts,
                                   csd.Service,
                                   csd.Warranty
                               };
                var GroupProducts = from p in Products
                                    group p by new
                                    {
                                        p.ProductID,
                                        p.ProductCode,
                                        p.ProductName,
                                        p.CategoryID,
                                        p.CompanyName,
                                        p.CategoryName,
                                        p.ColorName,
                                        p.UnitPrice,
                                        p.UTAmount,
                                        p.PPOffer
                                    } into g
                                    select new
                                    {
                                        g.Key.ProductID,
                                        g.Key.ProductCode,
                                        g.Key.ProductName,
                                        g.Key.CategoryName,
                                        g.Key.CompanyName,
                                        g.Key.ColorName,
                                        g.Key.UnitPrice,
                                        g.Key.PPOffer,
                                        Quantity = g.Sum(i => i.Quantity),
                                        UTAmount = g.Key.UnitPrice * g.Sum(i => i.Quantity),
                                        Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                        Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                        Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                        Service = g.Select(i => i.Service).FirstOrDefault(),
                                        Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                        IMEIs = g.Select(i => i.IMEI).ToList(),
                                        Warranty = g.Select(i => i.Warranty).ToList()
                                    };

                #endregion

                string IMEIs = string.Empty;
                int Count = 0;
                List<string> productInfos = new List<string>();
                List<string> productWarrantys = new List<string>();
                foreach (var item in GroupProducts)
                {
                    string productInfo = "<" + item.ProductName + "> ";

                    string prImeis = string.Join(",", item.IMEIs);
                    foreach (var IMEI in item.IMEIs)
                    {
                        Count++;
                        if (item.IMEIs.Count() != Count)
                            IMEIs = IMEIs + IMEI + ",";
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    productInfo = productInfo + prImeis + " ";
                    productInfos.Add(productInfo);

                    string productWarranty = "<" + item.ProductName + "> ";

                    string prWarran = string.Join(",", item.Warranty);

                    foreach (var WARR in item.Warranty)
                    {
                        Count++;
                        if (item.Warranty.Count() != Count)
                            Warrans = Warrans + WARR + ",";
                        else
                            Warrans = Warrans + WARR;
                    }

                    productWarranty = productWarranty + prWarran + " ";
                    productWarrantys.Add(productWarranty);

                    oCSPRow = CSProductDT.NewRow();
                    oCSPRow["SLNo"] = nCOunt.ToString();
                    oCSPRow["PName"] = item.ProductName;
                    oCSPRow["CName"] = item.CompanyName + " " + item.CategoryName;
                    oCSPRow["ColorName"] = item.ColorName;
                    oCSPRow["Qty"] = item.Quantity.ToString();
                    oCSPRow["UnitPrice"] = item.UnitPrice.ToString();
                    oCSPRow["PPOffer"] = item.PPOffer.ToString();
                    oCSPRow["TotalAmt"] = item.UTAmount.ToString();
                    foreach (var IMEI in item.IMEIs)
                    {
                        Count++;
                        if (item.IMEIs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }
                    oCSPRow["IMENO"] = IMEIs;
                    Count = 0;
                    CSProductDT.Rows.Add(oCSPRow);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;

                    dtWarrenty.Rows.Add(item.ProductName, IMEIs, Warrenty);
                    Warrenty = string.Empty;
                    IMEIs = string.Empty;
                    Warranty = string.Empty;
                    Warrans = string.Empty;

                }

                CSProductDT.TableName = "CSalesProduct";
                _dataSet.Tables.Add(CSProductDT);

                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.DownPayment));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("IssueDate", oOrder.SalesDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ProductName", "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerName", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CContactNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                string salesprice = (oOrder.TSalesAmt - oOrder.TotalOffer).ToString("F");
                _reportParameter = new ReportParameter("SalesPrice", salesprice);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("DownPayment", oOrder.DownPayment.ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Discount", (oOrder.Discount).ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemainingAmt", (oOrder.TSalesAmt - oOrder.TotalOffer - oOrder.DownPayment - oOrder.Discount).ToString("F"));
                _reportParameters.Add(_reportParameter);

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);

                _reportParameter = new ReportParameter("VatRegNo", currentSystemInfo.VatRegNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerCode", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                if (concernID == (int)EnumSisterConcern.GadetHouse)
                {
                    _reportParameter = new ReportParameter("Msg", " Note: Your mobile phone will be automatically switched off if you don't pay the installments within the due date.");
                    _reportParameters.Add(_reportParameter);
                }

                string allProductInfos = string.Join(" ", productInfos);
                _reportParameter = new ReportParameter("ProductWithModel", allProductInfos);
                _reportParameters.Add(_reportParameter);

                string allproductWarrantys = string.Join(" ", productWarrantys);
                _reportParameter = new ReportParameter("ProductWarranty", allproductWarrantys);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceTime", oOrder.CreateDate.ToShortTimeString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("InvoiceDate", oOrder.CreateDate.ToString());
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);


                if (concernID == (int)EnumSisterConcern.GadetHouse)
                {
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoice_Gadget_House.rdlc");
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoiceEmobile.rdlc");


            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public byte[] EmobileCreditSalesInvoiceReport(CreditSale oOrder, string userName, int concernID)
        {
            return EmobileCreditSalesInvoiceReportPrint(oOrder, userName, concernID);
        }

        public byte[] EmobileCreditSalesInvoiceReportByID(int oOrderID, string userName, int concernID)
        {
            CreditSale oOrder = new CreditSale();
            oOrder = _creditSalesOrderService.GetSalesOrderById(oOrderID);
            oOrder.CreditSalesSchedules = _creditSalesOrderService.GetSalesOrderSchedules(oOrderID).ToList();
            oOrder.CreditSaleDetails = _creditSalesOrderService.GetSalesOrderDetails(oOrderID).ToList();

            return EmobileCreditSalesInvoiceReportPrint(oOrder, userName, concernID);

        }


        public byte[] PrintPOSInvoice(int SOrderID, string userName, int concernID)
        {
            _dataSet = new DataSet();
            TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
            DataRow row = null;
            var SOrder = _salesOrderService.GetSalesOrderById(SOrderID);
            var customer = _customerService.GetCustomerById(SOrder.CustomerID);
            var Details = (from sod in _salesOrderDetailService.GetSOrderDetailsBySOrderID(SOrderID)
                           join p in _productService.GetAllProductIQueryable() on sod.ProductID equals p.ProductID
                           select new
                           {
                               ProductName = p.ProductName,
                               p.CategoryName,
                               p.CompanyName,
                               p.ProductID,
                               sod.UnitPrice,
                               sod.Quantity,
                           }).ToList();
            var gProducts = (from p in Details
                             group p by new { p.ProductID, p.ProductName, p.CategoryName, p.CompanyName, p.UnitPrice } into g
                             select new
                             {
                                 g.Key.ProductName,
                                 g.Key.CategoryName,
                                 g.Key.CompanyName,
                                 UnitPrice = g.Key.UnitPrice,
                                 Quantity = g.Sum(i => i.Quantity),
                             }).ToList();
            string IMEI = string.Empty;
            foreach (var item in gProducts)
            {
                row = dt.NewRow();
                //row["ProductName"] = item.ProductName + "," + Environment.NewLine + item.CategoryName + "," + Environment.NewLine + item.CompanyName;
                //row["ProductName"] = item.ProductName + "," + Environment.NewLine + item.CompanyName;
                row["ProductName"] = item.ProductName;
                row["Quantity"] = item.Quantity;
                row["Rate"] = item.UnitPrice;
                row["Amount"] = item.UnitPrice * item.Quantity;
                dt.Rows.Add(row);
                IMEI = string.Empty;
            }

            dt.TableName = "dtInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("InvoiceNo", SOrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", SOrder.CreateDate.ToString("dd MMM yyyy HH:mm:ss"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TDiscount", SOrder.NetDiscount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Total", SOrder.TotalAmount.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReceiveAmt", SOrder.RecAmount.Value.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CurrDue", SOrder.PaymentDue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            var cusotmer = _customerService.GetCustomerById(SOrder.CustomerID);
            _reportParameter = new ReportParameter("TotalDue", cusotmer.TotalDue.ToString("F2"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "POS\\rptPOSInvoice.rdlc");
        }


        #region DO Report Star Here 
        public byte[] DOInvoiceReport(string userName, int concernID, int DOID)
        {
            try
            {
                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                DataRow row = null;
                Customer customer = null;
                Supplier supplier = null;
                string Name = string.Empty, ContactNo = string.Empty, Address = string.Empty, CompanyName = string.Empty, Header = string.Empty;

                var DOData = _DOService.GetById(DOID);
                var Details = _DOService.GetDetailsById(DOID);
                #region LINQ
                var ProductInfos = (from d in Details
                                    join rp in _productService.GetAllProductIQueryable() on d.ProductID equals rp.ProductID
                                    join dos in _DOService.GetAll() on d.DOID equals dos.DOID
                                    join c in _ColorServce.GetAll() on d.ColorID equals c.ColorID
                                    select new
                                    {
                                        ProductID = d.ProductID.ToString(),
                                        ColorID = d.ColorID.ToString(),
                                        ColorCode = c.Code,
                                        ColorName = c.Name,
                                        ProductName = rp.ProductName,
                                        rp.CategoryName,
                                        DOQty = d.DOQty,
                                        GivenQty = d.GivenQty,
                                        NetDiscount = dos.NetDiscount,
                                        PaidAmount = dos.PaidAmt,
                                        UnitPrice = d.UnitPrice,
                                        TotalAmt = d.TotalAmt,
                                        TotalSoilPrice = d.TotalAmt,
                                        DODID = d.DODID.ToString(),
                                        DOID = d.DOID.ToString(),
                                        SegmentName = "",
                                        DDLiftingPrice = d.DDLiftingPrice,
                                        PPDisPercent = d.PPDisPercent,
                                        PPDisAmt = d.PPDisAmt,
                                        //PPTaxAmount = d.PPTaxAmount,
                                    }).ToList();
                #endregion

                var ppdisAmt = ProductInfos.Sum(d => d.PPDisAmt * d.DOQty);
                double totalPPDisAmt = Convert.ToDouble(ppdisAmt);
                var netsDiscount = ProductInfos[0].NetDiscount;
                double netDiscount = Convert.ToDouble(netsDiscount);
                double FlatDiscount = netDiscount - totalPPDisAmt;

                foreach (var item in ProductInfos)
                {
                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName;
                    row["Quantity"] = item.DOQty;
                    row["UnitQty"] = item.DOQty;
                    row["Rate"] = item.UnitPrice;
                    row["Discount"] = item.NetDiscount;
                    row["PaidAmount"] = item.PaidAmount;
                    row["Amount"] = item.TotalAmt;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = "";
                    row["DisPer"] = item.PPDisPercent;
                    row["DisAmt"] = item.PPDisAmt;
                    row["FlatDis"] = 0m;
                    row["CategoryName"] = item.CategoryName;
                    row["DDLiftingPrice"] = item.DDLiftingPrice;
                    dt.Rows.Add(row);
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                string sInwodTk = TakaFormat(Convert.ToDouble(DOData.TotalAmt));
                GetCommonParameters(userName, concernID);


                _reportParameter = new ReportParameter("TotalPPDiscount", totalPPDisAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalFlatDiscount", FlatDiscount.ToString()); ;
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("InvoiceNo", DOData.DONo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", DOData.Date.ToString());
                _reportParameters.Add(_reportParameter);

                if (DOData.CustomerID > 0)
                {
                    customer = _customerService.GetCustomerById(DOData.CustomerID);
                    Name = customer.Name;
                    ContactNo = customer.ContactNo;
                    Address = customer.Address;
                    CompanyName = customer.CompanyName;
                    Header = "Customer Sales OS Invoice";
                }
                else if (DOData.SupplierID > 0)
                {
                    supplier = _SupplierService.GetSupplierById(DOData.SupplierID);
                    Name = supplier.Name;
                    ContactNo = supplier.ContactNo;
                    Address = supplier.Address;
                    CompanyName = supplier.OwnerName;
                    Header = "Company Purchase OS Invoice";
                }

                _reportParameter = new ReportParameter("Company", CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", "Remarks: " + DOData.Remarks);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Header", Header);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("PaidAmt", DOData.PaidAmt.ToString("F2"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PaymentDue", (DOData.TotalAmt - DOData.PaidAmt).ToString("F2"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "DO\\DOInvoices.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public byte[] DOInvoiceReportExcel(string userName, int concernID, int DOID, int filetype)
        {
            try
            {
                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                DataRow row = null;
                Customer customer = null;
                Supplier supplier = null;
                string Name = string.Empty, ContactNo = string.Empty, Address = string.Empty, CompanyName = string.Empty, Header = string.Empty;

                var DOData = _DOService.GetById(DOID);
                var Details = _DOService.GetDetailsById(DOID);
                #region LINQ
                var ProductInfos = (from d in Details
                                    join rp in _productService.GetAllProductIQueryable() on d.ProductID equals rp.ProductID
                                    join c in _ColorServce.GetAll() on d.ColorID equals c.ColorID
                                    select new
                                    {
                                        ProductID = d.ProductID.ToString(),
                                        ColorID = d.ColorID.ToString(),
                                        ColorCode = c.Code,
                                        ColorName = c.Name,
                                        ProductName = rp.ProductName,
                                        rp.CategoryName,
                                        DOQty = d.DOQty,
                                        GivenQty = d.GivenQty,
                                        MRP = d.MRP,
                                        UnitPrice = d.MRP,
                                        TotalAmt = d.TotalAmt,
                                        TotalSoilPrice = d.TotalAmt,
                                        DODID = d.DODID.ToString(),
                                        DOID = d.DOID.ToString(),
                                        SegmentName = "",
                                        DDLiftingPrice = d.DDLiftingPrice,
                                        //PPDisPercent = d.PPDisPercent,
                                        //PPDisAmt = d.PPDisAmt,
                                        //PPTaxAmount = d.PPTaxAmount,
                                    }).ToList();
                #endregion

                foreach (var item in ProductInfos)
                {
                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName;
                    row["Quantity"] = item.DOQty;
                    row["UnitQty"] = item.DOQty;
                    row["Rate"] = item.UnitPrice;
                    row["Discount"] = item.MRP;
                    row["Amount"] = item.TotalAmt;
                    //row["DisPer"] = item.PPDisPercent;
                    //row["DisAmt"] = item.PPDisAmt;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = "";

                    row["CategoryName"] = item.CategoryName;
                    row["DDLiftingPrice"] = item.DDLiftingPrice;
                    dt.Rows.Add(row);
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                string sInwodTk = TakaFormat(Convert.ToDouble(DOData.TotalAmt));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", DOData.DONo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", DOData.Date.ToString());
                _reportParameters.Add(_reportParameter);

                if (DOData.CustomerID > 0)
                {
                    customer = _customerService.GetCustomerById(DOData.CustomerID);
                    Name = customer.Name;
                    ContactNo = customer.ContactNo;
                    Address = customer.Address;
                    CompanyName = customer.CompanyName;
                    Header = "Customer Sales OS Invoice";
                }
                else if (DOData.SupplierID > 0)
                {
                    supplier = _SupplierService.GetSupplierById(DOData.SupplierID);
                    Name = supplier.Name;
                    ContactNo = supplier.ContactNo;
                    Address = supplier.Address;
                    CompanyName = supplier.OwnerName;
                    Header = "Company Purchase OS Invoice";
                }

                _reportParameter = new ReportParameter("Company", CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", DOData.Remarks);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Header", Header);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PaidAmt", DOData.PaidAmt.ToString("F2"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PaymentDue", (DOData.TotalAmt - DOData.PaidAmt).ToString("F2"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, "DO\\DOInvoicesExcel.rdlc", filetype);
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] DOReport(string Username, int ConcernID, DateTime fromDate, DateTime toDate, int customerID, int SupplierID, int POType)
        {
            try
            {
                TransactionalDataSet.dtDOReportDataTable dt = new TransactionalDataSet.dtDOReportDataTable();
                DataRow row = null;
                IQueryable<Customer> Customers = null;
                IQueryable<Supplier> suppliers = null;
                IQueryable<DO> DOList = null;
                List<RPTDoTO> ProductInfos = new List<RPTDoTO>();
                int DOid = 0;
                decimal NetTamt = 0;
                string Header = string.Empty;
                if (POType == 1)
                {
                    Header = "Comapany Purchase OS Report from Date " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy");
                    if (SupplierID > 0)
                        suppliers = _SupplierService.GetAllSupplier().Where(i => i.SupplierID == SupplierID);
                    else
                        suppliers = _SupplierService.GetAllSupplier();

                    ProductInfos = (from d in _DOService.GetAll()
                                    join c in suppliers on d.SupplierID equals c.SupplierID
                                    join dd in _DOService.GetAllDetail() on d.DOID equals dd.DOID
                                    join p in _productService.GetDOProducts() on dd.ProductID equals p.ProductID
                                    where (d.Date >= fromDate && d.Date <= toDate) && (d.Status == EnumDOStatus.DO || d.Status == EnumDOStatus.Complete)
                                    select new RPTDoTO
                                    {
                                        CustomerName = c.Name,
                                        Address = c.Address,
                                        Mobile = c.ContactNo,
                                        DONO = d.DONo,
                                        FlatDiscountAmount = d.FlatDiscount,
                                        DODate = d.Date,
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        PPDisPercent = dd.PPDisPercent,
                                        PPDisAmt = dd.PPDisAmt,
                                        NetDiscount = d.NetDiscount,
                                        CategoryName = p.CategoryName,
                                        Quantity = dd.DOQty,
                                        BalanceQty = dd.DOQty - dd.GivenQty,
                                        GivenQty = dd.GivenQty,
                                        MRP = dd.MRP,
                                        TotalAmt = dd.TotalAmt,
                                        SuplierTotal = dd.TotalAmt,
                                        ProductCode = p.ProductCode,
                                        CompanyName = p.CompanyName,
                                        DDLiftingPrice = dd.DDLiftingPrice,
                                        TAmt = d.TotalAmt,
                                        DOID = d.DOID
                                    }).ToList();
                }
                else
                {
                    Header = "Customer Sales OS Report from Date " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy");


                    if (customerID > 0)
                        Customers = _customerService.GetAllIQueryable().Where(i => i.CustomerID == customerID);
                    else
                        Customers = _customerService.GetAllIQueryable();

                    ProductInfos = (from d in _DOService.GetAll()
                                    join c in Customers on d.CustomerID equals c.CustomerID
                                    join dd in _DOService.GetAllDetail() on d.DOID equals dd.DOID
                                    join p in _productService.GetDOProducts() on dd.ProductID equals p.ProductID
                                    where (d.Date >= fromDate && d.Date <= toDate) && (d.Status == EnumDOStatus.DO || d.Status == EnumDOStatus.Complete)
                                    select new RPTDoTO
                                    {
                                        CustomerName = c.Name,
                                        Address = c.Address,
                                        Mobile = c.ContactNo,
                                        DONO = d.DONo,
                                        FlatDiscountAmount = d.FlatDiscount,
                                        DODate = d.Date,
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        PPDisPercent = dd.PPDisPercent,
                                        PPDisAmt = dd.PPDisAmt,
                                        NetDiscount = d.NetDiscount,
                                        CategoryName = p.CategoryName,
                                        Quantity = dd.DOQty,
                                        BalanceQty = dd.DOQty - dd.GivenQty,
                                        GivenQty = dd.GivenQty,
                                        MRP = dd.MRP,
                                        TotalAmt = dd.TotalAmt,
                                        SuplierTotal = dd.TotalAmt,
                                        ProductCode = p.ProductCode,
                                        CompanyName = p.CompanyName,
                                        DDLiftingPrice = dd.DDLiftingPrice,
                                        TAmt = d.TotalAmt,
                                        DOID = d.DOID
                                    }).OrderBy(item => item.DONO)
                                   .ToList();

                }



                #region LINQ



                //var ppdisAmt = ProductInfos.Sum(d => d.PPDisAmt * d.DOQty);
                //double totalPPDisAmt = Convert.ToDouble(ppdisAmt);
                //var netsDiscount = ProductInfos[0].NetDiscount;
                //double netDiscount = Convert.ToDouble(netsDiscount);
                //double FlatDiscount = netDiscount - totalPPDisAmt;

                //if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                //    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                //dt.TableName = "dtInvoice";
                //_dataSet = new DataSet();
                //_dataSet.Tables.Add(dt);

                //string sInwodTk = TakaFormat(Convert.ToDouble(DOData.TotalAmt));
                //GetCommonParameters(userName, concernID);


                //_reportParameter = new ReportParameter("TotalPPDiscount", totalPPDisAmt.ToString());
                //_reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalFlatDiscount", FlatDiscount.ToString()); ;

                #endregion

                NetTamt = ProductInfos.GroupBy(d => d.DONO).Sum(d => d.First().TAmt);

                foreach (var item in ProductInfos)
                {

                    row = dt.NewRow();


                    row["Date"] = item.DODate.ToString("dd MMM yyyy");
                    row["DONo"] = item.DONO;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.Mobile;
                    row["Address"] = item.Address;
                    row["ProductName"] = item.ProductName;
                    row["DOQty"] = Math.Truncate(item.Quantity);
                    row["GivenQty"] = Math.Truncate(item.GivenQty);
                    row["BalanceQty"] = Math.Truncate(item.BalanceQty);
                    row["MRP"] = item.TAmt;
                    row["UnitPrice"] = item.DDLiftingPrice - item.PPDisAmt;
                    row["TotalAmt"] = item.TotalAmt;
                    row["SupplierSubTotal"] = item.SuplierTotal - item.FlatDiscountAmount;
                    row["DDLiftingPrice"] = item.DDLiftingPrice;
                    row["PPDisPer"] = item.PPDisPercent;
                    row["PPDisAmt"] = item.PPDisAmt;
                    row["NetDiscount"] = item.NetDiscount;
                    row["FlatDiscount"] = item.FlatDiscountAmount;

                    dt.Rows.Add(row);
                }


                dt.TableName = "dtDOReport";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(Username, ConcernID);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Header", Header);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("NetTamt", NetTamt.ToString());
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "DO\\DOReport.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] ProductWisePurchaseDOReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<ProductWisePurchaseModel> ReportData = new List<ProductWisePurchaseModel>();
                TransactionalDataSet.dtDOReportDataTable dt = new TransactionalDataSet.dtDOReportDataTable();
                DataRow row = null;

                var Data = _DOService.ProductWisePurchaseDOReport(CompanyID, CategoryID, ProductID, fromDate, toDate);

                if (reportType == 0)
                {
                    ReportData = Data;
                }
                else
                {
                    //ReportData = (from d in Data
                    //              group d by new { d.CategoryName } into g
                    //              select new ProductWisePurchaseModel
                    //              {
                    //                  CategoryName = g.Key.CategoryName,
                    //                  Quantity = g.Sum(i => i.Quantity),
                    //                  TotalAmount = g.Sum(i => i.TotalAmount)
                    //              }).ToList();
                    ReportData = Data;
                }

                foreach (var item in ReportData)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date.ToString("dd MMM yyyy");
                    row["DONo"] = item.ChallanNo;
                    //row["CustomerName"] = item.CustomerName;
                    //row["ContactNo"] = item.Mobile;
                    //row["Address"] = item.Address;
                    row["ProductName"] = item.ProductName;
                    row["DOQty"] = Math.Truncate(item.Quantity);
                    row["GivenQty"] = Math.Truncate(item.GivenQty);
                    row["BalanceQty"] = Math.Truncate(item.Quantity) - Math.Truncate(item.GivenQty);
                    row["MRP"] = item.PurchaseRate;
                    row["TotalAmt"] = item.TotalAmount;
                    row["CategoryName"] = item.CategoryName;
                    row["CompanyName"] = item.CompanyName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDOReport";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                if (reportType == 0)
                {
                    _reportParameter = new ReportParameter("Header", "Product Wise DO Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "DO\\rptProductWiseDO.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("Header", "Category Wise DO Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "DO\\rptCategoryWiseDO.rdlc");
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion DO Report End Here 



        public byte[] VoucherTransactionLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType)
        {
            try
            {
                string headName = string.Empty;
                _dataSet = new DataSet();
                var VoucherTransactionInfos = _ShareInvestmentService.VoucherTransactionLedgerData(fromDate, toDate, concernID, ExpenseItemID, headType);
                headName = VoucherTransactionInfos.Select(i => i.ItemName).LastOrDefault();
                TransactionalDataSet.dtVoucherTransactionDataTable dt = new TransactionalDataSet.dtVoucherTransactionDataTable();
                DataRow row = null;
                decimal LastBalance = 0m;
                int Counter = 0;
                foreach (var item in VoucherTransactionInfos)
                {
                    var defualtDate = default(DateTime);


                    Counter++;
                    row = dt.NewRow();
                    if (item.VoucherDate == defualtDate)
                    {
                        row["VoucherDate"] = "_";
                    }
                    else
                    {
                        row["VoucherDate"] = item.VoucherDate.ToString("dd MMM yyyy");
                    }

                    row["ModuleType"] = item.ModuleType;
                    row["VoucherNo"] = item.VoucherNo;
                    row["DebitAmount"] = item.DebitAmount;
                    row["CreditAmount"] = item.CreditAmount;
                    row["Balance"] = item.Balance;
                    row["Narration"] = item.Narration;

                    if (VoucherTransactionInfos.Count() == Counter)
                        LastBalance = item.Balance;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtVoucherTransaction";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("HeadName", headName);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM, yyy") + " to " + toDate.ToString("dd MMM, yyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptVoucherTransactionLedger.rdlc");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        public byte[] AdminProductStockReport(string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName, int UserConcernID, int filetype)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforAdminProductStockReport(userName, concernID, reportType, CompanyName, CategoryName, ProductName, UserConcernID).ToList();
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();
                if (reportType == 0)//Product Wise
                {

                    foreach (var item in stockInfos)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.Item2;
                        row["CompanyName"] = item.Item3;
                        row["CategoryName"] = item.Item4;
                        row["ModelName"] = item.Item5;
                        row["Quantity"] = item.Item6;
                        row["PRate"] = item.Item7;
                        row["TotalPrice"] = (item.Item6 * item.Rest.Item1);
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.Rest.Item1;
                        row["CreditSRate"] = item.Rest.Item2;// 6 months
                        row["TotalCreditPrice"] = (item.Item6 * item.Rest.Item2);
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.Rest.Item3;
                        row["TotalCrSR3"] = (item.Item6 * item.Rest.Item3);
                        row["CrSR12"] = item.Rest.Item4;
                        row["TotalCrSR12"] = (item.Item6 * item.Rest.Item4);
                        row["ConcernName"] = item.Rest.Item5;

                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                else if (reportType == 1) //Company wise
                {
                    foreach (var item in stockInfos)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.Item2;
                        row["CompanyName"] = item.Item3;
                        row["CategoryName"] = item.Item4;
                        row["ModelName"] = item.Item5;
                        row["Quantity"] = item.Item6;
                        row["PRate"] = item.Item7;
                        row["TotalPrice"] = (item.Item6 * item.Rest.Item1);
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.Rest.Item1;
                        row["CreditSRate"] = item.Rest.Item2;// 6 months
                        row["TotalCreditPrice"] = (item.Item6 * item.Rest.Item2);
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.Rest.Item3;
                        row["TotalCrSR3"] = (item.Item6 * item.Rest.Item3);
                        row["CrSR12"] = item.Rest.Item4;
                        row["TotalCrSR12"] = (item.Item6 * item.Rest.Item4);
                        row["ConcernName"] = item.Rest.Item5;

                        dtStockInfoDT.Rows.Add(row);
                    }
                }
                else if (reportType == 2) //category wise
                {
                    foreach (var item in stockInfos)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.Item2;
                        row["CompanyName"] = item.Item3;
                        row["CategoryName"] = item.Item4;
                        row["ModelName"] = item.Item5;
                        row["Quantity"] = item.Item6;
                        row["PRate"] = item.Item7;
                        row["TotalPrice"] = (item.Item6 * item.Rest.Item1);
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.Rest.Item1;
                        row["CreditSRate"] = item.Rest.Item2;// 6 months
                        row["TotalCreditPrice"] = (item.Item6 * item.Rest.Item2);
                        row["StockType"] = "Normal Stock";
                        row["CrSR3"] = item.Rest.Item3;
                        row["TotalCrSR3"] = (item.Item6 * item.Rest.Item3);
                        row["CrSR12"] = item.Rest.Item4;
                        row["TotalCrSR12"] = (item.Item6 * item.Rest.Item4);
                        row["ConcernName"] = item.Rest.Item5;

                        dtStockInfoDT.Rows.Add(row);
                    }
                }

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, UserConcernID);
                if (reportType == 0)
                {
                    reportName = "Stock\\AdminStockSummaryInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\AdminStockSummaryInfoCompany.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\AdminStockSummaryInfoCompany.rdlc";
                }
                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, reportName, filetype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] RateWiseStockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string CompanyName, string CategoryName, string ProductName)
        {
            try
            {

                List<StockLedger> DataGroupBy = _StockServce.GetRateWiseStockLedgerReport(reportType, CompanyName, CategoryName, ProductName, fromDate, toDate, concernID).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable();

                foreach (var item in DataGroupBy)
                {
                    row = dt.NewRow();
                    row["Date"] = item.Date;
                    row["ConcernID"] = item.ConcernID;
                    row["ProductID"] = item.ProductID;
                    row["Code"] = item.Code;
                    row["ProductName"] = item.ProductName;
                    row["ColorName"] = item.ColorName;
                    row["OpeningStockQuantity"] = item.OpeningStockQuantity;
                    row["TotalStockQuantity"] = item.TotalStockQuantity;
                    row["PurchaseQuantity"] = item.PurchaseQuantity;
                    row["SalesQuantity"] = item.SalesQuantity;
                    row["SalesReturnQuantity"] = item.SalesReturnQuantity;
                    row["ClosingStockQuantity"] = item.ClosingStockQuantity;
                    row["OpeningStockValue"] = item.OpeningStockValue;
                    row["TotalStockValue"] = item.TotalStockValue;
                    row["ClosingStockValue"] = item.ClosingStockValue;
                    row["PurchaseReturn"] = item.PurchaseReturnQuantity;
                    row["TransferIN"] = item.TransferInQuantity;
                    row["TransferOUT"] = item.TransferOutQuantity;
                    row["RepQty"] = item.RepQty;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtDailyStockandSalesSummary";


                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                reportName = "Stock\\rptStockLedgerPRateWise.rdlc";

                _reportParameter = new ReportParameter("DateRange", "Stock Ledger From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);



                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] WarrantyHireInvoicePrint(CreditSale oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyNewDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyNewDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                string user = _userService.GetUserNameById(oOrder.CreatedBy);
                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;

                var ProductInfos = (from sd in oOrder.CreditSaleDetails
                                    join std in _stockdetailService.GetAll() on sd.StockDetailID equals std.SDetailID
                                    join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                    join col in _ColorServce.GetAll() on std.ColorID equals col.ColorID
                                    select new
                                    {
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        Quantity = sd.Quantity,
                                        UnitPrice = sd.UnitPrice,
                                        SalesRate = sd.UTAmount,
                                        UTAmount = sd.UTAmount,
                                        PPOffer = sd.PPOffer,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CompanyName = p.CompanyName,
                                        CategoryName = p.CategoryName,
                                        Compressor = p.CompressorWarrentyMonth,
                                        Motor = p.MotorWarrentyMonth,
                                        Service = p.ServiceWarrentyMonth,
                                        Spareparts = p.SparePartsWarrentyMonth,
                                        Panel = p.PanelWarrentyMonth,
                                        UserInputWarranty = p.UserInputWarranty
                                    }).ToList();

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPOffer,
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPOffer = g.Key.PPOffer,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList(),
                                            UserInputWarranty = g.Select(i => i.UserInputWarranty).FirstOrDefault(),
                                        };

                foreach (var item in GroupProductInfos)
                {

                    foreach (var IMEI in item.IMENOs)
                    {
                        Count++;
                        if (item.IMENOs.Count() != Count)
                            IMEIs = IMEIs + IMEI + Environment.NewLine;
                        else
                            IMEIs = IMEIs + IMEI;
                    }

                    dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, 0, 0, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Warranty Period: " + "Compressor: " + item.Compressor + ",";
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + ",";
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + ",";
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + ",";
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;
                    if (!string.IsNullOrEmpty(item.UserInputWarranty))
                        Warrenty = Warrenty + "," + item.UserInputWarranty;

                    dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;


                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrentyNew";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.NetAmount));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                var sysInfo = GetCommonParameters(userName, concernID);



                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.SalesDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Code", customer.Code);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("User", user);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("HelpLine", "Helpline(Toll Free) : 08000016267" + Environment.NewLine + "waltonbd.com");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
                var checkTramsAndCodition = currentSystemInfo.TramsAndCondition;

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\WarrantySalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public byte[] WarrantyHireInvoice(int oOrderID, string userName, int concernID)
        {
            CreditSale oOrder = new CreditSale();
            oOrder = _creditSalesOrderService.GetSalesOrderById(oOrderID);
            oOrder.CreditSalesSchedules = _creditSalesOrderService.GetSalesOrderSchedules(oOrderID).ToList();
            oOrder.CreditSaleDetails = _creditSalesOrderService.GetSalesOrderDetails(oOrderID).ToList();

            return WarrantyHireInvoicePrint(oOrder, userName, concernID);

        }

        public byte[] StockSummaryReportZeroQty(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, int ColorID, int PCategoryID, bool IsVATManager, int StockType, int filetype)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforStockReportZeroQty(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ColorID, PCategoryID, IsVATManager, StockType).ToList();
                var InhouseDamageProductDetails = _productService.GetAllDamageProductFromDetail();
                //var CompanyDamageProductDetails = _purchaseOrderService.GetDamageReturnProductDetails(0, 0);
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();

                #region Product wise
                    InHouseProducts = (from p in InhouseDamageProductDetails
                                       group p by new { p.Item1, p.Item2, p.Item3, p.Item4, p.Item6, p.Item7, p.Rest.Item5 } into g
                                       select new ProductWisePurchaseModel
                                       {
                                           ProductCode = g.Key.Item2,
                                           ProductName = g.Key.Item3,

                                           CategoryName = g.Key.Item6,
                                           CompanyName = g.Key.Item7,
                                           Quantity = g.Count(),
                                           TotalAmount = g.Sum(i => i.Rest.Item3),
                                       }).ToList();

                    /* CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                                            group cds by new { cds.ProductID, cds.ColorID, cds.ProductCode, cds.ProductName, cds.CompanyName, cds.CategoryName } into g
                                            select new ProductWisePurchaseModel
                                            {
                                                ProductCode = g.Key.ProductCode,
                                                ProductName = g.Key.ProductName,
                                                CategoryName = g.Key.CategoryName,

                                                CompanyName = g.Key.CompanyName,
                                                Quantity = g.Count(),
                                                TotalAmount = g.Sum(i => i.MRP),
                                            }).ToList();*/

                    var Normalstocks = (from ns in stockInfos
                                        group ns by new { ProName = ns.Item2, ModelName = ns.Item5, CompanyName = ns.Item3, CategoryName = ns.Item4, PRate = ns.Item7, SRate = ns.Rest.Item1 } into g
                                        select new ProductWisePurchaseModel
                                        {
                                            ProductName = g.Key.ProName,
                                            ColorName = g.Key.ModelName,
                                            CompanyName = g.Key.CompanyName,
                                            CategoryName = g.Key.CategoryName,
                                            Quantity = g.Sum(i => i.Item6),
                                            TotalAmount = g.Sum(i => i.Rest.Item1 * i.Item6),
                                            TotalCreditSR3 = g.Sum(i => i.Rest.Item3 * i.Item6),
                                            TotalCreditSR6 = g.Sum(i => i.Rest.Item2 * i.Item6),
                                            TotalCreditSR12 = g.Sum(i => i.Rest.Item4 * i.Item6),
                                            PurchaseRate = g.Key.PRate,
                                            SalesRate = g.Key.SRate
                                        }).ToList();

                    bool IsHistoryShow = false;
                    if ((concernID != 1 || concernID != 5 || concernID != 6) && reportType == 0)
                        IsHistoryShow = true;
                    foreach (var item in Normalstocks)
                    {
                        row = dtStockInfoDT.NewRow();
                        row["UnitType"] = "Pice";
                        row["ProName"] = item.ProductName;
                        row["CompanyName"] = item.CompanyName;
                        row["CategoryName"] = item.CategoryName;
                        row["ModelName"] = item.ColorName;
                        row["Quantity"] = item.Quantity;
                        row["PRate"] = item.PurchaseRate;
                        row["TotalPrice"] = item.TotalAmount;//Total Sales Price
                        row["StockCode"] = IMENO;
                        row["SalesRate"] = item.SalesRate;
                        row["StockType"] = "Normal Stock";
                        row["Godown"] = "";

                        if (IsHistoryShow)
                        {
                            row["StockHistory"] = "";// _StockServce.GetStockProductsHistory(item.Item1);
                        }
                        dtStockInfoDT.Rows.Add(row);
                    }
                
                #endregion


                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                //if (reportType != 0)
                //{
                //    reportName = "Stock\\StockSummaryInfoWithZero.rdlc";
                //}


                return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, "Stock\\StockSummaryInfoWithZero.rdlc", filetype);

                //return ReportBase.GenerateTransactionalReportByFileType(_dataSet, _reportParameters, reportName, filetype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] StockDetailReportNew(string userName, int concernID, int reportType, List<int> CompanyIds, List<int> CategoriesList, List<int> ProductIds, List<int> GodownIds, List<int> ColorIds, bool IsVATManager)
        {
            try
            {

                var stockInfos = _StockServce.GetforStockReportNew(userName, concernID, reportType, CompanyIds, CategoriesList, ProductIds, GodownIds, ColorIds, IsVATManager).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                string IMENO = "";
                int count;
                //StockDetails = _stockdetailService.GetAll();
                foreach (var item in stockInfos)
                {
                    row = dtStockInfoDT.NewRow();
                    row["UnitType"] = "Pice";
                    //row["StockCode"] = item.Item1;
                    row["ProName"] = item.Item2;
                    row["CompanyName"] = item.Item3;
                    row["CategoryName"] = item.Item4;
                    row["ModelName"] = item.Item5;
                    row["Quantity"] = item.Item6;
                    row["PRate"] = item.Rest.Item7;
                    row["TotalPrice"] = (item.Item6 * item.Rest.Item7);
                    row["Godown"] = item.Rest.Item5;

                    //var SDetails = _StockServce.GetStockDetailsByID(item.Item1);
                    //var SDetails = StockDetails.Where(i=>i.StockID==item.Item1).ToList();

                    IMENO = "";
                    count = 0;

                    foreach (var imei in item.Rest.Item6)
                    {
                        if (count == 0)
                            IMENO = IMENO + imei;
                        else
                            IMENO = IMENO + System.Environment.NewLine + imei;
                        count++;
                    }

                    row["StockCode"] = IMENO;

                    dtStockInfoDT.Rows.Add(row);
                }

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStock.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStock.rdlc";
                }

                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStock.rdlc";
                }
                else if (reportType == 4)
                {
                    reportName = "Stock\\rptColorWiseStock.rdlc";
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }

}
