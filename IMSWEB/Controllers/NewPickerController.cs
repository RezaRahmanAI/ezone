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
    [Route("NewPicker")]
    [AllowAnonymous]
    public class NewPickerController : CoreController
    {
        IMapper _mapper;      
        IUserService _UserService;       
        private readonly ISisterConcernService _sisterConcernService;
        ISystemInformationService _SystemInformationService;

        public NewPickerController(
            IErrorService errorService, 
            IUserService UserService,           
            ISystemInformationService SystemInformationService,
   
        IMapper mapper,  ISisterConcernService sisterConcernService
            )
            : base(errorService, SystemInformationService)
        {
             _mapper = mapper;           
            _UserService = UserService;          
            _SystemInformationService = SystemInformationService;    
            _sisterConcernService = sisterConcernService;
        }

        public PartialViewResult RenderPicker(PickerType pickerType, int id)
        {
            switch (pickerType)
            {
                case PickerType.SisterConcern:
                    int beforeLoginConcernID = Convert.ToInt32(TempData["payConcernId"]);
                    TempData["payConcernId"] = beforeLoginConcernID;
                    if (beforeLoginConcernID == 0)
                    {

                    }

                    var allConcern = _sisterConcernService.GetAllConcernByConcernId(beforeLoginConcernID);       
                    if (id != default(int))
                    {
                        var vmColor = allConcern.First(x => x.Id == id);
                        ViewBag.ConcernName = vmColor.Name;
                        ViewBag.ConcernId = vmColor.Id;
                        ViewBag.ServiceCharge = vmColor.ServiceCharge;
                    }
                    return PartialView("~/Views/NewPicker/_ConcernPicker.cshtml", allConcern);
                case PickerType.CustomerSalesProduct:
                    return PartialView("~/Views/Pickers/_CustomerSalesProductPicker.cshtml");
                default:

                    ViewBag.Type = "Invalid Picker";
                    return PartialView("~/Views/Pickers/_ModulePicker.cshtml", null);
            }
        }


       
    }
}