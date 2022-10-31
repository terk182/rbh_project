using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TGBookingWeb.Models
{
    public class KBankQRInquiry
    {
        public string charge_id { get; set; }

        public KBankQRInquiryResponse chargeAPI()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = ConfigurationManager.AppSettings["KBANK.QRInquiryURL"];
            string secretKey = ConfigurationManager.AppSettings["KBANK.SecretKey"];
            string postData = JsonConvert.SerializeObject(this);
            string json = "";

            WebRequest request = WebRequest.Create(url + this.charge_id);
            request.Method = "GET";
            request.ContentType = "application/json;";
            request.Headers.Add("x-api-key", secretKey);

            // Get the response.  
            StreamReader reader;
            try
            {
                IAsyncResult asyncResult = request.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                json = streamReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                {
                    reader = new StreamReader(stream);
                    json = reader.ReadToEnd();
                    reader.Close();
                }
            }

            KBankQRInquiryResponse res = JsonConvert.DeserializeObject<KBankQRInquiryResponse>(json);
            res.json = json;
            return res;
        }
    }


    public class KBankQRInquiryResponse
    {
        public string id { get; set; }
        public string order_id { get; set; }
        public string transaction_state { get; set; }
        public string reference_order { get; set; }
        public string status { get; set; }
        public string json { get; set; }
    }
}