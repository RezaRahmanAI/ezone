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
    [RoutePrefix("supplier")]
    public class SupplierController : CoreController
    {
        ISupplierService _supplierService;
        IMiscellaneousService<Supplier> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/suppliers";
        ISisterConcernService _sisterConcernService;
        ISystemInformationService _sysInfoService;

        public SupplierController(IErrorService errorService,
            ISupplierService supplierService, IMiscellaneousService<Supplier> miscellaneousService,
            IMapper mapper,ISisterConcernService sisterConcernService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _supplierService = supplierService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _sisterConcernService = sisterConcernService;
            _sysInfoService = sysInfoService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var suppliersAsync = _supplierService.GetAllSupplierAsync();
            var vmodel = _mapper.Map<IEnumerable<Supplier>, IEnumerable<GetSupplierViewModel>>(await suppliersAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateSupplierViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateSupplierViewModel newSupplier, FormCollection formCollection,
            HttpPostedFileBase photo, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSupplier);

            if (newSupplier != null)
            {
                var existingSupplier = _miscellaneousService.GetDuplicateEntry(s => s.ContactNo == newSupplier.ContactNo);
                if (existingSupplier != null)
                {
                    AddToastMessage("", "A Supplier with same contact no. already exists in the system. Please try with a different contact no.", ToastType.Error);
                    return View(newSupplier);
                }

                if (photo != null)
                {
                    var photoName = newSupplier.Code + "_" + newSupplier.Name;
                    newSupplier.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                }

                var supplier = _mapper.Map<CreateSupplierViewModel, Supplier>(newSupplier);
                supplier.ConcernID = User.Identity.GetConcernId();
                supplier.OpeningDue = decimal.Parse(GetDefaultIfNull(newSupplier.OpeningDue));
                _supplierService.AddSupplier(supplier);
                _supplierService.SaveSupplier();

                AddToastMessage("", "Supplier has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Supplier data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            var vmodel = _mapper.Map<Supplier, CreateSupplierViewModel>(supplier);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSupplierViewModel newSupplier, FormCollection formCollection,
            HttpPostedFileBase photo, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSupplier);

            if (newSupplier != null)
            {
                var existingSupplier = _supplierService.GetSupplierById(int.Parse(newSupplier.Id));
                if (photo != null)
                {
                    var photoName = newSupplier.Code + "_" + newSupplier.Name;
                    existingSupplier.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                }

                existingSupplier.Code = newSupplier.Code;
                existingSupplier.Name = newSupplier.Name;
                existingSupplier.ContactNo = newSupplier.ContactNo;
                existingSupplier.TotalDue = decimal.Parse(newSupplier.TotalDue);
                existingSupplier.OwnerName = newSupplier.OwnerName;
                existingSupplier.ContactNo = newSupplier.ContactNo;
                existingSupplier.Address = newSupplier.Address;
                existingSupplier.ConcernID = User.Identity.GetConcernId();

                _supplierService.UpdateSupplier(existingSupplier);
                _supplierService.SaveSupplier();

                AddToastMessage("", "Supplier has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Supplier data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _supplierService.DeleteSupplier(id);
            _supplierService.SaveSupplier();
            AddToastMessage("", "Supplier has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ConcernWiseSupplierDueRpt()
        {
            return View("ConcernWiseSupplierDueRpt");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminSupplierDueRpt()
        {
            @ViewBag.Concerns = new SelectList(_sisterConcernService.GetAll(), "ConcernID", "Name");
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult SupplierAdjustmentReport()
        {
            ViewBag.Title = "Supplier Adjustment Report";
            return View();
        }



    }
}