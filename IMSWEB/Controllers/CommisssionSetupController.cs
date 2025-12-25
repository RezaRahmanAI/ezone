using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class CommissionSetupController : CoreController
    {
        ICommisssionSetupService _CommisssionService;
        IMiscellaneousService<CommissionSetup> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _SysInfo;
        public CommissionSetupController(IErrorService errorService,
            ICommisssionSetupService commissionService, IMiscellaneousService<CommissionSetup> miscellaneousService,
            ISystemInformationService SysInfo, IMapper mapper)
            : base(errorService, SysInfo)
        {
            _CommisssionService = commissionService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SysInfo = SysInfo;
        }
        public async Task<ActionResult> Index()
        {
            var Commission = await _CommisssionService.GetAllAsync();
            var vmCommission = _mapper.Map<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>, IEnumerable<CommissionSetupViewModel>>(Commission);
            return View(vmCommission);
        }

        public ActionResult Create()
        {
            return View();
        }

        void AddModelError(CommissionSetupViewModel NewCommissionVM, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            }
            else
                NewCommissionVM.EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);

            if (!string.IsNullOrEmpty(formCollection["CommissionMonth"]))
            {
                NewCommissionVM.CommissionMonth = Convert.ToDateTime(formCollection["CommissionMonth"]);
                var SysInfo = _SysInfo.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
                int CSID = Convert.ToInt32(NewCommissionVM.CSID);
                if (NewCommissionVM.CommissionMonth < DateRange.Item1)
                {
                    ModelState.AddModelError("CommissionMonth", "Commission Month can't be smaller than Salary Process Month.");
                }

                var ExisitingCommissions = _CommisssionService.GetByEmployeeIDandMonth(NewCommissionVM.EmployeeID, DateRange.Item1, DateRange.Item2);
                if (ExisitingCommissions.Any(i => NewCommissionVM.AchievedPercentStart >= i.AchievedPercentStart && NewCommissionVM.AchievedPercentStart <= i.AchievedPercentEnd && i.CSID != CSID))
                {
                    ModelState.AddModelError("AchievedPercentStart", "This percentage is already exsits.");
                }
                if (ExisitingCommissions.Any(i => NewCommissionVM.AchievedPercentEnd >= i.AchievedPercentStart && NewCommissionVM.AchievedPercentEnd <= i.AchievedPercentEnd && i.CSID != CSID))
                {
                    ModelState.AddModelError("AchievedPercentEnd", "This percentage is already exsits.");
                }
            }
            else
            {
                ModelState.AddModelError("CommissionMonth", "Commission Month is required.");
            }
        }

        [HttpPost]
        public ActionResult Create(CommissionSetupViewModel NewCommissionVM, FormCollection formCollection)
        {
            AddModelError(NewCommissionVM, formCollection);
            if (!ModelState.IsValid)
                return View(NewCommissionVM);

            var Commission = _mapper.Map<CommissionSetupViewModel, CommissionSetup>(NewCommissionVM);
            AddAuditTrail(Commission, true);
            Commission.ConcernID = User.Identity.GetConcernId();
            Commission.Status = (int)EnumActiveInactive.Active;
            _CommisssionService.Add(Commission);
            _CommisssionService.Save();
            AddToastMessage("", "Commission Setup Successfull.", ToastType.Success);
            return RedirectToAction("Create");
        }

        public ActionResult Edit(int id)
        {
            var Commission = _CommisssionService.GetById(id);
            var vmCommission = _mapper.Map<CommissionSetup, CommissionSetupViewModel>(Commission);
            return View("Create", vmCommission);
        }

        [HttpPost]
        public ActionResult Edit(CommissionSetupViewModel NewCommissionVM, FormCollection formCollection)
        {
            AddModelError(NewCommissionVM, formCollection);
            if (!ModelState.IsValid)
                return View("Create", NewCommissionVM);

            var commisssion = _CommisssionService.GetById(Convert.ToInt32(NewCommissionVM.CSID));
            AddAuditTrail(commisssion, false);
            commisssion.AchievedPercentEnd = NewCommissionVM.AchievedPercentEnd;
            commisssion.AchievedPercentStart = NewCommissionVM.AchievedPercentStart;
            commisssion.CommissionMonth = NewCommissionVM.CommissionMonth;
            commisssion.CommisssionAmt = NewCommissionVM.CommisssionAmt;
            commisssion.CommissionPercent = NewCommissionVM.CommissionPercent;
            commisssion.Status = (int)EnumActiveInactive.Active;
            commisssion.EmployeeID = NewCommissionVM.EmployeeID;
            _CommisssionService.Update(commisssion);
            _CommisssionService.Save();
            AddToastMessage("", "Update Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            _CommisssionService.Delete(id);
            _CommisssionService.Save();
            AddToastMessage("", "Delete Successfull.", ToastType.Success);
            return RedirectToAction("Index");
        }

    }
}
