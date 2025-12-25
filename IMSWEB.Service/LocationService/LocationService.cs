using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class LocationService : ILocationService
    {
        private readonly IBaseRepository<Location> _LocationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IBaseRepository<Location> locationRepository, IUnitOfWork unitOfWork)
        {
            _LocationRepository = locationRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddLocation(Location location)
        {
            _LocationRepository.Add(location);
        }

        public void UpdateLocation(Location location)
        {
            _LocationRepository.Update(location);
        }

        public void SaveLocation()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Location> GetAllLocation()
        {
            return _LocationRepository.All.ToList();
        }

        public async Task<IEnumerable<Location>> GetAllLocationAsync()
        {
            return await _LocationRepository.GetAllLocationAsync();
        }

        public Location GetLocationById(int id)
        {
            return _LocationRepository.FindBy(x => x.LocationID == id).First();
        }

        public void DeleteLocation(int id)
        {
            _LocationRepository.Delete(x => x.LocationID == id);
        }
    }
}
