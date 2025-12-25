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
    [RoutePrefix("color")]
    public class ColorController : CoreController
    {
        IColorService _colorService;
        IMiscellaneousService<Color> _miscellaneousService;
        private readonly ISisterConcernService _sisterConcernService;
        ISystemInformationService _SysInfoService;
        IMapper _mapper;
         
        public ColorController(IErrorService errorService,
            IColorService colorService, IMiscellaneousService<Color> miscellaneousService, ISystemInformationService SysInfoService, IMapper mapper, ISisterConcernService sisterConcernService)
            : base(errorService, SysInfoService)
        {
            _colorService = colorService;
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
            var colorsAsync = _colorService.GetAllColorAsync();
            var vmodel = _mapper.Map<IEnumerable<Color>, IEnumerable<CreateColorViewModel>>(await colorsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateColorViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateColorViewModel newColor, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                return View(newColor);

            }

            if (newColor != null)
            {

                var existingColor = _miscellaneousService.GetDuplicateEntry(c => c.Name == newColor.Name);
                if (existingColor != null)
                {
                    AddToastMessage("", "A Color with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newColor);
                }

                var model = _mapper.Map<CreateColorViewModel, Color>(newColor);
                model.ConcernID = User.Identity.GetConcernId();
                _colorService.AddColor(model);
                _colorService.SaveColor();

                var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                if (SystemInfo.IsColorEnable == 1)
                { 
                    AddToSisterConcern(newColor);
                }
                AddToastMessage("", "Color has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Color data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
            var model = _colorService.GetColorById(id);
            var vmodel = _mapper.Map<Color, CreateColorViewModel>(model);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateColorViewModel newColor, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                return View(newColor);
            }

            if (newColor != null)
            {
                var model = _colorService.GetColorById(int.Parse(newColor.Id));

                string tmpColorName = model.Name;
                model.Code = newColor.Code;
                model.Name = newColor.Name;
                model.ConcernID = User.Identity.GetConcernId(); ;

                _colorService.UpdateColor(model);
                _colorService.SaveColor();

                UpdateSisterConcernColor(model, tmpColorName);

                AddToastMessage("", "Color has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Color data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        private void UpdateSisterConcernColor(Color color, string colorName)
        {
            var families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                        .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                foreach (var item in families)
                {
                    var concernColor = _colorService.GetAll(item.ConcernID)
                        .FirstOrDefault(i => i.Name.TrimEnd().ToLower().Equals(colorName.TrimEnd().ToLower()));

                    if (concernColor != null)
                    {
                        concernColor.Name = color.Name;

                        _colorService.UpdateColor(concernColor);
                        _colorService.SaveColor();
                    }
                }
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _colorService.DeleteColor(id);
            _colorService.SaveColor();
            AddToastMessage("", "Color has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public JsonResult AddColor(string Name)
        //{
        //    if (!string.IsNullOrWhiteSpace(Name))
        //    {
        //        if (_colorService.GetAll().Any(i => i.Name.ToLower().Equals(Name.Trim().ToLower())))
        //        {
        //            return Json(new { result = false, msg = "This color is already exist." }, JsonRequestBehavior.AllowGet);
        //        }

        //        Color newColor = new Color();
        //        newColor.Name = Name.Trim();
        //        newColor.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
        //        AddAuditTrail(newColor, true);
        //        _colorService.AddColor(newColor);
        //        _colorService.SaveColor();
        //        CreateColorViewModel vmColor = new CreateColorViewModel
        //        {
        //            Name = newColor.Name,
        //            Code = newColor.Code
        //        };
        //        AddToSisterConcern(vmColor);
        //        return Json(new { result = true, data = newColor }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public JsonResult AddColor(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_colorService.GetAllColor().Any(i => i.Name.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This color is already exist." }, JsonRequestBehavior.AllowGet);
                }

                Color newColor = new Color();
                newColor.Name = Name.Trim();
                newColor.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                AddAuditTrail(newColor, true);
                _colorService.AddColor(newColor);
                _colorService.SaveColor();
                CreateColorViewModel vmColor = new CreateColorViewModel
                {
                    Name = newColor.Name,
                    Code = newColor.Code
                };
                AddToSisterConcern(vmColor);
                return Json(new { result = true, data = newColor }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetColorByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var colors = from c in _colorService.GetAllColor()
                              where c.Name.ToLower().Contains(prefix.ToLower())
                              select new
                              {
                                  ID = c.ColorID,
                                  Name = c.Name
                              };
                if (colors.Count() > 0)
                    return Json(colors, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var colors = (from c in _colorService.GetAllColor()
                               select new
                               {
                                   ID = c.ColorID,
                                   Name = c.Name
                               }).Take(10);
                if (colors.Count() > 0)
                    return Json(colors, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }



        public ActionResult GetColor(int id)
        {
           
            return View();
        }

        #region add color in sister concerns
        private void AddToSisterConcern(CreateColorViewModel color)
        {
            List<SisterConcern> families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                    .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                Color newColor = new Color();
                foreach (var concern in families)
                {
                    if (_colorService.GetAll(concern.ConcernID)
                        .Any(i => i.Name.ToLower().Equals(color.Name.Trim().ToLower())))
                        continue;

                    newColor = _mapper.Map<CreateColorViewModel, Color>(color);
                    newColor.ConcernID = concern.ConcernID;
                    _colorService.AddColor(newColor);
                    _colorService.SaveColor();
                }
            }
        }
        #endregion
    }
}