using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
namespace IMSWEB.Controllers
{
    [Authorize]
    public class SalaryProcessController : CoreController
    {
        ISalaryProcessorService _SalaryProcessorService;
        IMiscellaneousService<ISalaryProcessorService> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _SysInfoService;
        IAttendenceService _AttendenceService;
        IEmployeeService _EmployeeService;
        IEmpGradeSalaryAssignmentService _GradeSalaryAssignment;
        IAdvanceSalaryService _AdvanceSalaryService;
        IDepartmentService _DepartmentService;
        IHolidayCalenderService _HolidayCalenderService;
        ITargetSetupService _TargetSetupService;
        ISalesOrderService _SOrderService;
        IDesWiseCommissionService _DESGWiseCommissionService;
        IManualAttendencService _manualAttendencService;
        private readonly IMonthlyAttendenceService _monthlyAttendenceService;
        private readonly IEmployeeCommissionService _empCommissionService;

        public SalaryProcessController(IErrorService errorService,
            ISalaryProcessorService SalaryProcessorService,
            IMiscellaneousService<ISalaryProcessorService> miscellaneousService,
             ISystemInformationService SysInfoService,
             IAttendenceService AttendenceService, IEmpGradeSalaryAssignmentService GradeSalaryAssignment,
             IEmployeeService EmployeeService, IAdvanceSalaryService AdvanceSalaryService,
            IDepartmentService DepartmentService,
             IHolidayCalenderService HolidayCalenderService,
            ITargetSetupService TargetSetupService,
            ISalesOrderService SOrderService,
            IDesWiseCommissionService DESGWiseCommissionService,
            IMapper mapper,
            IManualAttendencService manualAttendencService,
            IMonthlyAttendenceService monthlyAttendenceService,
            IEmployeeCommissionService empCommissionService)
            : base(errorService, SysInfoService)
        {
            _SalaryProcessorService = SalaryProcessorService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SysInfoService = SysInfoService;
            _AttendenceService = AttendenceService;
            _EmployeeService = EmployeeService;
            _GradeSalaryAssignment = GradeSalaryAssignment;
            _AdvanceSalaryService = AdvanceSalaryService;
            _DepartmentService = DepartmentService;
            _HolidayCalenderService = HolidayCalenderService;
            _TargetSetupService = TargetSetupService;
            _SOrderService = SOrderService;
            _DESGWiseCommissionService = DESGWiseCommissionService;
            _manualAttendencService = manualAttendencService;
            _monthlyAttendenceService = monthlyAttendenceService;
            _empCommissionService = empCommissionService;
        }
        public ActionResult Index()
        {
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            return View(new SalaryProcessViewModel() { SalaryProcessMonth = SysInfo.NextPayProcessDate });
        }
        bool IsValid(List<int> IDList)
        {
            if (IDList.Count() == 0)
            {
                AddToastMessage("", "Please Select Employee First.", ToastType.Error);
                return false;
            }
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            var AttendMonth = _AttendenceService.GetAllIQueryable()
                .FirstOrDefault(i => i.AttendencMonth >= DateRange.Item1 && i.AttendencMonth <= DateRange.Item2);
            var manualAttends = _manualAttendencService.GetAll()
                .Where(i => i.Date >= DateRange.Item1 && i.Date <= DateRange.Item2);

            var monthlyAttends = _monthlyAttendenceService.GetAll()
           .Where(i => i.Month >= DateRange.Item1 && i.Month <= DateRange.Item2);

            if (AttendMonth == null && manualAttends.Count() == 0 && monthlyAttends.Count() == 0)
            {
                AddToastMessage("", "This month attendence not yet uploaded. Please upload attendence excel first.", ToastType.Error);
                return false;
            }

            return true;
        }
        public ActionResult ProcessSalary(SalaryProcessViewModel model, FormCollection formcollection)
        {
            List<int> EmployeeIDList = new List<int>();
            if (!string.IsNullOrEmpty(formcollection["EmployeeIdList"]))
            {
                EmployeeIDList = formcollection["EmployeeIdList"].Split(new char[] { ',' }).Select(Int32.Parse).Distinct().ToList();
            }
            if (!IsValid(EmployeeIDList))
            {
                return RedirectToAction("Index");
            };

            #region Variables declarations
            SalaryProcess salaryProcessNew = new SalaryProcess();
            EmpGradeSalaryAssignment empGradesalary = null;
            EmployeeCommission employeeCommission = null;
            Employee oEmployee = null;
            List<SalaryProcessErrorVM> ErrorList = new List<SalaryProcessErrorVM>();
            SalaryProcessErrorVM salaryProcessError = null;
            SalaryMonthly salaryMonthly = null;
            bool IsErrorFound = false;
            bool IsShowroomCommission = false;
            List<AdvanceSalary> AdvanceSalaries = new List<AdvanceSalary>();
            List<AdvanceSalary> DueSalaries = new List<AdvanceSalary>();

            TargetSetup EmployeeTarget = null;
            decimal TotalCollectionAmount = 0m, VoltageStabilizerComm = 0m,
                ExtraCommission = 0m, DSGNCommissionPercent = 0m;
            DesWiseCommission oDesWiseCommission = null;
            #endregion

            var Employees = _EmployeeService.GetAllEmployeeIQueryable().Where(i => EmployeeIDList.Contains(i.EmployeeID));
            var DESGCommissions = _DESGWiseCommissionService.GetAllIQueryable().Where(i => Employees.Select(j => j.DesignationID).Contains(i.DesignationID));
            var EmpCommissions = _empCommissionService.GetAll().Where(i => Employees.Select(j => j.EmployeeID).Contains(i.EmployeeID));

            var sysinfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(sysinfo.NextPayProcessDate);


            //if (User.Identity.GetConcernId() == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID
            //    || User.Identity.GetConcernId() == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID
            //    || User.Identity.GetConcernId() == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)
            //{
            //    EmployeeTarget = _TargetSetupService.GetAllIQueryable().FirstOrDefault(i => i.TargetMonth >= DateRange.Item1 && i.TargetMonth <= DateRange.Item2);
            //    IsShowroomCommission = true;
            //    TotalCollectionAmount = _SOrderService.GetAllCollectionAmountByDateRange(DateRange.Item1, DateRange.Item2);
            //    VoltageStabilizerComm = _SOrderService.GetVoltageStabilizerCommission(DateRange.Item1, DateRange.Item2);
            //    ExtraCommission = _SOrderService.GetExtraCommission(DateRange.Item1, DateRange.Item2, User.Identity.GetConcernId());
            //}

            foreach (var employeeID in EmployeeIDList)
            {
                oEmployee = Employees.FirstOrDefault(i => i.EmployeeID == employeeID);
                empGradesalary = _GradeSalaryAssignment.GetLastGradeSalaryByEmployeeID(employeeID);
                //if (IsShowroomCommission == false)
                //    EmployeeTarget = _TargetSetupService.GetByEmployeeIDandTargetMonth(employeeID, DateRange.Item1, DateRange.Item2);

                if (empGradesalary == null || IsErrorFound)
                {
                    salaryProcessError = new SalaryProcessErrorVM();
                    IsErrorFound = true;
                    if (empGradesalary == null)
                    {
                        salaryProcessError.Code = oEmployee.Code;
                        salaryProcessError.Name = oEmployee.Name;
                        salaryProcessError.Error = "Grade Salary not assigned.";
                        ErrorList.Add(salaryProcessError);
                    }
                    continue;
                }

                employeeCommission = EmpCommissions.FirstOrDefault(i => i.EmployeeID == employeeID);
                DSGNCommissionPercent = employeeCommission != null ? employeeCommission.CommissionAmt : 0m;
                salaryMonthly = new SalaryMonthly();

                salaryMonthly = _SalaryProcessorService.MonthlySalaryProcessByEmployeeID(oEmployee,
                    model.IsFestivalBonus,
                    empGradesalary,
                    sysinfo, EmployeeTarget, IsShowroomCommission,
                    TotalCollectionAmount, VoltageStabilizerComm,
                    ExtraCommission, DSGNCommissionPercent);

                var SMD = salaryMonthly.SalaryMonthlyDetails.FirstOrDefault(i => i.ItemID == (int)EnumSalaryItemCode.Net_Payable);
                if (SMD.CalculatedAmount < 0)
                {
                    AdvanceSalaries.Add(AddNextMonthAdvanceSalary(SMD, salaryMonthly.EmployeeID, salaryMonthly.SalaryMonth.Value));
                }
                salaryProcessNew.SalaryMonthlys.Add(salaryMonthly);
            }

            if (IsErrorFound)
            {
                SalaryProcessViewModel newModel = new SalaryProcessViewModel();
                newModel.IsFestivalBonus = model.IsFestivalBonus;
                newModel.SalaryProcessMonth = sysinfo.NextPayProcessDate;
                newModel.ErrorList.AddRange(ErrorList);
                return View("Index", newModel); ;
            }
            salaryProcessNew.ConcernID = User.Identity.GetConcernId();
            salaryProcessNew.CreatedBy = User.Identity.GetUserId<int>();
            salaryProcessNew.CreationDate = DateTime.Now;
            salaryProcessNew.IsFinalized = 0;
            salaryProcessNew.ProcessDate = DateTime.Now;
            salaryProcessNew.SalaryMonth = sysinfo.NextPayProcessDate;
            salaryProcessNew.Status = 1;
            salaryProcessNew.WorkDays = 0;
            _SalaryProcessorService.Add(salaryProcessNew);
            _SalaryProcessorService.Save();
            if (AdvanceSalaries.Count() > 0)
                SaveAdvanceSalary(salaryProcessNew, AdvanceSalaries);

            AddToastMessage("", "Salary Process Successfull.", ToastType.Success);

            return RedirectToAction("Index");
        }
        private void SaveAdvanceSalary(SalaryProcess newSalaryProcess, List<AdvanceSalary> advanceSalaries)
        {
            foreach (var item in advanceSalaries)
            {
                item.SalaryProcessID = newSalaryProcess.SalaryProcessID;
                _AdvanceSalaryService.Add(item);
            }
            _AdvanceSalaryService.Save();
        }
        private AdvanceSalary AddNextMonthAdvanceSalary(SalaryMonthlyDetail SMD, int EmployeeID, DateTime SalaryMonth)
        {
            AdvanceSalary advance = new AdvanceSalary();
            advance.Remarks = "System Generated for advance Salary carry forward if net payable is negative.";
            advance.Amount = -(SMD.CalculatedAmount);
            advance.Date = SalaryMonth.AddMonths(1);
            advance.EmployeeID = EmployeeID;
            advance.CreatedBy = User.Identity.GetUserId<int>();
            advance.CreatedDate = DateTime.Now;
            advance.ConcernID = User.Identity.GetConcernId();
            return advance;
        }

