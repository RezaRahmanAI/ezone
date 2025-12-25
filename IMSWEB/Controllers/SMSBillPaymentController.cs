using AutoMapper;
using IMSWEB.Helper;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Model.TO.Bkash;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace IMSWEB.Controllers
{
    public class SMSBillPaymentController : CoreController
    {
        IMiscellaneousService<SMSBillPayment> _miscellaneousService;
        IMapper _mapper;
        ISMSBillPaymentService _SMSBillPaymentService;
        ISystemInformationService _sysInfoService;
        ISystemInformationService _systemInformationService;
        ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        ISMSBillPaymentBkashDetailService _smsBillPaymentBkashDetailService;
        ISisterConcernService _sisterConcernService;

        public SMSBillPaymentController(IErrorService errorService,
            ISMSBillPaymentService SMSBillPaymentService, IMiscellaneousService<SMSBillPayment> miscellaneousService, IMapper mapper, ISystemInformationService sysInfoService,ISystemInformationService systemInformationService, ISMSBillPaymentBkashService sMSBillPaymentBkashService, ISMSBillPaymentBkashDetailService smsBillPaymentBkashDetailService, ISisterConcernService sisterConcernService)
           : base(errorService, sysInfoService)
        {
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SMSBillPaymentService = SMSBillPaymentService;
            _systemInformationService = systemInformationService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
            _smsBillPaymentBkashDetailService = smsBillPaymentBkashDetailService;
            _sisterConcernService = sisterConcernService;        
        }


        #region ManualCreate Sms Bill Start Here 
        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var SMSBillPaymentsAsync = _SMSBillPaymentService.GetAll();
            var vmodel = _mapper.Map<IEnumerable<SMSBillPayment>, IEnumerable<SMSBillPaymentViewModel>>(SMSBillPaymentsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            int ConcernID = User.Identity.GetConcernId();
            var SmsBalance = _smsBillPaymentBkashService.GetByConcernId(ConcernID);
            var RechargeBalance = _smsBillPaymentBkashDetailService.GetLastPayAmount(SmsBalance.SMSPaymentMasterID);
            if (RechargeBalance.Count() != 0)
            {
                var LastRechargeBalance = RechargeBalance.Last();
                ViewBag.ReceiptNo = LastRechargeBalance.ReceiptNo;          
                ViewBag.lastRechargeAmt = LastRechargeBalance.RecAmount;
            }
            ViewBag.TotalSMSBalance = SmsBalance.TotalRecAmt;
            ViewBag.IsMasking = SmsBalance.IsMasking;

            return View(new SMSBillPaymentViewModel());
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(SMSBillPaymentViewModel newSMSBillPayment, string returnUrl)
        {
            CheckAndAddError(newSMSBillPayment);

            if (!ModelState.IsValid)
                return View(newSMSBillPayment);

            if (newSMSBillPayment != null)
            {
                var existingSMSBillPayment = _miscellaneousService.GetDuplicateEntry(c => c.ReceiptNo == newSMSBillPayment.ReceiptNo);
                if (existingSMSBillPayment != null)
                {
                    AddToastMessage("", "A SMS Bill Payment with same ReceiptNo already exists in the system.", ToastType.Error);
                    return View(newSMSBillPayment);
                }

                var model = _mapper.Map<SMSBillPaymentViewModel, SMSBillPayment>(newSMSBillPayment);
                AddAuditTrail(model, true);
                _SMSBillPaymentService.Add(model);
                _SMSBillPaymentService.Save();

                AddToastMessage("", "SMSBillPayment has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No SMSBillPayment data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }

        }

        private void CheckAndAddError(SMSBillPaymentViewModel newSMSBillPayment)
        {
            if (_SMSBillPaymentService.GetAll().Any(i => (newSMSBillPayment.PaidToDate >= i.PaidFromDate && newSMSBillPayment.PaidToDate <= i.PaidToDate)))
            {
                ModelState.AddModelError("PaidToDate", "This month bill is already paid.");
            }
            if (_SMSBillPaymentService.GetAll().Any(i => (newSMSBillPayment.PaidFromDate >= i.PaidFromDate && newSMSBillPayment.PaidFromDate <= i.PaidToDate)))
            {
                ModelState.AddModelError("PaidFromDate", "This month bill is already paid.");
            }
            if (newSMSBillPayment.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount is required.");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var model = _SMSBillPaymentService.GetById(id);
            var vmodel = _mapper.Map<SMSBillPayment, SMSBillPaymentViewModel>(model);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(SMSBillPaymentViewModel newSMSBillPayment, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSMSBillPayment);

            if (newSMSBillPayment != null)
            {
                var model = _SMSBillPaymentService.GetById((int)newSMSBillPayment.BillPayID);

                model.ReceiptNo = newSMSBillPayment.ReceiptNo;
                model.Amount = newSMSBillPayment.Amount;
                model.PaidFromDate = newSMSBillPayment.PaidFromDate;
                model.PaidToDate = newSMSBillPayment.PaidToDate;
                model.Remarks = newSMSBillPayment.Remarks;
                AddAuditTrail(model, false);

                _SMSBillPaymentService.Update(model);
                _SMSBillPaymentService.Save();

                AddToastMessage("", "SMSBillPayment has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No SMSBillPayment data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _SMSBillPaymentService.Delete(id);
            _SMSBillPaymentService.Save();
            AddToastMessage("", "SMSBillPayment has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        #endregion

        #region Bkash BillPayment  
        [HttpGet]
        public ActionResult GetBkashTokenOrConfirmPayment(string paymentId, string amount)
        {
            string concernName = "";
            if (string.IsNullOrEmpty(paymentId))
            {
                string idToken = PaymentHelper.GetGrantToken();
                TempData["bKashToken"] = idToken;
                var inv = new Random();
                //string invoice = _miscellaneousService.GetUniqueKey(d => d.BillPayID);
                //invoice = "OCT-" + invoice;
                //string totalChargeAmount = "0";
                string totalChargeAmount = amount;
                //string concernId = User.Identity.GetUserId();
                int ConcernID = User.Identity.GetConcernId();
                string concernId = ConcernID.ToString();
                //concernName = _sisterConcernService.GetConcernNameById(Convert.ToInt32(concernId));
                concernName = CoreController.RemoveSpecialCharacters(_sisterConcernService.GetConcernNameById(Convert.ToInt32(concernId)));
                CreatePaymentResponseTO createResponse = PaymentHelper.CreatePaymentFromToken(totalChargeAmount, "sale", "BDT","SMS-" + concernName, idToken, concernId);
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
                        #region udpate payment info                  
                        int concernId = User.Identity.GetConcernId();
                        int paymentMasterId;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                        decimal sysLastPayUpdateDate = smsAmountDetails.TotalRecAmt;                        
                        smsAmountDetails.TotalRecAmt = sysLastPayUpdateDate + Convert.ToDecimal(response.amount);
                            _smsBillPaymentBkashService.Update(smsAmountDetails);
                            _smsBillPaymentBkashService.Save();
                        #endregion

                        #region save payment data                  
                        string userId = User.Identity.GetUserId();                      
                        DateTime trDate;
                        if (!DateTime.TryParseExact(response.paymentExecuteTime, "yyyy-MM-dd'T'HH:mm:ss:fff 'GMT'zzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out trDate))
                        {
                            trDate = DateTime.Now;
                        }
                        SisterConcern concernDetails = _sisterConcernService.GetSisterConcernById(concernId);
                        SMSPaymentMasterDetails SMSPaymentMasterDetail = new SMSPaymentMasterDetails();
                        SMSPaymentMasterDetail.SMSPaymentMasterID = paymentMasterId;
                        SMSPaymentMasterDetail.ReceiptNo = response.merchantInvoiceNumber;
                        SMSPaymentMasterDetail.RecAmount = decimal.Parse(response.amount);
                        SMSPaymentMasterDetail.PaymentMobNo = response.customerMsisdn;
                        SMSPaymentMasterDetail.TransactionId = response.trxID;
                        SMSPaymentMasterDetail.TransactionStatus = response.transactionStatus;
                        //SMSPaymentMasterDetail.StatusCode = response.statusCode;                    
                        SMSPaymentMasterDetail.StatusMessage = response.statusMessage;
                        SMSPaymentMasterDetail.ErrorCocde = response.errorCode;
                        SMSPaymentMasterDetail.ErrorMessage = response.errorMessage;
                        SMSPaymentMasterDetail.RecDate = trDate;
                        SMSPaymentMasterDetail.PaymentReference = response.payerReference;
                        SMSPaymentMasterDetail.PaymentId = response.paymentID;
                        SMSPaymentMasterDetail.CreateDate = DateTime.Now;
                        SMSPaymentMasterDetail.CreatedBy = Convert.ToInt32(userId);
                        SMSPaymentMasterDetail.ConcernName = concernName;

                        _smsBillPaymentBkashDetailService.Add(SMSPaymentMasterDetail);
                        _smsBillPaymentBkashDetailService.Save();
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
                        TempData.Remove("TotalChargeDue");              
                        TempData.Remove("bkashToken");                  
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
        #endregion
    }
}