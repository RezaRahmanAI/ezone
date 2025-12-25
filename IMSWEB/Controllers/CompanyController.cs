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
    [RoutePrefix("company")]
    public class CompanyController : CoreController
    {
        ICompanyService _companyService;
        IMiscellaneousService<Company> _miscellaneousService;
        IMapper _mapper;

        public CompanyController(IErrorService errorService,
            ICompanyService companyService, IMiscellaneousService<Company> miscellaneousService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _companyService = companyService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var companiesAsync = _companyService.GetAllCompanyAsync();
            var vmodel = _mapper.Map<IEnumerable<Company>, IEnumerable<CreateCompanyViewModel>>(await companiesAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateCompanyViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateCompanyViewModel newCompany, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newCompany);

            if (newCompany != null)
            {
                var existingCompany = _miscellaneousService.GetDuplicateEntry(c => c.Name == newCompany.Name);
                if (existingCompany != null)
                {
                    AddToastMessage("", "A Company with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newCompany);
                }

                var company = _mapper.Map<CreateCompanyViewModel, Company>(newCompany);
                company.ConcernID = User.Identity.GetConcernId();
                _companyService.AddCompany(company);
                _companyService.SaveCompany();

                AddToastMessage("", "Company has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Company data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var company = _companyService.GetCompanyById(id);
            var vmodel = _mapper.Map<Company, CreateCompanyViewModel>(company);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateCompanyViewModel newCompany, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newCompany);

            if (newCompany != null)
            {
                var company = _companyService.GetCompanyById(int.Parse(newCompany.Id));

                company.Code = newCompany.Code;
                company.Name = newCompany.Name;
                company.ConcernID = User.Identity.GetConcernId();

                _companyService.UpdateCompany(company);
                _companyService.SaveCompany();

                AddToastMessage("", "Company has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Company data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _companyService.DeleteCompany(id);
            _companyService.SaveCompany();
            AddToastMessage("", "Company has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        public JsonResult GetCompanyByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var companies = from c in _companyService.GetAllCompany()
                                where c.Name.ToLower().Contains(prefix.ToLower())
                                select new
                                {
                                    ID = c.CompanyID,
                                    Name = c.Name
                                };
                if (companies.Count() > 0)
                    return Json(companies, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var companies = (from c in _companyService.GetAllCompany()
                                 select new
                                 {
                                     ID = c.CompanyID,
                                     Name = c.Name
                                 }).Take(10);
                if (companies.Count() > 0)
                    return Json(companies, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddCompany(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_companyService.GetAllCompany().Any(i => i.Name.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This company is already exist." }, JsonRequestBehavior.AllowGet);
                }

                Company company = new Company();
                company.Name = Name.Trim();
                company.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                AddAuditTrail(company, true);
                _companyService.AddCompany(company);
                _companyService.SaveCompany();
                return Json(new { result = true, data = company }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);

        }
    }
}