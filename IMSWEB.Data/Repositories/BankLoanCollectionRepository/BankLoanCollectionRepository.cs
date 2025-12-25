using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories
{
    public class BankLoanCollectionRepository : IBankLoanCollectionRepository
    {

        public bool DelecteBankLoanCollectionUsingSP(int collectionId, EnumBankLoanType collectionType)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                if (collectionType == EnumBankLoanType.Normal)
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteLoanCollection", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@BankLoanCollectionId", SqlDbType.Int).Value = collectionId;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;

                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteCCLoanCollection", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@BankLoanCollectionId", SqlDbType.Int).Value = collectionId;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;

                    }
                }
                
            }
            return Result;
        }

        public List<RPTBankDueLoanTO> GetAllPendingLoanAsOnDate(IBaseRepository<BankLoanDetails> _bankLoanDetailsRepository, IBaseRepository<BankLoan> _bankLoanRepository, IBaseRepository<Bank> _bankRepository, DateTime currentDate)
        {
            var data = (from bld in _bankLoanDetailsRepository.All
                        join bl in _bankLoanRepository.All on bld.BankLoanId equals bl.Id
                        join b in _bankRepository.All on bl.BankId equals b.BankID
                        where bld.Status.Equals("Due") && DbFunctions.TruncateTime(bld.InstallmentDate) <= DbFunctions.TruncateTime(currentDate)
                        select new RPTBankDueLoanTO
                        {
                            BankName = b.BankName,
                            InstallmentDate = bld.InstallmentDate,
                            ScheduleNo = bld.ScheduleNo,
                            Status = bld.Status,
                            LoanCode = bl.Code,
                            InstallmentAmount = bld.ExpectedInstallmentAmount
                        }).ToList();

            return data;

        }

    }
}
