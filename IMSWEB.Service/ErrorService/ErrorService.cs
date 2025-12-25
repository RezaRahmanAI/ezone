using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ErrorService : IErrorService
    {
        private readonly IBaseRepository<Error> _errorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ErrorService(IBaseRepository<Error> errorRepository, IUnitOfWork unitOfWork)
        {
            _errorRepository = errorRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddError(Error error)
        {
            _errorRepository.Add(error);
        }

        public void SaveError()
        {
            _unitOfWork.Commit();
        }
    }
}
