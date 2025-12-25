using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("expense-item")]
    public class ExpenseItemController : CoreController
    {
        IExpenseItemService _expenseItemService;
        IMiscellaneousService<ExpenseItem> _miscellaneousService;
        ISystemInformationService _SysInfoService;
        IMapper _mapper;
        private readonly ISisterConcernService _sisterConcernService;

        public ExpenseItemController(IErrorService errorService,
            IExpenseItemService expenseItemService, IMiscellaneousService<ExpenseItem> miscellaneousService, ISystemInformationService SysInfoService, IMapper mapper, ISisterConcernService sisterConcernService)
            : base(errorService, SysInfoService)
        {
            _expenseItemService = expenseItemService;
            _miscellaneousService = miscellaneousService;
            _SysInfoService = SysInfoService;
            _mapper = mapper;
            _sisterConcernService = sisterConcernService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var itemsAsync = _expenseItemService.GetAllExpenseItemAsync();
            var vmodel = _mapper.Map<IEnumerable<ExpenseItem>, IEnumerable<CreateExpenseItemViewModel>>(await itemsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => x.ExpenseItemID);
            return View(new CreateExpenseItemViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateExpenseItemViewModel newExpenseItem, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newExpenseItem);

            if (newExpenseItem != null)
            {
                var existingItem = _miscellaneousService.GetDuplicateEntry(e => e.Description == newExpenseItem.Name);
                if (existingItem != null)
                {
                    AddToastMessage("", "An Expense Item with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newExpenseItem);
                }
               
                var expenseItem = _mapper.Map<CreateExpenseItemViewModel, ExpenseItem>(newExpenseItem);
              
                AddAuditTrail(expenseItem, true);
                _expenseItemService.AddExpenseItem(expenseItem);
                _expenseItemService.SaveExpenseItem();

                var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                if (SystemInfo.IsExpenseHeadEnable == 1)
                {
                    AddToSisterConcern(newExpenseItem);
                }

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var expenseItem = _expenseItemService.GetExpenseItemById(id);
            var vmodel = _mapper.Map<ExpenseItem, CreateExpenseItemViewModel>(expenseItem);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateExpenseItemViewModel newExpenseItem, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newExpenseItem);

            if (newExpenseItem != null)
            {
                var expenseItem = _expenseItemService.GetExpenseItemById(int.Parse(newExpenseItem.Id));
                AddAuditTrail(expenseItem, false);
                expenseItem.Code = newExpenseItem.Code;
                expenseItem.Description = newExpenseItem.Name;
                expenseItem.Status = newExpenseItem.Status;

                _expenseItemService.UpdateExpenseItem(expenseItem);
                _expenseItemService.SaveExpenseItem();

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
            _expenseItemService.DeleteExpenseItem(id);
            _expenseItemService.SaveExpenseItem();
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        #region Add expense/incom to sister concern
        private void AddToSisterConcern(CreateExpenseItemViewModel expenseItem)
        {
            List<SisterConcern> families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                    .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                ExpenseItem expense = new ExpenseItem();
                foreach (var concern in families)
                {
                    if (_expenseItemService.GetAll(concern.ConcernID)
                        .Any(i => i.Description.ToLower().Equals(expenseItem.Name.Trim().ToLower())))
                        continue;

                    var newExpenseItem = _mapper.Map<CreateExpenseItemViewModel, ExpenseItem>(expenseItem);

                    AddAuditTrail(newExpenseItem, true);
                    newExpenseItem.ConcernID = concern.ConcernID;
                    _expenseItemService.AddExpenseItem(newExpenseItem);
                    _expenseItemService.SaveExpenseItem();
                }
            }
        }
        #endregion
    }
}