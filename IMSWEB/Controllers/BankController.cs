
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
    [Authorize]
    [RoutePrefix("bank")]
    public class BankController : CoreController
    {
        private readonly IBankService _bankService;
        private readonly IMiscellaneousService<Bank> _miscellaneousService;
        private readonly ISystemInformationService _SysInfoService;
        private readonly IMapper _mapper;
        private readonly ISisterConcernService _sisterConcernService;

        public BankController(IErrorService errorService,
            IBankService bankService, IMiscellaneousService<Bank> miscellaneousService, ISystemInformationService SysInfoService,
            IMapper mapper, ISisterConcernService sisterConcernService)
            : base(errorService, SysInfoService)
        {
            _bankService = bankService;
            _miscellaneousService = miscellaneousService;
            _SysInfoService = SysInfoService;
            _sisterConcernService = sisterConcernService;
            _mapper = mapper;


        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            ViewBag.IsBankBalanceShow = _SysInfoService.IsBankBalanceShow();

            var systemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            int concernId = User.Identity.GetConcernId();
            if (systemInfo.IsCommonBank)
                concernId = _sisterConcernService.GetParentConcernId(concernId);

            var banksAsync = _bankService.GetAllBankByParentConcernAsync(concernId);
            var vmodel = _mapper.Map<IEnumerable<Bank>, IEnumerable<CreateBankViewModel>>(await banksAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateBankViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateBankViewModel newBank, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newBank);

            if (newBank != null)
            {
                var systemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var bank = _mapper.Map<CreateBankViewModel, Bank>(newBank);

                #region common Bank
                if (systemInfo.IsCommonBank)
                    bank.ConcernID = _sisterConcernService.GetParentConcernId(User.Identity.GetConcernId());
                else
                    bank.ConcernID = User.Identity.GetConcernId();
                #endregion

                var existingBank = _miscellaneousService.GetDuplicateEntry(c => c.BankName == newBank.BankName && c.AccountNo == newBank.AccountNo && c.ConcernID == bank.ConcernID);
                if (existingBank != null)
                {
                    AddToastMessage("", "A Bank with same name and account number already exists in the system. Please try with a different.", ToastType.Error);
                    return View(newBank);
                }                

               

               
                bank.IsAdvancedDueLimitApplicable = newBank.IsAdvancedDueLimitApplicable ? 1 : 0;
                bank.AdvancedAmountLimit = newBank.AdvancedAmountLimit;


                AddAuditTrail(bank, true);
                _bankService.AddBank(bank);
                _bankService.SaveBank();

                

                if (systemInfo.IsBankEnable == 1)
                {
                    AddToSisterConcern(newBank);
                }
                AddToastMessage("", "Bank has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Bank data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var bank = _bankService.GetBankById(id);
            var vmodel = _mapper.Map<Bank, CreateBankViewModel>(bank);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateBankViewModel newBank, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newBank);

            if (newBank != null)
            {
                var systemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var bank = _bankService.GetBankById(int.Parse(newBank.Id));

                #region common Bank
                if (systemInfo.IsCommonBank)
                    bank.ConcernID = _sisterConcernService.GetParentConcernId(User.Identity.GetConcernId());
                else
                    bank.ConcernID = User.Identity.GetConcernId();
                #endregion

                var existingBank = _miscellaneousService.GetDuplicateEntry(c => c.BankName == newBank.BankName && c.AccountNo == newBank.AccountNo && c.ConcernID == bank.ConcernID && c.BankID != bank.BankID);
                if (existingBank != null)
                {
                    AddToastMessage("", "A Bank with same name and account number already exists in the system. Please try with a different.", ToastType.Error);
                    return View(newBank);
                }

                bank.Code = newBank.Code;
                bank.BankName = newBank.BankName;
                bank.AccountName = newBank.AccountName;
                bank.BranchName = newBank.BranchName;
                bank.IsAdvancedDueLimitApplicable = newBank.IsAdvancedDueLimitApplicable ? 1 : 0;
                bank.AdvancedAmountLimit = newBank.AdvancedAmountLimit;
                bank.AccountNo = newBank.AccountNo;


                AddAuditTrail(bank, false);
                _bankService.UpdateBank(bank);
                _bankService.SaveBank();

                AddToastMessage("", "Bank has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Bank found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _bankService.DeleteBank(id);
            _bankService.SaveBank();
            AddToastMessage("", "Bank has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        #region add bank to sister concern
        private void AddToSisterConcern(CreateBankViewModel bank)
        {
            List<SisterConcern> families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                    .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                Bank newBank = new Bank();
                foreach (var concern in families)
                {
                    var allBank = _bankService.GetAll(concern.ConcernID);
                    if (allBank.Any(i => i.BankName.ToLower().Equals(bank.BankName.Trim().ToLower()) && i.AccountNo.ToLower().Equals(bank.AccountNo.Trim().ToLower())))
                        continue;

                    newBank = _mapper.Map<CreateBankViewModel, Bank>(bank);
                    newBank.IsAdvancedDueLimitApplicable = bank.IsAdvancedDueLimitApplicable ? 1 : 0;
                    newBank.AdvancedAmountLimit = bank.AdvancedAmountLimit;
                    newBank.ConcernID = concern.ConcernID;

                    _bankService.AddBank(newBank);
                    _bankService.SaveBank();
                }
            }
        }
        #endregion
    }
}