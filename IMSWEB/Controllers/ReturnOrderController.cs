using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("returnorder")]
    public class ReturnOrderController : CoreController
    {
        IROrderService _returnOrderService;
        IROrderDetailService _returnOrderDetailService;
        IROProductDetailService _rOProductDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        IMiscellaneousService<ROrder> _miscellaneousService;
        IMapper _mapper;

        public ReturnOrderController(IErrorService errorService,
            IROrderService returnOrderService, IROrderDetailService returnOrderDetailService,
            IROProductDetailService rOProductDetailService, IStockService stockService,
            IStockDetailService stockDetailService, IMiscellaneousService<ROrder> miscellaneousService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _returnOrderService = returnOrderService;
            _returnOrderDetailService = returnOrderDetailService;
            _rOProductDetailService = rOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            TempData["returnOrderViewModel"] = null;
            var customPO = _returnOrderService.GetAllReturnOrderAsync();
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string>>,
                IEnumerable<GetReturnOrderViewModel>>(await customPO);
            return View(vmPO);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(ROrderViewModel newReturnOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleReturnOrder(newReturnOrder, formCollection);
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            ROrderViewModel returnOrder = (ROrderViewModel)TempData.Peek("returnOrderViewModel");
            if (returnOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["returnOrderViewModel"] = returnOrder;
                return View("Create", returnOrder);
            }
            else
            {
                string RInvoiceNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                return View(new ROrderViewModel
                {
                    RODetail = new CreateReturnOrderDetailViewModel(),
                    RODetails = new List<CreateReturnOrderDetailViewModel>(),
                    ReturnOrder = new CreateReturnOrderViewModel { InvoiceNo = RInvoiceNo }
                });
            }
        }

        private ActionResult HandleReturnOrder(ROrderViewModel newReturnOrder, FormCollection formCollection)
        {
            if (newReturnOrder != null)
            {
                TempData["ROProductDetails"] = null;
                ROrderViewModel returnOrder = (ROrderViewModel)TempData.Peek("returnOrderViewModel");
                returnOrder = returnOrder ?? new ROrderViewModel()
                {
                    ReturnOrder = newReturnOrder.ReturnOrder
                };
                returnOrder.RODetail = new CreateReturnOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newReturnOrder, formCollection);
                    returnOrder.RODetails = returnOrder.RODetails ?? new List<CreateReturnOrderDetailViewModel>();
                    GetPickersValue(returnOrder, formCollection);
                    AddTempROProductDetails(newReturnOrder, formCollection);

                    if (!ModelState.IsValid)
                    {
                        return View("Create", returnOrder);
                    }
                    else if (HasDuplicateIMEIORBarcode(formCollection))
                    {
                        AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                        return View("Create", returnOrder);
                    }
                    else if (returnOrder.RODetails.Any(x => x.ProductID.Equals(formCollection["ProductsId"]) &&
                        x.ColorId.Equals(formCollection["ColorsId"])))
                    {
                        AddToastMessage("", "This product has already been added in the order", ToastType.Error);
                        return View("Create", returnOrder);
                    }

                    AddToOrder(newReturnOrder, returnOrder, formCollection);
                    ModelState.Clear();
                    TempData["POProductDetails"] = null;
                    return View("Create", returnOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newReturnOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        returnOrder.RODetails = returnOrder.RODetails ?? new List<CreateReturnOrderDetailViewModel>();
                        return View("Create", returnOrder);
                    }
                    else if (returnOrder.RODetails == null || returnOrder.RODetails.Count <= 0)
                    {
                        AddToastMessage("", "No order data found to save.", ToastType.Error);
                        return RedirectToAction("Create");
                    }

                    SaveOrder(newReturnOrder, returnOrder, formCollection);
                    TempData["returnOrderViewModel"] = null;
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new ROrderViewModel
                    {
                        RODetail = new CreateReturnOrderDetailViewModel(),
                        RODetails = new List<CreateReturnOrderDetailViewModel>(),
                        ReturnOrder = new CreateReturnOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

       
        private void AddToOrder(ROrderViewModel newReturnOrder, ROrderViewModel returnOrder, FormCollection formCollection)
        {
            returnOrder.ReturnOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newReturnOrder.ReturnOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(newReturnOrder.RODetail.UTAmount))).ToString();

            returnOrder.ReturnOrder.PaidAmount = decimal.Parse(GetDefaultIfNull(newReturnOrder.ReturnOrder.PaidAmount)).ToString();
            returnOrder.ReturnOrder.ReturnDate = formCollection["ReturnDate"];
            returnOrder.ReturnOrder.CustomerID = formCollection["CustomersId"];

            returnOrder.RODetail.ROrderDetailID = newReturnOrder.RODetail.ROrderDetailID;
            returnOrder.RODetail.ProductID = formCollection["ProductsId"];
            returnOrder.RODetail.ProductCode = formCollection["ProductsCode"];
            returnOrder.RODetail.ProductName = formCollection["ProductsName"];
            returnOrder.RODetail.ColorId = formCollection["ColorsId"];
            returnOrder.RODetail.ColorName = formCollection["ColorsName"];
            returnOrder.RODetail.Quantity = newReturnOrder.RODetail.Quantity;
            returnOrder.RODetail.UnitPrice = newReturnOrder.RODetail.UnitPrice;
            returnOrder.RODetail.UTAmount = newReturnOrder.RODetail.UTAmount;


            returnOrder.RODetail.ROProductDetails = returnOrder.RODetail.ROProductDetails ?? new List<ROProductDetail>();

            for (int i = 1; i <= decimal.Parse(returnOrder.RODetail.Quantity); i++)
            {
                var roProductDetail = new ROProductDetail();
                roProductDetail.ProductID = int.Parse(GetDefaultIfNull(returnOrder.RODetail.ProductID));
                roProductDetail.ColorID = int.Parse(GetDefaultIfNull(returnOrder.RODetail.ColorId));
                roProductDetail.IMENO = formCollection["IMEINo" + i.ToString()];

                if (returnOrder.RODetail.ROProductDetails.Any(x => x.ProductID == roProductDetail.ProductID
                    && x.ColorID == roProductDetail.ColorID && x.IMENO.Equals(roProductDetail.IMENO))) continue;
                returnOrder.RODetail.ROProductDetails.Add(roProductDetail);
            }

            returnOrder.RODetails = returnOrder.RODetails ?? new List<CreateReturnOrderDetailViewModel>();
            returnOrder.RODetails.Add(returnOrder.RODetail);

            ROrderViewModel vm = new ROrderViewModel
            {
                RODetail = new CreateReturnOrderDetailViewModel(),
                RODetails = returnOrder.RODetails,
                ReturnOrder = returnOrder.ReturnOrder
            };

            TempData["returnOrderViewModel"] = vm;
            returnOrder.RODetail = new CreateReturnOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private bool HasDuplicateIMEIORBarcode(FormCollection formCollection)
        {
            string[] IMEIS = formCollection.AllKeys
                             .Where(key => key.StartsWith("IMEINo"))
                             .Select(key => formCollection[key])
                             .ToArray();
            return IMEIS.Length != IMEIS.Distinct().Count();
        }

        private void CheckAndAddModelErrorForAdd(ROrderViewModel newReturnOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["ReturnDate"]))
                ModelState.AddModelError("ReturnOrder.ReturnDate", "Return Date is required");

            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("PurchaseOrder.SupplierId", "Supplier is required");

            if (string.IsNullOrEmpty(formCollection["ProductsId"]))
                ModelState.AddModelError("PODetail.ProductId", "Product is required");

            //if (string.IsNullOrEmpty(formCollection["ColorsId"]))
            //    ModelState.AddModelError("PODetail.ColorId", "Color is required");

            if (string.IsNullOrEmpty(newReturnOrder.ReturnOrder.InvoiceNo))
                ModelState.AddModelError("PurchaseOrder.InvoiceNo", "Invoice No. is required");

            if (string.IsNullOrEmpty(newReturnOrder.RODetail.Quantity))
                ModelState.AddModelError("RODetail.Quantity", "Quantity is required");


            if (string.IsNullOrEmpty(newReturnOrder.RODetail.UnitPrice))
                ModelState.AddModelError("PODetail.UnitPrice", "Purchase Rate is required");
        }

        private void GetPickersValue(ROrderViewModel returnOrder, FormCollection formCollection)
        {
            returnOrder.ReturnOrder.CustomerID = formCollection["CustomersId"];
            returnOrder.RODetail.ProductID = formCollection["ProductsId"];
            returnOrder.RODetail.ProductCode = formCollection["ProductsCode"];
            returnOrder.RODetail.ProductName = formCollection["ProductsName"];
            returnOrder.RODetail.ColorId = formCollection["ColorsId"];
        }

        private void AddTempROProductDetails(ROrderViewModel returnOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(returnOrder.RODetail.Quantity)) return;

            List<ROProductDetail> tempROPDetails = new List<ROProductDetail>();

            for (int i = 1; i <= decimal.Parse(returnOrder.RODetail.Quantity); i++)
            {
                var roProductDetail = new ROProductDetail();
                roProductDetail.ProductID = int.Parse(GetDefaultIfNull(returnOrder.RODetail.ProductID));
                roProductDetail.ColorID = int.Parse(GetDefaultIfNull(returnOrder.RODetail.ColorId));
                roProductDetail.IMENO = formCollection["IMEINo" + i.ToString()];

                tempROPDetails.Add(roProductDetail);
            }

            if (tempROPDetails.Count > 0)
                TempData["ROProductDetails"] = tempROPDetails;
        }

        private void CheckAndAddModelErrorForSave(ROrderViewModel newReturnOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newReturnOrder.ReturnOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newReturnOrder.ReturnOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("PurchaseOrder.GrandTotal", "Grand Total is required");

            //if (string.IsNullOrEmpty(newReturnOrder.ReturnOrder.TotalAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newReturnOrder.ReturnOrder.TotalAmount)) <= 0)
            //    ModelState.AddModelError("PurchaseOrder.TotalAmount", "Net Total is required");


        }

        private void SaveOrder(ROrderViewModel newReturnOrder, ROrderViewModel returnOrder, FormCollection formCollection)
        {

            returnOrder.ReturnOrder.PaidAmount = decimal.Parse(GetDefaultIfNull(newReturnOrder.ReturnOrder.PaidAmount)).ToString();
            //returnOrder.ReturnOrder.LabourCost = newReturnOrder.ReturnOrder.LabourCost;

            returnOrder.ReturnOrder.ReturnDate = formCollection["ReturnDate"];
            returnOrder.ReturnOrder.CustomerID = formCollection["CustomersId"];

            //removing unchanged previous order
            returnOrder.RODetails.Where(x => !string.IsNullOrEmpty(x.ROrderDetailID)).ToList()
                .ForEach(x => returnOrder.RODetails.Remove(x));

            DataTable dtReturnOrder = CreatePurchaseOrderDataTable(returnOrder);
            DataTable dtStock = CreateStockDataTable(returnOrder);
            DataSet dsReturnOrderDetail = CreateRODetailDataTable(returnOrder, dtStock);
            DataTable dtReturnOrderDetail = dsReturnOrderDetail.Tables[0];
            DataTable dtROProductDetail = dsReturnOrderDetail.Tables[1];
            DataTable dtStockDetail = dsReturnOrderDetail.Tables[2];

            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                _returnOrderService.UpdateReturnOrderUsingSP(int.Parse(returnOrder.ReturnOrder.ROrderID),
                    dtReturnOrder, dtReturnOrderDetail, dtROProductDetail, dtStock, dtStockDetail);
            else
                _returnOrderService.AddReturnOrderUsingSP(dtReturnOrder, dtReturnOrderDetail, dtROProductDetail,
                    dtStock, dtStockDetail);

            AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
        }

        private DataTable CreatePurchaseOrderDataTable(ROrderViewModel returnOrder)
        {
            DataTable dtReturnOrder = new DataTable();
            dtReturnOrder.Columns.Add("ReturnDate", typeof(DateTime));
            dtReturnOrder.Columns.Add("InvoiceNo", typeof(string));
            dtReturnOrder.Columns.Add("CustomerID", typeof(int));
            dtReturnOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtReturnOrder.Columns.Add("PaidAmount", typeof(decimal));
            dtReturnOrder.Columns.Add("ConcernId", typeof(int));
            dtReturnOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtReturnOrder.Columns.Add("CreatedBy", typeof(int));

            DataRow row = null;

            row = dtReturnOrder.NewRow();

            row["ReturnDate"] = returnOrder.ReturnOrder.ReturnDate;
            row["InvoiceNo"] = returnOrder.ReturnOrder.InvoiceNo;
            row["CustomerID"] = returnOrder.ReturnOrder.CustomerID;
            row["GrandTotal"] = returnOrder.ReturnOrder.GrandTotal;
            row["PaidAmount"] = GetDefaultIfNull(returnOrder.ReturnOrder.PaidAmount);
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            dtReturnOrder.Rows.Add(row);

            return dtReturnOrder;
        }

        private DataTable CreateStockDataTable(ROrderViewModel returnOrder)
        {
            DataTable dtStock = new DataTable();
            dtStock.Columns.Add("StockId", typeof(int));
            dtStock.Columns.Add("StockCode", typeof(string));
            dtStock.Columns.Add("ColorID", typeof(int));
            dtStock.Columns.Add("EntryDate", typeof(DateTime));
            dtStock.Columns.Add("Quantity", typeof(decimal));
            dtStock.Columns.Add("ProductID", typeof(int));
            dtStock.Columns.Add("MRPPrice", typeof(decimal));
            dtStock.Columns.Add("LPPrice", typeof(decimal));
            dtStock.Columns.Add("ConcernID", typeof(int));
            dtStock.Columns.Add("CreateDate", typeof(DateTime));
            dtStock.Columns.Add("CreatedBy", typeof(int));
            DataRow row = null;

            var stocks = _stockService.GetAllStock();

            foreach (var item in returnOrder.RODetails)
            {
                var stock = stocks.FirstOrDefault(x => x.ProductID == int.Parse(item.ProductID) &&
                        x.ColorID == int.Parse(item.ColorId));
                row = dtStock.NewRow();

                if (stock != null)
                    row["StockId"] = stock.StockID;
                else
                    row["StockId"] = DBNull.Value;
                row["StockCode"] = item.ProductCode;
                row["ColorID"] = item.ColorId;
                row["EntryDate"] = DateTime.Now;
                row["Quantity"] = item.Quantity;
                row["ProductID"] = item.ProductID;
                row["MRPPrice"] = stock.MRPPrice; //Sales Rate
                row["LPPrice"] = stock.LPPrice; //Purchase Rate
                row["ConcernID"] = User.Identity.GetConcernId();
                row["CreateDate"] = DateTime.Now;
                row["CreatedBy"] = User.Identity.GetUserId<int>();
                dtStock.Rows.Add(row);
            }

            return dtStock;
        }

        private DataSet CreateRODetailDataTable(ROrderViewModel returnOrder, DataTable dtStock)
        {
            DataSet dsReturnOrderDetail = new DataSet();
            DataTable dtReturnOrderDetail = new DataTable();
            DataTable dtROProductDetail = new DataTable();
            DataTable dtStockDetail = new DataTable();
            DataRow row = null;
            int id;

            dtReturnOrderDetail.Columns.Add("ROrderDetailID", typeof(int));
            dtReturnOrderDetail.Columns.Add("ProductID", typeof(int));
            dtReturnOrderDetail.Columns.Add("ColorID", typeof(int));
            //dtReturnOrderDetail.Columns.Add("Status", typeof(int));
            dtReturnOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtReturnOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtReturnOrderDetail.Columns.Add("UTAmount", typeof(decimal));
            //dtReturnOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            //dtReturnOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            //dtReturnOrderDetail.Columns.Add("MrpRate", typeof(decimal));


            //ROProductDetail
            dtROProductDetail.Columns.Add("ProductID", typeof(int));
            dtROProductDetail.Columns.Add("ColorId", typeof(int));
            dtROProductDetail.Columns.Add("IMENO", typeof(string));

            //StockDetail
            dtStockDetail.Columns.Add("StockCode", typeof(string));
            dtStockDetail.Columns.Add("ProductID", typeof(int));
            dtStockDetail.Columns.Add("ColorId", typeof(int));
            dtStockDetail.Columns.Add("IMENO", typeof(string));
            dtStockDetail.Columns.Add("Status", typeof(int));


            foreach (var item in returnOrder.RODetails)
            {
                row = dtReturnOrderDetail.NewRow();
                id = int.Parse(GetDefaultIfNull(item.ROrderDetailID));

                if (id > 0)
                    row["POrderDetailId"] = id;
                else
                    row["POrderDetailId"] = DBNull.Value;

                row["ProductId"] = item.ProductID;
                row["ColorId"] = item.ColorId;
                //row["Status"] = (int)item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["UTAmount"] = item.UTAmount;
                //row["PPDisPer"] = GetDefaultIfNull(item.PPDisPercentage);
                //row["PPDisAmt"] = GetDefaultIfNull(item.PPDiscountAmount);
                //row["MrpRate"] = item.MRPRate;


                //ROProductDetail
                CreateROProductDetailDataTable(item, dtROProductDetail);

                //StockDetail
                CreateStockDetailDataTable(dtStock, item, dtStockDetail);

                dtReturnOrderDetail.Rows.Add(row);
            }

            dsReturnOrderDetail.Tables.Add(dtReturnOrderDetail);
            dsReturnOrderDetail.Tables.Add(dtROProductDetail);
            dsReturnOrderDetail.Tables.Add(dtStockDetail);

            return dsReturnOrderDetail;
        }

        private void CreateROProductDetailDataTable(CreateReturnOrderDetailViewModel roDetail, DataTable dtROProductDetail)
        {
            DataRow row = null;
            foreach (var item in roDetail.ROProductDetails)
            {
                row = dtROProductDetail.NewRow();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = roDetail.ColorId;
                row["IMENO"] = item.IMENO;
                dtROProductDetail.Rows.Add(row);
            }
        }

        private void CreateStockDetailDataTable(DataTable dtStock, CreateReturnOrderDetailViewModel roDetail,
    DataTable dtStockDetail)
        {
            DataRow row = null;
            foreach (var item in roDetail.ROProductDetails)
            {
                row = dtStockDetail.NewRow();
                row["StockCode"] = dtStock.AsEnumerable().Where(x => x.Field<int>("ProductID") == item.ProductID).
                    Select(x => x.Field<string>("StockCode")).First();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = roDetail.ColorId;
                row["IMENO"] = item.IMENO;
                row["Status"] = (int)EnumStockStatus.Stock;

                dtStockDetail.Rows.Add(row);
            }
        }

	}
}