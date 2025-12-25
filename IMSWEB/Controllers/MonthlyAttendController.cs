using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class MonthlyAttendController : CoreController
    {
        IMonthlyAttendenceService _monthlyAttendenceService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;
        IEmployeeService _employeeService;
        private readonly IAttendenceService _attendenceService;
        private readonly ISalaryProcessorService _salaryProcessorService;
        private readonly IDepartmentService _departmentService;

        public MonthlyAttendController(IErrorService errorService,
            IMonthlyAttendenceService AttendenceService, ISystemInformationService SysService,
            IMapper mapper, IEmployeeService employeeService, IAttendenceService attendenceService,
            ISalaryProcessorService salaryProcessorService,
            IDepartmentService departmentService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _monthlyAttendenceService = AttendenceService;
            _mapper = mapper;
            _sysInfoService = SysService;
            _employeeService = employeeService;
            _attendenceService = attendenceService;
            _salaryProcessorService = salaryProcessorService;
            _departmentService = departmentService;

        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(PopulateDropdown(new CreateMonthlyAttendenceVM()));
        }

        [HttpPost]
        public ActionResult Index(CreateMonthlyAttendenceVM CreateMonthlyAttendence,
            FormCollection formCollection)
        {
            if (formCollection.Get("btnSave") != null)
            {
                if (!IsValid(CreateMonthlyAttendence))
                {
                    return View(PopulateDropdown(CreateMonthlyAttendence));
                }

                if (SaveAttendence(CreateMonthlyAttendence))
                {
                    AddToastMessage("", "Save successfull", ToastType.Success);
                    return RedirectToAction("Index");
                }
                AddToastMessage("", "Save failed.", ToastType.Error);
                return View(PopulateDropdown(CreateMonthlyAttendence));
            }
            else
            {

                var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
                string Month = sysInfo.NextPayProcessDate.ToString("MMM yyyy");
                var attendences = _monthlyAttendenceService.GetAll()
                                  .Where(i => i.Month >= DateRange.Item1 && i.Month <= DateRange.Item2).ToList();

                var emp = (from e in _employeeService.GetAllEmployeeDetails(CreateMonthlyAttendence.DepartmentID)
                           join ma in attendences on e.Item1 equals ma.EmployeeID into lma
                           from ma in lma.DefaultIfEmpty()
                           select new MonthlyAttendenceVM
                           {
                               MAID = ma != null ? ma.MAID : 0,
                               EmployeeID = e.Item1,
                               EmployeeName = e.Item3,
                               DepartmentName = e.Rest.Item1,
                               DesignationName = e.Item7,
                               IsSelect = ma != null ? true : false,
                               Days = ma != null ? ma.Days : 0,
                               OTDays = ma != null ? ma.OTDays : 0,
                               Month = Month
                           }).OrderByDescending(i => i.MAID).OrderByDescending(i => i.DepartmentName).ToList();

                CreateMonthlyAttendence.Attendences = emp;
                ModelState.Clear();
                return View(PopulateDropdown(CreateMonthlyAttendence));
            }
        }
        private CreateMonthlyAttendenceVM PopulateDropdown(CreateMonthlyAttendenceVM createEmpCommissionVM)
        {
            createEmpCommissionVM.Departments = _mapper.Map<IQueryable<Department>, List<DepartmentViewModel>>
                (_departmentService.GetAllDepartmentIQueryable());

            return createEmpCommissionVM;
        }
        private bool IsValid(CreateMonthlyAttendenceVM createMonthlyAttendence)
        {
            var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            int Days = DateTime.DaysInMonth(sysInfo.NextPayProcessDate.Year, sysInfo.NextPayProcessDate.Month);

            bool Result = true;
            if (createMonthlyAttendence.Attendences.Count() == 0)
            {
                AddToastMessage("", "Data is not found");
                return false;
            }

            for (int i = 0; i < createMonthlyAttendence.Attendences.Count(); i++)
            {
                if (createMonthlyAttendence.Attendences[i].Days < 0 && createMonthlyAttendence.Attendences[i].IsSelect)
                {
                    ModelState.AddModelError("Attendences[" + i + "].Days", "Present days can't be negative");
                    Result = false;
                }
            }

            if (!Result)
            {
                return false;
            }

            for (int i = 0; i < createMonthlyAttendence.Attendences.Count(); i++)
            {
                if (createMonthlyAttendence.Attendences[i].Days > Days && createMonthlyAttendence.Attendences[i].IsSelect)
                {
                    ModelState.AddModelError("Attendences[" + i + "].Days", "Present days can't be greated than total days of the month");
                    Result = false;
                }
            }

            if (!Result)
            {
                AddToastMessage("", "Present days can't be greated than total days of the month");
                return false;
            }
            var daterange = GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
            for (int i = 0; i < createMonthlyAttendence.Attendences.Count(); i++)
            {
                if (createMonthlyAttendence.Attendences[i].IsSelect
                    && _salaryProcessorService.IsSalaryProcessed(createMonthlyAttendence.Attendences[i].EmployeeID, daterange))
                {
                    ModelState.AddModelError("Attendences[" + i + "].Days", "This employee's salary is already processed");
                    Result = false;
                }
            }
            if (!Result)
            {
                AddToastMessage("", "This employee's salary is already processed. Please undo salary process before update", ToastType.Error);
                return false;
            }

            return true;
        }

        private bool SaveAttendence(CreateMonthlyAttendenceVM CreateMonthlyAttendence)
        {
            bool result = true;
            try
            {
                MonthlyAttendence manual = null;
                var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                var newAtt = CreateMonthlyAttendence.Attendences
                    .Where(i => i.IsSelect == true && i.MAID == 0).ToList();

                var deleted = CreateMonthlyAttendence.Attendences
                    .Where(i => i.IsSelect == false && i.MAID > 0).ToList();

                var updated = CreateMonthlyAttendence.Attendences
                    .Where(i => i.IsSelect == true && i.MAID > 0).ToList();

                foreach (var item in deleted)
                {
                    _monthlyAttendenceService.Delete((int)item.MAID);
                }

                foreach (var item in newAtt)
                {
                    manual = new MonthlyAttendence();
                    manual.Month = Convert.ToDateTime(item.Month);
                    manual.EmployeeID = item.EmployeeID;
                    manual.Days = item.Days;
                    manual.OTDays = item.OTDays;
                    AddAuditTrail(manual, true);
                    _monthlyAttendenceService.Add(manual);
                }

                foreach (var item in updated)
                {
                    manual = _monthlyAttendenceService.GetById((int)item.MAID);
                    manual.Month = Convert.ToDateTime(item.Month);
                    manual.EmployeeID = item.EmployeeID;
                    manual.Days = item.Days;
                    manual.OTDays = item.OTDays;
                    AddAuditTrail(manual, false);
                    _monthlyAttendenceService.Update(manual);
                }

                _monthlyAttendenceService.Save();
            }
            catch (Exception ex)
            {
                AddToastMessage("", ex.Message);
                result = false;
            }

            return result;

        }

    }
}