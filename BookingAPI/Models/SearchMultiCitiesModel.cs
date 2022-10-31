using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGBookingAPI.Models
{
    public class SearchMultiCitiesModel
    {
        public Guid pgSearchOID { get; set; }
        public string languageCode { get; set; }
        public List<Flights> flights { get; set; }
        public List<string> airline { get; set; }
        public string svcClass { get; set; }
        public bool directFlight { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public SearchMultiCitiesModel()
        {
        }
        public BL.Entities.MasterPricer.Request GetMPSearchRequest()
        {
            BL.Entities.MasterPricer.Request request = new BL.Entities.MasterPricer.Request();
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


            request.flights = new List<BL.Entities.MasterPricer.Flight>();
            foreach (Flights _flight in flights)
            {
                request.flights.Add(new BL.Entities.MasterPricer.Flight()
                {
                    arrivalCity = _flight.destinationCode,
                    arrivalDateTime = new DateTime(),
                    departureCity = _flight.originCode,
                    departureDateTime = _flight.departDate
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
    public class Flights
    {
        public DateTime departDate { get; set; }
        public string originCode { get; set; }
        public string destinationCode { get; set; }
    }
}