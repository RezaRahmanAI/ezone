using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMSWEB.Report;
using AutoMapper;
using IMSWEB.Service;
using System.Threading.Tasks;
using System.IO;
using IMSWEB.Model;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("report")]
    public class ReportController : CoreController
    {
        IBasicReport _basicReportService;
        ITransactionalReport _transactionalReportService;
        IMapper _mapper;
        ISisterConcernService _SisterConcernService;
        ISystemInformationService _systemInformationService;
        ISupplierService _supplierService;
        public ReportController(IErrorService errorService,
            IBasicReport basicReportService, ITransactionalReport transactionalReportService,
            IMapper mapper, ISisterConcernService SisterConcernService,
            ISystemInformationService systemInformationService)
            : base(errorService, systemInformationService)
        {
            _basicReportService = basicReportService;
            _transactionalReportService = transactionalReportService;
            _mapper = mapper;
            _SisterConcernService = SisterConcernService;
            _systemInformationService = systemInformationService;
        }

        //[HttpGet]
        //public ActionResult RenderReport()
        //{
        //    byte[] bytes = null;
        //    if (TempData["ReportData"] != null)
        //        bytes = (byte[])TempData["ReportData"];

        //    return File(bytes, "application/pdf");
        //}

        [HttpGet]
        public ActionResult RenderReport()
        {

            byte[] bytes = TempData["ReportData"] as byte[];

            if (bytes == null || bytes.Length == 0)
            {
                return Content("The report data is unavailable or expired. Please try again.");
            }

            return File(bytes, "application/pdf");
        }

        [HttpGet]
        public ActionResult RenderReportinExcel()
        {
            byte[] bytes = null;
            if (TempData["ReportData"] != null)
                bytes = (byte[])TempData["ReportData"];
            return File(bytes, "application/vnd.ms-excel");
        }

        [HttpGet]
        [Authorize]
        [Route("employeeinformation-report")]
        public async Task<PartialViewResult> EmployeeInformationReport()
        {
            byte[] bytes = await _basicReportService.EmployeeInformationReport(User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        private PartialViewResult CustomPartialView()
        {
            string userAgent = Request.UserAgent;
            if (userAgent.ToLower().Contains("windows"))
                return PartialView("~/Views/Shared/_ReportViewer.cshtml");
            else
                return PartialView("~/Views/Shared/_ReportViewMobile.cshtml");
        }

        private PartialViewResult CustomPartialViewForExcel()
        {
            string userAgent = Request.UserAgent;
            if (userAgent.ToLower().Contains("windows"))
                return PartialView("~/Views/Shared/_ReportViewerforExcel.cshtml");
            else
                return PartialView("~/Views/Shared/_ReportViewMobile.cshtml");
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DailySalesReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            int CustomerType = 0;
            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"]);

            string ClientDateTime = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                ClientDateTime = formCollection["ClientDateTime"];

            bool IsFalesReport = IsVATManager();

            byte[] bytes = _transactionalReportService.SalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Daily", CustomerType, IsFalesReport, ClientDateTime);
            TempData["ReportData"] = bytes;

            return CustomPartialView();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult MonthlySalesReport(FormCollection formCollection)
        {
            DateTime date = Convert.ToDateTime(formCollection["FromDate"].ToString());
            //DateTime fromDate =new DateTime(date.Year,date.Month,1,0,0,0);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            //DateTime toDate =new DateTime(date.Year,date.Month,30,23,59,59);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 11:59:59 PM");

            int CustomerType = 0;
            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"]);

            string ClientDateTime = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                ClientDateTime = formCollection["ClientDateTime"];

            var fromDate = new DateTime(date.Year, date.Month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            //var valueResult = ValueProvider.GetValue("ReportType");

            bool IsFalesReport = IsVATManager();
            byte[] bytes = _transactionalReportService.SalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Monthly", CustomerType, IsFalesReport, ClientDateTime);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult YearlySalesReport(FormCollection formCollection)
        {
            int year = Convert.ToInt32(formCollection["FromDate"].ToString());
            DateTime fromDate = new DateTime(year, 1, 1, 0, 0, 0);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = new DateTime(year, 12, 31, 23, 59, 59);
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            string ClientDateTime = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                ClientDateTime = formCollection["ClientDateTime"];
            //var valueResult = ValueProvider.GetValue("ReportType");

            // byte[] stock = _transactionalReportService.StockReport(User.Identity.Name, User.Identity.GetConcernId(), reportType);
            bool IsFalesReport = IsVATManager();
            byte[] bytes = _transactionalReportService.SalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Yearly", 0, IsFalesReport, ClientDateTime);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult StockReport(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, GodownsId = 0, PCategoryId = 0, StockType = 0, MaaManager = 0;

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["GodownsId"]))
                GodownsId = Convert.ToInt32(formCollection["GodownsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["PCategoriesId"]))
                PCategoryId = Convert.ToInt32(formCollection["PCategoriesId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["stockType"]))
                StockType = Convert.ToInt32(formCollection["stockType"]);

            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());

            if (User.IsInRole(EnumUserRoles.MaaElectronicManager.ToString()))
            {
                MaaManager = 1;
            }
            else
                MaaManager = 0;

            byte[] stock = _transactionalReportService.StockDetailReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, GodownsId, PCategoryId, IsVATManager(), StockType, MaaManager, fileType);


            TempData["ReportData"] = stock;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockSummaryReport(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, GodownsId = 0, ColorsId = 0, PCategoryId = 0, StockType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["GodownsId"]))
                GodownsId = Convert.ToInt32(formCollection["GodownsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ColorsId"]))
                ColorsId = Convert.ToInt32(formCollection["ColorsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["PCategoriesId"]))
                PCategoryId = Convert.ToInt32(formCollection["PCategoriesId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["stockType"]))
                StockType = Convert.ToInt32(formCollection["stockType"]);

            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());

            byte[] stock = _transactionalReportService.StockSummaryReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, GodownsId, ColorsId, PCategoryId, IsVATManager(), StockType, fileType);

            TempData["ReportData"] = stock;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult DailyPurchaseReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            //int reportType = 1;

            EnumPurchaseType PurchaseType = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            byte[] bytes = _transactionalReportService.PurchaseReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Daily", PurchaseType);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult MonthlyPurchaseReport(FormCollection formCollection)
        {
            DateTime date = Convert.ToDateTime(formCollection["FromDate"].ToString());
            //DateTime fromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            //DateTime toDate = new DateTime(date.Year, date.Month, 30, 23, 59, 59);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 11:59:59 PM");
            var fromDate = new DateTime(date.Year, date.Month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            EnumPurchaseType PurchaseType = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            byte[] bytes = _transactionalReportService.PurchaseReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Monthly", PurchaseType, IsVATManager());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult YearlyPurchaseReport(FormCollection formCollection)
        {
            int year = Convert.ToInt32(formCollection["FromDate"].ToString());
            DateTime fromDate = new DateTime(year, 1, 1, 0, 0, 0);// Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = new DateTime(year, 12, 31, 23, 59, 59);
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            EnumPurchaseType PurchaseType = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            byte[] bytes = _transactionalReportService.PurchaseReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Yearly", PurchaseType, IsVATManager());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SalesInvoiceById()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }
        [HttpGet]
        [Authorize]
        public PartialViewResult ChallanById()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.ChallanReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SalesInvoiceHistoryById()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.SalesInvoiceHistoryReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult PurchaseInvoiceHistoryById()
        {
            int orderId = (int)TempData["POrderID"];
            TempData["POrderID"] = orderId;
            byte[] bytes = _transactionalReportService.PurchaseInvoiceHistoryReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult CreditChallanById()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.CreditChallanReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpGet]
        [Authorize]
        public PartialViewResult SalesInvoice()
        {


            SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            bool isPreview = TempData.ContainsKey("IsPreview") ? (bool)TempData["IsPreview"] : false;
            TempData["salesInvoiceData"] = sorder;
            int orderId = sorder.SOrderID;
            byte[] bytes = _transactionalReportService.SalesInvoiceReport(sorder, User.Identity.Name, User.Identity.GetConcernId(), isPreview);
            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();


            //SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            //TempData["salesInvoiceData"] = sorder;
            //int orderId = sorder.SOrderID;
            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(sorder, User.Identity.Name, User.Identity.GetConcernId());

            ////int orderId = (int)TempData["OrderId"];
            ////TempData["OrderId"] = orderId;
            ////byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());


            ////TempData["ReportData"] = bytes;
            ////return CustomPartialView();
        }


        [HttpGet]
        [Authorize]
        public PartialViewResult Challan()
        {
            SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            int orderId = sorder.SOrderID;
            TempData["salesInvoiceData"] = sorder;
            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());

            byte[] bytes = _transactionalReportService.ChallanReport(sorder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }






        [HttpGet]
        [Authorize]
        public PartialViewResult SRVisitInvoice()
        {
            //SRVisit sorder = (SRVisit)TempData["SRVisitData"];
            SRVisitViewModel sorder = (SRVisitViewModel)TempData["SRVisitData"];
            string ChallanNo = sorder.SRVisit.ChallanNo;

            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());

            byte[] bytes = _transactionalReportService.SRInvoiceReportByChallanNo(ChallanNo, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SRVisitInvoiceByID()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.SRInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult CreditSalesInvoiceReportByID()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.CreditSalesInvoiceReportByID(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult CreditSalesInvoice()
        {
            CreditSale sorder = (CreditSale)TempData["CreditSalesInvoiceData"];
            byte[] bytes = _transactionalReportService.CreditSalesInvoiceReport(sorder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult CustomerSalesReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = 1;//Convert.ToInt32(formCollection["ReportType"].ToString());
            int CustomerId = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            if (CustomerId == 0)
            {
                AddToastMessage("Customer Sales", "Please at first select Customer.", ToastType.Warning);
            }

            //var valueResult = ValueProvider.GetValue("ReportType");

            byte[] bytes = _transactionalReportService.CustomeWiseSalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, CustomerId);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult MOWiseSalesReport(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int reportType = 0;
            int EmployeeId = 0;

            if (formCollection["ReportType"] != null)
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            else
                reportType = 0;

            if (formCollection["EmployeesId"] != null && formCollection["EmployeesId"] != "")
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);
            else
            {
                EmployeeId = 0;
            }

            byte[] bytes = _transactionalReportService.MOWiseSalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeId, reportType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult MOWiseCustomerDue(FormCollection formCollection)
        {
            int reportType = 0;
            int EmployeeId = 0;

            if (formCollection["ReportType"] != null)
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            else
                reportType = 0;

            if (formCollection["EmployeesId"] != null && formCollection["EmployeesId"] != "")
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);
            else
            {
                EmployeeId = 0;
            }

            byte[] bytes = _transactionalReportService.MOWiseCustomerDueRpt(User.Identity.Name, User.Identity.GetConcernId(), EmployeeId, reportType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpPost]
        [Authorize]
        public PartialViewResult ExpenditureReport(FormCollection formCollection)
        {
            int ExpenseIncomeItemID = 0;
            if (!string.IsNullOrEmpty(formCollection["ExpenseItemsId"]))
                ExpenseIncomeItemID = Convert.ToInt32(formCollection["ExpenseItemsId"]);

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            EnumCompanyTransaction reportType = (EnumCompanyTransaction)Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = _transactionalReportService.ExpenditureReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, ExpenseIncomeItemID, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult UpComingScheduleReport(FormCollection formCollection)
        {
            int EmployeeId = 0, customerType = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);
            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                customerType = Convert.ToInt32(formCollection["CustomerType"]);
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            //int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = _transactionalReportService.UpComingScheduleReport(fromDate, toDate, User.Identity.Name,
                User.Identity.GetConcernId(), (EnumCustomerType)customerType, EmployeeId);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult DefaultingCustomerReportOld(FormCollection formCollection)
        {

            byte[] bytes = _transactionalReportService.DefaultingCustomerReport(DateTime.Today, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DefaultingCustomerReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.DefaultingCustomerReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult InstallmentCollectionReport(FormCollection formCollection)
        {
            int EmployeeId = 0;
            if (formCollection["EmployeesId"] != null && formCollection["EmployeesId"] != "")
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            //int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = _transactionalReportService.InstallmentCollectionReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeId, 0, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult SuplierWisePurchaseReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = 1;
            int SupplierId = Convert.ToInt32(formCollection["SuppliersId"]);

            byte[] bytes = _transactionalReportService.SuplierWisePurchaseReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, SupplierId);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ConcernWiseCustomerDueRpt(FormCollection formCollection)
        {
            int reportType = 0, DueType = 0, CustomerId = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
            {
                reportType = Convert.ToInt32(formCollection["CustomerType"].ToString().Replace(",", string.Empty));
            }

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["DueType"]))
                DueType = Convert.ToInt32(formCollection["DueType"]);


            byte[] bytes = _basicReportService.CustomerCategoryWiseDueRpt(User.Identity.Name, User.Identity.GetConcernId(), CustomerId, reportType, DueType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ConcernWiseSupplierDueRpt(FormCollection formCollection)
        {
            int SupplierId = 0, ConcernID = 0;

            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierId = Convert.ToInt32(formCollection["SuppliersId"]);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            if (ConcernID == 0)
                ConcernID = User.Identity.GetConcernId();

            byte[] bytes = _basicReportService.ConcernWiseSupplierDueRpt(User.Identity.Name, User.Identity.GetConcernId(), SupplierId, ConcernID, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult CashCollectionReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            EnumCustomerType customerType = 0;

            int CustomerId = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
            {
                string type = formCollection["CustomerType"].Replace(",", "");
                customerType = (EnumCustomerType)Convert.ToInt32(type);
            }

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            byte[] bytes = _transactionalReportService.CashCollectionReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerId, customerType);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult CashDeliveryReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = 0, SupplierId = 0;

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierId = Convert.ToInt32(formCollection["SuppliersId"]);

            byte[] bytes = _transactionalReportService.CashDeliverReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), SupplierId, false, 0);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult MOWiseSDetailsReport(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int EmployeeId = 0;

            if (formCollection["EmployeesId"] != null && formCollection["EmployeesId"] != "")
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);
            else
            {
                EmployeeId = 0;
            }

            byte[] bytes = _transactionalReportService.MOWiseSDetailReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeId);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductWisePriceProtection(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            byte[] bytes = _transactionalReportService.ProductWisePriceProtection(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult ProductWisePandSReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int productId = Convert.ToInt32(formCollection["ProductsId"]);

            byte[] bytes = _transactionalReportService.ProductWisePandSReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), productId);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult SRVisitStatusReport(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int EmployeeId = 0;

            if (formCollection["EmployeesId"] != null && formCollection["EmployeesId"] != "")
                EmployeeId = Convert.ToInt32(formCollection["EmployeesId"]);
            else
            {
                EmployeeId = 0;
            }

            byte[] bytes = _transactionalReportService.SRVisitStatusReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeId);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult SRWiseCustomerSalesSummary(FormCollection formCollection)
        {
            int EmployeeID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
                EmployeeID = Convert.ToInt32(formCollection["EmployeesId"].ToString());
            byte[] bytes = _transactionalReportService.SRWiseCustomerSalesSummary(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult CustomerLedger(FormCollection formCollection)
        //{
        //    int CustomerID = 0, ReportType = 0;
        //    DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
        //    DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
        //    if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
        //        CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
        //    if (!string.IsNullOrEmpty(formCollection["IsSummaryReport"]))
        //        ReportType = Convert.ToInt32(formCollection["IsSummaryReport"]);
        //    byte[] bytes = null;
        //    if (ReportType == 1) //Summary
        //        bytes = _transactionalReportService.CustomerLedgerSummary(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);
        //    else
        //        bytes = _transactionalReportService.CustomerLedgerDetails(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);

        //    TempData["ReportData"] = bytes;
        //    return CustomPartialView();

        //}

        [HttpPost]
        [Authorize]
        public ActionResult BankLedger(FormCollection formCollection)
        {
            int BankID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["BanksId"]))
                BankID = Convert.ToInt32(formCollection["BanksId"]);


            var systemInfo = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            int commonConcernId = User.Identity.GetConcernId();
            if (systemInfo.IsCommonBank)
                commonConcernId = _SisterConcernService.GetParentConcernId(commonConcernId);


            byte[] bytes = _transactionalReportService.BankLedgerReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), BankID, commonConcernId);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult CustomerDueReport(FormCollection formCollection)
        {
            int CustomerID = 0, IsOnlyDue = 0, ConcernID = 0, selectedConcernID = 0;
            int CustomerType = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["DueType"]))
                IsOnlyDue = Convert.ToInt32(formCollection["DueType"]);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            ConcernID = User.Identity.GetConcernId();

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"].ToString().Replace(',', ' '));

            byte[] bytes = _transactionalReportService.CustomerDueReport(fromDate, toDate, User.Identity.Name, ConcernID, CustomerID, IsOnlyDue, (EnumCustomerType)CustomerType, false, selectedConcernID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult DailyStockVSSalesSummary(FormCollection formCollection)
        {
            int ProductID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductID = Convert.ToInt32(formCollection["ProductsId"].ToString());
            byte[] bytes = _transactionalReportService.DailyStockVSSalesSummary(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), ProductID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpPost]
        [Authorize]
        public ActionResult BankSummaryReport(FormCollection formCollection)
        {
            int ProductID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductID = Convert.ToInt32(formCollection["ProductsId"].ToString());
            byte[] bytes = _transactionalReportService.BankSummaryReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), ProductID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();



        }
        //[HttpPost]
        //[Authorize]
        //public ActionResult DailyCashBookLedgerReport(FormCollection formCollection)
        //{
        //    DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
        //    DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
        //    byte[] bytes = _transactionalReportService.DailyCashBookLedger(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());

        //    TempData["ReportData"] = bytes;
        //    return CustomPartialView();

        //}

        [HttpGet]
        [Authorize]
        public PartialViewResult ReplacementInvoice()
        {
            IEnumerable<ReplaceOrderDetail> rorderdetails = (IEnumerable<ReplaceOrderDetail>)TempData["ReplacementInvoicedetails"];
            ReplaceOrder ROrder = (ReplaceOrder)TempData["ReplacementInvoice"];
            byte[] bytes = _transactionalReportService.ReplacementInvoiceReport(rorderdetails, ROrder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpGet]
        [Authorize]
        public PartialViewResult ReplaceInvoiceById()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.ReplaceInvoiceReportByID(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpGet]
        [Authorize]
        public PartialViewResult ReturnInvoice()
        {
            IEnumerable<ReplaceOrderDetail> rorderdetails = (IEnumerable<ReplaceOrderDetail>)TempData["ReturnInvoicedetails"];
            ReplaceOrder ROrder = (ReplaceOrder)TempData["ReturnInvoice"];
            byte[] bytes = _transactionalReportService.ReturnInvoiceReport(rorderdetails, ROrder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpGet]
        [Authorize]
        public PartialViewResult ReturnInvoiceById()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.ReturnInvoiceReportByID(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public ActionResult DailyWorkSheet(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.DailyWorkSheet(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult SRVisitReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int EmployeeID = 0, ReportType = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
                EmployeeID = int.Parse(formCollection["EmployeesId"]);

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = int.Parse(formCollection["ReportType"]);
            byte[] bytes = _transactionalReportService.SRVisitReportDetails(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID, ReportType);

            //bytes = _transactionalReportService.SRVisitReportUsingSP(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult SRWiseCustomerStatusReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int EmployeeID = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                EmployeeID = int.Parse(formCollection["EmployeesId"]);
            }
            byte[] bytes = _transactionalReportService.SRWiseCustomerStatusReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult ReplacementReport(FormCollection formCollection)
        {
            int CustomerID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 00:00:00.000");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 00:00:00.000");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
            byte[] bytes = _transactionalReportService.ReplacementReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult ReturnReport(FormCollection formCollection)
        {
            int CustomerID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
            byte[] bytes = _transactionalReportService.ReturntReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt()
        {
            var CashCollection = (CashCollection)TempData["MoneyReceiptData"];

            byte[] bytes = _transactionalReportService.CashCollectionMoneyReceipt(CashCollection, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceiptByID()
        {
            var CashCollectionID = (int)TempData["CashCollectionID"];
            byte[] bytes = _transactionalReportService.CashCollectionMoneyReceiptByID(CashCollectionID, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashDeliveryReceipt()
        {
            var CashCollectionID = (int)TempData["CashCollectionID"];
            byte[] bytes = _transactionalReportService.CashDeliveryMoneyReceiptPrint(CashCollectionID, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreditMoneyReceipt()
        {
            var creditSales = (CreditSale)TempData["MoneyReceiptData"];
            var details = (List<CreditSaleDetails>)TempData["Details"];
            var CreditSalesSchedules = (CreditSalesSchedule)TempData["creditsalesSchedules"];

            byte[] bytes = _transactionalReportService.CrditSalesMoneyReceipt(creditSales, details, CreditSalesSchedules, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }
        [HttpGet]
        [Authorize]
        public ActionResult CreditSalesMoneyReceiptByID()
        {
            int CreditSalesID = (int)TempData["OrderId"];

            byte[] bytes = _transactionalReportService.CrditSalesMoneyReceiptByID(CreditSalesID, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public ActionResult MonthlyBenefit(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            byte[] bytes = _transactionalReportService.MonthlyBenefit(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public ActionResult ProductWiseBenefit(FormCollection formCollection)
        {
            int ProductID = 0, CompanyId = 0, CategoryId=0, CustomerID = 0;
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductID = Convert.ToInt32(formCollection["ProductsId"]);
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyId = Convert.ToInt32(formCollection["CompaniesId"]);


            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryId = Convert.ToInt32(formCollection["CategoriesId"]);

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);


            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            byte[] bytes = _transactionalReportService.ProductWiseBenefitReport(fromDate, toDate, ProductID, User.Identity.Name, User.Identity.GetConcernId(), CompanyId, CategoryId, CustomerID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }



        [HttpPost]
        [Authorize]
        public ActionResult ProductWiseSalesReport(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int CustomerID = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);


            byte[] bytes = _transactionalReportService.ProductWiseSalesReport(fromDate, toDate, CustomerID, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductWisePurchaseReport(FormCollection formCollection)
        {

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int SupplierID = 0;

            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierID = Convert.ToInt32(formCollection["SuppliersId"]);

            EnumPurchaseType PurchaseType = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            byte[] bytes = _transactionalReportService.ProductWisePurchaseReport(fromDate, toDate, SupplierID, User.Identity.Name, User.Identity.GetConcernId(), PurchaseType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public ActionResult DamageProductReport(FormCollection formCollection)
        {
            int CustomerID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 00:00:00.000");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 00:00:00.000");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
            byte[] bytes = _transactionalReportService.DamageProductReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult SRWiseCashCollectionReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int EmployeeID = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                EmployeeID = int.Parse(formCollection["EmployeesId"]);
            }

            byte[] bytes = _transactionalReportService.SRWiseCashCollectionReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ProductwiseSalesDetails(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, IsSummaryReport = 0, CustomerType = 0,
                CustomerID = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["IsSummaryReport"]))
                IsSummaryReport = Convert.ToInt32(formCollection["IsSummaryReport"].ToString());

            if (!string.IsNullOrEmpty(formCollection["CustomerTypes"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerTypes"].ToString());

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"].ToString());

            byte[] stock = null;
            if (IsSummaryReport == 2)
                stock = _transactionalReportService.ProductwiseSalesDetails(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, fromDate, toDate, CustomerType, CustomerID);
            else
                stock = _transactionalReportService.ProductwiseSalesSummary(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, fromDate, toDate, CustomerType, CustomerID);

            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ProductWisePurchaseDetailsReport(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, SupplierID = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["SuppliersID"]))
                SupplierID = Convert.ToInt32(formCollection["SuppliersID"].ToString());

            EnumPurchaseType PurchaseType = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            byte[] stock = _transactionalReportService.ProductWisePurchaseDetailsReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, fromDate, toDate, PurchaseType, SupplierID);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DailyCashBookLedgerReport(FormCollection formCollection)
        {
            // DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");


            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);
            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {

                var DateRange = GetFirstAndLastDateOfYear(Convert.ToInt32(formCollection["Year"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;


                //fromDate = Convert.ToDateTime(formCollection["Year"]);
                //fromDate = new DateTime(fromDate.Year, 1, 1);
                //fromDate = new DateTime(fromDate.Year, 12, 31);
            }

            //   DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.DailyCashBookLedger(fromDate, ToDate, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public ActionResult ProfitAndLossReport(FormCollection formCollection)
        {
            // DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");


            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);
            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {

                var DateRange = GetFirstAndLastDateOfYear(Convert.ToInt32(formCollection["Year"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;


                //fromDate = Convert.ToDateTime(formCollection["Year"]);
                //fromDate = new DateTime(fromDate.Year, 1, 1);
                //fromDate = new DateTime(fromDate.Year, 12, 31);
            }

            //   DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.ProfitAndLossReport(fromDate, ToDate, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }





        [HttpPost]
        [Authorize]
        public ActionResult SummaryReport(FormCollection formCollection)
        {
            // DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");


            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);
            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {

                var DateRange = GetFirstAndLastDateOfYear(Convert.ToInt32(formCollection["Year"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;


                //fromDate = Convert.ToDateTime(formCollection["Year"]);
                //fromDate = new DateTime(fromDate.Year, 1, 1);
                //fromDate = new DateTime(fromDate.Year, 12, 31);
            }

            //   DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.SummaryReport(fromDate, ToDate, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        //public PartialViewResult BankTransactionReport(FormCollection formCollection)
        //{
        //    int reportType = 0, BankID = 0, CategoryID = 0, ProductsId = 0;

        //    DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
        //    DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

        //    if (!string.IsNullOrEmpty(formCollection["BanksId"]))
        //        BankID = Convert.ToInt32(formCollection["BanksId"].ToString());
        //    byte[] stock = _transactionalReportService.BankTransactionReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, BankID, fromDate, toDate);
        //    TempData["ReportData"] = stock;
        //    return CustomPartialView();
        //}
        public PartialViewResult BankTransactionReport(FormCollection formCollection, bool isBT = false)
        {
            int BankID = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["BanksId"]))
                BankID = Convert.ToInt32(formCollection["BanksId"].ToString());
            int concernId = User.Identity.GetConcernId();
            int reportConcernId = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernId"]))
                reportConcernId = Convert.ToInt32(formCollection["ConcernId"].ToString());
            reportConcernId = isBT ? concernId : reportConcernId;
            byte[] stock = _transactionalReportService.NewBankTransactionsReport(fromDate, toDate, BankID, User.Identity.Name, concernId, reportConcernId);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }
        [Authorize]
        [HttpGet]
        public ActionResult PurchaseInvoice()
        {
            POrder PorderData = (POrder)TempData["POInvoiceData"];
            bool isPreview = TempData.ContainsKey("IsPreview") ? (bool)TempData["IsPreview"] : false;
            byte[] bytes = _transactionalReportService.POInvoice(PorderData, User.Identity.Name, User.Identity.GetConcernId(), isPreview);

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        //public ActionResult PurchaseInvoiceById()
        //{
        //    int POrderID = (int)TempData["POrderID"];
        //    byte[] bytes = _transactionalReportService.POInvoiceByID(POrderID, User.Identity.Name, User.Identity.GetConcernId());

        //    TempData["ReportData"] = bytes;
        //    return CustomPartialView();
        //}


        public ActionResult PurchaseInvoiceById()
        {
            if (TempData["POrderID"] == null)
            {
                return HttpNotFound("POrderID is not available in TempData.");
            }

            int POrderID;
            if (!int.TryParse(TempData["POrderID"].ToString(), out POrderID))
            {
                return HttpNotFound("Invalid POrderID in TempData.");
            }

            byte[] bytes = _transactionalReportService.POInvoiceByID(POrderID, User.Identity.Name, User.Identity.GetConcernId());

            if (bytes == null || bytes.Length == 0)
            {
                return HttpNotFound("Report data not found for the given POrderID.");
            }

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public ActionResult GetDamagePOReport(FormCollection formCollection)
        {
            int SupplierID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierID = Convert.ToInt32(formCollection["SuppliersId"]);

            byte[] bytes = _transactionalReportService.GetDamagePOReport(User.Identity.Name, User.Identity.GetConcernId(), SupplierID, fromDate, toDate);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult GetDamageReturnPOReport(FormCollection formCollection)
        {
            int SupplierID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierID = Convert.ToInt32(formCollection["SuppliersId"]);

            byte[] bytes = _transactionalReportService.GetDamageReturnPOReport(User.Identity.Name, User.Identity.GetConcernId(), SupplierID, fromDate, toDate);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult GetSalarySheet(FormCollection formCollection)
        {
            List<int> EmployeeIDList = new List<int>();
            int EmployeeID = 0, DepartmentID = 0;
            DateTime SalaryMonth = DateTime.MinValue;
            if (!string.IsNullOrEmpty(formCollection["EmployeeIdList"]))
            {
                EmployeeIDList = formCollection["EmployeeIdList"].Split(new char[] { ',' }).Select(Int32.Parse).Distinct().ToList();
            }

            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);
            }
            if (!string.IsNullOrEmpty(formCollection["SalaryMonth"]))
                SalaryMonth = Convert.ToDateTime(formCollection["SalaryMonth"]);
            if (!string.IsNullOrEmpty(formCollection["DepartmentsId"]))
                DepartmentID = Convert.ToInt32(formCollection["DepartmentsId"]);
            var DateRange = GetFirstAndLastDateOfMonth(SalaryMonth);
            byte[] bytes = _transactionalReportService.GetSalarySheet(SalaryMonth, EmployeeID, DepartmentID, EmployeeIDList, User.Identity.Name, User.Identity.GetConcernId(), DateRange);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult GetPaySlip(FormCollection formCollection)
        {
            int EmployeeID = 0, DepartmentID = 0;
            DateTime SalaryMonth = DateTime.MinValue;

            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);
            }
            if (!string.IsNullOrEmpty(formCollection["SalaryMonth"]))
                SalaryMonth = Convert.ToDateTime(formCollection["SalaryMonth"]);

            var DateRange = GetFirstAndLastDateOfMonth(SalaryMonth);
            byte[] bytes = _transactionalReportService.GetPaySlip(SalaryMonth, EmployeeID, User.Identity.Name, User.Identity.GetConcernId(), DateRange);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult GetAdvanceSalaryReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int EmployeeID = 0;

            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);
            }
            byte[] bytes = _transactionalReportService.GetAdvanceSalaryReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult AdminDailySalesReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            int CustomerType = 0, selectedConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"]);

            string ClientDateTime = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                ClientDateTime = formCollection["ClientDateTime"];

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.SalesReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Daily", CustomerType, true, ClientDateTime, selectedConcernID);
            TempData["ReportData"] = bytes;

            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminPOReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            EnumPurchaseType PurchaseType = 0;
            int selectedConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["PurchaseType"]))
                Enum.TryParse(formCollection["PurchaseType"], out PurchaseType);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
            {
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            }
            byte[] bytes = _transactionalReportService.PurchaseReportNew(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, "Daily", PurchaseType, true, selectedConcernID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult AdminCustomerDueRpt(FormCollection formCollection)
        {
            int CustomerID = 0, IsOnlyDue = 0, ConcernID = 0, selectedConcernID = 0;
            int CustomerType = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["DueType"]))
                IsOnlyDue = Convert.ToInt32(formCollection["DueType"]);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);

                ConcernID = User.Identity.GetConcernId();

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"].ToString().Replace(',', ' '));

            byte[] bytes = _transactionalReportService.CustomerDueReportNew(fromDate, toDate, User.Identity.Name, ConcernID, CustomerID, IsOnlyDue, (EnumCustomerType)CustomerType, true, selectedConcernID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult AdminCashColletions(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int ConcernID = 0, customerID = 0;
            EnumCustomerType customerType = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                customerType = (EnumCustomerType)Convert.ToInt32(formCollection["CustomerType"]);

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                customerID = Convert.ToInt32(formCollection["CustomersId"]);

            byte[] bytes = _transactionalReportService.AdminCashCollectionReport(User.Identity.Name, ConcernID, User.Identity.GetConcernId(), fromDate, toDate, customerType, customerID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        public PartialViewResult CashInHandReport(FormCollection formCollection)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0, CustomerType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"]);

            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {
                int year = Convert.ToInt32(formCollection["Year"]);
                fromDate = new DateTime(year, 1, 1);
                ToDate = new DateTime(year, 12, 31, 23, 59, 59);
            }
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();
            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());

            byte[] bytes = _transactionalReportService.CashInHandReport(User.Identity.Name, ConcernID, ReportType, fromDate, ToDate, CustomerType, fileType);

            TempData["ReportData"] = bytes;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }

        }

        [HttpGet]
        [Authorize]
        public PartialViewResult BankTransMoneyReceipt()
        {
            int BankTranID = (int)TempData["BankTranID"];

            byte[] bytes = _transactionalReportService.BankTransMoneyReceipt(User.Identity.Name, User.Identity.GetConcernId(), BankTranID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpGet]
        [Authorize]
        public PartialViewResult ExpenseMoneyReceipt()
        {
            int ExpenditureID = (int)TempData["ExpenditureID"];

            byte[] bytes = _transactionalReportService.ExpenseIncomeMoneyReceipt(User.Identity.Name, User.Identity.GetConcernId(), ExpenditureID, true);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpGet]
        [Authorize]
        public PartialViewResult IncomeMoneyReceipt()
        {
            int ExpenditureID = (int)TempData["ExpenditureID"];

            byte[] bytes = _transactionalReportService.ExpenseIncomeMoneyReceipt(User.Identity.Name, User.Identity.GetConcernId(), ExpenditureID, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DailyAttendence(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            int DepartmentID = 0;
            bool IsPresent = false, IsAbsent = false;
            if (!string.IsNullOrEmpty(formCollection["DepartmentsId"]))
                DepartmentID = Convert.ToInt32(formCollection["DepartmentsId"]);
            if (!string.IsNullOrEmpty(formCollection["IsPresent"]))
                IsPresent = Convert.ToInt32(formCollection["IsPresent"]) > 0 ? true : false;
            if (!string.IsNullOrEmpty(formCollection["IsAbsent"]))
                IsAbsent = Convert.ToInt32(formCollection["IsAbsent"]) > 0 ? true : false;

            byte[] bytes = _transactionalReportService.DailyAttendence(User.Identity.Name, User.Identity.GetConcernId(), DepartmentID, fromDate, IsPresent, IsAbsent);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockLedgerReport(FormCollection formCollection)
        {
            int reportType = 0;
            string ProductName = string.Empty, CategoryName = string.Empty, CompanyName = string.Empty;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            if (!string.IsNullOrEmpty(formCollection["CompanyName"]))
                CompanyName = formCollection["CompanyName"];

            if (!string.IsNullOrEmpty(formCollection["CategoriesName"]))
                CategoryName = formCollection["CategoriesName"];
            if (!string.IsNullOrEmpty(formCollection["ProductsName"]))
                ProductName = formCollection["ProductsName"];

            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();

            byte[] stock = _transactionalReportService.StockLedgerReport(fromDate, toDate, User.Identity.Name, ConcernID, reportType, CompanyName, CategoryName, ProductName);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockLedgerReportExcel(FormCollection formCollection)
        {
            int reportType = 0;
            string ProductName = string.Empty, CategoryName = string.Empty, CompanyName = string.Empty;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            if (!string.IsNullOrEmpty(formCollection["CompanyName"]))
                CompanyName = formCollection["CompanyName"];

            if (!string.IsNullOrEmpty(formCollection["CategoriesName"]))
                CategoryName = formCollection["CategoriesName"];
            if (!string.IsNullOrEmpty(formCollection["ProductsName"]))
                ProductName = formCollection["ProductsName"];

            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();

            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());

            byte[] stock = _transactionalReportService.StockLedgerReportExcel(fromDate, toDate, User.Identity.Name, ConcernID, reportType, CompanyName, CategoryName, ProductName, fileType);
            TempData["ReportData"] = stock;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }
        }



        [HttpPost]
        [Authorize]
        public PartialViewResult SupplierLedger(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int SupplierID = 0;
            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierID = Convert.ToInt32(formCollection["SuppliersId"]);

            int ReportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            byte[] bytes = _transactionalReportService.SupplierLedger(User.Identity.Name, User.Identity.GetConcernId(), fromDate, toDate, SupplierID, ReportType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public ActionResult CustomerLedger(FormCollection formCollection)
        {
            int CustomerID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
            int ReportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = null;

            //if (ReportType == 1) //Summary
            //    bytes = _transactionalReportService.CustomerLedgerSummary(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);
            //else
            //    bytes = _transactionalReportService.CustomerLedgerDetails(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID);


            bytes = _transactionalReportService.CustomerLedger(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerID, ReportType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        public async Task<JsonResult> GetConcernName(DateTime ClientDateTime)
        {
            TempData["ClientDateTime"] = ClientDateTime;
            string ConcernName = string.Empty, expiremsg = string.Empty;
            if (!TempData.ContainsKey("ConcernName"))
            {
                var Concern = await Task.Run(() => _SisterConcernService.GetSisterConcernById(User.Identity.GetConcernId()));
                TempData["ConcernName"] = Concern.Name;
                ConcernName = Concern.Name;

            }
            else
            {
                ConcernName = TempData.Peek("ConcernName").ToString();
            }

            if (!TempData.ContainsKey("ExpireMessage"))
            {
                var sysinfo = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                DateTime currentDate = GetLocalDateTime();
                if (sysinfo != null && sysinfo.ExpireDate > currentDate)
                {
                    if ((currentDate.Day >= 1 && currentDate.Day <= sysinfo.ExpireDate.Value.Day) && sysinfo.ExpireDate.Value.Month == currentDate.Month)
                    {
                        expiremsg = sysinfo.WarningMsg;
                        TempData["ExpireMessage"] = sysinfo.WarningMsg;
                    }
                }
                else
                {
                    TempData["ExpireMessage"] = "";
                }

            }
            else
            {
                expiremsg = TempData.Peek("ExpireMessage").ToString();
            }

            if (Session["SystemInfo"] == null)
            {
                var sysInfo = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                Session["SystemInfo"] = _mapper.Map<SystemInformation, CreateSystemInformationViewModel>(sysInfo);
            }
            return Json(new { ConcernName, expiremsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferInvoiceByID()
        {
            int TransferID = (int)TempData["TransferID"];
            byte[] bytes = _transactionalReportService.TransferInvoiceByID(TransferID, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public ActionResult TransferReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            //int ConcernID = 0;
            //if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
            //    ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            byte[] bytes = _transactionalReportService.TransferReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), 0);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult AdminTransferReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            byte[] bytes = _transactionalReportService.TransferReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), ConcernID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        public PartialViewResult SMSReport(FormCollection formCollection)
        {
            int Status = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["Status"]))
                Status = Convert.ToInt32(formCollection["Status"].ToString());
            byte[] stock = _transactionalReportService.SMSReport(User.Identity.Name, User.Identity.GetConcernId(), Status, fromDate, toDate, false);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminSMSReport(FormCollection formCollection)
        {
            int ConcernID = 0;
            int Status = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            if (!string.IsNullOrEmpty(formCollection["Status"]))
                Status = Convert.ToInt32(formCollection["Status"].ToString());
            byte[] bytes = _transactionalReportService.SMSReport(User.Identity.Name, User.Identity.GetConcernId(), Status, fromDate, toDate, true, ConcernID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        public JsonResult GetClientDateTime(string ClientDateTime)
        {
            Session["ClientDateTime"] = ClientDateTime;
            return Json(ClientDateTime, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetSummaryReport(FormCollection formCollection)
        {
            int ConcernID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();

            byte[] bytes = _transactionalReportService.GetSummaryReport(fromDate, ConcernID, User.Identity.Name);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult TrialBalance(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.GetTrialBalance(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime, 0, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult ProfitLossAccount(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.ProfitLossAccount(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult BalanceSheet(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.BalanceSheet(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult BalanceSheetNew(FormCollection formCollection)
        {
            DateTime asOnDate = Convert.ToDateTime(formCollection["AsOnDate"].ToString());
            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.BalanceSheetNew(asOnDate, User.Identity.Name, ConcernID, ClientDateTime, ConcernID, false);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult AdminBalanceSheetNew(FormCollection formCollection)
        {
            DateTime asOnDate = Convert.ToDateTime(formCollection["AsOnDate"].ToString());
            string ClientDateTime = formCollection["ClientDateTime"];

            //int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);


            int ConcernID = 0;
            int SelectedConcern = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                SelectedConcern = Convert.ToInt32(formCollection["ConcernID"]);

            ConcernID = User.Identity.GetConcernId();

            byte[] bytes = _transactionalReportService.BalanceSheetNew(asOnDate, User.Identity.Name, ConcernID, ClientDateTime, SelectedConcern, true);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminTrialBalance(FormCollection formCollection)
        {
            int ConcernID = 0;
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            string ClientDateTime = formCollection["ClientDateTime"];
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
             ConcernID = Convert.ToInt32(formCollection["ConcernID"].ToString());

            byte[] bytes = _transactionalReportService.GetTrialBalanceNew(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), ClientDateTime, ConcernID, true);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminProfitLossAccount(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            string ClientDateTime = formCollection["ClientDateTime"];

            int selectedConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                 selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]); 
            int ConcernID  = User.Identity.GetConcernId();

            byte[] bytes;
            if (selectedConcernID > 0)
            {
                 bytes = _transactionalReportService.ProfitLossAccount(fromDate, toDate, User.Identity.Name, selectedConcernID, ClientDateTime);
            }
            else
            {
                bytes = _transactionalReportService.AdminProfitLossAccount(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime);
            }
           
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminStockSummaryReport(FormCollection formCollection)
        {
            int reportType = 0, ConcernID = 0;
            string CompanyName = string.Empty;
            string CategoryName = string.Empty;
            string ProductsName = string.Empty;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesName"]))
                CompanyName = formCollection["CompaniesName"];
            if (!string.IsNullOrEmpty(formCollection["CategoriesName"]))
                CategoryName = formCollection["CategoriesName"];
            if (!string.IsNullOrEmpty(formCollection["ProductsName"]))
                ProductsName = formCollection["ProductsName"];
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"].ToString());
            byte[] stock = _transactionalReportService.AdminStockSummaryReport(User.Identity.Name, ConcernID, reportType, CompanyName, CategoryName, ProductsName, User.Identity.GetConcernId());
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult HireAccountDetails(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"]);
            var daterange = GetFirstAndLastDateOfMonth(fromDate);

            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.HireAccountDetails(daterange.Item1, daterange.Item2, User.Identity.Name, ConcernID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult MonthlyTransactionReport(FormCollection formCollection)
        {
            // DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");


            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);
            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {

                var DateRange = GetFirstAndLastDateOfYear(Convert.ToInt32(formCollection["Year"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;


                //fromDate = Convert.ToDateTime(formCollection["Year"]);
                //fromDate = new DateTime(fromDate.Year, 1, 1);
                //fromDate = new DateTime(fromDate.Year, 12, 31);
            }

            //   DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();

            byte[] bytes = _transactionalReportService.MonthlyTransactionReport(fromDate, ToDate, User.Identity.Name, ConcernID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpPost]
        [Authorize]
        public PartialViewResult LiabilityReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int HeadID = 0;
            if (!string.IsNullOrEmpty(formCollection["SIHID"]))
                HeadID = Convert.ToInt32(formCollection["SIHID"]);
            //bool OnlyHead = false;
            //if (!string.IsNullOrWhiteSpace(formCollection["OnlyHead"]))
            //    OnlyHead = formCollection["OnlyHead"] == "1" ? true : false;
            string OnlyHead = "";
            if (!string.IsNullOrWhiteSpace(formCollection["OnlyHead"]))
                OnlyHead = formCollection["OnlyHead"] == "1" ? OnlyHead : "";

            //byte[] bytes = _transactionalReportService.LiabilityReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), HeadID, OnlyHead);
            byte[] bytes = _transactionalReportService.VoucherTransactionLedger(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), HeadID, OnlyHead);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }


        [HttpGet]
        [Authorize]
        public ActionResult BarCodeGenerate()
        {
            var Data = (POrder)TempData["DataForBarCodePrint"];

            byte[] bytes = _transactionalReportService.BarCodeGenrator(Data, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpPost]
        [Authorize]
        public ActionResult BarCodeGenerateByPOrderID(int POrderID)
        {
            byte[] bytes = _transactionalReportService.BarCodeGenratorByID(POrderID, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult PrintPOSInvoice()
        //{
        //    int POrderID = (int)TempData["POSSOrderID"];

        //    byte[] bytes = _transactionalReportService.PrintPOSInvoice(POrderID, User.Identity.Name, User.Identity.GetConcernId());

        //    TempData["ReportData"] = bytes;
        //    return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        //}


        [HttpGet]
        [Authorize]
        public ActionResult PrintIMEI()
        {
            int SDetailID = (int)TempData["SDetailID"];

            byte[] bytes = _transactionalReportService.PrintIMEI(SDetailID, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminSupplierDueRpt(FormCollection formCollection)
        {
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _basicReportService.ConcernWiseSupplierDueRpt(User.Identity.Name, User.Identity.GetConcernId(), 0, ConcernID, true);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminInstallmentCollectionRpt(FormCollection formCollection)
        {
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.InstallmentCollectionReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), 0, ConcernID, true);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminCashDeliveryReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.CashDeliverReport(fromDate,
                toDate, User.Identity.Name, User.Identity.GetConcernId(), 0, true, ConcernID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult AdminExpenditureReport(FormCollection formCollection)
        {
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            EnumCompanyTransaction reportType = (EnumCompanyTransaction)Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = _transactionalReportService.ExpenditureReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), reportType, 0, true, ConcernID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DistributorAnalysis(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();
            byte[] bytes = _transactionalReportService.DistributorAnalysis(fromDate, toDate, User.Identity.Name, ConcernID);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult AuditInvoice(int OrderID, int salestype)

        {
            byte[] bytes = null;
            if (salestype == 1)
            {
                bytes = _transactionalReportService.SalesInvoiceReport(OrderID, User.Identity.Name, User.Identity.GetConcernId());
            }
            else if (salestype == 2)
            {
                bytes = _transactionalReportService.CreditSalesInvoiceReportByID(OrderID, User.Identity.Name, User.Identity.GetConcernId());
            }
            else if (salestype == 3)
            {
                bytes = _transactionalReportService.CrditSalesMoneyReceiptByID(OrderID, User.Identity.Name, User.Identity.GetConcernId());
            }
            else if (salestype == 4)
            {
                bytes = _transactionalReportService.CashCollectionMoneyReceiptByID(OrderID, User.Identity.Name, User.Identity.GetConcernId());
            }
            else if (salestype == 5)
            {
                bytes = _transactionalReportService.ExpenseIncomeMoneyReceipt(User.Identity.Name, User.Identity.GetConcernId(), OrderID, false);
            }
            else if (salestype == 6)
            {
                bytes = _transactionalReportService.ExpenseIncomeMoneyReceipt(User.Identity.Name, User.Identity.GetConcernId(), OrderID, true);
            }
            else if (salestype == 7)
            {

                bytes = _transactionalReportService.CashDeliveryMoneyReceiptPrint(OrderID, User.Identity.Name, User.Identity.GetConcernId());
            }



            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult AdjustmentReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int ReportType = 0;

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            byte[] bytes = _transactionalReportService.AdjustmentReport(fromDate, toDate, User.Identity.Name, ReportType, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult AdvanceLoanReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int EmployeeID = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeeID"]))
                EmployeeID = Convert.ToInt32(formCollection["EmployeeID"]);
            bool OnlyEmployee = false;
            if (!string.IsNullOrWhiteSpace(formCollection["OnlyEmployee"]))
                OnlyEmployee = formCollection["OnlyEmployee"] == "1" ? true : false;

            byte[] bytes = _transactionalReportService.AdvanceLoanReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), EmployeeID, OnlyEmployee);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }
        [HttpPost]
        [Authorize]
        public PartialViewResult StockQTYReport(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, GodownsId = 0, ColorsId = 0, PCategoryId = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["GodownsId"]))
                GodownsId = Convert.ToInt32(formCollection["GodownsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ColorsId"]))
                ColorsId = Convert.ToInt32(formCollection["ColorsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["PCategoriesId"]))
                PCategoryId = Convert.ToInt32(formCollection["PCategoriesId"].ToString());

            byte[] stock = _transactionalReportService.StockQTYReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, GodownsId, ColorsId, PCategoryId, IsVATManager());
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SalesMoneyReceipt()
        {
            SOrder Sorder = (SOrder)TempData["MoneyReceiptData"];
            TempData["MoneyReceiptData"] = Sorder;
            byte[] bytes = _transactionalReportService.SalesOrderMoneyReceipt(Sorder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpGet]
        [Authorize]
        public PartialViewResult SalesMoneyReceiptByID()
        {
            int SOrderID = (int)TempData["SOrderID"];
            bool isPosMoney = (bool)TempData["isPosRecipt"];
            TempData["SOrderID"] = SOrderID;
            byte[] bytes = _transactionalReportService.SOrderMoneyReceiptByID(SOrderID, User.Identity.Name, User.Identity.GetConcernId(), isPosMoney);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult LastPayAdjReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            byte[] bytes = _transactionalReportService.LastPayAdjReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockForcastingReport(FormCollection formCollection)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            DateTime fDate = DateTime.MinValue;

            fDate = Convert.ToDateTime(formCollection["FromDate"]);
            fromDate = fDate.AddMonths(-11);
            var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["FromDate"]));
            toDate = DateRange.Item2;

            //int year = Convert.ToInt32(formCollection["FromDate"].ToString());
            //DateTime fromDate = new DateTime(year, 1, 1, 0, 0, 0);
            //DateTime toDate = new DateTime(year, 12, 31, 23, 59, 59);

            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);
            byte[] stock = _transactionalReportService.StockForcastingReport(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockForcastingReportProductWise(FormCollection formCollection)
        {

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            DateTime fDate = DateTime.MinValue;

            fDate = Convert.ToDateTime(formCollection["FromDate"]);
            fromDate = fDate.AddMonths(-11);
            var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["FromDate"]));
            toDate = DateRange.Item2;

            //int year = Convert.ToInt32(formCollection["FromDate"].ToString());
            //DateTime fromDate = new DateTime(year, 1, 1, 0, 0, 0);
            //DateTime toDate = new DateTime(year, 12, 31, 23, 59, 59);
            string ClientDateTime = formCollection["ClientDateTime"];
            int ProductID = 0;
            if (!string.IsNullOrEmpty(formCollection["ProductsID"]))
                ProductID = Convert.ToInt32(formCollection["ProductsID"].ToString());
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);
            byte[] stock = _transactionalReportService.StockForcastingReportProductWise(fromDate, toDate, User.Identity.Name, ConcernID, ClientDateTime, ProductID);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult StockReportWithDate(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();
            byte[] stock = _transactionalReportService.StockDetailReportWithDate(User.Identity.Name, ConcernID, reportType, CompanyID, CategoryID, ProductsId);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }



        [HttpPost]
        [Authorize]
        public PartialViewResult TotalLiabilityPayRec(FormCollection formCollection)
        {
            DateTime asOnDate = Convert.ToDateTime(formCollection["AsOnDate"].ToString());
            string ClientDateTime = formCollection["ClientDateTime"];
            int ConcernID = string.IsNullOrEmpty(formCollection["ConcernID"]) ? User.Identity.GetConcernId() : Convert.ToInt32(formCollection["ConcernID"]);

            byte[] bytes = _transactionalReportService.TotalLiabilityPayRec(asOnDate, User.Identity.Name, ConcernID, ClientDateTime);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SalesInvoiceWithOutBarcode()
        {
            SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            TempData["salesInvoiceData"] = sorder;
            int orderId = sorder.SOrderID;
            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());

            byte[] bytes = _transactionalReportService.SalesInvoiceWithOutBarcodeReport(sorder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult SalesInvoiceWithOutBarcodeById()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.SalesInvoiceWithOutBarcodeReport(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult CashCollectionReportNew(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = 0;

            int CustomerId = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            //if (formCollection["ReportType"] != null)
            //{
            //    reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            //    formCollection["CustomersId"] = "0";

            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
            //    {
            //        reportType = 0;
            //        CustomerId = Convert.ToInt32(formCollection["CustomersId"]);
            //    }
            //    else
            //        CustomerId = 0;

            //}


            byte[] bytes = _transactionalReportService.CashCollectionReportNew(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerId, reportType);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ServiceCharge(FormCollection formCollection)
        {
            //int Month = 0;

            //if (!string.IsNullOrEmpty(formCollection["Month"]))
            //    Month = Convert.ToInt32(formCollection["Month"].ToString());
            DateTime date = Convert.ToDateTime(formCollection["FromDate"].ToString());
            int sMonth = date.Month;
            int sYear = date.Year;
            byte[] charge = _transactionalReportService.ServiceCharge(sMonth, sYear, User.Identity.Name, User.Identity.GetConcernId(), date);
            TempData["ReportData"] = charge;
            return CustomPartialView();
        }
        [HttpGet]
        [Authorize]
        public PartialViewResult HireReturnInvoice()
        {
            IEnumerable<ReplaceOrderDetail> rorderdetails = (IEnumerable<ReplaceOrderDetail>)TempData["ReturnInvoicedetails"];
            ReplaceOrder ROrder = (ReplaceOrder)TempData["ReturnInvoice"];
            byte[] bytes = _transactionalReportService.HireReturnInvoiceReport(rorderdetails, ROrder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpGet]
        [Authorize]
        public PartialViewResult HireReturnInvoiceById()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.HireReturnInvoiceReportByID(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpGet]
        [Authorize]
        public PartialViewResult WarrantyInvoice()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.WarrantyInvoice(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult UserAuditDetailsReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"]);
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"]);
            int ObjectType = 0;

            if (!string.IsNullOrEmpty(formCollection["ObjectType"]))
                ObjectType = Convert.ToInt32(formCollection["ObjectType"].ToString().Replace(',', ' '));


            byte[] bytes = _transactionalReportService.UserAuditDetailsReport(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), (EnumObjectType)ObjectType);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpGet]
        [Authorize]
        public PartialViewResult LoanInvoiceById()
        {
            int orderId = (int)TempData["loanId"];
            TempData.Remove("loanId");
            byte[] bytes = _transactionalReportService.GetBankLoanInvoice(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult LoanCollectionInvoiceById()
        {
            int collectionId = (int)TempData["loanCollectionId"];
            TempData.Remove("loanCollectionId");
            byte[] bytes = _transactionalReportService.GetBankLoanCollectionInvoice(collectionId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetAllPendingLoan()
        {
            byte[] bytes = _transactionalReportService.GetPendingBankLoan(User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }



        [HttpPost]
        [Authorize]
        public ActionResult CustomerAdjustmentReport(FormCollection formCollection)
        {
            int ConcernID = 0, selectedConcernID = 0; int AdjustmentType = 0; int CustomerId = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            ConcernID = User.Identity.GetConcernId();

            if (!string.IsNullOrEmpty(formCollection["AdjustmentType"]))
                AdjustmentType = Convert.ToInt32(formCollection["AdjustmentType"].ToString().Replace(',', ' '));

            byte[] bytes = _transactionalReportService.CustomerAdjustmentReport(fromDate, toDate, User.Identity.Name, ConcernID, (EnumTranType)AdjustmentType, selectedConcernID, CustomerId);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        [HttpPost]
        [Authorize]
        public ActionResult SupplierAdjustmentReport(FormCollection formCollection)
        {
            int ConcernID = 0, selectedConcernID = 0; int AdjustmentType = 0; int SupplierId = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                SupplierId = Convert.ToInt32(formCollection["SuppliersId"]);

            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                selectedConcernID = Convert.ToInt32(formCollection["ConcernID"]);

            ConcernID = User.Identity.GetConcernId();

            if (!string.IsNullOrEmpty(formCollection["AdjustmentType"]))
                AdjustmentType = Convert.ToInt32(formCollection["AdjustmentType"].ToString().Replace(',', ' '));

            byte[] bytes = _transactionalReportService.SupplierAdjustmentReport(fromDate, toDate, User.Identity.Name, ConcernID, (EnumTranType)AdjustmentType, selectedConcernID, SupplierId);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DiscountAdjReportNew(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int reportType = 0;

            int CustomerId = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                CustomerId = Convert.ToInt32(formCollection["CustomersId"]);

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());


            byte[] bytes = _transactionalReportService.DiscountAdjReportNew(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), CustomerId, reportType);
            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult TransferReportNewFormat(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");
            int FromConern = 0, ToConcern = 0;

            if (!string.IsNullOrEmpty(formCollection["FromConcern"].ToString()))
                FromConern = Convert.ToInt32(formCollection["FromConcern"]);

            if (!string.IsNullOrEmpty(formCollection["TOConcern"].ToString()))
                ToConcern = Convert.ToInt32(formCollection["TOConcern"]);


            byte[] bytes = _transactionalReportService.TransferReportNewFormat(fromDate, toDate, User.Identity.Name, User.Identity.GetConcernId(), FromConern, ToConcern);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }



        public PartialViewResult AdminCashInHandReport(FormCollection formCollection)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MinValue;
            int ReportType = 0, CustomerType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                ReportType = Convert.ToInt32(formCollection["ReportType"]);

            if (!string.IsNullOrEmpty(formCollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formCollection["CustomerType"]);

            if (ReportType == 1)
            {
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                ToDate = fromDate;
            }
            else if (ReportType == 2)
            {
                var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["Month"]));
                fromDate = DateRange.Item1;
                ToDate = DateRange.Item2;
            }
            else if (ReportType == 3)
            {
                int year = Convert.ToInt32(formCollection["Year"]);
                fromDate = new DateTime(year, 1, 1);
                ToDate = new DateTime(year, 12, 31, 23, 59, 59);
            }
            int ConcernID = 0;
            int SelectedConcern = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
              SelectedConcern = Convert.ToInt32(formCollection["ConcernID"]);
       
            ConcernID = User.Identity.GetConcernId();

            byte[] bytes = _transactionalReportService.AdminCashInHandReport(User.Identity.Name, ConcernID, ReportType, fromDate, ToDate, SelectedConcern);

            TempData["ReportData"] = bytes;
            return CustomPartialView();

        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductWiseBenefitReportNew(FormCollection formCollection)
        {
            int ProductID = 0, CompanyID = 0, CategoryID = 0;
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductID = Convert.ToInt32(formCollection["ProductsId"]);
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            byte[] bytes = _transactionalReportService.ProductWiseBenefitReportNew(fromDate, toDate, ProductID, CompanyID, CategoryID, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult EmobileSalesInvoice()
        {
            SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            TempData["salesInvoiceData"] = sorder;
            int orderId = sorder.SOrderID;
            //byte[] bytes = _transactionalReportService.SalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId());

            //byte[] bytes = _transactionalReportService.EmobileSalesInvoiceReport(sorder, User.Identity.Name, User.Identity.GetConcernId());
            byte[] bytes = _transactionalReportService.EmobileSalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId(), false);  
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult EmobileSalesInvoiceById()
        {
            int orderId = (int)TempData["OrderId"];
            bool IsFakeInvoice = false;
            if (TempData.ContainsKey("FInvoice"))
                bool.TryParse(TempData["FInvoice"].ToString(), out IsFakeInvoice);

            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.EmobileSalesInvoiceReport(orderId, User.Identity.Name, User.Identity.GetConcernId(), IsFakeInvoice);
            TempData["ReportData"] = bytes;
            return CustomPartialView();


        }

        [HttpGet]
        [Authorize]
        public PartialViewResult EmobileCreditSalesInvoiceReportByID()
        {
            int orderId = (int)TempData["OrderId"];
            byte[] bytes = _transactionalReportService.EmobileCreditSalesInvoiceReportByID(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult EmobileCreditSalesInvoice()
        {
            CreditSale sorder = (CreditSale)TempData["CreditSalesInvoiceData"];
            byte[] bytes = _transactionalReportService.EmobileCreditSalesInvoiceReport(sorder, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult PrintPOSInvoice()
        {

            SOrder sorder = (SOrder)TempData["salesInvoiceData"];
            bool isPreview = TempData.ContainsKey("IsPreview") ? (bool)TempData["IsPreview"] : false;
            TempData["salesInvoiceData"] = sorder;
            int orderId = sorder.SOrderID;

            byte[] bytes = _transactionalReportService.PrintPOSInvoice(orderId, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpGet]
        [Authorize]
        public ActionResult PrintPOSInvoiceById()
        {

            int POrderID = (int)TempData["POSSOrderID"];

            byte[] bytes = _transactionalReportService.PrintPOSInvoice(POrderID, User.Identity.Name, User.Identity.GetConcernId());

            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceiptByIDOfSo()
        {
            var SorderId = (int)TempData["SorderId"];
            bool isPosRecipt = (bool)TempData["isPosRecipt"];
            byte[] bytes = _transactionalReportService.SOrderMoneyReceiptByID(SorderId, User.Identity.Name, User.Identity.GetConcernId(), isPosRecipt);
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        #region Do Report Start Here 

        [HttpPost]
        [Authorize]
        public ActionResult DOInvoiceReport(FormCollection formCollection)
        {
            int DOID = (int)TempData["DOID"];
            byte[] bytes = _transactionalReportService.DOInvoiceReport(User.Identity.Name, User.Identity.GetConcernId(), DOID);

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public ActionResult DOReport(FormCollection formCollection)
        {
            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            int CustomerID = 0, SupplierID = 0, POType = 0;

            if (!string.IsNullOrEmpty(formCollection["CustomersId"]))
                int.TryParse(formCollection["CustomersId"], out CustomerID);

            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                int.TryParse(formCollection["SuppliersId"], out SupplierID);

            if (!string.IsNullOrEmpty(formCollection["chkDOType"]))
                int.TryParse(formCollection["chkDOType"], out POType);

            byte[] bytes = _transactionalReportService.DOReport(User.Identity.Name, User.Identity.GetConcernId(), fromDate, toDate, CustomerID, SupplierID, POType);

            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        

        [HttpPost]
        [Authorize]
        public ActionResult DOInvoiceReportExcel(FormCollection formCollection)
        {
            int DOID = (int)TempData["DOID"];
            int fileType = 1;
            byte[] bytes = _transactionalReportService.DOInvoiceReportExcel(User.Identity.Name, User.Identity.GetConcernId(), DOID, fileType);

            TempData["ReportData"] = bytes;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult ProductWisePurchaseDOReport(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());

            byte[] stock = _transactionalReportService.ProductWisePurchaseDOReport(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, fromDate, toDate);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }

        #endregion DO Report End Here

        [HttpPost]
        [Authorize]
        public PartialViewResult AdminProductStockReport(FormCollection formCollection)
        {
            int reportType = 0, ConcernID = 0;
            string CompanyName = string.Empty;
            string CategoryName = string.Empty;
            string ProductsName = string.Empty;

            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesName"]))
                CompanyName = formCollection["CompaniesName"];
            if (!string.IsNullOrEmpty(formCollection["CategoriesName"]))
                CategoryName = formCollection["CategoriesName"];
            if (!string.IsNullOrEmpty(formCollection["ProductsName"]))
                ProductsName = formCollection["ProductsName"];
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"].ToString());

            byte[] stock = _transactionalReportService.AdminProductStockReport(User.Identity.Name, ConcernID, reportType, CompanyName, CategoryName, ProductsName, User.Identity.GetConcernId(), fileType);
            TempData["ReportData"] = stock;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }
            //return CustomPartialView();
        }


        [HttpPost]
        [Authorize]
        public PartialViewResult RateWiseStockLedgerReport(FormCollection formCollection)
        {
            int reportType = 0;
            string ProductName = string.Empty, CategoryName = string.Empty, CompanyName = string.Empty;

            DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"].ToString() + " 12:00:00 AM");
            DateTime toDate = Convert.ToDateTime(formCollection["ToDate"].ToString() + " 11:59:59 PM");

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());

            if (!string.IsNullOrEmpty(formCollection["CompanyName"]))
                CompanyName = formCollection["CompanyName"];

            if (!string.IsNullOrEmpty(formCollection["CategoriesName"]))
                CategoryName = formCollection["CategoriesName"];
            if (!string.IsNullOrEmpty(formCollection["ProductsName"]))
                ProductName = formCollection["ProductsName"];

            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            else
                ConcernID = User.Identity.GetConcernId();

            byte[] stock = _transactionalReportService.RateWiseStockLedgerReport(fromDate, toDate, User.Identity.Name, ConcernID, reportType, CompanyName, CategoryName, ProductName);
            TempData["ReportData"] = stock;
            return CustomPartialView();
        }



        [HttpGet]
        [Authorize]
        public PartialViewResult WarrantyHireInvoice()
        {
            int orderId = (int)TempData["OrderId"];
            TempData["OrderId"] = orderId;
            byte[] bytes = _transactionalReportService.WarrantyHireInvoice(orderId, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return CustomPartialView();
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult StockSummaryReportZeroQty(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, GodownsId = 0, ColorsId = 0, PCategoryId = 0, StockType = 0;
            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
                CompanyID = Convert.ToInt32(formCollection["CompaniesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoryID = Convert.ToInt32(formCollection["CategoriesId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
                ProductsId = Convert.ToInt32(formCollection["ProductsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["GodownsId"]))
                GodownsId = Convert.ToInt32(formCollection["GodownsId"].ToString());
            if (!string.IsNullOrEmpty(formCollection["ColorsId"]))
                ColorsId = Convert.ToInt32(formCollection["ColorsId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["PCategoriesId"]))
                PCategoryId = Convert.ToInt32(formCollection["PCategoriesId"].ToString());

            if (!string.IsNullOrEmpty(formCollection["stockType"]))
                StockType = Convert.ToInt32(formCollection["stockType"]);

            int fileType = Convert.ToInt32(formCollection["FileType"].ToString());

            byte[] stock = _transactionalReportService.StockSummaryReportZeroQty(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyID, CategoryID, ProductsId, GodownsId, ColorsId, PCategoryId, IsVATManager(), StockType, fileType);

            TempData["ReportData"] = stock;
            if (fileType == 1)
            {
                return CustomPartialViewForExcel();
            }
            else
            {
                return CustomPartialView();
            }

        }


        [HttpPost]
        [Authorize]
        public PartialViewResult StockReportNew(FormCollection formCollection)
        {
            int reportType = 0, CompanyID = 0, CategoryID = 0, ProductsId = 0, GodownsId = 0, ColorsId = 0;

            List<int> CategoriesList = new List<int>();
            List<int> productList = new List<int>();
            List<int> CompanyList = new List<int>();
            List<int> godownList = new List<int>();
            List<int> colorList = new List<int>();

            if (!string.IsNullOrEmpty(formCollection["ReportType"]))
                reportType = Convert.ToInt32(formCollection["ReportType"].ToString());
            if (!string.IsNullOrEmpty(formCollection["CompaniesId"]))
            {
                CompanyList = formCollection["CompaniesId"].Split(',').Select(d => int.Parse(d.TrimEnd())).ToList();

                if (CompanyList.Count == 1 && CompanyList.First() == 0)
                {
                    CompanyList = null;
                }
            }


            if (!string.IsNullOrEmpty(formCollection["CategoriesId"]))
                CategoriesList = formCollection["CategoriesId"].Split(new char[] { ',' }).Select(Int32.Parse).Distinct().ToList();


            if (!string.IsNullOrEmpty(formCollection["ProductsId"]))
            {
                productList = formCollection["ProductsId"].Split(',').Select(d => int.Parse(d.TrimEnd())).ToList();
            }

            if (!string.IsNullOrEmpty(formCollection["GodownsId"]))
            {
                godownList = formCollection["GodownsId"].Split(',').Select(d => int.Parse(d.TrimEnd())).ToList();

                if (godownList.Count == 1 && godownList.First() == 0)
                {
                    godownList = null;
                }
            }

            if (!string.IsNullOrEmpty(formCollection["ColorsId"]))
            {
                colorList = formCollection["ColorsId"].Split(',').Select(d => int.Parse(d.TrimEnd())).ToList();

                if (colorList.Count == 1 && colorList.First() == 0)
                {
                    colorList = null;
                }
            }

            byte[] stock = _transactionalReportService.StockDetailReportNew(User.Identity.Name, User.Identity.GetConcernId(), reportType, CompanyList, CategoriesList, productList, godownList, colorList, IsVATManager());

            TempData["ReportData"] = stock;
            return CustomPartialView();
        }



    }
}
