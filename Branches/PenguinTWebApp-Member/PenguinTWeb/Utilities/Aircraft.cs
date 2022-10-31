using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GogojiiWeb
{
    public class Aircraft
    {

        public static string Show(string keyword)
        {
            string value = keyword;

            string localizeJson = "";
            var session = HttpContext.Current.Session;
            if (session["AIRCRAFT"] != null)
            {
                localizeJson = (string)session["AIRCRAFT"];
            }
            else
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/aircraft.json");
                localizeJson = System.IO.File.ReadAllText(path);
                session.Add("AIRCRAFT", localizeJson);
            }

            JToken root = JObject.Parse(localizeJson);
            JToken keyValue = root[keyword];
            if (keyValue != null)
            {
                return keyValue.ToString();

            }
            return "";
        }
    }

}