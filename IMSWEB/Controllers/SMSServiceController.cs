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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Text;
using System.Data;

namespace IMSWEB.Controllers
{
    [RoutePrefix("SMSService")]
    public class SMSServiceController : CoreController
    {
        ISMSStatusService _SMSStatusService;
        IMiscellaneousService<SMSStatus> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _SysInfoService;
        ICustomerService _CustomerService;
        ISisterConcernService _SisterConcernService;
        ISystemInformationService _sysInfoService;
        ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        public SMSServiceController(IErrorService errorService,
            ISMSStatusService colorService, IMiscellaneousService<SMSStatus> miscellaneousService, IMapper mapper,
             ISystemInformationService SysInfoService, ISisterConcernService SisterConcernService, ICustomerService CustomerService, ISystemInformationService sysInfoService,ISMSBillPaymentBkashService sMSBillPaymentBkashService
            )
            : base(errorService, sysInfoService)
        {
            _SMSStatusService = colorService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SysInfoService = SysInfoService;
            _CustomerService = CustomerService;
            _SisterConcernService = SisterConcernService;
            _smsBillPaymentBkashService = sMSBillPaymentBkashService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var sms = _SMSStatusService.GetAll(ViewBag.FromDate, ViewBag.ToDate, 0);
            var vmsms = _mapper.Map<IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>>, IEnumerable<SMSStatusViewModel>>(sms);
            return View(vmsms);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            int Status = 0;
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            if (!string.IsNullOrEmpty(formCollection["Status"]))
                Status = Convert.ToInt32(formCollection["Status"]);
            var sms = _SMSStatusService.GetAll(ViewBag.FromDate, ViewBag.ToDate, Status);
            var vmsms = _mapper.Map<IEnumerable<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>>, IEnumerable<SMSStatusViewModel>>(sms);
            return View(vmsms);
        }

        [HttpGet]
        public ActionResult SendSMS()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendSMS(FormCollection formcollection)
        {
            List<Customer> CustomerList = new List<Customer>();
            List<SMSRequest> smsList = new List<SMSRequest>();
            SMSRequest sms = null;
            string SMS = string.Empty;

            List<int> CustomerIdList = new List<int>();
            int CustomerType = 0;
            if (!string.IsNullOrEmpty(formcollection["SMS"]))
                SMS = formcollection["SMS"];
            if (!string.IsNullOrEmpty(formcollection["CustomerType"]))
                CustomerType = Convert.ToInt32(formcollection["CustomerType"]);
            if (!string.IsNullOrEmpty(formcollection["CustomerIdList"]))
                CustomerIdList = formcollection["CustomerIdList"].ToString().Split(',').Select(Int32.Parse).ToList();

            if (CustomerIdList.Count > 0)
                CustomerList = _CustomerService.GetAll().Where(i => CustomerIdList.Contains(i.CustomerID)).ToList();
            else if (CustomerType > 0)
                CustomerList = _CustomerService.GetAll().Where(i => i.CustomerType == (EnumCustomerType)CustomerType).ToList();
            else
                AddToastMessage("", "Please Select Customer first", ToastType.Error);

            var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            foreach (var item in CustomerList)
            {
                sms = new SMSRequest();
                sms.CustomerID = item.CustomerID;
                sms.MobileNo = item.ContactNo;
                sms.SMS = SMS;
                sms.SMSType = EnumSMSType.Offer;
                smsList.Add(sms);
            }
            decimal counter = smsList.Count();

            if (counter > 0)
            {
                if (SystemInfo.IsBanglaSmsEnable > 0)
                {
                    if (counter > 1)
                    {
                        decimal count = Math.Ceiling(Convert.ToDecimal(counter / 5));
                        for (int i = 0; i < count; i++)
                        {
                            var GroupSms = smsList.Skip(i * 5).Take(5).ToList();
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            var response = await Task.Run(() => SMSHTTPServiceBangla.SendSMSAsync(EnumOnnoRokomSMSType.ListSms, GroupSms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                }
                                #region udpate payment info                 
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion
                                response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(response);
                                _SMSStatusService.Save();
                            }

                            AddToastMessage("", "Message Sent Successfull.", ToastType.Success);

                        }
                    }
                    else
                    {

                        decimal count = Math.Ceiling(Convert.ToDecimal(counter / 5));
                        for (int i = 0; i < count; i++)
                        {
                            var GroupSms = smsList.Skip(i * 5).Take(5).ToList();
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            var response = await Task.Run(() => SMSHTTPServiceBangla.SendSMSAsync(EnumOnnoRokomSMSType.NumberSms, GroupSms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                }
                                #region udpate payment info                 
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion
                                response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(response);
                                _SMSStatusService.Save();
                            }

                            AddToastMessage("", "Message Sent Successfull.", ToastType.Success);

                        }

                    }

                }
                else
                {
                    #region English SMS 
                    if (counter > 1)
                    {
                        decimal count = Math.Ceiling(Convert.ToDecimal(counter / 5));
                        for (int i = 0; i < count; i++)
                        {
                            var GroupSms = smsList.Skip(i * 5).Take(5).ToList();
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            var response = await Task.Run(() => SMSHTTPService.SendSMSAsync(EnumOnnoRokomSMSType.ListSms, GroupSms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                }
                                #region udpate payment info                 
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion
                                response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(response);
                                _SMSStatusService.Save();
                            }

                            AddToastMessage("", "Message Sent Successfull.", ToastType.Success);

                        }
                    }
                    else
                    {

                        decimal count = Math.Ceiling(Convert.ToDecimal(counter / 5));
                        for (int i = 0; i < count; i++)
                        {
                            var GroupSms = smsList.Skip(i * 5).Take(5).ToList();
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            var response = await Task.Run(() => SMSHTTPService.SendSMSAsync(EnumOnnoRokomSMSType.NumberSms, GroupSms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in response)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                }
                                #region udpate payment info                 
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion
                                response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(response);
                                _SMSStatusService.Save();
                            }

                            AddToastMessage("", "Message Sent Successfull.", ToastType.Success);

                        }

                    }

                }

                #endregion 

            }

