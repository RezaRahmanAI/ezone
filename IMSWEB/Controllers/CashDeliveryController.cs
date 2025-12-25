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

namespace IMSWEB
{
    [Authorize]
    [RoutePrefix("Collection-item")]

    public class CashDeliveryController : CoreController
    {
        ICashCollectionService _CashCollectionService;
        ICustomerService _CustomerService;
        ISupplierService _SupplierService;
        IMiscellaneousService<CashCollection> _miscellaneousService;
        IMapper _mapper;
        private readonly ISystemInformationService _systemInformationService;
        ISisterConcernService _sisterConcernService;
        IUserAuditDetailService _userAuditDetailService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IRoleService _roleService;
        private readonly ICashCollTranHistoryService _cashCollTranHistoryService;

        public CashDeliveryController(IErrorService errorService,
            ICashCollectionService cashCollectionService, ICustomerService customerService,
            ISupplierService supplierService, IMiscellaneousService<CashCollection> miscellaneousService, IUserAuditDetailService userAuditDetailService,
            IMapper mapper, ISystemInformationService systemInformationService, ISisterConcernService sisterConcernService, ISessionMasterService sessionMasterService, IRoleService roleService, ICashCollTranHistoryService cashCollTranHistoryService)
            : base(errorService, systemInformationService)
        {
            _CashCollectionService = cashCollectionService;
            _CustomerService = customerService;
            _SupplierService = supplierService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _systemInformationService = systemInformationService;
            _sisterConcernService = sisterConcernService;
            _userAuditDetailService = userAuditDetailService;
            _sessionMasterService = sessionMasterService;
            _roleService = roleService;
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
            List<EnumTranType> enumTranTypes = new List<EnumTranType>();
            enumTranTypes.Add(EnumTranType.ToCompany);
            enumTranTypes.Add(EnumTranType.DeliveryPending);
            enumTranTypes.Add(EnumTranType.DebitAdjustment);
            enumTranTypes.Add(EnumTranType.CreditAdjustment);
            enumTranTypes.Add(EnumTranType.CollectionReturn);
            var itemsAsync = _CashCollectionService.GetAllCashDelivaeryAsync(ViewBag.FromDate, ViewBag.ToDate, enumTranTypes);
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
            return View(vmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            List<EnumTranType> enumTranTypes = new List<EnumTranType>();
            enumTranTypes.Add(EnumTranType.ToCompany);
            enumTranTypes.Add(EnumTranType.DeliveryPending);
            enumTranTypes.Add(EnumTranType.DebitAdjustment);
            enumTranTypes.Add(EnumTranType.CreditAdjustment);
            enumTranTypes.Add(EnumTranType.CollectionReturn);
            var itemsAsync = _CashCollectionService.GetAllCashDelivaeryAsync(ViewBag.FromDate, ViewBag.ToDate, enumTranTypes);
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
            return View(vmodel);
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
        public ActionResult Create(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl)
        {
            AddModelError(newCashCollection, formCollection);

            if (!ModelState.IsValid)
                return View(newCashCollection);

            if (newCashCollection != null)
            {
                newCashCollection.CreateDate = DateTime.Today.ToString();
                newCashCollection.CreatedBy = (User.Identity.GetUserId<string>());
                newCashCollection.ConcernID = User.Identity.GetConcernId().ToString();

                newCashCollection.CustomerID = "0";
                newCashCollection.SupplierID = formCollection["SuppliersId"];

                var sysInfo = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                //if (_systemInformationService.IsApprovalSystemEnable())
                if (sysInfo.ApprovalSystemEnable == 1)
                    newCashCollection.TransactionType = EnumTranType.DeliveryPending;
                else
                    newCashCollection.TransactionType = EnumTranType.ToCompany;

                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.ToCompany)
                    newCashCollection.TransactionType = EnumTranType.ToCompany;
                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.DebitAdjustment)
                    newCashCollection.TransactionType = EnumTranType.DebitAdjustment;
                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.CreditAdjustment)
                    newCashCollection.TransactionType = EnumTranType.CreditAdjustment;

                if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.PaymentReturn)
                    newCashCollection.TransactionType = EnumTranType.CollectionReturn;

                newCashCollection.ModifiedDate = DateTime.Now.ToString();

                if (newCashCollection.AccountNo == null)
                    newCashCollection.AccountNo = "No A/C";

                var cashCollection = _mapper.Map<CreateCashCollectionViewModel, CashCollection>(newCashCollection);

                #region Total Due Update
                //var sysInfo = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                var oSupplier = _SupplierService.GetSupplierById(Convert.ToInt32(newCashCollection.SupplierID));

