using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class SMSBillPaymentService : ISMSBillPaymentService
    {
        private readonly IBaseRepository<SMSBillPayment> _Repository;
        private readonly IUnitOfWork _unitOfWork;

        public SMSBillPaymentService(IBaseRepository<SMSBillPayment> Repository, IUnitOfWork unitOfWork)
        {
            _Repository = Repository;
            _unitOfWork = unitOfWork;
        }

        public void Add(SMSBillPayment SMSBillPayment)
        {
            _Repository.Add(SMSBillPayment);
        }

        public void Update(SMSBillPayment SMSBillPayment)
        {
            _Repository.Update(SMSBillPayment);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<SMSBillPayment> GetAll()
        {
            return _Repository.All.OrderByDescending(i=>i.PaidToDate);
        }
        public SMSBillPayment GetById(int id)
        {
            return _Repository.FindBy(x => x.BillPayID == id).First();
        }

        public void Delete(int id)
        {
            _Repository.Delete(x => x.BillPayID == id);
        }
    }
}
