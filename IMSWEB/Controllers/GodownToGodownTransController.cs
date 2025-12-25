using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
namespace IMSWEB.Controllers
{
    [Authorize]
    public class GodownToGodownTransController : CoreController
    {
        private readonly IGodownService _godownService;
        private readonly IProductService _productService;
        private readonly ITransferHistoryService _transferService;
        private readonly IStockService _stockService;
         private readonly IStockDetailService _stockDetailService;
        private readonly IMiscellaneousService<Stock> _MiscellaneousStockService;
        IMapper _mapper;
        public GodownToGodownTransController(IErrorService errorService, IGodownService godownSercie,IProductService productService, IMapper mapper,
            ITransferHistoryService transferService, IStockService stockService, IStockDetailService stockDetailService,
            IMiscellaneousService<Stock> MiscellaneousStockService, ISystemInformationService sysInfoService

            )
            : base(errorService, sysInfoService)
        {
            _godownService = godownSercie;
            _productService = productService;
            _mapper = mapper;
            _transferService = transferService;
            _stockService = stockService;
            _stockDetailService=stockDetailService;
            _MiscellaneousStockService = MiscellaneousStockService;
        }
        public async Task<ActionResult> Index()
        {
            var data = _godownService.GetAllTransferHistoryAsync();
            var vmdate = _mapper.Map<IEnumerable<Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>>, IEnumerable<GetTransferHistory>>(await data);
            return View(vmdate);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            TransferHistoryViewModel transferOrder = (TransferHistoryViewModel)TempData.Peek("transferOrderViewModel");
            if (transferOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring salesOrder 
                TempData["transferOrderViewModel"] = transferOrder;
                return View("Create", transferOrder);
            }
            else
            {
                return View(new TransferHistoryViewModel
                {
                    TransferModel = new CreateTransferHistory(),
                    TransferList = new List<CreateTransferHistory>()
                });
            }
        }

        [HttpPost]
        public ActionResult Create(TransferHistoryViewModel newtransfer, FormCollection formCollection, string returnUrl)
        {
            return HandleAddTotransfer(newtransfer, formCollection);
        }

        public ActionResult HandleAddTotransfer(TransferHistoryViewModel newtransfer, FormCollection formCollection)
        {
            if (newtransfer != null)
            {
                TransferHistoryViewModel transferOrder = (TransferHistoryViewModel)TempData.Peek("transferOrderViewModel");
                transferOrder = transferOrder ?? new TransferHistoryViewModel()
                {
                    TransferModel = newtransfer.TransferModel,
                    TransferList = new List<CreateTransferHistory>()
                };

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newtransfer, transferOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        transferOrder.TransferList = transferOrder.TransferList ?? new List<CreateTransferHistory>();
                        return View("Create", transferOrder);
                    }

                    AddToOrder(newtransfer, transferOrder, formCollection);

                    return View("Create", transferOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    if (transferOrder.TransferList.Count == 0)
                    {
                        AddToastMessage("", "Please Add For Transfer First.", ToastType.Error);
                        return View("Create", transferOrder);
                    }

                    

                    DataTable dtTransferHistories = CreateTransferHistoryDataTable(transferOrder);
                    //if (_transferService.AddTransferHistoryUsingSP(dtTransferHistories))
                    //    AddToastMessage("", "Internal transfer successful.", ToastType.Success);
                    //else
                    //    AddToastMessage("", "Internal transfer failed.", ToastType.Error);


                   
                    if (SaveToOrder(transferOrder))
                        AddToastMessage("", "Internal transfer successful.", ToastType.Success);
                    else
                        AddToastMessage("", "Internal transfer failed.", ToastType.Error);


                    ModelState.Clear();
                    TempData["transferOrderViewModel"] = null;

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(new TransferHistoryViewModel() { TransferModel = new CreateTransferHistory(), TransferList = new List<CreateTransferHistory>() });
                }

            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddToOrder(TransferHistoryViewModel newTransfer, TransferHistoryViewModel transfer, FormCollection formCollection)
        {
            transfer.TransferModel.ProductId = newTransfer.TransferModel.ProductId;
            transfer.TransferModel.TransferDate = DateTime.Now;
            transfer.TransferModel.GodownID = newTransfer.TransferModel.GodownID;
            transfer.TransferModel.FromGodownName = newTransfer.TransferModel.FromGodownName;
            transfer.TransferModel.ToGodown = newTransfer.TransferModel.ToGodown;
            transfer.TransferModel.ToGodownName = newTransfer.TransferModel.ToGodownName;
            transfer.TransferModel.ProductName = newTransfer.TransferModel.ProductName;
            transfer.TransferModel.Quantity = newTransfer.TransferModel.Quantity;
            transfer.TransferModel.CreatedBy = User.Identity.GetUserId<int>();
            transfer.TransferModel.StockID = newTransfer.TransferModel.StockID;
            transfer.TransferModel.StockCode = newTransfer.TransferModel.StockCode;
            transfer.TransferList.Add(transfer.TransferModel);


            TransferHistoryViewModel vm = new TransferHistoryViewModel()
            {
                TransferList = transfer.TransferList,
                TransferModel = new CreateTransferHistory()
            };

            TempData["transferOrderViewModel"] = vm;
            transfer.TransferModel = new CreateTransferHistory();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);

        }



