using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BL.Utilities
{
    public class CRMConnection
    {
        public static string apiKey = ConfigurationManager.AppSettings["CRM.APIKEY"];
        public static string apiURL = ConfigurationManager.AppSettings["CRM.URL"];

        public static string GetResponse(string functionName, string uuid, string token, string language)
        {
            //ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Net.WebRequest req = System.Net.WebRequest.Create(apiURL + functionName);
            HttpWebRequest httpWebRequest = req as HttpWebRequest;

            if (httpWebRequest != null)
            {
                httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }
            httpWebRequest.ProtocolVersion = HttpVersion.Version10; // fix 1
            httpWebRequest.KeepAlive = false; // fix 2
            httpWebRequest.Timeout = 30000; // fix 3
            httpWebRequest.ReadWriteTimeout = 30000; // fix 4
            httpWebRequest.Accept = "application/json";
            req.ContentType = "application/json";

            req.Headers.Add("Accept-Encoding", "gzip");

            req.Headers.Add("api-key", apiKey);
            req.Headers.Add("language", language);
            if (uuid != "")
            {
                req.Headers.Add("uuid", uuid);
            }
            if (token != "")
            {
                req.Headers.Add("Authorization", "Bearer " + token);
            }
            //req.Headers.Add("Accept", "application/xml");

            req.Method = "GET";


            try
            {
                IAsyncResult asyncResult = req.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                HttpWebResponse httpWebResponse = (HttpWebResponse)req.EndGetResponse(asyncResult);
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string sXmlResponse = streamReader.ReadToEnd();
                
                return sXmlResponse;
            }
            catch (WebException hex)
            {
                string sOutput = "";
                return sOutput;
            }
            catch (Exception ex)
            {
                string sOutput = "";
                return sOutput;
            }
        }

        public static string PostResponse(string postData, string functionName, string uuid, string token, string language)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Net.WebRequest req = System.Net.WebRequest.Create(apiURL + functionName);
            HttpWebRequest httpWebRequest = req as HttpWebRequest;

            if (httpWebRequest != null)
            {
                httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }
            httpWebRequest.ProtocolVersion = HttpVersion.Version10; // fix 1
            httpWebRequest.KeepAlive = false; // fix 2
            httpWebRequest.Timeout = 30000; // fix 3
            httpWebRequest.ReadWriteTimeout = 30000; // fix 4
            httpWebRequest.Accept = "application/json";
            req.ContentType = "application/json";

            req.Headers.Add("Accept-Encoding", "gzip");

            req.Headers.Add("api-key", apiKey);
            req.Headers.Add("language", language);
            if (uuid != "")
            {
                req.Headers.Add("uuid", uuid);
            }
            if (token != "")
            {
                req.Headers.Add("Authorization", "Bearer " + token);
            }

            req.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(postData);
            req.ContentLength = bytes.Length;


            try
            {
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                IAsyncResult asyncResult = req.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                HttpWebResponse httpWebResponse = (HttpWebResponse)req.EndGetResponse(asyncResult);
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string sResponse = streamReader.ReadToEnd();

                return sResponse;
            }
            catch (WebException hex)
            {
                string sOutput = "";
                if (hex.Response != null)
                {
                    Stream r_stream = hex.Response.GetResponseStream();
                    //convert it
                    StreamReader response_stream = new
                    StreamReader(r_stream, System.Text.Encoding.GetEncoding("utf-8"));
                    sOutput = response_stream.ReadToEnd();
                }
                return sOutput;
            }
            catch (Exception ex)
            {
                string sOutput = "";
                return sOutput;
            }
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static DateTime ConvertXmlDateToDateTime(string xmlDate)
        {
            string[] sDate = xmlDate.Split('-');
            return new DateTime(int.Parse(sDate[0]), int.Parse(sDate[1]), int.Parse(sDate[2]));
        }

        public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
        {
            public TrustAllCertificatePolicy()
            { }

            public bool CheckValidationResult(ServicePoint sp,
                      System.Security.Cryptography.X509Certificates.X509Certificate cert,
                      WebRequest req,
                      int problem)
            {
                return true;
            }
        }

    }
}
