using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TGBookingWeb
{
    public class Currency
    {
        public static void SetCurrency(string code)
        {
            code = code.ToUpper();
            HttpCookie aCookie = new HttpCookie("Gogojii.CurrencyCode");
            aCookie.Value = code;
            aCookie.Expires = DateTime.Now.AddMonths(3);
            HttpContext.Current.Response.Cookies.Add(aCookie);

            HttpContext.Current.Session.Add("GGCurrency", code);
        }

        public static string GetCurrency()
        {
            if (HttpContext.Current.Session["GGCurrency"] != null)
            {
                return (string)HttpContext.Current.Session["GGCurrency"];
            }
            string langCode = "THB";
            if (HttpContext.Current.Request.Cookies["Gogojii.CurrencyCode"] != null)
            {
                HttpCookie aCookie = HttpContext.Current.Request.Cookies["Gogojii.CurrencyCode"];
                langCode = aCookie.Value;
            }
            return langCode;
        }

        public static decimal convertFromTHB(decimal amount)
        {
            DataModel.UnitOfWork.UnitOfWork unitOfWork = new DataModel.UnitOfWork.UnitOfWork();
            BL.CurrencyExchange ccy = new BL.CurrencyExchange(unitOfWork);
            return ccy.ConvertFromTHB(GetCurrency(), amount);
        }
    }
    
}