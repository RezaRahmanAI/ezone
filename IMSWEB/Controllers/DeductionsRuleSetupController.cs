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
    public class DeductionsRuleSetupController : CoreController
    {
        IMapper _mapper;
        IAllowanceDeductionService _AllowanceDeductionService;
        IMiscellaneousService<AllowanceDeduction> _miscell;
        IADParameterBasicService _ADParameterBasicService;
        IADParamADParameterGradeService _ADParamADParameterGradeService;
        public DeductionsRuleSetupController(IErrorService errorService, IAllowanceDeductionService allowanceDeductionService,
            IMapper mapper, IMiscellaneousService<AllowanceDeduction> miscell, IADParameterBasicService aDParameterBasicService,
            IADParamADParameterGradeService ADParamADParameterGradeService, ISystemInformationService sysInfoService
            )
            : base(errorService, sysInfoService)
        {
            _AllowanceDeductionService = allowanceDeductionService;
            _mapper = mapper;
            _miscell = miscell;
            _ADParameterBasicService = aDParameterBasicService;
            _ADParamADParameterGradeService = ADParamADParameterGradeService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var adparambasics = _ADParameterBasicService.GetAllowancesDeductionRuleSetupAsPerGrade((int)EnumAllowOrDeduct.Deduction);
            var vmadparambasics = _mapper.Map<IEnumerable<Tuple<int, List<string>, string, string, string, string>>, IEnumerable<ADParameterBasicCreate>>(adparambasics);
            return View(vmadparambasics);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }
        private ActionResult ReturnCreateViewWithTempData()
        {
            var allowances = _AllowanceDeductionService.GetAll().Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Deduction);

            var CreateAllowancesRuleSetupViewModel = (CreateAllowancesRuleSetupViewModel)TempData.Peek("CreateAllowancesRuleSetupViewModel");
            if (CreateAllowancesRuleSetupViewModel != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["CreateAllowancesRuleSetupViewModel"] = CreateAllowancesRuleSetupViewModel;
                return View("Create", CreateAllowancesRuleSetupViewModel);
            }
            else
            {
                var allowid = allowances.FirstOrDefault().AllowDeductID;
                var UnassingGrades = _ADParameterBasicService.GetUnassignedGrades(allowid);
                var vmGrades = _mapper.Map<List<Grade>, List<GradeViewModel>>(UnassingGrades);
                return View(new CreateAllowancesRuleSetupViewModel
                {
                    ADParameterBasicCreate = new ADParameterBasicCreate() { Periodicity = EnumPeriodicity.Monthly, AllowDeductID = allowid.ToString() },
                    ADParameterEmployeeCreate = new ADParameterEmployeeCreate(),
                    ADParameterGradeCreate = new ADParameterGradeCreate(),
                    Grades = vmGrades
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAllowancesRuleSetupViewModel CreateAllowancesRuleSetupViewModel, FormCollection formcollection)
        {
            return HandleAllowanceSetup(CreateAllowancesRuleSetupViewModel, formcollection);
        }

        private void CreateADParamGrades(CreateAllowancesRuleSetupViewModel NewCreateAllowancesRuleSetupViewModel, ADParameterBasic ADParameterBasic, FormCollection formCollection)
        {
            if (NewCreateAllowancesRuleSetupViewModel.Grades != null)
            {
                ADParameterGradeCreate adParamGrade = null;
                NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList = NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList == null ? new List<ADParameterGradeCreate>() : NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList;
                string[] Grades = formCollection.AllKeys
                       .Where(key => key.StartsWith("GID"))
                       .Select(key => formCollection[key])
                       .ToArray();

                for (int i = 0; i < Grades.Count(); i++)
                {
                    adParamGrade = new ADParameterGradeCreate();
                    adParamGrade.ADParameterID = ADParameterBasic.ADParameterID;
                    adParamGrade.GradeID = Convert.ToInt32(Grades[i]);
                    NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList.Add(adParamGrade);
                }
            }
        }

        private void CheckSaveError(CreateAllowancesRuleSetupViewModel NewCreateAllowancesRuleSetupViewModel, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.AllowDeductID))
                ModelState.AddModelError("ADParameterBasicCreate.AllowDeductID", "Allowance is required.");
            if (NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.FlatAmount == 0 && NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.PercentOfBasic == 0 && NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.PercentOfGross == 0)
                ModelState.AddModelError("ADParameterBasicCreate.PercentOfBasic", "FlatAmount or PercentOfBasic or PercentOfGross is required.");

            string[] Grades = formCollection.AllKeys
                     .Where(key => key.StartsWith("GID"))
                     .Select(key => formCollection[key])
                     .ToArray();

            if (Grades.Count() == 0)
            {
                ModelState.AddModelError("ADParameterBasicCreate.GradeName", "");
                AddToastMessage("", "Please select Grade.", ToastType.Error);
            }
            GradeViewModel grade = null;
            for (int i = 0; i < Grades.Count(); i++)
            {
                grade = new GradeViewModel();
                grade = NewCreateAllowancesRuleSetupViewModel.Grades.FirstOrDefault(j => j.GradeID == Convert.ToInt32(Grades[i]));
                if (grade != null)
                    grade.IsSelected = true;
            }
        }
        private ActionResult HandleAllowanceSetup(CreateAllowancesRuleSetupViewModel NewCreateAllowancesRuleSetupViewModel, FormCollection formcollection)
        {
            if (NewCreateAllowancesRuleSetupViewModel != null)
            {
                GetPickerValue(NewCreateAllowancesRuleSetupViewModel, formcollection);
                if (!string.IsNullOrEmpty(NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.AllowDeductID))
                {
                    var grades = _ADParameterBasicService.GetUnassignedGrades(Convert.ToInt32(NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.AllowDeductID));
                    NewCreateAllowancesRuleSetupViewModel.Grades = _mapper.Map<IEnumerable<Grade>, IEnumerable<GradeViewModel>>(grades).ToList();
                }

                if (!string.IsNullOrEmpty(formcollection.Get("submitButton")))
                {
                    CheckSaveError(NewCreateAllowancesRuleSetupViewModel, formcollection);
                    if (!ModelState.IsValid)
                        return View("Create", NewCreateAllowancesRuleSetupViewModel);
                    Save(NewCreateAllowancesRuleSetupViewModel, formcollection);
                    return RedirectToAction("Index");
                }
                return View(NewCreateAllowancesRuleSetupViewModel);
            }
            return View(new CreateAllowancesRuleSetupViewModel() { ADParameterBasicCreate = new ADParameterBasicCreate() { Periodicity = EnumPeriodicity.Monthly } });
        }

        private void GetPickerValue(CreateAllowancesRuleSetupViewModel NewCreateAllowancesRuleSetupViewModel, FormCollection formcollection)
        {
            if (!string.IsNullOrEmpty(formcollection["AllowancesId"]))
            {
                NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.AllowDeductID = formcollection["AllowancesId"];
            }
            if (NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.IsIndividual)
            {
                NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.EntitleType = (int)EnumEntitleType.Individual;
            }
            else
            {
                NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.EntitleType = (int)EnumEntitleType.Grade;
            }
        }

        private void Save(CreateAllowancesRuleSetupViewModel NewCreateAllowancesRuleSetupViewModel, FormCollection formcollection)
        {
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.ConcernID = User.Identity.GetConcernId();
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.CreatedBy = User.Identity.GetUserId<int>();
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.CreateDate = DateTime.Now;
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.AllowOrDeduct = (int)EnumAllowOrDeduct.Deduction;
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.LastActivationDate = DateTime.Now;
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.IsCurrentlyActive = 1;
            NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.Status = 1;
            var adparambasic = _mapper.Map<ADParameterBasicCreate, ADParameterBasic>(NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate);

            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                var ADParamBasic = _ADParameterBasicService.GetById(NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.ADParameterID);
                ADParamBasic.EntitleType = NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.EntitleType;
                ADParamBasic.FlatAmount = NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.FlatAmount;
                ADParamBasic.PercentOfGross = NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.PercentOfGross;
                ADParamBasic.PercentOfBasic = NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.PercentOfBasic;
                ADParamBasic.ModifiedBy = User.Identity.GetUserId<int>();
                ADParamBasic.ModifiedDate = DateTime.Now;
                _ADParameterBasicService.Save();
                CreateADParamGrades(NewCreateAllowancesRuleSetupViewModel, adparambasic, formcollection);
                var adparamGrades = _mapper.Map<IEnumerable<ADParameterGradeCreate>, IEnumerable<ADParameterGrade>>(NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList);

                var OldADParamGrades = _ADParameterBasicService.GetAll().Where(i => i.ADParameterID == NewCreateAllowancesRuleSetupViewModel.ADParameterBasicCreate.ADParameterID);
                foreach (var item in OldADParamGrades)
                {
                    _ADParamADParameterGradeService.Delete(item.ADParameterID);
                }

                foreach (var item in adparamGrades)
                {
                    _ADParamADParameterGradeService.Add(item);
                }
                _ADParamADParameterGradeService.Save();
                AddToastMessage("", "Update Successfull", ToastType.Success);
            }
            else
            {
                _ADParameterBasicService.Add(adparambasic);
                _ADParameterBasicService.Save();
                CreateADParamGrades(NewCreateAllowancesRuleSetupViewModel, adparambasic, formcollection);
                var adparamGrades = _mapper.Map<IEnumerable<ADParameterGradeCreate>, IEnumerable<ADParameterGrade>>(NewCreateAllowancesRuleSetupViewModel.ADParameterGradeCreateList);

                foreach (var item in adparamGrades)
                {
                    _ADParamADParameterGradeService.Add(item);
                }
                _ADParamADParameterGradeService.Save();
                AddToastMessage("", "Save Successfull", ToastType.Success);
            }


        }


        public JsonResult GetUnassignGradesByAllowanceID(int AllowanceDeductID)
        {
            var Grades = _ADParameterBasicService.GetUnassignedGrades(AllowanceDeductID);
            if (Grades.Count() != 0)
            {
                return Json(new { Status = true, Grades = Grades }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Grades = Grades }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var ADParamBasic = _ADParameterBasicService.GetById(id);
            var vmADParamBasic = _mapper.Map<ADParameterBasic, ADParameterBasicCreate>(ADParamBasic);
            var ADParamGrades = _ADParamADParameterGradeService.GetGradesByADParamID(id);
            var unassinggrades = _ADParameterBasicService.GetUnassignedGrades(ADParamBasic.AllowDeductID);
            var vmGrades = _mapper.Map<List<Grade>, List<GradeViewModel>>(ADParamGrades);
            var vmunassinggrades = _mapper.Map<List<Grade>, List<GradeViewModel>>(unassinggrades);
            vmGrades.Select(c => { c.IsSelected = true; return c; }).ToList();
            vmGrades.AddRange(vmunassinggrades);
            return View("Create", new CreateAllowancesRuleSetupViewModel
            {
                ADParameterBasicCreate = vmADParamBasic,
                ADParameterEmployeeCreate = new ADParameterEmployeeCreate(),
                ADParameterGradeCreate = new ADParameterGradeCreate(),
                Grades = vmGrades
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateAllowancesRuleSetupViewModel CreateAllowancesRuleSetupViewModel, FormCollection formcollection)
        {
            return HandleAllowanceSetup(CreateAllowancesRuleSetupViewModel, formcollection);
        }
    }
}