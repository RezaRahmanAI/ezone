using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using IMSWEB.Service.Infrastructure;

namespace IMSWEB.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IBaseRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.Update(category);
        }

        public void SaveCategory()
        {
            _unitOfWork.Commit();
            LookupCache.RemoveByPrefix("Lookup:Category");
        }

        public IEnumerable<Category> GetAllCategory()
        {
            var concernId = LookupCache.GetConcernId();
            var cacheKey = LookupCache.BuildTenantKey("Lookup:Category:All", concernId);
            return LookupCache.GetOrCreate(cacheKey,
                () => _categoryRepository.All.AsNoTracking().ToList(),
                TimeSpan.FromMinutes(60));
        }

        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            return await _categoryRepository.GetAllCategoryAsync();
        }

        public Category GetCategoryById(int id)
        {
            return _categoryRepository.FindBy(x => x.CategoryID == id).First();
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(x => x.CategoryID == id);
            LookupCache.RemoveByPrefix("Lookup:Category");
        }

        public IQueryable<Category> GetAllIQueryable()
        {
            return _categoryRepository.All;
        }
        public IQueryable<Category> GetAllIQueryable(int ConcernID)
        {
            return _categoryRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }
    }
}
