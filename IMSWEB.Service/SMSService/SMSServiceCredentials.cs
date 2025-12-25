using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    #region SMS service provider
    public enum EnumSMSServiceProvider
    {
        [Display(Name = "Onnorokom")]
        Onnorokom_SMSServiceProvider = 1,
        [Display(Name = "AlphaNet")]
        AlphaNet_SMSServiceProvider = 2,
        [Display(Name = "REVE")]
        REVE_SMSServiceProvider = 3,
        [Display(Name = "Mobi Shastra")]
        Mobi_SMSServiceProvider = 4,
        [Display(Name = "Mobi Shastra Non Masking")]
        Mobi_Non_Masking_SMSServiceProvider = 5,
    }
    #endregion

    public static class SMSServiceCredentials
    {

        #region Onnorokom SMS service Credentials
        public static readonly string ONNOROKOM_UserName = "01724939433";
        public static readonly string ONNOROKOM_Password = "e47925";
        public static readonly string ONNOROKOM_APIKEY = "b0a90c98-fe9b-4747-b537-b14b38c56611";
        public static readonly string ONNOROKOM_BASE_URL = "https://api2.onnorokomsms.com/HttpSendSms.ashx?";
        #endregion

        #region Alpha net SMS service Credentials
        public static readonly string AlphaNet_UserName = "masudoct";
        public static readonly string AlphaNet_Password = "01724939433";
        public static readonly string AlphaNet_HashToken = "3d5547ff01e6267f6b30404f97f42a4d";
        public static readonly string AlphaNet_BASE_URL = "http://alphasms.biz/index.php?";
        #endregion

        #region REVE SMS service Credentials
        public static readonly string REVE_UserName = "oct";
        public static readonly string REVE_Password = "oct@123";
        public static readonly string REVE_CallerID = "8809612770444";
        public static readonly string REVE_APIKey = "689e72c0803220fe";
        public static readonly string REVE_SecretKey = "457551f4";
        public static readonly string REVE_BASE_URL = "https://smpp.ajuratech.com:7790/";
        #endregion

        #region Mobi Shastro SMS service Credentials for Masking
        public static readonly string Mobi_UserName = "OCTMAS";
        public static readonly string Mobi_Password = "s6h94e6j";
        public static readonly string Mobi_Priority = "High";
        public static readonly string Mobi_CountryCode = "+880";
        public static readonly string Mobi_BASE_URL = "https://mshastra.com/sendurl.aspx?";
        public static readonly string Mobi_DLR_BASE_URL = "https://mshastra.com/dlrstatus_api.aspx?";
        #endregion

        #region Mobi Shastro SMS service Credentials for non Masking
        public static readonly string Mobi_nonMasking_UserName = "20102151";
        public static readonly string Mobi_nonMasking_Password = "OCT@321";
        #endregion



        #region songbirdsms SMS service Credentials
        //public static readonly string songbirdsms_UserName = "oct";
        //public static readonly string songbirdsms_Password = "oct@123";
        public static readonly string songbirdsms_CallerID = "8801719996279";
        public static readonly string songbirdsms_APIKey = "1643e75836636262";
        public static readonly string songbirdsms_SecretKey = "3252724b";
        //public static readonly string songbirdsms_BASE_URL = "https://smpp.ajuratech.com:7790/"; 
        #endregion


        public static readonly int IntervalInSeconds = 30;

    }

    public class REVEResponse
    {
        public string Status { get; set; }
        public string Text { get; set; }
        public string Message_ID { get; set; }

        [Column("Delivery Time")]
        public DateTime DeliveryTime { get; set; }
    }


}
