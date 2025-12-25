using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TO;

namespace IMSWEB.Data
{
    public static class AllAsyncForIndex
    {
        public static async Task<IEnumerable<BankLoanTO>> GetAllBankLoanAsync(this IBaseRepository<BankLoan> _bankLoanRepository, IBaseRepository<Bank> _bankRepository, DateTime fromDate, DateTime toDate)
        {
            var data = (from bl in _bankLoanRepository.All
                        join b in _bankRepository.All on bl.BankId equals b.BankID
                        where bl.LoanDate >= fromDate && bl.LoanDate <= toDate
                        select new BankLoanTO
                        {
                            Id = bl.Id,
                            BankName = b.BankName,
                            Code = bl.Code,
                            LoanDate = bl.LoanDate,
                            InterestPercentage = bl.InterestPercentage,
                            PrincipleLoanAmount = bl.PrincipleLoanAmount,
                            ProcessingFeePercentage = bl.ProcessingFeePercentage,
                            TotalLoanAmount = bl.TotalLoanAmount,
                            NoOfInstallment = bl.NoOfInstallment
                        }).ToListAsync();
            return await data;
        }

        public static async Task<IEnumerable<BankLoanCollectionTO>> GetAllBankLoanCollectionAsync(this IBaseRepository<BankLoanCollection> _bankLoanCollectoinRepository, IBaseRepository<BankLoan> _bankLoanRepository, IBaseRepository<BankLoanDetails> _bankLoanDetailsRepository, IBaseRepository<Bank> _bankRepository, DateTime fromDate, DateTime toDate)
        {
            var data = await (from bc in _bankLoanCollectoinRepository.All
                              join bld in _bankLoanDetailsRepository.All on bc.Id equals bld.LoanCollectionId
                              join bl in _bankLoanRepository.All on bld.BankLoanId equals bl.Id
                              join b in _bankRepository.All on bl.BankId equals b.BankID
                              where bc.CollectionDate >= fromDate && bc.CollectionDate <= toDate
                              select new BankLoanCollectionTO
                              {
                                  Id = bc.Id,
                                  BankName = b.BankName,
                                  LoanCode = bl.Code,
                                  CollectionAmount = bc.CollectionAmount,
                                  Code = bc.Code,
                                  CollectionDate = bc.CollectionDate,
                                  CollectionType = EnumBankLoanType.Normal.ToString()
                              }).ToListAsync();

            return data;
        }

    }
}
