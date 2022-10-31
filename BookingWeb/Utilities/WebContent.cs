using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGBookingWeb
{
    public class WebContent
    {
        public static string mainWeb = ConfigurationManager.AppSettings["Main.URL"];
        public static string bookingWeb = ConfigurationManager.AppSettings["Booking.URL"];
    }
}