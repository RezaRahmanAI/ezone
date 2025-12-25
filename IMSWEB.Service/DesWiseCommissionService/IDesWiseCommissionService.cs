using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public interface IDesWiseCommissionService
    {
       void Add(DesWiseCommission DesWiseCommission);
        void Update(DesWiseCommission DesWiseCommission);
        void Save();
        IEnumerable<DesWiseCommission> GetAll();
        IQueryable<DesWiseCommission> GetAllIQueryable();
        Task<IEnumerable<Tuple<int, Decimal, string>>> GetAllAsync();
        DesWiseCommission GetById(int id);
        void Delete(int id);
    }
}
