using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IDesignationService
    {
        void AddDesignation(Designation Designation);
        void UpdateDesignation(Designation Designation);
        void SaveDesignation();
        IEnumerable<Designation> GetAllDesignation();
        IQueryable<Designation> GetAllIQueryable();
        Task<IEnumerable<Designation>> GetAllDesignationAsync();
        Designation GetDesignationById(int id);
        void DeleteDesignation(int id);
    }
}
