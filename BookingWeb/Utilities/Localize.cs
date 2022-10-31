using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGBookingWeb
{
    public class Localize
    {
        public static void SetLang(string code)
        {
            HttpCookie aCookie = new HttpCookie("Gogojii.LangCode");
            aCookie.Value = code;
            aCookie.Expires = DateTime.Now.AddMonths(3);
            HttpContext.Current.Response.Cookies.Add(aCookie);
            HttpContext.Current.Session.Add("GGLangCode", code);
        }

        public static string GetLang()
        {
            if (HttpContext.Current.Session["GGLangCode"] != null)
            {
                return (string)HttpContext.Current.Session["GGLangCode"];
            }
            string langCode = "en";
            if (HttpContext.Current.Request.Cookies["Gogojii.LangCode"] != null)
            {
                HttpCookie aCookie = HttpContext.Current.Request.Cookies["Gogojii.LangCode"];
                langCode = aCookie.Value;
            }
            return langCode;
        }

        public static string GetLangName()
        {
            string langName = "";
            switch(GetLang())
            {
                case "en":
                    langName = "Eng";
                    break;
                case "th":
                    langName = "ไทย";
                    break;
            }
            return langName;
        }

        public static string Show(string keyword)
        {
            string value = keyword;
            string langCode = GetLang();

            string localizeJson = "";
            var session = HttpContext.Current.Session;
            if (session["LOCALIZATION"] != null)
            {
                localizeJson = (string)session["LOCALIZATION"];
            }
            else
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/localization.json");
                localizeJson = System.IO.File.ReadAllText(path);
                session.Add("LOCALIZATION", localizeJson);
            }

            if (keyword != null)
            {
                JToken root = JObject.Parse(localizeJson);
                JToken keyValue = root[keyword];

                if (keyValue != null)
                {
                    List<string> val = JsonConvert.DeserializeObject<List<string>>(keyValue.ToString());
                    value = langCode.ToLower() == "th" ? val[1] : val[0];
                }
            }

            return value;
        }
    }
    
}