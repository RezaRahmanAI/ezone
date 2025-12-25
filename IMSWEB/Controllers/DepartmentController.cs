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
    public class DepartmentController : CoreController
    {
        IDepartmentService _DepartmentService;
        IMiscellaneousService<Department> _miscellaneousService;
        IMapper _mapper;

        public DepartmentController(IErrorService errorService,
            IDepartmentService DepartmentService, IMiscellaneousService<Department> miscellaneousService,
            IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _DepartmentService = DepartmentService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }
        public async Task<ActionResult> Index()
        {
            var departments = await _DepartmentService.GetAllDepartmentAsync();
            var vmdepartments = _mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);
            return View(vmdepartments);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.CODE));
            return View(new DepartmentViewModel { CODE = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(DepartmentViewModel newDepartment, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newDepartment);

            if (newDepartment != null)
            {

                var existingColor = _miscellaneousService.GetDuplicateEntry(c => c.DESCRIPTION == newDepartment.DESCRIPTION);
                if (existingColor != null)
                {
                    AddToastMessage("", "A Department with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newDepartment);
                }

                newDepartment.Status = EnumActiveInactive.Active;
                var model = _mapper.Map<DepartmentViewModel, Department>(newDepartment);
                model.CreateDate = DateTime.Now;
                model.CreatedBy = User.Identity.GetUserId<int>();
                _DepartmentService.AddDepartment(model);
                _DepartmentService.SaveDepartment();

                AddToastMessage("", "Department has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Department data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var model = _DepartmentService.GetDepartmentById(id);
            var vmodel = _mapper.Map<Department, DepartmentViewModel>(model);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(DepartmentViewModel newDepartment, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newDepartment);

            if (newDepartment != null)
            {
                var model = _DepartmentService.GetDepartmentById(newDepartment.DepartmentId);

                model.CODE = newDepartment.CODE;
                model.DESCRIPTION = newDepartment.DESCRIPTION;
                model.ModifiedBy = User.Identity.GetUserId<int>();
                model.ModifiedDate = DateTime.Now;
                _DepartmentService.UpdateDepartment(model);
                _DepartmentService.SaveDepartment();

                AddToastMessage("", "Department has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Department data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _DepartmentService.DeleteDepartment(id);
            _DepartmentService.SaveDepartment();
            AddToastMessage("", "Department has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}