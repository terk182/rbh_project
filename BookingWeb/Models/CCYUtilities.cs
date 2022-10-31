using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingWeb.Models
{
    public class CCYUtilities
    {
        public static string CurrencySymbol(string currencyCode)
        {
            string symbol = currencyCode;
            switch (currencyCode.ToUpper())
            {
                case "THB":
                    symbol = "฿";
                    break;
                case "HKD":
                    symbol = "HK$";
                    break;
            }
            return symbol;
        }
    }
}