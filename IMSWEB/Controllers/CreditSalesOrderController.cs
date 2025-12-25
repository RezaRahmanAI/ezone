using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.SqlTypes;
using log4net;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("credit-sales-order")]
    public class CreditSalesOrderController : CoreController
    {
        ICreditSalesOrderService _creditSalesOrderService;
        IPurchaseOrderDetailService _purchaseOrderDetailService;
        IPOProductDetailService _pOProductDetailService;
        IStockService _stockService;
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        IStockDetailService _stockDetailService;
        IProductService _productService;
        IMiscellaneousService<CreditSale> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _SysInfoService;
        ISMSStatusService _SMSStatusService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ICardTypeSetupService _CardTypeSetupService;
        ICardTypeService _CardTypeService;
        IBankService _BankService;
        IBankTransactionService _BankTransactionService;
        ISisterConcernService _SisterConcernService;
        IUserService _UserService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IUserAuditDetailService _userAuditDetailService;
        private readonly IRoleService _roleService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        ISRVisitService _SRVisitService;
        public CreditSalesOrderController(IErrorService errorService,
            ICreditSalesOrderService salesOrderService, IPurchaseOrderDetailService purchaseOrderDetailService,
            IPOProductDetailService pOProductDetailService, IStockService stockService,
            IStockDetailService stockDetailService, ICustomerService customerService, IEmployeeService employeeService,
            IMiscellaneousService<CreditSale> miscellaneousService, IProductService productService,
            IMapper mapper, ISystemInformationService SysInfoService,
            ISMSStatusService SMSStatusService, ICardTypeSetupService CardTypeSetupService, ICardTypeService CardTypeService, IBankService BankService,
            IBankTransactionService BankTransactionService, IUserService UserService,
            ISisterConcernService SisterConcernService, ISessionMasterService sessionMasterService, IRoleService roleService, IUserAuditDetailService userAuditDetailService, ISMSBillPaymentBkashService sMSBillPaymentBkashService, ISRVisitService SRVisitService)
              : base(errorService, SysInfoService)
        {
            _creditSalesOrderService = salesOrderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _pOProductDetailService = pOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _customerService = customerService;
            _employeeService = employeeService;
            _miscellaneousService = miscellaneousService;
            _productService = productService;
            _mapper = mapper;
            _SysInfoService = SysInfoService;
            _SMSStatusService = SMSStatusService;
            _CardTypeSetupService = CardTypeSetupService;
            _CardTypeService = CardTypeService;
            _BankTransactionService = BankTransactionService;
            _BankService = BankService;
            _SisterConcernService = SisterConcernService;
            _UserService = UserService;
            _userAuditDetailService = userAuditDetailService;
            _sessionMasterService = sessionMasterService;
            _roleService = roleService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
            _SRVisitService = SRVisitService;

        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 50)
        {
            TempData["creditSalesOrderViewModel"] = null;
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            NormalizePaging(ref page, ref pageSize);
            var customSO = _creditSalesOrderService.GetAllSalesOrderAsync(ViewBag.FromDate, ViewBag.ToDate, IsVATManager(), User.Identity.GetConcernId(),
                page: page, pageSize: pageSize);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection, int page = 1, int pageSize = 50)
        {
            TempData["creditSalesOrderViewModel"] = null;
            string InvoiceNo = string.Empty, ContactNo = "", CustomerName = "", AccountNo = "";
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            if (!string.IsNullOrEmpty(formCollection["InvoiceNo"]))
                InvoiceNo = formCollection["InvoiceNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["ContactNo"]))
                ContactNo = formCollection["ContactNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["CustomerName"]))
                CustomerName = formCollection["CustomerName"].Trim();
            if (!string.IsNullOrEmpty(formCollection["AccountNo"]))
                AccountNo = formCollection["AccountNo"].Trim();

            NormalizePaging(ref page, ref pageSize);
            var customSO = _creditSalesOrderService.GetAllSalesOrderAsync(ViewBag.FromDate, ViewBag.ToDate,
                IsVATManager(), User.Identity.GetConcernId(), InvoiceNo, ContactNo, CustomerName, AccountNo, page, pageSize);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsDateWiseProductPicker = _SysInfoService.IsDateWiseProductPicker();
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder != null)
            {
                return View(salesOrder);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                return View(new CreditSalesOrderViewModel
                {
                    SODetail = new CreateCreditSalesOrderDetailViewModel(),
                    SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                    SOSchedules = new List<CreateCreditSalesSchedules>(),
                    SalesOrder = new CreateCreditSalesOrderViewModel { InvoiceNo = invNo }
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreditSalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            ViewBag.IsDateWiseProductPicker = _SysInfoService.IsDateWiseProductPicker();
            if (newSalesOrder != null)
            {
                CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
                salesOrder = salesOrder ?? newSalesOrder;
                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSalesOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }

                    if (salesOrder.SODetails != null &&
                     salesOrder.SODetails.Any(x => x.IMENo.Equals(newSalesOrder.SODetail.IMENo)
                         && x.ProductId.Equals(newSalesOrder.SODetail.ProductId)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View(salesOrder);
                    }


                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.ProductId.Equals(newSalesOrder.SODetail.ProductId)))
                    {

                        if (salesOrder.SODetails != null &&
                       salesOrder.SODetails.Any(x => x.StockDetailId.Equals(newSalesOrder.SODetail.StockDetailId)))
                        {
                            AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                            return View(salesOrder);
                        }
                    }

                    AddToOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);

                    if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
                    {
                        ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
                        salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
                    }
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }


                    SaveOrder(newSalesOrder, salesOrder, formCollection);
                    TempData["creditSalesOrderViewModel"] = null;
                    TempData["IsCInvoiceReady"] = true;
                    ModelState.Clear();

                    //mapping for credit sales ivoice
                    var invoiceSalesOrder = _mapper.Map<CreateCreditSalesOrderViewModel, CreditSale>(salesOrder.SalesOrder);
                    invoiceSalesOrder.CreditSaleDetails = _mapper.Map<ICollection<CreateCreditSalesOrderDetailViewModel>,
                        ICollection<CreditSaleDetails>>(salesOrder.SODetails);
                    invoiceSalesOrder.CreditSalesSchedules = _mapper.Map<ICollection<CreateCreditSalesSchedules>,
                       ICollection<CreditSalesSchedule>>(salesOrder.SOSchedules);
                    TempData["CreditSalesInvoiceData"] = invoiceSalesOrder;

                    #region Sales SMS
                    //var ProductNameList = (from sod in invoiceSalesOrder.CreditSaleDetails
                    //                       join p in _productService.GetAllIQueryable() on sod.ProductID equals p.ProductID
                    //                       select new { p.ProductName }).ToList();
                    var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                    if (SystemInfo.IsHireSMSEnable == 1 && salesOrder.SalesOrder.IsSmsEnable == true)
                    {
                        if (SystemInfo.IsBanglaSmsEnable == 1)
                        {
                            var oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = oCustomer.ContactNo,
                                CustomerID = oCustomer.CustomerID,
                                TransNumber = invoiceSalesOrder.InvoiceNo,
                                Date = (DateTime)invoiceSalesOrder.SalesDate,
                                PreviousDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue - invoiceSalesOrder.Remaining,
                                ReceiveAmount = (decimal)invoiceSalesOrder.DownPayment,
                                PresentDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue,
                                SMSType = EnumSMSType.SalesTime,
                                SalesAmount = invoiceSalesOrder.NetAmount,
                                CustomerCode = oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = oCustomer.CustomerID,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.SalesDate,
                                    PreviousDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue - invoiceSalesOrder.Remaining,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.DownPayment,
                                    PresentDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.NetAmount,
                                    CustomerCode = oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            int paymentMasterId;
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                            previousBalance = smsAmountDetails.TotalRecAmt;    
                            
                                var response = await Task.Run(() => SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response != null || response.Count > 0)
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;                                   
                                }
                                #region udpate payment info                 
                                decimal sysLastPayUpdateDate = smsBalanceCount * .45m;
                                smsAmountDetails.TotalRecAmt = previousBalance - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion

                                response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(response);
                                _SMSStatusService.Save();
                            }
                        }
                        else
                        {
                            var oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = oCustomer.ContactNo,
                                CustomerID = oCustomer.CustomerID,
                                CustomerName = oCustomer.Name,
                                TransNumber = invoiceSalesOrder.InvoiceNo,
                                Date = (DateTime)invoiceSalesOrder.SalesDate,
                                PreviousDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue - invoiceSalesOrder.Remaining,
                                ReceiveAmount = (decimal)invoiceSalesOrder.DownPayment,
                                PresentDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue,
                                SMSType = EnumSMSType.SalesTime,
                                SalesAmount = invoiceSalesOrder.NetAmount,
                                CustomerCode = oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = oCustomer.CustomerID,
                                    CustomerName = oCustomer.Name,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.SalesDate,
                                    PreviousDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue - invoiceSalesOrder.Remaining,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.DownPayment,
                                    PresentDue = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue) + oCustomer.CreditDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.NetAmount,
                                    CustomerCode = oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            int paymentMasterId;
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;
                            if (smsAmountDetails.TotalRecAmt > 1)
                            {
                                var response = await Task.Run(() => SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                                if (response != null || response.Count > 0)
                                {
                                    decimal smsBalanceCount = 0m;
                                    foreach (var item in response)
                                    {
                                        smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                       
                                    }
                                    #region udpate payment info                  

                                    decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                    smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                    _smsBillPaymentBkashService.Update(smsAmountDetails);
                                    _smsBillPaymentBkashService.Save();
                                    #endregion

                                    response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                    _SMSStatusService.AddRange(response);
                                    _SMSStatusService.Save();
                                }
                            }
                            else
                            {
                                AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                            }
                          
                        }


                    }

                    #endregion

                    return RedirectToAction("Create");
                    //return RedirectToAction("Index");
                }
                else if (formCollection.Get("installmentButton") != null)
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentNo) ||
                        decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentNo)) <= 0)
                        ModelState.AddModelError("SalesOrder.InstallmentNo", "Installments is required");

                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }
                    CalculateInstallments(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else if (formCollection.Get("paymentButton") != null)
                {
                    //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentAmount) ||
                    //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentAmount)) <= 0)
                    //    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment is required");
                    //if (!ModelState.IsValid)
                    //    return View(salesOrder);
                    AddPaymentModelError(salesOrder, newSalesOrder, formCollection);


                    if (!ModelState.IsValid)
                        return View("Create", salesOrder);

                    InstallmentPayment(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else
                {
                    return View(new CreditSalesOrderViewModel
                    {
                        SODetail = new CreateCreditSalesOrderDetailViewModel(),
                        SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                        SOSchedules = new List<CreateCreditSalesSchedules>(),
                        SalesOrder = new CreateCreditSalesOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddPaymentModelError(CreditSalesOrderViewModel salesOrder, CreditSalesOrderViewModel NewsalesOrder, FormCollection formCollection)
        {
            if (!IsDateValid(Convert.ToDateTime(formCollection["Paydate"])))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            }

            if (string.IsNullOrEmpty(NewsalesOrder.SalesOrder.InstallmentAmount))
                NewsalesOrder.SalesOrder.InstallmentAmount = "0";

            if (NewsalesOrder.SalesOrder.IsAllPaid == false)
            {

                string scheduleDate = formCollection["scheduleDate"];
                CreateCreditSalesSchedules creditSalesSchedule = salesOrder.SOSchedules.FirstOrDefault(x => DateTime.Parse(x.ScheduleDate) ==
                    DateTime.Parse(scheduleDate) && x.PaymentStatus.Equals("Due"));
                if (salesOrder.SOSchedules.Any(i => i.PaymentStatus.Equals("Due") && int.Parse(i.ScheduleNo) < int.Parse(creditSalesSchedule.ScheduleNo)))
                {
                    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Please pay Previous Installment First.");
                    AddToastMessage("", "Please pay Previous Installment First.", ToastType.Error);
                }
            }
            if (decimal.Parse(NewsalesOrder.SalesOrder.InstallmentAmount) > decimal.Parse(salesOrder.SalesOrder.PaymentDue))
            {
                ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment amount can't be more than remaining amount.");
                AddToastMessage("", "Installment amount can't be more than remaining amount.", ToastType.Error);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("edit")]
        public ActionResult Edit(int orderId)
        {
            //credit sales sp for calculating penalty schedules
            //_salesOrderService.CalculatePenaltySchedules(ConcernID);
            ViewBag.IsDateWiseProductPicker = _SysInfoService.IsDateWiseProductPicker();
            var salesOrder = _creditSalesOrderService.GetSalesOrderById(orderId);
            var saleOrderDetails = _creditSalesOrderService.GetCustomSalesOrderDetails(orderId);
            var saleOderSchedules = _creditSalesOrderService.GetSalesOrderSchedules(orderId);

            var vm = new CreditSalesOrderViewModel();
            vm.SalesOrder = _mapper.Map<CreditSale, CreateCreditSalesOrderViewModel>(salesOrder);
            vm.SODetails = _mapper.Map<ICollection<Tuple<int, int, int, int, decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string, decimal>>>, ICollection<CreateCreditSalesOrderDetailViewModel>>(saleOrderDetails.ToList());
            vm.SOSchedules = _mapper.Map<ICollection<CreditSalesSchedule>, ICollection<CreateCreditSalesSchedules>>(saleOderSchedules.ToList());

            TempData["creditSalesOrderViewModel"] = vm;
            ViewBag.Status = "data-attr='calculated'";

            if (TempData.ContainsKey("IsAgreement"))
                ViewBag.AgreementStatus = TempData["IsAgreement"];

            if (salesOrder.CardPaidAmount > 0m)
            {
                var obankTrans = _BankTransactionService.GetBankTransactionById(salesOrder.BankTransID);
                var oBank = _BankService.GetBankById(obankTrans.BankID);
                vm.SalesOrder.BankID = obankTrans.BankID;
                vm.SalesOrder.CardTypeID = _CardTypeSetupService.GetById(salesOrder.CardTypeSetupID).CardTypeID;
                vm.SalesOrder.CardPaidAmount = 0;
            }

            return View("Create", vm);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Agreement(int orderId)
        {
            TempData["IsAgreement"] = true;
            return RedirectToAction("Edit", new { orderId = orderId });
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreditSalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            ViewBag.IsDateWiseProductPicker = _SysInfoService.IsDateWiseProductPicker();
            if (newSalesOrder != null)
            {
                CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
                if (formCollection.Get("paymentButton") != null)
                {
                    //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentAmount) ||
                    //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentAmount)) <= 0)
                    //    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment is required");

                    AddPaymentModelError(salesOrder, newSalesOrder, formCollection);

                    if (!ModelState.IsValid)
                        return View("Create", salesOrder);

                    InstallmentPayment(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    //return View("Create", salesOrder);
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else if (formCollection.Get("updateBtn") != null) // To Increase Installment
                {
                    var AllCreditSalesSchedule = _creditSalesOrderService.GetSalesOrderSchedules(int.Parse(salesOrder.SalesOrder.SalesOrderId));
                    var DueCreditSalesSchedule = AllCreditSalesSchedule.Where(i => i.PaymentStatus == "Due");
                    var PaidCreditSalesSchedule = AllCreditSalesSchedule.Where(i => i.PaymentStatus == "Paid");
                    int ScheduleNo = 0;
                    ScheduleNo = PaidCreditSalesSchedule.Count();

                    log.Info(new { InstallmentIncrease = newSalesOrder.SalesOrder.InstallmentNo, PreviousInstallment = AllCreditSalesSchedule.Count(), DueInstllemntNo = DueCreditSalesSchedule.Count(), PaidInstallment = PaidCreditSalesSchedule.Count() });

                    foreach (var item in DueCreditSalesSchedule)
                    {
                        _creditSalesOrderService.DeleteSchedule(item);
                    }
                    //newSalesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    var CreditSale = _creditSalesOrderService.GetSalesOrderById(int.Parse(salesOrder.SalesOrder.SalesOrderId));
                    newSalesOrder.SalesOrder.PaymentDue = (CreditSale.Remaining + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    newSalesOrder.SalesOrder.InterestAmount = (CreditSale.InterestAmount + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    CalculateInstallments(newSalesOrder, salesOrder, formCollection);
                    var Customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
                    if (Customer != null)
                    {
                        Customer.CreditDue = Customer.CreditDue - CreditSale.Remaining;
                        Customer.CreditDue += decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue));
                    }
                    if (CreditSale != null)
                    {
                        CreditSale.Remaining = Convert.ToDecimal(newSalesOrder.SalesOrder.PaymentDue);
                        CreditSale.InterestAmount = Convert.ToDecimal(newSalesOrder.SalesOrder.InterestAmount);
                        CreditSale.NoOfInstallment = PaidCreditSalesSchedule.Count() + int.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentNo));
                        CreditSale.ModifiedBy = User.Identity.GetUserId<int>();
                        CreditSale.ModifiedDate = GetLocalDateTime();
                        DateTime currentDate = GetLocalDateTime();
                        decimal prevInterstTotal = _creditSalesOrderService.GetTotalPrevInterest(CreditSale.CreditSalesID);
                        CreditInterestHistory interestHistory = new CreditInterestHistory
                        {
                            HireSaleId = CreditSale.CreditSalesID,
                            InterestAmount = CreditSale.InterestAmount - prevInterstTotal,
                            InterestDate = currentDate,
                            IsFirstTime = false
                        };
                        _creditSalesOrderService.AddInterestHistory(interestHistory);
                    }
                    var Schedules = _mapper.Map<IEnumerable<CreateCreditSalesSchedules>, IEnumerable<CreditSalesSchedule>>(salesOrder.SOSchedules);
                    //foreach (var item in Schedules.ToList())
                    //{
                    //    ScheduleNo++;
                    //    item.PaymentDate = DateTime.Now;
                    //    item.ScheduleNo = ScheduleNo;
                    //    _creditSalesOrderService.AddSchedule(item);
                    //}

                    // Determine the starting date for the new installments
                    DateTime lastPaidDate = PaidCreditSalesSchedule.Any()
                        ? PaidCreditSalesSchedule.Max(x => x.MonthDate) : DateTime.Now;

                    DateTime nextInstallmentDate = lastPaidDate.AddMonths(1);

                    foreach (var item in Schedules.ToList())
                    {
                        ScheduleNo++;
                        item.PaymentDate = DateTime.Now;
                        item.MonthDate = nextInstallmentDate;// Set the next installment date
                        nextInstallmentDate = nextInstallmentDate.AddMonths(1);
                        item.ScheduleNo = ScheduleNo;
                        _creditSalesOrderService.AddSchedule(item);
                    }
                    _creditSalesOrderService.SaveSalesOrder();
                    _customerService.SaveCustomer();
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else if (formCollection.Get("btnremaindDateSetup") != null) // To set remainder date when customer wants to pay installment
                {
                    int CSScheduleID = int.Parse(formCollection["CSScheduleID"]);
                    string scheduleDate = formCollection["Paydate"];
                    var Schedule = _creditSalesOrderService.GetSalesOrderSchedules(int.Parse(salesOrder.SalesOrder.SalesOrderId)).FirstOrDefault(i => i.CSScheduleID == CSScheduleID);
                    Schedule.RemindDate = Convert.ToDateTime(scheduleDate);
                    _creditSalesOrderService.UpdateSchedule(Schedule);
                    _creditSalesOrderService.SaveSalesOrder();
                    AddToastMessage("", "Update Successfull.", ToastType.Success);
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else if (formCollection.Get("installmentButton") != null) // False Invoice purpose
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentNo) ||
                        decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentNo)) <= 0)
                        ModelState.AddModelError("SalesOrder.InstallmentNo", "Installments is required");

                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }
                    CalculateInstallments(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    TempData["creditSalesOrderViewModel"] = null;
                    TempData["IsCInvoiceReady"] = true;
                    //mapping for credit sales ivoice
                    salesOrder.SalesOrder.Status = "0";
                    var invoiceSalesOrder = _mapper.Map<CreateCreditSalesOrderViewModel, CreditSale>(salesOrder.SalesOrder);
                    invoiceSalesOrder.CreditSaleDetails = _mapper.Map<ICollection<CreateCreditSalesOrderDetailViewModel>, ICollection<CreditSaleDetails>>(salesOrder.SODetails);
                    invoiceSalesOrder.CreditSalesSchedules = _mapper.Map<ICollection<CreateCreditSalesSchedules>, ICollection<CreditSalesSchedule>>(salesOrder.SOSchedules);

                    TempData["CreditSalesInvoiceData"] = invoiceSalesOrder;
                    return View("Create", salesOrder);
                }
                else
                {
                    return View(new CreditSalesOrderViewModel
                    {
                        SODetail = new CreateCreditSalesOrderDetailViewModel(),
                        SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                        SOSchedules = new List<CreateCreditSalesSchedules>(),
                        SalesOrder = new CreateCreditSalesOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{orderId}")]
        public ActionResult Delete(int orderId)
        {
            if (HasPaidInstallment(orderId))
            {
                AddToastMessage("", "This order can't be deleted. One or More Installment(s) has already been paid for this order.",
                    ToastType.Error);
                return RedirectToAction("Index");
            }
            var CreditSale = _creditSalesOrderService.GetSalesOrderById(orderId);
            if (!IsDateValid(CreditSale.SalesDate))
            {
                return RedirectToAction("Index");
            }
            if (_creditSalesOrderService.ReturnSalesOrderUsingSP(orderId, User.Identity.GetUserId<int>()))
                AddToastMessage("", "Item has been returned successfully", ToastType.Success);
            else
                AddToastMessage("", "Item return failed.", ToastType.Error);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("editfromview/{id}/{detailId}")]
        public ActionResult EditFromView(int id, int detailId)
        {
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                return RedirectToAction("Create");
            }

            CreateCreditSalesOrderDetailViewModel itemToEdit =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToEdit != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt)))).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                salesOrder.SODetails.Remove(itemToEdit);

                salesOrder.SODetail = itemToEdit;
                TempData["creditSalesOrderViewModel"] = salesOrder;
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId)
        {
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                return RedirectToAction("Create");
            }

            CreateCreditSalesOrderDetailViewModel itemToDelete =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToDelete != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt)))).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)))).ToString();

                salesOrder.SODetails.Remove(itemToDelete);

                salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();
                TempData["creditSalesOrderViewModel"] = salesOrder;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                return RedirectToAction("Create");
            }
        }

        private void RefreshFinalObject(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            salesOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newSalesOrder.SalesOrder.NetDiscount);
            salesOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)).ToString();
            salesOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)).ToString();
            salesOrder.SalesOrder.InterestRate = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestRate)).ToString();
            salesOrder.SalesOrder.InterestAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestAmount)).ToString();
            salesOrder.SalesOrder.TotalInterest = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalInterest)).ToString();
            salesOrder.SalesOrder.PayAdjustment = newSalesOrder.SalesOrder.PayAdjustment;
            salesOrder.SalesOrder.TotalDiscountPercentage = newSalesOrder.SalesOrder.TotalDiscountPercentage;
            salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            salesOrder.SalesOrder.ProcessingFee = newSalesOrder.SalesOrder.ProcessingFee;
            salesOrder.SalesOrder.RecieveAmount = newSalesOrder.SalesOrder.RecieveAmount;
            salesOrder.SalesOrder.VATPercentage = newSalesOrder.SalesOrder.VATPercentage;
            salesOrder.SalesOrder.VATAmount = newSalesOrder.SalesOrder.VATAmount;
            salesOrder.SalesOrder.Remarks = newSalesOrder.SalesOrder.Remarks;
            salesOrder.SalesOrder.CreateDateTime = newSalesOrder.SalesOrder.CreateDateTime;
            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            salesOrder.SalesOrder.CardPaidAmount = newSalesOrder.SalesOrder.CardPaidAmount;
            salesOrder.SalesOrder.CashPaidAmount = newSalesOrder.SalesOrder.CashPaidAmount;
            salesOrder.SalesOrder.BankID = newSalesOrder.SalesOrder.BankID;
            salesOrder.SalesOrder.CardTypeID = newSalesOrder.SalesOrder.CardTypeID;
            salesOrder.SalesOrder.IsSmsEnable = Convert.ToBoolean(newSalesOrder.SalesOrder.IsSmsEnable ? 1 : 0);
            salesOrder.SalesOrder.IsWeekly = Convert.ToBoolean(newSalesOrder.SalesOrder.IsWeekly ? 1 : 0);
            salesOrder.SalesOrder.GuarName = newSalesOrder.SalesOrder.GuarName;
            salesOrder.SalesOrder.GuarContactNo = newSalesOrder.SalesOrder.GuarContactNo;
            salesOrder.SalesOrder.GuarAddress = newSalesOrder.SalesOrder.GuarAddress;

            if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                if (_miscellaneousService.GetDuplicateEntry(i => i.InvoiceNo == newSalesOrder.SalesOrder.InvoiceNo) != null)
                {
                    string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                    salesOrder.SalesOrder.InvoiceNo = invNo;
                }
            }
        }

        private void CreateSchedule(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection, int noOfInstallment = 0, int total = 0)
        {
            var remainingAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue));
            int installmentNo = noOfInstallment == 0 ? int.Parse(newSalesOrder.SalesOrder.InstallmentNo) : noOfInstallment;
            decimal nTotalBalance = remainingAmount;
            decimal nInstallmentAmt = Math.Round((remainingAmount / installmentNo), 2);

            decimal netRemainingAmount = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) - (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount)));
            decimal netInstallmentAmt = Math.Round((netRemainingAmount / decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo))));
            decimal hireValue = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalInterest)) / decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo));


            CreateCreditSalesSchedules schedule = null;
            if (salesOrder.SalesOrder.IsWeekly == false)
            {
                DateTime installmentMonth = total == 0 ? DateTime.Parse(formCollection["InstallmentDate"]).AddMonths(1) :
                                DateTime.Parse(formCollection["InstallmentDate"]).AddMonths(total + 1);

                for (int i = 0; i < installmentNo; i++)
                {
                    schedule = new CreateCreditSalesSchedules();
                    schedule.ScheduleDate = installmentMonth.ToString("dd MMM yyyy");
                    schedule.PaymentStatus = "Due";
                    schedule.InstallmentAmount = nInstallmentAmt.ToString();
                    schedule.PayDate = string.Empty;
                    schedule.OpeningBalance = Math.Round(nTotalBalance).ToString();
                    nTotalBalance -= nInstallmentAmt;
                    schedule.ClosingBalance = Math.Round(nTotalBalance).ToString();
                    schedule.IsUnExpected = noOfInstallment == 0 ? false : true;

                    schedule.HireValue = hireValue;
                    schedule.NetValue = netInstallmentAmt;
                    schedule.SalesOrderId = salesOrder.SalesOrder.SalesOrderId;
                    salesOrder.SOSchedules.Add(schedule);

                    installmentMonth = installmentMonth.AddMonths(1);
                }
            }
            else if (salesOrder.SalesOrder.IsWeekly == true)
            {
                DateTime installmentMonth = total == 0 ? DateTime.Parse(formCollection["InstallmentDate"]).AddDays(7) :
                                DateTime.Parse(formCollection["InstallmentDate"]).AddDays(total + 7);

                for (int i = 0; i < installmentNo; i++)
                {
                    schedule = new CreateCreditSalesSchedules();
                    schedule.ScheduleDate = installmentMonth.ToString("dd MMM yyyy");
                    schedule.PaymentStatus = "Due";
                    schedule.InstallmentAmount = nInstallmentAmt.ToString();
                    schedule.PayDate = string.Empty;
                    schedule.OpeningBalance = Math.Round(nTotalBalance).ToString();
                    nTotalBalance -= nInstallmentAmt;
                    schedule.ClosingBalance = Math.Round(nTotalBalance).ToString();
                    schedule.IsUnExpected = noOfInstallment == 0 ? false : true;

                    schedule.HireValue = hireValue;
                    schedule.NetValue = netInstallmentAmt;
                    schedule.SalesOrderId = salesOrder.SalesOrder.SalesOrderId;
                    salesOrder.SOSchedules.Add(schedule);

                    installmentMonth = installmentMonth.AddDays(7);
                }
            }


        }

        private void CheckAndAddModelErrorForAdd(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
                salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            //ProductDetailsId is ProductId
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
            {
                newSalesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
                salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            }
            if (string.IsNullOrEmpty(newSalesOrder.SODetail.Quantity) || int.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity)) <= 0)
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
            }

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.MRPRate))
                ModelState.AddModelError("SODetail.MRPRate", "Purchase Rate is required");


            if (string.IsNullOrEmpty(newSalesOrder.SODetail.UnitPrice))
                ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");
            else
            {
                if (Convert.ToDecimal(newSalesOrder.SODetail.UnitPrice) <= 0m)
                    ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");
            }

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.IMENo))
            {
                ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
            }
            else
            {
                var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));
                int SDetailID = int.Parse(GetDefaultIfNull(formCollection["StockDetailsId"]));
                //  int ColorID = int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"]));
                var stockDeatilCount = _stockDetailService.GetNewById(SDetailID);// _stockService.GET.GetStockByProductIdandColorIDandGodownID(product.ProductID, 1, 1);
                newSalesOrder.SODetail.GodownID = stockDeatilCount.GodownID;

                if (product.ProductType == (int)EnumProductType.NoBarcode)
                {

                    decimal remainingQty = _SRVisitService.GetRemainingQuantityForSRVisit(stockDeatilCount.ProductID, stockDeatilCount.ColorID, User.Identity.GetConcernId(), stockDeatilCount.GodownID, 0, stockDeatilCount.SDetailID);
                    if (remainingQty < Convert.ToDecimal(newSalesOrder.SODetail.Quantity))
                    {
                        string msg = $"Those Qty not found in stock for this product. Available Qty: {remainingQty} for selected product";
                        //return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
                        ModelState.AddModelError("SODetail.Quantity", msg);
                    }
                    //var stockCount = _stockService.GetStockByProductIdandColorIDandGodownID(stockDeatilCount.ProductID, stockDeatilCount.GodownID, stockDeatilCount.ColorID);

                    //if (stockCount.Quantity < int.Parse(newSalesOrder.SODetail.Quantity))
                    //    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockCount.Quantity);
                }
                else
                {
                    //var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                    //if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                    //    ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
                    var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                    if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                        ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

                    int SDetailId = Convert.ToInt32(formCollection["StockDetailsId"]);
                    var PendingIMEI = _creditSalesOrderService.IsIMEIInPendingSales(SDetailId, Convert.ToInt32(GetDefaultIfNull(salesOrder.SalesOrder.SalesOrderId)));
                    if (PendingIMEI.Item1)
                        ModelState.AddModelError("SODetail.IMENo", "This IMEI is in pending sales Invoice No:" + PendingIMEI.Item2);
                }

            }


        }

        private void AddNote(CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["selSalesRate"]))
            {
                List<int> SDetailIDList = salesOrder.SODetails.Select(i => int.Parse(i.StockDetailId)).ToList();
                var StockDetails = _stockDetailService.GetAll().Where(i => (SDetailIDList.Contains(i.SDetailID)));
                decimal RateFrom = 0, RateTo = 0;
                if (formCollection["selSalesRate"].Equals("1"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "3";
                    RateFrom = StockDetails.Sum(i => i.CRSalesRate3Month);
                    RateTo = StockDetails.Sum(i => i.CreditSRate);
                    //salesOrder.SalesOrder.Remarks = "If you can't pay within 3 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }
                else if (formCollection["selSalesRate"].Equals("2"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "6";
                    RateFrom = StockDetails.Sum(i => i.CreditSRate);
                    RateTo = StockDetails.Sum(i => i.CRSalesRate12Month);
                    //salesOrder.SalesOrder.Remarks = "If you can't pay within 6 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }
                else if (formCollection["selSalesRate"].Equals("3"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "12";
                    RateFrom = StockDetails.Sum(i => i.CRSalesRate12Month);
                    RateTo = RateFrom + (StockDetails.Sum(i => i.CreditSRate) - StockDetails.Sum(i => i.CRSalesRate3Month));
                    //salesOrder.SalesOrder.Remarks = "If you can't pay within 12 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }

            }
        }







        private void CheckAndAddModelErrorForSave(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("SalesOrder.GrandTotal", "Grand Total is required");

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.TotalAmount) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)) <= 0)
                ModelState.AddModelError("SalesOrder.TotalAmount", "Net Total is required");

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.RecieveAmount", "Down Payment is required");

            #region Customer and Employee Due Limit check
            Customer customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
            Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);
            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            if (sysInfo != null)
            {
                if (sysInfo.CustomerDueLimitApply == 1)
                {
                    if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
                        ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");
                }
                if (sysInfo.EmployeeDueLimitApply == 1)
                {
                    if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
                        ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");
                }
            }
            #endregion

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.VATAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.VATAmount", "Vat Amount is required");

            //if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
            //{
            //    ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            //    salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            //}

            if (newSalesOrder.SalesOrder.CardPaidAmount > 0)
            {
                if (string.IsNullOrEmpty(formCollection["BanksId"]))
                    ModelState.AddModelError("SalesOrder.BankID", "Bank is required.");
                else
                    newSalesOrder.SalesOrder.BankID = Convert.ToInt32(formCollection["BanksId"]);
            }

            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                newSalesOrder.SalesOrder.CreateDateTime = Convert.ToDateTime(formCollection["ClientDateTime"]);
            else
                newSalesOrder.SalesOrder.CreateDateTime = GetLocalDateTime();


            decimal NetTotal = Convert.ToDecimal(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount));
            decimal Downpayment = Convert.ToDecimal(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount));
            if (Downpayment > NetTotal)
                ModelState.AddModelError("SalesOrder.RecieveAmount", "Down payment can't be greater than net amount.");

        }

        private void AddToOrder(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            decimal quantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity));
            //decimal PPTotalInterest = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.IntTotalAmt));
            decimal PPTotalInterest = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.IntTotalAmt));

            decimal totalOffer = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) +
            decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.UTAmount)) + totalOffer).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            //Dis Per. Of Total Amount
            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PPDiscountAmount))).ToString();

            //Net Discount
            salesOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountAmount)).ToString();

            salesOrder.SalesOrder.ProcessingFee = newSalesOrder.SalesOrder.ProcessingFee;


            salesOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATPercentage)).ToString();
            salesOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)).ToString();

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + totalOffer).ToString();

            //+ decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount))

            // decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount))
            var netTotal = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount))) -
                decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)));



            salesOrder.SalesOrder.TotalAmount = netTotal.ToString();
            salesOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount))).ToString();

            salesOrder.SalesOrder.PayAdjustment = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)).ToString();

            //For Total Offer Purpose
            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer)) + totalOffer).ToString();

            ////add interest for per product
            //salesOrder.SalesOrder.InterestAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount))
            //    + totalInterest).ToString();

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            salesOrder.SODetail.StockDetailId = formCollection["StockDetailsId"];
            salesOrder.SODetail.ColorId = formCollection["ColorsId"];
            salesOrder.SODetail.GodownID = newSalesOrder.SODetail.GodownID;
            salesOrder.SODetail.ColorName = newSalesOrder.SODetail.ColorName;
            salesOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            salesOrder.SODetail.IMENo = newSalesOrder.SODetail.IMENo;
            salesOrder.SODetail.Quantity = newSalesOrder.SODetail.Quantity;
            salesOrder.SODetail.IntPercentage = newSalesOrder.SODetail.IntPercentage;
            //salesOrder.SODetail.IntTotalAmt = newSalesOrder.SODetail.IntTotalAmt;
            salesOrder.SODetail.IntTotalAmt = PPTotalInterest.ToString();
            salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
            salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
            salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
            salesOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            //salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
            salesOrder.SODetail.PPOffer = totalOffer.ToString();

            salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
            salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
            salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
            salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
            salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;


            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
            salesOrder.SODetails.Add(salesOrder.SODetail);

            //Total Interest Rate and Total Interest Per.
            #region Interest Calculations
            TotalInterestCalculation(newSalesOrder, salesOrder);
            #endregion

            salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
            AddNote(salesOrder, formCollection);
            salesOrder.SalesOrder.InstallmentNo = "0";
            CreditSalesOrderViewModel vm = new CreditSalesOrderViewModel
            {
                SODetail = new CreateCreditSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SOSchedules = salesOrder.SOSchedules,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["creditSalesOrderViewModel"] = vm;
            salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private void TotalInterestCalculation(CreditSalesOrderViewModel newSalesOrder, CreditSalesOrderViewModel salesOrder)
        {
            salesOrder.SalesOrder.InterestRate = newSalesOrder.SalesOrder.InterestRate;
            salesOrder.SalesOrder.InterestAmount = newSalesOrder.SalesOrder.InterestAmount;//Flat interest
            decimal TotalPPInterAmt = 0m;
            foreach (var item in salesOrder.SODetails)
            {
                TotalPPInterAmt += Convert.ToDecimal(item.IntTotalAmt);
            }
            salesOrder.SalesOrder.tempPPInterestTotal = TotalPPInterAmt.ToString();
            salesOrder.SalesOrder.TotalInterest = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount)) + TotalPPInterAmt).ToString();
        }

        private void CalculateInstallments(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);
            salesOrder.SalesOrder.InstallmentDate = formCollection["InstallmentDate"];
            salesOrder.SalesOrder.InstallmentNo = newSalesOrder.SalesOrder.InstallmentNo;

            salesOrder.SOSchedules = new List<CreateCreditSalesSchedules>();
            CreateSchedule(newSalesOrder, salesOrder, formCollection);

            TempData["creditSalesOrderViewModel"] = salesOrder;
            ViewBag.Status = "data-attr='calculated'";
            AddToastMessage("", "Schedule has been calculated successfully.", ToastType.Success);
        }

        private void InstallmentPayment(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);

            string scheduleDate = formCollection["scheduleDate"];
            CreateCreditSalesSchedules oPaidSchedule = null;
            if (newSalesOrder.SalesOrder.IsAllPaid)
                oPaidSchedule = salesOrder.SOSchedules.FirstOrDefault(x => x.PaymentStatus.Equals("Due"));
            else
                oPaidSchedule = salesOrder.SOSchedules.FirstOrDefault(x => DateTime.Parse(x.ScheduleDate) == DateTime.Parse(scheduleDate) && x.PaymentStatus.Equals("Due"));

            if (oPaidSchedule != null)
            {
                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(oPaidSchedule.OpeningBalance) -
                    ((decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)))).ToString();
                salesOrder.SalesOrder.WInterestAmt = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)).ToString();
                if (!string.IsNullOrEmpty(formCollection["Paydate"]))
                {
                    oPaidSchedule.PayDate = formCollection["Paydate"];
                    oPaidSchedule.PayDate = DateTime.Parse(oPaidSchedule.PayDate).ToString("dd MMM yyyy");
                }
                else
                    oPaidSchedule.PayDate = DateTime.Today.ToString("dd MMM yyyy");

                if (SysInfo.ApprovalSystemEnable == 0)
                    oPaidSchedule.PaymentStatus = "Paid";
                else
                    oPaidSchedule.PaymentStatus = "Pending";


                oPaidSchedule.Remarks = formCollection["Remarks" + scheduleDate];
                oPaidSchedule.ExpectedInstallment = Convert.ToDecimal(oPaidSchedule.InstallmentAmount);
                oPaidSchedule.CreatedBy = User.Identity.GetUserId<int>();

                #region Card Payment during installment Collections
                if (newSalesOrder.SalesOrder.CardPaidAmount > 0m && ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                {
                    int CardTypeSetupID = 0;
                    decimal DepositChargePercent = 0m;

                    _BankTransactionService.CardPaymentNetAmtCalculation(salesOrder.SalesOrder.BankID, salesOrder.SalesOrder.CardTypeID,
                        salesOrder.SalesOrder.CardPaidAmount, out CardTypeSetupID, out DepositChargePercent);

                    oPaidSchedule.CardTypeSetupID = CardTypeSetupID;
                    oPaidSchedule.CardPaidAmount = newSalesOrder.SalesOrder.CardPaidAmount;
                    oPaidSchedule.DepositChargePercent = DepositChargePercent;
                    newSalesOrder.SalesOrder.InstallmentAmount = (Convert.ToDecimal(newSalesOrder.SalesOrder.InstallmentAmount) + newSalesOrder.SalesOrder.CardPaidAmount).ToString();
                }
                #endregion

                if (decimal.Parse(oPaidSchedule.InstallmentAmount) !=
                    (decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount) +
                    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment))))
                {
                    oPaidSchedule.InstallmentAmount = decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount).ToString();
                    oPaidSchedule.CreatedBy = User.Identity.GetUserId<int>();
                    oPaidSchedule.ClosingBalance = salesOrder.SalesOrder.PaymentDue;
                    List<CreateCreditSalesSchedules> schedulesToDelete = salesOrder.SOSchedules.Where(x => x.PaymentStatus.Equals("Due")).ToList();
                    int paidSchedules = salesOrder.SOSchedules.Where(x => x.PaymentStatus.Equals("Paid")).Count();
                    int remainingNoOfIns = (salesOrder.SOSchedules.Count() - paidSchedules);

                    foreach (var item in schedulesToDelete)
                        salesOrder.SOSchedules.Remove(item);

                    newSalesOrder.SalesOrder.PaymentDue = salesOrder.SalesOrder.PaymentDue;
                    CreateSchedule(newSalesOrder, salesOrder, formCollection, remainingNoOfIns, paidSchedules);
                }
                else
                {
                    oPaidSchedule.InstallmentAmount = decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount).ToString();
                }
                bool IsEditInstallment = false;

                log.Info(new { salesOrder.SalesOrder.SalesOrderId, oPaidSchedule.InstallmentAmount, newSalesOrder.SalesOrder.PayAdjustment });

                if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                {
                    //int CardTypeSetupID = 0;
                    //decimal DepositChargePercent = 0m;

                    //decimal DepositAmt = _BankTransactionService.CardPaymentNetAmtCalculation(salesOrder.SalesOrder.BankID, salesOrder.SalesOrder.CardTypeID,
                    //      salesOrder.SalesOrder.CardPaidAmount, out CardTypeSetupID, out DepositChargePercent);

                    //oPaidSchedule.CardTypeSetupID = CardTypeSetupID;
                    //oPaidSchedule.DepositChargePercent = DepositChargePercent;
                    //salesOrder.SalesOrder.DepositChargePercent = DepositChargePercent;

                    DataTable dtSalesOrderSchedules = CreateSOSchedulesDataTable(salesOrder);
                    //DataTable dtBankTrans = _BankTransactionService.CreateBankTransDataTable(Convert.ToDateTime(oPaidSchedule.PayDate),salesOrder.SalesOrder.InvoiceNo, 
                    //                       EnumTransactionType.Deposit, DepositAmt, salesOrder.SalesOrder.BankID,
                    //                       User.Identity.GetConcernId(),"Card payment for installment collection");
                    DataTable dtBankTrans = CreateBankTransDataTable(salesOrder.SalesOrder.BankID, salesOrder.SalesOrder.CardTypeID, salesOrder.SalesOrder.CardPaidAmount,
                       Convert.ToDateTime(oPaidSchedule.PayDate), salesOrder.SalesOrder.InvoiceNo);

                    if (SysInfo.ApprovalSystemEnable == 0)
                    {
                        _creditSalesOrderService.InstallmentPaymentUsingSP(int.Parse(salesOrder.SalesOrder.SalesOrderId),
                        decimal.Parse(oPaidSchedule.InstallmentAmount), dtSalesOrderSchedules, decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)),
                        dtBankTrans, oPaidSchedule.CardTypeSetupID);
                    }
                    else
                    {
                        _creditSalesOrderService.PendingInstallmentPaymentUsingSP(int.Parse(salesOrder.SalesOrder.SalesOrderId),
                                       decimal.Parse(oPaidSchedule.InstallmentAmount), dtSalesOrderSchedules, decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)),
                                        oPaidSchedule.CardTypeSetupID);
                    }

                    IsEditInstallment = true;
                }

                TempData["creditSalesOrderViewModel"] = salesOrder;
                ViewBag.Status = "data-attr='calculated'";

                //TempData["IsMoneyReceiptReady"] = true;

                var creditsalesSchedules = new CreditSalesSchedule()
                {
                    PaymentDate = Convert.ToDateTime(oPaidSchedule.PayDate),
                    InstallmentAmt = Convert.ToDecimal(oPaidSchedule.InstallmentAmount),
                };

                TempData["IsMoneyReceiptReadyByID"] = true;
                TempData["OrderId"] = Convert.ToInt32(salesOrder.SalesOrder.SalesOrderId);

                #region SMS Service
                var CreditSales = _creditSalesOrderService.GetSalesOrderById(Convert.ToInt32(salesOrder.SalesOrder.SalesOrderId));
                List<SMSRequest> sms = new List<SMSRequest>();
                var oCustomer = _customerService.GetCustomerById(CreditSales.CustomerID);
                var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                if (SystemInfo.IsInstallmentSMSEnable == 1 && salesOrder.SalesOrder.IsSmsEnable == true)
                {
                    if (SystemInfo.IsBanglaSmsEnable == 1)
                    {
                        sms.Add(new SMSRequest()
                        {
                            SMSType = EnumSMSType.InstallmentCollection,
                            Date = (DateTime)creditsalesSchedules.PaymentDate,
                            TransNumber = CreditSales.InvoiceNo,
                            ReceiveAmount = (decimal)creditsalesSchedules.InstallmentAmt,
                            PreviousDue = oCustomer.CreditDue - (decimal)creditsalesSchedules.InstallmentAmt - CreditSales.LastPayAdjAmt,
                            TotalReceiveAmount = CreditSales.NetAmount - CreditSales.Remaining - CreditSales.LastPayAdjAmt,
                            PresentDue = oCustomer.CreditDue,
                            CustomerID = CreditSales.CustomerID,
                            MobileNo = oCustomer.ContactNo,
                            CustomerCode = oCustomer.Code
                        });
                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                SMSType = EnumSMSType.InstallmentCollection,
                                Date = (DateTime)creditsalesSchedules.PaymentDate,
                                TransNumber = CreditSales.InvoiceNo,
                                ReceiveAmount = (decimal)creditsalesSchedules.InstallmentAmt,
                                PreviousDue = oCustomer.CreditDue - (decimal)creditsalesSchedules.InstallmentAmt - CreditSales.LastPayAdjAmt,
                                TotalReceiveAmount = CreditSales.NetAmount - CreditSales.Remaining - CreditSales.LastPayAdjAmt,
                                PresentDue = oCustomer.CreditDue,
                                CustomerID = CreditSales.CustomerID,
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerCode = oCustomer.Code
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        int paymentMasterId;
                        decimal previousBalance = 0m;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfo.smsCharge;

                        var Response = SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());
                        if (Response.Count > 0)
                        {
                            decimal smsBalanceCount = 0m;
                            foreach (var item in Response)
                            {
                                smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                
                            }
                            #region udpate payment info                 
                            decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                            smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                            _smsBillPaymentBkashService.Update(smsAmountDetails);
                            _smsBillPaymentBkashService.Save();
                            #endregion

                            Response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                            _SMSStatusService.AddRange(Response);
                            _SMSStatusService.Save();
                        }
                    }

                    else
                    {
                        sms.Add(new SMSRequest()
                        {
                            SMSType = EnumSMSType.InstallmentCollection,
                            Date = (DateTime)creditsalesSchedules.PaymentDate,
                            TransNumber = CreditSales.InvoiceNo,
                            ReceiveAmount = (decimal)creditsalesSchedules.InstallmentAmt,
                            PreviousDue = oCustomer.CreditDue - (decimal)creditsalesSchedules.InstallmentAmt - CreditSales.LastPayAdjAmt,
                            TotalReceiveAmount = CreditSales.NetAmount - CreditSales.Remaining - CreditSales.LastPayAdjAmt,
                            PresentDue = oCustomer.CreditDue,
                            CustomerID = CreditSales.CustomerID,
                            CustomerName=oCustomer.Name,
                            MobileNo = oCustomer.ContactNo,
                            CustomerCode = oCustomer.Code
                        });
                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                SMSType = EnumSMSType.InstallmentCollection,
                                Date = (DateTime)creditsalesSchedules.PaymentDate,
                                TransNumber = CreditSales.InvoiceNo,
                                ReceiveAmount = (decimal)creditsalesSchedules.InstallmentAmt,
                                PreviousDue = oCustomer.CreditDue - (decimal)creditsalesSchedules.InstallmentAmt - CreditSales.LastPayAdjAmt,
                                TotalReceiveAmount = CreditSales.NetAmount - CreditSales.Remaining - CreditSales.LastPayAdjAmt,
                                PresentDue = oCustomer.CreditDue,
                                CustomerID = CreditSales.CustomerID,
                                CustomerName = oCustomer.Name,
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerCode = oCustomer.Code
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        decimal previousBalance = 0m;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);            
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfos.smsCharge;
                        if (smsAmountDetails.TotalRecAmt > 1)
                        {
                            var Response = SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());
                            if (Response.Count > 0)
                            {

                                decimal smsBalanceCount = 0m;
                                foreach (var item in Response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                }
                                #region udpate payment info                 

                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);

                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion
                                Response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(Response);
                                _SMSStatusService.Save();
                            }
                        }
                        else
                        {
                            AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                        }

                           
                    }
                }

                #endregion

                salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();



                AddToastMessage("", "Installment has been paid successfully.", ToastType.Success);
            }
            else
            {
                AddToastMessage("", "No schedule found to paid.", ToastType.Error);
                return;
            }
        }

        private bool SaveOrder(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            bool IsSuccess = false;
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);
            int CardTypeSetupID = 0;
            decimal DepositChargePercent = 0m;
            decimal DepositAmt = _BankTransactionService.CardPaymentNetAmtCalculation(salesOrder.SalesOrder.BankID, salesOrder.SalesOrder.CardTypeID,
                salesOrder.SalesOrder.CardPaidAmount, out CardTypeSetupID, out DepositChargePercent);

            salesOrder.SalesOrder.CardTypeSetupID = CardTypeSetupID.ToString();
            salesOrder.SalesOrder.DepositChargePercent = DepositChargePercent;

            DataTable dtBankTrans = _BankTransactionService.CreateBankTransDataTable(Convert.ToDateTime(salesOrder.SalesOrder.OrderDate)
                , salesOrder.SalesOrder.InvoiceNo,
                EnumTransactionType.Deposit, DepositAmt,
                salesOrder.SalesOrder.BankID,
                                           User.Identity.GetConcernId(),
                                           "Hire sales card payment"
                        );

            DataTable dtSalesOrder = CreateSalesOrderDataTable(salesOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(salesOrder);
            DataTable dtSalesOrderSchedules = CreateSOSchedulesDataTable(salesOrder);

            log.Info(new { salesOrder.SalesOrder, salesOrder.SODetails, salesOrder.SOSchedules });

            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

           
            Tuple<bool, int> dbResult = null;
            if (sysInfo.ApprovalSystemEnable == 1)
                dbResult = _creditSalesOrderService.AddPendingSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail,dtSalesOrderSchedules);
            else
                dbResult = _creditSalesOrderService.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, dtSalesOrderSchedules, dtBankTrans);

            if (dbResult.Item1)
            {
                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = dbResult.Item2;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.HireSales;
                useraudit.ActionType = EnumActionType.Add;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                IsSuccess = dbResult.Item1;
            }
                if (IsSuccess)
                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Order has been failed.", ToastType.Error);

            return IsSuccess;


        }

        private DataTable CreateSalesOrderDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrder = new DataTable();
            dtSalesOrder.Columns.Add("InvoiceNo", typeof(string));
            dtSalesOrder.Columns.Add("CustomerId", typeof(int));
            dtSalesOrder.Columns.Add("TSalesAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("NoOfInstallment", typeof(int));
            dtSalesOrder.Columns.Add("InstallmentPrinciple", typeof(decimal));
            dtSalesOrder.Columns.Add("IssueDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("UserName", typeof(string));
            dtSalesOrder.Columns.Add("Remaining", typeof(decimal));
            dtSalesOrder.Columns.Add("InterestRate", typeof(decimal));
            dtSalesOrder.Columns.Add("InterestAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("SalesDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("DownPayment", typeof(decimal));
            dtSalesOrder.Columns.Add("WInterestAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("FixedAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("IsStatus", typeof(int));
            dtSalesOrder.Columns.Add("UnExInstallment", typeof(int));
            dtSalesOrder.Columns.Add("Quantity", typeof(int));
            dtSalesOrder.Columns.Add("Discount", typeof(decimal));
            dtSalesOrder.Columns.Add("NetAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("IsUnExpected", typeof(int));
            dtSalesOrder.Columns.Add("Remarks", typeof(string));
            dtSalesOrder.Columns.Add("Status", typeof(int));
            dtSalesOrder.Columns.Add("VatPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("VatAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalDue", typeof(decimal));
            dtSalesOrder.Columns.Add("ConcernId", typeof(int));
            dtSalesOrder.Columns.Add("CreatedBy", typeof(int));
            dtSalesOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("TotalOffer", typeof(decimal));
            dtSalesOrder.Columns.Add("InstallmentPeriod", typeof(string));

            dtSalesOrder.Columns.Add("CardPaidAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("CardTypeSetupID", typeof(int));
            dtSalesOrder.Columns.Add("DepositChargePercent", typeof(decimal));
            dtSalesOrder.Columns.Add("IsWeekly", typeof(int));
            dtSalesOrder.Columns.Add("GuarName", typeof(string));
            dtSalesOrder.Columns.Add("GuarContactNo", typeof(string));
            dtSalesOrder.Columns.Add("GuarAddress", typeof(string));
            dtSalesOrder.Columns.Add("ProcessingFee", typeof(decimal));

            DataRow row = null;
            int NumberOfInstallment = 0;
            row = dtSalesOrder.NewRow();
            row["InvoiceNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["CustomerId"] = int.Parse(salesOrder.SalesOrder.CustomerId);
            row["TSalesAmt"] = decimal.Parse(salesOrder.SalesOrder.GrandTotal);
            NumberOfInstallment = int.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo));
            row["NoOfInstallment"] = NumberOfInstallment;
            row["InstallmentPrinciple"] = 0.0;
            row["IssueDate"] = DateTime.Parse(salesOrder.SalesOrder.InstallmentDate);
            row["UserName"] = string.Empty;
            row["Remaining"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue));
            row["InterestRate"] = 0;
            row["SalesDate"] = DateTime.Parse(salesOrder.SalesOrder.OrderDate);
            row["DownPayment"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount));
            row["WInterestAmt"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.WInterestAmt));
            row["InterestRate"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestRate));
            row["InterestAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalInterest));
            row["FixedAmount"] = 0.0;
            row["IsStatus"] = EnumSalesType.Sales;
            row["UnExInstallment"] = 0.0;
            row["Quantity"] = salesOrder.SODetails.Sum(x => int.Parse(x.Quantity));
            row["Discount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount));
            row["NetAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount));
            row["IsUnExpected"] = 0;
            row["Remarks"] = salesOrder.SalesOrder.Remarks;
            row["Status"] = 0;
            row["VatPercentage"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATPercentage));
            row["VatAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount));
            row["TotalDue"] = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.CurrentPreviousDue)));
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["CreatedDate"] = DateTime.Now;
            row["TotalOffer"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer));
            row["InstallmentPeriod"] = NumberOfInstallment + " Months";
            row["IsWeekly"] = Convert.ToInt32(salesOrder.SalesOrder.IsWeekly);
            row["GuarName"] = salesOrder.SalesOrder.GuarName;
            row["GuarContactNo"] = salesOrder.SalesOrder.GuarContactNo;
            row["GuarAddress"] = salesOrder.SalesOrder.GuarAddress; 
            row["ProcessingFee"] = salesOrder.SalesOrder.ProcessingFee;
            if (salesOrder.SalesOrder.CardPaidAmount > 0)
            {
                row["CardPaidAmount"] = salesOrder.SalesOrder.CardPaidAmount;
                row["CardTypeSetupID"] = salesOrder.SalesOrder.CardTypeSetupID;
                row["DepositChargePercent"] = salesOrder.SalesOrder.DepositChargePercent;

            }
            else
            {
                row["CardPaidAmount"] = 0;
                row["CardTypeSetupID"] = 0;
                row["DepositChargePercent"] = 0;

            }
            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UTAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MpRateTotal", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("IntPercentage", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("IntTotalAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("Compressor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Motor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Panel", typeof(int));
            dtSalesOrderDetail.Columns.Add("Spareparts", typeof(int));
            dtSalesOrderDetail.Columns.Add("Service", typeof(int));
            dtSalesOrderDetail.Columns.Add("CreditSalesDetailsId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Warranty", typeof(string));

            DataRow row = null;

            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();

                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["CreditSalesDetailsId"] = int.Parse(item.SODetailId);
                row["ProductId"] = int.Parse(item.ProductId);
                row["StockDetailId"] = int.Parse(item.StockDetailId);
                row["ColorId"] = int.Parse(item.ColorId);
                row["Quantity"] = int.Parse(item.Quantity);
                row["UnitPrice"] = decimal.Parse(item.UnitPrice);
                row["UTAmount"] = decimal.Parse(item.UTAmount);
                row["MpRateTotal"] = decimal.Parse(item.MRPRate);
                row["MrpRate"] = decimal.Parse(item.MRPRate);
                row["PPOffer"] = decimal.Parse(GetDefaultIfNull(item.PPOffer));
                row["IntPercentage"] = decimal.Parse(GetDefaultIfNull(item.IntPercentage));
                row["IntTotalAmt"] = decimal.Parse(GetDefaultIfNull(item.IntTotalAmt));
                row["Compressor"] = item.GodownID;
                row["Motor"] = 0;
                row["Panel"] = 0;
                row["Spareparts"] = 0;
                row["Service"] = 0;
                row["Warranty"] = "";


                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        private DataTable CreateSOSchedulesDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderSchedules = new DataTable();
            dtSalesOrderSchedules.Columns.Add("MonthDate", typeof(DateTime));
            dtSalesOrderSchedules.Columns.Add("Balance", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("InstallmentAmt", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("PaymentDate", typeof(DateTime));
            dtSalesOrderSchedules.Columns.Add("PaymentStatus", typeof(string));
            dtSalesOrderSchedules.Columns.Add("InterestAmount", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("ClosingBalance", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("ScheduleNo", typeof(int));
            dtSalesOrderSchedules.Columns.Add("Remarks", typeof(string));
            dtSalesOrderSchedules.Columns.Add("IsUnExpected", typeof(int));
            dtSalesOrderSchedules.Columns.Add("HireValue", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("NetValue", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("ExpectedInstallment", typeof(decimal));

            dtSalesOrderSchedules.Columns.Add("CardPaidAmount", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("CardTypeSetupID", typeof(int));
            dtSalesOrderSchedules.Columns.Add("DepositChargePercent", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("BankTransID", typeof(int));
            dtSalesOrderSchedules.Columns.Add("CreatedBy", typeof(int));

            DataRow row = null;
            int sno = 1;
            foreach (var item in salesOrder.SOSchedules)
            {

                row = dtSalesOrderSchedules.NewRow();
                row["MonthDate"] = DateTime.Parse(item.ScheduleDate);
                row["Balance"] = decimal.Parse(item.OpeningBalance);
                row["InstallmentAmt"] = decimal.Parse(item.InstallmentAmount);
                row["PaymentDate"] = string.IsNullOrEmpty(item.PayDate) ? SqlDateTime.MinValue.Value : DateTime.Parse(item.PayDate);
                row["PaymentStatus"] = item.PaymentStatus;
                row["InterestAmount"] = 0.0;
                row["ClosingBalance"] = decimal.Parse(GetDefaultIfNull(item.ClosingBalance));
                row["ScheduleNo"] = sno;
                row["Remarks"] = item.Remarks;
                row["IsUnExpected"] = item.IsUnExpected;
                row["HireValue"] = item.HireValue;
                row["NetValue"] = item.NetValue;
                row["ExpectedInstallment"] = item.ExpectedInstallment;

                row["CardPaidAmount"] = item.CardPaidAmount;
                row["CardTypeSetupID"] = item.CardTypeSetupID;
                row["DepositChargePercent"] = item.DepositChargePercent;
                row["BankTransID"] = item.BankTransID;
                row["CreatedBy"] = User.Identity.GetUserId<int>();

                dtSalesOrderSchedules.Rows.Add(row);
                sno++;
            }

            return dtSalesOrderSchedules;
        }

        private DataTable CreateBankTransDataTable(int BankID, int CardTypeID, decimal CardPaidAmount,
           DateTime TransDate, string TransactionNo)
        {
            DataTable dtBankTrans = new DataTable();
            dtBankTrans.Columns.Add("TranDate", typeof(DateTime));
            dtBankTrans.Columns.Add("TransactionNo", typeof(string));
            dtBankTrans.Columns.Add("TransactionType", typeof(int));
            dtBankTrans.Columns.Add("Amount", typeof(decimal));
            dtBankTrans.Columns.Add("BankID", typeof(decimal));
            dtBankTrans.Columns.Add("CustomerID", typeof(int));
            dtBankTrans.Columns.Add("SupplierID", typeof(int));
            dtBankTrans.Columns.Add("AnotherBankID", typeof(int));
            dtBankTrans.Columns.Add("ChecqueNo", typeof(string));
            dtBankTrans.Columns.Add("Remarks", typeof(string));
            dtBankTrans.Columns.Add("ConcernID", typeof(int));

            int CardTypeSetupID = 0;
            decimal DepositChargePercent = 0m;

            decimal DepositAmt = CardPaymentNetAmtCalculation(BankID, CardTypeID, CardPaidAmount,
                   out CardTypeSetupID, out DepositChargePercent);

            DataRow row = null;

            row = dtBankTrans.NewRow();
            row["TranDate"] = TransDate;
            row["TransactionNo"] = TransactionNo;
            row["TransactionType"] = EnumTransactionType.Deposit;
            row["Amount"] = DepositAmt;
            row["BankID"] = BankID;
            row["CustomerID"] = 0;
            row["SupplierID"] = 0;
            row["AnotherBankID"] = 0;
            row["ChecqueNo"] = string.Empty;
            row["Remarks"] = "Card payment deposite transactions during credit sales.";
            row["ConcernID"] = User.Identity.GetConcernId();

            dtBankTrans.Rows.Add(row);

            return dtBankTrans;
        }
        private decimal CardPaymentNetAmtCalculation(int BankID, int CardTypeID, decimal CardPaidAmount, out int CardTypeSetupID,
            out decimal DepositChargePercent)
        {
            var CardTypeSetup = _CardTypeSetupService.GetAll().FirstOrDefault(i => i.BankID == BankID
            && i.CardTypeID == CardTypeID);

            CardTypeSetupID = CardTypeSetup == null ? 0 : CardTypeSetup.CardTypeSetupID;
            DepositChargePercent = CardTypeSetup == null ? 0 : CardTypeSetup.Percentage;
            decimal DepositAmt = CardPaidAmount - ((CardPaidAmount * DepositChargePercent) / 100m);
            return DepositAmt;
        }
        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int orderId)
        {
            TempData["IsCInvoiceReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int orderId)
        {
            TempData["IsMoneyReceiptReadyByID"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public ActionResult Challan(int orderId)
        {
            TempData["IsChallanReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }
        private bool HasPaidInstallment(int id)
        {
            return _creditSalesOrderService.HasPaidInstallment(id);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpComingScheduleReport()
        {
            return View("UpComingScheduleReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult InstallmentCollectionReport()
        {
            return View("InstallmentCollectionReport");
        }



        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {

            if (!string.IsNullOrEmpty(imeiNo))
            {
                //var customProductDetails = _productService.GetAllProductFromDetailForCredit();
                //var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

                //var vmProduct = vmProductDetails.FirstOrDefault(x => x.IMENo.ToLower().Equals(imeiNo.ToLower()));
                var vmProduct = _stockService.GetStockIMEIDetail(imeiNo);
                if (vmProduct != null)
                {
                    return Json(new
                    {
                        Code = vmProduct.ProductCode,
                        Name = vmProduct.ProductName,
                        Id = vmProduct.ProductId,
                        StockDetailId = vmProduct.StockDetailsId,
                        ColorId = vmProduct.ColorId,
                        ColorName = vmProduct.ColorName,
                        MrpRate = vmProduct.MRPRate,
                        IMEINo = vmProduct.IMENo,
                        OfferDescription = vmProduct.OfferDescription,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msg = "IMEI not available in stock.", status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DefaultingCustomer()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HireAccountDetails()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetFamilyTree(User.Identity.GetConcernId()), "ConcernID", "Name");
            return View();
        }

        [HttpPost]
        public JsonResult UndoInstallment(int ScheduleID)
        {
            if (ScheduleID > 0)
            {
                var undoSchedule = _creditSalesOrderService.GetScheduleByScheduleID(ScheduleID);
                if (undoSchedule.PaymentStatus == "Due")
                {
                    return Json(new { result = false, msg = "Only paid schedule can be returened." }, JsonRequestBehavior.AllowGet);
                }
                var CreditSale = _creditSalesOrderService.GetSalesOrderById(undoSchedule.CreditSalesID);
                var Customer = _customerService.GetCustomerById(CreditSale.CustomerID);
                if (Customer != null)
                {
                    Customer.CreditDue = Customer.CreditDue + undoSchedule.InstallmentAmt + undoSchedule.LastPayAdjust;
                }
                if (CreditSale != null)
                {
                    CreditSale.Remaining = CreditSale.Remaining + undoSchedule.InstallmentAmt + undoSchedule.LastPayAdjust;
                    CreditSale.LastPayAdjAmt = 0;
                    CreditSale.ModifiedBy = User.Identity.GetUserId<int>();
                    CreditSale.ModifiedDate = GetLocalDateTime();
                }
                undoSchedule.InstallmentAmt += undoSchedule.LastPayAdjust;
                undoSchedule.PaymentStatus = "Due";
                _creditSalesOrderService.SaveSalesOrder();
                _customerService.SaveCustomer();
                AddToastMessage("", "Schedule No " + undoSchedule.ScheduleNo + " is undo successfull.", ToastType.Success);
                return Json(new { result = true, msg = "Schedule No " + undoSchedule.ScheduleNo + " is undo successfull." }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { result = false, msg = "Schedule not found" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminInstallmentCollectionReport()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var SOrder = _creditSalesOrderService.GetAllIQueryable().FirstOrDefault(i => i.CreditSalesID == orderId);
            if (SOrder.IsStatus != EnumSalesType.Pending)
            {
                AddToastMessage("", "This Order is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var VMSorder = _mapper.Map<CreditSale, CreateCreditSalesOrderViewModel>(SOrder);
            var SOrderDetails = _creditSalesOrderService.GetCustomSalesOrderDetails(orderId);
            var VMSOrderDetails = _mapper.Map<ICollection<Tuple<int, int, int, int, decimal, decimal, decimal,
                Tuple<decimal, string, string, int, string, decimal>>>, ICollection<CreateCreditSalesOrderDetailViewModel>>(SOrderDetails.ToList());

            var schedules = _creditSalesOrderService.GetSalesOrderSchedules(orderId);
            CreditSalesOrderViewModel SOVM = new CreditSalesOrderViewModel();
            SOVM.SalesOrder = VMSorder;
            SOVM.SODetails = VMSOrderDetails;

            SOVM.SOSchedules = _mapper.Map<IEnumerable<CreditSalesSchedule>, ICollection<CreateCreditSalesSchedules>>(schedules);

            StockDetail sDetail = null;

            foreach (var item in VMSOrderDetails)
            {
                sDetail = _stockDetailService.GetById(Convert.ToInt32(item.StockDetailId));
                item.ColorId = sDetail.ColorID.ToString();
            }
            SOVM.SalesOrder.CreateDateTime = GetLocalDateTime();
            DataTable dtSOrder = CreateSalesOrderDataTable(SOVM);
            DataTable dtSOrderDetails = CreateSODetailDataTable(SOVM);
            DataTable dtSalesOrderSchedules = CreateSOSchedulesDataTable(SOVM);
            DataTable dtBankTrans = CreateBankTransDataTable(SOVM.SalesOrder.BankID, SOVM.SalesOrder.CardTypeID,
              SOVM.SalesOrder.CardPaidAmount, Convert.ToDateTime(SOVM.SalesOrder.OrderDate), SOVM.SalesOrder.InvoiceNo);




            if (_creditSalesOrderService.ApprovedSalesOrderUsingSP(dtSOrder, dtSOrderDetails, dtSalesOrderSchedules, dtBankTrans, orderId))
            {
                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = orderId;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.HireSales;
                useraudit.ActionType = EnumActionType.Approved;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                AddToastMessage("", "Approved Successfull.", ToastType.Success);
            }
            else
                AddToastMessage("", "Approved failed.", ToastType.Error);

            return Json(true, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [Authorize]
        public JsonResult ScheduleApproved(int orderId)
        {
            DataTable dtBankTrans = null;
            var schedule = _creditSalesOrderService.GetScheduleByID(orderId);
            if (schedule.PaymentStatus != "Pending")
            {
                AddToastMessage("", "This schedule is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var HireOrder = _creditSalesOrderService.GetSalesOrderById(schedule.CreditSalesID);

            if (schedule.CardTypeSetupID > 0)
            {
                var setup = _CardTypeSetupService.GetById(schedule.CardTypeSetupID);

                dtBankTrans = CreateBankTransDataTable(setup.BankID, setup.CardTypeID,
                            schedule.CardPaidAmount, Convert.ToDateTime(schedule.PaymentDate), HireOrder.InvoiceNo);

            }

            if (_creditSalesOrderService.InstallmentApprovedSP(HireOrder.CreditSalesID, schedule.InstallmentAmt, schedule.LastPayAdjust,
                  dtBankTrans, schedule.CardTypeSetupID, schedule.CSScheduleID))
            {
                AddToastMessage("", "Installment approved successfull.");
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LastPayAdjReport()
        {
            return View("LastPayAdjReport");
        }


        [HttpGet]
        [Authorize]
        public ActionResult WarrantyHireInvoice(int orderId)
        {
            TempData["WarrantyHireInvoice"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }

    }
}
