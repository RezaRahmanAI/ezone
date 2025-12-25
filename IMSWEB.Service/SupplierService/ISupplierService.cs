using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISupplierService
    {
        void AddSupplier(Supplier Supplier);
        void UpdateSupplier(Supplier Supplier);
        void SaveSupplier();
        IQueryable<Supplier> GetAllSupplier();
        Task<IEnumerable<Supplier>> GetAllSupplierAsync();

        IEnumerable<Tuple<string, string, string, string, string,decimal, string>>
        ConcernWiseSupplierDueRpt(int nConcernId,int nSupplierId,bool IsAdminReport);

        Supplier GetSupplierById(int id);
        void DeleteSupplier(int id);
        List<SupplierDueTO> GetCustomerDateWiseTotalDue(int supplierId, int concernId, DateTime fromDate, DateTime toDate, bool isTrialBalance, int UserConcernID);
        int GetSupplierIdBySDetailId(int id);
    }
}