                if (sysInfo.ApprovalSystemEnable == 0)
                {
                    if (newCashCollection.TransactionType == EnumTranType.ToCompany)
                        oSupplier.TotalDue = (oSupplier.TotalDue + cashCollection.InterestAmt) - (Convert.ToDecimal(newCashCollection.Amount) + (Convert.ToDecimal(newCashCollection.AdjustAmt)));
                    else if (newCashCollection.TransactionType == EnumTranType.DebitAdjustment)
                        oSupplier.TotalDue = (oSupplier.TotalDue + cashCollection.InterestAmt) + (Convert.ToDecimal(newCashCollection.Amount) + (Convert.ToDecimal(newCashCollection.AdjustAmt)));
                    else if (newCashCollection.TransactionType == EnumTranType.CreditAdjustment)
                        oSupplier.TotalDue = (oSupplier.TotalDue + cashCollection.InterestAmt) - (Convert.ToDecimal(newCashCollection.Amount) + (Convert.ToDecimal(newCashCollection.AdjustAmt)));
                    else if (newCashCollection.TransactionType == EnumTranType.CollectionReturn)
                        oSupplier.TotalDue = (oSupplier.TotalDue + cashCollection.InterestAmt) + (Convert.ToDecimal(newCashCollection.Amount) + (Convert.ToDecimal(newCashCollection.AdjustAmt)));

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
                    useraudit.ObjectType = EnumObjectType.CashDelivery;
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
                    trasHistory.Value = "SupplierId-" + cashCollection.SupplierID + ", SupName-" + cashCollection.Supplier.Name + ", Amount-" + cashCollection.Amount +
                        ", AdjAmt-" + cashCollection.AdjustAmt + ", TransType-" + cashCollection.TransactionType + ", PayDate-" + cashCollection.EntryDate;
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
                    _SupplierService.UpdateSupplier(oSupplier);
                    _SupplierService.SaveSupplier();
                }



