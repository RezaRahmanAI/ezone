using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SRVisitDetailService : ISRVisitDetailService
    {
        private readonly IBaseRepository<SRVisitDetail> _SRVisitDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SRVisitDetailService(IBaseRepository<SRVisitDetail> sRVisitDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IUnitOfWork unitOfWork)
        {
            _SRVisitDetailRepository = sRVisitDetailRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddSRVisitDetail(SRVisitDetail sRVisitDetail)
        {
            _SRVisitDetailRepository.Add(sRVisitDetail);
        }

        public void SaveSRVisitDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Tuple<int, int, int, decimal, string, string, int, Tuple<string>>>
            GetSRVisitDetailById(int id)
        {
            return _SRVisitDetailRepository.GetSRVisitDetailById(_productRepository, _colorRepository, id);
        }

        public void DeleteSRVisitDetail(int id)
        {
            _SRVisitDetailRepository.Delete(x => x.SRVisitID == id);
        }
    }
}
