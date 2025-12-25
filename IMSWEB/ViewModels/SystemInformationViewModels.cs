using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateSystemInformationViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string TelephoneNo { get; set; }

        public string EmailAddress { get; set; }

        public string WebAddress { get; set; }

        public DateTime SystemStartDate { get; set; }

        public string ProductPhotoPath { get; set; }

        public string SupplierPhotoPath { get; set; }

        public string CustomerPhotoPath { get; set; }

        public string CustomerNIDPatht { get; set; }

        public string SupplierDocPath { get; set; }

        public string EmployeePhotoPath { get; set; }

        public string ConcernNameB { get; set; }

        public int ConcernID { get; set; }
        [Display(Name = "Device IP")]
        public string DeviceIP { get; set; }

        [Display(Name = "Device SN")]
        public string DeviceSerialNO { get; set; }
        public string APIKey { get; set; }

        [Display(Name = "Is Customer Due Limit Applicable")]
        public bool CustomerDueLimitApply { get; set; }

        [Display(Name = "CustomerSmsWithCustomerName")]
        public bool CustomerSmsWithCustomerName { get; set; }

        [Display(Name = "Is Employee Due Limit Applicable")]
        public bool EmployeeDueLimitApply { get; set; }

        [Display(Name = "Enable SMS Service")]
        public bool SMSServiceEnable { get; set; }

        [Display(Name = "Owner Mobile")]
        public string InsuranceContactNo { get; set; }
        public int DaysBeforeSendSMS { get; set; }

        [Display(Name = "SMS Provider")]
        public EnumSMSServiceProvider SMSProviderID { get; set; }
        [Display(Name = "Logo")]
        public byte[] CompanyLogo { get; set; }
        public string LogoMimeType { get; set; }

        [Display(Name = "Brand Logo")]
        public byte[] BrandLogo { get; set; }
        public string BLogoMimeType { get; set; }

        [Display(Name = "SMS Send To Owner")]
        public bool SMSSendToOwner { get; set; }

        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "Expire Message")]
        public string ExpireMessage { get; set; }
        public string WarningMsg { get; set; }

        public EnumBarcodeSize BarcodeSize { get; set; }

        [Display(Name = "Enable Sales PP Discount")]
        public bool IsSalesPPDiscountShow { get; set; }

        [Display(Name = "Enable Auto Credit Int. Per")]
        public bool IsAutoCreditInterestPer { get; set; }

        [Display(Name = "Is Approval Enable")]
        public bool ApprovalSystemEnable { get; set; }

        [Display(Name = "Is Edit Approval Enable")]
        public bool IsEditApprovalSystemEnable { get; set; }

        [Display(Name = "MRP Update Enable")]
        public bool IsMrpUpdateShowForManager { get; set; }

        public string SenderId { get; set; }

        [Display(Name = "Sms Cost")]
        public decimal SmsCharge { get; set; }

        [Display(Name = "URL Link")]
        public string CompanyURL { get; set; }

        [Display(Name = "Enable Expense Head SC")]
        public bool IsExpenseHeadEnable { get; set; }

        [Display(Name = "Enable Bank Enable For  Sister Concern")]
        public bool IsBankEnable { get; set; }

        [Display(Name = "Enable Color Enable For Sister Concern")]
        public bool IsColorEnable { get; set; }

        [Display(Name = "Company Title")]
        public string CompanyTitle { get; set; }
        [Display(Name = "Enable Common Bank")]
        public bool IsCommonBank { get; set; }
        public bool TramsAndCondition { get; set; }
        [Display(Name = "Is Bank Balance Show")]
        public bool IsBankBalanceShow { get; set; }

        [Display(Name = "Back Date Period")]
        public int BackDateEntry { get; set; }
        [Display(Name = "Is Parent Category Show")]
        public bool IsParentCateShow { get; set; }
        [Display(Name = "Is RP Rate Show")]
        public bool IsRPRateShow { get; set; }
        [Display(Name = "Is Emobile Customer View")]
        public bool IsEmobileCustomerView { get; set; }
        [Display(Name = "Is Edit Req. Permission")]
        public bool IsEditReqPermission { get; set; }
        [Display(Name = "Is Hoque SOrder Sales Rate")]
        public bool IsHoqueSOrderSalesRate { get; set; }
        [Display(Name = "Is CC Debit Credit Adj. Hide")]
        public bool IsCCDebitCreditAdjHide { get; set; }
        [Display(Name = "Is Ecomputer Data Show")]
        public bool IsEcomputerShow { get; set; }
        [Display(Name = "Is Product Type Hide")]
        public bool IsProductTypeHide { get; set; }
        [Display(Name = "Is DO Show")]
        public bool IsDOShow { get; set; }
        [Display(Name = "Is Bank Approval Enable")]
        public bool IsBankApprovalEnable { get; set; }
        [Display(Name = "Is Pos Invoice")]
        public bool isposinvoice { get; set; }
        [Display(Name = "Is Pos Money Recipt")]
        public bool isposmoneyrecipt { get; set; }
        [Display(Name = "Is Date Wise Product Picker")]
        public bool IsDateWiseProductPicker { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateSystemInformationViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
    public class CreateSMSSettingViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Is Bangla SMS Service eEnable")]
        public bool IsBanglaSmsEnable { get; set; }

        [Display(Name = "Is Retail Sale SMS Service Applicable")]
        public bool RetailSaleSmsService { get; set; }

        [Display(Name = "Is Hire Sale SMS Service Applicable")]
        public bool HireSaleSmsService { get; set; }

        [Display(Name = "Is Cash Collection SMS Service Applicable")]
        public bool CashCollectionSmsService { get; set; }

        [Display(Name = "Is Installment SMS Service Applicable")]
        public bool InstallmentSmsService { get; set; }

        [Display(Name = "Is Remind Date SMS Service Applicable")]
        public bool RemindDateSmsService { get; set; }
    }



}