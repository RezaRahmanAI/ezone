using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class SMSBillPaymentBkashService : ISMSBillPaymentBkashService 
    {
        private readonly IBaseRepository<SMSPaymentMaster> _Repository;
        private readonly IUnitOfWork _unitOfWork;

        public SMSBillPaymentBkashService(IBaseRepository<SMSPaymentMaster> Repository, IUnitOfWork unitOfWork)
        {
            _Repository = Repository;
            _unitOfWork = unitOfWork;
        }

        public void Add(SMSPaymentMaster Model)
        {
            _Repository.Add(Model);
        }

        public void Update(SMSPaymentMaster SMSBillPayment)
        {
            _Repository.Update(SMSBillPayment);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<SMSPaymentMaster> GetAll()
        {
            return _Repository.All.OrderByDescending(i=>i.CreateDate);
        }
        public SMSPaymentMaster GetById(int id)
        {
            return _Repository.FindBy(x => x.SMSPaymentMasterID == id).First();
        }

        public SMSPaymentMaster GetByConcernId(int id)
        {
            return _Repository.FindBy(x => x.ConcernID == id).First();
        }

        
        public void Delete(int id)
        {
            _Repository.Delete(x => x.SMSPaymentMasterID == id);
        }
    }
}
