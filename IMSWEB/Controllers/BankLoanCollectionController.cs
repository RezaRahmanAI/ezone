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
    [RoutePrefix("bankloan-collection")]
    public class BankLoanCollectionController : CoreController
    {
        private readonly IBankLoanService _bankLoanService;
        private readonly IMiscellaneousService<BankLoanCollection> _miscellService;
        private readonly IBankService _bankService;
        private readonly IMapper _mapper;
        private readonly IBankLoanCollectionService _bankLoanCollectionService;
        private readonly IBankLoanDetailsService _bankLoanDetailsService;

        public BankLoanCollectionController(IErrorService errorService, IBankLoanService bankLoanService, IMiscellaneousService<BankLoanCollection> miscellService, IBankService bankService, IMapper mapper, IBankLoanCollectionService bankLoanCollectionService, IBankLoanDetailsService bankLoanDetailsService, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _bankLoanService = bankLoanService;
            _miscellService = miscellService;
            _bankService = bankService;
            _mapper = mapper;
            _bankLoanCollectionService = bankLoanCollectionService;
            _bankLoanDetailsService = bankLoanDetailsService;
        }

        [HttpGet]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var loanData = await _bankLoanCollectionService.GetAllAsync(DateRange.Item1, DateRange.Item2);

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

            var loanData = await _bankLoanCollectionService.GetAllAsync(ViewBag.FromDate, ViewBag.ToDate);

            return View(loanData);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var loanCode = _miscellService.GetUniqueKey(i => i.Id);
            var banks = _bankService.GetAllBankForDDL().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            var bankLoans = _bankLoanService.GetBankLoanByBankId(0).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            return View(new BankLoanCollectionVM() { Code = loanCode, Banks = banks, BankLoans = bankLoans, CollectionDate = DateTime.Now, CollectionType = EnumBankLoanType.Normal });
        }


        [HttpPost]
        public ActionResult Create(BankLoanCollectionVM model, FormCollection formCollection)
        {
            var banks = _bankService.GetAllBankForDDL().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            model.Banks = banks;

            var bankLoans = _bankLoanService.GetBankLoanByBankId(model.BankId).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            model.BankLoans = bankLoans;

            AddModelError(model);
            if (!ModelState.IsValid)
            {
                AddToastMessage("", "Invalid data!", ToastType.Error);
                return View(model);
            }

            BankLoanCollection loanCollection = new BankLoanCollection
            {
                CollectionAmount = model.CollectionAmount,
                CollectionDate = model.CollectionDate,
                Code = model.Code,
                SDPS = model.SDPS,
                Savings = model.Savings,
                CCLoanId = model.CollectionType == EnumBankLoanType.Normal ? (int?)null : model.BankLoanDetailsId,
                CollectionType = model.CollectionType
            };

            AddAuditTrail(loanCollection, true);

            #region get loan details info
            List<BankLoanDetails> allLoanDetails = _bankLoanDetailsService.GetAllLoanDetailsByBankLoanId(model.BankLoanDetailsId);
            int loanPaymentId = 0;

            if (allLoanDetails.Any())
            {
                BankLoanDetails oPaidLoan = allLoanDetails.FirstOrDefault(d => d.Status.ToLower().Equals("due"));

                BankLoanDetails lastPaidLoan = allLoanDetails.OrderByDescending(d => d.ScheduleNo).FirstOrDefault(d => d.Status.ToLower().Equals("due"));
                if (oPaidLoan != null)
                {
                    BankLoan loan = _bankLoanService.GetById(oPaidLoan.BankLoanId);
                    loanPaymentId = oPaidLoan.Id;

                    oPaidLoan.PaymentDate = model.CollectionDate;
                    oPaidLoan.Status = "Paid";
                    oPaidLoan.ExpectedInstallmentAmount = oPaidLoan.InstallmentAmount;
                    oPaidLoan.InstallmentAmount = model.CollectionAmount;
                    oPaidLoan.ClosingBalance = (oPaidLoan.OpeningBalance + oPaidLoan.PenaltyChargeAmount) - model.CollectionAmount;

                    if (oPaidLoan.Id == lastPaidLoan.Id)
                    {
                        oPaidLoan.LastPayAdjustment = (lastPaidLoan.ExpectedInstallmentAmount + lastPaidLoan.PenaltyChargeAmount) - model.CollectionAmount;
                        loan.IsPaid = true;
                        oPaidLoan.ClosingBalance = 0m;
                    }
                    else
                        oPaidLoan.LastPayAdjustment = 0m;

                    if (oPaidLoan.ExpectedInstallmentAmount != model.CollectionAmount)
                    {
                        int noOfRemainingInstallment = allLoanDetails.Where(s => s.Status.Equals("Due")).ToList().Count;

                        decimal paidAmount = allLoanDetails.Where(s => s.Status.Equals("Paid")).Sum(d => d.InstallmentAmount);
                        decimal penaltyAmount = allLoanDetails.Where(s => s.Status.Equals("Paid")).Sum(d => d.PenaltyChargeAmount);

                        decimal remainingAmount = loan.TotalLoanAmount - (paidAmount - penaltyAmount);

                        if (noOfRemainingInstallment > 0)
                        {
                            decimal nTotalInstallment = Math.Round((remainingAmount / noOfRemainingInstallment), 2);

                            foreach (var insSchedule in allLoanDetails.Where(s => s.Status.Equals("Due")))
                            {
                                insSchedule.InstallmentAmount = nTotalInstallment;
                                insSchedule.ExpectedInstallmentAmount = nTotalInstallment;
                                insSchedule.OpeningBalance = remainingAmount;
                                remainingAmount -= nTotalInstallment;
                                insSchedule.ClosingBalance = remainingAmount;
                            }

                            //allLoanDetails.ForEach(d => d.Id = 0);
                        }
                    }

                    _bankLoanDetailsService.DeleteByBankLoanId(oPaidLoan.BankLoanId);
                    _bankLoanDetailsService.Save();
                    _bankLoanDetailsService.AddMultiple(allLoanDetails);

                }
            }

            #endregion

            #region update cc loan details
            //if (model.CollectionType == EnumBankLoanType.CCLoan)
            //{
            //    BankCCLoan ccLoan = _bankCCLoanService.GetById(model.BankLoanDetailsId);
            //    decimal amount = 0m;
            //    if (model.CollectionAmount > ccLoan.TotalGivenLoanAmount)
            //    {
            //        loanCollection.PrevTotalGivenAmount = ccLoan.TotalGivenLoanAmount;
            //        loanCollection.PrevTotalInterestAmount = ccLoan.TotalInterestAmount;

            //        ccLoan.TotalGivenLoanAmount = 0m;
            //        amount = model.CollectionAmount - ccLoan.TotalGivenLoanAmount;
            //        ccLoan.TotalInterestAmount -= amount;

            //        loanCollection.TotalGivenAmount = ccLoan.TotalGivenLoanAmount;
            //        loanCollection.TotalInterestAmount = ccLoan.TotalInterestAmount;


            //    }
            //    else
            //    {
            //        loanCollection.PrevTotalGivenAmount = ccLoan.TotalGivenLoanAmount;
            //        loanCollection.PrevTotalInterestAmount = ccLoan.TotalInterestAmount;

            //        ccLoan.TotalGivenLoanAmount = ccLoan.TotalGivenLoanAmount - model.CollectionAmount;

            //        loanCollection.TotalGivenAmount = ccLoan.TotalGivenLoanAmount;
            //        loanCollection.TotalInterestAmount = ccLoan.TotalInterestAmount;
            //    }
            //    _bankCCLoanService.Update(ccLoan);
            //}
            #endregion

            _bankLoanCollectionService.Add(loanCollection);

            if (_bankLoanCollectionService.Save())
            {
                if (loanPaymentId > 0)
                {
                    if (model.CollectionType == EnumBankLoanType.Normal)
                    {
                        List<BankLoanDetails> allLoanDetailsList = _bankLoanDetailsService.GetAllLoanDetailsByBankLoanId(model.BankLoanDetailsId);
                        BankLoanDetails loan = allLoanDetailsList.OrderByDescending(d => d.ScheduleNo).FirstOrDefault(d => d.Status.ToLower().Equals("paid"));

                        loan.LoanCollectionId = loanCollection.Id;
                        _bankLoanDetailsService.Update(loan);
                        _bankLoanDetailsService.Save();
                    }

                }

                TempData["loanCollectionId"] = loanCollection.Id;
                TempData["IsInvoiceReadyById"] = true;

                AddToastMessage("", "Loan collection saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "Failed to save loan collection.", ToastType.Error);
            }

            return View(model);
        }






        private void AddModelError(BankLoanCollectionVM model)
        {
            if (model.CollectionAmount <= 0)
                ModelState.AddModelError("CollectionAmount", "Collection amount is required.");
            if (model.BankId <= 0)
                ModelState.AddModelError("BankId", "Bank is required.");
            if (model.BankLoanDetailsId <= 0)
                ModelState.AddModelError("BankLoanDetailsId", "Bank Loan is required.");
            if (model.CollectionType == EnumBankLoanType.Normal)
            {
                if (model.SDPS <= 0)
                    ModelState.AddModelError("SDPS", "SDPS amount is required.");
            }

        }


        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            if (_bankLoanCollectionService.IsDeleteAllowed(id))
            {

                BankLoanCollection collection = _bankLoanCollectionService.GetById(id);

                #region for cc loan later
                //if (collection.CollectionType == EnumBankLoanType.CCLoan)
                //{
                //    int lastId = _bankLoanCollectionService.GetLastCollectionIdForCCLoan(id);
                //    if (lastId > 0 && (id != lastId))
                //    {
                //        AddToastMessage("", "There is another collection after this collection with same cc loan. Can't delete.", ToastType.Error);
                //        return RedirectToAction("Index");
                //    }

                //}
                #endregion

                Tuple<int, int> loanIds = _bankLoanCollectionService.GetLoanIdByCollectionId(id);
                if (_bankLoanCollectionService.DelecteBankLoanCollectionUsingSP(id, collection.CollectionType))
                {
                    BankLoan loan = _bankLoanService.GetById(loanIds.Item1);
                    #region for re-schedule loan
                    List<BankLoanDetails> allLoanDetails = _bankLoanDetailsService.GetAllLoanDetailsByBankLoanId(loanIds.Item1);
                    allLoanDetails.Where(d => d.Id == loanIds.Item2).First().Status = "Due";

                    if (allLoanDetails.Any())
                    {
                        List<BankLoanDetails> oDueLoans = allLoanDetails.Where(d => d.Status.ToLower().Equals("due")).ToList();

                        if (oDueLoans != null && oDueLoans.Any())
                        {

                            int noOfRemainingInstallment = oDueLoans.Count;


                            decimal paidAmount = allLoanDetails.Where(s => s.Status.Equals("Paid")).Sum(d => d.InstallmentAmount);
                            decimal penaltyAmount = allLoanDetails.Where(s => s.Status.Equals("Paid")).Sum(d => d.PenaltyChargeAmount);

                            decimal remainingAmount = loan.TotalLoanAmount - (paidAmount - penaltyAmount);

                            if (noOfRemainingInstallment > 0)
                            {
                                decimal nTotalInstallment = Math.Round((remainingAmount / noOfRemainingInstallment), 2);
                                BankLoanDetails lastCount = oDueLoans.OrderByDescending(d => d.ScheduleNo).First();
                                foreach (var insSchedule in oDueLoans)
                                {
                                    insSchedule.InstallmentAmount = nTotalInstallment;
                                    insSchedule.ExpectedInstallmentAmount = nTotalInstallment;
                                    insSchedule.OpeningBalance = remainingAmount;
                                    remainingAmount -= nTotalInstallment;

                                    if (insSchedule.Id == lastCount.Id)
                                        insSchedule.ClosingBalance = remainingAmount;
                                    else
                                        insSchedule.ClosingBalance = remainingAmount;

                                }

                                //allLoanDetails.ForEach(d => d.Id = 0);
                            }

                            _bankLoanDetailsService.DeleteByBankLoanId(oDueLoans.First().BankLoanId);
                            _bankLoanDetailsService.Save();
                            _bankLoanDetailsService.AddMultiple(oDueLoans);

                            if (_bankLoanDetailsService.Save())
                                AddToastMessage("", "Loan Collection has been deleted successfully.", ToastType.Success);
                        }
                    }

                    #endregion
                }
                else
                    AddToastMessage("", "Failed to delete.", ToastType.Error);

            }
            else
            {
                AddToastMessage("", "There is another loan collection found after this loan collection. Can't delete.", ToastType.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetLoanByBank(int bankId, int collectionType)
        {
            if (collectionType == (int)EnumBankLoanType.Normal)
            {
                var allBankLoan = _bankLoanService.GetBankLoanByBankId(bankId);
                return Json(allBankLoan, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetRemainingAmountByBankLoanId(int bankLoanId, int collectionType)
        {
            if (collectionType == (int)EnumBankLoanType.Normal)
            {
                Tuple<decimal, decimal> loanData = _bankLoanDetailsService.GetFirstPendingLoanAmountByBankLoanId(bankLoanId);
                return Json(new { loanAmount = loanData.Item1, sdpsAmount = loanData.Item2 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //BankCCLoan ccLoan = _bankCCLoanService.GetById(bankLoanId);

                //decimal totalAmount = ccLoan.TotalGivenLoanAmount + ccLoan.TotalInterestAmount;

                //if (ccLoan.BankCCLoanDetails.Any())
                //{
                //    DateTime lastDate = ccLoan.BankCCLoanDetails.OrderByDescending(d => d.ScheduleNo).First().LoanPaymentDate.Date;
                //    DateTime currentDate = GetLocalDateTime().Date;
                //    if (lastDate < currentDate)
                //    {
                //        TimeSpan difference = currentDate - lastDate;
                //        int remainingDays = difference.Days;

                //        decimal interest = ((ccLoan.TotalGivenLoanAmount * (ccLoan.InterestPercentage / 100)) / 365) * remainingDays;
                //        totalAmount += interest;
                //    }
                //}


                return Json(new { loanAmount = 0m, sdpsAmount = 0m }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Invoice(int id)
        {
            TempData["loanCollectionId"] = id;
            TempData["IsInvoiceReadyById"] = true;
            return RedirectToAction("Index");
        }

        public ActionResult DueLoan()
        {
            TempData["IsPendingList"] = true;
            return View();
        }
    }
}