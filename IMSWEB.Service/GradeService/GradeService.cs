using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class GradeService : IGradeService
    {
        private readonly IBaseRepository<Grade> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;


        public GradeService(IBaseRepository<Grade> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }

        public void Add(Grade Grade)
        {
           _baseRepository.Add(Grade);
        }

        public void Update(Grade Grade)
        {
            _baseRepository.Update(Grade);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<Grade> GetAll()
        {
            return _baseRepository.All.Where(i=>i.Status==(int)EnumActiveInactive.Active).ToList();
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _baseRepository.GetAllGradeAsync();
        }

        public Grade GetById(int id)
        {
            return _baseRepository.FindBy(x => x.GradeID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.GradeID == id);
        }
    }
}
