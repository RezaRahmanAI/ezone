using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICustomerService
    {
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void SaveCustomer();
        IEnumerable<Customer> GetAllCustomer();
        IQueryable<Customer> GetAll();
        IQueryable<Customer> GetAll(int ConcernID);
        IEnumerable<Customer> GetAllCustomerByEmp(int EmpID);

        Task<IEnumerable<Customer>> GetAllCustomerAsync();

        Task<IEnumerable<Customer>> GetAllCustomerAsyncByEmpID(int EmpID);

        IEnumerable<Tuple<string, string, string, string, string, string, decimal, Tuple<decimal, decimal,int>>>
        CustomerCategoryWiseDueRpt(int nConcernId, int nCustomerId, int nReportType, int DueType);

        Customer GetCustomerById(int id);
        void DeleteCustomer(int id);
        IQueryable<SRWiseCustomerStatusReportModel> AdminCustomerDueReport(int concernID, int CustomerType, int DueType);
        string GetUniqueCodeByType(EnumCustomerType customerType);
        string GetUniqueMemberIDByType(EnumCustomerType customerType);
        IQueryable<Customer> GetAllCustomer(int ConcernID);
        IQueryable<Customer> GetAllShowrooms();

        bool IsCustomerSalesOrCollectionExists(int customerID);
        decimal GetCreditCustomerRemaingDue(int creditSalesId);
        List<RPTCustomerDueDateWise> GetCustomerDateWiseTotalDue(int customerId, int concernId, DateTime fromDate, DateTime toDate, int isOnlyDue, EnumCustomerType customerType, int SelectedConcernID);


        List<RPTCustomerDueDateWise> GetSingleCustomerDateWiseTotalDue(int customerId, int concernId, DateTime fromDate, DateTime toDate, int isOnlyDue, EnumCustomerType customerType, int SelectedConcernID);

        IQueryable<Customer> GetAllIQueryable();



    }
}
