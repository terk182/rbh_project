using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingAPI
{
    public class Aircraft
    {
        public static string Show(string keyword)
        {
            string value = keyword;

            string localizeJson = "";
            if (System.Web.HttpContext.Current.Cache["C_AIRCRAFT"] != null)
            {
                localizeJson = (string)System.Web.HttpContext.Current.Cache["C_AIRCRAFT"];
            }
            else
            {
                //Caching
                string path = HttpContext.Current.Server.MapPath("~/App_Data/aircraft.json");
                localizeJson = System.IO.File.ReadAllText(path);
                System.Web.HttpContext.Current.Cache.Insert("C_AIRCRAFT", localizeJson, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);

            }

            try
            {
                JToken root = JObject.Parse(localizeJson);
                JToken keyValue = root[keyword];
                if (keyValue != null)
                {
                    return keyValue.ToString();

                }
            }
            catch
            { }


            return "";
        }
        public static string GetCode(string keyword)
        {
            string value = keyword;

            string localizeJson = "";
            if (System.Web.HttpContext.Current.Cache["C_AIRCRAFT"] != null)
            {
                localizeJson = (string)System.Web.HttpContext.Current.Cache["C_AIRCRAFT"];
            }
            else
            {
                //Caching
                string path = HttpContext.Current.Server.MapPath("~/App_Data/aircraft.json");
                localizeJson = System.IO.File.ReadAllText(path);
                System.Web.HttpContext.Current.Cache.Insert("C_AIRCRAFT", localizeJson, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);

            }

            try
            {
                JToken root = JObject.Parse(localizeJson);
                var equipmentType = root.Children().ToList();
                var _equipmentType = equipmentType.Find(x => x.ToString().IndexOf(keyword) != -1);
                if (_equipmentType != null)
                {
                    return _equipmentType.Path;
                }
            }
            catch
            { }
            return "";
        }
    }
}