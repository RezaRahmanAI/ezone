using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
namespace IMSWEB.Controllers
{
    public class DamagePOReturnController : CoreController
    {
        IPurchaseOrderService _purchaseOrderService;
        IPurchaseOrderDetailService _purchaseOrderDetailService;
        IPOProductDetailService _pOProductDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        IMiscellaneousService<POrder> _miscellaneousService;
        IMapper _mapper;
        ISupplierService _supplierService;
        IColorService _colorService;
        IProductService _productService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DamagePOReturnController(IErrorService errorService,
            IPurchaseOrderService purchaseOrderService, IPurchaseOrderDetailService purchaseOrderDetailService,
            IPOProductDetailService pOProductDetailService, IStockService stockService, ISupplierService supplierService,
            IStockDetailService stockDetailService, IMiscellaneousService<POrder> miscellaneousService, IMapper mapper,
            IColorService colorService, IProductService productService, ISystemInformationService sysInfoService
            )
            : base(errorService, sysInfoService)
        {
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _pOProductDetailService = pOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _supplierService = supplierService;
            _colorService = colorService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            TempData["purchaseOrderViewModel"] = null;
            //var customPO = _purchaseOrderService.GetAllReturnPurchaseOrderAsync();
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            //var customPO = _purchaseOrderService.GetAllPurchaseOrderAsync(DateRange.Item1, DateRange.Item2, IsVATManager(), User.Identity.GetConcernId());
            //var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType, Tuple<int>>>,
            //    IEnumerable<GetPurchaseOrderViewModel>>(await customPO);

            //TempData["purchaseOrderViewModel"] = null;
            var customPO = _purchaseOrderService.GetAllDamageReturnPurchaseOrderAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);

