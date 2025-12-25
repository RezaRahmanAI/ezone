using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class InvestmentExtension
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>>
            GetAllAsync(this IBaseRepository<ShareInvestment> ShareInvestmentRepository,
            IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository,
            EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            IQueryable<ShareInvestment> ShareInvestments = null;
            if (InvestTransType != 0)
                ShareInvestments = ShareInvestmentRepository.All.Where(i => (int)i.TransactionType == InvestTransType);
            else
                ShareInvestments = ShareInvestmentRepository.All;

            var Result = await (from si in ShareInvestments
                                join h in ShareInvestmentHeadRepository.All on si.SIHID equals h.SIHID
                                where si.EntryDate >= fromDate && si.EntryDate <= toDate
                                && h.ParentId == (int)investmentType
                                select new
                                {
                                    si.SIID,
                                    si.EntryDate,
                                    si.Purpose,
                                    si.Amount,
                                    h.Name
                                }).OrderByDescending(i => i.EntryDate).ToListAsync();

            return Result.Select(x => new Tuple<int, DateTime, string, string, decimal>(
                        x.SIID,
                        x.EntryDate,
                        x.Name,
                        x.Purpose,
                        x.Amount
                          ));
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>>
            GetAllLiabilityAsync(this IBaseRepository<ShareInvestment> ShareInvestmentRepository,
            IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository,
            EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            IQueryable<ShareInvestment> ShareInvestments = null;
            if (InvestTransType != 0)
                ShareInvestments = ShareInvestmentRepository.All.Where(i => (int)i.TransactionType == InvestTransType);
            else
                ShareInvestments = ShareInvestmentRepository.All;

            var Result = await (from si in ShareInvestments
                                join h in ShareInvestmentHeadRepository.All on si.SIHID equals h.SIHID
                                where si.EntryDate >= fromDate && si.EntryDate <= toDate
                                && (h.ParentId == (int)EnumInvestmentType.Liability || h.ParentId == (int)EnumInvestmentType.FDR || h.ParentId == (int)EnumInvestmentType.PF || h.ParentId == (int)EnumInvestmentType.Security)
                                select new
                                {
                                    si.SIID,
                                    si.EntryDate,
                                    si.Purpose,
                                    si.Amount,
                                    h.Name
                                }).OrderByDescending(i => i.EntryDate).ToListAsync();

            return Result.Select(x => new Tuple<int, DateTime, string, string, decimal>(
                        x.SIID,
                        x.EntryDate,
                        x.Name,
                        x.Purpose,
                        x.Amount
                          ));
        }

        public static List<VoucherTransactionReportModel> VoucherTransactionLedgerData(this IBaseRepository<ShareInvestment> _voucherTransactionRepository,
        IBaseRepository<Customer> _CustomerRepository, IBaseRepository<ShareInvestmentHead> _ShareInvestmentHeadRepository, IBaseRepository<BankTransaction> _bankTransRepository, DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        {
            List<VoucherTransactionReportModel> ledgers = new List<VoucherTransactionReportModel>();
            List<VoucherTransactionReportModel> FinalLedgers = new List<VoucherTransactionReportModel>();

            #region InvestmentID

            var InvestOpeningData = (/*from VT in _voucherTransactionRepository.All*/
                                     from exp in _ShareInvestmentHeadRepository.All /*on VT.InvestmentHeadId equals exp.SIHID*/
                                     where exp.SIHID == ExpenseItemID
                                     select new VoucherTransactionReportModel
                                     {
                                         Opening = exp.OpeningBalance,
                                         ModuleType = "Opening",
                                         ItemName = exp.Name,
                                         VoucherDate = exp.OpeningDate,
                                         Narration = "Opening Balance"

                                     }).ToList();
            ledgers.AddRange(InvestOpeningData);

            var InvestPaymentVoucherData = (from VT in _voucherTransactionRepository.All
                                            join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                            where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
                                             VT.TransactionType == EnumInvestTransType.Pay && VT.SIHID == ExpenseItemID
                                            select new VoucherTransactionReportModel
                                            {
                                                VoucherNo = VT.SIID.ToString(),
                                                VoucherDate = VT.EntryDate,
                                                DebitAmount = 0m,
                                                CreditAmount = VT.Amount,
                                                ModuleType = "Payment Voucher",
                                                Narration = VT.Purpose,
                                                ItemName = exp.Name

                                            }).ToList();
            ledgers.AddRange(InvestPaymentVoucherData);

            var BankInvestPaymentVoucherData = (from VT in _bankTransRepository.All
                                                join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                                where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
                                                 VT.TransactionType == (int)EnumTransactionType.LiaPay && VT.SIHID == ExpenseItemID
                                                select new VoucherTransactionReportModel
                                                {
                                                    VoucherNo = VT.TransactionNo,
                                                    VoucherDate = (DateTime)VT.TranDate,
                                                    DebitAmount = 0m,
                                                    CreditAmount = VT.Amount,
                                                    ModuleType = "Payment Voucher Bank",
                                                    Narration = VT.Remarks,
                                                    ItemName = exp.Name

                                                }).ToList();
            ledgers.AddRange(BankInvestPaymentVoucherData);

            var InvestRecieptVoucherData = (from VT in _voucherTransactionRepository.All
                                            join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                            where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
                                             VT.TransactionType == EnumInvestTransType.Receive && VT.SIHID == ExpenseItemID
                                            select new VoucherTransactionReportModel
                                            {
                                                VoucherNo = VT.SIID.ToString(),
                                                VoucherDate = VT.EntryDate,
                                                DebitAmount = VT.Amount,
                                                CreditAmount = 0m,
                                                ModuleType = "Reciept Voucher",
                                                Narration = VT.Purpose,
                                                ItemName = exp.Name

                                            }).ToList();
            ledgers.AddRange(InvestRecieptVoucherData);

            var BankInvestRecieptVoucherData = (from VT in _bankTransRepository.All
                                            join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                            where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
                                             VT.TransactionType == (int)EnumTransactionType.LiaRec && VT.SIHID == ExpenseItemID
                                            select new VoucherTransactionReportModel
                                            {
                                                VoucherNo = VT.TransactionNo,
                                                VoucherDate = (DateTime)VT.TranDate,
                                                DebitAmount = VT.Amount,
                                                CreditAmount = 0m,
                                                ModuleType = "Reciept Voucher Bank",
                                                Narration = VT.Remarks,
                                                ItemName = exp.Name

                                            }).ToList();
            ledgers.AddRange(BankInvestRecieptVoucherData);

            #endregion


            decimal openingdue = ledgers.Select(i => i.Opening).FirstOrDefault();

            decimal balance = openingdue;
            ledgers = ledgers.OrderBy(i => i.VoucherDate).ToList();

            foreach (var item in ledgers)
            {
                //decimal balance = item.Opening;
                if (headType.ToLower().Equals("s"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else if (headType.ToLower().Equals("in"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else if (headType.ToLower().Equals("sc"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else
                {
                    item.Balance = balance + (item.DebitAmount - item.CreditAmount);
                }

                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.Narration) : item.Particulars;
                balance = item.Balance;
            }



            if (ledgers.Count > 0)
            {
                if (headType.ToLower().Equals("s"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else if (headType.ToLower().Equals("IN"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else if (headType.ToLower().Equals("sc"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, DebitAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, DebitAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }

            }
            else
            {
                if (headType.ToLower().Equals("s"))
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
                }
                else if (headType.ToLower().Equals("IN"))
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
                }
                else
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", DebitAmount = openingdue, CreditAmount = 0m, Balance = openingdue });
                }

            }

            return FinalLedgers;
        }

    }
}
