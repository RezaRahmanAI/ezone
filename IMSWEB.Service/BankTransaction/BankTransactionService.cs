using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.SPModel;
using System.Data;

namespace IMSWEB.Service
{
    public class BankTransactionService : IBankTransactionService
    {
        private readonly IBaseRepository<BankTransaction> _bankTransactionRepository;
        private readonly IBankTransactionRepository _bankTransactionDetailsRepository;
        private readonly IBaseRepository<Bank> _bankRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly ICardTypeSetupService _CardTypeSetupService;
        private readonly IUnitOfWork _unitOfWork;



        public BankTransactionService(IBaseRepository<BankTransaction> bankTransactionRepository, IBaseRepository<Bank> bankRepository,
             IBaseRepository<Customer> customerRepository,
             IBaseRepository<Supplier> supplierRepository,
            IBankTransactionRepository bankTransactionDetailsRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            IUnitOfWork unitOfWork, ICardTypeSetupService CardTypeSetupService)
        {
            _bankTransactionRepository = bankTransactionRepository;
            _bankRepository = bankRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _bankTransactionDetailsRepository = bankTransactionDetailsRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = SisterConcernRepository;
            _CardTypeSetupService = CardTypeSetupService;
        }

        public void AddBankTransaction(BankTransaction bankTransaction)
        {
            _bankTransactionRepository.Add(bankTransaction);
        }

        public void UpdateBankTransaction(BankTransaction bankTransaction)
        {
            _bankTransactionRepository.Update(bankTransaction);
        }

        public void SaveBankTransaction()
        {
            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string>>>>
            GetAllBankTransactionAsync(DateTime fromDate, DateTime toDate)
        {
            return await _bankTransactionRepository.GetAllBankTransactionAsync(_bankRepository, _customerRepository, _supplierRepository, fromDate, toDate);
        }


        public IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string, Tuple<string, string, string, string>>>> GetAllBankTransaction()
        {
            return _bankTransactionRepository.GetAllBankTransaction(_bankRepository, _customerRepository, _supplierRepository);
        }

        public IEnumerable<Tuple<int, string, string, string, string, string, string,
            Tuple<decimal, DateTime?, string, string, string, string, string,
                Tuple<string, string, string, string, EnumCustomerType>>>>
            GetBankTransactionData(DateTime fromDate, DateTime toDate, int concernID, int customerId,
            int SupplierID, int EmployeeID, EnumTransactionType Status, bool IsAdmin, EnumCustomerType customerType)
        {
            return _bankTransactionRepository.GetBankTransactionData(_bankRepository, _customerRepository,
                _supplierRepository, fromDate, toDate, concernID, customerId, SupplierID, EmployeeID, Status, IsAdmin, customerType);
        }

        //public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<int, int, int, int>>>> GetAllProductFromDetail()
        //{
        //    return _productRepository.GetAllProductFromDetail(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository);
        //}
        //public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, int, int, int, int>>>> GetAllProductFromDetailForCredit()
        //{
        //    return _productRepository.GetAllProductFromDetailForCredit(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository);
        //}

        //public IEnumerable<Tuple<int, string, string,
        // decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> GetAllSalesProductFromDetailByCustomerID(int CustomerID)
        //{
        //    return _productRepository.GetAllSalesProductFromDetailByCustomerID(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository,_SOrderRepository,_SOrderDetailRepository,CustomerID);
        //}

        //public IEnumerable<Tuple<int, string, string,
        //  decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> GetAllSalesProductFromDetailByCustomerID(int CustomerID)
        //{
        //    return _productRepository.GetAllSalesProductFromDetail(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository);
        //}
        public BankTransaction GetBankTransactionById(int id)
        {
            return _bankTransactionRepository.FindBy(x => x.BankTranID == id).FirstOrDefault();
        }

        public void DeleteBankTransaction(int id)
        {
            _bankTransactionRepository.Delete(x => x.BankTranID == id);
        }

        public List<BankLedgerModel> BankLedgerUsingSP(DateTime fromdate, DateTime todate, int ConcernID, int BankID)
        {

            return _bankTransactionDetailsRepository.BankLedgerUsingSP(fromdate, todate, ConcernID, BankID);

        }


        //public  IEnumerable<BankSummaryReportModel> GetBankSummary()
        //{
        //    return;
        //}


        //public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> SRWiseGetAllProductFromDetail(int EmployeeID)
        //{
        //    return _productRepository.SRWiseGetAllProductFromDetail(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository,_SRVisitRepository,_SRVisitDetailRepository,_SRVProductDetailRepository,EmployeeID);
        //}

        //public IEnumerable<Tuple<int,string, string, string, string>> GetProductDetails()
        //{
        //    return _productRepository.GetProductDetail(_categoryRepository, _companyRepository);
        //}


        public List<BankTransReportModel> BankTransactionsReport(DateTime fromdate, DateTime todate, int BankID, int concernId)
        {
            return _bankTransactionDetailsRepository.BankTransactionsReport(fromdate, todate, BankID, concernId);
        }
        public List<BankTransReportModel> BankLedger(DateTime fromdate, DateTime todate, int BankID, int concernId)
        {
            return _bankTransactionRepository.BankLedger(_bankRepository, _customerRepository, _supplierRepository, _SisterConcernRepository, BankID, fromdate, todate, concernId);
        }

        public IQueryable<CashCollectionReportModel> AdminCashCollectionByBank(int ConcernID,
            DateTime fromDate, DateTime toDate,
                                    EnumCustomerType customerType, int customerID)
        {
            return _bankTransactionRepository.AdminCashCollectionByBank(_bankRepository,
                _customerRepository, _SisterConcernRepository, ConcernID, fromDate, toDate, customerType, customerID);
        }

        public decimal CardPaymentNetAmtCalculation(int BankID, int CardTypeID, decimal CardPaidAmount,
                        out int CardTypeSetupID,
                        out decimal DepositChargePercent)
        {
            var CardTypeSetup = _CardTypeSetupService.GetAll().FirstOrDefault(i => i.BankID == BankID && i.CardTypeID == CardTypeID);

            CardTypeSetupID = CardTypeSetup == null ? 0 : CardTypeSetup.CardTypeSetupID;
            DepositChargePercent = CardTypeSetup == null ? 0 : CardTypeSetup.Percentage;
            decimal DepositAmt = CardPaidAmount - ((CardPaidAmount * DepositChargePercent) / 100m);
            return DepositAmt;
        }

        public DataTable CreateBankTransDataTable(DateTime TranDate, string TransactionNo,
            EnumTransactionType transactionType, decimal DepositAmt, int BankID, int ConcernID,
            string Remarks)
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

            DataRow row = null;

            row = dtBankTrans.NewRow();
            row["TranDate"] = TranDate;
            row["TransactionNo"] = TransactionNo;
            row["TransactionType"] = transactionType;
            row["Amount"] = DepositAmt;
            row["BankID"] = BankID;
            row["CustomerID"] = 0;
            row["SupplierID"] = 0;
            row["AnotherBankID"] = 0;
            row["ChecqueNo"] = string.Empty;
            row["Remarks"] = Remarks;
            row["ConcernID"] = ConcernID;

            dtBankTrans.Rows.Add(row);

            return dtBankTrans;
        }

