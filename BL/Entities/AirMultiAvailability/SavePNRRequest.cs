using BL.Entities.RobinhoodFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirMultiAvailability
{
    public class SavePNRRequest
    {
        public Guid pgSearchOID { get; set; }
        public List<FlightDetailSelected> depFlight { get; set; }
        public List<FlightDetailSelected> retFlight { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public string svc_class { get; set; }
        public string languageCode { get; set; }
        public string bookingOID { get; set; }
        public RobinhoodPax.ContactInfo contactInfo { get; set; }
        public List<RobinhoodPax.PaxInfo> adtPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> chdPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> infPaxs { get; set; }
        public string PNR { get; set; }
        public AirSell.Request GetAirSellRequest()
        {
            AirSell.Request request = new AirSell.Request();
            request.bookingOID = this.bookingOID;
            request.noOfAdults = this.noOfAdults;
            request.noOfChildren = this.noOfChildren;
            request.noOfInfants = this.noOfInfants;
            request.departureFlights = new List<AirSell.AirFlight>();
            for (int i = 0; i < this.depFlight.Count; i++)
            {
                var flight = new AirSell.AirFlight();

                flight.departureDateTime = this.depFlight[i].departureDateTime;
                flight.arrivalDateTime = this.depFlight[i].arrivalDateTime;
                flight.departureCity = this.depFlight[i].depCity.code;
                flight.arrivalCity = this.depFlight[i].arrCity.code;
                flight.companyCode = this.depFlight[i].airline.code;
                flight.flightNumber = this.depFlight[i].flightNumber;
                flight.RBDCode = this.depFlight[i].rbd;

                if (i > 0 && request.departureFlights[i - 1].companyCode == flight.companyCode && request.departureFlights[i - 1].flightNumber == flight.flightNumber)
                {
                    request.departureFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                    request.departureFlights[i - 1].arrivalCity = flight.arrivalCity;
                }
                else
                {
                    request.departureFlights.Add(flight);
                }
            }

            if (this.retFlight != null && this.retFlight.Count > 0)
            {
                request.returnFlights = new List<AirSell.AirFlight>();

                for (int i = 0; i < this.retFlight.Count; i++)
                {
                    var flight = new AirSell.AirFlight();

                    flight.departureDateTime = this.retFlight[i].departureDateTime;
                    flight.arrivalDateTime = this.retFlight[i].arrivalDateTime;
                    flight.departureCity = this.retFlight[i].depCity.code;
                    flight.arrivalCity = this.retFlight[i].arrCity.code;
                    flight.companyCode = this.retFlight[i].airline.code;
                    flight.flightNumber = this.retFlight[i].flightNumber;
                    flight.RBDCode = this.retFlight[i].rbd;

                    if (i > 0 && request.returnFlights[i - 1].companyCode == flight.companyCode && request.returnFlights[i - 1].flightNumber == flight.flightNumber)
                    {
                        request.returnFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                        request.returnFlights[i - 1].arrivalCity = flight.arrivalCity;
                    }
                    else
                    {
                        request.returnFlights.Add(flight);
                    }
                }
            }

            request.session = new AirSell.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";

            request.useBookingOfficeID = false;
            request.tripType = (this.retFlight != null && this.retFlight.Count > 0) ? "R" : "D";
            request.ClientIP = Utilities.HttpUtility.GetIPAddress();
            return request;
        }
    }

    public class FlightDetailSelected
    {
        public int Seq { get; set; }
        public Airport depCity { get; set; }
        public Airport arrCity { get; set; }
        public Airline airline { get; set; }
        public string flightNumber { get; set; }
        public string rbd { get; set; }
        public Airline operatedAirline { get; set; }
        public DateTime departureDateTime { get; set; }
        public DateTime arrivalDateTime { get; set; }
    }

}