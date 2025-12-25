using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISMSBillPaymentBkashDetailService
    {
        void Add(SMSPaymentMasterDetails Model);
        void Update(SMSPaymentMasterDetails Model);
        void Save();
        IQueryable<SMSPaymentMasterDetails> GetAll();
        SMSPaymentMasterDetails GetById(int id);
        SMSPaymentMasterDetails GetBySmsBillPaymentMasterId(int id);
        void Delete(int id);
        IEnumerable<SMSPaymentMasterDetails> GetLastPayAmount(int SMSBillpaymentMasterID);

    }
}
