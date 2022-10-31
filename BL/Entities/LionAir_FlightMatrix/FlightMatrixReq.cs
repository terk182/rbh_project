using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.LionAir_FlightMatrix
{
    public class FlightMatrixReq
    {
        public string BinarySecurityToken { get; set; }        
        public string TripType { get; set; }//OneWay,Return
        public int noAdult { get; set; }
        public int noChild { get; set; }
        public int noInfant { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string DepartureCode { get; set; }
        public string DestinationCode { get; set; }
        public string GetFlightMatrixRequest()
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
            sMessage += "<BinarySecurityToken>"+ BinarySecurityToken +"</BinarySecurityToken>";
            sMessage += "</Security>";
            sMessage += "</soap:Header>";
            sMessage += "<soap:Body>";
            sMessage += "<FlightMatrixRequest xmlns=\"http://www.vedaleon.com/webservices\">";
            sMessage += String.Format("<flightMatrixRQ Target=\"{0}\" Version=\"{1}\">", ConfigurationManager.AppSettings["LionAir.Target"].ToString(), ConfigurationManager.AppSettings["LionAir.Version"].ToString());
            sMessage += "<AirItinerary DirectionInd=\""+ TripType + "\">";
            //Route Detail
            sMessage += "<OriginDestinationOptions>";
            sMessage += "<OriginDestinationOption>";
            sMessage += String.Format("<FlightSegment DepartureDateTime=\"{0}\" {1} RPH=\"1\">", DepartureDateTime.ToString("yyyy'-'MM'-'dd'T00:00:00'"),("ArrivalDateTime=\""+ ArrivalDateTime.ToString("yyyy'-'MM'-'dd'T00:00:00'") + "\""));
            sMessage += "<DepartureAirport LocationCode=\""+ DepartureCode+"\"/>";
            sMessage += "<ArrivalAirport LocationCode=\"" + DestinationCode + "\"/>";
            sMessage += "<MarketingAirline Code=\"SL\"/>";
            sMessage += "</FlightSegment>";
            sMessage += "</OriginDestinationOption>";
            sMessage += "</OriginDestinationOptions>";
            //Route Detail
            sMessage += "</AirItinerary>";
            sMessage += "<TravelerInfoSummary>";
            //Traveller Detail
            sMessage += "<AirTravelerAvail>";
            sMessage += "<AirTraveler>";
            sMessage += String.Format("<PassengerTypeQuantity Code=\"ADT\" Quantity=\"{0}\" />",noAdult);
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
            sMessage += "</flightMatrixRQ>";
            sMessage += "</FlightMatrixRequest>";
            sMessage += "</soap:Body>";
            sMessage += "</soap:Envelope>";
            return sMessage;
        }
    }
}
