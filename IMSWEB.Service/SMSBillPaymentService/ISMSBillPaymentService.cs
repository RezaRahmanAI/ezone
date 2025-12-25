using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISMSBillPaymentService
    {
        void Add(SMSBillPayment SMSBillPayment);
        void Update(SMSBillPayment SMSBillPayment);
        void Save();
        IQueryable<SMSBillPayment> GetAll();
        SMSBillPayment GetById(int id);
        void Delete(int id);
    }
}
