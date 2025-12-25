using IMSWEB.Model.TO.Bkash;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMSWEB.Helper
{
    public static class PaymentHelper
    {
        //#region sandbox credentials
        //public const string APP_KEY = "4f6o0cjiki2rfm34kfdadl1eqq";
        //public const string APP_SECRET = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b";
        //public const string USER_NAME = "sandboxTokenizedUser02";
        //public const string USER_PASS = "sandboxTokenizedUser02@12345";
        //public const string BASE_URL = "https://tokenized.sandbox.bka.sh/v1.2.0-beta";
        //#endregion

        #region live credentials
        public const string APP_KEY = "svqwzdjYLz2O7EUR8rvcKfYUtc";
        public const string APP_SECRET = "ICC1cft2xn7DlxCk1UQQqYVCWkTubm2AnIa2JTNuE6HvdWMLmG3g";
        public const string USER_NAME = "01719049752";
        public const string USER_PASS = "4j<Y|84}n[S";
        public const string BASE_URL = "https://tokenized.pay.bka.sh/v1.2.0-beta";
        #endregion

        public static string GetGrantToken()
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/tokenized/checkout/token/grant", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("username", USER_NAME);
            request.AddHeader("password", USER_PASS);

            request.AddParameter("application/json", "{\"app_key\":\"" + APP_KEY + "\",\"app_secret\":\""
                + APP_SECRET + "\"}", ParameterType.RequestBody);

            //request.AddBody(new
            //{
            //    "app_key" = "4f6o0cjiki2rfm34kfdadl1eqq",
            //    app_secret = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b"
            //});
            var response = client.Execute(request);
            var content = JObject.Parse(response.Content);
            var token = (string)content["id_token"];
            return token;
        }

        public static CreatePaymentResponseTO CreatePaymentFromToken(string amount, string intent, string currency, string concernName, string token, string concernId)
        {
            var client = new RestClient(BASE_URL);           
            var request = new RestRequest("/tokenized/checkout/create", Method.POST);
            Uri requestUrl = HttpContext.Current.Request.Url;
            string path = requestUrl.AbsolutePath;
            bool isPathPresent = path.Contains("SMSBillPayment/GetBkashTokenOrConfirmPayment");
            string callbackUrl;
            if (isPathPresent)
            {       
                string baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                callbackUrl = baseUrl + "/SMSBillPayment/Create";
            }
            else
            {   
                string baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                callbackUrl = baseUrl + "/payment";
            }
            //request.AddHeader("Content-Type", "application/json");1
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", token);
            request.AddHeader("X-App-Key", APP_KEY);


            //request.AddParameter("application/json", "{\"mode\":\"0011\",\"payerReference\":\"" + concernId + "\",\"callbackURL\":\"https://trelectromart.com/payment\",\"amount\":\"" + amount + "\", \"currency\":\"" + currency + "\", \"intent\":\"" + intent + "\", \"merchantInvoiceNumber\":\"" + merchantInvoiceNo + "\"}", ParameterType.RequestBody);

            //request.AddParameter("application/json", "{\"mode\":\"0011\",\"payerReference\":\"" + concernId + "\",\"callbackURL\":\"http://localhost:7328/SMSBillPayment/Create\",\"amount\":\"" + amount + "\", \"currency\":\"" + currency + "\", \"intent\":\"" + intent + "\", \"merchantInvoiceNumber\":\"" + merchantInvoiceNo + "\"}", ParameterType.RequestBody);

            request.AddParameter("application/json", "{\"mode\":\"0011\",\"payerReference\":\"" + concernId + "\",\"callbackURL\":\"" + callbackUrl + "\",\"amount\":\"" + amount + "\", \"currency\":\"" + currency + "\", \"intent\":\"" + intent + "\", \"merchantInvoiceNumber\":\"" + concernName + "\"}", ParameterType.RequestBody);

            var response = client.Execute(request);
            CreatePaymentResponseTO apiResponse = JsonConvert.DeserializeObject<CreatePaymentResponseTO>(response.Content);
            return apiResponse;

        }

        public static ConfirmPaymentResponseTO ConfirmPaymentByPaymentId(string paymentId, string token)
        {
            var client = new RestClient(BASE_URL);
            var request = new RestRequest("/tokenized/checkout/execute", Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", token);
            request.AddHeader("X-App-Key", APP_KEY);
            request.AddParameter("application/json", "{\"paymentID\":\"" + paymentId + "\"}", ParameterType.RequestBody);
            var response = client.Execute(request);
            ConfirmPaymentResponseTO apiResponse = JsonConvert.DeserializeObject<ConfirmPaymentResponseTO>(response.Content);
            return apiResponse;

        }


    }
}