        private bool SaveToOrder( TransferHistoryViewModel transfer)
        {
           TransferHistory _TransferDetail =null;
           // AddToastMessage("", "Order has been added successfully.", ToastType.Success);
            List<TransferHistory> _TransferDetails = null;
            if (_TransferDetails == null)
                _TransferDetails = new List<TransferHistory>();


            #region TrasferHistory  Details
            foreach (var item in transfer.TransferList)
            {
                
                int transferQty = (int)item.Quantity;              
                int maxSDetailID = 0;

                while (transferQty > 0)
                {
                    int GID = Convert.ToInt32(item.GodownID);
                    int PID = Convert.ToInt32(item.ProductId);
                    StockDetail MaxSDetail = _stockDetailService.GetAll().FirstOrDefault(o => o.SDetailID > maxSDetailID && o.ProductID == PID && o.GodownID == GID && o.Quantity > 0 && o.Status == (int)EnumStockStatus.Stock);
                     StockDetail _StockDetail = _stockDetailService.GetAll().FirstOrDefault(o => o.StockID== (int)item.StockID);
                    if (MaxSDetail.Quantity >= transferQty)
                    {
                        _TransferDetail = new TransferHistory();
                        _TransferDetail.ProductId = Convert.ToInt32( item.ProductId);
                        _TransferDetail.FromSDetailID = MaxSDetail.SDetailID;
                        _TransferDetail.FromGodown = _StockDetail.GodownID;
                        _TransferDetail.ToGodown = Convert.ToInt32( item.ToGodown);
                        _TransferDetail.POrderDetailID = MaxSDetail.POrderDetailID;
                        _TransferDetail.TransferDate = item.TransferDate;
                        _TransferDetail.Qty = transferQty;
                        _TransferDetail.CreatedBy = item.CreatedBy;
                        transferQty = 0;
                       // _TransferDetail.ConcernID = _StockDetail.Stock.ConcernID;
                        _TransferDetails.Add(_TransferDetail);
                    }
                    else
                    {
                        _TransferDetail = new TransferHistory();
                        _TransferDetail.ProductId = Convert.ToInt32( item.ProductId);
                        _TransferDetail.FromSDetailID = MaxSDetail.SDetailID;
                        _TransferDetail.FromGodown = _StockDetail.GodownID;
                        _TransferDetail.ToGodown = Convert.ToInt32( item.ToGodown);
                        _TransferDetail.POrderDetailID = MaxSDetail.POrderDetailID;
                        _TransferDetail.TransferDate = item.TransferDate;

                        //if (MaxSDetail.Product.ProductType == (int)EnumProductType.AutoBC)
                        //{
                        //    _TransferDetail.Qty = 1;
                        //}
                        //else
                        //{
                            _TransferDetail.Qty = MaxSDetail.Quantity;
                        //}

                        _TransferDetail.CreatedBy = 1;
                      //  _TransferDetail.ConcernID = _StockDetail.Stock.ConcernID;


                        //if (MaxSDetail.Product.ProductType == (int)EnumProductType.AutoBC)
                        //{
                        //    transferQty = transferQty - 1;
                        //}
                        //else
                        //{
                            transferQty = transferQty - (int)MaxSDetail.Quantity;
                        //}


                        _TransferDetails.Add(_TransferDetail);
                    }
                    maxSDetailID = MaxSDetail.SDetailID;
                }

            }

            #endregion

            var StockDetailList = _stockDetailService.GetAll().ToList();
            List<Stock> StockList =_stockService.GetAllStock().ToList();
            var ProductList = _productService.GetAllProduct();
            Product product = null;
            int k = 0;

            #region Stocks
            k = 0;
            var _TransferDetailsGroupBy = (from td in _TransferDetails
                                           group td by new { td.ProductId, td.FromGodown, td.ToGodown }
                                               into g
                                               select new
                                               {
                                                   ProductId = g.Key.ProductId,
                                                   FromGodown = g.Key.FromGodown,
                                                   FromSDetailID = g.Select(o => o.FromSDetailID).FirstOrDefault(),
                                                   g.Key.ToGodown,

                                                   Qty = g.Sum(o => o.Qty)
                                               }
                        );

            foreach (var oPOPDItem in _TransferDetailsGroupBy)
            {
                k = k + 1;

               // product = ProductList.FirstOrDefault(o => o.Item1 == oPOPDItem.ProductId);
                StockDetail oEditStockDetail = StockDetailList.Where(o => o.SDetailID == oPOPDItem.FromSDetailID).FirstOrDefault();
                Stock oEDItStock = StockList.Where(o => o.StockID == oEditStockDetail.StockID).FirstOrDefault();
                oEDItStock.Quantity = oEDItStock.Quantity - oPOPDItem.Qty;
                Stock oToOStock = StockList.Where(o => o.GodownID == oPOPDItem.ToGodown && o.ProductID == oPOPDItem.ProductId && o.ColorID == oEditStockDetail.ColorID).FirstOrDefault();

                if (oToOStock == null)
                {
                    oToOStock = new Stock();
                    oToOStock.StockID = StockList.Max(o => o.StockID) + k;
                    oToOStock.GodownID = oPOPDItem.ToGodown;
                    oToOStock.ProductID = oPOPDItem.ProductId;
                    oToOStock.ColorID = oEditStockDetail.ColorID;
                    oToOStock.Quantity = oPOPDItem.Qty;
                    oToOStock.MRPPrice = oEditStockDetail.Stock.MRPPrice;
                    oToOStock.LPPrice = oEditStockDetail.Stock.LPPrice;
                    oToOStock.CreatedBy = oEditStockDetail.Stock.CreatedBy;
                    oToOStock.StockCode = oEditStockDetail.Stock.StockCode;
                    oToOStock.ConcernID = oEditStockDetail.Stock.ConcernID;
                    oToOStock.CreateDate = DateTime.Now;
                    oToOStock.EntryDate = DateTime.Now;
                    StockList.Add(oToOStock);
                    _stockService.AddStock(oToOStock);
                }
                else
                {
                    oToOStock.Quantity = oToOStock.Quantity + oPOPDItem.Qty;
                }

            }

            #endregion


            #region StockDetails

            k = 0;
            foreach (TransferHistory oPOPDItem in _TransferDetails.ToList())
            {
                k = k + 1;
                StockDetail oEditStockDetail = StockDetailList.Where(o => o.SDetailID == oPOPDItem.FromSDetailID).FirstOrDefault();
                Stock oEDItStock = StockList.Where(o => o.StockID == oEditStockDetail.StockID).FirstOrDefault();
                oEditStockDetail.Quantity = oEditStockDetail.Quantity - oPOPDItem.Qty;
                Stock oToOStock = StockList.Where(o => o.GodownID == oPOPDItem.ToGodown && o.ProductID == oPOPDItem.ProductId && o.ColorID == oEditStockDetail.ColorID).FirstOrDefault();
                
                StockDetail oStockDetail = new StockDetail();
                oStockDetail.SDetailID = StockDetailList.Max(o => o.SDetailID) + k;
                oStockDetail.StockCode = oEditStockDetail.StockCode;
                oStockDetail.ProductID = oPOPDItem.ProductId;
                oStockDetail.IMENO = oEditStockDetail.IMENO;
                oStockDetail.StockID = oEditStockDetail.StockID;
                oStockDetail.ColorID = oEditStockDetail.ColorID;
                oStockDetail.Status = 1;
                oStockDetail.PRate = oEditStockDetail.PRate;
                oStockDetail.SRate = oEditStockDetail.SRate;
                oStockDetail.CreditSRate = oEditStockDetail.CreditSRate;               
                oStockDetail.POrderDetailID = oEditStockDetail.POrderDetailID;
                oStockDetail.Quantity = oPOPDItem.Qty;
                oStockDetail.GodownID = oPOPDItem.ToGodown;
                if (oToOStock != null)
                {
                    oStockDetail.StockID = oToOStock.StockID;
                }                  
                else
                {
                    oStockDetail.StockID = 0;
                }                              
                
                oPOPDItem.ToSDetailID = oStockDetail.SDetailID;
                oPOPDItem.TransferDate = oPOPDItem.TransferDate;
                oPOPDItem.ConcernID = oEDItStock.ConcernID;
                _TransferDetails.Add(oPOPDItem);
                _stockDetailService.AddStockDetail(oStockDetail);
                _transferService.AddTransferHistory(oPOPDItem);


            }
            #endregion
            _transferService.SaveTransferHistory();
            //_godownService.SaveGodown();
            _stockService.SaveStock();
            _stockDetailService.SaveStockDetail();
            return true;

        }
        private void CheckAndAddModelErrorForAdd(TransferHistoryViewModel newTransferOrder,
    TransferHistoryViewModel transferOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("TransferModel.ProductId", "Product is required");
            else
            {
                newTransferOrder.TransferModel.ProductName = formCollection["ProductDetailsName"];
                newTransferOrder.TransferModel.ProductId = formCollection["ProductDetailsId"];
            }

            if (string.IsNullOrEmpty(formCollection["GodownsId"]))
                ModelState.AddModelError("TransferModel.ToGodownName", "To Godown is required");
            else
            {
                newTransferOrder.TransferModel.ToGodown = formCollection["GodownsId"];
                newTransferOrder.TransferModel.ToGodownName = formCollection["GodownsName"];
            }

            if (newTransferOrder.TransferModel.GodownID.Equals(newTransferOrder.TransferModel.ToGodown))
            {
                ModelState.AddModelError("TransferModel.ToGodownName", "From Godown and To Godown can not be same.");
            }

            if (!string.IsNullOrEmpty(newTransferOrder.TransferModel.GodownID))
            {
                newTransferOrder.TransferModel.FromGodownName = _godownService.GetGodownById(int.Parse(newTransferOrder.TransferModel.GodownID)).Name;
            }

            if (newTransferOrder.TransferModel.Quantity == 0)
            {
                ModelState.AddModelError("TransferModel.Quantity", "Quantity is required.");
            }

            if (newTransferOrder.TransferModel.PreviousStock < newTransferOrder.TransferModel.Quantity)
            {
                ModelState.AddModelError("TransferModel.PreviousStock", "Stock is not avaiable.");
            }

            if (!string.IsNullOrEmpty(newTransferOrder.TransferModel.ProductId) && !string.IsNullOrEmpty(newTransferOrder.TransferModel.ToGodown))
            {
                var toStock = _stockService.GetStockByProductIdandGodownID(int.Parse(newTransferOrder.TransferModel.ProductId), int.Parse(newTransferOrder.TransferModel.ToGodown));
                var fromStock = _stockService.GetStockByProductIdandGodownID(int.Parse(newTransferOrder.TransferModel.ProductId), int.Parse(newTransferOrder.TransferModel.GodownID));
                if (fromStock != null)
                {
                    newTransferOrder.TransferModel.StockID = fromStock.StockID;
                }
                else
                {
                    newTransferOrder.TransferModel.StockCode = _MiscellaneousStockService.GetUniqueKey(x => int.Parse((x.StockCode)));
                }
            }



        }

