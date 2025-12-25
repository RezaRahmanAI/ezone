using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class ColorService : IColorService
    {
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ColorService(IBaseRepository<Color> colorRepository, IUnitOfWork unitOfWork)
        {
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddColor(Color color)
        {
            _colorRepository.Add(color);
        }

        public void UpdateColor(Color color)
        {
            _colorRepository.Update(color);
        }

        public void SaveColor()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Color> GetAllColor()
        {
            return _colorRepository.All.ToList();
        }

        public async Task<IEnumerable<Color>> GetAllColorAsync()
        {
            return await _colorRepository.GetAllColorAsync();
        }

        public Color GetColorById(int id)
        {
            return _colorRepository.FindBy(x=>x.ColorID == id).First();
        }

        public void DeleteColor(int id)
        {
            _colorRepository.Delete(x => x.ColorID == id);
        }


        public Color GetColorByConcernAndColorName(int ConcernID, string ColorName)
        {
            return _colorRepository.GetAll().FirstOrDefault(i => i.ConcernID == ConcernID && i.Name.ToLower().Equals(ColorName));
        }

        public IQueryable<Color> GetAll()
        {
            return _colorRepository.All;
        }

        public IQueryable<Color> GetAll(int ConcernID)
        {
            return _colorRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }
    }
}
