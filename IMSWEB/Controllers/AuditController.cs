using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Report;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class AuditController : CoreController
    {
        ICreditSalesOrderService _hiresalesOrderService;
        ICustomerService _CustomerService;
        ISystemInformationService _SysInfoService;
        ISalesOrderService _salesOrderService;
        private readonly ICashCollectionService _cashCollectionService;
        private readonly IMapper _mapper;
        private readonly IExpenditureService _expenditureService;
        private readonly IBankTransactionService _bankTransactionService;

        public AuditController(IErrorService errorService,
             ICreditSalesOrderService SaleOrderService,
             ICustomerService CustomerService,
              ISystemInformationService SysInfoService, ISalesOrderService salesOrderService,
              ICashCollectionService cashCollectionService, IMapper mapper, IExpenditureService expenditureService,
              IBankTransactionService bankTransactionService)
            : base(errorService, SysInfoService)
        {
            _hiresalesOrderService = SaleOrderService;
            _CustomerService = CustomerService;
            _SysInfoService = SysInfoService;
            _salesOrderService = salesOrderService;
            _cashCollectionService = cashCollectionService;
            _mapper = mapper;
            _expenditureService = expenditureService;
            _bankTransactionService = bankTransactionService;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            AuditViewModel auditViewModel = new AuditViewModel();
            #region Hire sales
            var hireSales = _hiresalesOrderService.GetAllPendingSalesOrderAsync();
            var vmhireSales = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal,
                EnumSalesType, Tuple<string, string>>>,
                List<GetCreditSalesOrderViewModel>>(await hireSales);

            auditViewModel.HireSaleList = vmhireSales;
            #endregion

            #region cash collection
            var itemsAsync = _cashCollectionService.GetAllPendingCashCollAsync();
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string, Tuple<string, string, string>>>, List<GetCashCollectionViewModel>>(await itemsAsync);

            auditViewModel.CashCollectionList = vmodel;

            #endregion

            #region sales order
            var customSO = await _salesOrderService.GetAllPendingSalesOrderAsync();
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal,
                EnumSalesType, Tuple<string, string>>>,
            List<GetSalesOrderViewModel>>(customSO);

            auditViewModel.SOrderList = vmSO;
            #endregion


            #region Schedules 
            var Schedules = _hiresalesOrderService.GetScheduleCollection(DateTime.MinValue, DateTime.MinValue, "Pending");
            auditViewModel.Schedules = Schedules;
            #endregion

            List<EnumWFStatus> enumWFStatus = new List<EnumWFStatus>();
            enumWFStatus.Add(EnumWFStatus.Pending);

            #region income
            var incomes = _expenditureService.GetAllIncomeAsync(DateTime.MinValue, DateTime.MinValue, enumWFStatus);
            var vmincomes = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await incomes);
            auditViewModel.Incomes = vmincomes;
            #endregion

            #region expnese
            var expenses = _expenditureService.GetAllExpenditureAsync(DateTime.MinValue, DateTime.MinValue, enumWFStatus);
            var vmexpenses = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenses);
            auditViewModel.Expenses = vmexpenses;
            #endregion

            #region cash delivery
            List<EnumTranType> enumTranTypes = new List<EnumTranType>();
            enumTranTypes.Add(EnumTranType.DeliveryPending);
            var delviery = _cashCollectionService.GetAllCashDelivaeryAsync(DateTime.MinValue, DateTime.MinValue, enumTranTypes);
            var vmdelviery = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string>>, List<GetCashCollectionViewModel>>(await delviery);

            auditViewModel.CashDeliveryList = vmdelviery;
            #endregion

            #region bank transactions
            var customBankTransactionAsync = _bankTransactionService.GetAllBankTransactionAsync(DateTime.MinValue, DateTime.MinValue, enumWFStatus);
            var vm = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal,
                DateTime?, string, string, string, string,string>>>, IEnumerable<GetBankTransactionViewModel>>(await customBankTransactionAsync);
            auditViewModel.bankTransactions = vm;
            #endregion

            return View(auditViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public JsonResult ChangeRemindDate(int CustomerID, DateTime RemindDate)
        {
            if (CustomerID > 0)
            {
                var customer = _CustomerService.GetCustomerById(CustomerID);
                customer.RemindDate = RemindDate;
                _CustomerService.UpdateCustomer(customer);
                _CustomerService.SaveCustomer();
                return Json(true);
            }
            return Json(false);
        }
    }
}