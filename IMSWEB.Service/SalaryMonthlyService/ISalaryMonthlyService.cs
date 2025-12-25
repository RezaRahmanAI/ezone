using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISalaryMonthlyService
    {
        void Add(SalaryMonthly SalaryMonthly);
        void Update(SalaryMonthly SalaryMonthly);
        void Save();
        IQueryable<SalaryMonthly> GetAllIQueryable();
        List<SalaryMonthlyDetail> GetSalaryMonthlyDetailBy(int SalaryMonthlyID);
        Task<IEnumerable<SalaryMonthly>> GetAllAsync();
        SalaryMonthly GetById(int id);
        void Delete(int id);
    }
}
