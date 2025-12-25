using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IGradeService
    {
        void Add(Grade Grade);
        void Update(Grade Grade);
        void Save();
        IEnumerable<Grade> GetAll();
        Task<IEnumerable<Grade>> GetAllAsync();
        Grade GetById(int id);
        void Delete(int id);
    }
}
