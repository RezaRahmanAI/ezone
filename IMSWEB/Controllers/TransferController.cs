using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class TransferController : CoreController
    {
        IMapper _mapper;
        IMiscellaneousService<Transfer> _miscellaneousService;
        ITransferService _transferService;
        ISisterConcernService _SisterConcernService;
        IProductService _productService;
        IStockDetailService _stockDetailService;
        IStockService _StockService;
        IColorService _ColorService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IPurchaseOrderService _PurchaseOrderService;
        ISisterConcernService _SisterConcern;
        ISystemInformationService _sysInfoService;


        public TransferController(IErrorService errorService, IMiscellaneousService<Transfer> miscellaneousService,
            IMapper mapper, ITransferService TransferService, ISisterConcernService SisterConcernService, IProductService productService, ISisterConcernService SisterConcern, ISystemInformationService sysInfoService
            , IStockDetailService stockDetailService, IStockService StockService, IColorService ColorService, IPurchaseOrderService PurchaseOrderService)
            : base(errorService, sysInfoService)
        {
            _transferService = TransferService;
            _mapper = mapper;
            _miscellaneousService = miscellaneousService;
            _SisterConcernService = SisterConcernService;
            _productService = productService;
            _stockDetailService = stockDetailService;
            _StockService = StockService;
            _ColorService = ColorService;
            _PurchaseOrderService = PurchaseOrderService;
            _SisterConcern = SisterConcern;
            _sysInfoService = sysInfoService;

        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            TempData["TransferViewModel"] = null;

            var daterange = GetFirstAndLastDateOfMonth(DateTime.Now);
            var ITrans = _transferService.GetAllAsync(daterange.Item1, daterange.Item2, User.Identity.GetConcernId());
            ViewBag.FromDate = daterange.Item1;
            ViewBag.ToDate = daterange.Item2;
            var vmTrans = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>>, IEnumerable<GetTransferViewModel>>(await ITrans);
            return View(vmTrans);
        }
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            TempData["TransferViewModel"] = null;

            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var ITrans = _transferService.GetAllAsync(ViewBag.FromDate, ViewBag.ToDate, User.Identity.GetConcernId());
            var vmTrans = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>>, IEnumerable<GetTransferViewModel>>(await ITrans);
            return View(vmTrans);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            TransferViewModel TransferViewModel = (TransferViewModel)TempData.Peek("TransferViewModel");
            if (TransferViewModel != null)
            {
                //tempdata getting null after redirection, so we're restoring Transfer 
                TempData["TransferViewModel"] = TransferViewModel;
                return View("Create", TransferViewModel);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.TransferNo));
                return View(PopulateDropdown((new TransferViewModel
                {
                    Detail = new TransferDetailViewModel(),
                    Details = new List<TransferDetailViewModel>(),
                    Transfer = new GetTransferViewModel { TransferNo = invNo }
                })));
            }
        }



        [HttpPost]
        public ActionResult Create(TransferViewModel TransferViewModel, FormCollection formcollection)
        {
            return HandleTransferOrder(TransferViewModel, formcollection);
        }
        private ActionResult HandleTransferOrder(TransferViewModel NewTransferOrder, FormCollection formCollection)
        {
            if (NewTransferOrder != null)
            {

                NewTransferOrder.Detail = new TransferDetailViewModel();

                //if (formCollection.Get("addButton") != null)
                //{
                //    CheckAndAddModelErrorForAdd(NewTransferOrder, Transfer, formCollection);
                //    if (!ModelState.IsValid)
                //    {
                //        Transfer.Details = Transfer.Details ?? new List<TransferDetailViewModel>();
                //        return View("Create", PopulateDropdown(Transfer));
                //    }

                //    var product = _productService.GetProductById(NewTransferOrder.Detail.ProductID);

                //    if (Transfer.Details != null
                //            && Transfer.Details.Any(x => x.Status != EnumStatus.Updated
                //            && x.Status != EnumStatus.Deleted
                //            && x.SDetailID == NewTransferOrder.Detail.SDetailID))
                //    {
                //        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                //        return View("Create", PopulateDropdown(Transfer));
                //    }

                //    AddToOrder(NewTransferOrder, Transfer, formCollection);
                //    ModelState.Clear();
                //   // NewTransferOrder.Details.Any(x => x.ToGodownID == Transfer.Detail.ToGodownID);
                //    return View("Create", PopulateDropdown(Transfer));
                //}
                if (formCollection.Get("submitButton") != null)
                {
                    ModelState.Remove("Detail.PRate");
                    ModelState.Remove("Detail.Quantity");
                    ModelState.Remove("Detail.IMENo");

                    CheckAndAddModelErrorForAdd(NewTransferOrder, formCollection);
                    decimal calGrandtotal = NewTransferOrder.Details.Where(i => i.Status != EnumStatus.Deleted).Sum(i => Convert.ToDecimal(i.PRate) * Convert.ToDecimal(i.Quantity));

                    if (NewTransferOrder.Transfer.TotalAmount != calGrandtotal)
                    {
                        TempData["TransferViewModel"] = null;
                        AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                        return RedirectToAction("Index");
                    }

                    if (!ModelState.IsValid)
                    {
                        NewTransferOrder.Details = NewTransferOrder.Details ?? new List<TransferDetailViewModel>();
                        return View("Create", PopulateDropdown(NewTransferOrder));
                    }
                    bool Result = SaveOrder(NewTransferOrder, formCollection);
                    ModelState.Clear();
                    TempData["TransferViewModel"] = null;

                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new TransferViewModel
                    {
                        Detail = new TransferDetailViewModel(),
                        Details = new List<TransferDetailViewModel>(),
                        Transfer = new GetTransferViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId, string previousAction)
        {
            TransferViewModel transferOrder = (TransferViewModel)TempData.Peek("TransferViewModel");
            if (transferOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            var itemToDelete = transferOrder.Details.Where(x => x.ProductID == id &&
                             x.SDetailID == detailId).FirstOrDefault();

            if (itemToDelete != null)
            {


                transferOrder.Transfer.TotalAmount = transferOrder.Transfer.TotalAmount - itemToDelete.UTAmount;

                if (IsForEdit(previousAction) && itemToDelete.SDetailID != 0)
                {
                    itemToDelete.Status = EnumStatus.Deleted;
                }
                else
                {
                    transferOrder.Details.Remove(itemToDelete);
                }

                transferOrder.Detail = new TransferDetailViewModel();
                TempData["TransferViewModel"] = transferOrder;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);

                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                else
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
        public JsonResult ValidateProductColorForConcern(int toConcernId, int productId, int colorId)
        {
            var product = _productService.GetProductById(productId);
            var color = _ColorService.GetColorById(colorId);
            var concernColor = _ColorService.GetColorByConcernAndColorName(toConcernId, color?.Name);

            if (product == null)
                return Json(new { status = false, msg = "Product not found." }, JsonRequestBehavior.AllowGet);

            if (color == null || concernColor == null)
                return Json(new { status = false, msg = "Color not available in selected concern." }, JsonRequestBehavior.AllowGet);

            var targetProduct = _productService.GetProductByConcernAndName(toConcernId, product.ProductName);
            if (targetProduct == null)
                return Json(new { status = false, msg = "Product not available in selected concern." }, JsonRequestBehavior.AllowGet);

            if (targetProduct.ProductType != product.ProductType)
                return Json(new { status = false, msg = "Product type mismatch in target concern." }, JsonRequestBehavior.AllowGet);

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }



        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }
        //private void CheckAndAddModelErrorForAdd(TransferViewModel NewTransferOrder,
        //        TransferViewModel TransferOrder, FormCollection formCollection)
        //{
        //    string ConcernName = string.Empty;
        //    Product FProduct = null;
        //    if (string.IsNullOrEmpty(formCollection["TransferDate"]))
        //        ModelState.AddModelError("Transfer.TransferDate", "Transfer Date is required");

        //    //if (NewTransferOrder.Transfer.ToConcernID == 0)
        //    //    ModelState.AddModelError("Transfer.ToConcernID", "ToConcern  is required");
        //    //else
        //    //{
        //    //    var oToSisConcern = _SisterConcernService.GetSisterConcernById(NewTransferOrder.Transfer.ToConcernID);
        //    //    ConcernName = string.IsNullOrEmpty(oToSisConcern.Name) ? string.Empty : oToSisConcern.Name;
        //    //}
        //    //ProductDetailsId is ProductId
        //    //if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
        //    //    ModelState.AddModelError("Detail.ProductID", "Product is required");
        //    //else
        //    //{
        //    //    NewTransferOrder.Detail.ProductID = Convert.ToInt32(formCollection["ProductDetailsId"]);
        //    //    TransferOrder.Detail.ProductID = NewTransferOrder.Detail.ProductID;
        //    //}

        //    //if (NewTransferOrder.Detail.Quantity <= 0)
        //    //    ModelState.AddModelError("Detail.Quantity", "Quantity is required");

        //    //if (string.IsNullOrEmpty(formCollection["GodownsId"]))
        //    //    ModelState.AddModelError("Detail.ToGodownID", "To Godown is required");
        //    //else
        //    //{
        //    //    NewTransferOrder.Detail.ToGodownID = Convert.ToInt32(formCollection["GodownsId"]);
        //    //    TransferOrder.Detail.ToGodownID = NewTransferOrder.Detail.ToGodownID;
        //    //}




        //    if (string.IsNullOrEmpty(NewTransferOrder.Transfer.TransferNo))
        //        ModelState.AddModelError("Transfer.TransferNo", "Invoice No. is required");

        //    //if (string.IsNullOrEmpty(NewTransferOrder.Detail.MRPRate))
        //    //    ModelState.AddModelError("Detail.MRPRate", "Purchase Rate is required");

        //    //if (string.IsNullOrEmpty(NewTransferOrder.Detail.UnitPrice))
        //    //    ModelState.AddModelError("Detail.UnitPrice", "Sales Rate is required");

        //    if (string.IsNullOrEmpty(formCollection["StockDetailsId"]))
        //    {
        //        ModelState.AddModelError("Detail.IMENo", "IMENo/Barcode is required");
        //    }
        //    else
        //    {

        //        FProduct = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

        //        NewTransferOrder.Detail.SDetailID = int.Parse(GetDefaultIfNull(formCollection["StockDetailsId"]));
        //        //  int ColorID = int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"]));
        //        NewTransferOrder.Detail.ProductName = FProduct.ProductName;
        //        NewTransferOrder.Detail.ProductCode = FProduct.Code;

        //        StockDetail stockDeatilCount = null;
        //        stockDeatilCount = _stockDetailService.GetById(NewTransferOrder.Detail.SDetailID);// _stockService.GET.GetStockByProductIdandColorIDandGodownID(product.ProductID, 1, 1);
        //        if (FProduct.ProductType == (int)EnumProductType.NoBarcode)
        //        {
        //            var StockCount = _StockService.GetStockById(stockDeatilCount.StockID);
        //            if (StockCount.Quantity < NewTransferOrder.Detail.Quantity)
        //                ModelState.AddModelError("Detail.Quantity", "Stock is not available. Stock Quantity: " + StockCount.Quantity);


        //            var Color = _ColorService.GetColorById(stockDeatilCount.ColorID);
        //            var ToColor = _ColorService.GetColorByConcernAndColorName(NewTransferOrder.Transfer.ToConcernID, Color.Name);
        //            if (ToColor != null)
        //            {
        //                NewTransferOrder.Detail.ToColorID = ToColor.ColorID;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("Transfer.TransferNo", "");
        //                AddToastMessage("", "This color is not found in concern " + ConcernName, ToastType.Error);
        //            }

        //        }
        //        else
        //        {
        //            var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));


        //        }

        //        #region Receiver concern validations check
        //        var ToProduct = _productService.GetProductByConcernAndName(NewTransferOrder.Transfer.ToConcernID, NewTransferOrder.Detail.ProductName);

        //        if (ToProduct != null)
        //        {
        //            NewTransferOrder.Detail.ToProductID = ToProduct.ProductID;
        //            if (FProduct != null)
        //                if (FProduct.ProductType != ToProduct.ProductType)
        //                {
        //                    ModelState.AddModelError("Transfer.TransferNo", "Product Type is Not Same.");
        //                    AddToastMessage("", "Product type is not same in concern " + ConcernName, ToastType.Error);
        //                }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("Transfer.TransferNo", "");
        //            AddToastMessage("", "This product is not found in concern " + ConcernName, ToastType.Error);
        //        }
        //        /*
        //        var Color = _ColorService.GetColorById(stockDeatilCount.ColorID);
        //        var ToColor = _ColorService.GetColorByConcernAndColorName(NewTransferOrder.Transfer.ToConcernID, Color.Name);
        //        if (ToColor != null)
        //        {
        //            NewTransferOrder.Detail.ToColorID = ToColor.ColorID;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("Transfer.TransferNo", "");
        //            AddToastMessage("", "This color is not found in concern " + ConcernName, ToastType.Error);
        //        }*/

        //        if (!string.IsNullOrEmpty(formCollection["StockDetailsId"]) && FProduct.ProductType != (int)EnumProductType.NoBarcode)
        //        {
        //            string[] IMEIS = formCollection.AllKeys
        //                   .Where(key => key.StartsWith("IMEI"))
        //                   .Select(key => formCollection[key])
        //                   .ToArray();

        //            if (IMEIS.Length <= 0)
        //            {
        //                ModelState.AddModelError("Detail.IMENo", "IMEI/Barcode is required");
        //            }
        //            else
        //            {
        //                ProductDetailsModel oDetail = null;
        //                TransferOrder.IMEIList = new List<ProductDetailsModel>();
        //                int StockDetailID = 0;
        //                for (int i = 0; i < IMEIS.Length; i++)
        //                {
        //                    oDetail = _StockService.GetStockIMEIDetail(IMEIS[i]);
        //                    if (oDetail != null)
        //                    {
        //                        StockDetailID = oDetail.StockDetailsId;

        //                        var Color = _ColorService.GetColorById(stockDeatilCount.ColorID);
        //                        var ToColor = _ColorService.GetColorByConcernAndColorName(NewTransferOrder.Transfer.ToConcernID, oDetail.ColorName);
        //                        if (ToColor != null)
        //                        {
        //                            oDetail.ColorId = ToColor.ColorID;
        //                            //NewTransferOrder.Detail.ToColorID = ToColor.ColorID;
        //                        }
        //                        else
        //                        {
        //                            ModelState.AddModelError("Transfer.TransferNo", "");
        //                            AddToastMessage("", "This color is not found in concern " + ConcernName, ToastType.Error);
        //                        }


        //                        TransferOrder.IMEIList.Add(oDetail);
        //                        //TransferOrder.Detail.ProductID = oDetail.ProductId;

        //                        if (_StockService.IsIMEIExistInGodown(TransferOrder.Transfer.ToConcernID, NewTransferOrder.Detail.ToGodownID, oDetail.IMENo))
        //                        {
        //                            ModelState.AddModelError("Detail.ToProductID", "IMEI " + oDetail.IMENo + " is already exists in the selected godown of the concern " + ConcernName);
        //                            AddToastMessage("", "IMEI " + oDetail.IMENo + " is exist in concern " + ConcernName, ToastType.Error);
        //                        }

        //                        if (TransferOrder.Details != null)
        //                        {
        //                            if (TransferOrder.Details.Any(j => j.SDetailID == oDetail.StockDetailsId))
        //                            {
        //                                ModelState.AddModelError("SODetail.IMENo", "This IMEI is already added.");
        //                                AddToastMessage("", oDetail.IMENo + " is already added.", ToastType.Error);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        oDetail = new ProductDetailsModel();
        //                        oDetail.IMENo = IMEIS[i];
        //                        oDetail.ProductId = TransferOrder.Detail.ProductID;
        //                        TransferOrder.IMEIList.Add(oDetail);
        //                        ModelState.AddModelError("Detail.IMENo", IMEIS[i] + " is Invalid IMENo/Barcode");
        //                    }
        //                    oDetail = new ProductDetailsModel();
        //                }
        //            }
        //        }
        //        #endregion
        //    }


        //}

        private void CheckAndAddModelErrorForAdd(TransferViewModel NewTransferOrder, FormCollection formCollection)
        {
            string ConcernName = string.Empty;

            if (string.IsNullOrEmpty(formCollection["TransferDate"]))
                ModelState.AddModelError("Transfer.TransferDate", "Transfer Date is required");

            if (string.IsNullOrEmpty(NewTransferOrder.Transfer.TransferNo))
                ModelState.AddModelError("Transfer.TransferNo", "Invoice No. is required");

            NewTransferOrder.Transfer.TransferDate = Convert.ToDateTime(formCollection["TransferDate"]);
            NewTransferOrder.Transfer.FromConcernID = User.Identity.GetConcernId();
            NewTransferOrder.Transfer.Status = EnumTransferStatus.Transfer;
            //NewTransferOrder.Details.ToGodownID =
            if (!IsDateValid(Convert.ToDateTime(NewTransferOrder.Transfer.TransferDate)))
            {
                ModelState.AddModelError("Transfer.TransferDate", "Back dated entry is not valid.");
            }

            if (string.IsNullOrEmpty(formCollection["TransferDate"]))
                ModelState.AddModelError("Transfer.TransferDate", "Transfer Date is required.");
            else
                NewTransferOrder.Transfer.TransferDate = Convert.ToDateTime(formCollection["TransferDate"]);

            foreach (var detail in NewTransferOrder.Details)
            {

                Product FProduct = _productService.GetProductById(detail.ProductID);
                StockDetail stockDetailCount = _stockDetailService.GetById(detail.SDetailID);

                if (FProduct == null || stockDetailCount == null)
                    continue; // Skip this detail if product or stock detail is invalid

                detail.ProductName = FProduct.ProductName;
                detail.ProductCode = FProduct.Code;
                detail.ToGodownID = Convert.ToInt32(formCollection["GodownsId"]);

                if (FProduct.ProductType == (int)EnumProductType.NoBarcode)
                {
                    var StockCount = _StockService.GetStockById(stockDetailCount.StockID);
                    if (StockCount.Quantity < detail.Quantity)
                        ModelState.AddModelError("Detail.Quantity", "Stock is not available. Stock Quantity: " + StockCount.Quantity);

                    var Color = _ColorService.GetColorById(stockDetailCount.ColorID);
                    var ToColor = _ColorService.GetColorByConcernAndColorName(NewTransferOrder.Transfer.ToConcernID, Color.Name);
                    if (ToColor != null)
                    {
                        detail.ToColorID = ToColor.ColorID;
                    }
                    else
                    {
                        AddToastMessage("", "This color is not found in concern " + ConcernName, ToastType.Error);
                    }
                }
                else
                {
                    var stockDetails = _stockDetailService.GetStockDetailByProductId(detail.ProductID);
                    //if (!string.IsNullOrEmpty(formCollection["StockDetailsId"]))
                    //{
                    //foreach (var key in formCollection.AllKeys)
                    //{
                    //    Console.WriteLine($"{key}: {formCollection[key]}");
                    //}
                    //string[] IMEIs = formCollection.AllKeys
                    //                .Where(key => key.Contains(".IMENo")) // Filters keys that contain '.IMENo'
                    //                .Select(key => formCollection[key]) // Selects the values for those keys
                    //                .ToArray();


                    ProductDetailsModel oDetail = _StockService.GetStockIMEIDetail(detail.IMENo);
                    if (oDetail != null)
                    {
                        var ToColor = _ColorService.GetColorByConcernAndColorName(NewTransferOrder.Transfer.ToConcernID, oDetail.ColorName);
                        if (ToColor != null)
                        {
                            detail.ToColorID = ToColor.ColorID;
                        }
                        else
                        {
                            AddToastMessage("", "This color is not found in concern " + ConcernName, ToastType.Error);
                        }

                        if (_StockService.IsIMEIExistInGodown(NewTransferOrder.Transfer.ToConcernID, detail.ToGodownID, detail.IMENo))
                        {
                            ModelState.AddModelError("Detail.ToProductID", $"IMEI {detail.IMENo} is already in the selected godown of the concern {ConcernName}");
                            AddToastMessage("", $"IMEI {detail.IMENo} exists in concern {ConcernName}", ToastType.Error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Detail.IMENo", $"{detail.IMENo} is Invalid IMENo/Barcode");
                    }

                    ////if (IMEIs.Length <= 0)
                    ////{
                    ////    ModelState.AddModelError("Detail.IMENo", "IMEI/Barcode is required");
                    ////}                        
                    //else
                    //{
                    //    foreach (var imei in IMEIs)
                    //    {

                    //    }
                    //}
                    //}
                }

                // Validate receiver concern product type
                var ToProduct = _productService.GetProductByConcernAndName(NewTransferOrder.Transfer.ToConcernID, detail.ProductName);
                if (ToProduct != null)
                {
                    detail.ToProductID = ToProduct.ProductID;
                    if (FProduct.ProductType != ToProduct.ProductType)
                    {
                        ModelState.AddModelError("Transfer.TransferNo", "Product Type is Not Same.");
                        AddToastMessage("", $"Product type is not same in concern {ConcernName}", ToastType.Error);
                    }
                }
                else
                {
                    AddToastMessage("", "This product is not found in concern " + ConcernName, ToastType.Error);
                }
            }
        }



        //private void AddToOrder(TransferViewModel newTransfer,
        //              TransferViewModel Transfer, FormCollection formCollection)
        //{

        //    if (Transfer.Details == null)
        //        Transfer.Details = new List<TransferDetailViewModel>();

        //    Transfer.Transfer.FromConcernID = User.Identity.GetConcernId();
        //    Transfer.Transfer.Remarks = newTransfer.Transfer.Remarks;
        //    Transfer.Transfer.ToConcernID = newTransfer.Transfer.ToConcernID;
        //    Transfer.Transfer.TransferDate = newTransfer.Transfer.TransferDate;
        //    Transfer.Transfer.TransferNo = newTransfer.Transfer.TransferNo;

        //     newTransfer.Detail.Status = EnumStatus.New;
        //    var product = _productService.GetProductById(newTransfer.Detail.ProductID);
        //    if (product.ProductType == (int)EnumProductType.NoBarcode)
        //    {
        //        Transfer.Details.Add(newTransfer.Detail);
        //    }
        //    else
        //    {
        //        TransferDetailViewModel transferDetail = null;
        //        foreach (var item in Transfer.IMEIList)
        //        {
        //            transferDetail = new TransferDetailViewModel();
        //            transferDetail.ProductID = newTransfer.Detail.ProductID;
        //            transferDetail.ProductCode = newTransfer.Detail.ProductCode;
        //            transferDetail.ProductName = newTransfer.Detail.ProductName;
        //            transferDetail.SDetailID = item.StockDetailsId;
        //            transferDetail.PRate = item.PRate;
        //            transferDetail.UTAmount = item.PRate;
        //            transferDetail.IMENo = item.IMENo;
        //            transferDetail.Quantity = 1;
        //            transferDetail.Status = EnumStatus.New;
        //            transferDetail.ToProductID = newTransfer.Detail.ToProductID;
        //            transferDetail.ToColorID = item.ColorId;//newTransfer.Detail.ToColorID;
        //            transferDetail.ToGodownID = newTransfer.Detail.ToGodownID;
        //            Transfer.Details.Add(transferDetail);


        //        }
        //    }
        //    Transfer.Transfer.TotalAmount = Transfer.Details.Sum(i => (i.PRate * i.Quantity));

        //    TransferViewModel vm = new TransferViewModel
        //    {
        //        Detail = new TransferDetailViewModel(),
        //        Details = Transfer.Details,
        //        Transfer = Transfer.Transfer
        //    };
        //    Transfer.IMEIList = new List<ProductDetailsModel>();
        //    TempData["TransferViewModel"] = vm;
        //    int Godowon = Transfer.Detail.ToGodownID;
        //    Transfer.Detail = new TransferDetailViewModel() {ToGodownID = Godowon};
        //    AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        //}

        private bool SaveOrder(TransferViewModel newTransfer, FormCollection formCollection)
        {
            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            //newTransfer.Transfer.FromConcernID = User.Identity.GetConcernId();
            //newTransfer.Transfer.ToConcernID = newTransfer.Transfer.ToConcernID;
            //newTransfer.Transfer.Remarks = newTransfer.Transfer.Remarks;
            //newTransfer.Transfer.Status = EnumTransferStatus.Transfer;
            //newTransfer.Transfer.TotalAmount = newTransfer.Transfer.TotalAmount;
            //newTransfer.Transfer.TransferDate = newTransfer.Transfer.TransferDate;
            //newTransfer.Transfer.TransferNo = newTransfer.Transfer.TransferNo;


            //removing unchanged previous order
            //newTransfer.Details.Where(x => x.SDetailID != 0 && x.Status == default(int)).ToList()
            //    .ForEach(x => newTransfer.Details.Remove(x));

            //if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            //{
            //    string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.TransferNo));
            //    newTransfer.Transfer.TransferNo = invNo;
            //}

            DataTable dtTransferOrder = CreateTransferDataTable(newTransfer);
            DataTable dtDetails = CreateTransferDetailDataTable(newTransfer);
            DataTable dtTransferFromStock = CreateTransferFromStockDataTable(newTransfer);
            DataTable dtTransferToStock = CreateTransferToStockDataTable(newTransfer);
            #region Log
            //log.Info(new { Transfer = Transfer.Transfer, Details = Transfer.Details });
            #endregion

            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                //Result = _TransferService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(),Transfer.Transfer.TransferID,dtSalesOrder, dtSalesOrderDetail);
                if (Result.Item1)
                {
                    TempData["IsTransferInvoiceReadyById"] = true;
                    TempData["TransferID"] = newTransfer.Transfer.TransferID;
                }
            }
            else
            {

                Result = _transferService.AddTranserferUsingSP(dtTransferOrder, dtDetails, dtTransferFromStock, dtTransferToStock);

                if (Result.Item1)
                {
                    TempData["IsTransferInvoiceReadyById"] = true;
                    TempData["TransferID"] = Result.Item2;
                }
            }

            //_TransferService.CorrectionStockData(User.Identity.GetConcernId());
            #region For POS Invoice
            //PrintInvoice oPriInvoice = new PrintInvoice();
            //oPriInvoice.print(Transfer, _SisterConcern);
            #endregion

            if (Result.Item1)
                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Order has been failed.", ToastType.Error);

            return Result.Item1;
        }
        //private void CheckAndAddModelErrorForSave(TransferViewModel newTransfer, TransferViewModel Transfer, FormCollection formCollection)
        //{
        //    if (newTransfer.Transfer.TotalAmount <= 0)
        //        ModelState.AddModelError("Transfer.GrandTotal", "Grand Total is required");

        //    if (newTransfer.Transfer.TotalAmount <= 0)
        //        ModelState.AddModelError("Transfer.TotalAmount", "Net Total is required");

        //    var distinctIMEI = Transfer.Details
        //                        .GroupBy(i => i.SDetailID)
        //                        .Select(g => g.First())
        //                        .ToList();

        //    if (distinctIMEI.Count() != Transfer.Details.Count())
        //    {
        //        ModelState.AddModelError("SODetail.IMENo", "");
        //        AddToastMessage("", "Duplicate IMEI added.", ToastType.Error);
        //    }
        //    Transfer.Transfer.TransferDate = Convert.ToDateTime(formCollection["TransferDate"]);
        //    if (!IsDateValid(Convert.ToDateTime(Transfer.Transfer.TransferDate)))
        //    {
        //        ModelState.AddModelError("Transfer.TransferDate", "Back dated entry is not valid.");
        //    }

        //    if (string.IsNullOrEmpty(formCollection["TransferDate"]))
        //        ModelState.AddModelError("Transfer.TransferDate", "Transfer Date is required.");
        //    else
        //        newTransfer.Transfer.TransferDate = Convert.ToDateTime(formCollection["TransferDate"]);
        //}
        private DataTable CreateTransferDataTable(TransferViewModel newTransfer)
        {
            DataTable dtTransferOrder = new DataTable();
            dtTransferOrder.Columns.Add("TransferDate", typeof(DateTime));
            dtTransferOrder.Columns.Add("TransferNo", typeof(string));
            dtTransferOrder.Columns.Add("Remarks", typeof(string));
            dtTransferOrder.Columns.Add("TotalAmount", typeof(decimal));
            dtTransferOrder.Columns.Add("ToConcernID", typeof(int));
            dtTransferOrder.Columns.Add("ConcernId", typeof(int));
            dtTransferOrder.Columns.Add("FromConcernID", typeof(int));
            dtTransferOrder.Columns.Add("Status", typeof(int));
            dtTransferOrder.Columns.Add("CreatedBy", typeof(int));
            dtTransferOrder.Columns.Add("CreatedDate", typeof(DateTime));

            DataRow row = null;

            row = dtTransferOrder.NewRow();
            row["TransferDate"] = newTransfer.Transfer.TransferDate;
            row["TransferNo"] = newTransfer.Transfer.TransferNo;
            row["Remarks"] = newTransfer.Transfer.Remarks;
            row["TotalAmount"] = newTransfer.Transfer.TotalAmount;
            //row["Status"] = newTransfer.Transfer.Status;
            row["Status"] = EnumTransferStatus.Transfer;
            row["ToConcernID"] = newTransfer.Transfer.ToConcernID;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["FromConcernID"] = newTransfer.Transfer.FromConcernID;

            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();

            dtTransferOrder.Rows.Add(row);

            return dtTransferOrder;
        }

        private DataTable CreateTransferDetailDataTable(TransferViewModel newTransfer)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("TDetailID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ProductID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorID", typeof(int));
            dtSalesOrderDetail.Columns.Add("GodownID", typeof(int));

            dtSalesOrderDetail.Columns.Add("ToProductID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ToGodownID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ToColorID", typeof(int));
            dtSalesOrderDetail.Columns.Add("Status", typeof(int));

            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UTAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("SDetailID", typeof(int));

            dtSalesOrderDetail.Columns.Add("StockID", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockCode", typeof(string));
            dtSalesOrderDetail.Columns.Add("IMEI", typeof(string));
            dtSalesOrderDetail.Columns.Add("SRate", typeof(decimal));


            DataRow row = null;
            StockDetail oStockDetail = null;
            Stock oStock = null;
            foreach (var item in newTransfer.Details)
            {
                row = dtSalesOrderDetail.NewRow();
                if (item.TDetailID == 0)
                    row["TDetailID"] = item.TDetailID;

                oStockDetail = _stockDetailService.GetById(item.SDetailID);
                if (oStockDetail != null)
                {
                    row["ProductID"] = oStockDetail.ProductID;
                    row["GodownID"] = oStockDetail.GodownID;
                    row["ColorID"] = oStockDetail.ColorID;
                    row["IMEI"] = oStockDetail.IMENO;
                    row["SRate"] = oStockDetail.SRate;


                    row["ToProductID"] = item.ToProductID;
                    row["ToGodownID"] = item.ToGodownID;
                    row["ToColorID"] = item.ToColorID;
                }

                row["Status"] = item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.PRate;
                row["UTAmount"] = item.UTAmount;
                row["SDetailID"] = item.SDetailID;
                oStock = _StockService.GetStockByProductIdandColorIDandGodownID(item.ToProductID, item.ToGodownID, item.ToColorID);
                if (oStock != null)
                    row["StockID"] = oStock.StockID;
                else
                    row["StockID"] = 0;

                row["StockCode"] = item.ProductCode;

                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        private DataTable CreateTransferFromStockDataTable(TransferViewModel newTransfer)
        {
            var TransferProducts = from tp in newTransfer.Details
                                   join sd in _stockDetailService.GetAll() on tp.SDetailID equals sd.SDetailID
                                   group tp by new { tp.ProductID, sd.ColorID, sd.GodownID } into g
                                   select new
                                   {
                                       g.Key.ProductID,
                                       g.Key.ColorID,
                                       g.Key.GodownID,
                                       Quantity = g.Sum(i => i.Quantity),
                                       PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                       CreateDate = newTransfer.Transfer.TransferDate,
                                       CreatedBy = User.Identity.GetUserId<int>(),
                                   };

            DataTable dtStock = new DataTable();
            dtStock.Columns.Add("StockId", typeof(int));
            dtStock.Columns.Add("StockCode", typeof(string));
            dtStock.Columns.Add("ProductID", typeof(int));
            dtStock.Columns.Add("GodownID", typeof(int));
            dtStock.Columns.Add("ColorID", typeof(int));
            dtStock.Columns.Add("Status", typeof(int));
            dtStock.Columns.Add("EntryDate", typeof(DateTime));
            dtStock.Columns.Add("Quantity", typeof(decimal));
            dtStock.Columns.Add("MRPPrice", typeof(decimal));
            dtStock.Columns.Add("LPPrice", typeof(decimal));
            dtStock.Columns.Add("ConcernID", typeof(int));
            dtStock.Columns.Add("CreateDate", typeof(DateTime));
            dtStock.Columns.Add("CreatedBy", typeof(int));

            DataRow row = null;
            foreach (var item in TransferProducts)
            {
                row = dtStock.NewRow();

                row["StockId"] = 0;
                row["StockCode"] = string.Empty;
                row["ColorID"] = item.ColorID;
                row["Status"] = 0;
                row["EntryDate"] = DateTime.Now;
                row["Quantity"] = item.Quantity;
                row["ProductID"] = item.ProductID;
                row["MRPPrice"] = item.PRate;
                row["LPPrice"] = item.PRate;
                row["ConcernID"] = newTransfer.Transfer.ToConcernID;
                row["CreateDate"] = DateTime.Now;
                row["CreatedBy"] = User.Identity.GetUserId<int>();
                row["GodownID"] = item.GodownID;

                dtStock.Rows.Add(row);
            }

            return dtStock;
        }
        private DataTable CreateTransferToStockDataTable(TransferViewModel newTransfer)
        {
            var TransferProducts = from tp in newTransfer.Details
                                   group tp by new { tp.ToProductID, tp.ToColorID, tp.ToGodownID } into g
                                   select new
                                   {
                                       g.Key.ToProductID,
                                       g.Key.ToColorID,
                                       g.Key.ToGodownID,
                                       Quantity = g.Sum(i => i.Quantity),
                                       PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                       CreateDate = newTransfer.Transfer.TransferDate,
                                       CreatedBy = User.Identity.GetUserId<int>(),
                                   };

            DataTable dtStock = new DataTable();
            dtStock.Columns.Add("StockId", typeof(int));
            dtStock.Columns.Add("StockCode", typeof(string));
            dtStock.Columns.Add("ProductID", typeof(int));
            dtStock.Columns.Add("GodownID", typeof(int));
            dtStock.Columns.Add("ColorID", typeof(int));
            dtStock.Columns.Add("Status", typeof(int));
            dtStock.Columns.Add("EntryDate", typeof(DateTime));
            dtStock.Columns.Add("Quantity", typeof(decimal));
            dtStock.Columns.Add("MRPPrice", typeof(decimal));
            dtStock.Columns.Add("LPPrice", typeof(decimal));
            dtStock.Columns.Add("ConcernID", typeof(int));
            dtStock.Columns.Add("CreateDate", typeof(DateTime));
            dtStock.Columns.Add("CreatedBy", typeof(int));

            DataRow row = null;
            foreach (var item in TransferProducts)
            {
                row = dtStock.NewRow();

                row["StockId"] = 0;
                row["StockCode"] = string.Empty;
                row["ColorID"] = item.ToColorID;
                row["Status"] = 0;
                row["EntryDate"] = DateTime.Now;
                row["Quantity"] = item.Quantity;
                row["ProductID"] = item.ToProductID;
                row["MRPPrice"] = item.PRate;
                row["LPPrice"] = item.PRate;
                row["ConcernID"] = newTransfer.Transfer.ToConcernID;
                row["CreateDate"] = DateTime.Now;
                row["CreatedBy"] = User.Identity.GetUserId<int>();
                row["GodownID"] = item.ToGodownID;

                dtStock.Rows.Add(row);
            }

            return dtStock;
        }
        TransferViewModel PopulateDropdown(TransferViewModel tVM)
        {
            var concern = _SisterConcernService.GetSisterConcernById(User.Identity.GetConcernId());
            List<SisterConcern> ancestDesendents = null;
            SisterConcern Parent = null;
            if (concern.ParentID > 0)
            {
                ancestDesendents = _SisterConcernService.GetAll().Where(i => i.ParentID == concern.ParentID).ToList();
                Parent = _SisterConcernService.GetAll().FirstOrDefault(i => i.ConcernID == concern.ParentID);
                ancestDesendents.Add(Parent);
            }
            else
            {
                ancestDesendents = _SisterConcernService.GetAll().Where(i => i.ParentID == concern.ConcernID).ToList();
                Parent = _SisterConcernService.GetAll().FirstOrDefault(i => i.ConcernID == concern.ConcernID);
                ancestDesendents.Add(Parent);
            }

            tVM.Transfer.SisterConcerns = ancestDesendents;
            return tVM;
        }


        public ActionResult Invoice(int id)
        {
            TempData["TransferID"] = id;
            TempData["IsTransferInvoiceReadyById"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {


            if (id > 0)
            {

                if (HasTransferProductSorderCheckByTransDId(id))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. Can't delete this Transfer",
                        ToastType.Error);
                    return RedirectToAction("Index");
                }

                if (HasTransferProductCreditSalesCheckByTransDId(id))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. Can't delete this Transfer",
                        ToastType.Error);
                    return RedirectToAction("Index");
                }

                if (_transferService.ReturnTranserferUsingSP(id))
                    AddToastMessage("", "Return Successfull.", ToastType.Success);
                else
                    AddToastMessage("", "Return Failed.", ToastType.Error);


                //if (!_transferService.CheckTransProductStatusByTransDId(id))
                //{
                //       if (_transferService.ReturnTranserferUsingSP(id))
                //        AddToastMessage("", "Return Successfull.", ToastType.Success);
                //    else
                //        AddToastMessage("", "Return Failed.", ToastType.Error);
                //}

                //else
                //    AddToastMessage("", "This Product All Ready Sold Return Failed.", ToastType.Error);

            }

            return RedirectToAction("index");
        }

        private bool HasTransferProductSorderCheckByTransDId(int id)
        {
            int sold = _transferService.CheckTransProductStatusByTransDId(id);
            return sold > 0;
        }

        private bool HasTransferProductCreditSalesCheckByTransDId(int id)
        {
            int sold = _transferService.CheckTransProductHireSalesStatusByTransDId(id);
            return sold > 0;
        }

        [HttpGet]
        public ActionResult TransferReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdminTransferReport()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }
        [HttpGet]
        public ActionResult TransferReportNewFormat()
        {
            @ViewBag.FromConcerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            @ViewBag.ToConcerns = new SelectList(_SisterConcern.GetToAll(), "ConcernID", "Name");

            return View();
        }
    }
}