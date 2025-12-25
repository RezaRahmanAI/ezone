using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public class DbFactory : Disposable, IDbFactory
    {
        IMSWEBContext _dbContext;

        public IMSWEBContext Init()
        {
            return _dbContext ?? (_dbContext = new IMSWEBContext());
        }

        protected override void DisposeCore()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
    }
}
