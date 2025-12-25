using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class GodownService : IGodownService
    {
        private readonly IBaseRepository<Godown> _godownRepository;

        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<TransferHistory> _transferHistoryRepository;
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        //private readonly IBaseRepository<PModel> _modelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GodownService(IBaseRepository<Godown> godownRepository, IUnitOfWork unitOfWork,
            IBaseRepository<Product> productRepository, IBaseRepository<TransferHistory> transferHistoryRepository,
       
          IBaseRepository<Company> companyRepository, IBaseRepository<Category> categoryRepository
            )
        {
            _godownRepository = godownRepository;
            _unitOfWork = unitOfWork;

            _godownRepository = godownRepository;
            _productRepository = productRepository;
            _transferHistoryRepository = transferHistoryRepository;
            _unitOfWork = unitOfWork;
            _companyRepository = companyRepository;
            _categoryRepository = categoryRepository;
     
        }

        public void AddGodown(Godown comapany)
        {
            _godownRepository.Add(comapany);
        }

        public void UpdateGodown(Godown comapany)
        {
            _godownRepository.Update(comapany);
        }

        public void SaveGodown()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Godown> GetAllGodown()
        {
            return _godownRepository.All.ToList();
        }
        public IEnumerable<Godown> GetAllConcernGodown()
        {
            return _godownRepository.GetAll().ToList();
        }
        public IEnumerable<Godown> GetAllGodownByConcern(int ConcernID)
        {
            return _godownRepository.GetAll().Where(i=>i.ConcernID==ConcernID).ToList();
        }
        public async Task<IEnumerable<Godown>> GetAllGodownAsync()
        {
            return await _godownRepository.GetAllGodownAsync();
        }

        public Godown GetGodownById(int id)
        {
            return _godownRepository.FindBy(x=>x.GodownID == id).First();
        }

        public Godown GetGodownByName(string name)
        {
            return _godownRepository.FindBy(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
        }
        public async Task<IEnumerable<Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>>> GetAllTransferHistoryAsync()
        {
            return await _godownRepository.GetAllTransferHistoryAsync(_productRepository, _transferHistoryRepository, _companyRepository, _categoryRepository);
        }


   
        public void DeleteGodown(int id)
        {
            _godownRepository.Delete(x => x.GodownID == id);
        }
    }
}
