using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class ParentCategoryController : CoreController
    {
        IMapper _mapper;
        IParentCategoryService _ParentCategoryService;
        IMiscellaneousService<ParentCategory> _miscell;
        public ParentCategoryController(IErrorService errorService, IParentCategoryService ParentCategoryService, IMapper Mapper, IMiscellaneousService<ParentCategory> miscell, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _ParentCategoryService = ParentCategoryService;
            _mapper = Mapper;
            _miscell = miscell;
        }
        public ActionResult Index()
        {
            var pcategory = _ParentCategoryService.GetAll();
            var vmgrades = _mapper.Map<IEnumerable<ParentCategory>, IEnumerable<ParentCategoryViewModel>>(pcategory);
            return View(vmgrades);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var code = _miscell.GetUniqueKey(i => int.Parse(i.Code));
            return View(new ParentCategoryViewModel() { Code = code });
        }

        [HttpPost]
        public ActionResult Create(ParentCategoryViewModel NewParentCategory, FormCollection formcollection)
        {
            AddModelError(NewParentCategory);
            if (!ModelState.IsValid)
                return View(NewParentCategory);
            var pcategory = _mapper.Map<ParentCategoryViewModel, ParentCategory>(NewParentCategory);
            AddAuditTrail(pcategory, true);
            _ParentCategoryService.Add(pcategory);
            _ParentCategoryService.Save();
            AddToastMessage("", "ParentCategory Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void AddModelError(ParentCategoryViewModel NewParentCategory)
        {
            if (_ParentCategoryService.GetAll().Any(i => i.Name.ToLower().Equals(NewParentCategory.Name.ToLower()) && i.PCategoryID != NewParentCategory.PCategoryID))
            {
                ModelState.AddModelError("Name", "This Parent category is already exists.");
            }

            if(string.IsNullOrEmpty(NewParentCategory.Name))
                ModelState.AddModelError("Name", "This Parent category is required.");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _ParentCategoryService.Delete(id);
                _ParentCategoryService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ParentCategoryViewModel VMObj = new ParentCategoryViewModel();
            if (id != 0)
            {
                var obj = _ParentCategoryService.GetById(id);
                VMObj = _mapper.Map<ParentCategory, ParentCategoryViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(ParentCategoryViewModel ParentCategoryViewModel)
        {
            ParentCategoryViewModel VMObj = new ParentCategoryViewModel();

            AddModelError(ParentCategoryViewModel);
            if (!ModelState.IsValid)
                return View("Create",ParentCategoryViewModel);

            if (ParentCategoryViewModel.PCategoryID != 0)
            {
                var obj = _ParentCategoryService.GetById((int)ParentCategoryViewModel.PCategoryID);
                obj.Name = ParentCategoryViewModel.Name;
                AddAuditTrail(obj, false);
                _ParentCategoryService.Update(obj);
                _ParentCategoryService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetPCategoryByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var categories = from c in _ParentCategoryService.GetAll()
                                 where c.Name.ToLower().Contains(prefix.ToLower())
                                 select new
                                 {
                                     ID = c.PCategoryID,
                                     Name = c.Name
                                 };
                if (categories.Count() > 0)
                    return Json(categories, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var categories = (from c in _ParentCategoryService.GetAll()
                                  select new
                                  {
                                      ID = c.PCategoryID,
                                      Name = c.Name
                                  }).Take(10);
                if (categories.Count() > 0)
                    return Json(categories, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}