        private DataTable CreateTransferHistoryDataTable(TransferHistoryViewModel TransferHistoryViewModel)
        {
            DataTable dtTransferHistory = new DataTable();
            dtTransferHistory.Columns.Add("ProductId", typeof(int));
            dtTransferHistory.Columns.Add("Quantity", typeof(decimal));
            dtTransferHistory.Columns.Add("GodownID", typeof(int));
            dtTransferHistory.Columns.Add("ToGodownID", typeof(int));
            dtTransferHistory.Columns.Add("StockID", typeof(int));
            dtTransferHistory.Columns.Add("CreatedBy", typeof(int));
            dtTransferHistory.Columns.Add("StockCode", typeof(string));

            DataRow row = null;

            foreach (var item in TransferHistoryViewModel.TransferList)
            {
                row = dtTransferHistory.NewRow();

                row["ProductId"] = item.ProductId;
                row["Quantity"] = item.Quantity;
                row["GodownID"] = item.GodownID;
                row["ToGodownID"] = item.ToGodown;
                row["StockID"] = item.StockID;
                row["CreatedBy"] = item.CreatedBy;
                row["StockCode"] = item.StockCode;

                dtTransferHistory.Rows.Add(row);
            }

            return dtTransferHistory;
        }


        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId, string previousAction)
        {
            TransferHistoryViewModel transferOrder = (TransferHistoryViewModel)TempData.Peek("transferOrderViewModel");
            if (transferOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateTransferHistory itemToDelete =
                transferOrder.TransferList.Where(x => int.Parse(x.ProductId) == id && x.GodownID == detailId.ToString()).FirstOrDefault();

            if (itemToDelete != null)
            {
                transferOrder.TransferList.Remove(itemToDelete);
                TempData["transferOrderViewModel"] = transferOrder;
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

        [HttpGet]
        public ActionResult InternalTransferReport()
        {
            return View();
        }


    }
}