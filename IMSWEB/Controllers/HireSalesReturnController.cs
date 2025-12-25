using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class HireSalesReturnController : CoreController
    {
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IROrderService _returnOrderService;
        private readonly ICreditSalesOrderService _creditSalesOrderService;
        private readonly IMapper _mapper;
        IMiscellaneousService<HireSalesReturnCustomerDueAdjustment> _miscellaneousService;

        public HireSalesReturnController(IErrorService errorService, IProductService productService, ICustomerService customerService, IROrderService returnOrderService, ICreditSalesOrderService creditSalesOrderService, IMapper mapper, IMiscellaneousService<HireSalesReturnCustomerDueAdjustment> miscellaneousService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _productService = productService;
            _customerService = customerService;
            _returnOrderService = returnOrderService;
            _creditSalesOrderService = creditSalesOrderService;
            _mapper = mapper;
            _miscellaneousService = miscellaneousService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            TempData["creditSalesOrderViewModel"] = null;

            TempData["hsalesOrderViewModel"] = null;

            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var customSO = _creditSalesOrderService.GetAllSalesReturnOrderAsync(ViewBag.FromDate, ViewBag.ToDate, IsVATManager(), User.Identity.GetConcernId());
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            TempData["creditSalesOrderViewModel"] = null;
            TempData["hsalesOrderViewModel"] = null;

            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var customSO = _creditSalesOrderService.GetAllSalesReturnOrderAsync(ViewBag.FromDate, ViewBag.ToDate, IsVATManager(), User.Identity.GetConcernId());
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }


        #region Create Get
        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }
        #endregion


        #region Create Post

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(SalesOrderViewModel newReplacementOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleSalesOrder(newReplacementOrder, formCollection);
        }
        #endregion


        #region Handle Sale return
        private ActionResult HandleSalesOrder(SalesOrderViewModel newReplacementOrder, FormCollection formCollection)
        {
            bool Result = false;

            if (newReplacementOrder != null)
            {
                SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("hsalesOrderViewModel");
                decimal prevRemainingDue = salesOrder != null ? salesOrder.SalesOrder.CreditRemainingDue : 0m;
                salesOrder = salesOrder ?? new SalesOrderViewModel()
                {
                    SalesOrder = newReplacementOrder.SalesOrder
                };
                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newReplacementOrder, salesOrder, formCollection);

                    if (!ModelState.IsValid)
                    {
                        IEnumerable<ModelError> errors = ModelState.Values.SelectMany(m => m.Errors);
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        AddToastMessage(string.Empty, "Invalid Data!", ToastType.Error);
                        return View("Create", salesOrder);
                    }

                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                        x.IMENo.Equals(newReplacementOrder.SODetail.DamageIMEINO) && x.ProductId.Equals(newReplacementOrder.SODetail.ProductId)))
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View("Create", salesOrder);
                    }

                    if (salesOrder.SODetails != null && salesOrder.SODetails.Any())
                    {
                        int hireSalesId = int.Parse(GetDefaultIfNull(formCollection["HireSalesId"]));
                        if (salesOrder.SalesOrder.HireSalesId != hireSalesId)
                        {
                            salesOrder.SalesOrder.CreditRemainingDue = prevRemainingDue;

                            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                            AddToastMessage(string.Empty, "Please select only same invoice products!", ToastType.Error);
                            return View("Create", salesOrder);
                        }
                    }

                    AddToOrder(newReplacementOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", salesOrder);
                }

                else if (formCollection.Get("btnReturn") != null)
                {
                    CheckAndAddModelErrorForSave(newReplacementOrder, salesOrder, formCollection);
                    if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
                    {
                        ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
                        salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
                    }
                    ModelState.Remove("SODetail.CreditSalesDetailId");
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }

                    Result = SaveOrder(newReplacementOrder, salesOrder, formCollection);

                    ModelState.Clear();

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");

                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newSalesOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required.");
            else
                newSalesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.MemoNo))
            //    ModelState.AddModelError("SalesOrder.MemoNo", "Memo no is required.");

            if (string.IsNullOrEmpty(formCollection["dProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required.");
            else
                newSalesOrder.SODetail.ProductId = formCollection["dProductDetailsId"];

        }


        private void AddToOrder(SalesOrderViewModel newSalesOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            salesOrder.SalesOrder.HireSalesId = int.Parse(GetDefaultIfNull(formCollection["HireSalesId"]));
            salesOrder.SalesOrder.MemoNo = newSalesOrder.SalesOrder.MemoNo;
            salesOrder.SalesOrder.CreditRemainingDue = newSalesOrder.SalesOrder.CreditRemainingDue;
            salesOrder.SalesOrder.ToCustomerPayAmt = newSalesOrder.SalesOrder.ToCustomerPayAmt;
            salesOrder.SalesOrder.Remarks = newSalesOrder.SalesOrder.Remarks;

            salesOrder.SODetail.CreditSalesDetailId = newSalesOrder.SODetail.CreditSalesDetailId;
            salesOrder.SODetail.ProductId = formCollection["dProductDetailsId"];

            salesOrder.SODetail.IMENo = newSalesOrder.SODetail.DamageIMEINO;
            salesOrder.SODetail.Quantity = formCollection["dQuantity"];
            salesOrder.SODetail.CRSalePrice = newSalesOrder.SODetail.CRSalePrice;
            salesOrder.SODetail.CRPurchasePrice = newSalesOrder.SODetail.CRPurchasePrice;

            salesOrder.SODetail.ProductName = formCollection["dProductDetailsName"];
            salesOrder.SODetail.Status = newSalesOrder.SODetail.Status == default(int) ? EnumStatus.New : newSalesOrder.SODetail.Status;


            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
            salesOrder.SODetails.Add(salesOrder.SODetail);

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["hsalesOrderViewModel"] = vm;
            salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }


        #endregion

        private void CheckAndAddModelErrorForSave(SalesOrderViewModel newReplacementOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {

            decimal prevCreditRemaingDue = decimal.Parse(GetDefaultIfNull(formCollection["prevCreditRemainingDue"]));
            decimal creditRemainingDue = decimal.Parse(GetDefaultIfNull(formCollection["creditRemainingDue"]));

            if (decimal.Parse(GetDefaultIfNull(Convert.ToString(creditRemainingDue))) > decimal.Parse(GetDefaultIfNull(Convert.ToString(prevCreditRemaingDue))))
                ModelState.AddModelError("SalesOrder.CurrentDue", "Adjustment can't be negative");

            //if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
            //    ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid");

        }


        #region Save Hire Sale return
        private bool SaveOrder(SalesOrderViewModel newReplacementOrder,
            SalesOrderViewModel replacementOrder, FormCollection formCollection)
        {

            replacementOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            replacementOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            decimal purchaseRate = decimal.Parse(GetDefaultIfNull(formCollection["dMRPRate"]));
            decimal salePrice = decimal.Parse(GetDefaultIfNull(formCollection["dUnitPrice"]));
            int hireSalesId = int.Parse(GetDefaultIfNull(formCollection["HireSalesId"]));
            decimal prevCreditRemaingDue = decimal.Parse(GetDefaultIfNull(formCollection["prevCreditRemainingDue"]));           
            decimal creditRemainingDue = replacementOrder.SalesOrder.CreditRemainingDue;        
            decimal ToCustomerPayAmt = newReplacementOrder.SalesOrder.ToCustomerPayAmt;
            replacementOrder.SalesOrder.Remarks = newReplacementOrder.SalesOrder.Remarks;
            replacementOrder.SalesOrder.MemoNo = newReplacementOrder.SalesOrder.MemoNo;
            decimal adjustedDue = creditRemainingDue;         


            #region customer due data table
            DataTable dtCustomerDue = new DataTable();
            dtCustomerDue.Columns.Add("AdjDue", typeof(decimal));
            dtCustomerDue.Columns.Add("TotalRemainingDue", typeof(decimal));
            dtCustomerDue.Columns.Add("TransactionDate", typeof(DateTime));
            dtCustomerDue.Columns.Add("CreditSalesId", typeof(int));
            dtCustomerDue.Columns.Add("CustomerId", typeof(int));
            dtCustomerDue.Columns.Add("ConcernId", typeof(int));
            dtCustomerDue.Columns.Add("ToCustomerPayAmt", typeof(decimal));
            dtCustomerDue.Columns.Add("Remarks", typeof(string));
            dtCustomerDue.Columns.Add("MemoNo", typeof(string));


            DataRow row = dtCustomerDue.NewRow();

            row["AdjDue"] = adjustedDue;
            row["TotalRemainingDue"] = prevCreditRemaingDue;
            row["TransactionDate"] = replacementOrder.SalesOrder.OrderDate;
            row["CreditSalesId"] = hireSalesId;
            row["CustomerId"] = replacementOrder.SalesOrder.CustomerId;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["ToCustomerPayAmt"] = ToCustomerPayAmt;
            row["Remarks"] = replacementOrder.SalesOrder.Remarks;
            row["MemoNo"] = replacementOrder.SalesOrder.MemoNo;


            dtCustomerDue.Rows.Add(row);
            #endregion

            DataTable dtCrDetails = CreateCreditDetailsDataTable(replacementOrder, newReplacementOrder, formCollection);

            bool Result = _returnOrderService.AddHireSalesReturnUsingSP(dtCustomerDue, dtCrDetails, salePrice, purchaseRate);
            if (Result)
                AddToastMessage("", "Return has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Return has been Failed.", ToastType.Error);

            //  _salesOrderService.CorrectionStockData(User.Identity.GetConcernId());

            return Result;

        }

        private DataTable CreateCreditDetailsDataTable(SalesOrderViewModel replacementOrder, SalesOrderViewModel newReplacementOrder, FormCollection formCollection)
        {
            DataTable dtCreditDetails = new DataTable();
            dtCreditDetails.Columns.Add("CreditSaleDetailsId", typeof(int));
            dtCreditDetails.Columns.Add("SalePrice", typeof(decimal));
            dtCreditDetails.Columns.Add("PurchasePrice", typeof(decimal));
            dtCreditDetails.Columns.Add("CreditSalesId", typeof(int));


            if (replacementOrder.SODetails.Any())
            {
                foreach (var item in replacementOrder.SODetails)
                {
                    DataRow row = dtCreditDetails.NewRow();
                    row["CreditSaleDetailsId"] = item.CreditSalesDetailId;
                    row["SalePrice"] = item.CRSalePrice;
                    row["PurchasePrice"] = item.CRPurchasePrice;
                    row["CreditSalesId"] = replacementOrder.SalesOrder.HireSalesId;

                    dtCreditDetails.Rows.Add(row);
                }
            }

            return dtCreditDetails;

        }
        #endregion


        #region get cred product picker
        [HttpGet]
        [Authorize]
        public JsonResult GetCreditSalesProductDetailByCustomerID(int CustomerID)
        {
            var vmProductDetails = _productService.GetCreditSalesDetailByCustomerID(CustomerID, string.Empty);
            //var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
            //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

            if (vmProductDetails != null)
            {
                JsonResult jsonResult = Json(vmProductDetails, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region get credit customer remaining due
        [HttpGet]
        [Authorize]
        public JsonResult GetCreditSalesRemainingDueByCustomer(int hireSaslesId)
        {
            decimal remainingDue = _customerService.GetCreditCustomerRemaingDue(hireSaslesId);
            return Json(remainingDue, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Create view ready
        private ActionResult ReturnCreateViewWithTempData()
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("hsalesOrderViewModel");
            if (salesOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring salesOrder 
                TempData["hsalesOrderViewModel"] = salesOrder;
                return View("Create", salesOrder);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.MemoNo));
                return View(new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = new List<CreateSalesOrderDetailViewModel>(),
                    SalesOrder = new CreateSalesOrderViewModel { MemoNo = invNo }
                });
            }
        }
        #endregion

        [HttpGet]
        [Authorize]
        public JsonResult GetDamageProductDetailByIMEINo(string imeiNo, int CustomerID)
        {
            if (!string.IsNullOrEmpty(imeiNo))
            {
                var customProductDetails = _productService.GetCreditSalesDetailByCustomerID(CustomerID, imeiNo);
                var vmProduct = customProductDetails.FirstOrDefault();
                if (vmProduct != null)
                {
                    return Json(new
                    {
                        Code = vmProduct.ProductCode,
                        Name = vmProduct.ProductName,
                        Id = vmProduct.ProductId,
                        StockDetailId = vmProduct.StockDetailsId,
                        ColorId = vmProduct.ColorId,
                        ColorName = vmProduct.ColorName,
                        MrpRate = vmProduct.MRPRate,
                        IMEINo = vmProduct.IMENo,
                        OfferDescription = vmProduct.OfferDescription,
                        ProductType = vmProduct.ProductType,
                        Quantity = vmProduct.SalesQty,
                        HireSalesId = vmProduct.HireSalesId,
                        CreditSaleDetailsId = vmProduct.CreditSaleDetailsId
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int orderId)
        {
            TempData["IsInvoiceReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }


    }
}