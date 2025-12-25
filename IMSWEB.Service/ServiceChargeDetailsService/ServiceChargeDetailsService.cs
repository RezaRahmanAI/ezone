using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ServiceChargeDetailsService : IServiceChargeDetailsService
    {
        private readonly IBaseRepository<ServiceChargeDetails> _serviceChargeDetailsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceChargeDetailsService(IBaseRepository<ServiceChargeDetails> serviceChargeDetailsRepository, IUnitOfWork unitOfWork)
        {
            _serviceChargeDetailsRepository = serviceChargeDetailsRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(ServiceChargeDetails model)
        {
            _serviceChargeDetailsRepository.Add(model);
        }

        public void AddMultiple(List<ServiceChargeDetails> models)
        {
            _serviceChargeDetailsRepository.AddMultiple(models);
        }

        public void Delete(int id)
        {
            _serviceChargeDetailsRepository.Delete(d => d.Id == id);
        }

        public void DeleteByMaster(int serviceId)
        {
            _serviceChargeDetailsRepository.Delete(d => d.ServiceChargeId == serviceId);
        }

        public IEnumerable<ServiceChargeDetails> GetAll()
        {
            return _serviceChargeDetailsRepository.All;
        }

        public ServiceChargeDetails GetById(int id)
        {
            return _serviceChargeDetailsRepository.FindBy(d => d.Id == id).FirstOrDefault();
        }

        public List<ServiceChargeDetails> GetAllByServiceId(int serviceId)
        {
            return _serviceChargeDetailsRepository.FindBy(d => d.ServiceChargeId == serviceId).ToList();
        }

        public bool Save()
        {
            bool isSave = false;
            try
            {
                _unitOfWork.Commit();
                isSave = true;
            }
            catch (Exception ex)
            {
                
            }
            return isSave;
        }

        public void Update(ServiceChargeDetails model)
        {
            _serviceChargeDetailsRepository.Update(model);
        }
        public Tuple<decimal, DateTime, DateTime, int> GetDueServiceAmountByServicId(int serviceId, DateTime localDate)
        {
            List<ServiceChargeDetails> serviceChargeDetails = _serviceChargeDetailsRepository.FindBy(d => d.ServiceChargeId == serviceId && !d.IsPaid).ToList();
            if (serviceChargeDetails.Any())
            {
                int lowestMonth = serviceChargeDetails.Min(d => d.Month);
                int maxMonth = serviceChargeDetails.Max(d => d.Month);
                int Year = localDate.Year;
                DateTime fromDate = new DateTime(Year, lowestMonth, 1);
                int lasDayOfMaxMonth = DateTime.DaysInMonth(localDate.Year, maxMonth);
                DateTime toDate = new DateTime(Year, maxMonth, lasDayOfMaxMonth);


                decimal totalAmount = serviceChargeDetails.Sum(d => d.ExpectedServiceCharge);
                //PaymentDueInfoTO data = new PaymentDueInfoTO
                //{
                //    TotalChargeDue = totalAmount,
                //    ServiceFrom = fromDate,
                //    ServiceTo = toDate,

                //}

                return new Tuple<decimal, DateTime, DateTime, int>(totalAmount, fromDate, toDate, serviceChargeDetails.Count);
            }
            else
                return new Tuple<decimal, DateTime, DateTime, int>(0m, DateTime.Now, DateTime.Now, 0);
        }

        public List<ServiceChargeDetails> GetAllDueDetailsByMasterId(int serviceChargeId)
        {
            return _serviceChargeDetailsRepository.FindBy(d => d.ServiceChargeId == serviceChargeId && !d.IsPaid).ToList();
        }
    }
}
