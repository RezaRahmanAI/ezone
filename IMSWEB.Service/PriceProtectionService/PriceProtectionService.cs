using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class PriceProtectionService : IPriceProtectionService
    {
        private readonly IBaseRepository<PriceProtection> _PriceProtectionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PriceProtectionService(IBaseRepository<PriceProtection> priceProtectionRepository, IUnitOfWork unitOfWork)
        {
            _PriceProtectionRepository = priceProtectionRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddPriceProtection(PriceProtection pProtection)
        {
            _PriceProtectionRepository.Add(pProtection);
        }

        public void UpdatePriceProtection(PriceProtection pProtection)
        {
            _PriceProtectionRepository.Update(pProtection);
        }

        public void SavePriceProtection()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<PriceProtection> GetAllPriceProtection()
        {
            return _PriceProtectionRepository.All.ToList();
        }
    }
}
