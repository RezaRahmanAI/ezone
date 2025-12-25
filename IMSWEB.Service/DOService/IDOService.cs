using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IDOService
    {
        Tuple<bool, int> ADDDOEntry(DO DO, int DOID);
        void Update(DO DO);
        void UpdateDoDetails(DODetail Model);
        void Save();
        IQueryable<DO> GetAll();
        IQueryable<DODetail> GetAllDetail();
        IEnumerable<DO> GetAll(EnumDOStatus enumDOStatus);
        List<DO> GetAll(EnumDOStatus enumDOStatus, DateTime fromDate, DateTime toDate, bool IsSalesDO, int concernID);
        DO GetById(int id);
        bool Delete(int id, int userID, DateTime dateTime);
        List<DODetail> GetDetailsById(int id);

        DODetail GetDetailById(int id);
        List<ProductWisePurchaseModel> ProductWisePurchaseDOReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);
    }
}
