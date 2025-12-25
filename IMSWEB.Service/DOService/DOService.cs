using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class DOService : IDOService
    {
        private readonly IBaseRepository<DO> _baseRepository;
        private readonly IBaseRepository<DODetail> _DODetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDORepository _dORepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        public DOService(IBaseRepository<DO> baseRepository, IUnitOfWork unitOfWork, IDORepository dORepository,
            IBaseRepository<DODetail> DODetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<Company> companyRepository, IBaseRepository<Category> categoryRepository, IBaseRepository<Supplier> supplierRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _dORepository = dORepository;
            _DODetailRepository = DODetailRepository;
            _productRepository = productRepository;
            _companyRepository = companyRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        public Tuple<bool, int> ADDDOEntry(DO DO, int DOID)
        {
            return _dORepository.ADDDOEntry(DO, DOID);
        }

        public void Update(DO DO)
        {
            _baseRepository.Update(DO);
        }

        public void UpdateDoDetails(DODetail Model)
        {
            _DODetailRepository.Update(Model);
        }


        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IEnumerable<DO> GetAll(EnumDOStatus enumDOStatus)
        {
            return _baseRepository.All.Where(i => i.Status == enumDOStatus).ToList();
        }
        public DO GetById(int id)
        {
            return _baseRepository.FindBy(x => x.DOID == id).First();
        }


        public DODetail GetDetailById(int id)
        {
            return _DODetailRepository.FindBy(x => x.DODID == id).First();
        }
        public bool Delete(int id, int userID, DateTime dateTime)
        {
            return _dORepository.DeleteByID(id, userID, dateTime);
        }

        public List<DO> GetAll(EnumDOStatus enumDOStatus, DateTime fromDate, DateTime toDate, bool IsSalesDO, int concernID)
        {
            IEnumerable<DO> dos = null;
            if (IsSalesDO)
                dos = _baseRepository.AllIncluding(i => i.Customer).Where(i => i.Status == enumDOStatus && (i.Date >= fromDate && i.Date <= toDate) && i.CustomerID > 0 && i.ConcernID == concernID);
            else
                dos = _baseRepository.AllIncluding(i => i.Supplier).Where(i => i.Status == enumDOStatus && (i.Date >= fromDate && i.Date <= toDate) && i.SupplierID > 0 && i.ConcernID == concernID);

            return dos.OrderByDescending(i => i.Date).ThenByDescending(i => i.DONo).ToList();
        }

        public List<DODetail> GetDetailsById(int id)
        {
            return _DODetailRepository.All.Where(i => i.DOID == id).ToList();
        }

        public IQueryable<DO> GetAll()
        {
            return _baseRepository.All;
        }

        public IQueryable<DODetail> GetAllDetail()
        {
            return _DODetailRepository.All;
        }

        public List<ProductWisePurchaseModel> ProductWisePurchaseDOReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.ProductWisePurchaseDOReport(_DODetailRepository, _productRepository, _companyRepository, _categoryRepository, _supplierRepository, CompanyID, CategoryID, ProductID, fromDate, toDate);
        }
    }
}
