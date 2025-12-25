using IMSWEB.Data;
using IMSWEB.Data.Repositories;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class TransferHistoryService : ITransferHistoryService
    {
        private readonly IBaseRepository<TransferHistory> _transferHistoryRepository;
        private readonly ITransferHistoryRepository _transferRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TransferHistoryService(IBaseRepository<TransferHistory> transferHistoryRepository, IUnitOfWork unitOfWork, ITransferHistoryRepository transferRepo)
        {
            _transferHistoryRepository = transferHistoryRepository;
            _unitOfWork = unitOfWork;
            _transferRepo = transferRepo;
        }
     
        public void AddTransferHistory(TransferHistory TransferHistory)
        {
            _transferHistoryRepository.Add(TransferHistory);
        }

        public void UpdateTransferHistory(TransferHistory TransferHistory)
        {
            _transferHistoryRepository.Update(TransferHistory);
        }

        public void SaveTransferHistory()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<TransferHistory> GetAllTransferHistory()
        {
           return _transferHistoryRepository.All.ToList();
        }

        public Task<IEnumerable<TransferHistory>> GetAllTransferHistoryAsync()
        {
            throw new NotImplementedException();
        }

        public TransferHistory GetTransferHistoryById(int id)
        {
            return _transferHistoryRepository.FindBy(i => i.TransferHID == id).FirstOrDefault();
        }

        public void DeleteTransferHistory(int id)
        {
            _transferHistoryRepository.Delete(x => x.TransferHID == id);
        }

        public bool AddTransferHistoryUsingSP(DataTable dtTransferHistory)
        {
           return _transferRepo.AddTransferHistoryUsingSP(dtTransferHistory);
        }
    }
}
