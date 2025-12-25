using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IDepartmentService
    {
        void AddDepartment(Department Department);
        void UpdateDepartment(Department Department);
        void SaveDepartment();
        IEnumerable<Department> GetAllDepartment();
        IQueryable<Department> GetAllDepartmentIQueryable();
        Task<IEnumerable<Department>> GetAllDepartmentAsync();
        Department GetDepartmentById(int id);
        void DeleteDepartment(int id);
    }
}
