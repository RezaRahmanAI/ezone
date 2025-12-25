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
    public class InvestmentController : CoreController
    {
        IMapper _mapper;
        IShareInvestmentService _ShareInvestmentService;
        IMiscellaneousService<ShareInvestment> _miscell;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        ISisterConcernService _SisterConcernService;
        public InvestmentController(IErrorService errorService, IShareInvestmentService headService, IMapper Mapper,
            IMiscellaneousService<ShareInvestment> miscell, IShareInvestmentHeadService ShareInvestmentHeadService, ISisterConcernService SisterConcernService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _ShareInvestmentService = headService;
            _mapper = Mapper;
            _miscell = miscell;
            _ShareInvestmentHeadService = ShareInvestmentHeadService;
            _SisterConcernService = SisterConcernService;
        }
        ShareInvestmentViewModel PopulateDropdown(ShareInvestmentViewModel model, EnumInvestmentType investmentType)
        {
            model.Heads = _ShareInvestmentHeadService.GetAll()
                 .Where(i => i.ParentId == (int)investmentType).ToList();
            return model;
        }
        ShareInvestmentViewModel GetInvestmentHedDDL(ShareInvestmentViewModel model)
        {
            if(model == null)
                model = new ShareInvestmentViewModel();
            model.Heads = _ShareInvestmentHeadService.GetAll().Where(d => (d.ParentId == (int)EnumInvestmentType.Liability) || d.ParentId == (int)EnumInvestmentType.PF || (d.ParentId == (int)EnumInvestmentType.FDR) || (d.ParentId == (int)EnumInvestmentType.Security)).ToList();

            return model;
        }

        private async Task<IEnumerable<ShareInvestmentViewModel>> GetIndexData(EnumInvestmentType investmentType, DateTime FromDate, DateTime ToDate, int InvestTransType)
        {
            var IVHeads = _ShareInvestmentService.GetAllAsync(investmentType, FromDate, ToDate, InvestTransType);
            var vmheads = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, decimal>>, IEnumerable<ShareInvestmentViewModel>>(await IVHeads);
            return vmheads;
        }

        private async Task<IEnumerable<ShareInvestmentViewModel>> GetLiabilityIndexData(EnumInvestmentType investmentType, DateTime FromDate, DateTime ToDate, int InvestTransType)
        {
            var IVHeads = _ShareInvestmentService.GetAllLiabilityAsync(investmentType, FromDate, ToDate, InvestTransType);
            var vmheads = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, decimal>>, IEnumerable<ShareInvestmentViewModel>>(await IVHeads);
            return vmheads;
        }

        #region Fixed Asset
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(GetLocalDateTime());
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View(await GetIndexData(EnumInvestmentType.FixedAsset, ViewBag.FromDate, ViewBag.ToDate, 0));
        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            return View(await GetIndexData(EnumInvestmentType.FixedAsset, ViewBag.FromDate, ViewBag.ToDate, 0));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(PopulateDropdown(new ShareInvestmentViewModel(), EnumInvestmentType.FixedAsset));
        }

        [HttpPost]
        public ActionResult Create(ShareInvestmentViewModel ShareInvestment, FormCollection formcollection)
        {
            AddModelError(ShareInvestment, formcollection);
            if (!ModelState.IsValid)
                return View(PopulateDropdown(ShareInvestment, EnumInvestmentType.FixedAsset));
            ShareInvestment.TransactionType = EnumInvestTransType.Receive;
            SaveInvestment(ShareInvestment, formcollection);

            return RedirectToAction("Index");
        }
        #endregion

        #region current asset
        public async Task<ActionResult> CurrentAsset()
        {
            var DateRange = GetFirstAndLastDateOfMonth(GetLocalDateTime());
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View(await GetIndexData(EnumInvestmentType.CurrentAsset, ViewBag.FromDate, ViewBag.ToDate, 0));
        }

        [HttpPost]
        public async Task<ActionResult> CurrentAsset(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            return View(await GetIndexData(EnumInvestmentType.CurrentAsset, ViewBag.FromDate, ViewBag.ToDate, 0));
        }

        [HttpGet]
        public ActionResult CreateCurrentAsset()
        {
            return View(PopulateDropdown(new ShareInvestmentViewModel(), EnumInvestmentType.CurrentAsset));
        }

        [HttpPost]
        public ActionResult CreateCurrentAsset(ShareInvestmentViewModel ShareInvestment, FormCollection formcollection)
        {
            AddModelError(ShareInvestment, formcollection);
            if (!ModelState.IsValid)
                return View(PopulateDropdown(ShareInvestment, EnumInvestmentType.CurrentAsset));
            ShareInvestment.TransactionType = EnumInvestTransType.Receive;
            SaveInvestment(ShareInvestment, formcollection);

            return RedirectToAction("CurrentAsset");
        }
        #endregion

        #region Liability
        public async Task<ActionResult> LiabilityRec()
        {
            var DateRange = GetFirstAndLastDateOfMonth(GetLocalDateTime());
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View(await GetLiabilityIndexData(EnumInvestmentType.Liability, ViewBag.FromDate, ViewBag.ToDate, (int)EnumInvestTransType.Receive));
        }

        [HttpPost]
        public async Task<ActionResult> LiabilityRec(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            return View(await GetLiabilityIndexData(EnumInvestmentType.Liability, ViewBag.FromDate, ViewBag.ToDate, (int)EnumInvestTransType.Receive));
        }
        [HttpGet]
        public ActionResult CreateLiabilityRec()
        {
            return View(GetInvestmentHedDDL(null));
        }

        [HttpPost]
        public ActionResult CreateLiabilityRec(ShareInvestmentViewModel newInvestment, FormCollection formcollection)
        {
            AddModelError(newInvestment, formcollection);
            if (!ModelState.IsValid)
                return View(GetInvestmentHedDDL(newInvestment));

            newInvestment.TransactionType = EnumInvestTransType.Receive;
            if (newInvestment.SIID > 0)
            {
                UpdateInvestment(newInvestment);
            }
            else
                SaveInvestment(newInvestment, formcollection);

            return RedirectToAction("LiabilityRec");
        }


        [HttpGet]
        public ActionResult EditLR(int id)
        {
            ShareInvestmentViewModel VMObj = new ShareInvestmentViewModel();
            if (id != 0)
            {
                var obj = _ShareInvestmentService.GetById(id);
                VMObj = _mapper.Map<ShareInvestment, ShareInvestmentViewModel>(obj);
                VMObj.LiabilityRecType = (EnumLiabilityRecType)((int)obj.LiabilityType);
            }
            return View("CreateLiabilityRec", PopulateDropdown(VMObj, EnumInvestmentType.Liability));
        }
        public async Task<ActionResult> LiabilityPay()
        {
            var DateRange = GetFirstAndLastDateOfMonth(GetLocalDateTime());
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View(await GetLiabilityIndexData(EnumInvestmentType.Liability, ViewBag.FromDate, ViewBag.ToDate, (int)EnumInvestTransType.Pay));
        }

        [HttpPost]
        public async Task<ActionResult> LiabilityPay(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            return View(await GetLiabilityIndexData(EnumInvestmentType.Liability, ViewBag.FromDate, ViewBag.ToDate, (int)EnumInvestTransType.Pay));
        }


        [HttpGet]
        public ActionResult CreateLiabilityPay()
        {
            return View(GetInvestmentHedDDL(null));
        }

        [HttpPost]
        public ActionResult CreateLiabilityPay(ShareInvestmentViewModel newInvestment, FormCollection formcollection)
        {
            AddModelError(newInvestment, formcollection);
            if (!ModelState.IsValid)
                return View(GetInvestmentHedDDL(newInvestment));
            newInvestment.TransactionType = EnumInvestTransType.Pay;

            if (newInvestment.SIID > 0)
                UpdateInvestment(newInvestment);
            else
                SaveInvestment(newInvestment, formcollection);

            return RedirectToAction("LiabilityPay");
        }


        [HttpGet]
        public ActionResult EditLP(int id)
        {
            ShareInvestmentViewModel VMObj = new ShareInvestmentViewModel();
            if (id != 0)
            {
                var obj = _ShareInvestmentService.GetById(id);
                VMObj = _mapper.Map<ShareInvestment, ShareInvestmentViewModel>(obj);
                VMObj.LiabilityPayType = (EnumLiabilityPayType)((int)obj.LiabilityType);
            }
            return View("CreateLiabilityPay", PopulateDropdown(VMObj, EnumInvestmentType.Liability));
        }

        [HttpGet]
        public ActionResult LiabilityReport()
        {
            //ViewBag.Heads = PopulateDropdown(new ShareInvestmentViewModel(), EnumInvestmentType.Liability).Heads;
            ViewBag.Heads = GetInvestmentHedDDL(null).Heads;
            return View();
        }

        #endregion

        private bool SaveInvestment(ShareInvestmentViewModel ShareInvestment, FormCollection formCollection)
        {
            try
            {
                
                var IVhead = _mapper.Map<ShareInvestmentViewModel, ShareInvestment>(ShareInvestment);
                IVhead.CashInHandReportStatus = ShareInvestment.CashInHandReportStatus ? 1 : 0;
                AddAuditTrail(IVhead, true);
                ShareInvestmentHead head = null;
                if (ShareInvestment.LiabilityRecType > 0)
                {
                    head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                    head.Balance += ShareInvestment.Amount;
                    IVhead.LiabilityType = (EnumLiabilityType)(int)ShareInvestment.LiabilityRecType;

                }
                else if (ShareInvestment.LiabilityPayType > 0)
                {
                    head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                    head.Balance -= ShareInvestment.Amount;
                    IVhead.LiabilityType = (EnumLiabilityType)(int)ShareInvestment.LiabilityPayType;
                }
                else if (ShareInvestment.TransactionType == EnumInvestTransType.Receive && ShareInvestment.LiabilityRecType == 0 && ShareInvestment.LiabilityPayType == 0)
                {
                    head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                    head.Balance += ShareInvestment.Amount;
                }
                bool IsSuccess = false;
                try
                {
                    _ShareInvestmentService.Add(IVhead);
                    _ShareInvestmentService.Save();
                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    AddToastMessage("Error", ex.Message, ToastType.Error);
                }

                if (IsSuccess)
                {
                    if (head != null)
                    {
                        _ShareInvestmentHeadService.Update(head);
                        _ShareInvestmentHeadService.Save();
                    }
                    AddToastMessage("", "Share Investment Save Successfully", ToastType.Success);
                    return IsSuccess;
                }

                return IsSuccess;


            }
            catch (Exception)
            {
            }
            AddToastMessage("", "Share Investment Save Failed.", ToastType.Error);

            return false;
        }

        private void AddModelError(ShareInvestmentViewModel ShareInvestment, FormCollection formCollection)
        {

            if (!IsDateValid(Convert.ToDateTime(ShareInvestment.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }


        }


        private void UpdateInvestment(ShareInvestmentViewModel newInvestment)
        {
            var oldInvest = _ShareInvestmentService.GetById(newInvestment.SIID);
            oldInvest.CashInHandReportStatus = newInvestment.CashInHandReportStatus ? 1 : 0;
            ShareInvestmentHead head = null;
            //update old head balance
            if (oldInvest.LiabilityType == EnumLiabilityType.TakeLoan
                || oldInvest.LiabilityType == EnumLiabilityType.LoanCollection || oldInvest.LiabilityType == EnumLiabilityType.TakeSecurity ||
                oldInvest.LiabilityType == EnumLiabilityType.TakePF || oldInvest.LiabilityType == EnumLiabilityType.TakeFDR)
            {
                var oldHead = _ShareInvestmentHeadService.GetById(oldInvest.SIHID);
                oldHead.Balance -= oldInvest.Amount;
            }
            else if (oldInvest.LiabilityType == EnumLiabilityType.GiveLoan
                || oldInvest.LiabilityType == EnumLiabilityType.LoanPay || oldInvest.LiabilityType == EnumLiabilityType.GiveFDR ||
                oldInvest.LiabilityType == EnumLiabilityType.GivePF || oldInvest.LiabilityType == EnumLiabilityType.GiveSecurity)
            {
                var oldHead = _ShareInvestmentHeadService.GetById(oldInvest.SIHID);
                oldHead.Balance += oldInvest.Amount;
            }
            else if (oldInvest.TransactionType == EnumInvestTransType.Receive && oldInvest.LiabilityType == 0)
            {
                var oldHead = _ShareInvestmentHeadService.GetById(oldInvest.SIHID);
                oldHead.Balance -= oldInvest.Amount;
            }

            oldInvest.EntryDate = newInvestment.EntryDate;
            oldInvest.Amount = newInvestment.Amount;
            oldInvest.Purpose = newInvestment.Purpose;

            if (newInvestment.LiabilityRecType > 0)
            {
                head = _ShareInvestmentHeadService.GetById(newInvestment.SIHID);
                head.Balance += newInvestment.Amount;
                oldInvest.LiabilityType = (EnumLiabilityType)(int)newInvestment.LiabilityRecType;
            }
            else if (newInvestment.LiabilityPayType > 0)
            {
                head = _ShareInvestmentHeadService.GetById(newInvestment.SIHID);
                head.Balance -= newInvestment.Amount;
                oldInvest.LiabilityType = (EnumLiabilityType)(int)newInvestment.LiabilityPayType;
            }
            else if (newInvestment.TransactionType == EnumInvestTransType.Receive && newInvestment.LiabilityRecType == 0 && newInvestment.LiabilityPayType == 0)
            {
                head = _ShareInvestmentHeadService.GetById(newInvestment.SIHID);
                head.Balance += newInvestment.Amount;
            }

            AddAuditTrail(oldInvest, false);
            bool IsSuccess = false;
            try
            {
                _ShareInvestmentService.Update(oldInvest);
                _ShareInvestmentService.Save();
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                AddToastMessage("error", ex.Message, ToastType.Error);
            }
            if (IsSuccess)
            {
                if (head != null)
                {
                    _ShareInvestmentHeadService.Update(head);
                    _ShareInvestmentHeadService.Save();
                }
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }
            else
                AddToastMessage("error", "Update failed.", ToastType.Error);


        }






        [HttpGet]
        public ActionResult Delete(int id)

        {
            ShareInvestment IVhead = _ShareInvestmentService.GetById(id);
            ShareInvestmentHead head = null;

            if (id != 0)
            {
                
                _ShareInvestmentService.Delete(id);
                _ShareInvestmentService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }


            //update old head balance
            if (IVhead.LiabilityType == EnumLiabilityType.GiveLoan || IVhead.LiabilityType == EnumLiabilityType.LoanPay)
            {
                head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                head.Balance += IVhead.Amount;


            }
            else if (IVhead.LiabilityType == EnumLiabilityType.LoanCollection || IVhead.LiabilityType == EnumLiabilityType.TakeLoan)
            {
                head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                head.Balance -= IVhead.Amount;
               //IVhead.LiabilityType = (EnumLiabilityType)(int)ShareInvestment.LiabilityPayType;
            }
            else if (IVhead.TransactionType == EnumInvestTransType.Receive && IVhead.LiabilityType == 0)
            {
                head = _ShareInvestmentHeadService.GetById(IVhead.SIHID);
                head.Balance -= IVhead.Amount;
            }


            AddAuditTrail(IVhead, false);
            if (head != null)
            {
                _ShareInvestmentHeadService.Update(head);
                _ShareInvestmentHeadService.Save();
        
            }
            

            return RedirectToAction("Index");
        }




        //[HttpGet]
        //public ActionResult Delete(int id)

        //{

        //    if (id != 0)
        //    {

        //        _ShareInvestmentService.Delete(id);
        //        _ShareInvestmentService.Save();
        //        AddToastMessage("", "Delete Successfully", ToastType.Success);
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ShareInvestmentViewModel VMObj = new ShareInvestmentViewModel();
            if (id != 0)
            {
                var obj = _ShareInvestmentService.GetById(id);
                VMObj = _mapper.Map<ShareInvestment, ShareInvestmentViewModel>(obj);
            }
            return View("Create", PopulateDropdown(VMObj, EnumInvestmentType.CurrentAsset));
        }

        [HttpPost]
        public ActionResult Edit(ShareInvestmentViewModel ShareInvestmentViewModel)
        {
            ShareInvestmentViewModel VMObj = new ShareInvestmentViewModel();
            if (ShareInvestmentViewModel.SIID != 0)
            {
                var obj = _ShareInvestmentService.GetById(ShareInvestmentViewModel.SIID);
                obj.EntryDate = ShareInvestmentViewModel.EntryDate;
                obj.Amount = ShareInvestmentViewModel.Amount;
                obj.Purpose = ShareInvestmentViewModel.Purpose;
                AddAuditTrail(obj, false);
                _ShareInvestmentService.Update(obj);
                _ShareInvestmentService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetHeadsByParent(int ID)
        {
            var heads = (from c in _ShareInvestmentHeadService.GetAll()
                         join p in _ShareInvestmentHeadService.GetAll() on c.ParentId equals p.SIHID
                         where c.ParentId == ID
                         select new InvestmentheadViewModel
                         {
                             SIHID = c.SIHID,
                             Code = c.Code,
                             ParentName = p.Name,
                             Name = c.Name
                         }).ToList();
            return Json(heads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBalanceByHeadID(int headID)
        {
            if (headID > 0)
            {
                var head = _ShareInvestmentHeadService.GetById(headID);
                if (head != null)
                    return Json(new { result = true, data = head.Balance }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }



        //for concernwise report
        void PopulateConcernsDropdown()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetFamilyTree(User.Identity.GetConcernId()), "ConcernID", "Name");
        }

        [HttpGet]
        [Authorize]
        public ActionResult TotalLiabilityPayRec()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }




    }
}