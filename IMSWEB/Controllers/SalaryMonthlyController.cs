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
    [RoutePrefix("SalaryMonthly")]
    public class SalaryMonthlyController : CoreController
    {
        ISalaryMonthlyService _salaryMonthlyService;
        IMiscellaneousService<SalaryMonthly> _miscellaneousService;
        IMapper _mapper;

        public SalaryMonthlyController(IErrorService errorService,
           ISalaryMonthlyService salaryMonthlyService, IMiscellaneousService<SalaryMonthly> miscellaneousService,
            IMapper mapper)
            : base(errorService)
        {
            _salaryMonthlyService = salaryMonthlyService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var salaryMonthlyAsync = _salaryMonthlyService.GetAllSalaryMonthlyAsync();
            var vmodel = _mapper.Map<IEnumerable<SalaryMonthly>, IEnumerable<CreateSalaryMonthlyViewModel>>(await salaryMonthlyAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => x.SalaryMonthlyID);
            return View(new CreateCategoryViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateSalaryMonthlyViewModel newSalaryMonthly, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSalaryMonthly);

            if (newSalaryMonthly != null)
            {
                var existingSalaryMonthly = _miscellaneousService.GetDuplicateEntry(c => c.Designation == newSalaryMonthly.Designation);
                if (existingSalaryMonthly != null)
                {
                    AddToastMessage("", "A existingSalaryMonthly with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newSalaryMonthly);
                }

                var salaryMonthly = _mapper.Map<CreateSalaryMonthlyViewModel, SalaryMonthly>(newSalaryMonthly);
                salaryMonthly.ConcernID = User.Identity.GetConcernId();
                _salaryMonthlyService.AddSalaryMonthly(salaryMonthly);
                _salaryMonthlyService.SaveSalaryMonthly();

                AddToastMessage("", " SalaryMonthly has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No SalaryMonthly data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var salaryMonthly = _salaryMonthlyService.GetSalaryMonthlyById(id);
            var vmodel = _mapper.Map<SalaryMonthly, CreateSalaryMonthlyViewModel>(salaryMonthly);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSalaryMonthlyViewModel newSalaryMonthly, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newSalaryMonthly);

            if (newSalaryMonthly != null)
            {
                var salaryMonthly = _salaryMonthlyService.GetSalaryMonthlyById(newSalaryMonthly.SalaryMonthlyID);

              //  salaryMonthly.Code = newSalaryMonthly.Code;
                salaryMonthly.Designation = newSalaryMonthly.Designation;
                salaryMonthly.ConcernID = User.Identity.GetConcernId();

                _salaryMonthlyService.UpdateSalaryMonthly(salaryMonthly);
                _salaryMonthlyService.SaveSalaryMonthly();
             

                AddToastMessage("", "SalaryMonthly has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No SalaryMonthly data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _salaryMonthlyService.DeleteSalaryMonthly(id);
            _salaryMonthlyService.SaveSalaryMonthly();
            AddToastMessage("", "Salary Monthly has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}