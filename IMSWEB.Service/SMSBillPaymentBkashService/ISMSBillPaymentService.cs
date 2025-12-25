using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISMSBillPaymentBkashService
    {
        void Add(SMSPaymentMaster Model);
        void Update(SMSPaymentMaster Model);
        void Save();
        IQueryable<SMSPaymentMaster> GetAll();
        SMSPaymentMaster GetById(int id);
        SMSPaymentMaster GetByConcernId(int id);
        void Delete(int id);
    }
}
