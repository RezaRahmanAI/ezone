using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class DesignationService : IDesignationService
    {
        private readonly IBaseRepository<Designation> _designationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DesignationService(IBaseRepository<Designation> designationRepository, IUnitOfWork unitOfWork)
        {
            _designationRepository = designationRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddDesignation(Designation Designation)
        {
            _designationRepository.Add(Designation);
        }

        public void UpdateDesignation(Designation Designation)
        {
            _designationRepository.Update(Designation);
        }

        public void SaveDesignation()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Designation> GetAllDesignation()
        {
            return _designationRepository.All.ToList();
        }
        public  IQueryable<Designation> GetAllIQueryable()
        {
            return _designationRepository.All;
        }
        public async Task<IEnumerable<Designation>> GetAllDesignationAsync()
        {
            return await _designationRepository.GetAllDesignationAsync();
        }

        public Designation GetDesignationById(int id)
        {
            return _designationRepository.FindBy(x=>x.DesignationID == id).First();
        }

        public void DeleteDesignation(int id)
        {
            _designationRepository.Delete(x => x.DesignationID == id);
        }
    }
}
