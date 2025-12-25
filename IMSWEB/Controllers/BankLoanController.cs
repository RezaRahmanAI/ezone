using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("bankloan")]
    public class BankLoanController : CoreController
    {
        private readonly IBankLoanService _bankLoanService;
        private readonly IMiscellaneousService<BankLoan> _miscellService;
        private readonly IBankService _bankService;
        private readonly IMapper _mapper;

        public BankLoanController(IErrorService errorService, IBankLoanService bankLoanService, IMiscellaneousService<BankLoan> miscellService, IBankService bankService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _bankLoanService = bankLoanService;
            _miscellService = miscellService;
            _bankService = bankService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var loanData = await _bankLoanService.GetAllAsync(DateRange.Item1, DateRange.Item2);

            return View(loanData);
        }

        [HttpPost]
        [Route("index")]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            var loanData = await _bankLoanService.GetAllAsync(ViewBag.FromDate, ViewBag.ToDate);

            //IEnumerable<CreateBankLoanVM> loans = _mapper.Map<IEnumerable<BankLoan>, IEnumerable<CreateBankLoanVM>>(await loanData);
            return View(loanData);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var loanCode = _miscellService.GetUniqueKey(i => i.Id);
            var banks = _bankService.GetAllBankForDDL().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            return View(new CreateBankLoanVM() { Code = loanCode, Banks = banks, LoanDate = DateTime.Now });
        }


        [HttpPost]
        public ActionResult Create(CreateBankLoanVM model, FormCollection formCollection)
        {
            var banks = _bankService.GetAllBankForDDL().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            model.Banks = banks;
            AddModelError(model);
            if (!ModelState.IsValid)
            {
                AddToastMessage("", "Invalid data!", ToastType.Error);
                return View(model);
            }
                

            if (formCollection.Get("btnCalcSchedule") != null)
            {
                CreateSchedule(model);
                return View(model);
            }
            else if (formCollection.Get("submitButton") != null)
            {

                if (model.LoanDetails == null)
                {
                    AddToastMessage("", "No schedule found! Please create schedule and try again.", ToastType.Error);
                    return View(model);
                }

                BankLoan newBankLoan = _mapper.Map<CreateBankLoanVM, BankLoan>(model);
                AddAuditTrail(newBankLoan, true);
                newBankLoan.IsPaid = false;

                List<BankLoanDetails> newLoanDetails = _mapper.Map<List<CreateBankLoanDetailsVM>, List<BankLoanDetails>>(model.LoanDetails);
                newBankLoan.BankLoanDetails.AddRange(newLoanDetails);
                //if (newLoanDetails.Any())
                //{
                //    foreach (var loanDetail in newLoanDetails)
                //    {

                //    }
                //}
                _bankLoanService.Add(newBankLoan);
                if (_bankLoanService.Save())
                {
                    TempData["loanId"] = newBankLoan.Id;
                    TempData["IsInvoiceReadyById"] = true;

                    AddToastMessage("", "Loan Saved Successfully.", ToastType.Success);
                    return RedirectToAction("Create");
                }
                else
                    AddToastMessage("", "Failed to save loan.", ToastType.Error);
            }

            return View(model);
        }

        void CreateSchedule(CreateBankLoanVM model)
        {
            decimal nTotalBalance = model.TotalLoanAmount;
            model.LoanDetails = new List<CreateBankLoanDetailsVM>();
            for (int i = 0; i < model.NoOfInstallment; i++)
            {
                CreateBankLoanDetailsVM loanDetail = new CreateBankLoanDetailsVM();
                loanDetail.InstallmentDate = model.LoanDate.AddMonths(i + 1);
                loanDetail.Status = "Due";

                loanDetail.InstallmentAmount = model.InstallmentSize;
                loanDetail.OpeningBalance = nTotalBalance;

                if ((i + 1) == model.NoOfInstallment)
                {
                    loanDetail.InstallmentAmount = nTotalBalance;
                    nTotalBalance -= nTotalBalance;
                }
                    
                else
                    nTotalBalance -= model.InstallmentSize;

                loanDetail.ClosingBalance = Math.Round(nTotalBalance, 2);
                //loanDetail.ScheduleNo = i + 1;
                loanDetail.InterestAmount = (model.PrincipleLoanAmount * model.InterestPercentage) / 100;

                model.LoanDetails.Add(loanDetail);
            }
        }





        private void AddModelError(CreateBankLoanVM model)
        {
            if (model.NoOfInstallment <= 0)
                ModelState.AddModelError("NoOfInstallment", "Installments is required.");
            if (model.PrincipleLoanAmount <= 0)
                ModelState.AddModelError("PrincipleLoanAmount", "Loan amount is required.");
            if(model.TotalLoanAmount <= 0)
                ModelState.AddModelError("TotalLoanAmount", "Total Loan amount is required.");
            if (model.InstallmentSize <= 0)
                ModelState.AddModelError("InstallmentSize", "Installment size is required.");
            if (model.SDPS <= 0)
                ModelState.AddModelError("SDPS", "SDPS amount is required.");
        }


        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            BankLoan loan = _bankLoanService.GetById(id);
            if (loan.BankLoanDetails.Any(d => d.Status.Equals("Paid")))
            {
                AddToastMessage("", "Loan collection found! You can't delete this loan.", ToastType.Error);
                return RedirectToAction("Index");
            }
            _bankLoanService.Delete(id);
            if (_bankLoanService.Save())
                AddToastMessage("", "Loan has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        public ActionResult Invoice(int id)
        {
            TempData["loanId"] = id;
            TempData["IsInvoiceReadyById"] = true;
            return RedirectToAction("Index");
        }

        public ActionResult LoanDetails()
        {
            return View();
        }
    }
}