        [HttpGet]
        public ActionResult UndoSalaryProcess()
        {
            var All = _SalaryProcessorService.GetUndoAbleSalaryProcess();
            var vmall = _mapper.Map<IEnumerable<Tuple<int, DateTime, int>>, IEnumerable<SalaryProcessViewModel>>(All);
            return View(vmall);
        }


        public ActionResult Undo(int SalaryProcessID)
        {
            if (_SalaryProcessorService.UndoSalaryProcessUsingSP(SalaryProcessID))
                AddToastMessage("", "Salary Process undo Successfull.", ToastType.Success);
            else
                AddToastMessage("", "Salary Process undo Failed.", ToastType.Error);

            return RedirectToAction("UndoSalaryProcess");
        }

        public ActionResult GetSalarySheet()
        {
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            ViewBag.Departments = new SelectList(_DepartmentService.GetAllDepartment(), "DepartmentId", "DESCRIPTION");
            return View(new GetSalarySheetViewModel() { SalaryMonth = SysInfo.NextPayProcessDate });
        }

        public ActionResult GetEmployeesByDepartmentID(FormCollection formCollection)
        {
            int DepartmentID = 0;
            if (!string.IsNullOrEmpty(formCollection["DepartmentID"]))
                DepartmentID = Convert.ToInt32(formCollection["DepartmentID"]);
            var Employees = _EmployeeService.GetAllEmployeeIQueryable().Where(i => i.DepartmentID == DepartmentID).ToList();
            var vmEmployees = _mapper.Map<List<Employee>, List<GetEmployeeViewModel>>(Employees);
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            return View("GetSalarySheet", new GetSalarySheetViewModel() { Employees = vmEmployees, DepartmentID = DepartmentID, SalaryMonth = SysInfo.NextPayProcessDate });
        }

