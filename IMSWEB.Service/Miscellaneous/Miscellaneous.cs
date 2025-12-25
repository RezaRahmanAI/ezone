using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace IMSWEB.Service
{
    public class MiscellaneousService<T> : IMiscellaneousService<T> where T : class
    {
        private readonly IBaseRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MiscellaneousService(IBaseRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public string GetUniqueKey(Func<T, int> codeSelector)
        {
            if (_repository.All.Any())
            {
                return (_repository.All.ToList().Max(codeSelector) + 1).ToString("D5");
            }
            else
            {
                return "00001";
            }
        }

        public T GetDuplicateEntry(Expression<Func<T, bool>> duplicateSelector)
        {
            return _repository.All.Where(duplicateSelector).FirstOrDefault();
        }
    }
}
