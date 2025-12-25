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
    [RoutePrefix("Designation")]
    public class DesignationController : CoreController
    {
        IDesignationService _designationService;
        IMiscellaneousService<Designation> _miscellaneousService;
        IMapper _mapper;
        IDesWiseCommissionService _DesWiseCommissionService;
        public DesignationController(IErrorService errorService,
            IDesignationService designationService, IMiscellaneousService<Designation> miscellaneousService,
            IDesWiseCommissionService DesWiseCommissionService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _designationService = designationService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _DesWiseCommissionService = DesWiseCommissionService;
        }

        #region Designations
        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var designationsAsync = _designationService.GetAllDesignationAsync();
            var vmodel = _mapper.Map<IEnumerable<Designation>, IEnumerable<CreateDesignationViewModel>>(await designationsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateDesignationViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateDesignationViewModel newDesignation, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newDesignation);

            if (newDesignation != null)
            {
                var existingDesig = _miscellaneousService.GetDuplicateEntry(d => d.Description == newDesignation.Name);
                if (existingDesig != null)
                {
                    AddToastMessage("", "A Designation with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newDesignation);
                }

                var designation = _mapper.Map<CreateDesignationViewModel, Designation>(newDesignation);

                _designationService.AddDesignation(designation);
                _designationService.SaveDesignation();

                AddToastMessage("", "Designation has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Designation data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var designation = _designationService.GetDesignationById(id);
            var vmodel = _mapper.Map<Designation, CreateDesignationViewModel>(designation);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateDesignationViewModel newDesignation, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newDesignation);

            if (newDesignation != null)
            {
                var designation = _designationService.GetDesignationById(int.Parse(newDesignation.Id));

                designation.Code = newDesignation.Code;
                designation.Description = newDesignation.Name;
                _designationService.UpdateDesignation(designation);
                _designationService.SaveDesignation();

                AddToastMessage("", "Designation has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Designation data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _designationService.DeleteDesignation(id);
            _designationService.SaveDesignation();
            AddToastMessage("", "Designation has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
#endregion

        #region Desg wise commissions setup
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> DESwiseCommission()
        {
            var DesCommision = _DesWiseCommissionService.GetAllAsync();
            var VMData = _mapper.Map<IEnumerable<Tuple<int, Decimal, string>>, IEnumerable<DesWiseCommissionViewModel>>(await DesCommision);
            return View(VMData);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateDESwiseCommission()
        {
            ViewBag.Designations = new SelectList(_designationService.GetAllDesignation(), "DesignationID", "Description");
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateDESwiseCommission(DesWiseCommissionViewModel newDesWiseCommission, string returnUrl)
        {
            ViewBag.Designations = new SelectList(_designationService.GetAllDesignation(), "DesignationID", "Description");

            if (!ModelState.IsValid)
                return View(newDesWiseCommission);

            if (newDesWiseCommission != null)
            {
                var existingDesig = _DesWiseCommissionService.GetAllIQueryable().FirstOrDefault(d => d.DesignationID == newDesWiseCommission.DesignationID);
                if (existingDesig != null)
                {
                    AddToastMessage("", "This designation Commission already exists in the system. Please try with a different designation.", ToastType.Error);
                    return View(newDesWiseCommission);
                }

                var designation = _mapper.Map<DesWiseCommissionViewModel, DesWiseCommission>(newDesWiseCommission);
                AddAuditTrail(designation, true);
                _DesWiseCommissionService.Add(designation);
                _DesWiseCommissionService.Save();

                AddToastMessage("", "Commission has been saved successfully.", ToastType.Success);
                return RedirectToAction("DESwiseCommission");
            }
            else
            {
                AddToastMessage("", "No Commission data found to create.", ToastType.Error);
                return RedirectToAction("DESwiseCommission");
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult DeleteDesWiseComm(int id)
        {
            _DesWiseCommissionService.Delete(id);
            _DesWiseCommissionService.Save();
            AddToastMessage("", "Commission has been deleted successfully.", ToastType.Success);
            return RedirectToAction("DESwiseCommission");
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditDesWiseComm(int id)
        {
            ViewBag.Designations = new SelectList(_designationService.GetAllDesignation(), "DesignationID", "Description");
            var designation = _DesWiseCommissionService.GetById(id);
            var vmodel = _mapper.Map<DesWiseCommission, DesWiseCommissionViewModel>(designation);
            return View("CreateDESwiseCommission", vmodel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditDesWiseComm(DesWiseCommissionViewModel newDesWiseCommission, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("CreateDESwiseCommission", newDesWiseCommission);

            if (newDesWiseCommission != null)
            {
                var commission = _DesWiseCommissionService.GetById(int.Parse(newDesWiseCommission.ID));

                commission.CommissionPercent = newDesWiseCommission.CommissionPercent;
                commission.DesignationID = newDesWiseCommission.DesignationID;
                commission.ConcernID = User.Identity.GetConcernId();
                AddAuditTrail(commission, false);
                _DesWiseCommissionService.Update(commission);
                _DesWiseCommissionService.Save();

                AddToastMessage("", "Commission has been updated successfully.", ToastType.Success);
                return RedirectToAction("DESwiseCommission");
            }
            else
            {
                AddToastMessage("", "No Commission data found to update.", ToastType.Error);
                return RedirectToAction("DESwiseCommission");
            }
        }
        #endregion
    }
}