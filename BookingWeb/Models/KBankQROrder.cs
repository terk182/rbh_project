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
    public class KBankQROrder
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string source_type { get; set; }
        public string reference_order { get; set; }

        public KBankQROrderResponse createOrder()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = ConfigurationManager.AppSettings["KBANK.QROrder"];
            string secretKey = ConfigurationManager.AppSettings["KBANK.SecretKey"];
            string postData = JsonConvert.SerializeObject(this);
            string json = "";

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json;";
            request.ContentLength = byteArray.Length;
            request.Headers.Add("x-api-key", secretKey);

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            // Get the response.  
            StreamReader reader;
            WebResponse response;
            try
            {
                response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                json = reader.ReadToEnd();
                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
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

            KBankQROrderResponse res = JsonConvert.DeserializeObject<KBankQROrderResponse>(json);
            res.json = json;
            return res;
        }
    }


    public class KBankQROrderResponse
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string reference_order { get; set; }
        public string status { get; set; }
        public string json { get; set; }
    }
}