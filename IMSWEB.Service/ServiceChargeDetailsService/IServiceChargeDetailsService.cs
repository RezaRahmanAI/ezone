using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IServiceChargeDetailsService
    {
        void Add(ServiceChargeDetails model);
        void AddMultiple(List<ServiceChargeDetails> models);
        void Update(ServiceChargeDetails model);
        bool Save();
        IEnumerable<ServiceChargeDetails> GetAll();
        ServiceChargeDetails GetById(int id);
        void Delete(int id);
        void DeleteByMaster(int serviceId);
        List<ServiceChargeDetails> GetAllByServiceId(int serviceId);
        Tuple<decimal, DateTime, DateTime, int> GetDueServiceAmountByServicId(int serviceId, DateTime localDate);
        List<ServiceChargeDetails> GetAllDueDetailsByMasterId(int serviceChargeId);
    }
}
