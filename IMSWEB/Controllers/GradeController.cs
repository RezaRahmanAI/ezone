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
    public class GradeController : CoreController
    {
        IMapper _mapper;
        IGradeService _GradeService;
        IMiscellaneousService<Grade> _miscell;
        public GradeController(IErrorService errorService, IGradeService GradeService, IMapper Mapper, IMiscellaneousService<Grade> miscell, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _GradeService = GradeService;
            _mapper = Mapper;
            _miscell = miscell;
        }
        public async Task<ActionResult> Index()
        {
            var grades = _GradeService.GetAllAsync();
            var vmgrades = _mapper.Map<IEnumerable<Grade>, IEnumerable<GradeViewModel>>(await grades);
            return View(vmgrades);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var code = _miscell.GetUniqueKey(i => (int)i.GradeID);
            return View(new GradeViewModel() { Code = code });
        }

        [HttpPost]
        public ActionResult Create(GradeViewModel grade, FormCollection formcollection)
        {

            if (!ModelState.IsValid)
                return View(grade);
            grade.CreatedBy = (User.Identity.GetUserId<int>());
            grade.UserID = (User.Identity.GetUserId<int>());
            grade.CreateDate = DateTime.Now;
            grade.Status = (int)EnumActiveInactive.Active;
            grade.PayrollTypeID = 1;
            var mgrade = _mapper.Map<GradeViewModel, Grade>(grade);
            _GradeService.Add(mgrade);
            _GradeService.Save();
            AddToastMessage("", "Grade Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _GradeService.Delete(id);
                _GradeService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Active(int id)
        {
            if (id != 0)
            {
                var obj = _GradeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.Active; ;
                _GradeService.Update(obj);
                _GradeService.Save();
                AddToastMessage("", "Active Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Inactive(int id)
        {
            if (id != 0)
            {
                var obj = _GradeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.InActive; ;
                _GradeService.Update(obj);
                _GradeService.Save();
                AddToastMessage("", "Inactive Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            GradeViewModel VMObj = new GradeViewModel();
            if (id != 0)
            {
                var obj = _GradeService.GetById(id);
                VMObj = _mapper.Map<Grade, GradeViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(GradeViewModel gradeViewmodel)
        {
            GradeViewModel VMObj = new GradeViewModel();
            if (gradeViewmodel.GradeID != 0)
            {
                var obj = _GradeService.GetById(gradeViewmodel.GradeID);
                obj.Description = gradeViewmodel.Description;
                obj.BasicPercentOfGross = gradeViewmodel.BasicPercentOfGross;
                AddAuditTrail(obj, false);
                _GradeService.Update(obj);
                _GradeService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }

    }
}