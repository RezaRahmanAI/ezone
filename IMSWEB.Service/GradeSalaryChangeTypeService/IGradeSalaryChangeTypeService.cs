using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public interface IGradeSalaryChangeTypeService
    {
        void Add(GradeSalaryChangeType GradeSalaryChangeType);
        void Update(GradeSalaryChangeType GradeSalaryChangeType);
        void Save();
        IEnumerable<GradeSalaryChangeType> GetAll();
        //Task<IEnumerable<GradeSalaryChangeTypeService>> GetAllAsync();
        GradeSalaryChangeType GetById(int id);
        void Delete(int id);
    }
}
