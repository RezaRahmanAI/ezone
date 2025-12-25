using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class HolidayCalenderService : IHolidayCalenderService
    {
        private readonly IBaseRepository<HolidayCalender> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public HolidayCalenderService(IBaseRepository<HolidayCalender> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(HolidayCalender HolidayCalender)
        {
            _baseRepository.Add(HolidayCalender);
        }

        public void Update(HolidayCalender HolidayCalender)
        {
            _baseRepository.Update(HolidayCalender);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<HolidayCalender> GetAllIQueryable()
        {
            return _baseRepository.All.Where(i => i.Status == (int)EnumActiveInactive.Active);
        }
        public async Task<IEnumerable<HolidayCalender>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync();
        }
        public HolidayCalender GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ID == id).First();
        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ID == id);
        }
    }
}