        public async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>>
           GetAllBankTransactionAsync(DateTime fromDate, DateTime toDate,
               List<EnumWFStatus> enumWFStatuses)
        {
            return await _bankTransactionRepository.GetAllBankTransactionAsync(_bankRepository, _customerRepository, _supplierRepository, fromDate, toDate, enumWFStatuses);
        }

        public async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>> GetAllBankTransactionNewAsync(DateTime fromDate, DateTime toDate,
       List<EnumWFStatus> enumWFStatuses, int concernId)
        {
            return await _bankTransactionRepository.GetAllBankTransactionNewAsync(_bankRepository, _customerRepository, _supplierRepository, fromDate, toDate, enumWFStatuses, concernId);
        }

        public IEnumerable<Tuple<int, string, string, string, string, string, string,
            Tuple<decimal, DateTime?, string, string, string, string, string,
                Tuple<string, string, string, string, EnumCustomerType, string>>>>
            GetBankTransactionDataForAll(DateTime fromDate, DateTime toDate, int concernID, int customerId,
            int SupplierID, int EmployeeID, EnumTransactionType Status, bool IsAdmin, EnumCustomerType customerType)
        {
            return _bankTransactionRepository.GetBankTransactionDataForAll(_bankRepository, _customerRepository,
                _supplierRepository, fromDate, toDate, concernID, customerId, SupplierID, EmployeeID, Status, IsAdmin, customerType, _SisterConcernRepository);
        }

        public IQueryable<BankTransaction> GetAll()
        {
            return _bankTransactionRepository.All;
        }

    }
}
