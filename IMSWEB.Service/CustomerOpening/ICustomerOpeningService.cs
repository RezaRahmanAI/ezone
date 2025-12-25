using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICustomerOpeningService
    {
        void Add(CustomerOpeningDue CustomerOpeningDue);
        void Update(CustomerOpeningDue CustomerOpeningDue);
        void Save();
        IEnumerable<CustomerOpeningDue> GetAll();
        CustomerOpeningDue GetById(int id);
        void Delete(int id);
        List<CustomerOpeningDue> CustomerOpeningDueSave(int ConcernID);
    }
}
