using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class DOController : CoreController
    {
        IDOService _DOService;
        IMiscellaneousService<DO> _miscellaneousService;
        IMapper _mapper;
        IProductService _productService;
        IStockService _stockService;
        IGodownService _GodownService;
        IColorService _ColorService;
        IStockDetailService _stockDetailService;
        ICategoryService _categoryService;

        private readonly ISystemInformationService _systemInformationService;
        public DOController(IErrorService errorService,
            IDOService doService, IMiscellaneousService<DO> miscellaneousService,
            IMapper mapper, IProductService productService,
            IStockService stockService, IGodownService GodownService, IColorService ColorService, ISystemInformationService systemInformationService, IStockDetailService stockDetailService, ICategoryService categoryService)
            : base(errorService, systemInformationService)
        {
            _DOService = doService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _productService = productService;
            _stockService = stockService;
            _GodownService = GodownService;
            _ColorService = ColorService;
            _stockDetailService = stockDetailService;
            _categoryService = categoryService;
            this._systemInformationService = systemInformationService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            TempData["DOEntry"] = null;
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;

            var categoriesAsync = _DOService.GetAll(EnumDOStatus.DO, ViewBag.FromDate, ViewBag.ToDate, true, User.Identity.GetConcernId());
            var vmodel = _mapper.Map<List<DO>, IEnumerable<DOViewModel>>(categoriesAsync);
            return View(vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("index")]
        public ActionResult Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            var categoriesAsync = _DOService.GetAll(EnumDOStatus.DO, ViewBag.FromDate, ViewBag.ToDate, true, User.Identity.GetConcernId());
            var vmodel = _mapper.Map<List<DO>, IEnumerable<DOViewModel>>(categoriesAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }


        private ActionResult ReturnCreateViewWithTempData()
        {
            DOViewModel DO = (DOViewModel)TempData.Peek("DOEntry");
            if (DO != null)
            {
                TempData["DOEntry"] = DO;
                return View("Create", DO);
            }
            else
            {
                string invNo = "OS" + _miscellaneousService.GetUniqueKey(x => x.DOID);
                return View(new DOViewModel
                {
                    Detail = new DODetailViewModel(),
                    Details = new List<DODetailViewModel>(),
                    DONo = invNo
                });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(DOViewModel newProduction, FormCollection formCollection)
        {
            return ProcessDOEntry(newProduction, formCollection);
        }

        private ActionResult ProcessDOEntry(DOViewModel newDOVM, FormCollection formCollection)
        {
            if (newDOVM != null)
            {
                DOViewModel DOVM = (DOViewModel)TempData.Peek("DOEntry");
                DOVM = DOVM ?? new DOViewModel();

                DOVM.Detail = new DODetailViewModel();

                if (formCollection.Get("btnAdd") != null)
                {
                    CheckAndAddModelErrorForAdd(newDOVM, DOVM, formCollection);
                    if (!ModelState.IsValid)
                    {
                        DOVM.Details = DOVM.Details ?? new List<DODetailViewModel>();
                        return View("Create", DOVM);
                    }
                    var product = _productService.GetProductById(int.Parse(newDOVM.Detail.ProductID));

                    if (DOVM.Details != null &&
                        DOVM.Details.Any(x => x.ProductID.Equals(newDOVM.Detail.ProductID)
                         && x.ColorID.Equals(newDOVM.Detail.ColorID)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the DO", ToastType.Error);
                        return View("Create", DOVM);
                    }

                    if (DOVM.Details != null &&
                        DOVM.Details.Any(x => x.ProductID.Equals(newDOVM.Detail.ProductID)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the DO", ToastType.Error);
                        return View("Create", DOVM);
                    }

                    AddDetailToEntry(newDOVM, DOVM, formCollection);
                    ModelState.Clear();
                    return View("Create", DOVM);
                }
                else if (formCollection.Get("btnSave") != null)
                {
                    CheckAndAddModelErrorForSave(newDOVM, DOVM, formCollection);

                    if (!ModelState.IsValid || DOVM.Details.Count() == 0)
                    {
                        DOVM.Details = DOVM.Details ?? new List<DODetailViewModel>();
                        return View("Create", DOVM);
                    }


                    bool Result = SaveOrder(newDOVM, DOVM, formCollection);
                    ModelState.Clear();
                    TempData["DOEntry"] = null;

                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new DOViewModel
                    {
                        Detail = new DODetailViewModel(),
                        Details = new List<DODetailViewModel>()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No DO data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private bool SaveOrder(DOViewModel newDOModel, DOViewModel DOModel, FormCollection formCollection)
        {
            bool Result = false;
            try
            {
                UpdateParentModelData(newDOModel, DOModel);

                #region Log
                //log.Info(new { SalesOrder = salesOrder.SalesOrder, SODetails = salesOrder.SODetails });
                #endregion

                DO oNewDo = new DO();
                oNewDo.Date = DOModel.Date;
                oNewDo.DONo = DOModel.DONo;
                oNewDo.TotalAmt = DOModel.TotalAmt;
                oNewDo.TotalAmt = Convert.ToDecimal(formCollection["TotalAmt"]);
                Decimal flatDiscount = Convert.ToDecimal(formCollection["TotalDiscountAmount"]);
                oNewDo.NetDiscount = DOModel.NetDiscount + flatDiscount;
                oNewDo.GrandTotal = DOModel.GrandTotal;
                oNewDo.FlatDiscount = Convert.ToDecimal(formCollection["TotalDiscountAmount"]);
                oNewDo.FlatDiscountPer = Convert.ToDecimal(formCollection["TotalDiscountPercentage"]);

                oNewDo.PaidAmt = DOModel.PaidAmt;
                oNewDo.Remarks = DOModel.Remarks;
                oNewDo.CustomerID = DOModel.CustomerID;
                oNewDo.DOID = Convert.ToInt32(DOModel.DOID);
                oNewDo.Status = EnumDOStatus.DO;

                if (Convert.ToInt32(DOModel.DOID) > 0)
                    AddAuditTrail(oNewDo, false);
                else
                    AddAuditTrail(oNewDo, true);

                oNewDo.DODetails = _mapper.Map<List<DODetailViewModel>, ICollection<DODetail>>(DOModel.Details);



                var tResult = _DOService.ADDDOEntry(oNewDo, Convert.ToInt32(DOModel.DOID));

                Result = tResult.Item1;

                if (tResult.Item1)
                {
                    TempData["ISDOReady"] = true;
                    TempData["DOID"] = tResult.Item2;
                }

                #region For POS Invoice
                //PrintInvoice oPriInvoice = new PrintInvoice();
                //oPriInvoice.print(salesOrder, _SisterConcern);
                #endregion

                if (Result)
                    AddToastMessage("", "DO has been saved successfully.", ToastType.Success);
                else
                    AddToastMessage("", "DO has been failed.", ToastType.Error);
            }
            catch (Exception ex)
            {
                Result = false;
                throw new Exception(ex.Message);
            }
            return Result;
        }

        private void CheckAndAddModelErrorForSave(DOViewModel newDOVM, DOViewModel DOVM, FormCollection formCollection)
        {
            if (newDOVM.Date == DateTime.MinValue)
                ModelState.AddModelError("Date", "Date is required.");
            if (newDOVM.TotalAmt < 1)
                ModelState.AddModelError("TotalAmt", "TotalAmt is required.");
            if (DOVM.Details.Count() == 0)
                ModelState.AddModelError("TotalAmt", "Please add product first.");

            if (string.IsNullOrWhiteSpace(formCollection["CustomersId"]))
                ModelState.AddModelError("CustomerID", "Customer is required.");
            else
            {
                newDOVM.CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
                DOVM.CustomerID = Convert.ToInt32(formCollection["CustomersId"]);
            }
        }

        private void AddDetailToEntry(DOViewModel newDOVM, DOViewModel DOVM, FormCollection formCollection)
        {
            //finBrickStockEntry.DOID = newProduction.DOID;

            DOVM.Detail = newDOVM.Detail;
            DOVM.Details = DOVM.Details ?? new List<DODetailViewModel>();
            DOVM.Details.Add(DOVM.Detail);
            UpdateParentModelData(newDOVM, DOVM);

            DOViewModel vm = new DOViewModel
            {
                Detail = new DODetailViewModel(),
                Details = DOVM.Details,
                Date = DOVM.Date,
                DONo = DOVM.DONo,
                DOID = DOVM.DOID,
                TotalAmt = DOVM.TotalAmt,
                CustomerID = DOVM.CustomerID,
                PaymentDue = DOVM.PaymentDue,
                ColorID = DOVM.ColorID,
                GrandTotal = DOVM.GrandTotal,
                TempTootalAmount = DOVM.TotalAmt,
                NetDiscount = DOVM.NetDiscount,
                TotalDiscountAmount = 0,
                TotalDiscountPercentage = 0,



                //RoundingID = DOVM.RoundingID
            };

            TempData["DOEntry"] = vm;
            DOVM.Detail = new DODetailViewModel();
            AddToastMessage("", "DO has been added successfully.", ToastType.Success);
        }


        void UpdateParentModelData(DOViewModel newDOVM, DOViewModel DOVM)
        {
            decimal TotalPPDisAmt = DOVM.Details.Sum(i => i.PPDisAmt * i.DOQty);
            DOVM.Date = newDOVM.Date;
            DOVM.DONo = newDOVM.DONo;
            DOVM.CustomerID = newDOVM.CustomerID;
            DOVM.Remarks = newDOVM.Remarks;
            DOVM.DOID = newDOVM.DOID;
            DOVM.NetDiscount = DOVM.Details.Sum(i => i.PPDisAmt * i.DOQty);
            DOVM.GrandTotal = newDOVM.GrandTotal;
            DOVM.PPDiscountAmount = DOVM.Details.Sum(i => i.PPDisAmt * i.DOQty);
            DOVM.PaidAmt = newDOVM.PaidAmt;
            DOVM.TotalAmt = DOVM.Details.Sum(i => i.TotalAmt);
            DOVM.GrandTotal = DOVM.Details.Sum(i => i.DDLiftingPrice * i.DOQty);
            DOVM.PaymentDue = DOVM.TotalAmt - DOVM.PaidAmt;
            DOVM.PPDisAmt = (decimal.Parse(GetDefaultIfNull(DOVM.PPDisAmt)) - TotalPPDisAmt).ToString();
            DOVM.tempFlatDiscountAmount = TotalPPDisAmt.ToString();
            DOVM.tempNetDiscount = DOVM.NetDiscount.ToString();
            DOVM.TotalDiscountAmount = 0;
            DOVM.TotalDiscountPercentage = 0;
            DOVM.NetDiscount = DOVM.TotalDiscountAmount + DOVM.Details.Sum(i => i.PPDisAmt * i.DOQty);
            DOVM.tempNetDiscount = DOVM.NetDiscount.ToString();
            DOVM.ColorID = DOVM.ColorID;
            //mostafizur

            //DOVM.TotalAmt = DOVM.TotalAmt + newDOVM.Detail.TotalAmt;
        }


        private void CheckAndAddModelErrorForAdd(DOViewModel newDOviewModel, DOViewModel dovm,
           FormCollection formCollection)
        {

            if (string.IsNullOrWhiteSpace(formCollection["ProductsName"]))
                ModelState.AddModelError("Detail.ProductID", "Product is required.");
            else
            {
                newDOviewModel.Detail.ProductID = formCollection["ProductsId"];
                dovm.Detail.ProductID = formCollection["ProductsId"];
                newDOviewModel.Detail.ProductCode = formCollection["ProductsCode"];
                newDOviewModel.Detail.ProductName = formCollection["ProductsName"];

                var ColorID = formCollection["Detail.ColorID"].Trim(',');
                var Spliet = ColorID.Split(',').OrderByDescending(item => item).ToArray();
                for (int i = 0; i < Spliet.Length; i++)
                {
                    var actualColorId = Spliet[i];
                    var getColorId = dovm.ColorID;
                    if (getColorId != actualColorId)
                    {
                        newDOviewModel.Detail.ColorID = Spliet[i];
                    }

                }


            }


            if (string.IsNullOrEmpty(newDOviewModel.Detail.ColorID))
                ModelState.AddModelError("Detail.ColorID", "Color is required");

            if (Convert.ToDecimal(newDOviewModel.Detail.DOQty) < 1)
                ModelState.AddModelError("Detail.DOQty", "DO quantity is required.");

            if (Math.Abs(newDOviewModel.Detail.TotalAmt - (newDOviewModel.Detail.DOQty * newDOviewModel.Detail.UnitPrice)) > 1)
                ModelState.AddModelError("Detail.TotalAmt", "Total amount is not correct.");

            if (!string.IsNullOrWhiteSpace(formCollection["CustomersId"]))
                newDOviewModel.CustomerID = Convert.ToInt32(formCollection["CustomersId"]);

        }



        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var DO = _DOService.GetById(id);
            var details = (from d in _DOService.GetDetailsById(id)
                           join rp in _productService.GetDOProducts() on d.ProductID equals rp.ProductID
                           join dod in _DOService.GetAll() on d.DOID equals dod.DOID
                           join c in _ColorService.GetAll() on d.ColorID equals c.ColorID
                           select new DODetailViewModel
                           {
                               ProductID = d.ProductID.ToString(),
                               ProductName = rp.ProductName,
                               ColorID = d.ColorID.ToString(),
                               ColorCode = c.Code,
                               ColorName = c.Name,
                               DOQty = d.DOQty,
                               GivenQty = d.GivenQty,
                               MRP = d.MRP,
                               GrandTotal = dod.GrandTotal,
                               NetDiscount = dod.NetDiscount,
                               DDLiftingPrice = d.DDLiftingPrice,
                               TotalAmt = d.TotalAmt,
                               TotalSoilPrice = d.TotalAmt,
                               DODID = d.DODID.ToString(),
                               DOID = d.DOID.ToString(),
                               UnitPrice = d.UnitPrice,
                               PPDisPercent = d.PPDisPercent,
                               PPDisAmt = d.PPDisAmt,
                               TotalDiscountPercentage = dod.FlatDiscountPer,
                               TotalDiscountAmount = dod.FlatDiscount,
                           }).ToList();

            var totalPPDisAmt = details.Sum(d => d.PPDisAmt * d.DOQty);
            var netDiscount = details[0].NetDiscount;
            var grandTotal = details[0].GrandTotal;
            decimal totalPercentAmt = netDiscount - totalPPDisAmt;
            var TotalDiscountPercentage = 100 * totalPercentAmt / grandTotal;

            if (details.Any(d => d.GivenQty > 0))
            {
                AddToastMessage("", "This Product All Ready Sold, Edit Not Possible, Anything else Please Contact Support Center", ToastType.Error);
                return RedirectToAction("Index");
            }

            var vmodel = _mapper.Map<DO, DOViewModel>(DO);
            vmodel.Details = details;
            //vmodel.TotalDiscountAmount = totalPercentAmt;
            //vmodel.TotalDiscountPercentage = TotalDiscountPercentage;
            vmodel.tempNetDiscount = totalPPDisAmt.ToString();
            vmodel.tempFlatDiscountAmount = "0";
            vmodel.Detail = new DODetailViewModel();
            TempData["DOEntry"] = vmodel;
            return View("Create", vmodel);
        }


        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(DOViewModel newCarViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newCarViewModel);

            if (newCarViewModel != null)
            {
                var DO = _DOService.GetById(Convert.ToInt32(newCarViewModel.DOID));

                DO.DONo = newCarViewModel.DONo;
                DO.Date = newCarViewModel.Date;
                AddAuditTrail(DO, false);
                _DOService.Update(DO);
                _DOService.Save();

                AddToastMessage("", "DO has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No DO data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }


        private static void UpdateDOFromView(DOViewModel DO, DODetailViewModel itemToDelete)
        {
            DO.TotalAmt -= itemToDelete.TotalAmt;
            DO.GrandTotal -= itemToDelete.DDLiftingPrice * itemToDelete.DOQty;
            DO.NetDiscount -= itemToDelete.PPDisAmt * itemToDelete.DOQty;
            //DO.TotalDiscountAmount = DO.TotalDiscountAmount;
            DO.PaymentDue -= (itemToDelete.DDLiftingPrice - itemToDelete.PPDisAmt) * itemToDelete.DOQty;
            DO.ColorID = itemToDelete.ColorID;

        }


        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var DO = _DOService.GetById(id);
            var details = (from d in _DOService.GetDetailsById(id)
                           join rp in _productService.GetDOProducts() on d.ProductID equals rp.ProductID
                           join c in _ColorService.GetAll() on d.ColorID equals c.ColorID
                           select new DODetailViewModel
                           {
                               ProductID = d.ProductID.ToString(),
                               ProductName = rp.ProductName,
                               ColorID = d.ColorID.ToString(),
                               ColorCode = c.Code,
                               ColorName = c.Name,
                               DOQty = d.DOQty,
                               GivenQty = d.GivenQty,
                               MRP = d.MRP,
                               DDLiftingPrice = d.DDLiftingPrice,
                               TotalAmt = d.TotalAmt,
                               TotalSoilPrice = d.TotalAmt,
                               DODID = d.DODID.ToString(),
                               DOID = d.DOID.ToString(),
                               UnitPrice = d.UnitPrice,
                               PPDisPercent = d.PPDisPercent,
                               PPDisAmt = d.PPDisAmt,
                           }).ToList();

            if (details.Any(d => d.GivenQty == 0))
            {
                if (_DOService.Delete(id, User.Identity.GetUserId<int>(), GetLocalDateTime()))
                    AddToastMessage("", "Purchase DO has been deleted successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }


            else
                AddToastMessage("", "This Product All Ready Purchased delete failed.", ToastType.Error);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult DeleteFromView(int id, string previousAction)
        {
            var DO = (DOViewModel)TempData.Peek("DOEntry");
            if (DO == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            var itemToDelete = DO.Details.Where(x => int.Parse(x.ProductID) == id).FirstOrDefault();
            if (itemToDelete != null)
            {
                UpdateDOFromView(DO, itemToDelete);
                //DO.TotalAmt -= itemToDelete.TotalAmt;
                DO.Details.Remove(itemToDelete);
                DO.Detail = new DODetailViewModel();
                TempData["DOEntry"] = DO;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);

                //if (IsForEdit(previousAction))
                //    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                //else
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public ActionResult EditFromView(int id, string previousAction)
        {
            var dos = (from p in _productService.GetDOProducts()
                       join dd in _DOService.GetAllDetail() on p.ProductID equals dd.ProductID
                       where dd.ProductID == id
                       select dd.ColorID).FirstOrDefault();

            var DO = (DOViewModel)TempData.Peek("DOEntry");
            if (DO == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            var itemToEdit = DO.Details.Where(x => int.Parse(x.ProductID) == id).FirstOrDefault();

            if (itemToEdit != null)
            {
                UpdateDOFromView(DO, itemToEdit);
                //DO.TotalAmt -= itemToEdit.TotalAmt;
                DO.Details.Remove(itemToEdit);
                DO.Detail = itemToEdit;
                TempData["DOEntry"] = DO;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }
        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }

        [HttpGet]
        public ActionResult GetInvoice(int id)
        {
            TempData["ISDOReady"] = true;
            TempData["DOID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetInvoiceExcel(int id)
        {
            TempData["ISDOReady2"] = true;
            TempData["DOID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DOReport()
        {
            return View();
        }




        [HttpGet]
        public JsonResult GetDOByID(int DOID)
        {
            var dos = (from d in _DOService.GetAll()
                       join dd in _DOService.GetAllDetail() on d.DOID equals dd.DOID
                       join p in _productService.GetDOProducts() on dd.ProductID equals p.ProductID
                       join st in _stockService.GetAll() on p.ProductID equals st.ProductID into lst
                       from st in lst.DefaultIfEmpty()
                       join std in _stockDetailService.GetAll() on dd.ProductID equals std.ProductID
                       join co in _ColorService.GetAll() on std.ColorID equals co.ColorID
                       where d.DOID == DOID && (dd.DOQty - dd.GivenQty) > 0 && d.Status == EnumDOStatus.DO && std.Status == 1
                       group new
                       {
                           id = dd.ProductID,
                           StockID = st != null ? st.StockID : 0,
                           StockDetailID = std != null ? std.SDetailID : 0,
                           preStock = dd != null ? dd.DOQty - dd.GivenQty : 0,
                           code = p.ProductCode,
                           name = p.ProductName,
                           p.CompanyName,
                           category = p.CategoryName,
                           MRP = dd.MRP,
                           DDLiftingPrice = dd.MRP,
                           ColorName = co.Name,
                           ColorID = co.ColorID,
                           Qty = Math.Truncate(dd.DOQty),
                           ProductsType = p.ProductType,
                           dd.GivenQty,
                           BalanceQty = (dd.DOQty - dd.GivenQty)
                       } by new { dd.ProductID, dd.DOID } into grouped
                       select grouped.FirstOrDefault()).ToList();

            if (dos.Count() > 0)
                return Json(new { data = dos, result = true }, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetDOByCustomerID(int CustomerID, string Prefix)
        {
            var dos = (from d in _DOService.GetAll()
                       where d.CustomerID == CustomerID && d.Status == EnumDOStatus.DO && d.DONo.Contains(Prefix)
                       select new DOViewModel
                       {
                           DONo = d.DONo,
                           Date = d.Date,
                           DOID = d.DOID.ToString(),
                       }).ToList();
            foreach (var item in dos)
            {
                item.Remarks = item.Date.ToString("dd MMM yyyy") + " " + item.DONo;
            }
            if (dos.Count() > 0)
                return Json(dos, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDOBySupplierID(int SupplierID, string Prefix)
        {
            var dos = (from d in _DOService.GetAll()
                           //join dd in _DOService.GetAllDetail() on d.DOID equals dd.DOID
                           //join p in _productService.GetProducts() on dd.ProductID equals p.ProductID
                       where d.SupplierID == SupplierID && /*(dd.DOQty - dd.GivenQty) > 0 &&*/ d.Status == EnumDOStatus.DO && d.DONo.Contains(Prefix)
                       select new DOViewModel
                       {
                           DONo = d.DONo,
                           Date = d.Date,
                           DOID = d.DOID.ToString(),
                       }).ToList();
            foreach (var item in dos)
            {
                item.Remarks = item.Date.ToString("dd MMM yyyy") + " " + item.DONo;
            }
            if (dos.Count() > 0)
                return Json(dos, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}