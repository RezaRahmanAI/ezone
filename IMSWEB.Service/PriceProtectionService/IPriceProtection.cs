using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IPriceProtectionService
    {
        void AddPriceProtection(PriceProtection priceProtection);
        void UpdatePriceProtection(PriceProtection priceProtection);
        void SavePriceProtection();
        IEnumerable<PriceProtection> GetAllPriceProtection();
       
    }
}
