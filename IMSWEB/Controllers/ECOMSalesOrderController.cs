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
using Microsoft.AspNet.Identity;
using System.Data;
using IMSWEB.Report;
using log4net;
using System.Text;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("sales-order")]
    public class ECOMSalesOrderController : CoreController
    {
        ISalesOrderService _salesOrderService;
        ISalesOrderDetailService _salesOrderDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        ITransactionalReport _transactionalReportService;
        IMiscellaneousService<SOrder> _miscellaneousService;
        IProductService _productService;
        IMapper _mapper;
        ISisterConcernService _SisterConcern;
        ISRVisitService _SRVisitService;
        IUserService _UserService;
        ISystemInformationService _SysInfoService;
        ISMSStatusService _SMSStatusService;
        ICardTypeSetupService _CardTypeSetupService;
        ICardTypeService _CardTypeService;
        private readonly IBankTransactionService _bankTransactionService;
        private readonly ISessionMasterService _sessionMasterService;
        private readonly IUserAuditDetailService _userAuditDetailService;
        private readonly IRoleService _roleService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ECOMSalesOrderController(IErrorService errorService,
            ISalesOrderService salesOrderService, ISalesOrderDetailService salesOrderDetailService,
            IStockService stockService, IStockDetailService stockDetailService,
            ICustomerService customerService, IEmployeeService employeeService,
            ITransactionalReport transactionalReportService,
            IMiscellaneousService<SOrder> miscellaneousService, IMapper mapper,
            ISRVisitService SRVisitService, IUserService UserService,
            IProductService productService, ISisterConcernService sisterConcern,
            ISystemInformationService SysInfoService, ISMSStatusService SMSStatusService, IUserAuditDetailService userAuditDetailService, ICardTypeSetupService CardTypeSetupService, ICardTypeService CardTypeService, IBankTransactionService bankTransactionService, ISessionMasterService sessionMasterService, IRoleService roleService, ISMSBillPaymentBkashService sMSBillPaymentBkashService)
           : base(errorService, SysInfoService)
        {
            _salesOrderService = salesOrderService;
            _salesOrderDetailService = salesOrderDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _customerService = customerService;
            _employeeService = employeeService;
            _transactionalReportService = transactionalReportService;
            _miscellaneousService = miscellaneousService;
            _productService = productService;
            _mapper = mapper;
            _SisterConcern = sisterConcern;
            _SRVisitService = SRVisitService;
            _UserService = UserService;
            _SysInfoService = SysInfoService;
            _SMSStatusService = SMSStatusService;
            _CardTypeSetupService = CardTypeSetupService;
            _CardTypeService = CardTypeService;
            _bankTransactionService = bankTransactionService;
            _userAuditDetailService = userAuditDetailService;
            _sessionMasterService = sessionMasterService;
            _roleService = roleService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            ViewBag.IsEditReqPermission = _SysInfoService.IsEditReqPermission();
            ViewBag.IsEditReqPermissionFalse = _SysInfoService.IsEditReqPermissionFalse();
            ViewBag.IsEcomputerShow = _SysInfoService.IsEcomputerShow();
            int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            TempData["salesOrderViewModel"] = null;
            //if(userId==1014 || userId==1015 || userId==1016 || userId==1017|| userId==1018)
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var customSO = _salesOrderService.GetAllSalesOrderAsyncByUserID(userId, DateRange.Item1, DateRange.Item2, EnumSalesType.Sales);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View(vmSO);
            }
            else
            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Pending);
                var customSO = _salesOrderService.GetAllSalesOrderAsync(DateRange.Item1, DateRange.Item2, status, IsVATManager(), User.Identity.GetConcernId());
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int, decimal>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View(vmSO);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            ViewBag.IsEditReqPermission = _SysInfoService.IsEditReqPermission();
            ViewBag.IsEditReqPermissionFalse = _SysInfoService.IsEditReqPermissionFalse();
            ViewBag.IsEcomputerShow = _SysInfoService.IsEcomputerShow();
            TempData["salesOrderViewModel"] = null;
            string InvoiceNo = string.Empty, ContactNo = "", CustomerName = "", AccountNo = "";
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                toDate = Convert.ToDateTime(formCollection["ToDate"]);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            if (!string.IsNullOrEmpty(formCollection["InvoiceNo"]))
                InvoiceNo = formCollection["InvoiceNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["ContactNo"]))
                ContactNo = formCollection["ContactNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["CustomerName"]))
                CustomerName = formCollection["CustomerName"].Trim();

            if (!string.IsNullOrEmpty(formCollection["AccountNo"]))
                AccountNo = formCollection["AccountNo"].Trim();

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                var customSO = _salesOrderService.GetAllSalesOrderAsyncByUserID(userId, ViewBag.FromDate, ViewBag.ToDate, EnumSalesType.Sales,
                    InvoiceNo, ContactNo, CustomerName, AccountNo);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View("Index", vmSO);
            }
            else

            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Pending);

                var customSO = _salesOrderService.GetAllSalesOrderAsync(fromDate, toDate,
                    status, IsVATManager(), User.Identity.GetConcernId(), InvoiceNo, ContactNo, CustomerName, AccountNo);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int, decimal>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View("Index", vmSO);
            }
        }
        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {

            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            ViewBag.TramsAndCondition = sysInfo != null ? sysInfo.TramsAndCondition ? true : false : false;
            return ReturnCreateViewWithTempData();
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleSalesOrder(newSalesOrder, formCollection);
        }

        //[HttpGet]
        //[Authorize]
        //[Route("edit/{orderId}")]
        //public ActionResult Edit(int orderId, string previousAction)
        //{

        //    var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
        //    ViewBag.TramsAndCondition = sysInfo != null ? sysInfo.TramsAndCondition ? true : false : false;

        //    //bool Result = _customerService.IsCustomerSalesOrCollectionExists(orderId);
        //    bool Result = _salesOrderService.IsSoReturn(orderId);
        //    if (Result == true)
        //    {
        //        AddToastMessage("", "Edit Not Possible Return. Found.", ToastType.Error);
        //        return RedirectToAction("Index");
        //    }
        //    var Sales = _salesOrderService.GetSalesOrderById(orderId);
        //    if (Sales.SOrderDetails.Any())
        //    {
        //        if (Sales.SOrderDetails.Any(d => d.RepOrderID.HasValue))
        //        {
        //            AddToastMessage("", "Replacement found! Can't Edit.", ToastType.Error);
        //            return RedirectToAction("Index");
        //        }


        //        //if (Sales.SOrderDetails.Sum(d => d.RQuantity) > 0)
        //        //{
        //        //    AddToastMessage("", "Return found! Can't delete.", ToastType.Error);
        //        //    return RedirectToAction("Index");
        //        //}
        //    }



        //    if (TempData["salesOrderViewModel"] == null || string.IsNullOrEmpty(previousAction))
        //    {
        //        var salesOrder = _salesOrderService.GetSalesOrderById(orderId);
        //        if (!IsDateValid(salesOrder.InvoiceDate))
        //            return RedirectToAction("Index");
        //        var CardSetup = _CardTypeSetupService.GetById(salesOrder.CardTypeSetupID);
        //        var soDetails = _salesOrderDetailService.GetSalesOrderDetailByOrderId(orderId);

        //        var vmSalesOrder = _mapper.Map<SOrder, CreateSalesOrderViewModel>(salesOrder);
        //        var vmSoDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, int, string, string, string,
        //    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal>>>>, IEnumerable<CreateSalesOrderDetailViewModel>>(soDetails).ToList();
        //        log.Info(new { vmSalesOrder, vmSoDetails });

        //        var gDetails = (from d in vmSoDetails
        //                        group d by new
        //                        {
        //                            d.ProductId,
        //                            d.ProductCode,
        //                            d.ProductName,
        //                            d.ColorName,
        //                            d.ColorId,
        //                            d.IMENo,
        //                            d.GodownID
        //                        } into g
        //                        select new CreateSalesOrderDetailViewModel
        //                        {
        //                            ProductId = g.Key.ProductId,
        //                            ProductCode = g.Key.ProductCode,
        //                            ProductName = g.Key.ProductName,
        //                            ColorId = g.Key.ColorId,
        //                            ColorName = g.Key.ColorName,
        //                            GodownID = g.Key.GodownID,
        //                            IMENo = g.Key.IMENo,
        //                            UnitPrice = g.Select(i => i.UnitPrice).FirstOrDefault(),
        //                            MRPRate = g.Select(i => i.MRPRate).FirstOrDefault(),
        //                            UTAmount = g.Select(i => i.UTAmount).FirstOrDefault(),
        //                            SODetailId = g.Select(i => i.SODetailId).FirstOrDefault(),
        //                            SalesOrderId = g.Select(i => i.SalesOrderId).FirstOrDefault(),
        //                            StockDetailId = g.Select(i => i.StockDetailId).FirstOrDefault(),
        //                            PPDAmount = g.Select(i => i.PPDAmount).FirstOrDefault(),
        //                            PPDPercentage = g.Select(i => i.PPDPercentage).FirstOrDefault(),
        //                            PPOffer = g.Select(i => i.PPOffer).FirstOrDefault(),
        //                            Quantity = g.Sum(i => decimal.Parse(i.Quantity)).ToString(),
        //                        }).ToList();
        //        var vm = new SalesOrderViewModel
        //        {
        //            SODetail = new CreateSalesOrderDetailViewModel(),
        //            SODetails = gDetails,
        //            SalesOrder = vmSalesOrder
        //        };
        //        if (CardSetup != null)
        //        {
        //            vm.SalesOrder.BankID = CardSetup.BankID;
        //            vm.SalesOrder.CardTypeID = CardSetup.CardTypeID;
        //            vm.SalesOrder.CardTypes = _CardTypeService.GetAll().Where(i => i.CardTypeID == CardSetup.CardTypeID).ToList();
        //        }
        //        TempData["salesOrderViewModel"] = vm;
        //        return View("Create", vm);
        //    }
        //    else
        //    {
        //        return ReturnCreateViewWithTempData();
        //    }
        //}

        //[HttpPost]
        //[Authorize]
        //[Route("edit/returnUrl")]
        //public ActionResult Edit(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        //{
        //    return HandleSalesOrder(newSalesOrder, formCollection);
        //}


        [HttpGet]
        [Authorize]
        [Route("edit/{orderId}")]
        public ActionResult Edit(int orderId, string previousAction)
        {
            //ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            //ViewBag.IsShowNetProfit = _SysInfoService.IsNetProfitButtonEnabled();
            var Sales = _salesOrderService.GetSalesOrderById(orderId);
            if (Sales.SOrderDetails.Any())
            {
                if (Sales.SOrderDetails.Any(d => d.RepOrderID.HasValue))
                {
                    AddToastMessage("", "Replacement found! Can't Edit.", ToastType.Error);
                    return RedirectToAction("Index");
                }


                if (Sales.SOrderDetails.Sum(d => d.RQuantity) > 0)
                {
                    AddToastMessage("", "Return found! Can't Edit.", ToastType.Error);
                    return RedirectToAction("Index");
                }
            }

            if (TempData["salesOrderViewModel"] == null || string.IsNullOrEmpty(previousAction))
            {
                var salesOrder = _salesOrderService.GetSalesOrderById(orderId);
                var vmSalesOrder = _mapper.Map<SOrder, CreateSalesOrderViewModel>(salesOrder);
                var vmSoDetails = GetSODetailsVM(orderId.ToString());

                //#region get all offer
                //List<SaleWithOffer> allSalesOffer = _saleOfferService.GetAllSaleOfferBySalesOrder(orderId);
                //if (allSalesOffer.Any())
                //{
                //    foreach (var soDetail in vmSoDetails)
                //    {
                //        SaleWithOffer saleWithOffer = allSalesOffer.Where(sf => sf.ProductId.ToString().Equals(soDetail.ProductId)).FirstOrDefault();
                //        soDetail.OfferId = saleWithOffer != null ? saleWithOffer.SaleOfferId : 0;
                //    }
                //}
                //#endregion

                //var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                //ViewBag.minIMEIForSearch = sysInfo != null ? sysInfo.MinimumIMEIForSearch : 5;

                log.Info(new { vmSalesOrder, vmSoDetails });

                var vm = new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = vmSoDetails,
                    SalesOrder = vmSalesOrder
                };

                TempData["salesOrderViewModel"] = vm;
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
        public ActionResult Edit(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {


            return HandleSalesOrder(newSalesOrder, formCollection);
        }


        [HttpGet]
        [Authorize]
        [Route("delete/{orderId}")]
        public ActionResult Delete(int orderId)
        {
            var Sales = _salesOrderService.GetSalesOrderById(orderId);
            if (!IsDateValid(Sales.InvoiceDate))
            {
                return RedirectToAction("Index");
            }

            if (Sales.SOrderDetails.Any())
            {
                if (Sales.SOrderDetails.Any(d => d.RepOrderID.HasValue))
                {
                    AddToastMessage("", "Replacement found! Can't delete.", ToastType.Error);
                    return RedirectToAction("Index");
                }


                if (Sales.SOrderDetails.Sum(d => d.RQuantity) > 0)
                {
                    AddToastMessage("", "Return found! Can't delete.", ToastType.Error);
                    return RedirectToAction("Index");
                }
            }

            log.Info(new { SOrderID = orderId });

            if (_salesOrderService.DeleteSalesOrderUsingSP(orderId, User.Identity.GetUserId<int>()))
            {
                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = orderId;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.Sales;
                useraudit.ActionType = EnumActionType.Delete;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();
                AddToastMessage("", "Item has been deleted successfully", ToastType.Success);
            }

            else
                AddToastMessage("", "Item delete failed.", ToastType.Error);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("editfromview/{id}/{detailId}")]
        public ActionResult EditFromView(int id, int detailId, string previousAction)
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSalesOrderDetailViewModel itemToEdit =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToEdit != null)
            {
                UpdateSOrderFromView(salesOrder, itemToEdit);
                ProductDetailsModel editIMEI = new ProductDetailsModel();
                editIMEI.ProductId = Convert.ToInt32(itemToEdit.ProductId);
                editIMEI.ProductName = itemToEdit.ProductName;
                editIMEI.ProductCode = itemToEdit.ProductCode;
                editIMEI.IMENo = itemToEdit.IMENo;
                salesOrder.IMEIList = salesOrder.IMEIList ?? new List<ProductDetailsModel>();
                salesOrder.IMEIList.Add(editIMEI);
                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToEdit.SODetailId))
                {
                    itemToEdit.Status = EnumStatus.Deleted;
                    //int sorderDetailId = int.Parse(itemToEdit.SODetailId);
                    //int userId = User.Identity.GetUserId<int>();
                    //_salesOrderService.DeleteSalesOrderDetailUsingSP(sorderDetailId, userId);
                }
                else
                {
                    salesOrder.SODetails.Remove(itemToEdit);
                }

                salesOrder.SODetail = itemToEdit;
                TempData["salesOrderViewModel"] = salesOrder;

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

        private void UpdateSOrderFromView(SalesOrderViewModel salesOrder, CreateSalesOrderDetailViewModel itemToEdit)
        {
            decimal TotalPPDisAmt = decimal.Parse(GetDefaultIfNull(itemToEdit.PPDAmount)) * decimal.Parse(GetDefaultIfNull(itemToEdit.Quantity));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)) + TotalPPDisAmt)).ToString();

            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                TotalPPDisAmt).ToString();

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                TotalPPDisAmt - decimal.Parse(GetDefaultIfNull(itemToEdit.PPOffer))).ToString();

            salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

            salesOrder.SalesOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount))) -
                (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

            //For Total Offer Calculation
            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer)) -
            decimal.Parse(GetDefaultIfNull(itemToEdit.PPOffer))).ToString();


        }

        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId, string previousAction)
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSalesOrderDetailViewModel itemToDelete =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToDelete != null)
            {
                UpdateSOrderFromView(salesOrder, itemToDelete);

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToDelete.SODetailId))
                {
                    itemToDelete.Status = EnumStatus.Deleted;
                    //int sorderDetailId = int.Parse(itemToDelete.SODetailId);
                    //int userId = User.Identity.GetUserId<int>();
                    //_salesOrderService.DeleteSalesOrderDetailUsingSP(sorderDetailId, userId);
                }
                else
                {
                    salesOrder.SODetails.Remove(itemToDelete);
                }

                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
                TempData["salesOrderViewModel"] = salesOrder;
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



        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newSalesOrder, FormCollection formCollection)
        {
            //bool IsEmployeeWiseTransactionEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();

            if (formCollection["OrderDate"].IsNullOrEmpty())
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (formCollection["CustomersId"].IsNullOrEmpty())
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
                newSalesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            if (newSalesOrder.SalesOrder.InvoiceNo.IsNullOrEmpty())
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            }

            if (newSalesOrder.SalesOrder.RecieveAmount.IsNullOrEmpty())
                newSalesOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount);

            //var existingInvoiceNo = _miscellaneousService.GetDuplicateEntry(s => s.InvoiceNo == newSalesOrder.SalesOrder.InvoiceNo);
            //if (existingInvoiceNo != null)
            //{
            //    ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No Already Exists");
            //    AddToastMessage("", "A Sales Order with same invoice no already exists in the system. Please try with a different invoice no.", ToastType.Error);
            //}

            //if (IsEmployeeWiseTransactionEnable && !User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            //{
            //    if (formCollection["EmployeesId"].IsNullOrEmpty())
            //    {
            //        ModelState.AddModelError("SalesOrder.EmployeeID", "Employee is required");
            //        return;
            //    }
            //    newSalesOrder.SalesOrder.EmployeeID = formCollection["EmployeesId"];
            //}
            //else if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            //{
            //    int SRUID = User.Identity.GetUserId<int>();
            //    var user = _UserService.GetUserById(SRUID);
            //    newSalesOrder.SalesOrder.EmployeeID = user.EmployeeID.ToString();
            //}
            //else
            //    newSalesOrder.SalesOrder.EmployeeID = "0";

            ProductDetailsModel oDetail = null;
            int counter = 0;
            if (newSalesOrder.SODetails.Count() <= 0)
            {
                ModelState.AddModelError($"SODetails[{counter - 1}].IMENo", "IMENo/Barcode is required");
                AddToastMessage(message: "IMEI/Barcode is required", toastType: ToastType.Error);
                return;
            }

            List<CreateSalesOrderDetailViewModel> vmOldSoDetails = null;
            if (newSalesOrder.SalesOrder.SalesOrderId.IsNotNullOrEmpty())
                vmOldSoDetails = GetSODetailsVM(newSalesOrder.SalesOrder.SalesOrderId.ToString());

            foreach (var SODetail in newSalesOrder.SODetails)
            {
                counter++;
                #region Nobarcode validation check
                if (SODetail.ProductType == (int)EnumProductType.NoBarcode)
                {
                    var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(SODetail.ProductId)));

                    int SDetailID = int.Parse(GetDefaultIfNull(SODetail.StockDetailId));
                    //  int ColorID = int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"]));
                    var stockDeatilCount = _stockDetailService.GetById(SDetailID);// _stockService.GET.GetStockByProductIdandColorIDandGodownID(product.ProductID, 1, 1);
                    SODetail.GodownID = stockDeatilCount.GodownID;
                    SODetail.ColorId = stockDeatilCount.ColorID.ToString();

                    CreateSalesOrderDetailViewModel PreQty = null;
                    if (decimal.Parse(GetDefaultIfNull(SODetail.SODetailId)) > 0)
                    {
                        SODetail.Status = EnumStatus.New; //any thing could be change like qty, price,ppdiscount and ppOffer, that's why we insert as new. The old entry will be deleted
                        PreQty = vmOldSoDetails.FirstOrDefault(i => i.SODetailId.Equals(SODetail.SODetailId));
                    }
                    else
                    {
                        SODetail.Status = EnumStatus.New;
                    }

                    decimal StockQty = 0m;

                    var StockCount = _stockService.GetStockById(stockDeatilCount.StockID);
                    StockQty = StockCount.Quantity;
                    if (PreQty != null)
                        StockQty += Convert.ToDecimal(PreQty.Quantity);

                    if (StockQty < decimal.Parse(SODetail.Quantity))
                    {
                        string msg = $"[{counter}]: Stock is not available of the product: {product.ProductName}.Stock Quantity: {StockQty}";
                        ModelState.AddModelError($"SODetails[{counter - 1}].Quantity", msg);
                        AddToastMessage(message: msg, toastType: ToastType.Error);
                        return;
                    }



                    if (decimal.Parse(SODetail.Quantity) <= 0)
                    {
                        ModelState.AddModelError($"SODetails[{counter - 1}].Quantity", $"Please enter sales quantity");
                        AddToastMessage(message: $"Please enter sales quantity", toastType: ToastType.Error);
                        return;
                    }

                    if (decimal.Parse(SODetail.UnitPrice) <= 0)
                    {
                        ModelState.AddModelError($"SODetails[{counter - 1}].UnitPrice", $"Please enter sales rate");
                        AddToastMessage(message: $"Please enter sales rate", toastType: ToastType.Error);
                        return;
                    }
                }
                #endregion
                #region barcode products validation check
                else
                {
                    if (decimal.Parse(GetDefaultIfNull(SODetail.SODetailId)) > 0)
                    {
                        SODetail.Status = EnumStatus.Updated;
                        oDetail = _stockService.GetIMEIDetails(SODetail.IMENo, false);
                    }
                    else
                    {
                        SODetail.Status = EnumStatus.New;

                        oDetail = _stockService.GetIMEIDetails(SODetail.IMENo, true);
                        if (oDetail == null)
                        {
                            ModelState.AddModelError($"SODetails[{counter - 1}].IMENo", "This IMEI is not available in the stock");
                            AddToastMessage(message: $"IMEI is not available in the stock", toastType: ToastType.Error);
                            return;
                        }
                    }

                    if (decimal.Parse(SODetail.UnitPrice) <= 0)
                    {
                        ModelState.AddModelError($"SODetails[{counter - 1}].UnitPrice", $"Please enter sales rate");
                        AddToastMessage(message: $"Please enter sales rate", toastType: ToastType.Error);
                        return;
                    }



                    oDetail = new ProductDetailsModel();
                }
                #endregion
            }

            //deleted products added in list with delete status
            var barcodeNewProducts = newSalesOrder.SODetails.Where(i => i.ProductType != (int)EnumProductType.NoBarcode)
                                      .Select(x => new
                                      {
                                          x.SODetailId
                                      });


            if (vmOldSoDetails != null)
            {
                foreach (var itemOld in vmOldSoDetails.Where(i => i.ProductType != (int)EnumProductType.NoBarcode))
                {
                    if (!barcodeNewProducts.Any(i => GetDefaultIfNull(i.SODetailId)
                                .Equals(itemOld.SODetailId)))
                    {
                        itemOld.Status = EnumStatus.Deleted;
                        newSalesOrder.SODetails.Add(itemOld);
                    }
                }

                foreach (var itemOld in vmOldSoDetails.Where(i => i.ProductType == (int)EnumProductType.NoBarcode))
                {

                    itemOld.Status = EnumStatus.Deleted;
                    newSalesOrder.SODetails.Add(itemOld);
                }
            }


            //SetEmployeeID(newSalesOrder, formCollection);
        }

        //private List<CreateSalesOrderDetailViewModel> GetSODetailsVM(string SalesOrderId)
        //{
        //    var soDetails = _salesOrderDetailService.GetSalesOrderDetailByOrderId(Convert.ToInt32(SalesOrderId));
        //    var vmSoDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, int, string, string, string,
        //        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal>>>>, IEnumerable<CreateSalesOrderDetailViewModel>>(soDetails).ToList();
        //    return vmSoDetails;
        //}
        private List<CreateSalesOrderDetailViewModel> GetSODetailsVM(string SalesOrderId)
        {
            var soDetails = _salesOrderDetailService.GetSalesOrderDetailByOrderId(Convert.ToInt32(SalesOrderId));
            var vmSoDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, int, string, string, string,
            Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>>, IEnumerable<CreateSalesOrderDetailViewModel>>(soDetails).ToList();
            return vmSoDetails;
        }


        //private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newSalesOrder, SalesOrderViewModel salesOrder,
        //     FormCollection formCollection)
        //{
        //    if (formCollection["OrderDate"].IsNullOrEmpty())
        //        ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

        //    if (formCollection["CustomersId"].IsNullOrEmpty())
        //        ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
        //    else
        //        newSalesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

        //    if (newSalesOrder.SalesOrder.InvoiceNo.IsNullOrEmpty())
        //        ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

        //    if (newSalesOrder.SalesOrder.RecieveAmount.IsNullOrEmpty())
        //        newSalesOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount);

        //    if (string.IsNullOrEmpty(newSalesOrder.SODetail.Quantity) || Convert.ToInt32(double.Parse(newSalesOrder.SODetail.Quantity)) <= 0)
        //    {
        //        ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
        //    }

        //    if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InvoiceNo))
        //        ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

        //    if (string.IsNullOrEmpty(newSalesOrder.SODetail.MRPRate))
        //        ModelState.AddModelError("SODetail.MRPRate", "Purchase Rate is required");

        //    if (string.IsNullOrEmpty(newSalesOrder.SODetail.UnitPrice))
        //        ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");

        //    if (string.IsNullOrEmpty(newSalesOrder.SODetail.IMENo))
        //    {
        //        ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
        //    }
        //    else
        //    {
        //        var stockDetails = _stockDetailService.GetStockDetailByProductId(
        //            int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

        //        if (stockDetails.Count() < int.Parse(newSalesOrder.SODetail.Quantity))
        //        {
        //            ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockDetails.Count());
        //        }
        //        if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
        //            ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

        //        var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

        //        int SDetailID = int.Parse(GetDefaultIfNull(formCollection["StockDetailsId"]));
        //        //  int ColorID = int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"]));
        //        var stockDeatilCount = _stockDetailService.GetById(SDetailID);// _stockService.GET.GetStockByProductIdandColorIDandGodownID(product.ProductID, 1, 1);
        //        newSalesOrder.SODetail.GodownID = stockDeatilCount.GodownID;

        //        if (product.ProductType == (int)EnumProductType.NoBarcode)
        //        {
        //            CreateSalesOrderDetailViewModel PreQty = null;
        //            List<CreateSalesOrderDetailViewModel> preList = new List<CreateSalesOrderDetailViewModel>();
        //            decimal StockQty = 0m;
        //            if (salesOrder.SODetails != null)
        //            {
        //                PreQty = salesOrder.SODetails
        //                             .FirstOrDefault(i => i.ProductId.Equals(newSalesOrder.SODetail.ProductId)
        //                                && i.ColorId.Equals(formCollection["ColorsId"])
        //                                && i.GodownID == newSalesOrder.SODetail.GodownID);
        //                preList = salesOrder.SODetails
        //                             .Where(i => i.ProductId.Equals(newSalesOrder.SODetail.ProductId)
        //                                && i.ColorId.Equals(formCollection["ColorsId"])
        //                                && i.GodownID == newSalesOrder.SODetail.GodownID).ToList();
        //            }

        //            var StockCount = _stockService.GetStockById(stockDeatilCount.StockID);


        //            decimal addedQty = 0m;
        //            if (preList.Any())
        //            {
        //                foreach (var prevDetail in preList)
        //                {
        //                    addedQty += Convert.ToDecimal(prevDetail.Quantity);
        //                }

        //                StockQty = StockCount.Quantity;
        //                StockQty += addedQty;

        //                addedQty += decimal.Parse(newSalesOrder.SODetail.Quantity);

        //                if (StockQty < addedQty)
        //                    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity in total: " + StockQty);
        //            }
        //            else
        //            {
        //                StockQty = StockCount.Quantity;
        //                //if (PreQty != null)
        //                //    StockQty += Convert.ToDecimal(PreQty.Quantity);


        //                if (StockQty < decimal.Parse(newSalesOrder.SODetail.Quantity))
        //                    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + StockQty);
        //            }


        //            //if (StockQty < decimal.Parse(newSalesOrder.SODetail.Quantity))
        //            //    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + StockQty);

        //        }
        //        else
        //        {
        //            string[] IMEIS = formCollection.AllKeys
        //               .Where(key => key.StartsWith("IMEI"))
        //               .Select(key => formCollection[key])
        //               .ToArray();

        //            if (IMEIS.Length <= 0)
        //            {
        //                ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
        //            }
        //            else
        //            {
        //                ProductDetailsModel oDetail = null;
        //                salesOrder.IMEIList = new List<ProductDetailsModel>();
        //                decimal SaleRate = Convert.ToDecimal(newSalesOrder.SODetail.UTAmount);
        //                string StockDetailID = string.Empty;
        //                for (int i = 0; i < IMEIS.Length; i++)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(salesOrder.SalesOrder.SalesOrderId))
        //                        oDetail = _stockService.GetIMEIDetail(IMEIS[i]);
        //                    else
        //                        oDetail = _stockService.GetStockIMEIDetail(IMEIS[i]);

        //                    if (oDetail != null)
        //                    {
        //                        StockDetailID = oDetail.StockDetailsId.ToString();
        //                        salesOrder.IMEIList.Add(oDetail);
        //                        newSalesOrder.SODetail.ProductId = oDetail.ProductId.ToString();
        //                        //if (SaleRate < oDetail.PRate)
        //                        //    ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate can't be less than DP Rate.");

        //                        if (salesOrder.SODetails != null)
        //                        {
        //                            if (salesOrder.SODetails.Any(j => j.StockDetailId.Equals(StockDetailID) && j.Status != EnumStatus.Deleted))
        //                            {
        //                                ModelState.AddModelError("SODetail.IMENo", "This IMEI is already added.");
        //                                AddToastMessage("", oDetail.IMENo + " is already added.", ToastType.Error);
        //                            }
        //                        }
        //                    }
        //                    oDetail = new ProductDetailsModel();
        //                }
        //            }

        //            var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

        //            if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
        //                ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

        //            var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

        //            if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
        //                ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

        //            int SDetailId = Convert.ToInt32(formCollection["StockDetailsId"]);
        //            var PendingIMEI = _salesOrderService.IsIMEIInPendingSales(SDetailId, Convert.ToInt32(GetDefaultIfNull(salesOrder.SalesOrder.SalesOrderId)));
        //            if (PendingIMEI.Item1)
        //                ModelState.AddModelError("SODetail.IMENo", "This IMEI is in pending sales Invoice No:" + PendingIMEI.Item2);
        //        }
        //    }
        //}

        private void CheckAndAddModelErrorForSave(SalesOrderViewModel newSalesOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("SalesOrder.GrandTotal", "Grand Total is required");

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.TotalAmount) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)) <= 0)
                ModelState.AddModelError("SalesOrder.TotalAmount", "Net Total is required");

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.RecieveAmount", "Pay Amount is required");
            if (newSalesOrder.SalesOrder.RecieveAmount == null || newSalesOrder.SalesOrder.RecieveAmount == "")
            {
                newSalesOrder.SalesOrder.RecieveAmount = "0";
                salesOrder.SalesOrder.RecieveAmount = "0";
            }
            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
                salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            #region Customer and Employee Due Limit Check
            Customer customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
            Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);
            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            if (sysInfo != null)
            {
                if (sysInfo.CustomerDueLimitApply == 1)
                {
                    if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
                        ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");
                }
                if (sysInfo.EmployeeDueLimitApply == 1)
                {
                    if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
                        ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");
                }
            }

            #endregion

            var distinctIMEI = salesOrder.SODetails
                                .GroupBy(i => new { i.StockDetailId, i.Status })
                                .Select(g => g.First())
                                .ToList();

            if (distinctIMEI.Count() != salesOrder.SODetails.Count())
            {
                ModelState.AddModelError("SODetail.IMENo", "");
                AddToastMessage("", "Duplicate IMEI added.", ToastType.Error);
            }
            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];

            if (!IsDateValid(Convert.ToDateTime(salesOrder.SalesOrder.OrderDate)))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            }

            if (newSalesOrder.SalesOrder.CardPaidAmount > 0m)
            {
                if (string.IsNullOrEmpty(formCollection["BanksId"]))
                    ModelState.AddModelError("SalesOrder.BankID", "Bank is required.");
                else
                    newSalesOrder.SalesOrder.BankID = Convert.ToInt32(formCollection["BanksId"]);
            }

            if (!string.IsNullOrEmpty(formCollection["ClientDateTime"]))
                newSalesOrder.SalesOrder.CreateDate = Convert.ToDateTime(formCollection["ClientDateTime"]);
            else
                newSalesOrder.SalesOrder.CreateDate = GetLocalDateTime();

        }

        private void AddToOrder(SalesOrderViewModel newSalesOrder,
            SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            decimal quantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity));
            decimal totalDisAmount = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount));
            decimal totalOffer = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.UTAmount)) + totalDisAmount + totalOffer).ToString();

            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PPDiscountAmount)) + totalDisAmount).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            decimal flatDiscountPercent = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage));
            salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            salesOrder.SalesOrder.TempFlatDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount; //(decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) * flatDiscountPercent / 100m).ToString();

            salesOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATPercentage)).ToString();
            salesOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)).ToString();

            salesOrder.SalesOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.AdjAmount)).ToString();

            //salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer))).ToString();
            // decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount)) +

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount))
                + totalDisAmount + totalOffer + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount))).ToString();

            var netTotal = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount))) -
                (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount))));


            //if (!formCollection["CustomerType"].Equals(EnumCustomerType.Dealer.ToString()))
            //{
            //    newSalesOrder.SalesOrder.RecieveAmount = netTotal.ToString();
            //    newSalesOrder.SalesOrder.CashPaidAmount = netTotal;
            //}


            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalOffer)) + totalOffer).ToString();

            salesOrder.SalesOrder.TotalAmount = netTotal.ToString();
            salesOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount))).ToString();
            salesOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount);
            salesOrder.SalesOrder.CashPaidAmount = newSalesOrder.SalesOrder.CashPaidAmount;

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.RemindDate = formCollection["RemindDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();

            var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));
            if (product.ProductType == (int)EnumProductType.NoBarcode)
            {
                salesOrder.SODetail.SODetailId = newSalesOrder.SODetail.SODetailId;
                salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
                salesOrder.SODetail.StockDetailId = formCollection["StockDetailsId"];
                salesOrder.SODetail.ColorId = formCollection["ColorsId"];
                salesOrder.SODetail.GodownID = newSalesOrder.SODetail.GodownID;
                salesOrder.SODetail.ColorName = newSalesOrder.SODetail.ColorName;
                salesOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
                salesOrder.SODetail.IMENo = newSalesOrder.SODetail.IMENo;
                salesOrder.SODetail.Quantity = newSalesOrder.SODetail.Quantity;
                salesOrder.SODetail.PPDPercentage = newSalesOrder.SODetail.PPDPercentage;
                salesOrder.SODetail.PPDAmount = newSalesOrder.SODetail.PPDAmount;
                salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
                salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
                salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
                salesOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
                salesOrder.SODetail.Status = newSalesOrder.SODetail.Status == default(int) ? EnumStatus.New : newSalesOrder.SODetail.Status;
                salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
                salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
                salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
                salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
                salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
                salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;

                salesOrder.SODetails.Add(salesOrder.SODetail);
            }
            else
            {
                newSalesOrder.SODetail.UTAmount = (Convert.ToDecimal(newSalesOrder.SODetail.UTAmount) / Convert.ToDecimal(salesOrder.IMEIList.Count())).ToString();
                foreach (var item in salesOrder.IMEIList)
                {
                    salesOrder.SODetail.SODetailId = newSalesOrder.SODetail.SODetailId;
                    salesOrder.SODetail.ProductId = item.ProductId.ToString();
                    salesOrder.SODetail.StockDetailId = item.StockDetailsId.ToString();
                    salesOrder.SODetail.ColorId = item.ColorId.ToString();
                    salesOrder.SODetail.GodownID = item.GodownID;
                    salesOrder.SODetail.ColorName = item.ColorName.ToString();
                    salesOrder.SODetail.ProductCode = item.ProductCode;
                    salesOrder.SODetail.IMENo = item.IMENo;
                    salesOrder.SODetail.Quantity = "1";
                    salesOrder.SODetail.PPDPercentage = newSalesOrder.SODetail.PPDPercentage;
                    //salesOrder.SODetail.PPDAmount = newSalesOrder.SODetail.PPDAmount;
                    salesOrder.SODetail.PPDAmount = newSalesOrder.SODetail.PPDAmount;
                    salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
                    salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
                    salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
                    salesOrder.SODetail.ProductName = item.ProductName;
                    salesOrder.SODetail.Status = newSalesOrder.SODetail.Status == default(int) ? EnumStatus.New : newSalesOrder.SODetail.Status;
                    //salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
                    salesOrder.SODetail.PPOffer = totalOffer.ToString();
                    salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
                    salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
                    salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
                    salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
                    salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;

                    salesOrder.SODetails.Add(salesOrder.SODetail);
                    salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
                }

            }

            salesOrder.IMEIList = new List<ProductDetailsModel>();

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["salesOrderViewModel"] = vm;

            salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }


        private bool SaveOrder(
            SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            bool Result = false;
            DateTime RemindDate = DateTime.MinValue;
            var Customer = _customerService.GetCustomerById(Convert.ToInt32(salesOrder.SalesOrder.CustomerId));
            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            salesOrder.SalesOrder.PrevDue = Customer.TotalDue;
            //salesOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newSalesOrder.SalesOrder.NetDiscount);
            //salesOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)).ToString();
            //salesOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)).ToString();

            //salesOrder.SalesOrder.TotalDiscountPercentage = newSalesOrder.SalesOrder.TotalDiscountPercentage;
            //salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            //salesOrder.SalesOrder.RecieveAmount = newSalesOrder.SalesOrder.RecieveAmount;
            //salesOrder.SalesOrder.VATPercentage = newSalesOrder.SalesOrder.VATPercentage;
            //salesOrder.SalesOrder.VATAmount = newSalesOrder.SalesOrder.VATAmount;
            //salesOrder.SalesOrder.AdjAmount = newSalesOrder.SalesOrder.AdjAmount;
            //salesOrder.SalesOrder.Remarks = newSalesOrder.SalesOrder.Remarks;
            //salesOrder.SalesOrder.TramsAndCondition = newSalesOrder.SalesOrder.TramsAndCondition;
            //salesOrder.SalesOrder.SalesOrderId;
            if (string.IsNullOrEmpty(salesOrder.SalesOrder.SalesOrderId))
            {
                var SoId = salesOrder.SODetails.FirstOrDefault();
                if (SoId != null)
                {
                    salesOrder.SalesOrder.SalesOrderId = SoId.SalesOrderId;
                }
            }



            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.RemindDate = formCollection["RemindDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            salesOrder.SalesOrder.BankID = salesOrder.SalesOrder.BankID;
            salesOrder.SalesOrder.CardTypeID = salesOrder.SalesOrder.CardTypeID;
            salesOrder.SalesOrder.CardPaidAmount = salesOrder.SalesOrder.CardPaidAmount;
            //salesOrder.SalesOrder.CreateDate = newSalesOrder.SalesOrder.CreateDate;
            //salesOrder.SalesOrder.TSQty = newSalesOrder.SalesOrder.TSQty;
            salesOrder.SalesOrder.IsSmsEnable = Convert.ToBoolean(salesOrder.SalesOrder.IsSmsEnable ? 1 : 0);
            //removing unchanged previous order
            salesOrder.SODetails.Where(x => !string.IsNullOrEmpty(x.SODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => salesOrder.SODetails.Remove(x));

            if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                if (_miscellaneousService.GetDuplicateEntry(i => i.InvoiceNo == salesOrder.SalesOrder.InvoiceNo) != null)
                {
                    string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                    salesOrder.SalesOrder.InvoiceNo = invNo;
                }
            }
            if (Convert.ToDateTime(salesOrder.SalesOrder.RemindDate) > Convert.ToDateTime(salesOrder.SalesOrder.OrderDate))
                RemindDate = Convert.ToDateTime(salesOrder.SalesOrder.RemindDate);

            int CardTypeSetupID = 0;
            decimal DepositChargePercent = 0m;

            decimal TSQty = salesOrder.SODetails.Sum(i => Convert.ToDecimal(i.Quantity));


            salesOrder.SalesOrder.TSQty = TSQty.ToString();

            decimal DepositAmt = _bankTransactionService.CardPaymentNetAmtCalculation(salesOrder.SalesOrder.BankID, salesOrder.SalesOrder.CardTypeID,
                  salesOrder.SalesOrder.CardPaidAmount, out CardTypeSetupID, out DepositChargePercent);

            salesOrder.SalesOrder.CardTypeSetupID = CardTypeSetupID.ToString();
            salesOrder.SalesOrder.DepositChargePercent = DepositChargePercent;

            //DataTable dtBankTrans = _bankTransactionService.CreateBankTransDataTable(Convert.ToDateTime(salesOrder.SalesOrder.OrderDate)
            //    , salesOrder.SalesOrder.InvoiceNo, EnumTransactionType.Deposit,
            //    DepositAmt, salesOrder.SalesOrder.BankID,
            //                               User.Identity.GetConcernId(),
            //                               "Retail sales card payment"
            //      );
            DataTable dtBankTrans = CreateBankTransDataTable(salesOrder);
            DataTable dtSalesOrder = CreateSalesOrderDataTable(salesOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(salesOrder);
            var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            #region Log
            log.Info(new { SalesOrder = salesOrder.SalesOrder, SODetails = salesOrder.SODetails });
            #endregion
            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                if (SystemInfo.ApprovalSystemEnable == 1)
                {
                    if (salesOrder.SalesOrder.Status == "5")//pending sales update
                        Result = _salesOrderService.UpdatePendingSalesUsingSP(User.Identity.GetUserId<int>(), int.Parse(salesOrder.SalesOrder.SalesOrderId),
                                    dtSalesOrder, dtSalesOrderDetail);
                    else
                        Result = _salesOrderService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(), int.Parse(salesOrder.SalesOrder.SalesOrderId),
                                    dtSalesOrder, dtSalesOrderDetail, dtBankTrans);
                }

                else
                    Result = _salesOrderService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(), int.Parse(salesOrder.SalesOrder.SalesOrderId),
                                dtSalesOrder, dtSalesOrderDetail, dtBankTrans);

                if (Result)
                {
                    if (sysInfo.isposinvoice == 1)
                    {
                        TempData["OrderId"] = int.Parse(salesOrder.SalesOrder.SalesOrderId);
                        TempData["IsPOSInvoiceReady"] = true;
                    }
                    else
                    {
                        TempData["IsInvoiceReadyById"] = true;
                        TempData["OrderId"] = int.Parse(salesOrder.SalesOrder.SalesOrderId);
                    }

                    UserAuditDetail useraudit = new UserAuditDetail();
                    useraudit.ObjectID = int.Parse(salesOrder.SalesOrder.SalesOrderId);
                    useraudit.ActivityDtTime = GetLocalDateTime();
                    useraudit.ObjectType = EnumObjectType.Sales;
                    useraudit.ActionType = EnumActionType.Edit;
                    useraudit.ConcernID = User.Identity.GetConcernId();
                    useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                    useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                    _userAuditDetailService.Add(useraudit);
                    _userAuditDetailService.Save();
                }
            }
            else
            {
                Tuple<bool, int> dbResult = null;
                if (SystemInfo.ApprovalSystemEnable == 1)
                    dbResult = _salesOrderService.AddPendingSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate);
                else
                    dbResult = _salesOrderService.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate, dtBankTrans);

                if (dbResult.Item1)
                {
                    UserAuditDetail useraudit = new UserAuditDetail();
                    useraudit.ObjectID = dbResult.Item2;
                    useraudit.ActivityDtTime = GetLocalDateTime();
                    useraudit.ObjectType = EnumObjectType.Sales;
                    useraudit.ActionType = EnumActionType.Add;
                    useraudit.ConcernID = User.Identity.GetConcernId();
                    useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                    useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                    _userAuditDetailService.Add(useraudit);
                    _userAuditDetailService.Save();

                    Result = dbResult.Item1;
                    var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, SOrder>(salesOrder.SalesOrder);
                    invoiceSalesOrder.SOrderDetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                        ICollection<SOrderDetail>>(salesOrder.SODetails);
                    invoiceSalesOrder.SOrderID = dbResult.Item2;


                    TempData["salesInvoiceData"] = invoiceSalesOrder;
                    TempData["MoneyReceiptData"] = invoiceSalesOrder;
                    TempData["IsMoneyReceiptReady"] = true;

                    if (sysInfo.isposinvoice == 1)
                    {
                        TempData["IsPOSInvoiceReady"] = true;
                    }
                    else
                    {
                        TempData["salesInvoiceData"] = invoiceSalesOrder;
                        TempData["IsInvoiceReady"] = true;
                    }

                    //TempData["salesInvoiceData"] = invoiceSalesOrder;

                    ////TempData["IsInvoiceReadyById"] = true;

                    //TempData["IsInvoiceReadyById"] = true;
                    //TempData["OrderId"] = dbResult.Item2;

                    //TempData["IsInvoiceReady"] = true;

                    //TempData["MoneyReceiptData"] = invoiceSalesOrder;
                    //TempData["IsMoneyReceiptReady"] = true;


                    #region Sales SMS Service
                    //var ProductNameList = (from sod in invoiceSalesOrder.SOrderDetails
                    //                       join p in _productService.GetAllIQueryable() on sod.ProductID equals p.ProductID
                    //                       select new { p.ProductName }).ToList();

                    if (SystemInfo.IsRetailSMSEnable == 1 && salesOrder.SalesOrder.IsSmsEnable == true)
                    {
                        if (SystemInfo.IsBanglaSmsEnable == 1)
                        {
                            var _oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = _oCustomer.ContactNo,
                                CustomerID = _oCustomer.CustomerID,
                                //TransNumber = invoiceSalesOrder.InvoiceNo,
                                //Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                //PreviousDue = _oCustomer.TotalDue,
                                ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                SMSType = EnumSMSType.SalesTime,
                                //SalesAmount = invoiceSalesOrder.TotalAmount,
                                CustomerCode = _oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = _oCustomer.CustomerID,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                    PreviousDue = _oCustomer.TotalDue,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                    PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.TotalAmount,
                                    CustomerCode = _oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;
                            if (smsAmountDetails.TotalRecAmt > 1)
                            {
                                var response = SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());
                                if (response != null || response.Count > 0)
                                {
                                    decimal smsBalanceCount = 0m;
                                    foreach (var item in response)
                                    {
                                        smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    }
                                    #region udpate payment info                  
                                    decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                    smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                    _smsBillPaymentBkashService.Update(smsAmountDetails);
                                    _smsBillPaymentBkashService.Save();
                                    #endregion

                                    response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                    _SMSStatusService.AddRange(response);
                                    _SMSStatusService.Save();
                                }
                            }


                        }
                        else
                        {
                            var _oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = _oCustomer.ContactNo,
                                CustomerID = _oCustomer.CustomerID,
                                CustomerName = _oCustomer.Name,
                                TransNumber = invoiceSalesOrder.InvoiceNo,
                                Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                PreviousDue = _oCustomer.TotalDue,
                                ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                SMSType = EnumSMSType.SalesTime,
                                SalesAmount = invoiceSalesOrder.TotalAmount,
                                CustomerCode = _oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = _oCustomer.CustomerID,
                                    CustomerName = _oCustomer.Name,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                    PreviousDue = _oCustomer.TotalDue,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                    PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.TotalAmount,
                                    CustomerCode = _oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            if (smsAmountDetails.TotalRecAmt > 1)
                            {
                                var response = SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());

                                if (response != null || response.Count > 0)
                                {
                                    decimal smsBalanceCount = 0m;
                                    foreach (var item in response)
                                    {
                                        smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    }
                                    #region udpate payment info                  
                                    decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                    smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                    _smsBillPaymentBkashService.Update(smsAmountDetails);
                                    _smsBillPaymentBkashService.Save();
                                    #endregion

                                    response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                    _SMSStatusService.AddRange(response);
                                    _SMSStatusService.Save();

                                }

                            }
                            else
                            {
                                AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                            }


                        }
                    }


                    #endregion
                }

            }

            _salesOrderService.CorrectionStockData(User.Identity.GetConcernId());
            #region For POS Invoice
            //PrintInvoice oPriInvoice = new PrintInvoice();
            //oPriInvoice.print(salesOrder, _SisterConcern);
            #endregion

            if (Result)
            {

                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);

            }
            else
                AddToastMessage("", "Order has been failed.", ToastType.Error);

            return Result;
        }
        private DataTable CreateSalesOrderDataTable(SalesOrderViewModel salesOrder)
        {


            DataTable dtSalesOrder = new DataTable();
            dtSalesOrder.Columns.Add("InvoiceDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("InvoiceNo", typeof(string));
            dtSalesOrder.Columns.Add("VatPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("VatAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtSalesOrder.Columns.Add("TDiscountPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("TDiscountAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("RecAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalDue", typeof(decimal));
            dtSalesOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("Status", typeof(int));
            dtSalesOrder.Columns.Add("CustomerId", typeof(int));
            dtSalesOrder.Columns.Add("ConcernId", typeof(int));
            dtSalesOrder.Columns.Add("CreatedBy", typeof(int));
            dtSalesOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("TotalOffer", typeof(decimal));
            dtSalesOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtSalesOrder.Columns.Add("Remarks", typeof(string));
            dtSalesOrder.Columns.Add("TramsAndCondition", typeof(string));

            dtSalesOrder.Columns.Add("CardPaidAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("CardTypeSetupID", typeof(int));
            dtSalesOrder.Columns.Add("DepositChargePercent", typeof(decimal));
            dtSalesOrder.Columns.Add("TSQty", typeof(decimal));
            dtSalesOrder.Columns.Add("PrevDue", typeof(decimal));
            DataRow row = null;

            row = dtSalesOrder.NewRow();
            row["InvoiceDate"] = salesOrder.SalesOrder.OrderDate;
            row["InvoiceNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["VatPercentage"] = GetDefaultIfNull(salesOrder.SalesOrder.VATPercentage);
            row["VatAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.VATAmount);
            row["GrandTotal"] = GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal);
            row["TDiscountPercentage"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountPercentage);
            row["TDiscountAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount);
            row["PaymentDue"] = GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue);
            row["RecAmt"] = GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount);
            row["TotalAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount);
            row["TotalDue"] = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount);
            row["Status"] = EnumSalesType.Sales;
            row["CustomerId"] = salesOrder.SalesOrder.CustomerId;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["TotalOffer"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer);
            row["NetDiscount"] = GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount);
            row["Remarks"] = salesOrder.SalesOrder.Remarks;
            row["TramsAndCondition"] = salesOrder.SalesOrder.TramsAndCondition;

            if (salesOrder.SalesOrder.CardPaidAmount > 0)
            {
                row["CardPaidAmount"] = salesOrder.SalesOrder.CardPaidAmount;
                row["CardTypeSetupID"] = salesOrder.SalesOrder.CardTypeSetupID;
                row["DepositChargePercent"] = salesOrder.SalesOrder.DepositChargePercent;

            }
            else
            {
                row["CardPaidAmount"] = 0;
                row["CardTypeSetupID"] = 0;
                row["DepositChargePercent"] = 0;
            }

            row["TSQty"] = salesOrder.SalesOrder.TSQty;
            row["PrevDue"] = salesOrder.SalesOrder.PrevDue;


            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("SOrderDetailID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Status", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("Compressor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Motor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Panel", typeof(int));
            dtSalesOrderDetail.Columns.Add("Spareparts", typeof(int));
            dtSalesOrderDetail.Columns.Add("Service", typeof(int));
            dtSalesOrderDetail.Columns.Add("Warranty", typeof(string));


            DataRow row = null;
            #region Delete purpose when more than one SODetails save
            //if (salesOrder.SODetails.Any(i => i.Status == EnumStatus.Deleted) && !string.IsNullOrWhiteSpace(salesOrder.SalesOrder.SalesOrderId))
            //{
            //    var soDetails = _salesOrderDetailService.GetSalesOrderDetailByOrderId(Convert.ToInt32(salesOrder.SalesOrder.SalesOrderId));
            //    var vmSoDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, int, string, string, string,
            //        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>>, IEnumerable<CreateSalesOrderDetailViewModel>>(soDetails).ToList();

            //    var deletedDetails = salesOrder.SODetails.Where(i => i.Status == EnumStatus.Deleted).ToList();
            //    foreach (var item in deletedDetails)
            //    {
            //        var StockDetail = _stockDetailService.GetById(Convert.ToInt32(item.StockDetailId));
            //        var deleteDetail = vmSoDetails.Where(i => !i.SODetailId.Equals(item.SODetailId)
            //                            && i.ProductId.Equals(StockDetail.ProductID.ToString())
            //                            && i.ColorId.Equals(StockDetail.ColorID.ToString())
            //                            && i.GodownID == StockDetail.GodownID
            //                            );
            //        if (deleteDetail != null)
            //        {
            //            deleteDetail.Select(i => { i.Status = EnumStatus.Deleted; return i; }).ToList();
            //            foreach (var deitem in deleteDetail)
            //            {
            //                salesOrder.SODetails.Add(deitem);
            //            }
            //        }
            //    }
            //}
            #endregion

            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();
                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["SOrderDetailID"] = item.SODetailId;
                row["ProductId"] = item.ProductId;
                row["StockDetailId"] = item.StockDetailId;
                row["ColorId"] = item.ColorId;
                row["Status"] = item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = item.UTAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDAmount);
                row["MrpRate"] = item.UnitPrice;
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["Compressor"] = item.GodownID;
                row["Motor"] = 0;
                row["Panel"] = 0;
                row["Spareparts"] = 0;
                row["Service"] = 0;
                row["Warranty"] = item.Service;

                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder != null)
            {
                TempData["salesOrderViewModel"] = salesOrder;
                return View("Create", salesOrder);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                return View(new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = new List<CreateSalesOrderDetailViewModel>(),
                    SalesOrder = new CreateSalesOrderViewModel { InvoiceNo = invNo }
                });
            }
        }

        private ActionResult HandleSalesOrder(SalesOrderViewModel newSalesOrder, FormCollection formCollection)
        {

            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            ViewBag.TramsAndCondition = sysInfo != null ? sysInfo.TramsAndCondition ? true : false : false;

            if (newSalesOrder != null)
            {
                //SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
                //salesOrder = salesOrder ?? new SalesOrderViewModel()
                //{
                //    SalesOrder = newSalesOrder.SalesOrder
                //};
                newSalesOrder.SODetail = new CreateSalesOrderDetailViewModel();

                //if (formCollection.Get("addButton") != null)
                //{
                //    CheckAndAddModelErrorForAdd(newSalesOrder, salesOrder, formCollection);
                //    if (!ModelState.IsValid)
                //    {
                //        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                //        return View("Create", salesOrder);
                //    }
                //    var product = _productService.GetProductById(int.Parse(newSalesOrder.SODetail.ProductId));
                //    if (salesOrder.SODetails != null && product.ProductType == 1 &&
                //        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                //        x.IMENo.Equals(newSalesOrder.SODetail.IMENo)))
                //    {
                //        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                //        return View("Create", salesOrder);
                //    }

                //    AddToOrder(newSalesOrder, salesOrder, formCollection);
                //    ModelState.Clear();
                //    return View("Create", salesOrder);
                //}
                if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSalesOrder, formCollection);

                    //if (!ModelState.IsValid)
                    //{                        
                    //    return View("Create", newSalesOrder);
                    //} 

                    //decimal calGrandtotal = newSalesOrder.SODetails.Where(i => i.Status != EnumStatus.Deleted).Sum(i => Convert.ToDecimal(i.UnitPrice) * Convert.ToDecimal(i.Quantity));

                    //if ((Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) != calGrandtotal) || (Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) != (Convert.ToDecimal(newSalesOrder.SalesOrder.TotalAmount) + Convert.ToDecimal(newSalesOrder.SalesOrder.NetDiscount) + Convert.ToDecimal(newSalesOrder.SalesOrder.AdjAmount))))
                    //{
                    //    TempData["salesOrderViewModel"] = null;
                    //    AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                    //    return RedirectToAction("Index");
                    //}

                    var err = ModelState.Values.SelectMany(s => s.Errors);
                    if (!ModelState.IsValid)
                    {
                        //newSalesOrder.SODetails = newSalesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", newSalesOrder);
                    }

                    if (!IsValidAmount(newSalesOrder))
                    {
                        TempData["salesOrderViewModel"] = null;
                        AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                        return View("Create", newSalesOrder);
                    }

                    //if (!ModelState.IsValid)
                    //{
                    //    //newSalesOrder.SODetails = newSalesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                    //    return View("Create", newSalesOrder);
                    //}
                    //if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                    //{
                    //    if (salesOrder.SODetails.Any())
                    //    {
                    //        List<string> allIMEIs = new List<string>();
                    //        foreach (var sorder in salesOrder.SODetails)
                    //        {
                    //            if (!sorder.IMENo.ToLower().Equals("no barcode") && _salesOrderService.IsAlreadySold(sorder.IMENo, User.Identity.GetConcernId()))
                    //                allIMEIs.Add(sorder.IMENo);
                    //        }
                    //        if (allIMEIs.Any())
                    //        {
                    //            AddToastMessage("", $"IMEI: {string.Join(",", allIMEIs)} already sold!", ToastType.Error);
                    //            return View("Create", salesOrder);
                    //        }
                    //    }
                    //}


                    bool Result = SaveOrder(newSalesOrder, formCollection);
                    //if (Result)
                    //{
                    //    return RedirectToAction("Create");
                    //}
                    ModelState.Clear();
                    TempData["salesOrderViewModel"] = null;
                    
                    return RedirectToAction("Create");
                    //return RedirectToAction("Index");
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

        private bool IsValidAmount(SalesOrderViewModel newSalesOrder)
        {
            bool isValid = false;
            decimal detailsTotalAmount = newSalesOrder.SODetails.Where(i => i.Status != EnumStatus.Deleted)
                                    .Sum(i => Convert.ToDecimal(i.UnitPrice) * Convert.ToDecimal(i.Quantity));
            isValid = Convert.ToDecimal(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) == detailsTotalAmount;

            if (isValid)
                isValid = (Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) == (Convert.ToDecimal(newSalesOrder.SalesOrder.TotalAmount)
                            + Convert.ToDecimal(newSalesOrder.SalesOrder.NetDiscount)
                            + Convert.ToDecimal(newSalesOrder.SalesOrder.AdjAmount) - Convert.ToDecimal(newSalesOrder.SalesOrder.VATAmount)));
            return isValid;
        }

        [HttpGet]
        [Authorize]
        public ActionResult SalesMoneyReceipt(int id)
        {
            TempData["SOrderID"] = id;
            TempData["IsMoneyReceiptById"] = true;
            return RedirectToAction("Index");
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
        public ActionResult Invoice2(int orderId)
        {
            TempData["IsInvoiceReadyById2"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public ActionResult Challan(int orderId)
        {
            TempData["IsChallanReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailySalesReport()
        {
            return View("DailySalesReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MonthlySalesReport()
        {
            return View("MonthlySalesReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult YearlySalesReport()
        {
            return View("YearlySalesReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CustomerSalesReport()
        {
            return View("CustomerSalesReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MOWiseSalesReport()
        {
            return View("MOWiseSalesReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MOWiseCustomerDue()
        {
            return View("MOWiseCustomerDue");
        }

        //[HttpGet]
        //[Authorize]
        //public JsonResult GetProductDetailByIMEINo(string imeiNo)
        //{
        //    if (!string.IsNullOrEmpty(imeiNo))
        //    {
        //        if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
        //        {
        //            var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
        //            int EmployeeID = user.EmployeeID;
        //            var vmProduct = _stockService.GetSRVisitIMEIDetails(imeiNo, EmployeeID);
        //            if (vmProduct != null)
        //            {
        //                var data = new
        //                {
        //                    //Code = vmProduct.ProductCode,
        //                    //Name = vmProduct.ProductName,
        //                    //Id = vmProduct.ProductId,
        //                    //StockDetailId = vmProduct.StockDetailsId,
        //                    //ColorId = vmProduct.ColorId,
        //                    //ColorName = vmProduct.ColorName,
        //                    //MrpRate = vmProduct.MRPRate,
        //                    //IMEINo = vmProduct.IMENo,
        //                    //OfferDescription = vmProduct.OfferDescription,
        //                    //PRate = vmProduct.PRate,
        //                    //PPDPercentage= 0.00,
        //                    //PPDAmount= 0.00,
        //                    //Quantity = 1

        //                    Code = vmProduct.ProductCode,
        //                    Name = vmProduct.ProductName,
        //                    Id = vmProduct.ProductId,
        //                    StockDetailId = vmProduct.StockDetailsId,
        //                    ColorId = vmProduct.ColorId,
        //                    ColorName = vmProduct.ColorName,
        //                    UnitPrice = vmProduct.MRPRate,
        //                    IMEINo = vmProduct.IMENo,
        //                    OfferDescription = vmProduct.OfferDescription,
        //                    PRate = vmProduct.PRate,
        //                    UTAmount = vmProduct.MRPRate,
        //                    Quantity = 1,
        //                    GodownID = vmProduct.GodownID,
        //                    MrpRate = vmProduct.MRPRate,
        //                    PPDPercentage = 0.00,
        //                    PPDAmount = 0.00,



        //                };
        //                return Json(new { data = data, status = true }, JsonRequestBehavior.AllowGet);
        //            }

        //            return Json(new { msg = "IMEI not available in stock.", status = false }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var vmProduct = _stockService.GetStockIMEIDetail(imeiNo);
        //            if (vmProduct != null)
        //            {
        //                var data = new
        //                {
        //                    //Code = vmProduct.ProductCode,
        //                    //Name = vmProduct.ProductName,
        //                    //Id = vmProduct.ProductId,
        //                    //StockDetailId = vmProduct.StockDetailsId,
        //                    //ColorId = vmProduct.ColorId,
        //                    //ColorName = vmProduct.ColorName,
        //                    //MrpRate = vmProduct.MRPRate,
        //                    //IMEINo = vmProduct.IMENo,
        //                    //OfferDescription = vmProduct.OfferDescription,
        //                    //PRate = vmProduct.PRate,
        //                    //ProductType = vmProduct.ProductType

        //                    Code = vmProduct.ProductCode,
        //                    Name = vmProduct.ProductName,
        //                    Id = vmProduct.ProductId,
        //                    StockDetailId = vmProduct.StockDetailsId,
        //                    ColorId = vmProduct.ColorId,
        //                    ColorName = vmProduct.ColorName,
        //                    UnitPrice = vmProduct.MRPRate,
        //                    IMEINo = vmProduct.IMENo,
        //                    OfferDescription = vmProduct.OfferDescription,
        //                    PRate = vmProduct.PRate,
        //                    UTAmount = vmProduct.MRPRate,
        //                    Quantity = 1,
        //                    GodownID = vmProduct.GodownID,
        //                    MrpRate = vmProduct.MRPRate,
        //                    PPDPercentage= 0.00,
        //                    PPDAmount= 0.00,
        //                };
        //                return Json(new { data = data, status = true }, JsonRequestBehavior.AllowGet);
        //            }
        //            return Json(new { msg = "IMEI not available in stock.", status = false }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(new { msg = "Please enter IMEI.", status = false }, JsonRequestBehavior.AllowGet);
        //}




        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {
            if (!string.IsNullOrEmpty(imeiNo))
            {
                var vmProduct = _stockService.GetStockIMEIDetail(imeiNo);
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
                    };
                    return Json(new { data = data, status = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { msg = "IMEI not available in stock.", status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Please enter IMEI.", status = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize]
        public ActionResult MOWiseSDetailsReport()
        {
            return View("MOWiseSDetailsReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRWiseCustomerSalesSummary()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyWorkSheetReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MonthlyBenefitReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWiseBenefitReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWiseSalesReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWiseSalesDetailsReport()
        {
            var statuses = from Enum s in Enum.GetValues(typeof(EnumCustomerType))
                           select new { ID = Convert.ToInt32(s), Name = s.ToString() };

            ViewBag.CustomerTypes = new SelectList(statuses, "ID", "Name");
            return View();
        }

        public ActionResult AdminSalesReport()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetSummaryReport()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()) || User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.hoqueLocalAdmin.ToString()))
            {
                ViewBag.Title = "Admin Summary Report";
                @ViewBag.Concerns = new SelectList(_SisterConcern.GetFamilyTree(User.Identity.GetConcernId()), "ConcernID", "Name");
            }
            else
            {
                ViewBag.Title = "Summary Report";
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetCardType(int BankID)
        {
            if (BankID > 0)
            {
                var CardTypes = (from cs in _CardTypeSetupService.GetAll()
                                 join ct in _CardTypeService.GetAllActive() on cs.CardTypeID equals ct.CardTypeID
                                 where cs.BankID == BankID
                                 select new
                                 {
                                     ct.CardTypeID,
                                     ct.Description
                                 }).ToList();
                if (CardTypes.Count > 0)
                {
                    return Json(CardTypes, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private DataTable CreateBankTransDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtBankTrans = new DataTable();
            dtBankTrans.Columns.Add("TranDate", typeof(DateTime));
            dtBankTrans.Columns.Add("TransactionNo", typeof(string));
            dtBankTrans.Columns.Add("TransactionType", typeof(int));
            dtBankTrans.Columns.Add("Amount", typeof(decimal));
            dtBankTrans.Columns.Add("BankID", typeof(decimal));
            dtBankTrans.Columns.Add("CustomerID", typeof(int));
            dtBankTrans.Columns.Add("SupplierID", typeof(int));
            dtBankTrans.Columns.Add("AnotherBankID", typeof(int));
            dtBankTrans.Columns.Add("ChecqueNo", typeof(string));
            dtBankTrans.Columns.Add("Remarks", typeof(string));
            dtBankTrans.Columns.Add("ConcernID", typeof(int));

            var CardTypeSetup = _CardTypeSetupService.GetAll().FirstOrDefault(i => i.BankID == salesOrder.SalesOrder.BankID && i.CardTypeID == salesOrder.SalesOrder.CardTypeID);
            //if (CardTypeSetup == null)
            //    return dtBankTrans;

            salesOrder.SalesOrder.CardTypeSetupID = CardTypeSetup == null ? "0" : CardTypeSetup.CardTypeSetupID.ToString();
            salesOrder.SalesOrder.DepositChargePercent = CardTypeSetup == null ? 0 : CardTypeSetup.Percentage;
            decimal DepositAmt = salesOrder.SalesOrder.CardPaidAmount - ((salesOrder.SalesOrder.CardPaidAmount * salesOrder.SalesOrder.DepositChargePercent) / 100m);



            DataRow row = null;

            row = dtBankTrans.NewRow();
            row["TranDate"] = salesOrder.SalesOrder.OrderDate;
            row["TransactionNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["TransactionType"] = EnumTransactionType.Deposit;
            row["Amount"] = DepositAmt;
            row["BankID"] = salesOrder.SalesOrder.BankID;
            row["CustomerID"] = 0;
            row["SupplierID"] = 0;
            row["AnotherBankID"] = 0;
            row["ChecqueNo"] = string.Empty;
            row["Remarks"] = "Card payment deposite transactions";
            row["ConcernID"] = User.Identity.GetConcernId();

            dtBankTrans.Rows.Add(row);

            return dtBankTrans;
        }

        [HttpGet]
        [Authorize]
        public JsonResult Approved(int orderId)
        {
            var SOrder = _salesOrderService.GetAllIQueryable().FirstOrDefault(i => i.SOrderID == orderId);
            if (SOrder.Status != (int)EnumSalesType.Pending)
            {
                AddToastMessage("", "This Order is not pending.");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var VMSorder = _mapper.Map<SOrder, CreateSalesOrderViewModel>(SOrder);
            var SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(orderId);
            var VMSOrderDetails = _mapper.Map<IEnumerable<SOrderDetail>, List<CreateSalesOrderDetailViewModel>>(SOrderDetails);
            SalesOrderViewModel SOVM = new SalesOrderViewModel();
            VMSorder.TotalDue = "0";
            SOVM.SalesOrder = VMSorder;
            SOVM.SODetails = VMSOrderDetails;
            StockDetail sDetail = null;

            foreach (var item in VMSOrderDetails)
            {
                sDetail = _stockDetailService.GetById(Convert.ToInt32(item.StockDetailId));
                item.ColorId = sDetail.ColorID.ToString();
            }
            SOVM.SalesOrder.CreateDate = GetLocalDateTime();
            DataTable dtSOrder = CreateSalesOrderDataTable(SOVM);
            DataTable dtSOrderDetails = CreateSODetailDataTable(SOVM);
            DataTable dtBankTrans = CreateBankTransDataTable(SOVM);



            if (_salesOrderService.ApprovedSalesOrderUsingSP(dtSOrder, dtSOrderDetails, orderId, dtBankTrans))
            {
                UserAuditDetail useraudit = new UserAuditDetail();
                useraudit.ObjectID = orderId;
                useraudit.ActivityDtTime = GetLocalDateTime();
                useraudit.ObjectType = EnumObjectType.Sales;
                useraudit.ActionType = EnumActionType.Approved;
                useraudit.ConcernID = User.Identity.GetConcernId();
                useraudit.SessionID = _sessionMasterService.GetActiveSessionId(User.Identity.GetUserId<int>());
                useraudit.ActionPerformedRole = _roleService.GetRoleByUserId(User.Identity.GetUserId<int>());
                _userAuditDetailService.Add(useraudit);
                _userAuditDetailService.Save();

                AddToastMessage("", "Approved Successfull.", ToastType.Success);
            }
            else
                AddToastMessage("", "Approved failed.", ToastType.Error);

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWiseBenefitReportNew()
        {
            return View();
        }

        #region check available qty for visit, no barcode
        [HttpGet]
        [Authorize]
        public JsonResult CheckRemainingQtyForSale(decimal qty, int colorId, int productId, int godownId, int employeeId, int stockDetailsId = 0)
        {
            #region get emp id for mobile user
            if (employeeId == 0 && User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                int SRUID = User.Identity.GetUserId<int>();
                var user = _UserService.GetUserById(SRUID);
                employeeId = user.EmployeeID;
            }
            #endregion

            decimal remainingQty = _SRVisitService.GetRemainingQuantityForSRVisit(productId, colorId, User.Identity.GetConcernId(), godownId, employeeId, stockDetailsId);
            if (remainingQty < qty)
            {
                string msg = $"Those Qty not found in stock for this product. Available Qty: {remainingQty} for selected product";
                return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
            }

            #region sr old validation
            //List<int> allSR = _SRVisitService.GetAllSRListByProduct(productId, colorId, User.Identity.GetConcernId());
            //if (allSR.Any())
            //{
            //    if (!allSR.Contains(employeeId))
            //    {
            //        decimal remainingQty = _SRVisitService.GetRemainingQuantityForSRVisit(productId, colorId, User.Identity.GetConcernId());
            //        if (remainingQty < qty)
            //        {
            //            string msg = $"SR Visit found for this product. Available Qty: {remainingQty} for other employee.";
            //            return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
            //        }
            //    }
            //    else
            //    {
            //        decimal remainingQty = _SRVisitService.GetRemainingQuantityForSRVisit(productId, colorId, User.Identity.GetConcernId(), employeeId);
            //        if (remainingQty < qty)
            //        {
            //            string msg = $"SR Visit found for this product. Available Qty: {remainingQty} for selected employee";
            //            AddToastMessage(message: msg, toastType: ToastType.Error);
            //            return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
            //        }
            //    }

            //}
            #endregion

            return Json(new { status = true, msg = "Ok" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        [Authorize]
        public ActionResult EditPermission(int orderId)
        {
            if (orderId != 0)
            {
                var obj = _salesOrderService.GetSalesOrderById(orderId);
                obj.EditReqStatus = 1;
                _salesOrderService.Update(obj);
                _salesOrderService.SaveSalesOrder();
                AddToastMessage("", "For Edit Req. Permission Send Successfully", ToastType.Success);
                return RedirectToAction("Index");
            }

            AddToastMessage("", "For Edit Req. Permission Send Failed.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditApproved(int orderId)
        {
            if (orderId != 0)
            {
                var obj = _salesOrderService.GetSalesOrderById(orderId);
                obj.EditReqStatus = 2;
                _salesOrderService.Update(obj);
                _salesOrderService.SaveSalesOrder();
                AddToastMessage("", "For Edit Req. Permission Approved Successfully", ToastType.Success);
                return RedirectToAction("Index");
            }

            AddToastMessage("", "For Edit Req. Permission Approved Failed.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult InvoiceHistory(int orderId)
        {
            TempData["IsInvoiceReadyById"] = false;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult PrintPOSInvoice(int orderId)
        {
            TempData["POSSOrderID"] = orderId;
            TempData["IsPOSInvoiceReady"] = true;
            return RedirectToAction("Index");
        }

        

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int orderId, bool isPosRecipt)
        {
            TempData["SorderId"] = orderId;
            TempData["isPosRecipt"] = isPosRecipt;
            TempData["IsMoneyReceiptById"] = true;
            return RedirectToAction("Index");
        }
    }
}