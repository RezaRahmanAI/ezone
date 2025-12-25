using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ManualAttendencService : IManualAttendencService
    {
        private readonly IBaseRepository<ManualAttendence> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public ManualAttendencService(IBaseRepository<ManualAttendence> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(ManualAttendence ManualAttendence)
        {
            _baseRepository.Add(ManualAttendence);
        }

        public void Update(ManualAttendence ManualAttendence)
        {
            _baseRepository.Update(ManualAttendence);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }


        public IQueryable<ManualAttendence> GetAll()
        {
            return _baseRepository.All;
        }
        public ManualAttendence GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ID == id);
        }
    }
}
