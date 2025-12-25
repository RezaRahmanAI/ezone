using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace IMSWEB.Controllers
{
    [Authorize]
    public class HolidayCalenderController : CoreController
    {
        IMapper _mapper;
        IHolidayCalenderService _HolidayCalenderService;
        IMiscellaneousService<Grade> _miscell;
        ISystemInformationService _SysServie;
        public HolidayCalenderController(
            IErrorService errorService, IHolidayCalenderService HolidayCalenderService,
            ISystemInformationService SysServie,
            IMapper Mapper, IMiscellaneousService<Grade> miscell)
            : base(errorService, SysServie)
        {
            _HolidayCalenderService = HolidayCalenderService;
            _mapper = Mapper;
            _miscell = miscell;
            _SysServie = SysServie;
        }

        public ActionResult Index()
        {
            DateTime SearchDate = DateTime.MinValue;
            if (TempData.ContainsKey("SearchDate"))
            {
                SearchDate = Convert.ToDateTime(TempData["SearchDate"]);
            }
            else
            {
                var SysInfo = _SysServie.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                SearchDate = SysInfo.NextPayProcessDate;
            }
            ViewBag.SearchDate = SearchDate;
            var dateRange = GetFirstAndLastDateOfMonth(SearchDate);
            var Assignholidays = _HolidayCalenderService.GetAllIQueryable().Where(i => i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2);
            HolidayCalenderViewModel hcvm = null;
            List<HolidayCalenderViewModel> hcVmList = new List<HolidayCalenderViewModel>();
            HolidayCalender holiday = null;

            for (DateTime date = dateRange.Item1; date <= dateRange.Item2; date = date.AddDays(1))
            {
                hcvm = new HolidayCalenderViewModel();
                holiday = Assignholidays.FirstOrDefault(i => i.Date == date);
                hcvm.Type = holiday == null ? EnumHolidayType.SpecialHoliday : ((EnumHolidayType)holiday.Type);
                hcvm.Date = date.Date.ToString("dd MMM yyyy");
                if (Assignholidays.Any(i => i.Date == date))
                {
                    hcvm.IsAssigned = true;
                }
                hcVmList.Add(hcvm);
            }
            return View(hcVmList);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(List<HolidayCalenderViewModel> newHolidayList, FormCollection formcollection)
        {

            List<HolidayCalenderViewModel> SelectedholidayCalender = new List<HolidayCalenderViewModel>();
            var existingHolidays = _HolidayCalenderService.GetAllIQueryable();
            TempData["SearchDate"] = formcollection["FromDate"];
            if (formcollection.Get("btnSave") != null)
            {
                SelectedholidayCalender = newHolidayList.Where(i => i.IsSelect).ToList();
                var Sysinfo = _SysServie.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);

                DateTime date = DateTime.MinValue;
                foreach (var item in SelectedholidayCalender)
                {
                    date = Convert.ToDateTime(item.Date);
                    if (date < DateRange.Item1)
                    {
                        AddToastMessage("", date.ToString("MMM yyyy") + " is already finalized.", ToastType.Error);
                        return RedirectToAction("Index");
                    }
                }
                foreach (var item in SelectedholidayCalender)
                {
                    date = Convert.ToDateTime(item.Date);
                    if (existingHolidays.Any(i => i.Date == date))
                    {
                        AddToastMessage("", item.Date + " is already assigned.", ToastType.Error);
                        return RedirectToAction("Index");
                    }
                }

                var newholiday = _mapper.Map<List<HolidayCalenderViewModel>, List<HolidayCalender>>(SelectedholidayCalender);
                foreach (var item in newholiday)
                {
                    item.CreatedBy = User.Identity.GetUserId<int>();
                    item.CreatedDate = DateTime.Now;
                    item.ConcernID = User.Identity.GetConcernId();
                    item.Status = (int)EnumActiveInactive.Active;
                    item.Description = ((EnumHolidayType)item.Type).ToString();
                    _HolidayCalenderService.Add(item);
                }
                _HolidayCalenderService.Save();
                AddToastMessage("", "Save Successfull.", ToastType.Success);
            }
            else if (formcollection.Get("btnRemove") != null)
            {
                SelectedholidayCalender = newHolidayList.Where(i => i.IsSelect).ToList();
                var Sysinfo = _SysServie.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(Sysinfo.NextPayProcessDate);

                DateTime date = DateTime.MinValue;
                foreach (var item in SelectedholidayCalender)
                {
                    date = Convert.ToDateTime(item.Date);
                    if (date < DateRange.Item1)
                    {
                        AddToastMessage("", date.ToString("MMM yyyy") + " is already finalized.", ToastType.Error);
                        return RedirectToAction("Index");
                    }
                }

                foreach (var item in SelectedholidayCalender)
                {
                    date = Convert.ToDateTime(item.Date);
                    if (!existingHolidays.Any(i => i.Date == date))
                    {
                        AddToastMessage("", item.Date + " is not assigned.", ToastType.Error);
                        return RedirectToAction("Index");
                    }
                }
                HolidayCalender holidayCalender = null;
                var newholiday = _mapper.Map<List<HolidayCalenderViewModel>, List<HolidayCalender>>(SelectedholidayCalender);
                foreach (var item in newholiday)
                {
                    holidayCalender = _HolidayCalenderService.GetAllIQueryable().FirstOrDefault(i => i.Date == item.Date);
                    if (holidayCalender != null)
                        _HolidayCalenderService.Delete(holidayCalender.ID);
                }
                _HolidayCalenderService.Save();
                AddToastMessage("", "Remove Successfull.", ToastType.Success);
            }
            return RedirectToAction("Index");

        }

        //
        // GET: /HolidayCalender/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /HolidayCalender/Edit/5
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

        //
        // GET: /HolidayCalender/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /HolidayCalender/Delete/5
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
    }
}
