using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BL.Entities.RobinhoodFlight
{
    public class Flight
    {
        public List<FlightDetail> flightDetails { get; set; }
        public string totalTime { get; set; }
        public bool bShow { get; set; }
        public string refNumber { get; set; }
        public string flightFrom { get; set; }//A:Amadeus,L:LionAir
        public string pgSearchOID { get; set; }
        public string bookingCode
        {
            get
            {
                string bCode = "";
                foreach (var detail in flightDetails)
                {
                    bCode += bCode == "" ? "" : "|";
                    bCode += String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                        detail.depCity.code,
                        detail.arrCity.code,
                        detail.departureDateTime.ToString("yyyyMMddHHmm"),
                        detail.arrivalDateTime.ToString("yyyyMMddHHmm"),
                        detail.rbd,
                        detail.fareBasis.PadLeft(12, '.'),
                        detail.fareType.PadLeft(2, 'R').Substring(0, 2),
                        detail.airline.code,
                        detail.operatedAirline == null ? detail.airline.code : detail.operatedAirline.code,
                        detail.equipmentType.PadLeft(3, '0').Substring(0, 3),
                        detail.flightNumber);
                }
                bCode += "|" + totalTime;
                if(refNumber!=null && refNumber.Length > 0)
                {
                    bCode += "|" + refNumber;
                }
                if (flightFrom != null && flightFrom.Length > 0)
                {
                    bCode += "|" + flightFrom;
                }
                if (pgSearchOID != null)
                {
                    bCode += "|" + pgSearchOID;
                }
                return bCode;
            }
        }
        public BaggageDetails baggageDetails { get; set; }

    }

    public class InFlightServices
    {
        public bool wifiInternet { get; set; }
        public bool inflightEntertain { get; set; }
        public bool inSeatPower { get; set; }
        public bool dutyFreeSale { get; set; }
    }
    public class FlightDetail
    {
        public Airport depCity { get; set; }
        public Airport arrCity { get; set; }
        public Airline airline { get; set; }
        public string flightNumber { get; set; }
        public Airline operatedAirline { get; set; }
        public DateTime departureDateTime { get; set; }
        public DateTime arrivalDateTime { get; set; }
        public string flightTime { get; set; }
        public string connectingTime { get; set; }
        public string rbd { get; set; }
        public string fareBasis { get; set; }
        public int availableSeat { get; set; }
        public string fareType { get; set; }
        public string cabin { get; set; }
        public string equipmentType { get; set; }
        public string equipmentTypeName{ get; set; }
        public InFlightServices inFlightServices { get; set; }
        public DisplayDateTime depDisplayDateTime{ get; set; }
        public DisplayDateTime arrDisplayDateTime { get; set; }
        public int Seq { get; set; }
        public string controlNumber { get; set; }


        public void setDisplayDateTime(string langaugeCode, DateTime depDate)
        {
            CultureInfo ci = new CultureInfo(langaugeCode.ToLower() == "th" ? "th-TH" : "en-US");
            depDisplayDateTime = new DisplayDateTime();
            depDisplayDateTime.date = departureDateTime.Day;
            depDisplayDateTime.month = departureDateTime.Month;
            depDisplayDateTime.year = departureDateTime.Year;
            depDisplayDateTime.time = ((departureDateTime.Hour) * 100) + departureDateTime.Minute;
            depDisplayDateTime.shortDate = departureDateTime.ToString("ddd dd MMM", ci);
            depDisplayDateTime.longDate = departureDateTime.ToString("dddd dd MMMM", ci);
            depDisplayDateTime.shortDateWithoutDay = departureDateTime.ToString("dd MMM", ci);
            depDisplayDateTime.longDateWithoutDay = departureDateTime.ToString("dd MMMM", ci);
            depDisplayDateTime.displayTime = departureDateTime.ToString("HH:mm");
            TimeSpan ts = departureDateTime.Date - depDate.Date;
            if (ts.Days > 0)
            {
                depDisplayDateTime.displayTime += "(+" + ts.Days.ToString() + ")";
            }

            arrDisplayDateTime = new DisplayDateTime();
            arrDisplayDateTime.date = arrivalDateTime.Day;
            arrDisplayDateTime.month = arrivalDateTime.Month;
            arrDisplayDateTime.year = arrivalDateTime.Year;
            arrDisplayDateTime.time = ((arrivalDateTime.Hour) * 100) + arrivalDateTime.Minute;
            arrDisplayDateTime.shortDate = arrivalDateTime.ToString("ddd dd MMM", ci);
            arrDisplayDateTime.longDate = arrivalDateTime.ToString("dddd dd MMMM", ci);
            arrDisplayDateTime.shortDateWithoutDay = arrivalDateTime.ToString("dd MMM", ci);
            arrDisplayDateTime.longDateWithoutDay = arrivalDateTime.ToString("dd MMMM", ci);
            arrDisplayDateTime.displayTime = arrivalDateTime.ToString("HH:mm");
            ts = arrivalDateTime.Date - depDate.Date;
            if (ts.Days > 0)
            {
                arrDisplayDateTime.displayTime += "(+" + ts.Days.ToString() + ")";
            }
        }
    }
    public class BaggageDetails
    {
        public int freeAllowance { get; set; }
        public string quantityCode { get; set; }
        public string unitQualifier { get; set; }
    }
}
