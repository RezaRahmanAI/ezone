using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class MonthlyAttendenceService : IMonthlyAttendenceService
    {
        private readonly IBaseRepository<MonthlyAttendence> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public MonthlyAttendenceService(IBaseRepository<MonthlyAttendence> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(MonthlyAttendence MonthlyAttendence)
        {
            _baseRepository.Add(MonthlyAttendence);
        }

        public void Update(MonthlyAttendence MonthlyAttendence)
        {
            _baseRepository.Update(MonthlyAttendence);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }


        public IQueryable<MonthlyAttendence> GetAll()
        {
            return _baseRepository.All;
        }
        public MonthlyAttendence GetById(int id)
        {
            return _baseRepository.FindBy(x => x.MAID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.MAID == id);
        }
    }
}