            return View(vmPO);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            //ViewBag.IsEditReqPermission = _sysInfoService.IsEditReqPermission();
            //ViewBag.IsEditReqPermissionFalse = _sysInfoService.IsEditReqPermissionFalse();
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var customPO = _purchaseOrderService.GetAllDamageReturnPurchaseOrderAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View("Index", vmPO);
        }


        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }


        private ActionResult ReturnCreateViewWithTempData()
        {
            PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                return View("Create", purchaseOrder);
            }
            else
            {
                string chllnNo = "DRC-" + _miscellaneousService.GetUniqueKey(x => x.POrderID);
                var color = _colorService.GetAllColor().FirstOrDefault(i => i.ConcernID == User.Identity.GetConcernId());
                return View(new PurchaseReturnOrderViewModel
                {
                    POProductDetails = new CreatePOProductDetailViewModel(),
                    PurchaseOrder = new CreatePurchaseOrderViewModel { ChallanNo = chllnNo }
                });
            }
        }

        [HttpPost]
        public ActionResult Create(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            return HandlePurchaseOrder(newPurchaseOrder, formCollection);
        }
        private void CheckAndAddModelErrorForAdd(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {


            if (string.IsNullOrEmpty(formCollection["ProductId"]))
                ModelState.AddModelError("POProductDetails.ProductId", "Product is required");

            if (string.IsNullOrEmpty(formCollection["StockDetailsId"]))
                ModelState.AddModelError("POProductDetails.SDetailID", "IMEI is required");

            //if (string.IsNullOrEmpty(formCollection["ColorsId"]))
            //ModelState.AddModelError("PODetail.ColorId", "Color is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.ChallanNo))
                ModelState.AddModelError("PurchaseOrder.ChallanNo", "Challan No. is required");

            if ((newPurchaseOrder.POProductDetails.Quantity) > (newPurchaseOrder.POProductDetails.PreviousStock))
            {
                ModelState.AddModelError("POProductDetails.Quantity", "Quantity Exceeds Purchase Quantity");

            }


        }
        private void GetPickersValue(PurchaseReturnOrderViewModel purchaseOrder, PurchaseReturnOrderViewModel NewpurchaseOrder, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                purchaseOrder.PurchaseOrder.SupplierId = formCollection["SuppliersId"];
            if (!string.IsNullOrEmpty(formCollection["OrderDate"]))
                purchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];

            if (!string.IsNullOrEmpty(formCollection["StockDetailsId"]))
            {
                NewpurchaseOrder.POProductDetails.SDetailID = Convert.ToInt32(formCollection["StockDetailsId"]);
                purchaseOrder.POProductDetails.SDetailID = NewpurchaseOrder.POProductDetails.SDetailID;
                var StockDetail = _stockDetailService.GetById(purchaseOrder.POProductDetails.SDetailID);
                NewpurchaseOrder.POProductDetails.GodownID = StockDetail.GodownID;
                purchaseOrder.POProductDetails.GodownID = StockDetail.GodownID;
                NewpurchaseOrder.POProductDetails.ProductType = Convert.ToInt32(formCollection["ProductType"]);
            }

            if (!string.IsNullOrEmpty(formCollection["ProductId"]))
            {
                NewpurchaseOrder.POProductDetails.ProductId = formCollection["ProductId"];
                purchaseOrder.POProductDetails.ProductId = NewpurchaseOrder.POProductDetails.ProductId;
            }
            if (!string.IsNullOrEmpty(formCollection["ProductDetailsName"]))
            {
                NewpurchaseOrder.POProductDetails.ProductName = formCollection["ProductDetailsName"];
                purchaseOrder.POProductDetails.ProductName = NewpurchaseOrder.POProductDetails.ProductName;
            }
        }

        /// <summary>
        /// This method check duplicate IMEI in recently added product.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        private bool HasDuplicateIMEIInRecentAddToOrder(FormCollection formCollection, PurchaseReturnOrderViewModel NewpurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder)
        {
            int ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));
            if (ProductType != (int)EnumProductType.NoBarcode)
            {
                if (purchaseOrder.POProductDetailList.Count() > 0)
                {
                    if (purchaseOrder.POProductDetailList.Any(i => i.SDetailID == NewpurchaseOrder.POProductDetails.SDetailID))
                        return true;
                }
            }

            return false;
        }
        private ActionResult HandlePurchaseOrder(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            if (newPurchaseOrder != null)
            {
                PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
                purchaseOrder = purchaseOrder ?? new PurchaseReturnOrderViewModel()
                {
                    PurchaseOrder = newPurchaseOrder.PurchaseOrder
                };
                purchaseOrder.POProductDetails = new CreatePOProductDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newPurchaseOrder, formCollection);
                    purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();
                    GetPickersValue(purchaseOrder, newPurchaseOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        TempData["POProductDetails"] = purchaseOrder.POProductDetailList;
                        return View("Create", purchaseOrder);
                    }
                    else if (HasDuplicateIMEIInRecentAddToOrder(formCollection, newPurchaseOrder, purchaseOrder))
                    {
                        AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                        return View("Create", purchaseOrder);
                    }
                    AddToOrder(newPurchaseOrder, purchaseOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", purchaseOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newPurchaseOrder, formCollection);

                    //if (!IsDateValid(Convert.ToDateTime(newPurchaseOrder.PurchaseOrder.OrderDate)))
                    //{
                    //    ModelState.AddModelError("PurchaseOrder.OrderDate", "Back dated entry is not valid.");
                    //}
                    if (!ModelState.IsValid)
                    {
                        purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();
                        return View("Create", purchaseOrder);
                    }
                    else if (purchaseOrder.POProductDetailList == null || purchaseOrder.POProductDetailList.Count <= 0)
                    {
                        AddToastMessage("", "No order data found to save.", ToastType.Error);
                        return View("Create", purchaseOrder);
                    }
                    SaveOrder(newPurchaseOrder, purchaseOrder, formCollection);
                    TempData["purchaseOrderViewModel"] = null;
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new PurchaseOrderViewModel
                    {
                        PODetail = new CreatePurchaseOrderDetailViewModel(),
                        PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                        PurchaseOrder = new CreatePurchaseOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        List<CreatePurchaseOrderDetailViewModel> CreateFIFODetails(PurchaseReturnOrderViewModel purchaseOrder)
        {
            List<CreatePurchaseOrderDetailViewModel> POrderDetails = new List<CreatePurchaseOrderDetailViewModel>();
            CreatePurchaseOrderDetailViewModel POrderDetail = null;
            decimal ReturnQty = 0m;
            var BarcodeProducts = purchaseOrder.POProductDetailList.Where(i => i.ProductType == (int)EnumProductType.ExistingBC || i.ProductType == (int)EnumProductType.AutoBC).ToList();
            var NoBarcodeProducts = purchaseOrder.POProductDetailList.Where(i => i.ProductType == (int)EnumProductType.NoBarcode).ToList();
            StockDetail oStockDetail = null;
            foreach (var item in BarcodeProducts)
            {
                oStockDetail = _stockDetailService.GetById(item.SDetailID);

                POrderDetail = new CreatePurchaseOrderDetailViewModel();
                POrderDetail.ProductId = item.ProductId.ToString();
                POrderDetail.ColorId = item.ColorsId.ToString();
                POrderDetail.GodownID = oStockDetail.GodownID.ToString();
                POrderDetail.RQuantity = 1;
                POrderDetail.PRate = oStockDetail.PRate;
                POrderDetail.TAmount = (POrderDetail.RQuantity * oStockDetail.PRate).ToString();
                POrderDetail.PODetailId = oStockDetail.SDetailID.ToString();
                POrderDetail.ProductCode = oStockDetail.IMENO;
                POrderDetails.Add(POrderDetail);
            }

            foreach (var item in NoBarcodeProducts)
            {
                ReturnQty = item.Quantity;
                var stockdetails = _stockService.GetSupplierDamageStockDetails(Convert.ToInt32(purchaseOrder.PurchaseOrder.SupplierId), Convert.ToInt32(item.ProductId), Convert.ToInt32(item.ColorsId), item.GodownID).OrderBy(i => i.StockDetailsId);
                foreach (var sitem in stockdetails)
                {
                    POrderDetail = new CreatePurchaseOrderDetailViewModel();
                    POrderDetail.ProductId = item.ProductId.ToString();
                    POrderDetail.ColorId = item.ColorsId.ToString();
                    POrderDetail.GodownID = sitem.GodownID.ToString();
                    if (sitem.PreStock >= ReturnQty)
                    {
                        POrderDetail.RQuantity = ReturnQty;
                        POrderDetail.PRate = sitem.MRPRate;
                        POrderDetail.TAmount = (ReturnQty * sitem.MRPRate).ToString();
                        ReturnQty = 0m;
                        POrderDetail.PODetailId = sitem.StockDetailsId.ToString();
                        POrderDetail.ProductCode = sitem.IMENo;
                        POrderDetails.Add(POrderDetail);
                        break;
                    }
                    else if (sitem.PreStock < ReturnQty)
                    {
                        POrderDetail.RQuantity = sitem.PreStock;
                        POrderDetail.PRate = sitem.MRPRate;
                        POrderDetail.TAmount = (sitem.PreStock * sitem.MRPRate).ToString();
                        ReturnQty = (ReturnQty - sitem.PreStock);
                        POrderDetail.PODetailId = sitem.StockDetailsId.ToString();
                        POrderDetail.ProductCode = sitem.IMENo;
                        POrderDetails.Add(POrderDetail);
                    }
                }
            }

            return POrderDetails;
        }
        private void SaveOrder(PurchaseReturnOrderViewModel newPurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            var POrderDetails = CreateFIFODetails(purchaseOrder);
            #region POrder
            purchaseOrder.PurchaseOrder.Status = ((int)EnumPurchaseType.DamageReturn).ToString();
            purchaseOrder.PurchaseOrder.TotalAmount = POrderDetails.Sum(i => Convert.ToDecimal(i.TAmount)).ToString();
            purchaseOrder.PurchaseOrder.GrandTotal = purchaseOrder.PurchaseOrder.TotalAmount;
            purchaseOrder.PurchaseOrder.RecieveAmount = newPurchaseOrder.PurchaseOrder.RecieveAmount;
            purchaseOrder.PurchaseOrder.PaymentDue = newPurchaseOrder.PurchaseOrder.PaymentDue;
            purchaseOrder.PurchaseOrder.Remarks = newPurchaseOrder.PurchaseOrder.Remarks;

            AddAuditTrail(purchaseOrder.PurchaseOrder, true);
            #endregion
            PurchaseOrderViewModel POrder = new PurchaseOrderViewModel()
            {
                PurchaseOrder = new CreatePurchaseOrderViewModel(),
                PODetail = new CreatePurchaseOrderDetailViewModel(),
                PODetails = new List<CreatePurchaseOrderDetailViewModel>()
            };

            POrder.PurchaseOrder = purchaseOrder.PurchaseOrder;

            POProductDetail oPOProductDetail = null;

            foreach (var item in POrderDetails)
            {

                oPOProductDetail = new POProductDetail();
                oPOProductDetail.ProductID = Convert.ToInt32(item.ProductId);
                oPOProductDetail.ColorID = Convert.ToInt32(item.ColorId);
                oPOProductDetail.IMENO = item.ProductCode;
                oPOProductDetail.ReturnQty = 1; //(decimal)item.RQuantity;
                oPOProductDetail.ReturnSDetailID = Convert.ToInt32(item.PODetailId);
                item.POProductDetails.Add(oPOProductDetail);
            }

            var Details = (from pd in POrderDetails
                           group pd by new { pd.ProductId, pd.ColorId, pd.GodownID } into g
                           select new CreatePurchaseOrderDetailViewModel
                           {
                               ProductId = g.Key.ProductId,
                               ColorId = g.Key.ColorId,
                               GodownID = g.Key.GodownID,
                               RQuantity = g.Sum(i => i.RQuantity),
                               TAmount = g.Sum(i => (i.PRate * i.RQuantity)).ToString()
                           }).ToList();

            List<POProductDetail> POPD = new List<POProductDetail>();

            foreach (var item in Details)
            {
                item.PRate = Math.Round(Convert.ToDecimal(item.TAmount) / item.RQuantity, 3);
                item.UnitPrice = item.PRate.ToString();
                item.MRPRate = item.PRate.ToString();
                var ddd = POrderDetails.Where(i => i.ProductId == item.ProductId & i.ColorId == item.ColorId & i.GodownID == item.GodownID);
                foreach (var sitem in ddd)
                {
                    oPOProductDetail = new POProductDetail();
                    oPOProductDetail.ProductID = Convert.ToInt32(item.ProductId);
                    oPOProductDetail.ColorID = Convert.ToInt32(item.ColorId);
                    oPOProductDetail.IMENO = sitem.ProductCode;
                    oPOProductDetail.ReturnQty = sitem.RQuantity;
                    oPOProductDetail.ReturnSDetailID = Convert.ToInt32(sitem.PODetailId);
                    item.POProductDetails.Add(oPOProductDetail);
                }
            }

            POrder.PODetails = Details;

            DataTable dtPurchaseOrder = CreatePurchaseOrderDataTable(POrder);
            DataSet dsPurchaseOrderDetail = CreatePODetailDataTable(POrder);
            DataTable dtPurchaseOrderDetail = dsPurchaseOrderDetail.Tables[0];
            DataTable dtPOProductDetail = dsPurchaseOrderDetail.Tables[1];
            var Result = _purchaseOrderService.AddDamageReturnPurchaseOrderUsingSP(dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail);

            if (Result.Item1)
            {
                AddToastMessage("", "Product Return Successfull.", ToastType.Success);
                TempData["POrderID"] = Result.Item2;
                TempData["IsChallanReadyByID"] = true;
            }
            else
                AddToastMessage("", "Product Return Failed.", ToastType.Error);

        }
        private DataTable CreatePurchaseOrderDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataTable dtPurchaseOrder = new DataTable();
            dtPurchaseOrder.Columns.Add("OrderDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("ChallanNo", typeof(string));
            dtPurchaseOrder.Columns.Add("SupplierId", typeof(int));
            dtPurchaseOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("RecAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Status", typeof(int));
            dtPurchaseOrder.Columns.Add("PPTDisAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("LabourCost", typeof(decimal));
            dtPurchaseOrder.Columns.Add("ConcernId", typeof(int));
            dtPurchaseOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("CreatedBy", typeof(int));
            dtPurchaseOrder.Columns.Add("IsDamageOrder", typeof(int));
            dtPurchaseOrder.Columns.Add("Remarks", typeof(string));

            dtPurchaseOrder.Columns.Add("InvoiceNo", typeof(string));
            dtPurchaseOrder.Columns.Add("TPQty", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Field1", typeof(string));
            dtPurchaseOrder.Columns.Add("Field2", typeof(string));
            dtPurchaseOrder.Columns.Add("Field3", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Field4", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Field5", typeof(int));
            dtPurchaseOrder.Columns.Add("Field6", typeof(int));
            dtPurchaseOrder.Columns.Add("VatPercentage", typeof(decimal));
            dtPurchaseOrder.Columns.Add("VatAmount", typeof(decimal));
            DataRow row = null;

            row = dtPurchaseOrder.NewRow();
            row["OrderDate"] = purchaseOrder.PurchaseOrder.OrderDate;
            row["ChallanNo"] = purchaseOrder.PurchaseOrder.ChallanNo;
            row["SupplierId"] = purchaseOrder.PurchaseOrder.SupplierId;
            row["GrandTotal"] = purchaseOrder.PurchaseOrder.GrandTotal;
            row["TDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDiscountAmount);
            row["TotalAmt"] = purchaseOrder.PurchaseOrder.TotalAmount;
            row["RecAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.RecieveAmount);
            row["PaymentDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue);
            row["TotalDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue); ; //(decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.AdjAmount);
            row["Status"] = purchaseOrder.PurchaseOrder.Status;
            row["PPTDisAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PPDiscountAmount);
            row["NetDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount);
            row["LabourCost"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.LabourCost);
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["IsDamageOrder"] = purchaseOrder.PurchaseOrder.IsDamagePO ? 1 : 0;
            row["Remarks"] = purchaseOrder.PurchaseOrder.Remarks;

            row["InvoiceNo"] = purchaseOrder.PurchaseOrder.InvoiceNo;
            row["Field1"] = string.Empty;
            row["Field2"] = string.Empty;
            row["TPQty"] = 0m;
            row["Field3"] = 0m;
            row["Field4"] = 0m;
            row["Field5"] = 0;
            row["Field6"] = 0;
            row["VatPercentage"] = 0m;
            row["VatAmount"] = 0m;

            dtPurchaseOrder.Rows.Add(row);

            return dtPurchaseOrder;
        }

        private DataSet CreatePODetailDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataSet dsPurchaseOrderDetail = new DataSet();
            DataTable dtPurchaseOrderDetail = new DataTable();
            DataTable dtPOProductDetail = new DataTable();
            DataTable dtStockDetail = new DataTable();
            DataRow row = null;
            int id;

            dtPurchaseOrderDetail.Columns.Add("POrderDetailId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ProductId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ColorId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Status", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("SalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CreditSalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate12Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate3Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("GodownID", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("DOrderDetailId", typeof(int));

            //POProductDetail
            dtPOProductDetail.Columns.Add("ProductID", typeof(int));
            dtPOProductDetail.Columns.Add("ColorId", typeof(int));
            dtPOProductDetail.Columns.Add("IMENO", typeof(string));
            dtPOProductDetail.Columns.Add("DamagePOPDID", typeof(int));
            dtPOProductDetail.Columns.Add("ReturnSDetailID", typeof(int));
            dtPOProductDetail.Columns.Add("ReturnQty", typeof(int));



            foreach (var item in purchaseOrder.PODetails)
            {
                row = dtPurchaseOrderDetail.NewRow();
                id = int.Parse(GetDefaultIfNull(item.PODetailId));

                if (id > 0)
                    row["POrderDetailId"] = id;
                else
                    row["POrderDetailId"] = DBNull.Value;

                row["ProductId"] = item.ProductId;
                row["ColorId"] = item.ColorId;
                row["Status"] = (int)item.Status;
                row["Quantity"] = item.RQuantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = item.TAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDisPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDiscountAmount);
                row["MrpRate"] = GetDefaultIfNull(item.MRPRate);
                row["SalesRate"] = GetDefaultIfNull(item.SalesRate);
                row["ExtraPPDISPer"] = GetDefaultIfNull(item.ExtraPPDISPer);
                row["ExtraPPDISAmt"] = GetDefaultIfNull(item.ExtraPPDISAmt);
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["CreditSalesRate"] = GetDefaultIfNull(item.CreditSalesRate);
                row["CRSalesRate12Month"] = GetDefaultIfNull(item.CRSalesRate12Month);
                row["CRSalesRate3Month"] = GetDefaultIfNull(item.CRSalesRate3Month);
                row["GodownID"] = GetDefaultIfNull(item.GodownID);
                row["DOrderDetailId"] = "0";

                //POProductDetail
                CreatePOProductDetailDataTable(item, dtPOProductDetail);



                dtPurchaseOrderDetail.Rows.Add(row);
            }

            dsPurchaseOrderDetail.Tables.Add(dtPurchaseOrderDetail);
            dsPurchaseOrderDetail.Tables.Add(dtPOProductDetail);
            dsPurchaseOrderDetail.Tables.Add(dtStockDetail);

            return dsPurchaseOrderDetail;
        }
        private void CreatePOProductDetailDataTable(CreatePurchaseOrderDetailViewModel poDetail, DataTable dtPOProductDetail)
        {
            DataRow row = null;
            foreach (var item in poDetail.POProductDetails)
            {
                row = dtPOProductDetail.NewRow();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = poDetail.ColorId;
                row["IMENO"] = item.IMENO.Trim();
                row["DamagePOPDID"] = item.DamagePOPDID ?? 0;
                row["ReturnSDetailID"] = item.ReturnSDetailID;
                row["ReturnQty"] = item.ReturnQty;
                dtPOProductDetail.Rows.Add(row);
            }
        }

        private void CheckAndAddModelErrorForSave(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("PurchaseOrder.OrderDate", "Purchase Date is required");

            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("PurchaseOrder.SupplierId", "Supplier is required");

            if (_miscellaneousService
                .GetDuplicateEntry(i => i.ChallanNo.Equals(newPurchaseOrder.PurchaseOrder.ChallanNo)) != null)
            {
                newPurchaseOrder.PurchaseOrder.ChallanNo = "RC-" + _miscellaneousService.GetUniqueKey(x => x.POrderID);
            }

            if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
                ModelState.AddModelError("PurchaseOrder.OrderDate", "Back dated entry is not valid");
        }

        private void AddToOrder(PurchaseReturnOrderViewModel newPurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();
            var oStockDetail = _stockDetailService.GetById(newPurchaseOrder.POProductDetails.SDetailID);

            #region Details
            newPurchaseOrder.POProductDetails.PRate = oStockDetail.PRate;
            newPurchaseOrder.POProductDetails.ColorsId = oStockDetail.ColorID.ToString();
            purchaseOrder.POProductDetailList.Add(newPurchaseOrder.POProductDetails);
            #endregion

            #region Parent
            purchaseOrder.PurchaseOrder.TotalAmount = (Convert.ToDecimal(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalAmount)) + oStockDetail.PRate * newPurchaseOrder.POProductDetails.Quantity).ToString();
            purchaseOrder.PurchaseOrder.PaymentDue = (Convert.ToDecimal(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalAmount)) - Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.RecieveAmount)).ToString();
            purchaseOrder.PurchaseOrder.RecieveAmount = newPurchaseOrder.PurchaseOrder.RecieveAmount;
            #endregion

            PurchaseReturnOrderViewModel vm = new PurchaseReturnOrderViewModel
            {
                POProductDetails = new CreatePOProductDetailViewModel(),
                POProductDetailList = purchaseOrder.POProductDetailList,
                PurchaseOrder = purchaseOrder.PurchaseOrder
            };
            TempData["purchaseOrderViewModel"] = vm;

            purchaseOrder.POProductDetails = new CreatePOProductDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        public ActionResult DeleteFromView(int SDetailID)
        {
            PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                var DeletePOPDetail = purchaseOrder.POProductDetailList.FirstOrDefault(i => i.SDetailID == SDetailID);
                purchaseOrder.POProductDetailList.Remove(DeletePOPDetail);
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                return RedirectToAction("Create");
            }
            else
            {
                string chllnNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ChallanNo));
                var color = _colorService.GetAllColor().FirstOrDefault(i => i.ConcernID == User.Identity.GetConcernId());
                return View(new PurchaseReturnOrderViewModel
                {
                    POProductDetails = new CreatePOProductDetailViewModel(),
                    PurchaseOrder = new CreatePurchaseOrderViewModel { ChallanNo = chllnNo }
                });
            }
        }

        [Authorize]
        public ActionResult GetInvoiceByID(int orderId)
        {
            TempData["POrderID"] = orderId;
            TempData["IsChallanReadyByID"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyPurchaseReturnReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult MonthlyPurchaseReturnReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult YearlyPurchaseReturnReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePurchaseReturnReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult ModelWiseReport()
        {
            return View();
        }

        public JsonResult GetStockProductsBySupplier(int SuppplierID)
        {
            if (SuppplierID != 0)
            {
                var StockProducts = _stockService.GetDamageStockProductsBySupplier(SuppplierID);
                if (StockProducts != null)
                    return Json(StockProducts, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {
            if (!string.IsNullOrEmpty(imeiNo))
            {
                var vmProduct = _stockService.GetDamageStockIMEIDetail(imeiNo);
                if (vmProduct != null)
                {
                    var data = new
                    {
                        Code = vmProduct.ProductCode,
                        Name = vmProduct.ProductName,
                        Id = vmProduct.ProductId,
                        StockDetailId = vmProduct.StockDetailsId,
                        ColorId = vmProduct.ColorId,
                        ColorName = vmProduct.ColorName,
                        UnitPrice = vmProduct.MRPRate,
                        IMEINo = vmProduct.IMENo,
                        OfferDescription = vmProduct.OfferDescription,
                        PRate = vmProduct.PRate,
                        UTAmount = vmProduct.MRPRate,
                        Quantity = 1,
                        GodownID = vmProduct.GodownID,
                        MrpRate = vmProduct.MRPRate,
                        PPDPercentage = 0.00,
                        PPDAmount = 0.00,
                        ProductType = vmProduct.ProductType,
                        Service = ""
                    };
                    return Json(new { data = data, status = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { msg = "IMEI not available in stock.", status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Please enter IMEI.", status = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
