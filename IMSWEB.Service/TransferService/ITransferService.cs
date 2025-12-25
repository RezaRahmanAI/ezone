using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ITransferService
    {
        void Add(Transfer Transfer);
        void Update(Transfer Transfer);
        void Save();
        IQueryable<Transfer> GetAll();
        IQueryable<Transfer> GetAll(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>>>
            GetAllAsync(DateTime fromDate, DateTime toDate, int ConcernID, int page = 1, int pageSize = 50);
        Transfer GetById(int id);
        void Delete(int id);
        Tuple<bool, int> AddTranserferUsingSP(DataTable dtTranser, DataTable dtDetails, DataTable dtTransferFromStock, DataTable dtTransferToStock);
        IEnumerable<ProductWisePurchaseModel> GetDetailsByID(int TransferID);
        bool ReturnTranserferUsingSP(int TransferID);
        IEnumerable<ProductWisePurchaseModel> GetTransferReport(DateTime FromDate, DateTime ToDate,int ConcernID);

        //bool CheckTransProductStatusByTransDId(int id);
        int CheckTransProductStatusByTransDId(int id);
        int CheckTransProductHireSalesStatusByTransDId(int id);
        IEnumerable<ProductWisePurchaseModel> GetTransferReportFromTo(DateTime FromDate, DateTime ToDate, int FromConern, int ToConcern);

    }
} 
