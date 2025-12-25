using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ServiceChargeService : IServiceChargeService
    {
        private readonly IBaseRepository<ServiceCharge> _serviceChargeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceChargeService(IBaseRepository<ServiceCharge> serviceChargeRepository, IUnitOfWork unitOfWork)
        {
            _serviceChargeRepository = serviceChargeRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(ServiceCharge model)
        {
            _serviceChargeRepository.Add(model);
        }

        public void Delete(int id)
        {
            _serviceChargeRepository.Delete(d => d.Id == id);
        }

        public IEnumerable<ServiceCharge> GetAll()
        {
            return _serviceChargeRepository.All;
        }

        public ServiceCharge GetById(int id)
        {
            return _serviceChargeRepository.FindBy(d => d.Id == id).FirstOrDefault();
        }

        public ServiceCharge GetByYearAndConcern(int concernId, int year)
        {
            return _serviceChargeRepository.FindBy(d => d.ConcernId == concernId && d.ServiceYear == year).FirstOrDefault();
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

        public void Update(ServiceCharge model)
        {
            _serviceChargeRepository.Update(model);
        }
        public int GetServiceChargeIdByConcernAndYear(int concernId, int year)
        {
            ServiceCharge charge = _serviceChargeRepository.FindBy(d => d.ConcernId == concernId && d.ServiceYear == year).FirstOrDefault();
            return charge != null ? charge.Id : 0;
        }
        
    }
}
