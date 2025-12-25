using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICreditInterestPercentageService
    {
        void AddCreditInterestPercentage(CreditInterestPercentage creditInterestPercentage);
        void UpdateCreditInterestPercentage(CreditInterestPercentage creditInterestPercentage);
        void SaveCreditInterestPercentage();
        IQueryable<CreditInterestPercentage> GetAllCreditInterestPercentage();
        IQueryable<CreditInterestPercentage> GetAllCreditInterestPercentage(int ConcernID);
        Task<IEnumerable<CreditInterestPercentage>> GetAllCreditInterestPercentageAsync();
        CreditInterestPercentage GetCreditInterestPercentageById(int id);
        void DeleteCreditInterestPercentage(int id);
    }
}
