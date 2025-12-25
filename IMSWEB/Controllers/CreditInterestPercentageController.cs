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
    [RoutePrefix("CreditInterestPercentage")]
    public class CreditInterestPercentageController : CoreController
    {
        ICreditInterestPercentageService _CreditInterestPercentageService;
        IMiscellaneousService<CreditInterestPercentage> _miscellaneousService;
        IMapper _mapper;

        public CreditInterestPercentageController(IErrorService errorService,
            ICreditInterestPercentageService CreditInterestPercentageService, IMiscellaneousService<CreditInterestPercentage> miscellaneousService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _CreditInterestPercentageService = CreditInterestPercentageService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var IntPerAsync = _CreditInterestPercentageService.GetAllCreditInterestPercentageAsync();
            var vmodel = _mapper.Map<IEnumerable<CreditInterestPercentage>, IEnumerable<CreditInterestPercentageViewModel>>(await IntPerAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreditInterestPercentageViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreditInterestPercentageViewModel newIntPer, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newIntPer);

            if (newIntPer != null)
            {
                var existingIntPer = _miscellaneousService.GetDuplicateEntry(c => c.Percentage == newIntPer.Percentage);
                if (existingIntPer != null)
                {
                    AddToastMessage("", "A Credit Interest Percentage with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newIntPer);
                }

                var IntPer = _mapper.Map<CreditInterestPercentageViewModel, CreditInterestPercentage>(newIntPer);
                IntPer.ConcernID = User.Identity.GetConcernId();
                _CreditInterestPercentageService.AddCreditInterestPercentage(IntPer);
                _CreditInterestPercentageService.SaveCreditInterestPercentage();

                AddToastMessage("", "Credit Interest Percentage has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Credit Interest Percentage data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var IntPer = _CreditInterestPercentageService.GetCreditInterestPercentageById(id);
            var vmodel = _mapper.Map<CreditInterestPercentage, CreditInterestPercentageViewModel>(IntPer);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreditInterestPercentageViewModel newIntPer, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newIntPer);

            if (newIntPer != null)
            {
                var IntPer = _CreditInterestPercentageService.GetCreditInterestPercentageById(newIntPer.IntPercentageID);

                IntPer.Code = newIntPer.Code;
                IntPer.Percentage = newIntPer.Percentage;
                IntPer.EffectDate = Convert.ToDateTime(newIntPer.EffectDate);
                IntPer.ConcernID = User.Identity.GetConcernId();

                _CreditInterestPercentageService.UpdateCreditInterestPercentage(IntPer);
                _CreditInterestPercentageService.SaveCreditInterestPercentage();

                AddToastMessage("", "Credit Interest Percentage has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Credit Interest Percentage data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _CreditInterestPercentageService.DeleteCreditInterestPercentage(id);
            _CreditInterestPercentageService.SaveCreditInterestPercentage();
            AddToastMessage("", "Credit Interest Percentage has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

    }
}