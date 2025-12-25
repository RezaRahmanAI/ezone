using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IManualAttendencService
    {
        void Add(ManualAttendence ManualAttendence);
        void Update(ManualAttendence ManualAttendence);
        void Save();
        IQueryable<ManualAttendence> GetAll();
        ManualAttendence GetById(int id);
        void Delete(int id);
    }
}
