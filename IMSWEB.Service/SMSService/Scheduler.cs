using IMSWEB.Model;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMSWEB.Service
{

    public class JobScheduler 
    {
      
        public void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            Variable.Check = true;
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<GetData>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInSeconds(SMSServiceCredentials.IntervalInSeconds)
                .OnEveryDay()
                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0)))
                .Build();

            //ITrigger trigger = TriggerBuilder.Create()
            //                  .WithIdentity("trigger1", "group1")
            //                  .StartNow()
            //                  .WithSimpleSchedule(x => x
            //                  .WithIntervalInSeconds(10)
            //                  .RepeatForever())
            //                  .Build();


            scheduler.ScheduleJob(job, trigger);
        }
    }

    public class GetData : IJob
    {

        public GetData()
        {
        }
        public void Execute(IJobExecutionContext context)
        {
            new StatusUpdate().UpdateStatus();
        }


    }
    public interface IStatusUpdate
    {
        void UpdateStatus();
    }
    public class StatusUpdate
    {
        ISMSStatusService _StatusService;
        ISystemInformationService _systemInformationService;
        public StatusUpdate()
        {

        }
        public StatusUpdate(ISMSStatusService sMSStatusService, ISystemInformationService systemInformationService)
        {
            _StatusService = sMSStatusService;
            _systemInformationService = systemInformationService;
        }
        public void UpdateStatus()
        {
            var sysInfo = _systemInformationService.GetAllConcernSysInfo().FirstOrDefault();
            var PendingSMS = (from s in _StatusService.GetAllConcern()
                              where s.Code.Equals("ACCEPTD") && s.Message_ID != null
                              select new SMSRequest
                              {
                                  Message_ID = s.Message_ID
                              }).ToList();


           decimal  previousBalance = 0m;
            var response = SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.REVEGETStatus, PendingSMS, previousBalance, sysInfo, 0);
            foreach (var item in response)
            {
                var sms = _StatusService.GetAllConcern().FirstOrDefault(i => i.Message_ID == item.Message_ID);
                if (sms != null)
                {
                    sms.Code = item.Code;
                    sms.SendingStatus = item.SendingStatus;
                    _StatusService.Update(sms);
                }
                _StatusService.Save();
            }

        }
    }

    public static class Variable
    {
        public static bool Check { get; set; }
    }
}
