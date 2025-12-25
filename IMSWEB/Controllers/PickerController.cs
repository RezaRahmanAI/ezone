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

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("picker")]
    public class PickerController : CoreController
    {
        ICategoryService _categoryService;
        IBankService _bankService;
        IColorService _colorService;
        ICompanyService _companyService;
        IGodownService _godownService;

        ICustomerService _customerService;
        ISupplierService _supplierService;
        IEmployeeService _employeeService;
        IProductService _productService;
        IMapper _mapper;
        IExpenseItemService _expenseItemService;
        IUserService _UserService;
        IAllowanceDeductionService _allowanceDeductionService;
        ISystemInformationService _SystemInformationService;
        ISalaryMonthlyService _SalaryMonthlyService;
        IDepartmentService _DepartmentService;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        IParentCategoryService _parentCategoryService;
        ISalesOrderService _salesOrderService;
        private readonly ISisterConcernService _sisterConcernService;


        public PickerController(
            IErrorService errorService, ICategoryService categoryService, IBankService bankService,
            IColorService colorService, ICompanyService companyService,
            IGodownService godownService,
            ICustomerService customerService,
            ISupplierService supplierService, IEmployeeService employeeService, IProductService productService,
            IUserService UserService,
            IExpenseItemService expenseItemService, IAllowanceDeductionService allowanceDeductionService,
            ISystemInformationService SystemInformationService, ISalaryMonthlyService SalaryMonthlyService,
            IDepartmentService DepartmentService, ISalesOrderService SalesOrderService,
            IMapper mapper, IShareInvestmentHeadService ShareInvestmentHeadService, IParentCategoryService parentCategoryService, ISisterConcernService sisterConcernService
            )
            : base(errorService, SystemInformationService)
        {
            _categoryService = categoryService;
            _bankService = bankService;
            _colorService = colorService;
            _companyService = companyService;
            _godownService = godownService;
            _customerService = customerService;
            _supplierService = supplierService;
            _employeeService = employeeService;
            _productService = productService;
            _mapper = mapper;
            _expenseItemService = expenseItemService;
            _UserService = UserService;
            _allowanceDeductionService = allowanceDeductionService;
            _SystemInformationService = SystemInformationService;
            _SalaryMonthlyService = SalaryMonthlyService;
            _DepartmentService = DepartmentService;
            _ShareInvestmentHeadService = ShareInvestmentHeadService;
            _parentCategoryService = parentCategoryService;
            _salesOrderService = SalesOrderService;
            _sisterConcernService = sisterConcernService;
        }

        [ChildActionOnly]
        public PartialViewResult RenderPicker(PickerType pickerType, int id)
        {
            switch (pickerType)
            {
                case PickerType.Category:

                    var categories = _categoryService.GetAllCategory();
                    var vmCategories = _mapper.Map<IEnumerable<Category>, IEnumerable<CreateCategoryViewModel>>(categories);
                    if (id != default(int))
                    {
                        var vmCategory = vmCategories.First(x => int.Parse(x.Id) == id);
                        ViewBag.CategoriesCode = vmCategory.Code;
                        ViewBag.CategoriesName = vmCategory.Name;
                        ViewBag.CategoriesId = vmCategory.Id;
                    }
                    return PartialView("~/Views/Pickers/_CategoryPicker.cshtml", vmCategories);
                case PickerType.PCategory:

                    var pc = _parentCategoryService.GetParentCategories();
                    var vmpcs = _mapper.Map<IEnumerable<ParentCategory>, IEnumerable<ParentCategoryViewModel>>(pc);
                    if (id != default(int))
                    {
                        var vmpc = vmpcs.First(x => x.PCategoryID == id);
                        ViewBag.PCategoriesCode = vmpc.Code;
                        ViewBag.PCategoriesName = vmpc.Name;
                        ViewBag.PCategoriesId = vmpc.PCategoryID;
                    }
                    return PartialView("~/Views/Pickers/_PCPicker.cshtml", vmpcs);
                case PickerType.Bank:
                    int concernId = User.Identity.GetConcernId();
                    SystemInformation systemInfo = _SystemInformationService.GetSystemInformationByConcernId(concernId);
                    if (systemInfo.IsCommonBank)
                        concernId = _sisterConcernService.GetParentConcernId(concernId);

                    var Banks = _bankService.GetAll(concernId);
                    var vmBanks = _mapper.Map<IEnumerable<Bank>, IEnumerable<CreateBankViewModel>>(Banks);
                    if (id != default(int))
                    {
                        var vmBank = vmBanks.First(x => int.Parse(x.Id) == id);
                        ViewBag.BanksCode = vmBank.Code;
                        ViewBag.BanksName = vmBank.BankName;
                        ViewBag.BanksId = vmBank.Id;
                    }
                    return PartialView("~/Views/Pickers/_BankPicker.cshtml", vmBanks);

                case PickerType.AnotherBank:

                    int concernId2 = User.Identity.GetConcernId();
                    SystemInformation systemInfo2 = _SystemInformationService.GetSystemInformationByConcernId(concernId2);
                    if (systemInfo2.IsCommonBank)
                        concernId2 = _sisterConcernService.GetParentConcernId(concernId2);

                    var AnotherBanks = _bankService.GetAll(concernId2);
                    var vmAnotherBanks = _mapper.Map<IEnumerable<Bank>, IEnumerable<CreateBankViewModel>>(AnotherBanks);
                    if (id != default(int))
                    {
                        var vmAnotherBank = vmAnotherBanks.First(x => int.Parse(x.Id) == id);
                        ViewBag.AnotherBanksCode = vmAnotherBank.Code;
                        ViewBag.AnotherBanksName = vmAnotherBank.BankName;
                        ViewBag.AnotherBanksId = vmAnotherBank.Id;
                    }
                    return PartialView("~/Views/Pickers/_AnotherBankPicker.cshtml", vmAnotherBanks);

                case PickerType.Company:

                    var companies = _companyService.GetAllCompany();
                    var vmCompanies = _mapper.Map<IEnumerable<Company>, IEnumerable<CreateCompanyViewModel>>(companies);
                    if (id != default(int))
                    {
                        var vmCompany = vmCompanies.First(x => int.Parse(x.Id) == id);
                        ViewBag.CompaniesCode = vmCompany.Code;
                        ViewBag.CompaniesName = vmCompany.Name;
                        ViewBag.CompaniesId = vmCompany.Id;
                    }
                    return PartialView("~/Views/Pickers/_CompanyPicker.cshtml", vmCompanies);

                case PickerType.Color:

                    var colors = _colorService.GetAllColor();
                    var vmColors = _mapper.Map<IEnumerable<Color>, IEnumerable<CreateColorViewModel>>(colors);
                    if (id != default(int))
                    {
                        var vmColor = vmColors.First(x => int.Parse(x.Id) == id);
                        ViewBag.ColorsCode = vmColor.Code;
                        ViewBag.ColorsName = vmColor.Name;
                        ViewBag.ColorsId = vmColor.Id;
                    }
                    return PartialView("~/Views/Pickers/_ColorPicker.cshtml", vmColors);

                case PickerType.SisterConcern:

                    var allConcern = _sisterConcernService.GetAllConcernByConcernId(User.Identity.GetConcernId());
                    if (id != default(int))
                    {
                        var vmColor = allConcern.First(x => x.Id == id);
                        ViewBag.ConcernName = vmColor.Name;
                        ViewBag.ConcernId = vmColor.Id;
                        ViewBag.ServiceCharge = vmColor.ServiceCharge;
                    }
                    return PartialView("~/Views/Pickers/_ConcernPicker.cshtml", allConcern);

                case PickerType.Godown:

                    var godowns = _godownService.GetAllGodown();
                    var vmGodowns = _mapper.Map<IEnumerable<Godown>, IEnumerable<CreateGodownViewModel>>(godowns);
                    if (id != default(int))
                    {
                        var vmGodown = vmGodowns.First(x => int.Parse(x.Id) == id);
                        ViewBag.GodownsCode = vmGodown.Code;
                        ViewBag.GodownsName = vmGodown.Name;
                        ViewBag.GodownsId = vmGodown.Id;
                    }
                    return PartialView("~/Views/Pickers/_GodownPicker.cshtml", vmGodowns);
                case PickerType.GodownByConcern:

                    if (id != default(int))
                    {
                        var vmGodown = _godownService.GetGodownById(id);
                        ViewBag.GodownsCode = vmGodown.Code;
                        ViewBag.GodownsName = vmGodown.Name;
                        ViewBag.GodownsId = vmGodown.GodownID;
                        ViewBag.GodownsConcernName = vmGodown.SisterConcern.Name;

                    }
                    return PartialView("~/Views/Pickers/_GodownPickerByConcern.cshtml");
                case PickerType.Customer:

                    int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int EmpID = 0;
                    string userAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                        var user = _UserService.GetUserById(userId);
                        EmpID = user.EmployeeID;

                        var customers = _customerService.GetAllCustomerByEmp(EmpID);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomerType = vmCustomer.CustomerType;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!userAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomerType = vmCustomer.CustomerType;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!userAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                case PickerType.ExceptCreditCustomer:

                    int CuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int CEmpID = 0;
                    string CuserAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //CEmpID = ConstantData.GetEmployeeIDByUSerID(CuserId);
                        var user = _UserService.GetUserById(CuserId);
                        CEmpID = user.EmployeeID;
                        var Ccustomers = _customerService.GetAllCustomerByEmp(CEmpID);//.Where(i => i.CustomerType != EnumCustomerType.Hire);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomerType = vmCustomer.CustomerType;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        //if (!CuserAgent.ToLower().Contains("windows"))
                        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();//.Where(i => i.CustomerType != EnumCustomerType.Hire);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomerType = vmCustomer.CustomerType;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                            return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);
                        }
                        //if (!CuserAgent.ToLower().Contains("windows"))
                        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml");

                    }

                case PickerType.CreditCustomer:

                    int CreditCuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int CreditCEmpID = 0;
                    string CreditCuserAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //CreditCEmpID = ConstantData.GetEmployeeIDByUSerID(CreditCuserId);
                        var user = _UserService.GetUserById(CreditCuserId);
                        CreditCEmpID = user.EmployeeID;
                        var Ccustomers = _customerService.GetAllCustomerByEmp(CreditCEmpID).Where(i => i.CustomerType != EnumCustomerType.Dealer);
                        //|| i.CustomerType == EnumCustomerType.Project || i.CustomerType == EnumCustomerType.Mela);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = vmCustomer.CreditDue;
                            ViewBag.CustomerType = vmCustomer.CustomerType;
                        }
                        //if (!CreditCuserAgent.ToLower().Contains("windows"))
                        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {

                        if (id != default(int))
                        {
                            var customers = _customerService.GetAllCustomer().Where(i => i.CustomerType != EnumCustomerType.Dealer);
                            //|| i.CustomerType == EnumCustomerType.Project || i.CustomerType == EnumCustomerType.Mela);
                            var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = vmCustomer.CreditDue;
                            ViewBag.CustomerType = vmCustomer.CustomerType;

                            return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml", vmCustomers);
                        }
                        //if (!CreditCuserAgent.ToLower().Contains("windows"))
                        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml");

                    }
                //case PickerType.CreditCustomer:

                //    int CreditCuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                //    int CreditCEmpID = 0;
                //    string CreditCuserAgent = Request.UserAgent;

                //    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                //    {

                //        //CreditCEmpID = ConstantData.GetEmployeeIDByUSerID(CreditCuserId);
                //        var user = _UserService.GetUserById(CreditCuserId);
                //        CreditCEmpID = user.EmployeeID;
                //        var Ccustomers = _customerService.GetAllCustomerByEmp(CreditCEmpID).Where(i => i.CustomerType != EnumCustomerType.Dealer);
                //        //|| i.CustomerType == EnumCustomerType.Project || i.CustomerType == EnumCustomerType.Mela);
                //        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers);
                //        if (id != default(int))
                //        {
                //            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                //            ViewBag.CustomersCode = vmCustomer.Code;
                //            ViewBag.CustomersName = vmCustomer.Name;
                //            ViewBag.CustomersId = vmCustomer.Id;
                //            ViewBag.CustomersCurrentDue = vmCustomer.CreditDue;
                //            ViewBag.CustomerType = vmCustomer.CustomerType;
                //        }
                //        //if (!CreditCuserAgent.ToLower().Contains("windows"))
                //        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                //        return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml", vmCustomers);

                //    }
                //    else
                //    {

                //        if (id != default(int))
                //        {
                //            var customers = _customerService.GetAllCustomer().Where(i => i.CustomerType != EnumCustomerType.Dealer);
                //            //|| i.CustomerType == EnumCustomerType.Project || i.CustomerType == EnumCustomerType.Mela);
                //            var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                //            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                //            ViewBag.CustomersCode = vmCustomer.Code;
                //            ViewBag.CustomersName = vmCustomer.Name;
                //            ViewBag.CustomersId = vmCustomer.Id;
                //            ViewBag.CustomersCurrentDue = vmCustomer.CreditDue;
                //            ViewBag.CustomerType = vmCustomer.CustomerType;

                //            return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml", vmCustomers);
                //        }
                //        //if (!CreditCuserAgent.ToLower().Contains("windows"))
                //        //    return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                //        return PartialView("~/Views/Pickers/_CreditCustomerPicker.cshtml");

                //    }

                case PickerType.CustomerPostLoad:
                    return PartialView("~/Views/Pickers/_CustomerPickerPostLoad.cshtml");

                case PickerType.Supplier:

                    var suppliers = _supplierService.GetAllSupplier();
                    var vmSuppliers = _mapper.Map<IEnumerable<Supplier>, IEnumerable<CreateSupplierViewModel>>(suppliers);
                    if (id != default(int))
                    {
                        var vmSupplier = vmSuppliers.First(x => int.Parse(x.Id) == id);
                        ViewBag.SuppliersCode = vmSupplier.Code;
                        ViewBag.SuppliersName = vmSupplier.Name;
                        ViewBag.SuppliersId = vmSupplier.Id;
                        ViewBag.SuppliersCurrentDue = !string.IsNullOrEmpty(vmSupplier.TotalDue) ?
                            Math.Round(decimal.Parse(vmSupplier.TotalDue), 2).ToString() : string.Empty;
                    }
                    return PartialView("~/Views/Pickers/_SupplierPicker.cshtml", vmSuppliers);

                case PickerType.Employee:

                    var employees = _employeeService.GetAllEmployee();
                    var vmEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<CreateEmployeeViewModel>>(employees);
                    if (id != default(int))
                    {
                        var vmEmployee = vmEmployees.First(x => int.Parse(x.Id) == id);
                        ViewBag.EmployeesCode = vmEmployee.Code;
                        ViewBag.EmployeesName = vmEmployee.Name;
                        ViewBag.EmployeesId = vmEmployee.Id;
                    }
                    return PartialView("~/Views/Pickers/_EmployeePicker.cshtml", vmEmployees);

                case PickerType.Product:
                    var customProducts = _productService.GetAllProduct();
                    var vmProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>>, IEnumerable<GetProductViewModel>>(customProducts);
                    var groupProducts = from p in vmProducts
                                        group p by new
                                        {
                                            p.ProductId,
                                            p.ProductCode,
                                            p.ProductName,
                                            p.ProductType,
                                            p.CategoryID,
                                            p.CategoryName,
                                            p.CompanyName,
                                        } into g
                                        select new GetProductViewModel
                                        {
                                            ProductId = g.Key.ProductId,
                                            ProductCode = g.Key.ProductCode,
                                            ProductName = g.Key.ProductName,
                                            CompanyName = g.Key.CompanyName,
                                            CategoryName = g.Key.CategoryName,
                                            ProductType = g.Key.ProductType,
                                            PreStock = g.Sum(i => i.PreStock),
                                            MRPRate = g.Select(i => i.MRPRate).FirstOrDefault(),
                                            MRPRate12 = g.Select(i => i.MRPRate12).FirstOrDefault(),
                                            CashSalesRate = g.Select(i => i.CashSalesRate).FirstOrDefault(),
                                            PRate = g.Select(i => i.PRate).FirstOrDefault()
                                        };
                    if (id != default(int))
                    {
                        var vmProduct = vmProducts.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsPreviousStock = vmProduct.PreStock;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductPicker.cshtml", groupProducts);

                case PickerType.POProductPicker:
                    if (id != default(int))
                    {
                        var vmProduct = _productService.GetAllProductIQueryablePO().First(x => x.ProductID == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = vmProduct.Quantity;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;

                    }
                    return PartialView("~/Views/Pickers/_POProductPicker.cshtml");

                case PickerType.EmobileProduct:

                    var customDistinctProducts = _productService.GetAllUniqueProduct();
                    var vmDistinctProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, int, decimal>>>, IEnumerable<GetProductViewModel>>(customDistinctProducts);

                    if (id != default(int))
                    {
                        var vmProduct = vmDistinctProducts.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsPreviousStock = vmProduct.PreStock;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_EmobileProductPicker.cshtml", vmDistinctProducts);

                case PickerType.ECOMProduct:

                    //var customECOMProducts = _productService.GetAllProduct();
                    //var vmECOMProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>>, IEnumerable<GetProductViewModel>>(customECOMProducts);

                    var customECOMProducts = _productService.GetAllECOMProduct();
                    var vmECOMProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>>, IEnumerable<GetProductViewModel>>(customECOMProducts);

                    if (id != default(int))
                    {
                        var vmProduct = vmECOMProducts.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsPreviousStock = vmProduct.PreStock;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                        //ViewBag.CompanyName = vmProduct.CompanyName;
                    }
                    return PartialView("~/Views/Pickers/_ECOMProductPicker.cshtml", vmECOMProducts);

                case PickerType.ProductDetail:

                    ViewBag.Type = "Details";
                    //var customProductDetails = _productService.GetAllProductFromDetail();
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml");

                case PickerType.DateWiseProductDetail:

                    ViewBag.Type = "Details";
                    //var customProductDetails = _productService.GetAllProductFromDetail();
                    int DwConcernID = User.Identity.GetConcernId();
                    var vmDProductDetails = _salesOrderService.DateWiseProductPickerInStock(DwConcernID);


                    var vmDProductGroupBY = (from vm in vmDProductDetails
                                            group vm by new
                                            {
                                                vm.IMENO,
                                                vm.ProductID,
                                                vm.CategoryID,
                                                vm.ProductName,
                                                vm.ProductCode,
                                                vm.ColorID,
                                                vm.GodownID,
                                                vm.CategoryName,
                                                vm.ColorName,
                                                vm.GodownName,
                                                //vm.OrderDate
                                            }
                                            into g
                                            select new GetProductViewModel
                                            {
                                                IMENo = g.Key.IMENO,
                                                ProductId = g.Key.ProductID,
                                                ProductCode = g.Key.ProductCode,
                                                ProductName = g.Key.ProductName,
                                                CategoryID = g.Key.CategoryID,
                                                //OrderDate = g.Key.OrderDate.ToString("dd MMM yyyy"),
                                                OrderDate = g.Select(o => o.OrderDate.ToString("dd MMM yyyy")).FirstOrDefault(),

                                                CategoryName = g.Key.CategoryName,
                                                ColorName = g.Key.ColorName,
                                                ColorId = g.Key.ColorID,
                                                GodownID = g.Key.GodownID,
                                                StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),
                                                PreStock = g.Sum(o => o.PreStock),

                                                MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                GodownName = g.Key.GodownName,
                                                ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                                Service = g.Select(o => o.Service).FirstOrDefault(),
                                            }).OrderBy(i => i.ProductName).ToList();



                    if (id != default(int))
                    {
                        var vmProduct = vmDProductDetails.FirstOrDefault(x => x.ProductID == id);
                        if (vmProduct != null)
                        {
                            ViewBag.PDetailsCode = vmProduct.ProductCode;
                            ViewBag.PDetailsName = vmProduct.ProductName;
                            ViewBag.PDetailsId = vmProduct.ProductID;
                            ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                            ViewBag.PDetailsColorId = vmProduct.ColorID;
                        }
                        else
                        {
                            AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                        }
                    }
                    return PartialView("~/Views/Pickers/_DateWiseProductDetailPicker.cshtml", vmDProductGroupBY);

                case PickerType.ECOMProductDetail:

                    ViewBag.Type = "Details";
                    //var customProductDetails = _productService.GetAllProductFromDetail();
                    int ECOMConcernID = User.Identity.GetConcernId();
                    var vmECOMProductDetails = _salesOrderService.ProductPickerInStock(ECOMConcernID);

                    //var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    //Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails).ToList();


                    var vmECOMProductGroupBY = (from vm in vmECOMProductDetails
                                                group vm by new
                                                {
                                                    vm.IMENO,
                                                    vm.ProductID,
                                                    vm.CategoryID,
                                                    vm.ProductName,
                                                    vm.ProductCode,
                                                    vm.ColorID,
                                                    vm.GodownID,
                                                    vm.CategoryName,
                                                    vm.ColorName,
                                                    vm.GodownName,
                                                    vm.CompanyName
                                                }
                                            into g
                                                select new GetProductViewModel
                                                {
                                                    IMENo = g.Key.IMENO,
                                                    ProductId = g.Key.ProductID,
                                                    ProductCode = g.Key.ProductCode,
                                                    ProductName = g.Key.ProductName,
                                                    CategoryID = g.Key.CategoryID,

                                                    CategoryName = g.Key.CategoryName,
                                                    CompanyName = g.Key.CompanyName,
                                                    ColorName = g.Key.ColorName,
                                                    ColorId = g.Key.ColorID,
                                                    GodownID = g.Key.GodownID,
                                                    StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),
                                                    PreStock = g.Sum(o => o.PreStock),

                                                    MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                    PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                    Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                    GodownName = g.Key.GodownName,
                                                    ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                                    Service = g.Select(o => o.Service).FirstOrDefault(),
                                                }).OrderBy(i => i.ProductName).ToList();



                    if (id != default(int))
                    {
                        var vmProduct = vmECOMProductDetails.FirstOrDefault(x => x.ProductID == id);
                        if (vmProduct != null)
                        {
                            ViewBag.PDetailsCode = vmProduct.ProductCode;
                            ViewBag.PDetailsName = vmProduct.ProductName;
                            ViewBag.PDetailsId = vmProduct.ProductID;
                            ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                            ViewBag.PDetailsColorId = vmProduct.ColorID;
                        }
                        else
                        {
                            AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                        }
                    }
                    return PartialView("~/Views/Pickers/_ECOMProductDetailPicker.cshtml", vmECOMProductGroupBY);

                case PickerType.OLDProductDetail:

                    ViewBag.Type = "Details";
                    //var customProductDetails = _productService.GetAllProductFromDetail();
                    int OConcernID = User.Identity.GetConcernId();
                    var vmOProductDetails = _salesOrderService.ProductPickerInStock(OConcernID);

                    //var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    //Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails).ToList();

                    #region OLD Code
                    //var vmProductGroupBY = (from vm in vmProductDetails
                    //                        group vm by new
                    //                        {
                    //                            vm.IMENo,
                    //                            vm.ProductId,
                    //                            vm.CategoryID,
                    //                            vm.ProductName,
                    //                            vm.ProductCode,
                    //                            vm.ColorId,
                    //                            vm.CategoryName,
                    //                            vm.ColorName,
                    //                            vm.ModelName,
                    //                            vm.GodownName
                    //                        }
                    //                            into g
                    //                        select new GetProductViewModel
                    //                        {
                    //                            IMENo = g.Key.IMENo,
                    //                            ProductId = g.Key.ProductId,
                    //                            ProductCode = g.Key.ProductCode,
                    //                            ProductName = g.Key.ProductName,
                    //                            CategoryID = g.Key.CategoryID,

                    //                            CategoryName = g.Key.CategoryName,
                    //                            ColorName = g.Key.ColorName,
                    //                            ColorId = g.Key.ColorId,
                    //                            ModelName = g.Key.ModelName,
                    //                            StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                    //                            MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                    //                            MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                    //                            CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                    //                            PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                    //                            PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                    //                            PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                    //                            OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                    //                            ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                    //                            CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                    //                            PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                    //                            MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                    //                            SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                    //                            ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                    //                            IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                    //                            Status = g.Select(o => o.Status).FirstOrDefault(),

                    //                            Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                    //                            GodownName = g.Key.GodownName,
                    //                            PRate = g.Select(i => i.PRate).FirstOrDefault()
                    //                        }).OrderBy(i => i.ProductName).ToList();



                    //if (id != default(int))
                    //{
                    //    var vmProduct = vmProductDetails.FirstOrDefault(x => x.ProductId == id);
                    //    if (vmProduct != null)
                    //    {
                    //        ViewBag.PDetailsCode = vmProduct.ProductCode;
                    //        ViewBag.PDetailsName = vmProduct.ProductName;
                    //        ViewBag.PDetailsId = vmProduct.ProductId;
                    //        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                    //        ViewBag.PDetailsColorId = vmProduct.ColorId;
                    //        ViewBag.OfferDescription = vmProduct.OfferDescription;
                    //        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;
                    //    }
                    //    else
                    //    {
                    //        AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                    //    }
                    //}
                    #endregion
                    var vmOProductGroupBY = (from vm in vmOProductDetails
                                             group vm by new
                                             {
                                                 vm.IMENO,
                                                 vm.ProductID,
                                                 vm.CategoryID,
                                                 vm.ProductName,
                                                 vm.ProductCode,
                                                 vm.ColorID,
                                                 vm.GodownID,
                                                 vm.CategoryName,
                                                 vm.ColorName,
                                                 vm.GodownName
                                             }
                                            into g
                                             select new GetProductViewModel
                                             {
                                                 IMENo = g.Key.IMENO,
                                                 ProductId = g.Key.ProductID,
                                                 ProductCode = g.Key.ProductCode,
                                                 ProductName = g.Key.ProductName,
                                                 CategoryID = g.Key.CategoryID,

                                                 CategoryName = g.Key.CategoryName,
                                                 ColorName = g.Key.ColorName,
                                                 ColorId = g.Key.ColorID,
                                                 GodownID = g.Key.GodownID,
                                                 StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),
                                                 PreStock = g.Sum(o => o.PreStock),

                                                 MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                 PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                 Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                 GodownName = g.Key.GodownName,
                                                 ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                             }).OrderBy(i => i.ProductName).ToList();



                    if (id != default(int))
                    {
                        var vmProduct = vmOProductDetails.FirstOrDefault(x => x.ProductID == id);
                        if (vmProduct != null)
                        {
                            ViewBag.PDetailsCode = vmProduct.ProductCode;
                            ViewBag.PDetailsName = vmProduct.ProductName;
                            ViewBag.PDetailsId = vmProduct.ProductID;
                            ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                            ViewBag.PDetailsColorId = vmProduct.ColorID;
                        }
                        else
                        {
                            AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                        }
                    }
                    return PartialView("~/Views/Pickers/_OldProductDetailPicker.cshtml", vmOProductGroupBY);

                case PickerType.ProductDetailForBarcodeScan: // For TR Electronics Barcode Sales

                    ViewBag.Type = "Details";
                    //var customProductDetails = _productService.GetAllProductFromDetail();
                    int ConcernId = User.Identity.GetConcernId();
                    var vmProductDetails_ = _salesOrderService.ProductPickerInStock(0);


                    //var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    //Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails).ToList();

                    #region OLD Code
                    //var vmProductGroupBY = (from vm in vmProductDetails
                    //                        group vm by new
                    //                        {
                    //                            vm.IMENo,
                    //                            vm.ProductId,
                    //                            vm.CategoryID,
                    //                            vm.ProductName,
                    //                            vm.ProductCode,
                    //                            vm.ColorId,
                    //                            vm.CategoryName,
                    //                            vm.ColorName,
                    //                            vm.ModelName,
                    //                            vm.GodownName
                    //                        }
                    //                            into g
                    //                        select new GetProductViewModel
                    //                        {
                    //                            IMENo = g.Key.IMENo,
                    //                            ProductId = g.Key.ProductId,
                    //                            ProductCode = g.Key.ProductCode,
                    //                            ProductName = g.Key.ProductName,
                    //                            CategoryID = g.Key.CategoryID,

                    //                            CategoryName = g.Key.CategoryName,
                    //                            ColorName = g.Key.ColorName,
                    //                            ColorId = g.Key.ColorId,
                    //                            ModelName = g.Key.ModelName,
                    //                            StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                    //                            MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                    //                            MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                    //                            CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                    //                            PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                    //                            PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                    //                            PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                    //                            OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                    //                            ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                    //                            CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                    //                            PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                    //                            MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                    //                            SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                    //                            ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                    //                            IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                    //                            Status = g.Select(o => o.Status).FirstOrDefault(),

                    //                            Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                    //                            GodownName = g.Key.GodownName,
                    //                            PRate = g.Select(i => i.PRate).FirstOrDefault()
                    //                        }).OrderBy(i => i.ProductName).ToList();



                    //if (id != default(int))
                    //{
                    //    var vmProduct = vmProductDetails.FirstOrDefault(x => x.ProductId == id);
                    //    if (vmProduct != null)
                    //    {
                    //        ViewBag.PDetailsCode = vmProduct.ProductCode;
                    //        ViewBag.PDetailsName = vmProduct.ProductName;
                    //        ViewBag.PDetailsId = vmProduct.ProductId;
                    //        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                    //        ViewBag.PDetailsColorId = vmProduct.ColorId;
                    //        ViewBag.OfferDescription = vmProduct.OfferDescription;
                    //        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;
                    //    }
                    //    else
                    //    {
                    //        AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                    //    }
                    //}
                    #endregion
                    var vmProductGroupBY_ = (from vm in vmProductDetails_
                                             group vm by new
                                             {
                                                 vm.IMENO,
                                                 vm.ProductID,
                                                 vm.CategoryID,
                                                 vm.ProductName,
                                                 vm.ProductCode,
                                                 vm.ColorID,
                                                 vm.CategoryName,
                                                 vm.ColorName,
                                                 vm.GodownName
                                             }
                                            into g
                                             select new GetProductViewModel
                                             {
                                                 IMENo = g.Key.IMENO,
                                                 ProductId = g.Key.ProductID,
                                                 ProductCode = g.Key.ProductCode,
                                                 ProductName = g.Key.ProductName,
                                                 CategoryID = g.Key.CategoryID,

                                                 CategoryName = g.Key.CategoryName,
                                                 ColorName = g.Key.ColorName,
                                                 ColorId = g.Key.ColorID,
                                                 StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                 MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                 PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                 //Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                 GodownName = g.Key.GodownName,
                                             }).OrderBy(i => i.ProductName).ToList();



                    if (id != default(int))
                    {
                        var vmProduct = vmProductDetails_.FirstOrDefault(x => x.ProductID == id);
                        if (vmProduct != null)
                        {
                            ViewBag.PDetailsCode = vmProduct.ProductCode;
                            ViewBag.PDetailsName = vmProduct.ProductName;
                            ViewBag.PDetailsId = vmProduct.ProductID;
                            ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                            ViewBag.PDetailsColorId = vmProduct.ColorID;
                        }
                        else
                        {
                            AddToastMessage("", "This product is not avaiable in stock. Please delete and save first.", ToastType.Error);
                        }
                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmProductGroupBY_);




                case PickerType.DamageProductDetail:

                    ViewBag.Type = "Details";
                    var DamagecustomProductDetails = _productService.GetAllDamageProductFromDetail();
                    var vmDamageProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string>>>>, IEnumerable<GetProductViewModel>>(DamagecustomProductDetails).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = vmDamageProductDetails.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmDamageProductDetails);
                case PickerType.CreditProductDetail:

                    ViewBag.Type = "Details";
                    //var cproductdetails = _productService.GetAllProductFromDetailForCredit();
                    //var vmcreditpdeatils = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>>, IEnumerable<GetProductViewModel>>(cproductdetails).ToList();
                    int concernIdCredit = User.Identity.GetConcernId();
                    var vmcreditpdeatils = _salesOrderService.ProductPickerInStock(concernIdCredit);


                    #region OLD Code
                    //var vmCreditProductGroupBY = (from vm in vmcreditpdeatils
                    //                              group vm by new { vm.IMENo, vm.ProductId, vm.CategoryID, vm.ProductName, vm.ProductCode, vm.ColorId, vm.CategoryName, vm.ColorName, vm.ModelName, vm.GodownName }
                    //                                  into g
                    //                              select new GetProductViewModel
                    //                              {
                    //                                  IMENo = g.Key.IMENo,
                    //                                  ProductId = g.Key.ProductId,
                    //                                  ProductCode = g.Key.ProductCode,
                    //                                  ProductName = g.Key.ProductName,
                    //                                  CategoryID = g.Key.CategoryID,

                    //                                  CategoryName = g.Key.CategoryName,
                    //                                  ColorName = g.Key.ColorName,
                    //                                  ColorId = g.Key.ColorId,
                    //                                  ModelName = g.Key.ModelName,
                    //                                  StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                    //                                  MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                    //                                  MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                    //                                  CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                    //                                  PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                    //                                  PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                    //                                  PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                    //                                  OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                    //                                  ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                    //                                  CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                    //                                  PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                    //                                  MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                    //                                  SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                    //                                  ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                    //                                  IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                    //                                  Status = g.Select(o => o.Status).FirstOrDefault(),

                    //                                  Quantity = g.Select(o => o.Quantity).FirstOrDefault(),
                    //                                  GodownName = g.Key.GodownName

                    //                              });

                    #endregion


                    var vmProductGroupbY_ = (from vm in vmcreditpdeatils
                                             group vm by new
                                             {
                                                 vm.IMENO,
                                                 vm.ProductID,
                                                 vm.CategoryID,
                                                 vm.ProductName,
                                                 vm.ProductCode,
                                                 vm.ColorID,
                                                 vm.CategoryName,
                                                 vm.ColorName,
                                                 vm.GodownName
                                             }
                                           into g
                                             select new GetProductViewModel
                                             {
                                                 IMENo = g.Key.IMENO,
                                                 ProductId = g.Key.ProductID,
                                                 ProductCode = g.Key.ProductCode,
                                                 ProductName = g.Key.ProductName,
                                                 CategoryID = g.Key.CategoryID,

                                                 CategoryName = g.Key.CategoryName,
                                                 ColorName = g.Key.ColorName,
                                                 ColorId = g.Key.ColorID,
                                                 StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                 MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                 PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                 //Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                 GodownName = g.Key.GodownName,
                                             }).OrderBy(i => i.ProductName).ToList();





                    if (id != default(int))
                    {
                        var vmProduct = vmcreditpdeatils.First(x => x.ProductID == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductID;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorID;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductID).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_CreditProductDetailPicker.cshtml", vmProductGroupbY_);

                case PickerType.DateWiseCreditProductDetail:

                    ViewBag.Type = "Details";
                    int dconcernIdCredit = User.Identity.GetConcernId();
                    var vmdcreditpdeatils = _salesOrderService.DateWiseProductPickerInStock(dconcernIdCredit);



                    var vmdProductGroupbY_ = (from vm in vmdcreditpdeatils
                                             group vm by new
                                             {
                                                 vm.IMENO,
                                                 vm.ProductID,
                                                 vm.CategoryID,
                                                 vm.ProductName,
                                                 vm.ProductCode,
                                                 vm.ColorID,
                                                 vm.CategoryName,
                                                 vm.ColorName,
                                                 vm.GodownName
                                             }
                                           into g
                                             select new GetProductViewModel
                                             {
                                                 IMENo = g.Key.IMENO,
                                                 ProductId = g.Key.ProductID,
                                                 ProductCode = g.Key.ProductCode,
                                                 ProductName = g.Key.ProductName,
                                                 CategoryID = g.Key.CategoryID,
                                                 OrderDate = g.Select(o => o.OrderDate.ToString("dd MMM yyyy")).FirstOrDefault(),

                                                 CategoryName = g.Key.CategoryName,
                                                 ColorName = g.Key.ColorName,
                                                 ColorId = g.Key.ColorID,
                                                 StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                 MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                 PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                 //Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                 GodownName = g.Key.GodownName,
                                             }).OrderBy(i => i.ProductName).ToList();





                    if (id != default(int))
                    {
                        var vmProduct = vmdcreditpdeatils.First(x => x.ProductID == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductID;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorID;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductID).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_DateWiseCreditProductDetailPicker.cshtml", vmdProductGroupbY_);

                case PickerType.CreditProductDetailForBarcodeScan:

                    ViewBag.Type = "Details";
                    //var cproductdetails = _productService.GetAllProductFromDetailForCredit();
                    //var vmcreditpdeatils = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>>, IEnumerable<GetProductViewModel>>(cproductdetails).ToList();
                    int concernId_ = User.Identity.GetConcernId();
                    var vmcreditpdeatils_ = _salesOrderService.ProductPickerInStock(0);

                    var vmProductGroupbY = (from vm in vmcreditpdeatils_
                                            group vm by new
                                            {
                                                vm.IMENO,
                                                vm.ProductID,
                                                vm.CategoryID,
                                                vm.ProductName,
                                                vm.ProductCode,
                                                vm.ColorID,
                                                vm.CategoryName,
                                                vm.ColorName,
                                                vm.GodownName
                                            }
                                           into g
                                            select new GetProductViewModel
                                            {
                                                IMENo = g.Key.IMENO,
                                                ProductId = g.Key.ProductID,
                                                ProductCode = g.Key.ProductCode,
                                                ProductName = g.Key.ProductName,
                                                CategoryID = g.Key.CategoryID,

                                                CategoryName = g.Key.CategoryName,
                                                ColorName = g.Key.ColorName,
                                                ColorId = g.Key.ColorID,
                                                StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                                //Quantity = g.Select(o => o.ProductType).FirstOrDefault() == (int)EnumProductType.NoBarcode ? g.Select(o => o.PreStock).FirstOrDefault() : 1,
                                                GodownName = g.Key.GodownName,
                                            }).OrderBy(i => i.ProductName).ToList();





                    if (id != default(int))
                    {
                        var vmProduct = vmcreditpdeatils_.First(x => x.ProductID == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductID;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorID;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductID).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_CreditProductDetailPicker.cshtml", vmProductGroupbY);



                case PickerType.SRProductDetail:

                    ViewBag.Type = "Details";
                    int SRUID = User.Identity.GetUserId<int>();

                    int SRID = 0;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {
                        //SRID = ConstantData.GetEmployeeIDByUSerID(SRUID);
                        var user = _UserService.GetUserById(SRUID);
                        SRID = user.EmployeeID;
                    }
                    var srProducts = _productService.SRWiseGetAllProductFromDetail(SRID);
                    var vmsrProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(srProducts).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = vmsrProducts.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmsrProducts);
                case PickerType.ProductDetailMobile:

                    ViewBag.Type = "Details";
                    //var customProductDetailsMobile = _productService.GetAllProductFromDetail();
                    //var vmProductDetailsMobile = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(customProductDetailsMobile);
                    int MSRUID = User.Identity.GetUserId<int>();

                    int MSRID = 0;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {
                        //MSRID = ConstantData.GetEmployeeIDByUSerID(MSRUID);
                        var user = _UserService.GetUserById(MSRUID);
                        MSRID = user.EmployeeID;
                    }
                    var MsrProducts = _productService.SRWiseGetAllProductFromDetail(MSRID);
                    var MvmsrProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(MsrProducts).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = MvmsrProducts.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.mobile.cshtml", MvmsrProducts);
                case PickerType.SalesProductDetail:

                    ViewBag.Type = "Details";
                    var salesProductDetailsMobile = _productService.GetAllProductFromDetail();
                    var vmsalesProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>>, IEnumerable<GetProductViewModel>>(salesProductDetailsMobile);

                    if (id != default(int))
                    {
                        var vmProduct = vmsalesProductDetails.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;

                    }
                    return PartialView("~/Views/Pickers/_SalesProductDetailPicker.cshtml", vmsalesProductDetails);

                case PickerType.ExpenseItemHead:

                    ViewBag.Type = "ExpenseItem";
                    var expenseItems = _expenseItemService.GetAllExpenseItem().Where(i => i.Status == EnumCompanyTransaction.Expense).ToList();

                    if (id != default(int))
                    {
                        var expensitem = expenseItems.FirstOrDefault(i => i.ExpenseItemID == id);
                        ViewBag.ExpenseItemsId = expensitem.ExpenseItemID;
                        ViewBag.ExpenseItemsCode = expensitem.Code;
                        ViewBag.ExpensItemsName = expensitem.Description;

                    }
                    return PartialView("~/Views/Pickers/_ExpenseItemsPicker.cshtml", expenseItems);
                case PickerType.IncomeItemHead:

                    ViewBag.Type = "IncomeItem";
                    var incomeItems = _expenseItemService.GetAllExpenseItem().Where(i => i.Status == EnumCompanyTransaction.Income).ToList();

                    if (id != default(int))
                    {
                        var Incomeitem = incomeItems.FirstOrDefault(i => i.ExpenseItemID == id);
                        ViewBag.ExpenseItemsId = Incomeitem.ExpenseItemID;
                        ViewBag.ExpenseItemsCode = Incomeitem.Code;
                        ViewBag.ExpensItemsName = Incomeitem.Description;

                    }
                    return PartialView("~/Views/Pickers/_ExpenseItemsPicker.cshtml", incomeItems);
                case PickerType.IncomeItemHeadNew:

                    ViewBag.Type = "IncomeItemNew";
                    var incomeItemsNew = _expenseItemService.GetAllExpenseItem().Where(i => i.Status == EnumCompanyTransaction.Income).ToList();

                    if (id != default(int))
                    {
                        var Incomeitem = incomeItemsNew.FirstOrDefault(i => i.ExpenseItemID == id);
                        ViewBag.ExpenseItemsId = Incomeitem.ExpenseItemID;
                        ViewBag.ExpenseItemsCode = Incomeitem.Code;
                        ViewBag.ExpensItemsName = Incomeitem.Description;

                    }
                    return PartialView("~/Views/Pickers/_IncomeItemPicker.cshtml", incomeItemsNew);
                #region StockProduct
                case PickerType.StockProduct:

                    var stocks = _productService.GetAllProduct().Where(i => i.Rest.Item1 > 0);
                    var vmstocks = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>>, IEnumerable<GetProductViewModel>>(stocks);

                    if (id != default(int))
                    {
                        var vmProduct = vmstocks.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsPreviousStock = vmProduct.PreStock;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductPicker.cshtml", vmstocks);
                #endregion
                #region Allowance
                case PickerType.Allowance:

                    var allowances = _allowanceDeductionService.GetAll().Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Allowance);
                    var vmallowances = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(allowances);

                    if (id != default(int))
                    {
                        var vmallowance = vmallowances.First(x => x.AllowDeductID == id);
                        ViewBag.AllowancesCode = vmallowance.Code;
                        ViewBag.AllowancesName = vmallowance.Name;
                        ViewBag.AllowancesId = vmallowance.AllowDeductID;
                    }
                    return PartialView("~/Views/Pickers/_AllowancePicker.cshtml", vmallowances);
                #endregion
                #region Deduction
                case PickerType.Deduction:

                    var Deductions = _allowanceDeductionService.GetAll().Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Deduction);
                    var vmDeductions = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(Deductions);

                    if (id != default(int))
                    {
                        var vmallowance = vmDeductions.First(x => x.AllowDeductID == id);
                        ViewBag.AllowancesCode = vmallowance.Code;
                        ViewBag.AllowancesName = vmallowance.Name;
                        ViewBag.AllowancesId = vmallowance.AllowDeductID;
                    }
                    return PartialView("~/Views/Pickers/_AllowancePicker.cshtml", vmDeductions);
                #endregion
                #region Multiple Select Employee
                case PickerType.MSEmployee:

                    var sysinfo = _SystemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    var NextpayDate = ConstantData.GetFirstAndLastDateOfMonth(sysinfo.NextPayProcessDate);
                    var SalaryMonths = _SalaryMonthlyService.GetAllIQueryable().Where(i => i.SalaryMonth >= NextpayDate.Item1 && i.SalaryMonth <= NextpayDate.Item2);

                    var _Employees = _employeeService.GetAllEmployeeDetails().Where(i => (!SalaryMonths.Select(j => j.EmployeeID).Contains(i.Item1)) && i.Item6 <= NextpayDate.Item2);
                    var Employees = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>, IEnumerable<GetEmployeeViewModel>>(_Employees);
                    return PartialView("~/Views/Pickers/_MSEmployeePicker.cshtml", Employees);
                #endregion
                #region Department
                case PickerType.Department:
                    var Departments = _DepartmentService.GetAllDepartment();
                    if (id != default(int))
                    {
                        var department = Departments.FirstOrDefault(i => i.DepartmentId == id);
                        ViewBag.DepartmentsId = department.DepartmentId;
                        ViewBag.DepartmentsCode = department.CODE;
                        ViewBag.DepartmentsName = department.DESCRIPTION;
                    }
                    return PartialView("~/Views/Pickers/_DepartmentPicker.cshtml", Departments);
                #endregion


                case PickerType.MSCustomer:

                    ViewBag.IsCreditSales = false;
                    userId = User.Identity.GetUserId<int>();
                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                        var user = _UserService.GetUserById(userId);
                        EmpID = user.EmployeeID;

                        var customers = _customerService.GetAllCustomerByEmp(EmpID);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        return PartialView("~/Views/Pickers/_MSCustomerPicker.cshtml", vmCustomers);
                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        return PartialView("~/Views/Pickers/_MSCustomerPicker.cshtml", vmCustomers);

                    }
                case PickerType.SupplierProducts:

                    if (id != default(int))
                    {
                        var vmProduct = _productService.GetAllProductIQueryable().First(x => x.ProductID == id);

                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = 0m;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_SupplierProductDetailPicker.cshtml");

                case PickerType.DamageSupplierProducts:

                    if (id != default(int))
                    {
                        var vmProduct = _productService.GetAllProductIQueryable().First(x => x.ProductID == id);

                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = 0m;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_SupplierDamageProductDetailPicker.cshtml");

                #region InvestmentHeads
                case PickerType.InvestmentHeads:
                    if (id != default(int))
                    {
                        var InvestmentHead = _ShareInvestmentHeadService.GetById(id);
                        ViewBag.SIHID = InvestmentHead.SIHID;
                        ViewBag.InvestmentHeadCode = InvestmentHead.Code;
                        ViewBag.HeadName = InvestmentHead.Name;
                    }
                    return PartialView("~/Views/Pickers/_InvestmentHeadPicker.cshtml");
                #endregion

                case PickerType.CustomerSalesProduct:
                    return PartialView("~/Views/Pickers/_CustomerSalesProductPicker.cshtml");
                case PickerType.CustomerCreditSalesProduct:
                    return PartialView("~/Views/Pickers/_CustomerCreditSalesProductPicker.cshtml");
                default:

                    ViewBag.Type = "Invalid Picker";
                    return PartialView("~/Views/Pickers/_ModulePicker.cshtml", null);
            }
        }

        [ChildActionOnly]
        public PartialViewResult RenderProductPicker(PickerType pickerType, int id, int godownID)
        {
            switch (pickerType)
            {
                #region ProductWithGodown
                case PickerType.ProductDetail:

                    ViewBag.Type = "Details";
                    var customProductDetails = _productService.GetAllProductDetails();
                    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

                    if (id != 0 && godownID != 0)
                    {
                        var vmProduct = vmProductDetails.First(x => x.ProductId == id && x.GodownID == godownID);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsSalesRate = 0;
                        ViewBag.PDetailsPKTSheet = 0;
                        ViewBag.PDetailsPreStock = vmProduct.PreStock;
                        ViewBag.PDetailsPacket = 0;
                        ViewBag.PDetailsGodownID = vmProduct.GodownID;
                        ViewBag.PDetailsCategoryName = vmProduct.CategoryName;
                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker2.cshtml", vmProductDetails);
                #endregion
                default:

                    ViewBag.Type = "Invalid Picker";
                    return PartialView("~/Views/Pickers/_ModulePicker.cshtml", null);
            }
        }
    }
}