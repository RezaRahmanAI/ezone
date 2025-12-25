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
    public class BankService : IBankService
    {
        private readonly IBaseRepository<Bank> _bankRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BankService(IBaseRepository<Bank> bankRepository, IUnitOfWork unitOfWork)
        {
            _bankRepository = bankRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddBank(Bank bank)
        {
            _bankRepository.Add(bank);
        }

        public void UpdateBank(Bank bank)
        {
            _bankRepository.Update(bank);
        }

        public void SaveBank()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Bank> GetAllBank()
        {
            return _bankRepository.All.ToList();
        }

        public async Task<IEnumerable<Bank>> GetAllBankAsync()
        {
            return await _bankRepository.GetAllBankAsync();
        }

        public async Task<IEnumerable<Bank>> GetAllBankByParentConcernAsync(int concernId)
        {
            return await _bankRepository.GetAllBankByParentConcernAsync(concernId);
        }

        public Bank GetBankById(int id)
        {
            return _bankRepository.FindBy(x => x.BankID == id).First();
        }

        public void DeleteBank(int id)
        {
            _bankRepository.Delete(x => x.BankID == id);
        }

        public IQueryable<Bank> GetAll(int ConcernID)
        {
            return _bankRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }

        public IEnumerable<IdNameDDLTO> GetAllBankForDDL()
        {
            return _bankRepository.All.Select(s => new IdNameDDLTO
            {
                Id = s.BankID,
                Name = s.BankName + " (" + s.Code + ")"
            }).ToList();
        }
    }
}
