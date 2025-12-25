using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICompanyService
    {
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        void SaveCompany();
        IQueryable<Company> GetAllCompany();
        IQueryable<Company> GetAllCompany(int ConcernID);
        Task<IEnumerable<Company>> GetAllCompanyAsync();
        Company GetCompanyById(int id);
        Company GetCompanyByName(string name);
        void DeleteCompany(int id);
    }
}
