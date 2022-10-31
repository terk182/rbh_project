using BL.Utilities;
using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class FareMarkup
    {
        public List<MarkupFlight> MarkupList;
        public FareMarkup()
        {
            //if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            //{
            //    MarkupList = (MarkupList)System.Web.HttpContext.Current.Cache["C_FARENET"];
            //}
            //else
            //{
            //    string furl = FireBaseUtility.getValue("FileUrl/farenet");
            //    string json = HttpUtility.getHttp(furl);
            //    if (String.IsNullOrEmpty(json) == false)
            //    {
            //        //Caching
            //        MarkupList = JsonConvert.DeserializeObject<MarkupList>(json);
            //        System.Web.HttpContext.Current.Cache.Insert("C_FARENET", MarkupList, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
            //    }
            //}
        }

        public decimal getFareCharge(decimal fare, string airlineCode, string depCityCode, 
            string depCountryCode, string arrCityCode, string arrCountryCode, string fareType, 
            string rbd, string fareBasis, string paxType, ref decimal lessFare, string email, decimal paymentFee, bool isPaymentFeePct, decimal tax, string tripType)
        {
            string domain = "NODOMAIN.NONO";
            if (!String.IsNullOrEmpty(email) && email.Length > 1)
            {
                domain = email.Substring(email.IndexOf("@") + 1);
                domain = domain.ToLower();
            }
            if (paxType.IndexOf("CH") >= 0)
            {
                paxType = "CHD";
            }
            if (paxType.IndexOf("IN") >= 0)
            {
                paxType = "INF";
            }

            bool isAdult = paxType == "ADT";
            bool isChild = paxType == "CHD";
            bool isInfant = paxType == "INF";

            string[] ft = fareType.Split(',');
            string rFareType = ft[0].Length == 2 ? ft[0].Substring(1, 1) : ft[0];
            if(!rFareType.Equals("P") && !rFareType.Equals("A"))
            {
                rFareType = "N";
            }
            decimal fareCharge = fare;
            List<MarkupFlight> airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == airlineCode &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == rFareType &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && x.DomainName.IndexOf(domain) >= 0).ToList();

            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == airlineCode &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == rFareType &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && (String.IsNullOrEmpty(x.DomainName) || x.DomainName == "*")).ToList();
            }

            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == airlineCode &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == "*" &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && x.DomainName.IndexOf(domain) >= 0).ToList();
            }

            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == airlineCode &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == "*" &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && (String.IsNullOrEmpty(x.DomainName) || x.DomainName == "*")).ToList();
            }
            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == "YY" &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == rFareType &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && x.DomainName.IndexOf(domain) >= 0).ToList();
            }
            if (airlineFilter.Count == 0)
            {
                airlineFilter = this.MarkupList.Where(x => x.AirlineCodes == "YY" &&
            (isAdult ? x.PaxTypeADT.GetValueOrDefault() : (isChild ? x.PaxTypeCHD.GetValueOrDefault() : x.PaxTypeINF.GetValueOrDefault())) && x.Type == rFareType &&
            x.StartBookingDate <= DateTime.Now && x.FinishBookingDate >= DateTime.Now && (String.IsNullOrEmpty(x.DomainName) || x.DomainName == "*")).ToList();
            }



            if (airlineFilter.Count == 0)
            {
                return fareCharge;
            }

            List<MarkupFlight> rbdFilter = airlineFilter.Where(x => x.RBD.ToUpper().IndexOf(rbd.ToUpper()) >= 0
            && x.FareBasis.ToUpper().Split(',').Contains(fareBasis.ToUpper())).ToList();


            if (rbdFilter.Count == 0)
            {
                rbdFilter = airlineFilter.Where(x => x.RBD.ToUpper().IndexOf("*") >= 0
            && x.FareBasis.ToUpper().Split(',').Contains(fareBasis.ToUpper())).ToList();
            }

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

            List<MarkupFlight> cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            //--
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCityCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf("*") >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf(depCountryCode) >= 0 &&
            x.ZoneTo.ToUpper().IndexOf("*") >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf("*") >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCityCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf("*") >= 0 &&
            x.ZoneTo.ToUpper().IndexOf(arrCountryCode) >= 0).ToList();
            }
            if (cityFilter.Count == 0)
            {
                cityFilter = rbdFilter.Where(x =>
            x.ZoneFrom.ToUpper().IndexOf("*") >= 0 &&
            x.ZoneTo.ToUpper().IndexOf("*") >= 0).ToList();
            }

            if (cityFilter.Count == 0)
            {
                return fareCharge;
            }

            fareCharge = 0;
            if (tripType.Equals("O"))//oneway
            {
                //Level 1
                if (cityFilter[0].LV1Type == "Discount")
                {
                    if (cityFilter[0].IsPercentLV1.GetValueOrDefault())
                    {
                        fareCharge = fare * ((100 - cityFilter[0].LV1Value.GetValueOrDefault()) * 0.01m);
                        fareCharge = ceilingTo5Or10(fareCharge);
                    }
                    else
                    {
                        fareCharge = fare - cityFilter[0].LV1Value.GetValueOrDefault();
                    }

                    lessFare = fareCharge;
                }
                else
                {
                    if (cityFilter[0].IsPercentLV1.GetValueOrDefault())
                    {
                        fareCharge = fare * ((100 + cityFilter[0].LV1Value.GetValueOrDefault()) * 0.01m);
                    }
                    else
                    {
                        fareCharge = fare + cityFilter[0].LV1Value.GetValueOrDefault();
                    }
                }

                //Level 2
                if (cityFilter[0].LV2Type == "Discount")
                {
                    if (cityFilter[0].IsPercentLV2.GetValueOrDefault())
                    {
                        fareCharge = fareCharge * ((100 - cityFilter[0].LV2Value.GetValueOrDefault()) * 0.01m);
                    }
                    else
                    {
                        fareCharge = fareCharge - cityFilter[0].LV2Value.GetValueOrDefault();
                    }

                }
                else
                {
                    if (cityFilter[0].IsPercentLV2.GetValueOrDefault())
                    {
                        fareCharge = fareCharge * ((100 + cityFilter[0].LV2Value.GetValueOrDefault()) * 0.01m);
                        fareCharge = Math.Ceiling(fareCharge);
                    }
                    else
                    {
                        fareCharge = fareCharge + cityFilter[0].LV2Value.GetValueOrDefault();
                    }

                }
            }
            else//roundtrip
            {
                //Level 1
                if (cityFilter[0].LV1TypeRT == "Discount")
                {
                    if (cityFilter[0].IsPercentLV1RT.GetValueOrDefault())
                    {
                        fareCharge = fare * ((100 - cityFilter[0].LV1ValueRT.GetValueOrDefault()) * 0.01m);
                        fareCharge = ceilingTo5Or10(fareCharge);
                    }
                    else
                    {
                        fareCharge = fare - cityFilter[0].LV1ValueRT.GetValueOrDefault();
                    }

                    lessFare = fareCharge;
                }
                else
                {
                    if (cityFilter[0].IsPercentLV1RT.GetValueOrDefault())
                    {
                        fareCharge = fare * ((100 + cityFilter[0].LV1ValueRT.GetValueOrDefault()) * 0.01m);
                    }
                    else
                    {
                        fareCharge = fare + cityFilter[0].LV1ValueRT.GetValueOrDefault();
                    }
                }

                //Level 2
                if (cityFilter[0].LV2TypeRT == "Discount")
                {
                    if (cityFilter[0].IsPercentLV2RT.GetValueOrDefault())
                    {
                        fareCharge = fareCharge * ((100 - cityFilter[0].LV2ValueRT.GetValueOrDefault()) * 0.01m);
                    }
                    else
                    {
                        fareCharge = fareCharge - cityFilter[0].LV2ValueRT.GetValueOrDefault();
                    }

                }
                else
                {
                    if (cityFilter[0].IsPercentLV2RT.GetValueOrDefault())
                    {
                        fareCharge = fareCharge * ((100 + cityFilter[0].LV2ValueRT.GetValueOrDefault()) * 0.01m);
                        fareCharge = Math.Ceiling(fareCharge);
                    }
                    else
                    {
                        fareCharge = fareCharge + cityFilter[0].LV2ValueRT.GetValueOrDefault();
                    }

                }
            }

            if (isPaymentFeePct)
            {
                fareCharge = fareCharge + ((fareCharge + tax) * paymentFee * 0.01M);
            }
            else
            {
                fareCharge += paymentFee;
            }
            //fareCharge = ceilingTo5Or10(fareCharge);
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
