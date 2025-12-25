using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TO;
using System.Data;
using System.Data.SqlClient;


namespace IMSWEB.Service
{
    public class ShareInvestmentService : IShareInvestmentService
    {
        private readonly IBaseRepository<ShareInvestment> _baseRepository;
        private readonly IBaseRepository<ShareInvestmentHead> _ShareInvestmentHeadRepository;
        private readonly IBaseRepository<BankTransaction> _bankTransRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShareInvestmentService(IBaseRepository<ShareInvestment> baseRepository, IUnitOfWork unitOfWork, IBaseRepository<Customer> customerRepository, IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository, IBaseRepository<BankTransaction> bankTransRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _ShareInvestmentHeadRepository = ShareInvestmentHeadRepository;
            _customerRepository = customerRepository;
            _bankTransRepository = bankTransRepository;
        }

        public void Add(ShareInvestment ShareInvestment)
        {
            _baseRepository.Add(ShareInvestment);
        }

        public void Update(ShareInvestment ShareInvestment)
        {
            _baseRepository.Update(ShareInvestment);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<ShareInvestment> GetAll()
        {
            return _baseRepository.All;
        }
        public async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>> 
            GetAllAsync(EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            return await _baseRepository.GetAllAsync(_ShareInvestmentHeadRepository, investmentType, fromDate, toDate, InvestTransType);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>>
            GetAllLiabilityAsync(EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            return await _baseRepository.GetAllLiabilityAsync(_ShareInvestmentHeadRepository, investmentType, fromDate, toDate, InvestTransType);
        }
        public ShareInvestment GetById(int id)
        {
            return _baseRepository.FindBy(x => x.SIID == id).First();
        }



        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.SIID == id);
       
        }


        public void DeleteInvm(ShareInvestment ShareInvestment, int id)
        {
            _baseRepository.Delete(ShareInvestment);

            _baseRepository.Delete(x => x.SIID == id);
        }


        public List<LiabilityReportModel> TotalLiabilityPayRec(DateTime asOnDate, int concernID)
        {
            List<LiabilityReportModel> totalLiabilityPayRecData = new List<LiabilityReportModel>();
            LiabilityReportModel particulars = null;

            #region Liabilities Recived  part
            #region Liabilities received / Loan receivable
            TOAccountRecAndPay recAndPay = _customerRepository.ExecSP<TOAccountRecAndPay>("GetLoanPayableAndReceivable @asOnDate, @ConcernID",
                new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
                new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();
            decimal totalLoanReceivable = recAndPay != null ? recAndPay.TotalReceivable : 0m;

            particulars = new LiabilityReportModel
            {
                LiabilitiesPay = 0m,
                LiabilitiesReceived = totalLoanReceivable,
                CreditParticulars = "",
                DebitParticulars = "Liabilities received"
            };
            totalLiabilityPayRecData.Add(particulars);
            #endregion

            #endregion

            #region Liabilities Pay part
            #region Current Liabilities
            particulars = new LiabilityReportModel
            {
                LiabilitiesPay = 0m,
                LiabilitiesReceived = 0m,
                //CreditParticulars = "Current Liabilieties",
                //DebitParticulars = ""
            };
            totalLiabilityPayRecData.Add(particulars);
            #endregion


            //#region Account Payable / Total Supplieres Due
            //decimal totalSupplierDue = _customerRepository.ExecSP<decimal>("GetSupplierDueByDate @asOnDate, @ConcernID",
            //    new SqlParameter("asOnDate", SqlDbType.NVarChar) { Value = asOnDate },
            //    new SqlParameter("ConcernID", SqlDbType.Int) { Value = concernID }).FirstOrDefault();

            //particulars = new LiabilityReportModel
            //{
            //    LiabilitiesPay = totalSupplierDue,
            //    LiabilitiesReceived = 0m,
            //    CreditParticulars = "Account Payable",
            //    DebitParticulars = ""
            //};
            //totalLiabilityPayRecData.Add(particulars);
            //#endregion


            #region Liabilities Pay / Loan Payable
            decimal totalloanPayable = recAndPay != null ? recAndPay.TotalPayable : 0m;

            particulars = new LiabilityReportModel
            {
                LiabilitiesPay = totalloanPayable,
                LiabilitiesReceived = 0m,
                CreditParticulars = "Liabilities Pay",
                DebitParticulars = ""
            };
            totalLiabilityPayRecData.Add(particulars);
            #endregion

            //#region Total Current Liabilities
            //decimal totalCurrentLiabilities = totalSupplierDue + totalloanPayable;

            //particulars = new LiabilityReportModel
            //{
            //    LiabilitiesPay = totalCurrentLiabilities,
            //    LiabilitiesReceived = 0m,
            //    CreditParticulars = "Total Current Liabilities",
            //    DebitParticulars = ""
            //};
            //totalLiabilityPayRecData.Add(particulars);
            //#endregion

            #endregion

            return totalLiabilityPayRecData;
        }

        public List<VoucherTransactionReportModel> VoucherTransactionLedgerData(DateTime fromDate,
        DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        {
            return _baseRepository.VoucherTransactionLedgerData(_customerRepository, _ShareInvestmentHeadRepository, _bankTransRepository, fromDate, toDate, ConcernID, ExpenseItemID, headType);
        }
    }
}
