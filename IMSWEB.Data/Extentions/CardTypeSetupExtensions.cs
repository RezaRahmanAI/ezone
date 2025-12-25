using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class CardTypeSetupExtensions
    {

        public static async Task<IEnumerable<Tuple<int, string, decimal, string, string, string>>>
           GetAllAsync(this IBaseRepository<CardTypeSetup> CardTypeSetupRepository,
            IBaseRepository<Bank> BankRepository, IBaseRepository<CardType> CardTypeRepository)
        {
            var result = await (from cts in CardTypeSetupRepository.All
                                join b in BankRepository.All on cts.BankID equals b.BankID
                                join ct in CardTypeRepository.All on cts.CardTypeID equals ct.CardTypeID
                                select new
                                {
                                    cts.CardTypeSetupID,
                                    cts.Code,
                                    cts.Percentage,
                                    b.BankName,
                                    b.AccountNo,
                                    ct.Description
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, string, decimal, string, string, string>(
                x.CardTypeSetupID,
                x.Code,
                x.Percentage,
                x.BankName,
                x.AccountNo,
                x.Description
                ));
        }
    }
}
