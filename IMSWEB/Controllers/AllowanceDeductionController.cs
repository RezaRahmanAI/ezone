using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class AllowanceDeductionController : CoreController
    {
        IMapper _mapper;
        IAllowanceDeductionService _AllowanceDeductionService;
        IMiscellaneousService<AllowanceDeduction> _miscell;
        IADParameterBasicService _ADParameterBasicService;
        public AllowanceDeductionController(IErrorService errorService, IAllowanceDeductionService AllowanceDeductionService,
            IMapper Mapper, IMiscellaneousService<AllowanceDeduction> miscell, IADParameterBasicService ADParameterBasicService, ISystemInformationService sysInfoService
            )
            : base(errorService, sysInfoService)
        {
            _AllowanceDeductionService = AllowanceDeductionService;
            _mapper = Mapper;
            _miscell = miscell;
            _ADParameterBasicService = ADParameterBasicService;
        }

        #region AllowanceDeduction
        public async Task<ActionResult> Index()
        {
            var allowaces = _AllowanceDeductionService.GetAllAllowacneAsync();
            var vmallowacne = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(await allowaces);
            return View(vmallowacne);
        }
        [HttpGet]
        public async Task<ActionResult> Deductions()
        {
            var allowaces = _AllowanceDeductionService.GetAllDeductionAsync();
            var vmallowacne = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(await allowaces);
            return View(vmallowacne);
        }
        [HttpGet]
        public ActionResult Create(bool IsAllowance)
        {
            var code = _miscell.GetUniqueKey(i => (int)i.AllowDeductID);
            ViewBag.Title = IsAllowance == true ? "Create Allowance" : "Create Deduction";
            return View(new AllowanceDeductionViewModel() { Code = code, AllowORDeduct = IsAllowance == true ? (int)EnumAllowOrDeduct.Allowance : (int)EnumAllowOrDeduct.Deduction });
        }

        private void AddModelError(AllowanceDeductionViewModel allowDeductViewmodel)
        {
            if (_AllowanceDeductionService.GetAll().Any(i => i.Name.ToLower().Contains(allowDeductViewmodel.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", "This name already exists.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AllowanceDeductionViewModel allowDeductViewmodel)
        {
            AddModelError(allowDeductViewmodel);

            if (!ModelState.IsValid)
                return View(allowDeductViewmodel);

            allowDeductViewmodel.CreatedBy = (User.Identity.GetUserId<int>());
            allowDeductViewmodel.CreateDate = DateTime.Now;
            allowDeductViewmodel.Status = (int)EnumActiveInactive.Active;
            var allow = _mapper.Map<AllowanceDeductionViewModel, AllowanceDeduction>(allowDeductViewmodel);
            _AllowanceDeductionService.Add(allow);
            _AllowanceDeductionService.Save();
            AddToastMessage("", "Allowance Save Successfully", ToastType.Success);

            if (allowDeductViewmodel.AllowORDeduct == (int)EnumAllowOrDeduct.Allowance)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Deductions");
        }

        [HttpGet]
        public ActionResult Delete(int id, bool IsAllowance)
        {
            if (id != 0)
            {
                _AllowanceDeductionService.Delete(id);
                _AllowanceDeductionService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            if (IsAllowance == true)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Deductions");
        }
        [HttpGet]
        public ActionResult Active(int id, bool IsAllowance)
        {
            if (id != 0)
            {
                var obj = _AllowanceDeductionService.GetById(id);
                obj.Status = (int)EnumActiveInactive.Active; ;
                _AllowanceDeductionService.Update(obj);
                _AllowanceDeductionService.Save();
                AddToastMessage("", "Active Successfully", ToastType.Success);

            }
            if (IsAllowance == true)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Deductions");
        }

        [HttpGet]
        public ActionResult Inactive(int id, bool IsAllowance)
        {
            if (id != 0)
            {
                var obj = _AllowanceDeductionService.GetById(id);
                obj.Status = (int)EnumActiveInactive.InActive; ;
                _AllowanceDeductionService.Update(obj);
                _AllowanceDeductionService.Save();
                AddToastMessage("", "Inactive Successfully", ToastType.Success);

            }
            if (IsAllowance == true)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Deductions");
        }

        [HttpGet]
        public ActionResult Edit(int id, bool IsAllowance)
        {
            AllowanceDeductionViewModel VMObj = new AllowanceDeductionViewModel();
            if (id != 0)
            {
                var obj = _AllowanceDeductionService.GetById(id);
                VMObj = _mapper.Map<AllowanceDeduction, AllowanceDeductionViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(AllowanceDeductionViewModel allowDeductViewmodel)
        {
            AllowanceDeductionViewModel VMObj = new AllowanceDeductionViewModel();
            if (allowDeductViewmodel.AllowDeductID != 0)
            {
                var obj = _AllowanceDeductionService.GetById(allowDeductViewmodel.AllowDeductID);
                obj.Name = allowDeductViewmodel.Name;
                obj.ModifiedBy = User.Identity.GetUserId<int>();
                obj.ModifiedDate = DateTime.Now;
                _AllowanceDeductionService.Update(obj);
                _AllowanceDeductionService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            if (allowDeductViewmodel.AllowORDeduct == (int)EnumAllowOrDeduct.Allowance)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Deductions");
        }

        #endregion

    }
}