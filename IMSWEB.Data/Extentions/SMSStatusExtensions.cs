using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SMSStatusExtensions
    {

        public static IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>>
    GetAll(this IBaseRepository<SMSStatus> SMSStatusRepository,
    IBaseRepository<Customer> CustomerRepository, IBaseRepository<SMSFormate> SMSFormateRepository, DateTime fromDate, DateTime toDate, int Status)
        {
            IQueryable<SMSStatus> SMSStatusList = null;
            if (Status > 0)
                SMSStatusList = SMSStatusRepository.All.Where(i => i.SendingStatus == Status);
            else
                SMSStatusList = SMSStatusRepository.All;

            var result = (from sms in SMSStatusList
                          join smsf in SMSFormateRepository.All on sms.SMSFormateID equals smsf.SMSFormateID
                          join c in CustomerRepository.All on sms.CustomerID equals c.CustomerID into lc
                          from c in lc.DefaultIfEmpty()
                          where sms.EntryDate >= fromDate && sms.EntryDate <= toDate
                          select new
                          {
                              sms.SMSStatusID,
                              sms.EntryDate,
                              sms.ContactNo,
                              sms.Code,
                              sms.NoOfSMS,
                              sms.ResponseMsg,
                              SendingStatus = (EnumSMSSendStatus)sms.SendingStatus,
                              CustomerName = c == null ? "" : c.Name,
                              CustomerCode = c == null ? "" : c.Code,
                              Address = c == null ? "" : c.Address,
                              smsf.SMSDescription,
                              sms.SMS
                          }).OrderByDescending(i => i.SMSStatusID).ToList();

            return result.Select(x => new Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>(
                x.EntryDate,
                x.Code,
                x.ContactNo,
                x.NoOfSMS,
                x.SendingStatus,
                x.CustomerCode,
                x.CustomerName, new Tuple<string, string, string, string>
                    (x.Address,
                    x.ResponseMsg,
                    x.SMSDescription,
                    x.SMS
                    )
                ));

        }

    public static IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string, string, decimal>>>
            GetAllReport(this IBaseRepository<SMSStatus> SMSStatusRepository,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<SMSFormate> SMSFormateRepository, IBaseRepository<SisterConcern> sisterConcernRepo, IBaseRepository<SystemInformation> SystemRepository,
            DateTime fromDate, DateTime toDate, int Status, bool isAdminReport, int concernID)
        {
            IQueryable<SMSStatus> SMSStatusList = null;
            if (isAdminReport)
            {
                if (concernID > 0)
                {
                    SMSStatusList = SMSStatusRepository.GetAll().Where(i => i.ConcernID == concernID);
                    SMSStatusList = SMSStatusRepository.GetAll().Where(i => i.SendingStatus == Status && i.ConcernID == concernID);

                }
                else
                {
                    SMSStatusList = SMSStatusRepository.GetAll();
                    SMSStatusList = SMSStatusRepository.GetAll().Where(i => i.SendingStatus == Status);
                }

            }

            else
            {
                if (Status > 0)
                    SMSStatusList = SMSStatusRepository.All.Where(i => i.SendingStatus == Status);
                else
                    SMSStatusList = SMSStatusRepository.All;
            }


            var result = (from sms in SMSStatusList
                          join smsf in SMSFormateRepository.All on sms.SMSFormateID equals smsf.SMSFormateID
                          join c in CustomerRepository.GetAll() on sms.CustomerID equals c.CustomerID into lc
                          join sys in SystemRepository.GetAll() on sms.ConcernID equals sys.ConcernID 
                          join sis in sisterConcernRepo.GetAll() on sms.ConcernID equals sis.ConcernID
                          from c in lc.DefaultIfEmpty()
                          where sms.EntryDate >= fromDate && sms.EntryDate <= toDate
                          select new
                          {
                              sms.SMSStatusID,
                              sms.EntryDate,
                              sms.ContactNo,
                              sms.Code,
                              sms.NoOfSMS,
                              sms.ResponseMsg,
                              SendingStatus = (EnumSMSSendStatus)sms.SendingStatus,
                              CustomerName = c == null ? "" : c.Name,
                              CustomerCode = c == null ? "" : c.Code,
                              //CustomerName = c.Name,
                              //CustomerCode = c.Code,
                              Address = c == null ? "" : c.Address,
                              smsf.SMSDescription,
                              sms.SMS,
                              ConcernName = sis.Name,
                              SmsCharge = sys.smsCharge
                          }).OrderByDescending(i => i.SMSStatusID).ToList();

            return result.Select(x => new Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string, string, decimal>>(
                x.EntryDate,
                x.Code,
                x.ContactNo,
                x.NoOfSMS,
                x.SendingStatus,
                x.CustomerCode,
                x.CustomerName, new Tuple<string, string, string, string, string, decimal>
                    (x.Address,
                    x.ResponseMsg,
                    x.SMSDescription,
                    x.SMS,
                    x.ConcernName,
                    x.SmsCharge
                    )
                ));

        }


        public static IEnumerable<SMSPaymentMasterDetails> GetLastPayAmount(this IBaseRepository<SMSPaymentMasterDetails> _Repository, int SMSBillpayMasterID)
        {
            var data = _Repository.GetAll().Where(i => i.SMSPaymentMasterID == SMSBillpayMasterID)
                .Select(t => new
                {
                    Id = t.SMSPaymentDetailsID,
                    RecAmount = t.RecAmount,
                    ReceiptNo = t.ReceiptNo
                })
                .ToList();
            var result = data.Select(t => new SMSPaymentMasterDetails
            {
                SMSPaymentDetailsID = t.Id,
                RecAmount = t.RecAmount,
                ReceiptNo = t.ReceiptNo
            }).ToList();

            return result;
        }

    }
}
