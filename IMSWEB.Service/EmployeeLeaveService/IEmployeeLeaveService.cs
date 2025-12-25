using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmployeeLeaveService
    {
        void Add(EmployeeLeave EmployeeLeave);
        void Update(EmployeeLeave EmployeeLeave);
        void Save();
        IQueryable<EmployeeLeave> GetAllIQueryable();
        Task<IEnumerable<Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>>> GetAllAsync(DateTime fromDate,DateTime toDate);
        EmployeeLeave GetById(int id);
        void Delete(int id);
    }
}