                //_CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(newCashCollection.SupplierID), 0, 0, Convert.ToDecimal(Convert.ToDecimal(newCashCollection.Amount) + Convert.ToDecimal(newCashCollection.AdjustAmt)));
                TempData["CashCollectionID"] = cashCollection.CashCollectionID;

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }
        private void AddModelError(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("SupplierID", "Supplier is Required.");
            else
                newCashCollection.SupplierID = formCollection["SuppliersId"];

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");
            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
                ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

            if (!IsDateValid(Convert.ToDateTime(newCashCollection.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }
            if (newCashCollection.DeliveryType == 0)
                ModelState.AddModelError("DeliveryType", "Trans. Type is Required.");
        }




        private void AddModelErrorForEdit(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("SupplierID", "Supplier is Required.");
            else
                newCashCollection.SupplierID = formCollection["SuppliersId"];

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");
            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
                ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

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

            var vmodel = _mapper.Map<CashCollection, CreateCashCollectionViewModel>(cashCollection);
            var Supplier = _SupplierService.GetSupplierById(Convert.ToInt32(vmodel.SupplierID));
            vmodel.CurrentDue = Supplier.TotalDue.ToString();
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
                cashCollection.AdjustAmt = decimal.Parse(newCashCollection.AdjustAmt);
                cashCollection.AccountNo = newCashCollection.AccountNo;
                cashCollection.MBAccountNo = newCashCollection.MBAccountNo;
                cashCollection.BKashNo = newCashCollection.BKashNo;
                //if (newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.ToCompany)
                //    newCashCollection.TransactionType = EnumTranType.ToCompany;
                //else if (newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.DebitAdjustment)
                //    newCashCollection.TransactionType = EnumTranType.DebitAdjustment;
                //else if (newCashCollection.DeliveryType == EnumDeliveryDropdownTranType.CreditAdjustment)
                //    newCashCollection.TransactionType = EnumTranType.CreditAdjustment;

                //cashCollection.TransactionType = newCashCollection.TransactionType;
                cashCollection.CustomerID = 0;
                cashCollection.SupplierID = int.Parse(formCollection["SuppliersId"]);
                cashCollection.InterestAmt = decimal.Parse(GetDefaultIfNull(newCashCollection.InterestAmt));

                if (cashCollection.TransactionType == EnumTranType.CollectionReturn || cashCollection.TransactionType == EnumTranType.DebitAdjustment)
                {
                    _CashCollectionService.UpdateTotalDueWhenEditReturnType(0, (int)cashCollection.SupplierID, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt - cashCollection.InterestAmt));
                }
                else
                {
                    _CashCollectionService.UpdateTotalDueWhenEdit(0, (int)cashCollection.SupplierID, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt - cashCollection.InterestAmt));
                }

                cashCollection.BalanceDue = _SupplierService.GetSupplierById((int)cashCollection.SupplierID).TotalDue;


                _CashCollectionService.UpdateCashCollection(cashCollection);
                _CashCollectionService.SaveCashCollection();

                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = int.Parse(newCashCollection.CashCollectionID);
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.CashDelivery;
                useraudit.ActionType = EnumActionType.Edit;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                var supp = _SupplierService.GetSupplierById(int.Parse(newCashCollection.SupplierID));

                CashCollTranHistory trasHistory = new CashCollTranHistory();
                trasHistory.CashCollectionID = int.Parse(newCashCollection.CashCollectionID);
                trasHistory.ReceiptNo = newCashCollection.ReceiptNo;
                trasHistory.CreateOrEdit = "Edit";
                trasHistory.Value = "SupplierId-" + newCashCollection.SupplierID + ", SupName-" + supp.Name + ", Amount-" + newCashCollection.Amount + ", AdjAmt-" + newCashCollection.AdjustAmt + ", TransType-" + cashCollection.TransactionType + ", PayDate-" + newCashCollection.EntryDate;
                trasHistory.ConcernID = User.Identity.GetConcernId();
                trasHistory.CreateOrEditBy = cashCollection.CreatedBy;
                trasHistory.HistoryDate = GetLocalDateTime();
                _cashCollTranHistoryService.Add(trasHistory);
                _cashCollTranHistoryService.Save();

                TempData["CashCollectionID"] = cashCollection.CashCollectionID;
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
            var CashDelivery = _CashCollectionService.GetCashCollectionById(id);
            decimal Amount = 0m, AdjAmt = 0m, IntAmt = 0m;
            Amount = CashDelivery.Amount;
            AdjAmt = CashDelivery.AdjustAmt;
            IntAmt = CashDelivery.InterestAmt;

            if (!IsDateValid(Convert.ToDateTime(CashDelivery.EntryDate)))
            {
                return RedirectToAction("Index");
            }

            var oCustomer = _SupplierService.GetSupplierById(Convert.ToInt32(CashDelivery.SupplierID));
            if (CashDelivery.TransactionType == EnumTranType.ToCompany)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt) - IntAmt;
            else if (CashDelivery.TransactionType == EnumTranType.CollectionReturn)
                oCustomer.TotalDue = oCustomer.TotalDue - (Amount + AdjAmt);
            else if (CashDelivery.TransactionType == EnumTranType.DebitAdjustment)
                oCustomer.TotalDue = oCustomer.TotalDue - (Amount + AdjAmt);
            else if (CashDelivery.TransactionType == EnumTranType.CreditAdjustment)
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
                _SupplierService.UpdateSupplier(oCustomer);
                _SupplierService.SaveSupplier();
            }
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }



        //[HttpGet]
        //[Authorize]
        //[Route("delete/{id}")]
        //public ActionResult Delete(int id)
        //{
        //    var CashDelivery = _CashCollectionService.GetCashCollectionById(id);
        //    int SupplierID = (int)CashDelivery.SupplierID;
        //    decimal amt = CashDelivery.Amount + CashDelivery.AdjustAmt;
        //    decimal InsAmt = 0m;
        //    InsAmt = CashDelivery.InterestAmt;
        //    if (!IsDateValid(Convert.ToDateTime(CashDelivery.EntryDate)))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    _CashCollectionService.DeleteCashCollection(id);
        //    _CashCollectionService.SaveCashCollection();
        //    if (CashDelivery.TransactionType == EnumTranType.ToCompany)
        //    {
        //        _CashCollectionService.UpdateTotalDue(0, SupplierID, 0, 0, (-amt - InsAmt));

        //    }

        //    UserAuditDetail useraudit = new UserAuditDetail();
        //    useraudit.ObjectID = id;
        //    useraudit.ActivityDtTime = GetLocalDateTime();
        //    useraudit.ObjectType = EnumObjectType.CashDelivery;
        //    useraudit.ActionType = EnumActionType.Delete;
        //    useraudit.ConcernID = User.Identity.GetConcernId();
        //    useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
        //    useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
        //    _userAuditDetailService.Add(useraudit);
        //    _userAuditDetailService.Save();


        //    AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        [Authorize]
        public ActionResult CashDeliveryReport()
        {
            return View("CashDeliveryReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminCashDeliveryReport()
        {
            ViewBag.Concerns = new SelectList(_sisterConcernService.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["CashCollectionID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var CashCollection = _CashCollectionService.GetCashCollectionById(orderId);
            if (CashCollection.TransactionType != EnumTranType.DeliveryPending)
            {
                AddToastMessage("", "Cash delivery is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            _CashCollectionService.UpdateTotalDue(0, (int)CashCollection.SupplierID, 0, 0, CashCollection.Amount);
            CashCollection.TransactionType = EnumTranType.ToCompany;
            AddAuditTrail(CashCollection, false);
            _CashCollectionService.UpdateCashCollection(CashCollection);
            _CashCollectionService.SaveCashCollection();

            UserAuditDetail useraudit = new UserAuditDetail();
            useraudit.ObjectID = orderId;
            useraudit.ActivityDtTime = GetLocalDateTime();
            useraudit.ObjectType = EnumObjectType.CashDelivery;
            useraudit.ActionType = EnumActionType.Approved;
            useraudit.ConcernID = User.Identity.GetConcernId();
            useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
            useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
            _userAuditDetailService.Add(useraudit);
            _userAuditDetailService.Save();
            AddToastMessage("", "Item has been approved successfully.", ToastType.Success);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }


}
