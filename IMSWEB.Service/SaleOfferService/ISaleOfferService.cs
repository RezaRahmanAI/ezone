using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISaleOfferService
    {
        void AddSaleOffer(SaleOffer saleOffer);
        void UpdateSaleOffer(SaleOffer saleOffer);
        void SaveSaleOffer();
        IEnumerable<SaleOffer> GetAllSaleOffer();
        Task<IEnumerable<SaleOffer>> GetAllSaleOfferAsync();

        Task<IEnumerable<Tuple<int, string, string,
        DateTime, DateTime, string, string,Tuple<string,string,string>>>> GetAllSOfferAsync();


        SaleOffer GetSaleOfferById(int id);
        void DeleteSaleOffer(int id);
    }
}
