using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.SPModel;

namespace IMSWEB.Service
{
    public interface IAllowanceDeductionService
    {
        void Add(AllowanceDeduction AllowanceDeduction);
        void Update(AllowanceDeduction AllowanceDeduction);
        void Save();
        IEnumerable<AllowanceDeduction> GetAll();
        Task<IEnumerable<AllowanceDeduction>> GetAllAllowacneAsync();
        Task<IEnumerable<AllowanceDeduction>> GetAllDeductionAsync();
        AllowanceDeduction GetById(int id);
        void Delete(int id);
    }
}
