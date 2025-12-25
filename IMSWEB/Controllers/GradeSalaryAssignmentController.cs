using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class GradeSalaryAssignmentController : CoreController
    {

        IMapper _mapper;
        IEmpGradeSalaryAssignmentService _GradeSalaryAssingmentService;
        IMiscellaneousService<EmpGradeSalaryAssignmentService> _miscell;
        IGradeService _gradeService;
        IGradeSalaryChangeTypeService _gradeSalaryChangeTypeService;
        IEmployeeService _EmployeeService;
        public GradeSalaryAssignmentController(IErrorService errorService, IEmpGradeSalaryAssignmentService GradeSalaryAssingmentService,
            IGradeSalaryChangeTypeService gradeSalaryChangeTypeService, IEmployeeService EmployeeService,
            IGradeService gradeService, IMapper Mapper, IMiscellaneousService<EmpGradeSalaryAssignmentService> miscell, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _GradeSalaryAssingmentService = GradeSalaryAssingmentService;
            _mapper = Mapper;
            _miscell = miscell;
            _gradeService = gradeService;
            _gradeSalaryChangeTypeService = gradeSalaryChangeTypeService;
            _EmployeeService = EmployeeService;
        }
        public ActionResult Index()
        {
            ViewBag.Grades = new SelectList(_gradeService.GetAll(), "GradeID", "Description");
            ViewBag.ChangeTypes = new SelectList(_gradeSalaryChangeTypeService.GetAll(), "GradeSalaryChangeTypeID", "Name");

            if (TempData.ContainsKey("EmployeeID") && TempData["EmployeeID"] != null)
            {
                int EmployeeID = Convert.ToInt32(TempData["EmployeeID"]);
                var empGSAssignment = _GradeSalaryAssingmentService.GetAllByEmployeeID(EmployeeID);
                var VMempGSAssignment = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal?, decimal, string>>, IEnumerable<EmpGradeSalaryAssignmentViewModel>>(empGSAssignment);
                return View(new EmpGradeSalaryAssignmentViewModel() { EffectDate = DateTime.Now, EmpGradeSalaryAssignmentViewModels = VMempGSAssignment.ToList(), EmployeeID = EmployeeID.ToString() });
            }


            return View(new EmpGradeSalaryAssignmentViewModel() { EffectDate = DateTime.Now });
        }

        void AddModelError(EmpGradeSalaryAssignmentViewModel empGradeSalaryAssignmentViewModel, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                ModelState.AddModelError("EmployeeID", "Employee is Required");
            }
            else
                empGradeSalaryAssignmentViewModel.EmployeeID = formCollection["EmployeesId"];

            if (empGradeSalaryAssignmentViewModel.Type == null)
                ModelState.AddModelError("Type", "Change Type is Required");

            if (empGradeSalaryAssignmentViewModel.GradeID == null)
                ModelState.AddModelError("GradeID", "Grade is Required");

            if (string.IsNullOrEmpty(formCollection["EffectDate"]))
            {
                ModelState.AddModelError("EffectDate", "Effect Date is Required");
            }
            else
                empGradeSalaryAssignmentViewModel.EffectDate = Convert.ToDateTime(formCollection["EffectDate"]);

        }
        [HttpPost]
        public ActionResult Index(EmpGradeSalaryAssignmentViewModel empGradeSalaryAssignmentViewModel, FormCollection formCollection)
        {
            ViewBag.Grades = new SelectList(_gradeService.GetAll(), "GradeID", "Description");
            ViewBag.ChangeTypes = new SelectList(_gradeSalaryChangeTypeService.GetAll(), "GradeSalaryChangeTypeID", "Name");

            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                empGradeSalaryAssignmentViewModel.EmployeeID = formCollection["EmployeesId"];
            }

            if (!string.IsNullOrEmpty(formCollection.Get("btnSubmit")))
            {
                AddModelError(empGradeSalaryAssignmentViewModel, formCollection);

                empGradeSalaryAssignmentViewModel.ConcernID = User.Identity.GetConcernId();
                empGradeSalaryAssignmentViewModel.CreateDate = DateTime.Now;
                empGradeSalaryAssignmentViewModel.EntryDate = DateTime.Now;
                empGradeSalaryAssignmentViewModel.CreatedBy = User.Identity.GetUserId<int>();
                empGradeSalaryAssignmentViewModel.Status = (int)EnumActiveInactive.Active;
                var empAllgradsalary = _GradeSalaryAssingmentService.GetAll().Where(i => i.EmployeeID == int.Parse(empGradeSalaryAssignmentViewModel.EmployeeID) && i.Status == (int)EnumActiveInactive.Active);
                empGradeSalaryAssignmentViewModel.GradeSalaryID = empAllgradsalary.Count() == 0 ? 1 : empAllgradsalary.Count() + 1;
                if (empAllgradsalary.Count() != 0)
                {
                    var lastempGSA = empAllgradsalary.OrderByDescending(i => i.EmpGradeSalaryID).FirstOrDefault();
                    lastempGSA.TillDate = DateTime.Now;
                }
                if (!ModelState.IsValid)
                {
                    var tempempGSAssignment = _GradeSalaryAssingmentService.GetAllByEmployeeID(Convert.ToInt32(empGradeSalaryAssignmentViewModel.EmployeeID));
                    var tempVMempGSAssignment = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal?, decimal, string>>, IEnumerable<EmpGradeSalaryAssignmentViewModel>>(tempempGSAssignment);
                    empGradeSalaryAssignmentViewModel.EmpGradeSalaryAssignmentViewModels = tempVMempGSAssignment.ToList();

                    return View(empGradeSalaryAssignmentViewModel);
                }

                var empGSA = _mapper.Map<EmpGradeSalaryAssignmentViewModel, EmpGradeSalaryAssignment>(empGradeSalaryAssignmentViewModel);

                _GradeSalaryAssingmentService.Add(empGSA);
                _GradeSalaryAssingmentService.Save();

                var Employee = _EmployeeService.GetEmployeeById(empGSA.EmployeeID);
                Employee.GradeID = empGSA.GradeID;
                Employee.GrossSalary = (decimal)empGSA.GrossSalary;
                _EmployeeService.SaveEmployee();

                AddToastMessage("", "Grade and Salary Assign successfully.", ToastType.Success);
                TempData["EmployeeID"] = empGradeSalaryAssignmentViewModel.EmployeeID;
                return RedirectToAction("Index");
            }


            var empGSAssignment = _GradeSalaryAssingmentService.GetAllByEmployeeID(Convert.ToInt32(empGradeSalaryAssignmentViewModel.EmployeeID));
            var VMempGSAssignment = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal?, decimal, string>>, IEnumerable<EmpGradeSalaryAssignmentViewModel>>(empGSAssignment);
            empGradeSalaryAssignmentViewModel.EmpGradeSalaryAssignmentViewModels = VMempGSAssignment.ToList();


            return View(empGradeSalaryAssignmentViewModel);
        }

        public ActionResult DeleteFromView(int id, string previousAction, int EmployeeID, FormCollection formCollection)
        {

            if (EmployeeID != 0)
            {
                TempData["EmployeeID"] = EmployeeID;
                var empAllgradsalary = _GradeSalaryAssingmentService.GetAll().Where(i => i.EmployeeID == EmployeeID && i.Status == (int)EnumActiveInactive.Active);
                if (empAllgradsalary.Any(i => i.EmpGradeSalaryID > id))
                {
                    AddToastMessage("", "You can delete only the last information.", ToastType.Error);
                    return RedirectToAction(previousAction);
                }
                _GradeSalaryAssingmentService.Delete(id);
                _GradeSalaryAssingmentService.Save();
                AddToastMessage("", "Grade and Salary has been deleted successfully.", ToastType.Success);
            }
            else
            {

            }


            return RedirectToAction(previousAction);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult CalculateBasicFromGross(int GradeID, decimal GrossSalary)
        {
            decimal BasicSalary = 0;
            if (GradeID != 0)
            {
                var Grade = _gradeService.GetById(GradeID);
                if (Grade != null)
                {
                    BasicSalary = Math.Round((Grade.BasicPercentOfGross * GrossSalary) / 100m, 2);
                }
            }
            return Json(BasicSalary, JsonRequestBehavior.AllowGet);

        }
    }
}