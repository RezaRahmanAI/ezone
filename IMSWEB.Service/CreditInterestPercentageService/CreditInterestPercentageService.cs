using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CreditInterestPercentageService : ICreditInterestPercentageService
    {
        private readonly IBaseRepository<CreditInterestPercentage> _CreditInterestPercentageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreditInterestPercentageService(IBaseRepository<CreditInterestPercentage> CreditInterestPercentageRepository, IUnitOfWork unitOfWork)
        {
            _CreditInterestPercentageRepository = CreditInterestPercentageRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddCreditInterestPercentage(CreditInterestPercentage creditInterestPercentage)
        {
            _CreditInterestPercentageRepository.Add(creditInterestPercentage);
        }

        public void UpdateCreditInterestPercentage(CreditInterestPercentage creditInterestPercentage)
        {
            _CreditInterestPercentageRepository.Update(creditInterestPercentage);
        }

        public void SaveCreditInterestPercentage()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<CreditInterestPercentage> GetAllCreditInterestPercentage()
        {
            return _CreditInterestPercentageRepository.All;
        }
        public IQueryable<CreditInterestPercentage> GetAllCreditInterestPercentage(int ConcernID)
        {
            return _CreditInterestPercentageRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }

        public async Task<IEnumerable<CreditInterestPercentage>> GetAllCreditInterestPercentageAsync()
        {
            return await _CreditInterestPercentageRepository.GetAllCreditInterestPercentageAsync();
        }

        public CreditInterestPercentage GetCreditInterestPercentageById(int id)
        {
            return _CreditInterestPercentageRepository.FindBy(x => x.IntPercentageID == id).First();
        }


        public void DeleteCreditInterestPercentage(int id)
        {
            _CreditInterestPercentageRepository.Delete(x => x.IntPercentageID == id);
        }
    }
}
