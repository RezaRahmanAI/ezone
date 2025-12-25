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
    public class InvestmentHeadController : CoreController
    {
        IMapper _mapper;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        IMiscellaneousService<ShareInvestmentHead> _miscell;
        public InvestmentHeadController(IErrorService errorService, IShareInvestmentHeadService headService, IMapper Mapper,
            IMiscellaneousService<ShareInvestmentHead> miscell, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _ShareInvestmentHeadService = headService;
            _mapper = Mapper;
            _miscell = miscell;
        }
        public async Task<ActionResult> Index()
        {
            var IVHeads = _ShareInvestmentHeadService.GetAllAsync();
            var vmheads = _mapper.Map<IEnumerable<Tuple<int, string, string, string>>, IEnumerable<InvestmentheadViewModel>>(await IVHeads);
            return View(vmheads);
        }

      

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.IsTransactionFound = false;
            var code = _miscell.GetUniqueKey(i => (int)i.SIHID);
            return View(new InvestmentheadViewModel() { Code = code });
        }

        [HttpPost]
        public ActionResult Create(InvestmentheadViewModel newHead, FormCollection formcollection)
        {
            ViewBag.IsTransactionFound = false;
            AddModelError(newHead);
            if (!ModelState.IsValid)
                return View(newHead);
            var IVhead = _mapper.Map<InvestmentheadViewModel, ShareInvestmentHead>(newHead);
            AddAuditTrail(IVhead, true);
            IVhead.Balance = IVhead.OpeningBalance;
            _ShareInvestmentHeadService.Add(IVhead);
            _ShareInvestmentHeadService.Save();
            AddToastMessage("", "ShareInvestment Head Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void AddModelError(InvestmentheadViewModel newHead)
        {
            if (newHead.ParentId == 0)
                ModelState.AddModelError("ParentId", "Investment type is required.");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (_ShareInvestmentHeadService.IsChildExist(id))
                {
                    AddToastMessage("", "This head has child. You can't delete this.", ToastType.Error);
                    return RedirectToAction("Index");
                }
                if (_ShareInvestmentHeadService.IsTransactionFound(id, User.Identity.GetConcernId()))
                {
                    AddToastMessage("", "Transaction found! You can't delete this.", ToastType.Error);
                    return RedirectToAction("Index");
                }
                _ShareInvestmentHeadService.Delete(id);
                _ShareInvestmentHeadService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Active(int id)
        {
            if (id != 0)
            {
                var obj = _ShareInvestmentHeadService.GetById(id);
                _ShareInvestmentHeadService.Update(obj);
                _ShareInvestmentHeadService.Save();
                AddToastMessage("", "Active Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Inactive(int id)
        {
            if (id != 0)
            {
                var obj = _ShareInvestmentHeadService.GetById(id);
                _ShareInvestmentHeadService.Update(obj);
                _ShareInvestmentHeadService.Save();
                AddToastMessage("", "Inactive Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.IsTransactionFound = _ShareInvestmentHeadService.IsTransactionFound(id, User.Identity.GetConcernId());
            InvestmentheadViewModel VMObj = new InvestmentheadViewModel();
            if (id != 0)
            {
                var obj = _ShareInvestmentHeadService.GetById(id);
                VMObj = _mapper.Map<ShareInvestmentHead, InvestmentheadViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(InvestmentheadViewModel InvestmentheadViewModel)
        {
            bool isTrFound = _ShareInvestmentHeadService.IsTransactionFound(InvestmentheadViewModel.SIHID, User.Identity.GetConcernId());
            InvestmentheadViewModel VMObj = new InvestmentheadViewModel();
            AddModelError(InvestmentheadViewModel);
            ViewBag.IsTransactionFound = isTrFound;
            if (!ModelState.IsValid)
                return View("Create", InvestmentheadViewModel);

            if (InvestmentheadViewModel.SIHID != 0)
            {
                var obj = _ShareInvestmentHeadService.GetById(InvestmentheadViewModel.SIHID);
                obj.Code = InvestmentheadViewModel.Code;
                obj.Name = InvestmentheadViewModel.Name;
                obj.OpeningBalance = isTrFound ? obj.OpeningBalance : InvestmentheadViewModel.OpeningBalance;
                obj.OpeningType = isTrFound ? obj.OpeningType : InvestmentheadViewModel.OpeningType.ToString();
                obj.ParentId = Convert.ToInt32(InvestmentheadViewModel.ParentId);
                obj.Balance = isTrFound ? obj.Balance : InvestmentheadViewModel.OpeningBalance;
                AddAuditTrail(obj, false);
                _ShareInvestmentHeadService.Update(obj);
                _ShareInvestmentHeadService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }


    }
}