using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class TransferService : ITransferService
    {
        private readonly IBaseRepository<Transfer> _baseRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferRepository _TransferRepository;
        private readonly IBaseRepository<TransferDetail> _TransferDetailRepository;
        private readonly IBaseRepository<Product> _ProductRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Color> _ColorRepository;
        private readonly IBaseRepository<Godown> _GodownRepository;
        private readonly IBaseRepository<StockDetail> _StockDetailRepository;

        public TransferService(IBaseRepository<Transfer> baseRepository,
            IUnitOfWork unitOfWork, IBaseRepository<SisterConcern> SisterConcernRepository, ITransferRepository TransferRepository,IBaseRepository<TransferDetail> TransferDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Color> ColorRepository, IBaseRepository<Godown> GodownRepository, IBaseRepository<StockDetail> StockDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _TransferRepository = TransferRepository;
            _ProductRepository = ProductRepository;
            _CategoryRepository = CategoryRepository;
            _CompanyRepository = CompanyRepository;
            _ColorRepository = ColorRepository;
            _GodownRepository = GodownRepository;
            _TransferDetailRepository = TransferDetailRepository;
            _StockDetailRepository = StockDetailRepository;
        }

        public void Add(Transfer Transfer)
        {
            _baseRepository.Add(Transfer);
        }

        public void Update(Transfer Transfer)
        {
            _baseRepository.Update(Transfer);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<Transfer> GetAll()
        {
            return _baseRepository.All;
        }
        public IQueryable<Transfer> GetAll(DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.All.Where(i => i.TransferDate >= fromDate && i.TransferDate <= toDate);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>>>
            GetAllAsync(DateTime fromDate, DateTime toDate, int ConcernID, int page, int pageSize)
        {
            return await _baseRepository.GetAllAsync(_SisterConcernRepository, fromDate, toDate, ConcernID, page, pageSize);
        }

        public Transfer GetById(int id)
        {
            return _baseRepository.FindBy(x => x.TransferID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.TransferID == id);
        }

        public Tuple<bool, int> AddTranserferUsingSP(DataTable dtTransfer, DataTable dtDetails,DataTable dtTransferFromStock, DataTable dtTransferToStock)
        {
            return _TransferRepository.AddTransferUsingSP(dtTransfer, dtDetails, dtTransferFromStock, dtTransferToStock);
        }

        public IEnumerable<ProductWisePurchaseModel> GetDetailsByID(int TransferID)
        {
            return  _baseRepository.GetDetailsByID(_TransferDetailRepository, _ProductRepository, _CategoryRepository, _CompanyRepository, _ColorRepository, _GodownRepository, TransferID);
        }


        public bool ReturnTranserferUsingSP(int TransferID)
        {
            return _TransferRepository.ReturnTranserferUsingSP(TransferID);
        }

        public IEnumerable<ProductWisePurchaseModel> GetTransferReport(DateTime FromDate, DateTime ToDate,int ConcernID)
        {
            return _baseRepository.GetTransferReport(_TransferDetailRepository, _ProductRepository, _CategoryRepository, _CompanyRepository, _ColorRepository, _GodownRepository,_SisterConcernRepository, _StockDetailRepository, FromDate, ToDate,ConcernID);
        }



        //public bool CheckTransProductStatusByTransDId(int id)
        //{
        //    return _TransferRepository.CheckTransProductStatusByTransDId(id);
        //}

        public int CheckTransProductStatusByTransDId(int id)
        {
            return _TransferRepository.CheckTransProductStatusByTransDId(id);
        }

        public int CheckTransProductHireSalesStatusByTransDId(int id)
        {
            return _TransferRepository.CheckTransProductHireSalesStatusByTransDId(id);
        }
        public IEnumerable<ProductWisePurchaseModel> GetTransferReportFromTo(DateTime FromDate, DateTime ToDate, int FromConern, int ToConcern)
        {
            return _baseRepository.GetTransferReportFromTo(_TransferDetailRepository, _ProductRepository, _CategoryRepository, _CompanyRepository, _ColorRepository, _GodownRepository, _SisterConcernRepository, _StockDetailRepository, FromDate, ToDate, FromConern, ToConcern);
        }
    }
}
