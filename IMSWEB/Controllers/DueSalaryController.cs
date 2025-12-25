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
    public class DueSalaryController : CoreController
    {
        IAdvanceSalaryService _AdvanceSalaryService;
        IMiscellaneousService<Bank> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;
        IEmployeeService _EmployeeService;
        public DueSalaryController(IErrorService errorService,
            IAdvanceSalaryService AdvanceSalaryService, IMiscellaneousService<Bank> miscellaneousService,
            ISystemInformationService sysInfoService,
                    IEmployeeService EmployeeService,
            IMapper mapper)
            : base(errorService)
        {
            _AdvanceSalaryService = AdvanceSalaryService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _sysInfoService = sysInfoService;
            _EmployeeService = EmployeeService;
        }
        public ActionResult Search(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
            {
                TempData["FromDate"] = Convert.ToDateTime(formCollection["FromDate"]);
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Index()
        {
            DateTime dAttendencMonth = DateTime.MinValue;

            if (TempData.ContainsKey("FromDate"))
            {
                dAttendencMonth = Convert.ToDateTime(TempData["FromDate"]);
            }
            else
            {
                var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                dAttendencMonth = Sysinfo.NextPayProcessDate;
            }
            var DateRange = GetFirstAndLastDateOfMonth(dAttendencMonth);

            ViewBag.SearchDate = dAttendencMonth;
            var advances = await _AdvanceSalaryService.GetAllDueSalaryAsync(DateRange.Item1, DateRange.Item2);
            var vmadvances = _mapper.Map<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>>, IEnumerable<DueSalaryViewModel>>(advances);
            return View(vmadvances);
        }

        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        void CheckAndAddModelError(DueSalaryViewModel newDueSalary, FormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["EmployeesId"]))
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            else
            {
                newDueSalary.EmployeeID = collection["EmployeesId"];
                var employee = _EmployeeService.GetEmployeeById(Convert.ToInt32(newDueSalary.EmployeeID));

            }
            var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);
            if (newDueSalary.Date < DateRange.Item1)
            {
                ModelState.AddModelError("Date", "Date can't be smaller than Salary Process Month.");
            }

            if (newDueSalary.Amount == 0)
                ModelState.AddModelError("Amount", "Amount is required.");

        }
        [HttpPost]
        public ActionResult Create(DueSalaryViewModel newDueSalary, FormCollection collection)
        {

            CheckAndAddModelError(newDueSalary, collection);
            newDueSalary.CreatedBy = User.Identity.GetUserId<int>();
            newDueSalary.CreatedDate = DateTime.Now;
            newDueSalary.ConcernID = User.Identity.GetConcernId();

            if (!ModelState.IsValid)
                return View(newDueSalary);

            var dueSalary = _mapper.Map<DueSalaryViewModel, AdvanceSalary>(newDueSalary);
            dueSalary.SalaryType = EnumSalaryType.DueSalary;
            _AdvanceSalaryService.Add(dueSalary);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Save Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var dueSalary = _AdvanceSalaryService.GetById(id);
            var vmDue = _mapper.Map<AdvanceSalary, DueSalaryViewModel>(dueSalary);
            return View("Create", vmDue);
        }


        [HttpPost]
        public ActionResult Edit(DueSalaryViewModel newDueSalary, FormCollection collection)
        {
            CheckAndAddModelError(newDueSalary, collection);
            newDueSalary.ConcernID = User.Identity.GetConcernId();
            if (!ModelState.IsValid)
                return View("Create", newDueSalary);

            var dueSalary = _AdvanceSalaryService.GetById(Convert.ToInt32(newDueSalary.ID));
            dueSalary.ModifiedBy = User.Identity.GetUserId<int>();
            dueSalary.ModifiedDate = DateTime.Now;
            dueSalary.Date = newDueSalary.Date;
            dueSalary.Amount = newDueSalary.Amount;
            dueSalary.Remarks = newDueSalary.Remarks;
            dueSalary.EmployeeID = Convert.ToInt32(newDueSalary.EmployeeID);
            dueSalary.SalaryType = EnumSalaryType.DueSalary;
            _AdvanceSalaryService.Update(dueSalary);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Update Successfull.", ToastType.Success);

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id, FormCollection collection)
        {
            var dueSalary = _AdvanceSalaryService.GetById(Convert.ToInt32(id));
            var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);
            if (dueSalary.Date < DateRange.Item1)
            {
                AddToastMessage("", "You can't delete it. Because this month's salary is already finalized.", ToastType.Error);
                return RedirectToAction("Index");
            }
            _AdvanceSalaryService.Delete(id);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Delete Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}
