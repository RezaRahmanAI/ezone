using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("Stock")]
    public class StockController : CoreController
    {
        IStockService _StockService;
        IStockDetailService _StockDetailService;
        IMapper _mapper;
        IProductService _ProductService;
        ISisterConcernService _SisterConcernService;
        ISystemInformationService _sysInfoService;

        public StockController(IErrorService errorService, IStockDetailService StockDetailServie,
            IStockService StockService, IMapper mapper, IProductService ProductService, ISisterConcernService SisterConcernService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _StockService = StockService;
            _mapper = mapper;
            _SisterConcernService = SisterConcernService;
            _StockDetailService = StockDetailServie;
            _ProductService = ProductService;
            _sysInfoService = sysInfoService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            //int PageSize = 15;
            //int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
            var StocksAsync = _StockService.GetAllStockAsync(User.Identity.GetConcernId(), IsVATManager());
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, string, string,
            string, decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>>, IEnumerable<GetStockViewModel>>(await StocksAsync);
            //var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(vmodel);


        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult Index(FormCollection formCollection)
        //{
        //    int PageSize = 15;
        //    int Pages = 1;
        //    IQueryable<ProductDetailsModel> StocksAsync = null;
        //    if (!string.IsNullOrEmpty(formCollection["ProductName"]))
        //    {
        //        string productName = formCollection["ProductName"];
        //        StocksAsync = _StockService.GetStocs().Where(i => i.ProductName.Contains(productName));
        //    }
        //    var vmodel = _mapper.Map<IQueryable<ProductDetailsModel>, List<GetStockViewModel>>(StocksAsync);
        //    var pagelist = vmodel.ToPagedList(Pages, PageSize);
        //    return View(pagelist);

        //}


        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult StockDetails(int? Page)
        {
            int PageSize = 15;
            int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
            var StocksAsync = _StockService.GetStockDetails();
            var vmodel = _mapper.Map<IQueryable<ProductDetailsModel>, List<GetStockDetailViewModel>>(StocksAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);
        }

        [HttpPost]
        [Authorize]
        public ActionResult StockDetails(FormCollection formCollection)
        {
            int PageSize = 100;
            int Pages = 1;
            IQueryable<ProductDetailsModel> StocksAsync = null;
            if (!string.IsNullOrEmpty(formCollection["IMEI"]))
            {
                string IMEI = formCollection["IMEI"];
                StocksAsync = _StockService.GetStockDetails().Where(i => i.IMENo.EndsWith(IMEI));
            }
            if (!string.IsNullOrEmpty(formCollection["ProductName"]))
            {
                string productname = formCollection["ProductName"];
                StocksAsync = _StockService.GetStockDetails().Where(i => i.ProductName.Contains(productname));
            }
            var vmodel = _mapper.Map<IQueryable<ProductDetailsModel>, List<GetStockDetailViewModel>>(StocksAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);
        }



        [HttpGet]
        [Authorize]
        public ActionResult StockReport()
        {
            return View("StockReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockSummaryReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePriceProtection()
        {
            return View("ProductWisePriceProtection");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyStockVSSalesSummary()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult StockLedgerReport()
        {
            PopulateConcernDropdown();
            return View();
        }
        private void PopulateConcernDropdown()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetAll(), "ConcernID", "Name");
        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateSalesRate(int ProductID, int ColorID, decimal SalesRate, decimal CreditSalesRate3, decimal CreditSalesRate6, decimal CreditSalesRate12)
        {
            List<StockDetail> collection = new List<StockDetail>();
            if (ProductID != 0)
            {
                if (ColorID != 0)
                    collection = _StockDetailService.GetStockDetailByProductIdColorID(ProductID, ColorID).ToList();
                else
                    collection = _StockDetailService.GetStockDetailByProductId(ProductID).ToList();

                if (collection.Count() != 0)
                {
                    foreach (var item in collection)
                    {
                        item.SRate = SalesRate;
                        item.CreditSRate = CreditSalesRate6;
                        item.CRSalesRate3Month = CreditSalesRate3;
                        item.CRSalesRate12Month = CreditSalesRate12;
                    }
                    _StockDetailService.SaveStockDetail();
                    AddToastMessage("", "Stock update successfully", ToastType.Success);
                }

                var Products = _ProductService.GetProductById(ProductID);
                Products.MRP = SalesRate;
                Products.PurchaseRate = SalesRate;
                _ProductService.UpdateProduct(Products);
                _ProductService.SaveProduct();

                UpdateSisterConcernMRP(Products);

                AddToastMessage("", "Product config update successfully", ToastType.Success);
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            AddToastMessage("", "Update Failed;", ToastType.Error);

            return Json(false, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateEmobileSalesRate(int ProductID, int ColorID, decimal SalesRate, decimal CreditSalesRate3, decimal CreditSalesRate6, decimal CreditSalesRate12, decimal MRP)
        {
            List<StockDetail> collection = new List<StockDetail>();

            if (ProductID != 0 && ColorID != 0)
            {
                collection = _StockDetailService.GetStockDetailByProductIdColorID(ProductID, ColorID).ToList();
                if (collection.Count() != 0)
                {
                    foreach (var item in collection)
                    {
                        item.SRate = SalesRate;
                        item.CreditSRate = CreditSalesRate6;
                        item.CRSalesRate3Month = CreditSalesRate3;
                        item.CRSalesRate12Month = CreditSalesRate12;
                    }
                    _StockDetailService.SaveStockDetail();
                    AddToastMessage("", "Stock update successfully", ToastType.Success);
                }
                var Products = _ProductService.GetProductById(ProductID);
                Products.MRP = SalesRate;

                _ProductService.UpdateProduct(Products);
                _ProductService.SaveProduct();
                AddToastMessage("", "Product config update successfully", ToastType.Success);
            }

            if (ColorID == 0)
            {
                collection = _StockDetailService.GetStockDetailByProductId(ProductID).ToList();
                if (collection.Count() != 0)
                {
                    foreach (var item in collection)
                    {
                        item.SRate = SalesRate;
                        item.CreditSRate = CreditSalesRate6;
                        item.CRSalesRate3Month = CreditSalesRate3;
                        item.CRSalesRate12Month = CreditSalesRate12;
                    }
                    _StockDetailService.SaveStockDetail();
                    AddToastMessage("", "Stock update successfully", ToastType.Success);
                }
                var Products = _ProductService.GetProductById(ProductID);
                Products.MRP = SalesRate;
                Products.RP = MRP;
                _ProductService.UpdateProduct(Products);
                _ProductService.SaveProduct();
                AddToastMessage("", "Product config update successfully", ToastType.Success);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            //AddToastMessage("", "Update Failed;", ToastType.Error);

            return Json(false, JsonRequestBehavior.AllowGet);

        }

        private void UpdateSisterConcernMRP(Product product)
        {
            var families = _SisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                    .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                foreach (var item in families)
                {
                    Product concernProduct = _ProductService.GetAll(item.ConcernID).Where(i => i.ProductName.ToLower().Equals(product.ProductName.ToLower())).FirstOrDefault();



                    if (concernProduct != null)
                    {
                        List<StockDetail> stockDetails = _StockDetailService.GetStockDetailByProductId(concernProduct.ProductID).ToList();

                        if (stockDetails.Any())
                        {
                            foreach (var stk in stockDetails)
                            {
                                stk.SRate = product.MRP ?? 0;
                            }
                            _StockDetailService.SaveStockDetail();
                        }


                        concernProduct.MRP = product.MRP;
                        concernProduct.PurchaseRate = product.PurchaseRate;
                        //_ProductService.UpdateProduct(concernProduct);
                        _ProductService.SaveProduct();
                    }
                }
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminStockSummaryReport()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        public JsonResult GetProductByID(int ProductID)
        {
            if (ProductID > 0)
            {
                var stock = _StockService.GetforStockReport(User.Identity.Name, User.Identity.GetConcernId(), 0,
                    0, 0, ProductID, 0, 0, 0, false, 0).ToList();
                var vmStock = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, decimal, decimal,
                    Tuple<decimal, decimal, decimal, decimal, string,
                    List<string>, string>>>, List<ProductDetailsModel>>(stock);
                if (vmStock.Count() > 0)
                {
                    return Json(vmStock, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PrintIMEI(int id)
        {
            TempData["SDetailID"] = id;
            TempData["IsIMEIPrintReady"] = true;
            return RedirectToAction("StockDetails");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ShowroomAnalysis()
        {
            PopulateConcernDropdown();
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockQTYReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult DateWiseProducthistory()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockForcastingReport()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockForcastingReportProductWise()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockReportWithDate()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockLedgerReportExcel()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        public JsonResult GetPOProducts()
        {

            var products = (from p in _ProductService.GetAllProductIQueryableNew()
                            select new
                            {
                                p.StockID,
                                preStock = p.PrevStq,
                                id = p.ProductID,
                                code = p.ProductCode,
                                name = p.ProductName,
                                p.CompanyName,
                                category = p.CategoryName,
                                UnitType = p.UnitType,
                                PWDiscount = p.PWDiscount,
                                MRP = p.MRP,
                                PurchaseRate = p.MRP,
                                CompressorWarrentyMonth = p.CompressorWarrentyMonth,
                                MotorWarrentyMonth = p.MotorWarrentyMonth,
                                PanelWarrentyMonth = p.PanelWarrentyMonth,
                                ServiceWarrentyMonth = p.ServiceWarrentyMonth,
                                SparePartsWarrentyMonth = p.SparePartsWarrentyMonth,
                                ProductsType = p.ProductType,
                                ColorName = p.ColorName
                            }).ToList();

            if (products.Count() > 0)
            {
                JsonResult jsonResult = Json(products, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminProductStockReport()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult RateWiseStockLedgerReport()
        {
            PopulateConcernDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockSummaryReportZeroQty()
        {
            return View();
        }

        public ActionResult StockReportNew()
        {
            return View("StockReportNew");
        }

    }
}
