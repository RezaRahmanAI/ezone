using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SaleOfferService : ISaleOfferService
    {
        private readonly IBaseRepository<SaleOffer> _SaleOfferRepository;
        private readonly IBaseRepository<Product> _ProductRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SaleOfferService(IBaseRepository<SaleOffer> saleOfferRepository, IBaseRepository<Product> productRepository,IUnitOfWork unitOfWork)
        {
            _SaleOfferRepository = saleOfferRepository;
            _ProductRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddSaleOffer(SaleOffer saleOffer)
        {
            _SaleOfferRepository.Add(saleOffer);
        }

        public void UpdateSaleOffer(SaleOffer saleOffer)
        {
            _SaleOfferRepository.Update(saleOffer);
        }

        public void SaveSaleOffer()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<SaleOffer> GetAllSaleOffer()
        {
            return _SaleOfferRepository.All.ToList();
        }

        public async Task<IEnumerable<SaleOffer>> GetAllSaleOfferAsync()
        {
            return await _SaleOfferRepository.GetAllSaleOfferAsync();
        }

        public async Task<IEnumerable<Tuple<int, string, string, DateTime,
        DateTime, string, string, Tuple<string, string, string>>>> GetAllSOfferAsync()
        {
            return await _SaleOfferRepository.GetAllSOfferAsync(_ProductRepository);
        }

        public SaleOffer GetSaleOfferById(int id)
        {
            return _SaleOfferRepository.FindBy(x => x.OfferID == id).First();
        }

        public void DeleteSaleOffer(int id)
        {
            _SaleOfferRepository.Delete(x => x.OfferID == id);
        }

    }
}
