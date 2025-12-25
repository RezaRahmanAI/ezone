using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public interface ITargetSetupService
    {
        void Add(TargetSetup TargetSetup);
        void Update(TargetSetup TargetSetup);
        void Save();
        IQueryable<TargetSetup> GetAllIQueryable();
        Task<IEnumerable<Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>>> GetAllAsync();
        TargetSetup GetById(int id);
        TargetSetup GetByEmployeeIDandTargetMonth(int EmployeeID,DateTime fromDate,DateTime toDate);
        List<TargetSetupDetail> GetTargetSetupDetailsById(int TID);
        void Delete(int id);
        bool AddTargetSetupUsingSP(DataTable dtTargetSetup, DataTable dtTargetSetupDetails);
        bool DeleteTargetSetupUsingSP(int TID);
    }
}
