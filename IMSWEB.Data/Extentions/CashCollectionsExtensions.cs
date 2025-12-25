using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Model.SPModel;
using System.Data;
namespace IMSWEB.Data
{
    public static class CashCollectionsExtensions
    {
        public static async Task<IEnumerable<CashCollection>> GetAllCashCollectionAsync(this IBaseRepository<CashCollection> cashcollectionRepository)
        {
            return await cashcollectionRepository.All.ToListAsync();

        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string>>>>
        GetAllCashCollAsync(this IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<Customer> customerRepository,
            DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> customers = customerRepository.All;

            var items = await cashCollectionRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate).Join(customers,
                cashcoll => cashcoll.Customer.CustomerID, cus => cus.CustomerID, (cashcoll, cus) => new { CashCollection = cashcoll, Customer = cus }).
                Select(x => new
                {
                    x.CashCollection.CashCollectionID,
                    x.CashCollection.EntryDate,
                    x.CashCollection.ReceiptNo,
                    x.CashCollection.Customer.Code,
                    x.CashCollection.Customer.Name,
                    x.CashCollection.AccountNo,
                    x.CashCollection.Amount,
                    x.CashCollection.TransactionType,
                    x.CashCollection.Remarks
                }).ToListAsync();

            return items.Select(x => new Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string>>
                (
                    x.CashCollectionID,
                    x.EntryDate.Value,
                    x.ReceiptNo.ToString(),
                    x.Name.ToString(),
                    x.AccountNo.ToString(),
                    x.Amount.ToString(),
                    x.TransactionType.ToString(),
                     new Tuple<string, string>(x.Remarks, x.Code)
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string>>>> GetAllCashCollByEmployeeIDAsync(this IBaseRepository<CashCollection> cashCollectionRepository,

            IBaseRepository<Customer> customerRepository, int EmployeeID
            )
        {
            IQueryable<Customer> customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);

            var items = await cashCollectionRepository.All.Join(customers,
                cashcoll => cashcoll.Customer.CustomerID, cus => cus.CustomerID, (cashcoll, cus) => new { CashCollection = cashcoll, Customer = cus }).
                Select(x => new
                {
                    x.CashCollection.CashCollectionID,
                    x.CashCollection.EntryDate,
                    x.CashCollection.ReceiptNo,
                    x.CashCollection.Customer.Code,
                    x.CashCollection.Customer.Name,
                    x.CashCollection.AccountNo,
                    x.CashCollection.Amount,
                    x.CashCollection.TransactionType,
                    x.CashCollection.Remarks
                }).ToListAsync();

            return items.Select(x => new Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string>>
                (
                    x.CashCollectionID,
                    x.EntryDate.Value,
                    x.ReceiptNo.ToString(),
                    x.Name.ToString(),
                    x.AccountNo.ToString(),
                    x.Amount.ToString(),
                    x.TransactionType.ToString(),
                    new Tuple<string, string>(x.Remarks, x.Code)

                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string>>>
        GetAllCashDelivaeryAsync(this IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<Supplier> supplierRepository,
            DateTime fromDate, DateTime toDate)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await cashCollectionRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate).Join(suppliers,
                cashcoll => cashcoll.Supplier.SupplierID, sup => sup.SupplierID, (cashcoll, sup) => new { CashCollection = cashcoll, Supplier = sup }).
                Select(x => new
                {
                    x.CashCollection.CashCollectionID,
                    x.CashCollection.EntryDate,
                    x.CashCollection.ReceiptNo,
                    x.CashCollection.Supplier.Name,
                    x.CashCollection.AccountNo,
                    x.CashCollection.Amount,
                    x.CashCollection.TransactionType
                }).ToListAsync();

            return items.Select(x => new Tuple<int, DateTime, string, string, string, string, string>
                (
                    x.CashCollectionID,
                    x.EntryDate.Value,
                    x.ReceiptNo.ToString(),
                    x.Name.ToString(),
                    x.AccountNo.ToString(),
                    x.Amount.ToString(),
                    x.TransactionType.ToString()
                )).OrderByDescending(x => x.Item1).ToList();
        }


        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, string, string, string, string, string, EnumCustomerType>>>
        GetCashCollectionData(
            this IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Customer> CustomerRepository,
        DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID, EnumCustomerType customerType)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerID > 0)
                Customers = CustomerRepository.All.Where(i => i.CustomerID == CustomerID);
            else
            {
                if (customerType > 0)
                    Customers = CustomerRepository.All.Where(i => i.CustomerType == customerType);
                else
                    Customers = CustomerRepository.All;
            }

            var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                        join CO in Customers on CC.CustomerID equals CO.CustomerID
                                        where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate && CO.CustomerType == customerType)
                                        select new
                                        {
                                            CC.EntryDate,
                                            CO.Name,
                                            CO.Address,
                                            CO.ContactNo,
                                            CO.TotalDue,
                                            CC.Amount,
                                            RemaingAmt = CC.Amount,
                                            CC.AdjustAmt,
                                            CC.PaymentType,
                                            CC.BankName,
                                            CC.AccountNo,
                                            CC.BranchName,
                                            CC.BKashNo,
                                            CustomerType = CO.CustomerType == EnumCustomerType.Retail ? CO.CustomerType : EnumCustomerType.Dealer,
                                        }).OrderByDescending(x => x.EntryDate).ToList();

