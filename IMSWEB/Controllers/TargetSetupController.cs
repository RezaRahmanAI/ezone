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
using Microsoft.AspNet.Identity;
namespace IMSWEB.Controllers
{
    [Authorize]
    public class TargetSetupController : CoreController
    {
        ITargetSetupService _TargetSetupService;
        IMiscellaneousService<TargetSetup> _miscellaneousService;
        IMapper _mapper;
        IProductService _ProductService;
        ISystemInformationService _SysInfoService;
        public TargetSetupController(IErrorService errorService,
            ITargetSetupService TargetSetupService, IMiscellaneousService<TargetSetup> miscellaneousService,
            IProductService ProductService, ISystemInformationService SysInfo,
            IMapper mapper)
            : base(errorService, SysInfo)
        {
            _TargetSetupService = TargetSetupService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _ProductService = ProductService;
            _SysInfoService = SysInfo;
        }

        public async Task<ActionResult> Index()
        {
            TempData["TargetSetup"] = null;
            var targets = await _TargetSetupService.GetAllAsync();
            var vm = _mapper.Map<IEnumerable<Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>>, IEnumerable<TargetSetupViewModel>>(targets);
            return View(vm);
        }

        public ActionResult Create(FormCollection collection)
        {
            return ReturnCreateViewWithTempData(collection);
        }
        private ActionResult ReturnCreateViewWithTempData(FormCollection collection)
        {
            TargetSetupViewModel TargetSetup = (TargetSetupViewModel)TempData.Peek("TargetSetup");
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            if (TargetSetup != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                if (!string.IsNullOrEmpty(collection["CategoriesId"]))
                {
                    int CategoryID = Convert.ToInt32(collection["CategoriesId"]);
                    var Products = _ProductService.GetAllProductIQueryable().Where(i => i.CategoryID == CategoryID).ToList();
                    var vmProducts = _mapper.Map<List<ProductWisePurchaseModel>, List<GetProductViewModel>>(Products);
                    TargetSetup.Products = vmProducts;
                    TargetSetup.CategoryID = CategoryID.ToString();
                }
                TempData["TargetSetup"] = TargetSetup;
                return View("Create", TargetSetup);
            }
            else
            {
                return View("Create", new TargetSetupViewModel() { TargetMonth = SysInfo.NextPayProcessDate });
            }
        }
        private void AddModelError(TargetSetupViewModel TargetSetup, TargetSetupViewModel newTargetSetup, FormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["CategoriesId"]))
            {
                ModelState.AddModelError("CategoryID", "Category is required.");
            }
            else
            {
                int CategoryID = Convert.ToInt32(collection["CategoriesId"]);
                if (TargetSetup.TargetSetupDetailList.Any(i => i.CategoryID == CategoryID && i.Status != EnumStatus.Deleted))
                {
                    ModelState.AddModelError("CategoryID", "This Category is already added.");
                    newTargetSetup.CategoryID = CategoryID.ToString();
                }
            }
        }

        private void GetPickerValue(TargetSetupViewModel TargetSetup, TargetSetupViewModel newTargetSetup, FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["EmployeesId"]))
            {
                newTargetSetup.EmployeeID = Convert.ToInt32(collection["EmployeesId"]);
                TargetSetup.EmployeeID = newTargetSetup.EmployeeID;
            }
            if (!string.IsNullOrEmpty(collection["TargetMonth"]))
            {
                TargetSetup.TargetMonth = Convert.ToDateTime(collection["TargetMonth"]);
                newTargetSetup.TargetMonth = Convert.ToDateTime(collection["TargetMonth"]);
            }
        }

        [HttpPost]
        public ActionResult Create(TargetSetupViewModel newTargetSetup, FormCollection collection)
        {
            return HandleTargetSetup(newTargetSetup, collection);
        }

        private ActionResult HandleTargetSetup(TargetSetupViewModel newTargetSetup, FormCollection collection)
        {
            TargetSetupViewModel TargetSetup = (TargetSetupViewModel)TempData.Peek("TargetSetup");
            TargetSetup = TargetSetup ?? new TargetSetupViewModel();

            if (collection.Get("btnAddProduct") != null)
            {

                GetPickerValue(TargetSetup, newTargetSetup, collection);
                AddModelError(TargetSetup, newTargetSetup, collection);
                if (!ModelState.IsValid)
                    return View(TargetSetup);

                AddToTarget(TargetSetup, newTargetSetup, collection);

                return View("Create", TargetSetup);
            }
            else if (collection.Get("btnSave") != null)
            {
                TargetSetup.Amount = newTargetSetup.Amount;
                AddSaveModelError(TargetSetup, newTargetSetup, collection);
                if (!ModelState.IsValid)
                    return View(TargetSetup);
                TargetSetup.TargetMonth = newTargetSetup.TargetMonth;
                TargetSetup.Amount = newTargetSetup.Amount;
                TargetSetup.Quantity = TargetSetup.TargetSetupDetailList.Sum(i => i.Quantity).ToString();
                DataTable dtSetup = CreateTargetSetupDatatable(TargetSetup);
                DataTable dtSetupDetails = CreateTargetSetupDetailsDatatable(TargetSetup);
                string actionName = ControllerContext.RouteData.Values["action"].ToString().ToLower();

                if (actionName.Equals("create") && string.IsNullOrEmpty(TargetSetup.TID)) //New Add
                {

                    if (_TargetSetupService.AddTargetSetupUsingSP(dtSetup, dtSetupDetails))
                        AddToastMessage("", "Save Successfull", ToastType.Success);
                    else
                        AddToastMessage("", "Save Failed.", ToastType.Error);
                }
                else if (actionName.Equals("create") && !string.IsNullOrEmpty(TargetSetup.TID)) //Update
                {
                    if (_TargetSetupService.DeleteTargetSetupUsingSP(Convert.ToInt32(TargetSetup.TID)))
                    {
                        if (_TargetSetupService.AddTargetSetupUsingSP(dtSetup, dtSetupDetails))
                            AddToastMessage("", "Update Successfull", ToastType.Success);
                        else
                            AddToastMessage("", "Update Failed.", ToastType.Error);
                    }
                    else
                    {
                        AddToastMessage("", "Update Failed.", ToastType.Error);
                    }

                }
                TempData["TargetSetup"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                if (!string.IsNullOrEmpty(collection["CategoriesId"]))
                {
                    int CategoryID = Convert.ToInt32(collection["CategoriesId"]);
                    var Products = _ProductService.GetAllProductIQueryable().Where(i => i.CategoryID == CategoryID).ToList();
                    var vmProducts = _mapper.Map<List<ProductWisePurchaseModel>, List<GetProductViewModel>>(Products);
                    TargetSetup.Products = vmProducts;
                    GetPickerValue(TargetSetup, newTargetSetup, collection);
                    TargetSetup.CategoryID = CategoryID.ToString();
                }
                else
                    TargetSetup.Products = new List<GetProductViewModel>();
            }
            return View(TargetSetup);
        }

        private DataTable CreateTargetSetupDetailsDatatable(TargetSetupViewModel TargetSetup)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));
            dt.Columns.Add("CRUDStatus", typeof(int));
            dt.Columns.Add("Quanity", typeof(decimal));

            DataRow row = null;
            foreach (var item in TargetSetup.TargetSetupDetailList)
            {
                row = dt.NewRow();
                row["ProductID"] = item.ProductId;
                row["CRUDStatus"] = item.Status;
                row["Quanity"] = item.Quantity;
                dt.Rows.Add(row);
            }


            return dt;
        }

        private DataTable CreateTargetSetupDatatable(TargetSetupViewModel TargetSetup)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TID", typeof(int));
            dt.Columns.Add("TargetMonth", typeof(DateTime));
            dt.Columns.Add("Quantity", typeof(decimal));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("EmployeeID", typeof(int));
            dt.Columns.Add("ConcernID", typeof(int));
            dt.Columns.Add("CRUDStatus", typeof(int));
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("CreatedBy", typeof(int));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Columns.Add("ModifiedBy", typeof(int));
            dt.Columns.Add("ModifiedDate", typeof(DateTime));

            DataRow row = dt.NewRow();
            row["TID"] = DBNull.Value;
            row["TargetMonth"] = TargetSetup.TargetMonth;
            row["Quantity"] = Convert.ToDecimal(TargetSetup.Quantity) <= 0m ? 0m : Convert.ToDecimal(TargetSetup.Quantity);
            row["Amount"] = Convert.ToDecimal(TargetSetup.Quantity) <= 0m ? Convert.ToDecimal(TargetSetup.Amount) : 0m;
            row["EmployeeID"] = TargetSetup.EmployeeID;
            row["ConcernID"] = User.Identity.GetConcernId();
            row["CRUDStatus"] = 0;
            row["Status"] = 1;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["CreateDate"] = DateTime.Now;
            row["ModifiedBy"] = 0;
            row["ModifiedDate"] = DBNull.Value;
            dt.Rows.Add(row);

            return dt;
        }

        private void AddSaveModelError(TargetSetupViewModel TargetSetup, TargetSetupViewModel newTargetSetup, FormCollection collection)
        {

            if (!string.IsNullOrEmpty(collection["TargetMonth"]))
            {
                TargetSetup.TargetMonth = Convert.ToDateTime(collection["TargetMonth"]);
                var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
                if (TargetSetup.TargetMonth < DateRange.Item1)
                {
                    ModelState.AddModelError("TargetMonth", "Target Month can't be smaller than Salary Process Month.");
                }
            }
            else
            {
                ModelState.AddModelError("TargetMonth", "Target Month is required.");
            }

            if (string.IsNullOrEmpty(collection["EmployeesId"]))
            {
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            }
            else
            {
                newTargetSetup.EmployeeID = Convert.ToInt32(collection["EmployeesId"]);
                TargetSetup.EmployeeID = newTargetSetup.EmployeeID;
                int TID = Convert.ToInt32(TargetSetup.TID);
                if (_TargetSetupService.GetAllIQueryable().Any(i => i.EmployeeID == TargetSetup.EmployeeID && i.TargetMonth == TargetSetup.TargetMonth && i.TID != TID))
                {
                    ModelState.AddModelError("EmployeeID", "This month target is already given to this employee.");
                }
            }

            if (Convert.ToDecimal(TargetSetup.Amount) <= 0 && TargetSetup.TargetSetupDetailList.Count() == 0)
            {
                ModelState.AddModelError("Amount", "Both target amount and products can't be zero.");
            }

        }

        private void AddToTarget(TargetSetupViewModel TargetSetupVM, TargetSetupViewModel newTargetSetupVM, FormCollection collection)
        {
            var SelectedProducts = newTargetSetupVM.Products.Where(i => i.IsSelect).ToList();

            TargetSetupVM.TargetSetupDetailList.AddRange(SelectedProducts);

            var categorywiseproducts = from p in SelectedProducts
                                       group p by p.CategoryID into g
                                       select new CatergoryWiseDetailViewModel
                                           {
                                               CategoryID = g.Key,
                                               CategoryName = g.FirstOrDefault().CategoryName,
                                               Quantity = g.Sum(i => i.Quantity)
                                           };
            TargetSetupVM.CatergoryWiseDetails.AddRange(categorywiseproducts);
            TargetSetupVM.Quantity = TargetSetupVM.TargetSetupDetailList.Sum(i => i.Quantity).ToString();
            TempData["TargetSetup"] = TargetSetupVM;

            AddToastMessage("", "Products Added successfully.", ToastType.Success);
            TargetSetupVM.Products = new List<GetProductViewModel>();
        }
        bool IsEditValid(TargetSetup target)
        {
            if (target == null)
            {
                return false;
            }
            var TargetMonth = target.TargetMonth;
            var SysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            if (TargetMonth < DateRange.Item1)
            {
                AddToastMessage("TargetMonth", "This month salary is already processed. you can't edit/delete this target.");
                return false;
            }
            return true;
        }
        public ActionResult Edit(int TID, string previousAction, FormCollection collection)
        {

            if (TempData["TargetSetup"] == null)
            {
                var TargetSetup = _TargetSetupService.GetById(TID);

                if (!IsEditValid(TargetSetup))
                    return RedirectToAction("Index");

                var TargetSetupDetails = _TargetSetupService.GetTargetSetupDetailsById(TID);
                TargetSetupViewModel VmTargetSetup = _mapper.Map<TargetSetup, TargetSetupViewModel>(TargetSetup);
                var categorywiseproducts = (from ts in TargetSetupDetails
                                            join p in _ProductService.GetAllProductIQueryable() on ts.ProductID equals p.ProductID
                                            group p by p.CategoryID into g
                                            select new CatergoryWiseDetailViewModel
                                            {
                                                CategoryID = g.Key,
                                                CategoryName = g.FirstOrDefault().CategoryName,
                                                Quantity = g.Count()
                                            }).ToList();

                VmTargetSetup.CatergoryWiseDetails = categorywiseproducts;
                var SetupDetails = (from ts in TargetSetupDetails
                                    join p in _ProductService.GetAllProductIQueryable() on ts.ProductID equals p.ProductID
                                    select new ProductWisePurchaseModel
                                    {
                                        ProductCode = p.ProductCode,
                                        ProductID = p.ProductID,
                                        ProductName = p.ProductName,
                                        Quantity = ts.Quantity,
                                    }).ToList();

                var vmProducts = _mapper.Map<List<ProductWisePurchaseModel>, List<GetProductViewModel>>(SetupDetails);
                VmTargetSetup.TargetSetupDetailList = vmProducts;
                TempData["TargetSetup"] = VmTargetSetup;
                return RedirectToAction("Create");
            }
            else
            {
                return ReturnCreateViewWithTempData(collection);
            }

        }
        public ActionResult Delete(int TID)
        {
            if (_TargetSetupService.DeleteTargetSetupUsingSP(TID))
                AddToastMessage("", "Delete Succesfull.", ToastType.Success);
            else
                AddToastMessage("", "Delete Failed.", ToastType.Error);

            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize]
        [Route("deleteFromView/{id}")]
        public ActionResult DeleteFromView(int id, string previousAction)
        {
            TargetSetupViewModel TargetSetup = (TargetSetupViewModel)TempData.Peek("TargetSetup");
            if (TargetSetup == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");

            }

            List<GetProductViewModel> itemToDelete = TargetSetup.TargetSetupDetailList.Where(i => i.CategoryID == id).ToList();

            if (itemToDelete != null)
            {
                foreach (var item in itemToDelete)
                {
                    TargetSetup.TargetSetupDetailList.Remove(item);
                }

                var category = TargetSetup.CatergoryWiseDetails.FirstOrDefault(i => i.CategoryID == id);
                TargetSetup.CatergoryWiseDetails.Remove(category);
                TempData["TargetSetup"] = TargetSetup;
                TargetSetup.Products = new List<GetProductViewModel>();
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);

                //if (IsForEdit(previousAction))
                //    return RedirectToAction("Edit", new { TID = default(int), previousAction = "Edit" });
                //else
                return RedirectToAction("Create", TargetSetup);

            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                return RedirectToAction("Create");
            }
        }
        private bool IsForEdit(string previousAction)
        {
            return previousAction.ToLower().Equals("edit");
        }

    }
}
