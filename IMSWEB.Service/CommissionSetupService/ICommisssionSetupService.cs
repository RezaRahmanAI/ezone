using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICommisssionSetupService
    {
        void Add(CommissionSetup CommissionSetup);
        void Update(CommissionSetup CommissionSetup);
        void Save();
        IQueryable<CommissionSetup> GetAll();
        IQueryable<CommissionSetup> GetByEmployeeIDandMonth(int EmployeeID,DateTime fromDate,DateTime toDate);
        Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync();
        CommissionSetup GetById(int id);
        void Delete(int id);
    }
}
