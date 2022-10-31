using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGBookingAPI.Models
{
    public class CalendarSearchModel
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


        public CalendarSearchModel()
        {
        }

        public BL.Entities.MasterCalendar.Request GetMPSearchRequest()
        {
            BL.Entities.MasterCalendar.Request request = new BL.Entities.MasterCalendar.Request();
            request.pgSearchOID = Guid.NewGuid();
            if (this.airline == null || (this.airline != null && this.airline.Count == 0))
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

            request.flights = new List<BL.Entities.MasterCalendar.Flight>();
            //DEP
            request.flights.Add(new BL.Entities.MasterCalendar.Flight()
            {
                arrivalCity = arrCity,
                arrivalDateTime = new DateTime(),
                departureCity = depCity,
                departureDateTime = departDate
            });
            //RET
            if (this.tripType == "R")
            {
                request.flights.Add(new BL.Entities.MasterCalendar.Flight()
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

            return request;
        }
    }
}