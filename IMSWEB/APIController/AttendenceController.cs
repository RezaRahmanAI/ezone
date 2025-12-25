using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWEB.APIController
{
    [AllowAnonymous]
    public class AttendenceController : ApiController
    {
        IAttendenceService _attendenceService;
        ISystemInformationService _SystemInformationService;
        IMapper _mapper;
        IEmployeeService _EmployeeService;
        public AttendenceController(IAttendenceService AttendenceService,
            ISystemInformationService SystemInformationService, IEmployeeService EmployeeService,
            IMapper mapper)
        {
            _attendenceService = AttendenceService;
            _SystemInformationService = SystemInformationService;
            _mapper = mapper;
            _EmployeeService = EmployeeService;
        }

        public bool CheckAuthentication(int ConcernID, string APIKey, string DeviceSerialNO, string IP)
        {
            var SysInfo = _SystemInformationService.GetSystemInformationByConcernId(ConcernID);
            bool IsAuthentic = false;
            if (SysInfo != null)
            {
                if (SysInfo.APIKey.Equals(APIKey) && SysInfo.DeviceSerialNO.Equals(DeviceSerialNO) && SysInfo.DeviceIP.Trim().Equals(IP.Trim()))
                {
                    IsAuthentic = true;
                }
            }
            return IsAuthentic;
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetSalaryProcessDate(int ConcernID, string APIKey, string DeviceSerialNO, string IP)
        {
            if (!CheckAuthentication(ConcernID, APIKey, DeviceSerialNO, IP))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = "Authentication failed." });
            }

            var SysInfo = _SystemInformationService.GetSystemInformationByConcernId(ConcernID);
            var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            return Request.CreateResponse(HttpStatusCode.OK, new { FromDate = DateRange.Item1, ToDate = DateRange.Item2 });
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage GetAttendenceLog([FromBody]List<AttendenceLog> Data, int ConcernID, string APIKey, string DeviceSerialNO, string IP)
        {
            AttendenceMonthViewModel oAttViewModel = new AttendenceMonthViewModel();
            if (!CheckAuthentication(ConcernID, APIKey, DeviceSerialNO, IP))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = "Authentication failed." });
            }
            var SysInfo = _SystemInformationService.GetSystemInformationByConcernId(ConcernID);
            var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            var DeleteStatus = DeleteLog(DateRange.Item2, ConcernID);
            if (!DeleteStatus.Item1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = DeleteStatus.Item2 });
            }

            var Employees = _EmployeeService.GetAllEmployeeIQueryable(ConcernID);

            oAttViewModel.ConcernID = ConcernID;
            oAttViewModel.CreateDate = DateTime.Now;
            oAttViewModel.CreatedBy = 1;
            oAttViewModel.AttendencMonth = DateRange.Item2;

            var Attendences = _attendenceService.GetZKTecoAttendenceData(DateRange.Item1, DateRange.Item2, SysInfo, Data, Employees);
            oAttViewModel.Attendences = _mapper.Map<List<Attendence>, List<AttendenceViewModel>>(Attendences);
            var Status = CheckAndAddModelError(oAttViewModel);
            if (!Status.Item1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = Status.Item2 });
            }
            DataTable ATTMOnth = CreateAttendencMonthDataTable(oAttViewModel);
            DataTable ATTdays = CreateAttendencDetailsDataTable(oAttViewModel.Attendences.ToList());
            if (_attendenceService.AddAttendencMonthUsingSP(ATTMOnth, ATTdays))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Data = "Upload Successfull." });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { Data = "Upload failed." });
        }

        private Tuple<bool, string> DeleteLog(DateTime AttendencMonth, int ConcernID)
        {
            bool Result = false;
            string Msg = string.Empty;
            var attend = _attendenceService.GetAllIQueryable(ConcernID).FirstOrDefault(i => i.AttendencMonth == AttendencMonth);
            if (attend != null)
            {
                if (attend.IsFinalize == 1)
                {
                    Msg = "This salay month is finalized. You can't delete it.";
                    Result = false;
                }
                else
                {
                    if (_attendenceService.DeleteUsingSP(attend.AttenMonthID))
                    {
                        Msg = "Delete Successfull.";
                        Result = true;
                    }
                }
            }
            else
                Result = true;

            return new Tuple<bool, string>(Result, Msg);
        }
        public Tuple<DateTime, DateTime> GetFirstAndLastDateOfMonth(DateTime date)
        {
            DateTime firstDate = new DateTime(date.Year, date.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);
            DateTime ld = new DateTime(lastDate.Year, lastDate.Month, lastDate.Day, 23, 59, 59);
            return new Tuple<DateTime, DateTime>(firstDate, ld);
        }

        Tuple<bool, string> CheckAndAddModelError(AttendenceMonthViewModel attendMonth)
        {
            if (attendMonth.AttendencMonth == DateTime.MinValue)
            {
                return new Tuple<bool, string>(false, "Attendence Month is required.");
            }

            if (attendMonth.Attendences.Count() == 0)
            {
                return new Tuple<bool, string>(false, "Attendence logs are not found.");
            }

            var DateRange = GetFirstAndLastDateOfMonth(attendMonth.AttendencMonth);
            if (attendMonth.Attendences.Any(i => i.Date < DateRange.Item1) || attendMonth.Attendences.Any(i => i.Date > DateRange.Item2))
            {
                return new Tuple<bool, string>(false, "Attendence Month is not matching with attendence data.");
            }

            if (_attendenceService.GetAllIQueryable().Any(i => i.AttendencMonth == DateRange.Item2))
            {
                return new Tuple<bool, string>(false, "This month's attendence is already uploaded.");
            }

            return new Tuple<bool, string>(true, "This month's attendence is already uploaded.");
        }

        private DataTable CreateAttendencMonthDataTable(AttendenceMonthViewModel AttendenceMonth)
        {
            DataTable dtAttendMonth = new DataTable();
            dtAttendMonth.Columns.Add("AttendencMonth", typeof(DateTime));
            dtAttendMonth.Columns.Add("IsFinalize", typeof(int));
            dtAttendMonth.Columns.Add("CreatedBy", typeof(int));
            dtAttendMonth.Columns.Add("CreateDate", typeof(DateTime));
            dtAttendMonth.Columns.Add("ModifiedBy", typeof(int));
            dtAttendMonth.Columns.Add("ModifiedDate", typeof(DateTime));
            dtAttendMonth.Columns.Add("ConcernID", typeof(int));

            DataRow row = null;

            row = dtAttendMonth.NewRow();
            row["AttendencMonth"] = AttendenceMonth.AttendencMonth;
            row["IsFinalize"] = 0;
            row["CreatedBy"] = AttendenceMonth.CreatedBy;
            row["CreateDate"] = AttendenceMonth.CreateDate;
            row["ModifiedBy"] = DBNull.Value;
            row["ModifiedDate"] = DBNull.Value;
            row["ConcernID"] = AttendenceMonth.ConcernID;
            dtAttendMonth.Rows.Add(row);

            return dtAttendMonth;
        }

        private DataTable CreateAttendencDetailsDataTable(List<AttendenceViewModel> attendenceDetailsList)
        {
            DataTable dtAttendList = new DataTable();
            dtAttendList.Columns.Add("EmployeeNo", typeof(int));
            dtAttendList.Columns.Add("AccountNo", typeof(int));
            dtAttendList.Columns.Add("Name", typeof(string));
            dtAttendList.Columns.Add("Date", typeof(DateTime));
            dtAttendList.Columns.Add("Timetable", typeof(string));

            dtAttendList.Columns.Add("OnDuty", typeof(string));
            dtAttendList.Columns.Add("OffDuty", typeof(string));
            dtAttendList.Columns.Add("ClockIn", typeof(string));
            dtAttendList.Columns.Add("ClockOut", typeof(string));
            dtAttendList.Columns.Add("Normal", typeof(string));

            dtAttendList.Columns.Add("Realtime", typeof(decimal));
            dtAttendList.Columns.Add("Late", typeof(string));
            dtAttendList.Columns.Add("Early", typeof(string));
            dtAttendList.Columns.Add("Absent", typeof(int));
            dtAttendList.Columns.Add("OTTime", typeof(string));

            dtAttendList.Columns.Add("WorkTime", typeof(string));
            dtAttendList.Columns.Add("Exception", typeof(string));
            dtAttendList.Columns.Add("MustCheckIn", typeof(int));
            dtAttendList.Columns.Add("MustCheckOut", typeof(int));
            dtAttendList.Columns.Add("Department", typeof(string));

            dtAttendList.Columns.Add("NDays", typeof(decimal));
            dtAttendList.Columns.Add("Weekend", typeof(decimal));
            dtAttendList.Columns.Add("Holiday", typeof(decimal));
            dtAttendList.Columns.Add("ATTTime", typeof(string));
            dtAttendList.Columns.Add("NDaysOT", typeof(decimal));

            dtAttendList.Columns.Add("WeekendOT", typeof(decimal));
            dtAttendList.Columns.Add("HolidayOT", typeof(decimal));
            DataRow row = null;
            foreach (var item in attendenceDetailsList)
            {
                row = dtAttendList.NewRow();
                row["EmployeeNo"] = item.EmployeeNo;
                row["AccountNo"] = item.AccountNo;
                row["Name"] = item.Name;
                row["Date"] = item.Date;
                row["Timetable"] = item.Timetable;

                row["OnDuty"] = item.OnDuty;
                row["OffDuty"] = item.OffDuty;
                row["ClockIn"] = item.ClockIn;
                row["ClockOut"] = item.ClockOut;
                row["Normal"] = item.Normal;

                row["Realtime"] = item.Realtime;
                row["Late"] = item.Late;
                row["Early"] = item.Early;
                row["Absent"] = item.Absent;
                row["OTTime"] = item.OTTime;

                row["WorkTime"] = item.WorkTime;
                row["Exception"] = item.Exception;
                row["MustCheckIn"] = item.MustCheckIn;
                row["MustCheckOut"] = item.MustCheckOut;
                row["Department"] = item.Department;


                row["NDays"] = item.NDays;
                row["Weekend"] = item.Weekend;
                row["Holiday"] = item.Holiday;
                row["ATTTime"] = item.ATTTime;
                row["NDaysOT"] = item.NDaysOT;

                row["WeekendOT"] = item.WeekendOT;
                row["HolidayOT"] = item.HolidayOT;

                dtAttendList.Rows.Add(row);
            }


            return dtAttendList;
        }
    }
}
