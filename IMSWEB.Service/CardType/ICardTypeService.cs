using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICardTypeService
    {
        void Add(CardType CardType);
        void Update(CardType CardType);
        void Save();
        IEnumerable<CardType> GetAllActive();
        IQueryable<CardType> GetAll();
        CardType GetById(int id);
        void Delete(int id);
    }
}
