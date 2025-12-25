using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ReligionService : IReligionService
    {
        private readonly IBaseRepository<Religion> _religionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReligionService(IBaseRepository<Religion> religionRepository, IUnitOfWork unitOfWork)
        {
            _religionRepository = religionRepository;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Religion> GetAll()
        {
            return _religionRepository.All.ToList();
        }
    }
}
