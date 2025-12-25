using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public class DepartmentService:IDepartmentService
    {
        private readonly IBaseRepository<Department> _DepartmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IBaseRepository<Department> DepartmentRepository, IUnitOfWork unitOfWork)
        {
            _DepartmentRepository = DepartmentRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddDepartment(Department Department)
        {
            _DepartmentRepository.Add(Department);
        }

        public void UpdateDepartment(Department Department)
        {
            _DepartmentRepository.Update(Department);
        }

        public void SaveDepartment()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Department> GetAllDepartment()
        {
            return _DepartmentRepository.All.ToList();
        }
        public IQueryable<Department> GetAllDepartmentIQueryable()
        {
            return _DepartmentRepository.All;
        }
        public async Task<IEnumerable<Department>> GetAllDepartmentAsync()
        {
            return await _DepartmentRepository.GetAllDepartmentAsync();
        }

        public Department GetDepartmentById(int id)
        {
            return _DepartmentRepository.FindBy(x=>x.DepartmentId == id).First();
        }

        public void DeleteDepartment(int id)
        {
            _DepartmentRepository.Delete(x => x.DepartmentId == id);
        }
    }
}
