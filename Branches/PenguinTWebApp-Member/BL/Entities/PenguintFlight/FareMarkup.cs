using BL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class FareMarkup
    {
        public MarkupList MarkupList;
        public FareMarkup()
        {
            if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            {
                MarkupList = (MarkupList)System.Web.HttpContext.Current.Cache["C_FARENET"];
            }
            else
            {
                string furl = FireBaseUtility.getValue("FileUrl/farenet");
                string json = HttpUtility.getHttp(furl);
                if (String.IsNullOrEmpty(json) == false)
                {
                    //Caching
                    MarkupList = JsonConvert.DeserializeObject<MarkupList>(json);
                    System.Web.HttpContext.Current.Cache.Insert("C_FARENET", MarkupList, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }

        public decimal getFareCharge(decimal fare, string airlineCode, string depCityCode, 
            string depCountryCode, string arrCityCode, string arrCountryCode, string fareType, 
            string rbd, string fareBasis, string paxType, ref decimal lessFare)
        {
            if (paxType.IndexOf("CH") >= 0)
            {
                paxType = "CHD";
            }
            if (paxType.IndexOf("IN") >= 0)
            {
                paxType = "INF";
            }
            string[] ft = fareType.Split(',');
            decimal fareCharge = fare;
            List<FareNet> airlineFilter = this.MarkupList.FareNet.Where(x => x.Airline == airlineCode &&
            x.Ptc.ToUpper().IndexOf(paxType.ToUpper()) >= 0 && x.FareType == ft[0]).ToList();

            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.FareNet.Where(x => x.Airline == "*" &&
            x.Ptc.ToUpper().IndexOf(paxType.ToUpper()) >= 0 && x.FareType == ft[0]).ToList();
            }

            if (airlineFilter.Count == 0)
            {
                return fareCharge;
            }

            List<FareNet> rbdFilter = airlineFilter.Where(x => x.RBD.ToUpper().IndexOf(rbd.ToUpper()) >= 0
            && x.FareBasis.ToUpper().Split(',').Contains(fareBasis.ToUpper())).ToList();

            if (rbdFilter.Count == 0)
            {
                rbdFilter = airlineFilter.Where(x => x.RBD.ToUpper().IndexOf(rbd.ToUpper()) >= 0
            && x.FareBasis.ToUpper().Split(',').Contains("*")).ToList();
            }

            if (rbdFilter.Count == 0)
            {
                rbdFilter = airlineFilter.Where(x => x.RBD.ToUpper().IndexOf("*") >= 0
            && x.FareBasis.ToUpper().Split(',').Contains("*")).ToList();
            }

            if (rbdFilter.Count == 0)
            {
                return fareCharge;
            }

            List<FareNet> cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            //--
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.Destination.ToUpper().IndexOf("*") >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.Destination.ToUpper().IndexOf("*") >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf("*") >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf("*") >= 0 &&
            x.Destination.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.DepartCountry.ToUpper().IndexOf("*") >= 0 &&
            x.Destination.ToUpper().IndexOf("*") >= 0).ToList();
            }

            if (cityFilter.Count == 0)
            {
                return fareCharge;
            }

            fareCharge = ceilingTo5Or10(fare * ((100 - cityFilter[0].Discount) * 0.01m)) + cityFilter[0].Markup - cityFilter[0].DiscountLv2;

            lessFare = fare * ((100 - cityFilter[0].Discount) * 0.01m);
            return fareCharge;
        }

        private decimal ceilingTo5Or10(decimal d)
        {
            decimal result = d;
            decimal c = Math.Ceiling(d);
            decimal fragment = c % 10;
            if (fragment >= 1 && fragment <= 4)
            {
                result = c + (5 - fragment);
            }
            else if (fragment >= 6 && fragment <= 9)
            {
                result = c + (10 - fragment);
            }
            else
            {
                result = c;
            }
            return result;
        }
    }


    public class MarkupList
    {
        public List<FareNet> FareNet { get; set; }
    }

    public class FareNet
    {
        public int ID { get; set; }
        public string Airline { get; set; }
        public string FareBasis { get; set; }
        public string RBD { get; set; }
        public string Digits { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountLv2 { get; set; }
        public string TripType { get; set; }
        public decimal Markup { get; set; }
        public string FareType { get; set; }
        public string Trip { get; set; }
        public string DepartCountry { get; set; }
        public string Destination { get; set; }
        public string Ptc { get; set; }
    }
}
