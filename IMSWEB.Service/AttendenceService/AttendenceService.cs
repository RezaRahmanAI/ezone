using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class AttendenceService : IAttendenceService
    {
        private readonly IBaseRepository<AttendenceMonth> _baseAttendenceMonthRepository;
        private readonly IBaseRepository<Attendence> _AttendenceRepository;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendenceRepository _attendRepository;
        public AttendenceService(IBaseRepository<AttendenceMonth> baseRepository,
            IBaseRepository<Attendence> AttendenceRepository,
            IAttendenceRepository attendRepository, IBaseRepository<Employee> EmployeeRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseAttendenceMonthRepository = baseRepository;
            _AttendenceRepository = AttendenceRepository;
            _attendRepository = attendRepository;
            _EmployeeRepository = EmployeeRepository;
        }

        public void Add(AttendenceMonth AttendenceMonth)
        {
            _baseAttendenceMonthRepository.Add(AttendenceMonth);
        }

        public void Update(AttendenceMonth AttendenceMonth)
        {
            _baseAttendenceMonthRepository.Update(AttendenceMonth);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<AttendenceMonth> GetAllIQueryable()
        {
            return _baseAttendenceMonthRepository.All;
        }
        public IQueryable<AttendenceMonth> GetAllIQueryable(int ConcernID)
        {
            return _baseAttendenceMonthRepository.GetAllByConcernID(ConcernID);
        }
        public List<Attendence> GetAttendencByAttenMonthID(int AttenMonthID)
        {
            return _AttendenceRepository.All.Where(i => i.AttenMonthID == AttenMonthID).OrderBy(i => i.Date).ToList();
        }
        public IQueryable<Attendence> GetDetails()
        {
            return _AttendenceRepository.All;
        }
        public AttendenceMonth GetById(int id)
        {
            return _baseAttendenceMonthRepository.AllIncluding(i => i.Attendences).FirstOrDefault(i => i.AttenMonthID == id);
        }
        public bool DeleteUsingSP(int AttenMonthID)
        {
            return _attendRepository.DeleteAttendenceByAttenMonthID(AttenMonthID);
        }
        public bool AddAttendencMonthUsingSP(DataTable dtMonth, DataTable dtAttendenc)
        {
            return _attendRepository.AddAttendencMonthUsingSP(dtMonth, dtAttendenc);
        }

        public List<Attendence> GetZKTecoAttendenceData(DateTime FromDate, DateTime ToDate, SystemInformation SysInfo, List<AttendenceLog> lstMachineInfo, IQueryable<Employee> Employees)
        {
            var attendenceList = new List<Attendence>();
            try
            {
                var AllAttendences = lstMachineInfo.Select(dataRow =>
                     new Attendence
                     {
                         EmployeeNo = dataRow.AccountNo,
                         AccountNo = dataRow.AccountNo,
                         Date = Convert.ToDateTime(dataRow.DateTimeRecord),
                     }).Where(i => i.Date >= FromDate && i.Date <= ToDate).ToList();

                if (AllAttendences.Count() == 0)
                    return attendenceList;
                attendenceList = ProcessAttendenceData(FromDate, ToDate, SysInfo, Employees, AllAttendences);
            }
            catch (Exception ex)
            {
                attendenceList = new List<Attendence>();
            }
            return attendenceList;
        }
        Tuple<DateTime, DateTime> GetAttendDateRange(DateTime FromDate, DateTime ToDate, SystemInformation SysInfo)
        {
            var fd = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, SysInfo.OnDuty.Hours - 2, SysInfo.OnDuty.Minutes, SysInfo.OnDuty.Seconds);
            var td = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, SysInfo.OffDuty.Hours + 4, SysInfo.OffDuty.Minutes, SysInfo.OffDuty.Seconds);
            return new Tuple<DateTime, DateTime>(fd, td);
        }
        private List<Attendence> ProcessAttendenceData(DateTime FromDate, DateTime ToDate, SystemInformation SysInfo,
             IQueryable<Employee> Employees, List<Attendence> AllAttendences)
        {
            List<Attendence> ProcessedATT = new List<Attendence>();
            List<Attendence> EmployeeAtt = new List<Attendence>();
            List<Attendence> SecondShiftsAtt = new List<Attendence>();
            Attendence oCheckIN = null;
            Attendence oCheckOUT = null;
            Attendence oAttendence = null;
            DateTime nextDate = DateTime.MinValue;
            Tuple<DateTime, DateTime> dateRange = null;
            var FilterATTs = AllAttendences.Where(i => i.Date >= FromDate && i.Date <= ToDate).ToList();
            for (DateTime date = FromDate; date <= ToDate; date = date.AddDays(1))
            {
                dateRange = GetAttendDateRange(date, date, SysInfo);
                foreach (var item in Employees)
                {
                    EmployeeAtt = FilterATTs.Where(i => i.AccountNo == item.MachineEMPID && (i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2)).ToList();

                    oAttendence = new Attendence();
                    oAttendence.Name = item.Name;
                    oAttendence.EmployeeNo = item.EmployeeID;
                    oAttendence.OnDuty = SysInfo.OnDuty.Hours.ToString().PadLeft(2, '0') + ":" + SysInfo.OnDuty.Minutes.ToString().PadLeft(2, '0');
                    oAttendence.OffDuty = SysInfo.OffDuty.Hours.ToString().PadLeft(2, '0') + ":" + SysInfo.OffDuty.Minutes.ToString().PadLeft(2, '0');
                    oAttendence.Realtime = 1m;
                    oAttendence.MustCheckIn = 1;
                    oAttendence.MustCheckOut = 1;

                    #region default value
                    oAttendence.Late = string.Empty;
                    oAttendence.Early = string.Empty;
                    oAttendence.WorkTime = string.Empty;
                    oAttendence.Exception = string.Empty;
                    oAttendence.Normal = string.Empty;
                    oAttendence.Timetable = string.Empty;
                    oAttendence.NDays = 0;
                    oAttendence.Weekend = 0;
                    oAttendence.Holiday = 0;
                    oAttendence.ATTTime = string.Empty;
                    oAttendence.NDaysOT = 0m;
                    oAttendence.WeekendOT = 0m;
                    oAttendence.HolidayOT = 0m;
                    #endregion

                    #region IN and OUT
                    if (EmployeeAtt.Count > 1)
                    {
                        oCheckIN = EmployeeAtt.OrderBy(i => i.Date).FirstOrDefault();
                        oCheckOUT = EmployeeAtt.OrderByDescending(i => i.Date).FirstOrDefault();

                        #region GEN model
                        oAttendence.Absent = 0;
                        oAttendence.AccountNo = item.MachineEMPID;
                        oAttendence.Date = new DateTime(oCheckIN.Date.Year, oCheckIN.Date.Month, oCheckIN.Date.Day);
                        oAttendence.ClockIn = oCheckIN.Date.ToString("HH:mm");
                        oAttendence.ClockOut = oCheckOUT.Date.ToString("HH:mm");
                        oAttendence.OTTime = CalculateOT(oAttendence.OffDuty, oAttendence.ClockOut);
                        oAttendence.Late = CalculateLate(oAttendence.OnDuty, oAttendence.ClockIn);
                        oAttendence.WorkTime = CalculateWorkTime(oCheckOUT.Date, oCheckIN.Date, SysInfo);
                        #endregion
                    }
                    #endregion

                    #region Only IN or OUT and Night Shifts
                    else if (EmployeeAtt.Count == 1)
                    {
                        oCheckIN = EmployeeAtt.FirstOrDefault();
                        nextDate = date.Date.AddDays(1);
                        nextDate = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, SysInfo.OnDuty.Hours - 2, 0, 0);

                        SecondShiftsAtt = AllAttendences.Where(i => i.AccountNo == item.MachineEMPID && (i.Date <= nextDate && i.Date.Date > date.Date)).OrderBy(i => i.Date).ToList();

                        oCheckOUT = SecondShiftsAtt.FirstOrDefault();
                        #region GEN model
                        oAttendence.Absent = 0;
                        oAttendence.AccountNo = item.MachineEMPID;
                        oAttendence.Date = new DateTime(date.Year, date.Month, date.Day);
                        oAttendence.ClockIn = oCheckIN.Date.ToString("HH:mm");
                        if (oCheckOUT != null)
                        {
                            oAttendence.ClockOut = oCheckOUT.Date.ToString("HH:mm");
                            oAttendence.WorkTime = CalculateWorkTime(new DateTime(oCheckOUT.Date.Year, oCheckOUT.Date.Month, oCheckOUT.Date.Day, SysInfo.OffDuty.Hours, SysInfo.OffDuty.Minutes, 0), oCheckIN.Date, SysInfo);
                            oAttendence.OTTime = CalculateSecondShiftOT("02:00", oAttendence.ClockOut, 5);
                        }
                        oAttendence.Late = CalculateLate(oAttendence.OnDuty, oAttendence.ClockIn);
                        #endregion
                    }
                    #endregion

                    #region Absent
                    else
                    {
                        #region GEN model
                        oAttendence.Absent = 1;
                        oAttendence.AccountNo = item.MachineEMPID;
                        oAttendence.Date = date.Date;
                        oAttendence.ClockIn = string.Empty;
                        oAttendence.ClockOut = string.Empty;
                        oAttendence.OTTime = string.Empty;

                        #endregion
                    }
                    #endregion


                    ProcessedATT.Add(oAttendence);
                    oAttendence = null;
                    oCheckIN = null;
                    oCheckOUT = null;
                    EmployeeAtt = new List<Attendence>();
                }
            }

            return ProcessedATT;
        }

        string CalculateOT(string OffDuty, string ClockOUT)
        {
            try
            {
                if (ClockOUT.Split(':').Length == 2)
                {
                    decimal hours = Convert.ToDecimal(ClockOUT.Split(':')[0]) - Convert.ToDecimal(OffDuty.Split(':')[0]);
                    if (hours > 0m)
                    {
                        decimal mins = Convert.ToDecimal(ClockOUT.Split(':')[1]) - Convert.ToDecimal(OffDuty.Split(':')[1]);
                        return hours.ToString().PadLeft(2, '0') + ":" + mins.ToString().PadLeft(2, '0');
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        string CalculateSecondShiftOT(string OffDuty, string ClockOUT, int SecShiftOTHours)
        {
            try
            {
                if (ClockOUT.Split(':').Length == 2)
                {
                    decimal hours = Convert.ToDecimal(ClockOUT.Split(':')[0]) - Convert.ToDecimal(OffDuty.Split(':')[0]);
                    hours = hours + SecShiftOTHours;

                    decimal mins = Convert.ToDecimal(ClockOUT.Split(':')[1]) - Convert.ToDecimal(OffDuty.Split(':')[1]);

                    return hours.ToString().PadLeft(2, '0') + ":" + mins.ToString().PadLeft(2, '0');
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        string CalculateLate(string ONDute, string ClockIN)
        {
            try
            {
                if (ClockIN.Split(':').Length == 2)
                {
                    decimal hours = Convert.ToDecimal(ClockIN.Split(':')[0]) - Convert.ToDecimal(ONDute.Split(':')[0]);
                    if (hours >= 0m)
                    {
                        decimal mins = Convert.ToDecimal(ClockIN.Split(':')[1]) - Convert.ToDecimal(ONDute.Split(':')[1]);
                        if (mins > 5m)
                            return hours.ToString().PadLeft(2, '0') + ":" + mins.ToString().PadLeft(2, '0');
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        int CompareDate(DateTime date1, DateTime date2)
        {
            return DateTime.Compare(date1, date2);
        }
        string CalculateWorkTime(DateTime dtCheckOUT, DateTime dtCheckIN, SystemInformation SysInfo)
        {
            string WorkTime = string.Empty;

            try
            {
                TimeSpan span = new TimeSpan();
                if (CompareDate(new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, dtCheckIN.Hour, dtCheckIN.Minute, 0), new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, SysInfo.OnDuty.Hours, SysInfo.OnDuty.Minutes, 0)) <= 0)
                {
                    if (CompareDate(new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0), new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, SysInfo.OffDuty.Minutes, 0, 0)) <= 0)
                        span = new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0)
                            .Subtract(new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, SysInfo.OnDuty.Hours, 0, 0));

                    else if (CompareDate(new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0), new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, SysInfo.OffDuty.Minutes, 0)) > 0)
                        span = new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, 0, 0)
                            .Subtract(new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, SysInfo.OnDuty.Hours, 0, 0));
                }
                else
                {
                    if (CompareDate(new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0), new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, SysInfo.OffDuty.Minutes, 0, 0)) <= 0)
                        span = new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0)
                            .Subtract(new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, dtCheckIN.Hour, dtCheckIN.Minute, 0));

                    else if (CompareDate(new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, dtCheckOUT.Hour, dtCheckOUT.Minute, 0), new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, SysInfo.OffDuty.Minutes, 0)) > 0)
                        span = new DateTime(dtCheckOUT.Year, dtCheckOUT.Month, dtCheckOUT.Day, SysInfo.OffDuty.Hours, 0, 0)
                            .Subtract(new DateTime(dtCheckIN.Year, dtCheckIN.Month, dtCheckIN.Day, dtCheckIN.Hour, dtCheckIN.Minute, 0));
                }


                if (span.Hours >= 0 && span.Minutes >= 0)
                    WorkTime = span.Hours.ToString().PadLeft(2, '0') + ":" + span.Minutes.ToString().PadLeft(2, '0');

                return WorkTime;
            }
            catch (Exception)
            {
            }
            return WorkTime;
        }
    }
}