            return oAllCustomerCollData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal,
                Tuple<decimal, string, string, string, string, string, EnumCustomerType>>
                (
                 (DateTime)x.EntryDate,
                 x.Name,
                x.Address,
                x.ContactNo,
                x.TotalDue,
                x.Amount,
                x.RemaingAmt, new Tuple<decimal, string, string, string, string, string, EnumCustomerType>(
                                    x.AdjustAmt,
                                    x.PaymentType.ToString(),
                                    x.BankName,
                                   x.AccountNo,
                                   x.BranchName,
                                   x.BKashNo,
                                   x.CustomerType
                                   )
                ));

        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>>
        GetCashDeliveryData(this IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Supplier> SupplierRepository,
        DateTime fromDate, DateTime toDate, int ConcernID, int SupplierID, bool IsAdminReport, IBaseRepository<SisterConcern> SisterConcernRepository)
        {
            IQueryable<Supplier> suppliers = null;
            IQueryable<SisterConcern> sisterConcerns = null;
            IQueryable<CashCollection> cashCollections = null;
            if (IsAdminReport)
            {
                if (ConcernID > 0)
                {

                    sisterConcerns = SisterConcernRepository.GetAllByConcernID(ConcernID);
                    suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                    cashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                else
                {
                    suppliers = SupplierRepository.GetAll();
                    cashCollections = CashCollectionRepository.GetAll();
                    sisterConcerns = SisterConcernRepository.GetAll();
                }
            }
            else
            {
                if (ConcernID > 0)
                {
                    if (SupplierID > 0)
                    {
                        sisterConcerns = SisterConcernRepository.GetAllByConcernID(ConcernID);
                        suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.SupplierID == SupplierID);
                        cashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.SupplierID == SupplierID);
                    }
                    else
                        sisterConcerns = SisterConcernRepository.GetAllByConcernID(ConcernID);
                        suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                        cashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID);
                }
                //if (SupplierID > 0)
                //{
                //    sisterConcerns = SisterConcernRepository.GetAll();
                //    suppliers = SupplierRepository.All.Where(i => i.SupplierID == SupplierID);
                //    cashCollections = CashCollectionRepository.All.Where(i => i.SupplierID == SupplierID);
                //}
                else
                {
                    if (SupplierID > 0)
                    {
                        sisterConcerns = SisterConcernRepository.GetAll();
                        suppliers = SupplierRepository.All.Where(i => i.SupplierID == SupplierID);
                        cashCollections = CashCollectionRepository.All.Where(i => i.SupplierID == SupplierID);
                    }
                    else
                    {
                        sisterConcerns = SisterConcernRepository.GetAll();
                        suppliers = SupplierRepository.All;
                        cashCollections = CashCollectionRepository.All;
                    }

                }
            }

            var oSupplierDeliData = (from CC in cashCollections
                                     join SUP in suppliers on CC.SupplierID equals SUP.SupplierID
                                     join SIS in sisterConcerns on CC.ConcernID equals SIS.ConcernID
                                     where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate) && CC.TransactionType == EnumTranType.ToCompany
                                     select new
                                     {
                                         CC.EntryDate,
                                         SUP.Name,
                                         SUP.Address,
                                         SUP.ContactNo,
                                         SUP.TotalDue,
                                         CC.Amount,
                                         RemaingAmt = CC.Amount,
                                         CC.AdjustAmt,
                                         CC.PaymentType,
                                         CC.BankName,
                                         CC.AccountNo,
                                         CC.BranchName,
                                         CC.BKashNo,
                                         ConcernName = SIS.Name
                                     }).OrderByDescending(x => x.EntryDate).ToList();

            return oSupplierDeliData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>
                (
                 (DateTime)x.EntryDate,
                 x.Name,
                x.Address,
                x.ContactNo,
                x.TotalDue,
                x.Amount,
                x.RemaingAmt, new Tuple<decimal, string, string, string, string, string, string>(
                                    x.AdjustAmt,
                                    x.PaymentType.ToString(),
                                    x.BankName,
                                   x.AccountNo,
                                   x.BranchName,
                                   x.BKashNo,
                                   x.ConcernName)
                ));



        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>>
            GetSRWiseCashCollectionReportData(
            this IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Bank> Bank, IBaseRepository<BankTransaction> BankTransaction,
            DateTime fromDate, DateTime toDate, int ConcernID, int EmployeeID)
        {
            if (EmployeeID > 0)
            {
                var oCustomerCollData = (from CC in CashCollectionRepository.All
                                         join CO in CustomerRepository.All on CC.CustomerID equals CO.CustomerID
                                         join EMP in EmployeeRepository.All on CO.EmployeeID equals EMP.EmployeeID
                                         where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate && CC.ConcernID == ConcernID && EMP.EmployeeID == EmployeeID)
                                         select new
                                         {
                                             CC.EntryDate,
                                             CO.Name,
                                             CO.Address,
                                             CO.ContactNo,
                                             CO.TotalDue,
                                             CC.Amount,
                                             RemaingAmt = CC.Amount,
                                             AdjustAmt = CC.AdjustAmt,
                                             PaymentType = "Cash",
                                             CC.BankName,
                                             CC.AccountNo,
                                             CC.BranchName,
                                             CC.BKashNo,
                                             EmployeeName = EMP.Name
                                         }).OrderByDescending(x => x.EntryDate);

                var oBankCollData = (from CC in BankTransaction.All
                                     join B in Bank.All on CC.BankID equals B.BankID
                                     join CO in CustomerRepository.All on CC.CustomerID equals CO.CustomerID
                                     join EMP in EmployeeRepository.All on CO.EmployeeID equals EMP.EmployeeID
                                     where (CC.TranDate >= fromDate && CC.TranDate <= toDate && CC.ConcernID == ConcernID && EMP.EmployeeID == EmployeeID)
                                     select new
                                     {
                                         EntryDate = CC.TranDate,
                                         CO.Name,
                                         CO.Address,
                                         CO.ContactNo,
                                         CO.TotalDue,
                                         CC.Amount,
                                         RemaingAmt = CC.Amount,
                                         AdjustAmt = 0m,
                                         PaymentType = "Checque",
                                         BankName = B.BankName,
                                         AccountNo = B.AccountNo,
                                         BranchName = B.BranchName,
                                         BKashNo = "",
                                         EmployeeName = EMP.Name
                                     }).OrderByDescending(x => x.EntryDate);

                var joinedoCustomerCollData = oCustomerCollData.Concat(oBankCollData).ToList();

                return joinedoCustomerCollData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>
                    (
                     (DateTime)x.EntryDate,
                     x.Name,
                    x.Address,
                    x.ContactNo,
                    x.TotalDue,
                    x.Amount,
                    x.RemaingAmt, new Tuple<decimal, string, string, string, string, string, string>(
                                        x.AdjustAmt,
                                        x.PaymentType.ToString(),
                                        x.BankName,
                                       x.AccountNo,
                                       x.BranchName,
                                       x.BKashNo,
                                       x.EmployeeName
                                       )
                    ));

            }
            else
            {
                var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                            join CO in CustomerRepository.All on CC.CustomerID equals CO.CustomerID
                                            join EMP in EmployeeRepository.All on CO.EmployeeID equals EMP.EmployeeID
                                            where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate && CC.ConcernID == ConcernID)
                                            select new
                                            {
                                                CC.EntryDate,
                                                CO.Name,
                                                CO.Address,
                                                CO.ContactNo,
                                                CO.TotalDue,
                                                CC.Amount,
                                                RemaingAmt = CC.Amount,
                                                CC.AdjustAmt,
                                                PaymentType = CC.PaymentType.ToString(),
                                                CC.BankName,
                                                CC.AccountNo,
                                                CC.BranchName,
                                                CC.BKashNo,
                                                EmployeeName = EMP.Name
                                            }).OrderByDescending(x => x.EntryDate);

                var oAllBankCollData = (from CC in BankTransaction.All
                                        join B in Bank.All on CC.BankID equals B.BankID
                                        join CO in CustomerRepository.All on CC.CustomerID equals CO.CustomerID
                                        join EMP in EmployeeRepository.All on CO.EmployeeID equals EMP.EmployeeID
                                        where (CC.TranDate >= fromDate && CC.TranDate <= toDate && CC.ConcernID == ConcernID)
                                        select new
                                        {
                                            EntryDate = CC.TranDate,
                                            CO.Name,
                                            CO.Address,
                                            CO.ContactNo,
                                            CO.TotalDue,
                                            CC.Amount,
                                            RemaingAmt = CC.Amount,
                                            AdjustAmt = 0m,
                                            PaymentType = "Cheque",
                                            BankName = B.BankName,
                                            AccountNo = B.AccountNo,
                                            BranchName = B.BranchName,
                                            BKashNo = "",
                                            EmployeeName = EMP.Name
                                        }).OrderByDescending(x => x.EntryDate);

                var joinedoAllCustomerCollData = oAllCustomerCollData.Concat(oAllBankCollData).ToList();


                return joinedoAllCustomerCollData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, string, string, string, string, string, string>>
                    (
                     (DateTime)x.EntryDate,
                     x.Name,
                    x.Address,
                    x.ContactNo,
                    x.TotalDue,
                    x.Amount,
                    x.RemaingAmt, new Tuple<decimal, string, string, string, string, string, string>(
                                        x.AdjustAmt,
                                        x.PaymentType.ToString(),
                                        x.BankName,
                                       x.AccountNo,
                                       x.BranchName,
                                       x.BKashNo,
                                       x.EmployeeName

                                       )
                    ));
            }
        }

        public static IQueryable<CashCollectionReportModel>
            AdminCashCollectionReport(this IBaseRepository<CashCollection> CashCollectionRepository,
                        IBaseRepository<Customer> CustomerRepository,
                        IBaseRepository<SisterConcern> SisterConcernRepository,
                        DateTime fromDate, DateTime toDate, int ConcernID,
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

            var oAllCustomerCollData = from CC in CashCollectionRepository.GetAll()
                                       join c in Customers on CC.CustomerID equals c.CustomerID
                                       join sis in SisterConcernRepository.GetAll() on CC.ConcernID equals sis.ConcernID
                                       where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                       select new CashCollectionReportModel
                                       {
                                           EntryDate = CC.EntryDate,
                                           CustomerName = c.Name,
                                           CustomerCode = c.Code,
                                           Address = c.Address,
                                           ContactNo = c.ContactNo,
                                           TotalDue = c.TotalDue,
                                           Amount = CC.Amount,
                                           AdjustAmt = CC.AdjustAmt,
                                           PaymentType = CC.PaymentType,
                                           AccountNo = CC.AccountNo,
                                           ReceiptNo = CC.ReceiptNo,
                                           Remarks = CC.Remarks,
                                           ConcernName = sis.Name,
                                           ModuleType = "Cash Collection",
                                           CustomerType = c.CustomerType == EnumCustomerType.Hire ? EnumCustomerType.Retail : c.CustomerType
                                       };
            return oAllCustomerCollData;
        }

        public static bool IsCommissionApplicable(this IBaseRepository<CashCollection> CashCollectionRepository,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
            IBaseRepository<SOrder> SOrderRepository,
            DateTime fromDate, DateTime toDate, int EmployeeID)
        {
            bool IsApplicable = false;
            var PresentSaleCollection = GetTotalSalesAmtTotalCollectionAmount(CashCollectionRepository, CustomerRepository, BankTransactionRepository, SOrderRepository, fromDate, toDate, EmployeeID);
            DateTime PrevFromDate = fromDate.AddMonths(-1);
            DateTime PrevToDate = PrevFromDate.AddMonths(1).AddDays(-1);
            PrevToDate = new DateTime(PrevToDate.Year, PrevToDate.Month, PrevToDate.Day, 23, 59, 59);
            var PreviousSaleCollection = GetTotalSalesAmtTotalCollectionAmount(CashCollectionRepository, CustomerRepository, BankTransactionRepository, SOrderRepository, PrevFromDate, PrevToDate, EmployeeID);
            decimal PrevDue = PreviousSaleCollection.Item1 - PreviousSaleCollection.Item2;
            if (PrevDue > 0) //Previous Due +Ve
            {
                if (PresentSaleCollection.Item1 < PresentSaleCollection.Item2)
                {
                    decimal ExtraColletions = PresentSaleCollection.Item2 - PresentSaleCollection.Item1;
                    decimal percentage = (ExtraColletions * 100) / PrevDue;
                    if (percentage > 10)
                        IsApplicable = true;
                }
            } //Previous Due zero or -Ve
            else
            {
                if (PresentSaleCollection.Item2 >= PresentSaleCollection.Item1)
                {
                    IsApplicable = true;
                }
            }
            return IsApplicable;
        }

        public static Tuple<decimal, decimal> GetTotalSalesAmtTotalCollectionAmount(this IBaseRepository<CashCollection> CashCollectionRepository,
                IBaseRepository<Customer> CustomerRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
                IBaseRepository<SOrder> SOrderRepository,
                DateTime fromDate, DateTime toDate, int EmployeeID)
        {
            var EMPRecAmounts = (from so in SOrderRepository.All
                                 join c in CustomerRepository.All on so.CustomerID equals c.CustomerID
                                 where so.Status == (int)EnumSalesType.Sales && (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate)
                                 && c.EmployeeID == EmployeeID
                                 select new
                                 {
                                     RecAmount = (decimal)so.RecAmount,
                                     so.TotalAmount
                                 }).ToList();

            var EMPCashCollections = (from cc in CashCollectionRepository.All
                                      join c in CustomerRepository.All on cc.CustomerID equals c.CustomerID
                                      where (cc.EntryDate >= fromDate && cc.EntryDate <= toDate) && c.EmployeeID == EmployeeID
                                      select new
                                      {
                                          cc.Amount
                                      }).ToList();

            var EMPBankCollections = (from bt in BankTransactionRepository.All
                                      join c in CustomerRepository.All on bt.CustomerID equals c.CustomerID
                                      where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && c.EmployeeID == EmployeeID
                                      select new
                                      {
                                          bt.Amount
                                      }).ToList();

            decimal TotalCollectionAmt = EMPRecAmounts.Sum(i => i.RecAmount) + EMPCashCollections.Sum(i => i.Amount) + EMPBankCollections.Sum(i => i.Amount);
            decimal TotalSales = EMPRecAmounts.Sum(i => i.TotalAmount);

            return new Tuple<decimal, decimal>(TotalSales, TotalCollectionAmt);

        }




        public static List<CashInHandModel> CashInHandReport(



      this IBaseRepository<CashCollection> CashCollectionRepository,
          IBaseRepository<POrder> POrderRepository,
          IBaseRepository<POrderDetail> POrderDetailRepository,
          IBaseRepository<SOrder> SOrderRepository,
          IBaseRepository<SOrderDetail> SOrderDetailRepository,

          IBaseRepository<CreditSale> CreditSaleRepository,
          IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
          IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
          IBaseRepository<Stock> StockRepository,
          IBaseRepository<StockDetail> StockDetailRepository,

         IBaseRepository<ExpenseItem> ExpenseItemRepository,
         IBaseRepository<Expenditure> ExpenditureRepository,
         IBaseRepository<Bank> BankRepository,
         IBaseRepository<BankTransaction> BankTransactionRepository,

          IBaseRepository<ROrder> ROrderRepository,
          IBaseRepository<ROrderDetail> ROrderDetailRepository,


          IBaseRepository<Customer> CustomerRepository,
          IBaseRepository<SisterConcern> SisterConcernRepository,
          DateTime fromDate, DateTime toDate, int ConcernID
          )
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID > 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var oAllCustomerCollData = from CC in CashCollectionRepository.GetAll()
                                       join CO in Customers on CC.CustomerID equals CO.CustomerID
                                       join sis in SisterConcernRepository.GetAll() on CC.ConcernID equals sis.ConcernID
                                       where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                       select new CashCollectionReportModel
                                       {
                                           EntryDate = CC.EntryDate,
                                           CustomerName = CO.Name,
                                           CustomerCode = CO.Code,
                                           Address = CO.Address,
                                           ContactNo = CO.ContactNo,
                                           TotalDue = CO.TotalDue,
                                           Amount = CC.Amount,
                                           AdjustAmt = CC.AdjustAmt,
                                           PaymentType = CC.PaymentType,
                                           AccountNo = CC.AccountNo,
                                           ReceiptNo = CC.ReceiptNo,
                                           Remarks = CC.Remarks,
                                           ConcernName = sis.Name,
                                           ModuleType = "Cash Collection"
                                       };

            DateTime startDate = new DateTime(2019, 8, 30, 16, 5, 7, 123);

            string sFFdate = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
            startDate = Convert.ToDateTime(sFFdate);
            decimal StartAmount = 10805m;

            double PCashInHandAmt = 0;
            double TotalIncomeAmt = 0;

            double CCashInHandAmt = 0;
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double Opening_CashInhand = 0;
            double Current_CashInhand = 0;
            double Closing_CashInhand = 0;


            var Expense_Purchases_Details = (
                                          from POD in POrderDetailRepository.All
                                          join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,
                                              PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              PaymentDue = (((PO.GrandTotal + PO.LaborCost - PO.AdjAmount - PO.NetDiscount - PO.RecAmt) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              RecAmt = PO.RecAmt
                                          }
                             );



            var Expense_Purchases = (
                                          from PO in POrderRepository.All
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,

                                              RecAmt = PO.RecAmt

                                          }
                             );

            var Expense_Purchases_For_Sales_Details = (from SOD in SOrderDetailRepository.All
                                                       join SO in SOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                                       join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                       where SO.Status == 1 && SO.InvoiceDate >= startDate
                                                       select new
                                                       {

                                                           SO.InvoiceDate,
                                                           PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * SOD.Quantity
                                                       }
                         );

            var Expense_Purchases_For_CreditSales_Details = (from CSP in CreditSaleDetailsRepository.All
                                                             join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                                             join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                                             join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                             join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                             where CS.SalesDate >= startDate
                                                             select new
                                                             {
                                                                 CS.SalesDate,
                                                                 PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity
                                                             }
                        );




            var Expense_LastPay =
                 (from CS in CreditSaleRepository.All
                  join CSD in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSD.CreditSalesID
                  where CSD.PaymentStatus == "Paid"
                  select new
                  {
                      LastPayAdjustment = 0m

                  }
                     );



            var Expense_CustomerProductReturn = (from R in ROrderRepository.All
                                                 where R.CustomerID != null
                                                 where R.ReturnDate >= startDate
                                                 select new
                                                 {
                                                     GrandTotal = R.GrandTotal,
                                                     ReturnDate = R.ReturnDate,
                                                     PaymentDue = R.GrandTotal - R.PaidAmount,
                                                     PaidAmount = R.PaidAmount
                                                 }
                                      );



            var Expense_CustomerProductReturn_with_Purchase_Details = (from R in ROrderRepository.All
                                                                       join RTD in ROrderDetailRepository.All on R.ROrderID equals RTD.ROrderID
                                                                       join STD in StockDetailRepository.All on RTD.StockDetailID equals STD.SDetailID
                                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                       where R.CustomerID != null && R.ReturnDate >= startDate
                                                                       select new
                                                                       {
                                                                           GrandTotal = R.GrandTotal,
                                                                           ReturnDate = R.ReturnDate,
                                                                           Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                       }
                                    );




            var Expense_Direct = (from ex in ExpenditureRepository.All
                                  join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                  where ((int)exi.Status == 1 && ex.EntryDate >= startDate)
                                  select new
                                  {
                                      ex.EntryDate,
                                      exi.Description,
                                      ex.Amount,

                                  }
                       ).ToList();



            var Expense_CashDelivery = (from csd in CashCollectionRepository.All
                                        where (csd.TransactionType == EnumTranType.ToCompany && csd.EntryDate >= startDate)
                                        select new
                                        {
                                            csd.EntryDate,
                                            csd.Amount,

                                        }
                     ).ToList();
            var Expense_CashCollection_Adjustment = (from CAS in CashCollectionRepository.All
                                                     where CAS.TransactionType == EnumTranType.FromCustomer && CAS.EntryDate >= startDate
                                                     select new
                                                     {
                                                         AdjustAmt = CAS.AdjustAmt,
                                                         CAS.EntryDate,

                                                     }
                 );


            var Expense_BankDeposit =
                                  (from BT in BankTransactionRepository.All
                                   where BT.TransactionType == (int)EnumTransactionType.Deposit && BT.TranDate >= startDate
                                   select new
                                   {
                                       BT.Amount,
                                       BT.TranDate

                                   }
                 );


            //var Expense_ShareInvestment =
            //    (

            //    from SI in db.ShareInvestments
            //    join SH in db.ShareInvestmentHeads on SI.SIHID equals SH.SIHID
            //    where SI.TransactionType == 2 && (SH.ParentId == 2 || SH.ParentId == 3 || SH.ParentId == 4) && SI.EntryDate >= startDate
            //    select new
            //    {
            //        SH.Name,
            //        SI.Amount,
            //        SI.EntryDate

            //    }
            //     )
            ;


            //Income--------------------------------------------------------------------------------------------------




            var Income_Sales = (from SO in SOrderRepository.All
                                where SO.Status == 1 && SO.InvoiceDate >= startDate
                                select new
                                {
                                    SO.InvoiceDate,
                                    RecAmount = SO.RecAmount.Value,
                                    PaymentDue = SO.PaymentDue,
                                    SalesAmt = (SO.GrandTotal - SO.NetDiscount - SO.AdjAmount)

                                }
                                    );


            var Income_Credit_Sales = (from CS in CreditSaleRepository.All
                                       where CS.SalesDate >= startDate
                                       select new
                                       {
                                           CS.SalesDate,
                                           CS.DownPayment,
                                           SalesAmt = (CS.TSalesAmt - CS.Discount - CS.WInterestAmt)

                                       }
                     );


            var Income_Direct = (from ex in ExpenditureRepository.All
                                 join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                 where ((int)exi.Status == 2 && ex.EntryDate >= startDate)
                                 select new
                                 {
                                     exi.Description,
                                     ex.EntryDate,
                                     ex.Amount,

                                 }
                       ).ToList();

            var Income_CashCollection = (from csd in CashCollectionRepository.All
                                         where (csd.TransactionType == EnumTranType.FromCustomer && csd.EntryDate >= startDate)
                                         select new
                                         {
                                             csd.EntryDate,
                                             csd.Amount,


                                         }
                      ).ToList();

            var Income_CashDelivery_Adjustment = (from CAS in CashCollectionRepository.All
                                                  where CAS.TransactionType == EnumTranType.ToCompany
                                                  where CAS.EntryDate >= startDate
                                                  select new
                                                  {
                                                      AdjustAmt = CAS.AdjustAmt,
                                                      CAS.EntryDate
                                                  }
               );


            var Income_InstallmentCollection = (
                                  from csd in CreditSalesScheduleRepository.All
                                  join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                                  where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                                  select new
                                  {
                                      csd.PaymentDate,
                                      csd.InstallmentAmt,

                                  }
                        );

            var Income_HireCollection = (
                     from csd in CreditSalesScheduleRepository.All
                     join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                     where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                     select new
                     {
                         csd.PaymentDate,
                         csd.HireValue,

                     }
           );


            var Income_SupplierProductReturn = (from R in POrderRepository.All
                                                where R.SupplierID != null && R.OrderDate > startDate && R.Status == 5
                                                select new
                                                {
                                                    GrandTotal = R.GrandTotal,
                                                    PaymentDue = R.GrandTotal - R.RecAmt,
                                                    ReturnDate = R.OrderDate,
                                                    PaidAmount = R.RecAmt

                                                }
                                     );

            var Income_BankWithdraw =
                                (from BT in BankTransactionRepository.All
                                 where BT.TransactionType == (int)EnumTransactionType.Withdraw && BT.TranDate >= startDate
                                 select new
                                 {
                                     BT.Amount,
                                     BT.TranDate

                                 }
               );

            var Income_SupplierProductReturn_With_Purchase_Details = (from R in POrderRepository.All
                                                                      join RTD in POrderDetailRepository.All on R.POrderID equals RTD.POrderID
                                                                      join STD in StockDetailRepository.All on RTD.POrderDetailID equals STD.POrderDetailID
                                                                      join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                      join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                      where R.SupplierID != null && R.OrderDate >= startDate
                                                                      select new
                                                                      {
                                                                          GrandTotal = R.GrandTotal,
                                                                          ReturnDate = R.OrderDate,
                                                                          Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                      }
                                  );



            decimal Pre_Income_Direct = Income_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_Sales_Rec = Income_Sales.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate < fromDate).Sum(o => o.RecAmount) : 0m;
            decimal Pre_Income_CashColeection = Income_CashCollection.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_DownPayment = Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Sum(o => o.DownPayment) : 0m;
            decimal Pre_Income_InstallmentCollection = Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() : 0m;
            decimal Pre_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.PaidAmount) : 0m;
            decimal Pre_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate < fromDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;


            decimal Pre_Expense_Direct = Expense_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_Purchase_Rec = Expense_Purchases.Where(o => o.OrderDate < fromDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate < fromDate).Sum(o => o.RecAmt) : 0m;
            decimal Pre_Expense_CashDelivery = Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.PaidAmount) : 0m;
            decimal Pre_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate < fromDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;


            decimal OpeningCashInHand = Pre_Income_Direct + Pre_Income_Sales_Rec + Pre_Income_CashColeection + Pre_Income_DownPayment + Pre_Income_InstallmentCollection + Pre_Income_ShareInvestment + Pre_Income_SupplierProductReturn + Pre_Income_BankWithdraw
                - Pre_Expense_Direct - Pre_Expense_Purchase_Rec - Pre_Expense_CashDelivery - Pre_Expense_ShareInvestment - Pre_Expense_CustomerProductReturn - Pre_Expense_BankDeposit;


            Opening_CashInhand = (double)OpeningCashInHand + (double)StartAmount;


            decimal Curr_Income_Direct = Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_Sales_Rec = Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.RecAmount) : 0m;
            decimal Curr_Income_CashColeection = Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_DownPayment = Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.DownPayment) : 0m;
            decimal Curr_Income_InstallmentCollection = Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() : 0m;
            decimal Curr_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.PaidAmount) : 0m;
            decimal Curr_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;


            decimal Curr_Expense_Direct = Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_Purchase_Rec = Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Sum(o => o.RecAmt) : 0m;
            decimal Curr_Expense_CashDelivery = Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.PaidAmount) : 0m;
            decimal Curr_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;


            decimal CurrentCashInHand = Curr_Income_Direct + Curr_Income_Sales_Rec + Curr_Income_CashColeection + Curr_Income_DownPayment + Curr_Income_InstallmentCollection + Curr_Income_ShareInvestment + Curr_Income_SupplierProductReturn + Curr_Income_BankWithdraw
    - Curr_Expense_Direct - Curr_Expense_Purchase_Rec - Curr_Expense_CashDelivery - Curr_Expense_ShareInvestment - Curr_Expense_CustomerProductReturn - Curr_Expense_BankDeposit;


            Current_CashInhand = (double)CurrentCashInHand;

            decimal ClosingCashInHand = (decimal)Opening_CashInhand + CurrentCashInHand;

            Closing_CashInhand = (double)ClosingCashInHand;









            var Direct_Expense_GroupBy = (from ex in Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                          group ex by ex.Description into g
                                          select new
                                          {

                                              Description = g.Key,
                                              Amount = g.Sum(o => o.Amount)
                                          }
                                      );

            //var Expense_ShareInvestment_GroupBy =
            //    (from ex in Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
            //     group ex by ex.Name into g
            //     select new
            //     {

            //         Description = g.Key,
            //         Amount = g.Sum(o => o.Amount)
            //     }
            //                          );




            List<CashInHandModel> Data = new List<CashInHandModel>();

            List<Costing> x = new List<Costing>();
            Costing obj = null;







            int id = 0;



            if (Curr_Expense_Purchase_Rec != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Cash Paid";
                obj.ExpenseAmt = Curr_Expense_Purchase_Rec;
                x.Add(obj);

            }


            if (Curr_Expense_CashDelivery != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Cash Delivery";
                obj.ExpenseAmt = Curr_Expense_CashDelivery;
                x.Add(obj);
            }




            if (Curr_Expense_CustomerProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Sales Return";
                obj.ExpenseAmt = Curr_Expense_CustomerProductReturn;
                x.Add(obj);

            }


            if (Curr_Expense_BankDeposit != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Bank Deposit";
                obj.ExpenseAmt = Curr_Expense_BankDeposit;
                x.Add(obj);

            }

            foreach (var item in Direct_Expense_GroupBy)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = item.Description;
                obj.ExpenseAmt = item.Amount;
                x.Add(obj);

            }

            //foreach (var item in Expense_ShareInvestment_GroupBy)
            //{
            //    id = id + 1;
            //    obj = new Costing();
            //    obj.id = id;
            //    obj.Expense = item.Description;
            //    obj.ExpenseAmt = item.Amount;
            //    x.Add(obj);

            //}






            List<Costing> y = new List<Costing>();
            id = 0;



            if (Curr_Income_Sales_Rec != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Cash Sales";
                obj.IncomeAmt = Curr_Income_Sales_Rec;
                y.Add(obj);
            }


            if (Curr_Income_CashColeection != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Cash Collection";
                obj.IncomeAmt = Curr_Income_CashColeection;
                y.Add(obj);
            }


            if (Curr_Income_SupplierProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Purchase Return";
                obj.IncomeAmt = Curr_Income_SupplierProductReturn;
                y.Add(obj);
            }



            if (Curr_Income_BankWithdraw != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Bank Withdraw";
                obj.IncomeAmt = Curr_Income_BankWithdraw;
                y.Add(obj);
            }



            if (Curr_Income_InstallmentCollection != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Installment Collection";
                obj.IncomeAmt = Curr_Income_InstallmentCollection;
                y.Add(obj);
            }


            if (Curr_Income_DownPayment != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Down Payment";
                obj.IncomeAmt = Curr_Income_DownPayment;
                y.Add(obj);
            }





            if (Curr_Income_SupplierProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Supplier Product Return";
                obj.IncomeAmt = Curr_Income_SupplierProductReturn;
                y.Add(obj);
            }


            var Direct_Income_GroupBy = (from ex in Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                         group ex by ex.Description into g
                                         select new
                                         {

                                             Description = g.Key,
                                             Amount = g.Sum(o => o.Amount)
                                         }
                                        );

            foreach (var item in Direct_Income_GroupBy)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = item.Description;
                obj.IncomeAmt = item.Amount;
                y.Add(obj);


            }


            //var Income_ShareInvestment_GroupBy =
            //   (from ex in Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
            //    group ex by ex.Name into g
            //    select new
            //    {

            //        Description = g.Key,
            //        Amount = g.Sum(o => o.Amount)
            //    }
            //                         );

            //Direct_Income_GroupBy.ToList().AddRange(Income_ShareInvestment_GroupBy);






            //foreach (var item in Income_ShareInvestment_GroupBy)
            //{
            //    id = id + 1;
            //    obj = new Costing();
            //    obj.id = id;
            //    obj.Income = item.Description;
            //    obj.IncomeAmt = item.Amount;
            //    y.Add(obj);


            //}


            CashInHandModel obdata = null;


            if (x.Count > y.Count)
            {
                foreach (var item in x)
                {
                    var op = y.Where(o => o.id == item.id).FirstOrDefault();
                    obdata = new CashInHandModel();
                    obdata.TransDate = fromDate;
                    obdata.id = item.id;
                    obdata.Expense = item.Expense;
                    obdata.ExpenseAmt = item.ExpenseAmt;
                    obdata.Income = op != null ? op.Income : "";
                    obdata.IncomeAmt = op != null ? op.IncomeAmt : 0m;
                    Data.Add(obdata);

                }

            }
            else
            {
                foreach (var item in y)
                {
                    var op = x.Where(o => o.id == item.id).FirstOrDefault();

                    obdata = new CashInHandModel();
                    obdata.TransDate = fromDate;
                    obdata.id = item.id;
                    obdata.Expense = op != null ? op.Expense : "";// item.Expense;
                    obdata.ExpenseAmt = op != null ? op.ExpenseAmt : 0m;// item.ExpenseAmt;
                    obdata.Income = item.Income;
                    obdata.IncomeAmt = item.IncomeAmt;
                    Data.Add(obdata);

                    // dt.Rows.Add(fromDate, item.id, op != null ? op.Expense : "", op != null ? op.ExpenseAmt : 0m, item.Income, item.IncomeAmt);
                }
            }



            TotalPayable = (double)x.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)y.Sum(o => o.IncomeAmt);

            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Total Payable";
            obdata.ExpenseAmt = (decimal)TotalPayable;
            Data.Add(obdata);


            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Total Recivable";
            obdata.ExpenseAmt = (decimal)TotalRecivable;
            Data.Add(obdata);


            obdata = new CashInHandModel();

            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Opening Cash In Hand";
            obdata.ExpenseAmt = (decimal)Opening_CashInhand;
            Data.Add(obdata);


            obdata = new CashInHandModel();
            obdata.id = 0;
            obdata.TransDate = fromDate;
            obdata.Expense = "Current Cash In Hand";
            obdata.ExpenseAmt = (decimal)Current_CashInhand;
            Data.Add(obdata);



            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Closing Cash In Hand";
            obdata.ExpenseAmt = (decimal)Closing_CashInhand;
            Data.Add(obdata);










            //var Income_ShareInvestment =
            //  (

            //  from SI in db.ShareInvestments
            //  join SH in db.ShareInvestmentHeads on SI.SIHID equals SH.SIHID
            //  where SI.TransactionType == 1 && (SH.ParentId == 2 || SH.ParentId == 3 || SH.ParentId == 4) && SI.EntryDate >= startDate
            //  select new
            //  {
            //      SH.Name,
            //      SI.Amount,
            //      SI.EntryDate

            //  }
            //   )
            //  ;





















            return Data;

            //return oAllCustomerCollData;
        }



        public static List<CashInHandModel> ProfitAndLossReport(
       this IBaseRepository<CashCollection> CashCollectionRepository,
           IBaseRepository<POrder> POrderRepository,
           IBaseRepository<POrderDetail> POrderDetailRepository,
           IBaseRepository<SOrder> SOrderRepository,
           IBaseRepository<SOrderDetail> SOrderDetailRepository,

           IBaseRepository<CreditSale> CreditSaleRepository,
           IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
           IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
           IBaseRepository<Stock> StockRepository,
           IBaseRepository<StockDetail> StockDetailRepository,

          IBaseRepository<ExpenseItem> ExpenseItemRepository,
          IBaseRepository<Expenditure> ExpenditureRepository,
          IBaseRepository<Bank> BankRepository,
          IBaseRepository<BankTransaction> BankTransactionRepository,
               IBaseRepository<ROrder> ROrderRepository,
          IBaseRepository<ROrderDetail> ROrderDetailRepository,
           IBaseRepository<Customer> CustomerRepository,
           IBaseRepository<SisterConcern> SisterConcernRepository,
           DateTime fromDate, DateTime toDate, int ConcernID
           )
        {

            string sFFdate = fromDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
            fromDate = Convert.ToDateTime(sFFdate);
            sFFdate = toDate.ToString("dd MMM yyyy") + " 11:59:59 AM";
            toDate = Convert.ToDateTime(sFFdate);

            IQueryable<Customer> Customers = null;
            if (ConcernID > 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var oAllCustomerCollData = from CC in CashCollectionRepository.GetAll()
                                       join CO in Customers on CC.CustomerID equals CO.CustomerID
                                       join sis in SisterConcernRepository.GetAll() on CC.ConcernID equals sis.ConcernID
                                       where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                       select new CashCollectionReportModel
                                       {
                                           EntryDate = CC.EntryDate,
                                           CustomerName = CO.Name,
                                           CustomerCode = CO.Code,
                                           Address = CO.Address,
                                           ContactNo = CO.ContactNo,
                                           TotalDue = CO.TotalDue,
                                           Amount = CC.Amount,
                                           AdjustAmt = CC.AdjustAmt,
                                           PaymentType = CC.PaymentType,
                                           AccountNo = CC.AccountNo,
                                           ReceiptNo = CC.ReceiptNo,
                                           Remarks = CC.Remarks,
                                           ConcernName = sis.Name,
                                           ModuleType = "Cash Collection"
                                       };

            DateTime startDate = new DateTime(2010, 4, 1, 16, 5, 7, 123);

            sFFdate = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
            startDate = Convert.ToDateTime(sFFdate);
            decimal StartAmount = 0m;

            double PCashInHandAmt = 0;
            double TotalIncomeAmt = 0;

            double CCashInHandAmt = 0;
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double Opening_CashInhand = 0;
            double Current_CashInhand = 0;
            double Closing_CashInhand = 0;


            var Expense_Purchases_Details = (
                                          from POD in POrderDetailRepository.All
                                          join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,
                                              PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              PaymentDue = (((PO.GrandTotal + PO.LaborCost - PO.AdjAmount - PO.NetDiscount - PO.RecAmt) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              RecAmt = PO.RecAmt
                                          }
                             );



            var Expense_Purchases = (
                                          from PO in POrderRepository.All
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,

                                              RecAmt = PO.RecAmt

                                          }
                             );

            var Expense_Purchases_For_Sales_Details = (from SOD in SOrderDetailRepository.All
                                                       join SO in SOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                                       join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                       where SO.Status == 1 && SO.InvoiceDate >= startDate
                                                       select new
                                                       {

                                                           SO.InvoiceDate,
                                                           PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * SOD.Quantity
                                                       }
                         );

            var Expense_Purchases_For_CreditSales_Details = (from CSP in CreditSaleDetailsRepository.All
                                                             join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                                             join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                                             join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                             join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                             where CS.SalesDate >= startDate
                                                             select new
                                                             {
                                                                 CS.SalesDate,
                                                                 PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity
                                                             }
                        );




            var Expense_LastPay =
                 (from CS in CreditSaleRepository.All
                  join CSD in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSD.CreditSalesID
                  where CSD.PaymentStatus == "Paid"
                  select new
                  {
                      LastPayAdjustment = 0m

                  }
                     );



            var Expense_CustomerProductReturn = (from R in ROrderRepository.All
                                                 where R.CustomerID != null
                                                 where R.ReturnDate >= startDate
                                                 select new
                                                 {
                                                     GrandTotal = R.GrandTotal,
                                                     ReturnDate = R.ReturnDate,
                                                     PaymentDue = R.GrandTotal - R.PaidAmount,
                                                     PaidAmount = R.PaidAmount
                                                 }
                                      );



            var Expense_CustomerProductReturn_with_Purchase_Details = (from R in ROrderRepository.All
                                                                       join RTD in ROrderDetailRepository.All on R.ROrderID equals RTD.ROrderID
                                                                       join STD in StockDetailRepository.All on RTD.StockDetailID equals STD.SDetailID
                                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                       where R.CustomerID != null && R.ReturnDate >= startDate
                                                                       select new
                                                                       {
                                                                           GrandTotal = R.GrandTotal,
                                                                           ReturnDate = R.ReturnDate,
                                                                           Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                       }
                                    );



            var Expense_Direct = (from ex in ExpenditureRepository.All
                                  join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                  where ((int)exi.Status == 1 && ex.EntryDate >= startDate)
                                  select new
                                  {
                                      ex.EntryDate,
                                      exi.Description,
                                      ex.Amount,

                                  }
                       ).ToList();



            var Expense_CashDelivery = (from csd in CashCollectionRepository.All
                                        where (csd.TransactionType == EnumTranType.ToCompany && csd.EntryDate >= startDate)
                                        select new
                                        {
                                            csd.EntryDate,
                                            csd.Amount,

                                        }
                     ).ToList();





            var Expense_CashCollection_Adjustment = (from CAS in CashCollectionRepository.All
                                                     where CAS.TransactionType == EnumTranType.FromCustomer && CAS.EntryDate >= startDate
                                                     select new
                                                     {
                                                         AdjustAmt = CAS.AdjustAmt,
                                                         CAS.EntryDate,

                                                     }
                 );


            var Expense_BankDeposit =
                                  (from BT in BankTransactionRepository.All
                                   where BT.TransactionType == (int)EnumTransactionType.Deposit && BT.TranDate >= startDate
                                   select new
                                   {
                                       BT.Amount,
                                       BT.TranDate

                                   }
                 );


            //var Expense_ShareInvestment =
            //    (

            //    from SI in db.ShareInvestments
            //    join SH in db.ShareInvestmentHeads on SI.SIHID equals SH.SIHID
            //    where SI.TransactionType == 2 && (SH.ParentId == 2 || SH.ParentId == 3 || SH.ParentId == 4) && SI.EntryDate >= startDate
            //    select new
            //    {
            //        SH.Name,
            //        SI.Amount,
            //        SI.EntryDate

            //    }
            //     )
            ;


            //Income--------------------------------------------------------------------------------------------------




            var Income_Sales = (from SO in SOrderRepository.All
                                where SO.Status == 1 && SO.InvoiceDate >= startDate
                                select new
                                {
                                    SO.InvoiceDate,
                                    RecAmount = SO.RecAmount.Value,
                                    PaymentDue = SO.PaymentDue,
                                    SalesAmt = (SO.GrandTotal - SO.NetDiscount - SO.AdjAmount)

                                }
                                    );



            var Income_Sales_Details = (from SOD in SOrderDetailRepository.All
                                        join SO in SOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                        join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                        join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                        join PO in POrderRepository.All on POD.POrderID equals PO.POrderID

                                        where SO.Status == 1 && SO.InvoiceDate >= startDate
                                        select new
                                        {

                                            SO.InvoiceDate,
                                            SalesTotal = (((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,

                                            Discount = (SOD.PPDAmount / SOD.Quantity + (((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,


                                            PurchaseTotal = ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity,

                                            CommisionProfit = ((((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity),
                                            HireProfit = 0,
                                            HireCollection = 0,
                                            TotalProfit = ((((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity)


                                        }
                                    );



            var Income_Credit_Sales_Details = (from CSP in CreditSaleDetailsRepository.All
                                               join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                               join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                               join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                               join PO in POrderRepository.All on POD.POrderID equals PO.POrderID

                                               where CS.SalesDate >= startDate
                                               select new
                                               {

                                                   CS.SalesDate,
                                                   SalesTotal = (CSP.UnitPrice * CSP.Quantity),

                                                   Discount = 0m,

                                                   PurchaseTotal = (POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity,

                                                   CommisionProfit = ((CSP.UnitPrice * CSP.Quantity)) - ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity),
                                                   HireProfit = (((((0) / (CS.TSalesAmt - 0)) * (CSP.UnitPrice))) * CSP.Quantity) - (((((CS.Discount) / (CS.TSalesAmt - 0.00m)) * (CSP.UnitPrice))) * CSP.Quantity),
                                                   HireCollection = 0,
                                                   TotalProfit = (CSP.UTAmount - CSP.MPRateTotal - 0)


                                               }
                                   );


            var Income_Credit_Sales_Details_Hire_Collection = (from CSP in CreditSaleDetailsRepository.All
                                                               join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                                               join CSS in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSS.CreditSalesID
                                                               join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                                               join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                               join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                               where CS.SalesDate >= startDate && CSS.PaymentStatus == "Paid"
                                                               select new
                                                               {
                                                                   CSS.PaymentDate,
                                                                   CS.SalesDate,
                                                                   SalesTotal = 0m,
                                                                   Discount = 0m,
                                                                   PurchaseTotal = 0m,
                                                                   CommisionProfit = ((CSP.UnitPrice * CSP.Quantity)) - ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity),
                                                                   HireProfit = 0m,
                                                                   HireCollection = CSS.HireValue,
                                                                   TotalProfit = CSS.HireValue
                                                               }
                                 );



            var Income_Credit_Sales = (from CS in CreditSaleRepository.All
                                       where CS.SalesDate >= startDate
                                       select new
                                       {
                                           CS.SalesDate,
                                           CS.DownPayment,
                                           SalesAmt = (CS.TSalesAmt - CS.Discount - CS.WInterestAmt)

                                       }
                     );


            var Income_Direct = (from ex in ExpenditureRepository.All
                                 join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                 where ((int)exi.Status == 2 && ex.EntryDate >= startDate)
                                 select new
                                 {
                                     exi.Description,
                                     ex.EntryDate,
                                     ex.Amount,

                                 }
                       ).ToList();

            var Income_CashCollection = (from csd in CashCollectionRepository.All
                                         where (csd.TransactionType == EnumTranType.FromCustomer && csd.EntryDate >= startDate)
                                         select new
                                         {
                                             csd.EntryDate,
                                             csd.Amount,


                                         }
                      ).ToList();

            var Income_CashDelivery_Adjustment = (from CAS in CashCollectionRepository.All
                                                  where CAS.TransactionType == EnumTranType.ToCompany
                                                  where CAS.EntryDate >= startDate
                                                  select new
                                                  {
                                                      AdjustAmt = CAS.AdjustAmt,
                                                      CAS.EntryDate
                                                  }
               );


            var Income_InstallmentCollection = (
                                  from csd in CreditSalesScheduleRepository.All
                                  join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                                  where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                                  select new
                                  {
                                      csd.PaymentDate,
                                      csd.InstallmentAmt,

                                  }
                        );

            var Income_HireCollection = (
                     from csd in CreditSalesScheduleRepository.All
                     join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                     where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                     select new
                     {
                         csd.PaymentDate,
                         csd.HireValue,

                     }
           );


            var Income_SupplierProductReturn = (from R in POrderRepository.All
                                                where R.SupplierID != null && R.OrderDate > startDate && R.Status == 5
                                                select new
                                                {
                                                    GrandTotal = R.GrandTotal,
                                                    PaymentDue = R.GrandTotal - R.RecAmt,
                                                    ReturnDate = R.OrderDate,
                                                    PaidAmount = R.RecAmt

                                                }
                                     );

            var Income_BankWithdraw =
                                (from BT in BankTransactionRepository.All
                                 where BT.TransactionType == (int)EnumTransactionType.Withdraw && BT.TranDate >= startDate
                                 select new
                                 {
                                     BT.Amount,
                                     BT.TranDate

                                 }
               );

            var Income_SupplierProductReturn_With_Purchase_Details = (from R in POrderRepository.All
                                                                      join RTD in POrderDetailRepository.All on R.POrderID equals RTD.POrderID
                                                                      join STD in StockDetailRepository.All on RTD.POrderDetailID equals STD.POrderDetailID
                                                                      join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                      join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                      where R.SupplierID != null && R.OrderDate >= startDate
                                                                      select new
                                                                      {
                                                                          GrandTotal = R.GrandTotal,
                                                                          ReturnDate = R.OrderDate,
                                                                          Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                      }
                                  );



            decimal Pre_Income_Direct = Income_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_Sales = Income_Sales.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate < fromDate).Sum(o => o.RecAmount) : 0m;



            decimal PreSales = Income_Sales.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate < fromDate).Sum(o => o.SalesAmt) : 0m;
            decimal Pre_Credit_Sales = Income_Credit_Sales_Details.Where(o => o.SalesDate < fromDate).Count() != 0 ? Income_Credit_Sales_Details.Where(o => o.SalesDate < fromDate).Sum(o => o.SalesTotal) : 0m;

            Pre_Income_Sales = PreSales + Pre_Credit_Sales;


            decimal Pre_Income_CashColeection = 0;// Income_CashCollection.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_DownPayment = 0;// Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Sum(o => o.DownPayment) : 0m;
            decimal Pre_Income_InstallmentCollection = 0;// Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() : 0m;
            decimal Pre_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.GrandTotal) : 0m;
            decimal Pre_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate < fromDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_CashDelivery_Adjustment = Income_CashDelivery_Adjustment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_CashDelivery_Adjustment.Where(o => o.EntryDate < fromDate).Sum(o => o.AdjustAmt) : 0m;

            decimal Pre_Expense_Direct = Expense_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;

            decimal Pre_Sales_Purchase = Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate < fromDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal Pre_CreditSales_Purchase = Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate < fromDate).Count() != 0 ? Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate < fromDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal Pre_Expense_Purchase_Rec = Pre_Sales_Purchase + Pre_CreditSales_Purchase;


            decimal Pre_Expense_CashDelivery = 0;// Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.GrandTotal) : 0m;
            decimal Pre_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate < fromDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_CashCollection_Adjustment = Expense_CashCollection_Adjustment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_CashCollection_Adjustment.Where(o => o.EntryDate < fromDate).Sum(o => o.AdjustAmt) : 0m;



            decimal OpeningCashInHand = Pre_Income_Direct + Pre_Income_Sales + Pre_Income_CashColeection + Pre_Income_DownPayment + Pre_Income_InstallmentCollection + Pre_Income_ShareInvestment + Pre_Income_SupplierProductReturn + Pre_Income_BankWithdraw + Pre_Income_CashDelivery_Adjustment
                - Pre_Expense_Direct - Pre_Expense_Purchase_Rec - Pre_Expense_CashDelivery - Pre_Expense_ShareInvestment - Pre_Expense_CustomerProductReturn - Pre_Expense_BankDeposit - Pre_Expense_CashCollection_Adjustment;





            Opening_CashInhand = (double)OpeningCashInHand + (double)StartAmount;


            decimal Curr_Income_Direct = Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_Sales = 0m;


            decimal Sales = Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.SalesAmt) : 0m;
            decimal Credit_Sales = Income_Credit_Sales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Income_Credit_Sales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.SalesTotal) : 0m;
            //  decimal Discount_Sales = Income_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Income_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.Discount) : 0m;
            Curr_Income_Sales = Sales + Credit_Sales;


            decimal Credit_Collection = Income_Credit_Sales_Details_Hire_Collection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() != 0 ? Income_Credit_Sales_Details_Hire_Collection.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.SalesTotal) : 0m;


            decimal Curr_Income_CashColeection = 0;// Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_DownPayment = Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.DownPayment) : 0m;
            decimal Curr_Income_InstallmentCollection = Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() : 0m;
            decimal Curr_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.GrandTotal) : 0m;
            decimal Curr_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_CashDelivery_Adjustment = Income_CashDelivery_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_CashDelivery_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.AdjustAmt) : 0m;




            decimal Curr_Expense_Direct = Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_Purchase = 0m;
            decimal Sales_Purchase = Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal CreditSales_Purchase = Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.PurchaseAmt) : 0m;
            //Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate>= fromDate && o.SalesDate<= toDate).Sum(o => o.PurchaseAmt) : 0m;
            Curr_Expense_Purchase = Sales_Purchase + CreditSales_Purchase;




            decimal Curr_Expense_CashDelivery = 0;// Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.GrandTotal) : 0m;
            decimal Curr_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_CashCollection_Adjustment = Expense_CashCollection_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_CashCollection_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.AdjustAmt) : 0m;


            decimal CurrentCashInHand = Curr_Income_Direct + Curr_Income_Sales + Curr_Income_CashColeection + Curr_Income_DownPayment + Curr_Income_InstallmentCollection + Curr_Income_ShareInvestment + Curr_Income_SupplierProductReturn + Curr_Income_BankWithdraw + Curr_Income_CashDelivery_Adjustment
    - Curr_Expense_Direct - Curr_Expense_Purchase - Curr_Expense_CashDelivery - Curr_Expense_ShareInvestment - Curr_Expense_CustomerProductReturn - Curr_Expense_BankDeposit - Curr_Expense_CashCollection_Adjustment;


            Current_CashInhand = (double)CurrentCashInHand;

            decimal ClosingCashInHand = (decimal)Opening_CashInhand + CurrentCashInHand;

            Closing_CashInhand = (double)ClosingCashInHand;









            var Direct_Expense_GroupBy = (from ex in Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                          group ex by ex.Description into g
                                          select new
                                          {

                                              Description = g.Key,
                                              Amount = g.Sum(o => o.Amount)
                                          }
                                      );

            //var Expense_ShareInvestment_GroupBy =
            //    (from ex in Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
            //     group ex by ex.Name into g
            //     select new
            //     {

            //         Description = g.Key,
            //         Amount = g.Sum(o => o.Amount)
            //     }
            //                          );




            List<CashInHandModel> Data = new List<CashInHandModel>();

            List<Costing> x = new List<Costing>();
            Costing obj = null;







            int id = 0;



            if (Curr_Expense_Purchase != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Purchase Amount";
                obj.ExpenseAmt = Curr_Expense_Purchase;
                x.Add(obj);

            }



            if (Curr_Expense_CashCollection_Adjustment != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Adjust in Cash Collection";
                obj.ExpenseAmt = Curr_Expense_CashCollection_Adjustment;
                x.Add(obj);
            }






            if (Curr_Expense_CustomerProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Sales Return";
                obj.ExpenseAmt = Curr_Expense_CustomerProductReturn;
                x.Add(obj);

            }


            if (Curr_Expense_BankDeposit != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = "Bank Deposit";
                obj.ExpenseAmt = Curr_Expense_BankDeposit;
                x.Add(obj);

            }

            foreach (var item in Direct_Expense_GroupBy)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Expense = item.Description;
                obj.ExpenseAmt = item.Amount;
                x.Add(obj);

            }

            //foreach (var item in Expense_ShareInvestment_GroupBy)
            //{
            //    id = id + 1;
            //    obj = new Costing();
            //    obj.id = id;
            //    obj.Expense = item.Description;
            //    obj.ExpenseAmt = item.Amount;
            //    x.Add(obj);

            //}






            List<Costing> y = new List<Costing>();
            id = 0;



            if (Curr_Income_Sales != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Sales Amount";
                obj.IncomeAmt = Curr_Income_Sales;
                y.Add(obj);
            }


            if (Curr_Income_CashColeection != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Cash Collection";
                obj.IncomeAmt = Curr_Income_CashColeection;
                y.Add(obj);
            }


            if (Curr_Income_SupplierProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Purchase Return";
                obj.IncomeAmt = Curr_Income_SupplierProductReturn;
                y.Add(obj);
            }



            if (Curr_Income_BankWithdraw != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Bank Withdraw";
                obj.IncomeAmt = Curr_Income_BankWithdraw;
                y.Add(obj);
            }



            if (Curr_Income_InstallmentCollection != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Installment Collection";
                obj.IncomeAmt = Curr_Income_InstallmentCollection;
                y.Add(obj);
            }


            if (Curr_Income_DownPayment != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Down Payment";
                obj.IncomeAmt = Curr_Income_DownPayment;
                y.Add(obj);
            }





            if (Curr_Income_SupplierProductReturn != 0)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = "Purchase Return";
                obj.IncomeAmt = Curr_Income_SupplierProductReturn;
                y.Add(obj);
            }


            var Direct_Income_GroupBy = (from ex in Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                         group ex by ex.Description into g
                                         select new
                                         {

                                             Description = g.Key,
                                             Amount = g.Sum(o => o.Amount)
                                         }
                                        );

            foreach (var item in Direct_Income_GroupBy)
            {
                id = id + 1;
                obj = new Costing();
                obj.id = id;
                obj.Income = item.Description;
                obj.IncomeAmt = item.Amount;
                y.Add(obj);


            }


            //var Income_ShareInvestment_GroupBy =
            //   (from ex in Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
            //    group ex by ex.Name into g
            //    select new
            //    {

            //        Description = g.Key,
            //        Amount = g.Sum(o => o.Amount)
            //    }
            //                         );

            //Direct_Income_GroupBy.ToList().AddRange(Income_ShareInvestment_GroupBy);






            //foreach (var item in Income_ShareInvestment_GroupBy)
            //{
            //    id = id + 1;
            //    obj = new Costing();
            //    obj.id = id;
            //    obj.Income = item.Description;
            //    obj.IncomeAmt = item.Amount;
            //    y.Add(obj);


            //}


            CashInHandModel obdata = null;


            if (x.Count > y.Count)
            {
                foreach (var item in x)
                {
                    var op = y.Where(o => o.id == item.id).FirstOrDefault();
                    obdata = new CashInHandModel();
                    obdata.TransDate = fromDate;
                    obdata.id = item.id;
                    obdata.Expense = item.Expense;
                    obdata.ExpenseAmt = item.ExpenseAmt;
                    obdata.Income = op != null ? op.Income : "";
                    obdata.IncomeAmt = op != null ? op.IncomeAmt : 0m;
                    Data.Add(obdata);

                }

            }
            else
            {
                foreach (var item in y)
                {
                    var op = x.Where(o => o.id == item.id).FirstOrDefault();

                    obdata = new CashInHandModel();
                    obdata.TransDate = fromDate;
                    obdata.id = item.id;
                    obdata.Expense = op != null ? op.Expense : "";// item.Expense;
                    obdata.ExpenseAmt = op != null ? op.ExpenseAmt : 0m;// item.ExpenseAmt;
                    obdata.Income = item.Income;
                    obdata.IncomeAmt = item.IncomeAmt;
                    Data.Add(obdata);

                    // dt.Rows.Add(fromDate, item.id, op != null ? op.Expense : "", op != null ? op.ExpenseAmt : 0m, item.Income, item.IncomeAmt);
                }
            }



            TotalPayable = (double)x.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)y.Sum(o => o.IncomeAmt);

            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Total Payable";
            obdata.ExpenseAmt = (decimal)TotalPayable;
            Data.Add(obdata);


            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Total Recivable";
            obdata.ExpenseAmt = (decimal)TotalRecivable;
            Data.Add(obdata);


            obdata = new CashInHandModel();

            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Opening Cash In Hand";
            obdata.ExpenseAmt = (decimal)Opening_CashInhand;
            Data.Add(obdata);


            obdata = new CashInHandModel();
            obdata.id = 0;
            obdata.TransDate = fromDate;
            obdata.Expense = "Current Cash In Hand";
            obdata.ExpenseAmt = (decimal)Current_CashInhand;
            Data.Add(obdata);



            obdata = new CashInHandModel();
            obdata.TransDate = fromDate;
            obdata.id = 0;
            obdata.Expense = "Closing Cash In Hand";
            obdata.ExpenseAmt = (decimal)Closing_CashInhand;
            Data.Add(obdata);










            //var Income_ShareInvestment =
            //  (

            //  from SI in db.ShareInvestments
            //  join SH in db.ShareInvestmentHeads on SI.SIHID equals SH.SIHID
            //  where SI.TransactionType == 1 && (SH.ParentId == 2 || SH.ParentId == 3 || SH.ParentId == 4) && SI.EntryDate >= startDate
            //  select new
            //  {
            //      SH.Name,
            //      SI.Amount,
            //      SI.EntryDate

            //  }
            //   )
            //  ;








            return Data;

        }


        public static List<SummaryReportModel> SummaryReport(



     this IBaseRepository<CashCollection> CashCollectionRepository,
         IBaseRepository<POrder> POrderRepository,
         IBaseRepository<POrderDetail> POrderDetailRepository,
         IBaseRepository<SOrder> SOrderRepository,
         IBaseRepository<SOrderDetail> SOrderDetailRepository,

         IBaseRepository<CreditSale> CreditSaleRepository,
         IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
         IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
         IBaseRepository<Stock> StockRepository,
         IBaseRepository<StockDetail> StockDetailRepository,

        IBaseRepository<ExpenseItem> ExpenseItemRepository,
        IBaseRepository<Expenditure> ExpenditureRepository,
        IBaseRepository<Bank> BankRepository,
        IBaseRepository<BankTransaction> BankTransactionRepository,

         IBaseRepository<Customer> CustomerRepository,
         IBaseRepository<SisterConcern> SisterConcernRepository,
         DateTime fromDate, DateTime toDate,
         decimal OpeningCashInHand, decimal CurrentCashInHand, decimal ClosingCashInHand,

            int ConcernID
         )
        {



            string sFFdate = fromDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
            fromDate = Convert.ToDateTime(sFFdate);

            sFFdate = toDate.ToString("dd MMM yyyy") + " 11:59:59 AM";
            toDate = Convert.ToDateTime(sFFdate);


            DateTime startDate = new DateTime(2010, 4, 1, 16, 5, 7, 123);

            sFFdate = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
            startDate = Convert.ToDateTime(sFFdate);


            double Opening_CashInhand = 0;
            double Current_CashInhand = 0;
            double Closing_CashInhand = 0;


            var Expense_Purchases_Details = (
                                          from POD in POrderDetailRepository.All
                                          join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,
                                              PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              PaymentDue = (((PO.GrandTotal + PO.LaborCost - PO.AdjAmount - PO.NetDiscount - PO.RecAmt) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * POD.Quantity,
                                              RecAmt = PO.RecAmt
                                          }
                             );



            var Expense_Purchases = (
                                          from PO in POrderRepository.All
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {

                                              PO.OrderDate,
                                              Purchase = PO.GrandTotal - PO.NetDiscount,
                                              RecAmt = PO.RecAmt,
                                              PaymentDue = PO.GrandTotal - PO.NetDiscount,

                                          }
                             );

            var Expense_Purchases_For_Sales_Details = (from SOD in SOrderDetailRepository.All
                                                       join SO in SOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                                       join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                       where SO.Status == 1 && SO.InvoiceDate >= startDate
                                                       select new
                                                       {

                                                           SO.InvoiceDate,
                                                           PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * SOD.Quantity
                                                       }
                         );

            var Expense_Purchases_For_CreditSales_Details = (from CSP in CreditSaleDetailsRepository.All
                                                             join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                                             join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                                             join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                             join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                             where CS.SalesDate >= startDate
                                                             select new
                                                             {
                                                                 CS.SalesDate,
                                                                 PurchaseAmt = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity
                                                             }
                        );




            var Expense_LastPay =
                 (from CS in CreditSaleRepository.All
                  join CSD in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSD.CreditSalesID
                  where CSD.PaymentStatus == "Paid"
                  select new
                  {
                      LastPayAdjustment = 0m

                  }
                     );



            var Expense_CustomerProductReturn = (from R in SOrderRepository.All
                                                 where R.CustomerID != null
                                                 where R.InvoiceDate >= startDate && R.Status == 4
                                                 select new
                                                 {
                                                     GrandTotal = R.GrandTotal,
                                                     ReturnDate = R.InvoiceDate,
                                                     PaymentDue = R.GrandTotal - R.RecAmount,
                                                     PaidAmount = R.RecAmount.Value
                                                 }
                                      );



            var Expense_CustomerProductReturn_with_Purchase_Details = (from R in SOrderRepository.All
                                                                       join RTD in SOrderDetailRepository.All on R.SOrderID equals RTD.SOrderID
                                                                       join STD in StockDetailRepository.All on RTD.SDetailID equals STD.SDetailID
                                                                       join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                       join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                       where R.CustomerID != null && R.InvoiceDate >= startDate && R.Status == 4
                                                                       select new
                                                                       {
                                                                           GrandTotal = R.GrandTotal,
                                                                           ReturnDate = R.InvoiceDate,
                                                                           Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                       }
                                    );




            var Expense_Direct = (from ex in ExpenditureRepository.All
                                  join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                  where ((int)exi.Status == 1 && ex.EntryDate >= startDate)
                                  select new
                                  {
                                      ex.EntryDate,
                                      exi.Description,
                                      ex.Amount,

                                  }
                       ).ToList();



            var Expense_CashDelivery = (from csd in CashCollectionRepository.All
                                        where (csd.TransactionType == EnumTranType.ToCompany && csd.EntryDate >= startDate)
                                        select new
                                        {
                                            csd.EntryDate,
                                            csd.Amount,

                                        }
                     ).ToList();





            var Expense_CashCollection_Adjustment = (from CAS in CashCollectionRepository.All
                                                     where CAS.TransactionType == EnumTranType.FromCustomer && CAS.EntryDate >= startDate
                                                     select new
                                                     {
                                                         AdjustAmt = CAS.AdjustAmt,
                                                         CAS.EntryDate,

                                                     }
                 );


            var Expense_BankDeposit =
                                  (from BT in BankTransactionRepository.All
                                   where BT.TransactionType == (int)EnumTransactionType.Deposit && BT.TranDate >= startDate
                                   select new
                                   {
                                       BT.Amount,
                                       BT.TranDate

                                   }
                 );



            var Income_Sales = (from SO in SOrderRepository.All
                                where SO.Status == 1 && SO.InvoiceDate >= startDate
                                select new
                                {
                                    SO.InvoiceDate,
                                    RecAmount = SO.RecAmount.Value,
                                    PaymentDue = SO.PaymentDue,
                                    SalesAmt = (SO.GrandTotal - SO.NetDiscount - SO.AdjAmount)

                                }
                                    );



            var Income_Sales_Details = (from SOD in SOrderDetailRepository.All
                                        join SO in SOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                        join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                        join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                        join PO in POrderRepository.All on POD.POrderID equals PO.POrderID

                                        where SO.Status == 1 && SO.InvoiceDate >= startDate
                                        select new
                                        {

                                            SO.InvoiceDate,
                                            SalesTotal = (((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,

                                            Discount = (SOD.PPDAmount / SOD.Quantity + (((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity,


                                            PurchaseTotal = ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity,

                                            CommisionProfit = ((((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity),
                                            HireProfit = 0,
                                            HireCollection = 0,
                                            TotalProfit = ((((SOD.UnitPrice - SOD.PPDAmount / SOD.Quantity) - ((SO.TDAmount + SO.AdjAmount) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)) * (SOD.UnitPrice - SOD.PPDAmount))) * SOD.Quantity) - (((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice)) * SOD.Quantity)


                                        }
                                    );



            var Income_Credit_Sales_Details = (from CSP in CreditSaleDetailsRepository.All
                                               join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                               join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                               join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                               join PO in POrderRepository.All on POD.POrderID equals PO.POrderID

                                               where CS.SalesDate >= startDate
                                               select new
                                               {

                                                   CS.SalesDate,
                                                   SalesTotal = (CSP.UnitPrice * CSP.Quantity),

                                                   Discount = 0m,

                                                   PurchaseTotal = (POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity,

                                                   CommisionProfit = ((CSP.UnitPrice * CSP.Quantity)) - ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity),
                                                   HireProfit = (((((0) / (CS.TSalesAmt - 0)) * (CSP.UnitPrice))) * CSP.Quantity) - (((((CS.Discount) / (CS.TSalesAmt - 0.00m)) * (CSP.UnitPrice))) * CSP.Quantity),
                                                   HireCollection = 0,
                                                   TotalProfit = (CSP.UTAmount - CSP.MPRateTotal - 0)


                                               }
                                   );


            var Income_Credit_Sales_Details_Hire_Collection = (from CSP in CreditSaleDetailsRepository.All
                                                               join CS in CreditSaleRepository.All on CSP.CreditSalesID equals CS.CreditSalesID
                                                               join CSS in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSS.CreditSalesID
                                                               join STD in StockDetailRepository.All on CSP.StockDetailID equals STD.SDetailID
                                                               join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                               join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                               where CS.SalesDate >= startDate && CSS.PaymentStatus == "Paid"
                                                               select new
                                                               {
                                                                   CSS.PaymentDate,
                                                                   CS.SalesDate,
                                                                   SalesTotal = 0m,
                                                                   Discount = 0m,
                                                                   PurchaseTotal = 0m,
                                                                   CommisionProfit = ((CSP.UnitPrice * CSP.Quantity)) - ((POD.UnitPrice - ((PO.TDiscount - PO.LaborCost) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * CSP.Quantity),
                                                                   HireProfit = 0m,
                                                                   HireCollection = CSS.HireValue,
                                                                   TotalProfit = CSS.HireValue
                                                               }
                                 );



            var Income_Credit_Sales = (from CS in CreditSaleRepository.All
                                       where CS.SalesDate >= startDate
                                       select new
                                       {
                                           CS.SalesDate,
                                           CS.DownPayment,
                                           SalesAmt = (CS.TSalesAmt - CS.Discount - CS.WInterestAmt)

                                       }
                     );


            var Income_Direct = (from ex in ExpenditureRepository.All
                                 join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                 where ((int)exi.Status == 2 && ex.EntryDate >= startDate)
                                 select new
                                 {
                                     exi.Description,
                                     ex.EntryDate,
                                     ex.Amount,

                                 }
                       ).ToList();

            var Income_CashCollection = (from csd in CashCollectionRepository.All
                                         where (csd.TransactionType == EnumTranType.FromCustomer && csd.EntryDate >= startDate)
                                         select new
                                         {
                                             csd.EntryDate,
                                             csd.Amount,


                                         }
                      ).ToList();

            var Income_CashDelivery_Adjustment = (from CAS in CashCollectionRepository.All
                                                  where CAS.TransactionType == EnumTranType.ToCompany
                                                  where CAS.EntryDate >= startDate
                                                  select new
                                                  {
                                                      AdjustAmt = CAS.AdjustAmt,
                                                      CAS.EntryDate
                                                  }
               );


            var Income_InstallmentCollection = (
                                  from csd in CreditSalesScheduleRepository.All
                                  join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                                  where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                                  select new
                                  {
                                      csd.PaymentDate,
                                      csd.InstallmentAmt,

                                  }
                        );

            var Income_HireCollection = (
                     from csd in CreditSalesScheduleRepository.All
                     join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                     where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                     select new
                     {
                         csd.PaymentDate,
                         csd.HireValue,

                     }
           );


            var Income_SupplierProductReturn = (from R in POrderRepository.All
                                                where R.SupplierID != null && R.OrderDate > startDate && R.Status == 5
                                                select new
                                                {
                                                    GrandTotal = R.GrandTotal,
                                                    PaymentDue = R.GrandTotal - R.RecAmt,
                                                    ReturnDate = R.OrderDate,
                                                    PaidAmount = R.RecAmt

                                                }
                                     );

            var Income_BankWithdraw =
                                (from BT in BankTransactionRepository.All
                                 where BT.TransactionType == (int)EnumTransactionType.Withdraw && BT.TranDate >= startDate
                                 select new
                                 {
                                     BT.Amount,
                                     BT.TranDate

                                 }
               );

            var Income_SupplierProductReturn_With_Purchase_Details = (from R in POrderRepository.All
                                                                      join RTD in POrderDetailRepository.All on R.POrderID equals RTD.POrderID
                                                                      join STD in StockDetailRepository.All on RTD.POrderDetailID equals STD.POrderDetailID
                                                                      join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                      join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                      where R.SupplierID != null && R.OrderDate >= startDate
                                                                      select new
                                                                      {
                                                                          GrandTotal = R.GrandTotal,
                                                                          ReturnDate = R.OrderDate,
                                                                          Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                      }
                                  );



            decimal Pre_Income_Direct = Income_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;


            decimal Pre_Sales = Income_Sales.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate < fromDate).Sum(o => o.SalesAmt) : 0m;
            decimal Pre_Credit_Sales = Income_Credit_Sales_Details.Where(o => o.SalesDate < fromDate).Count() != 0 ? Income_Credit_Sales_Details.Where(o => o.SalesDate < fromDate).Sum(o => o.SalesTotal) : 0m;

            decimal Pre_Income_Sales = Pre_Sales + Pre_Credit_Sales;


            decimal Pre_Income_CashColeection = 0;// Income_CashCollection.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_DownPayment = 0;// Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate < fromDate).Sum(o => o.DownPayment) : 0m;
            decimal Pre_Income_InstallmentCollection = 0;// Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate < fromDate).Count() : 0m;
            decimal Pre_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.GrandTotal) : 0m;
            decimal Pre_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate < fromDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Income_CashDelivery_Adjustment = Income_CashDelivery_Adjustment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Income_CashDelivery_Adjustment.Where(o => o.EntryDate < fromDate).Sum(o => o.AdjustAmt) : 0m;

            decimal Pre_Expense_Direct = Expense_Direct.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;

            decimal Pre_Expense_Purchase = Expense_Purchases.Where(o => o.OrderDate < fromDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate < fromDate).Sum(o => o.Purchase) : 0m;

            decimal Pre_Sales_Purchase = Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate < fromDate).Count() != 0 ? Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate < fromDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal Pre_CreditSales_Purchase = Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate < fromDate).Count() != 0 ? Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate < fromDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal Pre_Expense_SalesAndCreditSales_Purchase = Pre_Sales_Purchase + Pre_CreditSales_Purchase;

            decimal Pre_Expense_CashDelivery = 0;// Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate < fromDate).Sum(o => o.GrandTotal) : 0m;
            decimal Pre_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate < fromDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate < fromDate).Sum(o => o.Amount) : 0m;
            decimal Pre_Expense_CashCollection_Adjustment = Expense_CashCollection_Adjustment.Where(o => o.EntryDate < fromDate).Count() != 0 ? Expense_CashCollection_Adjustment.Where(o => o.EntryDate < fromDate).Sum(o => o.AdjustAmt) : 0m;


            decimal Curr_Income_Direct = Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_Sales = 0m;


            decimal Sales = Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.SalesAmt) : 0m;
            decimal Credit_Sales = Income_Credit_Sales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Income_Credit_Sales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.SalesTotal) : 0m;
            //  decimal Discount_Sales = Income_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Income_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.Discount) : 0m;
            Curr_Income_Sales = Sales + Credit_Sales;


            decimal Credit_Collection = Income_Credit_Sales_Details_Hire_Collection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() != 0 ? Income_Credit_Sales_Details_Hire_Collection.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.SalesTotal) : 0m;


            decimal Curr_Income_CashColeection = 0;// Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_DownPayment = Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.DownPayment) : 0m;
            decimal Curr_Income_InstallmentCollection = Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate >= fromDate && o.PaymentDate <= toDate).Count() : 0m;
            decimal Curr_Income_ShareInvestment = 0;// Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_SupplierProductReturn = Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Income_SupplierProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.GrandTotal) : 0m;
            decimal Curr_Income_BankWithdraw = Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Income_BankWithdraw.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Income_CashDelivery_Adjustment = Income_CashDelivery_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Income_CashDelivery_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.AdjustAmt) : 0m;




            decimal Curr_Expense_Direct = Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;


            decimal Curr_Expense_Purchase = Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Sum(o => o.Purchase) : 0m;

            decimal Sales_Purchase = Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Count() != 0 ? Expense_Purchases_For_Sales_Details.Where(o => o.InvoiceDate >= fromDate && o.InvoiceDate <= toDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal CreditSales_Purchase = Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Count() != 0 ? Expense_Purchases_For_CreditSales_Details.Where(o => o.SalesDate >= fromDate && o.SalesDate <= toDate).Sum(o => o.PurchaseAmt) : 0m;
            decimal Curr_Expense_SalesAndCreditSales_Purchase = Sales_Purchase + CreditSales_Purchase;


            decimal Curr_Expense_CashPaid = Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).Sum(o => o.RecAmt) : 0m;

            decimal Curr_Expense_CashDelivery = 0;// Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_ShareInvestment = 0;// Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_ShareInvestment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_CustomerProductReturn = Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Count() != 0 ? Expense_CustomerProductReturn.Where(o => o.ReturnDate >= fromDate && o.ReturnDate <= toDate).Sum(o => o.GrandTotal) : 0m;
            decimal Curr_Expense_BankDeposit = Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Count() != 0 ? Expense_BankDeposit.Where(o => o.TranDate >= fromDate && o.TranDate <= toDate).Sum(o => o.Amount) : 0m;
            decimal Curr_Expense_CashCollection_Adjustment = Expense_CashCollection_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Count() != 0 ? Expense_CashCollection_Adjustment.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate).Sum(o => o.AdjustAmt) : 0m;


            var Direct_Expense_GroupBy = (from ex in Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                          group ex by ex.Description into g
                                          select new
                                          {

                                              Description = g.Key,
                                              Amount = g.Sum(o => o.Amount)
                                          }
                                      );


            List<CashInHandModel> Data = new List<CashInHandModel>();

            List<Costing> x = new List<Costing>();
            Costing obj = null;



            List<SummaryReportModel> SummaryData = new List<SummaryReportModel>();
            SummaryReportModel ob = null;



            int k = 0;

            decimal CustomerDue = Pre_Income_Sales - Pre_Income_CashColeection - Pre_Income_DownPayment;
            decimal Stock = Pre_Expense_Purchase - Pre_Expense_SalesAndCreditSales_Purchase;
            decimal Opening = CustomerDue + OpeningCashInHand + Stock;

            if (CustomerDue >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Opening";
                ob.Head = "Customer Due";
                ob.Total = Opening;
                ob.Amount = CustomerDue;
                SummaryData.Add(ob);
            }


            if (OpeningCashInHand >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Opening";
                ob.Head = "Cash In Hand";
                ob.Total = Opening;
                ob.Amount = OpeningCashInHand;
                SummaryData.Add(ob);
            }


            if (Stock >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Opening";
                ob.Head = "Stock";
                ob.Total = Opening;
                ob.Amount = Stock;
                SummaryData.Add(ob);
            }


            if (Curr_Income_Sales != 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Sales";
                ob.Head = "";
                ob.Total = Curr_Income_Sales;
                ob.Amount = 0m;
                SummaryData.Add(ob);
            }

            decimal Profit = Curr_Income_Sales - Curr_Expense_SalesAndCreditSales_Purchase;
            if (Profit != 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Profit";
                ob.Head = "";
                ob.Total = Profit;
                ob.Amount = 0m;
                SummaryData.Add(ob);

            }


            decimal PaymentDue_Purchase = Expense_Purchases.Where(o => o.OrderDate <= toDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate <= toDate).Sum(o => o.PaymentDue) : 0m;

            decimal SupplierDue = PaymentDue_Purchase - Pre_Expense_CashDelivery - Pre_Income_CashDelivery_Adjustment - Curr_Income_CashDelivery_Adjustment - Curr_Income_CashDelivery_Adjustment;



            if (SupplierDue >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Supplier Due";
                ob.Head = "";
                ob.Total = SupplierDue;
                ob.Amount = 0m;
                SummaryData.Add(ob);

            }


            if (Curr_Expense_Direct != 0)
                foreach (var item in Direct_Expense_GroupBy)
                {
                    k = k + 1;
                    ob = new SummaryReportModel();
                    ob.id = k;
                    ob.Category = "Cost";
                    ob.Head = item.Description;
                    ob.Total = Curr_Expense_Direct;
                    ob.Amount = item.Amount;
                    SummaryData.Add(ob);

                }


            var Direct_Income_GroupBy = (from ex in Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                         group ex by ex.Description into g
                                         select new
                                         {

                                             Description = g.Key,
                                             Amount = g.Sum(o => o.Amount)
                                         }
                                       );


            if (Curr_Income_Direct != 0)
                foreach (var item in Direct_Income_GroupBy)
                {
                    k = k + 1;
                    ob = new SummaryReportModel();
                    ob.id = k;
                    ob.Category = "Income";
                    ob.Head = item.Description;
                    ob.Total = Curr_Income_Direct;
                    ob.Amount = item.Amount;
                    SummaryData.Add(ob);

                }

            decimal GoodsOrCashIn = Curr_Expense_SalesAndCreditSales_Purchase + Curr_Income_CashColeection + Curr_Income_BankWithdraw;

            if (Curr_Expense_SalesAndCreditSales_Purchase >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Goods Or CashIn";
                ob.Head = "Purchase";
                ob.Total = GoodsOrCashIn;
                ob.Amount = Curr_Expense_SalesAndCreditSales_Purchase;
                SummaryData.Add(ob);

            }


            if (Curr_Income_CashColeection >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Goods Or CashIn";
                ob.Head = "Cash Collection";
                ob.Total = GoodsOrCashIn;
                ob.Amount = Curr_Income_CashColeection;
                SummaryData.Add(ob);

            }


            if (Curr_Income_BankWithdraw >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Goods Or CashIn";
                ob.Head = "Bank Withdraw";
                ob.Total = GoodsOrCashIn;
                ob.Amount = Curr_Income_BankWithdraw;
                SummaryData.Add(ob);

            }


            decimal CashOut = Curr_Expense_CashDelivery + Curr_Expense_CashPaid + Curr_Expense_BankDeposit;

            if (Curr_Expense_CashDelivery >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Cash Out";
                ob.Head = "Cash Delivery";
                ob.Total = CashOut;
                ob.Amount = Curr_Expense_CashDelivery;
                SummaryData.Add(ob);

            }

            if (Curr_Expense_CashPaid >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Cash Out";
                ob.Head = "Cash Paid";
                ob.Total = CashOut;
                ob.Amount = Curr_Expense_CashPaid;
                SummaryData.Add(ob);

            }

            if (Curr_Expense_BankDeposit >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Cash Out";
                ob.Head = "Bank Deposit";
                ob.Total = CashOut;
                ob.Amount = Curr_Expense_BankDeposit;
                SummaryData.Add(ob);

            }



            CustomerDue = Curr_Income_Sales - Curr_Income_CashColeection - Curr_Income_DownPayment;
            Stock = Curr_Expense_Purchase - Curr_Expense_SalesAndCreditSales_Purchase;
            decimal Closing = CustomerDue + ClosingCashInHand + Stock;

            if (CustomerDue >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Closing";
                ob.Head = "Customer Due";
                ob.Total = Closing;
                ob.Amount = CustomerDue;
                SummaryData.Add(ob);
            }


            if (OpeningCashInHand >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Closing";
                ob.Head = "Cash In Hand";
                ob.Total = Closing;
                ob.Amount = ClosingCashInHand;
                SummaryData.Add(ob);
            }


            if (Stock >= 0)
            {
                k = k + 1;
                ob = new SummaryReportModel();
                ob.id = k;
                ob.Category = "Closing";
                ob.Head = "Stock";
                ob.Total = Closing;
                ob.Amount = Stock;
                SummaryData.Add(ob);
            }


            //bd





















            return SummaryData;

        }



        public static List<TransactionReportModel> MonthlyTransactionReport(
this IBaseRepository<CashCollection> CashCollectionRepository,
IBaseRepository<POrder> POrderRepository,
IBaseRepository<POrderDetail> POrderDetailRepository,
IBaseRepository<SOrder> SOrderRepository,
IBaseRepository<SOrderDetail> SOrderDetailRepository,
IBaseRepository<CreditSale> CreditSaleRepository,
IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
IBaseRepository<Stock> StockRepository,
IBaseRepository<StockDetail> StockDetailRepository,
IBaseRepository<ExpenseItem> ExpenseItemRepository,
IBaseRepository<Expenditure> ExpenditureRepository,
IBaseRepository<Bank> BankRepository,
IBaseRepository<BankTransaction> BankTransactionRepository,
IBaseRepository<ROrder> ROrderRepository,
IBaseRepository<ROrderDetail> ROrderDetailRepository,
IBaseRepository<Customer> CustomerRepository,
IBaseRepository<SisterConcern> SisterConcernRepository,
DateTime fromDate, DateTime toDate, int ConcernID
)
        {
            IQueryable<Customer> Customers = null;

            if (ConcernID > 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var oAllCustomerCollData = from CC in CashCollectionRepository.GetAll()
                                       join CO in Customers on CC.CustomerID equals CO.CustomerID
                                       join sis in SisterConcernRepository.GetAll() on CC.ConcernID equals sis.ConcernID
                                       where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                       select new CashCollectionReportModel
                                       {
                                           EntryDate = CC.EntryDate,
                                           CustomerName = CO.Name,
                                           CustomerCode = CO.Code,
                                           Address = CO.Address,
                                           ContactNo = CO.ContactNo,
                                           TotalDue = CO.TotalDue,
                                           Amount = CC.Amount,
                                           AdjustAmt = CC.AdjustAmt,
                                           PaymentType = CC.PaymentType,
                                           AccountNo = CC.AccountNo,
                                           ReceiptNo = CC.ReceiptNo,
                                           Remarks = CC.Remarks,
                                           ConcernName = sis.Name,
                                           ModuleType = "Cash Collection"
                                       };

            DateTime startDate = new DateTime(2019, 8, 30, 16, 5, 7, 123);
            string sFFdate = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";


            startDate = Convert.ToDateTime(sFFdate);
            decimal StartAmount = 10805m;

            double PCashInHandAmt = 0;
            double TotalIncomeAmt = 0;

            double CCashInHandAmt = 0;
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double Opening_CashInhand = 0;
            double Current_CashInhand = 0;
            double Closing_CashInhand = 0;




            var Expense_Purchases = (
                                          from PO in POrderRepository.All
                                          where PO.Status == 1 && PO.OrderDate >= startDate
                                          select new
                                          {
                                              PO.OrderDate,
                                              RecAmt = PO.RecAmt
                                          }
                             );




            var Expense_LastPay =
                 (from CS in CreditSaleRepository.All
                  join CSD in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSD.CreditSalesID
                  where CSD.PaymentStatus == "Paid"
                  select new
                  {
                      LastPayAdjustment = 0m

                  }
                     );



            var Expense_CustomerProductReturn = (from R in ROrderRepository.All
                                                 join CUS in CustomerRepository.All on R.CustomerID equals CUS.CustomerID
                                                 where R.CustomerID != null
                                                 where R.ReturnDate >= startDate
                                                 select new
                                                 {
                                                     GrandTotal = R.GrandTotal,
                                                     ReturnDate = R.ReturnDate,
                                                     PaymentDue = R.GrandTotal - R.PaidAmount,
                                                     PaidAmount = R.PaidAmount,
                                                     CUS.CustomerType
                                                 }
                                      );




            var Expense_Direct = (from ex in ExpenditureRepository.All
                                  join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                  where ((int)exi.Status == 1 && ex.EntryDate >= startDate && exi.Description != "Send cash to head of accounts")
                                  select new
                                  {
                                      ex.EntryDate,
                                      exi.Description,
                                      ex.Amount,

                                  }
                       ).ToList();



            var Expense_CashDelivery = (from csd in CashCollectionRepository.All
                                        where (csd.TransactionType == EnumTranType.ToCompany && csd.EntryDate >= startDate)
                                        select new
                                        {
                                            csd.EntryDate,
                                            csd.Amount,

                                        }
                     ).ToList();


            var Expense_CashCollection_Adjustment = (from CAS in CashCollectionRepository.All
                                                     where CAS.TransactionType == EnumTranType.FromCustomer && CAS.EntryDate >= startDate
                                                     select new
                                                     {
                                                         AdjustAmt = CAS.AdjustAmt,
                                                         CAS.EntryDate,

                                                     }
                 );


            var Expense_BankDeposit =
                                  (from BT in BankTransactionRepository.All
                                   where BT.TransactionType == (int)EnumTransactionType.Deposit && BT.TranDate >= startDate
                                   select new
                                   {
                                       BT.Amount,
                                       BT.TranDate

                                   }
                 );



            var Expense_BankDelivery =
                               (from BT in BankTransactionRepository.All
                                where BT.TransactionType == (int)EnumTransactionType.CashDelivery && BT.TranDate >= startDate
                                select new
                                {
                                    BT.Amount,
                                    BT.TranDate

                                }
              );



            var Income_Sales = (from SO in SOrderRepository.All
                                join CUS in CustomerRepository.All on SO.CustomerID equals CUS.CustomerID
                                where SO.Status == 1 && SO.InvoiceDate >= startDate
                                select new
                                {
                                    SO.InvoiceDate,
                                    RecAmount = SO.RecAmount.Value,
                                    PaymentDue = SO.PaymentDue,
                                    SalesAmt = (SO.GrandTotal - SO.NetDiscount - SO.AdjAmount),
                                    CustomerType = CUS.CustomerType

                                }
                                    );


            var Income_Credit_Sales = (from CS in CreditSaleRepository.All
                                       where CS.SalesDate >= startDate && (int)CS.IsStatus == 1
                                       select new
                                       {
                                           CS.SalesDate,
                                           CS.DownPayment,
                                           SalesAmt = (CS.TSalesAmt - CS.Discount - CS.WInterestAmt)

                                       }
                     );


            var Income_Direct = (from ex in ExpenditureRepository.All
                                 join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                 where ((int)exi.Status == 2 && ex.EntryDate >= startDate)
                                 select new
                                 {
                                     exi.Description,
                                     ex.EntryDate,
                                     ex.Amount,

                                 }
                       ).ToList();

            var Income_CashCollection = (from csd in CashCollectionRepository.All
                                         join CUS in CustomerRepository.All on csd.CustomerID equals CUS.CustomerID
                                         where (csd.TransactionType == EnumTranType.FromCustomer && csd.EntryDate >= startDate)
                                         select new
                                         {
                                             csd.EntryDate,
                                             csd.Amount,
                                             CUS.CustomerType


                                         }
                      ).ToList();

            var Income_CashDelivery_Adjustment = (from CAS in CashCollectionRepository.All
                                                  where CAS.TransactionType == EnumTranType.ToCompany
                                                  where CAS.EntryDate >= startDate
                                                  select new
                                                  {
                                                      AdjustAmt = CAS.AdjustAmt,
                                                      CAS.EntryDate
                                                  }
               );


            var Income_InstallmentCollection = (
                                  from csd in CreditSalesScheduleRepository.All
                                  join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                                  where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                                  select new
                                  {
                                      csd.PaymentDate,
                                      csd.InstallmentAmt,

                                  }
                        );


            var Income_HireCollection = (
                     from csd in CreditSalesScheduleRepository.All
                     join cs in CreditSaleRepository.All on csd.CreditSalesID equals cs.CreditSalesID
                     where (csd.PaymentStatus == "Paid" && csd.PaymentDate >= startDate)
                     select new
                     {
                         csd.PaymentDate,
                         csd.HireValue,

                     }
           );


            var Income_SupplierProductReturn = (from R in POrderRepository.All
                                                where R.SupplierID != null && R.OrderDate > startDate && R.Status == 5
                                                select new
                                                {
                                                    GrandTotal = R.GrandTotal,
                                                    PaymentDue = R.GrandTotal - R.RecAmt,
                                                    ReturnDate = R.OrderDate,
                                                    PaidAmount = R.RecAmt

                                                }
                                     );

            var Income_BankWithdraw =
                                (from BT in BankTransactionRepository.All
                                 where BT.TransactionType == (int)EnumTransactionType.Withdraw && BT.TranDate >= startDate
                                 select new
                                 {
                                     BT.Amount,
                                     BT.TranDate

                                 }
               );


            var Income_BankCollection =
                                (from BT in BankTransactionRepository.All
                                 join CUS in CustomerRepository.All on BT.CustomerID equals CUS.CustomerID
                                 where BT.TransactionType == (int)EnumTransactionType.CashCollection && BT.TranDate >= startDate
                                 select new
                                 {
                                     BT.Amount,
                                     BT.TranDate,
                                     CUS.CustomerType

                                 }
               );


            var Income_SupplierProductReturn_With_Purchase_Details = (from R in POrderRepository.All
                                                                      join RTD in POrderDetailRepository.All on R.POrderID equals RTD.POrderID
                                                                      join STD in StockDetailRepository.All on RTD.POrderDetailID equals STD.POrderDetailID
                                                                      join POD in POrderDetailRepository.All on STD.POrderDetailID equals POD.POrderDetailID
                                                                      join PO in POrderRepository.All on POD.POrderID equals PO.POrderID
                                                                      where R.SupplierID != null && R.OrderDate >= startDate
                                                                      select new
                                                                      {
                                                                          GrandTotal = R.GrandTotal,
                                                                          ReturnDate = R.OrderDate,
                                                                          Purchase = (POD.UnitPrice + ((PO.LaborCost - PO.TDiscount) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * POD.UnitPrice) * RTD.Quantity

                                                                      }
                                  );



            var Direct_Expense_GroupBy = (from ex in Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                          group ex by ex.EntryDate into g
                                          select new
                                          {

                                              EntryDate = g.Key,
                                              Amount = g.Sum(o => o.Amount)
                                          }
                                      );







            var Direct_Income_GroupBy = (from ex in Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                         group ex by ex.Description into g
                                         select new
                                         {

                                             Description = g.Key,
                                             Amount = g.Sum(o => o.Amount)
                                         }
                                        );






            int Days = toDate.Day - fromDate.Day;
            startDate = fromDate;
            DateTime sfromDate = DateTime.Now;
            DateTime sToDate = DateTime.Now;

            List<TransactionReportModel> obtList = new List<TransactionReportModel>();
            TransactionReportModel obt = null;


            for (int i = 1; i <= Days; i++)
            {
                obt = new TransactionReportModel();
                string sFFdate2 = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";
                DateTime firstDate = Convert.ToDateTime(sFFdate2);
                DateTime lastDate = firstDate.AddDays(1).AddSeconds(-1);

                decimal RetailSale = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.SalesAmt) : 0m;


                decimal DealerSale = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.SalesAmt) : 0m;
                decimal HireSale = Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Sum(o => o.SalesAmt).Value : 0m;

                decimal RetailCash = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.RecAmount) : 0m;
                decimal DealerCash = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.RecAmount) : 0m;
                decimal DownPayment = Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Sum(o => o.DownPayment) : 0m;

                decimal Instalment = Income_InstallmentCollection.Where(o => o.PaymentDate >= firstDate && o.PaymentDate <= lastDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate >= firstDate && o.PaymentDate <= lastDate).Sum(o => o.InstallmentAmt) : 0m;
                decimal ExpenseAmt = Expense_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Sum(o => o.Amount) : 0m;
                decimal RetailCashCollection = Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.Amount) : 0m;
                decimal DealerCashCollection = Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.Amount) : 0m;
                decimal RetailCashCollectionByBank = Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;
                decimal DealerCashCollectionByBank = Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;

                decimal CompanyPayment = Expense_CashDelivery.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Sum(o => o.Amount) : 0m;
                decimal CompanyPaymentByBank = Expense_BankDelivery.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Expense_BankDelivery.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;

                decimal RetailReturnSale = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.GrandTotal) : 0m;
                decimal DealerReturnSale = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.GrandTotal) : 0m;


                decimal RetailReturnBack = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.PaidAmount) : 0m;
                decimal DealerReturnBack = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.PaidAmount) : 0m;
                CompanyPayment = CompanyPayment + CompanyPaymentByBank;

                obt.EntryDate = startDate;
                obt.RetailSale = RetailSale - RetailReturnSale;
                obt.HireSale = HireSale;
                obt.DealerSale = DealerSale - DealerReturnSale;
                obt.TotalSale = obt.RetailSale + obt.HireSale + obt.DealerSale;

                obt.RetailCash = RetailCash + RetailCashCollection + RetailCashCollectionByBank - RetailReturnBack;
                obt.DealerCollection = DealerCash + DealerCashCollection - DealerReturnBack + DealerCashCollectionByBank;
                obt.HireCollection = Instalment;
                obt.DownPayment = DownPayment;
                obt.TotalCollection = RetailCash + Instalment + DownPayment + DealerCash;
                obt.DailyExpense = ExpenseAmt;
                obt.CompanyPayment = CompanyPayment;
                obt.Balance = obt.TotalCollection - obt.DailyExpense - obt.CompanyPayment;


                obtList.Add(obt);
                startDate = startDate.AddDays(1);

            }






            return obtList;

        }




        /// <summary>
        /// Date: 25-10-2020
        /// Author: Mohammad Aminul Islam
        /// Reason: Concern wise admin transactional report
        /// </summary>
        /// <returns></returns>
        ///
        public static List<TransactionReportModel> MonthlyAdminTransactionReport(this IBaseRepository<CashCollection> CashCollectionRepository,
                    IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                    IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                    IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                    IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<Stock> StockRepository,
                    IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<ExpenseItem> ExpenseItemRepository,
                    IBaseRepository<Expenditure> ExpenditureRepository, IBaseRepository<Bank> BankRepository,
                    IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<ROrder> ROrderRepository,
                    IBaseRepository<ROrderDetail> ROrderDetailRepository, IBaseRepository<Customer> CustomerRepository,
                    IBaseRepository<SisterConcern> SisterConcernRepository,
                    DateTime fromDate, DateTime toDate, int ConcernID)
        {
            DateTime startDate = new DateTime(2019, 8, 30, 16, 5, 7, 123);
            string sFFdate = startDate.ToString("dd MMM yyyy") + " 12:00:00 AM";


            startDate = Convert.ToDateTime(sFFdate);

            #region Expense Transactions
            var Expense_Purchases = (from PO in POrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                     where PO.Status == (int)EnumPurchaseType.Purchase && (PO.OrderDate >= fromDate && PO.OrderDate <= toDate)
                                     select new
                                     {
                                         PO.OrderDate,
                                         RecAmt = PO.RecAmt
                                     });

            //var Expense_LastPay = (from CS in CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID)
            //                       join CSD in CreditSalesScheduleRepository.All on CS.CreditSalesID equals CSD.CreditSalesID
            //                       where CSD.PaymentStatus == "Paid" && CS.IsStatus == EnumSalesType.Sales && (CSD.PaymentDate >= fromDate && CSD.PaymentDate <= toDate)
            //                       select new
            //                       {
            //                           LastPayAdjustment = CSD.LastPayAdjust
            //                       });

            var Expense_CustomerProductReturn = (from R in ROrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                                 join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on R.CustomerID equals CUS.CustomerID
                                                 where R.ReturnDate >= fromDate && R.ReturnDate <= toDate
                                                 select new
                                                 {
                                                     GrandTotal = R.GrandTotal,
                                                     ReturnDate = R.ReturnDate,
                                                     PaymentDue = R.GrandTotal - R.PaidAmount,
                                                     PaidAmount = R.PaidAmount,
                                                     CUS.CustomerType
                                                 });

            var Expense_Direct = (from ex in ExpenditureRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                  join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                  where ((int)exi.Status == 1 && ex.EntryDate >= startDate && exi.Description != "Send cash to head of accounts")
                                  select new
                                  {
                                      ex.EntryDate,
                                      exi.Description,
                                      ex.Amount,
                                  }).ToList();

            var Expense_CashDelivery = (from csd in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                        where csd.TransactionType == EnumTranType.ToCompany && (csd.EntryDate >= fromDate && csd.EntryDate <= toDate)
                                        select new
                                        {
                                            csd.EntryDate,
                                            csd.Amount,
                                        }).ToList();


            //var Expense_CashCollection_Adjustment = (from CAS in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
            //                                         where CAS.TransactionType == EnumTranType.FromCustomer && (CAS.EntryDate >= fromDate && CAS.EntryDate <= toDate)
            //                                         select new
            //                                         {
            //                                             AdjustAmt = CAS.AdjustAmt,
            //                                             CAS.EntryDate,
            //                                         });


            var Expense_BankDeposit = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                       where BT.TransactionType == (int)EnumTransactionType.Deposit && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                       select new
                                       {
                                           BT.Amount,
                                           BT.TranDate
                                       });

            var Expense_BankWithdrow = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                        where BT.TransactionType == (int)EnumTransactionType.Withdraw && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                        select new
                                        {
                                            BT.Amount,
                                            BT.TranDate
                                        });

            var Expense_BankDelivery = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                        where BT.TransactionType == (int)EnumTransactionType.CashDelivery && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                        select new
                                        {
                                            BT.Amount,
                                            BT.TranDate
                                        });
            #endregion

            #region Income Transactions
            var Income_Sales = (from SO in SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on SO.CustomerID equals CUS.CustomerID
                                where SO.Status == (int)EnumSalesType.Sales && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate)
                                select new
                                {
                                    SO.InvoiceDate,
                                    RecAmount = SO.RecAmount.Value,
                                    PaymentDue = SO.PaymentDue,
                                    SalesAmt = SO.TotalAmount, //(SO.GrandTotal - SO.NetDiscount - SO.AdjAmount),
                                    CustomerType = CUS.CustomerType
                                });


            var Income_Credit_Sales = (from CS in CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                       where (CS.SalesDate >= fromDate && CS.SalesDate <= toDate) && CS.IsStatus == EnumSalesType.Sales
                                       select new
                                       {
                                           CS.SalesDate,
                                           CS.DownPayment,
                                           SalesAmt = (CS.TSalesAmt - CS.Discount - CS.WInterestAmt)
                                       });


            var Income_Direct = (from ex in ExpenditureRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                 join exi in ExpenseItemRepository.All on ex.ExpenseItemID equals exi.ExpenseItemID
                                 where (int)exi.Status == 2 && (ex.EntryDate >= fromDate && ex.EntryDate <= toDate)
                                 select new
                                 {
                                     exi.Description,
                                     ex.EntryDate,
                                     ex.Amount,
                                 }).ToList();

            var Income_CashCollection = (from csd in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                         join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on csd.CustomerID equals CUS.CustomerID
                                         where csd.TransactionType == EnumTranType.FromCustomer && (csd.EntryDate >= fromDate && csd.EntryDate <= toDate)
                                         select new
                                         {
                                             csd.EntryDate,
                                             csd.Amount,
                                             CUS.CustomerType
                                         }).ToList();

            //var Income_CashDelivery_Adjustment = (from CAS in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
            //                                      where CAS.TransactionType == EnumTranType.ToCompany && (CAS.EntryDate >= fromDate && CAS.EntryDate <= toDate)
            //                                      select new
            //                                      {
            //                                          AdjustAmt = CAS.AdjustAmt,
            //                                          CAS.EntryDate
            //                                      });

            var Income_InstallmentCollection = (from csd in CreditSalesScheduleRepository.All
                                                join cs in CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID) on csd.CreditSalesID equals cs.CreditSalesID
                                                where (csd.PaymentStatus == "Paid" && cs.IsStatus == EnumSalesType.Sales && (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate))
                                                select new
                                                {
                                                    csd.PaymentDate,
                                                    csd.InstallmentAmt,
                                                    csd.HireValue
                                                });

            var Income_HireCollection = (from csd in Income_InstallmentCollection
                                         select new
                                         {
                                             csd.PaymentDate,
                                             csd.HireValue,
                                         });


            var Income_SupplierProductReturn = (from po in POrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                                where po.SupplierID != null && (po.OrderDate >= fromDate && po.OrderDate <= toDate) && po.Status == (int)EnumPurchaseType.ProductReturn
                                                select new
                                                {
                                                    GrandTotal = po.GrandTotal,
                                                    PaymentDue = po.GrandTotal - po.RecAmt,
                                                    ReturnDate = po.OrderDate,
                                                    PaidAmount = po.RecAmt
                                                });

            var Income_BankWithdraw = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                       where BT.TransactionType == (int)EnumTransactionType.Withdraw && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                       select new
                                       {
                                           BT.Amount,
                                           BT.TranDate
                                       });


            var Income_BankCollection = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                         join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on BT.CustomerID equals CUS.CustomerID
                                         where BT.TransactionType == (int)EnumTransactionType.CashCollection && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                         && CUS.CustomerType != EnumCustomerType.Dealer
                                         select new
                                         {
                                             BT.Amount,
                                             BT.TranDate,
                                             CUS.CustomerType
                                         });
            var Income_DealerBankCollection = (from BT in BankTransactionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                               join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on BT.CustomerID equals CUS.CustomerID
                                               where BT.TransactionType == (int)EnumTransactionType.CashCollection && (BT.TranDate >= fromDate && BT.TranDate <= toDate)
                                               && CUS.CustomerType == EnumCustomerType.Dealer
                                               select new
                                               {
                                                   BT.Amount,
                                                   BT.TranDate,
                                                   CUS.CustomerType
                                               });

            var Income_SupplierProductReturn_With_Purchase_Details = (from po in POrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                                                      join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                                                      join std in StockDetailRepository.All on pod.POrderDetailID equals std.POrderDetailID
                                                                      where po.SupplierID != null && po.Status == (int)EnumPurchaseType.ProductReturn && (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                                                                      select new
                                                                      {
                                                                          GrandTotal = po.GrandTotal,
                                                                          ReturnDate = po.OrderDate,
                                                                          Purchase = (pod.UnitPrice + ((po.LaborCost - po.TDiscount) / (po.GrandTotal - po.NetDiscount + po.TDiscount)) * pod.UnitPrice) * pod.Quantity
                                                                      });
            #endregion

            var Direct_Expense_GroupBy = (from ex in Expense_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                          group ex by ex.EntryDate into g
                                          select new
                                          {
                                              EntryDate = g.Key,
                                              Amount = g.Sum(o => o.Amount)
                                          });

            var Direct_Income_GroupBy = (from ex in Income_Direct.Where(o => o.EntryDate >= fromDate && o.EntryDate <= toDate)
                                         group ex by ex.Description into g
                                         select new
                                         {
                                             Description = g.Key,
                                             Amount = g.Sum(o => o.Amount)
                                         });

            var Bank_Deposite = (from b in BankTransactionRepository.GetAll().Where(b => b.TranDate >= fromDate && b.TranDate <= toDate && b.TransactionType == 1 && b.ConcernID == ConcernID)
                                 select new
                                 {
                                     b.TranDate,
                                     DepositeAmt = b.Amount
                                 });

            var Bank_Withdrow = (from b in BankTransactionRepository.GetAll().Where(b => b.TranDate >= fromDate && b.TranDate <= toDate && b.TransactionType == 2 && b.ConcernID == ConcernID)
                                 select new
                                 {
                                     b.TranDate,
                                     WithdrowAmt = b.Amount
                                 });

            List<TransactionReportModel> obtList = new List<TransactionReportModel>();
            TransactionReportModel obt = null;

            DateTime firstDate = DateTime.MinValue;
            DateTime lastDate = DateTime.MinValue;
            decimal RetailSale = 0m, DealerSale = 0m, HireSale = 0m, RetailCash = 0m, DealerCash = 0m, DownPayment = 0m, Instalment = 0m, ExpenseAmt = 0m, IncomeAmt = 0m,
                RetailCashCollection = 0m, DealerCashCollection = 0m, RetailCashCollectionByBank = 0m, DealerCashCollectionByBank = 0m, CompanyPayment = 0m,
                CompanyPaymentByBank = 0m, RetailReturnSale = 0m, DealerReturnSale = 0m, RetailReturnBack = 0m, DealerReturnBack = 0m, Balance = 0m, BankDeposite = 0m, BankWithdrow = 0m, CompanyPurchasePayment = 0m;

            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                obt = new TransactionReportModel();
                firstDate = Convert.ToDateTime(date);
                lastDate = firstDate.AddDays(1).AddSeconds(-1);

                RetailSale = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.SalesAmt) : 0m;


                DealerSale = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.SalesAmt) : 0m;
                HireSale = Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Sum(o => o.SalesAmt).Value : 0m;

                RetailCash = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.RecAmount) : 0m;
                DealerCash = Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Income_Sales.Where(o => o.InvoiceDate >= firstDate && o.InvoiceDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.RecAmount) : 0m;
                DownPayment = Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Count() != 0 ? Income_Credit_Sales.Where(o => o.SalesDate >= firstDate && o.SalesDate <= lastDate).Sum(o => o.DownPayment) : 0m;

                Instalment = Income_InstallmentCollection.Where(o => o.PaymentDate >= firstDate && o.PaymentDate <= lastDate).Count() != 0 ? Income_InstallmentCollection.Where(o => o.PaymentDate >= firstDate && o.PaymentDate <= lastDate).Sum(o => o.InstallmentAmt) : 0m;
                ExpenseAmt = Expense_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Expense_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Sum(o => o.Amount) : 0m;


                IncomeAmt = Income_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Income_Direct.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Sum(o => o.Amount) : 0m;

                RetailCashCollection = Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.Amount) : 0m;
                DealerCashCollection = Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Income_CashCollection.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.Amount) : 0m;

                RetailCashCollectionByBank = Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Income_BankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;
                DealerCashCollectionByBank = Income_DealerBankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Income_DealerBankCollection.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;

                CompanyPayment = Expense_CashDelivery.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Count() != 0 ? Expense_CashDelivery.Where(o => o.EntryDate >= firstDate && o.EntryDate <= lastDate).Sum(o => o.Amount) : 0m;
                CompanyPaymentByBank = Expense_BankDelivery.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Expense_BankDelivery.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.Amount) : 0m;
                CompanyPurchasePayment = Expense_Purchases.Where(o => o.OrderDate >= firstDate && o.OrderDate <= lastDate).Count() != 0 ? Expense_Purchases.Where(o => o.OrderDate >= firstDate && o.OrderDate <= lastDate).Sum(o => o.RecAmt) : 0m;

                RetailReturnSale = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.GrandTotal) : 0m;
                DealerReturnSale = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.GrandTotal) : 0m;


                RetailReturnBack = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType != EnumCustomerType.Dealer).Sum(o => o.PaidAmount) : 0m;
                DealerReturnBack = Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Count() != 0 ? Expense_CustomerProductReturn.Where(o => obt.EntryDate >= firstDate && obt.EntryDate <= toDate && o.CustomerType == EnumCustomerType.Dealer).Sum(o => o.PaidAmount) : 0m;

                BankDeposite = Bank_Deposite.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Bank_Deposite.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.DepositeAmt) : 0m;

                BankWithdrow = Bank_Withdrow.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Count() != 0 ? Bank_Withdrow.Where(o => o.TranDate >= firstDate && o.TranDate <= lastDate).Sum(o => o.WithdrowAmt) : 0m;
                CompanyPayment = CompanyPayment + CompanyPaymentByBank + CompanyPurchasePayment;

                obt.EntryDate = date;
                obt.RetailSale = RetailSale - RetailReturnSale;
                obt.HireSale = HireSale;
                obt.DealerSale = DealerSale - DealerReturnSale;
                obt.TotalSale = obt.RetailSale + obt.HireSale + obt.DealerSale;

                obt.RetailCash = RetailCash + RetailCashCollection + RetailCashCollectionByBank - RetailReturnBack;

                obt.DealerCollection = DealerCash + DealerCashCollection - DealerReturnBack + DealerCashCollectionByBank;
                obt.HireCollection = Instalment;
                obt.DownPayment = DownPayment;
                obt.TotalCollection = (RetailCash + RetailCashCollection + RetailCashCollectionByBank - RetailReturnBack) + Instalment + DownPayment + (DealerCash + DealerCashCollection + DealerCashCollectionByBank);
                obt.DailyExpense = ExpenseAmt;
                obt.DailyIncome = IncomeAmt;
                obt.CompanyPayment = CompanyPayment;
                obt.BankDepositeAmount = BankDeposite;
                obt.BankWithdrowAmount = BankWithdrow;
                obt.Balance = obt.TotalCollection - obt.DailyExpense - obt.CompanyPayment - obt.BankDepositeAmount + obt.BankWithdrowAmount;

                Balance = Balance + obt.Balance;
                obt.CumulativeBalance = Balance;

                obtList.Add(obt);
            }
            return obtList;

        }



        public static Tuple<decimal, decimal, decimal, decimal, decimal>
            CustomerWiseCashCollection(this IBaseRepository<CashCollection> CashCollectionRepository,
               IBaseRepository<SOrder> SOrderRepository,
               IBaseRepository<CreditSale> CreditSaleRepository,
               IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
               IBaseRepository<Customer> CustomerRepository,
               IBaseRepository<Bank> _bankRepository,
               IBaseRepository<BankTransaction> _bankTransactionRepository,
               DateTime fromDate, DateTime toDate, int ConcernID)
        {
            decimal TotalRetail = 0m, totalDealerColl = 0m;
            #region Income Transactions
            var retailRecAmount = (from SO in SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                   join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on SO.CustomerID equals CUS.CustomerID
                                   where SO.Status == (int)EnumSalesType.Sales
                                   //&& CUS.CustomerType != EnumCustomerType.Dealer
                                   && CUS.CustomerType == EnumCustomerType.Retail
                                   && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate)
                                   select new
                                   {
                                       RecAmount = SO.RecAmount.Value,
                                   });
            if (retailRecAmount.Count() > 0)
                TotalRetail = retailRecAmount.Sum(i => i.RecAmount);

            var retailCollection = (from csd in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                    join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on csd.CustomerID equals CUS.CustomerID
                                    where csd.TransactionType == EnumTranType.FromCustomer && CUS.CustomerType == EnumCustomerType.Retail
                                    && (csd.EntryDate >= fromDate && csd.EntryDate <= toDate)
                                    select new
                                    {
                                        csd.Amount,
                                    });

            if (retailCollection.Count() > 0)
                TotalRetail += retailCollection.Sum(i => i.Amount);

            var bankCollection = (from bt in _bankTransactionRepository.All
                                  join b in _bankRepository.All on bt.BankID equals b.BankID
                                  join c in CustomerRepository.All on bt.CustomerID equals c.CustomerID into cus
                                  from c1 in cus.DefaultIfEmpty()
                                  where (bt.TranDate >= fromDate && bt.TranDate <= toDate) && bt.TransactionType == (int)EnumTransactionType.CashCollection && c1.CustomerType == EnumCustomerType.Retail
                                  select new
                                  {
                                      Amount = bt.Amount
                                  }).ToList();
            if (bankCollection.Count > 0)
                TotalRetail += bankCollection.Sum(b => b.Amount);


            var dealerRecAmount = (from SO in SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                   join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on SO.CustomerID equals CUS.CustomerID
                                   where SO.Status == (int)EnumSalesType.Sales && CUS.CustomerType == EnumCustomerType.Dealer
                                   && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate)
                                   select new
                                   {
                                       RecAmount = SO.RecAmount.Value,
                                   });
            if (dealerRecAmount.Count() > 0)
                totalDealerColl = dealerRecAmount.Sum(i => i.RecAmount);

            var dealerCollection = (from csd in CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                    join CUS in CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID) on csd.CustomerID equals CUS.CustomerID
                                    where csd.TransactionType == EnumTranType.FromCustomer && CUS.CustomerType == EnumCustomerType.Dealer
                                    && (csd.EntryDate >= fromDate && csd.EntryDate <= toDate)
                                    select new
                                    {
                                        csd.Amount,
                                    });

            if (dealerCollection.Count() > 0)
                totalDealerColl += dealerCollection.Sum(i => i.Amount);

            decimal downpayment = 0m;
            var downpayments = (from CS in CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID)
                                where (CS.SalesDate >= fromDate && CS.SalesDate <= toDate) && CS.IsStatus == EnumSalesType.Sales
                                select new
                                {
                                    CS.DownPayment,
                                });
            if (downpayments.Count() > 0)
                downpayment = downpayments.Sum(i => i.DownPayment);

            decimal InstallmentCollection = 0m;

            var InstallmentCollections = (from csd in CreditSalesScheduleRepository.All
                                          join cs in CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID) on csd.CreditSalesID equals cs.CreditSalesID
                                          where (csd.PaymentStatus == "Paid" && cs.IsStatus == EnumSalesType.Sales && (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate))
                                          select new
                                          {
                                              csd.InstallmentAmt,
                                          });

            if (InstallmentCollections.Count() > 0)
                InstallmentCollection = InstallmentCollections.Sum(i => i.InstallmentAmt);

            #endregion

            return new Tuple<decimal, decimal, decimal, decimal, decimal>(TotalRetail, totalDealerColl, downpayment,
                InstallmentCollection, (TotalRetail + totalDealerColl + downpayment + InstallmentCollection));
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string, string>>>>
                                    GetAllPendingCashCollAsync(this IBaseRepository<CashCollection> cashCollectionRepository,
                                    IBaseRepository<Customer> customerRepository, IBaseRepository<ApplicationUser> UserRepository
                        )
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<ApplicationUser> users = UserRepository.All;

            var items = await (from cashcoll in cashCollectionRepository.All
                               join cus in customers on cashcoll.CustomerID equals cus.CustomerID
                               join us in UserRepository.All on cashcoll.CreatedBy equals us.Id
                               where (cashcoll.TransactionType == EnumTranType.CollectionPending)
                               select new
                               {
                                   cashcoll.CashCollectionID,
                                   cashcoll.EntryDate,
                                   cashcoll.ReceiptNo,
                                   cashcoll.Customer.Code,
                                   cashcoll.Customer.Name,
                                   cashcoll.AccountNo,
                                   cashcoll.Amount,
                                   cashcoll.TransactionType,
                                   cashcoll.Remarks,

                                   us.UserName,
                               }).ToListAsync();

            return items.Select(x => new Tuple<int, DateTime, string, string, string, string, string, Tuple<string, string, string>>
                (
                    x.CashCollectionID,
                    x.EntryDate.Value,
                    x.ReceiptNo.ToString(),
                    x.Name.ToString(),
                    x.AccountNo.ToString(),
                    x.Amount.ToString(),
                    x.TransactionType.ToString(),
                     new Tuple<string, string, string>(x.Remarks, x.Code, x.UserName)
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, string, string, string>>>
        GetAllCashDelivaeryAsync(this IBaseRepository<CashCollection> cashCollectionRepository,
            IBaseRepository<Supplier> supplierRepository,
            DateTime fromDate, DateTime toDate, List<EnumTranType> enumTranTypes)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;
            IQueryable<CashCollection> cashdelivery = null;

            if (fromDate != DateTime.MinValue)
                cashdelivery = cashCollectionRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate);
            else
                cashdelivery = cashCollectionRepository.All;


            var items = await cashdelivery.Where(i => enumTranTypes.Contains(i.TransactionType)).Join(suppliers,
                cashcoll => cashcoll.Supplier.SupplierID, sup => sup.SupplierID,
                (cashcoll, sup) => new { CashCollection = cashcoll, Supplier = sup }).
                Select(x => new
                {
                    x.CashCollection.CashCollectionID,
                    x.CashCollection.EntryDate,
                    x.CashCollection.ReceiptNo,
                    x.CashCollection.Supplier.Name,
                    x.CashCollection.AccountNo,
                    x.CashCollection.Amount,
                    x.CashCollection.TransactionType
                }).ToListAsync();

            return items.Select(x => new Tuple<int, DateTime, string, string, string, string, string>
                (
                    x.CashCollectionID,
                    x.EntryDate.Value,
                    x.ReceiptNo.ToString(),
                    x.Name.ToString(),
                    x.AccountNo.ToString(),
                    x.Amount.ToString(),
                    x.TransactionType.ToString()
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal,
            Tuple<decimal, string, string, string, string, string, EnumCustomerType>>>
        GetCashCollectionDataForAll(
            this IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Customer> CustomerRepository,
        DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID, EnumCustomerType customerType)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerID > 0)
                Customers = CustomerRepository.All.Where(i => i.CustomerID == CustomerID);
            else
            {
                if (customerType > 0)
                    Customers = CustomerRepository.All.Where(i => i.CustomerType == customerType);
                else
                    Customers = CustomerRepository.All;
            }

            var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                        join CO in Customers on CC.CustomerID equals CO.CustomerID
                                        where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                        select new
                                        {
                                            CC.EntryDate,
                                            CO.Name,
                                            CO.Address,
                                            CO.ContactNo,
                                            CO.TotalDue,
                                            CC.Amount,
                                            RemaingAmt = CC.Amount,
                                            CC.AdjustAmt,
                                            CC.PaymentType,
                                            CC.BankName,
                                            CC.AccountNo,
                                            CC.BranchName,
                                            CC.BKashNo,
                                            CustomerType = CO.CustomerType == EnumCustomerType.Retail ? CO.CustomerType : EnumCustomerType.Dealer,
                                        }).OrderByDescending(x => x.EntryDate).ToList();

            return oAllCustomerCollData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal,
                Tuple<decimal, string, string, string, string, string, EnumCustomerType>>
                (
                 (DateTime)x.EntryDate,
                 x.Name,
                x.Address,
                x.ContactNo,
                x.TotalDue,
                x.Amount,
                x.RemaingAmt, new Tuple<decimal, string, string, string, string, string, EnumCustomerType>(
                                    x.AdjustAmt,
                                    x.PaymentType.ToString(),
                                    x.BankName,
                                   x.AccountNo,
                                   x.BranchName,
                                   x.BKashNo,
                                   x.CustomerType
                                   )
                ));

        }

        public static IEnumerable<AdjustmentReportModel> GetAdjustmentReport(this IBaseRepository<CashCollection> cashCollectionRepository,
            IBaseRepository<Customer> customerRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<CashCollection> cashCollections = cashCollectionRepository.All;
            IQueryable<Customer> customers = customerRepository.All;

            var data = (from cc in cashCollections
                        join co in customers on cc.CustomerID equals co.CustomerID
                        where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && cc.AdjustAmt > 0
                        select new AdjustmentReportModel
                        {
                            CSScheduleID = cc.CashCollectionID,
                            CustomerName = co.Name,
                            CustomerCode = co.Code,
                            Date = cc.EntryDate.Value,
                            InvoiceNo = cc.ReceiptNo,
                            AdjutmentAmt = cc.AdjustAmt

                        }).ToList();

            var adjdata = (from cc in cashCollections
                           join co in customers on cc.CustomerID equals co.CustomerID
                           where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && (cc.TransactionType == EnumTranType.DebitAdjustment || cc.TransactionType == EnumTranType.CreditAdjustment)
                           select new AdjustmentReportModel
                           {
                               CSScheduleID = cc.CashCollectionID,
                               CustomerName = co.Name,
                               CustomerCode = co.Code,
                               Date = cc.EntryDate.Value,
                               InvoiceNo = cc.ReceiptNo,
                               AdjutmentAmt = cc.Amount

                           }).ToList();
            data.AddRange(adjdata);

            return data;
        }

        public static IEnumerable<CashCollectionReportModel>
        CashCollectionReportData(this IBaseRepository<CashCollection> CashCollectionRepository,
                IBaseRepository<Customer> CustomerRepository,
                IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<BankTransaction> BankTransitionRepository, IBaseRepository<Bank> BanksRepository, DateTime fromDate, DateTime toDate, int ConcernID,
                int customerID, int ReportType)
        {
            IQueryable<Customer> Customers = null;
            if (customerID > 0)
                Customers = CustomerRepository.GetAll().Where(i => i.CustomerID == customerID);

            else
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            if (ReportType == 1)
            {
                var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                            join CO in Customers on CC.CustomerID equals CO.CustomerID
                                            where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                            && CC.TransactionType == EnumTranType.FromCustomer
                                            select new CashCollectionReportModel
                                            {
                                                EntryDate = CC.EntryDate,
                                                CustomerName = CO.Name + " , " + CO.Address + " , " + CO.ContactNo,
                                                Address = CO.Address,
                                                ContactNo = CO.ContactNo,
                                                TotalDue = CO.TotalDue,
                                                Amount = CC.Amount,
                                                BalanceDue = CC.Amount,
                                                AdjustAmt = CC.AdjustAmt,
                                                PaymentType = CC.PaymentType,
                                                BankName = CC.BankName,
                                                AccountNo = CC.AccountNo,
                                                BranchName = CC.BranchName,
                                                BKashNo = CC.BKashNo,
                                                ModuleType = "Cash Collection"
                                            }).OrderByDescending(x => x.EntryDate).ToList();
                return oAllCustomerCollData;
            }
            else if (ReportType == 2)
            {

                var oSalesData = (from sord in salesOrderRepository.All
                                  join cus in Customers on sord.CustomerID equals cus.CustomerID
                                  where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales & sord.RecAmount > 0)
                                  select new CashCollectionReportModel
                                  {
                                      CustomerCode = cus.Code,
                                      CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                      EntryDate = sord.InvoiceDate,
                                      Amount = (decimal)sord.RecAmount,
                                      CustomerID = sord.CustomerID,
                                      TotalDue = cus.TotalDue,
                                      BankName = "N/A",
                                      AccountNo = "N/A",
                                      BranchName = "N/A",
                                      BKashNo = "N/A",
                                      ModuleType = "Cash Sale"
                                  }).OrderByDescending(x => x.EntryDate).ToList();


                return oSalesData;

            }

            else if (ReportType == 3)
            {

                var oCreditSalesData = (from sord in CreditSaleRepository.All
                                        join cus in Customers on sord.CustomerID equals cus.CustomerID
                                        where (sord.SalesDate >= fromDate && sord.SalesDate <= toDate && sord.IsStatus == EnumSalesType.Sales)
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                            EntryDate = sord.SalesDate,
                                            Amount = (decimal)sord.DownPayment,
                                            CustomerID = sord.CustomerID,
                                            TotalDue = cus.CreditDue,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Credit Sale"
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                return oCreditSalesData;

            }

            else if (ReportType == 4)
            {

                var oInstallmentData = (from cs in CreditSaleRepository.All
                                        join csd in CreditSalesScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                        join cus in Customers on cs.CustomerID equals cus.CustomerID
                                        where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid"
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                            ContactNo = cus.ContactNo,
                                            EntryDate = csd.PaymentDate,
                                            Amount = csd.InstallmentAmt,
                                            Remarks = csd.Remarks,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Installment Collection"
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                return oInstallmentData;

            }

            else if (ReportType == 5)
            {

                var oAllCustomerCollFromBank = (from bt in BankTransitionRepository.All
                                                join b in BanksRepository.All on bt.BankID equals b.BankID
                                                join CO in Customers on bt.CustomerID equals CO.CustomerID
                                                where (bt.TranDate >= fromDate && bt.TranDate <= toDate)
                                                   && bt.TransactionType == 3
                                                select new CashCollectionReportModel
                                                {
                                                    EntryDate = bt.TranDate,
                                                    CustomerName = CO.Name + " , " + CO.Address + " , " + CO.ContactNo,
                                                    Address = CO.Address,
                                                    ContactNo = CO.ContactNo,
                                                    TotalDue = CO.TotalDue,
                                                    Amount = bt.Amount,
                                                    BalanceDue = bt.Amount,
                                                    AdjustAmt = 0m,
                                                    //PaymentType = bt.TransactionType,
                                                    BankName = b.BankName,
                                                    AccountNo = b.AccountNo,
                                                    BranchName = b.BranchName,
                                                    ModuleType = " Bank Cash Collection"
                                                }).OrderByDescending(x => x.EntryDate).ToList();
                return oAllCustomerCollFromBank;


            }
            else
            {
                var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                            join CO in Customers on CC.CustomerID equals CO.CustomerID
                                            where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate)
                                            && CC.TransactionType == EnumTranType.FromCustomer
                                            select new CashCollectionReportModel
                                            {
                                                EntryDate = CC.EntryDate,
                                                CustomerName = CO.Name + " , " + CO.Address + " , " + CO.ContactNo,
                                                Address = CO.Address,
                                                ContactNo = CO.ContactNo,
                                                TotalDue = CO.TotalDue,
                                                Amount = CC.Amount,
                                                BalanceDue = CC.Amount,
                                                AdjustAmt = CC.AdjustAmt,
                                                PaymentType = CC.PaymentType,
                                                BankName = CC.BankName,
                                                AccountNo = CC.AccountNo,
                                                BranchName = CC.BranchName,
                                                BKashNo = CC.BKashNo,
                                                ModuleType = "Cash Collection"
                                            }).OrderByDescending(x => x.EntryDate).ToList();

                var oSalesData = (from sord in salesOrderRepository.All
                                  join cus in Customers on sord.CustomerID equals cus.CustomerID
                                  where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales)
                                  select new CashCollectionReportModel
                                  {
                                      CustomerCode = cus.Code,
                                      CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                      EntryDate = sord.InvoiceDate,
                                      Amount = (decimal)sord.RecAmount,
                                      CustomerID = sord.CustomerID,
                                      TotalDue = cus.TotalDue,
                                      BankName = "N/A",
                                      AccountNo = "N/A",
                                      BranchName = "N/A",
                                      BKashNo = "N/A",
                                      ModuleType = "Cash Sales"
                                  }).OrderByDescending(x => x.EntryDate).ToList();

                var oCreditSalesData = (from sord in CreditSaleRepository.All
                                        join cus in Customers on sord.CustomerID equals cus.CustomerID
                                        where (sord.SalesDate >= fromDate && sord.SalesDate <= toDate && sord.IsStatus == EnumSalesType.Sales)
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                            EntryDate = sord.SalesDate,
                                            Amount = (decimal)sord.DownPayment,
                                            CustomerID = sord.CustomerID,
                                            TotalDue = cus.CreditDue,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Credit Sales"
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                var oInstallmentData = (from cs in CreditSaleRepository.All
                                        join csd in CreditSalesScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                        join cus in Customers on cs.CustomerID equals cus.CustomerID
                                        where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid"
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name + " , " + cus.Address + " , " + cus.ContactNo,
                                            ContactNo = cus.ContactNo,
                                            EntryDate = csd.PaymentDate,
                                            Amount = csd.InstallmentAmt,
                                            Remarks = csd.Remarks,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Installment Collection"
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                oAllCustomerCollData.AddRange(oSalesData);

                oAllCustomerCollData.AddRange(oCreditSalesData);

                oAllCustomerCollData.AddRange(oInstallmentData);

                return oAllCustomerCollData.OrderBy(i => i.EntryDate);
            }
        }



        public static IEnumerable<RPTCustomerAdjustmentDateWise> GetDebitCreditAdjustmentReport(this IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<Customer> customerRepository, int concernId, DateTime fromDate, DateTime toDate, EnumTranType adjustmentType, int CutomerId)
        {
            IQueryable<CashCollection> cashCollections = cashCollectionRepository.All;
            IQueryable<Customer> customers = customerRepository.All;

            #region Customer With Adjustment Type Select 
            if (CutomerId > 0 & adjustmentType > 0)
            {
                var data = (from cc in cashCollections
                            join co in customers on cc.CustomerID equals co.CustomerID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && cc.TransactionType == adjustmentType && co.CustomerID == CutomerId
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount
                            }).ToList();

                return data;
            }
            #endregion
            #region only Customer Select debit credit both
            else if (CutomerId > 0)
            {
                var data = (from cc in cashCollections
                            join co in customers on cc.CustomerID equals co.CustomerID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && ((int)cc.TransactionType == 6 || (int)cc.TransactionType == 7) && co.CustomerID == CutomerId
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount
                            }).ToList();

                return data;
            }
            #endregion

            #region  Adjustment Type Select 
            else
            {
                var data = (from cc in cashCollections
                            join co in customers on cc.CustomerID equals co.CustomerID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && cc.TransactionType == adjustmentType
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount

                            }).ToList();

                return data;
            }
            #endregion

        }




        public static IEnumerable<RPTCustomerAdjustmentDateWise> GetSupplierDebitCreditAdjustmentReport(this IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<Supplier> supplierRepository, int concernId, DateTime fromDate, DateTime toDate, EnumTranType adjustmentType, int SupplierId)
        {
            IQueryable<CashCollection> cashCollections = cashCollectionRepository.All;
            IQueryable<Supplier> suppliers = supplierRepository.All;

            #region Customer With Adjustment Type Select 
            if (SupplierId > 0 & adjustmentType > 0)
            {
                var data = (from cc in cashCollections
                            join co in suppliers on cc.SupplierID equals co.SupplierID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && cc.TransactionType == adjustmentType && co.SupplierID == SupplierId
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount
                            }).ToList();

                return data;
            }
            #endregion
            #region only Customer Select debit credit both
            else if (SupplierId > 0)
            {
                var data = (from cc in cashCollections
                            join co in suppliers on cc.SupplierID equals co.SupplierID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && ((int)cc.TransactionType == 6 || (int)cc.TransactionType == 7) && co.SupplierID == SupplierId
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount
                            }).ToList();

                return data;
            }
            #endregion

            #region Adjustment Type Select 
            else
            {
                var data = (from cc in cashCollections
                            join co in suppliers on cc.SupplierID equals co.SupplierID
                            where cc.EntryDate >= fromDate && cc.EntryDate <= toDate && cc.TransactionType == adjustmentType
                            select new RPTCustomerAdjustmentDateWise
                            {
                                CustomerName = co.Name,
                                Date = cc.EntryDate.Value,
                                ReceiptNo = cc.ReceiptNo,
                                AdjutmentAmt = cc.Amount

                            }).ToList();

                return data;
            }
            #endregion

        }

        public static IEnumerable<CashCollectionReportModel>
        DiscountAdjReportData(this IBaseRepository<CashCollection> CashCollectionRepository,
                IBaseRepository<Customer> CustomerRepository,
                IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<BankTransaction> BankTransitionRepository, IBaseRepository<Bank> BanksRepository, DateTime fromDate, DateTime toDate, int ConcernID,
                int customerID, int ReportType)
        {
            IQueryable<Customer> Customers = null;
            if (customerID > 0)
                Customers = CustomerRepository.GetAll().Where(i => i.CustomerID == customerID);

            else
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            if (ReportType == 1)
            {
                var oSalesData = (from sord in salesOrderRepository.All
                                  join cus in Customers on sord.CustomerID equals cus.CustomerID
                                  where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales && (sord.NetDiscount > 0 || sord.AdjAmount > 0))
                                  select new CashCollectionReportModel
                                  {
                                      CustomerCode = cus.Code,
                                      CustomerName = cus.Name,
                                      EntryDate = sord.InvoiceDate,
                                      Amount = (decimal)sord.RecAmount,
                                      CustomerID = sord.CustomerID,
                                      TotalDue = cus.TotalDue,
                                      BankName = "N/A",
                                      AccountNo = "N/A",
                                      BranchName = "N/A",
                                      BKashNo = "N/A",
                                      ModuleType = "Sales",
                                      AdjustAmt = sord.AdjAmount,
                                      TotalDis = sord.NetDiscount,
                                      ReceiptNo = sord.InvoiceNo
                                  }).OrderByDescending(x => x.EntryDate).ToList();


                return oSalesData;
            }
            else if (ReportType == 2)
            {

                var oCreditSalesData = (from sord in CreditSaleRepository.All
                                        join cus in Customers on sord.CustomerID equals cus.CustomerID
                                        where (sord.SalesDate >= fromDate && sord.SalesDate <= toDate && sord.IsStatus == EnumSalesType.Sales && sord.Discount > 0)
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name,
                                            EntryDate = sord.SalesDate,
                                            Amount = (decimal)sord.DownPayment,
                                            CustomerID = sord.CustomerID,
                                            TotalDue = cus.CreditDue,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Hire Sales",
                                            ReceiptNo = sord.InvoiceNo,
                                            TotalDis = sord.Discount

                                        }).OrderByDescending(x => x.EntryDate).ToList();

                return oCreditSalesData;


            }

            else if (ReportType == 3)
            {

                var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                            join CO in Customers on CC.CustomerID equals CO.CustomerID
                                            where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate && CC.AdjustAmt > 0)
                                            && CC.TransactionType == EnumTranType.FromCustomer
                                            select new CashCollectionReportModel
                                            {
                                                EntryDate = CC.EntryDate,
                                                CustomerName = CO.Name,
                                                Address = CO.Address,
                                                ContactNo = CO.ContactNo,
                                                TotalDue = CO.TotalDue,
                                                Amount = CC.Amount,
                                                BalanceDue = CC.Amount,
                                                AdjustAmt = CC.AdjustAmt,
                                                PaymentType = CC.PaymentType,
                                                BankName = CC.BankName,
                                                AccountNo = CC.AccountNo,
                                                BranchName = CC.BranchName,
                                                BKashNo = CC.BKashNo,
                                                ModuleType = "Cash Collection",
                                                ReceiptNo = CC.ReceiptNo

                                            }).OrderByDescending(x => x.EntryDate).ToList();
                return oAllCustomerCollData;

            }

            else if (ReportType == 4)
            {

                var oInstallmentData = (from cs in CreditSaleRepository.All
                                        join csd in CreditSalesScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                        join cus in Customers on cs.CustomerID equals cus.CustomerID
                                        where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid" && csd.LastPayAdjust > 0
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name,
                                            ContactNo = cus.ContactNo,
                                            EntryDate = csd.PaymentDate,
                                            Amount = csd.InstallmentAmt,
                                            Remarks = csd.Remarks,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Installment Collection",
                                            AdjustAmt = csd.LastPayAdjust,
                                            ReceiptNo = cs.InvoiceNo
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                return oInstallmentData;

            }
            else
            {
                var oSalesData = (from sord in salesOrderRepository.All
                                  join cus in Customers on sord.CustomerID equals cus.CustomerID
                                  where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate && sord.Status == (int)EnumSalesType.Sales && (sord.NetDiscount > 0 || sord.AdjAmount > 0))
                                  select new CashCollectionReportModel
                                  {
                                      CustomerCode = cus.Code,
                                      CustomerName = cus.Name,
                                      EntryDate = sord.InvoiceDate,
                                      Amount = (decimal)sord.RecAmount,
                                      CustomerID = sord.CustomerID,
                                      TotalDue = cus.TotalDue,
                                      BankName = "N/A",
                                      AccountNo = "N/A",
                                      BranchName = "N/A",
                                      BKashNo = "N/A",
                                      ModuleType = "Sales",
                                      //AdjustAmt = sord.AdjAmount,
                                      TotalDis = sord.NetDiscount + sord.AdjAmount,
                                      ReceiptNo = sord.InvoiceNo
                                  }).OrderByDescending(x => x.EntryDate).ToList();

                var oCreditSalesData = (from sord in CreditSaleRepository.All
                                        join cus in Customers on sord.CustomerID equals cus.CustomerID
                                        where (sord.SalesDate >= fromDate && sord.SalesDate <= toDate && sord.IsStatus == EnumSalesType.Sales && sord.Discount > 0)
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name,
                                            EntryDate = sord.SalesDate,
                                            Amount = (decimal)sord.DownPayment,
                                            CustomerID = sord.CustomerID,
                                            TotalDue = cus.CreditDue,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Hire Sales",
                                            ReceiptNo = sord.InvoiceNo,
                                            TotalDis = sord.Discount
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                var oAllCustomerCollData = (from CC in CashCollectionRepository.All
                                            join CO in Customers on CC.CustomerID equals CO.CustomerID
                                            where (CC.EntryDate >= fromDate && CC.EntryDate <= toDate && CC.AdjustAmt > 0)
                                            && CC.TransactionType == EnumTranType.FromCustomer
                                            select new CashCollectionReportModel
                                            {
                                                EntryDate = CC.EntryDate,
                                                CustomerName = CO.Name,
                                                Address = CO.Address,
                                                ContactNo = CO.ContactNo,
                                                TotalDue = CO.TotalDue,
                                                Amount = CC.Amount,
                                                BalanceDue = CC.Amount,
                                                AdjustAmt = CC.AdjustAmt,
                                                PaymentType = CC.PaymentType,
                                                BankName = CC.BankName,
                                                AccountNo = CC.AccountNo,
                                                BranchName = CC.BranchName,
                                                BKashNo = CC.BKashNo,
                                                ModuleType = "Cash Collection",
                                                ReceiptNo = CC.ReceiptNo
                                            }).OrderByDescending(x => x.EntryDate).ToList();

                var oInstallmentData = (from cs in CreditSaleRepository.All
                                        join csd in CreditSalesScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                                        join cus in Customers on cs.CustomerID equals cus.CustomerID
                                        where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid" && csd.LastPayAdjust > 0
                                        select new CashCollectionReportModel
                                        {
                                            CustomerCode = cus.Code,
                                            CustomerName = cus.Name,
                                            ContactNo = cus.ContactNo,
                                            EntryDate = csd.PaymentDate,
                                            Amount = csd.InstallmentAmt,
                                            Remarks = csd.Remarks,
                                            BankName = "N/A",
                                            AccountNo = "N/A",
                                            BranchName = "N/A",
                                            BKashNo = "N/A",
                                            ModuleType = "Installment Collection",
                                            AdjustAmt = csd.LastPayAdjust,
                                            ReceiptNo = cs.InvoiceNo
                                        }).OrderByDescending(x => x.EntryDate).ToList();

                oSalesData.AddRange(oCreditSalesData);

                oSalesData.AddRange(oAllCustomerCollData);

                oSalesData.AddRange(oInstallmentData);

                return oSalesData.OrderBy(i => i.EntryDate);
            }
        }

    }

}
