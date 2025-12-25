using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public interface ITransferHistoryService
    {
       void AddTransferHistory(TransferHistory TransferHistory);
       void UpdateTransferHistory(TransferHistory TransferHistory);
       void SaveTransferHistory();
       IEnumerable<TransferHistory> GetAllTransferHistory();
       Task<IEnumerable<TransferHistory>> GetAllTransferHistoryAsync();
       TransferHistory GetTransferHistoryById(int id);
       void DeleteTransferHistory(int id);
       bool AddTransferHistoryUsingSP(DataTable dtTransferHistory);
    }
}
