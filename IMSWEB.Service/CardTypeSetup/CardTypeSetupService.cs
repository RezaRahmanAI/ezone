using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CardTypeSetupService : ICardTypeSetupService
    {
        private readonly IBaseRepository<CardTypeSetup> _baseRepository;
        private readonly IBaseRepository<CardType> _CardTypeRepository;
        private readonly IBaseRepository<Bank> _BankRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CardTypeSetupService(IBaseRepository<CardTypeSetup> baseRepository, IUnitOfWork unitOfWork,
            IBaseRepository<CardType> CardTypeRepository, IBaseRepository<Bank> BankRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _CardTypeRepository = CardTypeRepository;
            _BankRepository = BankRepository;
        }

        public void Add(CardTypeSetup CardTypeSetup)
        {
           _baseRepository.Add(CardTypeSetup);
        }

        public void Update(CardTypeSetup CardTypeSetup)
        {
            _baseRepository.Update(CardTypeSetup);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<CardTypeSetup> GetAll()
        {
            return _baseRepository.All;
        }

        public CardTypeSetup GetById(int id)
        {
            return _baseRepository.FindBy(x => x.CardTypeSetupID == id).FirstOrDefault();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.CardTypeSetupID == id);
        }

        public Task<IEnumerable<Tuple<int, string, decimal, string, string, string>>> GetAllAsync()
        {
            return _baseRepository.GetAllAsync(_BankRepository, _CardTypeRepository);
        }
    }
}