        public ActionResult GetPaySlip()
        {
            return View();
        }

        public ActionResult GetAdvanceSalaryReport()
        {
            return View();
        }

        public ActionResult FinalizeMonth()
        {
            ViewBag.SalaryMonth = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId()).NextPayProcessDate;
            return View();
        }

        bool IsFinalizeValid(FormCollection formcollection)
        {
            if (string.IsNullOrEmpty(formcollection["SalaryMonth"]))
            {
                AddToastMessage("", "Salary Month is required", ToastType.Error);
                return false;
            }
            else
            {
                DateTime sm = Convert.ToDateTime(formcollection["SalaryMonth"]);
                var Daterange = GetFirstAndLastDateOfMonth(sm);
                //if (!_AttendenceService.GetAllIQueryable().Any(i => i.AttendencMonth >= Daterange.Item1 && i.AttendencMonth <= Daterange.Item2))
                //{
                //    AddToastMessage("", "This month attendence is not yet uploaded.", ToastType.Error);
                //    return false;
                //}
                if (!_SalaryProcessorService.GetAllIQueryable().Any(i => i.SalaryMonth >= Daterange.Item1 && i.SalaryMonth <= Daterange.Item2))
                {
                    AddToastMessage("", "This month salary is not yet processed.", ToastType.Error);
                    return false;
                }
            }
            return true;
        }
        List<HolidayCalender> WeeklyCalenderSet(DateTime NextPayProcessDate)
        {
            List<HolidayCalender> holidayCalenders = new List<HolidayCalender>();
            HolidayCalender objHC = null;
            var DateRange = GetFirstAndLastDateOfMonth(NextPayProcessDate);
            for (DateTime date = DateRange.Item1; date <= DateRange.Item2; date = date.AddDays(1))
            {
                if (date.DayOfWeek.ToString().Equals(EnumDaysOfWeek.Friday.ToString()))
                {
                    objHC = new HolidayCalender();
                    objHC.Date = date.Date;
                    objHC.Description = EnumHolidayType.WeeklyHoliday.ToString();
                    objHC.Type = (int)EnumHolidayType.WeeklyHoliday;
                    objHC.CreatedBy = User.Identity.GetUserId<int>();
                    objHC.CreatedDate = DateTime.Now;
                    objHC.Status = (int)EnumActiveInactive.Active;
                    objHC.ConcernID = User.Identity.GetConcernId();
                    holidayCalenders.Add(objHC);
                }
            }
            return holidayCalenders;
        }

