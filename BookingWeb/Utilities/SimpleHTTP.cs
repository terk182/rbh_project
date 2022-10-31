using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace TGBookingWeb
{
    public class SimpleHTTP
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string url = "";
        public List<PostData> postDatas = new List<PostData>();
        public SimpleHTTP()
        {
        }

        public string Get()
        {
            string response = "";
            try
            {
                using (var wb = new WebClient())
                {
                    response = wb.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                log.Error("Get Fail", ex);
            }

            return response;
        }

        public string Post()
        {
            string response = "";
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    if (postDatas != null && postDatas.Count > 0)
                    {
                        foreach (PostData pData in postDatas)
                        {
                            data[pData.name] = pData.value;
                        }
                    }
                    var responseBytes = wb.UploadValues(url, "POST", data);
                    response = Encoding.UTF8.GetString(responseBytes);
                }
            }
            catch (Exception ex)
            {
                log.Error("Post Fail", ex);
            }

            return response;
        }
    }

    public class PostData
    {
        public string name = "";
        public string value = "";
        public PostData()
        {

        }
    }
}