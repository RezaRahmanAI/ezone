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
    public class EmployeeLeaveController : CoreController
    {
        IMapper _mapper;
        IGradeService _GradeService;
        IMiscellaneousService<EmployeeLeave> _miscell;
        IEmployeeLeaveService _EmployeeLeaveService;
        IDepartmentService _DepartmentService;
        IDesignationService _DesignationService;
        IEmployeeService _EmployeeService;
        ISystemInformationService _sysInfoService;
        ISalaryProcessorService _SalaryProcessService;
        IHolidayCalenderService _HolidayService;
        public EmployeeLeaveController(IErrorService errorService, IGradeService GradeService,
            IMapper Mapper, IMiscellaneousService<EmployeeLeave> miscell, IEmployeeLeaveService EmployeeLeaveService,
            IDepartmentService DepartmentService,
            IDesignationService DesignationService,
            IEmployeeService EmployeeService, ISystemInformationService SysService
            , ISalaryProcessorService SalaryProcessService
            , IHolidayCalenderService HolidayService, ISystemInformationService sysInfoService
            )
            : base(errorService, sysInfoService)
        {
            _GradeService = GradeService;
            _mapper = Mapper;
            _miscell = miscell;
            _EmployeeLeaveService = EmployeeLeaveService;
            _DepartmentService = DepartmentService;
            _DesignationService = DesignationService;
            _EmployeeService = EmployeeService;
            _sysInfoService = SysService;
            _SalaryProcessService = SalaryProcessService;
            _HolidayService = HolidayService;
        }
        public async Task<ActionResult> Index()
        {
            DateTime SearchDate = DateTime.MinValue;
            if (TempData.ContainsKey("SearchDate"))
            {
                SearchDate = Convert.ToDateTime(TempData["SearchDate"]);
            }
            else
            {
                var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                SearchDate = Sysinfo.NextPayProcessDate;
            }
            ViewBag.SearchDate = SearchDate;
            var DateRange = GetFirstAndLastDateOfMonth(SearchDate);
            var leaves = await _EmployeeLeaveService.GetAllAsync(DateRange.Item1, DateRange.Item2);
            var vmLeaves = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>>, IEnumerable<EmployeeLeaveViewModel>>(leaves);
            return View(vmLeaves);
        }

        public ActionResult Create()
        {
            return View(new EmployeeLeaveViewModel() { LeaveType = EnumEmployeeLeaveType.DayLeave });
        }

        [HttpPost]
        public ActionResult Create(EmployeeLeaveViewModel newLeave, FormCollection formCollection)
        {
            CheckAndAddModelError(newLeave, formCollection);
            if (!ModelState.IsValid)
            {
                return View(newLeave);
            }

            newLeave.CreatedBy = User.Identity.GetUserId<int>();
            newLeave.CreateDate = DateTime.Now;
            newLeave.Status = EnumEmployeeLeaveStatus.Pending;
            var mnewLeave = _mapper.Map<EmployeeLeaveViewModel, EmployeeLeave>(newLeave);
            _EmployeeLeaveService.Add(mnewLeave);
            _EmployeeLeaveService.Save();
            AddToastMessage("", "Save Successfull.", ToastType.Success);
            return RedirectToAction("Index");

        }

        private void CheckAndAddModelError(EmployeeLeaveViewModel newLeave, FormCollection formCollection)
        {
            int EmployeeID = 0;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                newLeave.EmployeeID = formCollection["EmployeesId"];
                EmployeeID = Convert.ToInt32(newLeave.EmployeeID);
            }
            else
            {
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            }

            if (!string.IsNullOrEmpty(formCollection["LeaveDate"]))
            {
                newLeave.LeaveDate = Convert.ToDateTime(formCollection["LeaveDate"]);
            }
            if (_EmployeeLeaveService.GetAllIQueryable().Any(i => i.EmployeeID == EmployeeID && i.LeaveDate == newLeave.LeaveDate && i.EmployeeLeaveID != newLeave.EmployeeLeaveID))
            {
                ModelState.AddModelError("LeaveDate", "This day leave is already given to this employee.");
            }
            if (_HolidayService.GetAllIQueryable().Any(i => i.Date == newLeave.LeaveDate))
            {
                ModelState.AddModelError("LeaveDate", "Holiday can't be assigned as Leave.");
            }
            var SysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            if (newLeave.LeaveDate < DateRange.Item1 || newLeave.LeaveDate > DateRange.Item2)
            {
                ModelState.AddModelError("LeaveDate", "Only salary process month's date can be assigned.");
            }
        }

        //
        // GET: /EmployeeLeave/Edit/5
        public ActionResult Edit(int id)
        {
            var leave = _EmployeeLeaveService.GetById(id);
            var vmLeave = _mapper.Map<EmployeeLeave, EmployeeLeaveViewModel>(leave);
            return View("Create", vmLeave);
        }

        public ActionResult Search(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["LeaveDate"]))
            {
                TempData["SearchDate"] = Convert.ToDateTime(formCollection["LeaveDate"]);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(EmployeeLeaveViewModel newLeave, FormCollection formCollection)
        {

            CheckAndAddModelError(newLeave, formCollection);
            if (!ModelState.IsValid)
            {
                return View("Create", newLeave);
            }

            var leave = _EmployeeLeaveService.GetById(newLeave.EmployeeLeaveID);
            leave.LeaveDate = newLeave.LeaveDate;
            leave.Description = newLeave.Description;
            leave.LeaveType = (int)newLeave.LeaveType;
            leave.PaidLeave = newLeave.IsPaidLeave ? 1 : 0;
            leave.ModifiedBy = User.Identity.GetUserId<int>();
            leave.ModifiedDate = DateTime.Now;
            leave.ShortLeaveHour = (decimal)newLeave.ShortLeaveHour;
            leave.EmployeeID = Convert.ToInt32(newLeave.EmployeeID);
            _EmployeeLeaveService.Save();
            AddToastMessage("", "Update Successfull.", ToastType.Success);
            return RedirectToAction("Index");

        }

        bool IsUpdateValid(EmployeeLeave leave)
        {
            var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);
            if (leave.LeaveDate < DateRange.Item1)
            {
                AddToastMessage("", "This date is finalized. You can't edit it.");
                return false;
            }
            return true;
        }
        public ActionResult Delete(int id)
        {
            var leave = _EmployeeLeaveService.GetById(id);

            if (IsUpdateValid(leave))
            {
                _EmployeeLeaveService.Delete(id);
                _EmployeeLeaveService.Save();
                AddToastMessage("", "Update Successfull.", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Approve(int id)
        {
            var leave = _EmployeeLeaveService.GetById(id);
            if (IsUpdateValid(leave))
            {
                leave.Status = (int)EnumEmployeeLeaveStatus.Approved;
                _EmployeeLeaveService.Save();
                AddToastMessage("", "Approved Successfull.", ToastType.Success);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ApproveCancel(int id)
        {
            var leave = _EmployeeLeaveService.GetById(id);
            if (IsUpdateValid(leave))
            {
                leave.Status = (int)EnumEmployeeLeaveStatus.Pending;
                _EmployeeLeaveService.Save();
                AddToastMessage("", "Approve cancelled Successfully.", ToastType.Success);
            }
            return RedirectToAction("Index");
        }
        //
        // POST: /EmployeeLeave/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
