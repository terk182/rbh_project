using BL;
using BL.Entities.RobinhoodFlight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGBookingAPI.Models
{
    public class SearchModel
    {
        public Guid pgSearchOID { get; set; }
        public string languageCode { get; set; }
        public string tripType { get; set; }
        public string originCode { get; set; }
        public string destinationCode { get; set; }
        public List<string> airline { get; set; }
        public string svcClass { get; set; }
        public bool directFlight { get; set; }
        public DateTime departDate { get; set; }
        public DateTime returnDate { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }


        public SearchModel()
        {
        }
        
        public BL.Entities.MasterPricer.Request GetMPSearchRequest()
        {
            BL.Entities.MasterPricer.Request request = new BL.Entities.MasterPricer.Request();
            request.pgSearchOID = Guid.NewGuid();
            if (this.airline==null || (this.airline!=null && this.airline.Count==0))
            {
                request.airlineCode = null;
            }
            else
            {
                request.airlineCode = this.airline;
            }
            request.cabinClass = this.svcClass;
            request.corporateContractNumber = null;
            request.currencyCode = "THB";
            request.directFlight = this.directFlight;
            request.flexibleDate = 0;

            string depCity = this.originCode;
            string arrCity = this.destinationCode;

            request.flights = new List<BL.Entities.MasterPricer.Flight>();
            //DEP
            request.flights.Add(new BL.Entities.MasterPricer.Flight()
            {
                arrivalCity = arrCity,
                arrivalDateTime = new DateTime(),
                departureCity = depCity,
                departureDateTime = departDate
            });
            //RET
            if (this.tripType == "R")
            {
                request.flights.Add(new BL.Entities.MasterPricer.Flight()
                {
                    arrivalCity = depCity,
                    arrivalDateTime = new DateTime(),
                    departureCity = arrCity,
                    departureDateTime = returnDate
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
            request.languageCode = this.languageCode;
            request.bookingOID = ConfigurationManager.AppSettings["OfficeID"];

            request.fareFamilyInfoOption = new BL.Entities.MasterPricer.fareFamilyInfoOption();
            request.fareFamilyInfoOption.enable = true;
            request.fareFamilyInfoOption.feeType = "FFI";//default เป็น "FFI"
            request.fareFamilyInfoOption.feeIdNumber = "3";//default เป็น "3"

            request.upsellOption = new BL.Entities.MasterPricer.upsellOption();
            request.upsellOption.enable = true;
            request.upsellOption.feeType = "UPH";//default เป็น "UP"
            request.upsellOption.feeIdNumber = "6"; //default เป็น "6"

            return request;
        }

        public BL.Entities.AirMultiAvailability.Request GetANSearchRequest()
        {
            BL.Entities.AirMultiAvailability.Request request = new BL.Entities.AirMultiAvailability.Request();
            request.pgSearchOID = this.pgSearchOID;
            request.tripType = this.tripType;
            request.anRoute = request.tripType.Equals("R") ? "BOTH" : "DEP";
            if (this.airline == null || (this.airline != null && this.airline.Count == 0))
            {
                request.airlineCode = null;
            }
            else
            {
                request.airlineCode = this.airline;
            }
            request.cabinClass = this.svcClass;
            request.actionCode = "44";
            request.departureDateTime = departDate;
            request.returnDateTime = request.tripType.Equals("R") ? returnDate : departDate;
            request.originCity = this.originCode;
            request.destinationCity = this.destinationCode;

            request.noOfAdults = this.adult;
            request.noOfChildren = this.child;
            request.noOfInfants = this.infant;

            request.AN_optionInfo = new List<string>();
            request.AN_optionInfo.Add("SEV");
            request.AN_optionInfo.Add("FLO");

            request.languageCode = this.languageCode;
            request.bookingOID = ConfigurationManager.AppSettings["OfficeID"];

            return request;
        }
    }
}