using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ParentCategoryService : IParentCategoryService
    {
        private readonly IBaseRepository<ParentCategory> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ParentCategoryService(IBaseRepository<ParentCategory> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(ParentCategory ParentCategory)
        {
            _baseRepository.Add(ParentCategory);
        }

        public void Update(ParentCategory ParentCategory)
        {
            _baseRepository.Update(ParentCategory);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<ParentCategory> GetAll()
        {
            return _baseRepository.All;
        }

        //public async Task<IEnumerable<ParentCategory>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllGradeAsync();
        //}

        public ParentCategory GetById(int id)
        {
            return _baseRepository.FindBy(x => x.PCategoryID == id).First();
        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.PCategoryID == id);
        }

        public IEnumerable<ParentCategory> GetParentCategories()
        {
            return _baseRepository.All.ToList();
        }

    }
}
