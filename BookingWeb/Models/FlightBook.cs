using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingWeb.Models
{
    public class FlightBook
    {
        public FlightBooking FlightBookingDetail { get; set; }
        public FlightBookingFare FlightBookingFareDetail { get; set; }
        public FlightBookingBaggage FlightBookingBaggageDetail { get; set; }
        public FlightBookingFlightDetail FlightBookingFlightDetail { get; set; }
        public FlightBookingPaxInfo FlightBookingPaxInfoDetail { get; set; }
        //public FlightBookingAirport FlightBookingAirportDetail { get; set; }
        //public FlightBookingAirline FlightBookingAirlineDetail { get; set; }
        public FlightBookingFareRule FlightBookingFareRuleDetail { get; set; }
        public FlightBookingFareRuleDatail FlightBookingFareRuleDatail { get; set; }
    }
}