using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("SystemInformation")]
    public class SystemInformationController : CoreController
    {
        ISystemInformationService _systemInformationService;
        IMapper _mapper;

        public SystemInformationController(IErrorService errorService,
            ISystemInformationService systemInformationService, IMapper mapper)
            : base(errorService, systemInformationService)
        {
            _systemInformationService = systemInformationService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var systemInformation = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var vmodel = _mapper.Map<SystemInformation, CreateSystemInformationViewModel>(systemInformation);
            return View(vmodel);
        }
        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSystemInformationViewModel NewsystemInformation, string returnUrl, HttpPostedFileBase image, HttpPostedFileBase Brand)
        {
            TempData["IsConfig"] = false;
            if (!ModelState.IsValid)
                return View("Index", NewsystemInformation);

            if (NewsystemInformation != null)
            {
                var sysinfo = _systemInformationService.GetSystemInformationById(int.Parse(NewsystemInformation.Id));
                sysinfo.Address = NewsystemInformation.Address;
                sysinfo.ConcernID = User.Identity.GetConcernId();
                sysinfo.CustomerNIDPatht = NewsystemInformation.CustomerNIDPatht;
                sysinfo.CustomerPhotoPath = NewsystemInformation.CustomerPhotoPath;
                sysinfo.EmailAddress = NewsystemInformation.EmailAddress;
                sysinfo.EmployeePhotoPath = NewsystemInformation.EmployeePhotoPath;
                sysinfo.Name = NewsystemInformation.Name;
                sysinfo.ProductPhotoPath = NewsystemInformation.ProductPhotoPath;
                sysinfo.SupplierDocPath = NewsystemInformation.SupplierDocPath;
                sysinfo.SupplierPhotoPath = NewsystemInformation.SupplierPhotoPath;
                sysinfo.SystemStartDate = NewsystemInformation.SystemStartDate;
                sysinfo.TelephoneNo = NewsystemInformation.TelephoneNo;
                sysinfo.WebAddress = NewsystemInformation.WebAddress;
                sysinfo.APIKey = NewsystemInformation.APIKey;
                sysinfo.DeviceIP = NewsystemInformation.DeviceIP;
                sysinfo.DeviceSerialNO = NewsystemInformation.DeviceSerialNO;
                sysinfo.CompanyTitle = NewsystemInformation.CompanyTitle;


                if (image != null)
                {
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };
                    if (image.ContentLength > 0 && sAllowedExt.Contains(image.FileName.Substring(image.FileName.LastIndexOf('.')).ToLower()))
                    {
                        sysinfo.LogoMimeType = image.ContentType;
                        sysinfo.CompanyLogo = new byte[image.ContentLength];
                        image.InputStream.Read(sysinfo.CompanyLogo, 0, image.ContentLength);
                    }
                    else
                        AddToastMessage("", "Logo type is not supported. Supported types are .jpg, .jpeg, .gif, .png", ToastType.Error);
                }
                if (Brand != null)
                {
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };
                    if (Brand.ContentLength > 0 && sAllowedExt.Contains(Brand.FileName.Substring(Brand.FileName.LastIndexOf('.')).ToLower()))
                    {
                        sysinfo.BLogoMimeType = Brand.ContentType;
                        sysinfo.BrandLogo = new byte[Brand.ContentLength];
                        Brand.InputStream.Read(sysinfo.BrandLogo, 0, Brand.ContentLength);
                    }
                    else
                        AddToastMessage("", "Logo type is not supported. Supported types are .jpg, .jpeg, .gif, .png", ToastType.Error);
                }

                _systemInformationService.UpdateSystemInformation(sysinfo);
                _systemInformationService.SaveSystemInformation();
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult GenerateSHA256String()
        {
            return Json(GenerateCoupon(), JsonRequestBehavior.AllowGet);
        }
        public string GenerateCoupon()
        {
            int length = 50;
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }

        [HttpGet]
        public FileContentResult GetImage(int Id)
        {
            var SysInfo = _systemInformationService.GetSystemInformationById(Id);
            if (SysInfo != null && SysInfo.CompanyLogo != null && !string.IsNullOrEmpty(SysInfo.LogoMimeType))
            {
                return File(SysInfo.CompanyLogo, SysInfo.LogoMimeType);
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public FileContentResult GetBrand(int Id)
        {
            var SysInfo = _systemInformationService.GetSystemInformationById(Id);
            if (SysInfo != null && SysInfo.BrandLogo != null && !string.IsNullOrEmpty(SysInfo.BLogoMimeType))
            {
                return File(SysInfo.BrandLogo, SysInfo.BLogoMimeType);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Config(CreateSystemInformationViewModel NewsystemInformation, string returnUrl, HttpPostedFileBase image, HttpPostedFileBase Brand)
        {
            TempData["IsConfig"] = true;

            if (NewsystemInformation != null)
            {
                var sysinfo = _systemInformationService.GetSystemInformationById(int.Parse(NewsystemInformation.Id));
                sysinfo.SMSServiceEnable = NewsystemInformation.SMSServiceEnable ? 1 : 0;
                sysinfo.SMSProviderID = (int)NewsystemInformation.SMSProviderID;
                sysinfo.DaysBeforeSendSMS = NewsystemInformation.DaysBeforeSendSMS;
                sysinfo.InsuranceContactNo = NewsystemInformation.InsuranceContactNo;
                sysinfo.SMSSendToOwner = NewsystemInformation.SMSSendToOwner ? 1 : 0;
                sysinfo.CustomerSmsWithCustomerName = NewsystemInformation.CustomerSmsWithCustomerName ? 1 : 0;
                sysinfo.ExpireDate = NewsystemInformation.ExpireDate;
                sysinfo.ExpireMessage = NewsystemInformation.ExpireMessage;
                sysinfo.WarningMsg = NewsystemInformation.WarningMsg;
                sysinfo.BarcodeSize = NewsystemInformation.BarcodeSize;
                sysinfo.CustomerDueLimitApply = NewsystemInformation.CustomerDueLimitApply ? 1 : 0;
                sysinfo.EmployeeDueLimitApply = NewsystemInformation.EmployeeDueLimitApply ? 1 : 0;
                sysinfo.IsSalesPPDiscountShow = NewsystemInformation.IsSalesPPDiscountShow ? 1 : 0;
                sysinfo.ApprovalSystemEnable = NewsystemInformation.ApprovalSystemEnable ? 1 : 0;
                sysinfo.IsEditApprovalSystemEnable = NewsystemInformation.IsEditApprovalSystemEnable ? 1 : 0;
                sysinfo.SenderId = NewsystemInformation.SenderId;
                sysinfo.smsCharge = NewsystemInformation.SmsCharge;
                sysinfo.CompanyURL = NewsystemInformation.CompanyURL;
                sysinfo.ConcernNameB = NewsystemInformation.ConcernNameB;
                sysinfo.IsMrpUpdateShowForManager = NewsystemInformation.IsMrpUpdateShowForManager ? 1 : 0;
                sysinfo.IsAutoCreditInterestPer = NewsystemInformation.IsAutoCreditInterestPer ? 1 : 0;
                sysinfo.IsExpenseHeadEnable = NewsystemInformation.IsExpenseHeadEnable ? 1 : 0;
                sysinfo.IsBankEnable = NewsystemInformation.IsBankEnable ? 1 : 0;
                sysinfo.IsColorEnable = NewsystemInformation.IsColorEnable ? 1 : 0;
                sysinfo.IsCommonBank = NewsystemInformation.IsCommonBank;
                sysinfo.TramsAndCondition = NewsystemInformation.TramsAndCondition;
                sysinfo.IsBankBalanceShow = NewsystemInformation.IsBankBalanceShow ? 1 : 0;
                sysinfo.BackDateEntry = NewsystemInformation.BackDateEntry;
                sysinfo.IsParentCateShow = NewsystemInformation.IsParentCateShow ? 1 : 0;
                sysinfo.IsRPRateShow = NewsystemInformation.IsRPRateShow ? 1 : 0;
                sysinfo.IsEmobileCustomerView = NewsystemInformation.IsEmobileCustomerView ? 1 : 0;
                sysinfo.IsEditReqPermission = NewsystemInformation.IsEditReqPermission ? 1 : 0;
                sysinfo.IsHoqueSOrderSalesRate = NewsystemInformation.IsHoqueSOrderSalesRate ? 1 : 0;
                sysinfo.IsEcomputerShow = NewsystemInformation.IsEcomputerShow ? 1 : 0;
                sysinfo.IsProductTypeHide = NewsystemInformation.IsProductTypeHide ? 1 : 0;
                sysinfo.IsCCDebitCreditAdjHide = NewsystemInformation.IsCCDebitCreditAdjHide ? 1 : 0;
                sysinfo.IsDOShow = NewsystemInformation.IsDOShow ? 1 : 0;
                sysinfo.IsBankApprovalEnable = NewsystemInformation.IsBankApprovalEnable ? 1 : 0;
                sysinfo.isposinvoice = NewsystemInformation.isposinvoice ? 1 : 0;
                sysinfo.isposmoneyrecipt = NewsystemInformation.isposmoneyrecipt ? 1 : 0;
                sysinfo.IsDateWiseProductPicker = NewsystemInformation.IsDateWiseProductPicker ? 1 : 0;

                _systemInformationService.UpdateSystemInformation(sysinfo);
                _systemInformationService.SaveSystemInformation();
                Session["SystemInfo"] = sysinfo;
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

    }
}