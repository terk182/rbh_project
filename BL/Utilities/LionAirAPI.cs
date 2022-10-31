using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace BL.Utilities
{
    public class LionAirAPI
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string url = ConfigurationManager.AppSettings["LionAir.TargetURL"].ToString();//required
        public string serviceName = "";//required
        public string message = "";//required
        public string post()
        {
            string res = "";

            HttpWebRequest req = null;
            HttpWebResponse response = null;
            Stream StreamData = null;
            Stream r_stream = null;
            StreamReader response_stream = null;
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byte1 = encoding.GetBytes(this.message);

                req = (HttpWebRequest)WebRequest.Create(url + serviceName);
                req.ContentType = "text/xml;charset=UTF-8";
                //req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Accept = "*/*";
                req.ContentLength = this.message.Length;
                req.Timeout = 60000;
                req.AllowAutoRedirect = true;
                req.PreAuthenticate = true;
                req.Credentials = CredentialCache.DefaultCredentials;
                req.KeepAlive = false;

                req.Headers.Add("Accept-Encoding", "gzip,deflate");


                StreamData = req.GetRequestStream();
                StreamData.Write(byte1, 0, byte1.Length);//
                StreamData.Close();


                response = (HttpWebResponse)req.GetResponse();
                Log.Debug("StatusDescription=" + response.StatusDescription);
                r_stream = response.GetResponseStream();

                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    //Response.Write("found gzip<br/>");
                    string xxx = response.GetResponseHeader("content-encoding");
                    //Response.Write("header=" + xxx + "<br/>");
                    r_stream = new GZipStream(r_stream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    //Response.Write("found deflate<br/>");
                    r_stream = new DeflateStream(r_stream, CompressionMode.Decompress);
                }

                //convert it
                response_stream = new
                StreamReader(r_stream, System.Text.Encoding.GetEncoding("utf-8"));
                res = response_stream.ReadToEnd();
            }
            catch (Exception ex)
            {
                res = "";
                Log.Error("post fail", ex);
            }
            return res;
        }
    }
}
