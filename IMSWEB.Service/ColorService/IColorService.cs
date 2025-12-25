using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IColorService
    {
        void AddColor(Color color);
        void UpdateColor(Color color);
        void SaveColor();
        IEnumerable<Color> GetAllColor();
        IQueryable<Color> GetAll();
        IQueryable<Color> GetAll(int ConcernID);
        Task<IEnumerable<Color>> GetAllColorAsync();
        Color GetColorById(int id);
        void DeleteColor(int id);

        Color GetColorByConcernAndColorName(int ConcernID, string ColorName);
    }
}
