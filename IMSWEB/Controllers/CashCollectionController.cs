using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("Collection-item")]

    public class CashCollectionController : CoreController
    {
        ICashCollectionService _CashCollectionService;
        ICustomerService _CustomerService;
        ISupplierService _SupplierService;
        IMiscellaneousService<CashCollection> _miscellaneousService;
        IMapper _mapper;
        IUserService _UserService;
        ISisterConcernService _SisterConcern;
        ISMSStatusService _SMSService;
        ISystemInformationService _SysService;
        ISMSStatusService _SMSStatusService;
        IPrevBalanceService _PrevBalanceService;
        IUserAuditDetailService _userAuditDetailService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IRoleService _roleService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        private readonly ICashCollTranHistoryService _cashCollTranHistoryService;

        public CashCollectionController(IErrorService errorService,
            ICashCollectionService cashCollectionService, ICustomerService customerService,
            ISupplierService supplierService, IMiscellaneousService<CashCollection> miscellaneousService, IUserAuditDetailService userAuditDetailService, IMapper mapper, IUserService UserService, ISisterConcernService SisterConcern, ISMSStatusService SMSService, ISystemInformationService SysService, ISMSStatusService SMSStatusService, IPrevBalanceService PrevBalanceService, ISessionMasterService sessionMasterService, IRoleService roleService,  ISMSBillPaymentBkashService sMSBillPaymentBkashService, ICashCollTranHistoryService cashCollTranHistoryService)
             : base(errorService, SysService)
        {
            _CashCollectionService = cashCollectionService;
            _CustomerService = customerService;
            _SupplierService = supplierService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _UserService = UserService;
            _SisterConcern = SisterConcern;
            _SMSService = SMSService;
            _SMSStatusService = SMSStatusService;
            _SysService = SysService;
            _PrevBalanceService = PrevBalanceService;
            _userAuditDetailService = userAuditDetailService;
            _sessionMasterService = sessionMasterService;
            _roleService = roleService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
            _cashCollTranHistoryService = cashCollTranHistoryService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                int EmployeeID = user.EmployeeID;
                var EMPitemsAsync = _CashCollectionService.GetAllCashCollByEmployeeIDAsync(EmployeeID);
                var EMPvmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await EMPitemsAsync);
                return View(EMPvmodel);
            }
            else
            {
                var itemsAsync = _CashCollectionService.GetAllCashCollAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
                return View(vmodel);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                //int EmployeeID = ConstantData.GetEmployeeIDByUSerID(User.Identity.GetUserId<int>());
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                int EmployeeID = user.EmployeeID;
                var EMPitemsAsync = _CashCollectionService.GetAllCashCollByEmployeeIDAsync(EmployeeID);
                var EMPvmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await EMPitemsAsync);
                return View(EMPvmodel);
            }
            else
            {
                var itemsAsync = _CashCollectionService.GetAllCashCollAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
                return View(vmodel);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string recptNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ReceiptNo));
            return View(new CreateCashCollectionViewModel { ReceiptNo = recptNo });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl)
        {

            AddModelError(newCashCollection, formCollection);
            if (!ModelState.IsValid)
                return View(newCashCollection);

            if (newCashCollection != null)
            {
                newCashCollection.CreateDate = DateTime.Now.ToString();
                newCashCollection.CreatedBy = (User.Identity.GetUserId<string>());
                newCashCollection.ConcernID = User.Identity.GetConcernId().ToString();

                newCashCollection.CustomerID = formCollection["CustomersId"];
                newCashCollection.SupplierID = "0";
                var sysInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());


                if (sysInfo.ApprovalSystemEnable == 1)

                    newCashCollection.TransactionType = EnumTranType.CollectionPending;
                else
                    newCashCollection.TransactionType = EnumTranType.FromCustomer;


                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.Type == EnumDropdownTranType.FromCustomer)
                    newCashCollection.TransactionType = EnumTranType.FromCustomer;
                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                    newCashCollection.TransactionType = EnumTranType.CollectionReturn;
                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.Type == EnumDropdownTranType.DebitAdjustment)
                    newCashCollection.TransactionType = EnumTranType.DebitAdjustment;
                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.Type == EnumDropdownTranType.CreditAdjustment)
                    newCashCollection.TransactionType = EnumTranType.CreditAdjustment;       

                //newCashCollection.ModifiedBy = "0";
                //newCashCollection.ModifiedDate = DateTime.Now.ToString();

                if (newCashCollection.AccountNo == null)
                    newCashCollection.AccountNo = "No A/C";

                var cashCollection = _mapper.Map<CreateCashCollectionViewModel, CashCollection>(newCashCollection);



                #region Total Due Update
                var oCustomer = _CustomerService.GetCustomerById(Convert.ToInt32(newCashCollection.CustomerID));
                if (sysInfo.ApprovalSystemEnable == 0)
                {
                    if (newCashCollection.TransactionType == EnumTranType.FromCustomer)
                        oCustomer.TotalDue = (oCustomer.TotalDue + cashCollection.InterestAmt) - (cashCollection.Amount + cashCollection.AdjustAmt);
                    else if (newCashCollection.TransactionType == EnumTranType.CollectionReturn)
                        oCustomer.TotalDue = oCustomer.TotalDue + (cashCollection.Amount + cashCollection.AdjustAmt);
                    else if (newCashCollection.TransactionType == EnumTranType.DebitAdjustment)
                        oCustomer.TotalDue = oCustomer.TotalDue + (cashCollection.Amount + cashCollection.AdjustAmt);                   
                    else if (newCashCollection.TransactionType == EnumTranType.CreditAdjustment)
                        oCustomer.TotalDue = oCustomer.TotalDue - (cashCollection.Amount + cashCollection.AdjustAmt);
                    newCashCollection.BalanceDue = oCustomer.TotalDue.ToString();

                }

                #endregion


                #region Remind Date Update
                if (Convert.ToDateTime(newCashCollection.RemindDate) > Convert.ToDateTime(newCashCollection.EntryDate))
                {
                    if (oCustomer != null)
                    {
                        oCustomer.RemindDate = Convert.ToDateTime(newCashCollection.RemindDate);
                        _CustomerService.UpdateCustomer(oCustomer);
                        _CustomerService.SaveCustomer();
                    }
                }
                #endregion
                bool Status = false;
                try
                {
                    _CashCollectionService.AddCashCollection(cashCollection);
                    _CashCollectionService.SaveCashCollection();

                    Status = true;
                    UserAuditDetail useraudit = new UserAuditDetail();
                    useraudit.ObjectID = cashCollection.CashCollectionID;
                    useraudit.ActivityDtTime = GetLocalDateTime();
                    useraudit.ObjectType = EnumObjectType.CashCollection;
                    useraudit.ActionType = EnumActionType.Add;
                    useraudit.ConcernID = User.Identity.GetConcernId();
                    useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                    useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                    _userAuditDetailService.Add(useraudit);
                    _userAuditDetailService.Save();

                    CashCollTranHistory trasHistory = new CashCollTranHistory();
                    trasHistory.CashCollectionID = cashCollection.CashCollectionID;
                    trasHistory.ReceiptNo = cashCollection.ReceiptNo;                 
                    trasHistory.CreateOrEdit = "Create";
                    trasHistory.Value = "CustomerId-" + cashCollection.CustomerID + ", CusName-" + cashCollection.Customer.Name + ", Amount-" + cashCollection.Amount +
                        ", AdjAmt-" + cashCollection.AdjustAmt + ", IntAmt-" + cashCollection.InterestAmt + ", TransType-" + cashCollection.TransactionType + ", CollDate-" + cashCollection.EntryDate;
                    trasHistory.ConcernID = User.Identity.GetConcernId();
                    trasHistory.CreateOrEditBy = cashCollection.CreatedBy;
                    trasHistory.HistoryDate = GetLocalDateTime();
                    _cashCollTranHistoryService.Add(trasHistory);
                    _cashCollTranHistoryService.Save();



                }
                catch (Exception)
                {
                    Status = false;
                }

                if (Status)
                {
                    _CustomerService.UpdateCustomer(oCustomer);
                    _CustomerService.SaveCustomer();
                }

                //_CashCollectionService.UpdateTotalDue(Convert.ToInt32(newCashCollection.CustomerID), 0, 0, 0, Convert.ToDecimal(Convert.ToDecimal(newCashCollection.Amount) + Convert.ToDecimal(newCashCollection.AdjustAmt)));

                TempData["MoneyReceiptData"] = cashCollection;
                TempData["IsMoneyReceiptReady"] = true;

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);


                #region SMS Service
                var SystemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                if (SystemInfo.IsCashcollSMSEnable == 1 && newCashCollection.IsSmsEnable == true)
                {
                    if (SystemInfo.IsBanglaSmsEnable == 1)
                    {
                        List<SMSRequest> sms = new List<SMSRequest>();
                        sms.Add(new SMSRequest()
                        {
                            MobileNo = oCustomer.ContactNo,
                            CustomerID = oCustomer.CustomerID,
                            CustomerCode = oCustomer.Code,
                            TransNumber = cashCollection.ReceiptNo,
                            Date = (DateTime)cashCollection.EntryDate,
                            PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                            ReceiveAmount = (decimal)cashCollection.Amount,
                            PresentDue = oCustomer.TotalDue,
                            SMSType = EnumSMSType.CashCollection
                        });

                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerID = oCustomer.CustomerID,
                                CustomerCode = oCustomer.Code,
                                TransNumber = cashCollection.ReceiptNo,
                                Date = (DateTime)cashCollection.EntryDate,
                                PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                                ReceiveAmount = (decimal)cashCollection.Amount,
                                PresentDue = oCustomer.TotalDue,
                                SMSType = EnumSMSType.CashCollection
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        decimal previousBalance;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfos.smsCharge;
                        if (smsAmountDetails.TotalRecAmt > 1)
                        {
                            var response = await Task.Run(() => SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
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
                            
                    }

                    else
                    {
                        List<SMSRequest> sms = new List<SMSRequest>();
                        sms.Add(new SMSRequest()
                        {
                            MobileNo = oCustomer.ContactNo,
                            CustomerID = oCustomer.CustomerID,
                            CustomerCode = oCustomer.Code,
                            CustomerName = oCustomer.Name,
                            TransNumber = cashCollection.ReceiptNo,
                            Date = (DateTime)cashCollection.EntryDate,
                            PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                            ReceiveAmount = (decimal)cashCollection.Amount,
                            PresentDue = oCustomer.TotalDue,
                            SMSType = EnumSMSType.CashCollection
                        });

                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerID = oCustomer.CustomerID,
                                CustomerCode = oCustomer.Code,
                                CustomerName = oCustomer.Name,
                                TransNumber = cashCollection.ReceiptNo,
                                Date = (DateTime)cashCollection.EntryDate,
                                PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                                ReceiveAmount = (decimal)cashCollection.Amount,
                                PresentDue = oCustomer.TotalDue,
                                SMSType = EnumSMSType.CashCollection
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        int paymentMasterId;
                        decimal previousBalance;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfos.smsCharge;
                        if (smsAmountDetails.TotalRecAmt > 1)                     
                        {
                            var response = await Task.Run(() => SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
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
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddModelError(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("CustomerID", "Customer is Required.");
            else
                newCashCollection.CustomerID = formCollection["CustomersId"];

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");
            if (decimal.Parse(GetDefaultIfNull(newCashCollection.AdjustAmt)) < 0m)
                ModelState.AddModelError("AdjustAmt", "Adjustment can't be negative");

            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
            {
                string recNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ReceiptNo));
                newCashCollection.ReceiptNo = recNo;
            }
            // ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

            if (!IsDateValid(Convert.ToDateTime(newCashCollection.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }

            if (newCashCollection.Type == 0)
                ModelState.AddModelError("Type", "Trans. Type is Required.");
        }



        private void AddModelErrorForEdit(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("CustomerID", "Customer is Required.");
            else
                newCashCollection.CustomerID = formCollection["CustomersId"];

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");
            if (decimal.Parse(GetDefaultIfNull(newCashCollection.AdjustAmt)) < 0m)
                ModelState.AddModelError("AdjustAmt", "Adjustment can't be negative");

            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
            {
                string recNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ReceiptNo));
                newCashCollection.ReceiptNo = recNo;
            }
            // ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

            if (!IsDateValid(Convert.ToDateTime(newCashCollection.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }
           
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            //var customerItems = _CustomerService.GetAllCustomer().Select(cusItem
            //   => new SelectListItem { Text = cusItem.Name, Value = cusItem.CustomerID.ToString() }).ToList();

            //var supplierItems = _SupplierService.GetAllSupplier().Select(suppItem
            //    => new SelectListItem { Text = suppItem.Name, Value = suppItem.SupplierID.ToString() }).ToList();

            var cashCollection = _CashCollectionService.GetCashCollectionById(id);
            if (!IsDateValid(Convert.ToDateTime(cashCollection.EntryDate)))
                return RedirectToAction("Index");
            var vmodel = _mapper.Map<CashCollection, CreateCashCollectionViewModel>(cashCollection);
            var Customer = _CustomerService.GetCustomerById((int)cashCollection.CustomerID);
            vmodel.CurrentDue = Customer.TotalDue.ToString();
            vmodel.RemindDate = Customer.RemindDate.ToString();
            //vmodel.CustomerItems = customerItems;
            //vmodel.SupplierItems = supplierItems;
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl)
        {
            AddModelErrorForEdit(newCashCollection, formCollection);

            if (!ModelState.IsValid)
                return View("Create", newCashCollection);

            if (newCashCollection != null)
            {
                var cashCollection = _CashCollectionService.GetCashCollectionById(int.Parse(newCashCollection.CashCollectionID));

                cashCollection.PaymentType = newCashCollection.PaymentType;
                cashCollection.BankName = newCashCollection.BankName;
                cashCollection.BranchName = newCashCollection.BranchName;
                cashCollection.EntryDate = Convert.ToDateTime(newCashCollection.EntryDate);
                cashCollection.Amount = decimal.Parse(newCashCollection.Amount);
                cashCollection.AccountNo = newCashCollection.AccountNo;
                cashCollection.MBAccountNo = newCashCollection.MBAccountNo;
                cashCollection.BKashNo = newCashCollection.BKashNo;
                cashCollection.Remarks = newCashCollection.Remarks;
                cashCollection.TransactionType = cashCollection.TransactionType;
                cashCollection.AdjustAmt = decimal.Parse(newCashCollection.AdjustAmt);
                cashCollection.BalanceDue = decimal.Parse(newCashCollection.BalanceDue);
                cashCollection.CustomerID = int.Parse(formCollection["CustomersId"]);

                //if (newCashCollection.Type == EnumDropdownTranType.DebitAdjustment)
                //    newCashCollection.TransactionType = EnumTranType.DebitAdjustment;

                //if (newCashCollection.Type == EnumDropdownTranType.CreditAdjustment)
                //    newCashCollection.TransactionType = EnumTranType.CreditAdjustment;

                //if (newCashCollection.Type == EnumDropdownTranType.FromCustomer)
                //    newCashCollection.TransactionType = EnumTranType.FromCustomer;

                //else if (newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                //    newCashCollection.TransactionType = EnumTranType.CollectionReturn;

                //cashCollection.TransactionType = newCashCollection.TransactionType;
                cashCollection.ModifiedBy = User.Identity.GetUserId<int>();
                cashCollection.ModifiedDate = DateTime.Now;
                //cashCollection.SupplierID = int.Parse(newCashCollection.SupplierID);
                cashCollection.InterestAmt = decimal.Parse(GetDefaultIfNull(newCashCollection.InterestAmt));
                if(cashCollection.TransactionType == EnumTranType.CollectionReturn || cashCollection.TransactionType == EnumTranType.DebitAdjustment)
                {
                    _CashCollectionService.UpdateTotalDueWhenEditReturnType((int)cashCollection.CustomerID, 0, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt - cashCollection.InterestAmt));
                }
                else
                {
                    _CashCollectionService.UpdateTotalDueWhenEdit((int)cashCollection.CustomerID, 0, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt - cashCollection.InterestAmt));
                }

                cashCollection.BalanceDue = _CustomerService.GetCustomerById((int)cashCollection.CustomerID).TotalDue;
                _CashCollectionService.UpdateCashCollection(cashCollection);
                _CashCollectionService.SaveCashCollection();

                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = int.Parse(newCashCollection.CashCollectionID);
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.CashCollection;
                useraudit.ActionType = EnumActionType.Edit;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                var cus = _CustomerService.GetCustomerById(int.Parse(newCashCollection.CustomerID));

                CashCollTranHistory trasHistory = new CashCollTranHistory();
                trasHistory.CashCollectionID = int.Parse(newCashCollection.CashCollectionID);
                trasHistory.ReceiptNo = newCashCollection.ReceiptNo;
                trasHistory.CreateOrEdit = "Edit";
                trasHistory.Value = "CustomerId-" + newCashCollection.CustomerID + ", CusName-" + cus.Name + ", Amount-" + newCashCollection.Amount +", AdjAmt-" + newCashCollection.AdjustAmt + ", IntAmt-" + newCashCollection.InterestAmt + ", TransType-" + cashCollection.TransactionType + ", CollDate-" + newCashCollection.EntryDate;
                trasHistory.ConcernID = User.Identity.GetConcernId();
                trasHistory.CreateOrEditBy = cashCollection.CreatedBy;
                trasHistory.HistoryDate = GetLocalDateTime();
                _cashCollTranHistoryService.Add(trasHistory);
                _cashCollTranHistoryService.Save();

                #region Remind Date Update
                if (Convert.ToDateTime(newCashCollection.RemindDate) > Convert.ToDateTime(newCashCollection.EntryDate))
                {
                    var customer = _CustomerService.GetCustomerById(Convert.ToInt32(cashCollection.CustomerID));
                    if (customer != null)
                    {
                        customer.RemindDate = Convert.ToDateTime(newCashCollection.RemindDate);
                        _CustomerService.UpdateCustomer(customer);
                        _CustomerService.SaveCustomer();
                    }
                }
                #endregion

                TempData["MoneyReceiptData"] = cashCollection;
                TempData["IsMoneyReceiptReady"] = true;
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var oldCashCollection = _CashCollectionService.GetCashCollectionById(id);
            decimal Amount = 0m, AdjAmt = 0m, IntAmt = 0m;
            Amount = oldCashCollection.Amount;
            AdjAmt = oldCashCollection.AdjustAmt;
            IntAmt = oldCashCollection.InterestAmt;

            if (!IsDateValid(Convert.ToDateTime(oldCashCollection.EntryDate)))
            {
                return RedirectToAction("Index");
            }

            var oCustomer = _CustomerService.GetCustomerById(Convert.ToInt32(oldCashCollection.CustomerID));
            if (oldCashCollection.TransactionType == EnumTranType.FromCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt) - IntAmt;
            else if (oldCashCollection.TransactionType == EnumTranType.CollectionReturn)
                oCustomer.TotalDue = oCustomer.TotalDue - (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.DebitAdjustment)
                oCustomer.TotalDue = oCustomer.TotalDue - (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.CreditAdjustment)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.KPIForCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.IncentiveForCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.PriceProtectionForCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.PromoOfferForCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);
            else if (oldCashCollection.TransactionType == EnumTranType.RateAdjustmentForCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt);

            //_CashCollectionService.UpdateTotalDue(Convert.ToInt32(CashCollection.CustomerID), 0, 0, 0, -(Convert.ToDecimal(Convert.ToDecimal(CashCollection.Amount) + Convert.ToDecimal(CashCollection.AdjustAmt))));
            bool Status = false;
            try
            {
                _CashCollectionService.DeleteCashCollection(id);
                _CashCollectionService.SaveCashCollection();
                Status = true;

                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = id;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.CashCollection;
                useraudit.ActionType = EnumActionType.Delete;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();
            }
            catch (Exception)
            {
                Status = false;
            }

            if (Status)
            {
                _CustomerService.UpdateCustomer(oCustomer);
                _CustomerService.SaveCustomer();
            }
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashCollectionReport()//CashCollectionReport
        {
            return View("CashCollectionReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyCashBookLedgerReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRWiseCashCollectionReport()//CashCollectionReport
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["CashCollectionID"] = id;
            TempData["IsMoneyReceiptById"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminCashcolletionReport()//CashCollectionReport
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashInHandReport()
        {

            #region Opening Save for cash in hand report
            //var pb = _PrevBalanceService.DailyBalanceProcess(User.Identity.GetConcernId());
            //if (pb.Count != 0)
            //{
            //    foreach (var item in pb)
            //    {
            //        _PrevBalanceService.AddPrevBalance(item);
            //    }
            //}
            //_PrevBalanceService.Save();
            #endregion
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult TypeWiseCashInHand()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProfitAndLossReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MonthlyTransactionReport()
        {
            if (User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.superadmin.ToString()) || User.IsInRole(EnumUserRoles.hoqueLocalAdmin.ToString()))
                @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        public ActionResult AdminCashInhand()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var CashCollection = _CashCollectionService.GetCashCollectionById(orderId);
            if (CashCollection.TransactionType != EnumTranType.CollectionPending)
            {
                AddToastMessage("", "Cash collection is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);

            }

            decimal NetAmount = (Convert.ToDecimal(Convert.ToDecimal(CashCollection.Amount) + Convert.ToDecimal(CashCollection.AdjustAmt)))
                - Convert.ToDecimal(CashCollection.InterestAmt);
            _CashCollectionService.UpdateTotalDue(Convert.ToInt32(CashCollection.CustomerID), 0, 0, 0, NetAmount);

            CashCollection.TransactionType = EnumTranType.FromCustomer;
            _CashCollectionService.UpdateCashCollection(CashCollection);
            _CashCollectionService.SaveCashCollection();

            UserAuditDetail useraudit = new UserAuditDetail();
            useraudit.ObjectID = orderId;
            useraudit.ActivityDtTime = GetLocalDateTime();
            useraudit.ObjectType = EnumObjectType.CashCollection;
            useraudit.ActionType = EnumActionType.Approved;
            useraudit.ConcernID = User.Identity.GetConcernId();
            useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
            useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
            _userAuditDetailService.Add(useraudit);
            _userAuditDetailService.Save();
            AddToastMessage("", "Item has been approved successfully.", ToastType.Success);

            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize]
        public ActionResult AdjustmentReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ServiceCharge()
        {
            if (User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.superadmin.ToString()))
                @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashCollectionReportNew()
        {
            return View("CashCollectionReportNew");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DiscountAdjReportNew()
        {
            return View();
        }
    }
}