using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize(Roles = "vatadmin,Admin")]
    [RoutePrefix("SisterConcern")]
    public class VATSetupController : CoreController
    {


        ISisterConcernService _sisterConcernService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;


        public VATSetupController(IErrorService errorService,
            ISisterConcernService sisterConcernService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _sisterConcernService = sisterConcernService;
            _mapper = mapper;
            _sysInfoService = sysInfoService;
        }
        CreateSisterConcernViewModel PopulateDropdown(CreateSisterConcernViewModel oConcern)
        {
            oConcern.SisterConcerns = _sisterConcernService.GetAll().ToList();
            return oConcern;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var sisterConcern = _sisterConcernService.GetSisterConcernById(User.Identity.GetConcernId());
            var vmodel = _mapper.Map<SisterConcern, CreateSisterConcernViewModel>(sisterConcern);
            return View(vmodel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(CreateSisterConcernViewModel newSetup)
        {
            var sisterConcern = _sisterConcernService.GetSisterConcernById(User.Identity.GetConcernId());
            sisterConcern.SalesShowPercent = newSetup.SalesShowPercent;
            sisterConcern.PurchaseShowPercent = newSetup.PurchaseShowPercent;
            sisterConcern.StockShowPercent = newSetup.StockShowPercent;
            _sisterConcernService.SaveSisterConcern();
            AddToastMessage("", "Update Successfull", ToastType.Success);
            return View(newSetup);
        }
    }
}