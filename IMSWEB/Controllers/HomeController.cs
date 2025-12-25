using IMSWEB.Model;
using IMSWEB.Report;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class HomeController : CoreController
    {
        ICreditSalesOrderService _salesOrderService;
        ITransactionalReport _transactionalReportService;
        ICustomerService _CustomerService;
        ISystemInformationService _SysInfoService;
        ISMSStatusService _SMSStatusService;
        ICreditSalesOrderService _creditSalesOrderService;
        IPrevBalanceService _PrevBalanceService;
        IStockService _stockService;
        private readonly IServiceChargeService _serviceChargeService;
        private readonly IServiceChargeDetailsService _serviceChargeDetailsService;
        private readonly ISisterConcernService _sisterConcernService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IBankLoanService _bankLoanService;
        private readonly IBankLoanDetailsService _bankLoanDetailsService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        private readonly IUserService _user;


        public HomeController(IErrorService errorService, ITransactionalReport transactionalReportService, ICreditSalesOrderService SaleOrderService,
            ICustomerService CustomerService,
              ISystemInformationService SysInfoService,
              ISMSStatusService SMSStatusService,
              ICreditSalesOrderService creditSalesOrderService,
              IPrevBalanceService PrevBalanceService,IStockService stockService, ISessionMasterService sessionMasterService,
              IServiceChargeService serviceChargeService, IServiceChargeDetailsService serviceChargeDetailsService, ISisterConcernService sisterConcernService, IBankLoanService bankLoanService, IBankLoanDetailsService bankLoanDetailsService, ISMSBillPaymentBkashService sMSBillPaymentBkashService, IUserService user
            )
            : base(errorService, SysInfoService)
        {
            _salesOrderService = SaleOrderService;
            _transactionalReportService = transactionalReportService;
            _CustomerService = CustomerService;
            _SysInfoService = SysInfoService;
            _SMSStatusService = SMSStatusService;
            _creditSalesOrderService = creditSalesOrderService;
            _PrevBalanceService = PrevBalanceService;
            _stockService = stockService;
            _serviceChargeService = serviceChargeService;
            _serviceChargeDetailsService = serviceChargeDetailsService;
            _sisterConcernService = sisterConcernService;
            _sessionMasterService = sessionMasterService;
            _bankLoanService = bankLoanService;
            _bankLoanDetailsService = bankLoanDetailsService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
            _user = user;

        }

        [Authorize]
        public async Task<ActionResult> Index()
        {

            #region add sessionMaster
            SessionMaster oSessionMaster = new SessionMaster();
            oSessionMaster.ConcernID = User.Identity.GetConcernId();
            oSessionMaster.UserID = User.Identity.GetUserId<int>();
            oSessionMaster.SessionStartDT = DateTime.Now;
            oSessionMaster.IPAddress = "192.168.1.109";
            _sessionMasterService.Add(oSessionMaster);
            _sessionMasterService.Save();
            #endregion
            List<UpcommingScheduleReport> Schedules = new List<UpcommingScheduleReport>();

            var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            //if (SystemInfo.IsAutoCreditInterestPer == 1)
            //{
            //    _salesOrderService.CalculatePenaltySchedules(User.Identity.GetConcernId());
            //}
            #region Opening Save for cash in hand report
            var pb = _PrevBalanceService.DailyBalanceProcess(User.Identity.GetConcernId());
            if (pb.Count != 0)
            {
                foreach (var item in pb)
                {
                    _PrevBalanceService.AddPrevBalance(item);
                }
            }
            _PrevBalanceService.Save();
            TempData["UserConcern"] = User.Identity.GetConcernId();

            #endregion

            DateTime LocalDate = GetLocalDateTime();
            ViewBag.LocalDateTime = new DateTime(LocalDate.Year, LocalDate.Month, LocalDate.Day);
            var upcomingInstallemnts = _salesOrderService.GetUpcomingSchedule(ViewBag.LocalDateTime, ViewBag.LocalDateTime);
            foreach (var item in upcomingInstallemnts)
            {
                //item.DefaultAmount = _salesOrderService.GetDefaultAmount(item.CreditSalesID, DateTime.Today);
                //item.TotalPaymentDue += item.DefaultAmount;
                Schedules.Add(item);
            }

            #region SMS
            //SMSRequest smslist = new SMSRequest();
            //var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

            //if (SysInfo.IsRemindSMSEnable == 1)
            //{

            //    int DaysBeforeSendSMS = SysInfo.DaysBeforeSendSMS;
            //    DateTime Instfromday = LocalDate.AddDays(DaysBeforeSendSMS).Date;
            //    DateTime Instto = Instfromday.AddHours(23).AddMinutes(59);
            //    var forSMSIntallments = _salesOrderService.GetUpcomingSchedule(Instfromday, Instto);

            //    DateTime fromdate = new DateTime(LocalDate.Year, LocalDate.Month, LocalDate.Day);
            //    DateTime toDate = fromdate.AddHours(23).AddMinutes(59);

            //    var alreadySendSMSs = from ss in _SMSStatusService.GetAllIQueryable()
            //                          where ss.EntryDate >= fromdate && ss.EntryDate <= toDate
            //                          select ss.CustomerID;

            //    var TomorrowInstallment = (from C in forSMSIntallments.Where(i => !(alreadySendSMSs.Contains(i.CustomerID)))
            //                               select new SMSRequest
            //                               {
            //                                   CustomerID = C.CustomerID,
            //                                   CustomerCode = C.CustomerCode,
            //                                   CustomerName = C.CustomerName,
            //                                   MobileNo = C.CustomerConctact,
            //                                   CustomerAddress = C.CustomerAddress,
            //                                   PresentDue = C.TotalPaymentDue,
            //                                   Date = (DateTime)C.RemaindDate,
            //                                   SMSType = EnumSMSType.InstallmentAlert
            //                               }).OrderBy(x => x.CustomerID).ToList();
            //    if (TomorrowInstallment.Count() > 0)
            //    {
            //        decimal counter = Math.Ceiling(Convert.ToDecimal(TomorrowInstallment.Count()) / 5);
            //        for (int i = 0; i < counter; i++)
            //        {
            //            var GroupSends = TomorrowInstallment.Skip(i * 5).Take(5).ToList();


            //            int concernId = User.Identity.GetConcernId();
            //            decimal previousBalance = 0m;
            //            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
            //            previousBalance = smsAmountDetails.TotalRecAmt;

            //            var Response = await Task.Run(() => SMSHTTPService.SendSMSAsync(EnumOnnoRokomSMSType.ListSms, GroupSends, previousBalance, SysInfo, User.Identity.GetUserId<int>()));
            //            if (Response.Count() > 0)
            //            {

            //                decimal smsBalanceCount = 0m;
            //                foreach (var item in Response)
            //                {
            //                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
            //                }
            //                #region udpate payment info                  

            //                decimal sysLastPayUpdateDate = smsBalanceCount * .45m;
            //                smsAmountDetails.TotalRecAmt = previousBalance - Convert.ToDecimal(sysLastPayUpdateDate);

            //                _smsBillPaymentBkashService.Update(smsAmountDetails);
            //                _smsBillPaymentBkashService.Save();
            //                #endregion

            //                Response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
            //                _SMSStatusService.AddRange(Response);
            //                _SMSStatusService.Save();
            //            }
            //        }
            //    }
            //}


            #endregion


            #region check if payment info exists or not, if not then insert data
            AddPaymentInfoData();
            #endregion

            #region Update Bank Loan penalty amount
            UpdateBankLoanPenaltyAmount();
            #endregion

            return View(Schedules);
        }

        void UpdateBankLoanPenaltyAmount()
        {
            DateTime currentDate = GetLocalDateTime();
            List<BankLoanDetails> loanDetails = _bankLoanDetailsService.GetAllDueLoanByCurrentDate(currentDate);
            if (loanDetails.Any())
            {
                foreach (var details in loanDetails)
                {
                    BankLoanPenaltyDetails penalty = _bankLoanDetailsService.GetLastPenaltyByLoanDetails(details.Id);
                    if (penalty == null)
                    {
                        BankLoan loan = _bankLoanService.GetById(details.BankLoanId);
                        decimal penaltyAmount = Math.Round(((loan.PrincipleLoanAmount * loan.PenaltyChargePercentage) / 100), 2);
                        details.PenaltyChargeAmount += penaltyAmount;
                        _bankLoanDetailsService.Update(details);

                        BankLoanPenaltyDetails newPenalty = new BankLoanPenaltyDetails
                        {
                            LoanDetailsId = details.Id,
                            PenaltyAmount = penaltyAmount,
                            PenaltyPercentage = loan.PenaltyChargePercentage,
                            PenaltyDate = currentDate
                        };
                        AddAuditTrail(newPenalty, true);

                        _bankLoanDetailsService.AddPenalty(newPenalty);
                    }
                    else
                    {
                        if (penalty.PenaltyDate.Year <= currentDate.Year && penalty.PenaltyDate.Month < currentDate.Month)
                        {
                            BankLoan loan = _bankLoanService.GetById(details.BankLoanId);
                            decimal penaltyAmount = Math.Round(((loan.PrincipleLoanAmount * loan.PenaltyChargePercentage) / 100), 2);
                            details.PenaltyChargeAmount += penaltyAmount;
                            _bankLoanDetailsService.Update(details);

                            BankLoanPenaltyDetails newPenalty = new BankLoanPenaltyDetails
                            {
                                LoanDetailsId = details.Id,
                                PenaltyAmount = penaltyAmount,
                                PenaltyPercentage = loan.PenaltyChargePercentage,
                                PenaltyDate = currentDate
                            };
                            AddAuditTrail(newPenalty, true);
                            _bankLoanDetailsService.AddPenalty(newPenalty);
                        }
                    }

                }
                _bankLoanDetailsService.Save();
            }

        }

        private void AddPaymentInfoData()
        {
            int concernId = User.Identity.GetConcernId();
            DateTime todaysDate = GetLocalDateTime();

            List<SisterConcern> sisterConcerns = _sisterConcernService.GetFamilyTree(concernId);
            if (sisterConcerns.Any())
            {
                foreach (var concern in sisterConcerns)
                {
                    ServiceCharge serviceCharge = _serviceChargeService.GetByYearAndConcern(concern.ConcernID, todaysDate.Year);
                    if (serviceCharge == null)
                    {

                        ServiceCharge newServiceCharge = new ServiceCharge
                        {
                            ServiceYear = todaysDate.Year,
                            ConcernId = concern.ConcernID,
                            TotalServiceCollection = 0m,
                            CreateDate = DateTime.Now,
                            CreatedBy = User.Identity.GetUserId<int>()
                        };
                        _serviceChargeService.Add(newServiceCharge);
                        bool isServiceInserted = _serviceChargeService.Save();
                        if (isServiceInserted)
                        {
                            serviceCharge = _serviceChargeService.GetByYearAndConcern(concern.ConcernID, todaysDate.Year);
                        }
                    }

                    List<ServiceChargeDetails> serviceChargeDetails = _serviceChargeDetailsService.GetAllByServiceId(serviceCharge.Id);
                    SisterConcern sisterConcern = _sisterConcernService.GetSisterConcernById(concern.ConcernID);
                    List<ServiceChargeDetails> addChargeList = new List<ServiceChargeDetails>();
                    if (serviceChargeDetails.Any())
                    {
                        ServiceChargeDetails lastCharge = serviceChargeDetails.OrderByDescending(d => d.Month).ThenByDescending(d => d.ServiceChargeId).FirstOrDefault();

                        if (lastCharge.Month < todaysDate.Month)
                        {
                            int differentsOfMonth = todaysDate.Month - lastCharge.Month;
                            int month = lastCharge.Month;
                            for (int i = 0; i < differentsOfMonth; i++)
                            {
                                ServiceChargeDetails serviceDetail = new ServiceChargeDetails
                                {
                                    ServiceChargeId = serviceCharge.Id,
                                    ExpectedServiceCharge = sisterConcern.ServiceCharge,
                                    Month = month + i + 1,
                                    IsPaid = false,
                                    PaidServiceCharge = 0m
                                };
                                addChargeList.Add(serviceDetail);
                            }
                        }
                    }
                    else
                    {
                        int differentsOfMonth = todaysDate.Month;
                        for (int i = 1; i <= differentsOfMonth; i++)
                        {
                            ServiceChargeDetails serviceDetail = new ServiceChargeDetails
                            {
                                ServiceChargeId = serviceCharge.Id,
                                ExpectedServiceCharge = sisterConcern.ServiceCharge,
                                Month = i,
                                IsPaid = false,
                                PaidServiceCharge = 0m
                            };
                            addChargeList.Add(serviceDetail);
                        }
                    }

                    if (addChargeList.Any())
                    {
                        _serviceChargeDetailsService.AddMultiple(addChargeList);
                        _serviceChargeDetailsService.Save();
                    }
                }
            }

            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult UpComingScheduleReport(FormCollection formCollection)
        {
            byte[] bytes = _transactionalReportService.UpComingScheduleReport(DateTime.Today, DateTime.Today, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpPost]
        public JsonResult ChangeRemindDate(int CustomerID, DateTime RemindDate, int CSScheduleID)
        {
            if (CSScheduleID > 0)
            {
                var schedule = _creditSalesOrderService.GetScheduleByScheduleID(CSScheduleID);
                if (schedule != null)
                {
                    schedule.RemindDate = RemindDate;
                    _creditSalesOrderService.UpdateSchedule(schedule);
                    _creditSalesOrderService.SaveSalesOrder();
                    AddToastMessage("", "Update Successfull", ToastType.Success);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else if (CustomerID > 0)
            {
                var customer = _CustomerService.GetCustomerById(CustomerID);
                customer.RemindDate = RemindDate;
                _CustomerService.UpdateCustomer(customer);
                _CustomerService.SaveCustomer();
                AddToastMessage("", "Update Successfull", ToastType.Success);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            AddToastMessage("", "Update failed.", ToastType.Error);
            return Json(false, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult getConcernName()
        {
            var userId = Convert.ToInt32(User.Identity.GetUserId());
            var userName = _user.GetUserNameById(userId);
            if (userName != null)
            {
                return Json(new { Status = true, Name = userName }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Name = "" }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        [Authorize]
        public JsonResult GetDailySalesAmt(string dataLength)
        {
            var widgetData = _SysInfoService.GetHomeWidgeSales(dataLength, User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }

        

        [HttpGet]
        [Authorize]
        public JsonResult GetYearlyData(string dataLength)
        {
            var widgetData = _SysInfoService.GetYearlyData(dataLength, User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }



    }
}