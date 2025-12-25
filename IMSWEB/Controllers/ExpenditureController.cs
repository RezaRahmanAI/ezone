using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("expense-item")]
    public class ExpenditureController : CoreController
    {
        IExpenditureService _expenditureService;
        IExpenseItemService _expenseItemService;
        IMapper _mapper;
        ISisterConcernService _SisterConcernService;
        IStockService _stockService;
        IMiscellaneousService<Expenditure> _miscellService;
        ISystemInformationService _SystemInformationService;
        IUserAuditDetailService _userAuditDetailService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IRoleService _roleService;
        public ExpenditureController(IErrorService errorService,
            IExpenditureService expenditureService, IExpenseItemService expenseItemService, IMapper mapper, IMiscellaneousService<Expenditure> miscellService,
            ISisterConcernService SisterConcernService, IUserAuditDetailService userAuditDetailService,
            IStockService stockService, ISystemInformationService systemInformationService, ISessionMasterService sessionMasterService, IRoleService roleService)
            : base(errorService, systemInformationService)
        {
            _expenditureService = expenditureService;
            _SisterConcernService = SisterConcernService;
            _expenseItemService = expenseItemService;
            _mapper = mapper;
            _miscellService = miscellService;
            _stockService = stockService;
            _SystemInformationService = systemInformationService;
            _userAuditDetailService = userAuditDetailService;
            _sessionMasterService = sessionMasterService;
            _roleService = roleService;
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
                var expenditureList = _expenditureService.GetAllExpenditureByUserIDAsync(User.Identity.GetUserId<int>(), ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
            else
            {
                List<EnumWFStatus> enumWFStatus = new List<EnumWFStatus>();
                enumWFStatus.Add(EnumWFStatus.Approved);
                enumWFStatus.Add(EnumWFStatus.Pending);
                var expenditureList = _expenditureService.GetAllExpenditureAsync(ViewBag.FromDate, ViewBag.ToDate, enumWFStatus);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
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
                var expenditureList = _expenditureService.GetAllExpenditureByUserIDAsync(User.Identity.GetUserId<int>(), ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
            else
            {
                List<EnumWFStatus> enumWFStatus = new List<EnumWFStatus>();
                enumWFStatus.Add(EnumWFStatus.Approved);
                enumWFStatus.Add(EnumWFStatus.Pending);
                var expenditureList = _expenditureService.GetAllExpenditureAsync(ViewBag.FromDate, ViewBag.ToDate, enumWFStatus);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsEmobileCustomerView = _SystemInformationService.IsEmobileCustomerView();
            var voucherNo = _miscellService.GetUniqueKey(i => i.ExpenditureID);
            return View(new CreateExpenditureViewModel() { VoucherNo = voucherNo });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateExpenditureViewModel newExpenditure, FormCollection formcollection, string returnUrl)
        {
            ViewBag.IsEmobileCustomerView = _SystemInformationService.IsEmobileCustomerView();
            CheckAndAddModelError(newExpenditure, formcollection);

            if (!ModelState.IsValid)
                return View(newExpenditure);

            if (newExpenditure != null)
            {
                newExpenditure.CreateDate = DateTime.Today.ToString();
                newExpenditure.CreatedBy = (User.Identity.GetUserId<string>());
                newExpenditure.ConcernID = User.Identity.GetConcernId().ToString();
                var expenditure = _mapper.Map<CreateExpenditureViewModel, Expenditure>(newExpenditure);
                expenditure.CashInHandReportStatus = newExpenditure.CashInHandReportStatus ? 1 : 0;
                if (_SystemInformationService.IsApprovalSystemEnable())
                    expenditure.WFStatus = EnumWFStatus.Pending;
                else
                    expenditure.WFStatus = EnumWFStatus.Approved;

                _expenditureService.AddExpenditure(expenditure);
                _expenditureService.SaveExpenditure();

                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = expenditure.ExpenditureID;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.Expense;
                useraudit.ActionType = EnumActionType.Add;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                TempData["ExpenditureID"] = expenditure.ExpenditureID;
                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        private void CheckAndAddModelError(CreateExpenditureViewModel newExpenditure, FormCollection formcollection)
        {
            if (!string.IsNullOrEmpty(formcollection["EntryDate"]))
                newExpenditure.EntryDate = formcollection["EntryDate"].ToString();

            if (!IsDateValid(Convert.ToDateTime(newExpenditure.EntryDate)))
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid");

            if (string.IsNullOrEmpty(newExpenditure.VoucherNo))
                ModelState.AddModelError("VoucherNo", "VoucherNo is required.");

            if(string.IsNullOrEmpty(newExpenditure.ExpenseItemID))
                ModelState.AddModelError("ExpenseItemID", "Head is required.");
            int EXID = Convert.ToInt32(GetDefaultIfNull(newExpenditure.Id));
            if(_miscellService.GetDuplicateEntry(i=>i.VoucherNo.Equals(newExpenditure.VoucherNo)
            && i.ExpenditureID!= EXID) !=null)
            {
                newExpenditure.VoucherNo=_miscellService.GetUniqueKey(i => i.ExpenditureID);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            ViewBag.IsEmobileCustomerView = _SystemInformationService.IsEmobileCustomerView();
            var expenditure = _expenditureService.GetExpenditureById(id);
            if (!IsDateValid(expenditure.EntryDate))
                return RedirectToAction("Index");

            var vmodel = _mapper.Map<Expenditure, CreateExpenditureViewModel>(expenditure);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateExpenditureViewModel newExpenditure, FormCollection formcollection, string returnUrl)
        {
            ViewBag.IsEmobileCustomerView = _SystemInformationService.IsEmobileCustomerView();
            CheckAndAddModelError(newExpenditure, formcollection);

            if (!ModelState.IsValid)
                return View("Create", newExpenditure);

            if (newExpenditure != null)
            {
                var expenditure = _expenditureService.GetExpenditureById(int.Parse(newExpenditure.Id));

                expenditure.Amount = decimal.Parse(newExpenditure.Amount);
                expenditure.Purpose = newExpenditure.Purpose;
                expenditure.ExpenseItemID = int.Parse(newExpenditure.ExpenseItemID);
                expenditure.ModifiedBy = User.Identity.GetUserId<int>();
                expenditure.ModifiedDate = DateTime.Now;
                expenditure.CustomerType = newExpenditure.CustomerType;
                expenditure.EntryDate = Convert.ToDateTime(newExpenditure.EntryDate);
                _expenditureService.UpdateExpenditure(expenditure);
                _expenditureService.SaveExpenditure();

                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = expenditure.ExpenditureID;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.Expense;
                useraudit.ActionType = EnumActionType.Edit;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                TempData["ExpenditureID"] = expenditure.ExpenditureID;
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
            var model = _expenditureService.GetExpenditureById(id);
            if (!IsDateValid(model.EntryDate))
            {
                return RedirectToAction("Index");
            }
            _expenditureService.DeleteExpenditure(id);
            _expenditureService.SaveExpenditure();

            UserAuditDetail useraudit = new UserAuditDetail();
            useraudit.ObjectID = id;
            useraudit.ActivityDtTime = GetLocalDateTime();
            useraudit.ObjectType = EnumObjectType.Expense;
            useraudit.ActionType = EnumActionType.Delete;
            useraudit.ConcernID = User.Identity.GetConcernId();
            useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
            useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
            _userAuditDetailService.Add(useraudit);
            _userAuditDetailService.Save();

            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        [Route("Expenditure-report")]
        public ActionResult MiscellaneousReport()
        {
            return View("MiscellaneousReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult IncomeReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["ExpenditureID"] = id;
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public ActionResult TrialBalance()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProfitLossAccount()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }





        [HttpGet]
        [Authorize]
        public ActionResult BalanceSheet()
        {
            _stockService.SaveStockValue(User.Identity.GetConcernId());
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult BalanceSheetNew()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }

        void PopulateConcernsDropdown()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetFamilyTree(User.Identity.GetConcernId()), "ConcernID", "Name");
        }
        [HttpGet]
        public ActionResult AdminExpenseReport()
        {
            PopulateConcernsDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminBalanceSheetNew()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()) || User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.LocalAdmin.ToString())
                || User.IsInRole(EnumUserRoles.hoqueLocalAdmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult AdminTrialBalance()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()) || User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.LocalAdmin.ToString())
                || User.IsInRole(EnumUserRoles.hoqueLocalAdmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult AdminProfitLossAccount()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()) || User.IsInRole(EnumUserRoles.Admin.ToString()) ||
                User.IsInRole(EnumUserRoles.AdminReport.ToString()) || 
                User.IsInRole(EnumUserRoles.LocalAdmin.ToString())
                || User.IsInRole(EnumUserRoles.hoqueLocalAdmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }
        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var expenditure = _expenditureService.GetExpenditureById(orderId);
            if (expenditure.WFStatus != EnumWFStatus.Pending)
            {
                AddToastMessage("", "Item is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            expenditure.WFStatus = EnumWFStatus.Approved;
            AddAuditTrail(expenditure, false);
            _expenditureService.UpdateExpenditure(expenditure);
            _expenditureService.SaveExpenditure();

            UserAuditDetail useraudit = new UserAuditDetail();
            useraudit.ObjectID = orderId;
            useraudit.ActivityDtTime = GetLocalDateTime();
            useraudit.ObjectType = EnumObjectType.Expense;
            useraudit.ActionType = EnumActionType.Approved;
            useraudit.ConcernID = User.Identity.GetConcernId();
            useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
            useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
            _userAuditDetailService.Add(useraudit);
            _userAuditDetailService.Save();

            return Json(true, JsonRequestBehavior.AllowGet);

        }
    }
}