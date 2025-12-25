using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TO;

namespace IMSWEB.Service
{
    public class BankLoanService : IBankLoanService
    {
        private readonly IBaseRepository<BankLoan> _baseRepository;
        private readonly IBaseRepository<Bank> _bankRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BankLoanService(IBaseRepository<BankLoan> baseRepository, IBaseRepository<Bank> bankRepository, IUnitOfWork unitOfWork)
        {
            _baseRepository = baseRepository;
            _bankRepository = bankRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(BankLoan model)
        {
            _baseRepository.Add(model);
        }

        public void Update(BankLoan model)
        {
            _baseRepository.Update(model);
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<BankLoan> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        public async Task<IEnumerable<BankLoanTO>> GetAllAsync(DateTime fromDate, DateTime toDate)
        {
            return await _baseRepository.GetAllBankLoanAsync(_bankRepository, fromDate, toDate);
        }

        public BankLoan GetById(int id)
        {
            return _baseRepository.AllIncluding(d => d.BankLoanDetails).FirstOrDefault(d => d.Id == id);
        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.Id == id);
        }

        public List<IdNameDDLTO> GetForDDL()
        {
            return _baseRepository.All.Select(d => new IdNameDDLTO
            {
                Id = d.Id,
                Name = d.Code + "-" + d.LoanDate.ToString("dd MMM, yyyy") + "(" + d.TotalLoanAmount + ")"
            }).ToList();
        }

        public List<IdNameDDLTO> GetBankLoanByBankId(int bankId)
        {
            return _baseRepository.All
                                    .Where(d => d.BankId == bankId && !d.IsPaid)
                                    .Select(d => new
                                    {
                                        d.Id,
                                        d.Code,
                                        d.LoanDate,
                                        d.TotalLoanAmount
                                    })
                                    .ToList()
                                    .Select(d => new IdNameDDLTO
                                    {
                                        Id = d.Id,
                                        Name = d.Code + "-" + d.LoanDate.ToString("dd MMM, yyyy") + " (" + d.TotalLoanAmount + ")"
                                    })
                                    .ToList();
        }
    }
}
