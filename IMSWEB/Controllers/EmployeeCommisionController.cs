using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class EmployeeCommisionController : CoreController
    {
        IEmployeeCommissionService _employeeCommissionService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;
        IEmployeeService _employeeService;
        private readonly ISalaryProcessorService _salaryProcessorService;
        private readonly IDepartmentService _departmentService;

        public EmployeeCommisionController(IErrorService errorService,
            IEmployeeCommissionService commissionService, ISystemInformationService SysService,
            IMapper mapper, IEmployeeService employeeService,
            ISalaryProcessorService salaryProcessorService,
            IDepartmentService departmentService)
            : base(errorService, SysService)
        {
            _employeeCommissionService = commissionService;
            _mapper = mapper;
            _sysInfoService = SysService;
            _employeeService = employeeService;
            _salaryProcessorService = salaryProcessorService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(PopulateDropdown(new CreateEmpCommissionVM()));
        }

        [HttpPost]
        public ActionResult Index(CreateEmpCommissionVM newEmpCommission, FormCollection formCollection)
        {
            if (formCollection.Get("btnSave") != null)
            {
                if (!IsValid(newEmpCommission))
                {
                    return View(PopulateDropdown(newEmpCommission));
                }

                if (SaveCommission(newEmpCommission))
                {
                    AddToastMessage("", "Save successfull", ToastType.Success);
                    return RedirectToAction("Index");
                }
                AddToastMessage("", "Save failed.", ToastType.Error);
                return View(PopulateDropdown(newEmpCommission));
            }
            else
            {

                var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
                string CommissionMonth = sysInfo.NextPayProcessDate.ToString("MMM yyyy");
                var attendences = _employeeCommissionService.GetAll()
                                  .Where(i => i.CommissionMonth >= DateRange.Item1 && i.CommissionMonth <= DateRange.Item2).ToList();

                var emp = (from e in _employeeService.GetAllEmployeeDetails(newEmpCommission.DepartmentID)
                           join ma in attendences on e.Item1 equals ma.EmployeeID into lma
                           from ma in lma.DefaultIfEmpty()
                           select new EmpCommissionVM
                           {
                               ECID = ma != null ? ma.ECID : 0,
                               EmployeeID = e.Item1,
                               EmployeeName = e.Item3,
                               DepartmentName = e.Rest.Item1,
                               DesignationName = e.Item7,
                               IsSelect = ma != null ? true : false,
                               CommissionAmt = ma != null ? ma.CommissionAmt : 0,
                               CommissionMonth = CommissionMonth
                           }).OrderByDescending(i => i.ECID).OrderByDescending(i => i.DepartmentName).ToList();

                newEmpCommission.Commissions = emp;
                ModelState.Clear();
                return View(PopulateDropdown(newEmpCommission));
            }

        }

        private CreateEmpCommissionVM PopulateDropdown(CreateEmpCommissionVM createEmpCommissionVM)
        {
            createEmpCommissionVM.Departments = _mapper.Map<IQueryable<Department>, List<DepartmentViewModel>>
                (_departmentService.GetAllDepartmentIQueryable());

            return createEmpCommissionVM;
        }
        private bool IsValid(CreateEmpCommissionVM createMonthlyAttendence)
        {
            var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            bool Result = true;
            if (createMonthlyAttendence.Commissions.Count() == 0)
            {
                AddToastMessage("", "Data is not found");
                return false;
            }

            for (int i = 0; i < createMonthlyAttendence.Commissions.Count(); i++)
            {
                if (createMonthlyAttendence.Commissions[i].CommissionAmt < 0m
                    && createMonthlyAttendence.Commissions[i].IsSelect)
                {
                    ModelState.AddModelError("Commissions[" + i + "].CommissionAmt", "Commission Amt can't be negative");
                    Result = false;
                }
            }

            if (!Result)
            {
                return false;
            }

            var daterange = GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
            for (int i = 0; i < createMonthlyAttendence.Commissions.Count(); i++)
            {
                if (createMonthlyAttendence.Commissions[i].IsSelect
                    && _salaryProcessorService.IsSalaryProcessed(createMonthlyAttendence.Commissions[i].EmployeeID, daterange))
                {
                    ModelState.AddModelError("Commissions[" + i + "].CommissionAmt", "This employee's salary is already processed");
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

        private bool SaveCommission(CreateEmpCommissionVM CreateMonthlyAttendence)
        {
            bool result = true;
            try
            {
                EmployeeCommission manual = null;
                var sysInfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                var newAtt = CreateMonthlyAttendence.Commissions
                    .Where(i => i.IsSelect == true && i.ECID == 0).ToList();

                var deleted = CreateMonthlyAttendence.Commissions
                    .Where(i => i.IsSelect == false && i.ECID > 0).ToList();

                var updated = CreateMonthlyAttendence.Commissions
                    .Where(i => i.IsSelect == true && i.ECID > 0).ToList();

                foreach (var item in deleted)
                {
                    _employeeCommissionService.Delete((int)item.ECID);
                }

                foreach (var item in newAtt)
                {
                    manual = new EmployeeCommission();
                    manual.CommissionMonth = Convert.ToDateTime(item.CommissionMonth);
                    manual.EmployeeID = item.EmployeeID;
                    manual.CommissionAmt = item.CommissionAmt;
                    AddAuditTrail(manual, true);
                    _employeeCommissionService.Add(manual);
                }

                foreach (var item in updated)
                {
                    manual = _employeeCommissionService.GetById((int)item.ECID);
                    manual.CommissionMonth = Convert.ToDateTime(item.CommissionMonth);
                    manual.EmployeeID = item.EmployeeID;
                    manual.CommissionAmt = item.CommissionAmt;
                    AddAuditTrail(manual, false);
                    _employeeCommissionService.Update(manual);
                }

                _employeeCommissionService.Save();
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