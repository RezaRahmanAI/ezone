using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Jil;
using System.IO;
using IMSWEB.Model;
using System.Text.RegularExpressions;
using System.Net;

namespace IMSWEB.Service
{
    public static class SMSHTTPServiceBangla
    {

        public static async Task<List<SMSStatus>> SendSMSAsync(EnumOnnoRokomSMSType smsType, List<SMSRequest> smsList, decimal smsBalance, SystemInformation sysInfo, int UserID, bool isMultiple = false)
        {
            return await Task.Run(() => SendSMS(smsType, smsList, smsBalance, sysInfo, UserID, isMultiple));
        }

        #region send greetins for payment via oct
        public static List<SMSStatus> SendGreetings(EnumOnnoRokomSMSType smsType, List<SMSRequest> smsList, int UserID)
        {
            List<SMSStatus> smsResponses = new List<SMSStatus>();

            StringBuilder SMS = new StringBuilder();
            string result = string.Empty;
            string MobileNo = string.Empty;
            string BASE_URL = string.Empty;
            SMSRequest SMSInfo = null;
            int Counter = 0, Length = 0; ;

            HttpResponseMessage response;

            try
            {
                foreach (var item in smsList)
                {
                    if (item.MobileNo.Contains("/"))
                    {
                        item.MobileNo = item.MobileNo.Split('/')[0];
                    }
                    item.MobileNo = item.MobileNo.Replace("-", string.Empty);
                    if (item.MobileNo.StartsWith("1"))
                    {
                        item.MobileNo = "0" + item.MobileNo;
                    }
                }


                switch (smsType)
                {
                    case EnumOnnoRokomSMSType.OneToOne:
                        break;
                    case EnumOnnoRokomSMSType.OneToMany:
                        break;
                    case EnumOnnoRokomSMSType.DeliveryStatus:
                        break;
                    case EnumOnnoRokomSMSType.GetBalance:
                        break;
                    case EnumOnnoRokomSMSType.NumberSms:
                        if (smsList.Count() == 0)
                            return smsResponses;

                        SMS = GreetingsSMS(smsList, ref SMSInfo);

                        foreach (var item in smsList)
                        {
                            Counter++;
                            if (IsMobileNoValid(item.MobileNo))
                            {
                                if (smsList.Count != Counter)
                                    MobileNo = MobileNo + item.MobileNo + ",";
                                else
                                    MobileNo = MobileNo + item.MobileNo;
                            }
                            else
                                smsResponses.AddRange(AddValidationError(item.MobileNo, SMS.ToString(), UserID));
                        }

                        #region Mobi Shastra

                        foreach (var item in smsList)
                        {
                            //if (!IsMobileNoValid(MobileNo))
                            //return AddValidationError(MobileNo, SMS.ToString(), UserID);

                            if (!IsMobileNoValid(item.MobileNo))
                            {
                                MobileNo = item.MobileNo;
                                AddValidationError(item.MobileNo, SMS.ToString(), UserID);
                                continue;
                            }

                            //if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                            //    BASE_URL = string.Format("{0}user={1}&pwd={2}&senderId={3}&mobileno={4}&msgtext={5}&priority={6}&CountryCode={7}", SMSServiceCredentials.Mobi_BASE_URL, SMSServiceCredentials.Mobi_nonMasking_UserName, SMSServiceCredentials.Mobi_nonMasking_Password, sysInfo.SenderId, item.MobileNo, SMS, SMSServiceCredentials.Mobi_Priority, SMSServiceCredentials.Mobi_CountryCode);
                            //else
                            BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, SMSServiceCredentials.songbirdsms_CallerID);

                            //biplob
                            response = new HttpClient().GetAsync(BASE_URL + "," + item.MobileNo + "," + SMS).Result;
                            result = response.Content.ReadAsStringAsync().Result;

                            //string fullurl = BASE_URL + "&toUser=" + item.MobileNo + "&messageContent=" + SMS;

                            string[] apiResponse = result.Split(',');


                            #region response
                            //API Response: {"Status":"0","Text":"ACCEPTD","Message_ID":"194234"} 
                            #endregion

                            //if (apiResponse[0] != null)
                            //{
                            //    var smsResponse = new SMSStatus();
                            //    smsResponse.Code = "OK";
                            //    smsResponse.Message_ID = Convert.ToInt32(apiResponse[0]);
                            //    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                            //    smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                            //    smsResponse.ResponseMsg = "API Response: " + result;
                            //    smsResponse.SMS = SMS.ToString();
                            //    smsResponse.CreatedDate = DateTime.Now;
                            //    smsResponse.EntryDate = DateTime.Today;
                            //    smsResponse.CreatedBy = UserID;
                            //    smsResponse.CustomerID = SMSInfo.CustomerID;
                            //    smsResponse.ContactNo = item.MobileNo;//MobileNo;
                            //    if (smsResponse.Code.Equals("OK"))
                            //        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                            //    else
                            //        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                            //    smsResponses.Add(smsResponse);
                            //}

                        }
                        #endregion

                        break;
                    case EnumOnnoRokomSMSType.ListSms:
                        break;
                    case EnumOnnoRokomSMSType.GetCurrentBalance:
                        break;
                    case EnumOnnoRokomSMSType.REVEGETStatus:
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                //var smsResponse = new SMSStatus();
                //smsResponse.Code = "0000";
                //smsResponse.ResponseMsg = "API Response: " + result + Environment.NewLine + "Error: " + ex.Message;
                //smsResponse.SMSFormateID = (int)EnumSMSType.Error;
                //smsResponse.NoOfSMS = 0;
                //smsResponse.SMS = SMS.ToString();
                //smsResponse.CreatedDate = DateTime.Now;
                //smsResponse.EntryDate = DateTime.Today;
                //smsResponse.CreatedBy = UserID;
                //smsResponse.ContactNo = MobileNo;
                //smsResponse.CustomerID = 0;
                //smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                //smsResponses.Add(smsResponse);
                //return smsResponses;
            }
            return smsResponses;
        }

