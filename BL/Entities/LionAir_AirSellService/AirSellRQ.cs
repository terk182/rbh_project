using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.LionAir_AirSellService
{
    public class AirSellRQ
    {
        public string BinarySecurityToken { get; set; }
        public string TripType { get; set; }//OneWay,Return
        public int noAdult { get; set; }
        public int noChild { get; set; }
        public int noInfant { get; set; }
        public List<Flight> Flights { get; set; }


        public string GetAirSellRequest()
        {
            DateTime dtNow = DateTime.Now;
            string sMessage = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
            sMessage += "<soap:Header>";
            sMessage += "<MessageHeader xmlns=\"http://www.ebxml.org/namespaces/messageHeader\">";
            sMessage += "<CPAId>SL</CPAId>";
            sMessage += "<ConversationId>" + Guid.NewGuid() + "</ConversationId>";
            sMessage += "<Service>GetFlightMatrix</Service>";
            sMessage += "<Action>FlightMatrixRQ</Action>";
            sMessage += "<MessageData>";
            sMessage += "<MessageId>mid:" + dtNow.ToString("HH:mm:ss.ffff") + "</MessageId>";
            sMessage += "<Timestamp>" + dtNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "</Timestamp>";
            sMessage += "</MessageData>";
            sMessage += "</MessageHeader>";
            sMessage += "<Security xmlns=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">";
            sMessage += "<BinarySecurityToken>" + BinarySecurityToken + "</BinarySecurityToken>";
            sMessage += "</Security>";
            sMessage += "</soap:Header>";
            sMessage += "<soap:Body>";
            sMessage += "<AirSellRQ xmlns=\"http://www.vedaleon.com/webservices\">";
            sMessage += String.Format("<OTA_AirPriceRQ Target=\"{0}\" Version=\"{1}\" ReturnAncillariesInResponse=\"true\">", ConfigurationManager.AppSettings["LionAir.Target"].ToString(), ConfigurationManager.AppSettings["LionAir.Version"].ToString());

            sMessage += "<POS>";
            sMessage += "<Source ISOCountry=\"TH\" />";
            sMessage += "</POS>";

            sMessage += "<AirItinerary DirectionInd=\"" + TripType + "\">";
            //Route Detail
            sMessage += "<OriginDestinationOptions>";
            int noOfSeat = noAdult + noChild;
            int iRoute = 1;
            if (TripType.Equals("Return"))
            {
                iRoute = 2;
            }
            for (int i = 0; i < iRoute; i++)
            {
                var _Route = Flights.Find(x => x.Type == (i==0?"D":"R"));

                sMessage += "<OriginDestinationOption>";
                foreach (var _flight in _Route.FlightSegments)
                {
                    sMessage += String.Format("<FlightSegment DepartureDateTime=\"{0}\" ArrivalDateTime=\"{1}\"  FlightNumber=\"{2}\" RPH=\"{3}\" ResBookDesigCode=\"{4}\" NumberInParty=\"{5}\" Status=\"NN\">", _flight.DepartureDateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':00'"), _flight.ArrivalDateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':00'"), _flight.FlightNumber, _flight.RPH, _flight.ResBookDesigCode, noOfSeat);
                    sMessage += "<DepartureAirport LocationCode=\"" + _flight.DepartureCode + "\"/>";
                    sMessage += "<ArrivalAirport LocationCode=\"" + _flight.DestinationCode + "\" />";
                    sMessage += "<OperatingAirline Code=\"" + _flight.OperatingAirline + "\" CodeContext=\"IATA\"/>";
                    sMessage += "<MarketingAirline Code=\"" + _flight.MarketingAirline + "\" CodeContext=\"IATA\"/>";
                    sMessage += "</FlightSegment>";
                }
                sMessage += "</OriginDestinationOption>";
            }
            sMessage += "</OriginDestinationOptions>";
            //Route Detail
            sMessage += "</AirItinerary>";
            sMessage += "<TravelerInfoSummary>";
            //Traveller Detail
            sMessage += "<AirTravelerAvail>";
            sMessage += "<AirTraveler>";
            sMessage += String.Format("<PassengerTypeQuantity Code=\"ADT\" Quantity=\"{0}\" />", noAdult);
            sMessage += "</AirTraveler>";
            sMessage += "</AirTravelerAvail>";
            if (noChild > 0)
            {
                sMessage += "<AirTravelerAvail>";
                sMessage += "<AirTraveler>";
                sMessage += String.Format("<PassengerTypeQuantity Code=\"CNN\" Quantity=\"{0}\" />", noChild);
                sMessage += "</AirTraveler>";
                sMessage += "</AirTravelerAvail>";
            }
            if (noInfant > 0)
            {
                sMessage += "<AirTravelerAvail>";
                sMessage += "<AirTraveler>";
                sMessage += String.Format("<PassengerTypeQuantity Code=\"INF\" Quantity=\"{0}\" />", noInfant);
                sMessage += "</AirTraveler>";
                sMessage += "</AirTravelerAvail>";
            }
            //Traveller Detail
            sMessage += "</TravelerInfoSummary>";
            sMessage += "</OTA_AirPriceRQ>";
            sMessage += "</AirSellRQ>";
            sMessage += "</soap:Body>";
            sMessage += "</soap:Envelope>";

            return sMessage;
        }
    }
    public class Flight
    {
        public string Type { get; set; }
        public List<FlightSegment> FlightSegments { get; set; }
    }

    public class FlightSegment
    {
        public int Seq { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string FlightNumber { get; set; }
        public string RPH { get; set; }
        public string ResBookDesigCode { get; set; }//RBD
        public string DepartureCode { get; set; }
        public string DestinationCode { get; set; }
        public string OperatingAirline { get; set; }
        public string MarketingAirline { get; set; }
    }
}
