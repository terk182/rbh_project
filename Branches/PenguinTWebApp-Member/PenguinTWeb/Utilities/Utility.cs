using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GogojiiWeb.Utilities
{
    public class Utility
    {
        public static string GetSearchCityCode(string cityStr)
        {
            if (cityStr.Length == 3)
            {
                return cityStr;
            }

            if (cityStr.IndexOf("[") >= 0 && cityStr.IndexOf("]") >= 4)
            {
                return cityStr.Substring(cityStr.IndexOf("[") + 1, 3);
            }

            return "";
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