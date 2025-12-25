using IMSWEB.Helper;
using IMSWEB.Model;
using IMSWEB.Model.TO.Bkash;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Route("payment")]
    [AllowAnonymous]
    public class PaymentController : CoreController
    {
        private readonly IMiscellaneousService<ServiceChargeDetails> _miscellaneousService;
        private readonly IServiceChargeService _serviceChargeService;
        private readonly IServiceChargeDetailsService _serviceChargeDetailsService;
        private readonly ISystemInformationService _systemInformationService;
        private readonly ISisterConcernService _sisterConcernService;
        public PaymentController(IErrorService errorService, IMiscellaneousService<ServiceChargeDetails> miscellaneousService, IServiceChargeService serviceChargeService, IServiceChargeDetailsService serviceChargeDetailsService, ISystemInformationService systemInformationService, ISisterConcernService sisterConcernService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _miscellaneousService = miscellaneousService;
            _serviceChargeService = serviceChargeService;
            _serviceChargeDetailsService = serviceChargeDetailsService;
            _systemInformationService = systemInformationService;
            _sisterConcernService = sisterConcernService;
        }

        // GET: Payment
        public ActionResult Index(string paymentID, string status, int concernId = 0)
        {
            AddPaymentInfoData();
            #region if payment accessed for the first time then get data for entire life cycle
            DateTime todaysDate = GetLocalDateTime();

            if (!TempData.ContainsKey("ExpireMessage") && concernId > 0)
            {
                TempData["payConcernId"] = concernId;
                var sysinfo = _systemInformationService.GetSystemInformationByConcernId(concernId);

                if (sysinfo != null && sysinfo.ExpireDate > todaysDate)
                {
                    if ((todaysDate.Day >= 1 && todaysDate.Day <= sysinfo.ExpireDate.Value.Day) && sysinfo.ExpireDate.Value.Month == todaysDate.Month)
                    {
                        TempData["ExpireMessage"] = sysinfo.WarningMsg;
                    }
                }
                else
                {
                    TempData["ExpireMessage"] = "";
                }
            }

            if (TempData.ContainsKey("ExpireMessage"))
            {
                if (concernId > 0)
                    TempData["payConcernId"] = concernId;

                if (TempData.ContainsKey("payConcernId"))
                    concernId = Convert.ToInt32(TempData["payConcernId"].ToString());

                if (concernId == 0)
                    //concernId = User.Identity.GetConcernId();
                    concernId = Convert.ToInt32(TempData["UserConcern"]);

                TempData["payConcernId"] = concernId;

                int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concernId, todaysDate.Year);

                if (serviceChargeId > 0)
                {
                    Tuple<decimal, DateTime, DateTime, int> dueData = _serviceChargeDetailsService.GetDueServiceAmountByServicId(serviceChargeId, todaysDate);
                    if (dueData.Item1 > 0)
                    {
                        TempData["TotalChargeDue"] = dueData.Item1;
                        TempData["ServiceFrom"] = dueData.Item2;
                        TempData["ServiceTo"] = dueData.Item3;
                        TempData["ServiceRemainingMonth"] = dueData.Item4;
                    }
                }
            }
            if (concernId > 0)
            {
                var Concern = _sisterConcernService.GetSisterConcernById(concernId);
                TempData["ConcernName"] = Concern.Name;
                TempData["CheckId"] = Concern.ConcernID;
            }

            #endregion

            return View();
        }


        private void AddPaymentInfoData()
        {

            ApplicationUser LoggedUser = (ApplicationUser)TempData["UserName"];
            if (LoggedUser != null)
            {

                int concernId = LoggedUser.ConcernID;
                TempData["UserConcern"] = concernId;
                TempData["UserName"] = LoggedUser;

                //int concernId = User.Identity.GetConcernId();
                DateTime todaysDate = GetLocalDateTime();

                List<SisterConcern> sisterConcerns = _sisterConcernService.GetFamilyTreeForNotLoggedInUser(concernId);
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

        }

        [HttpGet]
        public ActionResult GetBkashTokenOrConfirmPayment(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                string idToken = PaymentHelper.GetGrantToken();
                TempData["bKashToken"] = idToken;

                var inv = new Random();

                //string invoice = _miscellaneousService.GetUniqueKey(d => d.Id);
                //invoice = "OCT-" + invoice;

                string totalChargeAmount = "0";
                if (TempData.ContainsKey("TotalChargeDue"))
                    totalChargeAmount = TempData["TotalChargeDue"].ToString();

                string concernId = "0";
                //if (TempData.ContainsKey("payConcernId"))
                //    concernId = TempData.ContainsKey("payConcernId").ToString();
                if (TempData.ContainsKey("payConcernId"))
                { 
                    concernId = TempData["payConcernId"].ToString(); 
                }
                string concernName  = CoreController.RemoveSpecialCharacters(_sisterConcernService.GetConcernNameById(Convert.ToInt32(concernId)));
                CreatePaymentResponseTO createResponse = PaymentHelper.CreatePaymentFromToken(totalChargeAmount, "sale", "BDT", concernName, idToken, concernId);
                return Json(createResponse, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ConfirmPaymentResponseTO response = new ConfirmPaymentResponseTO();
                if (TempData.ContainsKey("bkashToken"))
                {
                    response = PaymentHelper.ConfirmPaymentByPaymentId(paymentId, TempData["bKashToken"].ToString());
                    if (!string.IsNullOrEmpty(response.trxID))
                    {
                        //TO DO: save transaction details and update sys configs expiry date
                        //and remove all temp data related to payment and error message

                        if (TempData.ContainsKey("isSelectedConcern"))
                        {
                            UpdateSelectedConcerns(response);
                        }
                        else
                        {
                            #region udpate payment info
                            int concernId = Convert.ToInt32(TempData["CheckId"].ToString());

                            SystemInformation systemInformation = _systemInformationService.GetSystemInformationByConcernId(concernId);
                            SisterConcern concernDetails = _sisterConcernService.GetSisterConcernById(concernId);
                            DateTime? sysLastPayUpdateDate = systemInformation.ExpireDate;
                            int nextPaymentDay = sysLastPayUpdateDate.HasValue ? sysLastPayUpdateDate.Value.Day : 7;
                            DateTime nextPaymentDate = TempData.ContainsKey("ServiceTo") ? Convert.ToDateTime(TempData["ServiceTo"]) : DateTime.Now.AddMonths(1);
                            nextPaymentDate = nextPaymentDate.AddMonths(1);

                            //systemInformation.ExpireDate = nextPaymentDate;
                            systemInformation.ExpireDate = systemInformation.ExpireDate.HasValue ? systemInformation.ExpireDate.Value.AddMonths(1) : GetLocalDateTime().AddMonths(1);
                            _systemInformationService.UpdateSystemInformation(systemInformation);
                            _systemInformationService.SaveSystemInformation();
                            TempData.Remove("ExpireMessage");
                            #endregion

                            #region save payment data
                            DateTime todaysDate = GetLocalDateTime();
                            int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concernId, todaysDate.Year);
                            ServiceCharge serviceCharge = _serviceChargeService.GetById(serviceChargeId);
                            List<ServiceChargeDetails> chargeDetails = _serviceChargeDetailsService.GetAllDueDetailsByMasterId(serviceChargeId);
                            if (chargeDetails.Any())
                            {
                                decimal tCharge = chargeDetails.Sum(s => s.ExpectedServiceCharge);
                                serviceCharge.TotalServiceCollection += tCharge;
                                DateTime trDate;
                                if (!DateTime.TryParseExact(response.paymentExecuteTime, "yyyy-MM-dd'T'HH:mm:ss:fff 'GMT'zzz", null, DateTimeStyles.None, out trDate))
                                {
                                    trDate = DateTime.Now;
                                }


                                ServiceChargeDetails lastDueDetails = chargeDetails.OrderByDescending(d => d.Id).First();
                                //lastDueDetails.PaidServiceCharge = Convert.ToDecimal(TempData["TotalChargeDue"]);
                                lastDueDetails.Intent = response.intent;
                                lastDueDetails.InvoiceNo = response.merchantInvoiceNumber;
                                lastDueDetails.PaidServiceCharge = decimal.Parse(response.amount);
                                lastDueDetails.PaymentMobNo = response.customerMsisdn;
                                lastDueDetails.TransactionId = response.trxID;
                                lastDueDetails.TransactionStatus = response.transactionStatus;
                                lastDueDetails.StatusCode = response.statusCode;
                                lastDueDetails.StatusMessage = response.statusMessage;
                                lastDueDetails.ErrorCocde = response.errorCode;
                                lastDueDetails.ErrorMessage = response.errorMessage;
                                lastDueDetails.TransactionDate = trDate;
                                lastDueDetails.PaymentReference = response.payerReference;
                                lastDueDetails.PaymentId = response.paymentID;
                                lastDueDetails.Currency = response.currency;
                                lastDueDetails.ConcernName = concernDetails.Name;
                                foreach (var item in chargeDetails)
                                {
                                    item.IsPaid = true;
                                    _serviceChargeDetailsService.Update(item);
                                }
                                _serviceChargeDetailsService.Save();
                            }

                            if (TempData.ContainsKey("IsMultiPay"))
                            {
                                List<SisterConcern> concernList = _sisterConcernService.GetFamilyTreeForNotLoggedInUser(concernId).Where(c => c.ConcernID != concernId).ToList();
                                if (concernList.Any())
                                {
                                    foreach (var conernData in concernList)
                                    {
                                        SystemInformation childInformation = _systemInformationService.GetSystemInformationByConcernId(conernData.ConcernID);

                                        childInformation.ExpireDate = childInformation.ExpireDate.HasValue ? childInformation.ExpireDate.Value.AddMonths(1) : GetLocalDateTime().AddMonths(1);
                                        _systemInformationService.UpdateSystemInformation(childInformation);
                                        _systemInformationService.SaveSystemInformation();

                                        int childServiceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(conernData.ConcernID, todaysDate.Year);
                                        ServiceCharge childCharge = _serviceChargeService.GetById(childServiceChargeId);

                                        List<ServiceChargeDetails> childServiceDetails = _serviceChargeDetailsService.GetAllDueDetailsByMasterId(childServiceChargeId);
                                        if (childServiceDetails.Any())
                                        {
                                            decimal totalChargeForChild = childServiceDetails.Sum(s => s.ExpectedServiceCharge);
                                            childCharge.TotalServiceCollection += totalChargeForChild;
                                            foreach (var item in childServiceDetails)
                                            {
                                                item.IsPaid = true;
                                                _serviceChargeDetailsService.Update(item);
                                            }
                                            _serviceChargeDetailsService.Save();
                                        }
                                    }
                                }

                            }


                            #endregion

                            List<SMSRequest> sms = new List<SMSRequest>();

                            sms.Add(new SMSRequest()
                            {
                                MobileNo = concernDetails.SmsContactNo,
                                CustomerID = 0,
                                CustomerCode = "0",
                                TransNumber = "N/A",
                                Date = DateTime.Now,
                                PreviousDue = 0m,
                                ReceiveAmount = 0m,
                                PresentDue = 0m,
                                SMSType = EnumSMSType.OCTGreetings
                            });
                            var smStatus = SMSHTTPService.SendGreetings(EnumOnnoRokomSMSType.NumberSms, sms, 0);
                        }

                        TempData.Remove("TotalChargeDue");
                        TempData.Remove("ServiceFrom");
                        TempData.Remove("ServiceTo");
                        TempData.Remove("ServiceRemainingMonth");

                        TempData.Remove("bkashToken");
                        TempData.Remove("payConcernId");
                        TempData.Remove("IsMultiPay");
                        TempData.Remove("isSelectedConcern");
                        TempData.Remove("concernIds");
                    }
                    else
                    {
                        TempData["payConcernId"] = TempData["payConcernId"];
                        TempData["ExpireMessage"] = TempData["ExpireMessage"];
                    }

                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        private void UpdateSelectedConcerns(ConfirmPaymentResponseTO response)
        {
            int[] allConcernIds = TempData.Peek("concernIds") as int[];

            if (allConcernIds.Any())
            {
                int payConcernId = 0;
                payConcernId = allConcernIds[0];

                //if (User.Identity.GetConcernId() > 0)
                //{
                //    payConcernId = User.Identity.GetConcernId();
                //}
                //else
                //{
                //    payConcernId = allConcernIds[0];
                //}

                if (payConcernId > 0)
                {
                    DateTime todaysDate = GetLocalDateTime();
                    int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(payConcernId, todaysDate.Year);
                    ServiceCharge serviceCharge = _serviceChargeService.GetById(serviceChargeId);
                    List<ServiceChargeDetails> chargeDetails = _serviceChargeDetailsService.GetAllDueDetailsByMasterId(serviceChargeId);
                    if (chargeDetails.Any())
                    {


                        #region udpate payment info
                        int countMonth = chargeDetails.Count;
                        SystemInformation systemInformation = _systemInformationService.GetSystemInformationByConcernId(payConcernId);
                        SisterConcern concernDetails = _sisterConcernService.GetSisterConcernById(payConcernId);
                        DateTime? sysLastPayUpdateDate = systemInformation.ExpireDate;


                        //systemInformation.ExpireDate = nextPaymentDate;
                        systemInformation.ExpireDate = systemInformation.ExpireDate.HasValue ? systemInformation.ExpireDate.Value.AddMonths(countMonth) : GetLocalDateTime().AddMonths(1);
                        _systemInformationService.UpdateSystemInformation(systemInformation);
                        _systemInformationService.SaveSystemInformation();
                        TempData.Remove("ExpireMessage");

                        //mostafizur remove temp data

                        TempData.Remove("UserConcern");
                        TempData.Remove("UserName");
                        TempData.Remove("CheckId");

                        #endregion



                        decimal tCharge = chargeDetails.Sum(s => s.ExpectedServiceCharge);
                        serviceCharge.TotalServiceCollection += tCharge;
                        DateTime trDate;
                        if (!DateTime.TryParseExact(response.paymentExecuteTime, "yyyy-MM-dd'T'HH:mm:ss:fff 'GMT'zzz", null, DateTimeStyles.None, out trDate))
                        {
                            trDate = DateTime.Now;
                        }


                        ServiceChargeDetails lastDueDetails = chargeDetails.OrderByDescending(d => d.Id).First();
                        //lastDueDetails.PaidServiceCharge = Convert.ToDecimal(TempData["TotalChargeDue"]);
                        lastDueDetails.Intent = response.intent;
                        lastDueDetails.InvoiceNo = response.merchantInvoiceNumber;
                        lastDueDetails.PaidServiceCharge = decimal.Parse(response.amount);
                        lastDueDetails.PaymentMobNo = response.customerMsisdn;
                        lastDueDetails.TransactionId = response.trxID;
                        lastDueDetails.TransactionStatus = response.transactionStatus;
                        lastDueDetails.StatusCode = response.statusCode;
                        lastDueDetails.StatusMessage = response.statusMessage;
                        lastDueDetails.ErrorCocde = response.errorCode;
                        lastDueDetails.ErrorMessage = response.errorMessage;
                        lastDueDetails.TransactionDate = trDate;
                        lastDueDetails.PaymentReference = response.payerReference;
                        lastDueDetails.PaymentId = response.paymentID;
                        lastDueDetails.Currency = response.currency;
                        lastDueDetails.ConcernName = concernDetails.Name;
                        foreach (var item in chargeDetails)
                        {
                            item.IsPaid = true;
                            _serviceChargeDetailsService.Update(item);
                        }
                        _serviceChargeDetailsService.Save();
                    }
                }


                foreach (var concernId in allConcernIds.Where(a => a != payConcernId))
                {

                    #region save payment data
                    DateTime todaysDate = GetLocalDateTime();
                    _systemInformationService.SaveSystemInformation();

                    int childServiceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concernId, todaysDate.Year);
                    ServiceCharge childCharge = _serviceChargeService.GetById(childServiceChargeId);

                    List<ServiceChargeDetails> childServiceDetails = _serviceChargeDetailsService.GetAllDueDetailsByMasterId(childServiceChargeId);
                    if (childServiceDetails.Any())
                    {
                        int dueMonth = childServiceDetails.Count;
                        SystemInformation childInformation = _systemInformationService.GetSystemInformationByConcernId(concernId);

                        childInformation.ExpireDate = childInformation.ExpireDate.HasValue ? childInformation.ExpireDate.Value.AddMonths(dueMonth) : GetLocalDateTime().AddMonths(1);
                        _systemInformationService.UpdateSystemInformation(childInformation);
                        decimal totalChargeForChild = childServiceDetails.Sum(s => s.ExpectedServiceCharge);
                        childCharge.TotalServiceCollection += totalChargeForChild;
                        foreach (var item in childServiceDetails)
                        {
                            item.IsPaid = true;
                            _serviceChargeDetailsService.Update(item);
                        }
                        _serviceChargeDetailsService.Save();
                    }


                    #endregion

                    SisterConcern concernDetails = _sisterConcernService.GetSisterConcernById(concernId);

                    List<SMSRequest> sms = new List<SMSRequest>();

                    sms.Add(new SMSRequest()
                    {
                        MobileNo = concernDetails.SmsContactNo,
                        CustomerID = 0,
                        CustomerCode = "0",
                        TransNumber = "N/A",
                        Date = DateTime.Now,
                        PreviousDue = 0m,
                        ReceiveAmount = 0m,
                        PresentDue = 0m,
                        SMSType = EnumSMSType.OCTGreetings
                    });
                    var smStatus = SMSHTTPService.SendGreetings(EnumOnnoRokomSMSType.NumberSms, sms, 0);
                }
            }
        }


        public ActionResult GetPaymentForAllOrSingle(bool isAllConcern)
        {
            DateTime currentDate = GetLocalDateTime();
            int concernId = 0;
            if (TempData.ContainsKey("payConcernId"))
                concernId = Convert.ToInt32(TempData["payConcernId"]);
            if (concernId > 0)
                TempData["payConcernId"] = concernId;

            if (TempData.ContainsKey("payConcernId"))
                concernId = Convert.ToInt32(TempData["payConcernId"]);

            if (TempData.ContainsKey("UserConcern"))
                concernId = Convert.ToInt32(TempData["UserConcern"]);

            if (concernId > 0)
                TempData["payConcernId"] = concernId;

            if (concernId == 0)
                return Json(new { msg = "No concern found for payment", JsonRequestBehavior.AllowGet });

            if (!isAllConcern)
            {
                TempData.Remove("IsMultiPay");

                int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concernId, currentDate.Year);

                if (serviceChargeId > 0)
                {
                    Tuple<decimal, DateTime, DateTime, int> dueData = _serviceChargeDetailsService.GetDueServiceAmountByServicId(serviceChargeId, currentDate);
                    if (dueData.Item1 > 0)
                    {
                        TempData["TotalChargeDue"] = dueData.Item1;
                    }
                    return Json(new { msg = "ok", amount = dueData.Item1, totalConcern = 1, noOfMonth = dueData.Item4 }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                decimal totalAmount = 0m;
                int totalMonth = 0;
                TempData["IsMultiPay"] = true;
                List<SisterConcern> allConcern = _sisterConcernService.GetFamilyTreeForNotLoggedInUser(concernId);
                if (allConcern.Any())
                {
                    foreach (var concern in allConcern)
                    {
                        int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concern.ConcernID, currentDate.Year);

                        if (serviceChargeId > 0)
                        {
                            Tuple<decimal, DateTime, DateTime, int> dueData = _serviceChargeDetailsService.GetDueServiceAmountByServicId(serviceChargeId, currentDate);
                            if (dueData.Item1 > 0)
                                totalAmount += dueData.Item1;
                            totalMonth += dueData.Item4;
                        }
                    }
                    TempData["TotalChargeDue"] = totalAmount;
                    return Json(new { msg = "ok", amount = totalAmount, totalConcern = allConcern.Count, noOfMonth = totalMonth }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { msg = "No concern found for payment", JsonRequestBehavior.AllowGet });
        }



        public ActionResult GetPaymentForConcerns(int[] allConcernIds)
        {
            TempData["isSelectedConcern"] = true;
            TempData["concernIds"] = allConcernIds;
            DateTime currentDate = GetLocalDateTime();
            int concernId = 0;
            if (TempData.ContainsKey("payConcernId"))
                concernId = Convert.ToInt32(TempData["payConcernId"]);
            if (TempData.ContainsKey("UserConcern"))
                concernId = Convert.ToInt32(TempData["UserConcern"]);
            if (concernId > 0)
                TempData["payConcernId"] = concernId;

            if (concernId == 0)
                return Json(new { msg = "No concern found for payment", JsonRequestBehavior.AllowGet });

            decimal totalAmount = 0m;
            int totalMonth = 0;
            //TempData["IsMultiPay"] = true;
            if (allConcernIds.Any())
            {
                List<string> concernNames = new List<string>();
                TempData.Remove("IsMultiPay");
                foreach (var concern in allConcernIds)
                {
                    string concernName = _sisterConcernService.GetConcernNameById(concern);
                    if (!string.IsNullOrEmpty(concernName))
                        concernNames.Add(concernName);
                    int serviceChargeId = _serviceChargeService.GetServiceChargeIdByConcernAndYear(concern, currentDate.Year);

                    if (serviceChargeId > 0)
                    {
                        Tuple<decimal, DateTime, DateTime, int> dueData = _serviceChargeDetailsService.GetDueServiceAmountByServicId(serviceChargeId, currentDate);
                        if (dueData.Item1 > 0)
                            totalAmount += dueData.Item1;
                        totalMonth += dueData.Item4;
                    }
                }
                TempData["TotalChargeDue"] = totalAmount;
                string allNames = string.Join(", ", concernNames);
                return Json(new { msg = "ok", amount = totalAmount, totalConcern = allConcernIds.Length, noOfMonth = totalMonth, concernNames = allNames }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "No concern found for payment", JsonRequestBehavior.AllowGet });
        }

    }
}