        [HttpPost]
        public ActionResult Finalize(FormCollection formcollection)
        {
            if (!IsFinalizeValid(formcollection))
                return RedirectToAction("FinalizeMonth");

            DateTime sm = Convert.ToDateTime(formcollection["SalaryMonth"]);
            var sysinfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

            var DateRange = GetFirstAndLastDateOfMonth(sm);

            var existingHoliday = _HolidayCalenderService.GetAllIQueryable();
            DateTime NextMonth = sysinfo.NextPayProcessDate.AddMonths(1);

            var SelectedholidayCalender = WeeklyCalenderSet(NextMonth);
            DateTime date = DateTime.MinValue;

            foreach (var item in SelectedholidayCalender)
            {
                date = Convert.ToDateTime(item.Date);
                if (existingHoliday.Any(i => i.Date == date))
                {
                    AddToastMessage("", item.Date + " is already assigned.", ToastType.Error);
                    return RedirectToAction("FinalizeMonth");
                }
            }

            DataTable dtholidays = CreateWeeklyHolidayCalenderDataTable(SelectedholidayCalender);

            if (_SalaryProcessorService.FinalizeSalaryMonthUsingSP(DateRange.Item1, DateRange.Item2, User.Identity.GetConcernId(), User.Identity.GetUserId<int>(), GetLocalDateTime(), NextMonth, dtholidays))
                AddToastMessage("", "Finalize Successfull.", ToastType.Success);
            else
                AddToastMessage("", "Finalize Failed.", ToastType.Error);

            return RedirectToAction("FinalizeMonth");
        }

        DataTable CreateWeeklyHolidayCalenderDataTable(List<HolidayCalender> weeklyHolidays)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Type", typeof(int));
            dt.Columns.Add("CreatedBy", typeof(int));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("ConcernID", typeof(int));
            DataRow row = null;
            foreach (var item in weeklyHolidays)
            {
                row = dt.NewRow();
                row["Date"] = item.Date;
                row["Description"] = item.Description;
                row["Type"] = item.Type;
                row["CreatedBy"] = item.CreatedBy;
                row["CreatedDate"] = item.CreatedDate;
                row["Status"] = item.Status;
                row["ConcernID"] = item.ConcernID;
                dt.Rows.Add(row);
            }
            return dt;
        }

    }
}
