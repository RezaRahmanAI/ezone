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
    public class ManualAttendenceController : CoreController
    {
        IManualAttendencService _manualattendService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;
        IEmployeeService _employeeService;
        private readonly IAttendenceService _attendenceService;
        private readonly ISalaryProcessorService _salaryProcessorService;

        public ManualAttendenceController(IErrorService errorService,
            IManualAttendencService AttendenceService, ISystemInformationService SysService,
            IMapper mapper, IEmployeeService employeeService, IAttendenceService attendenceService,
            ISalaryProcessorService salaryProcessorService
            )
            : base(errorService, SysService)
        {
            _manualattendService = AttendenceService;
            _mapper = mapper;
            _sysInfoService = SysService;
            _employeeService = employeeService;
            _attendenceService = attendenceService;
            _salaryProcessorService = salaryProcessorService;
        }

        public ActionResult Index()
        {
            return View(PopulateDropdown(new CreateManualAttendenceVM()));
        }

        private CreateManualAttendenceVM PopulateDropdown(CreateManualAttendenceVM createManualAttendence)
        {
            var SysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            createManualAttendence.NextPayProcessDate = SysInfo.NextPayProcessDate.ToString("MMM yyyy");
            createManualAttendence.Employees = new SelectList(_employeeService.GetAllEmployeeIQueryable().Where(i=>i.Status == EnumActiveInactive.Active), "EmployeeID", "Name");
            return createManualAttendence;
        }

        [HttpPost]
        public ActionResult Index(CreateManualAttendenceVM createManualAttendence,
            FormCollection formCollection)
        {


            DateTime SearchDate = DateTime.MinValue;
            List<ManualAttendenceVM> attendenceVMs = null;
            if (formCollection.Get("btnSearch") != null)
            {
                attendenceVMs = SearchEmployeeAttendence(createManualAttendence, formCollection);
                createManualAttendence.Attendences = attendenceVMs;
                ModelState.Clear();
                return View(PopulateDropdown(createManualAttendence));
            }
            else if (formCollection.Get("btnSave") != null)
            {
                if (!IsValid(createManualAttendence))
                {
                    return View(PopulateDropdown(createManualAttendence));
                }

                if (SaveAttendence(createManualAttendence, formCollection))
                {
                    AddToastMessage("", "Save successfull.");
                    return RedirectToAction("Index");
                }
            }

            return View(PopulateDropdown(createManualAttendence));
        }

        private bool SaveAttendence(CreateManualAttendenceVM createManualAttendence,
            FormCollection formCollection)
        {
            bool result = true;
            try
            {
                ManualAttendence manual = null;
                var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var newAtt = createManualAttendence.Attendences.Where(i => i.IsSelect == true && i.ID == null).ToList();
                var deleted = createManualAttendence.Attendences.Where(i => i.IsSelect == false && i.ID > 0).ToList();
                foreach (var item in deleted)
                {
                    _manualattendService.Delete((int)item.ID);
                }

                foreach (var item in newAtt)
                {
                    manual = new ManualAttendence();
                    manual.Date = Convert.ToDateTime(item.Date);
                    manual.ClockIn = "10.00 AM";
                    manual.ClockOut = "6.00 PM";
                    manual.OnDuty = "10.00 AM";
                    manual.OffDuty = "6.00 PM";
                    manual.EmployeeID = createManualAttendence.EmployeeID;
                    AddAuditTrail(manual, true);
                    _manualattendService.Add(manual);
                }
                _manualattendService.Save();
            }
            catch (Exception ex)
            {
                AddToastMessage("", ex.Message);
                result = false;
            }

            return result;

        }

        private bool IsValid(CreateManualAttendenceVM createManualAttendence)
        {
            if (createManualAttendence.EmployeeID == 0)
            {
                AddToastMessage("", "Please select employee", ToastType.Error);
                return false;
            }

            if (createManualAttendence.Attendences.Count() == 0)
            {
                AddToastMessage("", "Please select date", ToastType.Error);
                return false;
            }
            var daterange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(createManualAttendence.NextPayProcessDate));
            if (_salaryProcessorService.IsSalaryProcessed(createManualAttendence.EmployeeID, daterange))
            {
                AddToastMessage("", "This employee's salary is already processed. Please undo salary process before update", ToastType.Error);
                return false;
            }

            return true;
        }

        private List<ManualAttendenceVM> SearchEmployeeAttendence(CreateManualAttendenceVM createManualAttendence,
            FormCollection formCollection)
        {

            var SysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

            var dateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);

            var Assignholidays = _manualattendService.GetAll()
                                .Where(i => i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2
                                && i.EmployeeID == createManualAttendence.EmployeeID);

            ManualAttendenceVM hcvm = null;
            List<ManualAttendenceVM> hcVmList = new List<ManualAttendenceVM>();

            var machineAttends = (from a in _attendenceService.GetAllIQueryable()
                                 .Where(i => i.AttendencMonth >= dateRange.Item1 && i.AttendencMonth <= dateRange.Item2)
                                  join d in _attendenceService.GetDetails() on a.AttenMonthID equals d.AttenMonthID
                                  where d.AccountNo == createManualAttendence.EmployeeID && d.Absent == 1
                                  select d).ToList();

            for (DateTime date = dateRange.Item1; date <= dateRange.Item2; date = date.AddDays(1))
            {
                if (!machineAttends.Any(i => i.Date == date))
                {
                    hcvm = new ManualAttendenceVM();
                    hcvm.Date = date.Date.ToString("dd MMM yyyy");
                    if (Assignholidays.Any(i => i.Date == date))
                    {
                        var assigned = Assignholidays.FirstOrDefault(i => i.Date == date);
                        hcvm.IsSelect = true;
                        hcvm.ID = assigned.ID;
                    }
                    hcVmList.Add(hcvm);
                }

            }

            return hcVmList;
        }
    }
}