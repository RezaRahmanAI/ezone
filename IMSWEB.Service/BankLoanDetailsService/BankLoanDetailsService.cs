using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System.Data.SqlClient;
using System.Data;

namespace IMSWEB.Service
{
    public class BankLoanDetailsService : IBankLoanDetailsService
    {
        private readonly IBaseRepository<BankLoanDetails> _baseRepository;
        private readonly IBaseRepository<BankLoan> _loanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<BankLoanPenaltyDetails> _loanPenaltyRepository;

        public BankLoanDetailsService(IBaseRepository<BankLoanDetails> baseRepository, IUnitOfWork unitOfWork, IBaseRepository<BankLoanPenaltyDetails> loanPenaltyRepository, IBaseRepository<BankLoan> loanRepository)
        {
            _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
            _loanPenaltyRepository = loanPenaltyRepository;
            _loanRepository = loanRepository;
        }

        public void Add(BankLoanDetails model)
        {
            _baseRepository.Add(model);
        }

        public void Update(BankLoanDetails model)
        {
            _baseRepository.Update(model);
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<BankLoanDetails> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        public BankLoanDetails GetById(int id)
        {
            return _baseRepository.FindBy(x => x.Id == id).First();
        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.Id == id);
        }

        public List<BankLoanDetails> GetAllLoanDetailsByBankLoanId(int bankLoanId)
        {
            return _baseRepository.All.Where(d => d.BankLoanId == bankLoanId).ToList();
        }

        public void DeleteByBankLoanId(int bankLoanId)
        {
            _baseRepository.Delete(d => d.BankLoanId == bankLoanId);
        }

        public void AddMultiple(List<BankLoanDetails> list)
        {
            _baseRepository.AddMultiple(list);
        }

        public decimal GetFirstPendingLoanAmountByBankLoanIdOld(int bankLoanId)
        {
            var loanDetails = _baseRepository.All.Where(d => d.BankLoanId == bankLoanId && d.Status.Equals("Due")).OrderBy(d => d.ScheduleNo).FirstOrDefault();

            return loanDetails != null ? (loanDetails.InstallmentAmount + loanDetails.PenaltyChargeAmount) : 0m;
        }

        public Tuple<decimal, decimal> GetFirstPendingLoanAmountByBankLoanId(int bankLoanId)
        {
            var loanDetails = _baseRepository.All.Where(d => d.BankLoanId == bankLoanId && d.Status.Equals("Due")).OrderBy(d => d.ScheduleNo).FirstOrDefault();
            var loan = _loanRepository.FindBy(d => d.Id == bankLoanId).FirstOrDefault();
            decimal sdpsAmount = loan != null ? loan.SDPS : 0m;
            decimal loanAmount = loanDetails != null ? (loanDetails.InstallmentAmount + loanDetails.PenaltyChargeAmount) : 0m;
            return new Tuple<decimal, decimal>(loanAmount, sdpsAmount);
        }

        public List<BankLoanDetails> GetAllDueLoanByCurrentDate(DateTime currentDate)
        {
            List<BankLoanDetails> data = _baseRepository.All.Where(d => d.Status.Equals("Due") && d.InstallmentDate.Year <= currentDate.Year && d.InstallmentDate.Month < currentDate.Month).ToList();

            return data;
        }

        public List<BankLoanPenaltyDetails> GetAllPenaltyByLoanDetails(int loanDetailsId)
        {
            return _loanPenaltyRepository.FindBy(d => d.LoanDetailsId == loanDetailsId).ToList();
        }

        public BankLoanPenaltyDetails GetLastPenaltyByLoanDetails(int loanDetailsId)
        {
            return _loanPenaltyRepository.FindBy(d => d.LoanDetailsId == loanDetailsId).OrderByDescending(d => d.PenaltyDate).FirstOrDefault();
        }

        public void AddPenalty(BankLoanPenaltyDetails model)
        {
            _loanPenaltyRepository.Add(model);
        }


        public List<RPTBankLoanDetailsTO> GetAllLoanDetails(DateTime fromDate, DateTime toDate, int concernId)
        {
            string query = string.Format(@"SELECT BL.Id, BL.Code, B.BankName, BL.TotalLoanAmount, 
                                                   SUM(BLD.InstallmentAmount) AS TotalInstallmentAmount,
                                                   (BL.TotalLoanAmount - SUM(BLD.InstallmentAmount)) AS RemainingAmount,
                                                   SUM(BLC.SDPS) AS TotalSDPS,
                                                   SUM(BLC.Savings) AS TotalSavings
                                            FROM BankLoanDetails BLD
                                            JOIN BankLoans BL ON BLD.BankLoanId = BL.Id
                                            JOIN BankLoanCollections BLC ON BLD.LoanCollectionId = BLC.Id
                                            JOIN Banks B ON BL.BankId = B.BankID
                                            WHERE BL.ConcernId = @ConcernId
                                              AND BLD.Status = 'Paid'
                                              AND BLD.LoanCollectionId IS NOT NULL
                                              AND CAST(BL.LoanDate AS DATE) BETWEEN CAST(@FromDate AS DATE) AND CAST(@ToDate AS DATE)
                                            GROUP BY BL.Id, BL.Code, BL.TotalLoanAmount, B.BankName");

            List<RPTBankLoanDetailsTO> loanDetailsDataList = _baseRepository.ExecSP<RPTBankLoanDetailsTO>(query, 
                new SqlParameter("ConcernId", SqlDbType.Int) { Value = concernId },
                new SqlParameter("FromDate", SqlDbType.DateTime) { Value = fromDate },
                new SqlParameter("ToDate", SqlDbType.DateTime) { Value = toDate }

                ).ToList();

            return loanDetailsDataList;
        }
    }
}
