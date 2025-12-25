using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<SOrder> _sorderRepository;
        private readonly IBaseRepository<CashCollection> _cashcollectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISisterConcernService _SisterConcernService;

        public CustomerService(IBaseRepository<Customer> customerRepository,
            IBaseRepository<Employee> employeeRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            IUnitOfWork unitOfWork, IBaseRepository<SOrder> sorderRepository, IBaseRepository<CashCollection> cashcollectionRepository, ISisterConcernService SisterConcernService)
        {
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = SisterConcernRepository;
            this._sorderRepository = sorderRepository;
            this._cashcollectionRepository = cashcollectionRepository;
            _SisterConcernService = SisterConcernService;

        }

        public void AddCustomer(Customer product)
        {
            _customerRepository.Add(product);
        }

        public void UpdateCustomer(Customer product)
        {
            _customerRepository.Update(product);
        }

        //public void SaveCustomer()
        //{
        //    _unitOfWork.Commit();
        //}
        public void SaveCustomer()
        {
            try
            {
                _unitOfWork.Commit();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            return _customerRepository.GetAllCustomer().ToList();
        }
        public IQueryable<Customer> GetAll()
        {
            return _customerRepository.All;
        }
        public IQueryable<Customer> GetAll(int ConcernID)
        {
            return _customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }
        public IQueryable<Customer> GetAllShowrooms()
        {
            return _customerRepository.GetAllShowrooms(_SisterConcernRepository);
        }
        public IEnumerable<Customer> GetAllCustomerByEmp(int EmpID)
        {
            return _customerRepository.GetAllCustomerByEmp(EmpID).ToList();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsyncByEmpID(int EmpID)
        {
            return await _customerRepository.GetAllCustomerAsyncByEmpID(EmpID);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _customerRepository.GetAllCustomerAsync();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepository.FindBy(x => x.CustomerID == id).First();
        }

        public IEnumerable<Tuple<string, string, string, string, string, string, decimal, Tuple<decimal, decimal, int>>>
        CustomerCategoryWiseDueRpt(int concernId, int customerId, int reportType, int DueType)
        {
            return _customerRepository.CustomerCategoryWiseDueRpt(concernId, customerId, reportType, DueType);
        }


        public void DeleteCustomer(int id)
        {
            _customerRepository.Delete(x => x.CustomerID == id);
        }


        public IQueryable<SRWiseCustomerStatusReportModel> AdminCustomerDueReport(int concernID, int CustomerType, int DueType)
        {
            return _customerRepository.AdminCustomerDueReport(_SisterConcernRepository, concernID, CustomerType, DueType);
        }

        public string GetUniqueCodeByType(EnumCustomerType customerType)
        {
            string Code = string.Empty;
            if (_customerRepository.All.Any(i => i.CustomerType == customerType))
            {
                var LastCustomer = _customerRepository.All.Where(i => i.CustomerType == customerType).OrderByDescending(i => i.CustomerID).FirstOrDefault();

                Code = (Convert.ToInt32((LastCustomer.Code.Substring(1))) + 1).ToString("D5");
            }
            else
                Code = "00001";

            if (customerType > 0)
            {
                Code = customerType.ToString().Substring(0, 1) + Code;
            }

            return Code;
        }


        public string GetUniqueMemberIDByType(EnumCustomerType customerType)
        {
            string MemberID = string.Empty;
            if (_customerRepository.All.Any(i => i.CustomerType == customerType))
            {
                var LastCustomer = _customerRepository.All.Where(i => i.CustomerType == customerType).OrderByDescending(i => i.CustomerID).FirstOrDefault();

                MemberID = (Convert.ToInt64((LastCustomer.MemberID.Substring(1))) + 1).ToString("D5");
            }
            else
                MemberID = "00001";

            if (customerType > 0)
            {
                MemberID = customerType.ToString().Substring(0, 1) + MemberID;
            }

            return MemberID;
        }

        public IQueryable<Customer> GetAllCustomer(int ConcernID)
        {
            return _customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }


        public bool IsCustomerSalesOrCollectionExists(int customerID)
        {
            if (_sorderRepository.All.Any(i => i.Status == (int)EnumSalesType.Sales && i.CustomerID == customerID))
                return true;

            if (_cashcollectionRepository.All.Any(i => i.TransactionType == EnumTranType.FromCustomer && i.CustomerID == customerID))
                return true;

            return false;
        }
        public decimal GetCreditCustomerRemaingDue(int creditSalesId)
        {
            string query = string.Format(@"SELECT TOP(1) Balance FROM dbo.CreditSalesSchedules WHERE CreditSalesID = {0} AND PaymentStatus = 'Due'
                            ORDER BY ScheduleNo", creditSalesId);

            decimal result = _customerRepository.SQLQuery<decimal>(query);
            return result;

        }

        public List<RPTCustomerDueDateWise> GetCustomerDateWiseTotalDue(int customerId, int concernId, DateTime fromDate, DateTime toDate, int isOnlyDue, EnumCustomerType customerType, int selectedConcernId)
        {
            DateTime asOnDate = fromDate.AddDays(-1);

            List<RPTCustomerDueDateWise> customerDateDueBetween = new List<RPTCustomerDueDateWise>();
            List<RPTCustomerDueDateWise> customerDateDueOpening = new List<RPTCustomerDueDateWise>();

            List<RPTCustomerDueDateWise> customerDateDueBetweenMulti = new List<RPTCustomerDueDateWise>();
            List<RPTCustomerDueDateWise> customerDateDueOpeningMulti = new List<RPTCustomerDueDateWise>();

            if (customerType == EnumCustomerType.Retail)
            {
                if (selectedConcernId > 0)
                {
                    #region sp call
                    List<RPTCustomerDueDateWise> customerDateDueOpeningR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                               new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                               new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                               new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                               new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                               new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                               new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                                ).ToList();

                    List<RPTCustomerDueDateWise> customerDateDueBetweenR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                       new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                    #endregion

                    #region customer info query
                    string queryR = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();

                    #endregion

                    #region combined final result
                    if (customerDateDueOpeningR.Any())
                    {
                        foreach (var item in customerDateDueOpeningR)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateR = customerDateDueBetweenR.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateR != null ? customerDueBetweenDateR.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateR != null ? customerDueBetweenDateR.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateR != null ? customerDueBetweenDateR.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateR != null ? (item.OpeningDue + customerDueBetweenDateR.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpeningR;
                }

                else
                {
                    var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        #region sp call
                        List<RPTCustomerDueDateWise> customerDateDueOpeningR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                               new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                               new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                               new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                               new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                               new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                               new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                                ).ToList();

                        List<RPTCustomerDueDateWise> customerDateDueBetweenR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();


                        customerDateDueOpening.AddRange(customerDateDueOpeningR);
                        customerDateDueBetween.AddRange(customerDateDueBetweenR);
                        #endregion

                    }

                    #region customer info query
                    List<TOCustomerInfoForDueReport> customerInfoR = new List<TOCustomerInfoForDueReport>();
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        string queryR = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                        //List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }).ToList();
                        List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR,
                          new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                          new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                          ).ToList();
                        customerInfoR.AddRange(customerInfos);
                    }
                    #endregion


                    #region combined final result
                    if (customerDateDueOpening.Any())
                    {
                        foreach (var item in customerDateDueOpening)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateR = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateR != null ? customerDueBetweenDateR.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateR != null ? customerDueBetweenDateR.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateR != null ? customerDueBetweenDateR.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateR != null ? (item.OpeningDue + customerDueBetweenDateR.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion

                    return customerDateDueOpening;

                }
            }
            else if (customerType == EnumCustomerType.Dealer)
            {
                if (selectedConcernId > 0)
                {
                    #region sp call

                    List<RPTCustomerDueDateWise> customerDateDueOpeningD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                    List<RPTCustomerDueDateWise> customerDateDueBetweenD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                       new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                    #endregion

                    #region customer info query
                    string queryD = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();
                    #endregion

                    #region combined final result
                    if (customerDateDueOpeningD.Any())
                    {
                        foreach (var item in customerDateDueOpeningD)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateD = customerDateDueBetweenD.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateD != null ? customerDueBetweenDateD.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateD != null ? customerDueBetweenDateD.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateD != null ? customerDueBetweenDateD.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateD != null ? (item.OpeningDue + customerDueBetweenDateD.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpeningD;
                }
                else
                {
                    var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        #region sp call

                        List<RPTCustomerDueDateWise> customerDateDueOpeningD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        List<RPTCustomerDueDateWise> customerDateDueBetweenD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        customerDateDueOpening.AddRange(customerDateDueOpeningD);
                        customerDateDueBetween.AddRange(customerDateDueBetweenD);
                        #endregion


                    }

                    #region customer info query

                    List<TOCustomerInfoForDueReport> customerInfoD = new List<TOCustomerInfoForDueReport>();
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        string queryD = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                        //List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }).ToList();
                        List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD,
                          new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                          new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                          ).ToList();
                        customerInfoD.AddRange(customerInfos);
                    }
                    #endregion

                    #region combined final result
                    if (customerDateDueOpening.Any())
                    {
                        foreach (var item in customerDateDueOpening)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoD.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateD = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateD != null ? customerDueBetweenDateD.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateD != null ? customerDueBetweenDateD.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateD != null ? customerDueBetweenDateD.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateD != null ? (item.OpeningDue + customerDueBetweenDateD.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpening;
                }

            }
            else if (customerType == EnumCustomerType.Hire)
            {
                if (selectedConcernId > 0)
                {
                    #region sp call

                    List<RPTCustomerDueDateWise> customerDateDueOpeningH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                    List<RPTCustomerDueDateWise> customerDateDueBetweenH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                       new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                    #endregion

                    #region customer info query
                    string queryD = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();
                    #endregion

                    #region combined final result
                    if (customerDateDueOpeningH.Any())
                    {
                        foreach (var item in customerDateDueOpeningH)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateH = customerDateDueBetweenH.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateH != null ? customerDueBetweenDateH.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateH != null ? customerDueBetweenDateH.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateH != null ? customerDueBetweenDateH.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateH != null ? (item.OpeningDue + customerDueBetweenDateH.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpeningH;
                }
                else
                {

                    var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        #region sp call

                        List<RPTCustomerDueDateWise> customerDateDueOpeningH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        List<RPTCustomerDueDateWise> customerDateDueBetweenH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        customerDateDueOpening.AddRange(customerDateDueOpeningH);
                        customerDateDueBetween.AddRange(customerDateDueBetweenH);
                        #endregion
                    }

                    #region customer info query
                    List<TOCustomerInfoForDueReport> customerInfoH = new List<TOCustomerInfoForDueReport>();
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        string queryR = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                        //List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }).ToList();
                        List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR,
                          new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                          new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                          ).ToList();
                        customerInfoH.AddRange(customerInfos);
                    }
                    #endregion

                    #region combined final result
                    if (customerDateDueOpening.Any())
                    {
                        foreach (var item in customerDateDueOpening)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoH.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateH = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateH != null ? customerDueBetweenDateH.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateH != null ? customerDueBetweenDateH.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateH != null ? customerDueBetweenDateH.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateH != null ? (item.OpeningDue + customerDueBetweenDateH.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpening;
                }

            }
            else if (customerType == EnumCustomerType.Branch)
            {
                if (selectedConcernId > 0)
                {
                    #region sp call

                    List<RPTCustomerDueDateWise> customerDateDueOpeningB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                    List<RPTCustomerDueDateWise> customerDateDueBetweenB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                       new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                       new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                    #endregion

                    #region customer info query
                    string queryD = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();
                    #endregion

                    #region combined final result
                    if (customerDateDueOpeningB.Any())
                    {
                        foreach (var item in customerDateDueOpeningB)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateB = customerDateDueBetweenB.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateB != null ? customerDueBetweenDateB.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateB != null ? customerDueBetweenDateB.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateB != null ? customerDueBetweenDateB.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateB != null ? (item.OpeningDue + customerDueBetweenDateB.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpeningB;
                }
                else
                {
                    var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        #region sp call

                        List<RPTCustomerDueDateWise> customerDateDueOpeningB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        List<RPTCustomerDueDateWise> customerDateDueBetweenB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                        customerDateDueOpening.AddRange(customerDateDueOpeningB);
                        customerDateDueBetween.AddRange(customerDateDueBetweenB);
                        #endregion
                    }

                    #region customer info query
                    List<TOCustomerInfoForDueReport> customerInfoR = new List<TOCustomerInfoForDueReport>();
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        string queryR = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                        //List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }).ToList();
                        List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR,
                          new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                          new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                          ).ToList();
                        customerInfoR.AddRange(customerInfos);
                    }
                    #endregion

                    #region combined final result
                    if (customerDateDueOpening.Any())
                    {
                        foreach (var item in customerDateDueOpening)
                        {
                            if (item.CustomerID == 196794)
                            {
                                string msg = string.Empty;
                            }
                            TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                            RPTCustomerDueDateWise customerDueBetweenDateB = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                            item.CustomerAddress = customer.CustomerAddress;
                            item.ConcernName = customer.ConcernName;
                            item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                            item.Sales = customerDueBetweenDateB != null ? customerDueBetweenDateB.Sales : item.Sales;
                            item.CashCollectionInterestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                            item.HireIntestrestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.HireIntestrestAmt : item.HireIntestrestAmt;
                            item.TotalAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalAmt : item.TotalAmt;
                            item.SalesReceive = customerDueBetweenDateB != null ? customerDueBetweenDateB.SalesReceive : item.SalesReceive;
                            item.CollectionAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionAmt : item.CollectionAmt;
                            item.InstallmentCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.InstallmentCollection : item.InstallmentCollection;
                            item.TotalCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalCollection : item.TotalCollection;
                            item.SaleReturn = customerDueBetweenDateB != null ? customerDueBetweenDateB.SaleReturn : item.SaleReturn;
                            item.ClosingDue = customerDueBetweenDateB != null ? (item.OpeningDue + customerDueBetweenDateB.ClosingDue) : item.ClosingDue;
                            item.CollectionReturnAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionReturnAmt : item.CollectionReturnAmt;
                            item.CashCollectionsTypeAdjustment = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                        }
                    }
                    #endregion
                    return customerDateDueOpening;
                }

            }
            #region sp call

            if (selectedConcernId > 0)
            {
                customerDateDueOpening = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                      new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
                      new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                      new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                      new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                      new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                      new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                       ).ToList();
                customerDateDueBetween = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
               new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId },
               new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
               new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
               new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
               new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
               new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                ).ToList();
            }
            else
            {
                //var concernData = _SisterConcernRepository.GetAll().ToList();
                var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                List<RPTCustomerDueDateWise> customerDateDueBetweens = new List<RPTCustomerDueDateWise>();
                foreach (var concern in concernData)
                {
                    var concernID = concern.ConcernID;

                    customerDateDueOpeningMulti = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                          new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                          new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                          new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                          new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                          new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                          new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                           ).ToList();
                    customerDateDueBetweenMulti = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();

                    customerDateDueOpening.AddRange(customerDateDueOpeningMulti);
                    customerDateDueBetween.AddRange(customerDateDueBetweenMulti);
                }


            }



            #endregion

            List<TOCustomerInfoForDueReport> customerInfo = new List<TOCustomerInfoForDueReport>();
            if (selectedConcernId > 0)
            {
                #region customer info all concern query
                string query = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                customerInfo = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();
                #endregion
            }
            else
            {
                #region customer Multi Concern info query
                //var concernData = _SisterConcernRepository.GetAll().ToList();
                var concernData = _SisterConcernService.GetFamilyTree(concernId);
                foreach (var concern in concernData)
                {
                    var concernID = concern.ConcernID;
                    string query = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    //List<TOCustomerInfoForDueReport> customerInfos = new List<TOCustomerInfoForDueReport>();

                    //customerInfo = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();


                    List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query,
                      new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                      new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                      ).ToList();
                    customerInfo.AddRange(customerInfos);
                }

                #endregion
            }


            #region combined final result
            if (customerDateDueOpening.Any())
            {
                foreach (var item in customerDateDueOpening)
                {
                    if (item.CustomerID == 196794)
                    {
                        string msg = string.Empty;
                    }
                    TOCustomerInfoForDueReport customer = customerInfo.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                    RPTCustomerDueDateWise customerDueBetweenDate = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                    item.CustomerAddress = customer.CustomerAddress;
                    item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                    item.ConcernName = customer.ConcernName;
                    item.Sales = customerDueBetweenDate != null ? customerDueBetweenDate.Sales : item.Sales;
                    item.CashCollectionInterestAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                    item.HireIntestrestAmt = customerDueBetweenDate != null ? customerDueBetweenDate.HireIntestrestAmt : item.HireIntestrestAmt;
                    item.TotalAmt = customerDueBetweenDate != null ? customerDueBetweenDate.TotalAmt : item.TotalAmt;
                    item.SalesReceive = customerDueBetweenDate != null ? customerDueBetweenDate.SalesReceive : item.SalesReceive;
                    item.CollectionAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CollectionAmt : item.CollectionAmt;
                    item.InstallmentCollection = customerDueBetweenDate != null ? customerDueBetweenDate.InstallmentCollection : item.InstallmentCollection;
                    item.TotalCollection = customerDueBetweenDate != null ? customerDueBetweenDate.TotalCollection : item.TotalCollection;
                    item.SaleReturn = customerDueBetweenDate != null ? customerDueBetweenDate.SaleReturn : item.SaleReturn;
                    item.ClosingDue = customerDueBetweenDate != null ? (item.OpeningDue + customerDueBetweenDate.ClosingDue) : item.ClosingDue;
                    item.CollectionReturnAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CollectionReturnAmt : item.CollectionReturnAmt;
                    item.CashCollectionsTypeAdjustment = customerDueBetweenDate != null ? customerDueBetweenDate.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                }
            }
            #endregion


            return customerDateDueOpening;
        }







        public List<RPTCustomerDueDateWise> GetSingleCustomerDateWiseTotalDue(int customerId, int concernId, DateTime fromDate, DateTime toDate, int isOnlyDue, EnumCustomerType customerType, int selectedConcernId)
        {
            DateTime asOnDate = fromDate.AddDays(-1);

            List<RPTCustomerDueDateWise> customerDateDueBetween = new List<RPTCustomerDueDateWise>();
            List<RPTCustomerDueDateWise> customerDateDueOpening = new List<RPTCustomerDueDateWise>();

            if (customerType == EnumCustomerType.Retail)
            {
                #region sp call

                List<RPTCustomerDueDateWise> customerDateDueOpeningR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                List<RPTCustomerDueDateWise> customerDateDueBetweenR = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptRetail @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();
                #endregion

                #region customer info query
                string queryR = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo,
                                            OpeningDue
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

                List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryR, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();
                #endregion

                #region combined final result
                if (customerDateDueOpeningR.Any())
                {
                    foreach (var item in customerDateDueOpeningR)
                    {
                        if (item.CustomerID == 196794)
                        {
                            string msg = string.Empty;
                        }
                        TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                        RPTCustomerDueDateWise customerDueBetweenDateR = customerDateDueBetweenR.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();


                        item.CustomerAddress = customer.CustomerAddress;
                        item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                        item.Sales = customerDueBetweenDateR != null ? customerDueBetweenDateR.Sales : item.Sales;
                        item.CashCollectionInterestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                        item.HireIntestrestAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.HireIntestrestAmt : item.HireIntestrestAmt;
                        item.TotalAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalAmt : item.TotalAmt;
                        item.SalesReceive = customerDueBetweenDateR != null ? customerDueBetweenDateR.SalesReceive : item.SalesReceive;
                        item.CollectionAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionAmt : item.CollectionAmt;
                        item.CollectionReturnAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionReturnAmt : item.CollectionReturnAmt;
                        item.CollectionReturnAmt = customerDueBetweenDateR != null ? customerDueBetweenDateR.CollectionReturnAmt : item.CollectionReturnAmt;
                        item.InstallmentCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.InstallmentCollection : item.InstallmentCollection;
                        item.TotalCollection = customerDueBetweenDateR != null ? customerDueBetweenDateR.TotalCollection : item.TotalCollection;
                        item.SaleReturn = customerDueBetweenDateR != null ? customerDueBetweenDateR.SaleReturn : item.SaleReturn;
                        item.ClosingDue = customerDueBetweenDateR != null ? (item.OpeningDue + customerDueBetweenDateR.ClosingDue) : item.ClosingDue;
                        item.CashCollectionsTypeAdjustment = customerDueBetweenDateR != null ? customerDueBetweenDateR.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;

                    }
                }
                #endregion
                return customerDateDueOpeningR;
            }
            else if (customerType == EnumCustomerType.Dealer)
            {
                #region sp call

                List<RPTCustomerDueDateWise> customerDateDueOpeningD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                List<RPTCustomerDueDateWise> customerDateDueBetweenD = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptDealer @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();
                #endregion

                #region customer info query
                string queryD = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo,
                                            OpeningDue
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

                List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();
                #endregion

                #region combined final result
                if (customerDateDueOpeningD.Any())
                {
                    foreach (var item in customerDateDueOpeningD)
                    {
                        if (item.CustomerID == 196794)
                        {
                            string msg = string.Empty;
                        }
                        TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                        RPTCustomerDueDateWise customerDueBetweenDateD = customerDateDueBetweenD.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                        item.CustomerAddress = customer.CustomerAddress;
                        item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                        item.Sales = customerDueBetweenDateD != null ? customerDueBetweenDateD.Sales : item.Sales;
                        item.CashCollectionInterestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                        item.HireIntestrestAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.HireIntestrestAmt : item.HireIntestrestAmt;
                        item.TotalAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalAmt : item.TotalAmt;
                        item.SalesReceive = customerDueBetweenDateD != null ? customerDueBetweenDateD.SalesReceive : item.SalesReceive;
                        item.CollectionAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionAmt : item.CollectionAmt;
                        item.CollectionReturnAmt = customerDueBetweenDateD != null ? customerDueBetweenDateD.CollectionReturnAmt : item.CollectionReturnAmt;
                        item.InstallmentCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.InstallmentCollection : item.InstallmentCollection;
                        item.TotalCollection = customerDueBetweenDateD != null ? customerDueBetweenDateD.TotalCollection : item.TotalCollection;
                        item.SaleReturn = customerDueBetweenDateD != null ? customerDueBetweenDateD.SaleReturn : item.SaleReturn;
                        item.ClosingDue = customerDueBetweenDateD != null ? (item.OpeningDue + customerDueBetweenDateD.ClosingDue) : item.ClosingDue;
                        item.CashCollectionsTypeAdjustment = customerDueBetweenDateD != null ? customerDueBetweenDateD.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                    }
                }
                #endregion
                return customerDateDueOpeningD;
            }
            else if (customerType == EnumCustomerType.Hire)
            {
                #region sp call

                List<RPTCustomerDueDateWise> customerDateDueOpeningH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                List<RPTCustomerDueDateWise> customerDateDueBetweenH = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptHire @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();
                #endregion

                #region customer info query
                string queryD = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo,
                                            OpeningDue
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

                List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();
                #endregion

                #region combined final result
                if (customerDateDueOpeningH.Any())
                {
                    foreach (var item in customerDateDueOpeningH)
                    {
                        if (item.CustomerID == 196794)
                        {
                            string msg = string.Empty;
                        }
                        TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                        RPTCustomerDueDateWise customerDueBetweenDateH = customerDateDueBetweenH.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                        item.CustomerAddress = customer.CustomerAddress;
                        item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                        item.Sales = customerDueBetweenDateH != null ? customerDueBetweenDateH.Sales : item.Sales;
                        item.CashCollectionInterestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                        item.HireIntestrestAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.HireIntestrestAmt : item.HireIntestrestAmt;
                        item.TotalAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalAmt : item.TotalAmt;
                        item.SalesReceive = customerDueBetweenDateH != null ? customerDueBetweenDateH.SalesReceive : item.SalesReceive;
                        item.CollectionAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionAmt : item.CollectionAmt;
                        item.CollectionReturnAmt = customerDueBetweenDateH != null ? customerDueBetweenDateH.CollectionReturnAmt : item.CollectionReturnAmt;
                        item.InstallmentCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.InstallmentCollection : item.InstallmentCollection;
                        item.TotalCollection = customerDueBetweenDateH != null ? customerDueBetweenDateH.TotalCollection : item.TotalCollection;
                        item.SaleReturn = customerDueBetweenDateH != null ? customerDueBetweenDateH.SaleReturn : item.SaleReturn;
                        item.ClosingDue = customerDueBetweenDateH != null ? (item.OpeningDue + customerDueBetweenDateH.ClosingDue) : item.ClosingDue;
                        item.CashCollectionsTypeAdjustment = customerDueBetweenDateH != null ? customerDueBetweenDateH.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                    }
                }
                #endregion
                return customerDateDueOpeningH;
            }
            else if (customerType == EnumCustomerType.Branch)
            {
                #region sp call

                List<RPTCustomerDueDateWise> customerDateDueOpeningB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                           new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                           new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                           new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                           new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                           new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                           new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                            ).ToList();

                List<RPTCustomerDueDateWise> customerDateDueBetweenB = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRptBranch @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();
                #endregion

                #region customer info query
                string queryD = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo,
                                            OpeningDue
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

                List<TOCustomerInfoForDueReport> customerInfoR = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(queryD, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();
                #endregion

                #region combined final result
                if (customerDateDueOpeningB.Any())
                {
                    foreach (var item in customerDateDueOpeningB)
                    {
                        if (item.CustomerID == 196794)
                        {
                            string msg = string.Empty;
                        }
                        TOCustomerInfoForDueReport customer = customerInfoR.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                        RPTCustomerDueDateWise customerDueBetweenDateB = customerDateDueBetweenB.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                        item.CustomerAddress = customer.CustomerAddress;
                        item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                        item.Sales = customerDueBetweenDateB != null ? customerDueBetweenDateB.Sales : item.Sales;
                        item.CashCollectionInterestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                        item.HireIntestrestAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.HireIntestrestAmt : item.HireIntestrestAmt;
                        item.TotalAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalAmt : item.TotalAmt;
                        item.SalesReceive = customerDueBetweenDateB != null ? customerDueBetweenDateB.SalesReceive : item.SalesReceive;
                        item.CollectionAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionAmt : item.CollectionAmt;
                        item.CollectionReturnAmt = customerDueBetweenDateB != null ? customerDueBetweenDateB.CollectionReturnAmt : item.CollectionReturnAmt;
                        item.InstallmentCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.InstallmentCollection : item.InstallmentCollection;
                        item.TotalCollection = customerDueBetweenDateB != null ? customerDueBetweenDateB.TotalCollection : item.TotalCollection;
                        item.SaleReturn = customerDueBetweenDateB != null ? customerDueBetweenDateB.SaleReturn : item.SaleReturn;
                        item.ClosingDue = customerDueBetweenDateB != null ? (item.OpeningDue + customerDueBetweenDateB.ClosingDue) : item.ClosingDue;
                        item.CashCollectionsTypeAdjustment = customerDueBetweenDateB != null ? customerDueBetweenDateB.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                    }
                }
                #endregion
                return customerDateDueOpeningB;
            }
            #region sp call

            if (concernId > 0)
            {
                customerDateDueOpening = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                      new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                      new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                      new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                      new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                      new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                      new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                       ).ToList();
                customerDateDueBetween = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
               new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
               new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
               new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
               new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
               new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
               new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                ).ToList();
            }
            else
            {
                //var concernData = _SisterConcernRepository.GetAll().ToList();
                var concernData = _SisterConcernRepository.GetFamilyTree(concernId);
                List<RPTCustomerDueDateWise> customerDateDueBetweens = new List<RPTCustomerDueDateWise>();
                foreach (var concern in concernData)
                {
                    var concernID = concern.ConcernID;
                    customerDateDueBetweens = _customerRepository.ExecSP<RPTCustomerDueDateWise>("GetDatewiseCustomerDueRpt @concernId, @CustomerID, @asOnDate, @isOnlyDue, @fromDate, @toDate",
                   new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                   new SqlParameter("CustomerID", SqlDbType.Int) { Value = customerId },
                   new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                   new SqlParameter("isOnlyDue", SqlDbType.Bit) { Value = 1 },
                   new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                   new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                    ).ToList();

                    customerDateDueOpening.AddRange(customerDateDueBetweens);
                    customerDateDueBetween.AddRange(customerDateDueBetweens);
                }


            }



            #endregion

            List<TOCustomerInfoForDueReport> customerInfo = new List<TOCustomerInfoForDueReport>();
            if (selectedConcernId > 0)
            {
                #region customer info all concern query
                string query = string.Format(@"SELECT CONCAT(Code, ', ', Name, ', ', Address, ', ', ContactNo) as CustomerAddress, Name as CustomerName,                                CompanyName,
	                                        CASE 
		                                        WHEN CustomerType=1 THEN 'Retail'
                                                WHEN CustomerType=2 THEN 'Dealer'
                                                WHEN CustomerType=3 THEN 'Hire'
                                                WHEN CustomerType=4 THEN 'Branch'
	                                        END
	                                        AS CustomerType,
	                                        Address,
	                                        CustomerID,
                                            ContactNo,
                                            OpeningDue
                                        FROM Customers
                                        WHERE ConcernID = @concernId");

                customerInfo = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query, new SqlParameter("concernId", SqlDbType.Int) { Value = selectedConcernId }).ToList();
                #endregion
            }
            else
            {
                #region customer Multi Concern info query
                //var concernData = _SisterConcernRepository.GetAll().ToList();
                var concernData = _SisterConcernService.GetFamilyTree(concernId);
                foreach (var concern in concernData)
                {
                    var concernID = concern.ConcernID;
                    string query = string.Format(@"SELECT CONCAT(c.Code, ', ', c.Name, ', ', c.Address, ', ', c.ContactNo) AS CustomerAddress,c.Name AS CustomerName,c.CompanyName,sc.Name AS ConcernName, CASE  
                   WHEN c.CustomerType = 1 THEN 'Retail'
                   WHEN c.CustomerType = 2 THEN 'Dealer'
                   WHEN c.CustomerType = 3 THEN 'Hire'
                   WHEN c.CustomerType = 4 THEN 'Branch'
                   END AS CustomerType,
                    c.Address,
                    c.CustomerID,
                    c.ContactNo,
                    c.OpeningDue,
                    sc.Name AS SisterConcernName
                FROM  Customers c
                INNER JOIN SisterConcerns sc ON c.ConcernID = sc.ConcernID
                WHERE c.ConcernID = @concernID");

                    //List<TOCustomerInfoForDueReport> customerInfos = new List<TOCustomerInfoForDueReport>();

                    //customerInfo = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query, new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }).ToList();


                    List<TOCustomerInfoForDueReport> customerInfos = _customerRepository.ExecSP<TOCustomerInfoForDueReport>(query,
                      new SqlParameter("customerId", SqlDbType.Int) { Value = customerId },
                      new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                      ).ToList();
                    customerInfo.AddRange(customerInfos);
                }

                #endregion
            }


            #region combined final result
            if (customerDateDueOpening.Any())
            {
                foreach (var item in customerDateDueOpening)
                {
                    if (item.CustomerID == 196794)
                    {
                        string msg = string.Empty;
                    }
                    TOCustomerInfoForDueReport customer = customerInfo.Where(c => c.CustomerID == item.CustomerID).FirstOrDefault();
                    RPTCustomerDueDateWise customerDueBetweenDate = customerDateDueBetween.Where(d => d.CustomerID == item.CustomerID).FirstOrDefault();

                    item.CustomerAddress = customer.CustomerAddress;
                    item.OpeningDue = customer.OpeningDue + item.ClosingDue;
                    item.ConcernName = customer.ConcernName;
                    item.Sales = customerDueBetweenDate != null ? customerDueBetweenDate.Sales : item.Sales;
                    item.CashCollectionInterestAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CashCollectionInterestAmt : item.CashCollectionInterestAmt;
                    item.HireIntestrestAmt = customerDueBetweenDate != null ? customerDueBetweenDate.HireIntestrestAmt : item.HireIntestrestAmt;
                    item.TotalAmt = customerDueBetweenDate != null ? customerDueBetweenDate.TotalAmt : item.TotalAmt;
                    item.SalesReceive = customerDueBetweenDate != null ? customerDueBetweenDate.SalesReceive : item.SalesReceive;
                    item.CollectionAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CollectionAmt : item.CollectionAmt;
                    item.CollectionReturnAmt = customerDueBetweenDate != null ? customerDueBetweenDate.CollectionReturnAmt : item.CollectionReturnAmt;
                    item.InstallmentCollection = customerDueBetweenDate != null ? customerDueBetweenDate.InstallmentCollection : item.InstallmentCollection;
                    item.TotalCollection = customerDueBetweenDate != null ? customerDueBetweenDate.TotalCollection : item.TotalCollection;
                    item.SaleReturn = customerDueBetweenDate != null ? customerDueBetweenDate.SaleReturn : item.SaleReturn;
                    item.ClosingDue = customerDueBetweenDate != null ? (item.OpeningDue + customerDueBetweenDate.ClosingDue) : item.ClosingDue;
                    item.CashCollectionsTypeAdjustment = customerDueBetweenDate != null ? customerDueBetweenDate.CashCollectionsTypeAdjustment : item.CashCollectionsTypeAdjustment;
                    //item.BankTransCashCollectionReturn = customerDueBetweenDate != null ? customerDueBetweenDate.BankTransCashCollectionReturn : item.BankTransCashCollectionReturn;
                }
            }
            #endregion


            return customerDateDueOpening;
        }

        public IQueryable<Customer> GetAllIQueryable()
        {
            return _customerRepository.All;
        }




    }
}
