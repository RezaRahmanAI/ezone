using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICardTypeSetupService
    {
        void Add(CardTypeSetup CardTypeSetup);
        void Update(CardTypeSetup CardTypeSetup);
        void Save();
        IEnumerable<CardTypeSetup> GetAll();
        Task<IEnumerable<Tuple<int, string, decimal, string, string, string>>> GetAllAsync();
        CardTypeSetup GetById(int id);
        void Delete(int id);
    }
}
