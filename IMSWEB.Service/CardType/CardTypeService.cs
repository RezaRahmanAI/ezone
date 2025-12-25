using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CardTypeService : ICardTypeService
    {
        private readonly IBaseRepository<CardType> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CardTypeService(IBaseRepository<CardType> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(CardType CardType)
        {
           _baseRepository.Add(CardType);
        }

        public void Update(CardType CardType)
        {
            _baseRepository.Update(CardType);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<CardType> GetAllActive()
        {
            return _baseRepository.All.Where(i=>i.Status==(int)EnumActiveInactive.Active).ToList();
        }
        public IQueryable<CardType> GetAll()
        {
            return _baseRepository.All;
        }
        public CardType GetById(int id)
        {
            return _baseRepository.FindBy(x => x.CardTypeID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.CardTypeID == id);
        }
    }
}