        private static StringBuilder GreetingsSMS(List<SMSRequest> smsList, ref SMSRequest SMSInfo)
        {
            SMSInfo = smsList.FirstOrDefault();
            StringBuilder SMS = new StringBuilder();
            if (SMSInfo != null)
            {
                SMS.Append("Dear Customer, We have received your payment. Your service has been extended for one month. Thank you for being with us." + Environment.NewLine);

            }
            return SMS;
        }

        #endregion

        public static List<SMSStatus> SendSMS(EnumOnnoRokomSMSType smsType, List<SMSRequest> smsList, decimal smsBalance, SystemInformation sysInfo, int UserID, bool isMultiple = false)
        {
            List<SMSStatus> smsResponses = new List<SMSStatus>();
            if (sysInfo.SMSServiceEnable == 0 && smsType != EnumOnnoRokomSMSType.REVEGETStatus)
                return smsResponses;
            StringBuilder SMS = new StringBuilder();
            string result = string.Empty;
            string MobileNo = string.Empty;
            string BASE_URL = string.Empty;
            SMSRequest SMSInfo = null;
            int Counter = 0, Length = 0;
            HttpResponseMessage response;
            try
            {
                foreach (var item in smsList)
                {
                    if (item.MobileNo.Contains("/"))
                    {
                        item.MobileNo = item.MobileNo.Split('/')[0];
                    }
                    item.MobileNo = item.MobileNo.Replace("-", string.Empty);
                    if (item.MobileNo.StartsWith("1"))
                    {
                        item.MobileNo = "0" + item.MobileNo;
                    }
                }
                switch (smsType)
                {
                    #region One To One username and password
                    case EnumOnnoRokomSMSType.OneToOne:

                        SMS = GenerateSMS(smsList, sysInfo, ref SMSInfo);
                        MobileNo = SMSInfo.MobileNo;

                        if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Onnorokom_SMSServiceProvider)
                        {
                            #region Onnorokom SMS

                            #region URL
                            //https://api2.onnorokomsms.com/HttpSendSms.ashx?op=OneToOne&type=TEXT&mobile=0170000000
                            //&smsText=Your Text&username=username&password=password&maskName=&campaignName=
                            #endregion

                            BASE_URL = new Uri(SMSServiceCredentials.ONNOROKOM_BASE_URL + "op=" + smsType + "&type=TEXT").ToString();

                            response = new HttpClient().GetAsync(BASE_URL + "&mobile=" + MobileNo + "&smsText=" + SMS + "&username=" + SMSServiceCredentials.ONNOROKOM_UserName + "&password=" + SMSServiceCredentials.ONNOROKOM_Password + "&maskName=&campaignName=").Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            //var obj = JObject.Parse(result);
                            #region response
                            //Success response: 1900||01762125041||100782732/
                            #endregion

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = result.Substring(0, result.IndexOf("||"));
                                smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                smsResponse.ResponseMsg = result;
                                smsResponse.SMS = SMS.ToString();
                                smsResponse.CreatedDate = DateTime.Now;
                                smsResponse.EntryDate = DateTime.Today;
                                smsResponse.CreatedBy = UserID;
                                smsResponse.CustomerID = SMSInfo.CustomerID;
                                smsResponse.ContactNo = MobileNo;
                                if (smsResponse.Code.Equals("1900"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }
                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.AlphaNet_SMSServiceProvider)
                        {
                            #region Alphnet

                            #region URL
                            //http://alphasms.biz/index.php?app=ws&u=YOUR_API_USERNAME&h=YOUR_API_HASH_TOKEN&op=pv&to=01979547393&msg=test+only
                            #endregion

                            if (!IsMobileNoValid(MobileNo))
                                return AddValidationError(MobileNo, SMS.ToString(), UserID);

                            BASE_URL = new Uri(SMSServiceCredentials.AlphaNet_BASE_URL + "app=ws&u=" + SMSServiceCredentials.AlphaNet_UserName + "&h=" + SMSServiceCredentials.AlphaNet_HashToken + "&op=pv").ToString();

                            response = new HttpClient().GetAsync(BASE_URL + "&to=" + MobileNo + "&msg=" + SMS).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            var obj = JSON.DeserializeDynamic(result, Jil.Options.ISO8601);

                            #region response
                            //API Response: {"data":[{"status":"OK","error":"0","smslog_id":"832111","queue":"457c20337522a0154454e5189622ae93","to":"8801762125041"}],"error_string":null,"timestamp":1578911622}
                            #endregion

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = obj.data[0].status;
                                smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                smsResponse.ResponseMsg = "API Response: " + result;
                                smsResponse.SMS = SMS.ToString();
                                smsResponse.CreatedDate = DateTime.Now;
                                smsResponse.EntryDate = DateTime.Today;
                                smsResponse.CreatedBy = UserID;
                                smsResponse.CustomerID = SMSInfo.CustomerID;
                                smsResponse.ContactNo = MobileNo;
                                if (smsResponse.Code.Equals("OK"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }
                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.REVE_SMSServiceProvider)
                        {
                            #region REVE

                            #region URL
                            //https://smpp.ajuratech.com:7790/sendtext?apikey=API_KEY&secretkey=SECRET_KEY&callerID=SENDER_ID&toUser=MOBILE_NUMBER&messageContent=MESSAGE 
                            #endregion

                            if (!IsMobileNoValid(MobileNo))
                                return AddValidationError(MobileNo, SMS.ToString(), UserID);

                            BASE_URL = new Uri(SMSServiceCredentials.REVE_BASE_URL + "sendtext?apikey=" + SMSServiceCredentials.REVE_APIKey + "&secretkey=" + SMSServiceCredentials.REVE_SecretKey + "&callerID=" + SMSServiceCredentials.REVE_CallerID).ToString();

                            response = new HttpClient().GetAsync(BASE_URL + "&toUser=" + MobileNo + "&messageContent=" + SMS).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            var obj = JSON.DeserializeDynamic(result, Jil.Options.ISO8601);

                            #region response
                            //API Response: {"Status":"0","Text":"ACCEPTD","Message_ID":"194234"} 
                            #endregion

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = obj.Text;
                                smsResponse.Message_ID = obj.Message_ID;
                                smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                smsResponse.ResponseMsg = "API Response: " + result;
                                smsResponse.SMS = SMS.ToString();
                                smsResponse.CreatedDate = DateTime.Now;
                                smsResponse.EntryDate = DateTime.Today;
                                smsResponse.CreatedBy = UserID;
                                smsResponse.CustomerID = SMSInfo.CustomerID;
                                smsResponse.ContactNo = MobileNo;
                                if (smsResponse.Code.Equals("OK"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }
                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider || sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                        {
                            #region SongBird

                            #region URL
                            //https://smpp.ajuratech.com:7790/sendtext?apikey=API_KEY&secretkey=SECRET_KEY&callerID=SENDER_ID&toUser=MOBILE_NUMBER&messageContent=MESSAGE 
                            #endregion

                            if (!IsMobileNoValid(MobileNo))
                                return AddValidationError(MobileNo, SMS.ToString(), UserID);
                            if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                                BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, SMSServiceCredentials.songbirdsms_CallerID, MobileNo, SMS);
                            else
                                BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, sysInfo.SenderId, MobileNo, SMS);
                            response = new HttpClient().GetAsync(BASE_URL).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            //string[] apiResponse = result.Split(',');
                            var obj = JSON.Deserialize<REVEResponse>(result, Jil.Options.ISO8601);

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = "OK";
                                smsResponse.Message_ID = int.Parse(obj.Message_ID);
                                smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                smsResponse.ResponseMsg = "API Response: " + result;
                                smsResponse.SMS = SMS.ToString();
                                smsResponse.CreatedDate = DateTime.Now;
                                smsResponse.EntryDate = DateTime.Today;
                                smsResponse.CreatedBy = UserID;
                                smsResponse.CustomerID = SMSInfo.CustomerID;
                                smsResponse.ContactNo = MobileNo;
                                if (smsResponse.Code.Equals("OK"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }

                            #endregion
                        }
                        break;
                    #endregion

                    #region One to Many API
                    case EnumOnnoRokomSMSType.NumberSms: //One to Many API
                        #region URL
                        // https://api2.onnorokomsms.com/HttpSendSms.ashx?op=NumberSms&apiKey=ApiKey&type=TEXT
                        //&mobile=0170000000, 0170000001&smsText=YourText&maskName=&campaignName= 
                        #endregion
                        if (smsList.Count() == 0)
                            return smsResponses;

                        SMSInfo = smsList.FirstOrDefault();
                        if (SMSInfo.SMSType != EnumSMSType.Offer)
                        {
                            SMS = GenerateSMS(smsList, sysInfo, ref SMSInfo);
                        }
                        else
                        {
                            SMS.Append(SMSInfo.SMS);
                            if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider && !string.IsNullOrEmpty(sysInfo.CompanyURL))
                                SMS.Append(Environment.NewLine + "Details: " + sysInfo.CompanyURL);
                            else if (sysInfo.SMSProviderID != (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider)
                                SMS.Append(Environment.NewLine + "Thank you." + Environment.NewLine + sysInfo.Name);

                        }

                        foreach (var item in smsList)
                        {
                            Counter++;
                            if (IsMobileNoValid(item.MobileNo))
                            {
                                if (smsList.Count != Counter)
                                    MobileNo = MobileNo + item.MobileNo + ",";
                                else
                                    MobileNo = MobileNo + item.MobileNo;
                            }
                            else
                                smsResponses.AddRange(AddValidationError(item.MobileNo, SMS.ToString(), UserID));
                        }

                        if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Onnorokom_SMSServiceProvider)
                        {
                            #region Onnorokom SMS
                            BASE_URL = new Uri(SMSServiceCredentials.ONNOROKOM_BASE_URL + "op=" + smsType + "&apiKey=" + SMSServiceCredentials.ONNOROKOM_APIKEY + "&type=TEXT").ToString();

                            response = new HttpClient().PostAsync(BASE_URL + "&mobile=" + MobileNo + "&smsText=" + SMS + "&maskName=&campaignName=", null).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            //var obj = JObject.Parse(result);
                            //Response: 1900||01714||39.../1900||017...||39.../
                            if (result != null)
                            {
                                string[] IndividulsResponse = result.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < IndividulsResponse.Length; i++)
                                {
                                    var smsResponse = new SMSStatus();

                                    smsResponse.Code = IndividulsResponse[i].Substring(0, IndividulsResponse[i].IndexOf("||"));
                                    Length = (IndividulsResponse[i].LastIndexOf("||") - IndividulsResponse[i].IndexOf("||")) - 2;
                                    MobileNo = IndividulsResponse[i].Substring(IndividulsResponse[i].IndexOf("||") + 2, Length);
                                    SMSInfo = smsList.FirstOrDefault(c => c.MobileNo.Contains(MobileNo.Trim()));

                                    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                    smsResponse.ResponseMsg = IndividulsResponse[i];
                                    smsResponse.SMS = SMS.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = SMSInfo.CustomerID;
                                    smsResponse.ContactNo = MobileNo;
                                    if (smsResponse.Code.Equals("1900"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }

                            }
                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.AlphaNet_SMSServiceProvider)
                        {
                            #region Alphnet

                            #region URL
                            //http://alphasms.biz/index.php?app=ws&u=YOUR_API_USERNAME&h=YOUR_API_HASH_TOKEN&op=pv&to=01979547393&msg=test+only
                            #endregion

                            BASE_URL = new Uri(SMSServiceCredentials.AlphaNet_BASE_URL + "app=ws&u=" + SMSServiceCredentials.AlphaNet_UserName + "&h=" + SMSServiceCredentials.AlphaNet_HashToken + "&op=pv").ToString();

                            response = new HttpClient().GetAsync(BASE_URL + "&to=" + MobileNo + "&msg=" + SMS).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            var obj = JSON.DeserializeDynamic(result, Jil.Options.ISO8601);
                            #region response
                            //API Response: [{"status":"OK","error":"0","smslog_id":"1538396","queue":"7353fb6cef0e9b43793cefce31146f99","to":"8801714006441"}
                            //,{"status":"OK","error":"0","smslog_id":"1538397","queue":"7353fb6cef0e9b43793cefce31146f99","to":"8801762125041"}]
                            //,"error_string":null,"timestamp":1597920391}
                            #endregion

                            if (result != null)
                            {
                                for (int i = 0; i < obj.data.Length; i++)
                                {
                                    var smsResponse = new SMSStatus();
                                    smsResponse.Code = obj.data[i].status;
                                    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                    smsResponse.ResponseMsg = "API Response: " + obj.data[i];
                                    smsResponse.SMS = SMS.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = SMSInfo.CustomerID;
                                    smsResponse.ContactNo = obj.data[i].to;
                                    if (smsResponse.Code.Equals("OK"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }
                            }
                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.REVE_SMSServiceProvider)
                        {
                            #region REVE

                            #region URL
                            //https://smpp.ajuratech.com:7790/sendtext?apikey=API_KEY&secretkey=SECRET_KEY&callerID=SENDER_ID&toUser=MOBILE_NUMBER&messageContent=MESSAGE 
                            #endregion

                            if (!IsMobileNoValid(MobileNo))
                                return AddValidationError(MobileNo, SMS.ToString(), UserID);

                            BASE_URL = new Uri(SMSServiceCredentials.REVE_BASE_URL + "sendtext?apikey=" + SMSServiceCredentials.REVE_APIKey + "&secretkey=" + SMSServiceCredentials.REVE_SecretKey + "&callerID=" + SMSServiceCredentials.REVE_CallerID).ToString();

                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            response = new HttpClient().GetAsync(BASE_URL + "&toUser=" + MobileNo + "&messageContent=" + SMS).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            var obj = JSON.Deserialize<REVEResponse>(result, Jil.Options.ISO8601);

                            #region response
                            //API Response: {"Status":"0","Text":"ACCEPTD","Message_ID":"194234"} 
                            #endregion

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = obj.Text;
                                smsResponse.Message_ID = int.Parse(obj.Message_ID);
                                smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                smsResponse.ResponseMsg = "API Response: " + result;
                                smsResponse.SMS = SMS.ToString();
                                smsResponse.CreatedDate = DateTime.Now;
                                smsResponse.EntryDate = DateTime.Today;
                                smsResponse.CreatedBy = UserID;
                                smsResponse.CustomerID = SMSInfo.CustomerID;
                                smsResponse.ContactNo = MobileNo;
                                if (smsResponse.Code.Equals("ACCEPTD"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Pending;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }
                            #endregion
                        }

                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider || sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                        {
                            #region Mobi Shastra
                            //if (!IsMobileNoValid(MobileNo))
                            //    return AddValidationError(MobileNo, SMS.ToString(), UserID);


                            //if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                            //    BASE_URL = string.Format("{0}user={1}&pwd={2}&senderId={3}&mobileno={4}&msgtext={5}&priority={6}&CountryCode={7}", SMSServiceCredentials.Mobi_BASE_URL, SMSServiceCredentials.Mobi_nonMasking_UserName, SMSServiceCredentials.Mobi_nonMasking_Password, sysInfo.SenderId, MobileNo, SMS, SMSServiceCredentials.Mobi_Priority, SMSServiceCredentials.Mobi_CountryCode);
                            //if (!IsMobileNoValid(MobileNo))
                            //    return AddValidationError(MobileNo, SMS.ToString(), UserID);

                            if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                                BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, SMSServiceCredentials.songbirdsms_CallerID, MobileNo, SMS);
                            else
                                BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, sysInfo.SenderId, MobileNo, SMS);

                            response = new HttpClient().GetAsync(BASE_URL).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            //string[] apiResponse = result.Split(',');
                            var obj = JSON.Deserialize<REVEResponse>(result, Jil.Options.ISO8601);
                            #region response
                            //API Response: {"Status":"0","Text":"ACCEPTD","Message_ID":"194234"} 
                            #endregion

                            if (result != null)
                            {
                                string messageId = obj.Message_ID.ToString();
                                string[] messageIds = messageId.Split(',');
                                string mobile = MobileNo.ToString();
                                string[] mobileNumber = mobile.Split(',');
                                for (int i = 0; i < mobileNumber.Length; i++)
                                {
                                    if (mobileNumber.Length > 1 && mobileNumber[i] != null)
                                    {
                                        obj.Message_ID = messageIds[i];
                                    }

                                    var smsResponse = new SMSStatus();
                                    smsResponse.Code = "OK";
                                    smsResponse.Message_ID = int.Parse(obj.Message_ID);
                                    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                    smsResponse.ResponseMsg = "API Response: " + result;
                                    smsResponse.SMS = SMS.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = SMSInfo.CustomerID;
                                    if (mobileNumber.Length > 1 && mobileNumber[i] != null)
                                    {
                                        smsResponse.ContactNo = mobileNumber[i];
                                    }
                                    else
                                    {
                                        smsResponse.ContactNo = mobileNumber[0];
                                    }

                                    if (smsResponse.Code.Equals("OK"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }

                            }
                            #endregion
                        }
                        break;
                    #endregion

                    #region Many to Many API
                    case EnumOnnoRokomSMSType.ListSms: //Many to Many API

                        if (smsList.Count == 0)
                            return smsResponses;

                        var jsonList = from sms in smsList
                                       select new
                                       {
                                           MobileNumber = sms.MobileNo,
                                           ////SmsText = "Dear Mr./Mrs. " + sms.CustomerName + "," + Environment.NewLine + "your due payment Date is " + DateddMMMYYYY(sms.Date) + " and due amout is " + sms.PresentDue + ". Thank you." + Environment.NewLine + sysInfo.Name,
                                           //SmsText = "This is to remind you that payment date of installment for " + sms.CustomerCode + " is " + FormatDatedTodMMMYYYY(sms.Date) + ". The installment amount is " + sms.PresentDue + ". Thank you." + Environment.NewLine + sysInfo.Name,
                                           //Type = "TEXT"

                                           //SmsText = "Dear Mr./Mrs. " + sms.CustomerName + "," + Environment.NewLine + "your due payment Date is " + DateddMMMYYYY(sms.Date) + " and due amout is " + sms.PresentDue + ". Thank you." + Environment.NewLine + sysInfo.Name,
                                           SmsText = sms.SMS + ". Thank you." + Environment.NewLine + sysInfo.Name,
                                           Type = "TEXT"
                                       };
                        if (isMultiple)
                        {
                            jsonList = from sms in smsList
                                       select new
                                       {
                                           MobileNumber = sms.MobileNo,
                                           SmsText = sms.SMS,
                                           Type = "TEXT"
                                       };
                        }

                        if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Onnorokom_SMSServiceProvider)
                        {
                            #region Onnorokom SMS

                            #region URL
                            //https://api2.onnorokomsms.com/HttpSendSms.ashx?op=ListSms&apiKey=ApiKey&type=TEXT
                            //&smsListJson=[{"MobileNumber":"017000000","SmsText":"Individual List SMS is ok","Type":"TEXT"},{"MobileNumber":"0180000000","SmsText":"Individual List SMS2 is ok","Type":"TEXT"}]&maskName=&campaignName=
                            #endregion

                            using (var output = new StringWriter())
                            {
                                JSON.SerializeDynamic(jsonList, output);
                                SMS.Append(output.ToString());
                            }

                            BASE_URL = new Uri(SMSServiceCredentials.ONNOROKOM_BASE_URL + "op=" + smsType + "&apiKey=" + SMSServiceCredentials.ONNOROKOM_APIKEY + "&type=TEXT").ToString();

                            response = new HttpClient().PostAsync(BASE_URL + "&smsListJson=" + SMS + "&maskName=&campaignName=", null).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            Length = 0;
                            if (result != null)
                            {
                                string[] IndividualsResponse = result.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < IndividualsResponse.Length; i++)
                                {
                                    var smsResponse = new SMSStatus();

                                    smsResponse.Code = IndividualsResponse[i].Substring(0, IndividualsResponse[i].IndexOf("||"));

                                    Length = (IndividualsResponse[i].LastIndexOf("||") - IndividualsResponse[i].IndexOf("||")) - 2;
                                    MobileNo = IndividualsResponse[i].Substring(IndividualsResponse[i].IndexOf("||") + 2, Length);

                                    SMSInfo = smsList.FirstOrDefault(c => c.MobileNo.Contains(MobileNo.Trim()));
                                    if (SMSInfo != null)
                                    {
                                        smsResponse.CustomerID = SMSInfo.CustomerID;
                                        smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    }
                                    else
                                    {
                                        smsResponse.SMSFormateID = (int)smsList.FirstOrDefault().SMSType;
                                    }
                                    var sms = jsonList.FirstOrDefault(c => c.MobileNumber.Contains(MobileNo.Trim()));
                                    if (sms != null)
                                    {
                                        smsResponse.SMS = sms.SmsText;
                                    }
                                    smsResponse.NoOfSMS = SMSCounter(smsResponse.SMS);
                                    smsResponse.ResponseMsg = IndividualsResponse[i];
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.ContactNo = MobileNo;
                                    if (smsResponse.Code.Equals("1900"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }
                            }

                            #endregion
                        }
                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.AlphaNet_SMSServiceProvider)
                        {
                            #region Alphnet

                            #region URL
                            //http://alphasms.biz/index.php?app=ws&u=YOUR_API_USERNAME&h=YOUR_API_HASH_TOKEN&op=pv&to=01979547393&msg=test+only
                            #endregion

                            BASE_URL = new Uri(SMSServiceCredentials.AlphaNet_BASE_URL + "app=ws&u=" + SMSServiceCredentials.AlphaNet_UserName + "&h=" + SMSServiceCredentials.AlphaNet_HashToken + "&op=pv").ToString();
                            foreach (var item in jsonList)
                            {
                                if (!IsMobileNoValid(item.MobileNumber))
                                {
                                    smsResponses.AddRange(AddValidationError(item.MobileNumber, item.SmsText, UserID));
                                    continue;
                                }
                                response = new HttpClient().GetAsync(BASE_URL + "&to=" + item.MobileNumber + "&msg=" + item.SmsText).Result;
                                result = response.Content.ReadAsStringAsync().Result;
                                var obj = JSON.DeserializeDynamic(result, Jil.Options.ISO8601);

                                #region response
                                //API Response: {"data":[{"status":"OK","error":"0","smslog_id":"832111","queue":"457c20337522a0154454e5189622ae93","to":"8801762125041"}],"error_string":null,"timestamp":1578911622}
                                #endregion

                                if (result != null)
                                {
                                    var smsResponse = new SMSStatus();
                                    SMSInfo = smsList.FirstOrDefault(c => c.MobileNo.Contains(item.MobileNumber.Trim()));

                                    smsResponse.Code = obj.data[0].status;
                                    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    smsResponse.NoOfSMS = SMSCounter(item.SmsText.ToString());
                                    smsResponse.ResponseMsg = "API Response: " + result;
                                    smsResponse.SMS = item.SmsText.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = SMSInfo.CustomerID;
                                    smsResponse.ContactNo = MobileNo;
                                    if (smsResponse.Code.Equals("OK"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }
                            }

                            #endregion
                        }

                        else if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider || sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                        {
                            #region Song Bird

                            foreach (var item in jsonList)
                            {
                                if (!IsMobileNoValid(item.MobileNumber))
                                {
                                    smsResponses.AddRange(AddValidationError(item.MobileNumber, item.SmsText, UserID));
                                    continue;
                                }

                                if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                                    BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, SMSServiceCredentials.songbirdsms_CallerID, item.MobileNumber, item.SmsText);
                                else
                                    BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, sysInfo.SenderId, item.MobileNumber, item.SmsText);


                                response = new HttpClient().GetAsync(BASE_URL).Result;
                                result = response.Content.ReadAsStringAsync().Result;
                                var obj = JSON.Deserialize<REVEResponse>(result, Jil.Options.ISO8601);

                                if (result != null)
                                {
                                    var smsResponse = new SMSStatus();
                                    SMSInfo = smsList.FirstOrDefault(c => c.MobileNo.Contains(item.MobileNumber.Trim()));
                                    smsResponse.Code = "successful";
                                    smsResponse.Message_ID = int.Parse(obj.Message_ID);
                                    smsResponse.SMSFormateID = (int)SMSInfo.SMSType;
                                    smsResponse.NoOfSMS = SMSCounter(item.SmsText.ToString());
                                    smsResponse.ResponseMsg = "API Response: " + result;
                                    smsResponse.SMS = item.SmsText.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = SMSInfo.CustomerID;
                                    smsResponse.ContactNo = item.MobileNumber;
                                    if (smsResponse.Code.ToLower().Contains("successful"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }

                            }
                            #endregion
                        }
                        break;
                    #endregion

                    case EnumOnnoRokomSMSType.REVEGETStatus:

                        #region REVE
                        #region URL
                        //http://smpp.ajuratech.com:7788/getstatus?apikey=API_KEY&secretkey=SECRET_KEY&messageid=MESSAGE_ID 
                        #endregion

                        BASE_URL = new Uri(SMSServiceCredentials.REVE_BASE_URL + "getstatus?apikey=" + SMSServiceCredentials.REVE_APIKey + "&secretkey=" + SMSServiceCredentials.REVE_SecretKey).ToString();

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        foreach (var item in smsList)
                        {
                            response = new HttpClient().GetAsync(BASE_URL + "&messageid=" + item.Message_ID).Result;
                            result = response.Content.ReadAsStringAsync().Result;
                            var obj = JSON.Deserialize<REVEResponse>(result, Jil.Options.ISO8601);

                            #region response
                            //API Response: {"Status": "0","Text": "DELIVRD","Message_ID": "194234","Delivery Time":  "1586420396602"} 
                            #endregion

                            if (result != null)
                            {
                                var smsResponse = new SMSStatus();
                                smsResponse.Code = "DELIVRD";
                                smsResponse.Message_ID = item.Message_ID;
                                smsResponse.EntryDate = obj.DeliveryTime == DateTime.MinValue ? DateTime.Today : obj.DeliveryTime;
                                if (smsResponse.Code.Equals("DELIVRD"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                else if (smsResponse.Code.Equals("SENT"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Pending;
                                else if (smsResponse.Code.Equals("PENDING"))
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Pending;
                                else
                                    smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                smsResponses.Add(smsResponse);
                            }
                        }

                        #endregion
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var smsResponse = new SMSStatus();
                smsResponse.Code = "0000";
                smsResponse.ResponseMsg = "API Response: " + result + Environment.NewLine + "Error: " + ex.Message;
                smsResponse.SMSFormateID = (int)EnumSMSType.Error;
                smsResponse.NoOfSMS = 0;
                smsResponse.SMS = SMS.ToString();
                smsResponse.CreatedDate = DateTime.Now;
                smsResponse.EntryDate = DateTime.Today;
                smsResponse.CreatedBy = UserID;
                smsResponse.ContactNo = MobileNo;
                smsResponse.CustomerID = 0;
                smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                smsResponses.Add(smsResponse);
                return smsResponses;
            }

            return smsResponses;
        }

        private static StringBuilder GenerateSMS(List<SMSRequest> smsList, SystemInformation sysInfo, ref SMSRequest SMSInfo)
        {
            SMSInfo = smsList.FirstOrDefault();
            StringBuilder SMS = new StringBuilder();
            if (SMSInfo != null)
            {
                //MobileNo = SMSInfo.MobileNo;
                SMSInfo.CustomerCode = SMSInfo.CustomerCode.Trim();
                //SMS.Append(sysInfo.Name + Environment.NewLine);
                if (SMSInfo.SMSType == EnumSMSType.CashCollection)
                {
                    SMS.Append("Receive Amt: " + SMSInfo.ReceiveAmount.ToString("F2") + Environment.NewLine);
                    SMS.Append("Present Due: " + SMSInfo.PresentDue.ToString("F2") + Environment.NewLine);
                    //SMS.Append("Date: " + FormatDatedTodMMMYYYY(SMSInfo.Date) + Environment.NewLine);
                    SMS.Append("প্রিয় গ্রাহক, যথাসময়ে বাকি পরিশোধের জন্য ওয়ালটন এর পক্ষ হতে আপনাকে ধন্যবাদ।" + Environment.NewLine);
                    //SMS.Append("Pre. Due: " + SMSInfo.PreviousDue.ToString("F2") + Environment.NewLine);

                }
                else if (SMSInfo.SMSType == EnumSMSType.SalesTime)
                {
                    //string ProductName = string.Join(",", SMSInfo.ProductNameList);
                    SMS.Append("Payment:" + SMSInfo.ReceiveAmount.ToString("F2") + Environment.NewLine);
                    SMS.Append("Present Due:" + SMSInfo.PresentDue.ToString("F2") + Environment.NewLine);
                    SMS.Append("প্রিয় গ্রাহক, ওয়ালটন পণ্য ক্রয়ের জন্য ওয়ালটন এর পক্ষ হতে আপনাকে ধন্যবাদ।" + Environment.NewLine);
                    //SMS.Append("Date:" + FormatDatedTodMMMYYYY(SMSInfo.Date) + Environment.NewLine);
                    //SMS.Append("Inv.No:" + SMSInfo.TransNumber + Environment.NewLine);
                    //SMS.Append(SMSInfo.CustomerCode + Environment.NewLine);
                    //SMS.Append("Model: " + ProductName + Environment.NewLine);
                    //SMS.Append("Sales Price:" + SMSInfo.SalesAmount.ToString("F2") + Environment.NewLine);
                    //SMS.Append("Pre.Due:" + SMSInfo.PreviousDue.ToString("F2") + Environment.NewLine);
                    //SMS.Append("Paid Date: " + SMSInfo.PresentDue + Environment.NewLine);
                }
                else if (SMSInfo.SMSType == EnumSMSType.InstallmentCollection)
                {
                    //SMS.Append("প্রিয় গ্রাহক,আপনার কিস্তি পরিশোধের তারিখঃ: " + FormatDatedTodMMMYYYY(SMSInfo.Date) + Environment.NewLine);
                    //SMS.Append("A/C No: " + SMSInfo.CustomerCode + Environment.NewLine);
                    SMS.Append("Installment: " + SMSInfo.ReceiveAmount.ToString("F2") + Environment.NewLine);
                    //SMS.Append("Cash to Date: " + SMSInfo.TotalReceiveAmount.ToString("F2") + Environment.NewLine);
                    SMS.Append("প্রিয় গ্রাহক, যথাসময়ে কিস্তি পরিশোধের জন্য ওয়ালটন এর পক্ষ হতে আপনাকে ধন্যবাদ। " + Environment.NewLine);
                }
                else if (SMSInfo.SMSType == EnumSMSType.Registration)
                {
                    //MobileNo = sysInfo.InsuranceContactNo;
                    SMS.Append(SMSInfo.CustomerName + ", S/O ");
                    SMS.Append(SMSInfo.CustomerFatherName + ", ");
                    SMS.Append(SMSInfo.CustomerAddress + "." + Environment.NewLine);
                    string EngineNo = string.Empty;
                    string ChasisNo = string.Empty;
                    foreach (var item in SMSInfo.ProductDetailList)
                    {
                        EngineNo = item.EngineNo.Length >= 7 ? item.EngineNo.Substring(item.EngineNo.Length - 7) : item.EngineNo; //Take Last 7 Digits
                        ChasisNo = item.ChasisNo.Length >= 6 ? item.ChasisNo.Substring(item.ChasisNo.Length - 6) : item.ChasisNo;//Take Last 6 Digits
                        SMS.Append(item.ProductName + ", " + item.ColorName + ", EN-" + EngineNo + ", CH-" + ChasisNo + Environment.NewLine);
                    }
                }
                if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider && !string.IsNullOrEmpty(sysInfo.CompanyURL))
                    SMS.Append("Details: " + sysInfo.CompanyURL);
                else if (sysInfo.SMSProviderID != (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider)
                    SMS.Append("Thank you." + Environment.NewLine + sysInfo.Name);
            }
            return SMS;
        }

        public static List<SMSStatus> SendBulkSMS(SystemInformation sysInfo, int UserID, string[] mobileNumbers, string message, decimal smsBalance)
        {
            List<SMSStatus> smsResponses = new List<SMSStatus>();
            if (sysInfo.SMSServiceEnable == 0)
                return smsResponses;

            HttpResponseMessage response;

            StringBuilder SMS = new StringBuilder();
            string result = string.Empty;
            string BASE_URL = string.Empty;
            SMS.Append(message);
            if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider && !string.IsNullOrEmpty(sysInfo.CompanyURL))
                SMS.Append(Environment.NewLine + "Details: " + sysInfo.CompanyURL);
            else if (sysInfo.SMSProviderID != (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider)
                SMS.Append(Environment.NewLine + "Thank you." + Environment.NewLine + sysInfo.Name);
            try
            {
                if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_SMSServiceProvider || sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                {
                    if (mobileNumbers != null && mobileNumbers.Length > 0)
                    {
                        for (int i = 0; i < mobileNumbers.Length; i++)
                        {
                            if (mobileNumbers[i].Contains("/"))
                            {
                                mobileNumbers[i] = mobileNumbers[i].Split('/')[0];
                            }
                            mobileNumbers[i] = mobileNumbers[i].Replace("-", string.Empty);
                            if (mobileNumbers[i].StartsWith("1"))
                            {
                                mobileNumbers[i] = "0" + mobileNumbers[i];
                            }

                            if (!string.IsNullOrEmpty(mobileNumbers[i]))
                            {
                                #region invalid phone number
                                if (!IsMobileNoValid(mobileNumbers[i]))
                                {
                                    smsResponses.AddRange(AddValidationError(mobileNumbers[i], SMS.ToString(), UserID));
                                    continue;
                                }
                                #endregion

                                #region Mobi Shastra
                                var NoOfSMS = SMSCounter(SMS.ToString());

                                if (sysInfo.SMSProviderID == (int)EnumSMSServiceProvider.Mobi_Non_Masking_SMSServiceProvider)
                                    BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, SMSServiceCredentials.songbirdsms_CallerID, mobileNumbers[i], SMS);
                                else
                                    BASE_URL = string.Format("http://103.53.84.15:8746/sendtext?apikey={0}&secretkey={1}&callerID={2}&toUser={3}&messageContent={4}", SMSServiceCredentials.songbirdsms_APIKey, SMSServiceCredentials.songbirdsms_SecretKey, sysInfo.SenderId, mobileNumbers[i], SMS);


                                response = new HttpClient().GetAsync(BASE_URL).Result;
                                result = response.Content.ReadAsStringAsync().Result;
                                string[] apiResponse = result.Split(',');

                                if (apiResponse[0] != null)
                                {
                                    //BASE_URL = string.Format("{0}user={1}&pwd={2}&messageid={3}", SMSServiceCredentials.Mobi_DLR_BASE_URL, SMSServiceCredentials.Mobi_UserName, SMSServiceCredentials.Mobi_Password, apiResponse[0]);

                                    //response = new HttpClient().GetAsync(BASE_URL).Result;
                                    string delarResult = response.Content.ReadAsStringAsync().Result;
                                    string delarResultUp = delarResult;
                                    string[] dealarResponse = delarResult.Split(',');
                                    var obj = JSON.Deserialize<REVEResponse>(delarResultUp, Jil.Options.ISO8601);

                                    var smsResponse = new SMSStatus();
                                    smsResponse.Code = "OK";
                                    smsResponse.Message_ID = int.Parse(obj.Message_ID);
                                    //smsResponse.SMSFormateID = (int)EnumSMSType.Promotional;
                                    smsResponse.SMSFormateID = (int)EnumSMSType.Offer;
                                    smsResponse.NoOfSMS = SMSCounter(SMS.ToString());
                                    smsResponse.ResponseMsg = "API Response: " + delarResult;
                                    smsResponse.SMS = SMS.ToString();
                                    smsResponse.CreatedDate = DateTime.Now;
                                    smsResponse.EntryDate = DateTime.Today;
                                    smsResponse.CreatedBy = UserID;
                                    smsResponse.CustomerID = 0;
                                    smsResponse.ContactNo = mobileNumbers[i];
                                    if (!dealarResponse[1].ToLower().Contains("fail"))
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Success;
                                    else
                                        smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
                                    smsResponses.Add(smsResponse);
                                }
                                #endregion

                            }
                        }
                    }


                }
            }
            catch (Exception)
            {

            }
            return smsResponses;
        }



        static int SMSCounter(string SMS)
        {
            if (string.IsNullOrEmpty(SMS))
                return 0;
            int Counter = 0;
            decimal temp = (decimal)SMS.Length / 60;
            int Interger_digit = (int)Math.Floor(temp);
            decimal Fractional_digit = temp - Interger_digit;
            Counter = Interger_digit + (Fractional_digit > 0 ? 1 : 0);
            return Counter;
        }


        static string FormatDatedTodMMMYYYY(DateTime dateTime)
        {
            return dateTime.ToString("dd MMM yyyy");
        }

        static List<SMSStatus> AddValidationError(string MobileNo, string SMS, int UserID)
        {
            List<SMSStatus> smsResponses = new List<SMSStatus>();

            var smsResponse = new SMSStatus();
            smsResponse.Code = "0000";
            smsResponse.ResponseMsg = "Invalid Destination";
            smsResponse.SMSFormateID = (int)EnumSMSType.Error;
            smsResponse.NoOfSMS = 0;
            smsResponse.SMS = SMS.ToString();
            smsResponse.CreatedDate = DateTime.Now;
            smsResponse.EntryDate = DateTime.Today;
            smsResponse.CreatedBy = UserID;
            smsResponse.ContactNo = MobileNo;
            smsResponse.CustomerID = 0;
            smsResponse.SendingStatus = (int)EnumSMSSendStatus.Fail;
            smsResponses.Add(smsResponse);
            return smsResponses;
        }

        static bool IsMobileNoValid(string MobileNo)
        {
            if (Regex.IsMatch(MobileNo, @"^(?:\+88|01)?\d{11}$")) //1762125041,01762125041,+8801762125041
                return true;
            else
                return false;
        }

    }
}
