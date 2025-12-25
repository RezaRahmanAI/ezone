using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IServiceChargeService
    {
        void Add(ServiceCharge model);
        void Update(ServiceCharge model);
        bool Save();
        IEnumerable<ServiceCharge> GetAll();
        ServiceCharge GetById(int id);
        void Delete(int id);
        ServiceCharge GetByYearAndConcern(int concernId, int year);
        int GetServiceChargeIdByConcernAndYear(int concernId, int year);
    }
}
