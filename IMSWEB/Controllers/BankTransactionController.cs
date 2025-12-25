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
using IMSWEB.Report;
using log4net;
using System.Text;
namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("bank-transaction")]
    public class BankTransactionController : CoreController
    {
        IBankService _baseBankService;
        IBankTransactionService _bankTransactionService;
        ICashCollectionService _CashCollectionService;
        IMiscellaneousService<BankTransaction> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/products";
        ISMSStatusService _SMSService;
        ISystemInformationService _SysService;
        ISMSStatusService _SMSStatusService;
        ICustomerService _CustomerService;
        IShareInvestmentHeadService _shareInvestmentHeadService;
        IExpenseItemService _expenseItemService;
        private readonly ISisterConcernService _sisterConcernService;
        private readonly ISystemInformationService _sysInfoService;

        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        public BankTransactionController
            (
            IErrorService errorService,
            IBankService baseBankService,
            IBankTransactionService bankTransactionService,
            ICashCollectionService CashCollectionService,
            IMiscellaneousService<BankTransaction> miscellaneousService,
            IMapper mapper, ISMSStatusService SMSService, ISystemInformationService SysService,
            ISMSStatusService SMSStatusService, ICustomerService CustomerService, IShareInvestmentHeadService shareInvestmentHeadService, IExpenseItemService expenseItemService, ISisterConcernService sisterConcernService, ISMSBillPaymentBkashService sMSBillPaymentBkashService
            )
            : base(errorService, SysService)
        {
            _bankTransactionService = bankTransactionService;
            _CashCollectionService = CashCollectionService;
            _miscellaneousService = miscellaneousService;
            _baseBankService = baseBankService;
            _mapper = mapper;
            _SMSService = SMSService;
            _SMSStatusService = SMSStatusService;
            _SysService = SysService;
            _CustomerService = CustomerService;
            _shareInvestmentHeadService = shareInvestmentHeadService;
            _expenseItemService = expenseItemService;
            _sisterConcernService = sisterConcernService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            ViewBag.IsBankApprovalEnable = _SysService.IsBankApprovalEnable();
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            List<EnumWFStatus> enumWFStatus = new List<EnumWFStatus>();
            enumWFStatus.Add(EnumWFStatus.Pending);
            enumWFStatus.Add(EnumWFStatus.Approved);


            var systemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            int concernId = User.Identity.GetConcernId();
            if (systemInfo.IsCommonBank)
                concernId = _sisterConcernService.GetParentConcernId(concernId);


            var customBankTransactionAsync = _bankTransactionService.GetAllBankTransactionNewAsync(ViewBag.FromDate, ViewBag.ToDate, enumWFStatus, concernId);
            var vm = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>, IEnumerable<GetBankTransactionViewModel>>(await customBankTransactionAsync);

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            ViewBag.IsBankApprovalEnable = _SysService.IsBankApprovalEnable();
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            List<EnumWFStatus> enumWFStatus = new List<EnumWFStatus>();
            enumWFStatus.Add(EnumWFStatus.Pending);
            enumWFStatus.Add(EnumWFStatus.Approved);

            var systemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            int concernId = User.Identity.GetConcernId();
            if (systemInfo.IsCommonBank)
                concernId = _sisterConcernService.GetParentConcernId(concernId);


            var customBankTransactionAsync = _bankTransactionService.GetAllBankTransactionNewAsync(ViewBag.FromDate, ViewBag.ToDate, enumWFStatus, concernId);
            var vm = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>, IEnumerable<GetBankTransactionViewModel>>(await customBankTransactionAsync);
            return View("Index", vm);
        }



        //private CreateBankTransactionViewModel PopulateDropdown(CreateBankTransactionViewModel model, EnumInvestmentType investmentType)
        //{
        //    model.Heads = _shareInvestmentHeadService.GetAll().Where(i => i.ParentId == (int)investmentType).ToList();
        //    return model;
        //}
        private CreateBankTransactionViewModel PopulateDropdown(CreateBankTransactionViewModel model, EnumInvestmentType investmentType, EnumInvestmentType investmentTypes)
        {
            model.Heads = _shareInvestmentHeadService.GetAll().Where(i => i.ParentId == (int)investmentTypes || i.ParentId == (int)investmentType).ToList();
            return model;
        }


        //private CreateBankTransactionViewModel PopulateDropdownEx(CreateBankTransactionViewModel model, EnumCompanyTransaction enumCompanyTran)
        //{
        //    model.ExpenseItemsHeads = _expenseItemService.GetAll().Where(i => i.ExpenseItemID == (int)enumCompanyTran).ToList();
        //    return model;
        //}

        //private CreateBankTransactionViewModel PopulateDropdownIn(CreateBankTransactionViewModel model, EnumCompanyTransaction enumCompanyTran)
        //{
        //    model.ExpenseItemsHeads = _expenseItemService.GetAll().Where(i => i.ExpenseItemID == (int)enumCompanyTran).ToList();
        //    return model;
        //}


        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {

            string TransactionNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.TransactionNo));
            return View(PopulateDropdown(new CreateBankTransactionViewModel { TransactionType = EnumTransactionType.Deposit, TransactionNo = TransactionNo }, EnumInvestmentType.Liability, EnumInvestmentType.Security
                ));

        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreateBankTransactionViewModel newBankTransaction, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(formCollection, newBankTransaction);

            if (!ModelState.IsValid)
                return View(PopulateDropdown(newBankTransaction, EnumInvestmentType.Liability, EnumInvestmentType.Security));

            if (newBankTransaction != null)
            {
                newBankTransaction.CreateDate = DateTime.Today.ToString();
                newBankTransaction.CreatedBy = (User.Identity.GetUserId<string>());
                MapFormCollectionValueWithNewEntity(newBankTransaction, formCollection);
                var bankTransaction = _mapper.Map<CreateBankTransactionViewModel, BankTransaction>(newBankTransaction);
                bankTransaction.ConcernID = User.Identity.GetConcernId();
                var sysInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                if (sysInfo.ApprovalSystemEnable == 0 && sysInfo.IsBankApprovalEnable == 0)
                {
                    BankBalanceUpdate(bankTransaction, true);
                }
                else
                    bankTransaction.WFStatus = EnumWFStatus.Pending;

                bankTransaction.ConcernID = User.Identity.GetConcernId();
                _bankTransactionService.AddBankTransaction(bankTransaction);
                _bankTransactionService.SaveBankTransaction();
                TempData["BankTranID"] = bankTransaction.BankTranID;
                AddToastMessage("", "Bank Transaction has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
                #region SMS Service
                if (bankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                {
                    var oCustomer = _CustomerService.GetCustomerById((int)bankTransaction.CustomerID);
                    List<SMSRequest> sms = new List<SMSRequest>(){
                        new SMSRequest()
                        {
                        MobileNo=oCustomer.ContactNo,
                        CustomerID=oCustomer.CustomerID,
                        CustomerCode = oCustomer.Code,
                        TransNumber = bankTransaction.TransactionNo,
                        Date=(DateTime)bankTransaction.TranDate,
                        PreviousDue=oCustomer.TotalDue+(decimal)bankTransaction.Amount,
                        ReceiveAmount=(decimal)bankTransaction.Amount,
                        PresentDue=oCustomer.TotalDue,
                        SMSType = EnumSMSType.CashCollection
                        }
                    };

                    var SystemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    int concernId = User.Identity.GetConcernId();
                    decimal previousBalance;
                    SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                    previousBalance = smsAmountDetails.TotalRecAmt;
                    var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    decimal smsFee = sysInfos.smsCharge;
                    if (smsAmountDetails.TotalRecAmt > 1)
                    {
                        var response = await Task.Run(() => SMSHTTPService.SendSMSAsync(EnumOnnoRokomSMSType.OneToOne, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));

                        if (response.Count > 0)
                        {
                            decimal smsBalanceCount = 0m;
                            foreach (var item in response)
                            {
                                smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                if (item.NoOfSMS == 0)
                                {
                                    AddToastMessage("", "Plz Check SMS Balance, Or Check Error Status ", ToastType.Error);
                                }
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
                #endregion
            }
            else
            {
                AddToastMessage("", "No Transaction data found to save.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        private bool BankBalanceUpdate(BankTransaction bankTransaction, bool IsNewEntry)
        {
            bankTransaction.WFStatus = EnumWFStatus.Approved;
            bool Result = true;
            decimal amountSign = IsNewEntry ? 1 : -1;
            int CurrUserId = Convert.ToInt32(User.Identity.GetUserId<string>());
            try
            {
                if (bankTransaction.TransactionType == (int)EnumTransactionType.Deposit)

                {
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.BankID), 0,
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.Withdraw)

                {
                    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(bankTransaction.BankID),
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                {
                    _CashCollectionService.UpdateTotalDue(Convert.ToInt32(bankTransaction.CustomerID), 0,
                        Convert.ToInt32(bankTransaction.BankID), 0, Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.CashDelivery)
                {
                    _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(bankTransaction.SupplierID), 0,
                        Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                //else if (bankTransaction.TransactionType == (int)EnumTransactionType.PaymentReturn)
                //{
                //    _CashCollectionService.UpdateTotalDuePaymentReturn(Convert.ToInt32(bankTransaction.SupplierID), 0,
                //        Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                //}
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
                {
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.AnotherBankID),
                        Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                //else if (bankTransaction.AnotherBankID != 0)
                //    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.AnotherBankID), 0, Convert.ToDecimal(bankTransaction.Amount));
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.LiaRec)
                {
                    _CashCollectionService.UpdateTotalDueForInvestment(Convert.ToInt32(bankTransaction.SIHID), 0, Convert.ToInt32(bankTransaction.BankID), 0,
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.LiaPay)
                {
                    _CashCollectionService.UpdateTotalDueForInvestment(0, Convert.ToInt32(bankTransaction.SIHID), 0, Convert.ToInt32(bankTransaction.BankID),
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }

                else if (bankTransaction.TransactionType == (int)EnumTransactionType.BankIncome)
                {
                    _CashCollectionService.UpdateTotalDueForExpenditure(Convert.ToInt32(bankTransaction.ExpenseItemID), Convert.ToInt32(bankTransaction.BankID), 0,
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign, bankTransaction.ConcernID, CurrUserId, bankTransaction.Remarks, Convert.ToDateTime(bankTransaction.TranDate));
                }

                else if (bankTransaction.TransactionType == (int)EnumTransactionType.BankExpense)
                {
                    _CashCollectionService.UpdateTotalDueForExpenditure(Convert.ToInt32(bankTransaction.ExpenseItemID), 0, Convert.ToInt32(bankTransaction.BankID),
                        Convert.ToDecimal(bankTransaction.Amount) * amountSign, bankTransaction.ConcernID, CurrUserId, bankTransaction.Remarks, Convert.ToDateTime(bankTransaction.TranDate));
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.ReturnFromSupplier)
                {
                    _CashCollectionService.UpdateTotalDuePaymentReturn(Convert.ToInt32(bankTransaction.SupplierID), Convert.ToInt32(bankTransaction.BankID),
                        0, Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.CustomerCollectionReturn)
                {
                    _CashCollectionService.UpdateTotalDueCustomerPaymentReturn(Convert.ToInt32(bankTransaction.CustomerID), 0,
                        Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount) * amountSign);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Result = false;
            }

            return Result;
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            CreateBankTransactionViewModel VMObj = new CreateBankTransactionViewModel();
            var bankTransaction = _bankTransactionService.GetBankTransactionById(id);
            if (!IsDateValid(Convert.ToDateTime(bankTransaction.TranDate)))
                return RedirectToAction("Index");
            VMObj = _mapper.Map<BankTransaction, CreateBankTransactionViewModel>(bankTransaction);

            return View("Create", PopulateDropdown(VMObj, EnumInvestmentType.Liability, EnumInvestmentType.Security));
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateBankTransactionViewModel newBankTransaction, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(formCollection, newBankTransaction);

            if (!ModelState.IsValid)
                return View("Create", newBankTransaction);

            if (newBankTransaction != null)
            {
                var existingBankTransaction = _bankTransactionService.GetBankTransactionById(int.Parse(newBankTransaction.BankTranID));
                //Previous amount return
                BankBalanceUpdate(existingBankTransaction, false);
                //Mapping picker values
                MapFormCollectionValueWithNewEntity(newBankTransaction, formCollection);

                #region update properties
                existingBankTransaction.TranDate = newBankTransaction.TranDate;
                existingBankTransaction.TransactionNo = newBankTransaction.TransactionNo;
                existingBankTransaction.CustomerID = Convert.ToInt32(newBankTransaction.CustomerID);
                existingBankTransaction.SupplierID = Convert.ToInt32(newBankTransaction.SupplierID);
                existingBankTransaction.BankID = Convert.ToInt32(newBankTransaction.BankID);
                existingBankTransaction.AnotherBankID = Convert.ToInt32(newBankTransaction.AnotherBankID);
                existingBankTransaction.Amount = newBankTransaction.Amount;
                existingBankTransaction.TransactionType = (int)newBankTransaction.TransactionType;
                existingBankTransaction.Amount = newBankTransaction.Amount;
                existingBankTransaction.ConcernID = User.Identity.GetConcernId();
                existingBankTransaction.Remarks = newBankTransaction.Remarks;
                existingBankTransaction.ChecqueNo = newBankTransaction.ChecqueNo;
                #endregion
                if (existingBankTransaction.WFStatus == EnumWFStatus.Approved)
                    BankBalanceUpdate(existingBankTransaction, true);

                existingBankTransaction.ModifiedDate = DateTime.Now;
                existingBankTransaction.ModifiedBy = User.Identity.GetUserId<int>();

                _bankTransactionService.UpdateBankTransaction(existingBankTransaction);
                _bankTransactionService.SaveBankTransaction();
                TempData["BankTranID"] = existingBankTransaction.BankTranID;
                AddToastMessage("", "BankTransaction has been updated successfully.", ToastType.Success);




                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Product data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        //[HttpGet]
        //[Authorize]
        //[Route("delete/{id}")]
        //public ActionResult Delete(int id)
        //{
        //    var existingBankTransaction = _bankTransactionService.GetBankTransactionById(id);
        //    if (!IsDateValid(Convert.ToDateTime(existingBankTransaction.TranDate)))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    BankBalanceUpdate(existingBankTransaction, false);

        //    _bankTransactionService.DeleteBankTransaction(id);
        //    _bankTransactionService.SaveBankTransaction();
        //    AddToastMessage("", "BankTransaction has been deleted successfully.", ToastType.Success);
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var existingBankTransaction = _bankTransactionService.GetBankTransactionById(id);
            if (!IsDateValid(Convert.ToDateTime(existingBankTransaction.TranDate)))
            {
                return RedirectToAction("Index");
            }
            if (existingBankTransaction.WFStatus == EnumWFStatus.Approved)
            {
                //existingBankTransaction.Amount = -existingBankTransaction.Amount;
                BankBalanceUpdate(existingBankTransaction, false);
            }
            _bankTransactionService.DeleteBankTransaction(id);
            _bankTransactionService.SaveBankTransaction();
            AddToastMessage("", "BankTransaction has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(FormCollection formCollection,
            CreateBankTransactionViewModel newBankTransaction)
        {
            int TransID = Convert.ToInt32(GetDefaultIfNull(newBankTransaction.BankTranID));
            if (_miscellaneousService.GetDuplicateEntry(i => i.TransactionNo.Equals(newBankTransaction.TransactionNo)
             && i.BankTranID != TransID) != null)
            {
                newBankTransaction.TransactionNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.TransactionNo));
            }

            if (string.IsNullOrEmpty(formCollection["BanksId"]))
                ModelState.AddModelError("BankID", "Bank is required");
            else
            {
                newBankTransaction.BankID = formCollection["BanksId"].ToString();
                if (!string.IsNullOrEmpty(formCollection["AnotherBanksId"]))
                {
                    if (formCollection["BanksId"].Equals(formCollection["AnotherBanksId"]))
                        ModelState.AddModelError("BankID", "From A/C,To A/C number can't be same.");

                    newBankTransaction.AnotherBankID = formCollection["AnotherBanksId"].ToString();
                }

                if (newBankTransaction.TransactionType == EnumTransactionType.Withdraw
                     || newBankTransaction.TransactionType == EnumTransactionType.CashDelivery
                     || newBankTransaction.TransactionType == EnumTransactionType.FundTransfer
                     || newBankTransaction.TransactionType == EnumTransactionType.BankExpense
                     )
                {
                    var bank = _baseBankService.GetBankById(Convert.ToInt32(newBankTransaction.BankID));
                    var PrevTrans = _bankTransactionService.GetBankTransactionById(TransID);

                    decimal bankBalance = bank.TotalAmount;
                    if (PrevTrans != null)
                    {
                        if (PrevTrans.TransactionType == (int)EnumTransactionType.Withdraw
                           || PrevTrans.TransactionType == (int)EnumTransactionType.CashDelivery
                           || PrevTrans.TransactionType == (int)EnumTransactionType.FundTransfer
                           || PrevTrans.TransactionType == (int)EnumTransactionType.BankExpense
                           )
                        {
                            bankBalance += PrevTrans.Amount;
                        }
                    }


                    if (bank.IsAdvancedDueLimitApplicable == 1)
                    {
                        if (bank.TotalAmount - newBankTransaction.Amount < bank.AdvancedAmountLimit)
                            ModelState.AddModelError("Amount", "Loan limit is exceeding");
                    }
                    else
                    {

                        if (bankBalance < newBankTransaction.Amount)
                        {
                            ModelState.AddModelError("Amount", "Amount is not available");
                        }
                    }
                }
            }

            if (newBankTransaction.TransactionType == 0)
                ModelState.AddModelError("TransactionType", "TransactionType is required");
            else
            {
                if (newBankTransaction.TransactionType == EnumTransactionType.CashCollection)
                {
                    if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                        ModelState.AddModelError("CustomerID", "Customer is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.CashDelivery)
                {
                    if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                        ModelState.AddModelError("SupplierID", "Supplier is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.FundTransfer)
                {
                    if (string.IsNullOrEmpty(formCollection["AnotherBanksId"]))
                        ModelState.AddModelError("AnotherBankID", "Another Bank is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.BankExpense)
                {
                    if (string.IsNullOrEmpty(newBankTransaction.ExpenseItemID))
                        ModelState.AddModelError("ExpenseItemID", "Expense Item is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.BankIncome)
                {
                    if (string.IsNullOrEmpty(newBankTransaction.ExpenseItemID))
                        ModelState.AddModelError("IncomeItemID", "Income Item is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.LiaPay || newBankTransaction.TransactionType == EnumTransactionType.LiaRec)
                {
                    if (string.IsNullOrEmpty(newBankTransaction.SIHID))
                        ModelState.AddModelError("SIHID", "Investment Head is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.ReturnFromSupplier)
                {
                    if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                        ModelState.AddModelError("SupplierID", "Supplier is required");
                }
                else if (newBankTransaction.TransactionType == EnumTransactionType.CustomerCollectionReturn)
                {
                    if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                        ModelState.AddModelError("CustomerID", "Customer is required");
                }

            }

            if (!IsDateValid(Convert.ToDateTime(newBankTransaction.TranDate)))
                ModelState.AddModelError("TranDate", "Back dated entry is not valid");

            if (newBankTransaction.TransactionType == EnumTransactionType.BankExpense || newBankTransaction.TransactionType == EnumTransactionType.BankIncome)
            {
                if (string.IsNullOrEmpty(formCollection["Remarks"]))
                    ModelState.AddModelError("Remarks", "Remarks is required for" + newBankTransaction.TransactionType.GetDisplayName() + " save");

            }
        }

        private void MapFormCollectionValueWithNewEntity(CreateBankTransactionViewModel newBankTransaction,
            FormCollection formCollection)
        {
            newBankTransaction.BankID = formCollection["BanksId"] != "" ? formCollection["BanksId"] : "0";
            newBankTransaction.CustomerID = formCollection["CustomersId"] != "" ? formCollection["CustomersId"] : "0";
            newBankTransaction.SupplierID = formCollection["SuppliersId"] != "" ? formCollection["SuppliersId"] : "0";
            newBankTransaction.AnotherBankID = formCollection["AnotherBanksId"] != "" ? formCollection["AnotherBanksId"] : "0";
        }

        private void MapFormCollectionValueWithExistingEntity(BankTransaction bankTransaction,
            FormCollection formCollection)
        {
            bankTransaction.BankID = int.Parse(formCollection["BanksId"]);
            bankTransaction.TransactionType = int.Parse(formCollection["TransactionType"]);
        }

        [HttpGet]
        [Authorize]
        public ActionResult BankTransactionReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CommonBankTransactionReport()
        {
            ViewBag.ConcernId = new SelectList(_sisterConcernService.GetFamilyTreeDDL(User.Identity.GetConcernId()), "Id", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult BankLedger()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["BankTranID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var bankTransaction = _bankTransactionService.GetBankTransactionById(orderId);

            if (bankTransaction.WFStatus != EnumWFStatus.Pending)
            {
                AddToastMessage("", "Bank transaction is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            BankBalanceUpdate(bankTransaction, true);

            bankTransaction.WFStatus = EnumWFStatus.Approved;
            AddAuditTrail(bankTransaction, false);
            _bankTransactionService.UpdateBankTransaction(bankTransaction);
            _bankTransactionService.SaveBankTransaction();
            AddToastMessage("", "Item has been approved successfully.", ToastType.Success);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ApprovedIndex(int orderId)
        {
            var bankTransaction = _bankTransactionService.GetBankTransactionById(orderId);

            if (bankTransaction.WFStatus != EnumWFStatus.Pending)
            {
                AddToastMessage("", "Bank transaction is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            BankBalanceUpdate(bankTransaction, true);

            bankTransaction.WFStatus = EnumWFStatus.Approved;
            AddAuditTrail(bankTransaction, false);
            _bankTransactionService.UpdateBankTransaction(bankTransaction);
            _bankTransactionService.SaveBankTransaction();
            AddToastMessage("", "Bank transaction has been approved successfully.", ToastType.Success);

            return RedirectToAction("Index");
        }

    }
}