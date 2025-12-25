using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("product")]
    public class ProductController : CoreController
    {
        IProductService _productService;
        IMiscellaneousService<Product> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/products";
        IPurchaseOrderService _purchaseOrderService;
        ISisterConcernService _sisterConcernService;
        ICompanyService _companyService;
        private readonly IParentCategoryService _parentCategoryService;
        ICategoryService _categoryService;
        private readonly ISystemInformationService _sysInfoService;
        ISalesOrderService _salesOrderService;
        public ProductController(IErrorService errorService,
            IProductService productService, IMiscellaneousService<Product> miscellaneousService, IMapper mapper,
             IPurchaseOrderService purchaseOrderService,
             ISisterConcernService sisterConcernService,
             ICompanyService companyService,
             ICategoryService categoryService, IParentCategoryService parentCategoryService, ISystemInformationService sysInfoService, ISalesOrderService salesOrderService)
            : base(errorService, sysInfoService)
        {
            _productService = productService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _purchaseOrderService = purchaseOrderService;
            _sisterConcernService = sisterConcernService;
            _categoryService = categoryService;
            _companyService = companyService;
            _parentCategoryService = parentCategoryService;
            _sysInfoService = sysInfoService;
            _salesOrderService = salesOrderService;
        }


        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());

            var sysInfo = Session["SystemInfo"] as CreateSystemInformationViewModel;
            //ViewBag.IsMRPUpdateForManager = sysInfo.IsMrpUpdateShowForManager;
            ViewBag.IsMRPUpdateForManager = _sysInfoService.IsMrpUpdateShowForManager();
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult EmobileIndex()
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());

            var sysInfo = Session["SystemInfo"] as CreateSystemInformationViewModel;
            //ViewBag.IsMRPUpdateForManager = sysInfo.IsMrpUpdateShowForManager;
            ViewBag.IsMRPUpdateForManager = _sysInfoService.IsMrpUpdateShowForManager();
            return View();
        }

        [HttpGet]
        public ActionResult GetProductsForSale()
        {
            var draw = Request["draw"];
            var start = Request["start"];
            var length = Request["length"];
            var searchValue = Request["search[value]"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            int ConcernID = User.Identity.GetConcernId();
            var vmProductDetails = _salesOrderService.ProductPickerInStock(ConcernID);

            var products = (from vm in vmProductDetails
                                    group vm by new
                                    {
                                        vm.IMENO,
                                        vm.ProductID,
                                        vm.CategoryID,
                                        vm.ProductName,
                                        vm.ProductCode,
                                        vm.ColorID,
                                        vm.GodownID,
                                        vm.CategoryName,
                                        vm.ColorName,
                                        vm.GodownName
                                    }
                                            into g
                                    select new GetProductViewModel
                                    {
                                        IMENo = g.Key.IMENO,
                                        ProductId = g.Key.ProductID,
                                        ProductCode = g.Key.ProductCode,
                                        ProductName = g.Key.ProductName,
                                        CategoryID = g.Key.CategoryID,

                                        CategoryName = g.Key.CategoryName,
                                        ColorName = g.Key.ColorName,
                                        ColorId = g.Key.ColorID,
                                        GodownID = g.Key.GodownID,
                                        StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),
                                        PreStock = g.Sum(o => o.PreStock),

                                        MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                        PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                        Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                        GodownName = g.Key.GodownName,
                                        ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                        Service = g.Select(o => o.Service).FirstOrDefault(),
                                    }).OrderBy(i => i.ProductName).ToList();

            //var products = _productService.GetAllProducts();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                products = products.Where(p =>
                    p.ProductCode.Contains(searchValue) ||
                    p.ProductName.Contains(searchValue)
                ).ToList(); // <-- Add this
            }


            var data = products.Skip(skip).Take(pageSize).ToList();

            return Json(new
            {
                draw = draw,
                recordsTotal = products.Count(),
                recordsFiltered = products.Count(),
                data = data
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        [Authorize]
        [Route("getProducts")]
        public JsonResult GetProducts()
        {
            var products = _productService.GetAllProductIQueryableForIndex();
            var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(products);

            var result = vmodel.Select(product => new
            {
                product.ProductId,
                product.ProductCode,
                product.ProductName,
                product.CategoryName,
                product.CompanyName,
                product.MRPRate,
                product.RP
            });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Authorize]
        [Route("getEmobileProducts")]
        public JsonResult GetEmobileProducts()
        {
            var products = _productService.GetAllProductIQueryableForIndex();
            var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(products);

            var result = vmodel.Select(product => new
            {
                product.ProductId,
                product.ProductCode,
                product.ProductName,
                product.CategoryName,
                product.CompanyName,
                product.MRPRate,
                product.RP
            });

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }




        //[HttpGet]
        //[Authorize]
        //[Route("index")]
        //public ActionResult Index(int? Page)
        //{
        //    ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
        //    int PageSize = 15;
        //    int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
        //    var customProductAsync = _productService.GetAllProductIQueryable();
        //    var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(customProductAsync);
        //    var pagelist = vmodel.ToPagedList(Pages, PageSize);
        //    ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
        //    return View(pagelist);


        //}

        //[HttpPost]
        //[Authorize]

        //public ActionResult Index(FormCollection formCollection)
        //{
        //    ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
        //    int PageSize = 15;
        //    int Pages = 1;
        //    IQueryable<ProductWisePurchaseModel> customProductAsync = null;
        //    if (!string.IsNullOrEmpty(formCollection["ProductName"]))
        //    {
        //        string productName = formCollection["ProductName"];
        //        customProductAsync = _productService.GetAllProductIQueryable().Where(i => i.ProductName.Contains(productName));
        //    }
        //    var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(customProductAsync);
        //    var pagelist = vmodel.ToPagedList(Pages, PageSize);
        //    ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
        //    return View(pagelist);


        //}

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsEcomputerShow = _sysInfoService.IsEcomputerShow();
            ViewBag.IsProductTypeHide = _sysInfoService.IsProductTypeHide();
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            var model = new CreateProductViewModel { Code = code, UnitType = EnumUnitType.Piece, ProductType = EnumProductType.AutoBC };
            var product = _productService.GetAllProductIQueryable()
                            .OrderByDescending(i => i.ProductID).FirstOrDefault();
            if (product != null)
            {
                model.ProductType = (EnumProductType)product.ProductType;
                model.UnitType = (EnumUnitType)product.UnitType;
            }

            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateProductViewModel newProduct, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsEcomputerShow = _sysInfoService.IsEcomputerShow();
            ViewBag.IsProductTypeHide = _sysInfoService.IsProductTypeHide();
            CheckAndAddModelError(newProduct, formCollection);
            if (!ModelState.IsValid)
            {
                ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                return View(newProduct);
            }
                

            if (newProduct != null)
            {
                var existingProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName);
                if (existingProduct != null)
                {
                    AddToastMessage("", "A Product with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                    return View(newProduct);
                }
                //MapFormCollectionValueWithNewEntity(newProduct, formCollection);
                if (picture != null)
                {
                    var photoName = newProduct.Code + "_" + newProduct.ProductName;
                    newProduct.PicturePath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), picture);
                }

                var product = _mapper.Map<CreateProductViewModel, Product>(newProduct);
                if(ViewBag.IsEcomputerShow == true)
                {
                    product.RP = newProduct.ECOMRP;
                }
                if (ViewBag.IsRPRateShow == true)
                {
                    product.RP = newProduct.RP;
                }
                product.PWDiscount = 0;
                product.ConcernID = User.Identity.GetConcernId();
                product.UnitType = EnumUnitType.Piece;
                _productService.AddProduct(product);
                _productService.SaveProduct();

                AddConcernsProducts(newProduct);

                AddToastMessage("", "Product has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Product data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void UpdateSisterConcernMRP(Product product, string productName)
        {
            var families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                        .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            if (families.Any())
            {
                foreach (var item in families)
                {
                    var concernProduct = _productService.GetAll(item.ConcernID)
                        .FirstOrDefault(i => i.ProductName.TrimEnd().ToLower().Equals(productName.TrimEnd().ToLower()));

                    if (concernProduct != null)
                    {
                        concernProduct.MRP = product.MRP;
                        concernProduct.PurchaseRate = product.PurchaseRate;
                        concernProduct.ProductName = product.ProductName;
                        concernProduct.CompressorWarrentyMonth = product.CompressorWarrentyMonth;
                        concernProduct.PanelWarrentyMonth = product.PanelWarrentyMonth;
                        concernProduct.MotorWarrentyMonth = product.MotorWarrentyMonth;
                        concernProduct.SparePartsWarrentyMonth = product.SparePartsWarrentyMonth;
                        concernProduct.ServiceWarrentyMonth = product.ServiceWarrentyMonth;
                        concernProduct.UserInputWarranty = product.UserInputWarranty;


                        var company = _companyService.GetCompanyById(product.CompanyID);
                        var category = _categoryService.GetCategoryById(product.CategoryID);

                        // Check if the company exists in the concern
                        var existingCompany = _companyService.GetAllCompany(item.ConcernID)
                            .FirstOrDefault(c => c.Name.ToLower() == company.Name.ToLower());

                        if (existingCompany == null)
                        {
                            var newCompany = new Company
                            {
                                Name = company.Name,
                                ConcernID = item.ConcernID,
                                Code = company.Code
                            };
                            _companyService.AddCompany(newCompany);
                            _companyService.SaveCompany();
                            concernProduct.CompanyID = newCompany.CompanyID;
                        }
                        else
                        {
                            concernProduct.CompanyID = existingCompany.CompanyID;
                        }

                        // Check if the category exists in the concern
                        var existingCategory = _categoryService.GetAllIQueryable(item.ConcernID)
                            .FirstOrDefault(c => c.Description.ToLower() == category.Description.ToLower());

                        if (existingCategory == null)
                        {
                            var newCategory = new Category
                            {
                                Description = category.Description,
                                ConcernID = item.ConcernID,
                                Code = category.Code,
                                PCategoryID = category.PCategoryID
                            };
                            _categoryService.AddCategory(newCategory);
                            _categoryService.SaveCategory();
                            concernProduct.CategoryID = newCategory.CategoryID;
                        }
                        else
                        {
                            concernProduct.CategoryID = existingCategory.CategoryID;
                        }

                        _productService.UpdateProduct(concernProduct);
                        _productService.SaveProduct();
                    }
                }
            }
        }

        //private void UpdateSisterConcernMRP(Product product, string productName)
        //{
        //    var families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
        //                            .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
        //    if (families.Any())
        //    {
        //        foreach (var item in families)
        //        {
        //            Product concernProduct = _productService.GetAll(item.ConcernID).Where(i => i.ProductName.TrimEnd().ToLower().Equals(productName.TrimEnd().ToLower())).FirstOrDefault();
        //            if (concernProduct != null)
        //            {
        //                concernProduct.MRP = product.MRP;
        //                concernProduct.PurchaseRate = product.PurchaseRate;
        //                concernProduct.ProductName = product.ProductName;
        //                _productService.UpdateProduct(concernProduct);
        //                _productService.SaveProduct();
        //            }
        //        }
        //    }
        //}

        private void AddConcernsProducts(CreateProductViewModel product)
        {
            var families = _sisterConcernService.GetFamilyTree(User.Identity.GetConcernId())
                                    .Where(i => i.ConcernID != User.Identity.GetConcernId()).ToList();
            Product newProduct = null;
            Company newCompany = null;
            Category newCategory = null;
            var Company = _companyService.GetCompanyById(Convert.ToInt32(product.CompanyId));
            var Category = _categoryService.GetCategoryById(Convert.ToInt32(product.CategoryId));
            var parentCat = _parentCategoryService.GetById(Convert.ToInt32(Category.PCategoryID));
            foreach (var item in families)
            {
                if (_productService.GetAll(item.ConcernID)
                    .Any(i => i.ProductName.ToLower().Equals(product.ProductName.ToLower())))
                    continue;

                if (!_companyService.GetAllCompany(item.ConcernID)
                         .Any(i => i.Name.ToLower().Equals(Company.Name.ToLower())))
                {
                    newCompany = new Company();
                    newCompany.Name = Company.Name;
                    newCompany.ConcernID = item.ConcernID;
                    newCompany.Code = Company.Code;
                    _companyService.AddCompany(newCompany);
                    _companyService.SaveCompany();
                }
                else
                {
                    newCompany = _companyService.GetAllCompany(item.ConcernID)
                                    .FirstOrDefault(i => i.Name.ToLower().Equals(Company.Name.ToLower()));
                }

                if (!_categoryService.GetAllIQueryable(item.ConcernID)
                   .Any(i => i.Description.ToLower().Equals(Category.Description.ToLower())))
                {
                    newCategory = new Category();
                    newCategory.Description = Category.Description;
                    newCategory.ConcernID = item.ConcernID;
                    newCategory.Code = Category.Code;
                    newCategory.PCategoryID = parentCat.PCategoryID;

                    _categoryService.AddCategory(newCategory);
                    _categoryService.SaveCategory();
                }
                else
                {
                    newCategory = _categoryService.GetAllIQueryable(item.ConcernID)
                                    .FirstOrDefault(i => i.Description.ToLower().Equals(Category.Description.ToLower()));
                }



                newProduct = _mapper.Map<CreateProductViewModel, Product>(product);
                AddAuditTrail(newProduct, true);
                newProduct.ProductID = 0;
                newProduct.CompanyID = newCompany.CompanyID;
                newProduct.CategoryID = newCategory.CategoryID;
                newProduct.ConcernID = item.ConcernID;
                _productService.AddProduct(newProduct);
                _productService.SaveProduct();
            }
        }
    
        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsEcomputerShow = _sysInfoService.IsEcomputerShow();
            ViewBag.IsProductTypeHide = _sysInfoService.IsProductTypeHide();
            var product = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.ProductID == id);
            var vmodel = _mapper.Map<ProductWisePurchaseModel, CreateProductViewModel>(product);
            ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateProductViewModel newProduct, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            ViewBag.IsRPRateShow = _sysInfoService.IsRPRateShow();
            ViewBag.IsEcomputerShow = _sysInfoService.IsEcomputerShow();
            ViewBag.IsProductTypeHide = _sysInfoService.IsProductTypeHide();
            CheckAndAddModelError(newProduct, formCollection);
            //if (string.IsNullOrEmpty(newProduct.PicturePath))
            //    ModelState.AddModelError("PicturePath", "Picture is required");

            if (!ModelState.IsValid)
            {
                ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                return View("Create", newProduct);
            }

            if (newProduct != null)
            {
                var existingProduct = _productService.GetProductById(int.Parse(newProduct.ProductId));

                var Oldproduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName
                                    && p.ProductID != existingProduct.ProductID);
                if (Oldproduct != null)
                {
                    AddToastMessage("", "A Product with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    ViewBag.IsMRPUpdateNotApplicable = _sisterConcernService.IsChildConcern(User.Identity.GetConcernId());
                    return View("Create", newProduct);
                }

                MapFormCollectionValueWithExistingEntity(existingProduct, formCollection);

                if (picture != null)
                {
                    var photoName = newProduct.Code + "_" + newProduct.ProductName;
                    existingProduct.PicturePath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), picture);
                }

                if (existingProduct.ProductType != (int)newProduct.ProductType)
                {
                    if (_purchaseOrderService.IsProductPurchase(existingProduct.ProductID))
                        AddToastMessage("", "Product type/unitType can't be changed. This product is already purchase.");
                    else
                    {
                        existingProduct.UnitType = newProduct.UnitType;
                        existingProduct.ProductType = (int)newProduct.ProductType;
                    }
                }

                existingProduct.Code = newProduct.Code;
                string tmpProductName = existingProduct.ProductName;
                existingProduct.ProductName = newProduct.ProductName;
                existingProduct.CompressorWarrentyMonth = newProduct.CompressorWarrentyMonth;
                existingProduct.MotorWarrentyMonth = newProduct.MotorWarrentyMonth;
                existingProduct.PanelWarrentyMonth = newProduct.PanelWarrentyMonth;
                existingProduct.SparePartsWarrentyMonth = newProduct.SparePartsWarrentyMonth;
                existingProduct.ServiceWarrentyMonth = newProduct.ServiceWarrentyMonth;
                existingProduct.UserInputWarranty = newProduct.UserInputWarranty;
                existingProduct.MRP = newProduct.MRP;
                existingProduct.CompanyID = Convert.ToInt32(newProduct.CompanyId);
                existingProduct.CategoryID = Convert.ToInt32(newProduct.CategoryId);
                existingProduct.PurchaseRate = newProduct.MRP;
                var product = _mapper.Map<CreateProductViewModel, Product>(newProduct);
                if (ViewBag.IsEcomputerShow == true)
                {
                    existingProduct.RP = newProduct.ECOMRP;
                }
                if (ViewBag.IsRPRateShow == true)
                {
                    existingProduct.RP = newProduct.RP;
                }
                

                if (newProduct.PWDiscount == null)
                    existingProduct.PWDiscount = 0;
                else
                    existingProduct.PWDiscount = decimal.Parse(newProduct.PWDiscount);

                existingProduct.ConcernID = User.Identity.GetConcernId();

                _productService.UpdateProduct(existingProduct);
                _productService.SaveProduct();

                UpdateSisterConcernMRP(existingProduct, tmpProductName);

                AddToastMessage("", "Product has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Product data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);
            _productService.SaveProduct();
            AddToastMessage("", "Product has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(CreateProductViewModel newProduct, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newProduct.ProductName))
                ModelState.AddModelError("ProductName", "ProductName is required");

            if (string.IsNullOrEmpty(newProduct.CompanyId))
                ModelState.AddModelError("CompanyId", "Company is required");

            if (string.IsNullOrEmpty(newProduct.CategoryId))
                ModelState.AddModelError("CategoryId", "Category is required");

            if (!string.IsNullOrEmpty(newProduct.CompressorWarrentyMonth))
            {
                var comp = newProduct.CompressorWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.CompressorWarrentyMonth = comp[0] + " " + formCollection["Compressor"];
                else
                    newProduct.CompressorWarrentyMonth = newProduct.CompressorWarrentyMonth + " " + formCollection["Compressor"];
            }

            if (!string.IsNullOrEmpty(newProduct.MotorWarrentyMonth))
            {
                var comp = newProduct.MotorWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.MotorWarrentyMonth = comp[0] + " " + formCollection["Motor"];
                else
                    newProduct.MotorWarrentyMonth = newProduct.MotorWarrentyMonth + " " + formCollection["Motor"];

            }

            if (!string.IsNullOrEmpty(newProduct.PanelWarrentyMonth))
            {
                var comp = newProduct.PanelWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.PanelWarrentyMonth = comp[0] + " " + formCollection["Panel"];
                else
                    newProduct.PanelWarrentyMonth = newProduct.PanelWarrentyMonth + " " + formCollection["Panel"];

            }

            if (!string.IsNullOrEmpty(newProduct.ServiceWarrentyMonth))
            {
                var comp = newProduct.ServiceWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.ServiceWarrentyMonth = comp[0] + " " + formCollection["Service"];
                else
                    newProduct.ServiceWarrentyMonth = newProduct.ServiceWarrentyMonth + " " + formCollection["Service"];

            }

            if (!string.IsNullOrEmpty(newProduct.SparePartsWarrentyMonth))
            {
                var comp = newProduct.SparePartsWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.SparePartsWarrentyMonth = comp[0] + " " + formCollection["SpareParts"];
                else
                    newProduct.SparePartsWarrentyMonth = newProduct.SparePartsWarrentyMonth + " " + formCollection["SpareParts"];

            }

        }

        private void MapFormCollectionValueWithNewEntity(CreateProductViewModel newProduct,
            FormCollection formCollection)
        {
            newProduct.CompanyId = formCollection["CompaniesId"];
            newProduct.CategoryId = formCollection["CategoriesId"];
        }

        private void MapFormCollectionValueWithExistingEntity(Product product,
            FormCollection formCollection)
        {
            product.DisDurationFDate = Convert.ToDateTime("31 Dec 2017");//DateTime.Parse(formCollection["DisDurationFDate"]);
            product.DisDurationToDate = Convert.ToDateTime("31 Dec 2017");//DateTime.Parse(formCollection["DisDurationToDate"]);
            //product.CompanyID = int.Parse(formCollection["CompaniesId"]);
            //product.CategoryID = int.Parse(formCollection["CategoriesId"]);
            product.PWDiscount = 0;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePandSReport()
        {
            return View("ProductWisePandSReport");
        }

        [HttpPost]
        public JsonResult GetProductByName(string ProductName)
        {
            var products = (from d in _productService.GetAllProductIQueryable()
                            where d.ProductName.ToLower().Contains(ProductName.ToLower())
                            select new
                            {
                                d.ProductName,
                                d.ProductID
                            }).ToList();
            if (products.Count() > 0)
                return Json(products, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddProduct(CreateProductViewModel newProduct, FormCollection formCollection)
        {
            newProduct.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            CheckAndAddModelError(newProduct, formCollection);
            if (!ModelState.IsValid)
                return Json(new { result = false, msg = "Product save failed." }, JsonRequestBehavior.AllowGet);
            var existingProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName);
            if (existingProduct != null)
            {
                return Json(new { result = false, msg = "A Product with same name already exists in the system. Please try with a different name." }, JsonRequestBehavior.AllowGet);
            }
            var product = _mapper.Map<CreateProductViewModel, Product>(newProduct);
            product.PWDiscount = 0;
            AddAuditTrail(product, true);
            _productService.AddProduct(product);
            _productService.SaveProduct();

            AddConcernsProducts(newProduct);
            var vmProduct = new
            {
                ProductID = product.ProductID,
                Code = product.Code,
                ProductType = product.ProductType,
                ProductName = product.ProductName,
                MRP = product.MRP,
                RP = product.RP
            };
            return Json(new { result = true, msg = "Save successfull", data = vmProduct }, JsonRequestBehavior.AllowGet);
        }


    }
}