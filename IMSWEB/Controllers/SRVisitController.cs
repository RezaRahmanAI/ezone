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
using log4net;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("srvisit")]
    public class SRVisitController : CoreController
    {
        ISRVisitService _srVisitService;
        ISRVisitDetailService _srvDetailService;
        ISRVProductDetailService _srVProductDetailService;
        IMiscellaneousService<SRVisit> _miscellaneousService;
        IStockService _StockService;
        IStockDetailService _stockDetailService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SRVisitController(IErrorService errorService,
            ISRVisitService srVisitService, ISRVisitDetailService srVisitDetailService, ISRVProductDetailService sRVProductDetailService,
            IStockService stockService, IStockDetailService stockDetailService,
            IMiscellaneousService<SRVisit> miscellaneousService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _srVisitService = srVisitService;
            _srvDetailService = srVisitDetailService;
            _srVProductDetailService = sRVProductDetailService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _StockService = stockService;
            _stockDetailService = stockDetailService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            TempData["SRVisitViewModel"] = null;
            var customPO = _srVisitService.GetAllSRVisitAsync();
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, EnumSRVisitType>>,
                IEnumerable<GetSRVisitViewModel>>(await customPO);
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
        public ActionResult Create(SRVisitViewModel newSRVisit, FormCollection formCollection, string returnUrl)
        {
            return HandleSRVisit(newSRVisit, formCollection);
        }


        [HttpGet]
        [Authorize]
        [Route("edit/{srvisitId}")]
        public ActionResult Edit(int orderId, string previousAction)
        {
            if (TempData["SRVisitViewModel"] == null || string.IsNullOrEmpty(previousAction))
            {
                var srVisit = _srVisitService.GetSRVisitById(orderId);
                var srvDetails = _srvDetailService.GetSRVisitDetailById(orderId);

                var vmSRVisit = _mapper.Map<SRVisit, CreateSRVisitViewModel>(srVisit);
                var vmSRVDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, decimal, string, string, int,
                    Tuple<string>>>, IEnumerable<CreateSRVisitDetailViewModel>>(srvDetails).ToList();

                var vm = new SRVisitViewModel
                {
                    SRVDetail = new CreateSRVisitDetailViewModel(),
                    SRVDetails = vmSRVDetails,
                    SRVisit = vmSRVisit
                };

                TempData["SRVisitViewModel"] = vm;
                return View("Create", vm);
            }
            else
            {
                return ReturnCreateViewWithTempData();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(SRVisitViewModel newSRVisit, FormCollection formCollection, string returnUrl)
        {
            return HandleSRVisit(newSRVisit, formCollection);
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{srvisitId}")]
        public ActionResult Delete(int orderId)
        {
            if (_srVisitService.CheckSRVisitReturnValidity(orderId))
            {
                if (_srVisitService.DeleteSRVisitUsingSP(orderId, User.Identity.GetUserId<int>()))
                    AddToastMessage("", "SR Visit Cancelled successfully", ToastType.Success);
                else
                    AddToastMessage("", "SR Visit Cancelled Failed", ToastType.Error);

            }
            else
            {
                AddToastMessage("", "This SR visit will not cancel. Some of Products are sold from this order.", ToastType.Error);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("EditFromView/{id}/{previousAction}")]
        public ActionResult EditFromView(int id, int cid, string previousAction)
        {
            SRVisitViewModel srVisit = (SRVisitViewModel)TempData.Peek("SRVisitViewModel");
            if (srVisit == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSRVisitDetailViewModel itemToEdit = srVisit.SRVDetails.Where(x => int.Parse(x.ProductID) == id &&
                    int.Parse(x.ColorID) == cid).FirstOrDefault();

            if (itemToEdit != null)
            {
                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToEdit.SRVisitDID))
                {
                    itemToEdit.Status = EnumStatus.Deleted;
                    int srVisitDetailId = int.Parse(itemToEdit.SRVisitDID);
                    int productId = int.Parse(itemToEdit.ProductID);

                    itemToEdit.SRVProductDetails = _srVProductDetailService.
                                            GetSRVProductDetailsById(srVisitDetailId, productId).ToList();
                }
                else
                {
                    srVisit.SRVDetails.Remove(itemToEdit);
                }

                TempData["SRVProductDetails"] = itemToEdit.SRVProductDetails.ToList();
                srVisit.SRVDetail = itemToEdit;
                itemToEdit.SRVProductDetails.Clear();
                TempData["SRVisitViewModel"] = srVisit;

                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                else
                    return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("deleteFromView/{id}")]
        public ActionResult DeleteFromView(int id, int cid, string previousAction)
        {
            SRVisitViewModel srVisit = (SRVisitViewModel)TempData.Peek("SRVisitViewModel");
            if (srVisit == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSRVisitDetailViewModel itemToDelete = srVisit.SRVDetails.Where(x => int.Parse(x.ProductID) == id &&
                    int.Parse(x.ColorID) == cid).FirstOrDefault();

            if (itemToDelete != null)
            {

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToDelete.SRVisitDID))
                {
                    itemToDelete.Status = EnumStatus.Deleted;
                    itemToDelete.SRVProductDetails = new List<SRVProductDetail>();
                }
                else
                {
                    srVisit.SRVDetails.Remove(itemToDelete);
                }

                TempData["SRVisitViewModel"] = srVisit;
                srVisit.SRVDetail = new CreateSRVisitDetailViewModel();
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

        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            SRVisitViewModel srVisit = (SRVisitViewModel)TempData.Peek("SRVisitViewModel");
            if (srVisit != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["SRVisitViewModel"] = srVisit;
                return View("Create", srVisit);
            }
            else
            {
                string chllnNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ChallanNo));
                return View(new SRVisitViewModel
                {
                    SRVDetail = new CreateSRVisitDetailViewModel(),
                    SRVDetails = new List<CreateSRVisitDetailViewModel>(),
                    SRVisit = new CreateSRVisitViewModel { ChallanNo = chllnNo }
                });
            }
        }

        private ActionResult HandleSRVisit(SRVisitViewModel newSRVisit, FormCollection formCollection)
        {
            if (newSRVisit != null)
            {
                TempData["SRVProductDetails"] = null;
                SRVisitViewModel srVisit = (SRVisitViewModel)TempData.Peek("SRVisitViewModel");
                srVisit = srVisit ?? new SRVisitViewModel()
                {
                    SRVisit = newSRVisit.SRVisit
                };
                srVisit.SRVDetail = new CreateSRVisitDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSRVisit, formCollection);
                    srVisit.SRVDetails = srVisit.SRVDetails ?? new List<CreateSRVisitDetailViewModel>();
                    GetPickersValue(srVisit, formCollection);
                    AddTempPOProductDetails(newSRVisit, formCollection);

                    if (!ModelState.IsValid)
                    {
                        return View("Create", srVisit);
                    }
                    else if (HasDuplicateIMEIORBarcode(formCollection))
                    {
                        AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                        return View("Create", srVisit);
                    }
                    else if (srVisit.SRVDetails.Any(x => x.ProductID.Equals(formCollection["ProductsId"]) &&
                        x.ColorID.Equals(string.IsNullOrEmpty(formCollection["ColorsId"]) ? "1" : formCollection["ColorsId"])
                        && x.Status != EnumStatus.Deleted))
                    {
                        AddToastMessage("", "This product has already been added in the order", ToastType.Error);
                        return View("Create", srVisit);
                    }

                    AddToOrder(newSRVisit, srVisit, formCollection);
                    ModelState.Clear();
                    TempData["SRVProductDetails"] = null;
                    return View("Create", srVisit);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    //CheckAndAddModelErrorForSave(newPurchaseOrder, formCollection);

                    if (!ModelState.IsValid)
                    {
                        srVisit.SRVDetails = srVisit.SRVDetails ?? new List<CreateSRVisitDetailViewModel>();
                        return View("Create", srVisit);
                    }
                    else if (srVisit.SRVDetails == null || srVisit.SRVDetails.Count <= 0)
                    {
                        AddToastMessage("", "No order data found to save.", ToastType.Error);
                        return RedirectToAction("Create");
                    }

                    //var invoiceSRVisit = _mapper.Map<CreateSRVisitViewModel, SRVisit>(srVisit.SRVisit);
                    //invoiceSRVisit.SRVisitDetails = _mapper.Map<ICollection<CreateSRVisitDetailViewModel>,
                    //    ICollection<SRVisitDetail>>(srVisit.SRVDetails);



                    bool Result = SaveOrder(newSRVisit, srVisit, formCollection);
                    TempData["SRVisitViewModel"] = null;
                    ModelState.Clear();

                    if (Result)
                    {
                        TempData["SRVisitData"] = srVisit;
                        TempData["IsInvoiceReady"] = true;
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new SRVisitViewModel
                    {
                        SRVDetail = new CreateSRVisitDetailViewModel(),
                        SRVDetails = new List<CreateSRVisitDetailViewModel>(),
                        SRVisit = new CreateSRVisitViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void CheckAndAddModelErrorForAdd(SRVisitViewModel newSRVisit, FormCollection formCollection)
        {

            int ProductID = 0, ColorID = 0;
            if (string.IsNullOrEmpty(formCollection["VisitDate"]))
                ModelState.AddModelError("SRVisit.VisitDate", "Visit Date is required");

            if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
                ModelState.AddModelError("SRVisit.EmpId", "Employee is required");

            if (string.IsNullOrEmpty(formCollection["ProductsId"]))
                ModelState.AddModelError("SRVDetail.ProductId", "Product is required");
            else
                ProductID = int.Parse(formCollection["ProductsId"]);

            if (string.IsNullOrEmpty(newSRVisit.SRVisit.ChallanNo))
                ModelState.AddModelError("SRVisit.ChallanNo", "Challan No. is required");

            if (string.IsNullOrEmpty(newSRVisit.SRVDetail.Quantity))
                ModelState.AddModelError("SRVDetail.Quantity", "Quantity is required");

            string[] IMEIS = formCollection.AllKeys
                             .Where(key => key.StartsWith("IMEINo"))
                             .Select(key => formCollection[key])
                             .ToArray();

            if (!string.IsNullOrEmpty(formCollection["ColorsId"]))
                ColorID = int.Parse(formCollection["ColorsId"]);

            for (int i = 0; i < IMEIS.Count(); i++)
            {
                if (_StockService.IsIMEIAvailableForSRVisit(ProductID, ColorID, IMEIS[i]))
                {
                    if (_srVisitService.IsIMEIAlreadyIssuedToSR(ProductID, ColorID, IMEIS[i]))
                    {
                        AddToastMessage("SR visit", "IMEI " + IMEIS[i] + " already issued.", ToastType.Error);
                        ModelState.AddModelError("", "IMEI already issued.");
                        break;
                    }
                }
                else
                {
                    AddToastMessage("SR visit", "IMEI " + IMEIS[i] + " is not available in stock", ToastType.Error);
                    ModelState.AddModelError("", "IMEI is not available in stock.");
                    break;
                }

            }

        }

        private void GetPickersValue(SRVisitViewModel srVisit, FormCollection formCollection)
        {
            srVisit.SRVisit.EmployeeID = formCollection["EmployeesId"];
            srVisit.SRVDetail.ProductID = formCollection["ProductsId"];
            srVisit.SRVDetail.ProductCode = formCollection["ProductsCode"];
            srVisit.SRVDetail.ProductName = formCollection["ProductsName"];
            srVisit.SRVDetail.ColorID = string.IsNullOrEmpty(formCollection["ColorsId"]) ? "1" : formCollection["ColorsId"]; ;
        }

        private void AddTempPOProductDetails(SRVisitViewModel srVisit, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(srVisit.SRVDetail.Quantity)) return;

            List<SRVProductDetail> tempSRVPDetails = new List<SRVProductDetail>();


            for (int i = 1; i <= decimal.Parse(srVisit.SRVDetail.Quantity); i++)
            {
                var srVProductDetail = new SRVProductDetail();
                srVProductDetail.ProductID = int.Parse(GetDefaultIfNull(srVisit.SRVDetail.ProductID));

                #region Purpose of No COlor
                if (User.Identity.GetConcernId() == 1)
                    srVProductDetail.ColorID = 5;//1
                else if (User.Identity.GetConcernId() == 2)
                    srVProductDetail.ColorID = 1;
                else if (User.Identity.GetConcernId() == 3)
                    srVProductDetail.ColorID = 2;
                else if (User.Identity.GetConcernId() == 4)
                    srVProductDetail.ColorID = 6;
                else if (User.Identity.GetConcernId() == 5)
                    srVProductDetail.ColorID = 4;
                else if (User.Identity.GetConcernId() == 6)
                    srVProductDetail.ColorID = 3;
                #endregion


                //poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                srVProductDetail.IMENO = formCollection["IMEINo" + i.ToString()];
                if (!string.IsNullOrEmpty(srVProductDetail.IMENO.Trim()))
                    tempSRVPDetails.Add(srVProductDetail);
            }

            if (tempSRVPDetails.Count > 0)
                TempData["SRVProductDetails"] = tempSRVPDetails;

            //if (ProductType == (int)EnumProductType.ExistingBC || ProductType == (int)EnumProductType.AutoBC)
            //{
            if (decimal.Parse(srVisit.SRVDetail.Quantity) != tempSRVPDetails.Count())
                ModelState.AddModelError("SRVDetail.Quantity", "Quantity is not equal to the number of IMEI.");
            //}
        }

        private bool HasDuplicateIMEIORBarcode(FormCollection formCollection)
        {
            string[] IMEIS = formCollection.AllKeys
                             .Where(key => key.StartsWith("IMEINo"))
                             .Select(key => formCollection[key])
                             .ToArray();
            return IMEIS.Length != IMEIS.Distinct().Count();
        }

        private void AddToOrder(SRVisitViewModel newSRVisit, SRVisitViewModel srVisit, FormCollection formCollection)
        {


            srVisit.SRVisit.VisitDate = formCollection["VisitDate"];
            srVisit.SRVisit.EmployeeID = formCollection["EmployeesId"];

            srVisit.SRVDetail.SRVisitDID = newSRVisit.SRVDetail.SRVisitDID;
            srVisit.SRVDetail.ProductID = formCollection["ProductsId"];
            srVisit.SRVDetail.ProductCode = formCollection["ProductsCode"];
            srVisit.SRVDetail.ProductName = formCollection["ProductsName"];

            #region Purpose of No COlor
            if (User.Identity.GetConcernId() == 1)
                srVisit.SRVDetail.ColorID = "5";//1
            else if (User.Identity.GetConcernId() == 2)
                srVisit.SRVDetail.ColorID = "1";
            else if (User.Identity.GetConcernId() == 3)
                srVisit.SRVDetail.ColorID = "2";
            else if (User.Identity.GetConcernId() == 4)
                srVisit.SRVDetail.ColorID = "6";
            else if (User.Identity.GetConcernId() == 5)
                srVisit.SRVDetail.ColorID = "4";
            else if (User.Identity.GetConcernId() == 6)
                srVisit.SRVDetail.ColorID = "3";
            #endregion


            //purchaseOrder.PODetail.ColorId = string.IsNullOrEmpty(formCollection["ColorsId"]) ? "1" : formCollection["ColorsId"];

            srVisit.SRVDetail.ColorName = formCollection["ColorsName"];
            srVisit.SRVDetail.Status = newSRVisit.SRVDetail.Status == default(int) ? EnumStatus.New : newSRVisit.SRVDetail.Status;
            srVisit.SRVDetail.Quantity = newSRVisit.SRVDetail.Quantity;


            srVisit.SRVDetail.SRVProductDetails = srVisit.SRVDetail.SRVProductDetails ?? new List<SRVProductDetail>();
            for (int i = 1; i <= decimal.Parse(srVisit.SRVDetail.Quantity); i++)
            {
                var srvProductDetail = new SRVProductDetail();
                srvProductDetail.ProductID = int.Parse(GetDefaultIfNull(srVisit.SRVDetail.ProductID));
                srvProductDetail.ColorID = int.Parse(GetDefaultIfNull(srVisit.SRVDetail.ColorID));
                srvProductDetail.IMENO = formCollection["IMEINo" + i.ToString()];

                if (srVisit.SRVDetail.SRVProductDetails.Any(x => x.ProductID == srvProductDetail.ProductID
                    && x.ColorID == srvProductDetail.ColorID && x.IMENO.Equals(srvProductDetail.IMENO))) continue;
                srVisit.SRVDetail.SRVProductDetails.Add(srvProductDetail);
            }

            srVisit.SRVDetails = srVisit.SRVDetails ?? new List<CreateSRVisitDetailViewModel>();
            srVisit.SRVDetails.Add(srVisit.SRVDetail);

            SRVisitViewModel vm = new SRVisitViewModel
            {
                SRVDetail = new CreateSRVisitDetailViewModel(),
                SRVDetails = srVisit.SRVDetails,
                SRVisit = srVisit.SRVisit
            };

            TempData["SRVisitViewModel"] = vm;
            srVisit.SRVDetail = new CreateSRVisitDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private void CheckAndAddModelErrorForSave(SRVisitViewModel newSRVisit, FormCollection formCollection)
        {
            //if (string.IsNullOrEmpty(newSRVisit.PurchaseOrder.GrandTotal) ||
            //    decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.GrandTotal)) <= 0)
            //    ModelState.AddModelError("PurchaseOrder.GrandTotal", "Grand Total is required");

            //if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.TotalAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.TotalAmount)) <= 0)
            //    ModelState.AddModelError("PurchaseOrder.TotalAmount", "Net Total is required");

        }

        private bool SaveOrder(SRVisitViewModel newSRVisit, SRVisitViewModel srVisit, FormCollection formCollection)
        {
            bool Result = false;

            srVisit.SRVisit.VisitDate = formCollection["VisitDate"];
            srVisit.SRVisit.EmployeeID = formCollection["EmployeesId"];

            //removing unchanged previous order
            srVisit.SRVDetails.Where(x => !string.IsNullOrEmpty(x.SRVisitDID) && x.Status == default(int)).ToList()
                .ForEach(x => srVisit.SRVDetails.Remove(x));

            DataTable dtSRVisit = CreateSRVisitDataTable(srVisit);
            DataSet dsSRVisitDetail = CreateSRVDetailDataTable(srVisit);
            DataTable dtSRVisitDetail = dsSRVisitDetail.Tables[0];
            DataTable dtSRVProductDetail = dsSRVisitDetail.Tables[1];
            //DataTable dtStockDetail = dsPurchaseOrderDetail.Tables[2];
            log.Info(new { srVisit.SRVisit, srVisit.SRVDetails });
            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                Result = _srVisitService.UpdateSRVisitChallanUsingSP(int.Parse(srVisit.SRVisit.SRVisitID),
                      dtSRVisit, dtSRVisitDetail, dtSRVProductDetail);
            else
                Result = _srVisitService.AddSRVisitChallanUsingSP(dtSRVisit, dtSRVisitDetail, dtSRVProductDetail);

            if (Result)
                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Order has been failed.", ToastType.Error);
            return Result;
        }

        private DataTable CreateSRVisitDataTable(SRVisitViewModel srVisit)
        {
            DataTable dtSRVisit = new DataTable();
            dtSRVisit.Columns.Add("VisitDate", typeof(DateTime));
            dtSRVisit.Columns.Add("ChallanNo", typeof(string));
            dtSRVisit.Columns.Add("EmpID", typeof(int));
            dtSRVisit.Columns.Add("Status", typeof(int));
            dtSRVisit.Columns.Add("ConcernId", typeof(int));
            dtSRVisit.Columns.Add("CreateDate", typeof(DateTime));
            dtSRVisit.Columns.Add("CreatedBy", typeof(int));
            DataRow row = null;

            row = dtSRVisit.NewRow();
            row["VisitDate"] = srVisit.SRVisit.VisitDate;
            row["ChallanNo"] = srVisit.SRVisit.ChallanNo;
            row["EmpID"] = srVisit.SRVisit.EmployeeID;
            row["Status"] = (int)EnumSRVisitType.Live;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreateDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();

            dtSRVisit.Rows.Add(row);

            return dtSRVisit;
        }

        private DataSet CreateSRVDetailDataTable(SRVisitViewModel srVisit)
        {
            DataSet dsSRVisitDetail = new DataSet();
            DataTable dtSRVisitDetail = new DataTable();
            DataTable dtSRVProductDetail = new DataTable();
            DataRow row = null;
            int id;

            dtSRVisitDetail.Columns.Add("SRVisitDID", typeof(int));
            dtSRVisitDetail.Columns.Add("ProductID", typeof(int));
            dtSRVisitDetail.Columns.Add("ColorID", typeof(int));
            dtSRVisitDetail.Columns.Add("Status", typeof(int));
            dtSRVisitDetail.Columns.Add("Quantity", typeof(decimal));


            //SRVProductDetail
            dtSRVProductDetail.Columns.Add("ProductID", typeof(int));
            dtSRVProductDetail.Columns.Add("ColorID", typeof(int));
            dtSRVProductDetail.Columns.Add("IMEINO", typeof(string));
            dtSRVProductDetail.Columns.Add("StockDetailID", typeof(string));



            foreach (var item in srVisit.SRVDetails)
            {
                row = dtSRVisitDetail.NewRow();
                id = int.Parse(GetDefaultIfNull(item.SRVisitDID));

                if (id > 0)
                    row["SRVisitDID"] = id;
                else
                    row["SRVisitDID"] = DBNull.Value;

                row["ProductId"] = item.ProductID;
                row["ColorId"] = item.ColorID;
                row["Status"] = (int)item.Status;
                row["Quantity"] = item.Quantity;

                //SRVProductDetail
                CreateSRVProductDetailDataTable(item, dtSRVProductDetail);
                dtSRVisitDetail.Rows.Add(row);
            }

            dsSRVisitDetail.Tables.Add(dtSRVisitDetail);
            dsSRVisitDetail.Tables.Add(dtSRVProductDetail);

            return dsSRVisitDetail;
        }

        private void CreateSRVProductDetailDataTable(CreateSRVisitDetailViewModel srvDetail, DataTable dtSRVProductDetail)
        {
            DataRow row = null;
            StockDetail stockDetail = null;
            foreach (var item in srvDetail.SRVProductDetails)
            {
                row = dtSRVProductDetail.NewRow();
                stockDetail = _stockDetailService.GetStockDetail(item.ProductID, item.ColorID, item.IMENO);

                row["ProductID"] = item.ProductID;
                row["ColorID"] = srvDetail.ColorID;
                row["IMEINO"] = item.IMENO.Trim();
                if (stockDetail != null)
                {
                    row["StockDetailID"] = stockDetail.SDetailID;
                }
                else
                    row["StockDetailID"] = DBNull.Value;

                dtSRVProductDetail.Rows.Add(row);
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

        [HttpGet]
        [Authorize]
        public ActionResult SRVisitStatusReport()
        {
            return View("SRVisitStatusReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRVisitReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRWiseCustomerStatusReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult IsIMEIExistInStock(int ProductID, int ColorID, string IMEINO)
        {

            if (_StockService.IsIMEIAvailableForSRVisit(ProductID, ColorID, IMEINO))
            {
                if (_srVisitService.IsIMEIAlreadyIssuedToSR(ProductID, ColorID, IMEINO))
                {
                    return Json(new { Status = false, Message = "IMEI " + IMEINO + " is Already issued." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Status = true, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Message = "IMEI " + IMEINO + " is not avaiable in stock." }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRVisitReturn()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetAllIssuedIMEIBySRID(int EmployeeID)
        {
            var srvisitStock = _srVisitService.GetAllSRVisitStockIMEIBySRID(EmployeeID);
            return Json(srvisitStock, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetStockIMEIBySRIDAndIMEI(int EmployeeID, string IMEI)
        {
            var srvisitStock = _srVisitService.GetAllSRVisitStockIMEIBySRID(EmployeeID).FirstOrDefault(i => i.IMEI.Equals(IMEI.Trim()));
            if (srvisitStock == null)
                return Json(new { Status = false, Message = "IMEI not Found." }, JsonRequestBehavior.AllowGet);

            return Json(new { Status = true, Data = srvisitStock }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult ReturnIMEIByEmployeeID(int EmployeeID, List<AdvancePODetail> RemoveIMEIList)
        {

            DataTable dt = new DataTable();
            DataRow row = null;
            dt.Columns.Add("SRVisitPDID", typeof(int));
            foreach (var item in RemoveIMEIList)
            {
                row = dt.NewRow();
                row["SRVisitPDID"] = item.ID;
                dt.Rows.Add(row);
            }

            if (RemoveIMEIList.Count() > 0)
            {
                if (_srVisitService.ReturnSRVisitUsingSP(dt, EmployeeID))
                {
                    AddToastMessage("SR Visit", "Return Successful.", ToastType.Success);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }


            return Json(false, JsonRequestBehavior.AllowGet);

        }


    }
}