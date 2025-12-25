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
    public class AdvanceSalaryController : CoreController
    {
        IAdvanceSalaryService _AdvanceSalaryService;
        IMiscellaneousService<Bank> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;
        IEmployeeService _EmployeeService;
        public AdvanceSalaryController(IErrorService errorService,
            IAdvanceSalaryService AdvanceSalaryService, IMiscellaneousService<Bank> miscellaneousService,
            ISystemInformationService sysInfoService,
                    IEmployeeService EmployeeService,
            IMapper mapper)
            : base(errorService, sysInfoService)
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
            var advances = await _AdvanceSalaryService.GetAllAsync(DateRange.Item1, DateRange.Item2);
            var vmadvances = _mapper.Map<IEnumerable<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string, EnumSalaryType>>>, IEnumerable<AdvanceSalaryViewModel>>(advances);
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
        void CheckAndAddModelError(AdvanceSalaryViewModel newAdvance, FormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["EmployeesId"]))
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            else
            {
                newAdvance.EmployeeID = collection["EmployeesId"];
                var employee = _EmployeeService.GetEmployeeById(Convert.ToInt32(newAdvance.EmployeeID));
                //if (employee.GrossSalary < newAdvance.Amount)
                //{
                //    ModelState.AddModelError("Amount", "Amount can't be greater than Gross Salary.");
                //}
            }
            var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);
            if (newAdvance.Date < DateRange.Item1)
            {
                ModelState.AddModelError("Date", "Date can't be smaller than Salary Process Month.");
            }

            if (newAdvance.Amount == 0)
                ModelState.AddModelError("Amount", "Amount is required.");

        }
        [HttpPost]
        public ActionResult Create(AdvanceSalaryViewModel newAdvance, FormCollection collection)
        {

            CheckAndAddModelError(newAdvance, collection);
            newAdvance.CreatedBy = User.Identity.GetUserId<int>();
            newAdvance.CreatedDate = DateTime.Now;
            newAdvance.ConcernID = User.Identity.GetConcernId();

            if (!ModelState.IsValid)
                return View(newAdvance);

            var advance = _mapper.Map<AdvanceSalaryViewModel, AdvanceSalary>(newAdvance);
            advance.SalaryType = newAdvance.SalaryType;
            advance.IsAdvanceLoanPay = newAdvance.IsAdvanceLoanPay ? 1 : 0;
           
            #region Employee Advance Loan Amount Update
            var oEmployee = _EmployeeService.GetEmployeeById(Convert.ToInt32(newAdvance.EmployeeID));
            if(advance.SalaryType == EnumSalaryType.AdvanceLoan)
            {
                oEmployee.AdvanceLoanAmt = oEmployee.AdvanceLoanAmt + advance.Amount;
            }
            else if (advance.SalaryType == EnumSalaryType.AdvanceSalary && advance.IsAdvanceLoanPay == 1)
            {
                oEmployee.AdvanceLoanAmt = oEmployee.AdvanceLoanAmt - advance.Amount;
            }
            _EmployeeService.UpdateEmployee(oEmployee);
            _EmployeeService.SaveEmployee();
            #endregion

            _AdvanceSalaryService.Add(advance);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Save Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var advance = _AdvanceSalaryService.GetById(id);
            var vmAdvance = _mapper.Map<AdvanceSalary, AdvanceSalaryViewModel>(advance);
            return View("Create", vmAdvance);
        }


        [HttpPost]
        public ActionResult Edit(AdvanceSalaryViewModel newAdvance, FormCollection collection)
        {
            CheckAndAddModelError(newAdvance, collection);
            newAdvance.ConcernID = User.Identity.GetConcernId();
            if (!ModelState.IsValid)
                return View("Create", newAdvance);

            var advance = _AdvanceSalaryService.GetById(Convert.ToInt32(newAdvance.ID));
            advance.ModifiedBy = User.Identity.GetUserId<int>();
            advance.ModifiedDate = DateTime.Now;
            advance.Date = newAdvance.Date;
            advance.Amount = newAdvance.Amount;
            advance.Remarks = newAdvance.Remarks;
            advance.EmployeeID = Convert.ToInt32(newAdvance.EmployeeID);
            advance.SalaryType = newAdvance.SalaryType;
            advance.IsAdvanceLoanPay = newAdvance.IsAdvanceLoanPay ? 1 : 0;
            _AdvanceSalaryService.Update(advance);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Update Successfull.", ToastType.Success);

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id, FormCollection collection)
        {
            var advance = _AdvanceSalaryService.GetById(Convert.ToInt32(id));
            var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);
            if (advance.Date < DateRange.Item1)
            {
                AddToastMessage("", "You can't delete it. Because this month's salary is already finalized.", ToastType.Error);
                return RedirectToAction("Index");
            }
            _AdvanceSalaryService.Delete(id);
            _AdvanceSalaryService.Save();
            AddToastMessage("", "Delete Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult AdvanceLoanReport()
        {
            PopulateEmployeeDropdown();
            return View();
        }

        private void PopulateEmployeeDropdown()
        {
            ViewBag.Employees = new SelectList(_EmployeeService.GetAllEmployeeIQueryable(), "EmployeeID", "Name");
        }
    }
}
