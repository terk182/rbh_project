using GreatFriends.ThaiBahtText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace TGBookingWeb.Utilities
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
        public static string getThaiBahtText(decimal amt)
        {
            return amt.ThaiBahtText();
        }
        public static string NumberToText(int number, bool isUK)
        {
            if (number == 0) return "Zero";
            string and = isUK ? "and " : ""; // deals with UK or US numbering
            if (number == -2147483648) return "Minus Two Billion One Hundred " + and +
            "Forty Seven Million Four Hundred " + and + "Eighty Three Thousand " +
            "Six Hundred " + and + "Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Million ", "Billion " };
            num[0] = number % 1000;           // units
            num[1] = number / 1000;
            num[2] = number / 1000000;
            num[1] = num[1] - 1000 * num[2];  // thousands
            num[3] = number / 1000000000;     // billions
            num[2] = num[2] - 1000 * num[3];  // millions
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10;              // ones
                t = num[i] / 10;
                h = num[i] / 100;             // hundreds
                t = t - 10 * h;               // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i < first) sb.Append(and);
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public static string getMeal(string code)
        {
            string result = code;
            switch (code)
            {
                case "AVML": result = "ASIAN VEGETARIAN MEAL"; break;
                case "BLML": result = "BLAND MEAL"; break;
                case "DBML": result = "DIABETIC MEAL"; break;
                case "FPML": result = "FRUIT PLATTER"; break;

                case "GFML": result = "GLUTEN-FREE MEAL"; break;
                case "HNML": result = "HINDU (NON VEGETARIAN) MEAL"; break;
                case "KSML": result = "KOSHER MEAL"; break;
                case "LCML": result = "LOW CALORIE MEAL"; break;

                case "LFML": result = "LOW CHOLESTEROL/LOW FAT MEAL"; break;
                case "LSML": result = "LOW SODIUM, NO SALT ADDED"; break;
                case "MOML": result = "MOSLEM MEAL"; break;
                case "NLML": result = "NON LACTOSE MEAL"; break;

                case "ORML": result = "ORIENTAL MEAL"; break;
                case "RVML": result = "RAW VEGETARIAN MEAL"; break;
                case "SFML": result = "SEA FOOD MEAL"; break;
                case "VGML": result = "VEGETARIAN MEAL (NON-DAIRY)"; break;

                case "VLML": result = "VEGETARIAN MEAL (LACTO-OVO)"; break;
                case "VOML": result = "VEGETARIAN ORIENTAL MEAL"; break;

                case "CHML": result = "CHILD MEAL"; break;
                case "BBML": result = "INFANT/BABY FOOD"; break;
            }
            return result;
        }
        
    }
}