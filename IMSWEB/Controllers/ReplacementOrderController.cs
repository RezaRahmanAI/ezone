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
using IMSWEB.ViewModels;
using log4net;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class ReplacementOrderController : CoreController
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
        IUserService _userService;
        IMapper _mapper;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReplacementOrderController(IErrorService errorService,
            ISalesOrderService salesOrderService, ISalesOrderDetailService salesOrderDetailService,
            IStockService stockService, IStockDetailService stockDetailService,
            ICustomerService customerService, IEmployeeService employeeService,
            ITransactionalReport transactionalReportService,
            IMiscellaneousService<SOrder> miscellaneousService, IMapper mapper,
            IUserService userService,
            IProductService productService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
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
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            TempData["salesOrderViewModel"] = null;
            int EmployeeID = 0;
            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _userService.GetUserById(User.Identity.GetUserId<int>());
                if (user != null)
                    EmployeeID = user.EmployeeID;
            }
            var repOrders = _salesOrderService.GetReplacementOrdersByAsync(EmployeeID);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>,
            IEnumerable<GetSalesOrderViewModel>>(await repOrders);
            return View(vmSO);

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
        public ActionResult Create(SalesOrderViewModel newReplacementOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleSalesOrder(newReplacementOrder, formCollection);
        }


        private ActionResult ReturnCreateViewWithTempData()
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring salesOrder 
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

        private ActionResult HandleSalesOrder(SalesOrderViewModel newReplacementOrder, FormCollection formCollection)
        {
            if (newReplacementOrder != null)
            {
                SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
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
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                        x.DamageIMEINO.Equals(newReplacementOrder.SODetail.DamageIMEINO)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View("Create", salesOrder);
                    }

                    AddToOrder(newReplacementOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", salesOrder);
                }
                else if (formCollection.Get("btnReplace") != null)
                {
                    CheckAndAddModelErrorForSave(newReplacementOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    SaveOrder(newReplacementOrder, salesOrder, formCollection);
                    ModelState.Clear();

                    //mapping for sales ivoice
                    //var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, SOrder>(salesOrder.SalesOrder);
                    //invoiceSalesOrder.SOrderDetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                    //    ICollection<ReplaceOrderDetail>>(salesOrder.SODetails);

                    var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, ReplaceOrder>(salesOrder.SalesOrder);

                    var invoiceSalesOrderdetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                            ICollection<ReplaceOrderDetail>>(salesOrder.SODetails);

                    TempData["ReplacementInvoice"] = invoiceSalesOrder;
                    TempData["ReplacementInvoicedetails"] = invoiceSalesOrderdetails;


                    TempData["salesOrderViewModel"] = null;
                    //TempData["IsInvoiceReadyById"] = true;
                    TempData["IsInvoiceReady"] = true;
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

        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newReplacementOrder,
    SalesOrderViewModel replacementOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
                replacementOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            //ProductDetailsId is ProductId
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
                replacementOrder.SODetail.ProductId = formCollection["ProductDetailsId"];

            if (string.IsNullOrEmpty(newReplacementOrder.SODetail.Quantity) || Convert.ToInt32(double.Parse(newReplacementOrder.SODetail.Quantity)) <= 0)
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
            }

            if (string.IsNullOrEmpty(newReplacementOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (string.IsNullOrEmpty(newReplacementOrder.SODetail.MRPRate))
                ModelState.AddModelError("SODetail.MRPRate", "Purchase Rate is required");

            if (string.IsNullOrEmpty(newReplacementOrder.SODetail.UnitPrice))
                ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");

            //if (string.IsNullOrEmpty(newReplacementOrder.SODetail.Remarks))
            //    ModelState.AddModelError("SODetail.Remarks", "Remarks is required");

            if (string.IsNullOrEmpty(newReplacementOrder.SODetail.DamageIMEINO))
            {
                ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
            }
            else
            {
                var stockDetails = _stockDetailService.GetStockDetailByProductId(
                    int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                if (!stockDetails.Any(x => x.IMENO.Equals(newReplacementOrder.SODetail.IMENo)))
                    ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
            }

            if (string.IsNullOrEmpty(formCollection["dStockDetailsId"]))
            {
                ModelState.AddModelError("SODetail.DamageIMEINO", "Damage Product is required.");
            }
            else
            {
                int SDetailID = Convert.ToInt32(formCollection["dStockDetailsId"]);
                if (_salesOrderService.IsIMEIAlreadyReplaced(SDetailID))
                {
                    ModelState.AddModelError("SODetail.DamageIMEINO", "This IMEI already replaced.");
                    AddToastMessage("", newReplacementOrder.SODetail.DamageIMEINO + " is already replaced. Please contact with OCT", ToastType.Error);
                }
            }

            if (newReplacementOrder.SODetail.ReplaceProductType == 0)
                ModelState.AddModelError("SODetail.ReplaceProductType", "Product type is required.");



        }

        private void CheckAndAddModelErrorForSave(SalesOrderViewModel newReplacementOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {

            newReplacementOrder.SalesOrder.CurrentDue = "0";
            newReplacementOrder.SalesOrder.NetDiscount = "0";
            newReplacementOrder.SalesOrder.PPDiscountAmount = "0";
            newReplacementOrder.SalesOrder.TotalDiscountPercentage = "0";
            newReplacementOrder.SalesOrder.VATAmount = "0";
            newReplacementOrder.SalesOrder.VATPercentage = "0";
            newReplacementOrder.SalesOrder.TotalOffer = "0";

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.RecieveAmount", "Pay Amount is required");
            if (newReplacementOrder.SalesOrder.RecieveAmount == null || newReplacementOrder.SalesOrder.RecieveAmount == "")
            {
                newReplacementOrder.SalesOrder.RecieveAmount = "0";
                salesOrder.SalesOrder.RecieveAmount = "0";
            }
            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
                salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            if (salesOrder.SODetails == null)
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Add to order first.");

            Customer customer = _customerService.GetCustomerById(Convert.ToInt32(salesOrder.SalesOrder.CustomerId));
            Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);

            //if (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
            //    ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");

            //if (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
            //    ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");

            if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid");
        }

        private void AddToOrder(SalesOrderViewModel newReplacementOrder,
            SalesOrderViewModel replacementOrder, FormCollection formCollection)
        {

            #region Parent Order
            replacementOrder.SalesOrder.DamageTotalAmount = (decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.DamageTotalAmount)) +
                decimal.Parse(GetDefaultIfNull(formCollection["dUnitPrice"]))).ToString();

            replacementOrder.SalesOrder.ReplaceTotalAmount = (decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.ReplaceTotalAmount)) +
                decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.UnitPrice))).ToString();


            replacementOrder.SalesOrder.GrandTotal = (decimal.Parse(replacementOrder.SalesOrder.ReplaceTotalAmount) -
                decimal.Parse(replacementOrder.SalesOrder.DamageTotalAmount)).ToString();


            //replacementOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PPDiscountAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPDAmount))).ToString();

            //replacementOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            //replacementOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalDiscountAmount)).ToString();

            //replacementOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.VATPercentage)).ToString();
            //replacementOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.VATAmount)).ToString();

            replacementOrder.SalesOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.AdjAmount)).ToString();

            //replacementOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPDAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPOffer))).ToString();

            var netTotal = ((decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.AdjAmount))));

            // For Total Offer Purpose
            //replacementOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalOffer)) +
            //decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPOffer))).ToString();

            replacementOrder.SalesOrder.TotalAmount = netTotal.ToString();
            replacementOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.RecieveAmount))).ToString();
            replacementOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newReplacementOrder.SalesOrder.RecieveAmount);

            replacementOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            replacementOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            #endregion

            replacementOrder.SODetail.SODetailId = newReplacementOrder.SODetail.SODetailId;
            replacementOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            replacementOrder.SODetail.ColorId = formCollection["ColorsId"];
            replacementOrder.SODetail.ColorName = newReplacementOrder.SODetail.ColorName;
            replacementOrder.SODetail.StockDetailId = formCollection["dStockDetailsId"];//damage
            replacementOrder.SODetail.RStockDetailId = formCollection["StockDetailsId"];//replace 
            replacementOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            replacementOrder.SODetail.dSOrderDetailID = formCollection["dSOrderDetailID"];//damage
            replacementOrder.SODetail.DamageIMEINO = newReplacementOrder.SODetail.DamageIMEINO;
            replacementOrder.SODetail.ReplaceIMEINO = newReplacementOrder.SODetail.IMENo;
            replacementOrder.SODetail.Quantity = newReplacementOrder.SODetail.Quantity;
            replacementOrder.SODetail.PPDPercentage = newReplacementOrder.SODetail.PPDPercentage;
            replacementOrder.SODetail.PPDAmount = newReplacementOrder.SODetail.PPDAmount;
            replacementOrder.SODetail.UnitPrice = newReplacementOrder.SODetail.UnitPrice;
            replacementOrder.SODetail.DamageUnitPrice = formCollection["dUnitPrice"];
            replacementOrder.SODetail.MRPRate = newReplacementOrder.SODetail.MRPRate;
            replacementOrder.SODetail.UTAmount = newReplacementOrder.SODetail.UTAmount;
            replacementOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            replacementOrder.SODetail.DamageProductName = formCollection["dProductDetailsName"];
            replacementOrder.SODetail.Status = newReplacementOrder.SODetail.Status == default(int) ? EnumStatus.New : newReplacementOrder.SODetail.Status;
            replacementOrder.SODetail.PPOffer = newReplacementOrder.SODetail.PPOffer;
            replacementOrder.SODetail.Remarks = newReplacementOrder.SODetail.Remarks;
            replacementOrder.SODetail.ReplaceProductType = newReplacementOrder.SODetail.ReplaceProductType;
            replacementOrder.SODetails = replacementOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
            replacementOrder.SODetails.Add(replacementOrder.SODetail);

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = replacementOrder.SODetails,
                SalesOrder = replacementOrder.SalesOrder
            };

            TempData["salesOrderViewModel"] = vm;
            replacementOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private void SaveOrder(SalesOrderViewModel newReplacementOrder,
            SalesOrderViewModel replacementOrder, FormCollection formCollection)
        {
            replacementOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newReplacementOrder.SalesOrder.NetDiscount);
            replacementOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalAmount)).ToString();
            replacementOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)).ToString();

            replacementOrder.SalesOrder.TotalDiscountPercentage = newReplacementOrder.SalesOrder.TotalDiscountPercentage;
            replacementOrder.SalesOrder.TotalDiscountAmount = newReplacementOrder.SalesOrder.TotalDiscountAmount;
            replacementOrder.SalesOrder.RecieveAmount = newReplacementOrder.SalesOrder.RecieveAmount;
            replacementOrder.SalesOrder.VATPercentage = newReplacementOrder.SalesOrder.VATPercentage;
            replacementOrder.SalesOrder.VATAmount = newReplacementOrder.SalesOrder.VATAmount;
            replacementOrder.SalesOrder.AdjAmount = newReplacementOrder.SalesOrder.AdjAmount;

            replacementOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            replacementOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            //removing unchanged previous order
            replacementOrder.SODetails.Where(x => !string.IsNullOrEmpty(x.SODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => replacementOrder.SODetails.Remove(x));

            DataTable dtSalesOrder = CreateSalesOrderDataTable(replacementOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(replacementOrder);
            log.Info(new { ReplaceOrder = replacementOrder.SalesOrder, ReplaceOrderDetails = replacementOrder.SODetails });

            //if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            //    _salesOrderService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(), int.Parse(replacementOrder.SalesOrder.SalesOrderId),
            //        dtSalesOrder, dtSalesOrderDetail);
            //else
            _salesOrderService.AddReplacementOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);

            _salesOrderService.CorrectionStockData(User.Identity.GetConcernId());

            AddToastMessage("", "Order has been saved successfully.", ToastType.Success);

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
            dtSalesOrder.Columns.Add("IsReplacement", typeof(int));

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
            row["IsReplacement"] = 1;

            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("SOrderDetailID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("RStockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Status", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("RepOrderID", typeof(int));
            dtSalesOrderDetail.Columns.Add("RepUnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("Remarks", typeof(string));
            dtSalesOrderDetail.Columns.Add("IsDamage", typeof(int));
            dtSalesOrderDetail.Columns.Add("dSOrderDetailID", typeof(int));

            DataRow row = null;

            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();
                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["SOrderDetailID"] = item.SODetailId;
                row["ProductId"] = item.ProductId;
                row["StockDetailId"] = item.StockDetailId;
                row["RStockDetailId"] = item.RStockDetailId;
                row["ColorId"] = item.ColorId;
                row["Status"] = item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = GetDefaultIfNull(item.UTAmount);
                row["PPDisPer"] = GetDefaultIfNull(item.PPDPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDAmount);
                row["MrpRate"] = item.MRPRate;
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["RepOrderID"] = item.RepOrderID;
                row["RepUnitPrice"] = item.UnitPrice;
                row["Remarks"] = GetDefaultIfNull(item.Remarks);
                row["IsDamage"] = item.ReplaceProductType;
                row["dSOrderDetailID"] = item.dSOrderDetailID;

                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {
            var customProductDetails = _productService.GetAllProductFromDetail();
            var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

            if (!string.IsNullOrEmpty(imeiNo))
            {
                var vmProduct = vmProductDetails.FirstOrDefault(x => x.IMENo.ToLower().Equals(imeiNo.ToLower()));
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
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetDamageProductDetailByIMEINo(string imeiNo, int CustomerID)
        {

            if (!string.IsNullOrEmpty(imeiNo))
            {
                var customProductDetails = _productService.GetSalesDetailByCustomerID(CustomerID, imeiNo);

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
                        SOrderDetailID = vmProduct.SOrderDetailID.ToString()
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
        public JsonResult GetSalesProductDetailByCustomerID(int CustomerID)
        {
            var customProductDetails = _productService.GetSalesDetailByCustomerID(CustomerID, string.Empty);
            if (customProductDetails != null)
            {
                return Json(customProductDetails, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
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
        public ActionResult ReplacementReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult DamageProductReport()
        {
            return View();
        }

    }
}