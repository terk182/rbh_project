using BL;
using BL.Entities.GogojiiFlight;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GogojiiWeb.Models
{
    public class SearchModel
    {
        public string lang { get; set; }
        public string triptype { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string originCode { get; set; }
        public string destinationCode { get; set; }
        public string departdate { get; set; }
        public string returndate { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public string svc_class { get; set; }
        public string svc_class_name { get; set; }
        public string directflight { get; set; }
        public string originName { get; set; }
        public string destinationName { get; set; }
        public string airline { get; set; }
        public Guid searchID { get; set; }
        public DateTime dtDep { get; set; }
        public DateTime dtRet { get; set; }

        public string line1 { get; set; }
        public string line2 { get; set; }

        public SearchModel()
        {
            this.triptype = "R";
            this.origin = Localize.Show("BKK_DEFAULT");
            this.departdate = DateTime.Today.AddDays(3).ToString("dd'/'MM'/'yyyy");
            this.returndate = DateTime.Today.AddDays(5).ToString("dd'/'MM'/'yyyy");
            this.adult = 1;
            this.svc_class = "Y";
        }

        public SelectList GetAirlineSearchBox()
        {
            AirlineSearchBox airSearch = new AirlineSearchBox();
            return airSearch.GetList(Localize.Show("ALL_AIRLINE"));
        }

        public string departdateCalendarFormat()
        {
            //if (this.departdate.Length == 10 && this.departdate.IndexOf("/") >= 0)
            //{
            //    DateTime dt = DateTime.ParseExact(this.departdate
            //                    , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //    return dt.ToString("ddd dd, MMM yyy");
            //}
            //else
            {
                return this.departdate;
            }
        }

        public string returndateCalendarFormat()
        {
            //if (this.returndate.Length == 10 && this.returndate.IndexOf("/") >= 0)
            //{
            //    DateTime dt = DateTime.ParseExact(this.returndate
            //                    , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //    return dt.ToString("ddd dd, MMM yyy");
            //}
            //else
            {
                return this.returndate;
            }
        }
        public void GetSearchText(INamingServices namingServices)
        {
            originCode = Utilities.Utility.GetSearchCityCode(origin);
            destinationCode = Utilities.Utility.GetSearchCityCode(destination);
            originName = namingServices.GetCityName(originCode, this.lang);
            if (originName == "")
            {
                originName = namingServices.GetAirportName(originCode, false, this.lang);
            }
            destinationName = namingServices.GetCityName(destinationCode, this.lang);
            if (destinationName == "")
            {
                destinationName = namingServices.GetAirportName(destinationCode, false, this.lang);
            }
            line1 = String.Format("{0}({1}) - {2}({3})",
                originName,
                originCode,
                destinationName,
                destinationCode
                );

            CultureInfo ci = new CultureInfo(this.lang.ToLower() == "th" ? "th-TH" : "en-US");
            dtDep = DateTime.ParseExact(this.departdate
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            dtRet = new DateTime();
            if (this.triptype == "R")
            {
                dtRet = DateTime.ParseExact(this.returndate
                                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            line2 = String.Format("{0} {1} | {2} | {3}",
                dtDep.ToString("ddd, dd MMM ", ci) + dtDep.Year.ToString(),
                this.triptype == "R" ? (" - " + dtRet.ToString("ddd, dd MMM ", ci) + dtRet.Year.ToString()) : "",
                this.GetPaxString(),
                this.GetSvcClassString());

            svc_class_name = GetSvcClassString();
        }

        private string GetSvcClassString()
        {
            string clsStr = "";
            switch(this.svc_class)
            {
                case "F":
                    clsStr = Localize.Show("FIRST");
                    break;
                case "C":
                    clsStr = Localize.Show("BUSINESS");
                    break;
                case "P":
                    clsStr = Localize.Show("PREMIUM_ECONOMY");
                    break;
                case "Y":
                    clsStr = Localize.Show("ECONOMY");
                    break;
            }
            return clsStr;
        }

        private string GetPaxString()
        {
            string paxStr = "";
            paxStr += this.adult.ToString() + " " + Localize.Show("ADULTS");
            if (this.child > 0)
            {
                paxStr += ", " + this.child.ToString() + " " + Localize.Show("CHILDREN");
            }
            if (this.infant > 0)
            {
                paxStr += ", " + this.infant.ToString() + " " + Localize.Show("INFANTS");
            }
            return paxStr;
        }

        public string GetJsonModel()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public BL.Entities.MasterPricer.Request GetMPSearchRequest()
        {
            BL.Entities.MasterPricer.Request request = new BL.Entities.MasterPricer.Request();
            request.pgSearchOID = this.searchID;
            if (String.IsNullOrEmpty(this.airline))
            {
                request.airlineCode = null;
            }
            else
            {
                request.airlineCode = new List<string>();
                request.airlineCode.Add(this.airline);
            }
            request.cabinClass = this.svc_class;
            request.corporateContractNumber = null;
            request.currencyCode = "THB";
            request.directFlight = !String.IsNullOrEmpty(this.directflight);
            request.flexibleDate = 0;

            string depCity = Utilities.Utility.GetSearchCityCode(this.origin);
            string arrCity = Utilities.Utility.GetSearchCityCode(this.destination);
            DateTime dtDep = DateTime.ParseExact(this.departdate
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            request.flights = new List<BL.Entities.MasterPricer.Flight>();
            //DEP
            request.flights.Add(new BL.Entities.MasterPricer.Flight()
            {
                arrivalCity = arrCity,
                arrivalDateTime = new DateTime(),
                departureCity = depCity,
                departureDateTime = dtDep
            });
            //RET
            if (this.triptype == "R")
            {
                DateTime dtRet = DateTime.ParseExact(this.returndate
                                , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                request.flights.Add(new BL.Entities.MasterPricer.Flight()
                {
                    arrivalCity = depCity,
                    arrivalDateTime = new DateTime(),
                    departureCity = arrCity,
                    departureDateTime = dtRet
                });
            }
            request.isPriceTypeRP = true;
            request.isPriceTypeRU = true;
            request.noOfAdults = this.adult;
            request.noOfChildren = this.child;
            request.noOfInfants = this.infant;
            request.noOfRecommendation = 200;
            request.nonstopFlight = false;
            request.sellingCity = "BKK";
            request.ticketingCity = "BKK";
            request.useInterlineAgreement = true;
            request.languageCode = this.lang;

            return request;
        }
    }
}