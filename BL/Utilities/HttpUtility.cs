using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using log4net;

namespace BL.Utilities
{
    public class HttpUtility
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string getHttp(string url)
        {
            string json = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            return json;
        }

        public static string getJSON(string url)
        {
            return getJSON(url, "");
        }

        public static string getJSON(string url, string auth)
        {
            string json = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            if (auth != "")
            {
                request.Headers.Add("Authorization", auth);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            return json;
        }

        public static string postJSON(string url, string postData)
        {
            return postJSON(url, postData, "");
        }

        public static string postJSON(string url, string postData, string auth)
        {
            string json = "";

            //WebRequest request = WebRequest.Create(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = byteArray.Length;
            //request.Timeout = 90000;
            if (auth != "")
            {
                request.Headers.Add("Authorization", auth);
            }
            Log.Debug("start post json");
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            // Get the response.  
            StreamReader reader;
            //WebResponse response;
            HttpWebResponse response;
            try
            {
                //response = request.GetResponse();
                response = request.GetResponse() as HttpWebResponse;
                Log.Debug("end 1 post json");
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                Log.Debug("end 2 post json");
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    dataStream = new GZipStream(dataStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    dataStream = new DeflateStream(dataStream, CompressionMode.Decompress);
                }
                Log.Debug("end 3 post json");
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

            return json;
        }

        public static string GetIPAddress()
        {
            string IPAddress = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }
    }
}