            return View();
        }

        [HttpGet]
        public ActionResult SMSReport()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View();
        }
        void PopulateConcernsDropdown()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetFamilyTree(User.Identity.GetConcernId()), "ConcernID", "Name");
        }
        [HttpGet]
        public ActionResult AdminSMSReport()
        {
            PopulateConcernsDropdown();
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateSMSStaus()
        {
            var sysInfo = _SysInfoService.GetAllConcernSysInfo().FirstOrDefault();
            var PendingSMS = (from s in _SMSStatusService.GetAllConcern()
                              where (s.Code.Equals("ACCEPTD") || s.Code.Equals("SENT") || s.Code.Equals("PENDING"))
                              && s.Message_ID != null
                              select new SMSRequest
                              {
                                  Message_ID = s.Message_ID,
                                  MobileNo = "00",
                                  SMSType = (EnumSMSType)s.SMSFormateID
                              }).Take(20).ToList();

            if (PendingSMS.Count() > 0)
            {

                int concernId = User.Identity.GetConcernId();
                decimal previousBalance;
                SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                previousBalance = smsAmountDetails.TotalRecAmt;

                var response = SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.REVEGETStatus, PendingSMS, previousBalance, sysInfo, 0);
                foreach (var item in response)
                {
                    var sms = _SMSStatusService.GetAllConcern().FirstOrDefault(i => i.Message_ID == item.Message_ID);
                    if (sms != null)
                    {
                        sms.Code = item.Code;
                        sms.SendingStatus = item.SendingStatus;
                        sms.EntryDate = item.EntryDate;
                        _SMSStatusService.Update(sms);
                    }
                    _SMSStatusService.Save();
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        [Route("settingindex")]
        public ActionResult SettingIndex()
        {
            var systemInformation = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var vmodel = _mapper.Map<SystemInformation, CreateSMSSettingViewModel>(systemInformation);
            return View(vmodel);
        }


        [HttpPost]
        [Authorize]
        [Route("settingedit/returnUrl")]
        public ActionResult SettingEdit(CreateSMSSettingViewModel NewsystemInformation, string returnUrl, HttpPostedFileBase image, HttpPostedFileBase Brand)
        {
            if (!ModelState.IsValid)
                return View("SettingIndex", NewsystemInformation);

            if (NewsystemInformation != null)
            {
                var sysinfo = _SysInfoService.GetSystemInformationById(int.Parse(NewsystemInformation.Id));

                sysinfo.IsBanglaSmsEnable = NewsystemInformation.IsBanglaSmsEnable ? 1 : 0;
                sysinfo.IsRetailSMSEnable = NewsystemInformation.RetailSaleSmsService ? 1 : 0;
                sysinfo.IsHireSMSEnable = NewsystemInformation.HireSaleSmsService ? 1 : 0;
                sysinfo.IsCashcollSMSEnable = NewsystemInformation.CashCollectionSmsService ? 1 : 0;
                sysinfo.IsInstallmentSMSEnable = NewsystemInformation.InstallmentSmsService ? 1 : 0;
                sysinfo.IsRemindSMSEnable = NewsystemInformation.RemindDateSmsService ? 1 : 0;

                _SysInfoService.UpdateSystemInformation(sysinfo);
                _SysInfoService.SaveSystemInformation();
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("SettingIndex");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("SettingIndex");
            }

        }
        #region Bulk SMS Excel
        [HttpGet]
        public ActionResult SendBulkSMS()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendBulkSMS(HttpPostedFileBase file, string SMS)
        {
            if (file == null)
            {
                AddToastMessage("", "No File Found. Please select a file.", ToastType.Info);
                return View();
            }
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                if (Request.Files["file"].ContentLength > 0)
                {
                    #region Save file
                    //string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    //if (System.IO.File.Exists(fileLocation))
                    //{
                    //    try
                    //    {
                    //        System.IO.File.Delete(fileLocation);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        AddToastMessage("", "Input file is used by another process. Error: " + ex.ToString());
                    //        return View();
                    //    }
                    //}
                    //try
                    //{
                    //    Request.Files["file"].SaveAs(fileLocation);
                    //}
                    //catch (Exception ex)
                    //{
                    //    AddToastMessage("", "Input file is used by another process. Error: " + ex.ToString());
                    //    return View();
                    //}
                    #endregion

                    #region open connection
                    //string excelConnectionString = string.Empty;
                    //excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    //fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    ////connection String for xls file format.
                    //if (fileExtension == ".xls")
                    //{
                    //    //excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    //    //fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    //    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";

                    //}
                    ////connection String for xlsx file format.
                    //else if (fileExtension == ".xlsx")
                    //{
                    //    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    //    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //}
                    ////Create Connection to Excel work book and add oledb namespace
                    //OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    //try
                    //{
                    //    excelConnection.Open();
                    //}
                    //catch (Exception ex)
                    //{
                    //    AddToastMessage("", "Could not open connection.", ToastType.Info);
                    //    return View();
                    //}
                    #endregion

                    #region Reading data
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    StringBuilder mobileNumbers = new StringBuilder();
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheets currentSheet = package.Workbook.Worksheets;
                        ExcelWorksheet workSheet = currentSheet.First();
                        int noOfRow = workSheet.Dimension.End.Row;

                        try
                        {
                            for (int i = 2; i <= noOfRow; i++)
                            {
                                string number = workSheet.Cells[i, 2].Value == null ? "" : workSheet.Cells[i, 2].Value.ToString().Trim(new[] { '\r', '\n', '-', '\"', ' ', '\t' });
                                if (!string.IsNullOrEmpty(number))
                                    mobileNumbers.Append(number + ",");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                    string[] finalNumbers = mobileNumbers.ToString().Split(',');
                    if (finalNumbers != null && finalNumbers.Length > 0)
                    {
                        var systemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                        if(systemInfo.IsBanglaSmsEnable > 0)
                        {
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfo.smsCharge;
                            List<SMSStatus> reponseList = SMSHTTPServiceBangla.SendBulkSMS(systemInfo, User.Identity.GetUserId<int>(), finalNumbers, SMS, previousBalance);
                            if (reponseList.Any())
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in reponseList)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    if (item.NoOfSMS == 0)
                                    {
                                        AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                                    }

                                }
                                #region udpate payment info                  
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = previousBalance - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion

                                reponseList.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(reponseList);
                                _SMSStatusService.Save();
                                AddToastMessage("", "SMS successfully sent.", ToastType.Info);
                            }
                        }
                        else
                        {
                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance = 0m;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfo.smsCharge;
                            List<SMSStatus> reponseList = SMSHTTPService.SendBulkSMS(systemInfo, User.Identity.GetUserId<int>(), finalNumbers, SMS, previousBalance);

                            if (reponseList.Any())
                            {
                                decimal smsBalanceCount = 0m;
                                foreach (var item in reponseList)
                                {
                                    smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    if (item.NoOfSMS == 0)
                                    {
                                    AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                                    }

                                }
                                #region udpate payment info                  
                                decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                smsAmountDetails.TotalRecAmt = previousBalance - Convert.ToDecimal(sysLastPayUpdateDate);
                                _smsBillPaymentBkashService.Update(smsAmountDetails);
                                _smsBillPaymentBkashService.Save();
                                #endregion

                                reponseList.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                _SMSStatusService.AddRange(reponseList);
                                _SMSStatusService.Save();
                                AddToastMessage("", "SMS successfully sent.", ToastType.Info);
                            }
                        }

                    }
                    else
                    {
                        AddToastMessage("", "No number found to send sms!", ToastType.Info);
                    }

                    #endregion
                }
            }
            else
            {
                AddToastMessage("", "Please select a valid excel file.", ToastType.Info);
                return View();
            }

            return View();
        }

        [HttpGet]
        public ActionResult DownloadExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            workSheet.Cells["A1"].Value = "SL.";
            workSheet.Cells["B1"].Value = "Mobile Number";
            workSheet.Column(2).Style.Numberformat.Format = "@";
            using (ExcelRange rng = workSheet.Cells[("A1:" + "B1")])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            workSheet.Cells[1, 1, 1, 2].AutoFitColumns();

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=BulkMobileNumberTemplate.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                Response.Clear();
                Response.ClearContent();
            }

            return RedirectToAction("SendBulkSMS");
        }

        #endregion
    }
}