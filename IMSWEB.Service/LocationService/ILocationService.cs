using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ILocationService
    {
        void AddLocation(Location location);
        void UpdateLocation(Location location);
        void SaveLocation();
        IEnumerable<Location> GetAllLocation();
        Task<IEnumerable<Location>> GetAllLocationAsync();
        Location GetLocationById(int id);
        void DeleteLocation(int id);
    }
}
