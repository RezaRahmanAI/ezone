using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("SaleOffer-item")]
    public class SaleOfferController : CoreController
    {

        ISaleOfferService _SaleOfferService;
        IProductService _ProductService;
        IMiscellaneousService<SaleOffer> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;

        public SaleOfferController(IErrorService errorService,
            ISaleOfferService saleOfferService, IProductService productService,
            IMiscellaneousService<SaleOffer> miscellaneousService, ISystemInformationService sysInfoService,
            IMapper mapper)
            : base(errorService, sysInfoService)
        {
            _SaleOfferService = saleOfferService;
            _ProductService = productService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {

            var itemsAsync = _SaleOfferService.GetAllSOfferAsync();
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, string, string, DateTime, DateTime,
                string, string, Tuple<string, string, string>>>, IEnumerable<GetSaleOfferViewModel>>(await itemsAsync);

            return View(vmodel);

        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string OfferCode = _miscellaneousService.GetUniqueKey(x => int.Parse(x.OfferCode));
            return View(new CreateSaleOfferViewModel { OfferCode = OfferCode });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateSaleOfferViewModel newSaleOffer, FormCollection formCollection, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSaleOffer);

            if (newSaleOffer != null)
            {
                newSaleOffer.CreateDate = DateTime.Now.ToString();
                newSaleOffer.CreatedBy = (User.Identity.GetUserId<string>());
                newSaleOffer.ConcernID = User.Identity.GetConcernId().ToString();
                newSaleOffer.ProductID = formCollection["ProductsId"];
                newSaleOffer.Status = EnumOfferStatus.Ongoing;
                newSaleOffer.ModifiedDate = DateTime.Now.ToString();

                var saleOffer = _mapper.Map<CreateSaleOfferViewModel, SaleOffer>(newSaleOffer);

                _SaleOfferService.AddSaleOffer(saleOffer);
                _SaleOfferService.SaveSaleOffer();

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var saleOffer = _SaleOfferService.GetSaleOfferById(id);
            var vmodel = _mapper.Map<SaleOffer, CreateSaleOfferViewModel>(saleOffer);
            return View("Create", vmodel);
        }

        void AddModelError(CreateSaleOfferViewModel newSaleOffer, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["ProductsId"]))
            {
                ModelState.AddModelError("ProductID", "Product is Required");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSaleOfferViewModel newSaleOffer, FormCollection formCollection, string returnUrl)
        {
            AddModelError(newSaleOffer, formCollection);

            if (!ModelState.IsValid)
                return View("Create", newSaleOffer);

            if (newSaleOffer != null)
            {
                var saleOffer = _SaleOfferService.GetSaleOfferById(int.Parse(newSaleOffer.OfferID));

                saleOffer.OfferCode = newSaleOffer.OfferCode;
                saleOffer.ProductID = Convert.ToInt32(formCollection["ProductsId"]);
                saleOffer.FromDate = Convert.ToDateTime(newSaleOffer.FromDate);
                saleOffer.ToDate = Convert.ToDateTime(newSaleOffer.ToDate);
                saleOffer.Description = newSaleOffer.Description;
                saleOffer.OfferValue = Convert.ToDecimal(newSaleOffer.OfferValue);
                saleOffer.OfferType = newSaleOffer.OfferType;
                saleOffer.ThresholdValue = Convert.ToDecimal(newSaleOffer.ThresholdValue);
                saleOffer.Status = newSaleOffer.Status;

                //saleOffer.ConcernID = saleOffer.ConcernID;
                //saleOffer.CreatedBy = saleOffer.CreatedBy;
                //saleOffer.CreateDate = saleOffer.CreateDate;
                saleOffer.ModifiedBy = User.Identity.GetUserId<int>();
                saleOffer.ModifiedDate = DateTime.Now;

                _SaleOfferService.UpdateSaleOffer(saleOffer);
                _SaleOfferService.SaveSaleOffer();

                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _SaleOfferService.DeleteSaleOffer(id);
            _SaleOfferService.SaveSaleOffer();
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

    }
}