using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace IMSWEB.Controllers
{
    [Authorize]
    public class UploadAttendenceController : CoreController
    {

        IMiscellaneousService<Category> _miscellaneousService;
        IAttendenceService _attendenceService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;

        public UploadAttendenceController(IErrorService errorService, IMiscellaneousService<Category> miscellaneousService,
            IAttendenceService AttendenceService, ISystemInformationService SysService,
            IMapper mapper)
            : base(errorService, SysService)
        {
            _miscellaneousService = miscellaneousService;
            _attendenceService = AttendenceService;
            _mapper = mapper;
            _sysInfoService = SysService;

        }
        public ActionResult Index()
        {
            DateTime dAttendencMonth = DateTime.MinValue;
            if (TempData.ContainsKey("AttendencMonth"))
            {
                dAttendencMonth = Convert.ToDateTime(TempData["AttendencMonth"]);
                var DateRange = GetFirstAndLastDateOfMonth(dAttendencMonth);
                dAttendencMonth = DateRange.Item2;
            }
            else
            {
                var Sysinfo = _sysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                dAttendencMonth = Sysinfo.NextPayProcessDate;
            }
            ViewBag.AttendencMonth = dAttendencMonth;
            AttendenceMonthViewModel vmAttendec = new AttendenceMonthViewModel();
            //var attenence = await _attendenceService.GetAttendencByAttenMonthIDAsync(dAttendencMonth);
            var attend = _attendenceService.GetAllIQueryable().FirstOrDefault(i => i.AttendencMonth == dAttendencMonth);
            if (attend != null)
            {
                var allattendences = _attendenceService.GetAttendencByAttenMonthID(attend.AttenMonthID);
                vmAttendec.Attendences = _mapper.Map<List<Attendence>, List<AttendenceViewModel>>(allattendences);
            }
            return View(vmAttendec);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /UploadAttendence/Create
        public ActionResult Create()
        {
            return View();
        }

        void CheckAndAddModelError(AttendenceMonthViewModel attendMonth, FormCollection formCollection)
        {
            if (attendMonth.AttendencMonth == DateTime.MinValue)
            {
                ModelState.AddModelError("AttendencMonth", "Attendence Month is required.");
                AddToastMessage("", "Attendence Month is required.", ToastType.Error);
            }
            if (attendMonth.Attendences.Count() == 0)
            {
                ModelState.AddModelError("AttendencMonth", "Excel is empty.");
                AddToastMessage("", "Excel is empty.", ToastType.Error);
            }
            var DateRange = GetFirstAndLastDateOfMonth(attendMonth.AttendencMonth);
            if (attendMonth.Attendences.Any(i => i.Date < DateRange.Item1) || attendMonth.Attendences.Any(i => i.Date > DateRange.Item2))
            {
                ModelState.AddModelError("AttendencMonth", "Attendence Month is required.");
                AddToastMessage("", "Attendence Month is not matching with attendence data.", ToastType.Error);
            }

            if (_attendenceService.GetAllIQueryable().Any(i => i.AttendencMonth == DateRange.Item2))
            {
                ModelState.AddModelError("AttendencMonth", "This month's attendence is already uploaded.");
                AddToastMessage("", "This month's attendence is already uploaded.", ToastType.Error);
            }
        }


        AttendenceMonthViewModel GetExcelData(HttpPostedFileBase FileUpload, FormCollection formCollection)
        {
            string filename = FileUpload.FileName;
            AttendenceMonthViewModel attendMonth = new AttendenceMonthViewModel();
            AttendenceViewModel attend = null;
            if (filename.EndsWith(".xlsx"))
            {
                #region Read Excel File

                string fileContentType = FileUpload.ContentType;
                byte[] fileBytes = new byte[FileUpload.ContentLength];
                var data = FileUpload.InputStream.Read(fileBytes, 0, Convert.ToInt32(FileUpload.ContentLength));
                using (var package = new ExcelPackage(FileUpload.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.FirstOrDefault();
                    if (workSheet == null)
                    {
                        AddToastMessage("", "Uploaded Excel is empty.", ToastType.Error);
                        return attendMonth;
                    }

                    int noOfRow = workSheet.Dimension.End.Row;
                    int noOfCol = workSheet.Dimension.End.Column;

                    if (noOfCol < 6)
                    {
                        AddToastMessage("", "Uploaded Excel format is not correct.", ToastType.Error);
                        return attendMonth;
                    }
                    for (int rowIterator = 2; rowIterator < noOfRow; rowIterator++)
                    {
                        attend = new AttendenceViewModel();
                        attend.EmployeeNo = Convert.ToInt32(GetDefaultIfNull(workSheet.Cells[rowIterator, 2].Value.ToString()));
                        attend.AccountNo = Convert.ToInt32(GetDefaultIfNull(workSheet.Cells[rowIterator, 2].Value.ToString()));
                        attend.Name = workSheet.Cells[rowIterator, 3].Value.ToString();
                        attend.Date = Convert.ToDateTime(workSheet.Cells[rowIterator, 5].Value.ToString());
                        attend.Timetable = string.Empty; //workSheet.Cells[rowIterator, 7].Value.ToString();
                        attend.OnDuty = string.Empty;//workSheet.Cells[rowIterator, 8].Value.ToString();
                        attend.OffDuty = string.Empty;// workSheet.Cells[rowIterator, 9].Value.ToString();
                        attend.ClockIn = GetDefaultIfNull(workSheet.Cells[rowIterator, 6].Value.ToString());
                        attend.ClockOut = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();// GetDefaultIfNull(workSheet.Cells[rowIterator, 7].Value.ToString());
                        attend.Normal = string.Empty;// workSheet.Cells[rowIterator, 12].Value.ToString();
                        attend.Realtime = 0;// Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 13].Value.ToString()));
                        attend.Late = string.Empty;//  workSheet.Cells[rowIterator, 14].Value.ToString();
                        attend.Early = string.Empty;//  workSheet.Cells[rowIterator, 15].Value.ToString();
                        attend.Absent = 0;// Convert.ToInt32((((workSheet.Cells[rowIterator, 16].Value.ToString()) == "True") ? 1 : 0));
                        attend.OTTime = string.Empty;//  workSheet.Cells[rowIterator, 17].Value.ToString();
                        attend.WorkTime = string.Empty;//  workSheet.Cells[rowIterator, 18].Value.ToString();
                        attend.Exception = string.Empty;//  workSheet.Cells[rowIterator, 19].Value.ToString();
                        attend.MustCheckIn = 0;//Convert.ToInt32((workSheet.Cells[rowIterator, 20].Value.ToString() == "True") ? 1 : 0);
                        attend.MustCheckOut = 0;// Convert.ToInt32((workSheet.Cells[rowIterator, 21].Value.ToString() == "True") ? 1 : 0);
                        attend.Department = workSheet.Cells[rowIterator, 1].Value.ToString();
                        attend.NDays = 0;//Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 23].Value.ToString()));
                        attend.Weekend = 0;// Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 24].Value.ToString()));
                        attend.Holiday = 0;// Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 25].Value.ToString()));
                        attend.ATTTime = string.Empty;//  workSheet.Cells[rowIterator, 26].Value.ToString();
                        attend.NDaysOT = 0;// Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 27].Value.ToString()));
                        attend.WeekendOT = 0;// Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 28].Value.ToString()));
                        attend.HolidayOT = 0;//Convert.ToDecimal(GetDefaultIfNull(workSheet.Cells[rowIterator, 29].Value.ToString()));
                        attendMonth.Attendences.Add(attend);
                    }
                }

                #endregion

                if (!string.IsNullOrEmpty(formCollection["AttendencMonth"]))
                {
                    var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["AttendencMonth"]));
                    attendMonth.AttendencMonth = DateRange.Item2;
                }
                attendMonth.CreateDate = DateTime.Now;
                attendMonth.CreatedBy = User.Identity.GetUserId<int>();
                attendMonth.ConcernID = User.Identity.GetConcernId();
            }
            return attendMonth;

        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase FileUpload, FormCollection formCollection)
        {
            if (formCollection.Get("btnSave") != null)
            {
                if (FileUpload != null)
                {
                    if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        var attendMonth = GetExcelData(FileUpload, formCollection);
                        CheckAndAddModelError(attendMonth, formCollection);

                        if (!ModelState.IsValid)
                        {
                            return RedirectToAction("Index");
                        }

                        DataTable ATTMOnth = CreateAttendencMonthDataTable(attendMonth);
                        DataTable ATTdays = CreateAttendencDataTable(attendMonth.Attendences.ToList());
                        if (_attendenceService.AddAttendencMonthUsingSP(ATTMOnth, ATTdays))
                        {
                            AddToastMessage("", "Upload Successfull.", ToastType.Success);
                        }
                        else
                            AddToastMessage("", "Upload Failed.", ToastType.Error);

                        return RedirectToAction("Index");

                    }
                }
                AddToastMessage("", "Please upload excel file(.xlsx)", ToastType.Error);
                return RedirectToAction("Index");

            }
            else if (formCollection.Get("btnSearch") != null)
            {
                if (!string.IsNullOrEmpty(formCollection["AttendencMonth"]))
                {
                    TempData["AttendencMonth"] = Convert.ToDateTime(formCollection["AttendencMonth"]);
                }
                return RedirectToAction("Index");
            }

            else if (formCollection.Get("btnDelete") != null)
            {
                if (!string.IsNullOrEmpty(formCollection["AttendencMonth"]))
                {
                    var DateRange = GetFirstAndLastDateOfMonth(Convert.ToDateTime(formCollection["AttendencMonth"]));
                    var attend = _attendenceService.GetAllIQueryable().FirstOrDefault(i => i.AttendencMonth == DateRange.Item2);
                    if (attend != null)
                    {
                        if (attend.IsFinalize == 1)
                        {
                            AddToastMessage("", "This salay month is finalized. You can't delete it.", ToastType.Success);
                        }
                        else
                        {
                            if (_attendenceService.DeleteUsingSP(attend.AttenMonthID))
                            {
                                AddToastMessage("", "Delete Successfull.", ToastType.Success);
                            }
                            else
                            {
                                AddToastMessage("", "Delete Failed.", ToastType.Success);
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }
            }
            AddToastMessage("", "Please upload excel file(.xlx or .xlsx)", ToastType.Error);
            return RedirectToAction("Index");
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

        private DataTable CreateAttendencDataTable(List<AttendenceViewModel> attendenceList)
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
            foreach (var item in attendenceList)
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DailyAttendence(FormCollection formCollection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult DailyAttendenceList()
        {
            DateTime dAttendencMonth = DateTime.Today;
            var DateRange = GetFirstAndLastDateOfMonth(dAttendencMonth);
            ViewBag.FromDate = dAttendencMonth;
            ViewBag.ToDate = dAttendencMonth;
            AttendenceMonthViewModel vmAttendec = new AttendenceMonthViewModel();

            var attend = _attendenceService.GetAllIQueryable().FirstOrDefault(i => i.AttendencMonth == DateRange.Item2);
            if (attend != null)
            {
                var allattendences = _attendenceService.GetAttendencByAttenMonthID(attend.AttenMonthID).Where(i => i.Date == dAttendencMonth).ToList();
                vmAttendec.Attendences = _mapper.Map<List<Attendence>, List<AttendenceViewModel>>(allattendences);
            }
            return View(vmAttendec);
        }

        [HttpPost]
        public ActionResult DailyAttendenceList(FormCollection formCollection)
        {
            DateTime FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            DateTime ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var DateRange = GetFirstAndLastDateOfMonth(FromDate);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            AttendenceMonthViewModel vmAttendec = new AttendenceMonthViewModel();

            var attend = _attendenceService.GetAllIQueryable().FirstOrDefault(i => i.AttendencMonth == DateRange.Item2);
            if (attend != null)
            {
                var allattendences = _attendenceService.GetAttendencByAttenMonthID(attend.AttenMonthID).Where(i => i.Date >= FromDate && i.Date <= ToDate).ToList();
                vmAttendec.Attendences = _mapper.Map<List<Attendence>, List<AttendenceViewModel>>(allattendences);
            }
            return View(vmAttendec);
        }




    }
}
