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
    public class CardTypeController : CoreController
    {
        IMapper _mapper;
        ICardTypeService _CardTypeService;
        IMiscellaneousService<CardType> _miscell;
        public CardTypeController(IErrorService errorService, ICardTypeService CardTypeService, IMapper Mapper, IMiscellaneousService<CardType> miscell, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _CardTypeService = CardTypeService;
            _mapper = Mapper;
            _miscell = miscell;
        }
        public ActionResult Index()
        {
            var grades = _CardTypeService.GetAll();
            var vmgrades = _mapper.Map<IQueryable<CardType>, IEnumerable<CardTypeViewModel>>(grades);
            return View(vmgrades);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var code = _miscell.GetUniqueKey(i => (int)i.CardTypeID);
            return View(new CardTypeViewModel() { Code = code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CardTypeViewModel CardType, FormCollection formcollection)
        {

            if (!ModelState.IsValid)
                return View(CardType);
            CardType.Status = (int)EnumActiveInactive.Active;
            var cardtype = _mapper.Map<CardTypeViewModel, CardType>(CardType);
            cardtype.ConcernID = User.Identity.GetConcernId();
            _CardTypeService.Add(cardtype);
            _CardTypeService.Save();
            AddToastMessage("", "CardType Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _CardTypeService.Delete(id);
                _CardTypeService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Active(int id)
        {
            if (id != 0)
            {
                var obj = _CardTypeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.Active; ;
                _CardTypeService.Update(obj);
                _CardTypeService.Save();
                AddToastMessage("", "Active Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Inactive(int id)
        {
            if (id != 0)
            {
                var obj = _CardTypeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.InActive; ;
                _CardTypeService.Update(obj);
                _CardTypeService.Save();
                AddToastMessage("", "Inactive Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            CardTypeViewModel VMObj = new CardTypeViewModel();
            if (id != 0)
            {
                var obj = _CardTypeService.GetById(id);
                VMObj = _mapper.Map<CardType, CardTypeViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CardTypeViewModel CardTypeViewModel)
        {
            CardTypeViewModel VMObj = new CardTypeViewModel();
            if (CardTypeViewModel.CardTypeID != 0)
            {
                var obj = _CardTypeService.GetById((int)CardTypeViewModel.CardTypeID);
                obj.Description = CardTypeViewModel.Description;
                obj.Sequence = CardTypeViewModel.Sequence;
                AddAuditTrail(obj, false);
                _CardTypeService.Update(obj);
                _CardTypeService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }
    }
}