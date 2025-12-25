using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISMSStatusService
    {
        void Add(SMSStatus SMSStatus);
        void AddRange(List<SMSStatus> SMSStatuses);
        void Update(SMSStatus SMSStatus);
        void Save();
        IEnumerable<SMSStatus> GetAll();

        IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>>
            GetAll(DateTime fromDate, DateTime toDate, int Status);

        IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string, string, decimal>>>
            GetAllReport(DateTime fromDate, DateTime toDate,int Status, bool isAdminReport, int concernID);

        //Task<IEnumerable<SMSStatus>> GetAllAsync();
        SMSStatus GetById(int id);
        void Delete(int id);
        IQueryable<SMSStatus> GetAllIQueryable();
        IQueryable<SMSStatus> GetAllConcern();
    }
}
