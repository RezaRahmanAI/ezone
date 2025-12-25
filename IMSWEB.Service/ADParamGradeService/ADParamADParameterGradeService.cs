using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ADParamADParameterGradeService : IADParamADParameterGradeService
    {
        private readonly IBaseRepository<ADParameterGrade> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Grade> _GradeRepository;

        public ADParamADParameterGradeService(IBaseRepository<ADParameterGrade> baseRepository, IUnitOfWork unitOfWork,IBaseRepository<Grade> GradeRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _GradeRepository = GradeRepository;
        }

        public void Add(ADParameterGrade ADParameterGrade)
        {
            _baseRepository.Add(ADParameterGrade);
        }

        public void Update(ADParameterGrade ADParameterGrade)
        {
            _baseRepository.Update(ADParameterGrade);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IEnumerable<ADParameterGrade> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        //public async Task<IEnumerable<ADParameterGrade>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllGradeAsync();
        //}

        public ADParameterGrade GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ADParameterID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ADParameterID == id);
        }
        public List<Grade> GetGradesByADParamID(int ADParamID)
        {
            return _baseRepository.GetGradesByADParameterID(_GradeRepository,ADParamID);
        }
    }
}
