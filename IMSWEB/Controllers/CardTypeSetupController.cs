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
    public class CardTypeSetupController : CoreController
    {
        ICardTypeSetupService _CardTypeSetupService;
        IMiscellaneousService<CardTypeSetup> _miscellaneousService;
        IMapper _mapper;
        ICardTypeService _CardTypeService;
        public CardTypeSetupController(IErrorService errorService,
            ICardTypeSetupService CardTypeSetupService, IMiscellaneousService<CardTypeSetup> miscellaneousService,
            IMapper mapper, ICardTypeService CardTypeService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _CardTypeSetupService = CardTypeSetupService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _CardTypeService = CardTypeService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var cardtypesetups = _CardTypeSetupService.GetAllAsync();
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, string, decimal, string, string, string>>, IEnumerable<CardTypeSetupViewModel>>(await cardtypesetups);
            return View(vmodel);
        }
        CardTypeSetupViewModel PopulateDropdown(CardTypeSetupViewModel cardTS)
        {
            var ct = _CardTypeService.GetAllActive();
            cardTS.CardTypes = ct.ToList();
            return cardTS;
        }
        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = "CT-" + _miscellaneousService.GetUniqueKey(x => x.CardTypeSetupID);
            return View(PopulateDropdown(new CardTypeSetupViewModel { Code = code }));
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CardTypeSetupViewModel NewCardTypeSetup, FormCollection formCollection)
        {
            AddModelError(NewCardTypeSetup, formCollection);
            if (!ModelState.IsValid)
                return View(PopulateDropdown(NewCardTypeSetup));

            if (NewCardTypeSetup != null)
            {
                var existingBank = _miscellaneousService.GetDuplicateEntry(c => c.CardTypeID == NewCardTypeSetup.CardTypeID && c.BankID == NewCardTypeSetup.BankID);
                if (existingBank != null)
                {
                    AddToastMessage("", "A CardTypeSetup with same bank account number already exists in the system. Please try with a different.", ToastType.Error);
                    return View(PopulateDropdown(NewCardTypeSetup));
                }

                var CardTypeSetup = _mapper.Map<CardTypeSetupViewModel, CardTypeSetup>(NewCardTypeSetup);
                CardTypeSetup.ConcernID = User.Identity.GetConcernId();
                _CardTypeSetupService.Add(CardTypeSetup);
                _CardTypeSetupService.Save();

                AddToastMessage("", "CardTypeSetup has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No CardTypeSetup data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddModelError(CardTypeSetupViewModel NewCardTypeSetup, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["BanksId"]))
                ModelState.AddModelError("BankID", "Bank is required.");
            else
                NewCardTypeSetup.BankID = Convert.ToInt32(formCollection["BanksId"]);

            if (NewCardTypeSetup.CardTypeID == 0)
                ModelState.AddModelError("CardTypeID", "CardType is required.");

            if (NewCardTypeSetup.Percentage <= 0)
                ModelState.AddModelError("Percentage", "Percentage is required.");

        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var CardTypeSetup = _CardTypeSetupService.GetById(id);
            var vmodel = _mapper.Map<CardTypeSetup, CardTypeSetupViewModel>(CardTypeSetup);
            return View("Create", PopulateDropdown(vmodel));
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CardTypeSetupViewModel NewCardTypeSetup, FormCollection formCollection)
        {
            AddModelError(NewCardTypeSetup, formCollection);
            if (!ModelState.IsValid)
                return View("Create", PopulateDropdown(NewCardTypeSetup));

            if (NewCardTypeSetup != null)
            {
                var CardTypeSetup = _CardTypeSetupService.GetById(Convert.ToInt32(NewCardTypeSetup.CardTypeSetupID));

                CardTypeSetup.Code = NewCardTypeSetup.Code;
                CardTypeSetup.BankID = NewCardTypeSetup.BankID;
                CardTypeSetup.CardTypeID = NewCardTypeSetup.CardTypeID;
                CardTypeSetup.Percentage = NewCardTypeSetup.Percentage;

                _CardTypeSetupService.Update(CardTypeSetup);
                _CardTypeSetupService.Save();

                AddToastMessage("", "CardTypeSetup has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No CardTypeSetup found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _CardTypeSetupService.Delete(id);
            _CardTypeSetupService.Save();
            AddToastMessage("", "CardTypeSetup has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}