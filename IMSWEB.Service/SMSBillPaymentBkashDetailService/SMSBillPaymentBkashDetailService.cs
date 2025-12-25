using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class SMSBillPaymentBkashDetailService : ISMSBillPaymentBkashDetailService
    {
        private readonly IBaseRepository<SMSPaymentMasterDetails> _Repository;
        private readonly IUnitOfWork _unitOfWork;

        public SMSBillPaymentBkashDetailService(IBaseRepository<SMSPaymentMasterDetails> Repository, IUnitOfWork unitOfWork)
        {
            _Repository = Repository;
            _unitOfWork = unitOfWork;
        }

        public void Add(SMSPaymentMasterDetails Model)
        {
            _Repository.Add(Model);
        }

        public void Update(SMSPaymentMasterDetails SMSBillPayment)
        {
            _Repository.Update(SMSBillPayment);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<SMSPaymentMasterDetails> GetAll()
        {
            return _Repository.All.OrderByDescending(i=>i.CreateDate);
        }
        public SMSPaymentMasterDetails GetById(int id)
        {
            return _Repository.FindBy(x => x.SMSPaymentDetailsID == id).First();
        }

        public SMSPaymentMasterDetails GetBySmsBillPaymentMasterId(int id)
        {
            return _Repository.FindBy(x => x.SMSPaymentMasterID == id).First();
        }

        
        public void Delete(int id)
        {
            _Repository.Delete(x => x.SMSPaymentMasterID == id);
        }

        public IEnumerable<SMSPaymentMasterDetails> GetLastPayAmount(int SMSBillpayMasterID)
        {
            return _Repository.GetLastPayAmount(SMSBillpayMasterID);
        }
    }
}
