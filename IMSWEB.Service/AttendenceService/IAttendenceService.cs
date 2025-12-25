using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
   public interface IAttendenceService
    {
        void Add(AttendenceMonth AttendenceMonth);
        void Update(AttendenceMonth AttendenceMonth);
        void Save();
        IQueryable<AttendenceMonth> GetAllIQueryable();
        List<Attendence> GetAttendencByAttenMonthID(int AttenMonthID);
        IQueryable<Attendence> GetDetails();
        AttendenceMonth GetById(int id);
        bool DeleteUsingSP(int AttenMonthID);
        bool AddAttendencMonthUsingSP(DataTable dtMonth, DataTable dtAttendenc);
        List<Attendence> GetZKTecoAttendenceData(DateTime FromDate, DateTime ToDate, SystemInformation SysInfo, List<AttendenceLog> lstMachineInfo, IQueryable<Employee> Employees);
        IQueryable<AttendenceMonth> GetAllIQueryable(int ConcernID);
    }
}
