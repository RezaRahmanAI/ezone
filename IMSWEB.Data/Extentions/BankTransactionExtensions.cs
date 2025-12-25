
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class BankTransactionExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string>>>>
            GetAllBankTransactionAsync
                (this IBaseRepository<BankTransaction> bankTransactionRepository,
                IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Bank> banks = bankRepository.All;
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Supplier> suppliers = supplierRepository.All;
            var items = await (from bt in bankTransactionRepository.All
                               join b in bankRepository.All on bt.BankID equals b.BankID
                               join c1 in customerRepository.All on bt.CustomerID equals c1.CustomerID into customers1
                               from c in customers1.DefaultIfEmpty()
                               join s1 in supplierRepository.All on bt.SupplierID equals s1.SupplierID into supppliers1
                               from s in supppliers1.DefaultIfEmpty()
                               join b21 in bankRepository.All on bt.AnotherBankID equals b21.BankID into AnothersBanks1
                               from b2 in AnothersBanks1.DefaultIfEmpty()
                               where bt.TranDate >= fromDate && bt.TranDate <= toDate
                               select new
                               {
                                   BankTransactionId = bt.BankTranID,
                                   BankName = b.BankName,
                                   b.AccountName,
                                   b.AccountNo,
                                   CustomerName = c.Name,
                                   SupplierName = s.Name,
                                   AnotherBank = b2.BankName,
                                   bt.TransactionNo,
                                   bt.TransactionType,
                                   bt.Amount,
                                   bt.TranDate,
                                   bt.ChecqueNo,
                                   bt.Remarks,
                               }).OrderByDescending(i => i.TranDate).ThenByDescending(i => i.TransactionNo).ToListAsync();
            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string>>
               (

              x.BankTransactionId,
              x.BankName,
              x.CustomerName,
              x.SupplierName,
              x.AnotherBank,
              x.TransactionNo,
              x.TransactionType.ToString(),

             new Tuple<decimal, DateTime?, string, string, string, string>(
                       x.Amount,
                       x.TranDate,
                       x.ChecqueNo,
                       x.Remarks,
                       x.AccountName,
                       x.AccountNo
                      )

               )).ToList();


        }

        public static IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string, Tuple<string, string, string, string>>>>
                GetAllBankTransaction
                  (this IBaseRepository<BankTransaction> bankTransactionRepository,
                  IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Bank> banks = bankRepository.All;
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Supplier> suppliers = supplierRepository.All;


            var items = (from bt in bankTransactionRepository.All
                         join b in bankRepository.All on bt.BankID equals b.BankID
                         join c1 in customerRepository.All on bt.CustomerID equals c1.CustomerID into customers1
                         from c in customers1.DefaultIfEmpty()
                         join s1 in supplierRepository.All on bt.SupplierID equals s1.SupplierID into supppliers1
                         from s in supppliers1.DefaultIfEmpty()
                         join b21 in bankRepository.All on bt.BankID equals b21.BankID into AnothersBanks1
                         from b2 in AnothersBanks1.DefaultIfEmpty()

                         select new
                         {
                             BankTransactionId = bt.BankTranID,
                             BankName = b.BankName,
                             CustomerName = c.Name,
                             SupplierName = s.Name,
                             AnotherBank = b2.BankName,
                             bt.TransactionNo,
                             bt.TransactionType,
                             bt.Amount,
                             bt.TranDate,
                             bt.Remarks,
                             CustomerAddress = c.Address,
                             CustomerContactNo = c.ContactNo,
                             SupplierAddress = s.Address,
                             SupplierContatcNo = s.ContactNo,
                             BranchName = b.BranchName,
                             AccountNo = b.AccountNo,
                             bt.ChecqueNo,
                             b.AccountName
                         }).ToList();
            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string, Tuple<string, string, string, string>>>
           (

          x.BankTransactionId,
          x.BankName,
          x.CustomerName,
          x.SupplierName,
          x.AnotherBank,
          x.TransactionNo,
          x.TransactionType.ToString(),
         new Tuple<decimal, DateTime?, string, string, string, string, string, Tuple<string, string, string, string>>(
                   x.Amount,
                   x.TranDate,
                   x.Remarks,
                   x.CustomerAddress,
                   x.CustomerContactNo,
                   x.SupplierAddress,
                   x.SupplierContatcNo,
                    new Tuple<string, string, string, string>(
                   x.BranchName,
                   x.AccountNo,
                   x.ChecqueNo,
                   x.AccountName
                  ))
           )).ToList();
        }





        public static IEnumerable<Tuple<int, string, string, string, string, string, string,
            Tuple<decimal, DateTime?, string, string, string, string, string,
                Tuple<string, string, string, string, EnumCustomerType>>>>
               GetBankTransactionData(this IBaseRepository<BankTransaction> bankTransactionRepository,
                                    IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
                                    IBaseRepository<Supplier> supplierRepository,
                                    DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID, int SupplierID,
                                    int EmployeeID,
                                    EnumTransactionType Status, bool IsAdminReport, EnumCustomerType customerType)
        {
            IQueryable<Bank> banks = null;
            IQueryable<Customer> customers = null;
            IQueryable<Supplier> suppliers = null;
            IQueryable<BankTransaction> BankTransactions = null;
            if (IsAdminReport)
            {
                if (ConcernID > 0)
                {
                    customers = customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    banks = bankRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    suppliers = supplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    BankTransactions = bankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                else
                {
                    banks = bankRepository.GetAll();
                    suppliers = supplierRepository.GetAll();
                    customers = customerRepository.GetAll();
                    BankTransactions = bankTransactionRepository.GetAll();
                }
            }
            else
            {
                if (CustomerID > 0)
                    customers = customerRepository.All.Where(i => i.CustomerID == CustomerID);
                else if (EmployeeID > 0)
                    customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
                else if (customerType > 0)
                    customers = customerRepository.All.Where(i => i.CustomerType == customerType);
                else
                    customers = customerRepository.All;


                banks = bankRepository.All;
                suppliers = supplierRepository.All;
                BankTransactions = bankTransactionRepository.All;
            }


            var items = (from bt in BankTransactions
                         join b in banks on bt.BankID equals b.BankID
                         join c1 in customers on bt.CustomerID equals c1.CustomerID into customers1
                         from c in customers1.DefaultIfEmpty()
                         join s1 in suppliers on bt.SupplierID equals s1.SupplierID into supppliers1
                         from s in supppliers1.DefaultIfEmpty()
                         join b21 in banks on bt.BankID equals b21.BankID into AnothersBanks1
                         from b2 in AnothersBanks1.DefaultIfEmpty()
                         where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && bt.TransactionType == (int)Status && c.CustomerType == customerType
                         select new
                         {
                             BankTransactionId = bt.BankTranID,
                             BankName = b.BankName,
                             CustomerName = c.Name,
                             SupplierName = s.Name,
                             AnotherBank = b2.BankName,
                             bt.TransactionNo,
                             bt.TransactionType,
                             bt.Amount,
                             bt.TranDate,
                             bt.Remarks,
                             CustomerAddress = c.Address,
                             CustomerContactNo = c.ContactNo,
                             SupplierAddress = s.Address,
                             SupplierContatcNo = s.ContactNo,
                             BranchName = b.BranchName,
                             AccountNo = b.AccountNo,
                             bt.ChecqueNo,
                             EmployeeName = c.Employee.Name,
                             CustomerType = c == null ? 0 : c.CustomerType == EnumCustomerType.Retail ? c.CustomerType
                                            : EnumCustomerType.Dealer,
                         }).ToList();


            return items.Select(x => new Tuple<int, string, string, string, string, string, string,
                Tuple<decimal, DateTime?, string, string, string, string, string,
                Tuple<string, string, string, string, EnumCustomerType>>>
           (
                  x.BankTransactionId,
                  x.BankName,
                  x.CustomerName,
                  x.SupplierName,
                  x.AnotherBank,
                  x.TransactionNo,
                  x.TransactionType.ToString(),
                 new Tuple<decimal, DateTime?, string, string, string, string, string,
                 Tuple<string, string, string, string, EnumCustomerType>>(
                           x.Amount,
                           x.TranDate,
                           x.Remarks,
                           x.CustomerAddress,
                           x.CustomerContactNo,
                           x.SupplierAddress,
                           x.SupplierContatcNo,
                            new Tuple<string, string, string, string, EnumCustomerType>(
                           x.BranchName,
                           x.AccountNo,
                           x.ChecqueNo,
                           x.EmployeeName,
                           x.CustomerType
                          )
                          )

           )).ToList();
        }

        /// <summary>
        /// Bank Ledger Report
        /// </summary>
        /// <summary>
        /// Bank Ledger Report
        /// </summary>
        public static List<BankTransReportModel> BankLedger(this IBaseRepository<BankTransaction> bankTransactionRepository,
                                    IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
                                    IBaseRepository<Supplier> supplierRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                                    int BankID, DateTime fromDate, DateTime toDate, int concernId)
        {
            List<BankTransReportModel> LedgerData = new List<BankTransReportModel>();
            var Banks = bankRepository.FindBy(d => d.ConcernID == concernId);
            var Customers = customerRepository.GetAll();
            var Suppliers = supplierRepository.GetAll();
            var banktrans = (from bt in bankTransactionRepository.GetAll()
                             where bt.BankID == BankID
                             select new BankTransReportModel
                             {
                                 BankID = bt.BankID,
                                 FromToBankID = bt.AnotherBankID,
                                 TransDate = (DateTime)bt.TranDate,
                                 TransactionNo = bt.TransactionNo,
                                 Amount = bt.Amount,
                                 TransactionType = (EnumTransactionType)bt.TransactionType,
                                 SupplierID = bt.SupplierID,
                                 CustomerID = bt.CustomerID,
                                 ConcernID = bt.ConcernID
                             }).ToList();

            var FundINbanktrans = (from bt in bankTransactionRepository.GetAll()
                                   where bt.AnotherBankID == BankID
                                   select new BankTransReportModel
                                   {
                                       BankID = bt.BankID,
                                       FromToBankID = bt.AnotherBankID,
                                       TransDate = (DateTime)bt.TranDate,
                                       TransactionNo = bt.TransactionNo,
                                       Amount = bt.Amount,
                                       TransType = "FundIN",
                                       SupplierID = bt.SupplierID,
                                       CustomerID = bt.CustomerID,
                                       ConcernID = bt.ConcernID
                                   }).ToList();
            banktrans.AddRange(FundINbanktrans);
            var AllTrans = banktrans.OrderBy(i => i.TransDate);

            BankTransReportModel oBankTran = new BankTransReportModel();
            var bank = bankRepository.FindBy(d => d.ConcernID == concernId).FirstOrDefault(i => i.BankID == BankID);
            decimal Opening = bank.OpeningBalance;
            Bank oBank = null;
            foreach (var item in AllTrans)
            {
                oBankTran.Opening = Opening;
                oBankTran.AccountName = bank.AccountName;
                oBankTran.BankName = bank.BankName;
                oBankTran.AccountNO = bank.AccountNo;
                oBankTran.TransDate = item.TransDate;
                oBankTran.TransactionNo = item.TransactionNo;
                oBankTran.ConcernID = item.ConcernID;

                if (item.TransactionType == EnumTransactionType.Deposit)
                {
                    oBankTran.Deposit = item.Amount;
                    oBankTran.Closing = oBankTran.Opening + item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.Withdraw)
                {
                    oBankTran.Withdraw = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.CashCollection)
                {
                    oBankTran.FromToAccountNo = Customers.FirstOrDefault(i => i.CustomerID == item.CustomerID).Name;
                    oBankTran.CashCollection = item.Amount;
                    oBankTran.Closing = oBankTran.Opening + item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.CashDelivery)
                {
                    oBankTran.FromToAccountNo = Suppliers.FirstOrDefault(i => i.SupplierID == item.SupplierID).Name;
                    oBankTran.CashDelivery = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.LiaPay) // Liability Pay
                {
                    oBankTran.LiaPay = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.LiaRec) // Liability Rec
                {
                    oBankTran.LiaRec = item.Amount;
                    oBankTran.Closing = oBankTran.Opening + item.Amount;
                }

                else if (item.TransactionType == EnumTransactionType.BankExpense) // BankExpense
                {
                    oBankTran.BankExpense = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.BankIncome) // Bank Income
                {
                    oBankTran.BankIncome = item.Amount;
                    oBankTran.Closing = oBankTran.Opening + item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.CustomerCollectionReturn) // Bank Income
                {
                    oBankTran.FundOut = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else if (item.TransactionType == EnumTransactionType.FundTransfer)//Fund Out
                {
                    oBank = bankRepository.FindBy(d => d.ConcernID == concernId).FirstOrDefault(i => i.BankID == item.FromToBankID);
                    oBankTran.FromToAccountNo = oBank.AccountName + " (" + oBank.AccountNo + ")";
                    oBankTran.FundOut = item.Amount;
                    oBankTran.Closing = oBankTran.Opening - item.Amount;
                }
                else //Fund IN
                {
                    oBank = bankRepository.FindBy(d => d.ConcernID == concernId).FirstOrDefault(i => i.BankID == item.BankID);
                    oBankTran.FromToAccountNo = oBank.AccountName + " (" + oBank.AccountNo + ")";
                    oBankTran.FundIN = item.Amount;
                    oBankTran.Closing = oBankTran.Opening + item.Amount;
                }
                Opening = oBankTran.Closing;

                LedgerData.Add(oBankTran);
                oBankTran = new BankTransReportModel();
            }
            return (from l in LedgerData.Where(i => i.TransDate >= fromDate && i.TransDate <= toDate).ToList()
                    join c in SisterConcernRepository.GetAll() on l.ConcernID equals c.ConcernID
                    select new BankTransReportModel
                    {
                        BankName = l.BankName,
                        AccountName = l.AccountName,
                        AccountNO = l.AccountNO,
                        TransDate = l.TransDate,
                        ConcernID = l.ConcernID,
                        ConcernName = c.Name,
                        TransactionNo = l.TransactionNo,
                        Opening = l.Opening,
                        Deposit = l.Deposit,
                        Withdraw = l.Withdraw,
                        CashCollection = l.CashCollection,
                        CashDelivery = l.CashDelivery,
                        FundIN = l.FundIN,
                        FundOut = l.FundOut,
                        FromToAccountNo = l.FromToAccountNo,
                        Closing = l.Closing,
                        BankIncome = l.BankIncome,
                        BankExpense = l.BankExpense,
                        LiaPay = l.LiaPay,
                        LiaRec = l.LiaRec
                    }).ToList();
        }


        public static IQueryable<CashCollectionReportModel> AdminCashCollectionByBank(this IBaseRepository<BankTransaction> bankTransactionRepository,
                                    IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> CustomerRepository,
                                    IBaseRepository<SisterConcern> SisterConcernRepository,
                                    int ConcernID, DateTime fromDate, DateTime toDate,
                                    EnumCustomerType customerType, int customerID)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID > 0)
            {
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                if (customerID != 0)
                    Customers = Customers.Where(i => i.CustomerID == customerID);
                else if (customerType != 0)
                    Customers = Customers.Where(i => i.CustomerType == customerType);

            }
            else
            {
                if (customerType != 0)
                    Customers = Customers.Where(i => i.CustomerType == customerType);
                else
                    Customers = CustomerRepository.GetAll();
            }

            var oAllCustomerCollData = from bt in bankTransactionRepository.GetAll()
                                       join c in Customers on bt.CustomerID equals c.CustomerID
                                       join b in bankRepository.GetAll() on bt.BankID equals b.BankID
                                       join sis in SisterConcernRepository.GetAll() on bt.ConcernID equals sis.ConcernID
                                       where (bt.TranDate >= fromDate && bt.TranDate <= toDate)
                                       select new CashCollectionReportModel
                                       {
                                           EntryDate = bt.TranDate,
                                           CustomerName = c.Name,
                                           CustomerCode = c.Code,
                                           Address = c.Address,
                                           ContactNo = c.ContactNo,
                                           TotalDue = c.TotalDue,
                                           Amount = bt.Amount,
                                           ModuleType = "Bank Trans",
                                           AccountNo = b.AccountNo,
                                           AccountName = b.AccountName,
                                           BankName = b.BankName,
                                           ReceiptNo = bt.TransactionNo,
                                           Remarks = bt.Remarks,
                                           BranchName = b.BranchName,
                                           ChecqueNo = bt.ChecqueNo,
                                           ConcernName = sis.Name,
                                           CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType
                                       };
            return oAllCustomerCollData;
        }

        public static async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>>
           GetAllBankTransactionAsync(this IBaseRepository<BankTransaction> bankTransactionRepository,
               IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
               IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate,
               List<EnumWFStatus> enumWFStatuses)
        {
            IQueryable<Bank> banks = bankRepository.All;
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Supplier> suppliers = supplierRepository.All;
            IQueryable<BankTransaction> bankTransactions = null;
            if (fromDate != DateTime.MinValue)
                bankTransactions = bankTransactionRepository.All.Where(bt => bt.TranDate >= fromDate && bt.TranDate <= toDate);
            else
                bankTransactions = bankTransactionRepository.All;

            var items = await (from bt in bankTransactions
                               join b in bankRepository.All on bt.BankID equals b.BankID
                               join c1 in customerRepository.All on bt.CustomerID equals c1.CustomerID into customers1
                               from c in customers1.DefaultIfEmpty()
                               join s1 in supplierRepository.All on bt.SupplierID equals s1.SupplierID into supppliers1
                               from s in supppliers1.DefaultIfEmpty()
                               join b21 in bankRepository.All on bt.AnotherBankID equals b21.BankID into AnothersBanks1
                               from b2 in AnothersBanks1.DefaultIfEmpty()
                               where enumWFStatuses.Contains(bt.WFStatus)
                               select new
                               {
                                   BankTransactionId = bt.BankTranID,
                                   BankName = b.BankName,
                                   b.AccountName,
                                   b.AccountNo,
                                   CustomerName = c.Name,
                                   SupplierName = s.Name,
                                   AnotherBank = b2.BankName,
                                   bt.TransactionNo,
                                   bt.TransactionType,
                                   bt.Amount,
                                   bt.TranDate,
                                   bt.ChecqueNo,
                                   bt.Remarks,
                                   bt.WFStatus,
                               }).OrderByDescending(i => i.TranDate).ThenByDescending(i => i.TransactionNo).ToListAsync();
            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>
               (

              x.BankTransactionId,
              x.BankName,
              x.CustomerName,
              x.SupplierName,
              x.AnotherBank,
              x.TransactionNo,
              x.TransactionType.ToString(),

             new Tuple<decimal, DateTime?, string, string, string, string, string>(
                       x.Amount,
                       x.TranDate,
                       x.ChecqueNo,
                       x.Remarks,
                       x.AccountName,
                       x.AccountNo,
                       x.WFStatus.ToString()
                      )

               )).ToList();


        }

        public static async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>>> GetAllBankTransactionNewAsync(this IBaseRepository<BankTransaction> bankTransactionRepository,
       IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
       IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate,
       List<EnumWFStatus> enumWFStatuses, int concernId)
        {
            IQueryable<Bank> banks = bankRepository.FindBy(d => d.ConcernID == concernId);
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Supplier> suppliers = supplierRepository.All;
            IQueryable<BankTransaction> bankTransactions = null;
            if (fromDate != DateTime.MinValue)
                bankTransactions = bankTransactionRepository.All.Where(bt => bt.TranDate >= fromDate && bt.TranDate <= toDate);
            else
                bankTransactions = bankTransactionRepository.All;

            var items = await (from bt in bankTransactions
                               join b in banks on bt.BankID equals b.BankID
                               join c1 in customerRepository.All on bt.CustomerID equals c1.CustomerID into customers1
                               from c in customers1.DefaultIfEmpty()
                               join s1 in supplierRepository.All on bt.SupplierID equals s1.SupplierID into supppliers1
                               from s in supppliers1.DefaultIfEmpty()
                               join b21 in banks on bt.AnotherBankID equals b21.BankID into AnothersBanks1
                               from b2 in AnothersBanks1.DefaultIfEmpty()
                               where enumWFStatuses.Contains(bt.WFStatus)
                               select new
                               {
                                   BankTransactionId = bt.BankTranID,
                                   BankName = b.BankName,
                                   b.AccountName,
                                   b.AccountNo,
                                   CustomerName = c.Name,
                                   SupplierName = s.Name,
                                   AnotherBank = b2.BankName,
                                   bt.TransactionNo,
                                   bt.TransactionType,
                                   bt.Amount,
                                   bt.TranDate,
                                   bt.ChecqueNo,
                                   bt.Remarks,
                                   bt.WFStatus,
                               }).OrderByDescending(i => i.TranDate).ThenByDescending(i => i.TransactionNo).ToListAsync();
            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string, string>>
               (

              x.BankTransactionId,
              x.BankName,
              x.CustomerName,
              x.SupplierName,
              x.AnotherBank,
              x.TransactionNo,
              x.TransactionType.ToString(),

             new Tuple<decimal, DateTime?, string, string, string, string, string>(
                       x.Amount,
                       x.TranDate,
                       x.ChecqueNo,
                       x.Remarks,
                       x.AccountName,
                       x.AccountNo,
                       x.WFStatus.ToString()
                      )

               )).ToList();


        }



        public static IEnumerable<Tuple<int, string, string, string, string, string, string,
            Tuple<decimal, DateTime?, string, string, string, string, string,
                Tuple<string, string, string, string, EnumCustomerType, string>>>>
               GetBankTransactionDataForAll(this IBaseRepository<BankTransaction> bankTransactionRepository,
                                    IBaseRepository<Bank> bankRepository, IBaseRepository<Customer> customerRepository,
                                    IBaseRepository<Supplier> supplierRepository,
                                    DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID, int SupplierID,
                                    int EmployeeID,
                                    EnumTransactionType Status, bool IsAdminReport, EnumCustomerType customerType, IBaseRepository<SisterConcern> sisterConcernRepository)
        {
            IQueryable<Bank> banks = null;
            IQueryable<Customer> customers = null;
            IQueryable<SisterConcern> sisterConcerns = null;
            IQueryable<Supplier> suppliers = null;
            IQueryable<BankTransaction> BankTransactions = null;
            if (IsAdminReport)
            {
                if (ConcernID > 0)
                {
                    customers = customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    banks = bankRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    suppliers = supplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    BankTransactions = bankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    sisterConcerns = sisterConcernRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                else
                {
                    banks = bankRepository.GetAll();
                    suppliers = supplierRepository.GetAll();
                    customers = customerRepository.GetAll();
                    BankTransactions = bankTransactionRepository.GetAll();
                    sisterConcerns = sisterConcernRepository.GetAll();
                }
            }
            else
            {



                if (ConcernID > 0)
                {
                    if (CustomerID > 0)
                        customers = customerRepository.All.Where(i => i.CustomerID == CustomerID && i.ConcernID == ConcernID);
                    else if (EmployeeID > 0)
                        customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID && i.ConcernID == ConcernID);
                    else if (customerType > 0)
                        customers = customerRepository.All.Where(i => i.CustomerType == customerType && i.ConcernID == ConcernID);
                    else
                        customers = customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    banks = bankRepository.All;
                    suppliers = supplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    BankTransactions = bankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    sisterConcerns = sisterConcernRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                else
                {
                    if (CustomerID > 0)
                        customers = customerRepository.All.Where(i => i.CustomerID == CustomerID);
                    else if (EmployeeID > 0)
                        customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
                    else if (customerType > 0)
                        customers = customerRepository.All.Where(i => i.CustomerType == customerType);
                    else
                        customers = customerRepository.All;

                    if (SupplierID > 0)
                    {
                        suppliers = supplierRepository.All.Where(i => i.SupplierID == SupplierID);
                        BankTransactions = bankTransactionRepository.GetAll().Where(i => i.SupplierID == SupplierID);
                    }
                    else
                        suppliers = supplierRepository.All;
                    banks = bankRepository.All;

                    BankTransactions = bankTransactionRepository.All;
                    sisterConcerns = sisterConcernRepository.All;
                }

                //banks = bankRepository.All;
                //suppliers = supplierRepository.All;
                //BankTransactions = bankTransactionRepository.All;
                //sisterConcerns = sisterConcernRepository.All;
            }

            if (SupplierID > 0)
            {
                var items = (from bt in BankTransactions
                             join b in banks on bt.BankID equals b.BankID
                             join sis in sisterConcerns on bt.ConcernID equals sis.ConcernID
                             join s1 in suppliers on bt.SupplierID equals s1.SupplierID
                             join b21 in banks on bt.BankID equals b21.BankID into AnothersBanks1
                             from b2 in AnothersBanks1.DefaultIfEmpty()
                             where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && bt.TransactionType == (int)Status
                             select new
                             {
                                 BankTransactionId = bt.BankTranID,
                                 BankName = b.BankName,
                                 SupplierName = s1.Name,
                                 AnotherBank = b2.BankName,
                                 bt.TransactionNo,
                                 bt.TransactionType,
                                 bt.Amount,
                                 bt.TranDate,
                                 bt.Remarks,
                                 SupplierAddress = s1.Address,
                                 SupplierContatcNo = s1.ContactNo,
                                 BranchName = b.BranchName,
                                 AccountNo = b.AccountNo,
                                 bt.ChecqueNo,
                                 ConcernName = sis.Name
                             }).ToList();

                return items.Select(x => new Tuple<int, string, string, string, string, string, string,
                    Tuple<decimal, DateTime?, string, string, string, string, string,
                    Tuple<string, string, string, string, EnumCustomerType, string>>>
               (
                      x.BankTransactionId,
                      x.BankName,
                      "",
                      x.SupplierName,
                      x.AnotherBank,
                      x.TransactionNo,
                      x.TransactionType.ToString(),
                     new Tuple<decimal, DateTime?, string, string, string, string, string,
                     Tuple<string, string, string, string, EnumCustomerType, string>>(
                               x.Amount,
                               x.TranDate,
                               x.Remarks,
                               "",
                               "",
                               x.SupplierAddress,
                               x.SupplierContatcNo,
                                new Tuple<string, string, string, string, EnumCustomerType, string>(
                               x.BranchName,
                               x.AccountNo,
                               x.ChecqueNo,
                               "",
                               0,
                               x.ConcernName
                              )
                              )

               )).ToList();
            }
            else if (CustomerID > 0)
            {

                var items = (from bt in BankTransactions
                             join b in banks on bt.BankID equals b.BankID
                             join sis in sisterConcerns on bt.ConcernID equals sis.ConcernID
                             join c in customers on bt.CustomerID equals c.CustomerID
                             join b21 in banks on bt.BankID equals b21.BankID into AnothersBanks1
                             from b2 in AnothersBanks1.DefaultIfEmpty()
                             where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && bt.TransactionType == (int)Status
                             select new
                             {
                                 BankTransactionId = bt.BankTranID,
                                 BankName = b.BankName,
                                 CustomerName = c.Name,
                                 SupplierName = "",
                                 AnotherBank = b2.BankName,
                                 bt.TransactionNo,
                                 bt.TransactionType,
                                 bt.Amount,
                                 bt.TranDate,
                                 bt.Remarks,
                                 CustomerAddress = c.Address,
                                 CustomerContactNo = c.ContactNo,
                                 SupplierAddress = "",
                                 SupplierContatcNo = "",
                                 BranchName = b.BranchName,
                                 AccountNo = b.AccountNo,
                                 bt.ChecqueNo,
                                 EmployeeName = c.Employee.Name,
                                 CustomerType = c == null ? 0 : c.CustomerType == EnumCustomerType.Retail ? c.CustomerType : EnumCustomerType.Dealer,
                                 ConcernName = sis.Name
                             }).ToList();

                return items.Select(x => new Tuple<int, string, string, string, string, string, string,
                    Tuple<decimal, DateTime?, string, string, string, string, string,
                    Tuple<string, string, string, string, EnumCustomerType, string>>>
               (
                      x.BankTransactionId,
                      x.BankName,
                      x.CustomerName,
                      x.SupplierName,
                      x.AnotherBank,
                      x.TransactionNo,
                      x.TransactionType.ToString(),
                     new Tuple<decimal, DateTime?, string, string, string, string, string,
                     Tuple<string, string, string, string, EnumCustomerType, string>>(
                               x.Amount,
                               x.TranDate,
                               x.Remarks,
                               x.CustomerAddress,
                               x.CustomerContactNo,
                               x.SupplierAddress,
                               x.SupplierContatcNo,
                                new Tuple<string, string, string, string, EnumCustomerType, string>(
                               x.BranchName,
                               x.AccountNo,
                               x.ChecqueNo,
                               x.EmployeeName,
                               x.CustomerType,
                               x.ConcernName
                              )
                              )

               )).ToList();

            }
            else
            {
                var items = (from bt in BankTransactions
                             join b in banks on bt.BankID equals b.BankID
                             join sis in sisterConcerns on bt.ConcernID equals sis.ConcernID
                             join c1 in customers on bt.CustomerID equals c1.CustomerID into customers1
                             from c in customers1.DefaultIfEmpty()
                             join s1 in suppliers on bt.SupplierID equals s1.SupplierID into supppliers1
                             from s in supppliers1.DefaultIfEmpty()
                             join b21 in banks on bt.BankID equals b21.BankID into AnothersBanks1
                             from b2 in AnothersBanks1.DefaultIfEmpty()
                             where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && bt.TransactionType == (int)Status
                             select new
                             {
                                 BankTransactionId = bt.BankTranID,
                                 BankName = b.BankName,
                                 CustomerName = c.Name,
                                 SupplierName = s.Name,
                                 AnotherBank = b2.BankName,
                                 bt.TransactionNo,
                                 bt.TransactionType,
                                 bt.Amount,
                                 bt.TranDate,
                                 bt.Remarks,
                                 CustomerAddress = c.Address,
                                 CustomerContactNo = c.ContactNo,
                                 SupplierAddress = s.Address,
                                 SupplierContatcNo = s.ContactNo,
                                 BranchName = b.BranchName,
                                 AccountNo = b.AccountNo,
                                 bt.ChecqueNo,
                                 EmployeeName = c.Employee.Name,
                                 CustomerType = c == null ? 0 : c.CustomerType == EnumCustomerType.Retail ? c.CustomerType : EnumCustomerType.Dealer,
                                 ConcernName = sis.Name
                             }).ToList();

                return items.Select(x => new Tuple<int, string, string, string, string, string, string,
                    Tuple<decimal, DateTime?, string, string, string, string, string,
                    Tuple<string, string, string, string, EnumCustomerType, string>>>
               (
                      x.BankTransactionId,
                      x.BankName,
                      x.CustomerName,
                      x.SupplierName,
                      x.AnotherBank,
                      x.TransactionNo,
                      x.TransactionType.ToString(),
                     new Tuple<decimal, DateTime?, string, string, string, string, string,
                     Tuple<string, string, string, string, EnumCustomerType, string>>(
                               x.Amount,
                               x.TranDate,
                               x.Remarks,
                               x.CustomerAddress,
                               x.CustomerContactNo,
                               x.SupplierAddress,
                               x.SupplierContatcNo,
                                new Tuple<string, string, string, string, EnumCustomerType, string>(
                               x.BranchName,
                               x.AccountNo,
                               x.ChecqueNo,
                               x.EmployeeName,
                               x.CustomerType,
                               x.ConcernName
                              )
                              )

               )).ToList();
            }



        }
    }
}
