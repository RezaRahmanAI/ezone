using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmployeeCommissionService
    {
        void Add(EmployeeCommission EmployeeCommission);
        void Update(EmployeeCommission EmployeeCommission);
        void Save();
        IQueryable<EmployeeCommission> GetAll();
        EmployeeCommission GetById(int id);
        void Delete(int id);
    }
}
